using RAGENativeUI;
using RAGENativeUI.Memory;

internal static class ModuleInitializer
{
    // This code executes after the assembly is loaded, before any other code executes, use for initialization stuff.
    // The actual module initializer is created with the InjectModuleInitializer.exe executed in the PostBuild event
    internal static void Run()
    {
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
    }
}

