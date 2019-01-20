namespace RAGENativeUI.TimerBars
{
#if RPH1
    extern alias rph1;
    using GameFiber = rph1::Rage.GameFiber;
#else
    /** REDACTED **/
#endif

    using System.Collections.Generic;

    // TODO: how can the user specify the timerbars order
    // currently they are ordered in the order of construction
    // which is not very good nor obvious

    internal static class TimerBarManager
    {
        private static readonly List<TimerBar> timerBars = new List<TimerBar>();
        private static GameFiber processFiber;

        public static bool IsProcessRunning => processFiber != null && processFiber.IsAlive;

        public static void AddTimerBar(TimerBar timerBar)
        {
            timerBars.Add(timerBar);

            if (!IsProcessRunning)
            {
                StartProcess();
            }
        }

        public static void RemoveTimerBar(TimerBar timerBar)
        {
            timerBars.Remove(timerBar);

            if (timerBars.Count <= 0 && IsProcessRunning)
            {
                AbortProcess();
            }
        }

        private static void StartProcess()
        {
            processFiber = GameFiber.StartNew(ProcessLoop, $"RAGENativeUI - {nameof(TimerBarManager)}");
        }

        private static void AbortProcess()
        {
            processFiber?.Abort();
            processFiber = null;
        }

        private static void ProcessLoop()
        {
            while (true)
            {
                GameFiber.Yield();

                OnProcess();
            }
        }

        private static void OnProcess()
        {
            if (timerBars.Count > 0)
            {
                bool anyVisible = false;
                int visiblePos = 0;
                for (int i = 0; i < timerBars.Count; i++)
                {
                    TimerBar b = timerBars[i];
                    if (b != null && b.IsVisible)
                    {
                        if (!anyVisible)
                        {
                            anyVisible = true;
                            N.SetScriptGfxAlign('R', 'B');
                            N.SetScriptGfxAlignParams(0.0f, 0.0f, 0.952f, 0.949f);
                        }

                        b.Draw(visiblePos++);
                    }
                }

                if (anyVisible)
                {
                    N.ResetScriptGfxAlign();

                    N.HideHudComponentThisFrame(6); // VehicleName
                    N.HideHudComponentThisFrame(7); // AreaName
                    N.HideHudComponentThisFrame(8); // ?
                    N.HideHudComponentThisFrame(9); // StreetName
                }
            }
        }
    }
}

