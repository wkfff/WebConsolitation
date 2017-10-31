using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class PlanningFormParamControl : Control
    {
        private readonly IForecastExtension extension;

        private int group;
        private int method;
        private int dataTo;
        private int dataFrom;
        private int progTo;
        private int progFrom;
        private int status;
        private string key;
        private string name;
        private int varForm2p;

        public PlanningFormParamControl(
                            int group, 
                            int method, 
                            int dataTo, 
                            int dataFrom, 
                            int progTo, 
                            int progFrom, 
                            int status, 
                            int varForm2p,
                            string key, 
                            string name, 
                            IForecastExtension extension)
        {
            this.group = group;
            this.method = method;
            this.dataTo = dataTo;
            this.dataFrom = dataFrom;
            this.progTo = progTo;
            this.progFrom = progFrom;
            this.status = status;
            this.key = key;
            this.name = name;
            this.extension = extension;
            this.varForm2p = varForm2p;
        }
        
        public override List<Component> Build(ViewPage page)
        {
            FormPanel infoPanel = new FormPanel
            {
                Border = false,
                AutoHeight = true,
                ////Height = 130,
                ////Width = 400,
                Collapsible = true,
                Layout = "form",
                Title = "Параметры",
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                Padding = 10
            };

            TableLayout tableLayout = new TableLayout
            {
                Height = 200,
                Columns = 4
            };
            
            Cell columnLeft = new Cell { RowSpan = 2 };
            
            Cell columnFrom = new Cell { };

            Cell columnTo = new Cell { };

            Cell columnButton = new Cell { };

            Cell rowStatus = new Cell { };

            Cell columnForm2p = new Cell { ColSpan = 2 };

            tableLayout.Cells.Add(columnLeft);
            tableLayout.Cells.Add(columnFrom);
            tableLayout.Cells.Add(columnTo);
            tableLayout.Cells.Add(columnButton);
            tableLayout.Cells.Add(rowStatus);
            tableLayout.Cells.Add(columnForm2p);

            columnLeft.Items.Add(LeftBlock());

            columnFrom.Items.Add(SpinnerForm(progFrom, dataFrom));

            columnTo.Items.Add(SpinnerTo(dataTo, progTo));

            columnButton.Items.Add(ButtonRefresh(key));

            rowStatus.Items.Add(StatusCombo(status));

            columnForm2p.Items.Add(Form2pCombo(varForm2p));
            
            ////infoPanel.Listeners.AfterRender.Handler = "viewportMain.doLayout()";
            
            infoPanel.Items.Add(tableLayout);

            return new List<Component> { infoPanel };
        }

        private static Component SpinnerForm(int progFrom, int dataFrom)
        {
            Panel panel = new Panel
            {
                Width = 250,
                ////Height = 100,
                LabelWidth = 160,
                LabelAlign = LabelAlign.Right,
                Layout = "form",
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                Border = false
            };

            var spfProgFromYear = new SpinnerField
            {
                FieldLabel = "Прогнозный период с",
                Name = "sfProgFromYear",
                ID = "sfProgFromYear",
                MinValue = 1990,
                MaxValue = 2020,
                Width = 80
            };

            if (progFrom > 0)
            {
                spfProgFromYear.Value = progFrom;
            }

            var spfDataFromYear = new SpinnerField
            {
                FieldLabel = "Период исходных данных с",
                Name = "sfDataFromYear",
                ID = "sfDataFromYear",
                MinValue = 1990,
                MaxValue = 2015,
                Width = 80
            };

            if (dataFrom > 0)
            {
                spfDataFromYear.Value = dataFrom;
            }

            panel.Items.Add(spfDataFromYear);
            panel.Items.Add(spfProgFromYear);

            panel.AddScript("sfProgFromYear.setDisabled(true);sfDataFromYear.setDisabled(true);");

            return panel;
        }

        private static Component SpinnerTo(int dataTo, int progTo)
        {
            Panel panel = new Panel
            {
                Width = 140,
                ////Height = 100,
                LabelWidth = 50,
                LabelAlign = LabelAlign.Right,
                Layout = "form",
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                Border = false
            };

            var spfDataToYear = new SpinnerField
            {
                FieldLabel = "по",
                Name = "sfDataToYear",
                ID = "sfDataToYear",
                MinValue = 1990,
                MaxValue = 2015,
                Width = 80,
                ////ReadOnly = true
            };

            if (dataTo > 0)
            {
                spfDataToYear.Value = dataTo;
            }

            var spfProgToYear = new SpinnerField
            {
                FieldLabel = "по",
                Name = "sfProgToYear",
                ID = "sfProgToYear",
                MinValue = 1990,
                MaxValue = 2020,
                Width = 80,
                ////ReadOnly = true
            };

            if (progTo > 0)
            {
                spfProgToYear.Value = progTo;
            }

            panel.Items.Add(spfDataToYear);
            panel.Items.Add(spfProgToYear);

            panel.AddScript("sfProgToYear.setDisabled(true);sfDataToYear.setDisabled(true);");

            return panel;
        }

        private static Component StatusCombo(int status)
        {
            Panel panel = new Panel
            {
                LabelWidth = 80,
                ////Height = 50,
                Width = 250,
                LabelAlign = LabelAlign.Right,
                Layout = "form",
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                Border = false
            };

            ComboBox statusCombo = new ComboBox
            {
                FieldLabel = "Статус",
                Name = "cbStatus",
                ID = "cbStatus",
                Editable = false,
                Width = 160
            };

            statusCombo.Items.AddRange(new ListItemCollection<ListItem> 
                                           {
                                               new ListItem { Text = "Новый", Value = "0" },
                                               new ListItem { Text = "Рассчитанный", Value = "1" },
                                               new ListItem { Text = "Заблокированный", Value = "2" }
                                           });

            if (status > 0)
            {
                statusCombo.Value = status;
            }
            else
            {
                statusCombo.Value = 0;
            }

            panel.Items.Add(statusCombo);

            return panel;
        }
        
        private static Component Form2pCombo(int varForm2p)
        {
            Panel panel = new Panel
            {
                LabelWidth = 70,
                ////Height = 50,
                Width = 250,
                LabelAlign = LabelAlign.Right,
                Layout = "form",
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                Border = false
            };

            ComboBox form2pCombo = new ComboBox
            {
                FieldLabel = "Форма-2п",
                Name = "cbForm2p",
                ID = "cbForm2p",
                Editable = false,
                StoreID = "varForm2pStore",
                EmptyText = "(не сопоставлена)",
                ValueNotFoundText = "(не сопоставлена)",
                TypeAhead = true,
                Mode = DataLoadMode.Local,
                ForceSelection = true,
                TriggerAction = TriggerAction.All,
                DisplayField = "Text",
                ValueField = "Value",
                Width = 170
            };

            form2pCombo.Value = varForm2p;

            form2pCombo.Template.Html = @"
<tpl for=""."">
    <tpl if=""[xindex] == 1""><table><tr><th><b>Наименование варианта формы-2п</b></th><th><b>Период</b></th></tr></tpl>
	
    <tr class = ""list-item"">
		<td style=""padding:3px 0px;"">{Text}</td>		        
        <td>{Year}</td>
	</tr>

	<tpl if=""[xcount] == [xindex]""></table></tpl>
</tpl>";
            form2pCombo.ItemSelector = "tr.list-item";

            form2pCombo.Listeners.Render.Handler = "varForm2pStore.reload();";
            form2pCombo.Listeners.Expand.Handler = "varForm2pStore.reload();";

            ////form2pCombo.Listeners.Select.Handler = "if (cbForm2p.getSelectedIndex() > -1) { btnInsertToForm2p.setDisabled(false) } else { btnInsertToForm2p.setDisabled(true) } ";

            panel.Items.Add(form2pCombo);

            return panel;
        }

        private static Component ButtonRefresh(string key)
        {
            Panel panel = new Panel
            {
                Width = 110,
                ////Height = 100,
                Layout = "form",
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                Border = false
            };

            Button btnEdit = new Button
            {
                ID = "btnEdit",
                Text = "Изменить периоды",
                Visible = true
            };

            btnEdit.DirectEvents.Click.Url = "/ProgData/BeginChangeYears";
            btnEdit.DirectEvents.Click.CleanRequest = true;

            Button btnRefresh = new Button
            {
                ID = "btnRefresh",
                Text = "Принять изменения",
                Visible = true
            };

            string script = "btnRefresh.setVisible(false);";

            btnRefresh.AddScript(script);

            btnRefresh.DirectEvents.Click.Url = "/ProgData/ApplyChangeYears";
            btnRefresh.DirectEvents.Click.CleanRequest = true;
            btnRefresh.DirectEvents.Click.ExtraParams.Add(new Parameter("key", String.Format("'{0}'", key), ParameterMode.Raw));
            btnRefresh.DirectEvents.Click.ExtraParams.Add(new Parameter("dataFrom", "sfDataFromYear.getValue()", ParameterMode.Raw));
            btnRefresh.DirectEvents.Click.ExtraParams.Add(new Parameter("dataTo", "sfDataToYear.getValue()", ParameterMode.Raw));
            btnRefresh.DirectEvents.Click.ExtraParams.Add(new Parameter("progFrom", "sfProgFromYear.getValue()", ParameterMode.Raw));
            btnRefresh.DirectEvents.Click.ExtraParams.Add(new Parameter("progTo", "sfProgToYear.getValue()", ParameterMode.Raw));
            btnRefresh.DirectEvents.Click.Success = "if (result.result == \"success\") { panelStat.reload(); panelProg.reload();stChartData.reload(); }"; ////"stProgData.load();stStaticData.load();";
            btnRefresh.DirectEvents.Click.Failure = String.Empty;

            panel.Items.Add(btnEdit);
            panel.Items.Add(btnRefresh);

            return panel;
        }
        
        private Component LeftBlock()
        {
            Panel panel = new Panel
            {
                Width = 450,
                ////Height = 150,
                LabelWidth = 125,
                LabelAlign = LabelAlign.Right,
                Layout = "form",
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                Border = false
            };

            var txfName = new TextField 
            { 
                FieldLabel = "Наименование варианта", 
                Name = "tfName", 
                ID = "tfName", 
                Width = 300, 
                Value = name 
            };

            panel.Items.Add(txfName);
            
            ComboBox methodGroupCombo = new ComboBox
                                            {
                                                FieldLabel = "Группа методов",
                                                Name = "cbGroupMethod",
                                                ID = "cbGroupMethod",
                                                Editable = false,
                                                EmptyText = "Выберите группу...",
                                                Width = 300,
                                                ForceSelection = true,
                                                TypeAhead = true,
                                                SelectOnFocus = true,
                                                Mode = DataLoadMode.Local,
                                                TriggerAction = TriggerAction.All
                                            };

            /*methodGroupCombo.Items.AddRange(new ListItemCollection<ListItem>
                                                {
                                                    new ListItem { Text = "Предустановленный расчет", Value = MathMethods.ComplexEquation },
                                                    new ListItem { Text = "Регрессия 1-го рода", Value = MathMethods.FirstOrderRegression },
                                                    new ListItem { Text = "Регрессия 2-го рода", Value = MathMethods.SecondOrderRegression },
                                                    new ListItem { Text = "ARMA", Value = MathMethods.ARMAMethod },
                                                    new ListItem { Text = "Множественная регрессия", Value = MathMethods.MultiRegression },
                                                    new ListItem { Text = "PCA прогнозирование", Value = MathMethods.PCAForecast }
                                                });*/

            foreach (var mathGroup in extension.LoadedMathGroups)
            {
                methodGroupCombo.Items.Add(new ListItem { Text = mathGroup.TextName, Value = mathGroup.Code.ToString() });
            }

            methodGroupCombo.Listeners.Select.Handler = "cbMethod.clearValue(); methodStore.reload();";

            panel.Items.Add(methodGroupCombo);

            ComboBox methodCombo = new ComboBox
                                       {
                                           FieldLabel = "Метод",
                                           Name = "cbMethod",
                                           ID = "cbMethod",
                                           Editable = false,
                                           EmptyText = "Выберите вначале группу методов...",
                                           Width = 300,
                                           ValueNotFoundText = "Выберете метод...",
                                           StoreID = "methodStore",
                                           TypeAhead = true,
                                           Mode = DataLoadMode.Local,
                                           ForceSelection = true,
                                           TriggerAction = TriggerAction.All,
                                           DisplayField = "Text",
                                           ValueField = "Value"
                                       };

            methodCombo.DirectEvents.Select.Url = "/ProgData/ChangeFormula";
            methodCombo.DirectEvents.Select.CleanRequest = true;
            methodCombo.DirectEvents.Select.EventMask.ShowMask = true;
            ////methodCombo.DirectEvents.Select.ExtraParams.Add(new Parameter("predCount", "sfProgToYear.getValue() - sfProgFromYear.getValue()", ParameterMode.Raw));
            methodCombo.DirectEvents.Select.ExtraParams.Add(new Parameter("group", "cbGroupMethod.getSelectedItem().value", ParameterMode.Raw));
            methodCombo.DirectEvents.Select.ExtraParams.Add(new Parameter("method", "cbMethod.getSelectedItem().value", ParameterMode.Raw));
            methodCombo.DirectEvents.Select.ExtraParams.Add(new Parameter("key", String.Format("'{0}'", key), ParameterMode.Raw));
            ////methodCombo.DirectEvents.Select.Success = "";

            if (group > -1)
            {
                methodGroupCombo.Value = group;

                panel.AddScript("methodStore.reload();");
            }

            if (method > -1)
            {
                methodCombo.Value = method;

                ////    var rm = page.Controls.OfType<HtmlForm>().First().Controls.OfType<ResourceManager>().First();

                /*rm.RegisterOnReadyScript(@"Ext.net.DirectEvent.confirmRequest(
    {
        cleanRequest: true,
        isUpload: false,
        url: '/PlanningProg/ChangeFormula?group='+cbGroupMethod.getSelectedItem().value+'&method='+cbMethod.getSelectedItem().value,
        control: this        
    });");*/
            }

            panel.Items.Add(methodCombo);

            return panel;
        }
    }
}
