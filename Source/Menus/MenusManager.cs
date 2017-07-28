namespace RAGENativeUI.Menus
{
    using System;
    using System.Linq;

    using Rage;

    using RAGENativeUI.Utility;

    public class MenusManager : IDisposable
    {
        private MenusCollection menus;
        /// <exception cref="ArgumentNullException">When setting the property to a null value.</exception>
        public MenusCollection Menus { get { return IsDisposed ? throw Common.NewDisposedException() : menus; } set { menus = IsDisposed ? throw Common.NewDisposedException() : value ?? throw new ArgumentNullException($"The manager {nameof(Menus)} collection can't be null."); } }
        public bool IsAnyMenuVisible { get { return IsDisposed ? throw Common.NewDisposedException() : menus.Any(m => m.IsVisible); } }
        protected internal GameFiber Fiber { get; private set; }
        public bool IsDisposed { get; private set; }

        public MenusManager()
        {
            Menus = new MenusCollection();
            Fiber = GameFiber.StartNew(ProcessLoop);
            Game.RawFrameRender += OnRawFrameRender;
        }

        public void HideAllMenus()
        {
            if (IsDisposed)
                throw Common.NewDisposedException();

            foreach (Menu m in menus)
            {
                m.Hide(false);
            }
        }

        public void ShowAllMenus()
        {
            if (IsDisposed)
                throw Common.NewDisposedException();

            foreach (Menu m in menus)
            {
                m.Show();
            }
        }

        private void ProcessLoop()
        {
            while (true)
            {
                GameFiber.Yield();

                OnProcessMenus();
            }
        }

        protected virtual void OnProcessMenus()
        {
            if (IsDisposed)
                throw Common.NewDisposedException();

            for (int i = 0; i < menus.Count; i++)
            {
                menus[i]?.Process();
            }
        }

        protected virtual void OnRawFrameRender(object sender, GraphicsEventArgs e)
        {
            if (IsDisposed)
                throw Common.NewDisposedException();

            Graphics g = e.Graphics;

            for (int i = 0; i < menus.Count; i++)
            {
                menus[i]?.Draw(g);
            }
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                Game.RawFrameRender -= OnRawFrameRender;
                Fiber?.Abort();
                Fiber = null;
                menus = null;
                IsDisposed = true;
            }
        }
    }


    public class MenusCollection : BaseCollection<Menu>
    {
    }
}

