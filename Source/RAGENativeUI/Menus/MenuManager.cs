namespace RAGENativeUI.Menus
{
#if RPH1
    extern alias rph1;
    using GameFiber = rph1::Rage.GameFiber;
    using Game = rph1::Rage.Game;
    using GraphicsEventArgs = rph1::Rage.GraphicsEventArgs;
    using Graphics = rph1::Rage.Graphics;
#else
    /** REDACTED **/
#endif

    using System.Collections.Generic;

    using Rage;

    internal static class MenuManager
    {
        private static readonly List<Menu> menus = new List<Menu>();
        private static readonly List<Menu> visibleMenus = new List<Menu>();
        private static GameFiber processFiber;

        public static bool IsProcessRunning => processFiber != null && processFiber.IsAlive;
        public static bool IsAnyMenuVisible => visibleMenus.Count > 0;

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
            processFiber = GameFiber.StartNew(ProcessLoop, $"RAGENativeUI - {nameof(MenuManager)}");
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
                visibleMenus[i]?.Theme.Draw(g);
            }
        }
    }
}

