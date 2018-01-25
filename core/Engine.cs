using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Indexator.Utils;

namespace Indexator
{
    public class Engine
    {
        string pattern = @"([\w\\\/-]+|([0-9]+[ \.,-])+)";
        KeySetDictionary<string, Guid> _keySetExpenses;
        WordSuggester _wordSuggester;
        Regex wordExtract;
        MostCommonSubsets _topPopularity;

        public Engine()
        {
            _keySetExpenses = new KeySetDictionary<string, Guid>();
            wordExtract = new Regex(pattern);
            _wordSuggester = new WordSuggester(.15m);
            _topPopularity = new MostCommonSubsets(new TimeSpan(0,0,30));
        }

        public void IndexDocuments(IEnumerable<Document> documents)
        {
            documents
                .DoForEach(IndexDocument)
                .ToList();
        }

        public void IndexDocument(Document document)
        {
            PreProcessDocument(document)
                .DoForEach(k => _keySetExpenses.Add(k, document.Guid))
                .DoForEach(_wordSuggester.AddWord)
                .DoForAll(_topPopularity.Add)
                .Do();
        }

        private IEnumerable<string> PreProcessDocument(Document doc)
        {
            string text = doc.Body;
            var documentKeysFound = wordExtract
                .Matches(text)
                .Cast<Match>()
                .Select(m => m.Groups[0].Value.Trim())
                .Where(k => k.Length >= 4 && k.Length <= 15)
                .Distinct();
            return documentKeysFound;
        }
        
        public void SubsetGenerator(IEnumerable<string> keys, Guid docGuid) 
        {
            var keysA = keys.ToArray();
            SubsetGeneratorDeep(keysA, 0, 0, new KeySet<string>(), docGuid);
        }

        public (IEnumerable<SuggestedWord>, IEnumerable<Guid>) Search(string keys)
        {
            var suggestions = keys.Split(' ')
                .AsEnumerable()
                .Select(_wordSuggester.GetWordOrSuggestion);

            if (suggestions.Any(s => s.NotFound))
                return (suggestions, new List<Guid>());
                
            KeySet<string> keySet = new KeySet<string>(suggestions.Select(sw => sw.Word));

            ISet<Guid> guids = new HashSet<Guid>();
            if (_keySetExpenses.TryGetValue(keySet, out guids))
                return (suggestions, guids);
            else
                return (suggestions, SearchIndexIntersect(keySet));
        }

        private IEnumerable<IEnumerable<string>> ProcessDocumentForSearch(Document document)
        {
            return PreProcessDocument(document)
                .OrderByDescending(w => _wordSuggester.GetFrequency(w))
                .Split(4)
                .DoForEach(e => SubsetGenerator(e, document.Guid));
        }

        private IEnumerable<Guid> SearchIndexIntersect(KeySet<string> correctedKeys)
        {
            Document doc = new Document {
                    Guid = new Guid(),
                    Body = string.Join(" ", correctedKeys)
                };

            IEnumerable<Guid> resultGuids = 
            ProcessDocumentForSearch(doc)
                .Select(e => new KeySet<string>(e))
                .Select(k => {
                    ISet<Guid> guids = new HashSet<Guid>();
                    _keySetExpenses.TryGetValue(k, out guids);
                    return guids;
                })
                .Intersect();

            return resultGuids;
        }

        public IEnumerable<KeyValuePair<int, IEnumerable<KeySet<string>>>> GetTop()
        {
            return _topPopularity.GetTop();
        }

        private void SubsetGeneratorDeep(
            string[] keys,
            int level, int position,
            KeySet<string> inputKeySet,
            Guid documentId)
            {
                if (position >= keys.Length || level >= keys.Length)
                    return;

                KeySet<string> newss = new KeySet<string>(inputKeySet);
                string key = keys[position];
                newss.Add(key);

                bool keySetEncountered = false;
                if (_keySetExpenses.ContainsKey(newss))
                {
                    keySetEncountered = true;
                }
                else if (inputKeySet.Count > 0)
                {
                    KeySet<string> ks = new KeySet<string>(key);
                    ISet<Guid> ksExpenses = null;
                    _keySetExpenses.TryGetValue(ks, out ksExpenses);
                    ISet<Guid> ssExpenses = null;
                    _keySetExpenses.TryGetValue(inputKeySet, out ssExpenses);

                    if (ksExpenses != null && ssExpenses != null)
                    {
                        var expsIds = ksExpenses.Intersect(ssExpenses);
                        var expenseDictionarySet = new HashSet<Guid>(expsIds);
                        if (!documentId.Equals(new Guid()))
                            expenseDictionarySet.Add(documentId);
                        keySetEncountered = true;
                        _keySetExpenses.AddNew(newss, expenseDictionarySet);
                    }
                }

                if (keySetEncountered)
                {
                    SubsetGeneratorDeep(keys, level + 1, position + 1, new KeySet<string>(newss), documentId);
                    if (!documentId.Equals(new Guid()))
                        _keySetExpenses.Add(newss, documentId);
                }

                SubsetGeneratorDeep(keys, level, position + 1, inputKeySet, documentId);
            }
        
        public void PrintStatus() 
        {
            _wordSuggester.PrintStatus();
            _keySetExpenses.PrintStatus();
            _topPopularity.PrintStatus();
        }
    }
}
