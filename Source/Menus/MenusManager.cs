namespace RAGENativeUI.Menus
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    using Rage;

    internal static class MenusManager
    {
        private static readonly List<Menu> menus;
        private static readonly List<Menu> visibleMenus;
        private static readonly GameFiber processFiber;

        public static bool IsAnyMenuVisible { get { return visibleMenus.Count > 0; } }

        static MenusManager()
        {
            menus = new List<Menu>();
            visibleMenus = new List<Menu>();
            processFiber = GameFiber.StartNew(ProcessLoop, "RAGENativeUI - Menus Manager");
            Game.RawFrameRender += OnRawFrameRender;
        }

        public static void AddMenu(Menu menu)
        {
            menu.VisibleChanged += OnMenuVisibleChanged;
            menus.Add(menu);
        }

        public static void RemoveMenu(Menu menu)
        {
            menu.VisibleChanged -= OnMenuVisibleChanged;
            menus.Remove(menu);
        }

        private static void OnMenuVisibleChanged(Menu menu, bool visible)
        {
            if (visible)
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
                visibleMenus[i]?.Process();
            }
        }

        private static void OnRawFrameRender(object sender, GraphicsEventArgs e)
        {
            Graphics g = e.Graphics;

            for (int i = 0; i < visibleMenus.Count; i++)
            {
                visibleMenus[i]?.Draw(g);
            }
        }
    }
}

