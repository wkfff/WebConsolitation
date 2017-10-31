using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.Forecast.Forma2p
{
    public class Form2pVarController : SchemeBoundController
    {
        private readonly IForecastForma2pVarRepository variantRepository;
        private readonly IForecastExtension extension;
        private readonly ILinqRepository<D_Forecast_VarScenCond> varScenCondRepository;
        private readonly ILinqRepository<D_Territory_RF> varTerritoryRepository;
        private readonly ILinqRepository<FX_Date_Year> dataYearRepository;
        private readonly ILinqRepository<DataSources> datasourceRepository;

        public Form2pVarController(
            IForecastForma2pVarRepository variantRepository, 
            IForecastExtension extension, 
            ILinqRepository<D_Forecast_VarScenCond> varScenCondRepository,
            ILinqRepository<D_Territory_RF> varTerritoryRepository,
            ILinqRepository<FX_Date_Year> dataYearRepository,
            ILinqRepository<DataSources> datasourceRepository)
        {
            this.variantRepository = variantRepository;
            this.extension = extension;
            this.varTerritoryRepository = varTerritoryRepository;
            this.varScenCondRepository = varScenCondRepository;
            this.dataYearRepository = dataYearRepository;
            this.datasourceRepository = datasourceRepository;
        }

        public ActionResult Load()
        {
            var view = from f in variantRepository.GetAllVariants()
                       select new
                       {
                           f.ID,
                           f.Name,
                           Year = f.RefYear.ID,
                           Forecast = f.RefForecast.Name,
                           Territory = f.RefTerritory.Name
                       };

            return new AjaxStoreResult(view, view.Count());
        }

        public ActionResult NewForm2p(string name, int estYear, int scenid, int terrid)
        {
            AjaxResult ar = new AjaxResult();
            
            D_Forecast_VarScenCond varScenCond = (from f in varScenCondRepository.FindAll() 
                                                  where f.ID == scenid
                                                 select f).First();

            D_Territory_RF territoryRf = (from f in varTerritoryRepository.FindAll() 
                                          where f.ID == terrid
                                          select f).First();

            FX_Date_Year year = (from f in dataYearRepository.FindAll()
                                 where f.ID == estYear
                                 select f).First();
            
            DataSources dataSources = (from f in datasourceRepository.FindAll()
                                       where (f.DataCode == 2) && (f.SupplierCode == "ЭО") && (f.DataName == "Прогноз") && (f.Year == estYear.ToString())
                                       select f).First();
            
            F_Forecast_VarForm2P newvar = new F_Forecast_VarForm2P 
            { 
                Name = name,
                RefTerritory = territoryRf,
                RefForecast = varScenCond,
                RefYear = year,
                TaskID = -1,
                SourceID = dataSources.ID
            };

            variantRepository.Save(newvar);
            variantRepository.DbContext.CommitChanges();

            ar.Result = "failure";

            if (newvar.ID > 0)
            {
                ////extension.Scheme.
                using (new ServerContext())
                {
                    if (Scheme.Form2pService.CreateNewForm2p(newvar.ID, estYear))
                    {
                        ar.Result = "success";
                    }
                }
            }
            
            return ar;
        }

        public ActionResult ComboVariantsLoad()
        {
            var list = (from f in varScenCondRepository.FindAll()
                       select new 
                       {
                           Value = f.ID,
                           Text = f.Name
                       }).ToList();

            return new AjaxStoreResult(list, list.Count);
        }

        public ActionResult ComboRegionsLoad()
        {
            var list = (from f in varTerritoryRepository.FindAll()
                        where f.RefTerritorialPartType.ID == 3
                        select new
                        {
                            Value = f.ID,
                            Text = f.Name
                        }).ToList();

            return new AjaxStoreResult(list, list.Count);
        }

        public ActionResult ExcelExport(int v1, int v2, int year)
        {
            byte[] buff;

            using (new ServerContext())
            {
                buff = Scheme.Form2pService.SaveFormToExcel(v1, v2, year);
            }

            FileContentResult fcr = new FileContentResult(buff, "application/vnd.ms-excel");

            fcr.FileDownloadName = "Форма2п_{0}.xls".FormatWith(year);

            return fcr;
        }

        public ActionResult ComboVarForm2pLoad(int selectedId)
        {
            if (selectedId != -1)
            {
                int year = (from f in variantRepository.GetAllVariants()
                            where f.ID == selectedId
                            select f.RefYear.ID).First();

                int terrid = (from f in variantRepository.GetAllVariants()
                              where f.ID == selectedId
                              select f.RefTerritory.ID).First();
                
                var list = (from f in variantRepository.GetAllVariants()
                            where (f.RefYear.ID == year) && 
                                  (f.RefTerritory.ID == terrid) 
                            select new
                            {
                                Value = f.ID,
                                Text = f.Name
                            }).ToList();

                ////list.Insert(0, variantRepository.GetAllVariants().Where(f => (f.ID == selectedId)).Select(f => new { Value = f.ID, Text = f.Name }).First());
                
                return new AjaxStoreResult(list, list.Count);
            }
            else
            {
                List<ListItem> list = new List<ListItem>();
                return new AjaxStoreResult(list, list.Count);
            }
        }
    }
}
