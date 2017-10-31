using System;

namespace Krista.FM.Common.OfficeHelpers
{
    public class ExcelWorkbook : OfficeDocument
    {
        public ExcelWorkbook(object document)
            : base(document)
        {
        }

        public override void SaveCopyAs(string fileName)
        {
            ReflectionHelper.CallMethod(OfficeObject, "SaveCopyAs", fileName);
        }

        public override void Close()
        {
            ReflectionHelper.CallMethod(OfficeObject, "Close", false, String.Empty, false);
        }

        public bool Activate
        {
            get { return (bool)ReflectionHelper.GetProperty(OfficeObject, "Activate", null); }
            set { ReflectionHelper.SetProperty(OfficeObject, "Activate", value); }
        }
    }
}
