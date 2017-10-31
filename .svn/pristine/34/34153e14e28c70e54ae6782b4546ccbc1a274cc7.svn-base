using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ext.Net;
using NUnit.Framework;

namespace Krista.FM.RIA.Core.Tests
{
    [TestFixture]
    public class ExtNetComponentListenerExtensionTests
    {
        [Test]
        public void AddBeforeTest()
        {
            ComponentListener componentListener = new ComponentListener();
            componentListener.Handler = "main";
            componentListener.AddBefore("before");

            Assert.AreEqual("before\r\nmain", componentListener.Handler);
        }

        [Test]
        public void AddAfterTest()
        {
            ComponentListener componentListener = new ComponentListener();
            componentListener.Handler = "main";
            componentListener.AddAfter("after");

            Assert.AreEqual("main\r\nafter", componentListener.Handler);
        }

        [Test]
        public void AddBeforeToEmptyTest()
        {
            ComponentListener componentListener = new ComponentListener();
            componentListener.AddBefore("before");

            Assert.AreEqual("before", componentListener.Handler);
        }

        [Test]
        public void AddAfterToEmptyTest()
        {
            ComponentListener componentListener = new ComponentListener();
            componentListener.AddAfter("after");

            Assert.AreEqual("after", componentListener.Handler);
        }
    }
}
