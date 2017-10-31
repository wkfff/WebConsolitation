using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win.UltraWinGrid;

using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.Common;

namespace Krista.FM.Client.ViewObjects.AdministrationUI
{
    internal partial class frmModalTemplate : Form
    {
        public frmModalTemplate()
        {
            InitializeComponent();
        }
    }

    public class UsersModalForm : IUsersModal
    {
        private IWorkplace _workplace = null;

        private AdministrationUI _uiObj = null;

        private frmModalTemplate _form = null;

        public UsersModalForm(IWorkplace workplace)
        {
            if (workplace == null)
                throw new Exception("Не задан интерфейс IWorkplace");
            _workplace = workplace;

            _form = new frmModalTemplate();

            _uiObj = new AdministrationUI(AdministrationNavigationObjectKeys.Users);
            _uiObj.Workplace = _workplace;
            _uiObj.InInplaceMode = true;
			_uiObj.CreatableTaskTypesIds = _workplace.ActiveScheme.UsersManager.GetUserCreatableTaskTypes(ClientAuthentication.UserID);
            _uiObj.Initialize();

            AdministrationView vo = (AdministrationView)_uiObj.ViewCtrl;
            vo.ugeAllList.Parent = null;
            vo.ugeAllList.Parent = _form.spcContainer.Panel1;
            vo.ugeAllList.Dock = DockStyle.Fill;
            vo.ugeAllList._utmMain.Visible = false;
            vo.ugeAllList._ugData.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
        }

        public bool ShowModal(NavigationNodeKind kind, ref int mainID, ref string name)
        {
            AdministrationView vo = (AdministrationView)_uiObj.ViewCtrl;
            _uiObj.SetNavigationNodeKind(kind);
            switch (kind)
            {
                case NavigationNodeKind.ndAllUsers:
                    _form.Text = "Выбор пользователя";
                    break;
                case NavigationNodeKind.ndTasksTypes:
                    _form.Text = "Выбор типа задачи";
                    break;
                case NavigationNodeKind.ndOrganizations:
                    _form.Text = "Выбор организации";
                    break;
                case NavigationNodeKind.ndDivisions:
                    _form.Text = "Выбор отдела";
                    break;
                default:
                    throw new Exception("Не поддерживаемый тип узла: " + kind.ToString());
            }
            _uiObj.LoadData();

            vo.ugeAllList._ugData.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            vo.ugeAllList._ugData.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            vo.ugeAllList._ugData.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            vo.ugeAllList._ugData.DisplayLayout.AddNewBox.Hidden = true;
            vo.ugeAllList._ugData.KeyDown += new KeyEventHandler(_ugData_KeyDown);

            if (kind == NavigationNodeKind.ndAllUsers)
            {
                // делаем активным текущего пользователя
				UltraGridRow curUserRow = UltraGridHelper.FindGridRow(vo.ugeAllList._ugData, "ID", ClientAuthentication.UserID.ToString());
                if (curUserRow != null)
                {
                    //curUserRow.Selected = true;
                    vo.ugeAllList.ugData.ActiveRow = curUserRow;
                }
            }

            bool succeeded = (_form.ShowDialog() == DialogResult.OK) && (vo.ugeAllList.ugData.ActiveRow != null);
            if (succeeded)
            {
                //UltraGridRow row = vo.ugeAllList._ugData.Selected.Rows[0];
                UltraGridRow row = vo.ugeAllList.ugData.ActiveRow;
                mainID = UltraGridHelper.GetRowID(row);
                //mainID = UltraGridHelper.GetActiveID(vo.ugeAllList._ugData);
                succeeded = mainID != -1;
                if (succeeded)
                {
                    name = Convert.ToString(UltraGridHelper.GetRowCells(row).Cells["Name"].Value);
                    //UltraGridRow row = UltraGridHelper.GetActiveRowCells(vo.ugeAllList._ugData);
                    //name = Convert.ToString(row.Cells["Name"].Value);
                }
            }
            return succeeded;
        }

        void _ugData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift && e.KeyCode == Keys.Enter)
            {
                _form.btnOk.PerformClick();
            }
        }

    }
}