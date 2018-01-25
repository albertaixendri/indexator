using System;
using System.Linq;
using System.Collections.Generic;
using Indexator.Utils;

namespace Indexator
{
    
    public class WordSuggester 
    {
        private const int MaxLevel = 3;

        Dictionary<string, int> _wordFrequency;

        Dictionary<string, List<String>> _rootSuggestions;

        decimal _margin;

        public WordSuggester(decimal margin)
        {
            _margin = margin;
            _wordFrequency = new Dictionary<string, int>();
            _rootSuggestions = new Dictionary<string, List<string>>();
        }

        public int GetFrequency(string word)
        {
            int freq = 0;
            _wordFrequency.TryGetValue(word, out freq);
            return freq;
        }

        public SuggestedWord GetWordOrSuggestion(string word)
        {
            if (_wordFrequency.ContainsKey(word))
                return new SuggestedWord {Requested = word, Word = word, Populatity = _wordFrequency[word]};
            
            SuggestedWord suggestion = GoThroughRoots(word, ProcessRootSuggestions)
                .OrderByDescending(sw => sw.Populatity)
                .FirstOrDefault();
            suggestion.Requested = word;
            return suggestion;
        }

        private IEnumerable<string> GetRoots(string word)
        {
            List<string> roots = new List<string>();
            for (int i = 0; i < word.Length; i++)
                roots.Add(word.Remove(i, 1));
            return roots;
        }

        private IEnumerable<string> GetRoots(IEnumerable<string> words)
        {
            return words.SelectMany(GetRoots);
        }

        public void AddWord(string word)
        {
            if (_wordFrequency.ContainsKey(word))
            {
                _wordFrequency[word]++;
                return;
            }
            var processor = BuildAddRootProcessor(word);
            GoThroughRoots(word, processor).Do();
            _wordFrequency.Add(word, 1);
        }

        private SuggestedWord ProcessRootSuggestions(IEnumerable<string> roots)
        {
            var suggestion = roots
                .SelectMany(r => _rootSuggestions.ContainsKey(r) ? _rootSuggestions[r] : new List<string>())
                .Select(w => new SuggestedWord{Word = w, Populatity = _wordFrequency[w]})
                .OrderByDescending(sw => sw.Populatity)
                .FirstOrDefault();
            
            return suggestion ?? new SuggestedWord {Word = "", Populatity = Int32.MinValue, NotFound = true};
        }

        private Func<IEnumerable<string>, string> BuildAddRootProcessor(string word)
        {
            Func<IEnumerable<string>, string> processor = (roots) =>
            {
                roots.DoForEach(root => 
                {
                    List<string> suggestedWords;
                    if (!_rootSuggestions.TryGetValue(root, out suggestedWords))
                    {
                        suggestedWords = new List<string>();
                        _rootSuggestions.Add(root, suggestedWords);
                    }

                    suggestedWords.Add(word);
                }).Do();
                return null;
            };

            return processor;
        }

        private IEnumerable<T> GoThroughRoots<T>(string word, Func<IEnumerable<string>, T> processor)
        {
            List<string> roots = new List<string> {word};
            int maxLevel = CalculateMaxLevel(word);
            return GoThroughRoots(roots, 0, maxLevel, processor);
        }

        private IEnumerable<T> GoThroughRoots<T>(IEnumerable<string> roots, int level, int maxLevel, Func<IEnumerable<string>, T> processor)
        {
            yield return processor(roots);
            if (level <= maxLevel)
                foreach (T result in GoThroughRoots(GetRoots(roots), level + 1, maxLevel, processor))
                    yield return result;
        }

        private int CalculateMaxLevel(string word)
        {
            return (int)Math.Min(MaxLevel, Math.Floor(word.Length * _margin));
        }

        public void PrintStatus()
        {
            Console.Write($" WF:{_wordFrequency.Count} R:{_rootSuggestions.Count} ");
        }
    }

    public class SuggestedWord
    {
        public string Requested {get; set;}
        public string Word {get;set;}
        public int Populatity {get; set;}
        public bool NotFound {get; set;}
    }
}