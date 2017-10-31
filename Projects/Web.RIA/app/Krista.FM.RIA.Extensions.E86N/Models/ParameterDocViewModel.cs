namespace Krista.FM.RIA.Extensions.E86N.Models
{
    public class ParameterDocViewModel : ViewModelBase
    {
        public int ID { get; set; }

        public string Note { get; set; }

        public bool PlanThreeYear { get; set; }

        public int RefPartDocID { get; set; }

        // RefPartDoc.ID
        public int RefSostID { get; set; }

        // RefSost.ID
        public int RefUchrID { get; set; }

        // RefUchr.ID
        public int RefYearFormID { get; set; }

        // RefYearForm.ID
        // ...для отображения принадлежности учреждения и параметров
        public string RefUchrID_RefOrgPPOID_Name { get; set; }

        // RefUchr.RefOrgPPO.Name
        public string RefUchrID_RefOrgGRBSID_Name { get; set; }

        // RefUchr.RefOrgGRBS.Name
        // ...для разыменовки учреждения и его типа
        public string RefUchrID_Name { get; set; }

        // RefUchr.Name
        public string RefUchrID_RefTypYcID_Name { get; set; }

        // RefUchr.RefTipYc.Name
        // ...для разыменовки параметров из шапки документа
        public string PlanThreeYear_Name { get; set; }

        // PlanThreeYear расшифрованный по ситуации
        public string RefPartDocID_Name { get; set; }

        // RefPartDoc.Name
        public string RefSostID_Name { get; set; }

        // RefSost.Name
        public string RefStates_Name { get; set; }

        // RefStates.Name
        public string OpeningDate { get; set; }

        public string CloseDate { get; set; }

        public string INN { get; set; }
    }
}
