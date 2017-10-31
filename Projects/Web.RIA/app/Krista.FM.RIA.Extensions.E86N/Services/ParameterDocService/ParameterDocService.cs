using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Services.AnnualBalance;
using Krista.FM.RIA.Extensions.E86N.Services.DocService;
using Krista.FM.RIA.Extensions.E86N.Services.InfControlMeasures;
using Krista.FM.RIA.Extensions.E86N.Services.PfhdService;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;
using Krista.FM.RIA.Extensions.E86N.Services.ResultsOfActivity;
using Krista.FM.RIA.Extensions.E86N.Services.SmetaService;
using Krista.FM.RIA.Extensions.E86N.Services.StateTaskService;

namespace Krista.FM.RIA.Extensions.E86N.Services.ParameterDocService
{
    public sealed class ParameterDocService : NewRestService, IParameterDocService
    {
        private readonly IDocService docService;
        private readonly IAnnualBalanceService annualBalanceService;
        private readonly IInfControlMeasuresService infControlMeasuresService;
        private readonly IPassportService passportService;
        private readonly IPfhdService pfhdService;
        private readonly IPfhd2017Service pfhd2017Service;
        private readonly IResultsOfActivityService resultsOfActivityService;
        private readonly ISmetaService smetaService;
        private readonly IStateTaskService stateTaskService;
        private readonly IStateTask2016Service stateTask2016Service;

        public ParameterDocService()
        {
            smetaService = Resolver.Get<ISmetaService>();
            pfhdService = Resolver.Get<IPfhdService>();
            pfhd2017Service = Resolver.Get<IPfhd2017Service>();
            docService = Resolver.Get<IDocService>();
            passportService = Resolver.Get<IPassportService>();
            stateTaskService = Resolver.Get<IStateTaskService>();
            stateTask2016Service = Resolver.Get<IStateTask2016Service>();
            resultsOfActivityService = Resolver.Get<IResultsOfActivityService>();
            infControlMeasuresService = Resolver.Get<IInfControlMeasuresService>();
            annualBalanceService = Resolver.Get<IAnnualBalanceService>();
        }

        public void Delete(int id)
        {
            passportService.GetItems(id).Each(x => passportService.Delete(x.ID));
            stateTaskService.DeleteDoc(id);
            stateTask2016Service.DeleteDoc(id);
            pfhdService.GetItems(id).Each(x => pfhdService.Delete(x.ID));
            pfhd2017Service.DeleteDoc(id);
            smetaService.GetItems(id).Each(x => smetaService.Delete(x.ID));
            docService.GetItems(id).Each(x => docService.Delete(x.ID));
            resultsOfActivityService.DeleteDoc(id);
            infControlMeasuresService.DeleteDoc(id);
            annualBalanceService.DeleteDoc(id);

            Delete<F_F_ParameterDoc>(id);
        }
    }
}
