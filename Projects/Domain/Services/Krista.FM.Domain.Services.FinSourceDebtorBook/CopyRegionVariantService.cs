using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Practices.ServiceLocation;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook
{
    public class CopyRegionVariantService
    {
        private IScheme scheme;
        private RegionsAccordanceService regionsAccordance;

        private int sourceYear;
        private int destYear;

        public CopyRegionVariantService()
            : this(ServiceLocator.Current.GetInstance<IScheme>(),
                new RegionsAccordanceService())
        {
        }

        public CopyRegionVariantService(IScheme scheme, 
            RegionsAccordanceService regionsAccordanceService)
        {
            this.scheme = scheme;
            regionsAccordance = regionsAccordanceService;
            sourceYear = -1;
            destYear = -1;
        }

        public string CopyRegionData(int destinationVariant, int currentVariantYear, DateTime reportDate,
            int currentRegion, int sourceID)
        {
            int sourceVariant = -1;
            int sourceYear = -1;
            GetPrevDebtBookVariant(reportDate, ref sourceVariant, ref sourceYear);
            if (sourceVariant == -1)
                return "Вариант, с которого можно копировать данные, не найден";
            int sourceRegion = currentRegion;
            if (sourceYear != currentVariantYear)
            {
                List<int> regions = regionsAccordance.GetRegionsByYear(sourceYear, currentRegion, currentVariantYear);
                foreach (var region in regions)
                {
                    string regionFilter = string.Format("RefVariant = {0} and (RefRegion = {1} or RefRegion in (select id from d_Regions_Analysis where ParentID in (select id from d_Regions_Analysis where ParentID = {1})))", sourceVariant, region);
                    this.sourceYear = sourceYear;
                    destYear = currentVariantYear;
                    CopyToVariant(destinationVariant, sourceID, currentRegion, regionFilter);
                }
            }
            else
            {
                string regionFilter = string.Format("RefVariant = {0} and (RefRegion = {1} or RefRegion in (select id from d_Regions_Analysis where ParentID in (select id from d_Regions_Analysis where ParentID = {1})))", sourceVariant, sourceRegion);
                this.sourceYear = sourceYear;
                destYear = currentVariantYear;
                CopyToVariant(destinationVariant, sourceID, currentRegion, regionFilter);
            }
            
            return string.Format("Данные с варианта с ID = {0} были успешно скопированы", sourceVariant);
        }

        public string CopySubjectData(int destinationVariant, int currentVariantYear, DateTime variantDate,
            int currentRegion, int sourceID)
        {
            int sourceVariant = -1;
            int sourceYear = -1;
            GetPrevDebtBookVariant(variantDate, ref sourceVariant, ref sourceYear);
            if (sourceVariant == -1)
                return "Вариант, с которого можно копировать данные, не найден";
            int sourceRegion = currentRegion;
            if (sourceYear != currentVariantYear)
                sourceRegion = regionsAccordance.GetRegionsByYear(sourceYear, currentRegion, currentVariantYear)[0];
            string regionFilter = string.Format("RefVariant = {0} and RefRegion = {1}", sourceVariant, sourceRegion);
            CopyToVariant(destinationVariant, sourceID, currentRegion, regionFilter);
            return string.Format("Данные с варианта с ID = {0} были успешно скопированы", sourceVariant);
        }

        public string CopyAllData(int destinationVariant, int currentVariantYear, DateTime variantDate,
            int currentRegion, int sourceID)
        {
            int sourceVariant = -1;
            int sourceYear = -1;
            GetPrevDebtBookVariant(variantDate, ref sourceVariant, ref sourceYear);
            if (sourceVariant == -1)
                return "Вариант, с которого можно копировать данные, не найден";
            if (sourceYear != currentVariantYear)
                currentRegion = regionsAccordance.GetRegionsByYear(sourceYear, currentRegion, currentVariantYear)[0];
            string regionFilter = string.Format("RefVariant = {0}", sourceVariant);
            CopyToVariant(destinationVariant, sourceID, currentRegion, regionFilter);
            return string.Format("Данные с варианта с ID = {0} были успешно скопированы", sourceVariant);
        }

        /// <summary>
        /// копируем данные по долговой книге с варианта на вариант
        /// </summary>
        private void CopyToVariant(int destinationVariant, int sourceID, int region, string regionFilter)
        {
            IEntity creditsRegionEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBCreditincome);
            CopyVariantData(creditsRegionEntity, destinationVariant, sourceID, region, regionFilter);
            IEntity creditsSettlementsEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBCreditincomePos);
            CopyVariantData(creditsSettlementsEntity, destinationVariant, sourceID, region, regionFilter);
            IEntity guaranteeRegionEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBGuarantissued);
            CopyVariantData(guaranteeRegionEntity, destinationVariant, sourceID, region, regionFilter);
            IEntity guaranteeSettlementsEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBGuarantissuedPos);
            CopyVariantData(guaranteeSettlementsEntity, destinationVariant, sourceID, region, regionFilter);
            IEntity capitalEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBCapital);
            CopyVariantData(capitalEntity, destinationVariant, sourceID, region, regionFilter);
        }

        /// <summary>
        /// копирование данных на новый вариант
        /// </summary>
        private DataTable CopyData(IEntity entity, int newVariant, int sourceID, DataTable dt)
        {
            DataTable dtNew = dt.Clone();
            dt.BeginLoadData();
            dtNew.BeginLoadData();
            foreach (DataRow row in dt.Rows)
            {
                int oldRegion = Convert.ToInt32(row["RefRegion"]);
                int newRegion = regionsAccordance.GetRegionsByYear(destYear, oldRegion, sourceYear)[0];
                DataRow newRow = dtNew.Rows.Add(row.ItemArray);
                //newRow["ID"] = entity.GetGeneratorNextValue;
                newRow["RefVariant"] = newVariant;
                newRow["SourceID"] = sourceID;
                newRow["RefRegion"] = newRegion;
            }
            dtNew.EndLoadData();
            dt.EndLoadData();
            return dtNew;
        }

        /// <summary>
        /// получение и сохранение данных в базу
        /// </summary>
        private void CopyVariantData(IEntity entity, int destinationVariant, int sourceID, int region, string filter)
        {
            using (IDataUpdater upd = entity.GetDataUpdater(filter, null))
            {
                DataTable dt = new DataTable();
                upd.Fill(ref dt);
                dt = CopyData(entity, destinationVariant, sourceID, dt);
                upd.Update(ref dt);
            }
        }

        private void GetPrevDebtBookVariant(DateTime currentVariantDate, ref int prevVariantID, ref int prevVariantYear)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                IEntity variantEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_Variant_Schuldbuch);
                DataTable dtVariant = (DataTable)db.ExecQuery(string.Format("select id, ReportDate, ActualYear from {0}",
                    variantEntity.FullDBName), QueryResultTypes.DataTable);
                DataRow[] rows = dtVariant.Select(string.Format("ReportDate < '{0}'", currentVariantDate), "ReportDate DESC");
                if (rows.Length > 0)
                {
                    prevVariantID = Convert.ToInt32(rows[0]["ID"]);
                    prevVariantYear = Convert.ToInt32(rows[0]["ActualYear"]);
                }
            }
        }
    }
}
