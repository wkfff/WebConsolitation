using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;

using Krista.FM.Update.Framework.Conditions;
using Krista.FM.Update.Framework.UpdateObjects;

namespace Krista.FM.Update.Framework.FeedReaders
{
    /// <summary>
    /// Задачи и условия ограничений в файле с описанием патчей, должны соответствовать именам реализованных классов
    /// </summary>
    [Serializable]
    public class XmlFeedReader : IUpdateFeedReader
    {
        /// <summary>
        /// Поддерживаемые типы условий
        /// </summary>
        private Dictionary<string, Type> _updateConditions { get; set; }
        /// <summary>
        /// Поддерживаемы типы задач
        /// </summary>
        private Dictionary<string, Type> _updateTasks { get; set; }

        public XmlFeedReader()
        {
            _updateConditions = new Dictionary<string, Type>();
            _updateTasks = new Dictionary<string, Type>();

            foreach (Type t in this.GetType().Assembly.GetTypes())
            {
                if (typeof(IUpdateTask).IsAssignableFrom(t))
                {
                    _updateTasks.Add(t.Name, t);
                }
                else if (typeof(IUpdateCondition).IsAssignableFrom(t))
                {
                    _updateConditions.Add(t.Name, t);
                }
            }
        }

        #region IUpdateFeedReader Members

        /// <summary>
        /// Получает полный список задач обновления разделенный по патчам
        /// </summary>
        /// <param name="feed"></param>
        /// <returns></returns>
        public IList<IUpdatePatch> Read(string feed, IUpdateFeed updateFeed)
        {
            var patches = new List<IUpdatePatch>();
            return ReadFeed(feed, patches, updateFeed);
        }
        
        private IList<IUpdatePatch> ReadFeed(string feed, List<IUpdatePatch> patches, IUpdateFeed updateFeed)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(feed);

                XmlNode root = doc.SelectSingleNode(@"/Patches[version=""1.0""] | /Patches");
                if (root == null) return patches;

                XmlNodeList nlPatches = root.SelectNodes("./Patch");

                foreach (XmlNode patchNode in nlPatches)
                {
                    ReadPatchTag(patchNode, patches, updateFeed);
                }

                Trace.TraceVerbose(String.Format("Патчей прочитано: {0}", patches.Count));
                return patches;
            }
            catch (Exception e)
            {
                Trace.TraceError(
                    String.Format("При загрузке канала обновления возникло исключение: {0}", e.Message));
                return patches;
            }
        }

        private void ReadPatchTag(XmlNode patchNode, List<IUpdatePatch> patches, IUpdateFeed updateFeed)
        {
            List<IUpdateTask> ret = new List<IUpdateTask>();

            string baseUrl = null;
            if (patchNode.Attributes["baseUrl"] != null && !string.IsNullOrEmpty(patchNode.Attributes["baseUrl"].Value))
                baseUrl = patchNode.Attributes["baseUrl"].Value;

            string description = (patchNode.Attributes["description"] != null)
                                     ? patchNode.Attributes["description"].Value
                                     : String.Empty;

            string detailDescription = (patchNode.Attributes["descriptionDetail"] != null)
                                     ? patchNode.Attributes["descriptionDetail"].Value
                                     : String.Empty;

            string name = (patchNode.Attributes["name"] != null)
                                 ? patchNode.Attributes["name"].Value
                                 : String.Empty;

            string objectKey = (patchNode.Attributes["objectKey"] != null)
                                 ? patchNode.Attributes["objectKey"].Value
                                 : String.Empty;

            Use use = ConvertToUse(patchNode.Attributes["use"]);

            XmlNodeList nl = patchNode.SelectNodes("./Tasks/*");

            IUpdatePatch updatePatch = new UpdatePatch(objectKey, name, description, detailDescription, use, baseUrl, updateFeed);
            if (patchNode.HasChildNodes)
            {
                // Считываем условия обновления
                if (patchNode["Conditions"] != null)
                {
                    IUpdateCondition conditionObject = ReadCondition(patchNode["Conditions"]);
                    if (conditionObject != null)
                    {
                        if (conditionObject is BooleanCondition)
                            updatePatch.UpdateConditions = conditionObject as BooleanCondition;
                        else
                            updatePatch.UpdateConditions.AddCondition(conditionObject);
                    }
                }
            }

            foreach (XmlNode node in nl)
            {
                // Проверяем, поддерживается или нет тип задачи
                if (!_updateTasks.ContainsKey(node.Name))
                    continue;

                IUpdateTask task = (IUpdateTask)Activator.CreateInstance(_updateTasks[node.Name], updatePatch);

                // Создаем атрибуты задачи
                foreach (XmlAttribute att in node.Attributes)
                {
                    if ("type".Equals(att.Name))
                        continue;

                    task.Attributes.Add(att.Name, att.Value);
                }

                if (node.HasChildNodes)
                {
                    if (node["Description"] != null)
                        task.Description = node["Description"].InnerText;

                    // Считываем условия обновления
                    if (node["Conditions"] != null)
                    {
                        IUpdateCondition conditionObject = ReadCondition(node["Conditions"]);
                        if (conditionObject != null)
                        {
                            if (conditionObject is BooleanCondition)
                                task.UpdateConditions = conditionObject as BooleanCondition;
                            else
                                task.UpdateConditions.AddCondition(conditionObject);
                        }
                    }
                }

                ret.Add(task);
            }

            updatePatch.Tasks = ret;
            patches.Add(updatePatch);
        }

        private static Use ConvertToUse(XmlAttribute xmlAttribute)
        {
            if (xmlAttribute == null)
                throw new ReadFeedException("Не указан тег обязательности патча");

            switch (xmlAttribute.Value.ToLower())
            {
                case "required":
                    return Use.Required;
                case "optional":
                    return Use.Optional;
                case "prohibited":
                    return Use.Prohibited;
            }

            throw new ReadFeedException(string.Format("Не известный тип обязательности: {0}", xmlAttribute.Value));
        }

        private IUpdateCondition ReadCondition(XmlNode cnd)
        {
            IUpdateCondition conditionObject = null;
            if (cnd.ChildNodes.Count > 0 || "GroupCondition".Equals(cnd.Name))
            {
                BooleanCondition bc = new BooleanCondition();
                foreach (XmlNode child in cnd.ChildNodes)
                {
                    IUpdateCondition childCondition = ReadCondition(child);
                    if (childCondition != null)
                        bc.AddCondition(childCondition, BooleanCondition.ConditionTypeFromString(child.Attributes["type"] == null ? null : child.Attributes["type"].Value));
                }
                if (bc.ChildConditionsCount > 0)
                    conditionObject = bc.Degrade();
            }
            else if (_updateConditions.ContainsKey(cnd.Name))
            {
                conditionObject = (IUpdateCondition)Activator.CreateInstance(_updateConditions[cnd.Name]);

                // Считывает атрибуты условия
                foreach (XmlAttribute att in cnd.Attributes)
                {
                    if ("type".Equals(att.Name))
                        continue;

                    conditionObject.Attributes.Add(att.Name, att.Value);
                }
            }
            return conditionObject;
        }

        #endregion
    }
}
