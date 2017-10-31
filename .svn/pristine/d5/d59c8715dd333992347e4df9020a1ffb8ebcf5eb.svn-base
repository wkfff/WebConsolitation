using System;
using System.Text;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtNet.Extensions.ExcelLikeSelectionModel;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.EO10MissivePRF.Presentation.Controls
{
    public class MeasuresReportsControl
    {
        public static string CreateGridScript(int measureId)
        {
            var sb = new StringBuilder();

            var gridId = "GridPanelRow_" + measureId;

            var store = CreateStore(measureId);
            store.GroupField = String.Empty;
            store.BaseParams.Add(new Parameter("measureId", measureId.ToString(), ParameterMode.Raw));

            store.Listeners.Load.Handler = String.Format(
                "var gp = Ext.getCmp('{0}'); if (gp) {{ gp.setHeight(gp.getView().mainHd.getHeight() + gp.getView().mainBody.getHeight()); }}",
                gridId);

            sb.AppendLine(store.ToScript(RenderMode.Auto, "row-" + measureId));

            var grid = CreateGridPanel(gridId, store.ID, measureId);
            grid.Height = 200;

            grid.ColumnModel.ID = "GridPanelRowCM_" + measureId;

            grid.View[0].ID = "GridPanelRowView_" + measureId;
            grid.View[0].ForceFit = true;
            grid.View[0].HeaderGroupRows.Clear();

            // important
            ExtNet.Get("row-" + measureId).SwallowEvent(new[] { "click", "mousedown", "mouseup", "dblclick" }, true);

            sb.AppendLine(grid.ToScript(RenderMode.RenderTo, "row-" + measureId));

            var instanceScript = (StringBuilder)System.Web.HttpContext.Current.Items["Ext.Net.ResourceMgr.InstanceScript"];
            sb.AppendLine(instanceScript.ToString());
            instanceScript.Remove(0, instanceScript.Length);

            return sb.ToString();
        }

        public static Store CreateStore(int measureId)
        {
            var store = new Store { ID = "store{0}".FormatWith(measureId) };

            var reader = new JsonReader { IDProperty = "ID", Root = "data", TotalProperty = "total" };
            reader.Fields.Add("ID");
            reader.Fields.Add("Date");
            reader.Fields.Add("Report");
            store.Reader.Add(reader);

            store.Proxy.Add(new HttpProxy
            {
                Url = "/Measures/ReadReports?measureId={0}".FormatWith(measureId),
                Method = HttpMethod.POST,
            });

            store.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = "/Measures/SaveReports",
                Method = HttpMethod.POST,
                Timeout = 500000
            });

            return store;
        }

        public static GridPanel CreateGridPanel(string gridId, string storeId, int measureId)
        {
            var gp = new GridPanel
            {
                ID = gridId,
                StoreID = storeId,
                MonitorResize = true,
                Border = false,
                AutoHeight = true,
                AutoExpandColumn = "Report",
                ColumnLines = true,
                LoadMask = { ShowMask = true },
                SaveMask = { ShowMask = true }
            };

            gp.SelectionModel.Add(new ExcelLikeSelectionModel());

            var view = new GroupingView
            {
                ForceFit = false,
                HideGroupedColumn = true,
                EnableGrouping = false,
                StartCollapsed = true,
                EnableNoGroups = false
            };
            gp.View.Add(view);

            gp.ColumnModel.AddColumn("ID", "Идентификатор", DataAttributeTypes.dtString).SetHidden(true);
            gp.ColumnModel.AddColumn("Date", "Дата", DataAttributeTypes.dtString);
            gp.ColumnModel.AddColumn("Report", "Текст", DataAttributeTypes.dtString).SetEditableString();

            // Устанавливаем для полей стиль переноса по словам
            gp.AddRefreshButton();

            var bookWindow = GetReportWindow(gridId, @"Добавить новый отчет");
            gp.AddNewRecordButton().Listeners.Click.Handler = @"
                {0}
                {1}.autoLoad.url = '/Measures/Book?measureId={2}';
                {1}.show();".FormatWith(bookWindow.ToScript(), bookWindow.ID, measureId);

            gp.AddSaveButton();

            return gp;
        }

        /// <summary>
        /// Создание модального окна для формы с новым отчетом
        /// </summary>
        /// <param name="gridId">Идентификатор грида, который нужно обновить после добавление отчета</param>
        /// <param name="title">Заголовок модального окна</param>
        /// <returns>Модальное окно</returns>
        public static Window GetReportWindow(string gridId, string title)
        {
            var win = new Window
            {
                ID = "{0}BookWindow".FormatWith(gridId),
                Width = 500,
                Height = 300,
                Title = title,
                Icon = Icon.ApplicationFormEdit,
                Hidden = false,
                Modal = true,
                Constrain = true
            };
            win.AutoLoad.Url = "/";
            win.AutoLoad.Mode = LoadMode.IFrame;
            win.AutoLoad.TriggerEvent = "show";
            win.AutoLoad.ReloadOnEvent = true;
            win.AutoLoad.ShowMask = true;
            win.AutoLoad.MaskMsg = @"Открытие формы для добавления отчета...";
            win.AutoLoad.Params.Add(new Parameter("id", string.Empty, ParameterMode.Value));

            var buttonSave = new Button
            {
                ID = "btnOk",
                Text = @"Сохранить",
                Icon = Icon.Accept,
                Listeners =
                {
                    Click =
                    {
                        Handler = @"if (btnOk.ownerCt.ownerCt.iframe.dom.contentWindow.ReportForm.isValid()) {{
                                                btnOk.ownerCt.ownerCt.iframe.dom.contentWindow.ReportForm.form.submit({{
                                                                waitMsg:'Сохранение...', 
                                                                success: function(form, action) {{ 
                                                                    {0}.hide(); 
                                                                    var reportGrid = Ext.getCmp('GridPanelRow_' + btnOk.ownerCt.ownerCt.iframe.dom.contentWindow.ReportForm.form.measureId);
                                                                    if (reportGrid != null && reportGrid != undefined) {{
                                                                        reportGrid.store.reload(); 
                                                                    }}
                                                                }}, 
                                                                failure: function(form, action) {{ 
                                                                    var fi = action.response.responseText.indexOf('message:') + 9;
                                                                    var li = action.response.responseText.lastIndexOf('{2}}}')
                                                                    var msg = action.response.responseText.substring(li, fi);
                                                                    Ext.net.Notification.show({{
                                                                        iconCls    : 'icon-information', 
                                                                        html       : msg, 
                                                                        title      : 'Внимание', 
                                                                        hideDelay  : 2500
                                                                    }}); 
                                                                }}
                                                            }});
                                         }}
                                         else {{
                                            Ext.net.Notification.show({{
                                                iconCls    : 'icon-information', 
                                                html       : 'Сохранение невозможно. Необходимо ввести дату.', 
                                                title      : 'Внимание', 
                                                hideDelay  : 2500
                                            }});
                                        }}".FormatWith(win.ID, gridId, '"')
                    }
                }
            };

            var buttonCancel = new Button
            {
                ID = "btnCancel",
                Text = @"Отмена",
                Icon = Icon.Cancel,
                Listeners =
                {
                    Click =
                    {
                        Handler = @"{0}.hide()".FormatWith(win.ID)
                    }
                }
            };

            win.Buttons.Add(buttonCancel);
            win.Buttons.Add(buttonSave);

            return win;
        }
    }
}
