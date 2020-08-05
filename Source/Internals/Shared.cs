namespace RAGENativeUI.Internals
{
    using System.Drawing;
    using System.IO.MemoryMappedFiles;
    using System.Runtime.InteropServices;
    using Rage;
    using RAGENativeUI.PauseMenu;

    internal static unsafe class Shared
    {
        private const string MappedFileName = "rnui_shared_state{74904B8B-6080-42D9-B17F-506FEF596943}";

        private static readonly StaticFinalizer Finalizer = new StaticFinalizer(Shutdown);
        private static MemoryMappedFile mappedFile;
        private static MemoryMappedViewAccessor mappedFileAccessor;
        private static SharedData* data;


        public static ref uint TimerBarsLastFrame => ref data->TimerBarsLastFrame;
        public static ref float TimerBarsTotalHeight => ref data->TimerBarsTotalHeight;
        public static ref uint NumInstructionalButtonsRowsLastFrame => ref data->NumInstructionalButtonsRowsLastFrame;
        public static ref int NumInstructionalButtonsRows => ref data->NumInstructionalButtonsRows;
        public static ref bool IngamehudScriptExecuting => ref data->IngamehudScriptExecuting;

        public static ref uint ScreenLastFrame => ref data->ScreenLastFrame;
        public static ref SizeF ActualScreenResolution => ref data->ActualScreenResolution;
        public static ref float AspectRatio => ref data->AspectRatio;

        public static ref uint NumberOfVisibleMenus => ref data->NumberOfVisibleMenus;
        public static ref uint NumberOfVisiblePauseMenus => ref data->NumberOfVisiblePauseMenus;

        public static long* MemoryAddresses => data->MemoryAddresses;
        public static int* MemoryInts => data->MemoryInts;

        public static ref uint TokenParserHookRefs => ref data->TokenParserHookRefs;

        static Shared()
        {
            Game.LogTrivialDebug($"[RAGENativeUI::Shared] Init from '{System.AppDomain.CurrentDomain.FriendlyName}'");
            Game.LogTrivialDebug($"[RAGENativeUI::Shared] > sizeof(SharedData) = {sizeof(SharedData)}");

            mappedFile = MemoryMappedFile.CreateOrOpen(MappedFileName, sizeof(SharedData));
            mappedFileAccessor = mappedFile.CreateViewAccessor(0, sizeof(SharedData));

            byte* ptr = null;
            mappedFileAccessor.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
            data = (SharedData*)ptr;
        }

        private static void Shutdown()
        {
            Game.LogTrivialDebug($"[RAGENativeUI::Shared] Shutdown from '{System.AppDomain.CurrentDomain.FriendlyName}'");

            // cleanup
            {
                // in case there is any visible menus when unloading the plugin
                NumberOfVisibleMenus -= UIMenu.NumberOfVisibleMenus;
                NumberOfVisiblePauseMenus -= TabView.NumberOfVisiblePauseMenus;
                TokenParserHook.Shutdown();
            }

            // dispose mapped file
            if (mappedFileAccessor != null)
            {
                data = null;
                mappedFileAccessor.SafeMemoryMappedViewHandle.ReleasePointer();
                mappedFileAccessor.Dispose();
                mappedFileAccessor = null;
            }

            if (mappedFile != null)
            {
                mappedFile.Dispose();
                mappedFile = null;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SharedData
        {
            public uint TimerBarsLastFrame;
            public float TimerBarsTotalHeight;
            public uint NumInstructionalButtonsRowsLastFrame;
            public int NumInstructionalButtonsRows;
            public bool IngamehudScriptExecuting;
            public uint ScreenLastFrame;
            public SizeF ActualScreenResolution;
            public float AspectRatio;
            public uint NumberOfVisibleMenus;
            public uint NumberOfVisiblePauseMenus;
            public fixed long MemoryAddresses[Memory.MaxMemoryAddresses];
            public fixed int MemoryInts[Memory.MaxInts];
            public uint TokenParserHookRefs;
        }
    }
}
