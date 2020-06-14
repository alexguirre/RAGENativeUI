namespace RNUIExamples
{
    using System.Windows.Forms;
    using Rage;
    using RAGENativeUI;

    internal static class Plugin
    {
        public const string MenuTitle = "RAGENativeUI";

        public static MenuPool Pool { get; } = new MenuPool();
        private static UIMenu ShowcaseMenu { get; set; }

        private static void Main()
        {
            ShowcaseMenu = new Showcase.MainMenu();

            // draw custom texture banners
            Game.RawFrameRender += (s, e) => Pool.DrawBanners(e.Graphics);

            while (true)
            {
                GameFiber.Yield();

                if (Game.IsKeyDown(Keys.F5) && !UIMenu.IsAnyMenuVisible)
                {
                    ShowcaseMenu.Visible = true;
                }

                // process input and draw the visible menus
                Pool.ProcessMenus();
            }
        }
    }
}
