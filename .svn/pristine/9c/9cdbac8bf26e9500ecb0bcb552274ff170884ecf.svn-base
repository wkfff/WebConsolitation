using System.Collections;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Services
{
    public interface IFinanceService
    {
        DataTable GetFinanceListTable(D_ExcCosts_ListPrg program);

        IList GetAllFinSourcesListForLookup();

        DataTable GetReportTable(D_ExcCosts_ListPrg program, int year);
        
        D_ExcCosts_Finances GetFinSource(int id);

        void CreateFactData(int programId, D_ExcCosts_Events action, D_ExcCosts_Finances finSource, Dictionary<int, decimal?> yearsValues);

        void UpdateFactData(int programId, D_ExcCosts_Events action, D_ExcCosts_Finances finSource, Dictionary<int, decimal?> yearsValues);

        void DeleteFactData(int programId, D_ExcCosts_Events action, D_ExcCosts_Finances finSource);

        void SaveReportFactData(int programId, int financePlanId, Dictionary<int, decimal?> monthFactList);
    }
}