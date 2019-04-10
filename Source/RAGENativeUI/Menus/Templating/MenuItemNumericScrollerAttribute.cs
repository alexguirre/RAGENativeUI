namespace RAGENativeUI.Menus.Templating
{
    using System;

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class MenuItemNumericScrollerAttribute : MenuItemAttribute
    {
        public MenuItemNumericScrollerAttribute()
        {
        }

        // These properties should be decimals since that's what MenuItemNumericScroller uses
        // but decimal is not a valid attribute type so we use double instead.
        public double Minimum { get; set; } = 0.0;
        public double Maximum { get; set; } = 100.0;
        public double Increment { get; set; } = 0.5;
        public bool ThousandsSeparator { get; set; }
        public bool Hexadecimal { get; set; }
        public int DecimalPlaces { get; set; } = 2;
    }
}

