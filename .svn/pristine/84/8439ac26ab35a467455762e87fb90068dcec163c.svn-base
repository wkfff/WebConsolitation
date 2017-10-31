using System;
using System.Linq;
using Krista.FM.RIA.Core.ExtensionModule.Services;
using Krista.FM.RIA.Core.Gui;
using NUnit.Framework;

namespace Krista.FM.RIA.Core.Tests
{
    [TestFixture]
    public class NavigationServiceTests
    {
        [Test]
        public void CanAddNavigationTest()
        {
            NavigationService service = new NavigationService();
            service.AddNavigation(new NavigationList(null));
            var list = service.GetNavigations();

            Assert.AreEqual(1, list.Count());
        }

        [Test]
        public void CanClearNavigationTest()
        {
            NavigationService service = new NavigationService();
            service.AddNavigation(new NavigationList(null));
            var list = service.GetNavigations();

            Assert.AreEqual(1, list.Count());

            service.Clear();

            list = service.GetNavigations();

            Assert.AreEqual(0, list.Count());
        }
    }
}
