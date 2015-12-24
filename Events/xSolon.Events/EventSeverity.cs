// xSolon.Events (int)EventSeverity.cs  8/23/2015 martin 

using System;
using System.Linq;

namespace xSolon.Events
{
    public enum EventSeverity
    {
        None = 10000,
        Error = 8000,
        Warning = 6000,
        Information = 4000,
        Verbose = 2000,
        VerboseEx = 1000
    }
}
