
using System.Linq;
using System;
using System.Collections.Generic;
using Indexator;
using Xunit;
using System.Threading;

namespace tests
{
    public class MostCommonSubsetsTests
    {
        const string A = "wordA";
        const string B = "wordB";
        const string C = "wordC";
        const string D = "wordD";
        const string E = "wordE";
        const string F = "wordF";
        const string G = "wordG";
        const string H = "wordH";
        const string I = "wordI";
        const string J = "wordJ";

        public MostCommonSubsetsTests()
        {
        }

        private void Dump(IEnumerable<KeyValuePair<int, IEnumerable<KeySet<string>>>> pairs)
        {
            foreach (var pair in pairs)
            {
                Console.WriteLine($"{pair.Key}:");
                foreach (var keyset in pair.Value)
                {
                    Console.WriteLine(keyset.ToString());
                }
            }
        }

        [Fact]
        public void TopPopularTest1()
        {
            MostCommonSubsets topPopularity = new MostCommonSubsets(new TimeSpan(1,0,0));

            topPopularity.Add(new [] {A, B, C, E, F, G});
            topPopularity.Add(new [] {A, B, C});
            Dump(topPopularity.GetTop());
        }

        [Fact]
        public void TopPopularTest2()
        {
            MostCommonSubsets topPopularity = new MostCommonSubsets(new TimeSpan(1,0,0));

            topPopularity.Add(new [] {A, B, C, E, F, G});
            topPopularity.Add(new [] {A, B, C});
            topPopularity.Add(new [] {A, B, C});
            var top = topPopularity.GetTop();
            var expected = new HashSet<string> {A, B, C};
            Assert.True(top.Where(kv => kv.Key == 3).SelectMany(kv => kv.Value.First()).Intersect(expected).Count() == 3);
        }

        [Fact]
        public void TopPopularTest3()
        {
            MostCommonSubsets topPopularity = new MostCommonSubsets(new TimeSpan(1,0,0));

            topPopularity.Add(new [] {A, B, C, E, F, G});
            topPopularity.Add(new [] {A, B, C});
            topPopularity.Add(new [] {A, B, C});
            topPopularity.Add(new [] {B, C, E});
            var top = topPopularity.GetTop();
            var expected3 = new HashSet<string> {A, B, C};
            Assert.True(top.Where(kv => kv.Key == 3).SelectMany(kv => kv.Value.First()).Intersect(expected3).Count() == 3);
            var expected2 = new HashSet<string> {B, C, E};
            var counter2 = top.Where(kv => kv.Key == 2).ToList();
            Assert.True(counter2.Count == 1);
            var counter2Values = counter2.First().Value.ToList();
            Assert.True(counter2Values.Count == 1);
            var counter2Intersect = counter2Values.First().Intersect(expected2);
            Assert.True(counter2Intersect.Count() == 3); 
        }

        [Fact]
        public void TopPopularityTimeWindow1()
        {
            MostCommonSubsets topPopularity = new MostCommonSubsets(new TimeSpan(0,0,2));
            topPopularity.Add(new [] {A, B, C, E, F, G});
            Thread.Sleep(3000);
            topPopularity.Add(new [] {H, I, J});
            var top = topPopularity.GetTop();
            var expected1 = new HashSet<string> {H, I, J};
            Assert.True(top.Where(kv => kv.Key == 1).SelectMany(kv => kv.Value.First()).Intersect(expected1).Count() == 3);
        }
    }
}