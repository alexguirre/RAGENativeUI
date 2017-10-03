namespace RAGENativeUI.Menus
{
    using Graphics = Rage.Graphics;
    
    public class MenuItemCheckbox : MenuItem
    {
        private bool isChecked;

        public event TypedEventHandler<MenuItemCheckbox, CheckedChangedEventArgs> CheckedChanged;
        public event TypedEventHandler<MenuItemCheckbox, CheckedChangedEventArgs> Checked;
        public event TypedEventHandler<MenuItemCheckbox, CheckedChangedEventArgs> Unchecked;

        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                if (value == isChecked)
                    return;

                isChecked = value;
                OnCheckedChanged(new CheckedChangedEventArgs(isChecked));
            }
        }

        public MenuItemCheckbox(string text) : base(text)
        {
        }

        protected internal override bool OnAccept()
        {
            IsChecked = !IsChecked;

            return base.OnAccept();
        }

        protected internal override void OnDraw(Graphics graphics, ref float x, ref float y)
        {
            if (!IsVisible)
                return;

            Parent.Style.DrawItemCheckbox(graphics, this, ref x, ref y);
        }

        protected virtual void OnCheckedChanged(CheckedChangedEventArgs e)
        {
            CheckedChanged?.Invoke(this, e);
            if (e.IsChecked)
                Checked?.Invoke(this, e);
            else
                Unchecked?.Invoke(this, e);
        }
    }
}

