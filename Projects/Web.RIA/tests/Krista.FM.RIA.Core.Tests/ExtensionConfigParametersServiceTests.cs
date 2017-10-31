using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Core.Tests.Stub;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Core.Tests
{
    [TestFixture]
    public class ExtensionConfigParametersServiceTests
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
            var service = new ExtensionConfigParametersService(stream, container);

            var result = service.GetParameterValue("UserRegionType");

            mocks.VerifyAll();

            Assert.AreEqual("Subject", result);
        }

        [Test]
        [ExpectedException(typeof(Exception), UserMessage = "Конфигурационный параметр 'Not existing parameter' не найден.")]
        public void MustThrowException()
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "Krista.FM.RIA.Core.Tests.Data.extension.config.xml");
            var service = new ExtensionConfigParametersService(stream, null);

            service.GetParameterValue("Not existing parameter");
        }
    }
}
