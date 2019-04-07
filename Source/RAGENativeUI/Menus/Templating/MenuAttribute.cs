namespace RAGENativeUI.Menus.Templating
{
    using System;

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class MenuAttribute : Attribute
    {
        public MenuAttribute()
        {
        }

        public string Title { get; set; } = "";
        public string Subtitle { get; set; } = "";
        public Type Theme { get; set; } = typeof(Themes.MenuDefaultTheme);
    }
}

