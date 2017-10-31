using System.Collections.Generic;
using System.Data;

namespace Krista.FM.ServerLibrary
{

    public enum NormativesKind
    { 
        AllNormatives = 0, 
        NormativesBK = 1, 
        NormativesRegionRF = 2, 
        NormativesMR = 3, 
        VarNormativesRegionRF = 4, 
        VarNormativesMR = 5, 
        Unknown = 6
    };

    /// <summary>
    /// Интерфейс правил расщепления
    /// </summary>
    public interface IDisintRules
    {
        /// <summary>
        /// Таблица правил расщепления
        /// </summary>
        IDataUpdater GetDisintRules_KD();

        /// <summary>
        /// Таблица альтернативных кодов для расщепления
        /// </summary>
        IDataUpdater GetDisintRules_ALTKD();

        /// <summary>
        /// Таблица исключений
        /// </summary>
        IDataUpdater GetDisintRules_EX();

        /// <summary>
        /// Исключения по району
        /// </summary>
        IDataUpdater GetDisintRules_ExPeriod();

        /// <summary>
        /// Исключения по периоду
        /// </summary>
        IDataUpdater GetDisintRules_ExRegion();

        /// <summary>
        /// Исключения района по периоду
        /// </summary>
        IDataUpdater GetDisintRules_ExBoth();

        /// <summary>
        /// возвращает норматив отчислений
        /// </summary>
        /// <param name="normatives"></param>
        /// <returns></returns>
        DataTable GetNormatives(NormativesKind normatives);

        /// <summary>
        /// применение изменений к нормативу
        /// </summary>
        /// <param name="normatives"></param>
        /// <param name="changes"></param>
        /// <returns></returns>
        bool ApplyChanges(NormativesKind normatives, DataTable changes);

        /// <summary>
        /// получение значения - ссылки на другой норматив
        /// </summary>
        /// <param name="normative"></param>
        /// <param name="refKD"></param>
        /// <param name="refYear"></param>
        /// <param name="refBudLevel"></param>
        /// <returns></returns>
        decimal GetConsRegionBudget(NormativesKind normative, int refKD, int refYear, int refBudLevel);

        /// <summary>
        /// расщепление данных по нормативам. С выбором вида расщепления
        /// </summary>
        string SplitData(int variantId, int refVariantType, bool checkNormativeType, ref int newNormativeId, int splitYear);

        /// <summary>
        /// расщепление данных с записью данных на указанный вариант
        /// </summary>
        string SplitData(int variantId, int refVariantType, bool checkNormativeType, int splitVariant, int splitYear);

        /// <summary>
        /// новый метод получения нормативов
        /// </summary>
        /// <param name="normatives"></param>
        /// <returns></returns>
        DataTable GetNewNormatives(NormativesKind normatives);

        /// <summary>
        /// получение информации по источникам данных
        /// </summary>
        /// <param name="sourcesIDs"></param>
        /// <returns></returns>
        DataTable GetSourcesTable(List<int> sourcesIDs);

        /// <summary>
        /// добавление источников и записей в классификаторы
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="classifiersRows"></param>
        /// <param name="classifierName"></param>
        /// <returns></returns>
        Dictionary<int, int> GetNewClassifiersRef(DataTable sources, DataTable classifiersRows,
            string classifierName);

        /// <summary>
        /// получение параметров родительской записи
        /// </summary>
        /// <param name="refKD"></param>
        /// <param name="refYear"></param>
        /// <param name="rowNormative"></param>
        /// <param name="parentRefKD"></param>
        /// <param name="parentRefYear"></param>
        /// <param name="parentRowNormative"></param>
        /// <returns></returns>
        bool GetParentRowParams(int refKD, int refYear, NormativesKind rowNormative,
            ref int parentRefKD, ref int parentRefYear, ref NormativesKind parentRowNormative);

        int[] DataTransfert(int oldYear, int newYear);

        int[] DataTransfert(int newYear, Dictionary<int, int> selectedRows);

        void FundDataTransfert(int fundVariant, int fundMarks, string kdCode, string kdName, ref string messages);
    }

    /// <summary>
    /// интерфейс для расщепления по нормативам
    /// </summary>
    interface INormativeSplitService
    {
        
    }
}
