// xSolon.EventsTests XSolonDictionaryTests.cs  8/24/2015 martin 

using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace xSolon.Events.Dto.Tests
{
    [TestClass()]
    public class XSolonDictionaryTests
    {
        [TestMethod()]
        public void XSolonDictionaryTest()
        {
            var dic = new XSolonDictionary();

            dic.Add("Test", "Balu");

            string raw = dic.Serialize();

            Trace.WriteLine(raw);

            XSolonDictionary dic2 = raw.DeserializeString<XSolonDictionary>();

            Assert.AreEqual(dic2, dic);
        }

        [TestMethod()]
        public void AddTest()
        {

            var dic = new XSolonDictionary();

            dic.Add("Test", "Balu");

            ArgumentException ex1 = null;
            try
            {
                dic.Add("Test", "Balu");
            }
            catch (ArgumentException ex)
            {

                ex1 = ex;
            }

            Assert.IsNotNull(ex1);

        }

        [TestMethod()]
        public void XSolonDictionaryTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void XSolonDictionaryTest2()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void XSolonDictionaryTest3()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ContainsKeyTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetDictionaryTest()
        {
            Assert.Fail();
        }
    }
}
