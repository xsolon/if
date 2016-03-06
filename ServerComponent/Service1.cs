using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using xSolon.Tracing;

namespace ServerComponent
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Service1 : IService1
    {
        public void DoWork()
        {
            var source = new ExtendedTraceSource("ServerComponent");
            try
            {

                using (var lScope = new LogicalOperationScope(source, "Process C"))
                {

                    var sample2 = new MyServerComponent(source);

                    sample2.TraceEvent(TraceEventType.Information, 10, "Server Message for the Process");

                }

                Trace.WriteLine("Hi");
                Console.WriteLine("HIII");
            }
            catch (Exception ex)
            {
                source.TraceEvent(TraceEventType.Error, 11, ex.ToString());
                Trace.WriteLine(ex.ToString());
                Console.WriteLine(ex.ToString());
            }
            finally {
                source.Close();
            }
        }
    }

    public class MyServerComponent : TracedClass
    {
        public MyServerComponent(ExtendedTraceSource source) : base(source)
        {
        }
    }

}
