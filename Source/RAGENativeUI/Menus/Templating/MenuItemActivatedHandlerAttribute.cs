namespace RAGENativeUI.Menus.Templating
{
    using System;

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class MenuItemActivatedHandlerAttribute : Attribute
    {
        public MenuItemActivatedHandlerAttribute(params string[] items)
        {
            Items = items;
        }

        public string[] Items { get; }
    }
}

