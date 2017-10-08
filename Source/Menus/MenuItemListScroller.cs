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
        public DisplayItemsCollection Items { get { return items; } set { Throw.IfNull(value, nameof(value)); items = value; } }

        /// <summary>
        /// Gets or sets the text that will be displayed when failed to get an item's display text, for example if <see cref="Items"/> is empty.
        /// </summary>
        /// <value>
        /// A <see cref="String"/> representing the default text.
        /// </value>
        /// <exception cref="ArgumentNullException">When setting the property to a null value.</exception>
        public string DefaultText { get { return defaultText; } set { Throw.IfNull(value, nameof(value)); defaultText = value; } }

        public IDisplayItem SelectedItem { get { return (SelectedIndex >= 0 && SelectedIndex < Items.Count) ? Items[SelectedIndex] : null; } set { SelectedIndex = Items.IndexOf(value); } }

        public MenuItemListScroller(string text) : base(text)
        {
            Items = new DisplayItemsCollection();
        }

        public MenuItemListScroller(string text, IEnumerable<IDisplayItem> items) : base(text)
        {
            Items = new DisplayItemsCollection(items);
        }

        public MenuItemListScroller(string text, IEnumerable<object> items) : this(text, items.Select(o => new DisplayItem(o)))
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
    }
}

