namespace RAGENativeUI.Menus
{
    using Graphics = Rage.Graphics;

    public class MenuBackground : IDynamicHeightMenuComponent
    {
        public Menu Menu { get; }

        public MenuBackground(Menu menu)
        {
            Menu = menu ?? throw new System.ArgumentNullException($"The component {nameof(Menu)} can't be null.");
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

