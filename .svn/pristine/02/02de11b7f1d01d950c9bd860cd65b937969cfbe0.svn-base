using System.Linq;

using Krista.FM.Common.Consolidation.Forms;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtensionModule.Services;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Extensions.Consolidation.Data;
using Krista.FM.RIA.Extensions.Consolidation.Forms;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DAL;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DomainGenerator;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DomainStore;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ExportReports;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ExportReports.Xml;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ImportReports;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ImportReports.Xml;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.Infrastructure;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.Metadata;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ScriptingEngine;
using Krista.FM.ServerLibrary;
using Microsoft.Practices.Unity;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public class ConsolidationExtensionInstaller : ExtensionInstallerBase, IExtensionInstaller
    {
        internal const string AdminIdentifier = "Krista.FM.RIA.Extensions.Consolidation.ConsolidationAdminExtensionInstaller, Krista.FM.RIA.Extensions.Consolidation";

        public ConsolidationExtensionInstaller()
            : base(typeof(ConsolidationExtensionInstaller).Assembly, "Krista.FM.RIA.Extensions.Consolidation.Config.xml")
        {
        }

        public string Identifier
        {
            get { return "Krista.FM.RIA.Extensions.Consolidation.ConsolidationExtensionInstaller, Krista.FM.RIA.Extensions.Consolidation"; }
        }

        public override void RegisterTypes(IUnityContainer container)
        {
            base.RegisterTypes(container);

            container.RegisterType<IReglamentService, ReglamentService>();
            container.RegisterType<ITaskBuilderFactory, TaskBuilderFactory>();
            container.RegisterType<ITaskService, TaskService>();
            container.RegisterType<IFormExportService, FormExportService>();
            container.RegisterType<IFormImportService, FormImportService>();
            container.RegisterType<IReportBuilder, ReportBuilder>();
            container.RegisterType<IFormScriptingEngine, FormScriptingEngine>();
            container.RegisterType<ScriptingEngineFactory, ScriptingEngineFactory>();
            container.RegisterType<IDatabaseObjectHashNameResolver, DatabaseObjectHashNameResolver>();
            container.RegisterType<IReportSectionDataService, ReportSectionDataService>();
            container.RegisterType<IReportRequisiteDataService, ReportRequisiteDataService>();
            container.RegisterType<ISectionRequisiteDataService, SectionRequisiteDataService>();
            container.RegisterType<IFormActivatorService, FormActivatorService>();
            container.RegisterType<IDomainFormsAssembliesStore, DomainFormsAssembliesStore>();
            container.RegisterType<IDomainTypesResolver, DomainTypesResolver>();
            container.RegisterType<IFormValidationService, FormValidationService>();
            container.RegisterType<IRebuildMappingService, RebuildMappingService>();
            container.RegisterType<ITaskBuilderService, TaskBuilderService>();
            container.RegisterType<IExportReportService, ExcelExportReportService>();
            container.RegisterType<IXmlExportReportService, XmlExportReportService>();
            container.RegisterType<IImportReportService, XmlImportReportService>();

            container.RegisterType<ISubjectTreeRepository, SubjectTreeRepository>();
            container.RegisterType<ICollectingTaskRepository, CollectingTaskRepository>();
            container.RegisterType<ICollectingPeriodRepository, CollectingPeriodRepository>();
            container.RegisterType<IReportDataRepository, ReportDataRepository>();

            // TODO: регистрация классов, реализующих IReportForm, через Reflection автоматически
            container.RegisterType<IReportForm, Forms.Mrot.Form>("Mrot.Form");
            container.RegisterType<IReportForm, Forms.MrotSummary.Form>("MrotSummary.Form");
            container.RegisterType<IReportForm, Forms.ConsForm.ReportForm>("ConsForm.ReportForm");
            container.RegisterType<IReportForm, Forms.Org3PricesAndTariffs.Gasoline.Form>("Gasoline.Form");
            container.RegisterType<IReportForm, Forms.Org3PricesAndTariffs.Consolidated.Form>("PriceAndTariffsCons.Form");
            container.RegisterType<IReportForm, Forms.Org3PricesAndTariffs.Food.Form>("Food.Form");
            
            // TODO: регистрация классов, необходимых для зарегистрированных форм
            Forms.Org3PricesAndTariffs.Gasoline.Form.Initialize(container);
            Forms.Org3PricesAndTariffs.Food.Form.Initialize(container);
            Forms.Org3PricesAndTariffs.Consolidated.Form.Initialize(container);
        }

        public override void ConfigureClientExtension(ClientExtensionService clientExtensionService)
        {
            base.ConfigureClientExtension(clientExtensionService);

            var extension = new UserSessionState(
                Resolver.Get<IScheme>(),
                Resolver.Get<IRepository<Users>>(),
                Resolver.Get<IRepository<D_Regions_Analysis>>(),
                Resolver.Get<ILinqRepository<D_CD_Subjects>>());

            extension.Initialize();

            Resolver.RegisterInstance<IUserSessionState>(extension, LifetimeManagerType.Session);

            // Инициализируем доменные объекты и выполняем маппинг
            DomainStoreInitializer.Instance().InitializeOnce(InitializeFormsDomain);
        }

        public void ConfigureNavigation(NavigationService navigationService, IParametersService parametersService)
        {
            navigationService.AddNavigation(base.ConfigureNavigation(parametersService));
        }

        public void InitializeFormsDomain()
        {
            var store = Resolver.Get<IDomainFormsAssembliesStore>();
            var formsRepository = Resolver.Get<ILinqRepository<D_CD_Templates>>();
            var rebuildMappingService = Resolver.Get<IRebuildMappingService>();

            // Получаем все активные и архивные формы и регистрируем их
            foreach (var form in formsRepository.FindAll().Where(x => x.Status > 0))
            {
                store.Register(form);
            }

            rebuildMappingService.Rebuild(store.GetAllAssemblies());
        }
    }
}
