using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.FO41
{
    public interface IFO41Extension
    {
        /// <summary>
        /// Текущий год
        /// </summary>
        int CurrentYear { get; }

        /// <summary>
        /// Группа пользователя
        /// </summary>
        int UserGroup { get; }

        string OKTMO { get; }
        
        /// <summary>
        /// Текущий период
        /// </summary>
        FX_Date_YearDayUNV CurrentYearUNV { get; }

        /// <summary>
        /// Пользователь системы
        /// </summary>
        Users User { get; }
        
        /// <summary>
        /// Ответственный ОГВ
        /// </summary>
        D_OMSU_ResponsOIV ResponsOIV { get; }

        /// <summary>
        /// Ответственный налогоплательщик
        /// </summary>
        D_Org_Privilege ResponsOrg { get; }
        
        /// <summary>
        /// Источник данных по году
        /// </summary>
        /// <param name="year">Год (YYYY)</param>
        /// <returns>Возвращает  источник</returns>
        DataSources DataSource(int year);

        string GetEstReqTabId(int categoryId, int periodId, int estReqId);

        string GetReqTabId(int categoryId, int periodId, int reqId);

        bool IsReqLastPeriod(int periodId);

        int GetCurPeriod();

        int GetPrevPeriod();

        int GetPrevPeriod(int periodId);

        string GetTextForPeriod(int periodId);
    }
}
