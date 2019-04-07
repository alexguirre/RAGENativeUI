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
                $"DoSomeOtherThing:{DoSomeOtherThing}"
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
                MenuItemCheckboxAttribute attr = prop.GetCustomAttribute<MenuItemCheckboxAttribute>();
                if (attr != null)
                {
                    string name = attr.Name ?? prop.Name;
                    MenuItemCheckbox item = RetrieveItemFromMenuMetadata(menu, name) as MenuItemCheckbox;
                    if (item != null)
                    {
                        OnCheckboxCheckedChangedFromTemplate(item, (bool)prop.GetValue(this));
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
                        case MenuItemCheckboxAttribute cb: BuildCheckbox(menu, classType, prop, cb); break;
                    }
                }
            }
        }

        private void BuildCheckbox(Menu menu, Type classType, PropertyInfo prop, MenuItemCheckboxAttribute attr)
        {
            MenuItemCheckbox cb = new MenuItemCheckbox(attr.Text, attr.Description);
            cb.Metadata.__TemplateClassInstance__ = this;
            cb.Metadata.__TemplatePropertyInfo__ = prop;
            cb.CheckedChanged += OnCheckboxCheckedChangedFromMenu;

            menu.Items.Add(cb);
            string name = attr.Name ?? prop.Name;
            StoreItemInMenuMetadata(menu, cb, name);
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

