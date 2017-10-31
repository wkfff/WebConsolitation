using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using ColumnStyle=Infragistics.Win.UltraWinGrid.ColumnStyle;

namespace Krista.FM.Client.ViewObjects.DataSourcesUI
{
    public partial class FrmSourceDeleteError : Form
    {
        public FrmSourceDeleteError()
        {
            InitializeComponent();
        }

        public static void ShowErrorData(DataTable errorData, int sourceID, Form parentForm)
        {
            FrmSourceDeleteError tmpFrmSourceDeleteError = new FrmSourceDeleteError();
            tmpFrmSourceDeleteError.ultraGridEx.StateRowEnable = true;
            tmpFrmSourceDeleteError.ultraGridEx.OnGridInitializeLayout += new GridInitializeLayout(tmpFrmSourceDeleteError.ultraGridEx_OnGridInitializeLayout);
            tmpFrmSourceDeleteError.ultraGridEx._utmMain.ToolClick += new ToolClickEventHandler(tmpFrmSourceDeleteError._utmMain_ToolClick);
            tmpFrmSourceDeleteError.ultraGridEx.OnInitializeRow += new InitializeRow(tmpFrmSourceDeleteError.ultraGridEx_OnInitializeRow);
            
            tmpFrmSourceDeleteError.ultraGridEx.DataSource = errorData;
            InfragisticComponentsCustomize.CustomizeUltraGridParams(tmpFrmSourceDeleteError.ultraGridEx._ugData);

            tmpFrmSourceDeleteError.ultraGridEx.SaveLoadFileName = string.Format("Источник ID = {0}_Ошибка при удалении", sourceID);
            tmpFrmSourceDeleteError.ultraGridEx.MaximumSize = new Size(0, 0);
            
            tmpFrmSourceDeleteError.ShowDialog(parentForm);
        }

        void ultraGridEx_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGridBand band = e.Layout.Bands[0];

            UltraGridColumn clmn = band.Columns["FullCaption"];
            clmn.Header.VisiblePosition = 1;
            clmn.Header.Caption = "Наименование объекта";
            clmn.Width = 250;

            clmn = band.Columns["FullDBName"];
            clmn.Header.VisiblePosition = 2;
            clmn.Header.Caption = "Имя в БД";
            clmn.Width = 200;
            clmn.Hidden = true;

            clmn = band.Columns["ID"];
            clmn.Header.VisiblePosition = 3;
            clmn.Header.Caption = "ID записи";
            clmn.Width = 150;

            clmn = band.Columns["ObjectType"];
            clmn.Header.VisiblePosition = 0;
            clmn.Header.Caption = string.Empty;
            clmn.Width = 16;
        }

        void _utmMain_ToolClick(object sender, ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "FullCaption":
                case "FullDBName":
                case "ID":
                    {
                        ultraGridEx._ugData.DisplayLayout.Bands[0].Columns[e.Tool.Key].Hidden =
                                !((StateButtonTool)e.Tool).Checked;
                        break;
                    }
            }
        }

        /// <summary>
        /// При инициализации строки добавляем иконку типа объекта.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ultraGridEx_OnInitializeRow(object sender, InitializeRowEventArgs e)
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
        /// По типу объекта возвращает соответствующую иконку.
        /// </summary>
        /// <param name="val">Тип объекта.</param>
        /// <returns>Иконка.</returns>
        private Image GetPicByType(string val)
        {
            switch (val)
            {
                case "Классификатор данных":
                    return Krista.FM.Client.ViewObjects.DataSourcesUI.Properties.Resources.kd;
                default:
                    return Krista.FM.Client.ViewObjects.DataSourcesUI.Properties.Resources.factCls;
            }
        }

    }
}