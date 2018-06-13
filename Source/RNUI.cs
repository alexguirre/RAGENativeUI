namespace RAGENativeUI
{
    using System;

    using RAGENativeUI.Memory;

    internal static class RNUI
    {
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

