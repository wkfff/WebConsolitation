using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.Common
{
    public class BalanceIncomeParams
    {
        public List<string> TaxNotaxCodes
        {
            get; set;
        }

        public Dictionary<string, string> TaxNotaxNames
        {
            get; set;
        }

        public List<string> FreeSupplyCodes
        {
            get; set;
        }

        public Dictionary<string, string> FreeSupplyNames
        {
            get; set;
        }

        public List<string> IncomeWorkCodes
        {
            get; set;
        }

        public Dictionary<string, string> IncomeWorkNames
        {
            get; set;
        }

        public BalanceIncomeParams(List<string> taxNotaxCodes, List<string> freeSupplyCodes, List<string> incomeWorkCodes,
            Dictionary<string, string> taxNotaxNames, Dictionary<string, string> freeSupplyNames, Dictionary<string, string> incomeWorkNames)
        {
            TaxNotaxCodes = taxNotaxCodes;
            FreeSupplyCodes = freeSupplyCodes;
            IncomeWorkCodes = incomeWorkCodes;
            TaxNotaxNames = taxNotaxNames;
            FreeSupplyNames = freeSupplyNames;
            IncomeWorkNames = incomeWorkNames;
        }
    }
}
