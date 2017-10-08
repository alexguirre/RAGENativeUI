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
            Throw.InvalidOperationIf(Menu != null && Menu != menu, $"{nameof(MenuUpDownDisplay)} already set to a {nameof(Menus.Menu)}.");
            Throw.IfNull(menu, nameof(menu));

            Menu = menu;
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

