namespace RAGENativeUI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class BaseCollection<T> : ICollection<T>
    {
        public delegate void CollectionItemAddedEventHandler(BaseCollection<T> sender, T addedItem);
        public delegate void CollectionItemRemovedEventHandler(BaseCollection<T> sender, T removedItem);
        public delegate void CollectionItemModifiedEventHandler(BaseCollection<T> sender, T oldItem, T newItem);
        public delegate void CollectionClearedEventHandler(BaseCollection<T> sender);

        protected List<T> InternalList { get; set; }

        /// <summary>
        /// Occurs when an item is added to this <see cref="BaseCollection{T}"/> using <see cref="Add(T)"/>.
        /// </summary>
        public virtual event CollectionItemAddedEventHandler ItemAdded;
        /// <summary>
        /// Occurs when an item is removed from this <see cref="BaseCollection{T}"/> using <see cref="Remove(T)"/> or <see cref="RemoveAt(int)"/>.
        /// </summary>
        public virtual event CollectionItemRemovedEventHandler ItemRemoved;
        /// <summary>
        /// Occurs when an item is modified in this <see cref="BaseCollection{T}"/> using <see cref="this[int]"/>.
        /// </summary>
        public virtual event CollectionItemModifiedEventHandler ItemModified;
        /// <summary>
        /// Occurs when all the items of this <see cref="BaseCollection{T}"/> are removed using <see cref="Clear"/>.
        /// </summary>
        public virtual event CollectionClearedEventHandler Cleared;

        /// <summary>
        /// Gets the number of elements contained in this <see cref="BaseCollection{T}"/>.
        /// </summary>
        public virtual int Count { get { return InternalList.Count; } }

        /// <summary>
        /// Gets a value indicating whether this <see cref="BaseCollection{T}"/> is read-only.
        /// </summary>
        public virtual bool IsReadOnly { get { return false; } }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <value>
        /// The item.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns>
        /// The item at the specified index.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        public virtual T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return InternalList[index];
            }
            set
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                T old = InternalList[index];
                if(!EqualityComparer<T>.Default.Equals(old, value))
                {
                    InternalList[index] = value;
                    ItemModified?.Invoke(this, old, value);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCollection{T}"/> class that is empty.
        /// </summary>
        public BaseCollection()
        {
            InternalList = new List<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCollection{T}"/> class that contains elements copied from the specified collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public BaseCollection(IEnumerable<T> collection)
        {
            InternalList = new List<T>(collection);
        }

        /// <summary>
        /// Adds an item to this <see cref="BaseCollection{T}" />.
        /// </summary>
        /// <param name="item">The item to add to this <see cref="BaseCollection{T}" />.</param>
        public virtual void Add(T item)
        {
            InternalList.Add(item);
            ItemAdded?.Invoke(this, item);
        }

        /// <summary>
        /// Removes the item at the specified index from this <see cref="BaseCollection{T}"/>.
        /// </summary>
        /// <param name="index">The index.</param>
        public virtual void RemoveAt(int index)
        {
            T i = InternalList[index];
            InternalList.RemoveAt(index);
            ItemRemoved?.Invoke(this, i);
        }

        /// <summary>
        /// Removes the first occurrence of the specified item from this <see cref="BaseCollection{T}"/>.
        /// </summary>
        /// <param name="item">The item to remove from this <see cref="BaseCollection{T}"/>.</param>
        /// <returns>
        /// <c>true</c> if the <paramref name="item"/> was successfully removed from this <see cref="BaseCollection{T}"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="BaseCollection{T}"/>.
        /// </returns>
        public virtual bool Remove(T item)
        {
            bool result = InternalList.Remove(item);
            if (result)
                ItemRemoved?.Invoke(this, item);
            return result;
        }

        /// <summary>
        /// Removes all items from this <see cref="BaseCollection{T}"/>.
        /// </summary>
        public virtual void Clear()
        {
            InternalList.Clear();
            Cleared?.Invoke(this);
        }

        /// <summary>
        /// Determines whether this <see cref="BaseCollection{T}"/> contains a specific item.
        /// </summary>
        /// <param name="item">The item to locate in this <see cref="BaseCollection{T}"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in this <see cref="BaseCollection{T}"/>; otherwise, false.
        /// </returns>
        public virtual bool Contains(T item)
        {
            return InternalList.Contains(item);
        }

        /// <summary>
        /// Copies the elements of this <see cref="BaseCollection{T}"/> to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from this <see cref="BaseCollection{T}"/>. The <see cref="Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            InternalList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Searches for the specified item and returns the zero-based index of the first occurrence within the entire <see cref="BaseCollection{T}"/>.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns>The zero-based index of the first occurrence of the item within the entire <see cref="BaseCollection{T}"/>, if found; otherwise, –1.</returns>
        public virtual int IndexOf(T item)
        {
            return InternalList.IndexOf(item);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public virtual IEnumerator<T> GetEnumerator()
        {
            return InternalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return InternalList.GetEnumerator();
        }
    }
}
