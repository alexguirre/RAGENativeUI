namespace RAGENativeUI.Menus
{
    using Graphics = Rage.Graphics;
    
    public class MenuItemCheckbox : MenuItem
    {
        public delegate void CheckedChangedEventHandler(MenuItem sender, bool isChecked);

        public event CheckedChangedEventHandler CheckedChanged;
        public event CheckedChangedEventHandler Checked;
        public event CheckedChangedEventHandler Unchecked;

        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                if (value == isChecked)
                    return;

                isChecked = value;
                OnCheckChanged(isChecked);
            }
        }

        public MenuItemCheckbox(string text) : base(text)
        {
        }

        protected internal override bool OnAccept(Menu origin)
        {
            IsChecked = !IsChecked;

            return base.OnAccept(origin);
        }

        protected internal override void OnDraw(Graphics graphics, Menu sender, bool selected, ref float x, ref float y)
        {
            if (!IsVisible)
                return;

            sender.Style.DrawItemCheckbox(graphics, this, ref x, ref y, selected);
        }

        protected virtual void OnCheckChanged(bool isChecked)
        {
            CheckedChanged?.Invoke(this, isChecked);
            if (isChecked)
                Checked?.Invoke(this, isChecked);
            else
                Unchecked?.Invoke(this, isChecked);
        }
    }
}

