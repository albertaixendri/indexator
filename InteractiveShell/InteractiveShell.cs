using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Indexator;
using Indexator.Utils;

namespace Indexator.MainConsole
{
    public class InteractiveShell{
        const string COMMAND_QUIT = "q";
        const string COMMAND_LOAD = "l";
        const string COMMAND_SEARCH = "s";
        const string COMMAND_STATISTICS = "t";
        Regex sentenceRx;
        Engine _engine;

        public InteractiveShell()
        {
            List<string> commands = new List<string>{COMMAND_LOAD, COMMAND_STATISTICS, COMMAND_QUIT, COMMAND_SEARCH};
            sentenceRx = new Regex($"({string.Join('|',commands)})(?: (.*))?");
            _engine = new Engine();
        }

        public void Start() 
        {
            string command = "";
            string param = "";
            string sentence = "";
            while (command != "q")
            {
                Console.WriteLine();
                Console.Write("> ");
                sentence = Console.ReadLine();
                Match match = sentenceRx.Match(sentence);
                if (!match.Success)
                {
                    Console.WriteLine("Invalid sentece");
                    continue;
                }
                command = match.Groups[1].Value;
                if (match.Groups.Count > 1)
                    param = match.Groups[2].Value;

                switch(command)
                {
                    case COMMAND_LOAD: Load(param); break;
                    case COMMAND_SEARCH: Search(param); break;
                    case COMMAND_STATISTICS: Statistics(); break;
                    case COMMAND_QUIT: return;
                }
            }
        }

        private void Load(string param)
        {
            int counter = 0;
            var documents =
                File
                .ReadLines(param)
                .Select(l => {
                    counter++;
                    if (counter % 1 == 0)
                    {
                        Console.Write($"{DateTime.Now.ToString()} {counter}");
                        _engine.PrintStatus();
                        Console.WriteLine();
                    }
                    var parts = l.Split('\t');
                    return new Document
                    {
                        Guid = Guid.Parse(parts[0].Trim().PadLeft(32, '0')),
                        Body = parts[1]
                    };
                });
            
            _engine.IndexDocuments(documents);
        }

        private void Search(string param)
        {
            var (suggestedWords, results) = _engine.Search(param);

            Console.Write("Found keys: ");
            suggestedWords.DoForEach(sw => Console.Write(sw.Requested + "(" + (sw.NotFound ? "Not found" : sw.Word) + ") ")).Do();
            Console.WriteLine();
            if (!results.Any())
                Console.WriteLine("No results found!!");
            else
                Console.WriteLine("Showing first 25 results");
            results.Take(25).DoForEach(g => Console.WriteLine(g)).Do();
        }

        private void Statistics()
        {
            foreach(var stat in _engine
                .GetTop()
                .Take(25)
            )
            {
                Console.Write($"{stat.Key} ");
                foreach(var keySet in stat.Value.OrderByDescending(ks => ks.Count).Take(10))
                {
                    Console.WriteLine($"{keySet.ToString()}");
                }
                Console.WriteLine();
            }

            _engine.PrintStatus();
        }
    }
}