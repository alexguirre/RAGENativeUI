namespace RAGENativeUI.Menus.Templating
{
    using System;
    using System.Reflection;
    using System.Linq;
    using System.Collections.Generic;

    [Menu(Title = "My Menu", Subtitle = "Subtitle for my menu")]
    public class TestMenu
    {
        [MenuItemCheckbox(Text = "Do Something?", Description = "Whether to do something or not.")]
        public bool DoSomething { get; set; }
        [MenuItemCheckbox(Text = "Do Some Other Thing?", Description = "Whether to do some other thing or not.")]
        public bool DoSomeOtherThing { get; set; }




        // TODO: move Build somewhere else
        public Menu Build()
        {
            Type classType = this.GetType();

            MenuAttribute menuAttr = classType.GetCustomAttribute<MenuAttribute>();
            if (menuAttr == null)
            {
                throw new InvalidOperationException($"Type '{classType}' does not have the '{typeof(MenuAttribute)}'");
            }

            Menu menu = new Menu(menuAttr.Title, menuAttr.Subtitle);
            menu.SetTheme(menuAttr.Theme);
            BuildItems(menu, classType);

            return menu;
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
            cb.Metadata.TemplateClassType = classType;
            cb.Metadata.TemplatePropertyInfo = prop;

            menu.Items.Add(cb);
            string name = attr.Name ?? prop.Name;
            StoreItemInMenuMetadata(menu, cb, name);
        }
    }
}

