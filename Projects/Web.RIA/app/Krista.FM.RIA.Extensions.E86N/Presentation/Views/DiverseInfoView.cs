using System.Collections.Generic;
using System.Web.Mvc;

using Ext.Net;

using Krista.FM.Domain;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Extensions;
using Krista.FM.RIA.Extensions.E86N.Models.DiverseInfo;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Views
{
    /// <summary>
    ///  Иная информация об учреждении
    /// </summary>
    public sealed class DiverseInfoView : DocBaseView
    {
        #region Setting

            private const string Scope = "E86n.View.DiverseInfoView";

            private const string TofkListID = "TofkList";
            private const string PaymentDetailsID = "PaymentDetails";
            private const string PaymentDetailsTargetsID = "PaymentDetailsTargets";
            private const string LicenseDetailsID = "LicenseDetails";
            private const string AccreditationDetailsID = "AccreditationDetails";

            private readonly TofkListModel tofkListModel = new TofkListModel();
            private readonly PaymentDetailsModel paymentDetailsModel = new PaymentDetailsModel();
            private readonly PaymentDetailsTargetsModel paymentDetailsTargetsModel = new PaymentDetailsTargetsModel();
            private readonly LicenseDetailsModel licenseDetailsModel = new LicenseDetailsModel();
            private readonly AccreditationDetailsModel accreditationDetailsModel = new AccreditationDetailsModel();

        #endregion

        public override List<Component> Build(ViewPage page)
        {
            ViewController = UiBuilders.GetControllerID<DiverseInfoViewController>();
            ReadOnlyDocHandler = Scope + ".SetReadOnlyDoc";

            var components = base.Build(page);

            ResourceManager.RegisterClientScriptBlock("DiverseInfoView", Resource.DiverseInfoView);

            Detail = UiBuilders.GetTabbedDetails(
                                                    new List<Component>
                                                    {
                                                        GetTofkList(),
                                                        GetPaymentDetailsUi(),
                                                        GetLicenseDetails(),
                                                        GetAccreditationDetails()
                                                    });
            Detail.ID = "DetailTabPanel";
            ((TabPanel)Detail).Listeners.TabChange.Fn(Scope, "reloadDetail");

            return components;
        }

        private GridPanel GetTofkList()
        {
            var store = GetStoreByModel(tofkListModel, TofkListID);

            var gp = UiBuilders.CreateGridPanel(TofkListID, store);
            gp.Title = @"Перечень организаций, в которых открыты счета";

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddDeleteRecordWithConfirmButton();
            gp.AddSaveButton();

            gp.ColumnModel.AddColumn(() => tofkListModel.TofkName, DataAttributeTypes.dtString)
                .SetEditableString().SetWidth(200).SetMaxLengthEdior(50);

            gp.ColumnModel.AddColumn(() => tofkListModel.TofkAddress, DataAttributeTypes.dtString)
                .SetEditableString().SetWidth(400).SetMaxLengthEdior(2000);

            var gridFilters = new GridFilters { Local = true };
            gridFilters.Filters.Add(new NumericFilter { DataIndex = tofkListModel.NameOf(() => tofkListModel.ID) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = tofkListModel.NameOf(() => tofkListModel.TofkName) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = tofkListModel.NameOf(() => tofkListModel.TofkAddress) });
            
            gp.Plugins.Add(gridFilters);

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private GridPanel GetPaymentDetails()
        {
            var store = GetStoreByModel(paymentDetailsModel, PaymentDetailsID);

            var gp = UiBuilders.CreateGridPanel(PaymentDetailsID, store);
            gp.Title = @"Платежные реквизиты";

            ((RowSelectionModel)gp.GetSelectionModel()).Listeners.RowSelect.AddAfter(string.Concat(Scope, ".RowSelect(record);"));
            
            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddDeleteRecordWithConfirmButton();
            gp.AddSaveButton();
            
            gp.ColumnModel.AddColumn(() => paymentDetailsModel.RefPaymentDetailsTypeName, DataAttributeTypes.dtString)
                .SetWidth(100)
                .SetComboBoxEditor(FX_FX_PaymentDetailsType.Key, Page, paymentDetailsModel.NameOf(() => paymentDetailsModel.RefPaymentDetailsType), null, null, false, false);

            gp.ColumnModel.AddColumn(() => paymentDetailsModel.TofkName, DataAttributeTypes.dtString)
                .SetNullable().SetEditableString().SetWidth(100).SetMaxLengthEdior(50);

            gp.ColumnModel.AddColumn(() => paymentDetailsModel.BankName, DataAttributeTypes.dtString)
                .SetEditableString().SetWidth(200).SetMaxLengthEdior(200);

            gp.ColumnModel.AddColumn(() => paymentDetailsModel.BankCity, DataAttributeTypes.dtString)
                .SetNullable().SetEditableString().SetWidth(200).SetMaxLengthEdior(25);

            gp.ColumnModel.AddColumn(() => paymentDetailsModel.Bik, DataAttributeTypes.dtString)
               .SetEditableString().SetWidth(70).SetLengthEdior(9).SetMaskReEdior(@"[0-9]");

            gp.ColumnModel.AddColumn(() => paymentDetailsModel.CorAccountCode, DataAttributeTypes.dtString)
                .SetNullable().SetEditableString().SetWidth(130).SetLengthEdior(20).SetMaskReEdior(@"[0-9]");

            gp.ColumnModel.AddColumn(() => paymentDetailsModel.CalcAccountCode, DataAttributeTypes.dtString)
               .SetEditableString().SetWidth(130).SetLengthEdior(20).SetMaskReEdior("[0-9]");

            gp.ColumnModel.AddColumn(() => paymentDetailsModel.PersonalAccountCode, DataAttributeTypes.dtString)
                .SetNullable().SetEditableString().SetWidth(130).SetMaxLengthEdior(50);

            gp.ColumnModel.AddColumn(() => paymentDetailsModel.AccountName, DataAttributeTypes.dtString)
                .SetNullable().SetEditableString().SetWidth(200).SetMaxLengthEdior(200);

            var gridFilters = new GridFilters { Local = true };
            gridFilters.Filters.Add(new NumericFilter { DataIndex = paymentDetailsModel.NameOf(() => paymentDetailsModel.ID) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = paymentDetailsModel.NameOf(() => paymentDetailsModel.RefPaymentDetailsTypeName) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = paymentDetailsModel.NameOf(() => paymentDetailsModel.TofkName) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = paymentDetailsModel.NameOf(() => paymentDetailsModel.BankName) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = paymentDetailsModel.NameOf(() => paymentDetailsModel.BankCity) });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = paymentDetailsModel.NameOf(() => paymentDetailsModel.Bik) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = paymentDetailsModel.NameOf(() => paymentDetailsModel.CorAccountCode) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = paymentDetailsModel.NameOf(() => paymentDetailsModel.CalcAccountCode) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = paymentDetailsModel.NameOf(() => paymentDetailsModel.PersonalAccountCode) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = paymentDetailsModel.NameOf(() => paymentDetailsModel.AccountName) });

            gp.Plugins.Add(gridFilters);

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private GridPanel GetPaymentDetailsTargets()
        {
            var store = GetStoreByModel(paymentDetailsTargetsModel, PaymentDetailsTargetsID);
            store.SetBaseParams("parentId", string.Concat(Scope, ".getSelectedPaymentDetailsId()"), ParameterMode.Raw);
            store.SetWriteBaseParams("parentId", string.Concat(Scope, ".getSelectedPaymentDetailsId()"), ParameterMode.Raw);

            var gp = UiBuilders.CreateGridPanel(PaymentDetailsTargetsID, store);
            gp.Title = @"Назначение платежа";
            gp.Disabled = true;

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddDeleteRecordWithConfirmButton();
            gp.AddSaveButton();

            gp.ColumnModel.AddColumn(() => paymentDetailsTargetsModel.PaymentType, DataAttributeTypes.dtString)
                .SetEditableString().SetWidth(400).SetMaxLengthEdior(2000);

            gp.ColumnModel.AddColumn(() => paymentDetailsTargetsModel.PaymentTargetName, DataAttributeTypes.dtString)
                .SetEditableString().SetWidth(200).SetMaxLengthEdior(130);

            gp.ColumnModel.AddColumn(() => paymentDetailsTargetsModel.Kbk, DataAttributeTypes.dtString)
               .SetEditableString().SetWidth(130).SetLengthEdior(20);

            var gridFilters = new GridFilters { Local = true };
            gridFilters.Filters.Add(new NumericFilter { DataIndex = paymentDetailsTargetsModel.NameOf(() => paymentDetailsTargetsModel.ID) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = paymentDetailsTargetsModel.NameOf(() => paymentDetailsTargetsModel.PaymentType) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = paymentDetailsTargetsModel.NameOf(() => paymentDetailsTargetsModel.PaymentTargetName) });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = paymentDetailsTargetsModel.NameOf(() => paymentDetailsTargetsModel.Kbk) });
            
            gp.Plugins.Add(gridFilters);

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private Panel GetPaymentDetailsUi()
        {
            return new Panel
                {
                    ID = string.Concat(PaymentDetailsID, "Panel"),
                    Title = @"Платежные реквизиты",
                    Border = false,
                    Items =
                        {
                            new RowLayout
                                {
                                    Split = true,
                                    Rows =
                                        {
                                            new LayoutRow
                                                {
                                                    RowHeight = 0.65m,
                                                    Items =
                                                        {
                                                            GetPaymentDetails()
                                                        }
                                                },
                                            new LayoutRow
                                                {
                                                    RowHeight = 0.35m,
                                                    Items =
                                                        {
                                                            GetPaymentDetailsTargets()
                                                        }
                                                }
                                        }
                                }
                        }
                };
        }

        private GridPanel GetLicenseDetails()
        {
            var store = GetStoreByModel(licenseDetailsModel, LicenseDetailsID);

            var gp = UiBuilders.CreateGridPanel(LicenseDetailsID, store);
            gp.Title = @"Cведения о лицензии";

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddDeleteRecordWithConfirmButton();
            gp.AddSaveButton();

            gp.ColumnModel.AddColumn(() => licenseDetailsModel.LicenseAgencyName, DataAttributeTypes.dtString)
                .SetEditableString().SetWidth(200).SetMaxLengthEdior(200);

            gp.ColumnModel.AddColumn(() => licenseDetailsModel.LicenseName, DataAttributeTypes.dtString)
                .SetEditableString().SetWidth(200).SetMaxLengthEdior(2000);

            gp.ColumnModel.AddColumn(() => licenseDetailsModel.LicenseNum, DataAttributeTypes.dtString)
                .SetEditableString().SetWidth(130).SetMaxLengthEdior(20);

            gp.ColumnModel.AddColumn(() => licenseDetailsModel.LicenseDate, DataAttributeTypes.dtDate)
                .SetEditableDate().SetWidth(130);

            gp.ColumnModel.AddColumn(() => licenseDetailsModel.LicenseExpDate, DataAttributeTypes.dtDate)
                .SetNullable().SetEditableDate().SetWidth(130);

            var gridFilters = new GridFilters { Local = true };
            gridFilters.Filters.Add(new NumericFilter { DataIndex = licenseDetailsModel.NameOf(() => licenseDetailsModel.ID) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = licenseDetailsModel.NameOf(() => licenseDetailsModel.LicenseAgencyName) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = licenseDetailsModel.NameOf(() => licenseDetailsModel.LicenseName) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = licenseDetailsModel.NameOf(() => licenseDetailsModel.LicenseNum) });
            gridFilters.Filters.Add(new DateFilter { DataIndex = licenseDetailsModel.NameOf(() => licenseDetailsModel.LicenseDate) });
            gridFilters.Filters.Add(new DateFilter { DataIndex = licenseDetailsModel.NameOf(() => licenseDetailsModel.LicenseExpDate) });

            gp.Plugins.Add(gridFilters);

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private GridPanel GetAccreditationDetails()
        {
            var store = GetStoreByModel(accreditationDetailsModel, AccreditationDetailsID);

            var gp = UiBuilders.CreateGridPanel(AccreditationDetailsID, store);
            gp.Title = @"Сведения об аккредитации";

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddDeleteRecordWithConfirmButton();
            gp.AddSaveButton();

            gp.ColumnModel.AddColumn(() => accreditationDetailsModel.AccreditationAgencyName, DataAttributeTypes.dtString)
                .SetEditableString().SetWidth(200).SetMaxLengthEdior(200);

            gp.ColumnModel.AddColumn(() => accreditationDetailsModel.AccreditationName, DataAttributeTypes.dtString)
                .SetEditableString().SetWidth(200).SetMaxLengthEdior(2000);

            gp.ColumnModel.AddColumn(() => accreditationDetailsModel.AccreditationExpDate, DataAttributeTypes.dtDate)
                .SetEditableDate().SetWidth(130);

            var gridFilters = new GridFilters { Local = true };
            gridFilters.Filters.Add(new NumericFilter { DataIndex = accreditationDetailsModel.NameOf(() => accreditationDetailsModel.ID) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = accreditationDetailsModel.NameOf(() => accreditationDetailsModel.AccreditationAgencyName) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = accreditationDetailsModel.NameOf(() => accreditationDetailsModel.AccreditationName) });
            gridFilters.Filters.Add(new DateFilter { DataIndex = accreditationDetailsModel.NameOf(() => accreditationDetailsModel.AccreditationExpDate) });
           
            gp.Plugins.Add(gridFilters);

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }
    }
}
