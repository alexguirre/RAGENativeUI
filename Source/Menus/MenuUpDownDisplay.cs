namespace RAGENativeUI.Menus
{
    using System.Linq;
    using System.Drawing;
    
    using Graphics = Rage.Graphics;

    public class MenuUpDownDisplay : IMenuComponent
    {
        public Menu Menu { get; }

        public virtual SizeF Size { get; set; } = new SizeF(Menu.DefaultWidth, 38f);
        
        public MenuUpDownDisplay(Menu menu)
        {
            Menu = menu;
        }

        public virtual void Process()
        {
        }

        private bool ShouldBeVisible() => Menu.IsAnyItemOnScreen && Menu.GetOnScreenItemsCount() < Menu.Items.Sum(i => i.IsVisible ? 1 : 0);

        public virtual void Draw(Graphics graphics, ref float x, ref float y)
        {
            if (ShouldBeVisible())
            {
                Menu.Skin.DrawUpDownDisplay(graphics, this, x, y);
                y += Size.Height;
            }
        }
    }
}

