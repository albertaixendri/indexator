using System;
using System.Collections.Generic;

namespace Indexator
{
    public class Document
    {
        public Guid Guid {get; set;}
        public ISet<string> Keys {get; set;}
        public string Body {get; set;}
    }
}