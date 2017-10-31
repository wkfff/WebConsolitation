using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Diagnostics;

using Krista.FM.Server.ProcessorLibrary;
using System.Collections;

namespace Krista.FM.Client.ViewObjects.MDObjectsManagementUI
{
    public partial class FormProcessOptions : Form
    {
        //private OptionsResult optionsResult = OptionsResult.cancel;
        //private DataView dataViewSelected;
        
        //private DataSetOlapBaseUtils dataSetOlapBaseUtils;

        /// <summary>
        /// Используется для вызовов функций GetCubeDimensions и GetCubePartitons
        /// </summary>
        /// <param name="cubeId">Идентификатор куба.</param>
        /// <returns>Список элементов, подчиненных данному кубу.</returns>
        //private delegate List<DataRow> GetRelatedItemsDelegate(string cubeId);


        //public FormProcessOptions()
        //{
        //    InitializeComponent();
        //    gridObjects.AutoGenerateColumns = false;
        //}

        public FormProcessOptions(/*DataSetOlapBaseUtils dataSetOlapBaseUtils*/)
        {
            InitializeComponent();
            //gridObjects.AutoGenerateColumns = false;            
            //Init(dataSetOlapBaseUtils);
        }

        //public void Init(DataSetOlapBaseUtils dataSetOlapBaseUtils)
        //{            
        //    this.dataSetOlapBaseUtils = dataSetOlapBaseUtils;
        //    dataViewSelected = this.dataSetOlapBaseUtils.DataSetOlapBase.DefaultViewManager.CreateDataView(
        //        dataSetOlapBaseUtils.DataTableOlapObjects);
        //    dataViewSelected.RowFilter = string.Format("Selected = {0}", true);
        //    gridObjects.DataSource = dataViewSelected;
        //}

        //private bool Inited
        //{
        //    get { return dataSetOlapBaseUtils != null; }
        //}

        //public OptionsResult ShowOptions(
        //    IWin32Window owner, IEnumerable gridRows, OlapObjectType objectType, out DataSet dsSelectedObjects)
        //{
        //    //Очищаем объекты, которые были выбраны на предыдущем шаге.
        //    foreach (DataRow dataRow in this.dataSetOlapBaseUtils.DataTableOlapObjects.Rows)
        //    {
        //        dataRow["Selected"] = false;
        //    }

        //    //Получаем записи датасета, соответствующие выбранным записям грида.
        //    List<DataRow> dataRows = ViewerUtils.GetDataRows(gridRows);
            
        //    //Если записи относятся к кубам, то ищем и добавляем записи разделов этих кубов.
        //    //Сами записи кубов не добавляем.
        //    if (OlapObjectType.cube == objectType)
        //    {
        //        //В этот список поместим записи разделов.
        //        List<DataRow> partitionRows = new List<DataRow>();

        //        //Проходим по кубам и выбираем для каждого разделы.
        //        foreach (DataRow dataRow in dataRows)
        //        {
        //            partitionRows.AddRange(
        //                dataSetOlapBaseUtils.GetCubePartitions((string)dataRow[DataSetOlapBaseUtils.FieldName_Id]));
        //        }
                
        //        //Подменяем список кубов на список разделов этих кубов.
        //        dataRows = partitionRows;
        //    }

        //    foreach (DataRow dataRow in dataRows)
        //    {
        //        AddRow(dataRow);
        //    }

        //    ShowDialog(owner);

        //    dsSelectedObjects = new DataSet();
        //    dsSelectedObjects.Tables.Add("SelectedObjects");

        //    return optionsResult;
        //}

        //private void AddRow(DataRow dataRow)
        //{
        //    dataRow["Selected"] = true;
        //    SetProcessType(dataRow, ProcessType.ProcessFull);            
        //}

        ////public OptionsResult ShowOptions(
        ////    IWin32Window owner, IEnumerable gridRows, OlapObjectType objectType, out DataSet dsSelectedObjects)
        ////{
        ////    InitSelectedObjectsDataSet();
        ////    dsSelectedObjects = this.dsSelectedObjects;
        ////    if (!Inited) return OptionsResult.cancel;
        ////    //Форма у нас создается один раз - при повторных вызовах очищаем содержимое.
        ////    tableSelectedObjects.Clear();

        ////    //Получаем записи датасета, соответствующие выбранным записям грида.
        ////    List<DataRow> dataRows = ViewerUtils.GetDataRows(gridRows);

        ////    //Если записи относятся к кубам, то ищем и добавляем записи разделов этих кубов.
        ////    //Сами записи кубов не добавляем.
        ////    if (OlapObjectType.cube == objectType)
        ////    {
        ////        //В этот список помести записи разделов.
        ////        List<DataRow> partitionRows = new List<DataRow>();

        ////        //Проходим по кубам и выбираем для каждого разделы.
        ////        foreach (DataRow dataRow in dataRows)
        ////        {
        ////            partitionRows.AddRange(
        ////                olapBaseUtils.GetCubePartitions((string)dataRow[OlapBaseUtils.FieldName_Id]));
        ////        }

        ////        //Подменяем список кубов на список разделов этих кубов.
        ////        dataRows = partitionRows;
        ////    }

        ////    //Для каждой выбранной записи из датасета многомерной базы создаем записи в датасете
        ////    foreach (DataRow dataRow in dataRows)
        ////    {
        ////        AddSelectedObject(dataRow);
        ////    }
        ////    //TODO Почему-то после импорта DataRow эта колонка (ObjectType) становится видимой - разобраться!
        ////    columnObjectType.Visible = false;

        ////    ShowDialog(owner);
        ////    return optionsResult;
        ////}        

        //private void SetProcessType(DataRow dataRow, ProcessType processType)
        //{
        //    OlapObjectType objectType = (OlapObjectType)dataRow["ObjectType"];
        //    dataRow["ProcessType"] = (int)CheckProcessType(objectType, processType);
        //}

        ///// <summary>
        ///// Близкие по смыслу виды расчета для разных типов объектов могут называться по разному.
        ///// Функция, в зависимости от типа объекта, возвращает соответствующий ему тип расчета.
        ///// </summary>
        ///// <param name="objectType">Тип объекта.</param>
        ///// <param name="processType">Исходный тип расчета.</param>
        ///// <returns>Тип расчета, модифицированный по типу объекта.</returns>
        //private static ProcessType CheckProcessType(OlapObjectType objectType, ProcessType processType)
        //{
        //    if (ProcessType.ProcessAdd == processType && OlapObjectType.dimension == objectType)
        //    {
        //        return ProcessType.ProcessUpdate;
        //    }
        //    else
        //    {
        //        if (ProcessType.ProcessUpdate == processType && OlapObjectType.partition == objectType)
        //        {
        //            return ProcessType.ProcessAdd;
        //        }
        //        else
        //        {
        //            return processType;
        //        }
        //    }            
        //}

        ///// <summary>
        ///// Возвращает тип объекта.
        ///// </summary>
        ///// <param name="gridRow">Запись в гриде.</param>
        ///// <returns>Тип объекта.</returns>
        //private OlapObjectType GetObjectType(DataGridViewRow gridRow)
        //{
        //    return (OlapObjectType)gridRow.Cells[columnObjectType.Index].Value;
        //}

        ///// <summary>
        ///// Возвращает идентификатор объекта.
        ///// </summary>
        ///// <param name="gridRow">Строка града с полями объекта.</param>
        ///// <returns>Идентификатор объекта.</returns>
        //private string GetObjectId(DataGridViewRow gridRow)
        //{
        //    return (string)gridRow.Cells[columnObjectId.Index].Value;
        //}

        ///// <summary>
        ///// Возвращает идентификатор куба.
        ///// </summary>
        ///// <param name="gridRow">Строка грида с полями объекта.</param>
        ///// <returns>Возвращает идентификатор куба.</returns>
        //private string GetParentId(DataGridViewRow gridRow)
        //{
        //    return (string)gridRow.Cells[columnParentId.Index].Value;
        //}        

        ///// <summary>
        ///// Получает тип объекта.
        ///// </summary>
        ///// <param name="dataRow">Запись объекта.</param>
        //private OlapObjectType GetObjectType(DataRow dataRow)            
        //{   
        //    if (null != dataRow)
        //    {
        //        return (OlapObjectType)dataRow[columnObjectType.Index];                
        //    }
        //    return OlapObjectType.dimension;
        //}        

        //private void AddRelatedItems(GetRelatedItemsDelegate getItems)
        //{
        //    if (gridObjects.SelectedRows.Count == 0) return;
        //    DataGridViewRow gridRow = gridObjects.SelectedRows[0];
        //    if (GetObjectType(gridRow) == OlapObjectType.partition)
        //    {
        //        List<DataRow> items = getItems(GetParentId(gridRow));
        //        foreach (DataRow dataRow in items)
        //        {
        //            AddRow(dataRow);
        //        }                
        //    }
        //}        

        //private void menuItemDelete_Click(object sender, EventArgs e)
        //{
        //    if (MessageBox.Show(this, string.Format(
        //        "Вы уверены, что хотите удалить {0} объект(а,ов)?", gridObjects.SelectedRows.Count),
        //        "Подтверждение удаления",
        //        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        //    {
        //        DeleteSelected();
        //    }
        //}

        //private void DeleteSelected()
        //{
        //    List<DataRow> selectedRows = ViewerUtils.GetDataRows(gridObjects.SelectedRows);
        //    foreach (DataRow dataRow in selectedRows)
        //    {
        //        dataRow["Selected"] = false;
        //    }
        //}

        //private void SetProcessType(ProcessType processType)
        //{
        //    List<DataRow> selectedRows = ViewerUtils.GetDataRows(gridObjects.SelectedRows);
        //    foreach (DataRow dataRow in selectedRows)
        //    {
        //        SetProcessType(dataRow, processType);
        //    }            
        //}

        //private void menuItemProcessFull_Click(object sender, EventArgs e)
        //{
        //    SetProcessType(ProcessType.ProcessFull);
        //}

        //private void menuItemProcessData_Click(object sender, EventArgs e)
        //{
        //    SetProcessType(ProcessType.ProcessData);
        //}

        //private void menuItemProcessIncremental_Click(object sender, EventArgs e)
        //{
        //    SetProcessType(ProcessType.ProcessAdd);
        //}

        //private void btnRunProcess_Click(object sender, EventArgs e)
        //{
        //    optionsResult = OptionsResult.addToAccumulatorAndProcess;
        //    this.Hide();
        //}

        //private void btnClose_Click(object sender, EventArgs e)
        //{
        //    optionsResult = OptionsResult.cancel;
        //    this.Hide();
        //}

        //private void btnAddToAccumulator_Click(object sender, EventArgs e)
        //{
        //    optionsResult = OptionsResult.addToAccumulator;
        //    this.Hide();
        //}

        //private void menuItemAddPartitions_Click(object sender, EventArgs e)
        //{
        //    AddRelatedItems(dataSetOlapBaseUtils.GetCubePartitions);            
        //}

        //private void menuItemAddDimensions_Click(object sender, EventArgs e)
        //{
        //    AddRelatedItems(dataSetOlapBaseUtils.GetCubeDimensions);            
        //}

        //private void gridObjects_RowEnter(object sender, DataGridViewCellEventArgs e)
        //{
        //    DataGridViewRow gridRow = gridObjects.Rows[e.RowIndex];
        //    if (null != gridRow && !gridRow.IsNewRow)
        //    {                
        //        OlapObjectType objectType = GetObjectType(gridRow);
        //        switch (objectType)
        //        {
        //            case OlapObjectType.dataBase:
        //            case OlapObjectType.cube:
        //            case OlapObjectType.measureGroup:
        //            case OlapObjectType.dimension:
        //                menuItemAddPartitions.Enabled = false;
        //                menuItemAddDimensions.Enabled = false;
        //                break;
        //            case OlapObjectType.partition:
        //                menuItemAddPartitions.Enabled = true;
        //                menuItemAddDimensions.Enabled = true;
        //                break;
        //        }
        //    }           
        //}
    }

    public enum OptionsResult
    {
        cancel = 0,
        addToAccumulator = 1,
        addToAccumulatorAndProcess = 2,
    }
}