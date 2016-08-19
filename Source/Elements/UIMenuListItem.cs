﻿using System;
using System.Collections.Generic;
using System.Drawing;

namespace RAGENativeUI.Elements
{
    public class UIMenuListItem : UIMenuItem
    {
        protected ResText _itemText;

        protected Sprite _arrowLeft;
        protected Sprite _arrowRight;

        public List<dynamic> Items;
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
        public event ItemListEvent OnListChanged = delegate { };

        protected int _index;
        
        /// <summary>
        /// Returns the current selected index.
        /// </summary>
        public virtual int Index
        {
            get { return _index % Items.Count; }
            set { _index = 100000 - (100000 % Items.Count) + value; }
        }


        /// <summary>
        /// List item, with left/right arrows.
        /// </summary>
        /// <param name="text">Item label.</param>
        /// <param name="items">List that contains your items.</param>
        /// <param name="index">Index in the list. If unsure user 0.</param>
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
        public UIMenuListItem(string text, List<dynamic> items, int index, string description)
            : base(text, description)
        {
            const int y = 0;
            Items = new List<dynamic>(items);
            _arrowLeft = new Sprite("commonmenu", "arrowleft", new Point(110, 105 + y), new Size(30, 30));
            _arrowRight = new Sprite("commonmenu", "arrowright", new Point(280, 105 + y), new Size(30, 30));
            _itemText = new ResText("", new Point(290, y + 104), 0.35f, Color.White, Common.EFont.ChaletLondon,
                ResText.Alignment.Left) {TextAlignment = ResText.Alignment.Right};
            Index = index;
        }


        /// <summary>
        /// Change item's position.
        /// </summary>
        /// <param name="y">New Y position.</param>
        public override void Position(int y)
        {
            _arrowLeft.Position = new Point(300 + Offset.X + Parent.WidthOffset, 147 + y + Offset.Y);
            _arrowRight.Position = new Point(400 + Offset.X + Parent.WidthOffset, 147 + y + Offset.Y);
            _itemText.Position = new Point(300 + Offset.X + Parent.WidthOffset, y + 147 + Offset.Y);
            base.Position(y);
        }


        /// <summary>
        /// Find an item in the list and return it's index.
        /// </summary>
        /// <param name="item">Item to search for.</param>
        /// <returns>Item index.</returns>
        public virtual int ItemToIndex(dynamic item)
        {
            return Items.FindIndex(item);
        }


        /// <summary>
        /// Find an item by it's index and return the item.
        /// </summary>
        /// <param name="index">Item's index.</param>
        /// <returns>Item</returns>
        public virtual dynamic IndexToItem(int index)
        {
            return Items[index];
        }


        /// <summary>
        /// Draw item.
        /// </summary>
        public override void Draw()
        {
            base.Draw();
            string caption = Items[Index % Items.Count].ToString();
            int offset = StringMeasurer.MeasureString(caption);

            _itemText.Color = Enabled ? Selected ? Color.Black : Color.WhiteSmoke : Color.FromArgb(163, 159, 148);
            
            _itemText.Caption = caption;

            _arrowLeft.Color = Enabled ? Selected ? Color.Black : Color.WhiteSmoke : Color.FromArgb(163, 159, 148);
            _arrowRight.Color = Enabled ? Selected ? Color.Black : Color.WhiteSmoke : Color.FromArgb(163, 159, 148);

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
            OnListChanged.Invoke(this, newindex);
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
