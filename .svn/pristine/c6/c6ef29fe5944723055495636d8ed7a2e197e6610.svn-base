using System;
using System.ComponentModel;

using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Models.ChangeLog
{
    [Description("Действия пользователей")]
    public class ChangeLogModel : ViewModelBase
    {
        [DataBaseBindingTable(typeof(F_F_ChangeLog))]
        public int ID { get; set; }

        [DataBaseBindingTable(typeof(F_F_ChangeLog))]
        public string Login { get; set; }

        [DataBaseBindingTable(typeof(F_F_ChangeLog))]
        public string OrgINN { get; set; }

        [DataBaseBindingField(typeof(F_F_ChangeLog), "Data")]
        public DateTime Date { get; set; }

        [Description("Время события")]
        public string Time { get; set; }

        [DataBaseBindingTable(typeof(F_F_ChangeLog))]
        public int DocId { get; set; }

        [DataBaseBindingTable(typeof(F_F_ChangeLog))]
        public int Year { get; set; }

        public int RefChangeType { get; set; }

        [DataBaseBindingField(typeof(FX_FX_ChangeLogActionType), "Name")]
        public string RefChangeTypeName { get; set; }

        public int RefType { get; set; }

        [DataBaseBindingField(typeof(FX_FX_PartDoc), "Name")]
        public string RefTypeName { get; set; }

        [DataBaseBindingTable(typeof(F_F_ChangeLog))]
        public string Note { get; set; }
    }
}
