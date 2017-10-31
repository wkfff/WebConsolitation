using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI
{
    public class DebtorBookUI : BaseViewObj
    {
        internal DebtorBookUI(string key)
            : base(key)
        {
            subjectCls = null;
            regionCls = null;
            settlementCls = null;
            InfragisticsRusification.LocalizeAll();
            fViewCtrl = new DebtorBookView();
            Workplace = DebtBookNavigation.Instance.Workplace;
        }

        public override Control Control
        {
            get { return fViewCtrl; }
        }

        protected override void SetViewCtrl()
        {
            if (fViewCtrl == null)
                fViewCtrl = new DebtorBookView();
        }

        public override void Initialize()
        {
            base.Initialize();

            DebtBookNavigation.Instance.VariantChanged += VariantChanged;
            SetUserDataVisible();
        }

        void VariantChanged(object sender, EventArgs e)
        {
            if (subjectCls != null)
            {
                subjectCls.CurrentDataSourceID = DebtBookNavigation.Instance.CurrentSourceID;
                subjectCls.CurrentDataSourceYear = DebtBookNavigation.Instance.VariantYear;
                subjectCls.FireRefreshData();
            }
            if (regionCls != null)
            {
                regionCls.CurrentDataSourceID = DebtBookNavigation.Instance.CurrentSourceID;
                regionCls.CurrentDataSourceYear = DebtBookNavigation.Instance.VariantYear;

                regionCls.FireRefreshData();
            }
            if (settlementCls != null)
            {
                settlementCls.CurrentDataSourceID = DebtBookNavigation.Instance.CurrentSourceID;
                settlementCls.CurrentDataSourceYear = DebtBookNavigation.Instance.VariantYear;
                settlementCls.FireRefreshData();
            }
        }

        private DebtBookCls subjectCls;
        private DebtBookCls regionCls;
        private DebtBookCls settlementCls;

        protected override void Dispose(bool disposing)
        {
            if (subjectCls != null)
                subjectCls.Dispose();
            if (regionCls != null)
                regionCls.Dispose();
            if (settlementCls != null)
                settlementCls.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// устанавливаем видимость данных
        /// </summary>
        protected void SetUserDataVisible()
        {
            DebtBookNavigation.Instance.UserRegionType =
                GetUserRegionType(DebtBookNavigation.Instance.Workplace.ActiveScheme.UsersManager.GetCurrentUserID());

            switch (DebtBookNavigation.Instance.UserRegionType)
            {
                case UserRegionType.Subject:
                //case UserRegionType.Unknown:
                    // для субъекта федерации фильтров не ставим
                    subjectCls = GetSubjectCls();
                    regionCls = GetRegionCls();
                    settlementCls = GetSettlementCls();
                    break;
                case UserRegionType.Town:
                    // для района ставим фильтр на район
                    regionCls = GetRegionCls();
                    break;
                case UserRegionType.Region:
                    // для района ставим фильтр на район
                    regionCls = GetRegionCls();
                    settlementCls = GetSettlementCls();
                    break;
                case UserRegionType.Settlement:
                    // для поселения ставим фильтр на поселение
                    settlementCls = GetSettlementCls();
                    break;
            }
            // прикручиваем объекты отображения данных
            ((DebtorBookView) Control).DataTabControl.Tabs[0].Visible = subjectCls != null;
            if (subjectCls != null)
            {
                subjectCls.Workplace = Workplace;
                subjectCls.Initialize();
                subjectCls.CurrentDataSourceID = DebtBookNavigation.Instance.CurrentSourceID;
                subjectCls.CurrentDataSourceYear = DebtBookNavigation.Instance.VariantYear;
                subjectCls.AttachViewObject(((DebtorBookView)Control).SubjectPanel, false);
                subjectCls.Refresh();
            }

            ((DebtorBookView) Control).DataTabControl.Tabs[1].Visible = regionCls != null;
            if (regionCls != null)
            {
                regionCls.Workplace = Workplace;
                regionCls.Initialize();
                regionCls.CurrentDataSourceID = DebtBookNavigation.Instance.CurrentSourceID;
                regionCls.CurrentDataSourceYear = DebtBookNavigation.Instance.VariantYear;
                regionCls.AttachViewObject(((DebtorBookView)Control).RegionPanel, false);
                regionCls.Refresh();
            }

            ((DebtorBookView) Control).DataTabControl.Tabs[2].Visible = settlementCls != null;
            if (settlementCls != null)
            {
                settlementCls.Workplace = Workplace;
                settlementCls.Initialize();
                settlementCls.CurrentDataSourceID = DebtBookNavigation.Instance.CurrentSourceID;
                settlementCls.CurrentDataSourceYear = DebtBookNavigation.Instance.VariantYear;
                settlementCls.AttachViewObject(((DebtorBookView)Control).SettlementPanel, false);
                settlementCls.Refresh();
            }
        }

        protected virtual DebtBookCls GetSubjectCls()
        {
            return null;
        }

        protected virtual DebtBookCls GetRegionCls()
        {
            return null;
        }

        protected virtual DebtBookCls GetSettlementCls()
        {
            return null;
        }

        /// <summary>
        /// получаем тип региона, к которому привязан пользователь
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        protected UserRegionType GetUserRegionType(int userID)
        {
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                string regionQuery = string.Format("select RefTerr from d_Regions_Analysis where id = {0}",
                    DebtBookNavigation.Instance.CurrentRegion);
                DataTable dtRegion = (DataTable) db.ExecQuery(regionQuery, QueryResultTypes.DataTable);
                if (dtRegion.Rows.Count != 0)
                {
                    int regionType = Convert.ToInt32(dtRegion.Rows[0][0]);
                    switch (regionType)
                    {
                        case 3:
                            return UserRegionType.Subject;
                        case 4:
                            return UserRegionType.Region;
                        case 7:
                            return UserRegionType.Town;
                        case 5:
                        case 6:
                        case 11:
                            return UserRegionType.Settlement;
                    }
                }
                return UserRegionType.Unknown;
            }
        }
    }
}