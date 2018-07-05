namespace RAGENativeUI
{
    using System;
    using System.Collections.Generic;

    /// <include file='..\Documentation\RAGENativeUI.DisplayItem.xml' path='D/IDisplayItem/Doc/*' />
    public interface IDisplayItem : IEquatable<IDisplayItem>
    {
        /// <include file='..\Documentation\RAGENativeUI.DisplayItem.xml' path='D/IDisplayItem/Member[@name="Value"]/*' />
        object Value { get; }
        /// <include file='..\Documentation\RAGENativeUI.DisplayItem.xml' path='D/IDisplayItem/Member[@name="DisplayText"]/*' />
        string DisplayText { get; }
    }

    /// <include file='..\Documentation\RAGENativeUI.DisplayItem.xml' path='D/DisplayItem/Doc/*' />
    public class DisplayItem : IDisplayItem
    {
        /// <include file='..\Documentation\RAGENativeUI.DisplayItem.xml' path='D/DisplayItem/Member[@name="NullValueDisplayText"]/*' />
        public string NullValueDisplayText { get; set; } = "{NULL}";

        /// <include file='..\Documentation\RAGENativeUI.DisplayItem.xml' path='D/IDisplayItem/Member[@name="Value"]/*' />
        public object Value { get; }

        private string displayText;
        /// <include file='..\Documentation\RAGENativeUI.DisplayItem.xml' path='D/DisplayItem/Member[@name="DisplayText"]/*' />
        public string DisplayText
        {
            get
            {
                if (!String.IsNullOrEmpty(displayText))
                    return displayText;
                return Value == null ? NullValueDisplayText : Value.ToString();
            }
        }

        /// <include file='..\Documentation\RAGENativeUI.DisplayItem.xml' path='D/DisplayItem/Member[@name="Ctor1"]/*' />
        public DisplayItem(object value, string displayText)
        {
            Value = value;
            this.displayText = displayText;
        }

        /// <include file='..\Documentation\RAGENativeUI.DisplayItem.xml' path='D/DisplayItem/Member[@name="Ctor2"]/*' />
        public DisplayItem(object value)
        {
            Value = value;
            this.displayText = String.Empty;
        }

        /// <include file='..\Documentation\RAGENativeUI.DisplayItem.xml' path='D/DisplayItem/Member[@name="Equals"]/*' />
        public bool Equals(IDisplayItem other)
        {
            if (other == null)
                return false;

            if (Value == null)
                return other.Value == null;
            return other.Value != null && Value.Equals(other.Value);
        }

        /// <include file='..\Documentation\RAGENativeUI.DisplayItem.xml' path='D/DisplayItem/Member[@name="ToString"]/*' />
        public override string ToString()
        {
            return DisplayText;
        }
    }



    /// <include file='..\Documentation\RAGENativeUI.DisplayItem.xml' path='D/DisplayItemsCollection/Doc/*' />
    public class DisplayItemsCollection : BaseCollection<IDisplayItem>
    {
        /// <include file='..\Documentation\RAGENativeUI.DisplayItem.xml' path='D/DisplayItemsCollection/Member[@name="Ctor1"]/*' />
        public DisplayItemsCollection() : base()
        {
        }

        /// <include file='..\Documentation\RAGENativeUI.DisplayItem.xml' path='D/DisplayItemsCollection/Member[@name="Ctor2"]/*' />
        public DisplayItemsCollection(IEnumerable<IDisplayItem> collection) : base(collection)
        {
        }

        /// <include file='..\Documentation\RAGENativeUI.DisplayItem.xml' path='D/DisplayItemsCollection/Member[@name="Add1"]/*' />
        public void Add(object value, string displayText)
        {
            base.Add(new DisplayItem(value, displayText));
        }

        /// <include file='..\Documentation\RAGENativeUI.DisplayItem.xml' path='D/DisplayItemsCollection/Member[@name="Add2"]/*' />
        public void Add(object value)
        {
            base.Add(new DisplayItem(value));
        }

        /// <include file='..\Documentation\RAGENativeUI.DisplayItem.xml' path='D/DisplayItemsCollection/Member[@name="Remove"]/*' />
        public void Remove(object value)
        {
            for (int i = Count - 1; i >= 0; i--)
            {
                if ((InternalList[i].Value == null && value == null) ||
                    (InternalList[i].Value.Equals(value)))
                    RemoveAt(i);
            }
        }

        /// <include file='..\Documentation\RAGENativeUI.DisplayItem.xml' path='D/DisplayItemsCollection/Member[@name="Contains"]/*' />
        public bool Contains(object value)
        {
            for (int i = 0; i < Count; i++)
            {
                if ((InternalList[i].Value == null && value == null) ||
                    (InternalList[i].Value.Equals(value)))
                    return true;
            }
            return false;
        }

        /// <include file='..\Documentation\RAGENativeUI.DisplayItem.xml' path='D/DisplayItemsCollection/Member[@name="IndexOf"]/*' />
        public int IndexOf(object value)
        {
            if (value == null)
            {
                for (int i = 0; i < Count; i++)
                {
                    if (InternalList[i].Value == null)
                        return i;
                }
            }
            else
            {
                for (int i = 0; i < Count; i++)
                {
                    if (value.Equals(InternalList[i].Value))
                        return i;
                }
            }

            return -1;
        }
    }
}

