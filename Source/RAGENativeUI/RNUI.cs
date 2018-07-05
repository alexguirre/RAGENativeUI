namespace RAGENativeUI
{
    using System;
    using System.Runtime.InteropServices;

    using RAGENativeUI.Memory;

    internal static class RNUI
    {
        public static class Helper
        {
            [StructLayout(LayoutKind.Sequential, Size = 16)]
            public struct TextureDesc
            {
                public IntPtr Name;
                public uint Width;
                public uint Height;
            };

            public const string DllName = "RAGENativeUI.Helper.dll";

            [DllImport(DllName)] public static extern void Init();
            [DllImport(DllName)] public static extern IntPtr Allocate(long size);
            [DllImport(DllName)] public static extern void Free(IntPtr ptr);

            [DllImport(DllName, CharSet = CharSet.Ansi)]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool DoesTextureDictionaryExist([MarshalAs(UnmanagedType.LPStr)] string name);

            [DllImport(DllName, CharSet = CharSet.Ansi)]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool DoesCustomTextureExist([MarshalAs(UnmanagedType.LPStr)] string name);

            [DllImport(DllName, CharSet = CharSet.Ansi)]
            public static extern uint CreateCustomTexture([MarshalAs(UnmanagedType.LPStr)] string name, uint width, uint height, IntPtr pixelData, [MarshalAs(UnmanagedType.I1)] bool updatable);

            [DllImport(DllName, CharSet = CharSet.Ansi)]
            public static extern uint GetNumberOfTexturesFromDictionary([MarshalAs(UnmanagedType.LPStr)] string name);

            [DllImport(DllName, CharSet = CharSet.Ansi)]
            public static extern void GetTexturesFromDictionary([MarshalAs(UnmanagedType.LPStr)] string name, [In, Out] TextureDesc[] outTextureDescs);
        }

        public static bool IsInitialized { get; private set; }

        public static void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }

            Common.LogDebug("Initializing...");

            AppDomain.CurrentDomain.DomainUnload += Shutdown;

            AssemblyResolver.Initialize();

            Helper.Init();
            bool gameFnInit = GameFunctions.Init();
            bool gameMemInit = GameMemory.Init();

            if (gameFnInit)
                Common.LogDebug($"Successful {nameof(GameFunctions)} init");
            if (gameMemInit)
                Common.LogDebug($"Successful {nameof(GameMemory)} init");

            if (!gameFnInit || !gameMemInit)
            {
                string str = "";
                if (!gameFnInit)
                {
                    str += nameof(GameFunctions);

                    if (!gameMemInit)
                    {
                        str += " and ";
                        str += nameof(GameMemory);
                    }
                }
                else if (!gameMemInit)
                {
                    str += nameof(GameMemory);
                }

                Common.Log($"[ERROR] Failed to initialize {str}");
            }

            Common.LogDebug("Finished initialization");
            IsInitialized = true;
        }

        private static void Shutdown(object sender, EventArgs e)
        {
            Common.LogDebug("Shutting down...");


            Common.LogDebug("Finished shutdown...");
        }
    }
}

