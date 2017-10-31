using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.OrgGKH
{
    public interface IOrgGkhExtension
    {
        /// <summary>
        /// Текущий год
        /// </summary>
        int CurrentYear { get; }

        /// <summary>
        /// Актуальный период
        /// </summary>
        FX_Date_YearDayUNV CurrentYearUNV { get; }

        /// <summary>
        /// Пользователь (определяется по логин/паролю)
        /// </summary>
        Users User { get; }

        /// <summary>
        /// Регион пользователя
        /// </summary>
        D_Regions_Analysis Region { get; }

        /// <summary>
        /// Возвращает актуальный источник
        /// </summary>
        /// <returns>актуальный источник</returns>
        DataSources DataSource { get; }

        /// <summary>
        /// Источник для районов
        /// </summary>
        DataSources RegionSource { get; }

        /// <summary>
        /// Группа, к которой принадлежит пользователь
        /// </summary>
        int UserGroup { get; }
    }
}
