using System;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Domain.Services.FinSourceDebtorBook;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Extensions;

namespace Krista.FM.RIA.Extensions.DebtBook.Presentation.ViewBuilders
{
    public class BebtBookLimitsRegionFormView : BebtBookFormView
    {
        public BebtBookLimitsRegionFormView(VariantProtocolService protocolService, IDebtBookExtension extension, IParametersService parametersService)
            : base(protocolService, extension, parametersService)
        {
            var regionId = extension.UserRegionId;
            var variantId = extension.Variant.Id;

            NHibernate.ITransaction transaction = NHibernateSession.Current.BeginTransaction();

            try
            {
                var reposotory = new NHibernateLinqRepository<F_S_SchBLimit>();

                var record = reposotory.FindAll()
                    .Where(x => x.RefRegion.ID == regionId && x.RefVariant.ID == variantId)
                    .FirstOrDefault();

                if (record == null)
                {
                    var region = new NHibernateLinqRepository<D_Regions_Analysis>().Get(regionId);
                    var variant = new NHibernateLinqRepository<D_Variant_Schuldbuch>().Get(variantId);
                    record = new F_S_SchBLimit
                                 {
                                     PumpID = -1,
                                     TaskID = -1,
                                     RefVariant = variant,
                                     RefRegion = region,
                                     SourceID = extension.CurrentSourceId
                                 };
                    reposotory.Save(record);
                    reposotory.DbContext.CommitChanges();
                }

                RecordId = record.ID;
                
                transaction.Commit();
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ExpandException());
                transaction.Rollback();
            }
        }
    }
}
