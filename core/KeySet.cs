using System.Collections.Generic;
using System.Linq;

namespace Indexator
{
    public interface SetClass<T>
    {
        ISet<T> Keys {get; set;}
    }

    public class KeySet<T>: SetClass<T>, IEnumerable<T>
    {
        public KeySet(T element) : this()
        {
            Keys.Add(element);
        }

        public KeySet(IEnumerable<T> data) : this()
        {
            Keys.UnionWith(data);
        }

        public KeySet()
        {
            Keys = new HashSet<T>();
        }

        public ISet<T> Keys {get; set;}

        public bool Add(T item)
        {
            return Keys.Add(item);
        }

        public int Count {
            get{ return Keys.Count;}
        }
        

        public override string ToString()
        {
            return string.Join(", ", Keys);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return null;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Keys.GetEnumerator();
        }

        public override int GetHashCode()
        {
            var list = Keys.ToList();
            list.Sort();
            int hashCode = 13;
            list.ForEach(s => {
                unchecked
                {
                    hashCode = (hashCode * 397) ^ s.GetHashCode();
                }
            });

            return hashCode;
        }         
    }

    public class KeySetComparer<T> : IEqualityComparer<KeySet<T>>
    {
        public bool Equals(KeySet<T> x, KeySet<T> y)
        {
            return x.Keys.SetEquals(y.Keys);
        }

        public int GetHashCode(KeySet<T> set)
        {
            var list = set.Keys.ToList();
            list.Sort();
            int hashCode = 13;
            list.ForEach(s => {
                unchecked
                {
                    hashCode = (hashCode * 397) ^ s.GetHashCode();
                }
            });

            return hashCode;
        }
    }
}