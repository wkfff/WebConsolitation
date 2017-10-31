using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Domain.Services.FinSourceDebtorBook;
using Krista.FM.RIA.Core.Extensions;

namespace Krista.FM.RIA.Extensions.DebtBook.Presentation.ViewBuilders
{
    public class BebtBookServiceRegionFormView : BebtBookStructureServiceRegionFormViewBase
    {
        public BebtBookServiceRegionFormView(IVariantProtocolService protocolService, IDebtBookExtension extension, IParametersService parametersService) 
            : base(protocolService, extension, parametersService)
        {
        }

        protected override int GetFormRecordId(int structureId, int variantId, int regionId)
        {
            var record = new NHibernateLinqRepository<F_S_ServiceDebt>().FindAll()
                .Where(x => x.RefRegion.ID == regionId && x.RefVariant.ID == variantId && x.RefKind.ID == structureId);

            return record.First().ID;
        }
    }
}
