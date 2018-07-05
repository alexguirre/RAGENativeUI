namespace RAGENativeUI.Menus.Themes
{
    using System;
    using System.Drawing;

    using Rage;
    using Graphics = Rage.Graphics;

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
            g.DrawRectangle(new RectangleF(position.X, position.Y, width, Game.Resolution.Height - position.Y - 15.0f), Color.FromArgb(80, Color.Black));


            float y = position.Y;
            if (Menu.Title != null)
            {
                /** REDACTED **/
            }
            y += 6.0f + titleFont.Height;
            if (Menu.Subtitle != null)
            {
                /** REDACTED **/
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
                g.DrawText(Menu.CurrentDescription, itemFont.Name, itemFont.Size, Color.White, new RectangleF(position.X, y, width, 100.0f), null);
                y += 4.0f + itemFont.Height;
            }


            y = position.Y;
            float x = position.X + width + 10.0f;
            /** REDACTED **/
        }

        private void DrawItem(Graphics g, MenuItem item, ref float y)
        {
            string t = item.IsSelected ? (" > " + item.Text) : item.Text;
            Color c = item.IsDisabled ? Color.DimGray : GetItemTextColor(item);

            /** REDACTED **/
            switch (item)
            {
                case MenuItemScroller scroller:
                    {
                        /** REDACTED **/
                        break;
                    }
                case MenuItemCheckbox checkbox:
                    {
                        /** REDACTED **/
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

