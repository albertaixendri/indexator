using System;
using System.IO;
using Xunit;
using Indexator;
using System.Linq;
using System.Collections.Generic;

namespace tests
{
    public class UnitTest1
    {
        Engine _engine;

        public UnitTest1()
        {
            _engine = new Engine();
        }
        
        private void LoadTestData(string filename)
        {
            var documents = File.
            ReadLines(filename)
            .Select(l => {
                var parts = l.Split('\t');
                return new Document
                {
                    Guid = Guid.Parse(parts[0].Trim().PadLeft(32, '0')),
                    Body = parts[1]
                };
            });

            _engine.IndexDocuments(documents);
        }

        private void DumpResult(KeyValuePair<int, IEnumerable<KeySet<string>>> result)
        {
            Console.Write($"{result.Key} ");
            foreach(var keySet in result.Value)
            {
                Console.Write($"{keySet.ToString()} # ");
            }
            Console.WriteLine();
        }
        
        [Fact]
        public void Test100()
        {
            Test100Do();
        }

        private void Test100Do()
        {
            _engine = new Engine();
            string fileData = "data/eng_news_2015_1M-sentences-100.txt";
            LoadTestData(fileData);
            var kvs = _engine.GetTop().Take(10).ToList();
            kvs.ForEach(DumpResult);
        }

        [Fact]
        public void TestWords()
        {
            _engine = new Engine();
            var body = String.Join(" ", Enumerable.Range(1, 20).Select(i => $"word{i}"));
            Document doc = new Document{
                Body = body,
                Guid = Guid.NewGuid()
            };
            _engine.IndexDocument(doc);
            var (words, guids) = _engine.Search(body);

            Assert.True(guids.Count() == 1);
        }
    }
}
