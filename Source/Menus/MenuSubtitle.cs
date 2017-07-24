namespace RAGENativeUI.Menus
{
    using System.Drawing;
    
    using Graphics = Rage.Graphics;

    using RAGENativeUI.Rendering;

    public class MenuSubtitle : IMenuComponent
    {
        public Menu Menu { get; }

        public string Text { get; set; }
        public SizeF Size { get; set; } = new SizeF(Menu.DefaultWidth, 37f);

        protected bool ShowCounter { get { return Menu.IsAnyItemOnScreen && Menu.Items.Count > Menu.MaxItemsOnScreen; } }

        public float BorderSafezone { get; set; } = 8.5f;

        public MenuSubtitle(Menu menu)
        {
            Menu = menu;
        }

        public virtual void Process()
        {
        }

        public virtual void Draw(Graphics graphics, ref float x, ref float y)
        {
            graphics.DrawRectangle(new RectangleF(x, y, Size.Width, Size.Height), Color.Black);
            Menu.Skin.DrawText(graphics, Text, Menu.Skin.SubtitleFont, new RectangleF(x + BorderSafezone, y, Size.Width, Size.Height), Color.White, TextHorizontalAligment.Left, TextVerticalAligment.Center);

            if (ShowCounter)
            {
                Menu.Skin.DrawText(graphics, $"{Menu.SelectedIndex + 1}/{Menu.Items.Count} ", Menu.Skin.SubtitleFont, new RectangleF(x, y, Size.Width, Size.Height), Color.White, TextHorizontalAligment.Right, TextVerticalAligment.Center);
            }

            y += Size.Height;
        }
    }
}

