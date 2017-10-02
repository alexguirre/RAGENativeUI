namespace RAGENativeUI.Menus
{
    using Graphics = Rage.Graphics;

    public class MenuUpDownDisplay : IMenuComponent
    {
        public Menu Menu { get; private set; }
        
        public MenuUpDownDisplay()
        {
        }

        internal void SetMenu(Menu menu)
        {
            if (Menu != null && Menu != menu)
                throw new System.InvalidOperationException($"{nameof(MenuUpDownDisplay)} already set to a {nameof(Menus.Menu)}.");
            Menu = menu ?? throw new System.ArgumentNullException($"The {nameof(MenuUpDownDisplay)} {nameof(Menu)} can't be null.");
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

