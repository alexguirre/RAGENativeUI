namespace RAGENativeUI.Menus
{
    using System;

    public class MenuItemEnumScroller : MenuItemScroller
    {
        public Type EnumType { get; }

        public object SelectedValue
        {
            get => Values.GetValue(SelectedIndex);
            set => SelectedIndex = Array.IndexOf(Values, value);
        }

        protected Array Values { get; }

        public override int OptionCount => Values.Length;
        public override string SelectedOptionText => Values.GetValue(SelectedIndex).ToString();

        public MenuItemEnumScroller(string text, string description, Type enumType) : base(text, description)
        {
            Throw.InvalidOperationIfNot(enumType.IsEnum, $"The type '{enumType.Name}' isn't an enum.");
            
            EnumType = enumType;
            Values = Enum.GetValues(enumType);
        }

        public MenuItemEnumScroller(string text, Type enumType) : this(text, String.Empty, enumType)
        {
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(SelectedIndex))
            {
                OnPropertyChanged(nameof(SelectedValue));
            }
        }
    }

    public class MenuItemEnumScroller<T> : MenuItemEnumScroller where T : Enum
    {
        public T SelectedEnumValue
        {
            get => (T)Values.GetValue(SelectedIndex);
            set => SelectedIndex = Array.IndexOf(Values, value);
        }

        public MenuItemEnumScroller(string text, string description) : base(text, description, typeof(T))
        {
        }

        public MenuItemEnumScroller(string text) : this(text, String.Empty)
        {
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(SelectedIndex))
            {
                OnPropertyChanged(nameof(SelectedEnumValue));
            }
        }
    }
}

