namespace RAGENativeUI.Elements
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    public class UIMenuListScrollerItem<T> : UIMenuScrollerItem
    {
        private IList<T> items;
        private string selectedValueText;
        private Func<T, string> formatter;

        public T Value
        {
            get => IsEmpty ? throw new InvalidOperationException("empty") : Items[Index];
        }

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

                    Index = IsEmpty ? -1 : ((Index == -1 ? 0 : Index) % OptionCount); // in case the new list has a different size
                }
            }
        }

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
                    SyncSelectedTextToValue();
                }
            }
        }

        /// <inheritdoc/>
        public override string OptionText => IsEmpty ? null : selectedValueText;

        /// <inheritdoc/>
        public override int OptionCount => Items.Count;

        public UIMenuListScrollerItem(string text, string description, IEnumerable<T> items) : base(text, description)
        {
            Formatter = DefaultFormatter;
            Items = new List<T>(items);
        }

        public UIMenuListScrollerItem(string text, string description) : this(text, description, Enumerable.Empty<T>())
        {
        }

        /// <inheritdoc/>
        protected override void OnSelectedIndexChanged(int oldIndex, int newIndex)
        {
            SyncSelectedTextToValue();

            base.OnSelectedIndexChanged(oldIndex, newIndex);
        }

        private void SyncSelectedTextToValue()
        {
            selectedValueText = (Items == null || IsEmpty) ? null : Formatter(Value);
        }

        public static string DefaultFormatter(T value)
        {
            return value.ToString();
        }
    }
}
