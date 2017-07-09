namespace RAGENativeUI.Menus
{
    using System;

    public class MenuItemEnumScroller<TEnum> : MenuItemEnumScroller where TEnum : struct
    {
        public TEnum SelectEnumValue { get { return (TEnum)Values.GetValue(SelectedIndex); } set { SelectedIndex = Array.IndexOf(Values, value); } }

        public MenuItemEnumScroller(string text) : base(text, typeof(TEnum))
        {
        }
    }

    public class MenuItemEnumScroller : MenuItemScroller
    {
        public Type EnumType { get; }
        public object SelectValue { get { return Values.GetValue(SelectedIndex); } set { SelectedIndex = Array.IndexOf(Values, value); } }
        protected Array Values { get; }

        public MenuItemEnumScroller(string text, Type enumType) : base(text)
        {
            if (!enumType.IsEnum)
                throw new InvalidOperationException($"The type {enumType.Name} isn't an enum.");

            EnumType = enumType;
            Values = Enum.GetValues(enumType);
        }

        protected override int GetOptionsCount()
        {
            return Values.Length;
        }

        protected override string GetSelectedOptionText()
        {
            return Values.GetValue(SelectedIndex).ToString();
        }
    }
}

