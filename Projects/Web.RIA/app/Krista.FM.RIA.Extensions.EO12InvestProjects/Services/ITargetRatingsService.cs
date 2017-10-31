using System.Collections.Generic;
using System.Data;

namespace Krista.FM.RIA.Extensions.EO12InvestProjects.Services
{
    public interface ITargetRatingsService
    {
        IList<object> GetQuarterList(int refProjId);

        DataTable GetRatingsTable(int refProjId, string yearQuarter);

        void CreateFactData(int refProjId, string yearQuarter, int refIndicatorId, decimal value);

        void UpdateFactData(int refProjId, string yearQuarter, int refIndicatorId, decimal? value);

        void DeleteFactData(int refProjId, string yearQuarter, int refIndicatorId);

        string EncodeYearQuarter(int year, int quarter);

        int DecodeYearFromYearQuarter(string yearQuarter);

        int DecodeQuarterFromYearQuarter(string yearQuarter);
    }
}
