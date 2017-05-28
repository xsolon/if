using Newtonsoft.Json;
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

            var trace = new TraceSource("TestProgram", SourceLevels.All);
            trace.Listeners.Add(new ConsoleTraceListener());
            //trace.Listeners.Add(new DefaultTraceListener());

            Extensions.GlobalSerializer = (obj) =>
            {
                return "MyDriver:"+ obj?.ToString() ;
            };

            var driver = new MyDriver(trace);
            driver = driver.Wrap();
            //driver = driver.Wrap((obj) =>
            //{
            //    var res = JsonConvert.SerializeObject(obj, Formatting.Indented); //Newtonsoft dependency
            //    return res;
            //});

            var resul = driver.AMethod(1, "dos");
            driver.ComplexParamsMethod(2, new MyDriver.MyParam() { Val1 = 3, Val2 = "A Val 2" });
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

            public MyParam ComplexParamsMethod(int param1, MyParam param2)
            {
                return param2;

            }
            public class MyParam
            {
                public int Val1 = 1;
                public string Val2 { get; set; }
            }
        }
    }
}
