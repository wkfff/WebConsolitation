using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using Ext.Net;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class PlanningView : View
    {
        private readonly IRepository<D_Forecast_PParams> paramsRepository;
        private readonly IForecastVariantsRepository variantsRepository;
        private readonly IForecastExtension extension;
        
        ////private PlanningVariantsParamsGridControl paramsGrid;
        ////private PlanningStatGridControl statGrid;
        ////private PlanningProgGridControl progGrid;

        private string key;

        private string name = String.Empty;
        private int dataFrom;
        private int dataTo;
        private int progFrom;
        private int progTo;
        private int status = -1;
        private int group = -1;
        private int method = -1;
        private int varForm2p = -1;
        
        private Store comboStore;

        public PlanningView(IRepository<D_Forecast_PParams> paramsRepository, IForecastVariantsRepository variantsRepository, IForecastExtension extension)
        {
            this.paramsRepository = paramsRepository;
            this.variantsRepository = variantsRepository;
            this.extension = extension;
        }

        public int ParamId { get; set; }

        public int VarId { get; private set; }

        public int Group
        {
            get { return group; }
            set { group = value; }
        }

        public int Method
        {
            get { return method; }
            set { method = value; }
        }

        public void Initialize(int varid)
        {
            VarId = varid;
            key = String.Format("planningForm_{0}", VarId);

            UserFormsControls ufc = extension.Forms[key];

            if (VarId != -1)
            {
                var variant = variantsRepository.FindOne(VarId);

                name = variant.Name;
                status = variant.Status;

                XDocument xDoc = XDocument.Parse(variant.XMLString);

                progFrom = Convert.ToInt32(xDoc.Root.Attribute("from").Value);
                progTo = Convert.ToInt32(xDoc.Root.Attribute("to").Value);
                varForm2p = Convert.ToInt32(xDoc.Root.Attribute("varform2p").Value);

                var usedDatas = xDoc.Root.Element("UsedDatas");
                
                ParamId = Convert.ToInt32(usedDatas.Attribute("fparamid").Value);

                Method = Convert.ToInt32(usedDatas.Attribute("method").Value);
                Group = Convert.ToInt32(usedDatas.Attribute("group").Value);

                var usedYears = xDoc.Root.Element("UsedYears");

                dataFrom = Convert.ToInt32(usedYears.Attribute("from").Value);
                dataTo = Convert.ToInt32(usedYears.Attribute("to").Value);
            }
            else
            {
            }

            ufc.ParamId = ParamId;

            ////paramsGrid = new PlanningVariantsParamsGridControl("PlanCont", key);
            ////statGrid = new PlanningStatGridControl(extension, String.Format("planningForm_{0}", VarId));
            ////progGrid = new PlanningProgGridControl(extension, String.Format("planningForm_{0}", VarId));
        }
        
        public override List<Component> Build(ViewPage page)
        {
            comboStore = CreateMethodStore();
            page.Controls.Add(comboStore);
            page.Controls.Add(CreateVarForm2pStore());
            
            ////var rm = page.Controls.OfType<HtmlForm>().First().Controls.OfType<ResourceManager>().First();
            
            BorderLayout layout = new BorderLayout
            {
                ID = "layoutMain",
                ////AutoScroll = true,
                ////AutoHeight = true
                ////Height = 400
            };
            
            ////BorderLayout layout = new BorderLayout { ID = "borderLayoutMain" };
            ////layout.North.Collapsible = false;
            ////layout.Split = true;

            layout.North.Items.Add(CreateToolBar());

            Panel centerPanel = new Panel
            {
                Border = false,
                AutoScroll = true
            };

            PlanningFormParamControl planningFormParamControl = new PlanningFormParamControl(
                Group, 
                Method, 
                dataTo, 
                dataFrom, 
                progTo, 
                progFrom, 
                status, 
                varForm2p,
                key, 
                name,
                extension);

            centerPanel.Items.Add(planningFormParamControl.Build(page));

            layout.Center.Items.Add(centerPanel);

            DataTable datStatic = this.extension.Forms[key].DataService.GetStaticData();
            DataTable datProg = this.extension.Forms[key].DataService.GetProgData();
            var statRowCount = datStatic.Rows.Count > 5 ? 5 : datStatic.Rows.Count;
            var progRowCount = datProg.Rows.Count > 5 ? 5 : datProg.Rows.Count + 1;

            Panel panelStat = new Panel
            {
                ////AnchorVertical = "30%",
                ID = "panelStat",
                Collapsible = true,
                ////AutoHeight = true,
                ////Split = true,
                Height = (50 + 15) + (statRowCount * 45) + 45,
                Title = "Исходные показатели",
                AutoLoad = 
                { 
                    Url = String.Format("/PlanningStat/Show/{0}", VarId), 
                    Mode = LoadMode.IFrame,
                    ManuallyTriggered = true,
                    ShowMask = true
                }
            };
            
            panelStat.Listeners.AfterRender.AddAfter("panelStat.reload();");

            Panel panelProg = new Panel
            {
                ////AnchorVertical = "70%",
                ////Split = true
                ID = "panelProg",
                Collapsible = true,
                ////AutoHeight = true,
                ////Height = 500,
                ////Split = true,
                Height = (50 + 15) + (progRowCount * 45) + 45,
                Title = "Прогнозируемые показатели",
                AutoLoad =
                {
                    Url = String.Format("/PlanningProg/Show/{0}", VarId),
                    Mode = LoadMode.IFrame,
                    ManuallyTriggered = true,
                    ShowMask = true
                }
            };
            
            panelProg.Listeners.AfterRender.AddAfter("panelProg.reload();");

            Panel panelReg = null;

            if (Group == FixedMathGroups.MultiRegression)
            {
                panelReg = new Panel
                {
                    ID = "panelReg",
                    Collapsible = true,
                    Title = "Регуляторы",
                };

                PlanningRegGridControl regGridControl = new PlanningRegGridControl(extension, key);

                panelReg.Items.Add(regGridControl.Build(page));

                panelReg.Listeners.AfterRender.AddAfter("stRegData.reload()");
            }
            
            Panel chartContainer = new Panel
            {
                ID = "chartContainer",
                ////Layout = "fit",
                Collapsible = true,
                Height = 400,
                ////AutoHeight = true,
                ////ColumnWidth = 0.7,
                Title = "График",
                /*AutoLoad =
                {
                    Url = String.Format("/PlanningChart/Show/{0}", VarId),
                    Mode = LoadMode.IFrame,
                    ManuallyTriggered = true,
                    ShowMask = true
                }*/
            };
            
            PlanningChartControl chartControl = new PlanningChartControl(extension, key);
            chartContainer.Items.Add(chartControl.Build(page));

            chartContainer.Listeners.BeforeCollapse.AddAfter("chart1.setVisible(false);");
            chartContainer.Listeners.Expand.AddAfter("chart1.setVisible(true)");
        
            ////chartContainer.Listeners.AfterRender.AddAfter("chartContainer.reload();");

            Panel formulaCoeffContainer = new Panel
            {
                ID = "formulaCoeffContainer",
                Layout = "fit",
                Collapsible = true,
                ////Height = 400,
                AutoHeight = true,
                ////ColumnWidth = 0.7,
                Title = "Описание метода"
            };

            PlanningFormulaCoeffControl formulaControl = new PlanningFormulaCoeffControl(extension, key);

            formulaCoeffContainer.Items.Add(formulaControl.Build(page));

            ////formulaCoeffContainer.Listeners.AfterRender.AddAfter("formulaCoeffContainer.reload();");

            Panel critsContainer = new Panel
            {
                ID = "critsContainer",
                Layout = "fit",
                Collapsible = true,
                ////Height = 400,
                AutoHeight = true,
                ////ColumnWidth = 0.7,
                Title = "Критерии прогноза"
            };

            PlanningCritsControl critsControl = new PlanningCritsControl(extension, key);
            critsContainer.Items.Add(critsControl.Build(page));
            
            ////critsContainer.Listeners.AfterRender.AddAfter("critsContainer.reload();");

            centerPanel.Items.Add(new List<Component> { panelStat, panelProg });

            if (panelReg != null)
            {
                centerPanel.Items.Add(panelReg);
            }

            centerPanel.Items.Add(new List<Component> { chartContainer, formulaCoeffContainer, critsContainer, PlanningStatGridControl.AddRowWindow(page, key) });

            ////centerPanel.Listeners.AfterRender

            ////topPanel.Items.Add(statGrid.Build(page));
            ////bottomPanel.Items.Add(progGrid.Build(page));
            
            Viewport viewport = new Viewport { ID = "viewportMain", Layout = "center" };
            viewport.Items.Add(layout);
            
            return new List<Component> { viewport };
        }

        private List<Component> CreateToolBar()
        {
            FormPanel toolbarPanel = new FormPanel
            {
                Border = false,
                ////AutoHeight = true,
                Height = 0,
                ////Width = 400,
                Collapsible = false,
                LabelWidth = 125,
                LabelAlign = LabelAlign.Right,
                Padding = 5,
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                Layout = "form"
            };

            Toolbar toolbar = new Toolbar
            {
                ID = "toolbar"
            };

            toolbarPanel.TopBar.Add(toolbar);

            Button btnSave = new Button
            {
                ID = "btnSave",
                Icon = Icon.Disk,
                ToolTip = "Сохранить"
            };

            btnSave.DirectEvents.Click.Url = "/ProgData/Save";
            btnSave.DirectEvents.Click.CleanRequest = true;
            btnSave.DirectEvents.Click.EventMask.ShowMask = true;
            btnSave.DirectEvents.Click.ExtraParams.Add(new Parameter("paramId", ParamId.ToString(), ParameterMode.Raw));
            btnSave.DirectEvents.Click.ExtraParams.Add(new Parameter("name", "tfName.getValue()", ParameterMode.Raw));
            btnSave.DirectEvents.Click.ExtraParams.Add(new Parameter("dataFrom", "sfDataFromYear.getValue()", ParameterMode.Raw));
            btnSave.DirectEvents.Click.ExtraParams.Add(new Parameter("dataTo", "sfDataToYear.getValue()", ParameterMode.Raw));
            btnSave.DirectEvents.Click.ExtraParams.Add(new Parameter("progFrom", "sfProgFromYear.getValue()", ParameterMode.Raw));
            btnSave.DirectEvents.Click.ExtraParams.Add(new Parameter("progTo", "sfProgToYear.getValue()", ParameterMode.Raw));
            btnSave.DirectEvents.Click.ExtraParams.Add(new Parameter("group", "cbGroupMethod.getSelectedItem().value", ParameterMode.Raw));
            btnSave.DirectEvents.Click.ExtraParams.Add(new Parameter("method", "cbMethod.getSelectedItem().value", ParameterMode.Raw));
            btnSave.DirectEvents.Click.ExtraParams.Add(new Parameter("status", "cbStatus.getSelectedItem().value", ParameterMode.Raw));
            btnSave.DirectEvents.Click.ExtraParams.Add(new Parameter("key", key, ParameterMode.Value));
            btnSave.DirectEvents.Click.ExtraParams.Add(new Parameter("varform2p", "cbForm2p.getSelectedItem().value", ParameterMode.Raw));
            btnSave.DirectEvents.Click.Success = "if (cbForm2p.getSelectedIndex() > -1) { btnInsertToForm2p.setDisabled(false) } else { btnInsertToForm2p.setDisabled(true) } ";
            
            ////btnSave.DirectEvents.Click.ExtraParams.Add(new Parameter("usedYears", "cbGroupMethod.getSelectedItem().value", ParameterMode.Raw));

            Button btnInsertToForm2p = new Button
            {
                ID = "btnInsertToForm2p",
                Icon = Icon.TableRowInsert,
                ToolTip = "Вставить в Форму 2п"
            };

            btnInsertToForm2p.Disabled = true;

            btnInsertToForm2p.DirectEvents.Click.Url = "/ProgData/InsertToForm2p";
            btnInsertToForm2p.DirectEvents.Click.ExtraParams.Add(new Parameter("varForm2p", "cbForm2p.getSelectedItem().value", ParameterMode.Raw));
            btnInsertToForm2p.DirectEvents.Click.ExtraParams.Add(new Parameter("key", String.Format("'{0}'", key), ParameterMode.Raw));
            btnInsertToForm2p.DirectEvents.Click.CleanRequest = true;
            btnInsertToForm2p.DirectEvents.Click.EventMask.ShowMask = true;

            Button btnCalc = new Button
            {
                ID = "Calc",
                Icon = Icon.Calculator,
                ToolTip = "Рассчитать"
            };

            btnCalc.DirectEvents.Click.Url = "/ProgData/Calc";
            btnCalc.DirectEvents.Click.CleanRequest = true;
            btnCalc.DirectEvents.Click.EventMask.ShowMask = true;
            btnCalc.DirectEvents.Click.ExtraParams.Add(new Parameter("predCount", "sfProgToYear.getValue() - sfProgFromYear.getValue() + 1", ParameterMode.Raw));
            btnCalc.DirectEvents.Click.ExtraParams.Add(new Parameter("group", "cbGroupMethod.getSelectedItem().value", ParameterMode.Raw));
            btnCalc.DirectEvents.Click.ExtraParams.Add(new Parameter("method", "cbMethod.getSelectedItem().value", ParameterMode.Raw));
            btnCalc.DirectEvents.Click.ExtraParams.Add(new Parameter("key", String.Format("'{0}'", key), ParameterMode.Raw));
            btnCalc.DirectEvents.Click.Success = @"
if (chart1.isVisible()) {
    stChartData.reload(); 
 
panelProg.getBody().stProgData.reload(); 
stCritData.reload(); 
stFormulaData.reload();
btnInsertToForm2p.setDisabled(true);
}";

            /*FormPanel formPanel = new FormPanel
            {
                ID = "frmButton"
            };*/
            
            Button btnExcelExp = new Button
            {
                ID = "ExcelExp",
                Icon = Icon.PageExcel,
                ToolTip = "Экспорт сохранненного варината в Excel",
                Type = ButtonType.Submit
            };

            btnExcelExp.DirectEvents.Click.Url = "/ProgData/ExportVariant";
            btnExcelExp.DirectEvents.Click.CleanRequest = true;
            btnExcelExp.DirectEvents.Click.IsUpload = true;
            ////btnExcelExp.DirectEvents.Click.
            //// btnExcelExp.DirectEvents.Click.EventMask.ShowMask = true;
            btnExcelExp.DirectEvents.Click.ExtraParams.Add(new Parameter("varid", "'{0}'".FormatWith(VarId), ParameterMode.Raw));

            ////formPanel.Add(btnExcelExp);
            
            ////btnCalc.DirectEvents.Click.Success = "stChartData.reload(); stProgData.reload(); stCritData.reload();";

            Button btnPumpData = new Button
            {
                ID = "PumpData",
                Icon = Icon.DatabaseRefresh,
                ToolTip = "Загрузка данных из базы"
            };

            btnPumpData.DirectEvents.Click.Url = "/ProgData/PumpData";
            btnPumpData.DirectEvents.Click.CleanRequest = true;
            ////bntPumpData.DirectEvents.Click.IsUpload = true;
            btnPumpData.DirectEvents.Click.EventMask.ShowMask = true;
            btnPumpData.DirectEvents.Click.ExtraParams.Add(new Parameter("varId", "'{0}'".FormatWith(VarId), ParameterMode.Raw));
            btnPumpData.DirectEvents.Click.ExtraParams.Add(new Parameter("key", String.Format("'{0}'", key), ParameterMode.Raw));
            btnPumpData.DirectEvents.Click.Success = @"
if (chart1.isVisible()) {
    stChartData.reload();
}
panelStat.getBody().stStaticData.reload();
panelProg.getBody().stProgData.reload();";
            
            toolbar.Items.Add(new List<Component> { btnSave, btnCalc, btnInsertToForm2p, btnExcelExp, btnPumpData });

            return new List<Component> { toolbarPanel };
        }
        
        /*private void ParamStoreDataBinding(object sender, EventArgs e)
        {
            if (paramsGrid.Store.DataSource == null)
            {
                paramsGrid.Store.DataSource = paramsRepository.GetAll();
                paramsGrid.Store.DataBind();
            }
        }*/
        
        private Store CreateMethodStore()
        {
            Store store = new Store
            {
                AutoLoad = false,
                ID = "methodStore"
            };

            store.DirectEventConfig.EventMask.ShowMask = false;
            ////store.RefreshData += new Store.AjaxRefreshDataEventHandler(MethodStoreRefreshData);

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "Value" };
            reader.Fields.Add("Text", RecordFieldType.String);
            reader.Fields.Add("Value", RecordFieldType.String);
            store.Reader.Add(reader);

            store.Listeners.Load.Handler = "cbMethod.setValue(cbMethod.store.getAt(0).get('id'));";

            store.BaseParams.Add(new Parameter("item", "cbGroupMethod.getSelectedItem().value", ParameterMode.Raw));
            
            HttpProxy httpProxy = new HttpProxy
            {
                Url = "/ProgData/GetMethod",
                Method = HttpMethod.POST
            };
            
            store.Proxy.Add(httpProxy);

            return store;
        }

        private Store CreateVarForm2pStore()
        {
            Store store = new Store
            {
                AutoLoad = false,
                ID = "varForm2pStore"
            };

            store.DirectEventConfig.EventMask.ShowMask = false;
            ////store.RefreshData += new Store.AjaxRefreshDataEventHandler(MethodStoreRefreshData);

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "Value" };
            reader.Fields.Add("Text", RecordFieldType.String);
            reader.Fields.Add("Value", RecordFieldType.String);
            reader.Fields.Add("Year", RecordFieldType.String);
            store.Reader.Add(reader);

            ////store.Listeners.Load.Handler = "cbMethod.setValue(cbMethod.store.getAt(0).get('id'));";

            ////store.BaseParams.Add(new Parameter("item", "cbGroupMethod.getSelectedItem().value", ParameterMode.Raw));

            HttpProxy httpProxy = new HttpProxy
            {
                Url = "/ProgData/VarForm2pStore",
                Method = HttpMethod.POST
            };

            store.Proxy.Add(httpProxy);

            return store;
        }
    }
}
