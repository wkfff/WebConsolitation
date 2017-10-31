using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml;
using System.Data;
using System.Text;
using Krista.FM.Common;
using Krista.FM.Common.Xml;
using Krista.FM.Server.Common;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Providers.Planing
{
    /// <summary>
    /// обёртка на провайдер планирования для скрытия реализации работы с 
    /// двумя многомерными базами
    /// </summary>
    [ComVisible(false)]
    public class PlaningProviderWrapper : DisposableObject, IPlaningProvider
    {
        #region fields

        private readonly IScheme scheme;
        private static readonly List<string> connectionsList = new List<string>();
        private static string metadataDate;
        private readonly List<PlaningProvider> providersPool;

        
        #endregion

        #region public methods

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="scheme"></param>
        /// формата "имя сервера|имя базы"</param>
        public PlaningProviderWrapper(IScheme scheme)
        {
            this.scheme = scheme;
            providersPool = new List<PlaningProvider>();
            Trace.TraceInformation("planing provider wrapper is creating");
            
            if (connectionsList.Count == 0)
                for (int i = 0; ; i++)
                {
                    string udlFileName;
                    string cacheFolder;
                    if (i == 0)
                    {
                        udlFileName = string.Format("{0}\\MAS.UDL", scheme.BaseDirectory);
                        cacheFolder = scheme.BaseDirectory + "\\Cache\\";
                    }
                    else
                    {
                        udlFileName = string.Format("{0}\\MAS{1}.UDL", scheme.BaseDirectory, i);
                        cacheFolder = scheme.BaseDirectory + "\\Cache" + i.ToString() + "\\";
                    }

                    if (!File.Exists(udlFileName))
                        break;

                    connectionsList.Add(udlFileName);
                    InitCache(cacheFolder);
                }

            for (int i = 0; i < connectionsList.Count; i++)
            {
                PlaningProvider pp = new PlaningProvider(scheme, connectionsList[i], i.ToString());
                providersPool.Add(pp);
            }
        }

        
        #endregion

        #region private methods

        [DebuggerStepThrough]
        private PlaningProvider ProviderInstance(int providerId)
        {
            return providersPool[providerId];
        }

        /// <summary>
        /// выполняет очистку указанного каталога
        /// </summary>
        /// <param name="cacheFolder"></param>
        private void InitCache(string cacheFolder)
        {
            if (Directory.Exists(cacheFolder))
            {
                bool error = false;
                string errorText = string.Empty;
                string[] filesList = Directory.GetFiles(cacheFolder);
                foreach (string fileName in filesList)
                    try
                    {
                        File.Delete(fileName);
                    }
                    catch (Exception e)
                    {
                        if (!error)
                        {
                            errorText = Diagnostics.KristaDiagnostics.ExpandException(e);
                            error = true;
                        }
                    }

                if (error)
                {
                   // throw new ServerException("При очищении серверного кэша произошла ошибка.");
                   Trace.TraceError(string.Format("При очищении серверного кэша произошла ошибка: {0}", errorText));
                }
            }
            else
                Directory.CreateDirectory(cacheFolder);
        }

        private ITask GetTask(int taskId)
        {
            ITask task;
            try
            {
                task = scheme.TaskManager.Tasks[taskId];
            }

            catch (Exception e)
            {
                return null;
            }
           
            if (!task.LockByCurrentUser())
                return null;

            if (task.Doer != Authentication.UserID &&
                !scheme.UsersManager.CheckPermissionForTask(task.ID, task.RefTasksTypes, (int)TaskOperations.View, false))
            {
                return null;
            }

            return task;
        }

        private static bool UpdateTaskParamProperties(ref ITaskParam param, string paramName, string paramValue, 
            string paramComment, bool multiselection)
        {
            bool result = param.Name != paramName;
            param.Name = paramName;

            result = result || (string) param.Values != paramValue;
            param.Values = paramValue;

            result = result || param.Comment != paramComment;
            param.Comment = paramComment;

            result = result || param.AllowMultiSelect != multiselection;
            param.AllowMultiSelect = multiselection;

            return result;
        }

        private static bool UpdateTaskConstProperties(ref ITaskConst taskConst, string constName, string constValue,
            string constComment)
        {
            bool result = taskConst.Name != constName;
            taskConst.Name = constName;

            result = result || (string) taskConst.Values != constValue;
            taskConst.Values = constValue;

            result = result || taskConst.Comment != constComment;
            taskConst.Comment = constComment;

            return result;
        }

        /// <summary>
        /// Если параметр с таким именем уже присутствует в коллекции, имя надо сменить
        /// </summary>
        /// <param name="taskParams"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        private static string ModifyParamName(ITaskParamsCollection taskParams, string paramName)
        {
            for (int i = 1; ; i++)
            { 
                string newName = paramName + '(' + i.ToString() + ')';
                ITaskParam param = taskParams.ParamByName(newName);
                if (param == null)
                    return newName;
            }            
        }

        private string ModifyConstName(ITaskConstsCollection taskConsts, string constName)
        {
            for (int i = 1; ; i++)
            {
                string newName = constName + '(' + i.ToString() + ')';
                ITaskConst taskConst = taskConsts.ConstByName(newName);
                if (taskConst == null)
                    return newName;
            }
        }

        /// <summary>
        /// Формирует результирующую строку для методов UpdateTaskParams и UpdateTaskConsts
        /// </summary>
        /// <param name="result"></param>
        /// <param name="oldId"></param>
        /// <param name="newId"></param>
        /// <param name="divider"></param>
        private static void MakeUpdatingResult(ref string result, int oldId, int newId, string divider)
        {
            if (result != string.Empty)
                result += divider;
            result += string.Format("{0}={1}", oldId, newId);
        }

        #endregion
            
        #region IPlaningProvider Members

        public string GetMetadataDate()
        {
            return metadataDate;
        }

        public string GetMetaData()
        {
            // за основу берем метаданные первичного подключения 
            XmlDocument resultDom = new XmlDocument();
            string xmlString = ProviderInstance(0).GetMetaData();
            bool metadataUpdated = ProviderInstance(0).MetadataUpdated;
            resultDom.LoadXml(xmlString);

            // дописываем данные остальных подключений
            for (int i = 1; i < connectionsList.Count; i++)
            {
                XmlDocument tmpDom = new XmlDocument();
                xmlString = ProviderInstance(i).GetMetaData();
                metadataUpdated = metadataUpdated || ProviderInstance(i).MetadataUpdated;
                tmpDom.LoadXml(xmlString);
                MergeMetaDataXml(resultDom, tmpDom);
            }

            // дополнительно создадим секцию с описаниями подключений
            XmlNode root = resultDom.CreateElement("Providers");
            for (int i = 0; i < connectionsList.Count; i++)
            {
                PlaningProvider pp = ProviderInstance(i);
                XmlHelper.AddChildNode(root, "Provider",
                        new string[] { "providerId", pp.ProviderId },
                        new string[] { "DataSource", pp.DataSource },
                        new string[] { "Catalog", pp.Catalog });
            }
            if (resultDom.DocumentElement != null)
                resultDom.DocumentElement.AppendChild(root);

            // Если было реальное обновление, то обновим дату
            if (metadataUpdated)
                metadataDate = DateTime.Now.ToString();
            XmlNode xmlNode = resultDom.SelectSingleNode("function_result");
            XmlHelper.SetAttribute(xmlNode, "date", metadataDate);

            return resultDom.InnerXml;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dom">результирующий документ, в который помещаем новые узлы</param>
        /// <param name="tmpDom">поставщик новых узлов</param>
        private void MergeMetaDataXml(XmlDocument dom, XmlDocument tmpDom)
        {
            // общие измерения
            XmlNode root = dom.SelectSingleNode("function_result/SharedDimensions");
            if (root == null)
                root = XmlHelper.AddChildNode(dom.DocumentElement, "SharedDimensions");

            XmlNodeList nl = tmpDom.SelectNodes("function_result/SharedDimensions/Dimension");
            if ((nl != null) && (nl.Count > 0))
                for (int i = 0; i < nl.Count; i++)
                {
                    XmlNode node = dom.ImportNode(nl[i], true);
                    root.AppendChild(node);
                }

            // кубы
            root = dom.SelectSingleNode("function_result/Cubes");
            if (root == null)
                root = XmlHelper.AddChildNode(dom.DocumentElement, "Cubes");

            nl = tmpDom.SelectNodes("function_result/Cubes/Cube");
            if ((nl != null) && (nl.Count > 0))
                for (int i = 0; i < nl.Count; i++)
                {
                    XmlNode node = dom.ImportNode(nl[i], true);
                    root.AppendChild(node);
                }
        }


        public string GetMembers(string providerId, string cubeName, string dimensionName,
            string hierarchyName, string levelNames, string memberPropertiesNames)
        {
            try
            {
                PlaningProvider provider = ProviderInstance(Convert.ToInt32(providerId));
                return provider.GetMembers(cubeName, dimensionName, hierarchyName,
                    levelNames, memberPropertiesNames);
            }
            catch (Exception e)
            {
                return Diagnostics.KristaDiagnostics.ExpandException(e);
            }
        }

        public string GetRecordsetData(string providerId, string queryText)
        {
            try
            {
                PlaningProvider provider = ProviderInstance(Convert.ToInt32(providerId));
                return provider.GetRecordsetData(queryText);
            }
            catch (Exception e)
            {
                return Diagnostics.KristaDiagnostics.ExpandException(e);
            }
        }

        public string GetCellsetData(string providerId, string queryText)
        {
            try
            {
                PlaningProvider provider = ProviderInstance(Convert.ToInt32(providerId));
                return provider.GetCellsetData(queryText);
            }
            catch (Exception e)
            {
                return Diagnostics.KristaDiagnostics.ExpandException(e);
            }
        }

        public void RefreshDimension(string providerId, string[] names)
        {
            try
            {
                PlaningProvider provider = ProviderInstance(Convert.ToInt32(providerId));
                provider.RefreshDimension(names);
                // обновим дату
                metadataDate = DateTime.Now.ToString();
            }
            catch (Exception)
            {

            }
        }

        public void RefreshCube(string providerId, string[] names)
        {
            try
            {
                PlaningProvider provider = ProviderInstance(Convert.ToInt32(providerId));
                provider.RefreshCube(names);
                // обновим дату
                metadataDate = DateTime.Now.ToString();
            }
            catch (Exception)
            {

            }
        }

        public string RefreshMetaData()
        {
            for (int i = 0; i < connectionsList.Count; i++)
            {
                ProviderInstance(i).ClearCache();
            }
            return GetMetaData();
        }

        /// <summary>
        /// Обеспечивает механизм синхронизации параметров в документе с задачей
        /// </summary>
        /// <param name="taskId">айди задачи</param>
        /// <param name="paramsText">строка с последовательно прописанными наборами свойств параметров</param>
        /// <param name="sectionDivider">разделитель параметров</param>
        /// <param name="valuesDivider">разделитель свойств параметра внутри блока</param>
        /// <returns>возвращает строку пар вида OldParamId=NewParamId, разделенных sectionDivider-ом</returns>
        public string UpdateTaskParams(int taskId, string paramsText, string sectionDivider, string valuesDivider)
        {
            ITask task = GetTask(taskId);
            if ((task == null) || (paramsText == null))
                return string.Empty;

            ITaskParamsCollection collection = task.GetTaskParams();
            string[] paramsDivided = paramsText.Split(new string[] { sectionDivider }, StringSplitOptions.RemoveEmptyEntries);
            string result = string.Empty;
            bool needSave = false;

            foreach (string paramText in paramsDivided)
            {
                string[] values = paramText.Split(new string[] { valuesDivider }, StringSplitOptions.None);

                int paramId = Convert.ToInt32(values[0]);
                string paramName = values[1];
                string paramValue = values[2];
                string paramDimension = values[3];
                string paramComment = values[4];
                bool multiselection = Convert.ToBoolean(values[5]);

                ITaskParam param;
                if (paramId >= 0) // положительные значения айди говорят, что параметр был связан с задачей
                {
                    // Имя параметра - главный критерий. Все неувязки с соответствием имен и айди были разрешены
                    // на стороне надстройки при загрузке контекста. В этот метод мы попадаем только в случаях
                    // добавления нового параметра или обновления существующего, айди тут не решает.
                    param = collection.ParamByName(paramName);
                    if (param != null)
                    {
                        // Измерения у одинаковых параметров *обязаны* совпадать
                        if (paramDimension == param.Dimension)
                        {
                            needSave = UpdateTaskParamProperties(ref param, paramName, paramValue, paramComment, multiselection) || needSave;
                            continue;
                        }
                        // Если измерения не совпали, то дадим параметру из листа новое имя и добавим его в коллекцию
                        paramName = ModifyParamName(collection, paramName);
                    }
                    else
                    {
                        // Случай, когда у существующего параметра поменяли имя *из листа*
                        // По имени мы его не нашли, но айди-то остался прежним. Ищем по айди 
                        // и переименовываем в задаче.
                        param = collection.ParamByID(paramId);
                        if (param != null)
                        {
                            if (paramDimension == param.Dimension)
                            {
                                UpdateTaskParamProperties(ref param, paramName, paramValue, paramComment, multiselection);
                                needSave = true;
                                MakeUpdatingResult(ref result, paramId, param.ID, sectionDivider);
                                continue;
                            }
                            // Не совпадает ни имя, ни измерение, а вот айди тот же. 
                            // Вероятно, измерение параметру поменяли ручками, в воркплейсе. Синхронизация в листе отработала переименованием
                            // и надо добавить параметр как новый.
                        }
                    }
                    // Если такого имени в коллекции нет - это будет новый параметр
                }
                else // отрицательные значения айди говорят, что параметр был локальным в документе
                {
                    // параметр был локальным в листе, поищем такой в задаче по имени
                    // если нашли, то параметру из листа надо присвоить айди параметра из задачи
                    param = collection.ParamByName(paramName);
                    if (param != null)
                    {
                        if (paramDimension == param.Dimension)
                        {
                            MakeUpdatingResult(ref result, paramId, param.ID, sectionDivider);
                            continue;
                        }
                        // Если измерения не совпали, то дадим параметру из листа новое имя и добавим его в коллекцию
                        paramName = ModifyParamName(collection, paramName);
                    }
                }

                //Нет такого параметра - создаем
                param = collection.AddNew();
                param.Dimension = paramDimension;
                UpdateTaskParamProperties(ref param, paramName, paramValue, paramComment, multiselection);
                needSave = true;
                MakeUpdatingResult(ref result, paramId, param.ID, sectionDivider);
            }

            if (needSave)
                collection.SaveChanges();
            return result;
        }

        /// <summary>
        /// Обеспечивает механизм синхронизации констант в документе с задачей
        /// </summary>
        /// <param name="taskId">айди задачи</param>
        /// <param name="constsText">строка с последовательно прописанными наборами свойств констант</param>
        /// <param name="sectionDivider">разделитель констант</param>
        /// <param name="valuesDivider">разделитель свойств константы внутри блока</param>
        /// <returns>возвращает строку пар вида OldParamId=NewParamId, разделенных sectionDivider-ом</returns>
        public string UpdateTaskConsts(int taskId, string constsText, string sectionDivider, string valuesDivider)
        {
            ITask task = GetTask(taskId);
            if ((task == null) || (constsText == null))
                return string.Empty;

            ITaskConstsCollection collection = task.GetTaskConsts();
            string[] constsDivided = constsText.Split(new string[] { sectionDivider }, StringSplitOptions.RemoveEmptyEntries);
            string result = string.Empty;
            bool needSave = false;

            foreach (string constText in constsDivided)
            {
                string[] values = constText.Split(new string[] { valuesDivider }, StringSplitOptions.None);

                int constId = Convert.ToInt32(values[0]);
                string constName = values[1];
                string constValue = values[2];
                string constComment = values[3];

                ITaskConst taskConst;
                if (constId >= 0) // отрицательные значения говорят, что константа была локальной в документе
                {
                    //Если находим константу по айди - значит требуется только обновить ее свойства
                    taskConst = collection.ConstByID(constId);
                    if (taskConst != null)
                    {
                        needSave = UpdateTaskConstProperties(ref taskConst, constName, constValue, constComment) || needSave;
                        continue;
                    }

                    //Айди разные, но константа с таким именем есть - с ней и надо работать.
                    //Делать ничего не надо, при следующем обращении листа к контексту произойдет синхронизация
                    taskConst = collection.ConstByName(constName);
                    if (taskConst != null)
                    {
                        continue;
                    }
                }
                else
                {
                    taskConst = collection.ConstByName(constName);
                    if (taskConst != null)
                    {
                        MakeUpdatingResult(ref result, constId, taskConst.ID, sectionDivider);
                        continue;
                    }
                }

                //Нет такой контстанты - создаем
                taskConst = collection.AddNew();
                UpdateTaskConstProperties(ref taskConst, constName, constValue, constComment);
                needSave = true;
                MakeUpdatingResult(ref result, constId, taskConst.ID, sectionDivider);
            }

            if (needSave)
                collection.SaveChanges();
            
            return result;
        }

       public string GetTaskContext(int taskId)
        {
            ITask task;
            try
            {
                task = scheme.TaskManager.Tasks[taskId];
            }

            catch (Exception e)
            {
                return Diagnostics.KristaDiagnostics.ExpandException(e);
            }

            
            DataSet ds = new DataSet("TaskContext");

            task.GetTaskConsts().ReloadItemsTable();
            DataTable dt = task.GetTaskConsts().ItemsTable;
            dt.TableName = "Constant";
            dt.Columns.Remove("PARAMTYPE");
            ds.Tables.Add(dt);

            task.GetTaskParams().ReloadItemsTable();
            dt = task.GetTaskParams().ItemsTable;
            dt.TableName = "Parameter";
            dt.Columns.Remove("PARAMTYPE");
            ds.Tables.Add(dt);

            using (MemoryStream stream = new MemoryStream())
            {
                StreamWriter sw = new StreamWriter(stream, Encoding.UTF8);
                ds.WriteXml(sw);
                stream.Seek(0, SeekOrigin.Begin);
                StreamReader sr = new StreamReader(stream);
                return sr.ReadToEnd();
            }
        }
        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            lock (this)
            {
                if (disposing)
                {
                    Trace.TraceInformation("planing provider wrapper is disposing");
                    for (int i = 0; i < providersPool.Count; i++)
                    {
                        providersPool[i].Dispose();
                    }
                }
            }
        }

        #endregion
    }
}
