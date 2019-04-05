namespace RAGENativeUI.Menus
{
    using System;

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
                if (value != isChecked)
                {
                    isChecked = value;
                    OnPropertyChanged(nameof(IsChecked));
                    OnCheckedChanged(new CheckedChangedEventArgs(isChecked));
                }
            }
        }

        public MenuItemCheckbox(string text, string description) : base(text, description)
        {
        }

        public MenuItemCheckbox(string text) : this(text, String.Empty)
        {
        }

        protected internal override void OnAccept()
        {
            if (IsDisabled)
            {
                return;
            }

            IsChecked = !IsChecked;
            base.OnAccept();
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

