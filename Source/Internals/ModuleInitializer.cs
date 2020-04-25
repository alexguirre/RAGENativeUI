namespace RAGENativeUI.Internals
{
    using System.Runtime.CompilerServices;

    internal static class ModuleInitializer
    {
        internal static void Run()
        {
            Rage.Game.LogTrivialDebug("[RAGENativeUI] Initializing...");

            Rage.Game.LogTrivialDebug($"[RAGENativeUI] > {nameof(Functions)}");
            RuntimeHelpers.RunClassConstructor(typeof(Functions).TypeHandle);

            Rage.Game.LogTrivialDebug($"[RAGENativeUI] > {nameof(Variables)}");
            RuntimeHelpers.RunClassConstructor(typeof(Variables).TypeHandle);
        }
    }
}
