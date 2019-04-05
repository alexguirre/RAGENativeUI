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
                int newIndex = RPH.MathHelper.Clamp(value, 0, RPH.MathHelper.Max(0, OptionCount - 1));

                if (newIndex != selectedIndex)
                {
                    int oldIndex = selectedIndex;
                    selectedIndex = newIndex;
                    OnPropertyChanged(nameof(SelectedIndex));
                    OnSelectedIndexChanged(new SelectedIndexChangedEventArgs(oldIndex, newIndex));
                    OnPropertyChanged(nameof(SelectedOptionText));
                }
            }
        }

        public abstract string SelectedOptionText { get; }
        public abstract int OptionCount { get; }

        public MenuItemScroller(string text, string description) : base(text, description)
        {
        }

        protected internal override void OnScrollingToPreviousValue()
        {
            if (IsDisabled)
            {
                return;
            }

            int newIndex = SelectedIndex - 1;

            if (newIndex < 0)
                newIndex = OptionCount - 1;

            SelectedIndex = newIndex;

            base.OnScrollingToPreviousValue();
        }

        protected internal override void OnScrollingToNextValue()
        {
            if (IsDisabled)
            {
                return;
            }

            int newIndex = SelectedIndex + 1;

            if (newIndex > (OptionCount - 1))
                newIndex = 0;

            SelectedIndex = newIndex;

            base.OnScrollingToNextValue();
        }

        protected virtual void OnSelectedIndexChanged(SelectedIndexChangedEventArgs e)
        {
            SelectedIndexChanged?.Invoke(this, e);
        }
    }
}

