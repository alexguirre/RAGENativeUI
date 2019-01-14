namespace RAGENativeUI.Menus.Themes
{
#if RPH1
    extern alias rph1;
    using Graphics = rph1::Rage.Graphics;
#else
    /** REDACTED **/
#endif

    using System.ComponentModel;
    
    public class MenuDefaultTheme : MenuTheme
    {
        public MenuDefaultTheme(Menu menu) : base(menu)
        {
        }

        public override MenuTheme Clone(Menu menu)
        {
            return new MenuDefaultTheme(menu)
            {
            };
        }

        public override void Draw(Graphics g)
        {
        }

        private void FormatDescription()
        {
        }

        private void OnMenuPropertyChanged(PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Menu.CurrentDescription):
                    {
                        FormatDescription();
                        break;
                    }
            }
        }
    }
}

