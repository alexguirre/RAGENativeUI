namespace RAGENativeUI.Menus
{
    using Rage;
    using Graphics = Rage.Graphics;

    public abstract class MenuItemScroller : MenuItem
    {
        public delegate void SelectedIndexChangedEventHandler(MenuItemScroller sender, int oldIndex, int newIndex);


        private int selectedIndex;

        public event SelectedIndexChangedEventHandler SelectedIndexChanged;

        public virtual int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                int newIndex = MathHelper.Clamp(value, 0, MathHelper.Max(0, GetOptionsCount() - 1));

                if (newIndex != selectedIndex)
                {
                    int oldIndex = selectedIndex;
                    selectedIndex = newIndex;
                    OnSelectedIndexChanged(oldIndex, newIndex);
                }
            }
        }

        public MenuItemScroller(string text) : base(text)
        {
        }

        public abstract string GetSelectedOptionText();
        public abstract int GetOptionsCount();

        protected internal override bool OnMoveLeft()
        {
            int newIndex = SelectedIndex - 1;

            if (newIndex < 0)
                newIndex = GetOptionsCount() - 1;

            SelectedIndex = newIndex;

            return base.OnMoveLeft();
        }

        protected internal override bool OnMoveRight()
        {
            int newIndex = SelectedIndex + 1;

            if (newIndex > (GetOptionsCount() - 1))
                newIndex = 0;

            SelectedIndex = newIndex;

            return base.OnMoveRight();
        }

        protected internal override void OnDraw(Graphics graphics, ref float x, ref float y)
        {
            if (!IsVisible)
                return;

            Parent.Style.DrawItemScroller(graphics, this, ref x, ref y);
        }

        protected virtual void OnSelectedIndexChanged(int oldIndex, int newIndex)
        {
            SelectedIndexChanged?.Invoke(this, oldIndex, newIndex);
        }
    }
}

