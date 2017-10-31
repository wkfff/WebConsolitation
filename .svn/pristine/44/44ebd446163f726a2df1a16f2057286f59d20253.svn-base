using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtensionModule.Services;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Extensions.EO12InvestProjects.Services;
using Microsoft.Practices.Unity;

namespace Krista.FM.RIA.Extensions.EO12InvestProjects
{
    public class InvestProjectsExtensionInstaller : ExtensionInstallerBase, IExtensionInstaller
    {
        public InvestProjectsExtensionInstaller() 
            : base(typeof(InvestProjectsExtensionInstaller).Assembly, "Krista.FM.RIA.Extensions.EO12InvestProjects.Config.xml")
        {
        }

        public string Identifier
        {
            get { return "Krista.FM.RIA.Extensions.EO12InvestProjects.InvestProjectsExtensionInstaller, Krista.FM.RIA.Extensions.EO12InvestProjects"; }
        }

        public void ConfigureNavigation(NavigationService navigationService, IParametersService parametersService)
        {
            navigationService.AddNavigation(base.ConfigureNavigation(parametersService));
        }

        public override void RegisterTypes(IUnityContainer container)
        {
            base.RegisterTypes(container);

            container.RegisterType<IProjectService, ProjectService>();
            container.RegisterType<IAdditionalDataService, AdditionalDataService>();
            container.RegisterType<IInvestPlanService, InvestPlanService>();
            container.RegisterType<ITargetRatingsService, TargetRatingsService>();
            container.RegisterType<IFilesService, FilesService>();
        }
    }
}
