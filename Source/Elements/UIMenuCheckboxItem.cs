using System;
using System.Drawing;

namespace RAGENativeUI.Elements
{
    public class UIMenuCheckboxItem : UIMenuItem
    {
        private readonly Sprite _checkedSprite;
        
        /// <summary>
        /// Triggered when the checkbox state is changed.
        /// </summary>
        public event ItemCheckboxEvent CheckboxEvent;

        /// <summary>
        /// Checkbox item with a toggleable checkbox.
        /// </summary>
        /// <param name="text">Item label.</param>
        /// <param name="check">Boolean value whether the checkbox is checked.</param>
        public UIMenuCheckboxItem(string text, bool check)
            : this(text, check, "")
        {
        }

        /// <summary>
        /// Checkbox item with a toggleable checkbox.
        /// </summary>
        /// <param name="text">Item label.</param>
        /// <param name="check">Boolean value whether the checkbox is checked.</param>
        /// <param name="description">Description for this item.</param>
        public UIMenuCheckboxItem(string text, bool check, string description)
            : base(text, description)
        {
            const int y = 0;
            _checkedSprite = new Sprite("commonmenu", "shop_box_blank", new Point(410, y + 95), new Size(50, 50));
            Checked = check;
        }


        /// <summary>
        /// Change or get whether the checkbox is checked.
        /// </summary>
        public bool Checked { get; set; }
        
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
            base.SetVerticalPosition(y);
            _checkedSprite.Position = new Point(380 + Offset.X + Parent.WidthOffset, y + 138 + Offset.Y);
        }


        /// <summary>
        /// Draw item.
        /// </summary>
        public override void Draw()
        {
            base.Draw();
            _checkedSprite.Position = _checkedSprite.Position = new Point(380 + Offset.X + Parent.WidthOffset, _checkedSprite.Position.Y);
            bool isDefaultHightlitedForeColor = HighlightedForeColor == DefaultHighlightedForeColor;
            if (Selected && isDefaultHightlitedForeColor)
            {
                _checkedSprite.TextureName = Checked ? "shop_box_tickb" : "shop_box_blankb";
            }
            else
            {
                _checkedSprite.TextureName = Checked ? "shop_box_tick" : "shop_box_blank";
            }
            _checkedSprite.Color = Enabled ? (Selected && !isDefaultHightlitedForeColor) ? HighlightedForeColor : ForeColor : Color.FromArgb(163, 159, 148);
            _checkedSprite.Draw();
        }

        public void CheckboxEventTrigger()
        {
            CheckboxEvent?.Invoke(this, Checked);
        }

        public override void SetRightBadge(BadgeStyle badge)
        {
            throw new Exception("UIMenuCheckboxItem cannot have a right badge.");
        }

        public override void SetRightLabel(string text)
        {
            throw new Exception("UIMenuListItem cannot have a right label.");
        }
    }
}