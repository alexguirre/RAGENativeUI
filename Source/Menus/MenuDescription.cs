namespace RAGENativeUI.Menus
{
    using System;
    using System.Drawing;
    
    using Graphics = Rage.Graphics;

    using RAGENativeUI.Rendering;

    public class MenuDescription : IMenuComponent
    {
        public Menu Menu { get; }

        private SizeF size = new SizeF(Menu.DefaultWidth, 0f);
        public SizeF Size
        {
            get { return size; }
            set
            {
                if (value == size)
                    return;

                bool widthChanged = value.Width != size.Width;
                size = value;
                if (widthChanged)
                {
                    FormatCurrentText();
                }
            }
        } 

        public float BorderSafezone { get; set; } = 8.5f;

        private MenuItem currentItem;
        private string currentOrigText;
        private string currentText;

        public MenuDescription(Menu menu)
        {
            Menu = menu;
        }

        private void FormatCurrentText()
        {
            if (currentText != null)
            {
                currentText = String.IsNullOrWhiteSpace(currentText) ? null : Common.WrapText(currentText.Replace('\n', ' '), Menu.Skin.DescriptionFont, Size.Width - BorderSafezone * 2f);

                Size = new SizeF(Size.Width, Menu.Skin.DescriptionFont.Measure(currentText).Height + BorderSafezone * 3f);
            }
        }

        public virtual void Process()
        {
            MenuItem selectedItem = Menu.SelectedItem;
            if (selectedItem != null)
            {
                if (currentItem != selectedItem || selectedItem.Description != currentOrigText)
                {
                    currentItem = selectedItem;
                    currentOrigText = selectedItem.Description;
                    currentText = currentOrigText;
                    FormatCurrentText();
                }
            }
            else
            {
                currentItem = null;
                currentText = null;
            }
        }

        public virtual void Draw(Graphics graphics, ref float x, ref float y)
        {
            if (currentText != null && Size.Height > 0f)
            {
                y += 4f;

                graphics.DrawRectangle(new RectangleF(x, y, Size.Width, 3.25f), Color.FromArgb(240, 0, 0, 0));
                graphics.DrawRectangle(new RectangleF(x, y, Size.Width, Size.Height), Color.FromArgb(95, 0, 0, 0));

                Menu.Skin.DrawText(graphics, currentText, Menu.Skin.DescriptionFont, new RectangleF(x + BorderSafezone, y + BorderSafezone, Size.Width - BorderSafezone * 2f, Size.Height), Color.White, TextHorizontalAligment.Left, TextVerticalAligment.Top);

                y += Size.Height;
            }
        }
    }
}

