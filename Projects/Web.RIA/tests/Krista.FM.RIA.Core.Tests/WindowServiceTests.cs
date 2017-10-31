using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ext.Net;
using Krista.FM.RIA.Core.ExtensionModule.Services;
using NUnit.Framework;

namespace Krista.FM.RIA.Core.Tests
{
    [TestFixture]
    public class WindowServiceTests
    {
        [Test]
        public void CanAddWindowTest()
        {
            WindowService service = new WindowService();
            service.AddWindow(new Window { ID = "w1" });
            var windows = service.GetWindows();

            Assert.AreEqual(1, windows.Count());
            Assert.AreEqual("w1", windows.First().ID);
        }

        [Test]
        public void CanClearWindowsTest()
        {
            WindowService service = new WindowService();
            service.AddWindow(new Window { ID = "w1" });
            var windows = service.GetWindows();

            Assert.AreEqual(1, windows.Count());
            Assert.AreEqual("w1", windows.First().ID);

            service.Clear();

            Assert.AreEqual(0, service.GetWindows().Count());
        }
    }
}
