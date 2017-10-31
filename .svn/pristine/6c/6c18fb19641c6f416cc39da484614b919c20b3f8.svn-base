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

namespace Krista.FM.RIA.Extensions.MarksOMSU.Tests
{
    [TestFixture]
    public class ExtensionInstallerTests
    {
        [Test]
        public void NavigationForMOTest()
        {
            MockRepository mocks = new MockRepository();

            IParametersService parametersService = mocks.DynamicMock<IParametersService>();
            Expect.Call(parametersService.GetParameterValue("UserName")).Return("mo").Repeat.Any();
            Expect.Call(parametersService.GetParameterValue("UserCanSeeIneffExpenses")).Return("0").Repeat.Any();

            IMarksOmsuExtension marksOmsuExtension = mocks.DynamicMock<IMarksOmsuExtension>();
            Expect.Call(marksOmsuExtension.Years).Return(new List<int> { 2010, 2011 });
            IUnityContainer container = mocks.DynamicMock<IUnityContainer>();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
            Expect.Call(container.Resolve(typeof(IMarksOmsuExtension)))
                .Return(marksOmsuExtension)
                .Repeat.Any();

            mocks.ReplayAll();

            ExtensionInstaller installer = new ExtensionInstaller();

            var navigationService = new NavigationService();
            installer.ConfigureNavigation(navigationService, parametersService);

            mocks.VerifyAll();

            Assert.AreEqual(1, navigationService.GetNavigations().Count());
            Assert.AreEqual(2, navigationService.GetNavigations().First().Items.Count());
        }

        [Test]
        public void NavigationForUIMTest()
        {
            MockRepository mocks = new MockRepository();

            IParametersService parametersService = mocks.DynamicMock<IParametersService>();

            Expect.Call(parametersService.GetParameterValue("UserName")).Return("econom1").Repeat.Any();
            Expect.Call(parametersService.GetParameterValue("UserCanSeeIneffExpenses")).Return("0").Repeat.Any();

            IMarksOmsuExtension marksOmsuExtension = mocks.DynamicMock<IMarksOmsuExtension>();
            Expect.Call(marksOmsuExtension.Years).Return(new List<int> { 2010, 2011 });
            IUnityContainer container = mocks.DynamicMock<IUnityContainer>();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
            Expect.Call(container.Resolve(typeof(IMarksOmsuExtension)))
                .Return(marksOmsuExtension)
                .Repeat.Any();

            mocks.ReplayAll();

            ExtensionInstaller installer = new ExtensionInstaller();

            var navigationService = new NavigationService();
            installer.ConfigureNavigation(navigationService, parametersService);

            mocks.VerifyAll();

            Assert.AreEqual(1, navigationService.GetNavigations().Count());
            Assert.AreEqual(4, navigationService.GetNavigations().First().Items.Count());
        }

        [Test]
        public void ViewsTest()
        {
            MockRepository mocks = new MockRepository();

            IParametersService parametersService = mocks.DynamicMock<IParametersService>();

            Expect.Call(parametersService.GetParameterValue("UserName")).Return("econom1").Repeat.Any();

            IUnityContainer container = mocks.DynamicMock<IUnityContainer>();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
            Expect.Call(container.Resolve(typeof(IPrincipalProvider)))
                .Return(new PrincipalProviderStub())
                .Repeat.Any();

            mocks.ReplayAll();

            ExtensionInstaller installer = new ExtensionInstaller();

            var viewService = new ViewService();
            installer.ConfigureViews(viewService, parametersService);

            mocks.VerifyAll();

            Assert.AreEqual("Ввод данных ОМСУ", viewService.GetView("MarksOMSU").Title);
            Assert.AreEqual("Ввод данных ОИВ", viewService.GetView("MarksOIV").Title);
            Assert.AreEqual("Соответствие пользователей ОИВ", viewService.GetView("OivUsers").Title);
            Assert.AreEqual("Утверждение доклада ОМСУ", viewService.GetView("MarksOMSUReport").Title);
        }
    }
}
