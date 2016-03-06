using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace xSolon.Tracing
{
    public abstract class TransactionTraceListener :TraceListener
    {
        public abstract string GetTransaction(); 
    }
}
