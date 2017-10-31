using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.FO41.Presentation.Controls;
using Krista.FM.RIA.Extensions.FO41.Presentation.Models;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Views.HMAO
{
    public class HMAODetailView : View
    {
        private readonly IFO41Extension extension;
        private readonly HMAODetailViewModel detailsViewModel;
        
        public HMAODetailView(IFO41Extension extension, HMAODetailViewModel detailsViewModel)
        {
            this.extension = extension;
            this.detailsViewModel = detailsViewModel;
        }

        /// <summary>
        /// Идентификатор Store для документов
        /// </summary>
        public string DocsStoreId { get; set; }

        /// <summary>
        /// Признак, доступен ли интерфейс для редактирования
        /// </summary>
        public bool Editable { get; set; }

        /// <summary>
        /// Признак, редактируемы ли документы и комментарии
        /// </summary>
        public bool DocsAndCommentsEditabe { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            if (extension.UserGroup != FO41Extension.GroupOGV && 
                extension.UserGroup != FO41Extension.GroupTaxpayer)
            {
                ResourceManager.GetInstance(page).RegisterOnReadyScript(
                    ExtNet.Msg.Alert(
                    "Ошибка",
                    "Текущему пользователю не сопоставлен налогоплательщик или экономический орган.").
                        ToScript());

                return new List<Component>();
            }

            page.Controls.Add(GetCommentsStore());
            page.Controls.Add(GetRequestDetailStore(detailsViewModel));

            var content = new FormPanel
            {
                ID = "DetailsForm",
                Border = false,
                Url = "/FO41HMAORequests/Save?applicaionId={0}&taxPayerId={1}&taxTypeId={2}".FormatWith(
                        detailsViewModel.Id, detailsViewModel.OrgId, detailsViewModel.TypeTax.ID),
                CssClass = "x-window-mc",
                BodyCssClass = "x-window-mc",
                StyleSpec = "margin: 0px 0px 5px 10px;",
                Layout = "RowLayout",
                AutoScroll = true
            };

            content.Add(new TextField { ID = "requestDateValue", DataIndex = "RequestDate", Hidden = true });
            content.Add(new DisplayField
            {
                ID = "requestDateValueLabel",
                StyleSpec = "padding-left: 10px",
                LabelAlign = LabelAlign.Left,
                FieldLabel = @"Дата создания",
                Value = detailsViewModel.RequestDate,
                ReadOnly = true,
                Enabled = false
            });
            var numberField = new DisplayField
            {
                ID = "requestNumberValue",
                FieldLabel = @"Номер",
                LabelAlign = LabelAlign.Left,
                StyleSpec = "padding-left: 10px",
                ReadOnly = true,
                DataIndex = "Id"
            };
            if (detailsViewModel.Id < 1)
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

            var typeTaxText = detailsViewModel.TypeTax.ID == 4
                                  ? "налогу на прибыль организаций"
                                  : (detailsViewModel.TypeTax.ID == 9
                                         ? "налогу на имущество организаций"
                                         : "транспортному налогу");

            var quarter = detailsViewModel.PeriodId % 10;
            var year = detailsViewModel.PeriodId / 10000;
            content.Add(new DisplayField
            {
                Text = @"Информация об использовании налоговых льгот по {0} за {1} квартал {2} года"
                    .FormatWith(typeTaxText, quarter, year),
                LabelAlign = LabelAlign.Top,
                NoteAlign = NoteAlign.Top,
                CssClass = "h4",
                StyleSpec = "font-size: 14px; padding-bottom: 5px; margin: 5px 10px 0px 10px; font-weight: bold; padding-top: 5px; text-align: center;"
            });

            var fields = new FieldSet
            {
                ID = "appFields",
                Collapsible = false,
                Collapsed = false,
                Layout = "form",
                Border = false,
                LabelSeparator = String.Empty,
                LabelAlign = LabelAlign.Left,
                LabelWidth = 400,
                DefaultAnchor = "0",
                Height = 330
            };

            if (extension.UserGroup == FO41Extension.GroupTaxpayer || detailsViewModel.OrgId > 0)
            {
                fields.Add(new DisplayField
                               {
                                   ID = "requestTaxPayerTitle",
                                   FieldLabel = @"Полное наименование организации",
                                   Text = detailsViewModel.OrgName
                               });
            }
            else
            {
                fields.Add(GetOrgCombo(page));
            }

            fields.Add(GetCategoryCombo(page));

            fields.Add(new DisplayField
                            {
                                ID = "requestINNTextValue",
                                FieldLabel = @"ИНН налогоплательщика",
                                Text = detailsViewModel.INN == null 
                                    ? string.Empty 
                                    : detailsViewModel.INN.ToString(),
                                Disabled = !Editable
                            });
            fields.Add(new DisplayField
            {
                ID = "requestKPPTextValue",
                FieldLabel = @"КПП налогоплательщика",
                Text = detailsViewModel.KPP,
                Disabled = !Editable
            });
            var legalAddress = new CompositeField
                                   {
                                       FieldLabel = @"Юридический адрес:",
                                       Items =
                                           {
                                               GetRegionsCombo(page),
                                               new TextField
                                                   {
                                                       ID = "requestLegalAddressTextValue",
                                                       Text = detailsViewModel.LegalAddress,
                                                       Disabled = !Editable,
                                                       Note = @"улица, дом, корпус",
                                                       Width = 300
                                                   }
                                           }
                                   };

            fields.Add(legalAddress);
            fields.Add(new TextField
            {
                ID = "requestAddressTextValue",
                FieldLabel = @"Местонахождение организации",
                Text = detailsViewModel.Address,
                Disabled = !Editable
            });

            fields.Add(new TextField
            {
                ID = "requestUnitTextValue",
                FieldLabel = @"Количество обособленных подразделений на территории автономного округа",
                Text = detailsViewModel.Unit.ToString(),
                Disabled = !Editable
            });

            fields.Add(new TextField
            {
                ID = "requestOKATOTextValue",
                FieldLabel = @"Код ОКАТО муниципального образования в соответствии с Общероссийским классификатором объектов административно-территориального деления, на территории которого мобилизуются денежные стредства от уплаты налога (сбора) в бюджетную систему Российской Федерации",
                Text = detailsViewModel.OKATO.ToString(),
                Disabled = !Editable
            });

            fields.Add(
                new TextArea 
            { 
                ID = "requestPersonsValue", 
                Width = 700,
                FieldLabel = @"Ответственное лицо - исполнитель (ФИО, контактный телефон)",
                Height = 40, 
                AllowBlank = false, 
                DataIndex = "Executor",
                Disabled = !Editable
            });

            content.Add(fields);
            var commentsGrid = new CommentsGridControl(
                page, 
                0, 
                (Editable || DocsAndCommentsEditabe) && extension.IsReqLastPeriod(detailsViewModel.PeriodId),
                extension.User.Name, 
                detailsViewModel.StateName);
            if (detailsViewModel.Id < 1)
            {
                commentsGrid.ToolTips.Add(new ToolTip
                {
                    TargetControl = commentsGrid,
                    TrackMouse = true,
                    Listeners =
                    {
                        Show =
                        {
                            Handler = @"
                                this.body.dom.innerHTML = 'Добавить комментарии можно после сохранения заявки';"
                        }
                    }
                });
            }

            commentsGrid.InitAll();
            content.Add(commentsGrid);

            var fileListPanel = CreateFileListPanel(page);
            fileListPanel.LabelAlign = LabelAlign.Top;
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

            return new List<Component>
                       {
                           tabDetails, 
                           fileListPanel.GetFileUploadWindow(), 
                           commentsGrid.GetAddCommentWindow()
                       };
        }

        private static Store GetRequestDetailStore(HMAODetailViewModel detailsModel)
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
                    new RecordField("CopiesText")
                }
            });

            store.DataSource = new List<DetailsViewModel> { detailsModel };
            store.DataBind();
            store.AddScript("DetailsForm.getForm().loadRecord(dsDetail.getAt(0));");

            return store;
        }

        private static ComboBox GetCombo(string id, string storeId, string label)
        {
            var categoryCombo = new ComboBox
            {
                ItemSelector = "tr.search-item",
                LoadingText = @"Поиск...",
                Editable = false,
                EmptyText = @"Выберите значение",
                HideTrigger = true,
                ID = id,
                AllowBlank = false,
                TriggerAction = TriggerAction.All,
                StoreID = storeId,
                ValueField = "ID",
                DisplayField = "ShortName",
                SelectOnFocus = true,
                TypeAhead = false,
                FieldLabel = label
            };

            var tableTemplate = new StringBuilder(@"
<table width=100%>
    <tpl for=""."">
        <tr class=""search-item"">
            ");
            tableTemplate.Append(@"<td class=""search-item primary""><b>{{{0}}}</b> ({{{1}}}) </td>"
                .FormatWith("ShortName", "Name"));
            tableTemplate.Append(@"
        </tr>
    </tpl>
</table>");
            categoryCombo.Template.Html = tableTemplate.ToString();

            return categoryCombo;
        }

        private static Store CreateRegionsStore()
        {
            var ds = new Store { ID = "dsRegions", AutoLoad = true };

            ds.SetHttpProxy("/FO41HMAODetail/LookupRegionsHMAO")
                .SetJsonReader()
                .AddField("ID")
                .AddField("ShortName")
                .AddField("Name");

            return ds;
        }

        private ComboBox GetCategoryCombo(ViewPage page)
        {
            page.Controls.Add(CreateCategoryStore());
            var categoryCombo = GetCombo(
                "requestCategoryValue", 
                "dsCategoryTaxPayer", 
                @"Основной вид деятельности (льготная категория)");

            categoryCombo.Disabled = extension.UserGroup != FO41Extension.GroupTaxpayer || !Editable;
            if (detailsViewModel.CategoryId > 0)
            {
                categoryCombo.SelectedItem.Value = detailsViewModel.CategoryId.ToString();
                categoryCombo.SelectedItem.Text = detailsViewModel.CategoryName;
            }

            return categoryCombo;
        }

        private ComboBox GetRegionsCombo(ViewPage page)
        {
            page.Controls.Add(CreateRegionsStore());
            var combo = GetCombo("requestRegionValue", "dsRegions", @"  ");
            var tableTemplate = new StringBuilder(@"
<table width=100%>
    <tpl for=""."">
        <tr class=""search-item"">
            ");
            tableTemplate.Append(@"<td class=""search-item primary"">{{{0}}} </td>".FormatWith("Name"));
            tableTemplate.Append(@"
        </tr>
    </tpl>
</table>");
            combo.Template.Html = tableTemplate.ToString();
            combo.DisplayField = "Name";
            combo.Width = 200;
            combo.Disabled = !Editable;
            if (extension.UserGroup == FO41Extension.GroupOGV)
            {
                combo.AllowBlank = false;
            }

            combo.Note = @"Муниципальное образование";
            if (detailsViewModel.BridgeRegionId > 0)
            {
                combo.SelectedItem.Value = detailsViewModel.BridgeRegionId.ToString();
                combo.SelectedItem.Text = detailsViewModel.BridgeRegionShortName;
            }

            return combo;
        }

        private ComboBox GetOrgCombo(ViewPage page)
        {
            var store = CreateOrgsStore();
            page.Controls.Add(store);
            var combo = new ComboBox
            {
                ItemSelector = "tr.search-item",
                LoadingText = @"Поиск...",
                Editable = false,
                EmptyText = @"Выберите значение",
                HideTrigger = true,
                ID = "requestOrgValue",
                AllowBlank = false,
                TriggerAction = TriggerAction.All,
                StoreID = store.ID,
                ValueField = "ID",
                DisplayField = "Name",
                SelectOnFocus = true,
                TypeAhead = false,
                FieldLabel = @"Полное наименование орагнизации"
            };

            var tableTemplate = new StringBuilder(@"
<table width=100%>
    <tpl for=""."">
        <tr class=""search-item"">
            ");
            tableTemplate.Append(@"<td class=""search-item primary"">{{{0}}} </td>".FormatWith("Name"));
            tableTemplate.Append(@"
        </tr>
    </tpl>
</table>");
            combo.Template.Html = tableTemplate.ToString();

            combo.Listeners.Select.AddAfter(@"
                // подтягиваем реквизиты организации
                var data = requestOrgValue.store.data.items[index].data;
                requestINNTextValue.setValue(data.INN);
                requestKPPTextValue.setValue(data.KPP);
                requestLegalAddressTextValue.setValue(data.LegalAddress);
                requestRegionValue.setValueAndFireSelect(data.BridgeRegionID);
                requestAddressTextValue.setValue(data.Address);
                requestUnitTextValue.setValue(data.Unit);
                requestOKATOTextValue.setValue(data.OKATO);

                // подтягиваем справочные показатели
                
            ");

            if (detailsViewModel.OrgId > 0)
            {
                combo.SelectedItem.Value = detailsViewModel.OrgId.ToString();
                combo.SelectedItem.Text = detailsViewModel.OrgName;
            }

            return combo;
        }

       private Store CreateCategoryStore()
        {
            var ds = new Store { ID = "dsCategoryTaxPayer", AutoLoad = true };
            ds.BaseParams.Add(new Parameter(
                "taxTypeId", 
                detailsViewModel.TypeTax.ID.ToString(), 
                ParameterMode.Raw));

            ds.BaseParams.Add(new Parameter(
                "orgId", 
                detailsViewModel.OrgId.ToString(), 
                ParameterMode.Raw));
            ds.BaseParams.Add(new Parameter(
                "periodId", 
                detailsViewModel.PeriodId.ToString(), 
                ParameterMode.Raw));
            ds.BaseParams.Add(new Parameter(
                "curCategoryId", 
                detailsViewModel.CategoryId.ToString(), 
                ParameterMode.Raw));

            ds.SetHttpProxy("/FO41HMAODetail/LookupCategoryTaxPayerHMAO")
                .SetJsonReader()
                .AddField("ID")
                .AddField("ShortName")
                .AddField("Name");

            return ds;
        }

        private Store CreateOrgsStore()
        {
            var ds = new Store { ID = "dsOrgs", AutoLoad = true };

            ds.SetHttpProxy("/FO41HMAODetail/LookupOrgsHMAO{0}"
                .FormatWith(extension.UserGroup == FO41Extension.GroupTaxpayer 
                    ? string.Empty
                    : "?categoryId={0}&periodId={1}&curOrgId={2}".FormatWith(
                        detailsViewModel.CategoryId, 
                        detailsViewModel.PeriodId, 
                        detailsViewModel.OrgId)))
                .SetJsonReader()
                .AddField("ID")
                .AddField("Name")
                .AddField("INN")
                .AddField("KPP")
                .AddField("Address")
                .AddField("LegalAddress")
                .AddField("Unit")
                .AddField("OKATO")
                .AddField("BridgeRegionID")
                .AddField("BridgeRegion");

            return ds;
        }

        private FileListPanel CreateFileListPanel(ViewPage page)
        {
            var fileListPanel = DetailFileListControl.Get(
                DocsAndCommentsEditabe && extension.IsReqLastPeriod(detailsViewModel.PeriodId), 
                detailsViewModel.Id);

            if (detailsViewModel.Id < 1)
            {
                fileListPanel.ToolTips.Add(new ToolTip
                {
                    TargetControl = fileListPanel,
                    TrackMouse = true,
                    Listeners =
                    {
                        Show =
                        {
                            Handler = @"
                                    this.body.dom.innerHTML = 'Добавить документы можно после сохранения заявки';"
                        }
                    }
                });
            }

            fileListPanel.AutoScroll = true;
            fileListPanel.Height = 170;
            DocsStoreId = fileListPanel.StoreID;

            page.Controls.Add(fileListPanel.GetFileListStore());
            return fileListPanel;
        }

        private Store GetCommentsStore()
        {
            var store = new Store
                            {
                                ID = "dsComments0", 
                                AutoLoad = true, 
                                Restful = true, 
                                SkipIdForNewRecords = false
                            };

            store.SetRestController("FO41Comments")
                .SetJsonReader()
                .AddField("ID")
                .AddField("Text")
                .AddField("StateName")
                .AddField("NoteDate")
                .AddField("Executor");

            store.Proxy.Proxy.RestAPI.ReadUrl = "/FO41Comments/OrgRead?applicationId={0}"
                .FormatWith(detailsViewModel.Id);
            const string UrlCreate = "/FO41Comments/OrgSave";
            store.Proxy.Proxy.RestAPI.CreateUrl = UrlCreate;
            store.Proxy.Proxy.RestAPI.UpdateUrl = UrlCreate;
            store.Proxy.Proxy.RestAPI.DestroyUrl = "/FO41Comments/OrgDestroy";
            store.AddScript("var tempId = -2;");
            return store;
        }
    }
}
