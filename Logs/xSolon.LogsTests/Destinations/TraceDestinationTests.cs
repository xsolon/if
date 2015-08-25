// xSolon.LogsTests TraceDestinationTests.cs  8/24/2015 martin 

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xSolon.Events;
using xSolon.Logs;
using xSolon.Logs.Destinations;

namespace xSolon.Instructions.DTO.Logs.Tests
{
    [TestClass()]
    public class TraceDestinationTests
    {
        [TestMethod()]
        public void LogTest()
        {
            var destination = new TraceDestination()
            {
                AutoCommit = false,
                Level = (int)EventSeverity.Verbose,
                WriteTrace = true
            };

            using (var logger = new LoggerClass("TraceDestinationTests"))
            {
                logger.Destinations.Add(destination);

                logger.NotifyInformation("Information");

                logger.Notify((int)EventSeverity.Verbose - 1, () => { return "Not be logged"; });

                int count = destination.LogList.Count;

                Assert.AreEqual(1, count);
            }
        }

        [TestMethod()]
        public void CommitTest()
        {

            using (var logger = new LoggerClass("TraceDestinationTests", new TraceDestination()
            {
                AutoCommit = false,
                Level = (int)EventSeverity.Verbose,
                WriteTrace = true
            }))
            {

                logger.NotifyInformation("Information");

                logger.CommitLog();

                Assert.IsNull(logger.Destinations.FirstOrDefault(i => i.LogList.Count > 0));

            }
        }

        [TestMethod()]
        public void CloneTest()
        {
            Assert.Fail();
        }
    }
}