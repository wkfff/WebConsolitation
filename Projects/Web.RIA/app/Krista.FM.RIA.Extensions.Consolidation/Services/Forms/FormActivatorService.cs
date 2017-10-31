using System;

using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.Consolidation.Models;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DomainStore;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.Infrastructure;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ScriptingEngine;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms
{
    public class FormActivatorService : IFormActivatorService
    {
        private readonly ILinqRepository<D_CD_Templates> templatesRepository;
        private readonly IFormScriptingEngine scriptingEngine;
        private readonly IScheme scheme;
        private readonly IDomainFormsAssembliesStore assembliesStore;
        private readonly IRebuildMappingService rebuildMappingService;

        public FormActivatorService(
            ILinqRepository<D_CD_Templates> templatesRepository, 
            IFormScriptingEngine scriptingEngine, 
            IScheme scheme,
            IDomainFormsAssembliesStore assembliesStore,
            IRebuildMappingService rebuildMappingService)
        {
            this.templatesRepository = templatesRepository;
            this.scriptingEngine = scriptingEngine;
            this.scheme = scheme;
            this.assembliesStore = assembliesStore;
            this.rebuildMappingService = rebuildMappingService;
        }

        public void Activate(int formId)
        {
            var form = templatesRepository.FindOne(formId);
            if (form.Status != (int)FormStatus.Inactive)
            {
                throw new InvalidOperationException("Шаблон формы не может быть активирован, т.к. он уже активирован или архивный.");
            }

            form.Status = (int)FormStatus.Active;
            templatesRepository.Save(form);

            var scripts = scriptingEngine.Create(form, form.FormVersion);

            using (new ServerContext())
            using (var db = scheme.SchemeDWH.DB)
            {
                foreach (var sctipt in scripts)
                {
                    db.ExecQuery(sctipt, QueryResultTypes.NonQuery);
                }
            }
        }

        public void RebuildSession(D_CD_Templates form)
        {
            assembliesStore.Register(form);

            rebuildMappingService.Rebuild(assembliesStore.GetAllAssemblies());
        }
    }
}
