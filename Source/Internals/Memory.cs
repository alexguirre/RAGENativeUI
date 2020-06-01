namespace RAGENativeUI.Internals
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;

    using Rage;

    using static RAGENativeUI.IL.Unsafe;
    using static RAGENativeUI.IL.Invoker;

    internal static unsafe class Memory
    {
        public static readonly IntPtr Screen_GetActualWidth, Screen_GetActualHeight;
        public static readonly IntPtr CTextStyle_ScriptStyle;
        public static readonly IntPtr CScaleformMgr_IsMovieRendering;
        public static readonly IntPtr CScaleformMgr_GetRawMovieView;
        public static readonly IntPtr CBusySpinner_InstructionalButtons;
        public static readonly IntPtr scrProgramRegistry_sm_Instance;
        public static readonly IntPtr scrProgramRegistry_Find;
        public static readonly int TimershudSharedGlobalId = -1;
        public static readonly int TimershudSharedTimerbarsTotalHeightOffset = -1;
        public static readonly int TimerbarsPrevTotalHeightGlobalId = -1;

        static Memory()
        {
            const string GetActualResolutionPattern = "48 83 EC 38 0F 29 74 24 ?? 66 0F 6E 35 ?? ?? ?? ?? 48 8D 0D ?? ?? ?? ??";

            Screen_GetActualHeight = FindAddress(0, () => Game.FindPattern(GetActualResolutionPattern));
            if (Screen_GetActualHeight != IntPtr.Zero)
            {
                Screen_GetActualWidth = FindAddress(1, () => Game.FindPattern(GetActualResolutionPattern, Screen_GetActualHeight + 1));
            }

            CTextStyle_ScriptStyle = FindAddress(2, () =>
            {
                IntPtr addr = Game.FindPattern("48 8D 0D ?? ?? ?? ?? 44 8A C3 E8 ?? ?? ?? ?? 83 25");
                if (addr != IntPtr.Zero)
                {
                    addr += *(int*)(addr + 3) + 7;
                }
                return addr;
            });

            CScaleformMgr_IsMovieRendering = FindAddress(3, () =>
            {
                IntPtr addr = Game.FindPattern("65 48 8B 04 25 ?? ?? ?? ?? 8B F9 48 8B 04 D0 B9 ?? ?? ?? ?? 8B 14 01");
                if (addr != IntPtr.Zero)
                {
                    addr -= 0x10;
                }
                return addr;
            });

            CScaleformMgr_GetRawMovieView = FindAddress(4, () =>
            {
                IntPtr addr = Game.FindPattern("0F B7 05 ?? ?? ?? ?? 3B D8 7D 78");
                if (addr != IntPtr.Zero)
                {
                    addr -= 0x10;
                }
                return addr;
            });

            CBusySpinner_InstructionalButtons = FindAddress(5, () =>
            {
                IntPtr addr = Game.FindPattern("0F B7 05 ?? ?? ?? ?? 33 DB 8B F8 85 C0");
                if (addr != IntPtr.Zero)
                {
                    addr += *(int*)(addr + 3) + 7 - 8;
                }
                return addr;
            });

            IntPtr scrProgramRegistryAddr = FindAddress(6, () => Game.FindPattern("48 8D 0D ?? ?? ?? ?? 8B D0 44 8B F0 89 85"));
            if (scrProgramRegistryAddr != IntPtr.Zero)
            {
                scrProgramRegistry_sm_Instance = scrProgramRegistryAddr + * (int*)(scrProgramRegistryAddr + 3) + 7;
                scrProgramRegistry_Find = scrProgramRegistryAddr + *(int*)(scrProgramRegistryAddr + 19) + 23;
            }

            if (Shared.MemoryInts[0] == 0 || Shared.MemoryInts[1] == 0 || Shared.MemoryInts[2] == 0)
            {
                IntPtr ingamehudAddr = FindPatternInScript("ingamehud",
                                                        new byte[] { 0x6F, 0x39, 0x08, 0x5F, 0x00, 0x00, 0x00, 0x5E, 0x00, 0x00, 0x00, 0x47 },
                                                        new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0xFF });
                if (ingamehudAddr != IntPtr.Zero)
                {
                    Shared.MemoryInts[0] = TimerbarsPrevTotalHeightGlobalId = *(int*)(ingamehudAddr + 4) & 0xFFFFFF;
                    Shared.MemoryInts[1] = TimershudSharedGlobalId = *(int*)(ingamehudAddr + 8) & 0xFFFFFF;
                    Shared.MemoryInts[2] = TimershudSharedTimerbarsTotalHeightOffset = *(short*)(ingamehudAddr + 12);
                }
            }
            else
            {
                TimerbarsPrevTotalHeightGlobalId = Shared.MemoryInts[0];
                TimershudSharedGlobalId = Shared.MemoryInts[1];
                TimershudSharedTimerbarsTotalHeightOffset = Shared.MemoryInts[2];
            }
        }

        private static IntPtr FindAddress(int key, Func<IntPtr> find)
        {
            if(Shared.MemoryAddresses[key] == 0)
            {
                IntPtr addr = find();
                Shared.MemoryAddresses[key] = addr.ToInt64();
                return addr;
            }
            else
            {
                return (IntPtr)Shared.MemoryAddresses[key];
            }
        }

        private static IntPtr FindPatternInScript(string name, byte[] pattern, byte[] mask)
        {
            if (!scrProgramRegistry.Available)
            {
                return IntPtr.Zero;
            }

            scrProgram* prog = scrProgramRegistry.Instance.Find(name);

            if (prog == null)
            {
                return IntPtr.Zero;
            }

            const uint CodePageMaxSize = 0x4000;

            int numCodePages = (int)(prog->CodeLength + (CodePageMaxSize - 1)) >> 14;
            for (int i = 0; i < numCodePages; i++)
            {
                uint size = i == (numCodePages - 1) ? (prog->CodeLength & 0x3FFF) : CodePageMaxSize;

                IntPtr match = FindPatternInRange(prog->CodePages[i], size, pattern, mask);
                if (match != IntPtr.Zero)
                {
                    return match;
                }
            }

            return IntPtr.Zero;
        }

        private static IntPtr FindPatternInRange(byte* address, uint size, byte[] pattern, byte[] mask)
        {
            byte* endAddress = address + (size - pattern.Length);
            for (; address <= endAddress; address += 1)
            {
                bool found = true;
                for (int i = 0; i < pattern.Length; i++)
                {
                    if ((address[i] & mask[i]) != pattern[i])
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                {
                    return (IntPtr)address;
                }
            }

            return IntPtr.Zero;
        }
    }

    internal static unsafe class ScriptGlobals
    {
        public static readonly bool TimersBarsTotalHeightAvailable = Memory.TimershudSharedGlobalId != -1 && Memory.TimershudSharedTimerbarsTotalHeightOffset != -1 && Memory.TimerbarsPrevTotalHeightGlobalId != -1;

        public static ref float TimerBarsTotalHeight => ref AsRef<float>(Game.GetScriptGlobalVariableAddress(Memory.TimershudSharedGlobalId) + Memory.TimershudSharedTimerbarsTotalHeightOffset * 8);
        public static ref float TimerBarsPrevTotalHeight => ref AsRef<float>(Game.GetScriptGlobalVariableAddress(Memory.TimerbarsPrevTotalHeightGlobalId));
    }

    internal static class Screen
    {
        private static readonly bool Available = Memory.Screen_GetActualWidth != IntPtr.Zero && Memory.Screen_GetActualHeight != IntPtr.Zero;

        /// <summary>
        /// Gets the resolution of the main screen, the one containing the UI in case multiple screens are used.
        /// </summary>
        public static SizeF ActualResolution
        {
            get
            {
                if (Available)
                {
                    using var tls = UsingTls.Scope();

                    return new SizeF(InvokeRetFloat(Memory.Screen_GetActualWidth), InvokeRetFloat(Memory.Screen_GetActualHeight));
                }
                else
                {
                    return Game.Resolution; // not the same since this the resolution of all screens combined, but good enough as a fallback
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x28)]
    internal struct CTextStyle
    {
        [FieldOffset(0x00)] public int Color;

        [FieldOffset(0x08)] public float Scale;
        [FieldOffset(0x0C)] public (float Start, float End) Wrap;
        [FieldOffset(0x14)] public int Font;

        [FieldOffset(0x1C)] public byte Justification;

        [FieldOffset(0x1E), MarshalAs(UnmanagedType.I1)] public bool DropShadow;
        [FieldOffset(0x1F), MarshalAs(UnmanagedType.I1)] public bool Outline;


        public static ref CTextStyle ScriptStyle => ref AsRef<CTextStyle>(Memory.CTextStyle_ScriptStyle);

        public static readonly bool ScriptStyleAvailable = Memory.CTextStyle_ScriptStyle != IntPtr.Zero;
    }

    internal static class CScaleformMgr
    {
        public static unsafe ref GFxMovieView GetRawMovieView(int index) => ref AsRef<GFxMovieView>(InvokeRetPointer(Memory.CScaleformMgr_GetRawMovieView, index));

        public static bool IsMovieRendering(int index)
        {
            // CScaleformMgr::IsMovieRendering checks if the current thread is the render thread,
            // so temporarily set this thread type to the render thread type (2)
            // doesn't seem to cause any issue in this case
            long v = UsingTls.Get(0xB4);
            UsingTls.Set(0xB4, 2);
            bool b = InvokeRetBool(Memory.CScaleformMgr_IsMovieRendering, index);
            UsingTls.Set(0xB4, v);
            return b;
        }

        public static readonly bool Available = Memory.CScaleformMgr_GetRawMovieView != IntPtr.Zero && Memory.CScaleformMgr_IsMovieRendering != IntPtr.Zero;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x18)]
    internal struct GFxValue
    {
        [FieldOffset(0x00)] public IntPtr ObjectInterface;
        [FieldOffset(0x08)] public uint Type;
        [FieldOffset(0x10)] public double Value_NValue;
        [FieldOffset(0x10), MarshalAs(UnmanagedType.I1)] public bool Value_BValue;

        public GFxValue(uint type) : this()
        {
            ObjectInterface = IntPtr.Zero;
            Type = type;
        }

        public new uint GetType() => Type & VTC_TypeMask;
        public double GetNumber() => Value_NValue;

        public const uint VTC_ConvertBit = 0x80;
        public const uint VTC_TypeMask = VTC_ConvertBit | 0x0F;

        public const uint VT_Number = 0x03;

        public const uint VT_ConvertNumber = VTC_ConvertBit | VT_Number;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct GFxMovieView
    {
        [FieldOffset(0x00)] public IntPtr* VTable;

        public bool GetVariable(ref GFxValue val, string pathToVar)
            => InvokeRetBool(VTable[17], AsPointer(ref this), AsPointer(ref val), pathToVar);

        public double GetVariableDouble(string pathToVar)
        {
            GFxValue v = new GFxValue(GFxValue.VT_ConvertNumber);
            GetVariable(ref v, pathToVar);

            if (v.GetType() == GFxValue.VT_Number)
            {
                return v.GetNumber();
            }

            return 0.0;
        }
    }

    internal static class CBusySpinner
    {
        public static ref atArray<int> InstructionalButtons => ref AsRef<atArray<int>>(Memory.CBusySpinner_InstructionalButtons);

        public static readonly bool Available = Memory.CBusySpinner_InstructionalButtons != IntPtr.Zero;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x658)]
    internal unsafe struct scrProgramRegistry
    {
        public scrProgram* Find(string name) => Find(Game.GetHashKey(name));
        public scrProgram* Find(uint name) => (scrProgram*)InvokeRetPointer(Memory.scrProgramRegistry_Find, AsPointer(ref this), name);

        public static ref scrProgramRegistry Instance => ref AsRef<scrProgramRegistry>(Memory.scrProgramRegistry_sm_Instance);

        public static readonly bool Available = Memory.scrProgramRegistry_sm_Instance != IntPtr.Zero && Memory.scrProgramRegistry_Find != IntPtr.Zero;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x80)]
    internal unsafe struct scrProgram
    {
        [FieldOffset(0x10)] public byte** CodePages;
        [FieldOffset(0x18)] public uint Hash;
        [FieldOffset(0x1C)] public uint CodeLength;
        [FieldOffset(0x20)] public uint ArgsCount;
        [FieldOffset(0x24)] public uint StaticsCount;
        [FieldOffset(0x28)] public uint GlobalsCount;
        [FieldOffset(0x2C)] public uint NativesCount;
        [FieldOffset(0x30)] public scrValue* StaticsInitialValues;
        [FieldOffset(0x38)] public scrValue** GlobalsInitialValues;
        [FieldOffset(0x40)] public void** Natives;
        
        [FieldOffset(0x58)] public uint NameHash;
        [FieldOffset(0x5C)] public uint NumRefs;
        [FieldOffset(0x60)] public IntPtr Name;
        [FieldOffset(0x68)] public byte** Strings;
        [FieldOffset(0x70)] public uint StringsCount;

        public byte* IP(uint ip) => &CodePages[ip >> 14][ip & 0x3FFF];
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x8)]
    internal unsafe struct scrValue 
    {
        [FieldOffset(0x0)] public IntPtr Val;
        [FieldOffset(0x0)] public float AsFloat;
        [FieldOffset(0x0)] public int AsInt;
    }
}
