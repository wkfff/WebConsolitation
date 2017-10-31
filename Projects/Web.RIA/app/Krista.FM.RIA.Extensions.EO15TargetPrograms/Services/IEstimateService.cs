using System.Collections;
using System.Collections.Generic;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.EO15TargetPrograms.Models;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Services
{
    public interface IEstimateService
    {
        IList<EstimateModel> GetReportTable(D_ExcCosts_ListPrg program, int year, ProgramStage stage);

        void SaveReportFactData(D_ExcCosts_ListPrg program, int year, int selectedCritId, string comment);

        IList GetSubcriteriasList(int criteriaId);
    }
}