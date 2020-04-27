using System;
using System.Drawing;

namespace RAGENativeUI.Elements
{
    public class UIMenuCheckboxItem : UIMenuItem
    {        
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
            Checked = check;
        }


        /// <summary>
        /// Change or get whether the checkbox is checked.
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// Draw item.
        /// </summary>
        public override void Draw(float x, float y, float menuWidth, float itemHeight)
        {
            base.Draw(x, y, menuWidth, itemHeight);

            string cbName;
            bool isDefaultHightlitedForeColor = HighlightedForeColor == DefaultHighlightedForeColor;
            if (Selected && Enabled && isDefaultHightlitedForeColor)
            {
                cbName = Checked ? UIMenu.CheckboxTickSelectedTextureName : UIMenu.CheckboxBlankSelectedTextureName;
            }
            else
            {
                cbName = Checked ? UIMenu.CheckboxTickTextureName : UIMenu.CheckboxBlankTextureName;
            }

            UIMenu.GetTextureDrawSize(UIMenu.CommonTxd, cbName, out float w, out float h);

            GetBadgeOffsets(out _, out float badgeOffset);
            float spriteX = x + menuWidth - (w * 0.5f) - badgeOffset;
            float spriteY = y + (h * 0.5f) - (0.00138888f * 4.0f);

            Color c = Enabled ? (Selected && !isDefaultHightlitedForeColor) ? HighlightedForeColor : ForeColor : DisabledForeColor;
            UIMenu.DrawSprite(UIMenu.CommonTxd, cbName, spriteX, spriteY, w, h, c);
        }

        public void CheckboxEventTrigger()
        {
            CheckboxEvent?.Invoke(this, Checked);
        }

        public override string RightLabel { get => base.RightLabel; set => throw new Exception($"{nameof(UIMenuCheckboxItem)} cannot have a right label."); }
    }
}
