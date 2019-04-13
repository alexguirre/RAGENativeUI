namespace RAGENativeUI.Menus.Templating
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public abstract class MenuTemplate : INotifyPropertyChanged
    {
        private Menu menu;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsMenuBuilt => Menu != null;
        public Menu Menu
        {
            get => menu;
            private set => SetProperty(ref menu, value, nameof(Menu), nameof(IsMenuBuilt));
        }

        public MenuItem GetItemByName(string name)
        {
            if (!IsMenuBuilt)
            {
                return null;
            }

            return MenuTemplateBuilder.RetrieveItemFromMetadata(Menu, name);
        }

        protected virtual void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null, params string[] additionalProperties)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
                foreach (string additionalProp in additionalProperties)
                {
                    OnPropertyChanged(additionalProp);
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void BuildMenu()
        {
            Menu = MenuTemplateBuilder.BuildMenu(this);
            OnMenuBuilt();
        }

        protected virtual void OnMenuBuilt()
        {
        }
    }
}

