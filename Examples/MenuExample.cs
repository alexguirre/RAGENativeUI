namespace Examples
{
    using System.Drawing;

    using Rage;
    using Rage.Attributes;
    using Graphics = Rage.Graphics;

    using RAGENativeUI;
    using RAGENativeUI.Menus;
    using RAGENativeUI.Menus.Rendering;

    internal static class MenuExample
    {
        [ConsoleCommand(Name = "MenuExample", Description = "Example showing RAGENativeUI menu API")]
        private static void Command()
        {
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

            Game.RawFrameRender += (s, e) =>
            {
                Graphics g = e.Graphics;
                
                menu.Draw(g);
            };

            MenuSkin redSkin = new MenuSkin(@"RAGENativeUI Resources\menu-red-skin.png");
            GameFiber.StartNew(() =>
            {
                while (true)
                {
                    GameFiber.Yield();

                    menu.Process();

                    if(Game.IsKeyDown(System.Windows.Forms.Keys.Y))
                    {
                        if (menu.Skin == MenuSkin.DefaultSkin)
                            menu.Skin = redSkin;
                        else
                            menu.Skin = MenuSkin.DefaultSkin;
                    }

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.T))
                        menu.IsVisible = !menu.IsVisible;
                }
            });
        }
    }
}

