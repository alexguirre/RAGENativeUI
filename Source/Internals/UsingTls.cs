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
                WinFunctions.SetTlsValue(thisThreadTls, thisThreadSavedValue, Memory.TLS_AllocatorOffset);
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
            }
        }

        public static UsingTls Scope()
        {
            if (thisThreadRefCount == 0)
            {
                EnsureTlsPointers();

                thisThreadSavedValue = WinFunctions.GetTlsValue(thisThreadTls, Memory.TLS_AllocatorOffset);
                WinFunctions.CopyTlsValue(mainThreadTls, thisThreadTls, Memory.TLS_AllocatorOffset);
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

        public static unsafe long GetFromMain(int offset)
        {
            EnsureTlsPointers();

            return *(long*)(*(byte**)mainThreadTls + offset);
        }

        private static IntPtr mainThreadTls;
        [ThreadStatic] private static int thisThreadRefCount;
        [ThreadStatic] private static IntPtr thisThreadTls;
        [ThreadStatic] private static long thisThreadSavedValue;

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

            public static long GetTlsValue(IntPtr tlsPtr, int valueOffset)
                => *(long*)(*(byte**)tlsPtr + valueOffset);

            public static void SetTlsValue(IntPtr tlsPtr, long value, int valueOffset)
                => *(long*)(*(byte**)tlsPtr + valueOffset) = value;

            public static void CopyTlsValue(IntPtr sourceTlsPtr, IntPtr targetTlsPtr, int valueOffset)
                => *(long*)(*(byte**)targetTlsPtr + valueOffset) = *(long*)(*(byte**)sourceTlsPtr + valueOffset);

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
