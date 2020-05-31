#if DEBUG
namespace RAGENativeUI.Internals
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using Rage;
    using Rage.Attributes;
    using RAGENativeUI.Elements;
    using RAGENativeUI.IL;

    internal static unsafe class DebugCommands
    {
        [ConsoleCommand("Creates three timer bars in a new pool and draws them for 1 minute")]
        private static void CreateTimerBars(string name)
        {
            TimerBarPool pool = new TimerBarPool();

            pool.Add(new TextTimerBar(name + " #1", "TEXT"));
            pool.Add(new BarTimerBar(name + " #2") { Percentage = 0.5f });
            pool.Add(new TextTimerBar(name + " #3", "") { Accent = HudColor.RedDark.GetColor() });

            GameFiber.StartNew(() =>
            {
                uint startTime = Game.GameTime;
                while ((startTime + 60_000) > Game.GameTime)
                {
                    GameFiber.Yield();
                    pool.Draw();
                }
            });
        }

        [ConsoleCommand]
        private static void DumpScript(string name)
        {
            if (!scrProgramRegistry.Available)
            {
                Game.LogTrivial($"{nameof(scrProgramRegistry)} is not available");
                return;
            }

            ref scrProgramRegistry reg = ref scrProgramRegistry.Instance;

            scrProgram* prog = reg.Find(name);

            Game.LogTrivial($"{name} => {new IntPtr(prog).ToString("X16")}");
        
            if (prog != null)
            {
                string dump = DumpScript(ref Unsafe.AsRef<scrProgram>(prog));

                Directory.CreateDirectory("script_dumps/");
                File.WriteAllText(Path.Combine("script_dumps/", Path.ChangeExtension(name, "txt")), dump);
            }
        }

        private static string DumpScript(ref scrProgram prog)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Address: {0}\n", new IntPtr(Unsafe.AsPointer(ref prog)).ToString("X16"));
            sb.AppendFormat("{0}: {1}\n", nameof(scrProgram.Name), Marshal.PtrToStringAnsi(prog.Name));
            sb.AppendFormat("{0}: 0x{1}\n", nameof(scrProgram.NameHash), prog.NameHash.ToString("X8"));
            sb.AppendFormat("{0}: {1}\n", nameof(scrProgram.NumRefs), prog.NumRefs);
            sb.AppendFormat("{0}: 0x{1}\n", nameof(scrProgram.Hash), prog.Hash.ToString("X8"));
            sb.AppendFormat("{0}: {1}\n", nameof(scrProgram.CodePages), new IntPtr(prog.CodePages).ToString("X16"));
            sb.AppendFormat("{0}: {1}\n", nameof(scrProgram.CodeLength), prog.CodeLength);
            sb.AppendFormat("{0}: {1}\n", nameof(scrProgram.ArgsCount), prog.ArgsCount);
            sb.AppendFormat("{0}: {1}\n", nameof(scrProgram.StaticsCount), prog.StaticsCount);
            sb.AppendFormat("{0}: {1}\n", nameof(scrProgram.StaticsInitialValues), new IntPtr(prog.StaticsInitialValues).ToString("X16"));
            for (int i = 0; i < prog.StaticsCount; i++)
            {
                ref scrValue v = ref prog.StaticsInitialValues[i];
                sb.AppendFormat("\t[{0}] = {1} ({2}) ({3})\n", i, v.Val.ToString("X16"), v.AsInt, v.AsFloat);
            }
            sb.AppendFormat("{0}: {1} (block: {2}, size: {3}, pages: {4})\n", nameof(scrProgram.GlobalsCount), prog.GlobalsCount, prog.GlobalsCount >> 18, prog.GlobalsCount & 0x3FFFF, ((prog.GlobalsCount & 0x3FFFF) + 0x3FFF) >> 14);
            sb.AppendFormat("{0}: {1}\n", nameof(scrProgram.GlobalsInitialValues), new IntPtr(prog.GlobalsInitialValues).ToString("X16"));
            uint globalsPagesCount = ((prog.GlobalsCount & 0x3FFFF) + 0x3FFF) >> 14;
            for (uint i = 0; i < globalsPagesCount; i++)
            {
                uint numGlobals = (i == globalsPagesCount - 1 ?
                                    (prog.GlobalsCount & 0x3FFFF) - (i << 14) :
                                    0x4000) / 8;

                sb.AppendFormat("\t[{0}] =\n", i);
                for (uint j = 0; j < numGlobals; j++)
                {
                    ref scrValue v = ref prog.GlobalsInitialValues[i][j];
                    sb.AppendFormat("\t\t[{0}] = {1} ({2}) ({3})\n", j, v.Val.ToString("X16"), v.AsInt, v.AsFloat);
                }
            }
            sb.AppendFormat("{0}: {1}\n", nameof(scrProgram.NativesCount), prog.NativesCount);
            sb.AppendFormat("{0}: {1}\n", nameof(scrProgram.Natives), new IntPtr(prog.Natives).ToString("X16"));
            for (int i = 0; i < prog.NativesCount; i++)
            {
                sb.AppendFormat("\t[{0}] = {1}\n", i, new IntPtr(prog.Natives[i]).ToString("X16"));
            }
            sb.AppendFormat("{0}: {1}\n", nameof(scrProgram.StringsCount), prog.StringsCount);
            sb.AppendFormat("{0}: {1}\n", nameof(scrProgram.Strings), new IntPtr(prog.Strings).ToString("X16"));
            uint stringsPagesCount = (prog.StringsCount + 0x3FFF) >> 14;
            for (uint i = 0; i < stringsPagesCount; i++)
            {
                uint size = i == stringsPagesCount - 1 ?
                                    prog.StringsCount - (i << 14) :
                                    0x4000;

                sb.AppendFormat("\t[{0}] =\n\t\t\"", i);
                for (uint j = 0; j < size; j++)
                {
                    if (prog.Strings[i][j] == 0)
                    {
                        sb.Append("\"\n\t\t\"");
                    }
                    else
                    {
                        sb.Append((char)prog.Strings[i][j]);
                    }
                }
                sb.AppendLine();
            }

            sb.AppendLine("Disassembly:");
            DisassembleScript(sb, ref prog);

            return sb.ToString();
        }

        private static void DisassembleScript(StringBuilder sb, ref scrProgram prog)
        {
            StringBuilder lineSB = new StringBuilder();

            for (uint ip = 0; ip < prog.CodeLength;)
            {
                byte* inst = prog.IP(ip);
                uint size = GetInstructionSize(ref prog, ip);

                lineSB.Clear();

                lineSB.Append(ip.ToString("000000"));
                lineSB.Append(" : ");
                for (uint offset = 0; offset < size; offset++)
                {
                    lineSB.Append(inst[offset].ToString("X2"));
                    if (offset < size - 1)
                    {
                        lineSB.Append(' ');
                    }
                }

                lineSB.Append(' ', Math.Max(32 - lineSB.Length, 4));

                DisassembleInstruction(lineSB, ref prog, ip);

                lineSB.AppendLine();

                sb.Append(lineSB.ToString());

                ip += size;
            }
        }

        private static void DisassembleInstruction(StringBuilder sb, ref scrProgram prog, uint ip)
        {
            byte* inst = prog.IP(ip);
            if (*inst >= InstructionNames.Length)
            {
                return;
            }

            sb.Append(InstructionNames[*inst]);

            ip++;
            foreach (char f in InstructionFormats[*inst])
            {
                switch (f)
                {
                    case '$':
                        {
                            byte emptyStr = 0;
                            byte* str = *prog.IP(ip) == 0 ? &emptyStr : prog.IP(ip + 1);
                            str = *str == 255 ? (str + 1) : str;

                            sb.Append("[ ");

                            const int MaxLength = 42;
                            int n = MaxLength;
                            while (*str != 0 && n != 0)
                            {
                                byte c = *str;

                                if ((char)c == '\r')
                                {
                                    sb.Append("\\r");
                                }
                                else if ((char)c == '\n')
                                {
                                    sb.Append("\\n");
                                }
                                else
                                {
                                    sb.Append((char)c);
                                }

                                str++;
                                n--;
                            }

                            if (*str != 0)
                            {
                                sb.Append("...");
                            }

                            sb.Append(']');
                        }
                        break;
                    case 'R':
                        {
                            short v = *(short*)prog.IP(ip);
                            ip += 2;

                            sb.Append(' ');
                            sb.Append((ip + v).ToString("000000"));
                            sb.Append(' ');
                            sb.Append('(');
                            sb.Append(v.ToString("+#;-#"));
                            sb.Append(')');
                        }
                        break;
                    case 'S':
                        {
                            const byte MaxCases = 32;
                            byte count = *prog.IP(ip);

                            sb.Append(' ');
                            sb.Append('[');
                            sb.Append(count);
                            sb.Append(']');

                            if (count > 0)
                            {
                                uint c = Math.Min(MaxCases, count);

                                uint currIP = ip + 7;
                                for (uint i = 0; i < c; i++, currIP += 6)
                                {
                                    uint caseValue = *(uint*)prog.IP(currIP - 6);
                                    short offset = *(short*)prog.IP(currIP - 2);

                                    sb.Append(' ');
                                    sb.Append(caseValue);
                                    sb.Append(':');
                                    sb.Append((currIP + offset).ToString("000000"));
                                }

                                ip += 1 + 6u * count;
                            }
                            else
                            {
                                ip++;
                            }

                            if (count > MaxCases)
                            {
                                sb.Append("... ");
                            }
                        }
                        break;
                    case 'a':
                        {
                            int v = *(int*)prog.IP(ip) & 0xFFFFFF;
                            ip += 3;

                            sb.Append(' ');
                            sb.Append(v.ToString("000000"));
                        }
                        break;
                    case 'b':
                        {
                            byte v = *prog.IP(ip);
                            ip += 1;

                            sb.Append(' ');
                            sb.Append(v);
                        }
                        break;
                    case 'd':
                        {
                            int v = *(int*)prog.IP(ip);
                            ip += 4;

                            sb.Append(' ');
                            sb.Append(v);
                            sb.Append("(0x");
                            sb.Append(unchecked((uint)v).ToString("X"));
                            sb.Append(')');
                        }
                        break;
                    case 'f':
                        {
                            float v = *(float*)prog.IP(ip);
                            ip += 4;

                            sb.Append(' ');
                            sb.Append(v);
                        }
                        break;
                    case 'h':
                    case 's':
                        {
                            short v = *(short*)prog.IP(ip);
                            ip += 2;

                            sb.Append(' ');
                            sb.Append(v);
                        }
                        break;
                }
            }
        }

        private static uint GetInstructionSize(ref scrProgram prog, uint ip)
        {
            byte inst = *prog.IP(ip);
            uint s = InstructionSizes[inst];
            if (s == 0)
            {
                s = inst switch
                {
                    0x2D => (uint)*prog.IP(ip + 4) + 5, // ENTER
                    0x62 => 6 * (uint)*prog.IP(ip + 1) + 2, // SWITCH
                    _ => throw new InvalidOperationException($"Unknown instruction 0x{inst:X}"),
                };
            }

            return s;
        }

        private static readonly byte[] InstructionSizes = new byte[256]
        {
            1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
            1,1,1,1,1,2,3,4,5,5,1,1,4,0,3,1,1,1,1,1,2,2,2,2,2,2,2,2,2,2,2,1,
            2,2,2,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,4,4,4,
            4,4,0,1,1,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        };

        private static readonly string[] InstructionNames = new string[127]
        {
            "NOP",
            "IADD",
            "ISUB",
            "IMUL",
            "IDIV",
            "IMOD",
            "INOT",
            "INEG",
            "IEQ",
            "INE",
            "IGT",
            "IGE",
            "ILT",
            "ILE",
            "FADD",
            "FSUB",
            "FMUL",
            "FDIV",
            "FMOD",
            "FNEG",
            "FEQ",
            "FNE",
            "FGT",
            "FGE",
            "FLT",
            "FLE",
            "VADD",
            "VSUB",
            "VMUL",
            "VDIV",
            "VNEG",
            "IAND",
            "IOR",
            "IXOR",
            "I2F",
            "F2I",
            "F2V",
            "PUSH_CONST_U8",
            "PUSH_CONST_U8_U8",
            "PUSH_CONST_U8_U8_U8",
            "PUSH_CONST_U32",
            "PUSH_CONST_F",
            "DUP",
            "DROP",
            "NATIVE",
            "ENTER",
            "LEAVE",
            "LOAD",
            "STORE",
            "STORE_REV",
            "LOAD_N",
            "STORE_N",
            "ARRAY_U8",
            "ARRAY_U8_LOAD",
            "ARRAY_U8_STORE",
            "LOCAL_U8",
            "LOCAL_U8_LOAD",
            "LOCAL_U8_STORE",
            "STATIC_U8",
            "STATIC_U8_LOAD",
            "STATIC_U8_STORE",
            "IADD_U8",
            "IMUL_U8",
            "IOFFSET",
            "IOFFSET_U8",
            "IOFFSET_U8_LOAD",
            "IOFFSET_U8_STORE",
            "PUSH_CONST_S16",
            "IADD_S16",
            "IMUL_S16",
            "IOFFSET_S16",
            "IOFFSET_S16_LOAD",
            "IOFFSET_S16_STORE",
            "ARRAY_U16",
            "ARRAY_U16_LOAD",
            "ARRAY_U16_STORE",
            "LOCAL_U16",
            "LOCAL_U16_LOAD",
            "LOCAL_U16_STORE",
            "STATIC_U16",
            "STATIC_U16_LOAD",
            "STATIC_U16_STORE",
            "GLOBAL_U16",
            "GLOBAL_U16_LOAD",
            "GLOBAL_U16_STORE",
            "J",
            "JZ",
            "IEQ_JZ",
            "INE_JZ",
            "IGT_JZ",
            "IGE_JZ",
            "ILT_JZ",
            "ILE_JZ",
            "CALL",
            "GLOBAL_U24",
            "GLOBAL_U24_LOAD",
            "GLOBAL_U24_STORE",
            "PUSH_CONST_U24",
            "SWITCH",
            "STRING",
            "STRINGHASH",
            "TEXT_LABEL_ASSIGN_STRING",
            "TEXT_LABEL_ASSIGN_INT",
            "TEXT_LABEL_APPEND_STRING",
            "TEXT_LABEL_APPEND_INT",
            "TEXT_LABEL_COPY",
            "CATCH",
            "THROW",
            "CALLINDIRECT",
            "PUSH_CONST_M1",
            "PUSH_CONST_0",
            "PUSH_CONST_1",
            "PUSH_CONST_2",
            "PUSH_CONST_3",
            "PUSH_CONST_4",
            "PUSH_CONST_5",
            "PUSH_CONST_6",
            "PUSH_CONST_7",
            "PUSH_CONST_FM1",
            "PUSH_CONST_F0",
            "PUSH_CONST_F1",
            "PUSH_CONST_F2",
            "PUSH_CONST_F3",
            "PUSH_CONST_F4",
            "PUSH_CONST_F5",
            "PUSH_CONST_F6",
            "PUSH_CONST_F7",
        };

        private static readonly string[] InstructionFormats = new string[127]
        {
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "b",
            "bb",
            "bbb",
            "d",
            "f",
            "",
            "",
            "bbb",
            "bs$",
            "bb",
            "",
            "",
            "",
            "",
            "",
            "b",
            "b",
            "b",
            "b",
            "b",
            "b",
            "b",
            "b",
            "b",
            "b",
            "b",
            "",
            "b",
            "b",
            "b",
            "s",
            "s",
            "s",
            "s",
            "s",
            "s",
            "h",
            "h",
            "h",
            "h",
            "h",
            "h",
            "h",
            "h",
            "h",
            "h",
            "h",
            "h",
            "R",
            "R",
            "R",
            "R",
            "R",
            "R",
            "R",
            "R",
            "a",
            "a",
            "a",
            "a",
            "a",
            "S",
            "",
            "",
            "b",
            "b",
            "b",
            "b",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
        };
    }
}
#endif
