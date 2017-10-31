using System;
using System.Data;
using System.Globalization;
using Microsoft.AnalysisServices.AdomdClient;

namespace Krista.FM.Server.Dashboards.Core.DataProviders
{
    //Часть класса DataProvider с реализацией методов трансформации данных
    public partial class DataProvider
    {
        
        /// <summary>
        /// Преобразование селлсета в датасет для таблицы
        /// </summary>
        private DataTable TransformForPivotTable(CellSet cls)
        {
            #warning need realization
            return null;            
        }
        
        
        #region Transform for chart
        
        private string GetSeparatorStr()
        {
            return "; ";
        }


        /// <summary>
        /// Есть серии?
        /// </summary>
        private bool SeriesExist(CellSet cls)
        {
            if (cls != null)
            {
                return (cls.OlapInfo.AxesInfo.Axes.Count > 1);
            }
            else
            {
                return false;
            }
        }        

        /// <summary>
        /// Перекачивает данные из селсета в стандартный датасет,
        /// который понимает диаграмма
        /// </summary>
        /// <param name="cls">Селлсет с данными запроса (входной формат)</param>
        /// <param name="dt">Наполняемый датасет (выходной формат)</param>
#warning этот метод надо скрыть
        public DataTable PopulateDataTableForChart(CellSet cls, DataTable dt, string SeriesFieldName)
        {
            if (cls != null)
            {
                //Если есть серии, создаем столбец для их названий
                if (SeriesExist(cls))
                {
                    DataColumn dataColumn = dt.Columns.Add();
                    dataColumn.DataType = Type.GetType("System.String");
                    dataColumn.Caption = SeriesFieldName;
                    while (dt.Columns.Contains(SeriesFieldName))
                    {
                        SeriesFieldName += ' ';
                    }
                    dataColumn.ColumnName = SeriesFieldName;
                }

                PopulateColumnsForChart(cls, dt);
                PopulateSeriesForChart(cls, dt);
                PopulateValuesForChart(cls, dt);
            }
            return dt;
        }

        /// <summary>
        /// Перекачивает названия серий из селсета в датасет
        /// </summary>
        private void PopulateSeriesForChart(CellSet cls, DataTable dt)
        {
            if (SeriesExist(cls))
            {
                string seriesName;
                int counter;

                foreach (Position pos in cls.Axes[1].Positions)
                {

                    counter = 0;
                    seriesName = string.Empty;
                    foreach (Member mem in pos.Members)
                    {
                        //серии обрабатываются всегда
                        if (true)
                        {
                            if (seriesName != string.Empty)
                            {
                                seriesName += GetSeparatorStr();
                            }
                            seriesName += mem.Caption;
                        }
                        counter++;

                    }
                    dt.Rows.Add(seriesName);
                }
            }

        }

        /// <summary>
        /// Попытка распознать тип меры
        /// </summary>
        private Type RecognizeMeasureType(CellSet cls, int posOrd)
        {
            object val;

            try
            {
                if (SeriesExist(cls))
                {
                    val = cls.Cells[posOrd, 0].Value;
                }
                else
                {
                    val = cls.Cells[posOrd].Value;
                }

                if (val != null)
                {
                    Decimal.Parse(val.ToString(), NumberStyles.Any);
                    return typeof (Decimal);
                }
                else
                {
                    // ищем первый неNULLовый элемент в столбце
                    int i = 1;
                    while (cls.Cells[posOrd, i] != null && cls.Cells[posOrd, i].Value == null)
                    {
                        i++;
                    }

                    decimal.Parse(cls.Cells[posOrd, i].Value.ToString(), NumberStyles.Any);
                    return typeof(Decimal);
                }
            }
            catch (Exception e)
            {
                return typeof(String);
            }
        }

        /// <summary>
        /// В датасете диаграммы по селсету создает столбцы (категории)
        /// Данными не заполняет, только пустые столбцы
        /// </summary>
        private void PopulateColumnsForChart(CellSet cls, DataTable dt)
        {
            DataColumn dataColumn;
            int counter;

            if (cls.OlapInfo.AxesInfo.Axes.Count > 0)
            {
                string columnName;
                foreach (Position pos in cls.Axes[0].Positions)
                {
                    counter = 0;
                    columnName = string.Empty;
                    foreach (Member mem in pos.Members)
                    {
                        if (true)
                        {
                            if (columnName != string.Empty)
                            {
                                columnName += GetSeparatorStr();
                            }
                            columnName += mem.Caption;
                        }
                        counter++;

                    }
                                                                             
                    dataColumn = dt.Columns.Add();
                    dataColumn.DataType = RecognizeMeasureType(cls, pos.Ordinal);
                    dataColumn.Caption = columnName;
                    while (dt.Columns.Contains(columnName))
                    {
                        columnName += ' ';
                    }
                    dataColumn.ColumnName = columnName;                   
                    
                }
            }
        }

        /// <summary>
        /// Записать в массив на позиции arrInd значение из селсета с индексом clsI
        /// </summary>
        private void CellValueToArray(object[] arr, int arrInd, CellSet cls, int clsX)
        {
            try
            {
                Cell curCell;
                curCell = cls.Cells[clsX];
                if (curCell != null)
                {
                    arr[arrInd] = curCell.Value;
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Записать в массив на позиции arrInd значение из селсета с индексом (clsX, clsY)
        /// </summary>
        private void CellValueToArray(object[] arr, int arrInd, CellSet cls, int clsX, int clsY)
        {
            try
            {
                Cell curCell;
                curCell = cls.Cells[clsX, clsY];
                if (curCell != null)
                {
                    arr[arrInd] = curCell.Value;
                }
            }
            catch
            {
            }
        }


        /// <summary>
        /// Перекачивает значения из селсета в датасет диаграммы
        /// </summary>
        private void PopulateValuesForChart(CellSet cls, DataTable dt)
        {
            object[] values;
            int i = 0;

            if (SeriesExist(cls))
            {
                foreach (DataRow row in dt.Rows)
                {
                    values = row.ItemArray;

                    foreach (DataColumn column in dt.Columns)
                    {
                        if (column.Ordinal != 0)
                        {
                            CellValueToArray(values, column.Ordinal, cls, column.Ordinal - 1, i);
                        }
                    }
                    row.ItemArray = values;
                    i++;
                }
            }
            else
            {
                if (cls.Cells.Count > 0)
                {
                    values = new object[dt.Columns.Count];
                    foreach (DataColumn column in dt.Columns)
                    {
                        CellValueToArray(values, column.Ordinal, cls, column.Ordinal);
                    }

                    #warning Нет обработки. см камент ниже...
                    //При занесении строки в датасет может возникнуть исключение, 
                    //если в фильтрах была задействована мера у которой не числовые
                    //значения (есть вычислимые меры со строковыми значениями).
                    //Идет исключение из-за того, что в дата-тэйбле проставлен
                    //числовой тип. Сейчас просто затраено, что делать пока не понятно.                    
                    try
                    {
                        dt.Rows.Add(values);
                    }
                    catch
                    {
                    }

                }
            }

        }                

        #endregion        
        
    }
}
