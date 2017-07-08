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

            Game.RawFrameRender += (s, e) =>
            {
                Graphics g = e.Graphics;

                menu.Draw(g);
            };

            GameFiber.StartNew(() =>
            {
                while (true)
                {
                    GameFiber.Yield();

                    menu.ProcessInput();
                }
            });
        }
    }
}

