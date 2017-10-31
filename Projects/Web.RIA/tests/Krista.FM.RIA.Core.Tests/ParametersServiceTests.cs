using System;
using System.IO;
using System.Reflection;
using Krista.FM.RIA.Core.ExtensionModule.Services;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.Tests.Stub;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Core.Tests
{
    [TestFixture]
    public class ParametersServiceTests
    {
        [Test]
        public void CanGetParameter()
        {
            MockRepository mocks = new MockRepository();
            IUnityContainer container = mocks.DynamicMock<IUnityContainer>();
            
            Expect.Call(container.Resolve(typeof(ParameterValueProvider)))
                .Return(null);

            mocks.ReplayAll();

            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "Krista.FM.RIA.Core.Tests.Data.extension.config.xml");
            var service = new ParametersService(container);
            service.RegisterExtensionConfigParameters(stream);

            var result = service.GetParameterValue("UserRegionType");

            mocks.VerifyAll();

            Assert.AreEqual("Subject", result);
        }

        [Test]
        [ExpectedException(typeof(Exception), UserMessage = "Конфигурационный параметр 'Not existing parameter' не найден.")]
        public void MustThrowNotFoundException()
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "Krista.FM.RIA.Core.Tests.Data.extension.config.xml");
            var service = new ParametersService(null);
            service.RegisterExtensionConfigParameters(stream);

            service.GetParameterValue("Not existing parameter");
        }

        [Test]
        [ExpectedException(typeof(Exception), UserMessage = "Конфигурационный параметр \"Duplicate\" уже определен.")]
        public void MustThrowDublicateException()
        {
            var service = new ParametersService(null);

            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "Krista.FM.RIA.Core.Tests.Data.extension.config.xml");
            service.RegisterExtensionConfigParameters(stream);

            stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "Krista.FM.RIA.Core.Tests.Data.extension.duplicateparams.config.xml");
            service.RegisterExtensionConfigParameters(stream);
        }

        [Test]
        [ExpectedException(typeof(Exception), UserMessage = "Конфигурационный параметр 'Not existing parameter' не найден.")]
        public void CanClearNavigationTest()
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "Krista.FM.RIA.Core.Tests.Data.extension.config.xml");
            var service = new ParametersService(null);
            service.RegisterExtensionConfigParameters(stream);

            service.Clear();

            service.GetParameterValue("UserRegionType");
        }
    }
}
