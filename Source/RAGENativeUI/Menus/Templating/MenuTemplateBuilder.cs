namespace RAGENativeUI.Menus.Templating
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;

    internal static class MenuTemplateBuilder
    {
        private const BindingFlags PropertiesFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private const string MenuTemplateItemsKey = "__TemplateItems__";

        public static MenuItem RetrieveItemFromMetadata(Menu menu, string name)
        {
            if (menu.Metadata.TryGetValue<IDictionary<string, MenuItem>>(MenuTemplateItemsKey, out IDictionary<string, MenuItem> dict))
            {
                if (dict.TryGetValue(name, out MenuItem item))
                {
                    return item;
                }
            }

            return null;
        }

        private static void StoreItemInMetadata(Menu menu, MenuItem item, string name)
        {
            if (!menu.Metadata.TryGetValue<IDictionary<string, MenuItem>>(MenuTemplateItemsKey, out IDictionary<string, MenuItem> dict))
            {
                dict = new Dictionary<string, MenuItem>();
                menu.Metadata[MenuTemplateItemsKey] = dict;
            }

            dict.Add(name, item);
        }

        public static Menu BuildMenu(MenuTemplate template)
        {
            Type classType = template.GetType();

            MenuAttribute menuAttr = classType.GetCustomAttribute<MenuAttribute>();
            if (menuAttr == null)
            {
                throw new InvalidOperationException($"Type '{classType}' does not have the {typeof(MenuAttribute)}");
            }

            Menu newMenu = new Menu(menuAttr.Title, menuAttr.Subtitle);
            newMenu.SetTheme(menuAttr.Theme);
            newMenu.Metadata.__TemplateClassInstance__ = template;
            BuildItems(newMenu);
            AttachHandlers(newMenu);

            template.PropertyChanged += OnTemplatePropertyChanged;

            return newMenu;
        }

        private static void BuildItems(Menu menu)
        {
            MenuTemplate template = menu.Metadata.__TemplateClassInstance__ as MenuTemplate;
            Type classType = template.GetType();

            foreach (PropertyInfo prop in classType.GetProperties(PropertiesFlags))
            {
                foreach (Attribute attr in prop.GetCustomAttributes())
                {
                    switch (attr)
                    {
                        case MenuItemCheckboxAttribute cb: BuildCheckbox(menu, prop, cb); break;
                        case MenuItemNumericScrollerAttribute num: BuildNumericScroller(menu, prop, num); break;
                        case MenuItemEnumScrollerAttribute enumScr: BuildEnumScroller(menu, prop, enumScr); break;
                        case MenuItemAttribute item: BuildBasicItem(menu, prop, item); break;
                    }
                }
            }
        }

        private static void AddItem(Menu menu, MenuItem item, string attrName, PropertyInfo prop)
        {
            menu.Items.Add(item);
            string name = attrName ?? prop.Name;
            StoreItemInMetadata(menu, item, name);
            SetItemMetadata(item, menu.Metadata.__TemplateClassInstance__, prop, name);
        }

        private static void SetItemMetadata(MenuItem item, MenuTemplate template, PropertyInfo prop, string name)
        {
            item.Metadata.__TemplateClassInstance__ = template;
            item.Metadata.__TemplatePropertyInfo__ = prop;
            item.Metadata.__TemplateItemName__ = name;
        }

        private static void BuildBasicItem(Menu menu, PropertyInfo prop, MenuItemAttribute attr)
        {
            if (!IsValidPropertyForBasicItem(prop))
            {
                throw new InvalidOperationException($"Property '{prop.DeclaringType.FullName}.{prop.Name}' cannnot be binded to a {nameof(MenuItem)}");
            }

            MenuTemplate template = menu.Metadata.__TemplateClassInstance__ as MenuTemplate;
            MenuItem item = new MenuItem(attr.Text, attr.Description);

            MethodInfo invokeMethod = prop.PropertyType.GetMethod("Invoke");
            item.Metadata.__TemplatePropertyInvokeMethod__ = invokeMethod;
            item.Metadata.__TemplatePropertyInvokeWithItem__ = invokeMethod.GetParameters().Length == 1;
            AddItem(menu, item, attr.Name, prop);

            item.Activated += OnMenuBasicItemActivated;
        }

        private static bool IsValidPropertyForBasicItem(PropertyInfo prop)
        {
            if (prop.CanRead)
            {
                MethodInfo invokeMethod = prop.PropertyType.GetMethod("Invoke"); // TODO: should check for private methods too?
                if (invokeMethod != null)
                {
                    ParameterInfo[] parameters = invokeMethod.GetParameters();
                    return parameters.Length == 0 ||
                           (parameters.Length == 1 && !parameters[0].IsOut && parameters[0].ParameterType == typeof(MenuItem));
                }
            }

            return false;
        }

        private static void BuildNumericScroller(Menu menu, PropertyInfo prop, MenuItemNumericScrollerAttribute attr)
        {
            if (!IsValidPropertyForNumericScroller(prop))
            {
                throw new InvalidOperationException($"Property '{prop.DeclaringType.FullName}.{prop.Name}' cannnot be binded to a {nameof(MenuItemNumericScroller)}");
            }

            MenuTemplate template = menu.Metadata.__TemplateClassInstance__ as MenuTemplate;
            MenuItemNumericScroller num = new MenuItemNumericScroller(attr.Text, attr.Description);

            num.Minimum = (decimal)attr.Minimum;
            num.Maximum = (decimal)attr.Maximum;
            num.Increment = (decimal)attr.Increment;
            num.ThousandsSeparator = attr.ThousandsSeparator;
            num.Hexadecimal = attr.Hexadecimal;
            num.DecimalPlaces = attr.DecimalPlaces;

            AddItem(menu, num, attr.Name, prop);

            num.Value = (decimal)Convert.ChangeType(prop.GetValue(template), typeof(decimal));
            num.SelectedIndexChanged += OnMenuNumericScrollerItemChanged;
        }

        private static bool IsValidPropertyForNumericScroller(PropertyInfo prop)
        {
            if (prop.CanWrite && prop.CanRead && !prop.PropertyType.IsEnum)
            {
                switch (Type.GetTypeCode(prop.PropertyType))
                {
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        return true;
                }
            }

            return false;
        }

        private static void BuildCheckbox(Menu menu, PropertyInfo prop, MenuItemCheckboxAttribute attr)
        {
            if (!IsValidPropertyForCheckbox(prop))
            {
                throw new InvalidOperationException($"Property '{prop.DeclaringType.FullName}.{prop.Name}' cannnot be binded to a {nameof(MenuItemCheckbox)}");
            }

            MenuTemplate template = menu.Metadata.__TemplateClassInstance__ as MenuTemplate;
            MenuItemCheckbox cb = new MenuItemCheckbox(attr.Text, attr.Description);

            AddItem(menu, cb, attr.Name, prop);

            cb.IsChecked = (bool)prop.GetValue(template);
            cb.CheckedChanged += OnMenuCheckboxItemChanged;
        }

        private static bool IsValidPropertyForCheckbox(PropertyInfo prop)
        {
            if (prop.CanWrite && prop.CanRead)
            {
                return Type.GetTypeCode(prop.PropertyType) == TypeCode.Boolean;
            }

            return false;
        }

        private static void BuildEnumScroller(Menu menu, PropertyInfo prop, MenuItemEnumScrollerAttribute attr)
        {
            if (!IsValidPropertyForEnumScroller(prop))
            {
                throw new InvalidOperationException($"Property '{prop.DeclaringType.FullName}.{prop.Name}' cannnot be binded to a {nameof(MenuItemEnumScroller)}");
            }

            MenuTemplate template = menu.Metadata.__TemplateClassInstance__ as MenuTemplate;
            MenuItemEnumScroller enumScr = new MenuItemEnumScroller(attr.Text, attr.Description, prop.PropertyType);

            AddItem(menu, enumScr, attr.Name, prop);

            enumScr.SelectedValue = prop.GetValue(template);
            enumScr.SelectedIndexChanged += OnMenuEnumScrollerItemChanged;
        }

        private static bool IsValidPropertyForEnumScroller(PropertyInfo prop)
        {
            return prop.CanWrite && prop.CanRead && prop.PropertyType.IsEnum;
        }

        private static void AttachHandlers(Menu menu)
        {
            MenuTemplate template = menu.Metadata.__TemplateClassInstance__ as MenuTemplate;
            Type classType = template.GetType();

            foreach (MethodInfo method in classType.GetMethods(PropertiesFlags))
            {
                foreach (Attribute attr in method.GetCustomAttributes())
                {
                    switch (attr)
                    {
                        case MenuItemActivatedHandlerAttribute act: AttachItemActivatedHandler(menu, method, act); break;
                    }
                }
            }
        }

        private static void AttachItemActivatedHandler(Menu menu, MethodInfo method, MenuItemActivatedHandlerAttribute attr)
        {
            MenuTemplate template = menu.Metadata.__TemplateClassInstance__ as MenuTemplate;
            TypedEventHandler<MenuItem, ActivatedEventArgs> d = (TypedEventHandler<MenuItem, ActivatedEventArgs>)method.CreateDelegate(typeof(TypedEventHandler<MenuItem, ActivatedEventArgs>), template);
            foreach (string itemName in attr.Items)
            {
                MenuItem item = RetrieveItemFromMetadata(menu, itemName);
                if (item != null)
                {
                    item.Activated += d;
                }
                else
                {
                    // TODO: log warning
                }
            }
        }

        private static void OnTemplatePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MenuTemplate template = (MenuTemplate)sender;
            Type classType = sender.GetType();
            PropertyInfo prop = classType.GetProperty(e.PropertyName, PropertiesFlags);

            if (prop != null)
            {
                foreach (Attribute attr in prop.GetCustomAttributes())
                {
                    switch (attr)
                    {
                        case MenuItemCheckboxAttribute cb: OnTemplateCheckboxPropertyChanged(template, prop, cb); break;
                        case MenuItemNumericScrollerAttribute num: OnTemplateNumericScrollerPropertyChanged(template, prop, num); break;
                        case MenuItemEnumScrollerAttribute enumScr: OnTemplateEnumScrollerPropertyChanged(template, prop, enumScr); break;
                    }
                }
            }
        }

        private static void OnMenuBasicItemActivated(MenuItem sender, ActivatedEventArgs e)
        {
            MenuTemplate template = sender.Metadata.__TemplateClassInstance__ as MenuTemplate;
            PropertyInfo prop = sender.Metadata.__TemplatePropertyInfo__ as PropertyInfo;

            if (prop != null)
            {
                MethodInfo invokeMethod = sender.Metadata.__TemplatePropertyInvokeMethod__ as MethodInfo;
                bool withItem = sender.Metadata.__TemplatePropertyInvokeWithItem__;
                object[] parameters = withItem ? new[] { sender } : null;
                invokeMethod.Invoke(prop.GetValue(template), parameters);
            }
        }

        private static void OnTemplateCheckboxPropertyChanged(MenuTemplate sender, PropertyInfo prop, MenuItemCheckboxAttribute attr)
        {
            string name = attr.Name ?? prop.Name;
            if (sender.GetItemByName(name) is MenuItemCheckbox item)
            {
                item.IsChecked = (bool)prop.GetValue(sender);
            }
        }

        private static void OnTemplateNumericScrollerPropertyChanged(MenuTemplate sender, PropertyInfo prop, MenuItemNumericScrollerAttribute attr)
        {
            string name = attr.Name ?? prop.Name;
            if (sender.GetItemByName(name) is MenuItemNumericScroller item)
            {
                item.Value = (decimal)Convert.ChangeType(prop.GetValue(sender), typeof(decimal));
            }
        }

        private static void OnTemplateEnumScrollerPropertyChanged(MenuTemplate sender, PropertyInfo prop, MenuItemEnumScrollerAttribute attr)
        {
            string name = attr.Name ?? prop.Name;
            if (sender.GetItemByName(name) is MenuItemEnumScroller item)
            {
                item.SelectedValue = Convert.ChangeType(prop.GetValue(sender), prop.PropertyType);
            }
        }

        private static void OnMenuCheckboxItemChanged(MenuItemCheckbox sender, CheckedChangedEventArgs e)
        {
            MenuTemplate template = sender.Metadata.__TemplateClassInstance__ as MenuTemplate;
            PropertyInfo prop = sender.Metadata.__TemplatePropertyInfo__ as PropertyInfo;

            if (prop != null)
            {
                prop.SetValue(template, e.IsChecked);
            }
        }

        private static void OnMenuNumericScrollerItemChanged(MenuItemScroller sender, SelectedIndexChangedEventArgs e)
        {
            MenuTemplate template = sender.Metadata.__TemplateClassInstance__ as MenuTemplate;
            PropertyInfo prop = sender.Metadata.__TemplatePropertyInfo__ as PropertyInfo;

            if (prop != null)
            {
                MenuItemNumericScroller num = (MenuItemNumericScroller)sender;

                prop.SetValue(template, Convert.ChangeType(num.Value, prop.PropertyType));
            }
        }

        private static void OnMenuEnumScrollerItemChanged(MenuItemScroller sender, SelectedIndexChangedEventArgs e)
        {
            MenuTemplate template = sender.Metadata.__TemplateClassInstance__ as MenuTemplate;
            PropertyInfo prop = sender.Metadata.__TemplatePropertyInfo__ as PropertyInfo;

            if (prop != null)
            {
                MenuItemEnumScroller enumScr = (MenuItemEnumScroller)sender;

                prop.SetValue(template, Convert.ChangeType(enumScr.SelectedValue, prop.PropertyType));
            }
        }
    }
}

