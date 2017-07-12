namespace Examples
{
    using System.Drawing;

    using Rage;
    using Rage.Attributes;
    using Graphics = Rage.Graphics;

    using RAGENativeUI;
    using RAGENativeUI.Menus;
    using RAGENativeUI.Menus.Rendering;
    using RAGENativeUI.Utility;

    internal static class MenuExample
    {
        [ConsoleCommand(Name = "MenuExample", Description = "Example showing RAGENativeUI menu API")]
        private static void Command()
        {
            MenusManager menusMgr = new MenusManager();

            Menu menu = new Menu("title", "SUBTITLE");
            menu.Location = new PointF(480, 17);

            menu.Items.Add(new MenuItem("item #0"));
            menu.Items.Add(new MenuItemCheckbox("cb #0") { State = MenuItemCheckboxState.Empty });
            menu.Items.Add(new MenuItemCheckbox("cb #1") { State = MenuItemCheckboxState.Cross });
            menu.Items.Add(new MenuItemCheckbox("cb #2") { State = MenuItemCheckboxState.Tick });
            menu.Items.Add(new MenuItemEnumScroller("enum scroller #0", typeof(GameControl)));
            menu.Items.Add(new MenuItemEnumScroller<GameControl>("enum scroller #1"));
            menu.Items.Add(new MenuItemNumericScroller("num scroller #0"));
            menu.Items.Add(new MenuItemNumericScroller("num scroller #1") { ThousandsSeparator = true, Minimum = -50000.0m, Maximum = 50000.0m, Value = 0.0m, Increment = 100.0m });
            menu.Items.Add(new MenuItemNumericScroller("num scroller #2") { Increment = 1.0m, Hexadecimal = true });
            menu.Items.Add(new MenuItemNumericScroller("num scroller #3") { Minimum = -1.0m, Maximum = 1.0m, Value = 0.0m, Increment = 0.00005m, DecimalPlaces = 5 });
            menu.Items.Add(new MenuItemListScroller("list scroller #0"));
            menu.Items.Add(new MenuItemListScroller("list scroller #1", new[] { "text #1", "other text #2", "and text #3" }));

            menusMgr.Menus.Add(menu);

            MenuSkin redSkin = new MenuSkin(@"RAGENativeUI Resources\menu-red-skin.png");
            GameFiber.StartNew(() =>
            {
                while (true)
                {
                    GameFiber.Yield();
                    
                    if(Game.IsKeyDown(System.Windows.Forms.Keys.Y))
                    {
                        if (menu.Skin == MenuSkin.DefaultSkin)
                            menu.Skin = redSkin;
                        else
                            menu.Skin = MenuSkin.DefaultSkin;
                    }

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.T))
                    {
                        if(menusMgr.IsAnyMenuVisible)
                        {
                            menusMgr.HideAllMenus();
                        }
                        else
                        {
                            menusMgr.ShowAllMenus();
                        }
                        //menu.IsVisible = !menu.IsVisible;
                    }
                }
            });
        }
    }
}

