using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace ngprojects.HaluEditor.Document
{
    public class CollectionDebugView<T>
    {
        private readonly ICollection<T> _collection;

        public CollectionDebugView(ICollection<T> collection)
        {
            _collection = collection ?? throw new ArgumentNullException("collection");
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                T[] array = new T[_collection.Count];
                _collection.CopyTo(array, 0);
                return array;
            }
        }
    }

    public class CollectionDebugView
    {
        private readonly ICollection _collection;

        public CollectionDebugView(ICollection collection)
        {
            _collection = collection ?? throw new ArgumentNullException("collection");
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public object[] Items
        {
            get
            {
                var array = new object[_collection.Count];
                _collection.CopyTo(array, 0);
                return array;
            }
        }
    }
}