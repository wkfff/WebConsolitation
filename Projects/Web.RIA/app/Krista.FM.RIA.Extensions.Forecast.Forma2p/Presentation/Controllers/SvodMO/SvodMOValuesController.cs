using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.Forecast.Forma2p
{
    public class SvodMOValuesController : SchemeBoundController
    {
        private const string ViewRoot = "~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/";

        private readonly IForecastSvodMOValuesMORepository valuesRepository;
        private readonly IForecastSvodMOVarsMORepository varsRepository;
        
        public SvodMOValuesController(
            IForecastSvodMOValuesMORepository valuesRepository,
            IForecastSvodMOVarsMORepository varsRepository)
        {
            this.valuesRepository = valuesRepository;
            this.varsRepository = varsRepository;
        }

        public ActionResult ShowExist(int id)
        {
            var varid = id;

            var viewControl = Resolver.Get<SvodMOValuesView>();

            var year = varsRepository.FindOne(varid).RefYear.ID / 10000;

            viewControl.Initialize(varid, year);

            return View(ViewRoot + "View.aspx", viewControl);
        }
        
        public ActionResult Load(int varid)
        {
            var list = from f in valuesRepository.GetAllValues()
                       where f.RefVars.ID == varid
                       select new
                       {
                           f.ID,
                           ParamName = f.RefParams.Name,
                           Units = f.RefParams.RefUnits.Symbol,
                           f.R1,
                           f.R2,
                           f.Est,
                           f.Y1v1,
                           f.Y1v2,
                           f.Y2v1,
                           f.Y2v2,
                           f.Y3v1,
                           f.Y3v2,
                           Code = f.RefParams.Code
                       };

            return new AjaxStoreResult(list, list.Count());
        }

        public ActionResult Save(string savedData)
        {
            AjaxStoreResult ar = new AjaxStoreResult(StoreResponseFormat.Save);
            StoreDataHandler dataHandler = new StoreDataHandler(String.Format("{{{0}}}", savedData));
            ChangeRecords<Dictionary<string, string>> data = dataHandler.ObjectData<Dictionary<string, string>>();

            var ci = CultureInfo.InvariantCulture.Clone() as CultureInfo;
            ci.NumberFormat.NumberDecimalSeparator = ",";

            valuesRepository.DbContext.BeginTransaction();
            
            try
            {
                foreach (var updated in data.Updated)
                {
                    int id = Convert.ToInt32(updated["ID"]);

                    T_Forecast_ValuesMO record = valuesRepository.FindOne(id);

                    if (!String.IsNullOrEmpty(updated["R1"]))
                    {
                        record.R1 = Decimal.Parse(updated["R1"].Replace(".", ","), ci);
                    }

                    if (!String.IsNullOrEmpty(updated["R2"]))
                    {
                        record.R2 = Decimal.Parse(updated["R2"].Replace(".", ","), ci);
                    }

                    if (!String.IsNullOrEmpty(updated["Est"]))
                    {
                        record.Est = Decimal.Parse(updated["Est"].Replace(".", ","), ci);
                    }

                    if (!String.IsNullOrEmpty(updated["Y1v1"]))
                    {
                        record.Y1v1 = Decimal.Parse(updated["Y1v1"].Replace(".", ","), ci);
                    }

                    if (!String.IsNullOrEmpty(updated["Y1v2"]))
                    {
                        record.Y1v2 = Decimal.Parse(updated["Y1v2"].Replace(".", ","), ci);
                    }

                    if (!String.IsNullOrEmpty(updated["Y2v1"]))
                    {
                        record.Y2v1 = Decimal.Parse(updated["Y2v1"].Replace(".", ","), ci);
                    }

                    if (!String.IsNullOrEmpty(updated["Y2v2"]))
                    {
                        record.Y2v2 = Decimal.Parse(updated["Y2v2"].Replace(".", ","), ci);
                    }

                    if (!String.IsNullOrEmpty(updated["Y3v1"]))
                    {
                        record.Y3v1 = Decimal.Parse(updated["Y3v1"].Replace(".", ","), ci);
                    }

                    if (!String.IsNullOrEmpty(updated["Y3v2"]))
                    {
                        record.Y3v2 = Decimal.Parse(updated["Y3v2"].Replace(".", ","), ci);
                    }
                    
                    valuesRepository.Save(record);
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
