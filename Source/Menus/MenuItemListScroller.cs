namespace RAGENativeUI.Menus
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    public class MenuItemListScroller : MenuItemScroller
    {
        private DisplayItemsCollection items;
        private string defaultText = "-";

        /// <exception cref="ArgumentNullException">When setting the property to a null value.</exception>
        public DisplayItemsCollection Items
        {
            get => items;
            set
            {
                Throw.IfNull(value, nameof(value));
                if (value != items)
                {
                    items = value;
                    OnPropertyChanged(nameof(Items));
                }
            }
        }

        /// <summary>
        /// Gets or sets the text that will be displayed when failed to get an item's display text, for example if <see cref="Items"/> is empty.
        /// </summary>
        /// <value>
        /// A <see cref="String"/> representing the default text.
        /// </value>
        /// <exception cref="ArgumentNullException">When setting the property to a null value.</exception>
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

        public IDisplayItem SelectedItem
        {
            get => (SelectedIndex >= 0 && SelectedIndex < Items.Count) ? Items[SelectedIndex] : null;
            set => SelectedIndex = Items.IndexOf(value);
        }

        public MenuItemListScroller(string text, string description) : base(text, description)
        {
            Items = new DisplayItemsCollection();
        }

        public MenuItemListScroller(string text) : this(text, String.Empty)
        {
            Items = new DisplayItemsCollection();
        }

        public MenuItemListScroller(string text, string description, IEnumerable<IDisplayItem> items) : base(text, description)
        {
            Items = new DisplayItemsCollection(items);
        }

        public MenuItemListScroller(string text, IEnumerable<IDisplayItem> items) : this(text, String.Empty, items)
        {
        }

        public MenuItemListScroller(string text, string description, IEnumerable<object> items) : this(text, description, items.Select(o => new DisplayItem(o)))
        {
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
                return Items[index].DisplayText;
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
    }
}

