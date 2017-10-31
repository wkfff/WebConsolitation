using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.FO41.Presentation.Controls;
using Krista.FM.RIA.Extensions.FO41.Presentation.Models;
using Krista.FM.RIA.Extensions.FO41.Services;
using Control = System.Web.UI.Control;
using Panel = Ext.Net.Panel;
using Parameter = Ext.Net.Parameter;
using View = Krista.FM.RIA.Core.Gui.View;

[assembly: WebResource("Krista.FM.RIA.Core.Extensions.css.CustomSearch.css", "text/javascript")]

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Views
{
    public class DetailView : View
    {
        /// <summary>
        /// Репозиторий заявот от налогоплательщиков
        /// </summary>
        private readonly IAppPrivilegeService appPrivilegeService;

        /// <summary>
        /// Глобальные параметры
        /// </summary>
        private readonly IFO41Extension extension;

        /// <summary>
        /// Стиль для контролов
        /// </summary>
        private const string StyleAll = "margin: 0px 0px 5px 0px; font-size: 12px;";

        /// <summary>
        /// Признак, можжет ли ОГВ добавлять комментарии и документы
        /// </summary>
        private bool ogvCanEdit;

        public DetailView(IFO41Extension extension, IAppPrivilegeService appPrivilegeService)
        {
            this.extension = extension;
            this.appPrivilegeService = appPrivilegeService;
        }
        
        /// <summary>
        /// Идентификатор заявки от налогоплательщика
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        /// Идентификатор категории заявки
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Идентификатор Store для документов
        /// </summary>
        public string DocsStoreId { get; set; }

        /// <summary>
        /// Идентфикатор заявки, с которой делается копия или -1
        /// </summary>
        public int CopyApplicId { get; set; }

        /// <summary>
        ///  Модель данных реквизитов заявки
        /// </summary>
        public DetailsViewModel DetailsViewModel { get; set; }

        /// <summary>
        /// Контрол (обновить при выборе налогоплательщика)
        /// </summary>
        public string OrgControl { get; set; }

        /// <summary>
        /// Признак, можно ли редактировать
        /// </summary>
        public bool Editable { get; set; }

        public new List<Component> Build(ViewPage page)
        {
            InitView(page);
            AddStores(page);
            var orgCombo = GetORGCombo();
            var categoryCombo = GetCategoryCombo();
            var content = new FormPanel
                              {
                                  ID = "DetailsForm",
                                  Border = false,
                                  Url = "/FO41Requests/Save?applicaionId={0}&copyApplicId={1}".FormatWith(
                                          CopyApplicId > -1 ? -1 : ApplicationId, CopyApplicId),
                                  CssClass = "x-window-mc",
                                  BodyCssClass = "x-window-mc",
                                  StyleSpec = "margin: 0px 0px 5px 10px;",
                                  Layout = "RowLayout"
                              };
            content.BaseParams.Add(new Parameter("state", "dsDetail.getAt(0).data.StateId"));

            content.Add(new TextField { ID = "requestDateValue", DataIndex = "RequestDate", Hidden = true });
            content.Add(new DisplayField
                            {
                                ID = "requestDateValueLabel",
                                StyleSpec = StyleAll,
                                LabelAlign = LabelAlign.Left,
                                FieldLabel = @"Дата создания",
                                Value = DetailsViewModel.RequestDate,
                                ReadOnly = true,
                                Enabled = false
                            });
            var numberField = new DisplayField
                                  {
                                      ID = "requestNumberValue",
                                      FieldLabel = @"Номер",
                                      LabelAlign = LabelAlign.Left,
                                      StyleSpec = StyleAll,
                                      ReadOnly = true,
                                      DataIndex = "Id"
                                  };
            if (ApplicationId < 1)
            {
                numberField.ToolTips.Add(new ToolTip
                                             {
                                                 TargetControl = numberField,
                                                 TrackMouse = true,
                                                 Listeners =
                                                     {
                                                         Show =
                                                             {
                                                                 Handler = @"this.body.dom.innerHTML = 'Номер присваивается после сохранения заявки';"
                                                             }
                                                     }
                                             });
            }

            content.Add(numberField);
            content.Add(new DisplayField
                            {
                                Text = @"Сведения для расчета бюджетной и социальной эффективности предоставляемых (планируемых к предоставлению) налоговых льгот", 
                                LabelAlign = LabelAlign.Top,
                                NoteAlign = NoteAlign.Top,
                                CssClass = "h4",
                                StyleSpec = "font-size: 14px; padding-bottom: 5px; margin: 5px 10px 0px 10px; font-weight: bold; padding-top: 5px; text-align: center;"
                            });
            content.Add(new DisplayField { ID = "requestTaxPayerTitle", Text = @"Наименование налогоплательщика:", StyleSpec = StyleAll + " font-weight: bold;" });
            
            if (extension.UserGroup == FO41Extension.GroupTaxpayer)
            {
                content.Add(new DisplayField
                                {
                                    ID = "requestTaxPayerTextValue",
                                    StyleSpec = StyleAll,
                                    Text = DetailsViewModel.OrgName
                                });
                orgCombo.Hidden = true;
            }

            content.Add(orgCombo);
            content.Add(new DisplayField { ID = "requestCopiesMsg", StyleSpec = "color: red;", Text = DetailsViewModel.CopiesText });
            content.Add(new DisplayField { ID = "requestRegionTitle", StyleSpec = StyleAll + " font-weight: bold;", Text = @"Район:" });
            content.Add(new DisplayField { ID = "requestRegionTextValue", StyleSpec = StyleAll, DataIndex = "RegionName" });
            content.Add(new DisplayField { ID = "requestCategoryTitle", StyleSpec = StyleAll + " font-weight: bold;", Text = @"Категория по налоговым льготам:" });

            content.Add(new DisplayField { ID = "requestCategoryTextValue", StyleSpec = StyleAll, Text = DetailsViewModel.CategoryName, Hidden = CopyApplicId > 0 || (extension.ResponsOrg != null && ApplicationId < 1) });

            categoryCombo.Hidden = CopyApplicId < 1 && !(extension.ResponsOrg != null && ApplicationId < 1);
            content.Add(categoryCombo);
            content.Add(new DisplayField { ID = "requestPersonsTitle", StyleSpec = StyleAll + " font-weight: bold;", Text = @"Ответственные лица (ФИО, телефон):" });
            content.Add(new TextArea { ID = "requestPersonsValue", Width = 700, Height = 40, AllowBlank = false, StyleSpec = StyleAll, ReadOnly = !Editable, DataIndex = "Executor" });
            content.Add(new TextField { ID = "requestState", Hidden = true });

            var commentsGrid = new CommentsGridControl(page, DetailsViewModel.CategoryId, Editable || ogvCanEdit, extension.User.Name, DetailsViewModel.StateName);
            commentsGrid.InitAll();
            content.Add(commentsGrid);

            var fileListPanel = CreateFileListPanel(page);
            content.Add(fileListPanel);

            var tabDetails = new Panel
            {
                ID = "DetailsTab",
                Title = @"Реквизиты",
                Border = false,
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                AutoScroll = true,
                Closable = false
            };
            tabDetails.Add(content);

            return new List<Component> { tabDetails, fileListPanel.GetFileUploadWindow(), commentsGrid.GetAddCommentWindow() };
        }

        private static Control GetRequestDetailStore(DetailsViewModel detailsViewModel)
        {
            var store = new Store { ID = "dsDetail", AutoLoad = true };
            store.Reader.Add(new JsonReader
            {
                Fields =
                {
                    new RecordField("Id"),
                    new RecordField("StateId"),
                    new RecordField("StateName"),
                    new RecordField("OrgName"),
                    new RecordField("OrgId"),
                    new RecordField("CategoryName"),
                    new RecordField("CategoryId"),
                    new RecordField("RegionName"),
                    new RecordField("RegionId"),
                    new RecordField("Executor"),
                    new RecordField("RequestDate"),
                    new RecordField("CopiesText"),
                    new RecordField("KPP"),
                    new RecordField("INN"),
                    new RecordField("BridgeRegionId"),
                    new RecordField("BridgeRegionName"),
                    new RecordField("BridgeRegionShortName"),
                    new RecordField("LegalAddress"),
                    new RecordField("Address"),
                    new RecordField("Unit"),
                    new RecordField("OKATO"),
                    new RecordField("TypeTax")
                }
            });

            store.DataSource = new List<DetailsViewModel> { detailsViewModel };
            store.DataBind();
            store.AddScript("DetailsForm.getForm().loadRecord(dsDetail.getAt(0));");

            return store;
        }

        private ComboBox GetCategoryCombo()
        {
            var categoryCombo = new ComboBox
            {
                ItemSelector = "tr.search-item",
                LoadingText = @"Поиск...",

                // PageSize = 10,
                Enabled = false,
                Hidden = true,
                EmptyText = @"Выберите значение",
                HideTrigger = true,
                ID = "requestCategoryValue",
                Width = 700,
                TriggerAction = TriggerAction.All,
                StoreID = "dsCategoryTaxPayer",
                ValueField = "ID",
                AutoWidth = true,
                DisplayField = "ShortName",
                Mode = DataLoadMode.Local,
                StyleSpec = StyleAll,
                SelectOnFocus = true,
                TypeAhead = true,
                ReadOnly = !Editable && CopyApplicId < 1,
                ForceSelection = true,
                Editable = false,
                AllowBlank = false
            };

            var tableTemplate = new StringBuilder(@"
<table width=100%>
    <tpl for=""."">
        <tr class=""search-item"">
            ");
            tableTemplate.Append(@"<td class=""search-item primary""><b>{{{0}}}</b> ({{{1}}}) </td>".FormatWith("ShortName", "Name"));
            tableTemplate.Append(@"
        </tr>
    </tpl>
</table>");

            categoryCombo.Template.Html = tableTemplate.ToString();

            if (DetailsViewModel.CategoryId > 0 && CopyApplicId < 1)
            {
                categoryCombo.SelectedItem.Value = DetailsViewModel.CategoryId.ToString();
                categoryCombo.SelectedItem.Text = DetailsViewModel.CategoryName;
            }

            return categoryCombo;
        }

        private ComboBox GetORGCombo()
        {
            var orgCombo = new ComboBox
            {
                ID = "requestOrgPrivilege",
                AllowBlank = false,
                ReadOnly = CopyApplicId > 1 || ApplicationId > -1 || !Editable || extension.ResponsOrg != null,
                Width = 700,
                TriggerAction = TriggerAction.All,
                StoreID = "dsOrgPrivilege",
                ValueField = "ID",
                DisplayField = "Name",
                StyleSpec = StyleAll,
                EmptyClass = "Нет организаций, по которым заявка не создана"
            };

            if (DetailsViewModel.OrgId > 0)
            {
                orgCombo.SelectedItem.Value = DetailsViewModel.OrgId.ToString();
                orgCombo.SelectedItem.Text = DetailsViewModel.OrgName;
            }

            orgCombo.DirectEvents.Select.Url = "/FO41Detail/GetRegionByOrg";
            orgCombo.DirectEvents.Select.CleanRequest = true;
            orgCombo.DirectEvents.Select.IsUpload = false;
            orgCombo.DirectEvents.Select.ExtraParams.Add(new Parameter(
                                                             "orgId",
                                                             "requestOrgPrivilege.value",
                                                             ParameterMode.Raw));
            orgCombo.DirectEvents.Select.ExtraParams.Add(new Parameter("textField", "requestRegionTextValue"));

            orgCombo.DirectEvents.BeforeSelect.Url = "/FO41Detail/GetCopiesMsg";
            orgCombo.DirectEvents.BeforeSelect.ExtraParams.Add(new Parameter("indicatorField", "requestCopiesMsg"));
            orgCombo.DirectEvents.BeforeSelect.ExtraParams.Add(new Parameter("periodId", DetailsViewModel.PeriodId.ToString()));
            orgCombo.DirectEvents.BeforeSelect.ExtraParams.Add(new Parameter(
                                                                   "orgId",
                                                                   "requestOrgPrivilege.value",
                                                                   ParameterMode.Raw));

            orgCombo.Listeners.AddListerer("Select", "{0}.store.baseParams.orgId = requestOrgPrivilege.value; {0}.store.reload();".FormatWith(OrgControl));
            return orgCombo;
        }

        private void AddStores(ViewPage page)
        {
            page.Controls.Add(GetRequestDetailStore(DetailsViewModel));
            page.Controls.Add(CreateCategoryStore());
            page.Controls.Add(CreateOrgPrivilegeStore());

            // Store для комментариев
            page.Controls.Add(GetCommentsStore());
        }

        private void InitView(ViewPage page)
        {
            ResourceManager.GetInstance(page).RegisterScript(
                "Combobox.HotKeys", "/Krista.FM.RIA.Core/ExtNet.Extensions/ExcelLikeSelectionModel/js/HotKeysForCombobox.js/extention.axd");
            ResourceManager.GetInstance(page).RegisterStyle(
                "CustomSearch.Style", "/Krista.FM.RIA.Core/Extensions/css/CustomSearch.css/extention.axd");

            if (CopyApplicId > -1)
            {
                DetailsViewModel.Id = -1;
                ApplicationId = -1;
            }
            else
            {
                DetailsViewModel = appPrivilegeService.GetDetailsViewModel(ApplicationId);
            }

            if (DetailsViewModel.RegionName.Equals("Несопоставленные данные"))
            {
                DetailsViewModel.RegionName = string.Empty;
            }
            
            if (ApplicationId == -1)
            {
                var categoryRepository = new CategoryTaxpayerService();
                DetailsViewModel.CategoryId = CategoryId;
                var category = categoryRepository.Get(CategoryId);
                if (category != null)
                {
                    DetailsViewModel.CategoryName = category.Name;
                }
            }

            var reqThisYear = extension.IsReqLastPeriod(DetailsViewModel.PeriodId);
            ogvCanEdit = extension.UserGroup == FO41Extension.GroupTaxpayer || !reqThisYear
                ? false
                : DetailsViewModel.StateId == 2 && extension.ResponsOIV.Role.Equals("ОГВ");
        }

        private FileListPanel CreateFileListPanel(ViewPage page)
        {
            var fileListPanel = DetailFileListControl.Get(ApplicationId > -1 && (Editable || ogvCanEdit), ApplicationId);
            if (ApplicationId < 0)
            {
                fileListPanel.ToolTips.Add(new ToolTip
                {
                    TargetControl = fileListPanel,
                    TrackMouse = true,
                    Listeners =
                    {
                        Show =
                        {
                            Handler = @"this.body.dom.innerHTML = 'Добавить документы можно после сохранения заявки';"
                        }
                    }
                });
            }

            fileListPanel.AutoScroll = true;
            fileListPanel.Enabled = ApplicationId > -1 && Editable;
            fileListPanel.Height = 170;
            DocsStoreId = fileListPanel.StoreID;

            page.Controls.Add(fileListPanel.GetFileListStore());
            return fileListPanel;
        }

        private Control CreateOrgPrivilegeStore()
        {
            var ds = new Store { ID = "dsOrgPrivilege", AutoLoad = true };
            ds.BaseParams.Add(new Parameter("orgValue", "-1", ParameterMode.Raw));
            ds.BaseParams.Add(new Parameter("category", DetailsViewModel.CategoryId.ToString(), ParameterMode.Raw));
            ds.BaseParams.Add(new Parameter("period", DetailsViewModel.PeriodId.ToString(), ParameterMode.Raw));
            ds.BaseParams.Add(new Parameter("org", DetailsViewModel.OrgId > 0 ? DetailsViewModel.OrgId.ToString() : "-1", ParameterMode.Raw));

            ds.SetHttpProxy("/FO41Detail/LookupOrgPrivilege")
                .SetJsonReader()
                .AddField("ID")
                .AddField("Name")
                .AddField("RefRegionsId");
            return ds;
        }

        private Control CreateCategoryStore()
        {
            var ds = new Store { ID = "dsCategoryTaxPayer", AutoLoad = true };
            if (extension.UserGroup != FO41Extension.GroupTaxpayer)
            {
                var ogv = (extension.ResponsOIV != null && extension.ResponsOIV.Role == "ДФ") ||
                          (!Editable || extension.ResponsOIV == null)
                            ? -1
                            : extension.ResponsOIV.ID;
                ds.BaseParams.Add(new Parameter("ogv", "{0}".FormatWith(ogv), ParameterMode.Raw));
            }

            if (CopyApplicId > 0 || (extension.ResponsOrg != null && ApplicationId < 1))
            {
                int orgId;
                int periodID;
                if (CopyApplicId > 0)
                {
                    var orignRequest = appPrivilegeService.Get(CopyApplicId);
                    orgId = orignRequest.RefOrgPrivilege.ID;
                    periodID = extension.GetPrevPeriod();
                }
                else
                {
                    orgId = extension.ResponsOrg.ID;
                    periodID = extension.GetPrevPeriod();
                }

                try
                {
                    var categories = appPrivilegeService.FindAll().Where(f =>
                                        f.RefOrgPrivilege.ID == orgId &&
                                        f.RefYearDayUNV.ID == periodID).ToList()
                        .Select(rec => rec.RefOrgCategory.ID).ToList();

                    ds.BaseParams.Add(new Parameter("exceptIDs", JSON.Serialize(categories)));
                }
                catch (Exception)
                {
                    ds.BaseParams.Add(new Parameter("exceptIDs", "null", ParameterMode.Raw));
                }
            }

            ds.SetHttpProxy(extension.UserGroup == FO41Extension.GroupTaxpayer
                ? "/FO41Detail/LookupCategoryTaxPayerByOrg"
                : "/FO41Detail/LookupCategoryTaxPayer")
                .SetJsonReader()
                .AddField("ID")
                .AddField("ShortName")
                .AddField("Name");

            return ds;
        }

        private Store GetCommentsStore()
        {
            var store = new Store { ID = "dsComments{0}".FormatWith(DetailsViewModel.CategoryId), AutoLoad = true, Restful = true, SkipIdForNewRecords = false };

            store.SetRestController("FO41Comments")
                .SetJsonReader()
                .AddField("ID")
                .AddField("Text")
                .AddField("StateName")
                .AddField("NoteDate")
                .AddField("Executor");

            store.Proxy.Proxy.RestAPI.ReadUrl = "/FO41Comments/OrgRead?applicationId={0}".FormatWith(DetailsViewModel.Id);
            const string UrlCreate = "/FO41Comments/OrgSave";
            store.Proxy.Proxy.RestAPI.CreateUrl = UrlCreate;
            store.Proxy.Proxy.RestAPI.UpdateUrl = UrlCreate;
            store.Proxy.Proxy.RestAPI.DestroyUrl = "/FO41Comments/OrgDestroy"; 
            store.AddScript("var tempId = -2;");
            return store;
        }
    }
}
