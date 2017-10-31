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
    /// ������ �� ��������� ������������ ��� ������� ���������� ������ � 
    /// ����� ������������ ������
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
        /// �����������
        /// </summary>
        /// <param name="scheme"></param>
        /// ������� "��� �������|��� ����"</param>
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
        /// ��������� ������� ���������� ��������
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
                   // throw new ServerException("��� �������� ���������� ���� ��������� ������.");
                   Trace.TraceError(string.Format("��� �������� ���������� ���� ��������� ������: {0}", errorText));
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
        /// ���� �������� � ����� ������ ��� ������������ � ���������, ��� ���� �������
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
        /// ��������� �������������� ������ ��� ������� UpdateTaskParams � UpdateTaskConsts
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
            // �� ������ ����� ���������� ���������� ����������� 
            XmlDocument resultDom = new XmlDocument();
            string xmlString = ProviderInstance(0).GetMetaData();
            bool metadataUpdated = ProviderInstance(0).MetadataUpdated;
            resultDom.LoadXml(xmlString);

            // ���������� ������ ��������� �����������
            for (int i = 1; i < connectionsList.Count; i++)
            {
                XmlDocument tmpDom = new XmlDocument();
                xmlString = ProviderInstance(i).GetMetaData();
                metadataUpdated = metadataUpdated || ProviderInstance(i).MetadataUpdated;
                tmpDom.LoadXml(xmlString);
                MergeMetaDataXml(resultDom, tmpDom);
            }

            // ������������� �������� ������ � ���������� �����������
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

            // ���� ���� �������� ����������, �� ������� ����
            if (metadataUpdated)
                metadataDate = DateTime.Now.ToString();
            XmlNode xmlNode = resultDom.SelectSingleNode("function_result");
            XmlHelper.SetAttribute(xmlNode, "date", metadataDate);

            return resultDom.InnerXml;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dom">�������������� ��������, � ������� �������� ����� ����</param>
        /// <param name="tmpDom">��������� ����� �����</param>
        private void MergeMetaDataXml(XmlDocument dom, XmlDocument tmpDom)
        {
            // ����� ���������
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

            // ����
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
                // ������� ����
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
                // ������� ����
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
        /// ������������ �������� ������������� ���������� � ��������� � �������
        /// </summary>
        /// <param name="taskId">���� ������</param>
        /// <param name="paramsText">������ � ��������������� ������������ �������� ������� ����������</param>
        /// <param name="sectionDivider">����������� ����������</param>
        /// <param name="valuesDivider">����������� ������� ��������� ������ �����</param>
        /// <returns>���������� ������ ��� ���� OldParamId=NewParamId, ����������� sectionDivider-��</returns>
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
                if (paramId >= 0) // ������������� �������� ���� �������, ��� �������� ��� ������ � �������
                {
                    // ��� ��������� - ������� ��������. ��� �������� � ������������� ���� � ���� ���� ���������
                    // �� ������� ���������� ��� �������� ���������. � ���� ����� �� �������� ������ � �������
                    // ���������� ������ ��������� ��� ���������� �������������, ���� ��� �� ������.
                    param = collection.ParamByName(paramName);
                    if (param != null)
                    {
                        // ��������� � ���������� ���������� *�������* ���������
                        if (paramDimension == param.Dimension)
                        {
                            needSave = UpdateTaskParamProperties(ref param, paramName, paramValue, paramComment, multiselection) || needSave;
                            continue;
                        }
                        // ���� ��������� �� �������, �� ����� ��������� �� ����� ����� ��� � ������� ��� � ���������
                        paramName = ModifyParamName(collection, paramName);
                    }
                    else
                    {
                        // ������, ����� � ������������� ��������� �������� ��� *�� �����*
                        // �� ����� �� ��� �� �����, �� ����-�� ������� �������. ���� �� ���� 
                        // � ��������������� � ������.
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
                            // �� ��������� �� ���, �� ���������, � ��� ���� ��� ��. 
                            // ��������, ��������� ��������� �������� �������, � ����������. ������������� � ����� ���������� ���������������
                            // � ���� �������� �������� ��� �����.
                        }
                    }
                    // ���� ������ ����� � ��������� ��� - ��� ����� ����� ��������
                }
                else // ������������� �������� ���� �������, ��� �������� ��� ��������� � ���������
                {
                    // �������� ��� ��������� � �����, ������ ����� � ������ �� �����
                    // ���� �����, �� ��������� �� ����� ���� ��������� ���� ��������� �� ������
                    param = collection.ParamByName(paramName);
                    if (param != null)
                    {
                        if (paramDimension == param.Dimension)
                        {
                            MakeUpdatingResult(ref result, paramId, param.ID, sectionDivider);
                            continue;
                        }
                        // ���� ��������� �� �������, �� ����� ��������� �� ����� ����� ��� � ������� ��� � ���������
                        paramName = ModifyParamName(collection, paramName);
                    }
                }

                //��� ������ ��������� - �������
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
        /// ������������ �������� ������������� �������� � ��������� � �������
        /// </summary>
        /// <param name="taskId">���� ������</param>
        /// <param name="constsText">������ � ��������������� ������������ �������� ������� ��������</param>
        /// <param name="sectionDivider">����������� ��������</param>
        /// <param name="valuesDivider">����������� ������� ��������� ������ �����</param>
        /// <returns>���������� ������ ��� ���� OldParamId=NewParamId, ����������� sectionDivider-��</returns>
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
                if (constId >= 0) // ������������� �������� �������, ��� ��������� ���� ��������� � ���������
                {
                    //���� ������� ��������� �� ���� - ������ ��������� ������ �������� �� ��������
                    taskConst = collection.ConstByID(constId);
                    if (taskConst != null)
                    {
                        needSave = UpdateTaskConstProperties(ref taskConst, constName, constValue, constComment) || needSave;
                        continue;
                    }

                    //���� ������, �� ��������� � ����� ������ ���� - � ��� � ���� ��������.
                    //������ ������ �� ����, ��� ��������� ��������� ����� � ��������� ���������� �������������
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

                //��� ����� ���������� - �������
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
