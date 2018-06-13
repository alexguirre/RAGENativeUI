namespace RAGENativeUI.Menus.Themes
{
    public abstract class MenuTheme
    {
        public Menu Menu { get; }

        public MenuTheme(Menu menu)
        {
            Menu = menu;
        }

        public abstract MenuTheme Clone(Menu menu);
        public abstract void Draw(Rage.Graphics g);
    }
}
