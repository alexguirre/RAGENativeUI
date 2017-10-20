namespace RAGENativeUI.Menus
{
    using System.Collections.Generic;

    using Rage;

    internal static class MenusManager
    {
        private static readonly List<Menu> menus;
        private static readonly List<Menu> visibleMenus;
        private static GameFiber processFiber;

        public static bool IsProcessRunning => processFiber != null && processFiber.IsAlive;
        public static bool IsAnyMenuVisible => visibleMenus.Count > 0;

        static MenusManager()
        {
            menus = new List<Menu>();
            visibleMenus = new List<Menu>();
        }

        public static void AddMenu(Menu menu)
        {
            menu.VisibleChanged += OnMenuVisibleChanged;
            menus.Add(menu);

            if (!IsProcessRunning)
            {
                StartProcess();
            }
        }

        public static void RemoveMenu(Menu menu)
        {
            menu.VisibleChanged -= OnMenuVisibleChanged;
            menus.Remove(menu);

            if (menus.Count <= 0 && IsProcessRunning)
            {
                AbortProcess();
            }
        }

        private static void StartProcess()
        {
            processFiber = GameFiber.StartNew(ProcessLoop, "RAGENativeUI - Menus Manager");
            Game.FrameRender += OnFrameRender;
        }

        private static void AbortProcess()
        {
            processFiber?.Abort();
            processFiber = null;
            Game.FrameRender -= OnFrameRender;
        }

        private static void OnMenuVisibleChanged(Menu menu, VisibleChangedEventArgs e)
        {
            if (e.IsVisible)
            {
                visibleMenus.Add(menu);
            }
            else
            {
                visibleMenus.Remove(menu);
            }
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
            for (int i = 0; i < visibleMenus.Count; i++)
            {
                visibleMenus[i]?.OnProcess();
            }
        }

        private static void OnFrameRender(object sender, GraphicsEventArgs e)
        {
            Graphics g = e.Graphics;

            for (int i = 0; i < visibleMenus.Count; i++)
            {
                visibleMenus[i]?.OnDraw(g);
            }
        }
    }
}

