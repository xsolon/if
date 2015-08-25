// xSolon.LogsTests LoggerClassTests.cs  8/24/2015 martin 
 
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xSolon.Events;
using xSolon.Logs.Destinations;
using xSolon.Events.Dto;

namespace xSolon.Logs.Tests
{
    [TestClass()]
    public class LoggerClassTests
    {
        [TestMethod()]
        public void LoggerClassTest()
        {
            var logger = new List<LoggerClass>()
            {
                new LoggerClass("Default",new TraceDestination()),
                new LoggerClass("Handler",new TraceDestination())
            };

            var raw = logger.Serialize(new Type[] { typeof(TraceDestination)});

            Trace.TraceInformation(raw);
        }

        [TestMethod()]
        public void LoggerClassSerializationTest()
        {
            var dic = new XSolonDictionary();
            var logger = new List<LoggerClass>()
            {
                new LoggerClass("Default",new TraceDestination()),
                new LoggerClass("Handler",new TraceDestination())
            };

            dic.Add("Test", logger);

            var raw = dic.Serialize(new Type[] {typeof(List<LoggerClass>), typeof(LoggerClass), typeof(TraceDestination)});

            Trace.TraceInformation(raw);
        }

        [TestMethod()]
        public void LoggerClassTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void LoggerClassTest2()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void NotifyGenericTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DisposeTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CommitLogTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CloneTest()
        {
            Assert.Fail();
        }
    }
}
