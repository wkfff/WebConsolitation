using System;
using System.IO;
using System.Reflection;
using System.Data;
using System.Runtime.InteropServices;
using Krista.FM.Common;

namespace Krista.FM.Server.DataPumps.DataAccess
{

    /// <summary>
    /// ����� ������ ������
    /// </summary>
    public class ExcelCellFont
    {
        /// <summary>
        /// ������
        /// </summary>
        public bool Bold;

        /// <summary>
        /// ���������
        /// </summary>
        public bool Italic;

        /// <summary>
        /// ������������
        /// </summary>
        public string Name;

        /// <summary>
        /// ������
        /// </summary>
        public double Size;

        /// <summary>
        /// �����������
        /// </summary>
        public bool Strikeout;

        /// <summary>
        /// ������������
        /// </summary>
        public bool Underline;
    }

    /// <summary>
    /// ������ ������
    /// </summary>
    public class ExcelCell
    {
        /// <summary>
        /// �����
        /// </summary>
        public ExcelCellFont Font;

        /// <summary>
        /// ��������
        /// </summary>
        public string Value;
    }

    /// <summary>
    /// ������ ������� excel ������
    /// </summary>
    public enum ExcelBorderStyles
    {
        DiagonalDown = 5,
        DiagonalUp = 6,
        EdgeLeft = 7,
        EdgeTop = 8,
        EdgeBottom = 9,
        EdgeRight = 10,
        InsideVertical = 11,
        InsideHorizontal = 12,
    }

    /// <summary>
    /// ����� ������� ������
    /// </summary>
    public enum ExcelLineStyles
    {
        LineStyleNone = -4142,
        Double = -4119,
        Dot = -4118,
        Dash = -4115,
        Continuous = 1,
        DashDot = 4,
        DashDotDot = 5,
        SlantDashDot = 13,
    }

    /// <summary>
    /// ����� ��� ������ � MS Excel
    /// </summary>
    public class ExcelHelper : DisposableObject
    {

        #region ����

        public bool skipHiddenRows = false;

        private object excelObj = null;
        private object workbooks = null;
        private object workbook = null;
        private object worksheets = null;
        private object worksheet = null;

        #endregion ����

        #region ���������

        private const string PROG_ID = "Excel.Application";
        private const int XL_LAST_CELL = 11;
        private const int XL_PASTE_COLUMN_WIDTHS = 8;
        private const int XL_PASTE_SPECIAL_OPERATION_NONE = -4142;

        #endregion ���������

        #region �������������

        /// <summary>
        /// ����������� ������
        /// </summary>
        public ExcelHelper()
        {
            // �������� ������ �� ��������� IDispatch
            try
            {
                // �������� ��� �������
                Type objectType = Type.GetTypeFromProgID(PROG_ID);
                // ������� ������ MS Excel
                excelObj = Activator.CreateInstance(objectType);
                // ���������� ������ �� ���������� ������
                //objectsList.Add(obj);
            }
            catch
            {
                // ���� �� ������� - ���������� ���������� �� ������
                throw new Exception("���������� ������� ������ " + PROG_ID);
            }
        }

        /// <summary>
        /// ������� ��������
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {

        }

        #endregion �������������

        #region ��������

        public bool AskToUpdateLinks
        {
            set
            {
                excelObj.GetType().InvokeMember(
                    "AskToUpdateLinks", BindingFlags.SetProperty, null, excelObj, new object[] { value });
            }
        }

        /// <summary>
        /// ����� ���������� ��������� � �������������� 
        /// </summary>
        public bool DisplayAlerts
        {
            set
            {
                excelObj.GetType().InvokeMember(
                    "DisplayAlerts", BindingFlags.SetProperty, null, excelObj, new object[] { value });
            }
        }

        /// <summary>
        /// �������� ������� ��� ������� Application
        /// </summary>
        public bool EnableEvents
        {
            set
            {
                excelObj.GetType().InvokeMember(
                    "EnableEvents", BindingFlags.SetProperty, null, excelObj, new object[] { value });
            }
        }

        /// <summary>
        /// ������ Excel
        /// </summary>
        public string Version
        {
            get
            {
                try
                {
                    return Convert.ToString(excelObj.GetType().InvokeMember(
                        "Version", BindingFlags.GetProperty, null, excelObj, null));
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        #endregion ��������

        #region ������ � ����������

        /// <summary>
        /// ������� ����� Excel-�������� (�� ��������� ����������� ������ ���� �� ��������� ������)
        /// </summary>
        /// <param name="show">�������� Excel</param>
        public void CreateDocument(bool show)
        {
            excelObj.GetType().InvokeMember(
                "Visible", BindingFlags.SetProperty, null, excelObj, new object[] { show });
            workbooks = excelObj.GetType().InvokeMember(
                "Workbooks", BindingFlags.GetProperty, null, excelObj, null);
            workbook = workbooks.GetType().InvokeMember(
                "Add", BindingFlags.InvokeMethod, null, workbooks, null);
            worksheets = workbook.GetType().InvokeMember(
                "Worksheets", BindingFlags.GetProperty, null, workbook, null);
            worksheet = worksheets.GetType().InvokeMember(
                "Item", BindingFlags.GetProperty, null, worksheets, new object[] { 1 });
        }

        /// <summary>
        /// ������� ����� Excel-�������� (�� ��������� ����������� ������ ���� �� ��������� ������)
        /// </summary>
        public void CreateDocument()
        {
            CreateDocument(false);
        }

        /// <summary>
        /// ������� Excel-�������� (�� ��������� ����������� ������ ���� �� ��������� ������)
        /// </summary>
        /// <param name="filename">��� �����</param>
        /// <param name="show">�������� Excel</param>
        public void OpenDocument(string filename, bool show)
        {
            excelObj.GetType().InvokeMember(
                "Visible", BindingFlags.SetProperty, null, excelObj, new object[] { show });
            workbooks = excelObj.GetType().InvokeMember(
                "Workbooks", BindingFlags.GetProperty, null, excelObj, null);
            workbook = workbooks.GetType().InvokeMember(
                "Open", BindingFlags.InvokeMethod, null, workbooks, new object[] { filename, true });
            worksheets = workbook.GetType().InvokeMember(
                "Worksheets", BindingFlags.GetProperty, null, workbook, null);
            worksheet = worksheets.GetType().InvokeMember(
                "Item", BindingFlags.GetProperty, null, worksheets, new object[] { 1 });
        }

        /// <summary>
        /// ������� Excel-�������� (�� ��������� ����������� ������ ���� �� ��������� ������)
        /// </summary>
        /// <param name="filename">��� �����</param>
        public void OpenDocument(string filename)
        {
            OpenDocument(filename, false);
        }

        /// <summary>
        /// ��������� �������� � Excel-����
        /// </summary>
        /// <param name="filename">��� �����</param>
        public void SaveDocument(string filename)
        {
            if (File.Exists(filename))
            {
                workbook.GetType().InvokeMember(
                    "Save", BindingFlags.InvokeMethod, null, workbook, null);
            }
            else
            {
                workbook.GetType().InvokeMember(
                    "SaveAs", BindingFlags.InvokeMethod, null, workbook, new object[] { filename });
            }
        }

        /// <summary>
        /// ������� Excel-��������
        /// </summary>
        public void CloseDocument()
        {
            try
            {
                workbooks.GetType().InvokeMember(
                    "Close", BindingFlags.InvokeMethod, null, workbooks, null);
                Marshal.ReleaseComObject(worksheet);
                Marshal.ReleaseComObject(worksheets);
                Marshal.ReleaseComObject(workbook);
                Marshal.ReleaseComObject(workbooks);
                excelObj.GetType().InvokeMember(
                    "Quit", BindingFlags.InvokeMethod, null, excelObj, null);
                Marshal.ReleaseComObject(excelObj);
                GC.GetTotalMemory(true);
            }
            catch
            {
            }
        }

        #endregion ������ � ����������

        #region ������ � �������

        /// <summary>
        /// �������� ����� �����
        /// </summary>
        /// <param name="activate">������� ����� ����� ��������</param>
        /// <returns>������ �����</returns>
        public object AddWorkbook(bool activate)
        {
            object workbookObj = workbooks.GetType().InvokeMember(
                "Add", BindingFlags.InvokeMethod, null, workbooks, null);
            if (activate)
            {
                SetWorkbook(workbookObj);
            }
            return workbookObj;
        }

        /// <summary>
        /// �������� ����� ����� � ������������ �
        /// </summary>
        /// <returns>������ �����</returns>
        public object AddWorkbook()
        {
            return AddWorkbook(true);
        }

        /// <summary>
        /// ��������� ������� ����� � ����
        /// </summary>
        /// <param name="filename">��� �����</param>
        public void SaveWorkbook(string filename)
        {
            workbook.GetType().InvokeMember(
                "SaveAs", BindingFlags.InvokeMethod, null, workbook, new object[] { filename });
        }

        /// <summary>
        /// ���������� ������� ����� �� � ������ (��������� ���������� � 1)
        /// </summary>
        /// <param name="index">����� ����� � ���������</param>
        /// <param name="sheetIndex">����� �������� ����� � �����</param>
        public void SetWorkbook(int index, int sheetIndex)
        {
            workbook = workbooks.GetType().InvokeMember(
                "Item", BindingFlags.GetProperty, null, workbooks, new object[] { index });
            worksheets = workbook.GetType().InvokeMember(
                "Worksheets", BindingFlags.GetProperty, null, workbook, null);
            worksheet = worksheets.GetType().InvokeMember(
                "Item", BindingFlags.GetProperty, null, worksheets, new object[] { sheetIndex });
        }

        /// <summary>
        /// ���������� ������� ����� �� � ������ (��������� ���������� � 1)
        /// </summary>
        /// <param name="index">����� ����� � ���������</param>
        public void SetWorkbook(int index)
        {
            SetWorkbook(index, 1);
        }

        /// <summary>
        /// ���������� ������� �����
        /// </summary>
        /// <param name="workbookObj">������ �����</param>
        public void SetWorkbook(object workbookObj)
        {
            workbook = workbookObj;
            worksheets = workbook.GetType().InvokeMember(
                "Worksheets", BindingFlags.GetProperty, null, workbook, null);
            worksheet = worksheets.GetType().InvokeMember(
                "Item", BindingFlags.GetProperty, null, worksheets, new object[] { 1 });
        }

        /// <summary>
        /// �������� ����� �� � ������
        /// </summary>
        /// <param name="index">����� �����</param>
        /// <returns>������ �����</returns>
        public object GetWorkbook(int index)
        {
            return workbooks.GetType().InvokeMember(
                "Item", BindingFlags.GetProperty, null, workbooks, new object[] { index });
        }

        /// <summary>
        /// ������� ������� �����
        /// </summary>
        public void CloseWorkbook()
        {
            workbook.GetType().InvokeMember(
                "Worksheets", BindingFlags.GetProperty, null, workbook, null);
        }

        #endregion ������ � �������

        #region ������ � �������

        /// <summary>
        /// �������� ���������� ������ � �����
        /// </summary>
        /// <returns>���������� ������ � �����</returns>
        public int GetWorksheetsCount()
        {
            try
            {
                return Convert.ToInt32(worksheets.GetType().InvokeMember(
                    "Count", BindingFlags.GetProperty, null, worksheets, null));
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// ���������� ������� ���� �� ��� ������ (��������� ���������� � 1)
        /// </summary>
        /// <param name="index">����� ����� � �����</param>
        public void SetWorksheet(int index)
        {
            worksheet = worksheets.GetType().InvokeMember(
                "Item", BindingFlags.GetProperty, null, worksheets, new object[] { index });
        }

        /// <summary>
        /// ���������� ������� ���� �� ��� ��������
        /// </summary>
        /// <param name="name">��� ����� � �����</param>
        public void SetWorksheet(string name)
        {
            worksheet = worksheets.GetType().InvokeMember(
                "Item", BindingFlags.GetProperty, null, worksheets, new object[] { name });
        }

        /// <summary>
        /// �������� ��� �������� �����
        /// </summary>
        /// <returns></returns>
        public string GetWorksheetName()
        {
            try
            {
                return (string)worksheet.GetType().InvokeMember(
                    "Name", BindingFlags.GetProperty, null, worksheet, null);
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion ������ � �������

        #region ������ � ����������� �����

        /// <summary>
        /// �������� �������� ����� �� �������� �����
        /// </summary>
        /// <param name="cells">�������� ����� � ������� CR ��� C1R1:C2R2 (��������, A1 ��� B1:E12)</param>
        /// <returns>������ ��������� �����</returns>
        public object GetRange(string cells)
        {
            return worksheet.GetType().InvokeMember(
                "Range", BindingFlags.GetProperty, null, worksheet, new object[] { cells });
        }

        /// <summary>
        /// ���������� ��� ���������
        /// </summary>
        /// <param name="range1">������ ������� ��������� �����</param>
        /// <param name="range2">������ ������� ��������� �����</param>
        /// <returns>������������ ��������</returns>
        public object UnionRanges(object range1, object range2)
        {
            return excelObj.GetType().InvokeMember(
                "Union", BindingFlags.InvokeMethod, null, excelObj, new object[] { range1, range2 });
        }

        /// <summary>
        /// �������� �������� ������ �������� �����
        /// </summary>
        /// <param name="cellAddress">����� ������</param>
        /// <returns>��������</returns>
        public string GetValue(string cellAddress)
        {
            try
            {
                object range = GetRange(cellAddress);
                return Convert.ToString(range.GetType().InvokeMember(
                    "Value", BindingFlags.GetProperty, null, range, null));
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// �������� �������� ������ �������� �����
        /// </summary>
        /// <param name="rowIndex">����� ������ (������� � 1)</param>
        /// <param name="colIndex">����� ������� (������� � 1)</param>
        /// <returns>��������</returns>
        public string GetValue(int rowIndex, int colIndex)
        {
            try
            {
                object cell = worksheet.GetType().InvokeMember(
                    "Cells", BindingFlags.GetProperty, null, worksheet, new object[] { rowIndex, colIndex });
                return Convert.ToString(cell.GetType().InvokeMember(
                    "Value", BindingFlags.GetProperty, null, cell, null));
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// �������� �������� � ������ �������� �����
        /// </summary>
        /// <param name="cellAddress">����� ������</param>
        /// <param name="cellValue">��������</param>
        public void SetValue(string cellAddress, string cellValue)
        {
            try
            {
                object range = GetRange(cellAddress);
                range.GetType().InvokeMember(
                    "Value", BindingFlags.SetProperty, null, range, new object[] { cellValue });
            }
            catch
            {
            }
        }

        /// <summary>
        /// �������� �������� � ������ �������� �����
        /// </summary>
        /// <param name="rowIndex">����� ������ (������� � 1)</param>
        /// <param name="colIndex">����� ������� (������� � 1)</param>
        /// <param name="cellValue">��������</param>
        public void SetValue(int rowIndex, int colIndex, string cellValue)
        {
            try
            {
                object cell = worksheet.GetType().InvokeMember(
                   "Cells", BindingFlags.GetProperty, null, worksheet, new object[] { rowIndex, colIndex });
                cell.GetType().InvokeMember(
                    "Value", BindingFlags.SetProperty, null, cell, new object[] { cellValue });
            }
            catch
            {
            }
        }

        /// <summary>
        /// ����������� �������� ����� �������� ����� � ����� ������
        /// </summary>
        /// <param name="range">������ ��������� �����</param>
        public void CopyRange(object range)
        {
            // ��� ���������� �������� ���������� � ����� ������
            range.GetType().InvokeMember("Copy", BindingFlags.InvokeMethod, null, range, null);
        }

        /// <summary>
        /// �������� �������� ����� �� ������ ������ � ��������� �������� ����� �������� �����
        /// </summary>
        /// <param name="range">������ ��������� �����</param>
        public void PasteRange(object range)
        {
            worksheet.GetType().InvokeMember(
                "Paste", BindingFlags.InvokeMethod, null, worksheet, new object[] { range, Type.Missing });
        }

        /// <summary>
        /// ����������� ������� ��������� ����� �� ������ ������ � ����������� ������ ��������
        /// </summary>
        /// <param name="range">������ ��������� �����</param>
        public void PasteSpecialRange(object range)
        {
            range.GetType().InvokeMember(
                "PasteSpecial", BindingFlags.InvokeMethod, null, range,
                new object[] { XL_PASTE_COLUMN_WIDTHS, XL_PASTE_SPECIAL_OPERATION_NONE, false, false });
            worksheet.GetType().InvokeMember(
                "Paste", BindingFlags.InvokeMethod, null, worksheet, new object[] { range, Type.Missing });
        }

        /// <summary>
        /// �������� ���������� ����� � ������� �����
        /// </summary>
        /// <returns>���������� �����</returns>
        public int GetRowsCount()
        {
            try
            {
                object cells = worksheet.GetType().InvokeMember(
                    "Cells", BindingFlags.GetProperty, null, worksheet, null);
                object range = cells.GetType().InvokeMember(
                    "SpecialCells", BindingFlags.GetProperty, null, cells, new object[] { XL_LAST_CELL });
                return Convert.ToInt32(range.GetType().InvokeMember(
                    "Row", BindingFlags.GetProperty, null, range, null));
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// ���������� ������ �������
        /// </summary>
        /// <param name="columnIndex">����� ������� (������� � 1)</param>
        /// <param name="width">������</param>
        public void SetColumnWidth(int columnIndex, float width)
        {
            object range = worksheet.GetType().InvokeMember(
                "Columns", BindingFlags.GetProperty, null, worksheet, new object[] { columnIndex });
            object column = range.GetType().InvokeMember(
                "EntireColumn", BindingFlags.GetProperty, null, range, null);
            column.GetType().InvokeMember(
                "ColumnWidth", BindingFlags.SetProperty, null, column, new object[] { width });
        }

        /// <summary>
        /// ���������� ������ ����������� ������ ��� ���� ����� ���������� �������
        /// </summary>
        /// <param name="columnIndex">����� ������� (������� � 1)</param>
        /// <param name="format">������ ������ (��������, "#,##0.00")</param>
        public void SetColumnFormat(int columnIndex, string format)
        {
            object range = worksheet.GetType().InvokeMember(
                "Columns", BindingFlags.GetProperty, null, worksheet, new object[] { columnIndex });
            range.GetType().InvokeMember(
                "NumberFormat", BindingFlags.SetProperty, null, range, new object[] { format });
        }

        /// <summary>
        /// ���������, �������� �� ������ �������
        /// </summary>
        /// <param name="rowIndex">����� ������ (������� � 1)</param>
        /// <returns></returns>
        public bool IsRowHidden(int rowIndex)
        {
            try
            {
                object row = worksheet.GetType().InvokeMember(
                    "Rows", BindingFlags.GetProperty, null, worksheet, new object[] { rowIndex });
                return Convert.ToBoolean(row.GetType().InvokeMember(
                    "Hidden", BindingFlags.GetProperty, null, row, null));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// �������� ����� ������� ������
        /// </summary>
        /// <param name="row">����� ������</param>
        /// <param name="column">����� �������</param>
        /// <param name="index">������ �������</param>
        /// <returns></returns>
        public ExcelLineStyles GetBorderStyle(int rowIndex, int colIndex, ExcelBorderStyles borderIndex)
        {
            object cell = worksheet.GetType().InvokeMember(
                    "Cells", BindingFlags.GetProperty, null, worksheet, new object[] { rowIndex, colIndex });
            
            object border = cell.GetType().InvokeMember("Borders", BindingFlags.GetProperty, null, cell, new object[] {borderIndex});
            return (ExcelLineStyles)(border.GetType().InvokeMember("LineStyle", BindingFlags.GetProperty, null, border, null));
        }

        #endregion ������ � ����������� �����

        #region ���������� ������

        #region ������ � �������� Excel

        // ���������� ��������� ��� ���������� ������
        private object GetExcelObject()
        {
            return excelObj;
        }

        public void SetAskToUpdateLinks(object obj, bool displayAlert)
        {
            obj.GetType().InvokeMember("AskToUpdateLinks", BindingFlags.SetProperty, null, obj, new object[1] { displayAlert });
        }

        public void SetDisplayAlert(object obj, bool displayAlert)
        {
            obj.GetType().InvokeMember("DisplayAlerts", BindingFlags.SetProperty, null, obj, new object[1] { displayAlert });
        }

        /// <summary>
        /// ��������/������ ������ MS Excel
        /// </summary>
        /// <param name="obj">������ MS Excel</param>
        /// <param name="visible">������� ���������</param>
        public void SetObjectVisible(object obj, bool visible)
        {
            obj.GetType().InvokeMember("Visible", BindingFlags.SetProperty, null, obj, new object[1] { visible });
        }

        /// <summary>
        /// ������� �������� Excel
        /// </summary>
        /// <param name="show">�������� Excel</param>
        /// <returns>��������� �� ������ Excel ��� ���������� ������</returns>
        public object OpenExcel(bool show)
        {
            try
            {
                // ��������� Excel
                object obj = GetExcelObject();

                // ��������� ��������� ����� ��������
                ReflectionHelper.SetProperty(obj, "ScreenUpdating", false);
                ReflectionHelper.SetProperty(obj, "Interactive", false);
                // ��������� ��������������
                SetDisplayAlert(obj, false);
                // ��������� ������� �������������� � ������
                SetAskToUpdateLinks(obj, false);

                // ���������� Excel ���� ����������
                if (show)
                {
                    SetObjectVisible(obj, true);
                }

                if (obj == null)
                    throw new Exception();

                return obj;
            }
            catch
            {
                throw new Exception("������ ��� ������� MS Excel. ��������, �� �� ����������.");
            }
        }

        /// <summary>
        /// ��������� ����
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="fileName"></param>
        public void SaveChanges(object workbook, string fileName)
        {
            try
            {
                // ��������� � ����
                object[] args11 = new object[11] {
				fileName, // Filename: OleVariant; 
				Missing.Value,  // FileFormat: OleVariant; 
				Missing.Value,  // Password: OleVariant; 
				Missing.Value,  // WriteResPassword: OleVariant; 
				Missing.Value,  // ReadOnlyRecommended: OleVariant; 
				Missing.Value,  // CreateBackup: OleVariant; 
				Missing.Value,  // AccessMode: XlSaveAsAccessMode; 
				Missing.Value,  // ConflictResolution: OleVariant; 
				Missing.Value,  // AddToMru: OleVariant; 
				Missing.Value,  // TextCodepage: OleVariant; 
				Missing.Value//,  // TextVisualLayout: OleVariant; 
				//CurrentLCID     // lcid: Integer
			    };

                workbook.GetType().InvokeMember("SaveAs", BindingFlags.InvokeMethod, null, workbook, args11);
            }
            catch
            {
            }
        }

        public void CloseExcel(ref object obj)
        {
            // �������� obj ��� �� �����, �������� ��� �������������
            if (excelObj == null)
            {
                obj = null;
                return;
            }
            // ��������� �����
            CloseWorkBooks(excelObj);
        }

        public override void Close()
        {
            try
            {
                // ���� Excel ��� �� ������ - �������� �������
                // ��������� �������������� 
                SetDisplayAlert(excelObj, false);
                // �������� ����� Quit
                excelObj.GetType().InvokeMember("Quit", BindingFlags.InvokeMethod, null, excelObj, null);
                // �������� ���������� COM-������
                Marshal.ReleaseComObject(excelObj);
                // ������ Excel'� �� ������ ����� ������� ���� ������
                // ������� ���� �� ���������� ��� ������ � ������ .NET Framework
                // ��� ���� �� ������ ��������...
                excelObj = null;
                GC.GetTotalMemory(true);
            }
            catch
            {
            }
            base.Close();
        }

        #endregion ������ � �������� Excel

        #region ������ � �������

        /// <summary>
        /// ��������� ��������� Workbooks
        /// </summary>
        /// <param name="applicationObject">������ Excel</param>
        /// <returns>��������� Workbooks</returns>
        private object GetWorkbooks(object applicationObject)
        {
            return applicationObject.GetType().InvokeMember(
                "Workbooks", BindingFlags.GetProperty, null, applicationObject, null);
        }

        /// <summary>
        /// ������� ��������� ����
        /// </summary>
        /// <param name="obj">������ Excel</param>
        public void CloseWorkBooks(object obj)
        {
            object workbooks = GetWorkbooks(obj);
            SetDisplayAlert(obj, false);
            workbooks.GetType().InvokeMember("Close", BindingFlags.InvokeMethod, null, workbooks, null);
        }

        public object InitWorkBook(ref object obj, string fileFullName)
        {
            // �������� obj ��� �� �����, �������� ��� �������������
            obj = excelObj;
            return GetWorkbook(obj, fileFullName, true);
        }

        /// <summary>
        /// �������� ������� �����
        /// </summary>
        /// <param name="obj">������ Excel</param>
        /// <returns>�����</returns>
        public object CreateWorkbook(object obj)
        {
            try
            {
                object workbooks = GetWorkbooks(obj);
                return workbooks.GetType().InvokeMember("Add", BindingFlags.InvokeMethod, null, workbooks, null);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// �������� ���������� �����. ������������ ��� �������� ������������ ���������� �������
        /// </summary>
        /// <param name="fileName">��� �����</param>
        /// <returns>true/false � ����������� �� �������������� ����������</returns>
        private bool CheckFileExt(string fileName)
        {
            string fileExt = Path.GetExtension(fileName);
            return string.Compare(fileExt.ToUpper(), ".XLS") == 0;
        }

        /// <summary>
        /// �������� ����� ����� �� ������������� � �������������� � ������� ������� ���� (�� ����������).
        /// ������������ ��� �������� ���������� �������
        /// </summary>
        /// <param name="fileName">��� �����</param>
        private void CheckFile(string fileName)
        {
            // ��������� ��������������
            if (!CheckFileExt(fileName)) throw new Exception("���� '" + fileName + "' �� �������� ������ Excel");

            // ��������� ������� �����
            if (!File.Exists(fileName)) throw new Exception("���� '" + fileName + "' �� ����������");
        }

        /// <summary>
        /// ��������� ����� (����) ������
        /// </summary>
        /// <param name="obj">������ ������</param>
        /// <param name="fileName">��� �����</param>
        /// <param name="openReadOnly">������� ������ ��� ������</param>
        /// <returns>�����</returns>
        public object GetWorkbook(object obj, string fileName, bool openReadOnly)
        {
            // ��������� ������������ �����
            CheckFile(fileName);

            try
            {
                // �������� ��������� Workbooks
                object workbooksObj = GetWorkbooks(obj);

                // ��������� ����
                object[] args15 = new object[15];
                args15[0] = fileName;
                args15[1] = Type.Missing;
                args15[2] = openReadOnly;
                args15[3] = Type.Missing;
                args15[4] = Type.Missing;
                args15[5] = Type.Missing;
                args15[6] = Type.Missing;
                args15[7] = Type.Missing;
                args15[8] = Type.Missing;
                args15[9] = Type.Missing;
                args15[10] = Type.Missing;
                args15[11] = Type.Missing;
                args15[12] = Type.Missing;
                args15[13] = Type.Missing;
                args15[14] = Type.Missing;

                // ������� ������� �����
                return workbooksObj.GetType().InvokeMember("Open", BindingFlags.InvokeMethod, null, workbooksObj, args15);
            }
            catch
            {
                return null;
            }
        }

        #endregion ������ � �������

        #region ������ � ����������

        /// <summary>
        /// ���������� ��������� �������� ���������
        /// </summary>
        /// <param name="workbook">��������</param>
        /// <param name="sheetIndex">������ �������� (������� � 1)</param>
        /// <returns>��������</returns>
        public object GetSheet(object workbook, int sheetIndex)
        {
            try
            {
                object sheets = workbook.GetType().InvokeMember(
                    "Sheets", BindingFlags.GetProperty, null, workbook, null);
                return sheets.GetType().InvokeMember("Item",
                    BindingFlags.GetProperty, null, sheets, new object[] { sheetIndex });
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// ���������� ��������� �������� ���������
        /// </summary>
        /// <param name="workbook">��������</param>
        /// <param name="sheetName">�������� ��������</param>
        /// <returns>��������</returns>
        public object GetSheet(object workbook, string sheetName)
        {
            try
            {
                object sheets = workbook.GetType().InvokeMember(
                    "Sheets", BindingFlags.GetProperty, null, workbook, null);
                return sheets.GetType().InvokeMember("Item",
                    BindingFlags.GetProperty, null, sheets, new object[] { sheetName });
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// ���������� ��������� �������� ���������
        /// </summary>
        /// <param name="workbook">��������</param>
        /// <param name="sheetIndex">������ �������� (������� � 1)</param>
        /// <returns>��������</returns>
        public int GetSheetCount(object workbook)
        {
            try
            {
                object sheets = workbook.GetType().InvokeMember(
                    "Sheets", BindingFlags.GetProperty, null, workbook, null);
                return Convert.ToInt32(sheets.GetType().InvokeMember("Count",
                    BindingFlags.GetProperty, null, sheets, null));
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// ���������� �������� ��������
        /// </summary>
        /// <param name="sheet">��������</param>
        /// <returns>��������</returns>
        public string GetSheetName(object sheet)
        {
            try
            {
                return Convert.ToString(sheet.GetType().InvokeMember(
                    "Name", BindingFlags.GetProperty, null, sheet, null));
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion ������ � ����������

        #region ������ � ��������

        /// <summary>
        /// �������������� ��������� ������ ������� ������
        /// </summary>
        /// <param name="cell">������</param>
        /// <returns>��������� ������</returns>
        private ExcelCell InitExcelCell(object cell)
        {
            ExcelCell result = new ExcelCell();
            // !!! ����� ������ �������������� ������ ���� �����-����� �����
            if (InitCellFont)
                result.Font = GetCellFont(cell);
            result.Value = GetCellValue(cell);
            return result;
        }

        /// <summary>
        /// ���������� ��������� ������ ��������
        /// </summary>
        /// <param name="sheet">��������</param>
        /// <param name="rowIndex">����� ������ (������� � 1)</param>
        /// <param name="colIndex">����� ������� (������� � 1)</param>
        /// <returns>������</returns>
        public ExcelCell GetCell(object sheet, int rowIndex, int colIndex)
        {
            try
            {
                return InitExcelCell(sheet.GetType().InvokeMember(
                    "Cells", BindingFlags.GetProperty, null, sheet, new object[] { rowIndex, colIndex }));
            }
            catch
            {
                return InitExcelCell(null);
            }
        }

        /// <summary>
        /// ���������� ��������� ������ ��������
        /// </summary>
        /// <param name="sheet">��������</param>
        /// <param name="range">����� ������</param>
        /// <returns>������</returns>
        public ExcelCell GetCell(object sheet, string range)
        {
            return InitExcelCell(sheet.GetType().InvokeMember("Range",
                BindingFlags.GetProperty, null, sheet, new object[] { range }));
        }

        /// <summary>
        /// ���������� �������� ������
        /// </summary>
        /// <param name="cell">������</param>
        /// <returns>�������� ������</returns>
        private string GetCellValue(object cell)
        {
            if (cell != null)
            {
                return Convert.ToString(
                    cell.GetType().InvokeMember("Value", BindingFlags.GetProperty, null, cell, null)).Trim();
            }

            return string.Empty;
        }

        /// <summary>
        /// ������ ������ � ��������
        /// </summary>
        /// <param name="sheet">��������</param>
        /// <param name="rowIndex">����� ������ (������� � 1)</param>
        /// <param name="colIndex">����� ������� (������� � 1)</param>
        /// <param name="cellValue">�������� ������</param>
        public void SetCell(object sheet, int rowIndex, int colIndex, string cellValue)
        {
            try
            {
                sheet.GetType().InvokeMember(
                    "Cells", BindingFlags.SetProperty, null, sheet, new object[] { rowIndex, colIndex, cellValue });
            }
            catch
            {
            }
        }

        private bool initCellFont = false;
        public bool InitCellFont
        {
            get { return initCellFont; }
            set { initCellFont = value; }
        }

        /// <summary>
        /// ���������� ����� ������
        /// </summary>
        /// <param name="cell">������</param>
        /// <returns>����� ������</returns>
        private ExcelCellFont GetCellFont(object cell)
        {
            ExcelCellFont result = new ExcelCellFont();

            if (cell != null)
            {
                object font = cell.GetType().InvokeMember("Font", BindingFlags.GetProperty, null, cell, null);
                result.Bold = Convert.ToBoolean(font.GetType().InvokeMember("Bold", BindingFlags.GetProperty, null, font, null));
                result.Italic = Convert.ToBoolean(font.GetType().InvokeMember("Italic", BindingFlags.GetProperty, null, font, null));
                result.Name = Convert.ToString(font.GetType().InvokeMember("Name", BindingFlags.GetProperty, null, font, null));
                result.Size = (float)Convert.ToDouble(font.GetType().InvokeMember("Size", BindingFlags.GetProperty, null, font, null));
                result.Strikeout = Convert.ToBoolean(font.GetType().InvokeMember("Strikethrough", BindingFlags.GetProperty, null, font, null));
                result.Underline = Convert.ToBoolean(font.GetType().InvokeMember("Underline", BindingFlags.GetProperty, null, font, null));
            }

            return result;
        }

        #endregion ������ � ��������

        #region ��������� �������� �� ��������� ��������

        private bool IsRowHidden(object row)
        {
            if (row == null)
                return false;
            return Convert.ToBoolean(row.GetType().InvokeMember("Hidden", BindingFlags.GetProperty, null, row, null));
        }

        public object GetExcelRow(object sheet, int rowIndex)
        {
            return sheet.GetType().InvokeMember("Rows", BindingFlags.GetProperty, null, sheet, new object[] { rowIndex });
        }

        /// <summary>
        /// ������� ������� �� ������ �������� �����
        /// </summary>
        /// <param name="sheet">�������� ������</param>
        /// <param name="top">����� ������� ������ ������� ������ ��������</param>
        /// <param name="bottom">����� ������ ������ ������� ������ ��������</param>
        /// <param name="columnsMapping">������ ��� ���_����-�����_�������. ���_���� - ��� ���� � ��������, ����
        /// ����� ���������� ������ �� ������� ������ � ������� �����_�������. ���� null - ��� �������.</param>
        /// <returns>�������</returns>
        public DataTable GetSheetDataTable(object sheet, int top, int bottom, object[] columnsMapping)
        {
            DataTable dt = new DataTable();

            // ������� ������� 
            dt.BeginInit();
            for (int i = 0; i < columnsMapping.GetLength(0); i += 2)
            {
                if (columnsMapping[i] != null)
                {
                    dt.Columns.Add(Convert.ToString(columnsMapping[i]));
                }
                else
                {
                    dt.Columns.Add();
                }
            }
            dt.EndInit();

            // ������������ ������ �� ������ � �������
            dt.BeginLoadData();
            for (int i = top; i <= bottom; i++)
            {
                if (skipHiddenRows)
                {
                    object excelRow = GetExcelRow(sheet, i);
                    if (IsRowHidden(excelRow))
                        continue;
                }

                DataRow row = dt.NewRow();

                for (int j = 0; j < columnsMapping.GetLength(0); j += 2)
                {
                    row[j / 2] = GetCell(sheet, i, Convert.ToInt32(columnsMapping[j + 1])).Value;
                }

                dt.Rows.Add(row);
            }
            dt.EndLoadData();

            return dt;
        }

        /// <summary>
        /// ������� ������� �� ������ �������� �����
        /// </summary>
        /// <param name="sheet">�������� ������</param>
        /// <param name="top">����� ������� ������ ������� ������ ��������</param>
        /// <param name="bottom">����� ������ ������ ������� ������ ��������</param>
        /// <param name="startColumn">����� ������� �������</param>
        /// <param name="endColumn">����� ���������� �������</param>
        /// <returns>�������</returns>
        public DataTable GetSheetDataTable(object sheet, int top, int bottom, int startColumn, int endColumn)
        {
            object[] columnsMapping = new object[(endColumn - startColumn + 1) * 2];
            int index = 1;

            for (int i = startColumn; i <= endColumn; i++)
            {
                columnsMapping[index] = i;
                index += 2;
            }

            return GetSheetDataTable(sheet, top, bottom, columnsMapping);
        }

        // ������������� �������, �� ������ �������� �������� - ����� ��������
        private DataTable InitDT(object[][] mappings)
        {
            DataTable dt = new DataTable();
            dt.BeginInit();
            for (int i = 0; i < mappings.GetLength(0); i++)
                for (int j = 0; j < mappings[i].GetLength(0); j += 2)
                    dt.Columns.Add(Convert.ToString(mappings[i][j]));
            dt.EndInit();
            return dt;
        }

        // ��������� ������ ����� � �������; �������: ��� ���� ������� - ������ ������� 
        private void LoadData(ref DataTable dt, object[] sheets, int[] firstRows, int[] lastRows, object[][] mapping)
        {
            dt.BeginLoadData();
            int rowsCount = lastRows[0] - firstRows[0];
            for (int curRow = 0; curRow <= rowsCount; curRow++)
            {
                int rowIndex = 0;
                DataRow row = dt.NewRow();
                for (int i = 0; i < sheets.GetLength(0); i++)
                    try
                    {
                        for (int j = 0; j < mapping[i].GetLength(0); j += 2)
                        {
                            row[rowIndex] = GetCell(sheets[i], curRow + firstRows[i], Convert.ToInt32(mapping[i][j + 1])).Value;
                            rowIndex++;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("��� ��������� ������ {0} ����� {1} �������� ������: {2}",
                            curRow + firstRows[i], GetSheetName(sheets[i]), ex.Message), ex);
                    }
                dt.Rows.Add(row);
            }
            dt.EndLoadData();
        }

        /// <summary>
        /// ������� ������� �� ������ �����
        /// </summary>
        /// <param name="sheets">�������� ������</param>
        /// <param name="firstRow">� ����� ������</param>
        /// <param name="lastRow">�� ����� ������</param>
        /// <param name="columnsMapping">�������: ��� ���� ������� - ������ ������� </param>
        public DataTable GetSheetDataTable(object[] sheets, int[] firstRows, int[] lastRows, object[][] mappings)
        {
            DataTable dt = InitDT(mappings);
            LoadData(ref dt, sheets, firstRows, lastRows, mappings);
            return dt;
        }

        #endregion ��������� �������� �� ��������� ��������

        #endregion ���������� ������

    }
}
