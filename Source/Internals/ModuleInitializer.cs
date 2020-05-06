namespace RAGENativeUI.Internals
{
    using System.Runtime.CompilerServices;

    internal static class ModuleInitializer
    {
        internal static void Run()
        {
            Rage.Game.LogTrivialDebug("[RAGENativeUI] Initializing...");

            Rage.Game.LogTrivialDebug($"[RAGENativeUI] > {nameof(Memory)}");
            RuntimeHelpers.RunClassConstructor(typeof(Memory).TypeHandle);
        }
    }
}
