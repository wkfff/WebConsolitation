using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.ServerLibrary;
using ColumnStyle=Infragistics.Win.UltraWinGrid.ColumnStyle;
using Resources=Krista.FM.Client.ViewObjects.DataSourcesUI.Properties.Resources;

namespace Krista.FM.Client.ViewObjects.DataSourcesUI
{
    /// <summary>
    /// ��� ������ ��������� ������: ����� �������������, ��� ��� �������� ���������.
    /// </summary>
    public enum DependedDataSearchType
    {
        DeleteSource,
        User
    }

    public partial class FrmSourcesDependedData : Form
    {
        private DataTable dependedData;
        
        public FrmSourcesDependedData()
        {
            InitializeComponent();
        }

        public static DialogResult ShowDependedData(DataTable dependedData, int sourceID, Form parentForm, DependedDataSearchType searchType)
        {
            FrmSourcesDependedData tmpFrmDependedData = new FrmSourcesDependedData();

            if (searchType == DependedDataSearchType.User)
            {
                // ������ ���������
                tmpFrmDependedData.Text = "��������� ������";
                // ��������������� Cancel � OK, � ���� ������ OK ������. DialogResult � ���� ������ �� �����.
                tmpFrmDependedData.btnCancel.Text = "OK";
                tmpFrmDependedData.btnOk.Visible = false;
            }
            
            // �������� ��������� ������.
            tmpFrmDependedData.dependedData = dependedData;

            // ���� ����� ���������, �� ���������� �����.
            if (tmpFrmDependedData.dependedData.Rows.Count > 0)
            {
                tmpFrmDependedData.DependedDataGridEx.StateRowEnable = true;
                tmpFrmDependedData.DependedDataGridEx.OnGridInitializeLayout += new GridInitializeLayout(tmpFrmDependedData.DependedDataGridEx_OnGridInitializeLayout);
                tmpFrmDependedData.DependedDataGridEx.OnInitializeRow += new InitializeRow(tmpFrmDependedData.DependedDataGridEx_OnInitializeRow);
                tmpFrmDependedData.DependedDataGridEx._utmMain.ToolClick += new ToolClickEventHandler(tmpFrmDependedData._utmMain_ToolClick);

                tmpFrmDependedData.DependedDataGridEx.DataSource = tmpFrmDependedData.dependedData;
                InfragisticComponentsCustomize.CustomizeUltraGridParams(tmpFrmDependedData.DependedDataGridEx._ugData);

                tmpFrmDependedData.DependedDataGridEx.SaveLoadFileName = string.Format("�������� ID = {0}_��������� ������", sourceID);
                tmpFrmDependedData.DependedDataGridEx.MaximumSize = new Size(0, 0);
                
                foreach (UltraGridBand band in tmpFrmDependedData.DependedDataGridEx.ugData.DisplayLayout.Bands)
                {
                    band.Columns["UserRowCount"].SortIndicator = SortIndicator.Descending;
                }

                tmpFrmDependedData.ShowDialog(parentForm);
            }
            // ����� ������� ���������, ��� �� �����.
            else
            {
                if (searchType == DependedDataSearchType.User)
                {
                    MessageBox.Show("��������� ������ �� �������.", "����������",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    tmpFrmDependedData.DialogResult = MessageBox.Show("��������� ������ �� �������. ������� ��������?", "����������",
                                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                }
            }
            return tmpFrmDependedData.DialogResult;
        }
        
        void _utmMain_ToolClick(object sender, ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "FullCaption":
                case "FullDBName":
                case "Name":
                case "ObjectType":
                case "Count":
                {
                    DependedDataGridEx._ugData.DisplayLayout.Bands[0].Columns[e.Tool.Key].Hidden =
                            !((StateButtonTool)e.Tool).Checked;
                    break;
                }
            }
        }
        
        /// <summary>
        /// ��� ������������� ������ ��������� ������ ���� �������.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DependedDataGridEx_OnInitializeRow(object sender, InitializeRowEventArgs e)
        {
            UltraGridRow row = e.Row;
            UltraGridCell cell = row.Cells["ObjectType"];

            cell.Style = ColumnStyle.Image;
            cell.Column.AutoSizeMode = ColumnAutoSizeMode.None;

            string val = Convert.ToString(cell.Value);
            cell.Appearance.ImageBackground = GetPicByType(val);
            cell.ToolTipText = val;
        }

        /// <summary>
        /// �� ���� ������� ���������� ��������������� ������.
        /// </summary>
        /// <param name="val">��� �������.</param>
        /// <returns>������.</returns>
        private Image GetPicByType(string val)
        {
            if (val == "������������� ������")
            {
                return Resources.kd;
            }
            else
            {
                return Resources.factCls;
            }
        }

        void DependedDataGridEx_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGridBand band = e.Layout.Bands[0];

            int visiblePosition = 0;
 
            UltraGridColumn clmn = band.Columns["ObjectType"];
            clmn.Header.VisiblePosition = visiblePosition++;
            clmn.Header.Caption = string.Empty;
            clmn.Width = 16;

            clmn = band.Columns["FullCaption"];
            clmn.Header.VisiblePosition = visiblePosition++;
            clmn.Header.Caption = "������������ �������";
            clmn.Width = 250;

            clmn = band.Columns["UserRowCount"];
            clmn.Header.VisiblePosition = visiblePosition++;
            clmn.Header.Caption = "���������� �������";
            clmn.Width = 150;

            clmn = band.Columns["ServeRowCount"];
            clmn.Header.VisiblePosition = visiblePosition++;
            clmn.Header.Caption = "���������� ��������� �������";
            clmn.Width = 150;

            clmn = band.Columns["FullDBName"];
            clmn.Header.VisiblePosition = visiblePosition++;
            clmn.Header.Caption = "��� � ��";
            clmn.Width = 200;
            clmn.Hidden = true;

            clmn = band.Columns["Name"];
            clmn.Header.VisiblePosition = visiblePosition++;
            clmn.Header.Caption = "���������� ���";
            clmn.Width = 150;
            clmn.Hidden = true;
        }
    }
}
