using System;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Text;
using System.Drawing;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.ServerLibrary;
using CC = Krista.FM.Client.Components;
using Krista.FM.Client.Common;
using Krista.FM.Common;

using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.Design;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinMaskedEdit;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls
{
    public abstract partial class BaseClsUI : BaseViewObj, IInplaceClsView
    {
        #region Обработчики и методы, связанные с отображением аудита

        /// <summary>
        /// обработчик клика мыши на гриде с данными
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ugData_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (!InInplaceMode && _activeUIElementIsRow)
                {
                    //_auditShowObject = AuditShowObject.Row;
                    activeGridEx = vo.ugeCls;
                    auditObjectName = ActiveDataObj.FullName;
                    classType = ActiveDataObj.ClassType;
                    auditRow = GetActiveDataRow();
                    vo.cmsAudit.Show(vo.ugeCls.ugData.PointToScreen(e.Location));
                }
            }
        }

        // показывает, что курсор находится на элементе выбора записей в гриде с данными
        private bool _activeUIElementIsRow = false;

        public bool ActiveGridElementIsRow
        {
            get { return _activeUIElementIsRow; }
        }

        // показывает, что курсор находится на элементе выюлра записей в навигационном гриде
        private bool _activeUIElementIsSchemeObject = false;

        internal UltraGridEx activeGridEx;

        internal string  auditObjectName;

        internal DataRow auditRow;

        internal ClassTypes classType;

        void ugDataClsList_MouseEnterElement(object sender, UIElementEventArgs e)
        {
            _activeUIElementIsSchemeObject = e.Element is RowSelectorUIElement;
        }

        /// <summary>
        /// Вызов аудита для строки классификатора.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cmsAudit_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name == "аудитToolStripMenuItem")
            {
                vo.cmsAudit.Hide();
                int pumpId = -1;
                string strPump = auditRow.Table.Columns.Contains("PumpID") ? auditRow["PumpID"].ToString() : string.Empty;
                int.TryParse(strPump, out pumpId);
                    
                if (pumpId > 0)
                {
                    frmAuditModal.ShowAudit(Workplace, auditObjectName, GetClsRusName(), Convert.ToInt32(auditRow["ID"]), pumpId, classType);
                }
                else
                    frmAuditModal.ShowAudit(Workplace, auditObjectName, GetClsRusName(), UltraGridHelper.GetActiveID(activeGridEx.ugData), AuditShowObjects.RowObject);
            }
        }

        /// <summary>
        /// Вызов аудита для всего классификатора.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cmsAuditSchemeObject_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name == "аудитSchemeObjectToolStripMenuItem")
            {
                vo.cmsAuditSchemeObject.Hide();
                frmAuditModal.ShowAudit(this.Workplace, this.activeDataObj.FullName, this.GetClsRusName(), -1, AuditShowObjects.ClsObject);
            }
        }

        #endregion

    }
}