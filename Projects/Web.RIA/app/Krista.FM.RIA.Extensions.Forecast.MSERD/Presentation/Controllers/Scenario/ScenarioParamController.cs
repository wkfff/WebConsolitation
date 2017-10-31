using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.Forecast.MSERD
{
    public struct AdjsListItem
    {
        public int ID;
        public string ParamName;
        public decimal ValueEstimate;
        public decimal ValueY1;
        public decimal ValueY2;
        public decimal ValueY3;
        public decimal ValueY4;
        public decimal ValueY5;
        public decimal MaxBound;
        public decimal MinBound;
        public decimal IndexDef;
        public string UserName;
        public bool Finished;
        public string GroupName;
        public string Units;
    }

    public struct StatListItem
    {
        public int ID;
        public string ParamName;
        public decimal ValueBase;
        public decimal ValueEstimate;
        public string UserName;
        public bool Finished;
        public string GroupName;
        public string Units;
    }

    public struct UnregListItem
    {
        public int ID;
        public string ParamName;
        public decimal ValueEstimate;
        public decimal ValueY1;
        public decimal ValueY2;
        public decimal ValueY3;
        public decimal ValueY4;
        public decimal ValueY5;
        public string UserName;
        public bool Finished;
        public string GroupName;
        public string Units;
    }

    public struct IndListItem
    {
        public int ID;
        public string ParamName;
        public decimal ValueEstimate;
        public decimal ValueY1;
        public decimal ValueY2;
        public decimal ValueY3;
        public decimal ValueY4;
        public decimal ValueY5;
        public decimal MaxBound;
        public decimal MinBound;
        public string GroupName;
        public string Units;
    }
    
    public class ScenarioParamController : SchemeBoundController
    {
        public const string ViewRoot = "~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/";

        private readonly IForecastScenarioAdjsRepository adjsRepository;
        private readonly IForecastScenarioIndsRepository indsRepository;
        private readonly IForecastScenarioUnRegsRepository unregsRepository;
        private readonly IForecastScenarioStaticRepository staticRepository;
        private readonly IForecastScenarioVarsRepository varsRepository;
        
        public ScenarioParamController(
                    IForecastScenarioAdjsRepository adjsRepository,
                    IForecastScenarioIndsRepository indsRepository,
                    IForecastScenarioUnRegsRepository unregsRepository,
                    IForecastScenarioStaticRepository staticRepository,
                    IForecastScenarioVarsRepository varsRepository)
        {
            this.adjsRepository = adjsRepository;
            this.indsRepository = indsRepository;
            this.unregsRepository = unregsRepository;
            this.staticRepository = staticRepository;
            this.varsRepository = varsRepository;
        }

        public ActionResult ShowExist(int id)
        {
            var scenId = id;
            var viewControl = Resolver.Get<ScenarioParamView>();

            var year = varsRepository.FindOne(id).RefYear;
            
            viewControl.Initialize(scenId, year.ID);
            return View(ViewRoot + "View.aspx", viewControl);
        }
        
        public ActionResult AdjsLoad(int scenId)
        {
            string selectSQL;

            if (NHibernateSession.Current.Connection.GetType().Name == "OracleConnection")
            {
                selectSQL = @"select s.id, p.name, s.valueestimate, s.valueY1, s.valueY2, s.valueY3, s.valueY4, s.valueY5, s.maxbound, s.minbound, s.indexdef ,s.userid, s.finished, grp.name as groupname, o.symbol
from t_forecast_adjvalues s
left join d_forecast_parametrs p on p.id = s.refparams
left join v_forecast_scengroups grp on substr(p.code, 1, 3) = grp.groupid
left join d_units_okei o on o.id = p.refunits
where s.refscenario = {0}".FormatWith(scenId);
            }
            else
            {
                selectSQL = @"select s.id, p.name, s.valueestimate, s.valueY1, s.valueY2, s.valueY3, s.valueY4, s.valueY5, s.maxbound, s.minbound, s.indexdef ,s.userid, s.finished, grp.name as groupname, o.symbol
from t_forecast_adjvalues s
left join d_forecast_parametrs p on p.id = s.refparams
left join v_forecast_scengroups grp on substring(CONVERT(VARCHAR(6),p.code), 1, 3) = grp.groupid
left join d_units_okei o on o.id = p.refunits
where s.refscenario = {0}".FormatWith(scenId);
            }

            var queryResult = NHibernateSession.Current.CreateSQLQuery(selectSQL).List();

            List<AdjsListItem> list = new List<AdjsListItem>();

            foreach (object[] row in queryResult)
            {
                list.Add(new AdjsListItem
                {
                    ID = Convert.ToInt32(row[0]),
                    ParamName = Convert.ToString(row[1]),
                    ValueEstimate = Convert.ToDecimal(row[2]),
                    ValueY1 = Convert.ToDecimal(row[3]),
                    ValueY2 = Convert.ToDecimal(row[4]),
                    ValueY3 = Convert.ToDecimal(row[5]),
                    ValueY4 = Convert.ToDecimal(row[6]),
                    ValueY5 = Convert.ToDecimal(row[7]),
                    MaxBound = Convert.ToDecimal(row[8]),
                    MinBound = Convert.ToDecimal(row[9]),
                    IndexDef = Convert.ToDecimal(row[10]),
                    UserName = Scheme.UsersManager.GetUserNameByID(Convert.ToInt32(row[11])),
                    Finished = Convert.ToBoolean(row[12]),
                    GroupName = Convert.ToString(row[13]),
                    Units = Convert.ToString(row[14])
                });
            }

            /*var list = from f in adjsRepository.GetAllAdjs()
                       where f.RefScenario.ID == scenId
                       select new
                       {
                           f.ID,
                           ParamName = f.RefParams.Name,
                           f.ValueEstimate,
                           f.ValueY1,
                           f.ValueY2,
                           f.ValueY3,
                           f.ValueY4,
                           f.ValueY5,
                           f.MaxBound,
                           f.MinBound,
                           f.IndexDef,
                           f.UserID,
                           f.Finished
                       };*/

            return new AjaxStoreResult(list, list.Count());
        }

        public ActionResult AdjsSave(int scenId, string savedData)
        {
            AjaxStoreResult ar = new AjaxStoreResult(StoreResponseFormat.Save);
            StoreDataHandler dataHandler = new StoreDataHandler(String.Format("{{{0}}}", savedData));
            ChangeRecords<Dictionary<string, string>> data = dataHandler.ObjectData<Dictionary<string, string>>();

            var ci = CultureInfo.InvariantCulture.Clone() as CultureInfo;
            ci.NumberFormat.NumberDecimalSeparator = ",";

            adjsRepository.DbContext.BeginTransaction();

            try
            {
                foreach (var updated in data.Updated)
                {
                    var id = Convert.ToInt32(updated["ID"]);
                    var adj = adjsRepository.FindOne(id);

                    string val = updated["ValueEstimate"].Replace(".", ",");
                    var yearData = Decimal.Parse(val, ci);
                    adj.ValueEstimate = yearData;

                    val = updated["ValueY1"].Replace(".", ",");
                    yearData = Decimal.Parse(val, ci);
                    adj.ValueY1 = yearData;

                    val = updated["ValueY2"].Replace(".", ",");
                    yearData = Decimal.Parse(val, ci);
                    adj.ValueY2 = yearData;

                    val = updated["ValueY3"].Replace(".", ",");
                    yearData = Decimal.Parse(val, ci);
                    adj.ValueY3 = yearData;

                    val = updated["ValueY4"].Replace(".", ",");
                    yearData = Decimal.Parse(val, ci);
                    adj.ValueY4 = yearData;

                    val = updated["ValueY5"].Replace(".", ",");
                    yearData = Decimal.Parse(val, ci);
                    adj.ValueY5 = yearData;

                    val = updated["Finished"];
                    adj.Finished = Convert.ToBoolean(val);

                    adjsRepository.Save(adj);
                }

                adjsRepository.DbContext.CommitTransaction();
                ar.SaveResponse.Success = true;
            }
            catch (Exception e)
            {
                adjsRepository.DbContext.RollbackTransaction();
                ar.SaveResponse.Success = false;
                throw new Exception(e.Message, e);
            }

            return ar;
        }

        public ActionResult IndsLoad(int scenId)
        {
            string selectSQL;

            if (NHibernateSession.Current.Connection.GetType().Name == "OracleConnection")
            {
                selectSQL = @"select s.id, p.name, s.valueestimate, s.valueY1, s.valueY2, s.valueY3, s.valueY4, s.valueY5, s.maxbound, s.minbound, grp.name as groupname, o.symbol
from t_forecast_indvalues s
left join d_forecast_parametrs p on p.id = s.refparams
left join v_forecast_scengroups grp on substr(p.code, 1, 3) = grp.groupid
left join d_units_okei o on o.id = p.refunits
where s.refscenario = {0}".FormatWith(scenId);
            }
            else
            {
                selectSQL = @"select s.id, p.name, s.valueestimate, s.valueY1, s.valueY2, s.valueY3, s.valueY4, s.valueY5, s.maxbound, s.minbound, grp.name as groupname, o.symbol
from t_forecast_indvalues s
left join d_forecast_parametrs p on p.id = s.refparams
left join v_forecast_scengroups grp on substring(CONVERT(VARCHAR(6),p.code), 1, 3) = grp.groupid
left join d_units_okei o on o.id = p.refunits
where s.refscenario = {0}".FormatWith(scenId);
            }

            var queryResult = NHibernateSession.Current.CreateSQLQuery(selectSQL).List();

            List<IndListItem> list = new List<IndListItem>();

            foreach (object[] row in queryResult)
            {
                list.Add(new IndListItem
                {
                    ID = Convert.ToInt32(row[0]),
                    ParamName = Convert.ToString(row[1]),
                    ValueEstimate = Convert.ToDecimal(row[2]),
                    ValueY1 = Convert.ToDecimal(row[3]),
                    ValueY2 = Convert.ToDecimal(row[4]),
                    ValueY3 = Convert.ToDecimal(row[5]),
                    ValueY4 = Convert.ToDecimal(row[6]),
                    ValueY5 = Convert.ToDecimal(row[7]),
                    MaxBound = Convert.ToDecimal(row[8]),
                    MinBound = Convert.ToDecimal(row[9]),
                    GroupName = Convert.ToString(row[10]),
                    Units = Convert.ToString(row[11])
                });
            }

            /*var list = from f in indsRepository.GetAllInds()
                       where f.RefScenario.ID == scenId
                       select new
                       {
                           f.ID,
                           ParamName = f.RefParams.Name,
                           f.ValueEstimate,
                           f.ValueY1,
                           f.ValueY2,
                           f.ValueY3,
                           f.ValueY4,
                           f.ValueY5,
                           f.MaxBound,
                           f.MinBound
                       };*/
            
            return new AjaxStoreResult(list, list.Count());
        }

        public ActionResult StaticLoad(int scenId)
        {
            /*var list = from f in staticRepository.GetAllStatic()
                       where f.RefScenario.ID == scenId
                       select new
                       {
                           f.ID,
                           ParamName = f.RefParams.Name,
                           f.ValueBase,
                           f.ValueEstimate,
                           f.UserID,
                           f.Finished
                       };*/

            string selectSQL;
            
            if (NHibernateSession.Current.Connection.GetType().Name == "OracleConnection")
            {
                selectSQL = @"select s.id, p.name, s.valuebase, s.valueestimate, s.userid, s.finished, grp.name as groupname, o.symbol
from t_forecast_staticvalues s
left join d_forecast_parametrs p on p.id = s.refparams
left join v_forecast_scengroups grp on substr(p.code, 1, 3) = grp.groupid
left join d_units_okei o on o.id = p.refunits
where s.refscenario = {0}".FormatWith(scenId);
            }
            else
            {
                selectSQL = @"select s.id, p.name, s.valuebase, s.valueestimate, s.userid, s.finished, grp.name as groupname, o.symbol
from t_forecast_staticvalues s
left join d_forecast_parametrs p on p.id = s.refparams
left join v_forecast_scengroups grp on substring(CONVERT(VARCHAR(6),p.code), 1, 3) = grp.groupid
left join d_units_okei o on o.id = p.refunits
where s.refscenario = {0}".FormatWith(scenId);
            }
            
            var queryResult = NHibernateSession.Current.CreateSQLQuery(selectSQL).List();

            List<StatListItem> list = new List<StatListItem>();

            foreach (object[] row in queryResult)
            {
                list.Add(new StatListItem 
                             { 
                                 ID = Convert.ToInt32(row[0]),
                                 ParamName = Convert.ToString(row[1]),
                                 ValueBase = Convert.ToDecimal(row[2]),
                                 ValueEstimate = Convert.ToDecimal(row[3]),
                                 UserName = Scheme.UsersManager.GetUserNameByID(Convert.ToInt32(row[4])),
                                 Finished = Convert.ToBoolean(row[5]),
                                 GroupName = Convert.ToString(row[6]),
                                 Units = Convert.ToString(row[7])
                             });
            }

            return new AjaxStoreResult(list, list.Count());
        }

        public ActionResult StaticSave(int scenId, string savedData)
        {
            AjaxStoreResult ar = new AjaxStoreResult(StoreResponseFormat.Save);
            StoreDataHandler dataHandler = new StoreDataHandler(String.Format("{{{0}}}", savedData));
            ChangeRecords<Dictionary<string, string>> data = dataHandler.ObjectData<Dictionary<string, string>>();

            var ci = CultureInfo.InvariantCulture.Clone() as CultureInfo;
            ci.NumberFormat.NumberDecimalSeparator = ",";

            staticRepository.DbContext.BeginTransaction();

            try
            {
                foreach (var updated in data.Updated)
                {
                    var id = Convert.ToInt32(updated["ID"]);
                    var stat = staticRepository.FindOne(id);

                    string val = updated["ValueBase"].Replace(".", ",");
                    stat.ValueEstimate = Decimal.Parse(val, ci);

                    val = updated["ValueEstimate"].Replace(".", ",");
                    stat.ValueEstimate = Decimal.Parse(val, ci);
                    
                    val = updated["Finished"];
                    stat.Finished = Convert.ToBoolean(val);

                    staticRepository.Save(stat);
                }

                staticRepository.DbContext.CommitTransaction();
                ar.SaveResponse.Success = true;
            }
            catch (Exception e)
            {
                staticRepository.DbContext.RollbackTransaction();
                ar.SaveResponse.Success = false;
                throw new Exception(e.Message, e);
            }

            return ar;
        }

        public ActionResult UnregLoad(int scenId)
        {
            /*var list = from f in unregsRepository.GetAllUnRegs()
                       where f.RefScenario.ID == scenId
                       select new
                       {
                           f.ID,
                           ParamName = f.RefParams.Name,
                           f.ValueEstimate,
                           f.ValueY1,
                           f.ValueY2,
                           f.ValueY3,
                           f.ValueY4,
                           f.ValueY5,
                           f.UserID,
                           f.Finished
                       };*/

            string selectSQL;

            if (NHibernateSession.Current.Connection.GetType().Name == "OracleConnection")
            {
                selectSQL = @"select s.id, p.name, s.valueestimate, s.valueY1, s.valueY2, s.valueY3, s.valueY4, s.valueY5 ,s.userid, s.finished, grp.name as groupname, o.symbol
from t_forecast_unregadj s
left join d_forecast_parametrs p on p.id = s.refparams
left join v_forecast_scengroups grp on substr(p.code, 1, 3) = grp.groupid
left join d_units_okei o on o.id = p.refunits
where s.refscenario = {0}".FormatWith(scenId);
            }
            else
            {
                selectSQL = @"select s.id, p.name, s.valueestimate, s.valueY1, s.valueY2, s.valueY3, s.valueY4, s.valueY5 ,s.userid, s.finished, grp.name as groupname, o.symbol
from t_forecast_unregadj s
left join d_forecast_parametrs p on p.id = s.refparams
left join v_forecast_scengroups grp on substring(CONVERT(VARCHAR(6),p.code), 1, 3) = grp.groupid
left join d_units_okei o on o.id = p.refunits
where s.refscenario = {0}".FormatWith(scenId);
            }

            var queryResult = NHibernateSession.Current.CreateSQLQuery(selectSQL).List();

            List<UnregListItem> list = new List<UnregListItem>();

            foreach (object[] row in queryResult)
            {
                list.Add(new UnregListItem
                {
                    ID = Convert.ToInt32(row[0]),
                    ParamName = Convert.ToString(row[1]),
                    ValueEstimate = Convert.ToDecimal(row[2]),
                    ValueY1 = Convert.ToDecimal(row[3]),
                    ValueY2 = Convert.ToDecimal(row[4]),
                    ValueY3 = Convert.ToDecimal(row[5]),
                    ValueY4 = Convert.ToDecimal(row[6]),
                    ValueY5 = Convert.ToDecimal(row[7]),
                    UserName = Scheme.UsersManager.GetUserNameByID(Convert.ToInt32(row[8])),
                    Finished = Convert.ToBoolean(row[9]),
                    GroupName = Convert.ToString(row[10]),
                    Units = Convert.ToString(row[11])
                });
            }

            return new AjaxStoreResult(list, list.Count());
        }

        public ActionResult UnregSave(int scenId, string savedData)
        {
            AjaxStoreResult ar = new AjaxStoreResult(StoreResponseFormat.Save);
            StoreDataHandler dataHandler = new StoreDataHandler(String.Format("{{{0}}}", savedData));
            ChangeRecords<Dictionary<string, string>> data = dataHandler.ObjectData<Dictionary<string, string>>();

            var ci = CultureInfo.InvariantCulture.Clone() as CultureInfo;
            ci.NumberFormat.NumberDecimalSeparator = ",";

            unregsRepository.DbContext.BeginTransaction();

            try
            {
                foreach (var updated in data.Updated)
                {
                    var id = Convert.ToInt32(updated["ID"]);
                    var unreg = unregsRepository.FindOne(id);

                    string val = updated["ValueEstimate"].Replace(".", ",");
                    unreg.ValueEstimate = Decimal.Parse(val, ci);

                    val = updated["ValueY1"].Replace(".", ",");
                    unreg.ValueY1 = Decimal.Parse(val, ci);

                    val = updated["ValueY2"].Replace(".", ",");
                    unreg.ValueY2 = Decimal.Parse(val, ci);

                    val = updated["ValueY3"].Replace(".", ",");
                    unreg.ValueY3 = Decimal.Parse(val, ci);

                    val = updated["ValueY4"].Replace(".", ",");
                    unreg.ValueY4 = Decimal.Parse(val, ci);

                    val = updated["ValueY5"].Replace(".", ",");
                    unreg.ValueY5 = Decimal.Parse(val, ci);

                    val = updated["Finished"];
                    unreg.Finished = Convert.ToBoolean(val);

                    unregsRepository.Save(unreg);
                }

                unregsRepository.DbContext.CommitTransaction();
                ar.SaveResponse.Success = true;
            }
            catch (Exception e)
            {
                unregsRepository.DbContext.RollbackTransaction();
                ar.SaveResponse.Success = false;
                throw new Exception(e.Message, e);
            }

            return ar;
        }
    }
}
