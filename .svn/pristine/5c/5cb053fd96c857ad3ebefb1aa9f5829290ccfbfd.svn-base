using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;

using Ext.Net;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Extensions;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.RIA.Extensions.E86N.Services.AnnualBalance;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Views
{
    public class AnnualBalanceBaseView : View
    {
        public const string Scope = "E86n.View.AnnualBalanceView";

        public readonly IAuthService Auth;

        private const string HeadAttributeFormID = "HeadAttribute";

        public AnnualBalanceBaseView()
        {
            ViewController = "AnnualBalanceView";
            IsVisibleField = false;
            Auth = Resolver.Get<IAuthService>();
            Details = new List<Component>();
        }

        public int? DocId
        {
            get { return Params["docId"] == "null" ? -1 : Convert.ToInt32(Params["docId"]); }
        }

        /// <summary>
        /// определяет, показывать или нет поля ОКПО учредителя
        /// </summary>
        public bool IsVisibleField { get; set; }

        public ViewPage Page { get; set; }

        public string ViewController { get; set; }

        public List<Component> Details { get; set; }

        public virtual void InitDoc(ViewPage page)
        {
            var store = GetStore(HeadAttributeFormID, new HeadAttributeFields());
            store.Listeners.DataChanged.Fn(Scope, "StoreDataChanged");
            Page.Controls.Add(store);
        }

        public override List<Component> Build(ViewPage page)
        {
            Page = page;

            ResourceManager.GetInstance(page).RegisterClientScriptBlock("AnnualBalanceView", Resource.AnnualBalanceView);
            ResourceManager.GetInstance(page).RegisterClientScriptBlock("CodeMaskBuilder", Resource.CodeMaskBuilder);
            ResourceManager.GetInstance(page).RegisterClientScriptBlock("StdHandBooks", Resource.StdHandBooks);

            var restActions = ResourceManager.GetInstance(page).RestAPI;
            restActions.Create = HttpMethod.POST;
            restActions.Read = HttpMethod.GET;
            restActions.Update = HttpMethod.POST;
            restActions.Destroy = HttpMethod.DELETE;

            InitDoc(page);
            
            var tb = new NewStateToolBarControl(Convert.ToInt32(DocId.ToString())).BuildComponent(page);

            tb.Add(new VersioningControl(Convert.ToInt32(DocId.ToString()), ViewController, Scope + ".SetReadOnlyDoc").Build(page));
            
            tb.Add(new ToolbarSeparator());
            
            if (Auth.IsAdmin())
            {
                var export = new UpLoadFileBtnControl
                                 {
                                     Id = "btnExport",
                                     Name = "Экспорт в XML",
                                     Icon = Icon.DiskDownload,
                                     Upload = false,
                                     UploadController = "/{0}/ExportToXml".FormatWith(ViewController),
                                     Params = { { "docId", DocId.ToString() } }
                                 };

                tb.Add(export.Build(Page));

                tb.Add(new ToolbarSeparator());

                tb.Add(new SetDocStateBtn(Convert.ToInt32(DocId.ToString())).Build(Page));
            }

            tb.Add(new ToolbarSeparator());

            tb.Add(new ConsolidationPumpBtn(DocId ?? -1).Build(page));
            
            Details.Add(new DocsDetailControl(DocId ?? -1).BuildComponent(page));

            var details = (TabPanel)UiBuilders.GetTabbedDetails(Details);
            details.ID = "DetailTabPanel";
            details.Listeners.TabChange.Fn(Scope, "reloadDetail");

            var view = new Viewport
                           {
                               Items =
                                   {
                                       new BorderLayout
                                           {
                                               North = { Items = { new ParamDocPanelControl(DocId ?? -1, tb).BuildComponent(page) } },
                                               Center =
                                                   {
                                                       Items =
                                                           {
                                                               new Panel
                                                                   {
                                                                       Border = false,
                                                                       Items =
                                                                           {
                                                                               new RowLayout
                                                                                   {
                                                                                       Rows =
                                                                                           {
                                                                                               new LayoutRow
                                                                                                   {
                                                                                                       Items =
                                                                                                           {
                                                                                                               GetAttrFormPanel()
                                                                                                           }
                                                                                                   },
                                                                                               new LayoutRow
                                                                                                   {
                                                                                                       RowHeight = 1,
                                                                                                       Items =
                                                                                                           {
                                                                                                               details
                                                                                                           }
                                                                                                   }
                                                                                           }
                                                                                   }
                                                                           }
                                                                   }
                                                           }
                                                   }
                                           }
                                   }
                           };

            return new List<Component> { view };
        }

        public Store GetStore(string pId, Enum fields, bool autoLoad = true, string action = "")
        {
            var act = action != string.Empty ? action : pId;

            var store = StoreExtensions.StoreCreateDefault(
                pId + "Store",
                autoLoad,
                ViewController,
                "{0}Read".FormatWith(act),
                "{0}Save".FormatWith(act),
                "{0}Save".FormatWith(act),
                "{0}Delete".FormatWith(act));

            store.SetBaseParams("docId", DocId.ToString(), ParameterMode.Raw);
            store.SetWriteBaseParams("docId", DocId.ToString(), ParameterMode.Raw);

            store.AddFieldsByEnum(fields);

            return store;
        }

        public void AddLineCodeColumn(GridPanel gp, string fieldID, string fieldName, bool hiden = false, bool editble = true)
        {
            if (editble)
            {
                gp.ColumnModel.AddColumn(fieldID, fieldName, DataAttributeTypes.dtString)
                    .SetWidth(50).SetHidden(hiden).Editor.Add(
                        new TextArea
                        {
                            AllowBlank = false,
                            MaxLength = 6,
                            MaskRe = @"[0-9\.]",
                            Regex = @"^\d{3}?(\.\d{1,2})?$",
                            RegexText =
                                @"Значение «{0}» должно состоять либо из 3 цифр,
                                      либо из 3х цифр и 1й или 2х после точки.<br>
                                      Пример: 123 или 123.1 или 123.12".FormatWith(fieldName)
                        });
            }
            else
            {
                gp.ColumnModel.AddColumn(fieldID, fieldName, DataAttributeTypes.dtString)
                    .SetWidth(50).SetHidden(hiden);
            }
        }

        /// <summary>
        /// Добавление кнопки расчета сумм
        /// </summary>
        /// <param name="gp">грид для которого кнопочка</param>
        /// <param name="section">детализация документа</param>
        /// <returns>кнопка расчета сумм</returns>
        public Button AddSummBtn(GridPanel gp, int section)
        {
            var btnSumm = new Button
                {
                    ID = string.Concat(gp.ID, "SummBtn"),
                    Icon = Icon.Sum,
                    ToolTip = @"Вычислить суммы",
                    DirectEvents =
                        {
                            Click =
                                {
                                    Url = UiBuilders.GetUrl<AnnualBalanceViewController>("CalculateSumm"),
                                    CleanRequest = true,
                                    ExtraParams =
                                        {
                                            new Parameter("docId", DocId.ToString(), ParameterMode.Raw),
                                            new Parameter("section", section.ToString(CultureInfo.InvariantCulture), ParameterMode.Raw)
                                        },
                                    Success = "{0}.reload();".FormatWith(gp.StoreID),
                                    Before = string.Concat("return ", Scope, ".BeforeSumm('{0}');".FormatWith(gp.ID)) 
                                }
                        }
                };

            gp.Toolbar().Add(new ToolbarSeparator());
            gp.Toolbar().Add(btnSumm);
            
            return btnSumm;
        }

        private FormPanel GetAttrFormPanel()
        {
            var form = new FormPanel
            {
                ID = HeadAttributeFormID,
                Border = false,
                Url = "/{0}/HeadAttributeSave".FormatWith(ViewController),
                BaseParams = { new Parameter("docId", DocId.ToString()) },
                AutoScroll = true,
                LabelWidth = 180,
                LabelPad = 10,
                Padding = 6,
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                MonitorValid = true,
                Height = !IsVisibleField ? 85 : 110
            };

            form.Listeners.ClientValidation.Handler = Scope + ".ClientValidation(valid);";

            form.TopBar.Add(
                new Toolbar
                {
                    Items =
                            {
                                new Button
                                    {
                                        ID = "btn{0}Save".FormatWith(HeadAttributeFormID),
                                        Icon = Icon.TableSave,
                                        ToolTip = @"Сохранить",
                                        Listeners = { Click = { Fn = Scope + ".Save" } }
                                    },
                                new Button
                                    {
                                        ID = "btn{0}Refresh".FormatWith(HeadAttributeFormID),
                                        Icon = Icon.PageRefresh,
                                        ToolTip = @"Обновить",
                                        Listeners = { Click = { Fn = Scope + ".Refresh" } },
                                    },
                            }
                });

            var field = HeadAttributeFields.HeadAttributeID.ToString();
            form.Items.Add(
                new Hidden
                {
                    ID = field,
                    FieldLabel = AnnualBalanceHelpers
                        .HeadAttributeFieldsNameMapping(HeadAttributeFields.HeadAttributeID),
                    DataIndex = field
                });

            field = HeadAttributeFields.Datedata.ToString();
            form.Items.Add(
                new DateField
                {
                    ID = field,
                    DataIndex = field,
                    AllowBlank = false,
                    FieldLabel = AnnualBalanceHelpers
                        .HeadAttributeFieldsNameMapping(HeadAttributeFields.Datedata),
                    Width = 300
                });

            var combobox = new DBComboBox
                {
                    Box = { Disabled = true, Width = 300, Value = 1 }, ////todo: пока этого достаточно, но при снятии Disabled надо убирать Value.
                    BoxStore = { AutoLoad = true },
                    ID = HeadAttributeFields.RefPeriodic.ToString(),
                    Key = FX_FX_Periodic.Key,
                    DataIndex = HeadAttributeFields.RefPeriodicName.ToString(),
                    FieldLabel = AnnualBalanceHelpers.HeadAttributeFieldsNameMapping(HeadAttributeFields.RefPeriodicName),
                };
            
            form.Items.Add(combobox.Build(Page));

            field = HeadAttributeFields.FounderAuthorityOkpo.ToString();
            form.Items.Add(
                new TextField
                {
                    ID = field,
                    Hidden = !IsVisibleField,
                    DataIndex = field,
                    AllowBlank = !IsVisibleField,
                    FieldLabel = AnnualBalanceHelpers
                        .HeadAttributeFieldsNameMapping(HeadAttributeFields.FounderAuthorityOkpo),
                    Width = 300,
                    MaxLength = 10,
                    MaskRe = @"[0-9]",
                    Regex = @"^\d{8}$|^\d{10}$",
                    RegexText = @"Неверный формат ОКПО"
                });

            return form;
        }
    }
}
