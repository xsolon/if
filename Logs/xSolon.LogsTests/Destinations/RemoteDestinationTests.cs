using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xSolon.Logs.Destinations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xSolon.Events;
namespace xSolon.Logs.Destinations.Tests
{
    [TestClass()]
    public class RemoteDestinationTests
    {
        [TestMethod()]
        public void CommitTest()
        {
            var destination = new RemoteDestination()
            {
                AutoCommit = false,
                Level = (int)EventSeverity.Verbose,
                ServiceUrl = "http://md.xsolon.net/logs/slog.ashx"
            };

            using (var logger = new LoggerClass("TraceDestinationTests"))
            {
                logger.NotifyEvent += (e) =>
                {
                    e.Props["DomainKey"] = "STSv2";
                };

                logger.Destinations.Add(destination);

                logger.NotifyInformation("Information");

                logger.Notify((int)EventSeverity.Verbose - 1, () => { return "Not be logged"; });

                int count = destination.LogList.Count;

                Assert.AreEqual(1, count);
            }
        }

        [TestMethod()]
        public void CloneTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void HttpPostTest()
        {
            Assert.Fail();
        }
    }
}
