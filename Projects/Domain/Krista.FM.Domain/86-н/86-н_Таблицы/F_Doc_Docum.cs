using System;

namespace Krista.FM.Domain
{
    public class F_Doc_Docum : FactTable
    {
        public static readonly string Key = "e50092e2-2dc4-4180-ab40-889183d5847c";

        public virtual int SourceID { get; set; }

        public virtual int TaskID { get; set; }

        public virtual string Url { get; set; }

        public virtual string Name { get; set; }

        public virtual DateTime? DocDate { get; set; }

        public virtual string NumberNPA { get; set; }

        public virtual string ProoveOrg { get; set; }

        public virtual string UrlExternal { get; set; }

        public virtual D_Doc_TypeDoc RefTypeDoc { get; set; }

        public virtual F_F_ParameterDoc RefParametr { get; set; }

        public virtual int? FileSize { get; set; }
    }
}
