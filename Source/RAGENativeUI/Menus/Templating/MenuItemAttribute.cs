namespace RAGENativeUI.Menus.Templating
{
    using System;

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class MenuItemAttribute : Attribute
    {
        public MenuItemAttribute()
        {
        }

        public string Name { get; set; }
        public string Text { get; set; } = "";
        public string Description { get; set; } = "";
    }
}

