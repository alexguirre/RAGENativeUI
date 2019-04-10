namespace RAGENativeUI.Menus.Templating
{
    using System;

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class MenuItemCheckboxAttribute : MenuItemAttribute
    {
        public MenuItemCheckboxAttribute()
        {
        }
    }
}

