using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Krista.FM.Common.OfficeHelpers
{
    public class WordApplication : OfficeApplication
    {
        public WordApplication(object officeApp) : base(officeApp)
        {
        }

        public override OfficeDocument CreateAsTemplate(string templatePath)
        {
            object[] paramArray = new object[4];
            paramArray[0] = templatePath;
            paramArray[1] = false;
            paramArray[2] = 0;
            paramArray[3] = true;
            return new WordDocument(ReflectionHelper.CallMethod(Documents, "Add", paramArray));
        }

        private const int wdFormatDocument = 0;
        public override object CreateEmptyDocument(string fileName)
        {
            // добавляем новый пустой документ
            object document = ReflectionHelper.CallMethod(Documents, "Add",
                Missing.Value,	// var Template: OleVariant; 
                Missing.Value,	// var NewTemplate: OleVariant; 
                0,              // var DocumentType: OleVariant = wdNewBlankDocument 
                true			// var Visible: OleVariant
            );
            
            // сохраняем
            ReflectionHelper.CallMethod(document, "SaveAs2000",
                fileName,		// var FileName: OleVariant; 
                wdFormatDocument,
                //Missing.Value,	// var FileFormat: OleVariant; 
                Missing.Value,	// var LockComments: OleVariant; 
                Missing.Value,	// var Password: OleVariant; 
                false,			// var AddToRecentFiles: OleVariant; 
                Missing.Value,	// var WritePassword: OleVariant; 
                Missing.Value,	// var ReadOnlyRecommended: OleVariant; 
                Missing.Value,	// var EmbedTrueTypeFonts: OleVariant; 
                Missing.Value,	// var SaveNativePictureFormat: OleVariant; 
                Missing.Value,	// var SaveFormsData: OleVariant; 
                Missing.Value 	// var SaveAsAOCELetter: OleVariant
            );
            
            // показываем офис
            Visible = true;
            
            ReleaseNormalDot();
            
            return document;
        }

        public override object LoadFile(string fileName, bool openReadOnly)
        {
            try
            {
                return ReflectionHelper.CallMethod(Documents, "Open",
                    fileName,      // FileName
                    Missing.Value, // ConfirmConversions
                    openReadOnly,  // ReadOnly
                    Missing.Value, // AddToRecentFiles
                    Missing.Value, // PasswordDocument
                    Missing.Value, // PasswordTemplate
                    Missing.Value, // Revert
                    Missing.Value, // WritePasswordDocument
                    Missing.Value, // WritePasswordTemplate
                    Missing.Value, // Format
                    Missing.Value, // Encoding
                    Missing.Value, // Visible
                    Missing.Value, // OpenAndRepair
                    Missing.Value, // DocumentDirection
                    Missing.Value  // NoEncodingDialog
                );
            }
            catch (Exception e)
            {
                throw new OfficeApplicationException(String.Format(
                    "Невозможно открыть файл '{0}'. Возможно он не существует, поврежден или используется другим процессом. Исходная ошибка: {1}",
                    fileName, e.Message), e);
            }
        }

        public override void SaveChanges(object docObj, string fileName)
        {
            object document = docObj ?? GetFirstDocument();
            
            bool originDisplayAlerts = DisplayAlerts;
            try
            {
                DisplayAlerts = false;
                ReflectionHelper.CallMethod(document, "SaveAs2000",
                    fileName,      // FileName
                    wdFormatDocument,
                    //Missing.Value, // FileFormat
                    Missing.Value, // LockComments
                    Missing.Value, // Password
                    Missing.Value, // AddToRecentFiles
                    Missing.Value, // WritePassword
                    Missing.Value, // ReadOnlyRecommended
                    Missing.Value, // EmbedTrueTypeFonts
                    Missing.Value, // SaveNativePictureFormat
                    Missing.Value, // SaveFormsData
                    Missing.Value  // SaveAsAOCELetter
                );
            }
            finally
            {
                DisplayAlerts = originDisplayAlerts;
            }
        }

        public static bool IsApplicableFile(string fileName)
        {
            string ext = Path.GetExtension(fileName);
            return ((ext == OfficeFileExt.WordDocument) || (ext == OfficeFileExt.WordDocumentX));
        }

        protected override bool CheckFileExt(string fileName)
        {
            return IsApplicableFile(fileName);
        }

        public override string GetExtension()
        {
            if (GetVersionNumber() < 12)
                return OfficeFileExt.WordDocument;

            return OfficeFileExt.WordDocumentX;
        }

        public override int GetVersionNumber()
        {
            return OfficeHelper.GetWordVersionNumber();            
        }

        private void ReleaseNormalDot()
        {
            // освобождаем нормал дот
            object normalTemplate = null;
            try
            {
                normalTemplate = ReflectionHelper.GetProperty(OfficeApp, "NormalTemplate");
                ReflectionHelper.SetProperty(normalTemplate, "Saved", true);
            }
            finally
            {
                if (normalTemplate != null)
                    Marshal.ReleaseComObject(normalTemplate);
            }
        }

        private object GetFirstDocument()
        {
            return ReflectionHelper.CallMethod(Documents, "Item", 1);
        }

        public void CloseFirstDocument()
        {
            object firstDocument = GetFirstDocument();
            ReleaseNormalDot();
            if (firstDocument != null)
            {
                ReflectionHelper.CallMethod(firstDocument, "Close", false, Missing.Value, Missing.Value);
                Marshal.ReleaseComObject(firstDocument);
            }
        }

        public override bool DisplayAlerts
        {
            set
            {
                uint alertsLevel = value ? 0xFFFFFFFF : 0;
                ReflectionHelper.SetProperty(OfficeApp, "DisplayAlerts", alertsLevel);
            }
        }

        public override void Activate()
        {
            ReflectionHelper.CallMethod(OfficeApp, "Activate");
        }

        public override void Quit()
        {
            ReleaseNormalDot();
            base.Quit();
        }

        public object Selection
        {
            get { return ReflectionHelper.GetProperty(OfficeApp, "Selection"); }
        }

        public object Documents
        {
            get { return ReflectionHelper.GetProperty(OfficeApp, "Documents"); }
        }

        public object ActiveDocument
        {
            get { return ReflectionHelper.GetProperty(OfficeApp, "ActiveDocument"); }
        }
    }
}