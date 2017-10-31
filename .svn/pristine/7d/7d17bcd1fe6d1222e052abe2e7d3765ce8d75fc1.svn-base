using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Domain.Services.FinSourceDebtorBook;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Extensions;
using NHibernate;

namespace Krista.FM.RIA.Extensions.DebtBook.Presentation.ViewBuilders
{
    public abstract class BebtBookStructureServiceRegionFormViewBase : BebtBookFormView
    {
        private static readonly List<string> regionsInitialized = new List<string>();
        private readonly int userRegionId;
        private readonly int variantId;

        public BebtBookStructureServiceRegionFormViewBase(IVariantProtocolService protocolService, IDebtBookExtension extension, IParametersService parametersService)
            : base(protocolService, extension, parametersService)
        {
            userRegionId = extension.UserRegionId;
            variantId = extension.Variant.Id;

            string key = "{0}_{1}".FormatWith(variantId, userRegionId);

            if (!regionsInitialized.Contains(key) && extension.UserRegionType != UserRegionType.Subject)
            {
                InitializeRows(extension, variantId, userRegionId);

                regionsInitialized.Add(key);
            }
        }

        public override List<Ext.Net.Component> Build(System.Web.Mvc.ViewPage page)
        {
            if (RecordId == 0)
            {
                RecordId = GetFormRecordId(TabRegionType, variantId, userRegionId);
            }

            return base.Build(page);
        }

        protected override bool NeedCreateChilds()
        {
            return false;
        }

        protected abstract int GetFormRecordId(int structureId, int variantId, int regionId);

        private static void InitializeRows(IDebtBookExtension extension, int variantId, int regionId)
        {
            ITransaction transaction = NHibernateSession.Current.BeginTransaction();

            try
            {
                InitializeEntityRows<F_S_ServiceDebt>(regionId, variantId, extension);
                InitializeEntityRows<F_S_StructureDebt>(regionId, variantId, extension);

                transaction.Commit();
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ExpandException());
                transaction.Rollback();
            }
        }

        private static void InitializeEntityRows<T>(int regionId, int variantId, IDebtBookExtension extension) 
            where T : DebtorBookStructureServiceBase, new()
        {
            var reposotory = new NHibernateLinqRepository<T>();

            var record = reposotory.FindAll()
                .Where(x => x.RefRegion.ID == regionId && x.RefVariant.ID == variantId)
                .FirstOrDefault();

            if (record == null)
            {
                var region = new NHibernateLinqRepository<D_Regions_Analysis>().Get(regionId);
                var variant = new NHibernateLinqRepository<D_Variant_Schuldbuch>().Get(variantId);

                foreach (var kindMunDebt in new NHibernateLinqRepository<FX_FX_KindMunDebt>().GetAll())
                {
                    record = new T
                    {
                        TaskID = -1,
                        RefVariant = variant,
                        RefRegion = region,
                        SourceID = extension.CurrentSourceId,
                        RefKind = kindMunDebt
                    };
                    reposotory.Save(record);
                } 

                reposotory.DbContext.CommitChanges();
            }
        }
    }
}
