using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.Forecast.Forma2p
{
    public class SvodMOVarController : SchemeBoundController
    {
        private IForecastSvodMOVarsMORepository varsRepository;
        private IForecastSvodMOValuesMORepository valuesRepository;
        private IForecastSvodMOParamsMORepository paramsRepository;
        private ILinqRepository<D_Territory_RF> territoryRepository;
        private ILinqRepository<FX_Date_YearDayUNV> dateyearUNVRepository;

        public SvodMOVarController(
                                   IForecastSvodMOVarsMORepository varsRepository,
                                   ILinqRepository<D_Territory_RF> territoryRepository,
                                   ILinqRepository<FX_Date_YearDayUNV> dateyearUNVRepository,
                                   IForecastSvodMOValuesMORepository valuesRepository,
                                   IForecastSvodMOParamsMORepository paramsRepository)
        {
            this.varsRepository = varsRepository;
            this.territoryRepository = territoryRepository;
            this.dateyearUNVRepository = dateyearUNVRepository;
            this.valuesRepository = valuesRepository;
            this.paramsRepository = paramsRepository;
        }

        public ActionResult Load()
        {
            var list = from f in varsRepository.GetAllVars()
                       select new
                       {
                           f.ID,
                           f.Name,
                           Territory = f.RefTerritory.Name,
                           Year = f.RefYear.ID / 10000
                       };

            return new AjaxStoreResult(list, list.Count());
        }

        public ActionResult LoadTerritory()
        {
            var list = from f in territoryRepository.FindAll()
                       where (f.RefTerritorialPartType.ID > 3) //// && f.OKATO.StartsWith("63")
                       select new ListItem
                       {
                           Value = f.ID.ToString(),
                           Text = f.Name
                       };

            return new AjaxStoreResult(list, list.Count());
        }

        public ActionResult NewVar(string name, int year, int territory)
        {
            AjaxResult ar = new AjaxResult();

            var refYear = dateyearUNVRepository.FindOne((year * 10000) + 1);

            var refTerritory = territoryRepository.FindOne(territory);

            F_Forecast_VariantsMO variant = new F_Forecast_VariantsMO
            {
                Name = name,
                RefYear = refYear,
                RefTerritory = refTerritory
            };

            varsRepository.Save(variant);

            valuesRepository.DbContext.BeginTransaction();
            
            try
            {
                foreach (var param in paramsRepository.GetAllParams())
                {
                    T_Forecast_ValuesMO value = new T_Forecast_ValuesMO
                    {
                        RefParams = param,
                        RefVars = variant
                    };

                    valuesRepository.Save(value);
                }

                valuesRepository.DbContext.CommitTransaction();
            }
            catch (Exception e)
            {
                valuesRepository.DbContext.RollbackTransaction();
                throw new Exception(e.Message, e);
            }

            return ar;
        }
    }
}
