using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.Domain;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI
{
    public class PostsHandBookUI : DataClsUI
    {
        public PostsHandBookUI(IEntity entity)
            : base(entity)
        {
        }

        private int refRegion = -1;

        public PostsHandBookUI(IEntity entity, int refRegion)
            : base(entity)
        {
            this.refRegion = refRegion;
        }

        public override void Initialize()
        {
            base.Initialize();
            vo.ugeCls.ServerFilterEnabled = false;
            DebtBookNavigation.Instance.VariantChanged += VariantChanged;
        }

        private void VariantChanged(object sender, EventArgs e)
        {
            //InfragisticsHelper.BurnTool(((BaseClsView)ViewCtrl).ugeCls.utmMain.Toolbars["FinSourcePlanning"].Tools["lbSelectedVariantName"], true);
            ((BaseClsView) ViewCtrl).ugeCls.BurnRefreshDataButton(true);
        }

        public override void SetPermissionsToClassifier(UltraGridEx gridEx)
        {
            ((BaseClsView)ViewCtrl).ugeCls.ServerFilterEnabled = false;
            ((BaseClsView)ViewCtrl).ugeCls.AllowAddNewRecords = true;
            ((BaseClsView)ViewCtrl).ugeCls.AllowDeleteRows = true;
            ((BaseClsView)ViewCtrl).ugeCls.AllowEditRows = true;
            ((BaseClsView)ViewCtrl).ugeCls.IsReadOnly = false;
        }

        public override void SetTaskId(ref Infragistics.Win.UltraWinGrid.UltraGridRow row)
        {
            base.SetTaskId(ref row);

            row.Cells["RefRegion"].Value = DebtBookNavigation.Instance.CurrentRegion;
        }

        protected override IDataUpdater GetActiveUpdater(int? parentID, out string filterStr)
        {

            dataQuery = refRegion == -1 ?
                string.Format("RefRegion = {0}", DebtBookNavigation.Instance.CurrentRegion) :
                string.Format("RefRegion = {0}", refRegion);
            filterStr = dataQuery;
            return ActiveDataObj.GetDataUpdater(dataQuery, null, null);
        }

        public static bool GetSignature(int refRegion, out DataTable signature)
        {
            signature = null;
            // получаем нужный классификатор
            IEntity cls = DebtBookNavigation.Instance.Workplace.ActiveScheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_S_TitleReport);
            // создаем объект просмотра классификаторов нужного типа
            DataClsUI clsUI = new PostsHandBookUI(cls, refRegion);
            clsUI.Workplace = DebtBookNavigation.Instance.Workplace;
            clsUI.RestoreDataSet = false;
            clsUI.Initialize();
            clsUI.InitModalCls(-1);
            // создаем форму
            frmModalTemplate modalClsForm = new frmModalTemplate();
            modalClsForm.AttachCls(clsUI);
            ComponentCustomizer.CustomizeInfragisticsControls(modalClsForm);
            // ...загружаем данные
            clsUI.RefreshAttachedData();
            if (modalClsForm.ShowDialog((Form)DebtBookNavigation.Instance.Workplace) == DialogResult.OK)
            {
                int clsID = modalClsForm.AttachedCls.GetSelectedID();
                // если ничего не выбрали - считаем что функция завершилась неудачно
                if (clsID == -10)
                    return false;
                signature = modalClsForm.AttachedCls.GetClsDataSet().Tables[0].Clone();
                signature.Rows.Add(
                    modalClsForm.AttachedCls.GetClsDataSet().Tables[0].Select(string.Format("ID = {0}", clsID))[0].ItemArray);
                //signature.Columns.Remove("ID");
                //signature.Columns.Remove("RowType");
                //signature.Columns.Remove("RefRegion");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Вызов справочника должностей и подписей
        /// </summary>
        /// <param name="signature"></param>
        /// <returns></returns>
        public static bool GetSignature(out DataTable signature)
        {
            return GetSignature(-1, out signature);
        }
    }
}
