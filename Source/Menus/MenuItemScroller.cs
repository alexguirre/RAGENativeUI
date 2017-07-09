namespace RAGENativeUI.Menus
{
    using System.Drawing;

    using Rage;
    using Graphics = Rage.Graphics;

    using RAGENativeUI.Rendering;
    using RAGENativeUI.Menus.Rendering;

    public abstract class MenuItemScroller : MenuItem
    {
        private int selectedIndex;
        public int SelectedIndex { get { return selectedIndex; } set { selectedIndex = MathHelper.Clamp(value, 0, GetOptionsCount()); } }

        public MenuItemScroller(string text) : base(text)
        {
        }

        protected abstract string GetSelectedOptionText();
        protected abstract int GetOptionsCount();
        
        protected internal override bool OnPreviewMoveLeft(Menu menuSender)
        {
            int newIndex = SelectedIndex - 1;

            if (newIndex < 0)
                newIndex = GetOptionsCount() - 1;

            SelectedIndex = newIndex;

            return true;
        }

        protected internal override bool OnPreviewMoveRight(Menu menuSender)
        {
            int newIndex = SelectedIndex + 1;

            if (newIndex > (GetOptionsCount() - 1))
                newIndex = 0;

            SelectedIndex = newIndex;

            return true;
        }

        public override void Draw(Graphics graphics, MenuSkin skin, bool selected, ref float x, ref float y)
        {
            if (selected)
            {
                skin.DrawSelectedGradient(graphics, x, y, Size.Width, Size.Height);
                skin.DrawText(graphics, Text, "Arial", 20.0f, new RectangleF(x + TextSafezone, y, Size.Width , Size.Height), Color.FromArgb(225, 10, 10, 10));
                skin.DrawArrowRight(graphics, x + Size.Width - Size.Height, y + TextSafezone * 0.5f, Size.Height - TextSafezone, Size.Height - TextSafezone);
                skin.DrawText(graphics, GetSelectedOptionText(), "Arial", 20.0f, new RectangleF(x, y, Size.Width - Size.Height / 1.5f - TextSafezone, Size.Height), Color.FromArgb(225, 10, 10, 10), TextHorizontalAligment.Right);
                SizeF textSize = Graphics.MeasureText(GetSelectedOptionText(), "Arial", 20.0f);
                skin.DrawArrowLeft(graphics, x + Size.Width - Size.Height - textSize.Width - TextSafezone * 2.5f, y + TextSafezone * 0.5f, Size.Height - TextSafezone, Size.Height - TextSafezone);
            }
            else
            {
                skin.DrawText(graphics, Text, "Arial", 20.0f, new RectangleF(x + TextSafezone, y, Size.Width, Size.Height), Color.FromArgb(240, 240, 240, 240));
                skin.DrawText(graphics, GetSelectedOptionText(), "Arial", 20.0f, new RectangleF(x, y, Size.Width - TextSafezone, Size.Height), Color.FromArgb(240, 240, 240, 240), TextHorizontalAligment.Right);
            }

            y += Size.Height;
        }
    }
}

