namespace RAGENativeUI.Menus
{
    using Graphics = Rage.Graphics;

    public class MenuBanner : IMenuComponent
    {
        public Menu Menu { get; private set; }
        public virtual string Title { get; set; }

        public MenuBanner(string title)
        {
            Title = title;
        }

        public MenuBanner() : this(null)
        {
        }

        internal void SetMenu(Menu menu)
        {
            Throw.InvalidOperationIf(Menu != null && Menu != menu, $"{nameof(MenuBanner)} already set to a {nameof(Menus.Menu)}.");
            Throw.IfNull(menu, nameof(menu));

            Menu = menu;
        }

        public virtual void Process()
        {
        }

        public virtual void Draw(Graphics graphics, ref float x, ref float y)
        {
            Menu.Style.DrawBanner(graphics, this, ref x, ref y);
        }
    }
}

