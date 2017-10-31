using System;
using System.Collections.Generic;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtensionModule.Services;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Extensions.EO15TargetPrograms.Services;
using Krista.FM.ServerLibrary;
using Microsoft.Practices.Unity;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms
{
    public class TargetProgramsExtensionInstaller : ExtensionInstallerBase, IExtensionInstaller
    {
        public TargetProgramsExtensionInstaller()
            : base(typeof(TargetProgramsExtensionInstaller).Assembly, "Krista.FM.RIA.Extensions.EO15TargetPrograms.Config.xml")
        {
        }

        public string Identifier
        {
            get { return "Krista.FM.RIA.Extensions.EO15TargetPrograms.TargetProgramsExtensionInstaller, Krista.FM.RIA.Extensions.EO15TargetPrograms"; }
        }

        public void ConfigureNavigation(NavigationService navigationService, IParametersService parametersService)
        {
            navigationService.AddNavigation(base.ConfigureNavigation(parametersService));
        }

        public override void RegisterTypes(IUnityContainer container)
        {
            base.RegisterTypes(container);

            container.RegisterType<IProgramService, ProgramService>();
            container.RegisterType<INpaService, NpaService>();
            container.RegisterType<ITargetService, TargetService>();
            container.RegisterType<ITaskService, TaskService>();
            container.RegisterType<IActionService, ActionService>();
            container.RegisterType<IFinanceService, FinanceService>();
            container.RegisterType<ITargetRatingService, TargetRatingService>();
            container.RegisterType<ISubsidyService, SubsidyService>();
            container.RegisterType<IAdditionalService, AdditionalService>();
            container.RegisterType<IDatasourceService, DatasourceService>();
            container.RegisterType<IEstimateService, EstimateService>();
        }

        public override void ConfigureClientExtension(ClientExtensionService clientExtensionService)
        {
            base.ConfigureClientExtension(clientExtensionService);

            var extension = new Extension(Resolver.Get<IScheme>());
            extension.Initialize();

            Resolver.RegisterInstance<IExtension>(extension, LifetimeManagerType.Session);

            RegisterDatasources();
        }

        private void RegisterDatasources()
        {
            try
            {
                IDatasourceService datasourceService = Resolver.Get<IDatasourceService>();
                
                // Проверяем наличие источника для ввода "первоначальных данных"
                try
                {
                    var sourceId = datasourceService.GetDefaultDatasourceId();
                }
                catch (KeyNotFoundException)
                {
                    // Не нашли -> создаем
                    datasourceService.CreateDefaultDatasource();
                }

                // Проверяем наличие источника для ввода "фактических значений"
                try
                {
                    var sourceId = datasourceService.GetFactDatasourceId();
                }
                catch (KeyNotFoundException)
                {
                    // Не нашли -> создаем
                    datasourceService.CreateFactDatasource();
                }

                // Проверяем наличие источника для критериев
                try
                {
                    var sourceId = datasourceService.GetCriteriasSourceId(ProgramStage.Concept);
                }
                catch (KeyNotFoundException)
                {
                    // Не нашли -> создаем
                    datasourceService.CreateCriteriasDatasource(ProgramStage.Concept);
                }

                // Проверяем наличие источника для критериев
                try
                {
                    var sourceId = datasourceService.GetCriteriasSourceId(ProgramStage.Design);
                }
                catch (KeyNotFoundException)
                {
                    // Не нашли -> создаем
                    datasourceService.CreateCriteriasDatasource(ProgramStage.Design);
                }

                // Проверяем наличие источника для критериев
                try
                {
                    var sourceId = datasourceService.GetCriteriasSourceId(ProgramStage.Realization);
                }
                catch (KeyNotFoundException)
                {
                    // Не нашли -> создаем
                    datasourceService.CreateCriteriasDatasource(ProgramStage.Realization);
                }
            }
            catch (Exception e)
            {
                 Trace.TraceError("Ошибка инициализации \"Целевые программы\" - инициализация источников: {0}", Diagnostics.KristaDiagnostics.ExpandException(e));
            }
        }
    }
}
