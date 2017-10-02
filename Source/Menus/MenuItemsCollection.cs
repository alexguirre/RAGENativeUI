namespace RAGENativeUI.Menus
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    
    using Graphics = Rage.Graphics;

    /// <include file='..\Documentation\RAGENativeUI.Menus.MenuItemsCollection.xml' path='D/MenuItemsCollection/Doc/*' />
    public class MenuItemsCollection : BaseCollection<MenuItem>, IDynamicHeightMenuComponent
    {
        public Menu Menu { get; private set; }

        public MenuItemsCollection() : base()
        {
        }

        public MenuItemsCollection(IEnumerable<MenuItem> items) : base(items)
        {
        }

        internal void SetMenu(Menu menu)
        {
            if (Menu != null && Menu != menu)
                throw new InvalidOperationException($"{nameof(MenuItemsCollection)} already set to a {nameof(Menus.Menu)}.");
            Menu = menu ?? throw new ArgumentNullException($"The {nameof(MenuItemsCollection)} {nameof(Menu)} can't be null.");
        }

        public float GetHeight()
        {
            float h = 0f;
            Menu.ForEachItemOnScreen((item, index) => h += Menu.Style.ItemHeight);
            return h;
        }

        public void Process()
        {
            int selectedIndex = Menu.SelectedIndex;
            for (int i = 0; i < Count; i++)
            {
                MenuItem item = this[i];
                item.Selected = i == selectedIndex;
                item?.OnProcess();
            }
        }

        public void Draw(Graphics g, ref float x, ref float y)
        {
            float currentX = x, currentY = y;
            Menu.ForEachItemOnScreen((item, index) =>
            {
                item.OnDraw(g, ref currentX, ref currentY);
            });
            x = currentX;
            y = currentY;
        }

        public override void Add(MenuItem item)
        {
            base.Add(item);
            item.SetParentInternal(Menu);
            Menu.UpdateVisibleItemsIndices();
        }

        public override void Insert(int index, MenuItem item)
        {
            base.Insert(index, item);
            item.SetParentInternal(Menu);
            Menu.UpdateVisibleItemsIndices();
        }

        public override bool Remove(MenuItem item)
        {
            bool b = base.Remove(item);
            if (b)
            {
                item.SetParentInternal(null);
                Menu.UpdateVisibleItemsIndices();
            }
            return b;
        }

        public override void RemoveAt(int index)
        {
            if (index >= 0 && index < Count)
            {
                this[index].SetParentInternal(null);
            }
            base.RemoveAt(index);
            Menu.UpdateVisibleItemsIndices();
        }

        public override void Clear()
        {
            foreach (MenuItem item in this)
            {
                item.SetParentInternal(null);
            }
            base.Clear();
            Menu.UpdateVisibleItemsIndices();
        }

        public MenuItem FindByText(string regexSearchPattern) => FindByText(regexSearchPattern, 0, Count - 1);
        public MenuItem FindByText(string regexSearchPattern, int startIndex, int endIndex)
        {
            return FindByInternal(regexSearchPattern, startIndex, endIndex, (m) => m.Text);
        }

        public MenuItem FindByDescription(string regexSearchPattern) => FindByText(regexSearchPattern, 0, Count - 1);
        public MenuItem FindByDescription(string regexSearchPattern, int startIndex, int endIndex)
        {
            return FindByInternal(regexSearchPattern, startIndex, endIndex, (m) => m.Description);
        }

        private MenuItem FindByInternal(string regexSearchPattern, int startIndex, int endIndex, Func<MenuItem, string> getInput)
        {
            return FindAllByInternal(regexSearchPattern, startIndex, endIndex, getInput).FirstOrDefault();
        }

        public IEnumerable<MenuItem> FindAllByText(string regexSearchPattern) => FindAllByText(regexSearchPattern, 0, Count - 1);
        public IEnumerable<MenuItem> FindAllByText(string regexSearchPattern, int startIndex, int endIndex)
        {
            return FindAllByInternal(regexSearchPattern, startIndex, endIndex, (m) => m.Text);
        }

        public IEnumerable<MenuItem> FindAllByDescription(string regexSearchPattern) => FindAllByText(regexSearchPattern, 0, Count - 1);
        public IEnumerable<MenuItem> FindAllByDescription(string regexSearchPattern, int startIndex, int endIndex)
        {
            return FindAllByInternal(regexSearchPattern, startIndex, endIndex, (m) => m.Description);
        }

        private IEnumerable<MenuItem> FindAllByInternal(string regexSearchPattern, int startIndex, int endIndex, Func<MenuItem, string> getInput)
        {
            if (regexSearchPattern == null)
                throw new ArgumentNullException(nameof(regexSearchPattern));
            if (getInput == null)
                throw new ArgumentNullException(nameof(getInput));

            for (int i = startIndex; i < endIndex; i++)
            {
                if (i > 0 && i < Count)
                {
                    MenuItem item = this[i];
                    string input = getInput(item);
                    if (input != null && Regex.IsMatch(getInput(item), regexSearchPattern, RegexOptions.IgnoreCase))
                    {
                        yield return item;
                    }
                }
            }
        }
    }
}

