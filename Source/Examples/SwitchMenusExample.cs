namespace RNUIExamples
{
    using System.Linq;
    using Rage;
    using Rage.Attributes;
    using RAGENativeUI;
    using RAGENativeUI.Elements;
    using RAGENativeUI.PauseMenu;

    internal static class SwitchMenusExample
    {
        private static MenuPool pool;
        private static UIMenuSwitchMenusItem menuSwitcher;

        private static void Main()
        {
            // create the pool that handles drawing and processing the menus
            pool = new MenuPool();

            // create the menus
            UIMenu menu1 = CreateMenu("First");
            UIMenu menu2 = CreateMenu("Second");
            UIMenu menu3 = CreateMenu("Third");
            UIMenu menu4 = CreateMenu("Fourth");
            UIMenu menu5 = CreateMenu("Fifth");

            // create the item that will handle switching between the menus
            menuSwitcher = new UIMenuSwitchMenusItem("Menu", "", new DisplayItem(menu1, "The First"),
                                                                 new DisplayItem(menu2, "The Second"),
                                                                 new DisplayItem(menu3, "The Third"),
                                                                 new DisplayItem(menu4, "The Fourth"),
                                                                 new DisplayItem(menu5, "The Fifth"));

            // insert the menu switcher item as the first item of all the menus
            menu1.AddItem(menuSwitcher, 0);
            menu2.AddItem(menuSwitcher, 0);
            menu3.AddItem(menuSwitcher, 0);
            menu4.AddItem(menuSwitcher, 0);
            menu5.AddItem(menuSwitcher, 0);

            // update the description when the selected menu changes
            menuSwitcher.OnListChanged += (s, i) => menuSwitcher.Description = i switch
            {
                0 => "This is the first menu.",
                1 => "This is the second menu.",
                2 => "This is the third menu.",
                3 => "This is the fourth menu.",
                4 => "This is the fifth menu.",
                _ => ""
            };

            // add all the menus to the pool
            pool.Add(menu1, menu2, menu3, menu4, menu5);

            // start the fiber which will handle drawing and processing the menus
            GameFiber.StartNew(ProcessMenus);

            // continue with the plugin...
            Game.Console.Print("  Press F8 to open the menu.");
        }

        private static void ProcessMenus()
        {
            // draw the menu banners (only needed if UIMenu.SetBannerType(Rage.Texture) is used)
            // Game.RawFrameRender += (s, e) => pool.DrawBanners(e.Graphics);

            while (true)
            {
                GameFiber.Yield();

                pool.ProcessMenus();

                if (Game.IsKeyDown(System.Windows.Forms.Keys.F8) && !UIMenu.IsAnyMenuVisible && !TabView.IsAnyPauseMenuVisible)
                {
                    menuSwitcher.CurrentMenu.Visible = true;
                }
            }
        }

        private static UIMenu CreateMenu(string name)
        {
            // create a menu with random items
            var menu = new UIMenu("Menu Switcher", name);

            int count = MathHelper.GetRandomInteger(5, 15);
            menu.AddItems(Enumerable.Range(0, count)
                            .Select(i => MathHelper.GetRandomInteger(0, 4) switch
                            {
                                1 => new UIMenuCheckboxItem($"Checkbox #{i}", MathHelper.GetChance(2), RandomDescription()),
                                2 => new UIMenuNumericScrollerItem<int>($"Slider #{i}", RandomDescription(), 0, 10, 1) { SliderBar = new UIMenuScrollerSliderBar() },
                                3 => new UIMenuListScrollerItem<string>($"List #{i}", RandomDescription(), Enumerable.Range(0, 10).Select(i => RandomString(8))),
                                _ => new UIMenuItem($"Item #{i}", RandomDescription()),
                            }));

            return menu;

            static string RandomDescription() => MathHelper.GetChance(5) ? RandomString(150) : "";
            static string RandomString(int maxLength) =>
                new string(Enumerable.Range(0, MathHelper.GetRandomInteger(1, maxLength))
                          .Select(i => MathHelper.GetChance(25) ? ' ' : MathHelper.GetRandomAlphaNumericCharacter())
                          .ToArray());
        }


        // a command that simulates loading the plugin
        [ConsoleCommand]
        private static void RunSwitchMenusExample() => GameFiber.StartNew(Main);
    }
}
