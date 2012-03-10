using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlissInSoftware.Sandcastle.Cobol
{
    class ParsingException : Exception
    {
        public ParsingException(string message) : base(message) { }
    }
}
