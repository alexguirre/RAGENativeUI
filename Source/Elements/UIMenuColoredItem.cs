namespace RAGENativeUI.Elements
{
    using System;
    using System.Drawing;

    [Obsolete("UIMenuColoredItem is obsolete, use the UIMenuItem's color properties instead.")]
    public class UIMenuColoredItem : UIMenuItem
    {
        [Obsolete("UIMenuColoredItem is obsolete, use the UIMenuItem.BackColor property instead.")]
        public Color MainColor { get { return BackColor; } set { BackColor = value; } }
        [Obsolete("UIMenuColoredItem is obsolete, use the UIMenuItem.BackColor property instead.")]
        public Color HighlightColor { get { return HighlightedBackColor; } set { HighlightedBackColor = value; } }

        [Obsolete("UIMenuColoredItem is obsolete, use the UIMenuItem.TextColor property instead.")]
        public Color TextColor { get { return ForeColor; } set { ForeColor = value; } }
        [Obsolete("UIMenuColoredItem is obsolete, use the UIMenuItem.HighlightedTextColor property instead.")]
        public Color HighlightedTextColor { get { return HighlightedForeColor; } set { HighlightedForeColor = value; } }

        public UIMenuColoredItem(string label, Color color, Color highlightColor) : base(label)
        {
            MainColor = color;
            HighlightColor = highlightColor;

            TextColor = Color.White;
            HighlightedTextColor = Color.Black;
        }

        public UIMenuColoredItem(string label, string description, Color color, Color highlightColor) : base(label, description)
        {
            MainColor = color;
            HighlightColor = highlightColor;

            TextColor = Color.White;
            HighlightedTextColor = Color.Black;
        }

        protected void Init()
        {
        }
    }
}
