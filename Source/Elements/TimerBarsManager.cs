namespace RAGENativeUI.Elements
{
    using System;
    using System.Linq;

    using Rage;
    using Rage.Native;

    public class TimerBarsManager : IDisposable
    {
        private TimerBarsCollection timerBars;

        /// <exception cref="ArgumentNullException">When setting the property to a null value.</exception>
        public TimerBarsCollection TimerBars { get { return IsDisposed ? throw Common.NewDisposedException() : timerBars; } set { timerBars = IsDisposed ? throw Common.NewDisposedException() : value ?? throw new ArgumentNullException($"The manager {nameof(TimerBars)} collection can't be null."); } }
        public bool IsAnyTimerBarVisible { get { return IsDisposed ? throw Common.NewDisposedException() : timerBars.Any(m => m.IsVisible); } }
        protected internal GameFiber Fiber { get; private set; }
        public bool IsDisposed { get; private set; }

        public TimerBarsManager()
        {
            TimerBars = new TimerBarsCollection();
            Fiber = GameFiber.StartNew(ProcessLoop);
        }

        public void HideAllTimerBars()
        {
            if (IsDisposed)
                throw Common.NewDisposedException();

            foreach (TimerBarBase t in timerBars)
            {
                t.IsVisible = false;
            }
        }

        public void ShowAllTimerBars()
        {
            if (IsDisposed)
                throw Common.NewDisposedException();

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
            if (IsDisposed)
                throw Common.NewDisposedException();
            
            float x = 0.5f + (NativeFunction.Natives.GetSafeZoneSize<float>() / 2f); // safezone bottom-right corner
            float y = x;
            for (int i = 0; i < timerBars.Count; i++)
            {
                timerBars[i]?.Draw(ref y, x);
            }
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                Fiber?.Abort();
                Fiber = null;
                timerBars = null;
                IsDisposed = true;
            }
        }
    }

    public class TimerBarsCollection : BaseCollection<TimerBarBase>
    {
    }
}

