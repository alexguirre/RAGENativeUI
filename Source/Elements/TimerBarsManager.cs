namespace RAGENativeUI.Elements
{
    using System;
    using System.Linq;

    using Rage;
    using Rage.Native;

    using RAGENativeUI.Utility;

    public class TimerBarsManager
    {
        private TimerBarsCollection timerBars;
        /// <exception cref="ArgumentNullException">When setting the property to a null value.</exception>
        public TimerBarsCollection TimerBars { get { return timerBars; } set { timerBars = value ?? throw new ArgumentNullException($"The manager {nameof(TimerBars)} collection can't be null."); } }
        public bool IsAnyTimerBarVisible { get { return timerBars.Any(m => m.IsVisible); } }
        /// <summary>
        /// Gets or sets the Y-coordinate where the first timer bar is drawn.
        /// </summary>
        public float InitialY { get; } = 0.965f; // TODO: implement safezone in TimerBars
        protected internal GameFiber Fiber { get; }

        public TimerBarsManager()
        {
            TimerBars = new TimerBarsCollection();
            Fiber = GameFiber.StartNew(ProcessLoop);
        }

        public void HideAllTimerBars()
        {
            foreach (TimerBarBase t in timerBars)
            {
                t.IsVisible = false;
            }
        }

        public void ShowAllTimerBars()
        {
            foreach (TimerBarBase t in timerBars)
            {
                t.IsVisible = true;
            }
        }

        private void ProcessLoop()
        {
            while (true)
            {
                GameFiber.Yield();

                OnProcessTimerBars();
            }
        }

        protected virtual void OnProcessTimerBars()
        {
            float y = InitialY;
            for (int i = 0; i < timerBars.Count; i++)
            {
                timerBars[i]?.Draw(ref y);
            }
        }
    }

    public class TimerBarsCollection : BaseCollection<TimerBarBase>
    {
    }
}

