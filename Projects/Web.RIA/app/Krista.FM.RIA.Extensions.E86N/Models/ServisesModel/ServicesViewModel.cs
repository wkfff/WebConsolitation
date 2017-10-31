using System;
using System.ComponentModel;

namespace Krista.FM.RIA.Extensions.E86N.Models.ServisesModel
{
    public class ServicesViewModel : ViewModelBase
    {
        public int ID { get; set; }

        /// <summary>
        /// Статус услуги\работы
        /// </summary>
        [Description("Статус")]
        public bool BusinessStatus { get; set; }

        /// <summary>
        /// Код услуги\работы
        /// </summary>
        [Description("Код")]
        public string Code { get; set; }

        /// <summary>
        /// Наименование услуги\работы
        /// </summary>
        [Description("Наименование")]
        public string Name { get; set; }

        /// <summary>
        /// ППО услуги\работы
        /// </summary>
        [Description("ППО")]
        public int? RefOrgPPO { get; set; }

        /// <summary>
        /// ППО услуги\работы
        /// </summary>
        [Description("ППО")]
        public string RefOrgPPOName { get; set; }

        /// <summary>
        /// Платность услуги\работы
        /// </summary>
        [Description("Платность")]
        public int? RefPl { get; set; }

        /// <summary>
        /// Платность услуги\работы
        /// </summary>
        [Description("Платность")]
        public string RefPlName { get; set; }

        /// <summary>
        /// ГРБС услуги\работы
        /// </summary>
        [Description("ГРБС")]
        public int? RefGRBSs { get; set; }

        /// <summary>
        /// ГРБС услуги\работы
        /// </summary>
        [Description("ГРБС")]
        public string RefGRBSsName { get; set; }

        /// <summary>
        /// Услуга/ работа
        /// </summary>
        [Description("Услуга/работа")]
        public int RefTipY { get; set; }

        /// <summary>
        /// Услуга/ работа
        /// </summary>
        [Description("Услуга/работа")]
        public string RefTipYName { get; set; }

        /// <summary>
        /// Сфера деятельности
        /// </summary>
        [Description("Сфера деятельности")]
        public int RefSferaD { get; set; }

        /// <summary>
        /// Сфера деятельности
        /// </summary>
        [Description("Сфера деятельности")]
        public string RefSferaDName { get; set; }

        /// <summary>
        /// Дата включения
        /// </summary>
        [Description("Дата включения")]
        public DateTime? DataVkluch { get; set; }

        /// <summary>
        /// Дата исключения
        /// </summary>
        [Description("Дата исключения")]
        public DateTime? DataIskluch { get; set; }
    }
}
