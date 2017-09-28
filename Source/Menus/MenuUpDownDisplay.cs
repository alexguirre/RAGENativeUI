namespace RAGENativeUI.Menus
{
    using Graphics = Rage.Graphics;

    public class MenuUpDownDisplay : IMenuComponent
    {
        public Menu Menu { get; }
        
        public MenuUpDownDisplay(Menu menu)
        {
            Menu = menu ?? throw new System.ArgumentNullException($"The component {nameof(Menu)} can't be null.");
        }

        public virtual void Process()
        {
        }
        
        public virtual void Draw(Graphics graphics, ref float x, ref float y)
        {
            Menu.Style.DrawUpDownDisplay(graphics, this, ref x, ref y);
        }
    }
}

