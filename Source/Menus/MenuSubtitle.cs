namespace RAGENativeUI.Menus
{
    using Graphics = Rage.Graphics;

    public class MenuSubtitle : IMenuComponent
    {
        private int counterTotalCount = 0, counterOnScreenSelectedIndex = 0;

        public Menu Menu { get; }
        public virtual string Text { get; set; }

        public MenuSubtitle(Menu menu, string text)
        {
            Menu = menu ?? throw new System.ArgumentNullException($"The component {nameof(Menu)} can't be null.");
            Menu.SelectedIndexChanged += OnMenuSelectedIndexChanged;
            Text = text;
        }

        public MenuSubtitle(Menu menu) : this(menu, null)
        {
        }

        ~MenuSubtitle()
        {
            Menu.SelectedIndexChanged -= OnMenuSelectedIndexChanged;
        }

        protected internal virtual string GetItemsCounterText()
        {
            if ((counterTotalCount == 0 && counterOnScreenSelectedIndex == 0) || counterTotalCount != Menu.Items.Count)
                UpdateCounter();
            return $"{counterOnScreenSelectedIndex}/{counterTotalCount} ";
        }

        public virtual void Process()
        {
        }

        public virtual void Draw(Graphics graphics, ref float x, ref float y)
        {
            Menu.Style.DrawSubtitle(graphics, this, ref x, ref y);
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

