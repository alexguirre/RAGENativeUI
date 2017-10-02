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
            if (Menu != null && Menu != menu)
                throw new System.InvalidOperationException($"{nameof(MenuBackground)} already set to a {nameof(Menus.Menu)}.");
            Menu = menu ?? throw new System.ArgumentNullException($"The {nameof(MenuBackground)} {nameof(Menu)} can't be null.");
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

