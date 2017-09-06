namespace RAGENativeUI.Menus
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    using Rage;

    internal static class MenusManager
    {
        private static readonly List<Menu> menus;
        private static readonly GameFiber processFiber;

        public static bool IsAnyMenuVisible { get { return menus.Any(m => m.IsVisible); } }

        static MenusManager()
        {
            menus = new List<Menu>();
            processFiber = GameFiber.StartNew(ProcessLoop, "RAGENativeUI - Menus Manager");
            Game.RawFrameRender += OnRawFrameRender;
        }

        public static void AddMenu(Menu menu)
        {
            menus.Add(menu);
        }

        public static void RemoveMenu(Menu menu)
        {
            menus.Remove(menu);
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
            for (int i = 0; i < menus.Count; i++)
            {
                menus[i]?.Process();
            }
        }

        private static void OnRawFrameRender(object sender, GraphicsEventArgs e)
        {
            Graphics g = e.Graphics;

            for (int i = 0; i < menus.Count; i++)
            {
                menus[i]?.Draw(g);
            }
        }
    }
}

