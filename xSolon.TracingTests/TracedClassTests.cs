using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xSolon.Tracing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xSolon.TracingTests;
using System.Diagnostics;
namespace xSolon.Tracing.Tests
{
    [TestClass()]
    public class TracedClassTests
    {
        [TestMethod()]
        public void TracedClassTest()
        {

            var sample = new SampleClass();

            sample.Method1();
            sample.Method2("1", "2");

            sample.TraceInformation("---------------------------------------");

            sample = Extensions.Wrap<SampleClass>(sample);

            sample.Method1();
            sample.Method2("1", "2");

            var listeners = sample.Trace.Listeners;
            var listener = listeners.OfType<TransactionTraceListener>().First();

            sample.Indent();

            sample.TraceData(TraceEventType.Error, 100, "Error1");

            sample.UnIndent();

            sample.TraceData(TraceEventType.Verbose, 101, "End");
            var msg = listener.GetTransaction();

            Trace.WriteLine(msg.Length);
        }

        [TestMethod()]
        public void ActivityThenLogical()
        {

            var source = new ExtendedTraceSource();

            using (var scope = new ActivityScope(source, "MultipleTracedClassTestScope"))
            {
                var sample1 = new SampleClass();

                sample1 = Extensions.Wrap<SampleClass>(sample1);

                var sample2 = new SampleClass();

                var oppId = "MyLogicalOpp";

                using (var lScope = new LogicalOperationScope(source, "MyLogicalScope"))
                {
                    sample1.Method1();


                    using (var scope2 = new ActivityScope(source, "Sub Activity"))
                    {
                        using (var lScope1 = new LogicalOperationScope(source, "Scope 2"))
                            sample1.TraceInformation("test1");
                        sample2.TraceInformation("test2");
                    }
                }

                var listeners = sample2.Trace.Listeners;
                var listener = listeners.OfType<TransactionTraceListener>().First();

                var log = listener.GetTransaction();
            }
        }

        [TestMethod()]
        public void LogicalThenActivity()
        {

            var source = new ExtendedTraceSource();

            using (var lScope = new LogicalOperationScope(source, "Process C"))
            {

                var sample2 = new SampleClass(source);

                using (var scope = new ActivityScope(source, "Login"))
                {

                    sample2.TraceEvent(TraceEventType.Verbose, 1, "Logged in!");
                }

                using (var scope = new ActivityScope(source, "Validate"))
                {

                    var service = new TracingTests.ServiceReference1.Service1Client("BasicHttpBinding_IService1");

                    service.DoWork();

                    sample2.TraceEvent(TraceEventType.Verbose, 2, "Request is Valid");
                }

                using (var scope = new ActivityScope(source, "Update"))
                {
                    sample2.TraceEvent(TraceEventType.Verbose, 3, "Record updating");

                    using (var scope1 = new ActivityScope(source, "COmmit"))
                    {

                        sample2.TraceEvent(TraceEventType.Verbose, 4, "Commit worked");
                    }

                }

            }

        }

        void ClientCall()
        {

        }

        void ServerCall()
        {
            var source = new ExtendedTraceSource("ServerComponent");

            using (var lScope = new LogicalOperationScope(source, "Process B"))
            {

                var sample2 = new SampleClass(source);

                sample2.TraceEvent(TraceEventType.Information, 10, "Server Message for the Process");

            }


        }

        [TestMethod()]
        public void ServerLogicalThenActivity()
        {

        }

    }

}
