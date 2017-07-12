namespace RAGENativeUI.Menus
{
    using System;
    using System.Linq;

    using Rage;

    using RAGENativeUI.Utility;

    public class MenusManager
    {
        private MenusCollection menus;
        /// <exception cref="ArgumentNullException">When setting the property to a null value.</exception>
        public MenusCollection Menus { get { return menus; } set { menus = value ?? throw new ArgumentNullException($"The manager {nameof(Menus)} collection can't be null."); } }
        public bool IsAnyMenuVisible { get { return menus.Any(m => m.IsVisible); } }
        protected internal GameFiber Fiber { get; }

        public MenusManager()
        {
            Menus = new MenusCollection();
            Fiber = GameFiber.StartNew(ProcessLoop);
            Game.RawFrameRender += OnRawFrameRender;
        }

        public void HideAllMenus()
        {
            foreach (Menu m in menus)
            {
                m.IsVisible = false;
            }
        }

        public void ShowAllMenus()
        {
            foreach (Menu m in menus)
            {
                m.IsVisible = true;
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
            for (int i = 0; i < menus.Count; i++)
            {
                menus[i]?.Process();
            }
        }

        protected virtual void OnRawFrameRender(object sender, GraphicsEventArgs e)
        {
            Graphics g = e.Graphics;

            for (int i = 0; i < menus.Count; i++)
            {
                menus[i]?.Draw(g);
            }
        }
    }


    public class MenusCollection : BaseCollection<Menu>
    {
    }
}

