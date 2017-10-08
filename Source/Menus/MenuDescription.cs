namespace RAGENativeUI.Menus
{
    using System;
    using System.Drawing;
    
    using Graphics = Rage.Graphics;
    
    public class MenuDescription : IDynamicHeightMenuComponent
    {
        private MenuItem currentItem;
        private string currentOrigText;
        private string currentFormattedText;
        private string textOverride = null;
        private float height;

        public Menu Menu { get; private set; }
        public virtual string Text { get { return currentOrigText; } }
        public virtual string FormattedText { get { return currentFormattedText; } }
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

        public MenuDescription()
        {
        }

        internal void SetMenu(Menu menu)
        {
            Throw.InvalidOperationIf(Menu != null && Menu != menu, $"{nameof(MenuDescription)} already set to a {nameof(Menus.Menu)}.");
            Throw.IfNull(menu, nameof(menu));

            Menu = menu;
        }

        public float GetHeight() => height;

        private void FormatCurrentText()
        {
            if (currentOrigText != null)
            {
                float h = 0.0f;
                if (!String.IsNullOrWhiteSpace(currentOrigText))
                {
                    currentFormattedText = Menu.Style.FormatDescriptionText(this, currentOrigText, out SizeF textSize);
                    h = textSize.Height;
                }
                else
                {
                    currentFormattedText = null;
                }

                height = h;
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
            Menu.Style.DrawDescription(graphics, this, ref x, ref y);
        }
    }
}

