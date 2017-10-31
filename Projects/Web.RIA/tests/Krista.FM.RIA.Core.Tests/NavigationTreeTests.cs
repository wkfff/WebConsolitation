using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using Krista.FM.RIA.Core.ExtensionModule;
using Krista.FM.RIA.Core.ExtensionModule.Services;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Core.Gui;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Core.Tests
{
    [TestFixture]
    public class NavigationTreeTests
    {
        [Test]
        public void CanBuild()
        {
            IUnityContainer container = MockRepository.GenerateMock<IUnityContainer>();

            ParametersService parametersService = new ParametersService(container);

            var installer = new ExtensionInstallerBase(
                Assembly.GetExecutingAssembly(),
                "Krista.FM.RIA.Core.Tests.Data.extension.config.xml");

            installer.RegisterParameters(parametersService);

            NavigationTree tree = new NavigationTree(parametersService);
            tree.Items.Add(new NavigationItem { ID = "itemId1", Title = "Test1", 
                Params = new List<NavigationItemParameter>
                             {
                                 new NavigationItemParameter { Name = "url", Value = "'/'" },
                                 new NavigationItemParameter { Name = "objectKey", Value = "'Guid'" },
                                 new NavigationItemParameter { Name = "userRegionType", Value = "$(UserRegionType)" }
                             }});
            tree.Build(new ViewPage());
        }
    }
}
