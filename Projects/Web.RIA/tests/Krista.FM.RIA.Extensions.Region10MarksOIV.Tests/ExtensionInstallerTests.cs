using System.Collections.Generic;
using System.Linq;
using Krista.FM.RIA.Core.ExtensionModule.Services;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Core.Principal;
using Krista.FM.RIA.Core.Tests.Stub;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Extensions.Region10MarksOIV.Tests
{
    [TestFixture]
    public class ExtensionInstallerTests
    {
        [Test]
        public void NavigationTest()
        {
            MockRepository mocks = new MockRepository();

            IParametersService parametersService = mocks.DynamicMock<IParametersService>();
            Expect.Call(parametersService.GetParameterValue("UserName")).Return("user").Repeat.Any();

            IRegion10MarksOivExtension marksOivExtension = mocks.DynamicMock<IRegion10MarksOivExtension>();
            Expect.Call(marksOivExtension.Years).Return(new List<int> { 2010, 2011 });
            
            IUnityContainer container = mocks.DynamicMock<IUnityContainer>();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
            Expect.Call(container.Resolve(typeof(IRegion10MarksOivExtension)))
                  .Return(marksOivExtension)
                  .Repeat.Any();

            mocks.ReplayAll();

            var installer = new Region10MarksOivExtensionInstaller();

            var navigationService = new NavigationService();
            installer.ConfigureNavigation(navigationService, parametersService);

            mocks.VerifyAll();

            Assert.AreEqual(1, navigationService.GetNavigations().Count());
            Assert.AreEqual(3, navigationService.GetNavigations().First().Items.Count());
        }

        [Test]
        public void ViewsTest()
        {
            MockRepository mocks = new MockRepository();

            IParametersService parametersService = mocks.DynamicMock<IParametersService>();

            Expect.Call(parametersService.GetParameterValue("UserName")).Return("user").Repeat.Any();

            IUnityContainer container = mocks.DynamicMock<IUnityContainer>();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
            Expect.Call(container.Resolve(typeof(IPrincipalProvider)))
                .Return(new PrincipalProviderStub())
                .Repeat.Any();

            mocks.ReplayAll();

            var installer = new Region10MarksOivExtensionInstaller();

            var viewService = new ViewService();
            installer.ConfigureViews(viewService, parametersService);

            mocks.VerifyAll();

            Assert.AreEqual("Ввод данных ОИВ", viewService.GetView("Region10MarksOIV").Title);
            Assert.AreEqual("Ввод данных ОМСУ", viewService.GetView("Region10MarksOMSU").Title);
            Assert.AreEqual("Сравнение данных ОМСУ", viewService.GetView("Region10MarksCompare").Title);
        }
    }
}
