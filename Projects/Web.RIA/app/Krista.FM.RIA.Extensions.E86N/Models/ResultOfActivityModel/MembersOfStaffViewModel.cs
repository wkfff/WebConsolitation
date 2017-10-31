using System.ComponentModel;

namespace Krista.FM.RIA.Extensions.E86N.Models.ResultOfActivityModel
{
    public class MembersOfStaffViewModel : ViewModelBase
    {
        [Description("ID")]
        public int MembersOfStaffID { get; set; }

        [Description("Средняя заработная плата сотрудников, (руб.)")]
        public decimal AvgSalary { get; set; }

        [Description("Количество штатных единиц на начало года")]
        public decimal BeginYear { get; set; }

        [Description("Количество штатных единиц на конец года")]
        public decimal EndYear { get; set; }
    }
}
