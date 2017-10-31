using System.Reflection;

namespace Krista.FM.Common.OfficeHelpers
{
    public class WordDocument : OfficeDocument
    {
        public WordDocument(object document) 
            : base(document)
        {
        }

        public override void SaveCopyAs(string fileName)
        {
            ReflectionHelper.CallMethod(OfficeObject, "SaveAs2000",
                fileName,		// var FileName: OleVariant; 
                0,
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
        }
    }
}
