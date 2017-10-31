using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Krista.FM.Server.ProcessorLibrary;
using Microsoft.AnalysisServices;

namespace Krista.FM.Server.OLAP.Processor
{
    /// <summary>
    /// Класс предназначен для генерации XMLA скриптов SSAS2005 для расчета объектов.
    /// </summary>
    static class XmlaGenerator
    {
        private static readonly string xmlnsValue = "http://schemas.microsoft.com/analysisservices/2003/engine";
        private static readonly string xsdValue = "http://www.w3.org/2001/XMLSchema";
        private static readonly string xsiValue = "http://www.w3.org/2001/XMLSchema-instance";
        private static readonly string ddl2Value = "http://schemas.microsoft.com/analysisservices/2003/engine/2";
        private static readonly string ddl2_2Value = "http://schemas.microsoft.com/analysisservices/2003/engine/2/2";

        private static XmlWriterSettings GetXmlWriterSettings()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.NewLineChars = Environment.NewLine;
            settings.Indent = true;
            settings.CloseOutput = false;
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            return settings;
        }

        /// <summary>
        /// Генерирует скрипт для последовательного расчета объектов из накопителя.
        /// </summary>
        /// <param name="items">Записи накопителя.</param>
        /// <param name="databaseId">Идентификатор базы данных.</param>
        /// <returns>Сгенерированный скрипт.</returns>
        public static string GetBatchScriptParallel(
            IEnumerable<ProcessorLibrary.IProcessableObjectInfo> items, string databaseId)
        {
            return GetBatchScript(items, databaseId, true, true);
        }

        /// <summary>
        /// Генерирует скрипт для последовательного расчета объектов из накопителя.
        /// </summary>
        /// <param name="items">Записи накопителя.</param>
        /// <param name="databaseId">Идентификатор базы данных.</param>
        /// <param name="oneTransaction">Расчитывать ли объекты в одной транзакции.</param>
        /// <returns>Сгенерированный скрипт.</returns>
        /// <param name="server"></param>
        public static string GetBatchScriptSequential(
            IEnumerable<ProcessorLibrary.IProcessableObjectInfo> items, string databaseId, bool oneTransaction, Microsoft.AnalysisServices.Server server)
        {
            StringBuilder sb = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(sb, GetXmlWriterSettings()))
            {
                Scripter.WriteStartBatch(writer, oneTransaction);

                foreach (IProcessableObjectInfo processableObjectInfo in items)
                {
                    Scripter.WriteProcess(writer, GetObjectByName(processableObjectInfo, server.Databases[databaseId]),
                                          processableObjectInfo.ProcessType);
                }

                Scripter.WriteEndBatch(writer);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Генерирует скрипт для параллельного расчета объектов из накопителя.
        /// </summary>
        /// <param name="items">Записи накопителя.</param>
        /// <param name="databaseId">Идентификатор базы данных.</param>
        /// <returns>Сгенерированный скрипт.</returns>
        /// <param name="server"></param>
        public static string GetBatchScriptParallel(
            IEnumerable<ProcessorLibrary.IProcessableObjectInfo> items, string databaseId, Microsoft.AnalysisServices.Server server)
        {
            StringBuilder sb = new StringBuilder();

            using (XmlWriter writer = XmlWriter.Create(sb, GetXmlWriterSettings()))
            {
                Scripter.WriteStartBatch(writer, true);
                Scripter.WriteStartParallel(writer);

                foreach (IProcessableObjectInfo processableObjectInfo in items)
                {
                    Scripter.WriteProcess(writer,
                                               GetObjectByName(processableObjectInfo, server.Databases[databaseId]),
                                               processableObjectInfo.ProcessType, WriteBackTableCreation.UseExisting);
                }

                Scripter.WriteEndParallel(writer);
                Scripter.WriteEndBatch(writer);
            }
            return sb.ToString();
        }

        private static IMajorObject GetObjectByName(IProcessableObjectInfo objectInfo, Microsoft.AnalysisServices.Database database)
        {
            switch(objectInfo.ObjectType)
            {
                case OlapObjectType.Cube:
                    return database.Cubes[objectInfo.ObjectID];
                case OlapObjectType.Partition:
                    return
                        database.Cubes[objectInfo.CubeId].MeasureGroups[objectInfo.MeasureGroupId].Partitions[
                            objectInfo.ObjectID];
                case OlapObjectType.Dimension:
                    return database.Dimensions[objectInfo.ObjectID];
                case OlapObjectType.MeasureGroup:
                    return database.Cubes[objectInfo.CubeId].MeasureGroups[objectInfo.ObjectID];
                default :
                    throw new Exception(String.Format("Необработанный тип объекта: {0}", objectInfo.ObjectType));
            }
        }


        /// <summary>
        /// Генерирует скрипт для расчет объектов накопителя.
        /// </summary>
        /// <param name="items">Записи накопителя.</param>
        /// <param name="databaseId">Идентификатор базы данных.</param>
        /// <param name="parallel">True, если необход параллельный расчет.</param>
        /// <param name="oneTransaction">True, если необходим расчет в одной транзакции.</param>
        /// <returns></returns>
        private static string GetBatchScript(
            IEnumerable<ProcessorLibrary.IProcessableObjectInfo> items,
            string databaseId, bool parallel, bool oneTransaction)
        {
            StringBuilder script = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(script, GetXmlWriterSettings());
            try
            {
                writer.WriteStartElement("Batch", xmlnsValue);
                
                if (!oneTransaction)
                {
                    writer.WriteAttributeString("Transaction", "false");
                }
                
                if (parallel)
                {
                    writer.WriteStartElement("Parallel");
                }
                
                foreach (ProcessorLibrary.IProcessableObjectInfo item in items)
                {
                    WriteSingleObjectScript(writer, item, databaseId);
                }

                if (parallel)
                {
                    writer.WriteEndElement();//"Parallel"                
                }

                writer.WriteEndElement();//Batch
                writer.Flush();
            }
            finally
            {                
                writer.Close();
            }
            return script.ToString();
        }        
                
        /// <summary>
        /// Генерирует фрагмент скрипта для расчета одного объекта.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="item">Запись накопителя.</param>
        /// <param name="databaseId">Идентификатор базы данных.</param>
        private static void WriteSingleObjectScript(
            XmlWriter writer, ProcessorLibrary.IProcessableObjectInfo item, string databaseId)
        {
            writer.WriteStartElement("Process");
            writer.WriteAttributeString("xmlns", "xsd", null, xsdValue);
            writer.WriteAttributeString("xmlns", "xsi", null, xsiValue);
            writer.WriteAttributeString("xmlns", "ddl2", null, ddl2Value);
            writer.WriteAttributeString("xmlns", "ddl2_2", null, ddl2_2Value);
            writer.WriteStartElement("Object");
            writer.WriteElementString("DatabaseID", databaseId);
            if (!string.IsNullOrEmpty(item.CubeId))
            {
                writer.WriteElementString("CubeID", item.CubeId);
                writer.WriteElementString("MeasureGroupID", item.MeasureGroupId);
                writer.WriteElementString("PartitionID", item.ObjectID);
            }
            else
            {
                writer.WriteElementString("DimensionID", item.ObjectID);
            }
            writer.WriteEndElement();//Object
            writer.WriteElementString("Type", item.ProcessType.ToString());
            writer.WriteElementString("WriteBackTableCreation", "UseExisting");            
            writer.WriteEndElement();//Process            
        }
    }
}
