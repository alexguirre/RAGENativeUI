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
        public virtual SizeF Size
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
        
        private MenuItem currentItem;
        private string currentOrigText;
        private string currentFormattedText;

        public virtual string Text { get { return currentOrigText; } }
        public virtual string FormattedText { get { return currentFormattedText; } }

        private string textOverride = null;
        public virtual string TextOverride
        {
            get { return textOverride; }
            set
            {
                if (value == textOverride)
                    return;

                textOverride = value;
                if (textOverride != null)
                {
                    currentItem = null;
                    currentOrigText = textOverride;
                    currentFormattedText = currentOrigText;
                    FormatCurrentText();
                }
                else
                {
                    currentItem = null;
                    currentOrigText = null;
                    currentFormattedText = null;
                }
            }
        }

        public MenuDescription(Menu menu)
        {
            Menu = menu;
        }

        private void FormatCurrentText()
        {
            if (currentOrigText != null)
            {
                float height = 0.0f;
                if (!String.IsNullOrWhiteSpace(currentOrigText))
                {
                    currentFormattedText = Menu.Skin.FormatDescriptionText(this, currentOrigText, out SizeF textSize);
                    height = textSize.Height;
                }
                else
                {
                    currentFormattedText = null;
                }

                Size = new SizeF(Size.Width, height);
            }
        }

        public virtual void Process()
        {
            if (textOverride == null)
            {
                MenuItem selectedItem = Menu.SelectedItem;
                if (selectedItem != null && selectedItem.IsVisible)
                {
                    if (currentItem != selectedItem || selectedItem.Description != currentOrigText)
                    {
                        currentItem = selectedItem;
                        currentOrigText = selectedItem.Description;
                        currentFormattedText = currentOrigText;
                        FormatCurrentText();
                    }
                }
                else
                {
                    currentItem = null;
                    currentOrigText = null;
                    currentFormattedText = null;
                }
            }
        }

        public virtual void Draw(Graphics graphics, ref float x, ref float y)
        {
            Menu.Skin.DrawDescription(graphics, this, x, y);
            y += currentFormattedText == null ? 0.0f : Size.Height;
        }
    }
}

