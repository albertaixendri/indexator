using System;
using System.IO;
using System.Linq;

namespace Indexator.MainConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            InteractiveShell shell = new InteractiveShell();
            shell.Start();
        }
    }
}
