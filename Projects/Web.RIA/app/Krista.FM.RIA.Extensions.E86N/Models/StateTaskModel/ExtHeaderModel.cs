using System;
using System.ComponentModel;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Models.StateTaskModel
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
        /// Не доводить ГЗ
        /// </summary>
        [DataBaseBindingTable(typeof(T_F_ExtHeader))]
        public bool NotBring { get; set; }

        [DataBaseBindingTable(typeof(T_F_ExtHeader))]
        public virtual string StatementTask { get; set; }

        [DataBaseBindingTable(typeof(T_F_ExtHeader))]
        public virtual string StateTaskNumber { get; set; }

        [DataBaseBindingTable(typeof(T_F_ExtHeader))]
        public virtual string ApproverFirstName { get; set; }

        [DataBaseBindingTable(typeof(T_F_ExtHeader))]
        public virtual string ApproverLastName { get; set; }

        [DataBaseBindingTable(typeof(T_F_ExtHeader))]
        public virtual string ApproverMiddleName { get; set; }

        [DataBaseBindingTable(typeof(T_F_ExtHeader))]
        public virtual string ApproverPosition { get; set; }
    }
}