﻿using System;
using System.IO;
using System.Windows.Forms;
using System.Data;
using System.Reflection;
using System.Collections;
using Microsoft.Office.Interop.Excel;
using Microsoft.VisualBasic;
using DataTable = System.Data.DataTable;

namespace Krista.FM.Update.Framework.Controls
{
    public class ExportToExcel
    {
        /// <summary>
        /// Exporta la informaciуn de un dataGridView a Excel
        /// </summary>
        /// <param name="dataGridView">DataGridView de origen</param>
        /// <param name="pFullPath_toExport">Ruta del archivo exportado</param>
        /// <param name="nameSheet">Nombre de la hoja</param>
        /// <param name="showExcel">Mostrar excel?</param>
        public void DataGridView2Excel(DataGridView dataGridView, string pFullPath_toExport, string nameSheet)
        {
            Object obj = dataGridView.DataSource;
            DataTable dt = new DataTable();

            //Obtener un datatable del datagridview
            if (dataGridView.DataSource is DataSet)
            {
                if (((DataSet)dataGridView.DataSource).Tables.Count > 0)
                    dt = ((DataSet)dataGridView.DataSource).Tables[0];
                else
                    dt = new System.Data.DataTable();
            }
            else if (dataGridView.DataSource is System.Data.DataTable)
            {
                dt = (System.Data.DataTable)dataGridView.DataSource;
            }
            else if (dataGridView.DataSource is ArrayList)
            {
                ArrayList arr = (ArrayList)dataGridView.DataSource;
                dt = ArrayListToDataTable(arr);
            }
            else if (dataGridView.DataSource == null)
            {
                dt = RowsToDataTable(dataGridView);
            }

            DataTable2Excel(dt, dataGridView, pFullPath_toExport, nameSheet);
        }
        

        /// <summary>
        /// Exporta la informaciуn de un DataTable a Excel
        /// </summary>
        /// <param name="pDataTable">DataTable de origen</param>
        /// <param name="dgv">DataGridView de origen (solo para tomar los titulos de las columnas y determinar las columnas a mostrar)</param>
        /// <param name="pFullPath_toExport">Ruta a exportar</param>
        /// <param name="nameSheet">Nombre de la hoja</param>
        /// <param name="showExcel">Mostrar excel?</param>
        public void DataTable2Excel(System.Data.DataTable pDataTable, DataGridView dgv, string pFullPath_toExport, string nameSheet)
        {
            string vFileName = Path.GetTempFileName();
            FileSystem.FileOpen(1, vFileName, OpenMode.Output, OpenAccess.Default, OpenShare.Default, -1);

            string sb = string.Empty;
            //si existe datagridview, tomar de йl los nombres de columnas y la visibilidad de las mismas
            if (dgv != null)
            {
                foreach (DataColumn dc in pDataTable.Columns)
                {
                    System.Windows.Forms.Application.DoEvents();
                    string title = string.Empty;

                    //recuperar el tнtulo que aparece en la grilla
                    //Notar que debe haber sincronнa con las columnas del detalle
                    if (dgv.Columns[dc.Caption] != null)
                    {
                        //Obtener el texto de cabecera de la grilla
                        title = dgv.Columns[dc.Caption].HeaderText;
                        sb += title + ControlChars.Tab;
                    }
                }
            }
            else
            {
                //si no existe datagridview tomar el nombre de la columna del datatable
                foreach (DataColumn dc in pDataTable.Columns)
                {
                    System.Windows.Forms.Application.DoEvents();
                    string title = string.Empty;

                    title = dc.Caption;
                    sb += title + ControlChars.Tab;

                }
            }

            FileSystem.PrintLine(1, sb);

            int i = 0;
            //para cada fila de datos
            foreach (DataRow dr in pDataTable.Rows)
            {
                System.Windows.Forms.Application.DoEvents();
                i = 0;
                sb = string.Empty;
                //para cada columna de datos
                foreach (DataColumn dc in pDataTable.Columns)
                {
                    //solo mostrar aquellas columnas q pertenezcan a la grilla
                    //notar que debe haber sincronia con las columnas del la cabecera
                    if (dgv != null && dgv.Columns[dc.Caption] != null)
                    {
                        System.Windows.Forms.Application.DoEvents();
                        //Linea q genera la impresiуn del registro
                        sb = sb + (Information.IsDBNull(dr[i]) ? string.Empty : FormatCell(dr[i])) + ControlChars.Tab;

                    }
                    else if (dgv == null)
                    {
                        System.Windows.Forms.Application.DoEvents();
                        //Linea q genera la impresiуn del registro
                        sb = sb + (Information.IsDBNull(dr[i]) ? string.Empty : FormatCell(dr[i])) + ControlChars.Tab;
                    }
                    i++;
                }
                FileSystem.PrintLine(1, sb);
            }
            FileSystem.FileClose(1);
            TextToExcel(vFileName, pFullPath_toExport, nameSheet);
        }

        /// <summary>
        /// Limpieza de caracteres de la celda a exportar
        /// </summary>
        /// <param name="cell">Celda del datarow a formatear</param>
        /// <returns>cadena formateada</returns>
        private static string FormatCell(Object cell)
        {
            string TextToParse = Convert.ToString(cell);
            return TextToParse.Replace(",", string.Empty);
        }

        /// <summary>
        /// Exporta un determinado texto en cadena a excel
        /// </summary>
        /// <param name="pFileName">Filename del archivo exportado</param>
        /// <param name="pFullPath_toExport">Ruta del archivo exportado</param>
        /// <param name="nameSheet">nombre de la hoja</param>
        /// <param name="showExcel">Mostrar excel?</param>
        private static void TextToExcel(string pFileName, string pFullPath_toExport, string nameSheet)
        {
            System.Globalization.CultureInfo vCultura = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            Microsoft.Office.Interop.Excel.Application Exc = new Microsoft.Office.Interop.Excel.Application();
            Exc.Workbooks.OpenText(pFileName, Missing.Value, 1,
                XlTextParsingType.xlDelimited,
                XlTextQualifier.xlTextQualifierNone,
                Missing.Value, Missing.Value,
                Missing.Value, true,
                Missing.Value, Missing.Value,
                Missing.Value, Missing.Value,
                Missing.Value, Missing.Value,
                Missing.Value, Missing.Value, Missing.Value);

            Workbook Wb = Exc.ActiveWorkbook;
            Worksheet Ws = (Worksheet)Wb.ActiveSheet;
            Ws.Name = nameSheet;

            try
            {
                //Formato de cabecera
                Ws.get_Range(Ws.Cells[1, 1], Ws.Cells[Ws.UsedRange.Rows.Count, Ws.UsedRange.Columns.Count]).AutoFormat(XlRangeAutoFormat.xlRangeAutoFormatClassic1, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
            }
            catch
            {
                Ws.get_Range(Ws.Cells[1, 1], Ws.Cells[Ws.UsedRange.Rows.Count, Ws.UsedRange.Columns.Count]);
            }

            string tempPath = Path.GetTempFileName();

            pFileName = tempPath.Replace("tmp", "xls");
            File.Delete(pFileName);

            if (File.Exists(pFullPath_toExport))
            {
                File.Delete(pFullPath_toExport);
            }
            Exc.ActiveWorkbook.SaveAs(pFullPath_toExport, 1, null, null, null, null, XlSaveAsAccessMode.xlNoChange, null, null, null, null, null);

            Exc.Workbooks.Close();

            System.Runtime.InteropServices.Marshal.ReleaseComObject(Ws);

            System.Runtime.InteropServices.Marshal.ReleaseComObject(Wb);

            Exc.Quit();

            System.Runtime.InteropServices.Marshal.ReleaseComObject(Exc);
            Exc = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            System.Threading.Thread.CurrentThread.CurrentCulture = vCultura;

        }

        private static DataTable RowsToDataTable(DataGridView dataGrid)
        {
            DataTable dt = new DataTable();
            if (dataGrid.Rows.Count > 0)
            {
                foreach (DataGridViewColumn dataGridViewColumn in dataGrid.Columns)
                {
                    dt.Columns.Add(dataGridViewColumn.Name, typeof(string));
                }
            }

            foreach (DataGridViewRow dataGridViewRow in dataGrid.Rows)
            {
                DataRow dr = dt.NewRow();

                int i = 0;
                foreach (DataGridViewCell dataGridViewCell in dataGridViewRow.Cells)
                {
                    dr[i] = dataGridViewCell.Value;
                    i++;
                }

                dt.Rows.Add(dr);
            }

            return dt;
        }

        /// <summary>
        /// Convierte un arraylist de objetos en un datatable a partir de las 'propiedades' del arraylist
        /// </summary>
        /// <param name="array">Arraylist de objetos</param>
        /// <returns>DataTable de salida</returns>
        public static System.Data.DataTable ArrayListToDataTable(ArrayList array)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            if (array.Count > 0)
            {
                object obj = array[0];
                //Convertir las propiedades del objeto en columnas del datarow
                foreach (PropertyInfo info in obj.GetType().GetProperties())
                {
                    dt.Columns.Add(info.Name, info.PropertyType);
                }
            }
            foreach (object obj in array)
            {
                DataRow dr = dt.NewRow();
                foreach (DataColumn col in dt.Columns)
                {
                    Type type = obj.GetType();

                    MemberInfo[] members = type.GetMember(col.ColumnName);

                    object valor;
                    if (members.Length != 0)
                    {
                        switch (members[0].MemberType)
                        {
                            case MemberTypes.Property:
                                //leer las propiedades del objeto
                                PropertyInfo prop = (PropertyInfo)members[0];
                                try
                                {
                                    valor = prop.GetValue(obj, new object[0]);
                                }
                                catch
                                {
                                    valor = prop.GetValue(obj, null);
                                }

                                break;
                            case MemberTypes.Field:
                                //leer los campos del objeto (no se usa 
                                //dado q hemos poblado el dt con las propiedades del arraylist)
                                FieldInfo field = (FieldInfo)members[0];
                                valor = field.GetValue(obj);
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                        dr[col] = valor;
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public static string Readcell(Range oRange)
        {
            String result = string.Empty;
            if (oRange != null)
            {
                if (oRange.Text != null)
                {
                    result = oRange.Text.ToString();
                }
            }
            return result;
        }

    }
}