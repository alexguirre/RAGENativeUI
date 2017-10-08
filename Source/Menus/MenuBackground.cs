namespace RAGENativeUI.Menus
{
    using Graphics = Rage.Graphics;

    public class MenuBackground : IDynamicHeightMenuComponent
    {
        public Menu Menu { get; private set; }

        public MenuBackground()
        {
        }

        internal void SetMenu(Menu menu)
        {
            Throw.InvalidOperationIf(Menu != null && Menu != menu, $"{nameof(MenuBackground)} already set to a {nameof(Menus.Menu)}.");
            Throw.IfNull(menu, nameof(menu));

            Menu = menu;
        }

        public float GetHeight() => Menu.Items.GetHeight();

        public virtual void Process()
        {
        }

        public virtual void Draw(Graphics graphics, ref float x, ref float y)
        {
            Menu.Style.DrawBackground(graphics, this, ref x, ref y);
        }
    }
}

