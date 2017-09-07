namespace RAGENativeUI.Menus
{
    using System.Linq;
    using System.Drawing;
    
    using Graphics = Rage.Graphics;

    using RAGENativeUI.Rendering;

    public class MenuSubtitle : IMenuComponent
    {
        public Menu Menu { get; }

        public virtual string Text { get; set; }
        public virtual SizeF Size { get; set; } = new SizeF(Menu.DefaultWidth, 36f);

        private int counterTotalCount = 0, counterOnScreenSelectedIndex = 0;

        public MenuSubtitle(Menu menu)
        {
            Menu = menu;
            Menu.SelectedIndexChanged += OnMenuSelectedIndexChanged;
        }

        ~MenuSubtitle()
        {
            Menu.SelectedIndexChanged -= OnMenuSelectedIndexChanged;
        }

        protected internal virtual bool ShouldShowItemsCounter() => Menu.IsAnyItemOnScreen && Menu.GetOnScreenItemsCount() < Menu.Items.Sum(i => i.IsVisible ? 1 : 0);
        protected internal virtual string GetItemsCounterText()
        {
            if (counterTotalCount == 0 && counterOnScreenSelectedIndex == 0)
                UpdateCounter();
            return $"{counterOnScreenSelectedIndex}/{counterTotalCount} ";
        }

        public virtual void Process()
        {
        }

        public virtual void Draw(Graphics graphics, ref float x, ref float y)
        {
            Menu.Skin.DrawSubtitle(graphics, this, x, y);
            y += Size.Height;
        }

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

