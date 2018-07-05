namespace RAGENativeUI.Menus
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    
    using Graphics = Rage.Graphics;
    
    public class MenuItemsCollection : BaseCollection<MenuItem>
    {
        public Menu Owner { get; }

        public MenuItemsCollection(Menu owner) : base()
        {
            Owner = owner;
        }

        public override void Add(MenuItem item)
        {
            base.Add(item);
            item.Parent = Owner;
            Owner.UpdateVisibleItemsIndices();
        }

        public override void Insert(int index, MenuItem item)
        {
            base.Insert(index, item);
            item.Parent = Owner;
            Owner.UpdateVisibleItemsIndices();
        }

        public override bool Remove(MenuItem item)
        {
            bool b = base.Remove(item);
            if (b)
            {
                item.Parent = null;
                Owner.UpdateVisibleItemsIndices();
            }
            return b;
        }

        public override void RemoveAt(int index)
        {
            if (index >= 0 && index < Count)
            {
                this[index].Parent = null;
            }
            base.RemoveAt(index);
            Owner.UpdateVisibleItemsIndices();
        }

        public override void Clear()
        {
            foreach (MenuItem item in this)
            {
                item.Parent = null;
            }
            base.Clear();
            Owner.UpdateVisibleItemsIndices();
        }

        public virtual void ClearAndAdd(IEnumerable<MenuItem> items)
        {
            foreach (MenuItem item in this)
            {
                item.Parent = null;
            }
            base.Clear();

            foreach (MenuItem item in items)
            {
                base.Add(item);
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

