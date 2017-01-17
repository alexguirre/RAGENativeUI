namespace RAGENativeUI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents an object with a value and the text that represents the value.
    /// </summary>
    /// <seealso cref="System.IEquatable{IDisplayItem}"/>
    /// <seealso cref="DisplayItem"/>
    public interface IDisplayItem : IEquatable<IDisplayItem>
    {
        /// <summary>
        /// Gets the value of this <see cref="IDisplayItem"/>.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        object Value { get; }
        /// <summary>
        /// Gets the display text of this <see cref="IDisplayItem"/>.
        /// </summary>
        /// <value>
        /// The display text.
        /// </value>
        string DisplayText { get; }
    }

    /// <summary>
    /// Default implementation of <see cref="IDisplayItem"/>
    /// </summary>
    /// <seealso cref="IDisplayItem" />
    public class DisplayItem : IDisplayItem
    {
        /// <summary>
        /// Gets or sets the <see cref="String"/> that <see cref="DisplayText"/> would return if <see cref="Value"/> is null.
        /// </summary>
        /// <value>
        /// The null value display text.
        /// </value>
        public string NullValueDisplayText { get; set; } = "{NULL}";

        /// <summary>
        /// Gets the value of this <see cref="DisplayItem"/>.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public object Value { get; }
        private string displayText;
        /// <summary>
        /// Gets the display text of this <see cref="DisplayItem"/>.
        /// <para>
        /// If no display text was specified in the constructor, this will return <see cref="Value"/>'s <see cref="object.ToString"/>.
        /// </para>
        /// <para>
        /// If <see cref="Value"/> is null and no display text was specified, this will return <see cref="NullValueDisplayText"/>.
        /// </para>
        /// </summary>
        /// <value>
        /// The display text.
        /// </value>
        public string DisplayText
        {
            get
            {
                if (displayText != null && displayText.Length > 0)
                    return displayText;
                if (Value == null)
                    return NullValueDisplayText;
                return Value.ToString();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayItem"/> struct with the specified value and display text.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="displayText">The display text.</param>
        public DisplayItem(object value, string displayText)
        {
            Value = value;
            this.displayText = displayText;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayItem"/> struct with the specified value and its <see cref="object.ToString"/> implementation as the display text.
        /// </summary>
        /// <param name="value">The value.</param>
        public DisplayItem(object value)
        {
            Value = value;
            this.displayText = String.Empty;
        }

        /// <summary>
        /// Indicates whether the current <see cref="DisplayItem"/>'s <see cref="Value"/> equals the other <see cref="IDisplayItem"/>'s <see cref="IDisplayItem.Value"/>.
        /// </summary>
        /// <param name="other">An <see cref="IDisplayItem"/> to compare with this <see cref="DisplayItem"/>.</param>
        /// <returns>
        /// <c>true</c> if the current <see cref="DisplayItem"/> has the same <see cref="Value"/> object as the <paramref name="other"/> <see cref="IDisplayItem"/> parameter; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(IDisplayItem other)
        {
            if (Value == null)
                return other.Value == null;
            if (other.Value == null)
                return false;
            return Value.Equals(other.Value);
        }

        /// <summary>
        /// Returns a <see cref="String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return DisplayText;
        }
    }

    /// <summary>
    /// Represents a collection of <see cref="IDisplayItem"/>.
    /// </summary>
    /// <seealso cref="System.Collections.Generic.ICollection{IDisplayItem}" />
    public class DisplayItemsCollection : ICollection<IDisplayItem>
    {
        public delegate void DisplayItemsCollectionItemChangedEventHandler(DisplayItemsCollection sender, IDisplayItem changedItem);
        public delegate void DisplayItemsCollectionClearedEventHandler(DisplayItemsCollection sender);

        private List<IDisplayItem> internalList;

        /// <summary>
        /// Occurs when an item is added to this <see cref="DisplayItemsCollection"/>.
        /// </summary>
        public event DisplayItemsCollectionItemChangedEventHandler ItemAdded;
        /// <summary>
        /// Occurs when an item is removed to this <see cref="DisplayItemsCollection"/>.
        /// </summary>
        public event DisplayItemsCollectionItemChangedEventHandler ItemRemoved;
        /// <summary>
        /// Occurs when all the items of this <see cref="DisplayItemsCollection"/> are removed using <see cref="Clear"/>.
        /// </summary>
        public event DisplayItemsCollectionClearedEventHandler Cleared;

        /// <summary>
        /// Gets the number of elements contained in this <see cref="DisplayItemsCollection"/>.
        /// </summary>
        public int Count { get { return internalList.Count; } }

        /// <summary>
        /// Gets a value indicating whether this <see cref="DisplayItemsCollection"/> is read-only.
        /// </summary>
        public bool IsReadOnly { get { return false; } }

        /// <summary>
        /// Gets the <see cref="IDisplayItem"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="IDisplayItem"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns>The <see cref="IDisplayItem"/> at the specified index.</returns>
        public IDisplayItem this[int index]
        {
            get { return internalList[index]; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayItemsCollection"/> class that is empty.
        /// </summary>
        public DisplayItemsCollection()
        {
            internalList = new List<IDisplayItem>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayItemsCollection"/> class that contains elements copied from the specified collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public DisplayItemsCollection(IEnumerable<IDisplayItem> collection)
        {
            internalList = new List<IDisplayItem>(collection);
        }

        /// <summary>
        /// Adds an <see cref="IDisplayItem"/> to this <see cref="DisplayItemsCollection" />.
        /// </summary>
        /// <param name="item">The <see cref="IDisplayItem"/> to add to this <see cref="DisplayItemsCollection" />.</param>
        public void Add(IDisplayItem item)
        {
            internalList.Add(item);
            ItemAdded?.Invoke(this, item);
        }

        /// <summary>
        /// Adds the specified value and display text using the default implementation of <see cref="IDisplayItem"/>, <see cref="DisplayItem"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="displayText">The display text.</param>
        public void Add(object value, string displayText)
        {
            Add(new DisplayItem(value, displayText));
        }

        /// <summary>
        /// Adds the specified value using the default implementation of <see cref="IDisplayItem"/>, <see cref="DisplayItem"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Add(object value)
        {
            Add(new DisplayItem(value));
        }

        /// <summary>
        /// Removes the <see cref="IDisplayItem"/> at the specified index from this <see cref="DisplayItemsCollection"/>.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            IDisplayItem i = internalList[index];
            internalList.RemoveAt(index);
            ItemRemoved?.Invoke(this, i);
        }

        /// <summary>
        /// Removes the first occurrence of the specified <see cref="IDisplayItem"/> from this <see cref="DisplayItemsCollection"/>.
        /// </summary>
        /// <param name="item">The <see cref="IDisplayItem"/> to remove from this <see cref="DisplayItemsCollection"/>.</param>
        /// <returns>
        /// <c>true</c> if the <paramref name="item"/> was successfully removed from this <see cref="DisplayItemsCollection"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="DisplayItemsCollection"/>.
        /// </returns>
        public bool Remove(IDisplayItem item)
        {
            bool result = internalList.Remove(item);
            if (result)
                ItemRemoved?.Invoke(this, item);
            return result;
        }

        /// <summary>
        /// Removes all the occurrences of <see cref="IDisplayItem"/>s which <see cref="IDisplayItem.Value"/> equals the specified <paramref name="value"/> from this <see cref="DisplayItemsCollection"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Remove(object value)
        {
            for (int i = Count - 1; i >= 0; i--)
            {
                if ((internalList[i].Value == null && value == null) ||
                    (internalList[i].Value.Equals(value)))
                    RemoveAt(i);
            }
        }

        /// <summary>
        /// Removes all items from this <see cref="DisplayItemsCollection"/>.
        /// </summary>
        public void Clear()
        {
            internalList.Clear();
            Cleared?.Invoke(this);
        }

        /// <summary>
        /// Determines whether this <see cref="DisplayItemsCollection"/> contains a specific <see cref="IDisplayItem"/>.
        /// </summary>
        /// <param name="item">The <see cref="IDisplayItem"/> to locate in this <see cref="DisplayItemsCollection"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in this <see cref="DisplayItemsCollection"/>; otherwise, false.
        /// </returns>
        public bool Contains(IDisplayItem item)
        {
            return internalList.Contains(item);
        }

        /// <summary>
        /// Determines whether this <see cref="DisplayItemsCollection"/> contains a <see cref="IDisplayItem"/> which <see cref="IDisplayItem.Value"/> equals the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// true if a <see cref="IDisplayItem"/> which <see cref="IDisplayItem.Value"/> equals the specified <paramref name="value"/> is found in this <see cref="DisplayItemsCollection"/>; otherwise, false.
        /// </returns>
        public bool Contains(object value)
        {
            for (int i = 0; i < Count; i++)
            {
                if ((internalList[i].Value == null && value == null) ||
                    (internalList[i].Value.Equals(value)))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Copies the elements of this <see cref="DisplayItemsCollection"/> to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from this <see cref="DisplayItemsCollection"/>. The <see cref="Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        public void CopyTo(IDisplayItem[] array, int arrayIndex)
        {
            internalList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Searches for the specified <see cref="IDisplayItem"/> and returns the zero-based index of the first occurrence within the entire <see cref="DisplayItemsCollection"/>.
        /// </summary>
        /// <param name="item">The <see cref="IDisplayItem"/>.</param>
        /// <returns>The zero-based index of the first occurrence of the item within the entire <see cref="DisplayItemsCollection"/>, if found; otherwise, –1.</returns>
        public int IndexOf(IDisplayItem item)
        {
            return internalList.IndexOf(item);
        }

        /// <summary>
        /// Searches for the <see cref="IDisplayItem"/> which <see cref="IDisplayItem.Value"/> equals the specified <paramref name="value"/> and returns the zero-based index of the first occurrence within the entire <see cref="DisplayItemsCollection"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The zero-based index of the first occurrence of the <see cref="IDisplayItem"/> which <see cref="IDisplayItem.Value"/> equals the specified <paramref name="value"/> within the entire <see cref="DisplayItemsCollection"/>, if found; otherwise, –1.</returns>
        public int IndexOf(object value)
        {
            if (value == null)
            {
                for (int i = 0; i < Count; i++)
                {
                    if (internalList[i].Value == null)
                        return i;
                }
            }
            else
            {
                for (int i = 0; i < Count; i++)
                {
                    if (value.Equals(internalList[i].Value))
                        return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<IDisplayItem> GetEnumerator()
        {
            return internalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return internalList.GetEnumerator();
        }
    }
}
