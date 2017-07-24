namespace RAGENativeUI.Menus
{
    using System.Drawing;
    
    using Graphics = Rage.Graphics;

    using RAGENativeUI.Rendering;

    public class MenuBanner : IMenuComponent
    {
        public Menu Menu { get; }

        public string Title { get; set; }
        public SizeF Size { get; set; } = new SizeF(Menu.DefaultWidth, 109f);

        public MenuBanner(Menu menu)
        {
            Menu = menu;
        }

        public virtual void Process()
        {
        }

        public virtual void Draw(Graphics graphics, ref float x, ref float y)
        {
            Menu.Skin.DrawBanner(graphics, x, y, Size.Width, Size.Height);
            Menu.Skin.DrawText(graphics, Title, Menu.Skin.TitleFont, new RectangleF(x, y, Size.Width, Size.Height), Color.White, TextHorizontalAligment.Center, TextVerticalAligment.Center);

            y += Size.Height;
        }
    }
}

