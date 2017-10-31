using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class PlanningStatController : SchemeBoundController
    {
        public const string ViewRoot = "~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/";

        private readonly IForecastExtension extension;
        private readonly IForecastParamsRepository paramsRepository;
        private readonly IForecastRegulatorsRepository regulatorsRepository;

        public PlanningStatController(IForecastExtension extension, IForecastParamsRepository paramsRepository, IForecastRegulatorsRepository regulatorsRepository)
        {
            this.extension = extension;
            this.paramsRepository = paramsRepository;
            this.regulatorsRepository = regulatorsRepository;
        }
        
        public ActionResult Show(int id)
        {
            var viewControl = Resolver.Get<PlanningStatView>();

            viewControl.Initialize(id);

            return View(ViewRoot + "View.aspx", viewControl);
        }

        public ActionResult StaticLoad(string key)
        {
            UserFormsControls ufc = this.extension.Forms[key];

            DataTable datStatic = ufc.DataService.GetStaticData();

            /*  var dr = dt.AsEnumerable();
            var view = from f in dr
                       select new 
                       { 
                           Param = f.ItemArray[0],
                           year_2000 = f.ItemArray[1],
                           year_2001 = f.ItemArray[2],
                           year_2002 = f.ItemArray[3],
                           year_2003 = f.ItemArray[4],
                           year_2004 = f.ItemArray[5] 
                       };*/

            /*var dr = dt.AsEnumerable().AsQueryable();
            var view = dr.ToList();*/

            ////return new AjaxStoreResult(view, view.Count()););
            return new AjaxStoreResult(datStatic, datStatic.Rows.Count);
        }

        public ActionResult DeleteRow(int paramId, string key)
        {
            AjaxResult ar = new AjaxResult();

            UserFormsControls ufc = this.extension.Forms[key];

            DataTable datStatic = ufc.DataService.GetStaticData();

            List<ForecastStruct> foreList = ufc.DataService.GetForecastList();

            ar.Result = "failure";
            
            if (foreList.Count == 1)
            {
                var foreItem = foreList.First();

                if (foreItem.ForecastingParam.ParamId != paramId)
                {
                    for (int i = 0; i < datStatic.Rows.Count; i++)
                    {
                        DataRow row = datStatic.Rows[i];
                        if (Convert.ToInt32(row["id"]) == paramId)
                        {
                            datStatic.Rows[i].Delete();
                            ar.Result = "success";
                        }
                    }

                    var index = foreItem.UsedParams.FindIndex(x => x.ParamId == paramId);
                    if (index > -1)
                    {
                        foreItem.UsedParams.RemoveAt(index);
                    }
                }
                else
                {
                    ar.Script = @"Ext.MessageBox.show({
                    title: 'Ошибка',
                    msg: 'Недопускается удалять статистику по прогнозируемому параметру.',
                    buttons: Ext.MessageBox.OK,
                    icon: Ext.MessageBox.ERROR
                });";
                }
            }
            else
            {
                ar.Script = @"Ext.MessageBox.show({
                    title: 'Ошибка',
                    msg: 'Недопускается удалять параметр для предустановленной методики прогноза.',
                    buttons: Ext.MessageBox.OK,
                    icon: Ext.MessageBox.ERROR
                });";
            }

            return ar;
        }

        public ActionResult AddParamRow(int paramId, string key)
        {
            AjaxResult ar = new AjaxResult();

            UserFormsControls ufc = this.extension.Forms[key];
            
            DataTable datStatic = ufc.DataService.GetStaticData();

            List<ForecastStruct> foreList = ufc.DataService.GetForecastList();

            bool alreadyPresent = false;

            foreach (DataRow row in datStatic.Rows)
            {
                if (Convert.ToInt32(row["id"]) == paramId)
                {
                    ar.Result = "failes";
                    alreadyPresent = true;
                }
            }
            
            if ((!alreadyPresent) && (foreList.Count == 1))
            {
                var foreItem = foreList.First();
                int group = foreItem.Group;
                int method = foreItem.Method;
                int maxParamCount = 1;
                
                /*switch (group)
                {
                    case MathMethods.FirstOrderRegression:
                        maxParamCount = FirstOrderRegression.GetMethod(group).Value.MaxParamCount;
                        break;
                    case MathMethods.SecondOrderRegression:
                        maxParamCount = SecondOrderRegression.GetMethod(group).Value.MaxParamCount;
                        break;
                    case MathMethods.MultiRegression:
                        maxParamCount = MultiRegression.GetMethod(method).Value.MaxParamCount;
                        break;
                }*/

                var mathGroup = extension.LoadedMathGroups.GetGroupByCode(group);

                if (mathGroup.HasValue)
                {
                    var mathMethod = mathGroup.Value.Methods.GetMethodByCode(method);

                    if (mathMethod.HasValue)
                    {
                        maxParamCount = mathMethod.Value.MaxParamCount;
                    }
                }

                if (foreItem.UsedParams.Count < maxParamCount)
                {
                    var param = paramsRepository.FindOne(paramId);

                    DataRow dataRow = datStatic.NewRow();
                    dataRow["id"] = param.ID;
                    dataRow["Param"] = String.Format("{0}, {1}", param.Name, param.RefOKEI.Designation);

                    datStatic.Rows.Add(dataRow);

                    foreList.First().UsedParams.Add(new ForecastParameter { Name = param.Name, ParamId = param.ID });

                    ar.Result = "success";
                }
                else
                {
                    ar.Result = "failes";
                }
            }

            ar.Script = "wndAddRow.hide();";

            return ar;
        }

        public ActionResult AddRegRow(int regId, string key)
        {
            AjaxResult ar = new AjaxResult();

            UserFormsControls ufc = this.extension.Forms[key];

            DataTable datRegs = ufc.DataService.GetRegulatorData();

            List<ForecastStruct> foreList = ufc.DataService.GetForecastList();

            bool alreadyPresent = false;

            foreach (DataRow row in datRegs.Rows)
            {
                if (Convert.ToInt32(row["id"]) == regId)
                {
                    ar.Result = "failes";
                    alreadyPresent = true;
                }
            }

            if ((!alreadyPresent) && (foreList.Count == 1))
            {
                var foreItem = foreList.First();
                int group = foreItem.Group;
                int method = foreItem.Method;
                int maxParamCount = 1;
                
                var mathGroup = extension.LoadedMathGroups.GetGroupByCode(group);

                if (mathGroup.HasValue)
                {
                    var mathMethod = mathGroup.Value.Methods.GetMethodByCode(method);

                    if (mathMethod.HasValue)
                    {
                        maxParamCount = mathMethod.Value.MaxParamCount;
                    }
                }

                if (foreItem.UsedParams.Count < maxParamCount)
                {
                    var param = regulatorsRepository.FindOne(regId);

                    DataRow dataRow = datRegs.NewRow();
                    dataRow["id"] = param.ID;
                    dataRow["Param"] = String.Format("{0}, {1}", param.Name, param.RefUnits.Designation);
                    dataRow["fvarcode"] = "0";

                    datRegs.Rows.Add(dataRow);

                    foreList.First().UsedRegs.Add(new ForecastRegulator { Name = param.Name, RegId = param.ID, FVar = -1 });

                    ar.Result = "success";
                }
                else
                {
                    ar.Result = "failes";
                }
            }

            ar.Script = "wndAddRow.hide();";

            return ar;
        }

        public ActionResult ShowAddRowWnd(string key)
        {
            AjaxResult ar = new AjaxResult();

            UserFormsControls ufc = this.extension.Forms[key];
            
            List<ForecastStruct> foreList = ufc.DataService.GetForecastList();

            if (foreList.Count == 1)
            {
                var foreItem = foreList.First();
                int group = foreItem.Group;
                int method = foreItem.Method;
                int maxParamCount = 1;

                /*switch (group.ToString())
                {
                    case MathMethods.FirstOrderRegression:
                        maxParamCount = FirstOrderRegression.GetMethod(method).Value.MaxParamCount;
                        break;
                    case MathMethods.SecondOrderRegression:
                        maxParamCount = SecondOrderRegression.GetMethod(method).Value.MaxParamCount;
                        break;
                    case MathMethods.MultiRegression:
                        maxParamCount = MultiRegression.GetMethod(method).Value.MaxParamCount;
                        break;
                }*/

                var mathGroup = extension.LoadedMathGroups.GetGroupByCode(group);

                if (mathGroup.HasValue)
                {
                    var mathMethod = mathGroup.Value.Methods.GetMethodByCode(method);

                    if (mathMethod.HasValue)
                    {
                        maxParamCount = mathMethod.Value.MaxParamCount;
                    }
                }

                if (foreItem.UsedParams.Count < maxParamCount)
                {
                    ar.Script = "parent.wndAddRow.show();";
                }
                else
                {
                    ar.Script = @"Ext.MessageBox.show({
                        title: 'Ошибка',
                        msg: 'В прогнозе уже добавлено максимальное количество параметров.',
                        buttons: Ext.MessageBox.OK,
                        icon: Ext.MessageBox.ERROR
                    });"; 
                }
            }
            else
            {
                ar.Script = @"Ext.MessageBox.show({
                        title: 'Ошибка',
                        msg: 'В предустановленную методику прогноза недопускается добавлять новые параметры.',
                        buttons: Ext.MessageBox.OK,
                        icon: Ext.MessageBox.ERROR
                    });"; 
            }

            return ar;
        }

        public ActionResult ChangeStatData(int rowid, string col, double newVal, string key)
        {
            AjaxResult ajaxResult = new AjaxResult();

            UserFormsControls ufc = this.extension.Forms[key];

            DataTable datStatic = ufc.DataService.GetStaticData();

            foreach (DataRow row in datStatic.Rows)
            {
                if (Convert.ToInt32(row["id"]) == rowid)
                {
                    row[col] = newVal;
                    break;
                }
            }

            return ajaxResult;
        }

        public ActionResult CheckYear(int year, bool status, string key)
        {
            AjaxResult ajaxResult = new AjaxResult();

            UserFormsControls ufc = this.extension.Forms[key];

            SortedList<int, bool> arrYears = ufc.DataService.GetArrYears();

            arrYears[year] = status;

            return ajaxResult;
        }
    }
}
