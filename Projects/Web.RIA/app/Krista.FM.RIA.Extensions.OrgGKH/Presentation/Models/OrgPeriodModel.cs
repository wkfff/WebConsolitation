using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.OrgGKH.Presentation.Models
{
    public class OrgPeriodModel
    {
        /// <summary>
        /// Идентификатор организации
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Организации.Реестр организаций
        /// </summary>
        public D_Org_RegistrOrg RefRegistrOrg { get; set; }

        /// <summary>
        /// Статус объекта
        /// </summary>
        public FX_Org_StatusD RefStatusD { get; set; }

        /// <summary>
        /// Есть ли данные по этой организации в заданном периоде
        /// </summary>
        public bool HasData { get; set; }
    }
}
