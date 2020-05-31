namespace RAGENativeUI.Internals
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    internal struct UsingTls : IDisposable
    {
        public void Dispose()
        {
            thisThreadRefCount--;
            if (thisThreadRefCount == 0)
            {
                WinFunctions.SetTlsValues(thisThreadTls, thisThreadSavedValues, Offsets);
            }
        }

        private static void EnsureTlsPointers()
        {
            if (mainThreadTls == IntPtr.Zero)
            {
                mainThreadTls = WinFunctions.GetTlsPointer(WinFunctions.GetProcessMainThreadId());
            }

            if (thisThreadTls == IntPtr.Zero)
            {
                thisThreadTls = WinFunctions.GetTlsPointer(WinFunctions.GetCurrentThreadId());
                thisThreadSavedValues = new long[Offsets.Length];
            }
        }

        public static UsingTls Scope()
        {
            if (thisThreadRefCount == 0)
            {
                EnsureTlsPointers();

                WinFunctions.GetTlsValues(thisThreadTls, thisThreadSavedValues, Offsets);
                WinFunctions.CopyTlsValues(mainThreadTls, thisThreadTls, Offsets);
            }
            thisThreadRefCount++;
            return default;
        }

        public static unsafe long Get(int offset)
        {
            EnsureTlsPointers();

            return *(long*)(*(byte**)thisThreadTls + offset);
        }

        public static unsafe void Set(int offset, long value)
        {
            EnsureTlsPointers();

            *(long*)(*(byte**)thisThreadTls + offset) = value;
        }

        private static readonly int[] Offsets = { 0xC8 };

        private static IntPtr mainThreadTls;
        [ThreadStatic] private static int thisThreadRefCount;
        [ThreadStatic] private static IntPtr thisThreadTls;
        [ThreadStatic] private static long[] thisThreadSavedValues;

        private static unsafe class WinFunctions
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
            public static extern IntPtr GetModuleHandle(string moduleName);
            [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
            public static extern IntPtr GetProcAddress(IntPtr moduleHandle, string procName);
            [DllImport("kernel32.dll")]
            public static extern IntPtr OpenThread(ThreadAccess desiredAccess, bool inheritHandle, int threadId);
            [DllImport("kernel32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CloseHandle(IntPtr handle);
            [DllImport("kernel32.dll")]
            public static extern int GetCurrentThreadId();

            public delegate int NtQueryInformationThreadDelegate(IntPtr threadHandle, uint threadInformationClass, THREAD_BASIC_INFORMATION* outThreadInformation, ulong threadInformationLength, ulong* returnLength);
            public static NtQueryInformationThreadDelegate NtQueryInformationThread { get; }

            static WinFunctions()
            {
                IntPtr ntdllHandle = GetModuleHandle("ntdll.dll");
                NtQueryInformationThread = Marshal.GetDelegateForFunctionPointer<NtQueryInformationThreadDelegate>(GetProcAddress(ntdllHandle, "NtQueryInformationThread"));
            }

            public static int GetProcessMainThreadId()
            {
                long lowestStartTime = long.MaxValue;
                ProcessThread lowestStartTimeThread = null;
                foreach (ProcessThread thread in Process.GetCurrentProcess().Threads)
                {
                    long startTime = thread.StartTime.Ticks;
                    if (startTime < lowestStartTime)
                    {
                        lowestStartTime = startTime;
                        lowestStartTimeThread = thread;
                    }
                }

                return lowestStartTimeThread == null ? -1 : lowestStartTimeThread.Id;
            }

            public static IntPtr GetTlsPointer(int threadId)
            {
                IntPtr threadHandle = IntPtr.Zero;
                try
                {
                    threadHandle = OpenThread(ThreadAccess.QUERY_INFORMATION, false, threadId);

                    THREAD_BASIC_INFORMATION threadInfo = new THREAD_BASIC_INFORMATION();

                    int status = NtQueryInformationThread(threadHandle, 0, &threadInfo, (ulong)sizeof(THREAD_BASIC_INFORMATION), null);
                    if (status != 0)
                    {
                        Rage.Game.LogTrivialDebug($"Thread Invalid Query Status: {status}");
                        return IntPtr.Zero;
                    }

                    TEB* teb = (TEB*)threadInfo.TebBaseAddress;
                    return teb->ThreadLocalStoragePointer;
                }
                finally
                {
                    if (threadHandle != IntPtr.Zero)
                        CloseHandle(threadHandle);
                }
            }

            public static void GetTlsValues(IntPtr tlsPtr, long[] dest, params int[] valuesOffsets)
            {
                if (dest.Length != valuesOffsets.Length)
                {
                    throw new ArgumentException();
                }

                int i = 0;
                foreach (int offset in valuesOffsets)
                {
                    dest[i++] = *(long*)(*(byte**)tlsPtr + offset);
                }
            }

            public static void SetTlsValues(IntPtr tlsPtr, long[] src, params int[] valuesOffsets)
            {
                if (src.Length != valuesOffsets.Length)
                {
                    throw new ArgumentException();
                }

                int i = 0;
                foreach (int offset in valuesOffsets)
                {
                    *(long*)(*(byte**)tlsPtr + offset) = src[i++];
                }
            }

            public static void CopyTlsValues(IntPtr sourceTlsPtr, IntPtr targetTlsPtr, params int[] valuesOffsets)
            {
                foreach (int offset in valuesOffsets)
                {
                    *(long*)(*(byte**)targetTlsPtr + offset) = *(long*)(*(byte**)sourceTlsPtr + offset);
                }
            }

            [Flags]
            public enum ThreadAccess : int
            {
                QUERY_INFORMATION = (0x0040),
            }

            [StructLayout(LayoutKind.Explicit, Size = 0x30)]
            public struct THREAD_BASIC_INFORMATION
            {
                [FieldOffset(0x0000)] public int ExitStatus;
                [FieldOffset(0x0008)] public IntPtr TebBaseAddress;
            }

            // http://msdn.moonsols.com/win7rtm_x64/TEB.html
            [StructLayout(LayoutKind.Explicit, Size = 0x1818)]
            public struct TEB
            {
                [FieldOffset(0x0058)] public IntPtr ThreadLocalStoragePointer;
            }
        }
    }
}
