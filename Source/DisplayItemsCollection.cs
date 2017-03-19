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
    /// <seealso cref="BaseCollection{IDisplayItem}" />
    public class DisplayItemsCollection : BaseCollection<IDisplayItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayItemsCollection"/> class that is empty.
        /// </summary>
        public DisplayItemsCollection() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayItemsCollection"/> class that contains elements copied from the specified collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public DisplayItemsCollection(IEnumerable<IDisplayItem> collection) : base(collection)
        {
        }

        /// <summary>
        /// Adds the specified value and display text using the default implementation of <see cref="IDisplayItem"/>, <see cref="DisplayItem"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="displayText">The display text.</param>
        public void Add(object value, string displayText)
        {
            base.Add(new DisplayItem(value, displayText));
        }

        /// <summary>
        /// Adds the specified value using the default implementation of <see cref="IDisplayItem"/>, <see cref="DisplayItem"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Add(object value)
        {
            base.Add(new DisplayItem(value));
        }

        /// <summary>
        /// Removes all the occurrences of <see cref="IDisplayItem"/>s which <see cref="IDisplayItem.Value"/> equals the specified <paramref name="value"/> from this <see cref="DisplayItemsCollection"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Remove(object value)
        {
            for (int i = Count - 1; i >= 0; i--)
            {
                if ((InternalList[i].Value == null && value == null) ||
                    (InternalList[i].Value.Equals(value)))
                    RemoveAt(i);
            }
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
                if ((InternalList[i].Value == null && value == null) ||
                    (InternalList[i].Value.Equals(value)))
                    return true;
            }
            return false;
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
