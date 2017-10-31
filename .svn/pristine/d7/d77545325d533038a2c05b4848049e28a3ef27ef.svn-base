using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using System.Collections;


using Krista.FM.Server.ProcessorLibrary;
using System.Diagnostics;


namespace Krista.FM.Client.ViewObjects.MDObjectsManagementUI
{
    //internal static class ViewerUtils
    //{
    //    /// <summary>
    //    /// Возврашает суммарную ширину отображаемых колонок.
    //    /// </summary>
    //    /// <returns>Суммарная ширина отображаемых колонок.</returns>
    //    public static int GetRowWidth(DataGridView grid)
    //    {
    //        int columnsWidth = 0;
    //        foreach (DataGridViewColumn column in grid.Columns)
    //        {
    //            if (column.Visible)
    //            {
    //                columnsWidth += column.Width;
    //            }
    //        }
    //        return columnsWidth;
    //    }

    //    public static Brush GetAnalysisStateBackBrush(Microsoft.AnalysisServices.AnalysisState state, bool isActual)
    //    {
    //        switch (state)
    //        {
    //            case Microsoft.AnalysisServices.AnalysisState.PartiallyProcessed:                    
    //            case Microsoft.AnalysisServices.AnalysisState.Processed:
    //                if (isActual)
    //                {
    //                    return Brushes.LightGreen;
    //                }
    //                else
    //                {
    //                    return Brushes.LightYellow;
    //                }                    
    //            default:
    //            case Microsoft.AnalysisServices.AnalysisState.Unprocessed:
    //                return Brushes.White;
    //        }
    //    }

    //    public static Brush GetRecordStatusBackBrush(Krista.FM.Server.ProcessorLibrary.RecordStatus recordStatus)
    //    {
    //        switch (recordStatus)
    //        {
    //            default:
    //            case Krista.FM.Server.ProcessorLibrary.RecordStatus.waiting:
    //                return Brushes.White;
    //            case Krista.FM.Server.ProcessorLibrary.RecordStatus.inBatch:
    //                return Brushes.LightYellow;
    //            case Krista.FM.Server.ProcessorLibrary.RecordStatus.inProcess:
    //                return Brushes.Yellow;
    //            case Krista.FM.Server.ProcessorLibrary.RecordStatus.complitedSuccessful:
    //                return Brushes.LightGreen;
    //            case Krista.FM.Server.ProcessorLibrary.RecordStatus.complitedWithErrors:
    //                return Brushes.Pink;
    //            case Krista.FM.Server.ProcessorLibrary.RecordStatus.canceledByUser:
    //                return Brushes.Gray;
    //            case Krista.FM.Server.ProcessorLibrary.RecordStatus.canceledByOptimization:
    //                return Brushes.LightGray;
    //        }
    //    }

    //    public static void grid_RowPrePaint(DataGridView grid, DataGridViewRowPrePaintEventArgs e, Brush backBrush)
    //    {
    //        if ((e.PaintParts & DataGridViewPaintParts.Background) == DataGridViewPaintParts.Background &&
    //            (e.State & DataGridViewElementStates.Visible) == DataGridViewElementStates.Visible)
    //        {
    //            Rectangle bounds = new Rectangle(e.RowBounds.X, e.RowBounds.Y, GetRowWidth(grid), e.RowBounds.Height);
    //            e.Graphics.FillRectangle(backBrush, bounds);
    //        }
    //    }

    //    public static Microsoft.AnalysisServices.AnalysisState GetAnalysisState(DataGridView grid, int rowIndex, int columnIndex)
    //    {
    //        if (!grid.Rows[rowIndex].IsNewRow &&
    //            grid.Rows[rowIndex].Cells[columnIndex].Value != null)
    //        {
    //            return (Microsoft.AnalysisServices.AnalysisState)Convert.ToInt32(grid.Rows[rowIndex].Cells[columnIndex].Value);
    //        }
    //        return Microsoft.AnalysisServices.AnalysisState.Unprocessed;
    //    }

    //    public static bool GetIsActual(DataGridView grid, int rowIndex, int columnIndex)
    //    {
    //        if (!grid.Rows[rowIndex].IsNewRow &&
    //            grid.Rows[rowIndex].Cells[columnIndex].Value != null)
    //        {
    //            return Convert.ToBoolean(grid.Rows[rowIndex].Cells[columnIndex].Value);
    //        }
    //        return true;
    //    }

    //    public static Krista.FM.Server.ProcessorLibrary.RecordStatus GetRecordStatus(DataGridView grid, int rowIndex, int columnIndex)
    //    {
    //        if (!grid.Rows[rowIndex].IsNewRow &&
    //            grid.Rows[rowIndex].Cells[columnIndex].Value != null)
    //        {
    //            return (Krista.FM.Server.ProcessorLibrary.RecordStatus)grid.Rows[rowIndex].Cells[columnIndex].Value;
    //        }
    //        return Krista.FM.Server.ProcessorLibrary.RecordStatus.waiting;
    //    }

    //    /// <summary>
    //    /// Возвращает список записей датасета, соответствующих выбранным строкам грида.
    //    /// </summary>
    //    /// <param name="gridRows">Коллекция строк грида.</param>
    //    /// <returns>Список записей датасета.</returns>
    //    public static List<DataRow> GetDataRows(IEnumerable gridRows)
    //    {
    //        List<DataRow> selectedRows = new List<DataRow>();
    //        foreach (DataGridViewRow gridRow in gridRows)
    //        {
    //            selectedRows.Add(GetDataRow(gridRow));                    
    //        }
    //        return selectedRows;
    //    }

    //    /// <summary>
    //    /// Возвращает запись датасета, соответствующую строчке грида.
    //    /// </summary>
    //    /// <param name="gridRows">Строчка грида.</param>
    //    /// <returns>Запись датасета.</returns>
    //    public static DataRow GetDataRow(DataGridViewRow gridRow)
    //    {            
    //        if (null != gridRow && null != gridRow.DataBoundItem)
    //        {
    //            return ((DataRowView)gridRow.DataBoundItem).Row;
    //        }
    //        return null;
    //    }        
    //}
}
