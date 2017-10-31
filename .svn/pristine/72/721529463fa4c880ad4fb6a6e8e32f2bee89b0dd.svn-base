using System.Collections;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Services
{
    public interface ITargetRatingService
    {
        DataTable GetRatingsListTable(D_ExcCosts_ListPrg program);

        IList GetAllRateTypeListForLookup();

        D_ExcCosts_GoalMark GetRate(int id);

        void CreateRateWithFactData(int programId, D_ExcCosts_Tasks task, string rateName, FX_ExcCosts_TypeMark rateType, D_Units_OKEI unit, Dictionary<int, decimal?> yearsValues);

        void UpdateRateWithFactData(int programId, int rateId, string rateName, FX_ExcCosts_TypeMark rateType, D_Units_OKEI unit, D_ExcCosts_Tasks task, Dictionary<int, decimal?> yearsValues);        
        
        void DeleteRateWithFactData(int programId, int rateId);

        FX_ExcCosts_TypeMark GetRateType(int id);

        DataTable GetReportTable(D_ExcCosts_ListPrg program, int year);

        void SaveReportFactData(int programId, int rateId, int year, Dictionary<int, decimal?> monthFactList);

        void DeleteAllRate(int taskId);
    }
}