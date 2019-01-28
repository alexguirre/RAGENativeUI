namespace RAGENativeUI.Menus
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text.RegularExpressions;
    
    public class MenuItemsCollection : Collection<MenuItem>
    {
        public Menu Owner { get; }

        public MenuItemsCollection(Menu owner) : base()
        {
            Owner = owner;
        }

        protected override void ClearItems()
        {
            foreach (MenuItem item in this)
            {
                item.Parent = null;
            }
            base.ClearItems();
            Owner.UpdateVisibleItemsIndices();
        }

        protected override void InsertItem(int index, MenuItem item)
        {
            base.InsertItem(index, item);
            item.Parent = Owner;
            Owner.UpdateVisibleItemsIndices();
        }

        protected override void RemoveItem(int index)
        {
            Items[index].Parent = null;
            base.RemoveItem(index);
            Owner.UpdateVisibleItemsIndices();
        }

        protected override void SetItem(int index, MenuItem item)
        {
            if (Items[index] != item)
            {
                Items[index].Parent = null;
                base.SetItem(index, item);
                item.Parent = Owner;
                Owner.UpdateVisibleItemsIndices();
            }
        }

        public virtual void Replace(IEnumerable<MenuItem> items)
        {
            // the enumerable may reference the internal list,
            // which will be empty after Clear(), so get all the
            // enumerable contents
            MenuItem[] copy = items.ToArray(); 

            foreach (MenuItem item in this)
            {
                item.Parent = null;
            }
            Items.Clear();

            foreach (MenuItem item in copy)
            {
                Items.Add(item);
                item.Parent = Owner;
            }

            Owner.UpdateVisibleItemsIndices();
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
            Throw.IfNull(regexSearchPattern, nameof(regexSearchPattern));
            Throw.IfNull(getInput, nameof(getInput));

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

