using System;
using System.Collections.Generic;
using System.Linq;

using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Models
{
    public class OKEIModel : ViewModelBase
    {
        public int ID { get; set; }

        [DataBaseBindingTable(typeof(D_Org_OKEI))]
        public string Name { get; set; }

        [DataBaseBindingTable(typeof(D_Org_OKEI))]
        public int Code { get; set; }

        [DataBaseBindingTable(typeof(D_Org_OKEI))]
        public string Symbol { get; set; }

        [DataBaseBindingTable(typeof(D_Org_OKEI))]
        public DateTime? StartDateActive { get; set; }

        [DataBaseBindingTable(typeof(D_Org_OKEI))]
        public DateTime? EndDateActive { get; set; }

        public static List<OKEIModel> GetList(int limit, int start, string query)
        {
            return GetNewRestService().GetItems<D_Org_OKEI>()
                    .Where(p =>

                            // если нет дат
                            ((!p.StartDateActive.HasValue && !p.EndDateActive.HasValue)

                            // если присутствует только даты действует с
                            || (p.StartDateActive.HasValue && p.StartDateActive < DateTime.Today)

                            // если присутствует только даты действует по
                            || (p.EndDateActive.HasValue && p.EndDateActive > DateTime.Today)
                            
                            // если присутствуют все даты
                            || (p.StartDateActive < DateTime.Today && p.EndDateActive > DateTime.Today))
                                                                                   
                            && p.Name.Contains(query))
                    .Select(
                        p => new OKEIModel
                        {
                            ID = p.ID,
                            Name = p.Name,
                            Code = p.Code,
                            Symbol = p.Symbol,
                            StartDateActive = p.StartDateActive,
                            EndDateActive = p.EndDateActive
                        }).ToList();
        }
    }
}
