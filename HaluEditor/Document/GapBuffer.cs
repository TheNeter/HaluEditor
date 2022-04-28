using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace ngprojects.HaluEditor.Document
{
    /// <summary>
    /// Represents a strongly typed collection of objects that can be accessed by index. Insertions
    /// and deletions to the collection near the same relative index are optimized.
    /// </summary>
    /// <typeparam name="T"> The type of elements in the buffer. </typeparam>
    //http://www.codeproject.com/Articles/20910/Generic-Gap-Buffer
    //https://github.com/jacobslusser/ScintillaNET
    [Serializable]
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    public class GapBuffer<T> : IList<T>, IList
    {
        private const int MIN_CAPACITY = 4;

        private T[] _buffer;
        private int _gapEnd;
        private int _gapStart;

        [NonSerialized]
        private object _syncRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="GapBuffer{T}" /> class.
        /// </summary>
        public GapBuffer()
        {
            _buffer = new T[MIN_CAPACITY];
            _gapEnd = _buffer.Length;
        }

        /// <summary>
        /// Gets or sets the total number of elements the internal data structure can hold without resizing.
        /// </summary>
        /// <value>
        /// The number of elements that the <see cref="GapBuffer{T}" /> can contain before resizing
        /// is required.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <see cref="Capacity" /> is set to a value that is less than <see cref="Count" />.
        /// </exception>
        public int Capacity
        {
            get
            {
                return _buffer.Length;
            }
            set
            {
                // Is there any work to do?
                if (value == _buffer.Length)
                    return;

                // Look for naughty boys and girls
                if (value < Count)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                if (value > 0)
                {
                    // Allocate a new buffer
                    T[] newBuffer = new T[value];
                    int newGapEnd = newBuffer.Length - (_buffer.Length - _gapEnd);

                    // Copy the spans into the front and back of the new buffer
                    Array.Copy(_buffer, 0, newBuffer, 0, _gapStart);
                    Array.Copy(_buffer, _gapEnd, newBuffer, newGapEnd, newBuffer.Length - newGapEnd);
                    _buffer = newBuffer;
                    _gapEnd = newGapEnd;
                }
                else
                {
                    // Reset everything
                    _buffer = new T[MIN_CAPACITY];
                    _gapStart = 0;
                    _gapEnd = _buffer.Length;
                }
            }
        }

        /// <summary>
        /// Gets the number of elements actually contained in the <see cref="GapBuffer{T}" />.
        /// </summary>
        /// <value> The number of elements actually contained in the <see cref="GapBuffer{T}" />. </value>
        public int Count
        {
            get
            {
                return _buffer.Length - (_gapEnd - _gapStart);
            }
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        bool IList.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    Interlocked.CompareExchange(ref _syncRoot, new object(), null);
                }

                return _syncRoot;
            }
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index"> The zero-based index of the element to get or set. </param>
        /// <value> The element at the specified index. </value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.
        /// <para> -or- </para>
        /// <paramref name="index" /> is equal to or greater than <see cref="Count" />.
        /// </exception>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }

                // Find the correct span and get the item
                if (index >= _gapStart)
                {
                    index += (_gapEnd - _gapStart);
                }

                return _buffer[index];
            }
            set
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }

                // Find the correct span and set the item
                if (index >= _gapStart)
                {
                    index += (_gapEnd - _gapStart);
                }

                _buffer[index] = value;
            }
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                VerifyValueType(value);
                this[index] = (T)value;
            }
        }

        /// <summary>
        /// Adds an object to the end of the <see cref="GapBuffer{T}" />.
        /// </summary>
        /// <param name="item">
        /// The object to be added to the end of the <see cref="GapBuffer{T}" />. The value can be
        /// null for reference types.
        /// </param>
        public void Add(T item)
        {
            Insert(Count, item);
        }

        int IList.Add(object value)
        {
            VerifyValueType(value);
            Add((T)value);
            return (Count - 1);
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="GapBuffer{T}" />.
        /// </summary>
        /// <param name="collection">
        /// The collection whose elements should be inserted into the <see cref="GapBuffer{T}" />.
        /// The collection itself cannot be null, but it can contain elements that are null, if type
        /// <typeparamref name="T" /> is a reference type.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection" /> is a null reference.
        /// </exception>
        public void AddRange(IEnumerable<T> collection)
        {
            InsertRange(Count, collection);
        }

        /// <summary>
        /// Removes all elements from the <see cref="GapBuffer{T}" />.
        /// </summary>
        public void Clear()
        {
            // Clearing the buffer means simply enlarging the gap to the entire buffer size

            Array.Clear(_buffer, 0, _buffer.Length);
            _gapStart = 0;
            _gapEnd = _buffer.Length;
        }

        /// <summary>
        /// Determines whether an element is in the <see cref="GapBuffer{T}" />.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the <see cref="GapBuffer{T}" />. The value can be null for
        /// reference types.
        /// </param>
        /// <returns>
        /// <b> true </b> if item is found in the <see cref="GapBuffer{T}" />; otherwise, <b> false </b>.
        /// </returns>
        public bool Contains(T item)
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < _gapStart; i++)
            {
                if (comparer.Equals(_buffer[i], item))
                {
                    return true;
                }
            }
            for (int i = _gapEnd; i < _buffer.Length; i++)
            {
                if (comparer.Equals(_buffer[i], item))
                {
                    return true;
                }
            }

            return false;
        }

        bool IList.Contains(object value)
        {
            return IsCompatibleObject(value) && Contains((T)value);
        }

        /// <summary>
        /// Copies the <see cref="GapBuffer{T}" /> to a compatible one-dimensional array, starting
        /// at the specified index of the target array.
        /// </summary>
        /// <param name="array">     
        /// The one-dimensional <see cref="Array" /> that is the destination of the elements copied
        /// from <see cref="GapBuffer{T}" />. The <see cref="Array" /> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        /// The zero-based index in <paramref name="array" /> at which copying begins.
        /// </param>
        /// <exception cref="ArgumentNullException"> <paramref name="array" /> is a null reference. </exception>
        /// <exception cref="ArgumentOutOfRange">
        /// <paramref name="arrayIndex" /> is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="array" /> is multidimensional.
        /// <para> -or- </para>
        /// <paramref name="arrayIndex" /> is equal to or greater than the length of <paramref name="array" />.
        /// <para> -or- </para>
        /// The number of elements in the source <see cref="GapBuffer{T}" /> is greater than the
        /// available space from <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.
        /// </exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            else if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex");
            }
            else if (array.Rank != 1)
            {
                throw new ArgumentException("not supported rank multi dim array");
            }
            else if (arrayIndex >= array.Length || arrayIndex + Count > array.Length)
            {
                throw new ArgumentOutOfRangeException("arrayIndex");
            }

            // Copy the spans into the destination array at the offset
            Array.Copy(_buffer, 0, array, arrayIndex, _gapStart);
            Array.Copy(_buffer, _gapEnd, array, arrayIndex + _gapStart, _buffer.Length - _gapEnd);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            try
            {
                CopyTo((T[])array, index);
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException("invalid type array");
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first
        /// occurrence within the <see cref="GapBuffer{T}" />.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the <see cref="GapBuffer{T}" />. The value can be null for
        /// reference types.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item" /> within the
        /// <see cref="GapBuffer{T}" />, if found; otherwise, �1.
        /// </returns>
        public int IndexOf(T item)
        {
            // Search within the buffer spans

            int index = Array.IndexOf<T>(_buffer, item, 0, _gapStart);
            if (index < 0)
            {
                index = Array.IndexOf<T>(_buffer, item, _gapEnd, _buffer.Length - _gapEnd);

                // Translate the internal index to the index in the collection
                if (index != -1)
                {
                    return index - (_gapEnd - _gapStart);
                }
            }

            return index;
        }

        int IList.IndexOf(object value)
        {
            return IsCompatibleObject(value) ? IndexOf((T)value) : -1;
        }

        /// <summary>
        /// Inserts an element into the <see cref="GapBuffer{T}" /> at the specified index.
        /// Consecutive operations at or near previous inserts are optimized.
        /// </summary>
        /// <param name="index"> The object to insert. The value can be null for reference types. </param>
        /// <param name="item"> 
        /// The zero-based index at which <paramref name="item" /> should be inserted.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.
        /// <para> -or- </para>
        /// <paramref name="index" /> is greater than <see cref="Count" />.
        /// </exception>
        public void Insert(int index, T item)
        {
            if (index < 0 || index > Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            // Prepare the buffer
            PlaceGapStart(index);
            EnsureGapCapacity(1);

            _buffer[index] = item;
            _gapStart++;
        }

        void IList.Insert(int index, object value)
        {
            VerifyValueType(value);
            Insert(index, (T)value);
        }

        /// <summary>
        /// Inserts the elements of a collection into the <see cref="GapBuffer{T}" /> at the
        /// specified index. Consecutive operations at or near previous inserts are optimized.
        /// </summary>
        /// <param name="index">     
        /// The zero-based index at which the new elements should be inserted.
        /// </param>
        /// <param name="collection">
        /// The collection whose elements should be inserted into the <see cref="GapBuffer{T}" />.
        /// The collection itself cannot be null, but it can contain elements that are null, if type
        /// <typeparamref name="T" /> is a reference type.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection" /> is a null reference.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.
        /// <para> -or- </para>
        /// <paramref name="index" /> is greater than <see cref="Count" />.
        /// </exception>
        public void InsertRange(int index, IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            if (index < 0 || index > Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            ICollection<T> col = collection as ICollection<T>;
            if (col != null)
            {
                int count = col.Count;
                if (count > 0)
                {
                    PlaceGapStart(index);
                    EnsureGapCapacity(count);

                    // Copy the collection directly into the buffer
                    col.CopyTo(_buffer, _gapStart);
                    _gapStart += count;
                }
            }
            else
            {
                // Add the items to the buffer one-at-a-time :(
                using (IEnumerator<T> enumerator = collection.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Insert(index, enumerator.Current);
                        index++;
                    }
                }
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="GapBuffer{T}" />.
        /// </summary>
        /// <param name="item">
        /// The object to remove from the <see cref="GapBuffer{T}" />. The value can be null for
        /// reference types.
        /// </param>
        /// <returns>
        /// <b> true </b> if <paramref name="item" /> is successfully removed; otherwise, <b> false
        /// </b>. This method also returns <b> false </b> if <paramref name="item" /> was not found
        /// in the <see cref="GapBuffer{T}" />.
        /// </returns>
        public bool Remove(T item)
        {
            // Get the index of the item
            int index = IndexOf(item);
            if (index < 0)
            {
                return false;
            }

            // Remove the item
            RemoveAt(index);
            return true;
        }

        void IList.Remove(object value)
        {
            if (IsCompatibleObject(value))
                Remove((T)value);
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="GapBuffer{T}" />.
        /// </summary>
        /// <param name="index"> The zero-based index of the element to remove. </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.
        /// <para> -or- </para>
        /// <paramref name="index" /> is equal to or greater than <see cref="Count" />.
        /// </exception>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            // Place the gap at the index and increase the gap size by 1
            PlaceGapStart(index);
            _buffer[_gapEnd] = default;
            _gapEnd++;
        }

        /// <summary>
        /// Removes a range of elements from the <see cref="GapBuffer{T}" />.
        /// </summary>
        /// <param name="index"> The zero-based starting index of the range of elements to remove. </param>
        /// <param name="count"> The number of elements to remove. </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0 or is equal to or greater than <see cref="Count" />.
        /// <para> -or- </para>
        /// <paramref name="count" /> is less than 0.
        /// <para> -or- </para>
        /// <paramref name="index" /> and <paramref name="count" /> do not denote a valid range of
        /// elements in the <see cref="GapBuffer{T}" />.
        /// </exception>
        public void RemoveRange(int index, int count)
        {
            int size = Count;

            if (count == 0)
            {
                return;
            }

            if (index < 0 || index >= size)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (count < 0 || size - index < count)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            // Move the gap over the index and increase the gap size by the number of elements
            // removed. Easy as pie!

            if (count > 0)
            {
                PlaceGapStart(index);
                Array.Clear(_buffer, _gapEnd, count);
                _gapEnd += count;
            }
        }

        /// <summary>
        /// Sets the <see cref="Capacity" /> to the actual number of elements in the
        /// <see cref="GapBuffer{T}" />, if that number is less than a threshold value.
        /// </summary>
        public void TrimExcess()
        {
            int size = Count;
            int threshold = (int)(_buffer.Length * 0.9);
            if (size < threshold)
            {
                Capacity = size;
            }
        }

        internal IEnumerable<T> GetRange(int start, int length)
        {
            for (int i = 0; i < length; i++)
            {
                yield return this[start + i];
            }
        }

        private static bool IsCompatibleObject(object value)
        {
            // Ensure the object is compatible with the generic type
            return value is T || value == null && !typeof(T).IsValueType;
        }

        private static void VerifyValueType(object value)
        {
            if (!IsCompatibleObject(value))
            {
                throw new ArgumentException("wrong type", "value");
            }
        }

        private void EnsureGapCapacity(int required)
        {
            // Is the available space in the gap?
            if (required > (_gapEnd - _gapStart))
            {
                // Calculate a new size (double the size necessary)
                int newCapacity = Count * 2;
                if (newCapacity < required)
                {
                    newCapacity = required + MIN_CAPACITY;
                }
                else if (newCapacity < MIN_CAPACITY)
                {
                    newCapacity = MIN_CAPACITY;
                }

                Capacity = newCapacity;
            }
        }

        private void PlaceGapStart(int index)
        {
            // Are we already there?
            if (index == _gapStart)
            {
                return;
            }

            // Is there even a gap?
            if ((_gapEnd - _gapStart) == 0)
            {
                _gapStart = index;
                _gapEnd = index;
                return;
            }

            // Which direction do we move the gap?
            if (index < _gapStart)
            {
                // Move the gap near (by copying the items at the beginning of the gap to the end)
                int count = _gapStart - index;
                int deltaCount = (_gapEnd - _gapStart < count ? _gapEnd - _gapStart : count);
                Array.Copy(_buffer, index, _buffer, _gapEnd - count, count);
                _gapStart -= count;
                _gapEnd -= count;

                // Clear the contents of the gap
                Array.Clear(_buffer, index, deltaCount);
            }
            else
            {
                // Move the gap far (by copying the items at the end of the gap to the beginning)
                int count = index - _gapStart;
                int deltaIndex = (index > _gapEnd ? index : _gapEnd);
                Array.Copy(_buffer, _gapEnd, _buffer, _gapStart, count);
                _gapStart += count;
                _gapEnd += count;

                // Clear the contents of the gap
                Array.Clear(_buffer, deltaIndex, _gapEnd - deltaIndex);
            }
        }
    }
}