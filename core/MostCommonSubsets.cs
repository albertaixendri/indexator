using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Indexator.Utils;

namespace Indexator
{
    public class MostCommonSubsets
    {
        Dictionary<KeySet<string>, SubsetInfo> _keySetSubset;
        SortedDictionary<int, ISet<SubsetInfo>> _counterSubsets;
        SortedDictionary<DateTime, SubsetInfo> _datetimeSubset;
        TimeSpan _window;

        public MostCommonSubsets(TimeSpan window)
        {
            _window = window;
            _counterSubsets = new SortedDictionary<int, ISet<SubsetInfo>>();
            _datetimeSubset = new SortedDictionary<DateTime, SubsetInfo>();
            KeySetComparer<string> comparer = new KeySetComparer<string>();
            _keySetSubset = new Dictionary<KeySet<string>, SubsetInfo>(comparer);
        }

        public void PrintStatus()
        {
            Console.Write($"E {_keySetSubset.Count} CE {_counterSubsets.Count} DE {_datetimeSubset.Count}");
        }

        public void Add(IEnumerable<string> keys)
        {
            Add(keys.ToArray());
        }

        public void Add(string[] keys)
        {
            if (keys.Count() < 3)
                return;
            
            int maxElements = Math.Min(10, keys.Count());

            Enumerable
                .Range(3, maxElements - 2)
                .SelectMany(i => FindCombinations(i, maxElements))
                .Select(comb => comb.Select(pos => keys[pos]))
                .Select(subset => new KeySet<string>(subset))
                .DoForEach(Add)
                .Do();
        }

        private void Add(KeySet<string> keySet)
        {
            CleanOld();

            SubsetInfo element = null;
            if (!_keySetSubset.TryGetValue(keySet, out element))
            {
                element = new SubsetInfo(){
                    Timestamp = DateTime.UtcNow,
                    KeySet = keySet
                };
                _keySetSubset.Add(keySet, element);
            } 
            else 
            {
                var prevTimestamp = element.Timestamp;
                element.Timestamp = DateTime.UtcNow;
                _datetimeSubset.Remove(prevTimestamp);
            }
            _datetimeSubset.Add(element.Timestamp, element);

            ElementUpdateCounter(element, 1);
        }

        private void CleanOld()
        {
            var now = DateTime.UtcNow;
            var kvToDelete = _datetimeSubset.Keys
                .TakeWhile(t => now.Subtract(t).CompareTo(_window) > 0)
                .Select(t => new KeyValuePair<DateTime, SubsetInfo>(t, _datetimeSubset[t]))
                .ToArray()
                ;

            Array.ForEach(kvToDelete, kv => {
                _datetimeSubset.Remove(kv.Key);
                ElementUpdateCounter(kv.Value, -1);
            });
        }

        private void ElementUpdateCounter(SubsetInfo element, int delta)
        {
            int prevCounter = element.Counter;
            element.Counter += delta;
            int newCounter = element.Counter;
            if (newCounter == 0)
                _keySetSubset.Remove(element.KeySet);
            if (prevCounter > 0)
            {
                _counterSubsets[prevCounter].Remove(element);
                if (_counterSubsets[prevCounter].Count == 0)
                    _counterSubsets.Remove(prevCounter);
            }
            if (newCounter > 0)
            {
                ISet<SubsetInfo> counterElements = null;
                    if (!_counterSubsets.TryGetValue(newCounter, out counterElements))
                    {
                        counterElements = new HashSet<SubsetInfo>();
                        _counterSubsets.Add(newCounter, counterElements);
                    }
                    counterElements.Add(element);
            }
        }

        /// From https://rosettacode.org/wiki/Combinations#C.23
        public IEnumerable<int[]> FindCombosRec(int[] buffer, int done, int begin, int end)
        {
            for (int i = begin; i < end; i++)
            {
                buffer[done] = i;
            
                if (done == buffer.Length - 1)
                    yield return buffer;
                else
                    foreach (int[] child in FindCombosRec(buffer, done+1, i+1, end))
                    yield return child;
            }
        }
 
        public IEnumerable<int[]> FindCombinations(int m, int n)
        {
            return FindCombosRec(new int[m], 0, 0, n);
        }

        public IEnumerable<KeyValuePair<int, IEnumerable<KeySet<string>>>> GetTop()
        {
            return _counterSubsets
                .Reverse()
                .Select(kv => new KeyValuePair<int, IEnumerable<KeySet<string>>>(kv.Key, kv.Value.Select(te => te.KeySet)))
                ;
        }

        private class SubsetInfo
        {
            public DateTime Timestamp {get; set;}
            public int Counter { get; set; }
            public KeySet<string> KeySet { get; set; }
        }
    }
}