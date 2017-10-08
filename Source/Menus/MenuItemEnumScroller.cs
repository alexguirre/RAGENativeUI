namespace RAGENativeUI.Menus
{
    using System;

    public class MenuItemEnumScroller : MenuItemScroller
    {
        public Type EnumType { get; }
        public object SelectedValue { get { return Values.GetValue(SelectedIndex); } set { SelectedIndex = Array.IndexOf(Values, value); } }
        protected Array Values { get; }

        public MenuItemEnumScroller(string text, Type enumType) : base(text)
        {
            Throw.InvalidOperationIfNot(enumType.IsEnum, $"The type {enumType.Name} isn't an enum.");
            
            EnumType = enumType;
            Values = Enum.GetValues(enumType);
        }

        public override int GetOptionsCount()
        {
            return Values.Length;
        }

        public override string GetSelectedOptionText()
        {
            return Values.GetValue(SelectedIndex).ToString();
        }
    }

    public class MenuItemEnumScroller<TEnum> : MenuItemEnumScroller where TEnum : struct
    {
        public TEnum SelectedEnumValue { get { return (TEnum)Values.GetValue(SelectedIndex); } set { SelectedIndex = Array.IndexOf(Values, value); } }

        public MenuItemEnumScroller(string text) : base(text, typeof(TEnum))
        {
        }
    }
}

