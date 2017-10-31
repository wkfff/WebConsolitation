using System.ComponentModel;

namespace Krista.FM.RIA.Extensions.E86N.Models.InfControlMeasures
{
    public class ExtHeaderModel : ViewModelBase
    {
        [Description("ID")]
        public int ID { get; set; }

        /// <summary>
        /// ШапкаДокумента /Ссылка
        /// </summary>
        [Description("ШапкаДокумента/Ссылка")]
        public int RefParameterID { get; set; }

        /// <summary>
        /// Нет контрольных мероприятий
        /// </summary>
        [Description("Нет контрольных мероприятий")]
        public bool NotInspectionActivity { get; set; }
    }
}
