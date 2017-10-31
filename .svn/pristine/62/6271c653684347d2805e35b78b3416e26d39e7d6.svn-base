using System.Collections.Generic;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.Region10MarksOIV
{
    public interface IRegion10MarksOivExtension
    {
        /// <summary>
        /// Значение текущего выбранного года
        /// </summary>
        int CurrentYearVal { get; set; }
        
        /// <summary>
        /// Текущий выбранный год
        /// </summary>
        FX_Date_Year CurrentYear { get; }

        /// <summary>
        /// Источник данных, на который пишем факты
        /// </summary>
        DataSources DataSourceOiv { get; }
        
        /// <summary>
        /// Территория, соответствующая пользователю
        /// </summary>
        D_Territory_RF UserTerritoryRf { get; }
        
        /// <summary>
        /// ОИВ, к которому относится пользователь
        /// </summary>
        D_OMSU_ResponsOIV UserResponseOiv { get; }

        /// <summary>
        /// Территория, соответствующая субьекту (на неё пишутся факты/показатели для ОИВ-ов)
        /// </summary>
        D_Territory_RF RootTerritoryRf { get; }

        /// <summary>
        /// Возвращает список лет, на которые есть зарегистрированные источники
        /// </summary>
        List<int> Years { get; }
    }
}