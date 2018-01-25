using System;
using System.Collections.Generic;

namespace Indexator
{
    public class KeySetDictionary<T, U>
    {
        Dictionary<int, ISet<U>> _keySetToDocuments;
        KeySetComparer<T> _comparer;

        public KeySetDictionary()
        {
            _comparer = new KeySetComparer<T>();
            _keySetToDocuments = new Dictionary<int, ISet<U>>();
        }

        public void PrintStatus()
        {
            Console.Write($"KSTD: {_keySetToDocuments.Count}");
        }

        public void Add(KeySet<T> keySet, U document)
        {
            ISet<U> documents;
            if (_keySetToDocuments.TryGetValue(keySet.GetHashCode(), out documents))
            {
                documents.Add(document);
            }
            else 
            {
                documents = new HashSet<U>();
                documents.Add(document);
                _keySetToDocuments.Add(keySet.GetHashCode(), documents);
            }
        }

        public void Add(T key, U document)
        {
            var keySet = new KeySet<T>(key);
            Add(keySet, document);
        }

        public void AddNew(KeySet<T> keySet, IEnumerable<U> documents)
        {
            AddNew(keySet, new HashSet<U>(documents));
        }
        
        public void AddNew(KeySet<T> keySet, ISet<U> documents)
        {
            if (_keySetToDocuments.ContainsKey(keySet.GetHashCode()))
                throw new Exception("Item already exists in dictionary");

            _keySetToDocuments.Add(keySet.GetHashCode(), documents);
        }

        public ISet<U> this[KeySet<T> index]
        {
            get {return _keySetToDocuments[index.GetHashCode()]; }
        }

        public bool TryGetValue(KeySet<T> keySet, out ISet<U> documents)
        {
            return _keySetToDocuments.TryGetValue(keySet.GetHashCode(), out documents);
        }

        public bool ContainsKey(KeySet<T> ks)
        {
            return _keySetToDocuments.ContainsKey(ks.GetHashCode());
        }
    }
}