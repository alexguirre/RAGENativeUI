namespace RAGENativeUI.Menus
{
    using Graphics = Rage.Graphics;

    public class MenuBanner : IMenuComponent
    {
        public Menu Menu { get; }
        public virtual string Title { get; set; }

        public MenuBanner(Menu menu, string title)
        {
            Menu = menu ?? throw new System.ArgumentNullException($"The component {nameof(Menu)} can't be null.");
            Title = title;
        }

        public MenuBanner(Menu menu) : this(menu, null)
        {
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

