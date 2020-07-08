using System;
using System.Drawing;

namespace RAGENativeUI.Elements
{
    public enum UIMenuCheckboxStyle
    {
        Tick,
        Cross,
    }

    public class UIMenuCheckboxItem : UIMenuItem
    {        
        /// <summary>
        /// Triggered when the checkbox state is changed.
        /// </summary>
        public event ItemCheckboxEvent CheckboxEvent;

        /// <summary>
        /// Change or get whether the checkbox is checked.
        /// </summary>
        public bool Checked { get; set; }

        public UIMenuCheckboxStyle Style { get; set; } = UIMenuCheckboxStyle.Tick;

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
        /// Draw item.
        /// </summary>
        public override void Draw(float x, float y, float menuWidth, float itemHeight)
        {
            base.Draw(x, y, menuWidth, itemHeight);

            var (name, color) = GetCheckboxTexture();

            UIMenu.GetTextureDrawSize(UIMenu.CommonTxd, name, out float w, out float h);

            GetBadgeOffsets(out _, out float badgeOffset);
            float spriteX = x + menuWidth - (w * 0.5f) - badgeOffset;
            float spriteY = y + (h * 0.5f) - (0.00138888f * 4.0f);

            UIMenu.DrawSprite(UIMenu.CommonTxd, name, spriteX, spriteY, w, h, color);
        }

        protected internal override bool OnInput(UIMenu menu, Common.MenuControls control)
        {
            bool consumed = base.OnInput(menu, control);

            switch (control)
            {
                case Common.MenuControls.Select when Enabled:
                    consumed = true;
                    Toggle(menu);
                    break;
            }

            return consumed;
        }

        public void Toggle(UIMenu menu = null)
        {
            Checked = !Checked;
            menu?.CheckboxChange(this, Checked);
            CheckboxEventTrigger();
        }

        public void CheckboxEventTrigger()
        {
            CheckboxEvent?.Invoke(this, Checked);
        }

        public override string RightLabel { get => base.RightLabel; set => throw new Exception($"{nameof(UIMenuCheckboxItem)} cannot have a right label."); }

        private (string Name, Color Color) GetCheckboxTexture()
        {
            static string GetSelected(UIMenuCheckboxStyle style) => style switch
            {
                UIMenuCheckboxStyle.Tick => UIMenu.CheckboxTickSelectedTextureName,
                UIMenuCheckboxStyle.Cross => UIMenu.CheckboxCrossSelectedTextureName,
                _ => throw new ArgumentException(),
            };

            static string GetUnselected(UIMenuCheckboxStyle style) => style switch
            {
                UIMenuCheckboxStyle.Tick => UIMenu.CheckboxTickTextureName,
                UIMenuCheckboxStyle.Cross => UIMenu.CheckboxCrossTextureName,
                _ => throw new ArgumentException(),
            };


            bool isDefaultHightlitedForeColor = HighlightedForeColor == DefaultHighlightedForeColor;
            
            string name;
            if (Selected && Enabled && isDefaultHightlitedForeColor)
            {
                name = Checked ? GetSelected(Style) : UIMenu.CheckboxBlankSelectedTextureName;
            
            }
            else
            {
                name = Checked ? GetUnselected(Style) : UIMenu.CheckboxBlankTextureName;
            }

            Color color = Enabled ?
                        (Selected && !isDefaultHightlitedForeColor) ? HighlightedForeColor : ForeColor :
                        DisabledForeColor;

            return (name, color);
        }
    }
}
