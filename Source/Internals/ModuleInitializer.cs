namespace RAGENativeUI.Internals
{
    using System.Runtime.CompilerServices;
    using Rage;

    internal static class ModuleInitializer
    {
        internal static void Run()
        {
            Game.LogTrivialDebug("[RAGENativeUI] Initializing...");

            Game.LogTrivialDebug($"[RAGENativeUI] > {nameof(Memory)}");
            RuntimeHelpers.RunClassConstructor(typeof(Memory).TypeHandle);

#if DEBUG
            Game.LogTrivialDebug("[RAGENativeUI] > Registering debug commands");
            Game.AddConsoleCommands(new[] { typeof(DebugCommands) });
#endif
        }
    }
}
