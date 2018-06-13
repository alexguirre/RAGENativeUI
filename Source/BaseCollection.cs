namespace RAGENativeUI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <include file='..\Documentation\RAGENativeUI.BaseCollection.xml' path='D/BaseCollection/Doc/*' />
    public class BaseCollection<T> : IList<T>
    {
        /// <include file='..\Documentation\RAGENativeUI.BaseCollection.xml' path='D/BaseCollection/Member[@name="InternalList"]/*' />
        protected virtual List<T> InternalList { get; set; }

        /// <include file='..\Documentation\RAGENativeUI.BaseCollection.xml' path='D/BaseCollection/Member[@name="Count"]/*' />
        public virtual int Count { get { return InternalList.Count; } }

        /// <include file='..\Documentation\RAGENativeUI.BaseCollection.xml' path='D/BaseCollection/Member[@name="Count"]/*' />
        public virtual bool IsReadOnly { get { return false; } }

        /// <include file='..\Documentation\RAGENativeUI.BaseCollection.xml' path='D/BaseCollection/Member[@name="Indexer"]/*' />
        public virtual T this[int index]
        {
            get
            {
                Throw.IfOutOfRange(index, 0, Count - 1, nameof(index));

                return InternalList[index];
            }
            set
            {
                Throw.IfOutOfRange(index, 0, Count - 1, nameof(index));

                InternalList[index] = value;
            }
        }

        /// <include file='..\Documentation\RAGENativeUI.BaseCollection.xml' path='D/BaseCollection/Member[@name="Ctor1"]/*' />
        public BaseCollection()
        {
            InternalList = new List<T>();
        }

        /// <include file='..\Documentation\RAGENativeUI.BaseCollection.xml' path='D/BaseCollection/Member[@name="Ctor2"]/*' />
        public BaseCollection(IEnumerable<T> collection)
        {
            Throw.IfNull(collection, nameof(collection));

            InternalList = new List<T>(collection);
        }

        /// <include file='..\Documentation\RAGENativeUI.BaseCollection.xml' path='D/BaseCollection/Member[@name="Add"]/*' />
        public virtual void Add(T item)
        {
            InternalList.Add(item);
        }

        /// <include file='..\Documentation\RAGENativeUI.BaseCollection.xml' path='D/BaseCollection/Member[@name="Insert"]/*' />
        public virtual void Insert(int index, T item)
        {
            InternalList.Insert(index, item);
        }

        /// <include file='..\Documentation\RAGENativeUI.BaseCollection.xml' path='D/BaseCollection/Member[@name="RemoveAt"]/*' />
        public virtual void RemoveAt(int index)
        {
            T i = InternalList[index];
            InternalList.RemoveAt(index);
        }

        /// <include file='..\Documentation\RAGENativeUI.BaseCollection.xml' path='D/BaseCollection/Member[@name="Remove"]/*' />
        public virtual bool Remove(T item)
        {
            bool result = InternalList.Remove(item);
            return result;
        }

        /// <include file='..\Documentation\RAGENativeUI.BaseCollection.xml' path='D/BaseCollection/Member[@name="Clear"]/*' />
        public virtual void Clear()
        {
            InternalList.Clear();
        }

        /// <include file='..\Documentation\RAGENativeUI.BaseCollection.xml' path='D/BaseCollection/Member[@name="Contains"]/*' />
        public virtual bool Contains(T item)
        {
            return InternalList.Contains(item);
        }

        /// <include file='..\Documentation\RAGENativeUI.BaseCollection.xml' path='D/BaseCollection/Member[@name="CopyTo"]/*' />
        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            InternalList.CopyTo(array, arrayIndex);
        }

        /// <include file='..\Documentation\RAGENativeUI.BaseCollection.xml' path='D/BaseCollection/Member[@name="IndexOf"]/*' />
        public virtual int IndexOf(T item)
        {
            return InternalList.IndexOf(item);
        }

        /// <include file='..\Documentation\RAGENativeUI.BaseCollection.xml' path='D/BaseCollection/Member[@name="GetEnumerator"]/*' />
        public virtual IEnumerator<T> GetEnumerator()
        {
            return InternalList.GetEnumerator();
        }

        /// <include file='..\Documentation\RAGENativeUI.BaseCollection.xml' path='D/BaseCollection/Member[@name="GetEnumerator"]/*' />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return InternalList.GetEnumerator();
        }
    }
}

