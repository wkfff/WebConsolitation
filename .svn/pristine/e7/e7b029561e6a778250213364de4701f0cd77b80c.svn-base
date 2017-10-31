using System;
using System.Collections.Generic;
using System.Linq;
using Ext.Net.MVC;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.FO41.Services;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Controllers.HMAO
{
    public class FO41HMAOMarkDetailsController : SchemeBoundController
    {
        private readonly FactsService factsRepository;
        private const int PercentCode = 744;

        public FO41HMAOMarkDetailsController(FactsService factsRepository)
        {
            this.factsRepository = factsRepository;
        }

        public RestResult Read(int categoryId, int markId, int periodId)
        {
            try
            {
                // получили список значений по показателю markId 
                // по выбранной категории и периоду
                var facts = factsRepository.FindAll().Where(f => 
                    f.RefApplication.RefOrgCategory.ID == categoryId && 
                    f.RefMarks.ID == markId && 
                    f.RefApplication.RefYearDayUNV.ID == periodId).ToList();

                var pfactSum = facts.Sum(x => x.PreviousFact ?? 0);
                if (pfactSum == 0)
                {
                    pfactSum = 1;
                }

                var factSum = facts.Sum(x => x.Fact ?? 0);
                if (factSum == 0)
                {
                    factSum = 1;
                }

                var defaultFact = facts.FirstOrDefault();

                var okei = (defaultFact == null) ? 0 : defaultFact.RefMarks.RefOKEI != null ? defaultFact.RefMarks.RefOKEI.Code : 0;
                var numberStr = 1;
                var data = new List<object>();
                foreach (var fact in facts)
                {
                    data.Add(new
                    {
                        RefNumberString = numberStr,
                        TaxPayerName = fact.RefApplication.RefOrgPrivilege.Name,
                        MarkName = fact.RefMarks.Name,
                        PreviousFactValue = fact.PreviousFact,
                        PreviousFactPart = okei == PercentCode ? null : (decimal?)((fact.PreviousFact ?? 0) / pfactSum),
                        FactValue = fact.Fact,
                        FactPart = okei == PercentCode ? null : (decimal?)(fact.Fact ?? 0) / factSum,
                        OKEI = okei,
                        TempID = fact.ID
                    });
                    numberStr++;
                }
                
                return new RestResult { Success = true, Data = data };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }
    }
}
