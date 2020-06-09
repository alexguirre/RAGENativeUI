namespace RNUIExamples
{
    using System.Windows.Forms;
    using Rage;
    using RAGENativeUI;
    using RAGENativeUI.Elements;

    internal static class Plugin
    {
        public const string MenuTitle = "RAGENativeUI";

        public static MenuPool Pool { get; } = new MenuPool();
        private static UIMenu MainMenu { get; set; }

        private static void Main()
        {
            MainMenu = CreateMenu("SHOWCASE");

            {
                UIMenuItem item = new UIMenuItem("Menu Items", "Showcases the available menu items");

                MainMenu.AddItem(item);
                MainMenu.BindMenuToItem(CreateMenuItemsShowcaseMenu(), item);
            }

            {
                UIMenuItem item = new UIMenuItem("Timer Bars", "Showcases the available timer bars");

                MainMenu.AddItem(item);
                MainMenu.BindMenuToItem(CreateTimerBarsShowcaseMenu(), item);
            }

            while (true)
            {
                GameFiber.Yield();

                if (Game.IsKeyDown(Keys.F5))
                {
                    MainMenu.Visible = !MainMenu.Visible;
                }

                Pool.ProcessMenus();
            }
        }

        private static UIMenu CreateMenu(string subtitle)
        {
            UIMenu m = new UIMenu(MenuTitle, subtitle);
            Pool.Add(m);
            return m;
        }

        private static UIMenu CreateMenuItemsShowcaseMenu()
        {
            UIMenu m = new UIMenu(MenuTitle, "MENU ITEMS");
            Pool.Add(m);
            return m;
        }

        private static UIMenu CreateTimerBarsShowcaseMenu() => new TimerBars();
    }
}
