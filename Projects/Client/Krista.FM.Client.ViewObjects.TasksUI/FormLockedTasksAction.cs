using System;
using System.Data;
using System.Windows.Forms;

using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;

using Krista.FM.Client.Common;
using Krista.FM.Client.Components;

namespace Krista.FM.Client.ViewObjects.TasksUI
{
    public partial class FormLockedTasksAction : Form
    {
        public enum LockedTasksAction {NoAction, ApplayChanges, ContinueWork};

        public FormLockedTasksAction()
        {

            InitializeComponent();
            ComponentCustomizer.CustomizeInfragisticsControls(this);
        }

        public static LockedTasksAction SelectLockedTasksAction(ref DataTable lockedTasks)
        {
            LockedTasksAction res /*= LockedTasksAction.NoAction*/;
            FormLockedTasksAction tmpFrm = new FormLockedTasksAction();
            try
            {
                tmpFrm.ugLockedTasks.DataSource = lockedTasks;
                tmpFrm.CreateCheckBoxOnStringField();
                DialogResult dlgRes = tmpFrm.ShowDialog();
                switch (dlgRes)
                {
                    case DialogResult.OK:
                        res = LockedTasksAction.ApplayChanges;
                        break;
                    case DialogResult.Ignore:
                        res = LockedTasksAction.ContinueWork;
                        break;
                    default:
                        res = LockedTasksAction.NoAction;
                        break;
                }
                /*
                if (res == DialogResult.OK)
                {
                    switch (tmpFrm.uosLockedTasksAction.CheckedIndex)
                    {
                        case 0:
                            res = LockedTasksAction.NoAction;
                            break;
                        case 1:
                            res = LockedTasksAction.ApplayChanges;
                            break;
                        case 2:
                            res = LockedTasksAction.ContinueWork;
                            break;
                    }
                }
                */
            }
            finally
            {
                tmpFrm.Dispose();
            }
            return res;
        }

        private static void ugLockedTasks_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            //e.Layout.Override.RowSizing = RowSizing.Default;
            e.Layout.ViewStyleBand = ViewStyleBand.Vertical;
            e.Layout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
            e.Layout.Override.AllowDelete = DefaultableBoolean.False;
            e.Layout.Override.AllowUpdate = DefaultableBoolean.True;
            e.Layout.Override.AllowRowFiltering = DefaultableBoolean.False;
            e.Layout.Override.AllowRowSummaries = AllowRowSummaries.False;
            e.Layout.Override.CellClickAction = CellClickAction.Edit;
            //e.Layout.Override.HeaderStyle = HeaderStyle.
            foreach (UltraGridBand gb in e.Layout.Bands)
            {
                UltraGridColumn clmn = gb.Columns["ID"];
                clmn.Header.VisiblePosition = 0;
                clmn.Width = 38;
                clmn.CellActivation = Activation.NoEdit;

                clmn = gb.Columns["State"];
                clmn.Header.Caption = "Состояние";
                clmn.Header.VisiblePosition = 1;
                clmn.Width = 70;
                clmn.CellActivation = Activation.NoEdit;

                if (gb.Columns.IndexOf("CAction") >= 0)
                {
                    clmn = gb.Columns["CAction"];
                    clmn.Header.Caption = "Действие";
                    clmn.Header.VisiblePosition = 2;
                    clmn.Width = 70;
                    clmn.CellActivation = Activation.NoEdit;
                }

                clmn = gb.Columns["ApplayChanges"];
                clmn.Header.Caption = "Применить изменения";
                clmn.Header.VisiblePosition = 3;
                clmn.Width = 120;
                clmn.CellActivation = Activation.AllowEdit;
                clmn.CellClickAction = CellClickAction.Edit;

                clmn = gb.Columns["HeadLine"];
                clmn.Header.Caption = "Наименование";
                clmn.Header.VisiblePosition = 4;
                clmn.Width = 146;
                clmn.CellActivation = Activation.NoEdit;
            }
        }

        /// <summary>
        /// добавление в грид чекбокса в поля определенного типа
        /// </summary>
        private void CreateCheckBoxOnStringField()
        {
            CheckBoxOnHeader checkBoxOnHeader = new CheckBoxOnHeader(typeof(bool), GetCheckState(), ugLockedTasks);
            ugLockedTasks.CreationFilter = checkBoxOnHeader;
        }

        /*
        void checkBoxOnHeader_OnHeaderLinesChange(object sender, int bandIndex, int colLines)
        {
            this.ugLockedTasks.DisplayLayout.Bands[bandIndex].ColHeaderLines = colLines +1;
            this.ugLockedTasks.DisplayLayout.NotifyPropChange(Infragistics.Win.UltraWinGrid.PropertyIds.ColHeaderLines);
        }*/

        /// <summary>
        /// определение, какое состояние у чекбокса выставлять в момент создания
        /// </summary>
        /// <returns></returns>
        CheckState GetCheckState()
        {
            //CheckState returnValue = CheckState.Indeterminate;
            //CheckState returnValue = Convert.ToBoolean(ugLockedTasks.Rows[0].Cells["ApplayChanges"].Value) ? CheckState.Checked : CheckState.Unchecked;
            foreach (UltraGridRow row in ugLockedTasks.Rows)
            {
                /*if (Convert.ToBoolean(row.Cells["ApplayChanges"].Value))
                {
                    returnValue = CheckState.Indeterminate;
                    break;
                }*/
                row.Cells["ApplayChanges"].Value = true;
                row.Update();
            }
            //return returnValue;
            return CheckState.Checked;
        }
        
        /*
        /// <summary>
        /// обработчик нажатия на чекбокс
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void checkBoxOnHeader_OnHeaderCheckBoxClick(object sender, UIElementEventArgs e)
        {
            CheckBoxUIElement checkBox = (CheckBoxUIElement)e.Element;

            Infragistics.Win.UltraWinGrid.ColumnHeader header = (Infragistics.Win.UltraWinGrid.ColumnHeader)checkBox.GetAncestor(typeof(HeaderUIElement)).GetContext(typeof(Infragistics.Win.UltraWinGrid.ColumnHeader));

            header.Tag = checkBox.CheckState;
            bool checkState = Convert.ToBoolean(checkBox.CheckState);

            UltraGridColumn column = header.Column;

            string columnName = column.Key;
            bool check = checkBox.CheckState == CheckState.Checked;
            foreach (UltraGridRow row in this.ugLockedTasks.Rows)
            {
                row.Cells["ApplayChanges"].Value = check;
                row.Update();
            }
        }*/

        /*private Infragistics.Win.ToolTip toolTipValue = null;

        private Infragistics.Win.ToolTip pToolTip
        {
            get
            {
                if (null == this.toolTipValue)
                {
                    this.toolTipValue = new Infragistics.Win.ToolTip(this);
                    this.toolTipValue.DisplayShadow = true;
                    this.toolTipValue.AutoPopDelay = 0;
                    this.toolTipValue.InitialDelay = 0;
                }
                return this.toolTipValue;
            }
        }

        private void ugLockedTasks_MouseEnterElement(object sender, UIElementEventArgs e)
        {
            if (e.Element is CheckBoxUIElement)
            {
                CheckBoxUIElement checkBox = (CheckBoxUIElement)e.Element;
                if (checkBox != null)
                    if (checkBox.Parent is HeaderUIElement)
                    {
                        if (checkBox.CheckState == CheckState.Unchecked)
                            pToolTip.ToolTipText = "Применить для всех";
                        else
                            pToolTip.ToolTipText = "Отменить для всех";

                        Point tooltipPos = new Point(e.Element.ClipRect.Left, e.Element.ClipRect.Bottom);
                        tooltipPos.Y = tooltipPos.Y + checkBox.Rect.Height + 2;
                        pToolTip.Show(this.ugLockedTasks.PointToScreen(tooltipPos));
                    }
            }
        }

        private void ugLockedTasks_MouseLeaveElement(object sender, UIElementEventArgs e)
        {
            if (e.Element is CheckBoxUIElement)
                pToolTip.Hide();
        }*/
    }
}