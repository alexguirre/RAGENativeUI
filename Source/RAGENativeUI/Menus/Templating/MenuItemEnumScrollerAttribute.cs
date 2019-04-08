namespace RAGENativeUI.Menus.Templating
{
    using System;

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class MenuItemEnumScrollerAttribute : Attribute
    {
        public MenuItemEnumScrollerAttribute()
        {
        }

        public string Name { get; set; }
        public string Text { get; set; } = "";
        public string Description { get; set; } = "";
    }
}

