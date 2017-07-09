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
            for (int i = 0; i < 5; i++)
            {
                menu.Items.Add(new MenuItem("item #" + i));
            }
            menu.Items.Add(new MenuItemCheckbox("cb #0") { State = MenuItemCheckboxState.Empty });
            menu.Items.Add(new MenuItemCheckbox("cb #1") { State = MenuItemCheckboxState.Cross });
            menu.Items.Add(new MenuItemCheckbox("cb #2") { State = MenuItemCheckboxState.Tick });

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
                }
            });
        }
    }
}

