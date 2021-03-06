﻿using System.Diagnostics;
using xSolon.Tracing;

namespace xSolon.TracingTests
{
    class SampleClass : TracedClass
    {

        public SampleClass(TraceSource source): base(source)
        {

        }
        public SampleClass()
        {

        }
        public SampleClass(string sourceName) : base(sourceName) 
        {

        }

        public void Method1(){

            TraceEvent(System.Diagnostics.TraceEventType.Information, 1, "Running Method1!");
        }

        public int Method2(string param1, string param2)
        {

            TraceEvent(System.Diagnostics.TraceEventType.Information, 2, "Running Method2!");

            return 1;

        }
    }
}
