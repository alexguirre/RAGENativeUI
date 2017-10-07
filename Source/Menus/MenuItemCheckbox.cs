namespace RAGENativeUI.Menus
{
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

