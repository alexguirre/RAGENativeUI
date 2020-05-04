namespace RAGENativeUI.Elements
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a scroller item with a list of items to scroll through.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the items.
    /// </typeparam>
    public class UIMenuListScrollerItem<T> : UIMenuScrollerItem
    {
        private IList<T> items;
        private string selectedItemText;
        private Func<T, string> formatter;

        /// <summary>
        /// Gets the item that is currently selected.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <see cref="Items"/> is empty.
        /// </exception>
        public T SelectedItem
        {
            get => IsEmpty ? throw new InvalidOperationException($"{nameof(Items)} list is empty") : Items[Index];
        }

        /// <summary>
        /// Gets or sets the list containing the items.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <c>value</c> is null.
        /// </exception>
        public IList<T> Items
        {
            get => items;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (value != items)
                {
                    items = value;

                    Index = IsEmpty ? EmptyIndex : (Index % OptionCount); // in case the new list has a different size
                }
            }
        }

        /// <summary>
        /// Gets or sets the formatter used to display the selected item.
        /// A <see cref="Func{T, TResult}"/> that takes in a <typeparamref name="T"/> and returns its <see cref="string"/> representation.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <c>value</c> is null.
        /// </exception>
        /// <seealso cref="DefaultFormatter(T)"/>
        public Func<T, string> Formatter
        {
            get => formatter;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (value != formatter)
                {
                    formatter = value;
                    SyncSelectedItemText();
                }
            }
        }

        /// <inheritdoc/>
        public override string OptionText => IsEmpty ? null : selectedItemText;

        /// <inheritdoc/>
        public override int OptionCount => Items.Count;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIMenuListScrollerItem{T}"/> class with the specified items.
        /// </summary>
        /// <param name="text">The label of this list scroller.</param>
        /// <param name="description">The description of this list scroller.</param>
        /// <param name="items">The items of this list scroller.</param>
        public UIMenuListScrollerItem(string text, string description, IEnumerable<T> items) : base(text, description)
        {
            Formatter = DefaultFormatter;
            Items = new List<T>(items);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UIMenuListScrollerItem{T}"/> class with no items.
        /// </summary>
        /// <param name="text">The label of this menu item.</param>
        /// <param name="description">The description of this menu item.</param>
        public UIMenuListScrollerItem(string text, string description) : this(text, description, Enumerable.Empty<T>())
        {
        }

        /// <inheritdoc/>
        protected override void OnSelectedIndexChanged(int oldIndex, int newIndex)
        {
            SyncSelectedItemText();

            base.OnSelectedIndexChanged(oldIndex, newIndex);
        }

        private void SyncSelectedItemText()
        {
            selectedItemText = (Items == null || IsEmpty) ? null : Formatter(SelectedItem);
        }

        /// <summary>
        /// The default value of <see cref="Formatter"/>.
        /// The returned <see cref="string"/> is obtained through the <see cref="object.ToString"/> method of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <returns>
        /// The <see cref="string"/> representation of <paramref name="value"/>.
        /// If <paramref name="value"/> is <c>null</c>, <c>null</c> is returned.
        /// </returns>
        public static string DefaultFormatter(T value)
        {
            return value?.ToString();
        }
    }
}
