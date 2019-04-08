namespace RAGENativeUI.Menus.Templating
{
    extern alias rph1;

    using System;
    using System.Reflection;
    using System.Linq;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    [Menu(Title = "My Menu", Subtitle = "Subtitle for my menu")]
    public class TestMenu : INotifyPropertyChanged
    {
        private bool doSomething;
        private bool doSomeOtherThing;
        private float floatValue;
        private int intValue;

        public event PropertyChangedEventHandler PropertyChanged;

        [MenuItemCheckbox(Text = "Do Something?", Description = "Whether to do something or not.")]
        public bool DoSomething
        {
            get => doSomething;
            set => SetProperty(ref doSomething, value);
        }

        [MenuItemCheckbox(Text = "Do Some Other Thing?", Description = "Whether to do some other thing or not.")]
        public bool DoSomeOtherThing
        {
            get => doSomeOtherThing;
            set => SetProperty(ref doSomeOtherThing, value);
        }

        [MenuItemNumericScroller(Text = "Float Value")]
        public float FloatValue
        {
            get => floatValue;
            set => SetProperty(ref floatValue, value);
        }

        [MenuItemNumericScroller(Text = "Int Value", Increment = 1.0, DecimalPlaces = 0)]
        public int IntValue
        {
            get => intValue;
            set => SetProperty(ref intValue, value);
        }

        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Display()
        {
            rph1::Rage.Game.DisplaySubtitle(
                $"DoSomething:{DoSomething}~n~" +
                $"DoSomeOtherThing:{DoSomeOtherThing}~n~" +
                $"FloatValue:{FloatValue}~n~" +
                $"IntValue:{IntValue}"
                );
        }


        // TODO: move Build somewhere else
        private Menu menu;
        public Menu Build()
        {
            Type classType = this.GetType();

            MenuAttribute menuAttr = classType.GetCustomAttribute<MenuAttribute>();
            if (menuAttr == null)
            {
                throw new InvalidOperationException($"Type '{classType}' does not have the '{typeof(MenuAttribute)}'");
            }

            menu = new Menu(menuAttr.Title, menuAttr.Subtitle);
            menu.SetTheme(menuAttr.Theme);
            menu.Metadata.__TemplateClassInstance__ = this;
            BuildItems(menu, classType);

            PropertyChanged += OnTemplatePropertyChanged;

            return menu;
        }

        private void OnTemplatePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Type classType = this.GetType();
            PropertyInfo prop = classType.GetRuntimeProperty(e.PropertyName);
            if (prop != null)
            {
                {
                    MenuItemCheckboxAttribute attr = prop.GetCustomAttribute<MenuItemCheckboxAttribute>();
                    if (attr != null)
                    {
                        string name = attr.Name ?? prop.Name;
                        if (RetrieveItemFromMenuMetadata(menu, name) is MenuItemCheckbox item)
                        {
                            OnCheckboxCheckedChangedFromTemplate(item, (bool)prop.GetValue(this));
                        }
                    }
                }
                {
                    MenuItemNumericScrollerAttribute attr = prop.GetCustomAttribute<MenuItemNumericScrollerAttribute>();
                    if (attr != null)
                    {
                        string name = attr.Name ?? prop.Name;
                        if (RetrieveItemFromMenuMetadata(menu, name) is MenuItemNumericScroller item)
                        {
                            OnNumericScrollerValueChangedFromTemplate(item, (decimal)Convert.ChangeType(prop.GetValue(this), typeof(decimal)));
                        }
                    }
                }
            }
        }

        private void StoreItemInMenuMetadata(Menu menu, MenuItem item, string name)
        {
            if (!menu.Metadata.TryGetValue<IDictionary<string, MenuItem>>("__TemplateItems__", out IDictionary<string, MenuItem> dict))
            {
                dict = new Dictionary<string, MenuItem>();
                menu.Metadata["__TemplateItems__"] = dict;
            }

            dict.Add(name, item);
        }

        private MenuItem RetrieveItemFromMenuMetadata(Menu menu, string name)
        {
            if (menu.Metadata.TryGetValue<IDictionary<string, MenuItem>>("__TemplateItems__", out IDictionary<string, MenuItem> dict))
            {
                if (dict.TryGetValue(name, out MenuItem item))
                {
                    return item;
                }
            }

            return null;
        }

        private void BuildItems(Menu menu, Type classType)
        {

            foreach (PropertyInfo prop in classType.GetRuntimeProperties())
            {
                foreach (Attribute attr in prop.GetCustomAttributes(true))
                {
                    switch (attr)
                    {
                        case MenuItemCheckboxAttribute cb: BuildCheckbox(menu, prop, cb); break;
                        case MenuItemNumericScrollerAttribute num: BuildNumericScroller(menu, prop, num); break;
                    }
                }
            }
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

            menu.Items.Add(num);
            string name = attr.Name ?? prop.Name;
            StoreItemInMenuMetadata(menu, num, name);

            num.Value = (decimal)Convert.ChangeType(prop.GetValue(this), typeof(decimal));

            num.SelectedIndexChanged += OnNumericScrollerValueChangedFromMenu;
        }

        // Src:Menu -> Dst:Template binding
        private void OnNumericScrollerValueChangedFromMenu(MenuItemScroller sender, SelectedIndexChangedEventArgs e)
        {
            MenuItemNumericScroller num = (MenuItemNumericScroller)sender;
            object o = num.Metadata.__TemplateClassInstance__;
            PropertyInfo prop = num.Metadata.__TemplatePropertyInfo__ as PropertyInfo;

            if (o != null && prop != null)
            {
                prop.SetValue(o, Convert.ChangeType(num.Value, prop.PropertyType));
            }
        }
        // Src:Template -> Dst:Menu binding
        private void OnNumericScrollerValueChangedFromTemplate(MenuItemNumericScroller receiver, decimal value)
        {
            receiver.Value = value;
        }

        private void BuildCheckbox(Menu menu, PropertyInfo prop, MenuItemCheckboxAttribute attr)
        {
            MenuItemCheckbox cb = new MenuItemCheckbox(attr.Text, attr.Description);
            cb.Metadata.__TemplateClassInstance__ = this;
            cb.Metadata.__TemplatePropertyInfo__ = prop;

            menu.Items.Add(cb);
            string name = attr.Name ?? prop.Name;
            StoreItemInMenuMetadata(menu, cb, name);

            cb.IsChecked = (bool)prop.GetValue(this);

            cb.CheckedChanged += OnCheckboxCheckedChangedFromMenu;
        }

        
        // Src:Menu -> Dst:Template binding
        private void OnCheckboxCheckedChangedFromMenu(MenuItemCheckbox sender, CheckedChangedEventArgs e)
        {
            object o = sender.Metadata.__TemplateClassInstance__;
            PropertyInfo prop = sender.Metadata.__TemplatePropertyInfo__ as PropertyInfo;

            if (o != null && prop != null)
            {
                prop.SetValue(o, e.IsChecked);
            }
        }
        // Src:Template -> Dst:Menu binding
        private void OnCheckboxCheckedChangedFromTemplate(MenuItemCheckbox receiver, bool isChecked)
        {
            receiver.IsChecked = isChecked;
        }
    }
}

