namespace RAGENativeUI.Menus
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class MenuItemListScroller : MenuItemScroller
    {
        private string defaultText = "-";

        public ItemCollection Items { get; }

        /// <summary>
        /// Gets or sets the text that will be displayed when failed to get the selected option text, for example if <see cref="Items"/> is empty.
        /// </summary>
        /// <value>
        /// A <see cref="String"/> representing the default text.
        /// </value>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null</exception>
        public string DefaultText
        {
            get => defaultText;
            set
            {
                Throw.IfNull(value, nameof(value));
                if (value != defaultText)
                {
                    defaultText = value;
                    OnPropertyChanged(nameof(DefaultText));
                }
            }
        }

        public object SelectedItem
        {
            get => (SelectedIndex >= 0 && SelectedIndex < Items.Count) ? Items[SelectedIndex] : null;
            set => SelectedIndex = Items.IndexOf(value);
        }

        public MenuItemListScroller(string text, string description) : base(text, description)
        {
            Items = new ItemCollection();
        }

        public MenuItemListScroller(string text) : this(text, String.Empty)
        {
        }

        public MenuItemListScroller(string text, string description, IEnumerable<object> items) : base(text, description)
        {
             Items = new ItemCollection(items);
        }

        public MenuItemListScroller(string text, IEnumerable<object> items) : this(text, String.Empty, items)
        {
        }

        public override int GetOptionsCount()
        {
            return Items.Count;
        }

        public override string GetSelectedOptionText()
        {
            int index = SelectedIndex;
            if (index >= 0 && index < Items.Count)
                return Items[index].ToString();
            return DefaultText;
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            if(propertyName == nameof(SelectedIndex))
            {
                OnPropertyChanged(nameof(SelectedItem));
            }
        }


        public class ItemCollection : Collection<object>
        {
            public ItemCollection() : base()
            {
            }

            public ItemCollection(IEnumerable<object> items) : base(items.ToList())
            {
            }
        }
    }
}

