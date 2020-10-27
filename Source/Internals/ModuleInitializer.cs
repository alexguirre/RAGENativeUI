namespace RAGENativeUI.Internals
{
    using System.Runtime.CompilerServices;
    using Rage;

    internal static class ModuleInitializer
    {
        internal static void Run()
        {
            Game.LogTrivialDebug("[RAGENativeUI] Initializing...");

            Game.LogTrivialDebug($"[RAGENativeUI] > {nameof(Shared)}");
#if DEBUG
            var sw = System.Diagnostics.Stopwatch.StartNew();
#endif
            RuntimeHelpers.RunClassConstructor(typeof(Shared).TypeHandle);
#if DEBUG
            sw.Stop();
            Game.LogTrivialDebug($"[RAGENativeUI] >> Took {sw.ElapsedMilliseconds}ms");
#endif


            Game.LogTrivialDebug($"[RAGENativeUI] > {nameof(Memory)}");
#if DEBUG
            sw.Restart();
#endif
            RuntimeHelpers.RunClassConstructor(typeof(Memory).TypeHandle);
#if DEBUG
            sw.Stop();
            Game.LogTrivialDebug($"[RAGENativeUI] >> Took {sw.ElapsedMilliseconds}ms");
#endif

            Game.LogTrivialDebug("[RAGENativeUI] > Applying hooks");
#if DEBUG
            sw.Restart();
#endif
            Hooks.Init();
#if DEBUG
            sw.Stop();
            Game.LogTrivialDebug($"[RAGENativeUI] >> Took {sw.ElapsedMilliseconds}ms");
#endif

#if DEBUG
            Game.LogTrivialDebug("[RAGENativeUI] > Registering debug commands");
            Game.AddConsoleCommands(new[] { typeof(DebugCommands) });
#endif
        }
    }
}
