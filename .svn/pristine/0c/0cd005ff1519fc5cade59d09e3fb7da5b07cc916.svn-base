using System;
using System.Collections.Generic;
using System.Web.Mvc;

using Ext.Net;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Extensions;
using Krista.FM.RIA.Extensions.E86N.Models.PfhdModel;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.PFHD;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.RIA.Extensions.E86N.Utils;
using Krista.FM.ServerLibrary;

using GlobalConsts = Krista.FM.RIA.Extensions.E86N.Utils.GlobalConsts;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Views
{
    internal class PfhdView : View
    {
        #region Setting

        private const string PfhdStoreID = "dsPfhd";

        private const string PfhdFormCol1ID = "frmPfhd1";

        private const string PfhdFormCol2ID = "frmPfhd2";

        private const string PfhdFormCol3ID = "frmPfhd3";

        private const string PfhdFormsButtonRefreshID = "btnPfhdRefresh";

        private const string PfhdFormsButtonSaveID = "btnPfhdSave";

        private const string CapFundsStoreID = "dsCapFunds";

        private const string CapFundsPanelID = "gpCapFunds";

        private const string RealAssetFundsStoreID = "dsRealAssetFunds";

        private const string RealAssetFundsPanelID = "gpRealAssetFunds";

        private const string OtherGrantFundsStoreID = "dsOtherGrantFunds";

        private const string OtherGrantFundsPanelID = "gpOtherGrantFunds";

        private const string Scope = "E86n.View.PfhdView";

        private readonly PfhdViewModel pfhdModel = new PfhdViewModel();

        private readonly CapFundsViewModel capFundsModel = new CapFundsViewModel();

        private readonly RealAssetFundsViewModel realAssetModel = new RealAssetFundsViewModel();

        private readonly OtherGrantFundsViewModel otherGrantModel = new OtherGrantFundsViewModel();

        public ViewPage Page { get; set; }

        public IAuthService Auth { get; set; }
        #endregion

        public int? DocId { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            DocId = Params["docId"] == "null" ? null : (int?)Convert.ToInt32(Params["docId"]);
            
            Page = page;

            Auth = Resolver.Get<IAuthService>();

            RestActions restActions = ResourceManager.GetInstance(Page).RestAPI;
            restActions.Create = HttpMethod.POST;
            restActions.Read = HttpMethod.GET;
            restActions.Update = HttpMethod.POST;
            restActions.Destroy = HttpMethod.DELETE;

            ResourceManager.GetInstance(page).RegisterClientScriptBlock("CodeMaskBuilder", Resource.CodeMaskBuilder);
            ResourceManager.GetInstance(page).RegisterClientScriptBlock("StdHandBooks", Resource.StdHandBooks);
            ResourceManager.GetInstance(page).RegisterClientScriptBlock("PfhdView.js", Resource.PfhdView);

            var fundsTab = new TabPanel
                {
                    Title = @"Операции с целевыми средствами",
                    Border = false,
                    EnableTabScroll = true,
                    Plugins = { new TabScrollerMenu { PageSize = 5 } },
                    Items =
                        {
                            GetCapFundsUi(),
                            GetRealAssetFundsUi(),
                            GetOtherGrantFundsUi()
                        }
                };

            BorderLayout borderLayout = UiBuilders.RenderBasicDocumentLayout(
                page, 
                DocId.ToString(), 
                new List<Component> { GetDetailUi(), fundsTab }, 
                GetToolBar());

            Component docsDetail = ((TabPanel)borderLayout.Center.Items[0]).Items[2];

            // Устанавливаем обработчик на грид с типами прикрепляемых документов нужен для включения и отключения кнопки "добавить"
            var gp = (GridPanel)((ColumnLayout)((Panel)docsDetail).Items[0]).Columns[0].Items[0];
            ((ImageCommandColumn)gp.ColumnModel.GetColumnById("ComandColumn")).PrepareGroupCommand.Handler =
                GlobalConsts.ScopeStateToolBar + ".prepareCommand(command);";

            var view =
                new Viewport
                    {
                        ID = "vpPfhd", 
                        Items =
                            {
                                borderLayout
                            }
                    };

            return new List<Component> { view };
        }

        private Toolbar GetToolBar()
        {
            Toolbar tb = new NewStateToolBarControl(DocId.HasValue ? DocId.Value : 0).BuildComponent(Page);
            tb.Add(new ToolbarSeparator());

            tb.Add(new VersioningControl(Convert.ToInt32(DocId.ToString()), UiBuilders.GetControllerID<PfhdController>(), Scope + ".SetReadOnlyDoc").Build(Page));

            if (Auth.IsAdmin() || Auth.IsKristaRu())
            {
                var export = new UpLoadFileBtnControl
                {
                    Id = "btnExport",
                    Name = "Экспорт в XML",
                    Icon = Icon.DiskDownload,
                    Upload = false,
                    UploadController = UiBuilders.GetUrl<PfhdController>("ExportToXml"),
                    Params = { { "recId", DocId.ToString() } }
                };

                tb.Add(export.Build(Page));

                tb.Add(new ToolbarSeparator());

                tb.Add(new SetDocStateBtn(Convert.ToInt32(DocId.ToString())).Build(Page));
            }

            return tb;
        }

        #region PFHD

        private void GetStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(PfhdStoreID, true, typeof(PfhdController), "Read", "Save", "Save");
            store.AddFieldsByClass(pfhdModel);

            store.SetBaseParams("parentId", DocId.ToString(), ParameterMode.Raw);
            store.SetWriteBaseParams("parentId", DocId.ToString(), ParameterMode.Raw);
            store.Listeners.Load.Fn(Scope, "LoadPFHD");

            Page.Controls.Add(store);
        }

        private Panel GetDetailUi()
        {
            GetStore();

            FormPanel row0Form = GetFormForRow(PfhdFormCol1ID, true, 1);
            FormPanel row1Form = GetFormForRow(PfhdFormCol2ID, false, 2);
            FormPanel row2Form = GetFormForRow(PfhdFormCol3ID, false, 3);

            row0Form.AutoScroll = false;
            row1Form.AutoScroll = false;
            row2Form.AutoScroll = false;

            row0Form.AutoHeight = true;
            row1Form.AutoHeight = true;
            row2Form.AutoHeight = true;

            var panel =
                new Panel
                    {
                        TopBar =
                            {
                                new Toolbar
                                    {
                                        Items =
                                            {
                                                new Button
                                                    {
                                                        ID = PfhdFormsButtonRefreshID, 
                                                        Icon = Icon.PageRefresh, 
                                                        ToolTip = @"Обновить", 
                                                        Listeners =
                                                            {
                                                                Click =
                                                                    {
                                                                       Handler = @"{0}.reload()".FormatWith(PfhdStoreID) 
                                                                    }
                                                            }
                                                    }, 
                                                new Button
                                                    {
                                                        ID = PfhdFormsButtonSaveID, 
                                                        Icon = Icon.TableSave, 
                                                        Text = @"Сохранить данные", 
                                                        ToolTip = @"Сохранить данные", 
                                                        Listeners =
                                                            {
                                                                Click =
                                                                    {
                                                                        Handler = Scope + @".SavePFHD();"
                                                                    }
                                                            }
                                                    }
                                            }
                                    }
                            }, 
                        Title = @"План финансово-хозяйственной деятельности", 
                        Border = false, 
                        AutoScroll = true, 
                        Layout = "HBox", 
                        Padding = 5, 
                        Items = { row0Form, row1Form, row2Form }
                    };

            return panel;
        }

        // <summary>
        //   Вариант раскладки без 3х форм. Так более красиво и компактно. Но долго переделывать
        // </summary>
        // <param name="formId"> </param>
        // <param name="showLabels"> </param>
        // <param name="mainColumnNum"> </param>
        // <returns> </returns>
        // public FormPanel GetFormForRowNew(bool Plain3Year = false)
        // {
        // var form =
        // new FormPanel
        // {
        // ID = PfhdForm,
        // Border = false,
        // AutoScroll = true,
        // AutoHeight = true,
        // MonitorValid = true,
        // DefaultAnchor = "100%",
        // LabelWidth = 400,
        // LabelSeparator = "",
        // };
        // var CopmositFields = new CompositeField { FieldLabel = "Показатели финансового состояния", AnchorHorizontal = "100%" };
        // CopmositFields.Items.Add(new DisplayField { Text = "Очередной финансовый год,руб", Flex = 1 });
        // CopmositFields.Items.Add(new DisplayField { ID = "DF1", Text = "Первый год планового периода, руб", Flex = 1, Hidden = false});
        // CopmositFields.Items.Add(new DisplayField { ID = "DF2", Text = "Второй год планового периода, руб", Flex = 1, Hidden = false });
        // form.Items.Add(CopmositFields);
        // CopmositFields = new CompositeField { FieldLabel = "Общая сумма балансовой стоимости нефинансовых активов", AnchorHorizontal = "100%" };
        // CopmositFields.Items.Add(new NumberField { StyleSpec = "border-top: 1px Solid Black;", ID = "totnonfinAssets1", DataIndex = "totnonfinAssets1", AllowDecimals = false, Flex = 1 });
        // CopmositFields.Items.Add(new NumberField { StyleSpec = "border-top: 1px Solid Black;", ID = "totnonfinAssets2", DataIndex = "totnonfinAssets2", AllowDecimals = false, Flex = 1 });
        // CopmositFields.Items.Add(new NumberField { StyleSpec = "border-top: 1px Solid Black;", ID = "totnonfinAssets3", DataIndex = "totnonfinAssets3", AllowDecimals = false, Flex = 1 });
        // form.Items.Add(CopmositFields);
        // var group = new FieldSet
        // {
        // Title = "Из них:",
        // Collapsible = true,
        // Collapsed = true,
        // Layout = LayoutType.Form.ToString(),
        // };
        // form.Items.Add(group);
        // CopmositFields = new CompositeField { FieldLabel = "Недвижимое имущество", AnchorHorizontal = "100%" };
        // CopmositFields.Items.Add(new NumberField { ID = "realAssets1", DataIndex = "realAssets1", AllowDecimals = false, Flex = 1 });
        // CopmositFields.Items.Add(new NumberField { ID = "realAssets2", DataIndex = "realAssets2", AllowDecimals = false, Flex = 1 });
        // CopmositFields.Items.Add(new NumberField { ID = "realAssets3", DataIndex = "realAssets3", AllowDecimals = false, Flex = 1 });
        // group.Items.Add(CopmositFields);
        // CopmositFields = new CompositeField { FieldLabel = "Особо ценное движимое имущество", AnchorHorizontal = "100%" };
        // CopmositFields.Items.Add(new NumberField { ID = "highValPersAssets1", DataIndex = "highValPersAssets1", AllowDecimals = false, Flex = 1 });
        // CopmositFields.Items.Add(new NumberField { ID = "highValPersAssets2", DataIndex = "highValPersAssets2", AllowDecimals = false, Flex = 1 });
        // CopmositFields.Items.Add(new NumberField { ID = "highValPersAssets3", DataIndex = "highValPersAssets3", AllowDecimals = false, Flex = 1 });
        // group.Items.Add(CopmositFields);
        // return form;
        // }
        private FormPanel GetFormForRow(string formId, bool showLabels, int mainColumnNum)
        {
            const string Spacer = " ";

            var form =
                new FormPanel
                    {
                        ID = formId, 
                        Border = false, 
                        MonitorValid = true, 
                        DefaultAnchor = "100%", 
                        Width = showLabels ? 660 : 190, 
                        LabelWidth = showLabels ? 390 : 5, 
                        LabelSeparator = string.Empty, 
                    };

            form.Items.Add(
                new NumberField
                    {
                        ID = pfhdModel.NameOf(() => pfhdModel.RefParameterID) + mainColumnNum, 
                        DataIndex = pfhdModel.NameOf(() => pfhdModel.RefParameterID), 
                        FieldLabel = @"ШапкаДокумента/Ссылка", 
                        Hidden = true, 
                        AllowDecimals = false
                    });

            switch (mainColumnNum)
            {
                case 1:
                    form.Items.Add(
                        new Label
                            {
                               ID = "Label1", Text = @"Показатели финансового состояния, на 1 янв. 1-го года" 
                            });
                    break;
                case 2:
                    form.Items.Add(new Label { ID = "Label2", Text = @"2-го года", Hidden = true });
                    break;
                case 3:
                    form.Items.Add(new Label { ID = "Label3", Text = @"3-го года", Hidden = true });
                    break;
            }

            form.Items.Add(
                new NumberField
                    {
                        StyleSpec = "border-top: 1px Solid Black;",
                        ID = pfhdModel.NameOf(() => pfhdModel.TotnonfinAssets) + mainColumnNum, 
                        DataIndex = pfhdModel.NameOf(() => pfhdModel.TotnonfinAssets), 
                        FieldLabel =
                            showLabels ? "Общая сумма балансовой стоимости нефинансовых активов" : Spacer, 
                        AllowDecimals = true, 
                        DecimalPrecision = 2, 
                        DecimalSeparator = ",", 
                        Hidden = !showLabels
                    });
            form.Items.Add(
                new Label
                    {
                        Text = @"Из них:", 
                        Hidden = !showLabels
                    });

            form.Items.Add(
                new NumberField
                    {
                        ID = pfhdModel.NameOf(() => pfhdModel.RealAssets) + mainColumnNum, 
                        DataIndex = pfhdModel.NameOf(() => pfhdModel.RealAssets), 
                        FieldLabel = showLabels ? "Недвижимое имущество" : Spacer, 
                        AllowDecimals = true, 
                        DecimalPrecision = 2, 
                        DecimalSeparator = ",", 
                        Hidden = !showLabels
                    });

            form.Items.Add(
                new NumberField
                    {
                        StyleSpec = "border-bottom: 1px Solid Black;", 
                        ID = pfhdModel.NameOf(() => pfhdModel.HighValPersAssets) + mainColumnNum, 
                        DataIndex = pfhdModel.NameOf(() => pfhdModel.HighValPersAssets), 
                        FieldLabel = showLabels ? "Особо ценное движимое имущество" : Spacer, 
                        AllowDecimals = true, 
                        DecimalPrecision = 2, 
                        DecimalSeparator = ",", 
                        Hidden = !showLabels
                    });

            form.Items.Add(new Label(Spacer));

            form.Items.Add(
                new NumberField
                    {
                        StyleSpec = "border-top: 1px Solid Black;", 
                        ID = pfhdModel.NameOf(() => pfhdModel.FinAssets) + mainColumnNum, 
                        DataIndex = pfhdModel.NameOf(() => pfhdModel.FinAssets), 
                        FieldLabel = showLabels ? "Общая сумма финансовых активов" : Spacer, 
                        AllowDecimals = true, 
                        DecimalPrecision = 2, 
                        DecimalSeparator = ",", 
                        Hidden = !showLabels
                    });

            form.Items.Add(
                new Label
                    {
                        Text = @"Из них:", 
                        Hidden = !showLabels
                    });

            form.Items.Add(
                new NumberField
                    {
                        ID = pfhdModel.NameOf(() => pfhdModel.Income) + mainColumnNum, 
                        DataIndex = pfhdModel.NameOf(() => pfhdModel.Income), 
                        FieldLabel = showLabels ? "Сумма дебиторской задолженности по доходам" : Spacer, 
                        AllowDecimals = true, 
                        DecimalPrecision = 2, 
                        DecimalSeparator = ",", 
                        Hidden = !showLabels
                    });

            form.Items.Add(
                new NumberField
                    {
                        StyleSpec = "border-bottom: 1px Solid Black;", 
                        ID = pfhdModel.NameOf(() => pfhdModel.Expense) + mainColumnNum, 
                        DataIndex = pfhdModel.NameOf(() => pfhdModel.Expense), 
                        FieldLabel = showLabels ? "Сумма дебиторской задолженности по расходам" : Spacer, 
                        AllowDecimals = true, 
                        DecimalPrecision = 2, 
                        DecimalSeparator = ",", 
                        Hidden = !showLabels
                    });

            form.Items.Add(new Label(Spacer));

            form.Items.Add(
                new NumberField
                    {
                        StyleSpec = "border-top: 1px Solid Black;", 
                        ID = pfhdModel.NameOf(() => pfhdModel.FinCircum) + mainColumnNum, 
                        DataIndex = pfhdModel.NameOf(() => pfhdModel.FinCircum), 
                        FieldLabel = showLabels ? "Общая сумма обязательств" : Spacer, 
                        AllowDecimals = true, 
                        DecimalPrecision = 2, 
                        DecimalSeparator = ",", 
                        Hidden = !showLabels
                    });

            form.Items.Add(
                new Label
                    {
                        Text = @"Из них:", 
                        Hidden = !showLabels
                    });

            form.Items.Add(
                new NumberField
                    {
                        StyleSpec = "border-bottom: 1px Solid Black;", 
                        ID = pfhdModel.NameOf(() => pfhdModel.KreditExpir) + mainColumnNum, 
                        DataIndex = pfhdModel.NameOf(() => pfhdModel.KreditExpir), 
                        FieldLabel = showLabels ? "Сумма просроченной кредиторской задолженности" : Spacer, 
                        AllowDecimals = true, 
                        DecimalPrecision = 2, 
                        DecimalSeparator = ",", 
                        Hidden = !showLabels
                    });

            form.Items.Add(new Label(Spacer));
            form.Items.Add(new Label(Spacer));
            switch (mainColumnNum)
            {
                case 1:
                    form.Items.Add(new Label { ID = "Label4", Text = @"Показатели поступлений и выплат, 1-й год'" });
                    break;
                case 2:
                    form.Items.Add(
                        new Label
                            {
                                ID = "Label5", 
                                Text = @"2-й год", 
                                StyleSpec = "margin-top: 316px; display: inline-block;"
                            });
                    break;
                case 3:
                    form.Items.Add(
                        new Label
                            {
                                ID = "Label6", 
                                Text = @"3-й год", 
                                StyleSpec = "margin-top: 316px; display: inline-block;"
                            });
                    break;
            }

            form.Items.Add(
                new NumberField
                    {
                        StyleSpec = "border-top: 1px Solid Black;", 
                        ID = pfhdModel.NameOf(() => pfhdModel.PlanInpayments) + mainColumnNum, 
                        DataIndex = pfhdModel.NameOf(() => pfhdModel.PlanInpayments), 
                        FieldLabel = showLabels ? "Планируемая сумма поступлений, всего" : Spacer, 
                        AllowDecimals = true, 
                        DecimalPrecision = 2, 
                        DecimalSeparator = ",", 
                    });
            form.Items.Add(new Label { Text = @"Из них:" });
            form.Items.Add(
                new NumberField
                    {
                        ID = pfhdModel.NameOf(() => pfhdModel.StateTaskGrant) + mainColumnNum, 
                        DataIndex = pfhdModel.NameOf(() => pfhdModel.StateTaskGrant), 
                        FieldLabel = showLabels ? "Субсидии на выполнение задания" : Spacer, 
                        AllowDecimals = true, 
                        DecimalPrecision = 2, 
                        DecimalSeparator = ",", 
                    });
            form.Items.Add(
                new NumberField
                    {
                        ID = pfhdModel.NameOf(() => pfhdModel.ActionGrant) + mainColumnNum, 
                        DataIndex = pfhdModel.NameOf(() => pfhdModel.ActionGrant),
                        FieldLabel = showLabels ? pfhdModel.DescriptionOf(() => pfhdModel.ActionGrant) : Spacer, 
                        AllowDecimals = true, 
                        DecimalPrecision = 2, 
                        DecimalSeparator = ",", 
                    });
            form.Items.Add(
                new NumberField
                    {
                        ID = "budgetaryFunds" + mainColumnNum, 
                        DataIndex = pfhdModel.NameOf(() => pfhdModel.BudgetaryFunds), 
                        FieldLabel = showLabels ? "Бюджетные инвестиции" : Spacer, 
                        AllowDecimals = true, 
                        DecimalPrecision = 2, 
                        DecimalSeparator = ",", 
                    });
            form.Items.Add(
                new NumberField
                    {
                        StyleSpec = "border-bottom: 1px Solid Black;", 
                        ID = pfhdModel.NameOf(() => pfhdModel.PaidServices) + mainColumnNum, 
                        DataIndex = pfhdModel.NameOf(() => pfhdModel.PaidServices), 
                        FieldLabel =
                            showLabels
                                ? "Оказание платных услуг(работ) и приносящая доход деятельность"
                                : Spacer, 
                        AllowDecimals = true, 
                        DecimalPrecision = 2, 
                        DecimalSeparator = ",", 
                    });

            form.Items.Add(new Label(Spacer));
            form.Items.Add(
                new NumberField
                    {
                        StyleSpec = "border-top: 1px Solid Black;", 
                        ID = pfhdModel.NameOf(() => pfhdModel.PlanPayments) + mainColumnNum, 
                        DataIndex = pfhdModel.NameOf(() => pfhdModel.PlanPayments), 
                        FieldLabel = showLabels ? "Планируемая сумма выплат, всего" : Spacer, 
                        AllowDecimals = true, 
                        DecimalPrecision = 2, 
                        DecimalSeparator = ",", 
                    });
            form.Items.Add(new Label { Text = @"Из них:" });
            form.Items.Add(
                new NumberField
                    {
                        ID = pfhdModel.NameOf(() => pfhdModel.LabourRemuneration) + mainColumnNum, 
                        DataIndex = pfhdModel.NameOf(() => pfhdModel.LabourRemuneration), 
                        FieldLabel =
                            showLabels ? "Оплата труда и начисления на выплаты по оплате труда" : Spacer, 
                        AllowDecimals = true, 
                        DecimalPrecision = 2, 
                        DecimalSeparator = ",", 
                    });
            form.Items.Add(
                new NumberField
                    {
                        ID = pfhdModel.NameOf(() => pfhdModel.TelephoneServices) + mainColumnNum, 
                        DataIndex = pfhdModel.NameOf(() => pfhdModel.TelephoneServices), 
                        FieldLabel = showLabels ? "Оплата услуг связи" : Spacer, 
                        AllowDecimals = true, 
                        DecimalPrecision = 2, 
                        DecimalSeparator = ",", 
                    });
            form.Items.Add(
                new NumberField
                    {
                        ID = pfhdModel.NameOf(() => pfhdModel.FreightServices) + mainColumnNum, 
                        DataIndex = pfhdModel.NameOf(() => pfhdModel.FreightServices), 
                        FieldLabel = showLabels ? "Оплата транспортных услуг" : Spacer, 
                        AllowDecimals = true, 
                        DecimalPrecision = 2, 
                        DecimalSeparator = ",", 
                    });
            form.Items.Add(
                new NumberField
                    {
                        ID = pfhdModel.NameOf(() => pfhdModel.PublicServeces) + mainColumnNum, 
                        DataIndex = pfhdModel.NameOf(() => pfhdModel.PublicServeces), 
                        FieldLabel = showLabels ? "Оплата коммунальных услуг" : Spacer, 
                        AllowDecimals = true, 
                        DecimalPrecision = 2, 
                        DecimalSeparator = ",", 
                    });
            form.Items.Add(
                new NumberField
                    {
                        ID = pfhdModel.NameOf(() => pfhdModel.Rental) + mainColumnNum, 
                        DataIndex = pfhdModel.NameOf(() => pfhdModel.Rental), 
                        FieldLabel = showLabels ? "Арендная плата за пользование имуществом" : Spacer, 
                        AllowDecimals = true, 
                        DecimalPrecision = 2, 
                        DecimalSeparator = ",", 
                    });
            form.Items.Add(
                new NumberField
                    {
                        ID = pfhdModel.NameOf(() => pfhdModel.MaintenanceCosts) + mainColumnNum, 
                        DataIndex = pfhdModel.NameOf(() => pfhdModel.MaintenanceCosts), 
                        FieldLabel = showLabels ? "Оплата услуг по содержанию имущества" : Spacer, 
                        AllowDecimals = true, 
                        DecimalPrecision = 2, 
                        DecimalSeparator = ",", 
                    });
            form.Items.Add(
                new NumberField
                    {
                        ID = pfhdModel.NameOf(() => pfhdModel.MainFunds) + mainColumnNum, 
                        DataIndex = pfhdModel.NameOf(() => pfhdModel.MainFunds), 
                        FieldLabel = showLabels ? "Приобретение основных средств" : Spacer, 
                        AllowDecimals = true, 
                        DecimalPrecision = 2, 
                        DecimalSeparator = ",", 
                    });
            form.Items.Add(
                new NumberField
                    {
                        ID = pfhdModel.NameOf(() => pfhdModel.FictitiousAssets) + mainColumnNum, 
                        DataIndex = pfhdModel.NameOf(() => pfhdModel.FictitiousAssets), 
                        FieldLabel = showLabels ? "Приобретение нематериальных активов" : Spacer, 
                        AllowDecimals = true, 
                        DecimalPrecision = 2, 
                        DecimalSeparator = ",", 
                    });
            form.Items.Add(
                new NumberField
                    {
                        StyleSpec = "border-bottom: 1px Solid Black;", 
                        ID = pfhdModel.NameOf(() => pfhdModel.TangibleAssets) + mainColumnNum, 
                        DataIndex = pfhdModel.NameOf(() => pfhdModel.TangibleAssets), 
                        FieldLabel = showLabels ? "Приобретение материальных запасов" : Spacer, 
                        AllowDecimals = true, 
                        DecimalPrecision = 2, 
                        DecimalSeparator = ",", 
                    });

            form.Items.Add(new Label(Spacer));
            form.Items.Add(
                new NumberField
                    {
                        StyleSpec = "border-top: 1px Solid Black; border-bottom: 1px Solid Black;", 
                        ID = pfhdModel.NameOf(() => pfhdModel.Publish) + mainColumnNum, 
                        DataIndex = pfhdModel.NameOf(() => pfhdModel.Publish), 
                        FieldLabel =
                            showLabels ? "Планируемая сумма выплат по публичным обязательствам" : Spacer, 
                        AllowDecimals = true, 
                        DecimalPrecision = 2, 
                        DecimalSeparator = ",", 
                    });

            return form;
        }

        #endregion

        #region CapFunds

        private Store GetCapFundsStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(CapFundsStoreID, true, typeof(CapFundsController), "Read", "Save", "Save");
            store.AddFieldsByClass(capFundsModel);

            store.SetBaseParams("parentId", DocId.ToString(), ParameterMode.Raw);
            store.SetWriteBaseParams("parentId", DocId.ToString(), ParameterMode.Raw);

            Page.Controls.Add(store);
            return store;
        }

        private Component GetCapFundsUi()
        {
            Store capFundsStore = GetCapFundsStore();
            GridPanel gridPanel = UiBuilders.CreateGridPanel(CapFundsPanelID, capFundsStore);
            gridPanel.Title = @"Информация об объектах капитального строительства";

            gridPanel.AddRefreshButton();
            gridPanel.AddNewRecordNoEditButton();
            gridPanel.AddRemoveRecordButton();
            gridPanel.AddSaveButton();

            gridPanel.ColumnModel.AddColumn(() => capFundsModel.ID, DataAttributeTypes.dtInteger)
                .SetHidden(true);
            gridPanel.ColumnModel.AddColumn(() => capFundsModel.RefParameterID, DataAttributeTypes.dtInteger)
                .SetHidden(true);
            gridPanel.ColumnModel.AddColumn(() => capFundsModel.Name, DataAttributeTypes.dtString)
                .SetEditableString().SetWidth(500).SetMaxLengthEdior(255);

            gridPanel.ColumnModel.AddColumn(() => capFundsModel.Funds, DataAttributeTypes.dtDouble)
                .SetEditableDouble(2).SetWidth(250);

            var summ = new SummFieldControl("Summ3", capFundsStore, capFundsModel.NameOf(() => capFundsModel.Funds), "Общая сумма");

            gridPanel.BottomBar.Add(new Toolbar { Items = { summ.BuildComponent(Page) } });

            return gridPanel;
        }

        #endregion

        #region RealAssetFunds

        private Store GetRealAssetFundsStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(RealAssetFundsStoreID, true, typeof(RealAssetFundsController), "Read", "Save", "Save");
            store.AddFieldsByClass(realAssetModel);

            store.SetBaseParams("parentId", DocId.ToString(), ParameterMode.Raw);
            store.SetWriteBaseParams("parentId", DocId.ToString(), ParameterMode.Raw);

            Page.Controls.Add(store);
            return store;
        }

        private Component GetRealAssetFundsUi()
        {
            Store realAssetFundsStore = GetRealAssetFundsStore();
            GridPanel gridPanel = UiBuilders.CreateGridPanel(RealAssetFundsPanelID, realAssetFundsStore);
            gridPanel.Title = @"Информация об объектах приобретаемого недвижимого имущества";

            gridPanel.AddRefreshButton();
            gridPanel.AddNewRecordNoEditButton();
            gridPanel.AddRemoveRecordButton();
            gridPanel.AddSaveButton();

            gridPanel.ColumnModel.AddColumn(() => realAssetModel.ID, DataAttributeTypes.dtInteger).SetHidden(true);
            gridPanel.ColumnModel.AddColumn(() => realAssetModel.RefParameterID, DataAttributeTypes.dtInteger)
                .SetHidden(true);
            gridPanel.ColumnModel.AddColumn(() => realAssetModel.Name, DataAttributeTypes.dtString)
                .SetEditableString().SetWidth(500).SetMaxLengthEdior(255);

            gridPanel.ColumnModel.AddColumn(() => realAssetModel.Funds, DataAttributeTypes.dtDouble)
                .SetEditableDouble(2).SetWidth(250);

            var summ = new SummFieldControl("Summ2", realAssetFundsStore, realAssetModel.NameOf(() => realAssetModel.Funds), "Общая сумма");

            gridPanel.BottomBar.Add(new Toolbar { Items = { summ.BuildComponent(Page) } });

            return gridPanel;
        }

        #endregion

        #region OtherGrantFunds

        private Store GetOtherGrantFundsStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(OtherGrantFundsStoreID, true, typeof(OtherGrantFundsController), "Read", "Save", "Save");
            store.AddFieldsByClass(otherGrantModel);

            store.SetBaseParams("parentId", DocId.ToString(), ParameterMode.Raw);
            store.SetWriteBaseParams("parentId", DocId.ToString(), ParameterMode.Raw);

            Page.Controls.Add(store);
            return store;
        }

        private Component GetOtherGrantFundsUi()
        {
            Store otherGrantFundsStore = GetOtherGrantFundsStore();
            GridPanel gridPanel = UiBuilders.CreateGridPanel(OtherGrantFundsPanelID, otherGrantFundsStore);
            gridPanel.Title = @"Информация об операциях с субсидиями на иные цели";

            gridPanel.AddRefreshButton();
            gridPanel.AddNewRecordNoEditButton();
            gridPanel.AddRemoveRecordButton();
            gridPanel.AddSaveButton();

            var btnSumm = new Button
                {
                    ID = string.Concat(gridPanel.ID, "SummBtn"),
                    Icon = Icon.Sum,
                    Text = @"Записать сумму субсидий в ПФХД",
                    ToolTip = @"Записать сумму субсидий в ПФХД",
                    DirectEvents =
                        {
                            Click =
                                {
                                    Url = UiBuilders.GetUrl<OtherGrantFundsController>("CalculateSumm"),
                                    CleanRequest = true,
                                    ExtraParams =
                                        {
                                            new Parameter("docId", DocId.ToString(), ParameterMode.Raw)
                                        },
                                    Success = @"Ext.net.Notification.show({{ iconCls : 'icon-information',
                                                                             html : 'Целевые субсидии обновлены',
                                                                             title : 'Обновление данных ПФХД',
                                                                             hideDelay  : 2000}});
                                                {0}.reload();".FormatWith(PfhdStoreID),
                                    Before = string.Concat("return ", Scope, ".BeforeSumm('{0}');".FormatWith(gridPanel.ID)) 
                                }
                        }
                };

            gridPanel.Toolbar().Add(new ToolbarSeparator());
            gridPanel.Toolbar().Add(btnSumm);
         
            gridPanel.ColumnModel.AddColumn(() => otherGrantModel.RefOtherGrantCode, DataAttributeTypes.dtInteger)
                .SetWidth(100).SetEditable(false).Renderer.Handler = "return buildMask(value, '###.##.####');";

            gridPanel.ColumnModel.AddColumn(() => otherGrantModel.RefOtherGrantName, DataAttributeTypes.dtInteger)
                .SetWidth(400).SetHbLookup(D_Fin_OtherGant.Key, "{0}.getOtherGrantFilter('{1}')".FormatWith(Scope, DocId));

            gridPanel.ColumnModel.AddColumn(() => otherGrantModel.Funds, DataAttributeTypes.dtDouble)
                .SetEditableDouble(2).SetWidth(250);

            var temp = gridPanel.ColumnModel.AddColumn(() => otherGrantModel.KOSGY, DataAttributeTypes.dtInteger);
            temp.SetWidth(50).Editor.Add(new TextField { AllowBlank = false, MaxLength = 3, MaskRe = @"[0-9]" });
            temp.Renderer.Handler = "return buildMask(value, '#.#.#');";

            var summ = new SummFieldControl("Summ1", otherGrantFundsStore, otherGrantModel.NameOf(() => otherGrantModel.Funds), "Сумма по всем субсидиям");

            gridPanel.BottomBar.Add(new Toolbar { Items = { summ.BuildComponent(Page) } });

            return gridPanel;
        }

        #endregion
    }
}
