// xSolon.Events (int)EventSeverity.cs  8/23/2015 martin 

using System;
using System.Linq;

namespace xSolon.Events
{
    public enum EventSeverity
    {
        None = 500,
        Error = 100,
        Warning = 70,
        Information = 50,
        Verbose = 20,
        VerboseEx = 10
    }
}