namespace RAGENativeUI.Menus
{
    using System;

    using Rage;

    public abstract class MenuItemScroller : MenuItem
    {
        private int selectedIndex;

        public event TypedEventHandler<MenuItemScroller, SelectedIndexChangedEventArgs> SelectedIndexChanged;

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
                    OnPropertyChanged(nameof(SelectedIndex));
                    OnSelectedIndexChanged(new SelectedIndexChangedEventArgs(oldIndex, newIndex));
                }
            }
        }

        public MenuItemScroller(string text, string description) : base(text, description)
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

        protected virtual void OnSelectedIndexChanged(SelectedIndexChangedEventArgs e)
        {
            SelectedIndexChanged?.Invoke(this, e);
        }
    }
}

