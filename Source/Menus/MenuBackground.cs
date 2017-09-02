namespace RAGENativeUI.Menus
{
    using System.Drawing;
    
    using Graphics = Rage.Graphics;

    public class MenuBackground : IMenuComponent
    {
        public Menu Menu { get; }
        
        public virtual SizeF Size { get { return Menu.Items.Size; } set { Menu.Items.Size = value; } }

        public MenuBackground(Menu menu)
        {
            Menu = menu;
        }

        public virtual void Process()
        {
        }

        public virtual void Draw(Graphics graphics, ref float x, ref float y)
        {
            Menu.Skin.DrawBackground(graphics, this, x, y);
        }
    }
}

