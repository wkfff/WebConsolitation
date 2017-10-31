using System;
using System.Data;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Server.ReportsService
{
    public class TemplateReport : DisposableObject, ITemplateReport
    {
        public TemplateReport(DataRow templateRow)
        {
            Id = Convert.ToInt32(templateRow["ID"]);
            Key = templateRow["Code"].ToString();
            Caption = templateRow["Name"].ToString();
            DocumentType = (TemplateDocumentTypes) Convert.ToInt32(templateRow["Type"]);
            DocumentName = templateRow.IsNull("DocumentFileName") ? string.Empty : templateRow["DocumentFileName"].ToString();
        }

        public int Id
        {
            get; set;
        }

        private string _key;
        public string Key
        {
            get
            {
                if (String.IsNullOrEmpty(_key))
                    return Caption;
                return _key;
            }
            set
            {
                _key = value;
            }
        }

        public string Caption
        {
            get; set;
        }

        public TemplateDocumentTypes DocumentType
        {
            get; set;
        }

        public string DocumentName
        {
            get; set;
        }
    }
}
