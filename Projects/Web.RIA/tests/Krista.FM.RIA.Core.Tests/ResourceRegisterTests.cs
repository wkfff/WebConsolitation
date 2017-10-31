using System;
using System.Reflection;
using Ext.Net;
using Krista.FM.Extensions;
using NUnit.Framework;

namespace Krista.FM.RIA.Core.Tests
{
    [TestFixture]
    public class ResourceRegisterTests
    {
        [SetUp]
        public void SetUp()
        {
            var dummy = typeof(RiaMvcApplication);
            dummy = typeof(MvcApplication);
            
            ResourceRegister.ResetCache();
        }

        [Test]
        public void ContentScriptTest()
        {
            var version = new AssemblyName(typeof(RiaMvcApplication).Assembly.FullName).Version.ToString();

            const string url = "/Content/js/Extension.View.js";
            var result = ResourceRegister.Script(url);

            Assert.AreEqual("{0}?v={1}".FormatWith(url, version), result);
        }

        [Test]
        public void EmbeddedScriptTest()
        {
            var version = new AssemblyName(typeof(MvcApplication).Assembly.FullName).Version.ToString();

            const string url = "/Krista.FM.RIA.Core/Content/js/Entity.Book.js/extention.axd";
            var result = ResourceRegister.Script(url);

            Assert.AreEqual("{0}?v={1}".FormatWith(url, version), result);
        }

        [Test]
        public void MissedEmbeddedScriptTest()
        {
            const string url = "/Krista.FM.RIA.Missed/Content/js/Entity.Book.js/extention.axd";
            var result = ResourceRegister.Script(url);

            Assert.AreEqual(url, result);
        }

        [Test]
        public void ExtNetEmbeddedScriptTest()
        {
            const string url = "/extnet/extnet-data-debug-js/ext.axd?v=34580";
            var result = ResourceRegister.Script(url);

            Assert.AreEqual(url, result);
        }

        [Test]
        public void ContentStyleTest()
        {
            var version = new AssemblyName(typeof(RiaMvcApplication).Assembly.FullName).Version.ToString();

            const string url = "/Content/js/Extension.View.css";
            var result = ResourceRegister.Style(url);

            Assert.AreEqual("{0}?v={1}".FormatWith(url, version), result);
        }

        [Test]
        public void ContentRegisterScriptTest()
        {
            var version = new AssemblyName(typeof(RiaMvcApplication).Assembly.FullName).Version.ToString();

            ResourceManager resourceManager = new ResourceManager();
            const string url = "/Content/js/Extension.View.css";
            resourceManager.RegisterScript("key", url);

            Assert.AreEqual("{0}?v={1}".FormatWith(url, version), resourceManager.ClientScriptIncludeBag["key"]);
        }

        [Test]
        public void ContentRegisterStyleTest()
        {
            var version = new AssemblyName(typeof(RiaMvcApplication).Assembly.FullName).Version.ToString();

            ResourceManager resourceManager = new ResourceManager();
            const string url = "/Content/js/Extension.View.css";
            resourceManager.RegisterStyle("key", url);

            Assert.AreEqual("{0}?v={1}".FormatWith(url, version), resourceManager.ClientStyleIncludeBag["key"]);
        }
    }
}
