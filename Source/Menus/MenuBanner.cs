namespace RAGENativeUI.Menus
{
    using System.Drawing;
    
    using Graphics = Rage.Graphics;

    public class MenuBanner : IMenuComponent
    {
        public Menu Menu { get; }

        public virtual string Title { get; set; }
        public virtual SizeF Size { get; set; } = new SizeF(Menu.DefaultWidth, 109f);

        public MenuBanner(Menu menu)
        {
            Menu = menu;
        }

        public virtual void Process()
        {
        }

        public virtual void Draw(Graphics graphics, ref float x, ref float y)
        {
            Menu.Skin.DrawBanner(graphics, this, x, y);
            y += Size.Height;
        }
    }
}

