using System;
using System.ComponentModel;

namespace Krista.FM.RIA.Extensions.E86N.Models.DocumentsRegisterModel
{
    public class DocumentsRegisterViewModel : ViewModelBase
    {
        [Description("ID")]
        public int ID { get; set; }

        public bool Check { get; set; }

        public int StructureID { get; set; }

        [Description("Название")]
        public string StructureName { get; set; }

        [Description("ГРБС")]
        public string StructureGrbs { get; set; }

        [Description("ППО")]
        public string StructurePpo { get; set; }

        [Description("Состояние")]
        public string State { get; set; }

        [Description("Документ")]
        public string Type { get; set; }

        [Description("Примечание")]
        public string Note { get; set; }

        [Description("Год формирования")]
        public int Year { get; set; }

        public string Url { get; set; }

        public bool Closed { get; set; }

        public bool ClosedOrg { get; set; }

        public string StructureShortName { get; set; }

        public string StructureInn { get; set; }

        public string StructureKpp { get; set; }

        public string StructureGrbsCode { get; set; }

        public string StructureType { get; set; }

        [Description("Дата закрытия учреждения")]
        public DateTime? StructureCloseDate { get; set; }
    }
}
