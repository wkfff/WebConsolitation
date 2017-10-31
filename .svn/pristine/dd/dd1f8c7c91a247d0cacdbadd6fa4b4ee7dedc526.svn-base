using System;
using System.ComponentModel;

namespace Krista.FM.RIA.Extensions.E86N.Models.InstitutionsRegisterModel
{
    [Description("Реестр учреждений")]
    public class InstitutionsRegisterModel : ViewModelBase
    {
        [Description("Идентификатор")]
        public int ID { get; set; }

        [Description("Статус")]
        public bool Status { get; set; }

        [Description("Полное наименование")]
        public string Name { get; set; }

        [Description("Сокращенное наименование")]
        public string ShortName { get; set; }

        public int? RefTipYc { get; set; }

        [Description("Тип учреждения")]
        public string RefTipYcName { get; set; }

        public int? RefOrgPpo { get; set; }

        [Description(@"   ППО   ")]
        public string RefOrgPpoName { get; set; }

        public int? RefOrgGrbs { get; set; }

        [Description(@"   ГРБС   ")]
        public string RefOrgGrbsName { get; set; }

        [Description(@"   ИНН   ")]
        public string Inn { get; set; }

        [Description(@"   КПП   ")]
        public string Kpp { get; set; }

        [Description("Дата включения")]
        public DateTime? OpenDate { get; set; }

        [Description("Дата исключения")]
        public DateTime? CloseDate { get; set; }
    }
}
