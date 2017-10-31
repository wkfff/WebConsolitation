using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI
{
    /// <summary>
    /// Тип поиска зависимых данных: вызов пользователем, или при копировании варианта.
    /// </summary>
    public enum DependedDataSearchType
    {
        CopyVariant,
        User
    }

    public partial class FrmDependedData : Form
    {
        public FrmDependedData()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Показ зависимых данных.
        /// </summary>
        /// <param name="dataObj">Объект, для которого ищем.</param>
        /// <param name="rowId">Идетификатор строки в таблице.</param>
        /// <param name="directChildRowCount">Количество подчиненных записей в самом объекте.</param>
        /// <param name="allChildRowCount">Общее количество дочерних записей.</param>
        /// <param name="searchType">Тип поиска.</param>
        /// <param name="saveFileName">Имя файла для сохранения отчета.</param>
        /// <param name="parentForm">Ссылка на родительскую форму.</param>
        public static DialogResult ShowDependedData(IEntity dataObj, int rowId, int directChildRowCount, int allChildRowCount , DependedDataSearchType searchType, string saveFileName, Form parentForm)
        {
            FrmDependedData tmpFrmDependedData = new FrmDependedData();
            // Получаем зависимые данные.
            DataSet dsDepended = dataObj.GetDependedData(rowId, false);
            // Переименовываем Cancel в OK, а саму кнопку OK прячем. DialogResult в этом случае не важен.
            tmpFrmDependedData.btnCancel.Text = "OK";
            tmpFrmDependedData.btnOk.Visible = false;
            DataTable dependedData = dsDepended.Tables[0];
            
            // Добавляем собственные зависимые данные.
            if (directChildRowCount > 0)
            {
                dependedData.Rows.Add(dataObj.GetObjectType(), dataObj.FullCaption,
                        dataObj.FullDBName, dataObj.Name, "Подчиненные записи в этом же объекте", directChildRowCount);
            }

            // Добавляем все зависимые данные.
            if (allChildRowCount > 0)
            {
                dependedData.Rows.Add(dataObj.GetObjectType(), dataObj.FullCaption,
                        dataObj.FullDBName, dataObj.Name, "Все дочерние записи в этом же объекте", allChildRowCount);
            }
            return ShowForm(dsDepended, parentForm, saveFileName, tmpFrmDependedData);
        }
        
        /// <summary>
        /// Показ зависимых данных.
        /// </summary>
        /// <param name="dataObj">Объект, для которого ищем.</param>
        /// <param name="rowId">Идетификатор строки в таблице.</param>
        /// <param name="searchType">Тип поиска.</param>
        /// <param name="saveFileName">Имя файла для сохранения отчета.</param>
        /// <param name="parentForm">Ссылка на родительскую форму.</param>
        /// <returns></returns>
        public static DialogResult ShowDependedData(IEntity dataObj, int rowId, DependedDataSearchType searchType, string saveFileName, Form parentForm)
        {
            // Если не при копировании варианта
            if (searchType != DependedDataSearchType.CopyVariant)
            {
                // будем делать просто поиск, без зависимых Parent-Child
                return ShowDependedData(dataObj, rowId, 0, 0, searchType, saveFileName, parentForm);
            }
            // Иначе ищем рекурсивно.
            return ShowDependedDataRecursive(dataObj, rowId, saveFileName, parentForm);
        }
                
        private static DialogResult ShowDependedDataRecursive(IEntity dataObj, int rowId, string saveFileName, Form parentForm)
        {
            FrmDependedData tmpFrmDependedData = new FrmDependedData();
            tmpFrmDependedData.Text = "Будут скопированы следующие записи";
            // Получаем зависимые данные.
            DataSet dsDepended = dataObj.GetDependedData(rowId, true);
            return ShowForm(dsDepended, parentForm, saveFileName, tmpFrmDependedData);
        }

        private static DialogResult ShowForm(DataSet dsDepended, Form parentForm, string saveFileName, FrmDependedData tmpFrmDependedData)
        {
            DataTable dependedData = dsDepended.Tables[0];
            // Если нашли зависимые, то отображаем форму.
            if (dependedData.Rows.Count > 0)
            {
                tmpFrmDependedData.DependedDataGridEx.StateRowEnable = true;
                tmpFrmDependedData.DependedDataGridEx.OnGridInitializeLayout +=
                    new GridInitializeLayout(tmpFrmDependedData.DependedDataGridEx_OnGridInitializeLayout);
                tmpFrmDependedData.DependedDataGridEx.OnInitializeRow +=
                    new InitializeRow(tmpFrmDependedData.DependedDataGridEx_OnInitializeRow);
                tmpFrmDependedData.DependedDataGridEx._utmMain.ToolClick +=
                    new ToolClickEventHandler(tmpFrmDependedData._utmMain_ToolClick);

                tmpFrmDependedData.DependedDataGridEx.DataSource = dependedData;
                InfragisticComponentsCustomize.CustomizeUltraGridParams(tmpFrmDependedData.DependedDataGridEx._ugData);

                tmpFrmDependedData.DependedDataGridEx.SaveLoadFileName = string.Format("{0}_Зависимые данные", saveFileName);
                tmpFrmDependedData.DependedDataGridEx.MaximumSize = new Size(0, 0);

                tmpFrmDependedData.ShowDialog(parentForm);
            }
                // Если не нашли зависимые, то выводим сообщение.
            else
            {
                MessageBox.Show("Зависимые данные не найдены.", "Информация", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
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
                case "Count":
                case "AssociationType":
                case "ObjectType":
                {
                    DependedDataGridEx._ugData.DisplayLayout.Bands[0].Columns[e.Tool.Key].Hidden =
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
        void DependedDataGridEx_OnInitializeRow(object sender, InitializeRowEventArgs e)
        {
            UltraGridRow row = e.Row;

            // Если грид не плоский, и номер группы не соответствует глубине строки
            if (DependedDataGridEx.ugData.DisplayLayout.Bands.Count > 1 && e.Row.Band.Index != Convert.ToInt32(e.Row.Cells["Depth"].Value))
            {
                // скрываем строку.
                e.Row.Hidden = true;
            }

            UltraGridCell cell = row.Cells["ObjectType"];

            cell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
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
                case "Сопоставимый классификатор":
                    return Krista.FM.Client.ViewObjects.AssociatedCLSUI.Properties.Resources.bridgeCls;
                case "Классификатор данных":
                    return Krista.FM.Client.ViewObjects.AssociatedCLSUI.Properties.Resources.kd;
                case "Таблица фактов":
                    return Krista.FM.Client.ViewObjects.AssociatedCLSUI.Properties.Resources.factCls;
                case "Фиксированный классификатор":
                    return Krista.FM.Client.ViewObjects.AssociatedCLSUI.Properties.Resources.fixedCls;
                default :
                    return Krista.FM.Client.ViewObjects.AssociatedCLSUI.Properties.Resources.tableCls;
            }
        }

        void DependedDataGridEx_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            foreach (UltraGridBand band in e.Layout.Bands)
            {
                UltraGridColumn clmn = band.Columns["FullCaption"];
                clmn.Header.VisiblePosition = 1;
                clmn.Header.Caption = "Наименование объекта";
                clmn.Width = 150;

                clmn = band.Columns["FullDBName"];
                clmn.Header.VisiblePosition = 2;
                clmn.Header.Caption = "Имя в БД";
                clmn.Width = 200;
                clmn.Hidden = true;

                clmn = band.Columns["Name"];
                clmn.Header.VisiblePosition = 3;
                clmn.Header.Caption = "Английское имя";
                clmn.Width = 150;
                clmn.Hidden = true;

                clmn = band.Columns["Count"];
                clmn.Header.VisiblePosition = 4;
                clmn.Header.Caption = "Количество записей";
                clmn.Width = 100;
                clmn.SortIndicator = SortIndicator.Descending;

                clmn = band.Columns["AssociationType"];
                clmn.Header.VisiblePosition = 5;
                clmn.Header.Caption = "Тип ассоциации";
                clmn.Width = 250;

                clmn = band.Columns["ObjectType"];
                clmn.Header.VisiblePosition = 0;
                clmn.Header.Caption = string.Empty;
                clmn.Width = 16;

                
                if (band.Columns.Exists("ResultRowID"))
                {
                    clmn = band.Columns["ResultRowID"];
                    clmn.Header.VisiblePosition = 0;
                    clmn.Header.Caption = string.Empty;
                    clmn.Hidden = true;
                    clmn.Width = 16;
                }

                if (band.Columns.Exists("ParentID"))
                {
                    clmn = band.Columns["ParentID"];
                    clmn.Header.VisiblePosition = 0;
                    clmn.Header.Caption = string.Empty;
                    clmn.Hidden = true;
                    clmn.Width = 16;
                }
                
                if (band.Columns.Exists("Depth"))
                {
                    clmn = band.Columns["Depth"];
                    clmn.Header.VisiblePosition = 0;
                    clmn.Header.Caption = string.Empty;
                    clmn.Hidden = true;
                    clmn.Width = 16;
                }
            }
        }
    }
}
