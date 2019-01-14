namespace RAGENativeUI.Menus.Themes
{
#if RPH1
    extern alias rph1;
    using Graphics = rph1::Rage.Graphics;
    using Vector2 = rph1::Rage.Vector2;
#else
    /** REDACTED **/
#endif
    
    using System.Drawing;
    
    public class MenuDebugTheme : MenuTheme
    {
        readonly Vector2 position = new Vector2(15.0f, 65.0f);
        readonly GraphicsFont titleFont = new GraphicsFont("Consolas", 45.0f);
        readonly GraphicsFont subtitleFont = new GraphicsFont("Consolas", 30.0f);
        readonly GraphicsFont itemFont = new GraphicsFont("Consolas", 26.5f);
        readonly float width = 500.0f;

        public MenuDebugTheme(Menu menu) : base(menu)
        {
        }

        public override MenuTheme Clone(Menu menu)
        {
            return new MenuDebugTheme(menu);
        }

        public override void Draw(Graphics g)
        {
            g.DrawRectangle(new RectangleF(position.X, position.Y, width, RPH.Resolution.Height - position.Y - 15.0f), Color.FromArgb(80, Color.Black));


            float y = position.Y;
            if (Menu.Title != null)
            {
#if RPH1
                g.DrawText(Menu.Title, titleFont.Name, titleFont.Size, new PointF(position.X, y), Color.White);
#else
                /** REDACTED **/
#endif
            }
            y += 6.0f + titleFont.Height;
            if (Menu.Subtitle != null)
            {
#if RPH1
                g.DrawText(Menu.Subtitle, subtitleFont.Name, subtitleFont.Size, new PointF(position.X, y), Color.White);
#else
                /** REDACTED **/
#endif
            }
            y += 6.0f + subtitleFont.Height * 2.0f;

            g.DrawLine(new Vector2(position.X, y), new Vector2(position.X + width, y), Color.White);

            y += 6.0f;

            Menu.ForEachItemOnScreen((item, index) =>
            {
                DrawItem(g, item, ref y);
            });

            y += itemFont.Height;

            g.DrawLine(new Vector2(position.X, y), new Vector2(position.X + width, y), Color.White);

            y += 6.0f;

            if (Menu.CurrentDescription != null)
            {
#if RPH1
                g.DrawText(Menu.CurrentDescription, itemFont.Name, itemFont.Size, new PointF(position.X, y), Color.White);
#else
                /** REDACTED **/
#endif
                y += 4.0f + itemFont.Height;
            }


            y = position.Y;
            float x = position.X + width + 10.0f;
#if RPH1
            g.DrawText($"OnScreen -> Start {Menu.ItemsOnScreenStartIndex}, End {Menu.ItemsOnScreenEndIndex}, Max {Menu.MaxItemsOnScreen}", "Consolas", 25.0f, new PointF(x, y), Color.DarkRed);
#else
            /** REDACTED **/
#endif
        }

        private void DrawItem(Graphics g, MenuItem item, ref float y)
        {
            string t = item.IsSelected ? (" > " + item.Text) : item.Text;
            Color c = item.IsDisabled ? Color.DimGray : GetItemTextColor(item);

#if RPH1
            g.DrawText(t, itemFont.Name, itemFont.Size, new PointF(position.X, y), c);
#else
            /** REDACTED **/
#endif
            switch (item)
            {
                case MenuItemScroller scroller:
                    {
#if RPH1
                        g.DrawText("<" + scroller.GetSelectedOptionText() + ">", itemFont.Name, itemFont.Size, new PointF(position.X, y), c);
#else
                        /** REDACTED **/
#endif
                        break;
                    }
                case MenuItemCheckbox checkbox:
                    {
#if RPH1
                        g.DrawText("[" + (checkbox.IsChecked ? "X" : " ") + "]", itemFont.Name, itemFont.Size, new PointF(position.X, y), c);
#else
                        /** REDACTED **/
#endif
                        break;
                    }
            }
            
            y += 4.0f + itemFont.Height;
        }

        public void SetItemTextColor(MenuItem item, Color color)
        {
            item.Metadata[ItemTextColorMetadataKey] = color;
        }

        public Color GetItemTextColor(MenuItem item)
        {
            if (item.Metadata.TryGetValue<Color>(ItemTextColorMetadataKey, out Color c))
            {
                return c;
            }
            return Color.White;
        }

        public const string ItemTextColorMetadataKey = "TextColor";
    }
}

