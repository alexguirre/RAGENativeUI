namespace RAGENativeUI.Menus
{
    using System.Drawing;

    using Rage;
    using Graphics = Rage.Graphics;

    using RAGENativeUI.Rendering;

    public abstract class MenuItemScroller : MenuItem
    {
        public delegate void SelectedIndexChangedEventHandler(MenuItemScroller sender, int oldIndex, int newIndex);


        public event SelectedIndexChangedEventHandler SelectedIndexChanged;

        private int selectedIndex;
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
        
        protected internal override bool OnPreviewMoveLeft(Menu origin)
        {
            if (base.OnPreviewMoveLeft(origin))
            {
                int newIndex = SelectedIndex - 1;

                if (newIndex < 0)
                    newIndex = GetOptionsCount() - 1;

                SelectedIndex = newIndex;

                return true;
            }

            return false;
        }

        protected internal override bool OnPreviewMoveRight(Menu origin)
        {
            if (base.OnPreviewMoveRight(origin))
            {
                int newIndex = SelectedIndex + 1;

                if (newIndex > (GetOptionsCount() - 1))
                    newIndex = 0;

                SelectedIndex = newIndex;

                return true;
            }

            return false;
        }

        protected internal override void OnDraw(Graphics graphics, Menu sender, bool selected, ref float x, ref float y)
        {
            if (!IsVisible)
                return;

            sender.Skin.DrawItemScroller(graphics, this, x, y, selected);
            y += Size.Height;
        }

        protected virtual void OnSelectedIndexChanged(int oldIndex, int newIndex)
        {
            SelectedIndexChanged?.Invoke(this, oldIndex, newIndex);
        }
    }
}

