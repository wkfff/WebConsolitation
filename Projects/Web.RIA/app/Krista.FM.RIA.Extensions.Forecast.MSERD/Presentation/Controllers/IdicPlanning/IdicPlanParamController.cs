using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.Forecast.MSERD
{
    public class IdicPlanParamController : SchemeBoundController
    {
        public const string ViewRoot = "~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/";

        private IForecastIdicAdjsRepository adjsRepository;
        private IForecastIdicIndsRepository indsRepository;
        private IForecastScenarioVarsRepository varsRepository;
        private IForecastScenarioAdjsRepository scenarioAdjsRepository;
        private IForecastScenarioIndsRepository scenarioIndsRepository;
        private IForecastIdicPlanRepository planRepository;
        private IForecastExtension extension;

        public IdicPlanParamController(
                                       IForecastIdicAdjsRepository adjsRepository,
                                       IForecastIdicIndsRepository indsRepository,
                                       IForecastScenarioVarsRepository varsRepository,
                                       IForecastIdicPlanRepository planRepository,
                                       IForecastScenarioAdjsRepository scenarioAdjsRepository,
                                       IForecastScenarioIndsRepository scenarioIndsRepository,
                                       IForecastExtension extension)
        {
            this.adjsRepository = adjsRepository;
            this.indsRepository = indsRepository;
            this.varsRepository = varsRepository;
            this.scenarioAdjsRepository = scenarioAdjsRepository;
            this.planRepository = planRepository;
            this.scenarioIndsRepository = scenarioIndsRepository;
            this.extension = extension;
        }

        public ActionResult ShowExist(int id)
        {
            var scenId = id;
            var viewControl = Resolver.Get<IdicPlanParamView>();

            var list = (from f in planRepository.GetAllIdicPlan()
                        where f.ID == scenId
                        select new
                        {
                            f.Parent
                        }).ToList().First();

            int baseId = list.Parent;

            string key = String.Format("idicPlanForm_{0}", id);

            if (extension.Forms.ContainsKey(key))
            {
                extension.Forms.Remove(key);
            }

            extension.Forms.Add(key, new UserFormsControls());

            UserFormsControls ufc = this.extension.Forms[key];

            Dictionary<int, int> usedAdj = new Dictionary<int, int>();
            Dictionary<int, int> usedInd = new Dictionary<int, int>();
            
            ufc.AddObject("usedAdj", usedAdj);
            ufc.AddObject("usedInd", usedInd);

            viewControl.Key = key;

            viewControl.Initialize(scenId, baseId);
            return View(ViewRoot + "View.aspx", viewControl);
        }

        public ActionResult AdjsLoad(int varId, string key)
        {
            UserFormsControls ufc = this.extension.Forms[key];

            Dictionary<int, int> usedAdj = ufc.GetObject("usedAdj") as Dictionary<int, int>;
            
            var list = from f in adjsRepository.GetAllIdicAdjs()
                       select new
                       {
                           f.ID,
                           ParamName = f.RefParam.Name,
                           Units = f.RefParam.RefUnits.Designation,
                           f.ValueEstimate,
                           f.ValueY1,
                           f.ValueY2,
                           f.ValueY3,
                           f.ValueY4,
                           f.ValueY5,
                           f.MinBound,
                           f.MaxBound,
                           Mask = (usedAdj != null) && usedAdj.ContainsKey(f.ID) ? usedAdj[f.ID] : 0
                       };

            return new AjaxStoreResult(list, list.Count());
        }
        
        public ActionResult IndsLoad(int varId, string key)
        {
            UserFormsControls ufc = this.extension.Forms[key];

            Dictionary<int, int> usedInd = ufc.GetObject("usedInd") as Dictionary<int, int>;

            var list = from f in indsRepository.GetAllIdicInds()
                       select new
                       {
                           f.ID,
                           ParamName = f.RefParam.Name,
                           Units = f.RefParam.RefUnits.Designation,
                           f.ValueEstimate,
                           f.ValueY1,
                           f.ValueY2,
                           f.ValueY3,
                           f.ValueY4,
                           f.ValueY5,
                           f.MinBound,
                           f.MaxBound,
                           f.LeftPenaltyCoef,
                           f.RightPenaltyCoef,
                           Mask = (usedInd != null) && usedInd.ContainsKey(f.ID) ? usedInd[f.ID] : 0
                       };

            return new AjaxStoreResult(list, list.Count());
        }

        public ActionResult LoadAdjGroup()
        {
            string query = "select v.groupid, v.name from dv.v_forecast_scengroups v where v.groupid like '2%'";

            var queryResult = NHibernateSession.Current.CreateSQLQuery(query).List();

            var lstGroup = new Dictionary<string, string>();

            foreach (object[] res in queryResult)
            {
                lstGroup.Add(Convert.ToString(res[0]), Convert.ToString(res[1]));
            }
            
            var list = from f in lstGroup
                       select new
                       {
                           Text = f.Value,
                           Value = f.Key
                       };

            return new AjaxStoreResult(list, list.Count());
        }

        public ActionResult LoadIndGroup()
        {
            string query = "select v.groupid, v.name from dv.v_forecast_scengroups v where v.groupid like '1%'";

            var queryResult = NHibernateSession.Current.CreateSQLQuery(query).List();

            var lstGroup = new Dictionary<string, string>();

            foreach (object[] res in queryResult)
            {
                lstGroup.Add(Convert.ToString(res[0]), Convert.ToString(res[1]));
            }

            var list = from f in lstGroup
                       select new
                       {
                           Text = f.Value,
                           Value = f.Key
                       };

            return new AjaxStoreResult(list, list.Count());
        }

        public ActionResult AddAdjsLoad(int groupId, int parentId)
        {
            var list = from f in scenarioAdjsRepository.GetAllAdjs()
                       where Convert.ToString(f.RefParams.Code).Substring(0, 3) == groupId.ToString() && (f.RefScenario.ID == parentId)
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
                           f.MinBound,
                           f.MaxBound,
                           Units = f.RefParams.RefUnits.Designation
                       };

            return new AjaxStoreResult(list, list.Count());
        }

        public ActionResult AddIndsLoad(int groupId, int parentId)
        {
            var list = from f in scenarioIndsRepository.GetAllInds()
                       where Convert.ToString(f.RefParams.Code).Substring(0, 3) == groupId.ToString() && (f.RefScenario.ID == parentId)
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
                           f.MinBound,
                           f.MaxBound,
                           f.LeftPenaltyCoef,
                           f.RightPenaltuCoef,
                           Units = f.RefParams.RefUnits.Designation
                       };

            return new AjaxStoreResult(list, list.Count());
        }

        public ActionResult AddAdjuster(int varId, int adjId)
        {
            var ar = new AjaxResult();

            var param = scenarioAdjsRepository.FindOne(adjId);

            var variant = planRepository.FindOne(varId);

            D_Forecast_IdicAdjs adjs = new D_Forecast_IdicAdjs
            {
                ValueEstimate = param.ValueEstimate,
                ValueY1 = param.ValueY1,
                ValueY2 = param.ValueY2,
                ValueY3 = param.ValueY3,
                ValueY4 = param.ValueY4,
                ValueY5 = param.ValueY5,
                MaxBound = param.MaxBound,
                MinBound = param.MinBound,
                RefParam = param.RefParams,
                RefIndicVars = variant
            };
            
            adjsRepository.Save(adjs);

            adjsRepository.DbContext.CommitChanges();

            return ar;
        }

        public ActionResult AddIndicator(int varId, int indId)
        {
            var ar = new AjaxResult();

            var param = scenarioIndsRepository.FindOne(indId);

            var variant = planRepository.FindOne(varId);

            D_Forecast_IdicInds inds = new D_Forecast_IdicInds
            {
                ValueEstimate = param.ValueEstimate,
                ValueY1 = param.ValueY1,
                ValueY2 = param.ValueY2,
                ValueY3 = param.ValueY3,
                ValueY4 = param.ValueY4,
                ValueY5 = param.ValueY5,
                MaxBound = param.MaxBound,
                MinBound = param.MinBound,
                RefParam = param.RefParams,
                RefIndicVars = variant,
                LeftPenaltyCoef = param.LeftPenaltyCoef,
                RightPenaltyCoef = param.RightPenaltuCoef
            };

            indsRepository.Save(inds);

            indsRepository.DbContext.CommitChanges();

            return ar;
        }

        public ActionResult SelectAdjYear(int paramId, string columnName, string key)
        {
            AjaxResult ar = new AjaxResult();

            UserFormsControls ufc = this.extension.Forms[key];

            Dictionary<int, int> usedAdj = ufc.GetObject("usedAdj") as Dictionary<int, int>;

            int mask;

            if (!usedAdj.ContainsKey(paramId))
            {
                usedAdj.Add(paramId, 0);
            }
            
            mask = usedAdj[paramId];

            int data = 0;

            switch (columnName)
            {
                case "ValueEstimate":
                    data = 0x20;
                    break;
                case "ValueY1":
                    data = 0x10;
                    break;
                case "ValueY2":
                    data = 0x08;
                    break;
                case "ValueY3":
                    data = 0x04;
                    break;
                case "ValueY4":
                    data = 0x02;
                    break;
                case "ValueY5":
                    data = 0x01;
                    break;
            }

            var current = mask & data;

            if (current != 0)
            {
                mask = 0; //// mask &= ~data; // unset
            }
            else
            {
                mask = data; //// mask |= data; // set;
            }

            usedAdj[paramId] = mask;

            ar.Result = "success";

            return ar;
        }

        public ActionResult SelectIndYear(int paramId, string columnName, string key)
        {
            AjaxResult ar = new AjaxResult();

            UserFormsControls ufc = this.extension.Forms[key];

            Dictionary<int, int> usedInd = ufc.GetObject("usedInd") as Dictionary<int, int>;

            int mask;

            if (!usedInd.ContainsKey(paramId))
            {
                usedInd.Add(paramId, 0);
            }

            mask = usedInd[paramId];

            int data = 0;

            switch (columnName)
            {
                case "ValueEstimate":
                    data = 0x20;
                    break;
                case "ValueY1":
                    data = 0x10;
                    break;
                case "ValueY2":
                    data = 0x08;
                    break;
                case "ValueY3":
                    data = 0x04;
                    break;
                case "ValueY4":
                    data = 0x02;
                    break;
                case "ValueY5":
                    data = 0x01;
                    break;
            }

            var current = mask & data;

            if (current != 0)
            {
                mask = 0; ////mask &= ~data; // unset
            }
            else
            {
                mask = data; ////mask |= data; // set;
            }

            usedInd[paramId] = mask;

            ar.Result = "success";

            return ar;
        }

        public ActionResult DeleteInd(int paramid, string key)
        {
            AjaxResult ar = new AjaxResult();

            ar.Result = "failure";

            var data = indsRepository.FindOne(paramid);

            if (data != null)
            {
                try
                {
                    indsRepository.Delete(data);
                    indsRepository.DbContext.CommitChanges();

                    UserFormsControls ufc = this.extension.Forms[key];

                    Dictionary<int, int> usedInd = ufc.GetObject("usedInd") as Dictionary<int, int>;

                    if (usedInd.ContainsKey(paramid))
                    {
                        usedInd.Remove(paramid);
                    }

                    ar.Result = "success";
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e);
                }
            }

            return ar;
        }

        public ActionResult DeleteAdj(int paramid, string key)
        {
            AjaxResult ar = new AjaxResult();

            ar.Result = "failure";

            var data = adjsRepository.FindOne(paramid);

            if (data != null)
            {
                try
                {
                    adjsRepository.Delete(data);
                    adjsRepository.DbContext.CommitChanges();

                    UserFormsControls ufc = this.extension.Forms[key];

                    Dictionary<int, int> usedAdj = ufc.GetObject("usedAdj") as Dictionary<int, int>;

                    if (usedAdj.ContainsKey(paramid))
                    {
                        usedAdj.Remove(paramid);
                    }

                    ar.Result = "success";
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e);
                }
            }

            return ar;
        }

        public ActionResult IdicPlan(int parentId, string key)
        {
            AjaxResult ar = new AjaxResult();

            UserFormsControls ufc = this.extension.Forms[key];

            Dictionary<int, int> usedInd = ufc.GetObject("usedInd") as Dictionary<int, int>;
            Dictionary<int, int> usedAdj = ufc.GetObject("usedAdj") as Dictionary<int, int>;

            Dictionary<int, string> ind = new Dictionary<int, string>();
            Dictionary<int, string> adj = new Dictionary<int, string>();

            foreach (KeyValuePair<int, int> pair in usedInd)
            {
                var refparam = indsRepository.FindOne(pair.Key).RefParam;

                var paramid = (from f in scenarioIndsRepository.GetAllInds()
                             where (f.RefParams == refparam) && (f.RefScenario.ID == parentId)
                             select f.ID).ToList().First();
                
                if ((pair.Value & 0x01) != 0)
                {
                    ind.Add(paramid, "ValueY5");
                }

                if ((pair.Value & 0x02) != 0)
                {
                    ind.Add(paramid, "ValueY4");
                }

                if ((pair.Value & 0x04) != 0)
                {
                    ind.Add(paramid, "ValueY3");
                }

                if ((pair.Value & 0x08) != 0)
                {
                    ind.Add(paramid, "ValueY2");
                }

                if ((pair.Value & 0x10) != 0)
                {
                    ind.Add(paramid, "ValueY1");
                }

                if ((pair.Value & 0x20) != 0)
                {
                    ind.Add(paramid, "ValueEstimate");
                }
            }

            foreach (KeyValuePair<int, int> pair in usedAdj)
            {
                var refparam = adjsRepository.FindOne(pair.Key).RefParam;

                var paramid = (from f in scenarioAdjsRepository.GetAllAdjs()
                               where (f.RefParams == refparam) && (f.RefScenario.ID == parentId)
                               select f.ID).ToList().First();

                if ((pair.Value & 0x01) != 0)
                {
                    adj.Add(paramid, "ValueY5");
                }

                if ((pair.Value & 0x02) != 0)
                {
                    adj.Add(paramid, "ValueY4");
                }

                if ((pair.Value & 0x04) != 0)
                {
                    adj.Add(paramid, "ValueY3");
                }

                if ((pair.Value & 0x08) != 0)
                {
                    adj.Add(paramid, "ValueY2");
                }

                if ((pair.Value & 0x10) != 0)
                {
                    adj.Add(paramid, "ValueY1");
                }

                if ((pair.Value & 0x20) != 0)
                {
                    adj.Add(paramid, "ValueEstimate");
                }
            }

            try
            {
                using (new ServerContext())
                {
                    Scheme.ForecastService.IdicPlanning(parentId, ind, adj);
                }

                foreach (KeyValuePair<int, int> pair in usedInd)
                {
                    var idicInd = indsRepository.FindOne(pair.Key);

                    var refparam = idicInd.RefParam;

                    var param = (from f in scenarioIndsRepository.GetAllInds()
                                   where (f.RefParams == refparam) && (f.RefScenario.ID == parentId)
                                   select f).ToList().First();

                    idicInd.ValueEstimate = param.ValueEstimate;
                    idicInd.ValueY1 = param.ValueY1;
                    idicInd.ValueY2 = param.ValueY2;
                    idicInd.ValueY3 = param.ValueY3;
                    idicInd.ValueY4 = param.ValueY4;
                    idicInd.ValueY5 = param.ValueY5;

                    indsRepository.Save(idicInd);
                    indsRepository.DbContext.CommitChanges();
                }

                foreach (KeyValuePair<int, int> pair in usedAdj)
                {
                    var idicAdj = adjsRepository.FindOne(pair.Key);

                    var refparam = idicAdj.RefParam;

                    var param = (from f in scenarioAdjsRepository.GetAllAdjs()
                                 where (f.RefParams == refparam) && (f.RefScenario.ID == parentId)
                                 select f).ToList().First();

                    idicAdj.ValueEstimate = param.ValueEstimate;
                    idicAdj.ValueY1 = param.ValueY1;
                    idicAdj.ValueY2 = param.ValueY2;
                    idicAdj.ValueY3 = param.ValueY3;
                    idicAdj.ValueY4 = param.ValueY4;
                    idicAdj.ValueY5 = param.ValueY5;

                    adjsRepository.Save(idicAdj);
                    adjsRepository.DbContext.CommitChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }

            return ar;
        }
    }
}
