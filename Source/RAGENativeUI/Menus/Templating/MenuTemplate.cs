namespace RAGENativeUI.Menus.Templating
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public abstract class MenuTemplate : INotifyPropertyChanged
    {
        private const BindingFlags PropertiesFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private const string MenuTemplateItemsKey = "__TemplateItems__"; 

        private Menu menu;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsMenuBuilt => Menu != null;
        public Menu Menu
        {
            get => menu;
            private set => SetProperty(ref menu, value, nameof(Menu), nameof(IsMenuBuilt));
        }

        public MenuTemplate()
        {
        }

        public MenuItem GetItemByName(string name)
        {
            if (!IsMenuBuilt)
            {
                return null;
            }

            if (Menu.Metadata.TryGetValue<IDictionary<string, MenuItem>>(MenuTemplateItemsKey, out IDictionary<string, MenuItem> dict))
            {
                if (dict.TryGetValue(name, out MenuItem item))
                {
                    return item;
                }
            }

            return null;
        }

        private void StoreItemInMetadata(Menu menu, MenuItem item, string name)
        {
            if (!menu.Metadata.TryGetValue<IDictionary<string, MenuItem>>(MenuTemplateItemsKey, out IDictionary<string, MenuItem> dict))
            {
                dict = new Dictionary<string, MenuItem>();
                menu.Metadata[MenuTemplateItemsKey] = dict;
            }

            dict.Add(name, item);
        }

        protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null, params string[] additionalProperties)
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

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnTemplatePropertyChanged(propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void BuildMenu()
        {
            Type classType = this.GetType();

            MenuAttribute menuAttr = classType.GetCustomAttribute<MenuAttribute>();
            if (menuAttr == null)
            {
                throw new InvalidOperationException($"Type '{classType}' does not have the '{typeof(MenuAttribute)}'");
            }

            Menu newMenu = new Menu(menuAttr.Title, menuAttr.Subtitle);
            newMenu.SetTheme(menuAttr.Theme);
            newMenu.Metadata.__TemplateClassInstance__ = this;
            BuildItems(newMenu);

            Menu = newMenu;
        }

        private void BuildItems(Menu menu)
        {
            Type classType = this.GetType();

            foreach (PropertyInfo prop in classType.GetProperties(PropertiesFlags))
            {
                foreach (Attribute attr in prop.GetCustomAttributes())
                {
                    switch (attr)
                    {
                        case MenuItemCheckboxAttribute cb: BuildCheckbox(menu, prop, cb); break;
                        case MenuItemNumericScrollerAttribute num: BuildNumericScroller(menu, prop, num); break;
                    }
                }
            }
        }

        private void AddItem(Menu menu, MenuItem item, string attrName, PropertyInfo prop)
        {
            menu.Items.Add(item);
            string name = attrName ?? prop.Name;
            StoreItemInMetadata(menu, item, name);
        }

        // TODO: validate that property is of a valid type
        private void BuildNumericScroller(Menu menu, PropertyInfo prop, MenuItemNumericScrollerAttribute attr)
        {
            MenuItemNumericScroller num = new MenuItemNumericScroller(attr.Text, attr.Description);
            num.Metadata.__TemplateClassInstance__ = this;
            num.Metadata.__TemplatePropertyInfo__ = prop;

            num.Minimum = (decimal)attr.Minimum;
            num.Maximum = (decimal)attr.Maximum;
            num.Increment = (decimal)attr.Increment;
            num.ThousandsSeparator = attr.ThousandsSeparator;
            num.Hexadecimal = attr.Hexadecimal;
            num.DecimalPlaces = attr.DecimalPlaces;

            AddItem(menu, num, attr.Name, prop);

            num.Value = (decimal)Convert.ChangeType(prop.GetValue(this), typeof(decimal));
            num.SelectedIndexChanged += OnMenuNumericScrollerItemChanged;
        }

        private void BuildCheckbox(Menu menu, PropertyInfo prop, MenuItemCheckboxAttribute attr)
        {
            MenuItemCheckbox cb = new MenuItemCheckbox(attr.Text, attr.Description);
            cb.Metadata.__TemplateClassInstance__ = this;
            cb.Metadata.__TemplatePropertyInfo__ = prop;

            AddItem(menu, cb, attr.Name, prop);

            cb.IsChecked = (bool)prop.GetValue(this);
            cb.CheckedChanged += OnMenuCheckboxItemChanged;
        }

        private void OnTemplatePropertyChanged(string propertyName)
        {
            Type classType = this.GetType();
            PropertyInfo prop = classType.GetProperty(propertyName, PropertiesFlags);
            if (prop != null)
            {
                foreach (Attribute attr in prop.GetCustomAttributes())
                {
                    switch (attr)
                    {
                        case MenuItemCheckboxAttribute cb: OnTemplateCheckboxPropertyChanged(prop, cb); break;
                        case MenuItemNumericScrollerAttribute num: OnTemplateNumericScrollerPropertyChanged(prop, num); break;
                    }
                }
            }
        }

        private void OnTemplateCheckboxPropertyChanged(PropertyInfo prop, MenuItemCheckboxAttribute attr)
        {
            string name = attr.Name ?? prop.Name;
            if (GetItemByName(name) is MenuItemCheckbox item)
            {
                item.IsChecked = (bool)prop.GetValue(this);
            }
        }

        private void OnTemplateNumericScrollerPropertyChanged(PropertyInfo prop, MenuItemNumericScrollerAttribute attr)
        {
            string name = attr.Name ?? prop.Name;
            if (GetItemByName(name) is MenuItemNumericScroller item)
            {
                item.Value = (decimal)Convert.ChangeType(prop.GetValue(this), typeof(decimal));
            }
        }

        private void OnMenuCheckboxItemChanged(MenuItemCheckbox sender, CheckedChangedEventArgs e)
        {
            PropertyInfo prop = sender.Metadata.__TemplatePropertyInfo__ as PropertyInfo;

            if (prop != null)
            {
                prop.SetValue(this, e.IsChecked);
            }
        }

        private void OnMenuNumericScrollerItemChanged(MenuItemScroller sender, SelectedIndexChangedEventArgs e)
        {
            PropertyInfo prop = sender.Metadata.__TemplatePropertyInfo__ as PropertyInfo;

            if (prop != null)
            {
                MenuItemNumericScroller num = (MenuItemNumericScroller)sender;

                prop.SetValue(this, Convert.ChangeType(num.Value, prop.PropertyType));
            }
        }
    }
}

