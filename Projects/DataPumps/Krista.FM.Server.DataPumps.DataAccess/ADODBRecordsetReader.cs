using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Xml;

using ADODB;

using Krista.FM.Common;
using Krista.FM.Common.Xml;

namespace Krista.FM.Server.DataPumps.DataAccess
{
    /// <summary>
    /// Структура с методами преобразования ADODB.Recordset -> System.Data.DataTable
    /// </summary>
    public struct ADODBRecordsetReader
    {
        // название корневого тэга в xml-recordset'а, формируемого слоями
        private const string recordsetNodeTagName = "RecordSet";
        // название тэка с имененм слоя
        private const string recordsetNameAttrTagName = "name";

        /// <summary>
        /// Загрузить DataTable из XML-файлы в стандартном формате ADODB.Recordset или слоя
        /// </summary>
        /// <param name="destDataTable">DataTable куда будут помещены данные</param>
        /// <param name="fileName">полный путь к XML-файлу</param>
        public static void LoadRecordset(ref DataTable destDataTable, string fileName)
        {
            // проверяем наличие и доступность файла
            if (!File.Exists(fileName))
                throw new FileNotFoundException(fileName);

            #region Получение дополнительных свойств
            string recordsetName = String.Empty;
            XmlDocument xmlDoc = null;
            try
            {
                xmlDoc = new XmlDocument();
                xmlDoc.Load(fileName);
                XmlNode recordsetNode = xmlDoc.DocumentElement.SelectSingleNode(recordsetNodeTagName);
                // пытаемся получить имя рекордсета (слоя)
                if (recordsetNode != null)
                {
                    recordsetName = XmlHelper.GetStringAttrValue(recordsetNode,
                        recordsetNameAttrTagName, String.Empty);
                }
            }
            finally
            {
                // освобождаем память из под XML документа
                if (xmlDoc != null)
                {
                    XmlHelper.ClearDomDocument(ref xmlDoc);
                }
            }
            #endregion

            #region Загрузка Recordset в DataTable
            Recordset rs = null;
            try
            {
                // пытаемся загрузить рекордсет
                rs = new Recordset();
                rs.Open(fileName, "PROVIDER=MSPersist;", CursorTypeEnum.adOpenStatic,
                    LockTypeEnum.adLockReadOnly, (int)CommandTypeEnum.adCmdFile);
                // загружаем содержимое рекордсета в DataTable
                LoadRecordset(ref destDataTable, recordsetName, rs);
            }
            catch (Exception e)
            {
                throw new Exception(
                    String.Format("Невозможно загрузить Recordset из файла '{0}'", fileName), e);
            }
            finally
            {
                // закрываем рекордсет
                if ((rs != null) && (rs.State == (int)ObjectStateEnum.adStateOpen))
                {
                    rs.Close();
                }
            }
            #endregion
        }

        /// <summary>
        /// Загрузить DataTable из XML-файлы в стандартном формате ADODB.Recordset или слоя
        /// </summary>
        /// <param name="destDataTable">DataTable куда будут помещены данные</param>
        /// <param name="recordsetName">Название рекордсета (слоя)</param>
        /// <param name="recordset">ADODB.Recordset  с данными</param>
        public static void LoadRecordset(ref DataTable destDataTable, string recordsetName, 
            Recordset recordset)
        {
            if (recordset == null)
                throw new Exception("Не задан объект Recordset");

            if (destDataTable == null)
                destDataTable = new DataTable();

            destDataTable.TableName = recordsetName;

            using (OleDbDataAdapter upd = new OleDbDataAdapter())
            {
                upd.Fill(destDataTable, recordset);
            }
        }

    }
}
