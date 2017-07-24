namespace RAGENativeUI.Menus
{
    using System.Drawing;
    
    using Graphics = Rage.Graphics;

    public class MenuUpDownDisplay : IMenuComponent
    {
        public Menu Menu { get; }

        public SizeF Size { get; set; } = new SizeF(Menu.DefaultWidth, 38f);
        private float ArrowsUpDownSize { get { return Size.Height; } }
        
        private bool ShouldBeVisible { get { return (Menu.MaxItemsOnScreen < Menu.Items.Count) && Menu.IsAnyItemOnScreen; } }

        public MenuUpDownDisplay(Menu menu)
        {
            Menu = menu;
        }

        public virtual void Process()
        {
        }

        public virtual void Draw(Graphics graphics, ref float x, ref float y)
        {
            if (ShouldBeVisible)
            {
                Menu.Skin.DrawArrowsUpDownBackground(graphics, x, y, Size.Width, Size.Height);
                Menu.Skin.DrawArrowsUpDown(graphics, x - ArrowsUpDownSize / 2f + Size.Width / 2f, y, ArrowsUpDownSize, ArrowsUpDownSize);

                y += Size.Height;
            }
        }
    }
}

