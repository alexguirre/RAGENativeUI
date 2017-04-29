using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;

namespace RAGENativeUI.Elements
{
    /// <summary>
    ///  A list item, with left/right arrows.
    /// </summary>
    /// <seealso cref="RAGENativeUI.Elements.UIMenuItem" />
    public class UIMenuListItem : UIMenuItem
    {
        protected ResText _itemText;

        protected Sprite _arrowLeft;
        protected Sprite _arrowRight;

        [Obsolete("UIMenuListItem._items will be removed soon, use UIMenuListItem.Collection instead.")]
        protected List<dynamic> _items;

        internal uint _holdTime; //used instead of UIMenu's _holdtime to eliminate issues with menu switches instantly switching back

        /// <summary>
        /// Gets or sets the items from this <see cref="UIMenuListItem"/> instance.
        /// </summary>
        /// <remarks>
        /// For better safety before modifying the items <see cref="List{dynamic}"/> set <see cref="Index"/> to 0.
        /// </remarks>
        [Obsolete("UIMenuListItem.Items will be removed soon, use UIMenuListItem.Collection instead.")]
        public virtual List<dynamic> Items
        {
            get { return _items; }
            set { _items = value;  }
        }

        private DisplayItemsCollection collection;
        /// <summary>
        /// Gets the collection of items of this <see cref="UIMenuListItem"/>.
        /// </summary>
        /// <value>
        /// The <see cref="DisplayItemsCollection"/> of this <see cref="UIMenuListItem"/>.
        /// </value>
        public DisplayItemsCollection Collection
        {
            get { return collection; }
            set { collection = value ?? throw new ArgumentNullException("value", "The collection can't be null"); }
        }

        /// <summary>
        /// Gets the current selection.
        /// </summary>
        /// <value>
        /// The current selection's <see cref="IDisplayItem"/>.
        /// </value>
        public IDisplayItem SelectedItem { get { return Collection.Count > 0 ? Collection[Index] : null; } }
        /// <summary>
        /// Gets the current selection value.
        /// </summary>
        /// <value>
        /// The current selection's <see cref="IDisplayItem.Value"/>.
        /// </value>
        public object SelectedValue { get { return SelectedItem == null ? null : SelectedItem.Value; } }

        /// <summary>
        /// Enables or disables scrolling through the list by holding the key
        /// </summary>
        public bool ScrollingEnabled = true;
         /// <summary>
        /// Hold time in milliseconds before scrolling to the next item on list when holding the key [Default = 200]
        /// </summary>
        public uint HoldTimeBeforeScroll = 200;

        /// <summary>
        /// Triggered when the list is changed.
        /// </summary>
        public event ItemListEvent OnListChanged;

        protected int _index;
        
        /// <summary>
        /// Returns the current selected index or -1 if the collection is empty.
        /// </summary>
        public virtual int Index
        {
            get
            {
                if (Collection == null && _items != null && _items.Count == 0)
                    return -1;
                if (Collection != null && Collection.Count == 0)
                    return -1;

                return _index % (Collection == null ? _items.Count : Collection.Count);
            }
            set
            {
                if (Collection == null && _items != null && _items.Count == 0)
                    return;
                if (Collection != null && Collection.Count == 0)
                    return;

                _index = 100000 - (100000 % (Collection == null ? _items.Count : Collection.Count)) + value;
            }
        }


        /// <summary>
        /// List item, with left/right arrows.
        /// </summary>
        /// <param name="text">Item label.</param>
        /// <param name="items">List that contains your items.</param>
        /// <param name="index">Index in the list. If unsure user 0.</param>
        [Obsolete("This constructor overload will be removed soon, use one of the other overloads that don't require a List<dynamic> for the items. If this constructor is used UIMenuListItem.Collection will be null.")]
        public UIMenuListItem(string text, List<dynamic> items, int index)
            : this(text, items, index, "")
        {
        }

        /// <summary>
        /// List item, with left/right arrows.
        /// </summary>
        /// <param name="text">Item label.</param>
        /// <param name="items">List that contains your items.</param>
        /// <param name="index">Index in the list. If unsure user 0.</param>
        /// <param name="description">Description for this item.</param>
        [Obsolete("This constructor overload will be removed soon, use one of the other overloads that don't require a List<dynamic> for the items. If this constructor is used UIMenuListItem.Collection will be null.")]
        public UIMenuListItem(string text, List<dynamic> items, int index, string description)
            : base(text, description)
        {
            const int y = 0;
            _items = new List<dynamic>(items);
            _arrowLeft = new Sprite("commonmenu", "arrowleft", new Point(110, 105 + y), new Size(30, 30));
            _arrowRight = new Sprite("commonmenu", "arrowright", new Point(280, 105 + y), new Size(30, 30));
            _itemText = new ResText("", new Point(290, y + 104), 0.35f, Color.White, Common.EFont.ChaletLondon,
                ResText.Alignment.Left) {TextAlignment = ResText.Alignment.Right};
            Index = index;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UIMenuListItem"/> class, from a collection of <see cref="IDisplayItem"/>s.
        /// </summary>
        /// <param name="text">The <see cref="UIMenuListItem"/>'s label.</param>
        /// <param name="description">The <see cref="UIMenuListItem"/>'s description.</param>
        /// <param name="items">The collection of <see cref="IDisplayItem"/>s.</param>
        public UIMenuListItem(string text, string description, IEnumerable<IDisplayItem> items)
            : base(text, description)
        {
            const int y = 0;
            Collection = new DisplayItemsCollection(items);
            _arrowLeft = new Sprite("commonmenu", "arrowleft", new Point(110, 105 + y), new Size(30, 30));
            _arrowRight = new Sprite("commonmenu", "arrowright", new Point(280, 105 + y), new Size(30, 30));
            _itemText = new ResText("", new Point(290, y + 104), 0.35f, Color.White, Common.EFont.ChaletLondon,
                ResText.Alignment.Left)
            { TextAlignment = ResText.Alignment.Right };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UIMenuListItem"/> class, from a collection of <see cref="IDisplayItem"/>s.
        /// </summary>
        /// <param name="text">The <see cref="UIMenuListItem"/>'s label.</param>
        /// <param name="description">The <see cref="UIMenuListItem"/>'s description.</param>
        /// <param name="items">The collection of <see cref="IDisplayItem"/>s.</param>
        public UIMenuListItem(string text, string description, params IDisplayItem[] items)
            : this(text, description, (IEnumerable<IDisplayItem>)items)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UIMenuListItem"/> class, from a collection of <see cref="object"/>s using the default implementation of <see cref="IDisplayItem"/>, <see cref="DisplayItem"/>.
        /// </summary>
        /// <param name="text">The <see cref="UIMenuListItem"/>'s label.</param>
        /// <param name="description">The <see cref="UIMenuListItem"/>'s description.</param>
        /// <param name="itemsValues">The collection of <see cref="object"/>s.</param>
        public UIMenuListItem(string text, string description, IEnumerable<object> itemsValues)
            : this(text, description, itemsValues.Select(o => new DisplayItem(o)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UIMenuListItem"/> class, from a collection of <see cref="object"/>s using the default implementation of <see cref="IDisplayItem"/>, <see cref="DisplayItem"/>.
        /// </summary>
        /// <param name="text">The <see cref="UIMenuListItem"/>'s label.</param>
        /// <param name="description">The <see cref="UIMenuListItem"/>'s description.</param>
        /// <param name="itemsValues">The collection of <see cref="object"/>s.</param>
        public UIMenuListItem(string text, string description, params object[] itemsValues)
            : this(text, description, (IEnumerable<object>)itemsValues)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UIMenuListItem"/> class with an empty <see cref="Collection"/>.
        /// </summary>
        /// <param name="text">The <see cref="UIMenuListItem"/>'s label.</param>
        /// <param name="description">The <see cref="UIMenuListItem"/>'s description.</param>
        public UIMenuListItem(string text, string description)
            : this(text, description, Enumerable.Empty<IDisplayItem>())
        {
        }

        /// <summary>
        /// Set item's position.
        /// </summary>
        /// <param name="y"></param>
        [Obsolete("Use UIMenuItem.SetVerticalPosition instead.")]
        public override void Position(int y)
        {
            SetVerticalPosition(y);
        }

        /// <summary>
        /// Set item's vertical position.
        /// </summary>
        /// <param name="y"></param>
        public override void SetVerticalPosition(int y)
        {
            _arrowLeft.Position = new Point(300 + Offset.X + Parent.WidthOffset, 147 + y + Offset.Y);
            _arrowRight.Position = new Point(400 + Offset.X + Parent.WidthOffset, 147 + y + Offset.Y);
            _itemText.Position = new Point(300 + Offset.X + Parent.WidthOffset, y + 147 + Offset.Y);
            base.SetVerticalPosition(y);
        }


        /// <summary>
        /// Find an item in the list and return it's index.
        /// </summary>
        /// <param name="item">Item to search for.</param>
        /// <returns>Item index.</returns>
        [Obsolete("UIMenuListItem.ItemToIndex() will be removed soon, use UIMenuListItem.Collection instead.")]
        public virtual int ItemToIndex(dynamic item)
        {
            return Collection == null ? _items.FindIndex(item) : Collection.IndexOf(item);
        }


        /// <summary>
        /// Find an item by it's index and return the item.
        /// </summary>
        /// <param name="index">Item's index.</param>
        /// <returns>Item</returns>
        [Obsolete("UIMenuListItem.IndexToItem() will be removed soon, use UIMenuListItem.Collection instead.")]
        public virtual dynamic IndexToItem(int index)
        {
            return Collection == null ? _items[index] : Collection[index];
        }


        /// <summary>
        /// Draw item.
        /// </summary>
        public override void Draw()
        {
            base.Draw();
            string caption = Collection == null ? 
                                (_items.Count > 0 ? _items[Index].ToString() : " ") : 
                                (Collection.Count > 0 ? Collection[Index].DisplayText : " ");
            int offset = StringMeasurer.MeasureString(caption);

            _itemText.Color = Enabled ? Selected ? HighlightedForeColor : ForeColor : Color.FromArgb(163, 159, 148);
            
            _itemText.Caption = caption;

            _arrowLeft.Color = Enabled ? Selected ? HighlightedForeColor : ForeColor : Color.FromArgb(163, 159, 148);
            _arrowRight.Color = Enabled ? Selected ? HighlightedForeColor : ForeColor : Color.FromArgb(163, 159, 148);

            _arrowLeft.Position = new Point(375 - offset + Offset.X + Parent.WidthOffset, _arrowLeft.Position.Y);
            if (Selected)
            {
                _arrowLeft.Draw();
                _arrowRight.Draw();
                _itemText.Position = new Point(405 + Offset.X + Parent.WidthOffset, _itemText.Position.Y);
            }
            else
            {
                _itemText.Position = new Point(420 + Offset.X + Parent.WidthOffset, _itemText.Position.Y);
            }
            _itemText.Draw();
        }

        internal virtual void ListChangedTrigger(int newindex)
        {
            OnListChanged?.Invoke(this, newindex);
        }

        public override void SetRightBadge(BadgeStyle badge)
        {
            throw new Exception("UIMenuListItem cannot have a right badge.");
        }

        public override void SetRightLabel(string text)
        {
            throw new Exception("UIMenuListItem cannot have a right label.");
        }
    }
}
