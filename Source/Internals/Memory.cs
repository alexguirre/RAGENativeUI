namespace RAGENativeUI.Internals
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    using Rage;

    internal static unsafe class Memory
    {
        [Conditional("DEBUG")]
        private static void Log(string str) => Game.LogTrivialDebug($"[RAGENativeUI::{nameof(Memory)}] {str}");

        public const int MaxMemoryAddresses = 20;
        public const int MaxInts = 4;
        
        public static readonly int sysMemAllocator_Allocate_VTableOffset = -1;
        public static readonly int TLSOffset_sysMemAllocator_Current = -1;
        public static readonly int TLSOffset_sysMemAllocator_Master = -1;
        public static readonly IntPtr Screen_GetActualWidth, Screen_GetActualHeight;
        public static readonly IntPtr CTextStyle_ScriptStyle;
        public static readonly IntPtr CScaleformMgr_IsMovieRendering;
        public static readonly IntPtr CScaleformMgr_BeginMethod;
        public static readonly IntPtr CBusySpinner_InstructionalButtons;
        public static readonly IntPtr scrProgramRegistry_sm_Instance;
        public static readonly IntPtr scrProgramRegistry_Find;
        public static readonly IntPtr Native_DisableControlAction;
        public static readonly IntPtr Native_DrawSprite;
        public static readonly IntPtr Native_DrawRect;
        public static readonly IntPtr CTextFormat_GetInputSourceIcons;
        public static readonly IntPtr CTextFormat_GetIconListFormatString;
        public static readonly IntPtr CControlMgr_sm_MappingMgr_KeyboardLayout;
        public static readonly IntPtr CTextFile_sm_Instance;
        public static readonly IntPtr CTextFile_sm_CriticalSection;
        public static readonly IntPtr CTextFile_GetStringByHash;
        public static readonly int CTextFile_OverridesTextMapOffset = -1;
        public static readonly IntPtr g_FragmentStore;
        public static readonly IntPtr g_DrawableStore;
        public static readonly IntPtr g_TxdStore;
        public static readonly IntPtr atStringHash;
        public static readonly int TimershudSharedGlobalId = -1;
        public static readonly int TimershudSharedTimerbarsTotalHeightOffset = -1;
        public static readonly int TimerbarsPrevTotalHeightGlobalId = -1;
        public static readonly int TimershudSharedInstructionalButtonsNumRowsOffset = -1;
        public static readonly IntPtr grcTextureFactory_Instance;
        public static readonly IntPtr pgDictionary_grcTexture_ctor;

        static Memory()
        {
            var tlsAllocatorOffsetAddr = FindAddress(() => Game.FindPattern("41 BB ?? ?? ?? ?? 48 8B 14 C8 48 8B 0C C8 41 B8 ?? ?? ?? ?? 4D 8B 0C 13"));
            if (tlsAllocatorOffsetAddr != IntPtr.Zero)
            {
                TLSOffset_sysMemAllocator_Current = *(int*)(tlsAllocatorOffsetAddr + 2);
                TLSOffset_sysMemAllocator_Master = *(int*)(tlsAllocatorOffsetAddr + 16);
            }

            var sysMemAllocatorAllocAddr = FindAddress(() => Game.FindPattern("B9 ?? ?? ?? ?? 48 8B 0C 01 45 33 C9 49 8B D2"));
            if (sysMemAllocatorAllocAddr != IntPtr.Zero)
            {
                sysMemAllocator_Allocate_VTableOffset = *(byte*)(sysMemAllocatorAllocAddr + 0x15);
            }

            const string GetActualResolutionPattern = "48 83 EC 38 0F 29 74 24 ?? 66 0F 6E 35 ?? ?? ?? ?? 48 8D 0D ?? ?? ?? ??";

            Screen_GetActualHeight = FindAddress(() => Game.FindPattern(GetActualResolutionPattern));
            if (Screen_GetActualHeight != IntPtr.Zero)
            {
                Screen_GetActualWidth = FindAddress(() => Game.FindPattern(GetActualResolutionPattern, Screen_GetActualHeight + 1));
            }

            CTextStyle_ScriptStyle = FindAddress(() =>
            {
                IntPtr addr = Game.FindPattern("48 8D 0D ?? ?? ?? ?? 44 8A C3 E8 ?? ?? ?? ?? 83 25");
                if (addr != IntPtr.Zero)
                {
                    addr += *(int*)(addr + 3) + 7;
                }
                return addr;
            });

            CScaleformMgr_IsMovieRendering = FindAddress(() =>
            {
                IntPtr addr = Game.FindPattern("65 48 8B 04 25 ?? ?? ?? ?? 8B F9 48 8B 04 D0 B9 ?? ?? ?? ?? 8B 14 01");
                if (addr != IntPtr.Zero)
                {
                    addr -= 0x10;
                }
                return addr;
            });

            CScaleformMgr_BeginMethod = FindAddress(() => Game.FindPattern("48 83 EC 38 41 83 C9 FF 44 89 4C 24 ?? E8"));

            CBusySpinner_InstructionalButtons = FindAddress(() =>
            {
                IntPtr addr = Game.FindPattern("48 8B 05 ?? ?? ?? ?? 3B 1C 90 74 22 48 FF C2 ");
                if (addr != IntPtr.Zero)
                {
                    addr += *(int*)(addr + 3) + 7;
                }
                return addr;
            });

            IntPtr scrProgramRegistryAddr = FindAddress(() => Game.FindPattern("48 8D 0D ?? ?? ?? ?? 8B D0 44 8B F0 89 85"));
            if (scrProgramRegistryAddr != IntPtr.Zero)
            {
                scrProgramRegistry_sm_Instance = scrProgramRegistryAddr + *(int*)(scrProgramRegistryAddr + 3) + 7;
                scrProgramRegistry_Find = scrProgramRegistryAddr + *(int*)(scrProgramRegistryAddr + 19) + 23;
            }

            Native_DisableControlAction = FindAddress(() =>
            {
                IntPtr addr = Game.FindPattern("81 FA ?? ?? ?? ?? 77 48 E8 ?? ?? ?? ?? 45 33 C9");
                if (addr != IntPtr.Zero)
                {
                    addr -= 0x20;
                }
                return addr;
            });

            Native_DrawSprite = FindAddress(() =>
            {
                IntPtr addr = Game.FindPattern("48 8B D9 0F 28 FA 0F 28 F3 E8 ?? ?? ?? ?? 80 BC 24");
                if (addr != IntPtr.Zero)
                {
                    addr -= 0x22;
                }
                return addr;
            });

            Native_DrawRect = FindAddress(() =>
            {
                IntPtr addr = Game.FindPattern("44 0F 29 40 ?? 44 0F 29 48 ?? 44 0F 28 C3 48 69 C9");
                if (addr != IntPtr.Zero)
                {
                    addr -= 0x1B;
                }
                return addr;
            });

            {
                IntPtr addr = FindAddress(() => Game.FindPattern("E8 ?? ?? ?? ?? 83 7C 24 ?? ?? 7E 3A 48 8D 4C 24"));
                if (addr != IntPtr.Zero)
                {
                    CTextFormat_GetInputSourceIcons = addr + *(int*)(addr + 1) + 5;
                    addr += 0x27;
                    CTextFormat_GetIconListFormatString = addr + *(int*)(addr + 1) + 5;
                }
            }

            CControlMgr_sm_MappingMgr_KeyboardLayout = FindAddress(() =>
            {
                IntPtr addr = Game.FindPattern("48 8D 05 ?? ?? ?? ?? 41 B8 ?? ?? ?? ?? 48 C1 ?? 04 48 03 ?? 44 39 ??");
                if (addr != IntPtr.Zero)
                {
                    addr += *(int*)(addr + 3) + 7;
                }
                return addr;
            });

            CTextFile_sm_Instance = FindAddress(() =>
            {
                IntPtr addr = Game.FindPattern("48 8D 0D ?? ?? ?? ?? 48 8B D3 E8 ?? ?? ?? ?? 48 8D 54 24 ?? 48 8D 0D");
                if (addr != IntPtr.Zero)
                {
                    addr += *(int*)(addr + 3) + 7;
                }
                return addr;
            });

            {
                IntPtr addr = FindAddress(() => Game.FindPattern("48 8D 0D ?? ?? ?? ?? 44 8B F2 E8 ?? ?? ?? ?? 83 8B"));
                if (addr != IntPtr.Zero)
                {
                    CTextFile_GetStringByHash = addr - 0x19;
                    CTextFile_sm_CriticalSection = addr + *(int*)(addr + 3) + 7;
                    CTextFile_OverridesTextMapOffset = *(int*)(addr + 0x19);
                }
            }

            {
                IntPtr addr = FindAddress(() => Game.FindPattern("48 8B 05 ?? ?? ?? ?? 48 8D 0D ?? ?? ?? ?? FF 90 ?? ?? ?? ?? 48 8B 0D"));
                if (addr != IntPtr.Zero)
                {
                    addr -= 0x30;
                    g_FragmentStore = addr + *(int*)(addr + 3) + 7;
                    addr -= 0xC;
                    g_TxdStore = addr + *(int*)(addr + 3) + 7;
                    addr -= 0xC;
                    g_DrawableStore = addr + *(int*)(addr + 3) + 7;
                }
            }

            atStringHash = FindAddress(() =>
            {
                IntPtr addr = Game.FindPattern("E8 ?? ?? ?? ?? 44 8B 47 10 48 8B CB 8B D0 E8 ?? ?? ?? ?? 48 8D 4F 38 41 B0 01");
                if (addr != IntPtr.Zero)
                {
                    addr += *(int*)(addr + 1) + 5;
                }
                return addr;
            });

            grcTextureFactory_Instance = FindAddress(() =>
            {
                IntPtr addr = Game.FindPattern("48 8B 1D ?? ?? ?? ?? 48 8B 10 48 8B 3B 48 8B C8 FF 92");
                if (addr != IntPtr.Zero)
                {
                    addr += *(int*)(addr + 3) + 7;
                }
                return addr;
            });

            pgDictionary_grcTexture_ctor = FindAddress(() =>
            {
                IntPtr addr = Game.FindPattern("E8 ?? ?? ?? ?? 48 8B F8 EB 02 33 FF 4C 8D 3D");
                if (addr != IntPtr.Zero)
                {
                    addr += *(int*)(addr + 1) + 5;
                }
                return addr;
            });

            if (Shared.MemoryInts[0] == 0 || Shared.MemoryInts[1] == 0 || Shared.MemoryInts[2] == 0)
            {
                // b2802 introduced three new script opcodes after CALL (0x5D)
                int fix = Game.ProductVersion.Build >= 2802 ? 3 : 0;
                IntPtr ingamehudAddr = FindPatternInScript("ingamehud",
                                                        new byte[] { (byte)(0x6F + fix), 0x39, 0x08, (byte)(0x5F + fix), 0x00, 0x00, 0x00, (byte)(0x5E + fix), 0x00, 0x00, 0x00, 0x47 },
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

            if (Shared.MemoryInts[3] == 0)
            {
                IntPtr ingamehudAddr = FindPatternInScript("ingamehud",
                                                        new byte[] { 0x47, 0x00, 0x00, 0x22, 0x10, 0x0E, 0x39, },
                                                        new byte[] { 0xFF, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, });
                if (ingamehudAddr != IntPtr.Zero)
                {
                    Shared.MemoryInts[3] = TimershudSharedInstructionalButtonsNumRowsOffset = *(short*)(ingamehudAddr + 1);
                }
            }
            else
            {
                TimershudSharedInstructionalButtonsNumRowsOffset = Shared.MemoryInts[3];
            }

#if DEBUG
            Game.LogTrivialDebug($"[RAGENativeUI] Available memory stuff:");
            Game.LogTrivialDebug($"[RAGENativeUI]  > TLS_AllocatorOffset = {TLSOffset_sysMemAllocator_Current != -1}");
            Game.LogTrivialDebug($"[RAGENativeUI]  > ScriptGlobals.TimerBarsInstructionButtonsNumRows = {ScriptGlobals.TimerBarsInstructionButtonsNumRowsAvailable}");
            Game.LogTrivialDebug($"[RAGENativeUI]  > ScriptGlobals.TimersBarsTotalHeight = {ScriptGlobals.TimersBarsTotalHeightAvailable}");
            Game.LogTrivialDebug($"[RAGENativeUI]  > Screen = {Screen.Available}");
            Game.LogTrivialDebug($"[RAGENativeUI]  > DirectNatives.DisableControlAction = {DirectNatives.DisableControlActionAvailable}");
            Game.LogTrivialDebug($"[RAGENativeUI]  > DirectNatives.DrawRect = {DirectNatives.DrawRectAvailable}");
            Game.LogTrivialDebug($"[RAGENativeUI]  > DirectNatives.DrawSprite = {DirectNatives.DrawSpriteAvailable}");
            Game.LogTrivialDebug($"[RAGENativeUI]  > CTextStyle.ScriptStyle = {CTextStyle.ScriptStyleAvailable}");
            Game.LogTrivialDebug($"[RAGENativeUI]  > CScaleformMgr = {CScaleformMgr.Available}");
            Game.LogTrivialDebug($"[RAGENativeUI]  > CBusySpinner = {CBusySpinner.Available}");
            Game.LogTrivialDebug($"[RAGENativeUI]  > scrProgramRegistry = {scrProgramRegistry.Available}");
            Game.LogTrivialDebug($"[RAGENativeUI]  > CTextFormat = {CTextFormat.Available}");
            Game.LogTrivialDebug($"[RAGENativeUI]  > CTextFile = {CTextFile.Available}");
            Game.LogTrivialDebug($"[RAGENativeUI]  > grcTextureFactory = {grcTextureFactory.Available}");
            Game.LogTrivialDebug($"[RAGENativeUI]  > pgDictionary<grcTexture>::ctor = {pgDictionary_grcTexture_ctor != IntPtr.Zero}");
#endif
        }

        private static int findAddressKey = 0;
        private static IntPtr FindAddress(Func<IntPtr> find, [CallerLineNumber] int lineNumber = 0)
        {
            int key = findAddressKey++;

            if (key >= MaxMemoryAddresses)
            {
                throw new InvalidOperationException($"{nameof(Shared.MemoryAddresses)} array is full");
            }

            if (Shared.MemoryAddresses[key] == 0)
            {
                IntPtr addr = find();
                if (addr == IntPtr.Zero)
                {
                    Log($"{nameof(FindAddress)}: ERROR - Address at line #{lineNumber} not found");
                }
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
                Log($"{nameof(FindPatternInScript)}: ERROR - scrProgramRegistry not available");
                return IntPtr.Zero;
            }

            scrProgram* prog = scrProgramRegistry.Instance.Find(name);

            if (prog == null)
            {
                Log($"{nameof(FindPatternInScript)}: ERROR - scrProgram '{name}' not loaded");
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

            Log($"{nameof(FindPatternInScript)}: ERROR - Pattern '{string.Join(" ", pattern.Zip(mask, (p, m) => m == 0xFF ? p.ToString("X2") : "??"))}' in scrProgram '{name}' not found");
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

        public static int StrLen(byte* str)
        {
            int len = 0;
            while (str[len] != 0) { len++; }
            return len;
        }
    }

    internal static unsafe class ScriptGlobals
    {
        public static readonly bool TimersBarsTotalHeightAvailable = Memory.TimershudSharedGlobalId != -1 && Memory.TimershudSharedTimerbarsTotalHeightOffset != -1 && Memory.TimerbarsPrevTotalHeightGlobalId != -1;

        public static ref float TimerBarsTotalHeight => ref Unsafe.AsRef<float>((void*)(Game.GetScriptGlobalVariableAddress(Memory.TimershudSharedGlobalId) + Memory.TimershudSharedTimerbarsTotalHeightOffset * 8));
        public static ref float TimerBarsPrevTotalHeight => ref Unsafe.AsRef<float>((void*)Game.GetScriptGlobalVariableAddress(Memory.TimerbarsPrevTotalHeightGlobalId));

        public static readonly bool TimerBarsInstructionButtonsNumRowsAvailable = Memory.TimershudSharedGlobalId != -1 && Memory.TimershudSharedInstructionalButtonsNumRowsOffset != -1;

        public static ref int TimerBarsInstructionButtonsNumRows => ref Unsafe.AsRef<int>((void*)(Game.GetScriptGlobalVariableAddress(Memory.TimershudSharedGlobalId) + Memory.TimershudSharedInstructionalButtonsNumRowsOffset * 8));

#if false // not needed for now
        public static readonly bool MenuIdsAvailable = true;
        public static ref ScriptUIntArray MenuIds => ref AsRef<ScriptUIntArray>(Game.GetScriptGlobalVariableAddress(22350) + 5721 * 8);

        [StructLayout(LayoutKind.Explicit)]
        public struct ScriptUIntArray
        {
            [FieldOffset(0x0)] public int Size;

            public ref uint this[int index] => ref *(uint*)((byte*)AsPointer(ref this) + 8 + (8 * (Size - index - 1)));

            public Enumerator GetEnumerator() => new Enumerator(ref this);

            public ref struct Enumerator
            {
                private readonly ScriptUIntArray* array;
                private int index;

                [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                public Enumerator(ref ScriptUIntArray arr)
                {
                    array = (ScriptUIntArray*)AsPointer(ref arr);
                    index = -1;
                }

                [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                public bool MoveNext()
                {
                    int newIndex = index + 1;
                    if (newIndex < array->Size)
                    {
                        index = newIndex;
                        return true;
                    }

                    return false;
                }

                public ref uint Current
                {
                    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                    get => ref (*array)[index];
                }
            }
        }
#endif
    }

    internal static class Screen
    {
        public static readonly bool Available = Memory.Screen_GetActualWidth != IntPtr.Zero && Memory.Screen_GetActualHeight != IntPtr.Zero;

        /// <summary>
        /// Gets the resolution of the main screen, the one containing the UI in case multiple screens are used.
        /// </summary>
        public static unsafe SizeF ActualResolution
        {
            get
            {
                if (Available)
                {
                    using var tls = UsingTls.Scope();

                    var w = ((delegate* unmanaged[Stdcall]<float>)Memory.Screen_GetActualWidth)();
                    var h = ((delegate* unmanaged[Stdcall]<float>)Memory.Screen_GetActualHeight)();
                    return new SizeF(w, h);
                }
                else
                {
                    return Game.Resolution; // not the same since this the resolution of all screens combined, but good enough as a fallback
                }
            }
        }
    }

    internal static unsafe class DirectNatives
    {
        public static readonly bool DisableControlActionAvailable = Memory.Native_DisableControlAction != IntPtr.Zero;
        public static void DisableControlAction(int index, int control, byte b) => ((delegate* unmanaged[Stdcall]<int, int, byte, void>)Memory.Native_DisableControlAction)(index, control, b);

        public static readonly bool DrawSpriteAvailable = Memory.Native_DrawSprite != IntPtr.Zero;
        public static void DrawSprite(string textureDict, string textureName, float x, float y, float width, float height, float rotation, int r, int g, int b, int a, byte unk)
        {
            using var tls = UsingTls.Scope();
            ((delegate* unmanaged[Stdcall]<string, string, float, float, float, float, float, int, int, int, int, byte, void>)Memory.Native_DrawSprite)(textureDict, textureName, x, y, width, height, rotation, r, g, b, a, unk);
        }

        public static readonly bool DrawRectAvailable = Memory.Native_DrawRect != IntPtr.Zero;
        public static void DrawRect(float x, float y, float width, float height, int r, int g, int b, int a, byte unk)
        {
            using var tls = UsingTls.Scope();
            ((delegate* unmanaged[Stdcall]<float, float, float, float, int, int, int, int, byte, void>)Memory.Native_DrawRect)(x, y, width, height, r, g, b, a, unk);
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


        public static unsafe ref CTextStyle ScriptStyle => ref Unsafe.AsRef<CTextStyle>((void*)Memory.CTextStyle_ScriptStyle);

        public static readonly bool ScriptStyleAvailable = Memory.CTextStyle_ScriptStyle != IntPtr.Zero;
    }

    internal static class CScaleformMgr
    {
        public static unsafe bool BeginMethod(int index, int baseClass, string methodName)
        {
            // set thread type to 1 so CScaleformMgr::BeginMethod adds the method call to the same queue as if it was done using natives
            long v = UsingTls.Get(0xB4);
            UsingTls.Set(0xB4, 1);
            byte b = ((delegate* unmanaged[Stdcall]<int, int, string, byte>)Memory.CScaleformMgr_BeginMethod)(index, baseClass, methodName);
            UsingTls.Set(0xB4, v);
            return b != 0;
        }

        public static unsafe bool IsMovieRendering(int index)
        {
            // CScaleformMgr::IsMovieRendering checks if the current thread is the render thread,
            // so temporarily set this thread type to the render thread type (2)
            // doesn't seem to cause any issue in this case
            long v = UsingTls.Get(0xB4);
            UsingTls.Set(0xB4, 2);
            byte b = ((delegate* unmanaged[Stdcall]<int, byte>)Memory.CScaleformMgr_IsMovieRendering)(index);
            UsingTls.Set(0xB4, v);
            return b != 0;
        }

        public static readonly bool Available = Memory.CScaleformMgr_IsMovieRendering != IntPtr.Zero && Memory.CScaleformMgr_BeginMethod != IntPtr.Zero;
    }

    internal static class CBusySpinner
    {
        public static unsafe ref atArray<int> InstructionalButtons => ref Unsafe.AsRef<atArray<int>>((void*)Memory.CBusySpinner_InstructionalButtons);

        public static readonly bool Available = Memory.CBusySpinner_InstructionalButtons != IntPtr.Zero;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x658)]
    internal unsafe struct scrProgramRegistry
    {
        public scrProgram* Find(string name) => Find(Game.GetHashKey(name));
        public scrProgram* Find(uint name) => ((delegate* unmanaged[Thiscall]<ref scrProgramRegistry, uint, scrProgram*>)Memory.scrProgramRegistry_Find)(ref this, name);

        public static ref scrProgramRegistry Instance => ref Unsafe.AsRef<scrProgramRegistry>((void*)Memory.scrProgramRegistry_sm_Instance);

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

    internal static unsafe class CTextFormat
    {
        public static readonly bool Available = Memory.CTextFormat_GetInputSourceIcons != IntPtr.Zero &&
                                                Memory.CTextFormat_GetIconListFormatString != IntPtr.Zero;

        public static void GetInputSourceIcons(uint mapperSource, uint mapperParameter, ref atFixedArray_sIconData_4 icons)
            => ((delegate* unmanaged[Stdcall]<uint, uint, ref atFixedArray_sIconData_4, void>)Memory.CTextFormat_GetInputSourceIcons)(mapperSource, mapperParameter, ref icons);

        public static void GetIconListFormatString(ref atFixedArray_sIconData_4 icons, byte* buffer, uint bufferSize)
            => ((delegate* unmanaged[Stdcall]<ref atFixedArray_sIconData_4, byte*, uint, void*, void>)Memory.CTextFormat_GetIconListFormatString)(ref icons, buffer, bufferSize, null);
    }

    [StructLayout(LayoutKind.Sequential, Size = 0x8)]
    internal struct sIconData
    {
        public int rawIdOrIndex;
        public int type;
    }

    [StructLayout(LayoutKind.Sequential, Size = 0x24)]
    internal struct atFixedArray_sIconData_4
    {
        public sIconData Item0;
        public sIconData Item1;
        public sIconData Item2;
        public sIconData Item3;
        public int Count;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct CTextFile
    {
        [StructLayout(LayoutKind.Explicit, Size = 0x18)]
        public struct atBinaryMap
        {
            [StructLayout(LayoutKind.Explicit, Size = 0x10)]
            public struct DataPair
            {
                [FieldOffset(0x0)] public uint Key;
                [FieldOffset(0x8)] public IntPtr Value;
            }

            [FieldOffset(0x00), MarshalAs(UnmanagedType.I1)] public bool IsSorted;
            [FieldOffset(0x08)] public atArray<DataPair> Pairs;

            // returns old value if the key already existed, IntPtr.Zero otherwise
            public IntPtr AddOrSet(uint key, IntPtr value)
            {
                // TODO: AddOrSet/Remove use linear search, binary search could be used
                // insert keeping the array sorted
                DataPair pairToInsert = new DataPair { Key = key, Value = value };
                foreach (ref var pair in Pairs)
                {
                    if (pair.Key == pairToInsert.Key)
                    {
                        // key already exists, replace its value
                        var tmp = pair.Value;
                        pair.Value = pairToInsert.Value;
                        return tmp;
                    }

                    // key does not exist, insert it in the middle, and move back the remaining pairs
                    if (pair.Key > pairToInsert.Key)
                    {
                        // swap
                        var tmp = pair;
                        pair = pairToInsert;
                        pairToInsert = tmp;
                    }
                }

                // add the last pair
                Pairs.Add() = pairToInsert;
                return IntPtr.Zero;
            }

            public IntPtr Remove(uint key)
            {
                int index = -1;
                for (int i = 0; i < Pairs.Count; i++)
                {
                    if (Pairs[i].Key == key)
                    {
                        index = i;
                        break;
                    }
                }

                return index != -1 ? Pairs.RemoveAt(index).Value : IntPtr.Zero;
            }
        }

        // this is the first map checked when retrieving text labels and seems to be unused, it is always empty as far as I can tell
        // Should be good enough to allow us to override/add text labels
        public ref atBinaryMap OverridesTextMap
        {
            get
            {
                fixed (void* This = &this)
                {
                    return ref Unsafe.AsRef<atBinaryMap>((byte*)This + Memory.CTextFile_OverridesTextMapOffset);
                }
            }
        }

        public IntPtr GetStringByHash(uint hash) => ((delegate* unmanaged[Thiscall]<ref CTextFile, uint, nint>)Memory.CTextFile_GetStringByHash)(ref this, hash);

        public static ref CTextFile Instance => ref Unsafe.AsRef<CTextFile>((void*)Memory.CTextFile_sm_Instance);
        public static ref CRITICAL_SECTION CriticalSection => ref Unsafe.AsRef<CRITICAL_SECTION>((void*)Memory.CTextFile_sm_CriticalSection);

        public static readonly bool Available = Memory.CTextFile_sm_Instance != IntPtr.Zero &&
                                                Memory.CTextFile_OverridesTextMapOffset != -1 &&
                                                Memory.CTextFile_GetStringByHash != IntPtr.Zero &&
                                                Memory.CTextFile_sm_CriticalSection != IntPtr.Zero;
    }

    [StructLayout(LayoutKind.Sequential, Size = 0x28)]
    internal struct CRITICAL_SECTION
    {
        public IntPtr DebugInfo;
        // ...

        public void Enter()
        {
            if (DebugInfo != IntPtr.Zero)
            {
                EnterCriticalSection(ref this);
            }
        }

        public void Leave()
        {
            if (DebugInfo != IntPtr.Zero)
            {
                LeaveCriticalSection(ref this);
            }
        }

        [DllImport("kernel32.dll")] static extern void EnterCriticalSection(ref CRITICAL_SECTION lpCriticalSection);
        [DllImport("kernel32.dll")] static extern void LeaveCriticalSection(ref CRITICAL_SECTION lpCriticalSection);
    }

    [StructLayout(LayoutKind.Sequential, Size = 8)]
    internal unsafe struct sysMemAllocator
    {
        private readonly IntPtr* vtable;

        public void* Allocate(ulong size, ulong align, int subAllocator) => ((delegate* unmanaged[Thiscall]<ref sysMemAllocator, ulong, ulong, int, void*>)vtable[VTableIndexAllocate])(ref this, size, align, subAllocator);
        public void Free(void* ptr) => ((delegate* unmanaged[Thiscall]<ref sysMemAllocator, void*, void>)vtable[VTableIndexFree])(ref this, ptr);

        private static readonly int VTableIndexAllocate = Memory.sysMemAllocator_Allocate_VTableOffset / 8;
        private static readonly int VTableIndexFree = VTableIndexAllocate + 2;

        public static ref sysMemAllocator Current => ref Unsafe.AsRef<sysMemAllocator>((void*)(IntPtr)UsingTls.GetFromMain(Memory.TLSOffset_sysMemAllocator_Current));
    }

    internal unsafe struct grcTexture
    {
    }

    internal enum grcBufferFormat : uint
    {
        B8G8R8A8_UNORM = 0x2,  // DXGI_FORMAT_B8G8R8A8_UNORM
        R8G8B8A8_UNORM = 0x28, // DXGI_FORMAT_R8G8B8A8_UNORM
    }

    [StructLayout(LayoutKind.Sequential, Size = 8)]
    internal unsafe struct grcTextureFactory
    {
        [StructLayout(LayoutKind.Explicit, Size = 0x30)]
        public struct TextureCreateParams
        {
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct VTable
        {
            [FieldOffset(8 * 2)] public delegate* unmanaged[Thiscall]<ref grcTextureFactory, uint, uint, grcBufferFormat, IntPtr, byte, TextureCreateParams*, grcTexture*> Create_raw;

            [FieldOffset(8 * 4)] public delegate* unmanaged[Thiscall]<ref grcTextureFactory, string, TextureCreateParams*, grcTexture*> Create_name;
        }

        private readonly VTable* vtable;

        public grcTexture* Create(uint width, uint height, grcBufferFormat format, IntPtr initialData, TextureCreateParams* createParams = null) => vtable->Create_raw(ref this, width, height, format, initialData, 0, createParams);
        public grcTexture* Create(string name, TextureCreateParams* createParams = null) => vtable->Create_name(ref this, name, createParams);

        public static ref grcTextureFactory Instance => ref Unsafe.AsRef<grcTextureFactory>(*(void**)Memory.grcTextureFactory_Instance);

        public static readonly bool Available = Memory.grcTextureFactory_Instance != IntPtr.Zero;
    }

    [StructLayout(LayoutKind.Sequential, Size = 4)]
    internal struct strLocalIndex { public uint Value; }

    internal unsafe struct fwTxdStore
    {
        [StructLayout(LayoutKind.Explicit)]
        private struct VTable
        {
            [FieldOffset(8 * 1)] public delegate* unmanaged[Thiscall]<ref fwTxdStore, out strLocalIndex, string, void> Register;
            [FieldOffset(8 * 2)] public delegate* unmanaged[Thiscall]<ref fwTxdStore, out strLocalIndex, string, void> FindSlot;

            [FieldOffset(8 * 4)] public delegate* unmanaged[Thiscall]<ref fwTxdStore, strLocalIndex, void> RemoveSlot;

            [FieldOffset(8 * 16)] public delegate* unmanaged[Thiscall]<ref fwTxdStore, strLocalIndex, uint, void> AddRef;
            [FieldOffset(8 * 17)] public delegate* unmanaged[Thiscall]<ref fwTxdStore, strLocalIndex, uint, void> RemoveRef;

            [FieldOffset(8 * 19)] public delegate* unmanaged[Thiscall]<ref fwTxdStore, strLocalIndex, int> GetNumRefs;

            [FieldOffset(8 * 32)] public delegate* unmanaged[Thiscall]<ref fwTxdStore, uint> GetSize;
            [FieldOffset(8 * 33)] public delegate* unmanaged[Thiscall]<ref fwTxdStore, uint> GetNumUsedSlots;

            [FieldOffset(8 * 37)] public delegate* unmanaged[Thiscall]<ref fwTxdStore, strLocalIndex, pgDictionary<grcTexture>*, void> Set;

            [FieldOffset(8 * 39)] public delegate* unmanaged[Thiscall]<ref fwTxdStore, out strLocalIndex, in uint, void> AddSlot;
        }

        private readonly VTable* vtable;

        public uint Size => vtable->GetSize(ref this);
        public uint NumUsedSlots => vtable->GetNumUsedSlots(ref this);

        public strLocalIndex Register(string name)
        {
            vtable->Register(ref this, out var result, name);
            return result;
        }

        public strLocalIndex FindSlot(string name)
        {
            vtable->FindSlot(ref this, out var result, name);
            return result;
        }

        public void RemoveSlot(strLocalIndex index) => vtable->RemoveSlot(ref this, index);
        public void AddRef(strLocalIndex index) => vtable->AddRef(ref this, index, 0);
        public void RemoveRef(strLocalIndex index) => vtable->RemoveRef(ref this, index, 0);
        public int GetNumRefs(strLocalIndex index) => vtable->GetNumRefs(ref this, index);
        public void Set(strLocalIndex index, pgDictionary<grcTexture>* txd) => vtable->Set(ref this, index, txd);

        public strLocalIndex AddSlot(uint nameHash)
        {
            vtable->AddSlot(ref this, out var result, nameHash);
            return result;
        }

        public static ref fwTxdStore Instance => ref Unsafe.AsRef<fwTxdStore>((void*)Memory.g_TxdStore);

        public static readonly bool Available = Memory.g_TxdStore != IntPtr.Zero;
    }
}
