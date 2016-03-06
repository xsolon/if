using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xSolon.TraceListeners;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Messaging;

namespace xSolon.TraceListeners.Tests
{
    [TestClass()]
    public class MsmqTraceListenerTests
    {
        [TestMethod()]
        public void MsmqTraceListenerTest()
        {
            var path = @"FormatName:DIRECT=OS:.\private$\test2";
            path = @".\private$\test2";

            var listener = new MsmqTraceListener(path);

            listener = xSolon.TraceListeners.Extensions.Wrap<MsmqTraceListener>(listener);

            var source = new TraceSource("MSMQTEST1", SourceLevels.All);

            source.Listeners.Add(listener);
            source.Listeners.Add(new ConsoleTraceListener());

            source.TraceInformation("test1");

            source.TraceEvent(TraceEventType.Information, 1, "Test 2 {0}", 123);

        }

        [TestMethod()]
        public void MSMQ_Exists()
        {
            var path = @".\private$\test2";
            //path = @"precision1\private$\test1";
            if (MessageQueue.Exists(path))
            {

                var msmq = new MessageQueue(path);

                msmq.Send("Hello");
            }

        }

        [TestMethod()]
        public void ConfigTest()
        {
            var source = new TraceSource("MSMQTEST", SourceLevels.All);

            source.TraceInformation("test1");

            source.TraceEvent(TraceEventType.Error, 1, "Test 2 {0}", 123);

        }

        [TestMethod()]
        public void WriteLineTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CommitTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DisposeTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SendToMSMQTest()
        {
            Assert.Fail();
        }
    }
}
