using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Extensions.Consolidation.Forms.ConsForm.Pumpers;

namespace Krista.FM.RIA.Extensions.Consolidation.Forms.Org3PricesAndTariffs
{
    public class Pumper
    {
        private readonly ILinqRepository<T_Org_CPrice> reportFactRepository;
        private readonly ILinqRepository<F_Org_Price> olapFactRepository;
        private readonly ILinqRepository<DataSources> datasourceRepository;
        private readonly ILinqRepository<FX_Date_YearDayUNV> dateYearDayUNVRepository;
        private readonly ILinqRepository<D_Territory_RF> territoryRepository;

        private readonly ILinqRepository<D_Org_TypePrice> typePriceRepository;
        private readonly ILinqRepository<FX_Org_State> stateRepository;

        public Pumper(
                       ILinqRepository<T_Org_CPrice> reportFactRepository, 
                       ILinqRepository<F_Org_Price> olapFactRepository, 
                       ILinqRepository<DataSources> datasourceRepository, 
                       ILinqRepository<D_Org_TypePrice> typePriceRepository, 
                       ILinqRepository<FX_Date_YearDayUNV> dateYearDayUnvRepository, 
                       ILinqRepository<D_Territory_RF> territoryRepository, 
                       ILinqRepository<FX_Org_State> stateRepository)
        {
            this.reportFactRepository = reportFactRepository;
            this.olapFactRepository = olapFactRepository;
            this.datasourceRepository = datasourceRepository;
            this.typePriceRepository = typePriceRepository;
            dateYearDayUNVRepository = dateYearDayUnvRepository;
            this.territoryRepository = territoryRepository;
            this.stateRepository = stateRepository;
        }

        public void Pump(D_Org_Report report, PamperActionsEnum actions)
        {
            var rows = reportFactRepository.FindAll().Where(x => x.RefReport == report).ToList();

            int sourceId = GetSourceId();
            D_Territory_RF territory = GetTerritory(report);
            FX_Date_YearDayUNV day = GetDateYearDayUNV(report);

            var typePrice = typePriceRepository.FindOne(3); ////Розничная цена
            var state = stateRepository.FindOne(3); ////Статус - Принят

            // Удаляем предыдущие данные
            if ((actions & PamperActionsEnum.Clear) == PamperActionsEnum.Clear)
            {
                var oldFacts = olapFactRepository.FindAll()
                                             .Where(x => x.SourceID == sourceId
                                                      && x.RefOrg == typePrice
                                                      && x.RefTerritory == territory
                                                      && x.RefDay == day
                                                      && x.RefState == state)
                                             .ToList();
                foreach (var row in oldFacts)
                {
                    olapFactRepository.Delete(row);
                }

                olapFactRepository.DbContext.CommitChanges();
            }

            // Пишем новые данные
            if ((actions & PamperActionsEnum.Pump) == PamperActionsEnum.Pump)
            {
                foreach (var row in rows)
                {
                    var fact = new F_Org_Price
                                   {
                                       SourceID = sourceId,
                                       TaskID = -1,
                                       RefOrg = typePrice,
                                       RefTerritory = territory,
                                       RefDay = day,
                                       RefState = state,
                                       RefOrgRegistrOrg = row.RefRegistrOrg,
                                       RefGoodOrg = row.RefGood,
                                       Price = row.Price
                                   };

                    olapFactRepository.Save(fact);
                }

                olapFactRepository.DbContext.CommitChanges();
            }

            if ((actions & PamperActionsEnum.ClearPamp) == PamperActionsEnum.ClearPamp)
            {
                ProtectionFromDuplicatesInFactData(sourceId, typePrice, territory, day, state);
            }
        }

        private void ProtectionFromDuplicatesInFactData(int sourceId, D_Org_TypePrice typePrice, D_Territory_RF territory, FX_Date_YearDayUNV day, FX_Org_State state)
        {
            var facts = olapFactRepository.FindAll()
                                             .Where(x => x.SourceID == sourceId
                                                      && x.RefOrg == typePrice
                                                      && x.RefTerritory == territory
                                                      && x.RefDay == day
                                                      && x.RefState == state)
                                             .ToList();
            var countAll = facts.Count;
            var countDistinct = facts.Distinct(new FOrgPriceUniqComparer()).Count();
            if (countAll > countDistinct)
            {
                throw new Exception("При переносе данных обнаружены дубликаты. Попробуйте ещё раз.");
            }
        }

        private FX_Date_YearDayUNV GetDateYearDayUNV(D_Org_Report report)
        {
            var date = report.RefTask.BeginDate;
            int id = (date.Year * 10000) + (date.Month * 100) + date.Day;
            var entity = dateYearDayUNVRepository.FindOne(id);
            return entity;
        }

        private D_Territory_RF GetTerritory(D_Org_Report report)
        {
            var region = report.RefTask.RefSubject.RefRegion;
            var territory = territoryRepository.FindAll()
                                               .FirstOrDefault(x => x.RefRegionsBridge == region.RefBridgeRegions);
            if (territory == null)
            {
                throw new Exception(String.Format("Не найдена территория, соответствующая району: {0}", region.Name));
            }

            return territory;
        }

        private int GetSourceId()
        {
            const string SupplierCode = "ФО";
            const int DataCode = 2;
            const string DataName = "Ежемесячный отчет";

            var datasource = datasourceRepository.FindAll()
                                                 .FirstOrDefault(x => x.SupplierCode == SupplierCode
                                                                   && x.DataCode == DataCode
                                                                   && x.DataName == DataName);
            if (datasource == null)
            {
                throw new Exception("Не найден источник (SourceId)");
            }

            return datasource.ID;
        }

        private class FOrgPriceUniqComparer : IEqualityComparer<F_Org_Price>
        {
            public bool Equals(F_Org_Price x, F_Org_Price y)
            {
                return x.RefDay == y.RefDay
                       && x.SourceID == y.SourceID
                       && x.RefOrg == y.RefOrg
                       && x.RefTerritory == y.RefTerritory
                       && x.RefState == y.RefState
                       && x.RefGoodOrg == y.RefGoodOrg
                       && x.RefOrgRegistrOrg == y.RefOrgRegistrOrg;
            }

            public int GetHashCode(F_Org_Price obj)
            {
                return obj.ID.GetHashCode();
            }
        }
    }
}
