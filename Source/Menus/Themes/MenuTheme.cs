namespace RAGENativeUI.Menus.Themes
{
    public abstract class MenuTheme
    {
        public Menu Menu { get; }
        
        public MenuTheme(Menu menu) // TODO: document that all MenuThemes need a constructor with this same signature for Menu.SetTheme to work
        {
            Menu = menu;
        }

        public abstract MenuTheme Clone(Menu menu);
        public abstract void Draw(Rage.Graphics g);
    }
}
