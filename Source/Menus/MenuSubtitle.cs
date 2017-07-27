namespace RAGENativeUI.Menus
{
    using System.Drawing;
    
    using Graphics = Rage.Graphics;

    using RAGENativeUI.Rendering;

    public class MenuSubtitle : IMenuComponent
    {
        public Menu Menu { get; }

        public string Text { get; set; }
        public SizeF Size { get; set; } = new SizeF(Menu.DefaultWidth, 37f);

        protected bool ShowCounter { get { return Menu.IsAnyItemOnScreen && Menu.Items.Count > Menu.MaxItemsOnScreen; } }

        public float BorderSafezone { get; set; } = 8.5f;

        public MenuSubtitle(Menu menu)
        {
            Menu = menu;
            Menu.SelectedIndexChanged += OnMenuSelectedIndexChanged;
        }

        ~MenuSubtitle()
        {
            Menu.SelectedIndexChanged -= OnMenuSelectedIndexChanged;
        }

        public virtual void Process()
        {
        }

        public virtual void Draw(Graphics graphics, ref float x, ref float y)
        {
            graphics.DrawRectangle(new RectangleF(x, y, Size.Width, Size.Height), Color.Black);
            Menu.Skin.DrawText(graphics, Text, Menu.Skin.SubtitleFont, new RectangleF(x + BorderSafezone, y, Size.Width, Size.Height), Color.White, TextHorizontalAligment.Left, TextVerticalAligment.Center);

            if (ShowCounter)
            {
                // initial counter update
                if (counterTotalCount == 0 && counterOnScreenSelectedIndex == 0)
                    UpdateCounter();

                Menu.Skin.DrawText(graphics, $"{counterOnScreenSelectedIndex}/{counterTotalCount} ", Menu.Skin.SubtitleFont, new RectangleF(x, y, Size.Width, Size.Height), Color.White, TextHorizontalAligment.Right, TextVerticalAligment.Center);
            }

            y += Size.Height;
        }

        int counterTotalCount = 0;
        int counterOnScreenSelectedIndex = 0;
        private void UpdateCounter()
        {
            counterTotalCount = 0;
            counterOnScreenSelectedIndex = 0;

            int realSelectedIndex = Menu.SelectedIndex;

            for (int i = 0; i < Menu.Items.Count; i++)
            {
                MenuItem item = Menu.Items[i];

                if (item != null && item.IsVisible)
                {
                    counterTotalCount++;
                    if (i == realSelectedIndex)
                        counterOnScreenSelectedIndex = counterTotalCount;
                }
            }
        }

        private void OnMenuSelectedIndexChanged(Menu sender, int oldIndex, int newIndex)
        {
            UpdateCounter();
        }
    }
}

