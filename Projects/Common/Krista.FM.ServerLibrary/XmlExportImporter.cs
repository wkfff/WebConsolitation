using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Krista.FM.ServerLibrary
{
    [Serializable]
    public delegate void IncorrectXMLAttributes(string incorrectAttributesMessage);

    [Serializable]
    public delegate void IncorrectXmlStructure(DataTable dtMessages);

    [Serializable]
    public enum ObjectType { Classifier, FactTable, ConversionTable };

    public interface IExportImportManager : IDisposable
    {
        IExportImporter GetExportImporter(ObjectType objectType);
    }

    public interface IExportImporter : IDisposable
    {
        bool CheckXml(Stream stream, string objectKey, ref ImportPatams importParams, ref DataTable varianceTable);
        
        void ExportData(Stream stream, ImportPatams importParams, ExportImportClsParams exportImportClsParams);

        Dictionary<int, int> ImportData(Stream stream, ExportImportClsParams exportImportClsParams);

        void ExportMasterDetail(Stream masterStream, Dictionary<string, Stream> detailStreams,
            ExportImportClsParams exportImportClsParams, ImportPatams importParams);

        Dictionary<string, Dictionary<int, int>> ImportMasterDetail(Stream masterStream, Dictionary<string, Stream> detailStreams,
            ExportImportClsParams exportImportClsParams);
    }

    public interface IConversionTableExportImporter : IExportImporter
    {

        void ExportData(Stream stream, string associationName, string ruleName, ImportPatams importParams);


        void ExportSelectedData(Stream stream, string associationName,
            string ruleName, ImportPatams importParams, int[] selectedRowsID);

        /// <summary>
        /// импорт 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="associationName"></param>
        /// <param name="ruleName"></param>
        void ImportData(Stream stream, string associationName, string ruleName);
    }

    [Serializable]
    public struct ImportPatams
    {
        /// <summary>
        /// удаление данных перед импортом
        /// </summary>
        public bool deleteDataBeforeImport;
        /// <summary>
        /// удаление всех данных, включая защищенные
        /// </summary>
        public bool deleteDeveloperData;
        /// <summary>
        /// сохранение ID при импорте
        /// </summary>
        public bool useOldID;
        /// <summary>
        /// восстанавливать свой источник данных
        /// </summary>
        public bool restoreDataSource;
        /// <summary>
        /// обновлять данные по уникальности в схеме
        /// </summary>
        public bool refreshDataByUnique;
        /// <summary>
        /// обновлять данные по атрибутам, указанным пользоветелем
        /// </summary>
        public bool refreshDataByAttributes;
        /// <summary>
        /// перечень атрибутов, указанных пользователем через запятую
        /// </summary>
        public string uniqueAttributesNames;
    }

    [Serializable]
    public struct ExportImportClsParams
    {
        // уникальный ключ объекта
        public string ClsObjectKey;
        // источник данных
        public int DataSource;
        // задача
        public int TaskID;
        // список всех выделенных записей
        public List<int> SelectedRowsId;
        // дополинительный фильтр
        public string Filter;
        // параметры по дополнительному фильтру
        public IDbDataParameter[] FilterParams;
        // Id текущей закачки
        public int PumpID;
        // соотношение старых id к новым для импорта деталей
        public Dictionary<int, int> NewIds;
        // ссылка детали на мастер 
        public string DetailReferenceColumnName;
        // ссылка на вариант
        public int RefVariant;
        /// <summary>
        /// объект для импорта экспорта
        /// </summary>
        public IEntity ExportImportObject;
    }
}
