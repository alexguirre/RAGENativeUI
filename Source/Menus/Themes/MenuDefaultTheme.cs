namespace RAGENativeUI.Menus.Themes
{
    using System.ComponentModel;
    using Rage;

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

