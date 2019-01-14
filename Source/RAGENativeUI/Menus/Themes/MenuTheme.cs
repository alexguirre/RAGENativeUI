namespace RAGENativeUI.Menus.Themes
{
#if RPH1
    extern alias rph1;
    using Graphics = rph1::Rage.Graphics;
#else
    /** REDACTED **/
#endif

    public abstract class MenuTheme
    {
        public Menu Menu { get; }
        
        public MenuTheme(Menu menu) // TODO: document that all MenuThemes need a constructor with this same signature for Menu.SetTheme to work
        {
            Menu = menu;
        }

        public abstract MenuTheme Clone(Menu menu);
        public abstract void Draw(Graphics g);
    }
}
