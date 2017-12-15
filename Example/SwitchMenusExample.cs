[assembly: Rage.Attributes.Plugin("SwitchMenusExample", Author = "alexguirre", Description = "Example using the UIMenuSwitchMenusItem class")]

namespace SwitchMenusExampleProject
{
    using System.Windows.Forms;

    using Rage;

    using RAGENativeUI;
    using RAGENativeUI.Elements;

    public static class EntryPoint
    {
        private static GameFiber menusProcessFiber;

        private static UIMenuSwitchMenusItem menuSwitcher;

        private static UIMenu menu1;
        private static UIMenu menu2;
        private static UIMenu menu3;
        private static UIMenu menu4;

        private static MenuPool menuPool;

        public static void Main()
        {
            // Create a fiber to process our menus
            menusProcessFiber = new GameFiber(ProcessLoop);

            // Create the MenuPool to easily process our menus
            menuPool = new MenuPool();

            // Create our menus
            menu1 = CreateMenu("First");
            menu2 = CreateMenu("Second");
            menu3 = CreateMenu("Third");
            menu4 = CreateMenu("Fourth");

            // Create the menu switcher
            menuSwitcher = new UIMenuSwitchMenusItem("Menu", "", new DisplayItem(menu1, "The First"),
                                                                 new DisplayItem(menu2, "The Second"),
                                                                 new DisplayItem(menu3, "The Third"),
                                                                 new DisplayItem(menu4, "The Fourth"));

            // We use the DisplayItem class to specify the custom text that will appear in the menu switcher;
            // we can pass the menus directly too and it will use the menu title text instead:
            //
            // menuSwitcher = new UIMenuSwitchMenusItem("Menu", "", menu1, menu2, menu3, menu4);
            //

            // Add the menu switcher to the menus
            menu1.AddItem(menuSwitcher, 0);
            menu2.AddItem(menuSwitcher, 0);
            menu3.AddItem(menuSwitcher, 0);
            menu4.AddItem(menuSwitcher, 0);


            menu1.RefreshIndex();
            menu2.RefreshIndex();
            menu3.RefreshIndex();
            menu4.RefreshIndex();

            // Temporal fix to prevent some flickering that happens occasionally when switching menus
            menuSwitcher.OnListChanged += (s, i) => { menuSwitcher.CurrentMenu.Draw(); };

            // Start our process fiber
            menusProcessFiber.Start();


            // Continue with our plugin... in this example, hibernate to prevent it from being unloaded
            GameFiber.Hibernate();
        }
        
        // The method that contains a loop to handle our menus
        public static void ProcessLoop()
        {
            // if we are using banners with a Rage.Texture (UIMenu.SetBannerType(...)), we need to draw them in the RawFrameRender, in this example we don't have a Rage.Texture banner so this isn't needed
            //Game.RawFrameRender += (s, e) => 
            //{
            //    _menuPool.DrawBanners(e.Graphics);
            //};

            while (true)
            {
                GameFiber.Yield();

                if (Game.IsKeyDown(Keys.F5) && !menuPool.IsAnyMenuOpen()) // Our menu on/off switch.
                {
                    menuSwitcher.CurrentMenu.Visible = !menuSwitcher.CurrentMenu.Visible;
                }
                
                menuPool.ProcessMenus();       // Process all our menus: draw the menu and process the key strokes and the mouse.
            }
        }

        private static UIMenu CreateMenu(string name)
        {
            UIMenu m = new UIMenu("Switch Example", name);
            int n = MathHelper.GetRandomInteger(2, 10);
            for (int i = 0; i < n; i++)
            {
                m.AddItem(new UIMenuItem("Item #" + i));
            }

            menuPool.Add(m);
            return m;
        }
    }
}


