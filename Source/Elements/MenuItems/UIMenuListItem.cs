using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;

namespace RAGENativeUI.Elements
{
    /// <summary>
    ///  A list item, with left/right arrows.
    /// </summary>
    /// <seealso cref="RAGENativeUI.Elements.UIMenuItem" />
    public class UIMenuListItem : UIMenuItem
    {
        [Obsolete] protected ResText _itemText;

        [Obsolete] protected Sprite _arrowLeft;
        [Obsolete] protected Sprite _arrowRight;

        [Obsolete("UIMenuListItem._items will be removed soon, use UIMenuListItem.Collection instead.")]
        protected List<dynamic> _items;

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
        public override void Draw(float x, float y, float width, float height)
        {
            base.Draw(x, y, width, height);

            string selectedOption = Collection == null ?
                                (_items.Count > 0 ? _items[Index].ToString() : " ") :
                                (Collection.Count > 0 ? Collection[Index].DisplayText : " ");

            SetTextCommandOptions(false);
            float optTextWidth = TextCommands.GetWidth(selectedOption);

            GetBadgeOffsets(out _, out float badgeOffset);

            if (Selected && CanDrawNavBar && Enabled)
            {
                Color textColor = CurrentForeColor;
                float optTextX = x + width - (0.00390625f * 1.5f) - optTextWidth - (0.0046875f * 1.5f) - badgeOffset;
                float optTextY = y + 0.00277776f;

                SetTextCommandOptions(false);
                TextCommands.Display(selectedOption, optTextX, optTextY);

                {
                    UIMenu.GetTextureDrawSize(UIMenu.CommonTxd, UIMenu.ArrowRightTextureName, out float w, out float h);

                    float spriteX = x + width - (0.00390625f * 0.5f) - (w * 0.5f) - badgeOffset;
                    float spriteY = y + (0.034722f * 0.5f);

                    UIMenu.DrawSprite(UIMenu.CommonTxd, UIMenu.ArrowRightTextureName, spriteX, spriteY, w, h, textColor);
                }
                {
                    UIMenu.GetTextureDrawSize(UIMenu.CommonTxd, UIMenu.ArrowLeftTextureName, out float w, out float h);

                    float spriteX = x + width - (0.00390625f * 1.5f) - (w * 0.5f) - optTextWidth - (0.0046875f * 1.5f) - badgeOffset;
                    float spriteY = y + (0.034722f * 0.5f);

                    UIMenu.DrawSprite(UIMenu.CommonTxd, UIMenu.ArrowLeftTextureName, spriteX, spriteY, w, h, textColor);
                }
            }
            else
            {
                float optTextX = x + width - 0.00390625f - optTextWidth - badgeOffset;
                float optTextY = y + 0.00277776f;

                SetTextCommandOptions(false);
                TextCommands.Display(selectedOption, optTextX, optTextY);
            }
        }

        protected internal override bool OnInput(UIMenu menu, Common.MenuControls control)
        {
            bool consumed = base.OnInput(menu, control);

            if (ScrollingEnabled && Enabled)
            {
                switch (control)
                {
                    case Common.MenuControls.Left:
                        consumed = true;
                        if ((Collection == null ? Items.Count : Collection.Count) != 0)
                        {
                            Index--;
                            Common.PlaySound(menu.AUDIO_LEFTRIGHT, menu.AUDIO_LIBRARY);
                            menu.ListChange(this, Index);
                            ListChangedTrigger(Index);
                        }
                        break;

                    case Common.MenuControls.Right:
                        consumed = true;
                        if ((Collection == null ? Items.Count : Collection.Count) != 0)
                        {
                            Index++;
                            Common.PlaySound(menu.AUDIO_LEFTRIGHT, menu.AUDIO_LIBRARY);
                            menu.ListChange(this, Index);
                            ListChangedTrigger(Index);
                        }
                        break;
                }
            }

            return consumed;
        }

        protected internal override bool OnMouseInput(UIMenu menu, RectangleF itemBounds, PointF mousePos, MouseInput input)
        {
            // copied from UIMenuScrollerItem.OnMouseInput
            if (menu == null)
            {
                throw new ArgumentNullException(nameof(menu));
            }

            bool consumed = false;
            if (Selected && Hovered)
            {
                bool inSelectBounds = false;
                float selectBoundsX = itemBounds.X + itemBounds.Width * 0.33333f;

                if (mousePos.X <= selectBoundsX)
                {
                    inSelectBounds = true;
                    // approximately hovering the label, first 1/3 of the item width
                    // TODO: game shows cursor sprite 5 when hovering this part, but only if the item does something when selected.
                    //       Here, we don't really know if the user does something when selected, maybe add some bool property in UIMenuListItem?
                    if (input == MouseInput.JustReleased)
                    {
                        consumed = true;
                        OnInput(menu, Common.MenuControls.Select);
                    }
                }

                if (!inSelectBounds && ScrollingEnabled && Enabled && (input == MouseInput.JustPressed || input == MouseInput.PressedRepeat))
                {
                    UIMenu.GetTextureDrawSize(UIMenu.CommonTxd, UIMenu.ArrowRightTextureName, out float rightW, out _);

                    float rightX = (0.00390625f * 1.0f) + (rightW * /*0.5f*/1) + (0.0046875f * 0.75f);

                    if (menu.ScaleWithSafezone)
                    {
                        N.SetScriptGfxAlign('L', 'T');
                        N.SetScriptGfxAlignParams(-0.05f, -0.05f, 0.0f, 0.0f);
                    }
                    N.GetScriptGfxPosition(rightX, 0.0f, out rightX, out _);
                    N.GetScriptGfxPosition(0.0f, 0.0f, out float borderX, out _);
                    if (menu.ScaleWithSafezone)
                    {
                        N.ResetScriptGfxAlign();
                    }

                    rightX = itemBounds.Right - rightX + borderX;

                    // It does not check if the mouse in exactly on top of the arrow sprites intentionally:
                    //  - If to the right of the right arrow's left border, go right
                    //  - Anywhere else in the item, go left.
                    // This is how the vanilla menus behave
                    consumed = true;
                    if (mousePos.X >= rightX)
                    {
                        OnInput(menu, Common.MenuControls.Right);
                    }
                    else
                    {
                        OnInput(menu, Common.MenuControls.Left);
                    }
                }
            }

            return consumed;
        }

        internal virtual void ListChangedTrigger(int newindex)
        {
            OnListChanged?.Invoke(this, newindex);
        }

        public override string RightLabel { get => base.RightLabel; set => throw new Exception($"{nameof(UIMenuListItem)} cannot have a right label."); }

        #region Obsolete Stuff

        [Obsolete("Use UIMenu.SetKeyAcceleration(Common.MenuControls, AccelerationStep[]) instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public uint HoldTimeBeforeScroll = 200;

        #endregion
    }
}

