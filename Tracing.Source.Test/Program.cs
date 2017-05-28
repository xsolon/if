using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracing.Source.Test
{
    class Program
    {
        static void Main(string[] args)
        {

            var trace = new TraceSource("Me", SourceLevels.All);

            var driver = new MyDriver(trace);
            driver = driver.Wrap();

            var resul = driver.AMethod(1, "dos");
            
            trace.Listeners.Add(new ConsoleTraceListener());
            //trace.Listeners.Add(new DefaultTraceListener());

            trace.TraceInformation("Hello World");
            trace.Indent();
            trace.TraceInformation("Hello America");
            trace.Unindent();

            using (var scope = new ActivityScope(trace, "Activity2"))
            {
                trace.TraceInformation("Activity 2 stuff");
            }

            trace.TraceInformation("--------------------------------------------");
            using (var scope = new ActivityScope(trace, 1, 2, 3, 4, "TransferIn", "Start", "TransferOut", "Stop", "My Activity"))
            {
                trace.TraceEvent(TraceEventType.Verbose, 120, "Activity 1");
            }

            using (var logicalScope = new LogicalOperationScope(trace, "Say Goodbye"))
            {
                trace.TraceEvent(TraceEventType.Verbose, 123, "Goodbye!");
            }
        }

        class MyDriver : TracedClass
        {
            public MyDriver(TraceSource source) : base(source)
            {

            }
            public string AMethod(int param1, string param2)
            {

                return $"{param2}.{param1}";
            }
        }
    }
}
