using RAGENativeUI;


internal static class ModuleInitializer
{
    // This code executes after the assembly is loaded, before any other code executes, use for initialization stuff.
    // The actual module initializer is created with the InjectModuleInitializer.exe executed in the PostBuild event
    internal static void Run()
    {
        RNUI.Initialize();
    }
}

