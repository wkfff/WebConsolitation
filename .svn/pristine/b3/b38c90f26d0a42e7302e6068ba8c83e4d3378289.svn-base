using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Krista.FM.Common.OfficeHelpers
{
    public class ExcelApplication : OfficeApplication
    {
        public ExcelApplication(object officeApp) : base(officeApp)
        {
        }

        public override OfficeDocument CreateAsTemplate(string templatePath)
        {
            object workBooks = GetWorkbooks();
            return new ExcelWorkbook(ReflectionHelper.CallMethod(workBooks, "Add", templatePath));
        }

        public override object CreateEmptyDocument(string fileName)
        {
            // добавляем пустую книгу
            object workbooks = GetWorkbooks();
            object workbook = ReflectionHelper.CallMethod(workbooks, "Add");
            InternalSaveChanges(workbook, fileName);
            return workbook;
        }

        public override object LoadFile(string fileName, bool openReadOnly)
        {
            try
            {
                // получаем коллекцию Workbooks
                object workbooksObj = GetWorkbooks();
                // Пробуем открыть книгу
                return ReflectionHelper.CallMethod(workbooksObj, "Open",
                  fileName, Type.Missing, openReadOnly, Type.Missing, Type.Missing, 
                  Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, 
                  Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            }
            catch (Exception e)
            {
                if (fileName.Length > 218)
                {
                    throw new OfficeApplicationException(
                        string.Format(
                            "Невозможно открыть файл '{0}'. Длина полного имени файла (включая путь) превышает предельно возможное для MS Excel значение в 218 символов. Исходная ошибка: {1}",
                            fileName,
                            e.Message),
                        e);
                }

                throw new OfficeApplicationException(
                    string.Format(
                        "Невозможно открыть файл '{0}'. Возможно он не существует, поврежден или используется другим процессом. Исходная ошибка: {1}",
                        fileName,
                        e.Message),
                    e);
            }
        }

        public override void SaveChanges(object docObj, string fileName)
        {
            object document = docObj ?? GetDocument(fileName);

            // Сохраняем
            bool originDisplayAlerts = DisplayAlerts;
            try
            {
                DisplayAlerts = false;
                InternalSaveChanges(document, fileName);
            }
            finally
            {
                DisplayAlerts = originDisplayAlerts;
            }

            if (docObj == null && document != null)
                Marshal.ReleaseComObject(document);
        }

        private object GetDocument(string fileName)
        {
            object workbooks = GetWorkbooks();
            int bookcount = (int)ReflectionHelper.GetProperty(workbooks, "Count");
            for (int i = 1; i <= bookcount; i++)
            {
                object book = ReflectionHelper.GetProperty(workbooks, "Item", i);
                if (ReflectionHelper.GetProperty(book, "Fullname").ToString() == fileName)
                {
                    return book;
                }
                if (ReflectionHelper.GetProperty(book, "Fullname").ToString().StartsWith(fileName))
                {
                    return book;
                }
            }
            return null;
        }

        public static bool IsApplicableFile(string fileName)
        {
            string ext = Path.GetExtension(fileName);
            return ((ext == OfficeFileExt.ExcelDocument) || (ext == OfficeFileExt.ExcelDocumentX));
        }

        protected override bool CheckFileExt(string fileName)
        {
            return IsApplicableFile(fileName);
        }

        public override string GetExtension()
        {
            if (GetVersionNumber() < 12)
                return OfficeFileExt.ExcelDocument;
            return OfficeFileExt.ExcelDocumentX;
        }

        public override int GetVersionNumber()
        {
            return OfficeHelper.GetExcelVersionNumber();            
        }

        public override void Activate()
        {
            int excelWnd = (int)ReflectionHelper.GetProperty(OfficeApp, "Hwnd");
            object activeBook = ReflectionHelper.GetProperty(OfficeApp, "ActiveWorkbook");
            if (activeBook != null)
                ReflectionHelper.CallMethod(activeBook, "Activate");

            if (excelWnd > 0)
            {
                EnableWindow((IntPtr)excelWnd, true);
                if (IsIconic((IntPtr)excelWnd))
                    ShowWindow((IntPtr)excelWnd, SW_MAXIMIZE);
                else
                    SetForegroundWindow((IntPtr)excelWnd);
                ShowWindow((IntPtr)excelWnd, SW_MAXIMIZE);
            }
        }

        public override void Deactivate()
        {
            int excelWnd = (int)ReflectionHelper.GetProperty(OfficeApp, "Hwnd");
            EnableWindow((IntPtr)excelWnd, false);
        }

        public override void Quit()
        {
            DisplayAlerts = false;
            base.Quit();
        }

        /// <summary>
        /// получение коллекции Workbooks.
        /// </summary>
        /// <returns>коллекция Workbooks.</returns>
        public object GetWorkbooks()
        {
            return ReflectionHelper.GetProperty(OfficeApp, "Workbooks");
        }

        /// <summary>
        /// Закрыть коллекцию книг.
        /// </summary>
        public void CloseWorkBooks()
        {
            object workbooks = GetWorkbooks();
            ReflectionHelper.CallMethod(workbooks, "Close");
            Marshal.ReleaseComObject(workbooks);
        }

        /// <summary>
        /// Сохранить файл
        /// </summary>
        private void InternalSaveChanges(object workbook, string fileName)
        {
            // сохраняем в файл
            try
            {
                ReflectionHelper.CallMethod(workbook, "_SaveAs",
                    fileName,       // Filename: OleVariant; 
                    ReflectionHelper.GetProperty(workbook, "FileFormat"),//xlWorkbookDefault, //xlWorkbookNormal,
                    //Missing.Value,  // FileFormat: OleVariant; 
                    Missing.Value,  // Password: OleVariant; 
                    Missing.Value,  // WriteResPassword: OleVariant; 
                    Missing.Value,  // ReadOnlyRecommended: OleVariant; 
                    Missing.Value,  // CreateBackup: OleVariant; 
                    Missing.Value,  // AccessMode: XlSaveAsAccessMode; 
                    Missing.Value,  // ConflictResolution: OleVariant; 
                    Missing.Value,  // AddToMru: OleVariant; 
                    Missing.Value,  // TextCodepage: OleVariant; 
                    Missing.Value   // TextVisualLayout: OleVariant; 
                );
            }
            catch
            {
                throw new Exception(
                    String.Format(
                        "Невозможно сохранить изменения в документе {0}. " +
                        "Возможно файл занят другим процессом или имеет некорректное наименование.",
                        fileName)
                    );
            }
        }
    }
}