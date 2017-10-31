using System;
using System.Runtime.InteropServices;

namespace Krista.FM.Common.OfficeHelpers
{
    /// <summary>
    /// Базовый класс для документов офиса: Excel/Word.
    /// </summary>
    public abstract class OfficeDocument : DisposableObject
    {
        private readonly object document;

        protected OfficeDocument(object document)
        {
            this.document = document;
        }

        public object OfficeObject
        {
            get { return document; }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if ((document != null) && (Marshal.IsComObject(document)))
                {
                    Marshal.ReleaseComObject(document);
                    GC.GetTotalMemory(true);
                }
            }
            base.Dispose(disposing);
        }

        public abstract void SaveCopyAs(string fileName);
    }
}
