using System;
using System.Collections.Generic;
using System.Xml;

using Krista.FM.Common.Xml;
using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.Modifications;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Classes
{
    internal class DimensionLevelCollection : ModifiableCollection<string, IDimensionLevel>, IDimensionLevelCollection
    {
        private HierarchyType hierarchyType;
        private readonly Classifier parent;
        private bool allHidden;

        internal DimensionLevelCollection(Classifier parent, ServerSideObjectStates state)
            : base(parent, state)
        {
            this.parent = parent;
        }

        internal Classifier Parent
        {
            get { return Instance.parent; }
        }

        /// <summary>
        /// Тип иерархии классификатора
        /// </summary>
        public HierarchyType HierarchyType
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return Instance.hierarchyType; }
            set
            {
                if (HierarchyType != value)
                {
                    SetInstance.hierarchyType = value;
                    parent.InitializeDefaultHierarchy();
                }

            }
        }

        /// <summary>
        /// ALL - фиктивный, по умолчанию falsе, в xml конфигурацию сохраняем, если только true
        /// </summary>
        public bool AllHidden
        {
            get { return allHidden; }
            set { allHidden = value; }
        }

        #region Инициализация

        private void InitializeParentChildHierarchy(XmlNode xmlNode)
        {
            this.Instance.HierarchyType = HierarchyType.ParentChild;
            this.Clear();

            // В измерениях которые делятся по источникам вне зависимости от настроек в XML делаем уровень ALL 
            // и называть его «Данные всех источников».
            if (Parent.IsDivided)
            {
                RegularLevel allLevel = new RegularLevel(Guid.Empty.ToString(), Parent);
                allLevel.Name = "Данные всех источников";
                allLevel.LevelType = LevelTypes.All;
                this.Add(allLevel.Name, allLevel);
            }
            else if (xmlNode.Attributes["allLevelName"] != null)
            {
                RegularLevel allLevel = new RegularLevel(Guid.Empty.ToString(), Parent);
                allLevel.Name = xmlNode.Attributes["allLevelName"].Value;
                allLevel.LevelType = LevelTypes.All;
                this.Add(allLevel.Name, allLevel);
            }

            IDataAttribute memberKey = Parent.Attributes[DataAttribute.IDColumnName/*xmlNode.Attributes["memberKey"].Value*/];
            if (memberKey == null)
                throw new Exception(String.Format("Ключ уровня {0} не определен в коллекции атрибутов.", xmlNode.Attributes["memberKey"].Value));

            IDataAttribute memberName = DataAttributeCollection.GetAttributeByKeyName(
                Parent.Attributes, 
                xmlNode.Attributes["memberName"].Value, 
                xmlNode.Attributes["memberName"].Value);

            if (memberName == null)
                throw new Exception(String.Format("Ключ уровня {0} не определен в коллекции атрибутов.", xmlNode.Attributes["memberName"].Value));

            IDataAttribute parentKey;
            string parentKeyName = "ParentID"/*xmlNode.Attributes["parentKey"].Value*/;
            if (parentKeyName == DataAttribute.ParentIDColumnName)
            {
                parentKey = DataAttribute.SystemParentID;
            }
            else
            {
                if (Parent.Attributes.ContainsKey(parentKeyName))
                    parentKey = Parent.Attributes[parentKeyName];
                else
                    throw new Exception(String.Format("Родительский ключ {0} не определен в коллекции атрибутов.", parentKeyName));
            }

            XmlNode xmlKeyNode = xmlNode.Attributes["objectKey"];
            string key = xmlKeyNode == null ? Guid.Empty.ToString() : xmlKeyNode.Value;

            if (key == Guid.Empty.ToString() && parent.State == ServerSideObjectStates.New)
            {
                key = Guid.NewGuid().ToString();
            }

            ParentChildLevel level = new ParentChildLevel(key, Parent);
            level.Name = xmlNode.Attributes["name"].Value;
            level.LevelType = LevelTypes.Regular;
            level.MemberKey = memberKey;
            level.MemberName = memberName;
            level.ParentKey = parentKey;
            if (xmlNode.Attributes["levelNamingTemplate"] != null)
                level.LevelNamingTemplate = xmlNode.Attributes["levelNamingTemplate"].Value;
            this.Add(KeyIdentifiedObject.GetKey(level.ObjectKey, level.Name), level);
        }

        private void InitializeRegularHierarchy(XmlNodeList xmlNodeList)
        {
            this.HierarchyType = HierarchyType.Regular;
            this.Clear();

            // Добавляем в классификатор родительский ключ, который будет использаваться
            // только при построении иерархии в интерфейсе классификаторов. 
            // При построении измерений в кубах использоваться не будет!

            bool allLevelPresent = false;
            foreach (XmlNode xmlLevel in xmlNodeList)
            {
                if (xmlLevel.Attributes["all"] != null)
                {
                    allLevelPresent = true;
                }
            }

            if (xmlNodeList.Count > 2 && allLevelPresent)
            {
                Parent.Attributes.Add(DataAttribute.SystemParentID);
            }
            else if (xmlNodeList.Count > 1 && !allLevelPresent)
            {
                Parent.Attributes.Add(DataAttribute.SystemParentID);
            }

            // обрабатываем уровень All плоской иерархии
            foreach (XmlNode xmlLevel in xmlNodeList)
            {
                if (xmlLevel.Attributes["all"] != null)
                {
                    XmlNode xmlKeyNode = xmlLevel.Attributes["objectKey"];
                    string key = xmlKeyNode == null ? Guid.Empty.ToString() : xmlKeyNode.Value;
                    if (state == ServerSideObjectStates.New && key == Guid.Empty.ToString())
                        key = Guid.NewGuid().ToString();

                    RegularLevel allLevel = new RegularLevel(key, Parent);
                    allLevel.Name = xmlLevel.Attributes["all"].Value;
                    allLevel.LevelType = LevelTypes.All;
                    this.Add(KeyIdentifiedObject.GetKey(allLevel.ObjectKey, allLevel.Name), allLevel);
                }
            }

            // обрабатываем остальные уровни плоской иерархии
            int levelNo = 1;
            foreach (XmlNode xmlLevel in xmlNodeList)
            {
                if (xmlLevel.Attributes["all"] == null)
                {
                    string memberKeyName;
                    string memberNameName;

                    XmlNode xmlKeyNode = xmlLevel.Attributes["objectKey"];
                    string key = xmlKeyNode == null ? Guid.Empty.ToString() : xmlKeyNode.Value;
                    if (state == ServerSideObjectStates.New && key == Guid.Empty.ToString())
                        key = Guid.NewGuid().ToString();

                    // TODO: Доработать именовку уровней Level1..N Name1..N
                    string levelName = xmlLevel.Attributes["name"].Value;

                    if (xmlLevel.Attributes["memberKey"] == null)
                        memberKeyName = "Level" + levelNo;
                    else
                    {
                        memberKeyName = xmlLevel.Attributes["memberKey"].Value;
                        if (DataAttributeCollection.GetAttributeByKeyName(Parent.Attributes, memberKeyName, memberKeyName) == null)
                        {
                            bool attributeExist = false;
                            foreach (IDataAttribute item in Parent.Attributes.Values)
                            {
                                if (item.Name == memberKeyName)
                                {
                                    attributeExist = true;
                                    break;
                                }
                            }
                            if (!attributeExist)
                                throw new Exception(String.Format("Уровень \"{0}\" содержит неверный ключ.", levelName));
                        }
                    }

                    //if (!Parent.Attributes.ContainsKey(memberKeyName))
                    //{
                    //    DataAttribute dataAttribute = EntityDataAttribute.CreateAttribute(memberKeyName, Parent, AttributeClass.Regular, Parent.State);
                    //    dataAttribute.Class = DataAttributeClassTypes.Fixed;
                    //    dataAttribute.Type = DataAttributeTypes.dtInteger;
                    //    dataAttribute.Caption = levelName + ".Код";
                    //    Parent.Attributes.Add(dataAttribute);
                    //}

                    if (xmlLevel.Attributes["memberName"] == null)
                        memberNameName = "Name" + levelNo++;
                    else
                    {
                        memberNameName = xmlLevel.Attributes["memberName"].Value;
                        if (!Parent.Attributes.ContainsKey(memberNameName))
                        {
                            bool attributeExist = false;
                            foreach (IDataAttribute item in Parent.Attributes.Values)
                            {
                                if (item.Name == memberNameName)
                                {
                                    attributeExist = true;
                                    break;
                                }
                            }
                            if (!attributeExist)
                                throw new Exception(String.Format("Уровень \"{0}\" содержит неверный memberName.", levelName));
                        }
                    }

                    //if (!Parent.Attributes.ContainsKey(memberNameName))
                    //{
                    //    DataAttribute dataAttribute = EntityDataAttribute.CreateAttribute(memberNameName, Parent, AttributeClass.Regular, Parent.State);
                    //    dataAttribute.Class = DataAttributeClassTypes.Fixed;
                    //    dataAttribute.Type = DataAttributeTypes.dtInteger;
                    //    dataAttribute.Caption = levelName + ".Имя";
                    //    Parent.Attributes.Add(dataAttribute);
                    //}

                    IDataAttribute memberKey = null;
                    string memberKeyKeyValue = xmlLevel.Attributes["memberKey"].Value;
                    if (Parent.Attributes.ContainsKey(memberKeyKeyValue))
                    {
                        memberKey = Parent.Attributes[memberKeyKeyValue];
                    }
                    else
                    {
                        foreach (IDataAttribute item in Parent.Attributes.Values)
                        {
                            if (item.Name == memberKeyKeyValue)
                            {
                                memberKey = item;
                                break;
                            }
                        }
                    }
                    if (memberKey == null)
                        throw new Exception(String.Format("Ключ уровня {0} не определен в коллекции атрибутов.", memberKeyName));

                    IDataAttribute memberName = null;
                    string memberNameKeyValue = xmlLevel.Attributes["memberName"].Value;
                    if (Parent.Attributes.ContainsKey(memberNameKeyValue))
                    {
                        memberName = Parent.Attributes[memberNameKeyValue];
                    }
                    else
                    {
                        foreach (IDataAttribute item in Parent.Attributes.Values)
                        {
                            if (item.Name == memberNameKeyValue)
                            {
                                memberName = item;
                                break;
                            }
                        }
                    }
                    if (memberName == null)
                        throw new Exception(String.Format("Ключ уровня {0} не определен в коллекции атрибутов.", xmlLevel.Attributes["memberName"].Value));

                    /*IDataAttribute memberName = Parent.Attributes[memberNameName];
                    if (memberName == null)
                        throw new Exception(String.Format("Ключ уровня {0} не определен в коллекции атрибутов.", memberNameName));
                    */
                    RegularLevel level = new RegularLevel(key, Parent);
                    level.Name = xmlLevel.Attributes["name"].Value;
                    level.LevelType = LevelTypes.Regular;
                    level.MemberKey = memberKey;
                    level.MemberName = memberName;
                    this.Add(KeyIdentifiedObject.GetKey(level.ObjectKey, level.Name), level);
                }
            }
        }

        /// <summary>
        /// Инициализация иерархии 
        /// </summary>
        /// <param name="xmlHierarchy"></param>
        internal void InitializeHierarchy(XmlNode xmlHierarchy)
        {
            this.Clear();

            XmlNode xmlNode;
            if (xmlHierarchy == null)
                return;
            if (xmlHierarchy.SelectSingleNode("AllHidden") != null)
                allHidden = Convert.ToBoolean(xmlHierarchy.SelectSingleNode("AllHidden").Value);

            xmlNode = xmlHierarchy.SelectSingleNode("ParentChild");
            // Деревянная иерархия
            if (xmlNode != null)
                InitializeParentChildHierarchy(xmlNode);
            else
                InitializeRegularHierarchy(xmlHierarchy.SelectNodes("Regular/Level"));
        }

        #endregion

        internal XmlNode HierarchySave2Xml(XmlNode node)
        {
            XmlNode xmlHierarchy = null;

            if (this.Count > 0)
            {
                xmlHierarchy = node.OwnerDocument.CreateNode(XmlNodeType.Element, "Hierarchy", null);

                if (allHidden)
                    XmlHelper.SetAttribute(xmlHierarchy, "AllHidden", "true");

                if (this.HierarchyType == HierarchyType.Regular)
                {
                    XmlNode xmlRegular = node.OwnerDocument.CreateNode(XmlNodeType.Element, "Regular", null);

                    foreach (DimensionLevel level in this.Values)
                    {
                        XmlNode xmlLevel = node.OwnerDocument.CreateNode(XmlNodeType.Element, "Level", null);

                        if (!String.IsNullOrEmpty(level.ObjectKey) && level.ObjectKey != Guid.Empty.ToString())
                        {
                            XmlHelper.SetAttribute(xmlLevel, "objectKey", level.ObjectKey);
                        }

                        if (level.LevelType == LevelTypes.All)
                        {
                            XmlHelper.SetAttribute(xmlLevel, "all", level.Name);
                        }
                        else
                        {
                            XmlHelper.SetAttribute(xmlLevel, "name", level.Name);
                            XmlHelper.SetAttribute(xmlLevel, "memberKey", level.MemberKey.Name);
                            XmlHelper.SetAttribute(xmlLevel, "memberName", level.MemberName.Name);
                        }

                        xmlRegular.AppendChild(xmlLevel);
                    }

                    xmlHierarchy.AppendChild(xmlRegular);
                }
                else
                {
                    XmlNode xmlParentChild = node.OwnerDocument.CreateNode(XmlNodeType.Element, "ParentChild", null);

                    DimensionLevel parentChildLevel = null;
                    DimensionLevel allLevel = null;
                    foreach (DimensionLevel level in this.Values)
                    {
                        if (allLevel == null && level.LevelType == LevelTypes.All)
                        {
                            allLevel = level;
                            continue;
                        }
                        
                        parentChildLevel = level;
                    }

                    if (!String.IsNullOrEmpty(parentChildLevel.ObjectKey) && parentChildLevel.ObjectKey != Guid.Empty.ToString())
                    {
                        XmlHelper.SetAttribute(xmlParentChild, "objectKey", parentChildLevel.ObjectKey);
                    }

                    XmlHelper.SetAttribute(xmlParentChild, "name", parentChildLevel.Name);
                    XmlHelper.SetAttribute(xmlParentChild, "memberKey", parentChildLevel.MemberKey.Name);
                    XmlHelper.SetAttribute(xmlParentChild, "memberName", parentChildLevel.MemberName.Name);
                    XmlHelper.SetAttribute(xmlParentChild, "parentKey", parentChildLevel.ParentKey.Name);

                    if (allLevel != null)
                        XmlHelper.SetAttribute(xmlParentChild, "allLevelName", allLevel.Name);

                    if (!String.IsNullOrEmpty(parentChildLevel.LevelNamingTemplate))
                        XmlHelper.SetAttribute(xmlParentChild, "levelNamingTemplate", parentChildLevel.LevelNamingTemplate);

                    xmlHierarchy.AppendChild(xmlParentChild);
                }
                node.AppendChild(xmlHierarchy);
            }
            return xmlHierarchy;
        }

        public IDimensionLevel CreateItem(string name, LevelTypes levelType)
        {
            DimensionLevel level;

            if (levelType == LevelTypes.All)
            {
                level = new RegularLevel(Guid.NewGuid().ToString(), Parent);
                level.Name = name;
                level.LevelType = levelType;
            }
            else if (HierarchyType == HierarchyType.ParentChild)
            {
                level = new ParentChildLevel(Guid.NewGuid().ToString(), Parent);
                level.Name = Parent.SemanticCaption;
                level.LevelType = LevelTypes.Regular;
                level.MemberKey = Parent.Attributes[DataAttribute.IDColumnName];
                level.MemberName = Parent.Attributes[DataAttribute.IDColumnName];
                level.ParentKey = Parent.Attributes[DataAttribute.ParentIDColumnName];
                level.LevelNamingTemplate = "Уровень 1; Уровень 2; Уровень 3; Уровень *";
            }
            else
            {
                level = new RegularLevel(Guid.NewGuid().ToString(), Parent);
                level.Name = name;
                level.LevelType = LevelTypes.Regular;
                level.MemberKey = Parent.Attributes[DataAttribute.IDColumnName];
                level.MemberName = Parent.Attributes[DataAttribute.IDColumnName];
            }

            this.Add(level.Name, level);
            return level;
        }

        #region IMinorModifiable Members

        public void Update(ModificationContext context, IModifiable toObject)
        {
            DimensionLevelCollection toHierarchy = (DimensionLevelCollection)toObject;
            XmlNode xmlHierarchyNode = toHierarchy.HierarchySave2Xml((new XmlDocument()).CreateElement("Dummy"));

            // Применяем изменения к базе данных

            if (AllHidden != toHierarchy.AllHidden)
                AllHidden = toHierarchy.AllHidden;

            if (this.HierarchyType != toHierarchy.HierarchyType)
            {
                if (toHierarchy.HierarchyType == HierarchyType.ParentChild)
                {
                    this.InitializeHierarchy(xmlHierarchyNode);

                    // Добавляем атрибут ParentID
                    if (!parent.Attributes.ContainsKey(DataAttribute.ParentIDColumnName))
                        parent.Attributes.Add(DataAttribute.SystemParentID);

                    SchemeClass.Instance.DDLDatabase.RunScript(((EntityDataAttribute)DataAttribute.SystemParentID).AddScript(parent, true, false).ToArray());
                    Trace.WriteLine("Атрибут ParentID успешно добавлен.");

                    // Добавляем атрибут CubeParentID
                    if (parent.IsDivided)
                    {
                        bool cubeParentIDExists = parent.Attributes.ContainsKey(DataAttribute.CubeParentIDColumnName);
                        if (!cubeParentIDExists)
                            parent.Attributes.Add(DataAttribute.SystemCubeParentID);

                        SchemeClass.Instance.DDLDatabase.RunScript(
                            ((EntityDataAttribute)DataAttribute.SystemCubeParentID).AddScript(parent, true, false).ToArray(), 
                            true/*cubeParentIDExists*/);// TODO: Убрать глушение ошибок. Атрибут CubeParentID уже может существовать в базе, т.к. он мог быть добавлен при установки деления по источникам.
                        Trace.WriteLine("Атрибут CubeParentID успешно добавлен.");
                    }
                    string[] dependentScripts = parent.GetDependentScripts();
                    SchemeClass.Instance.DDLDatabase.RunScript(dependentScripts);
                }
                else
                {
                    this.InitializeHierarchy(xmlHierarchyNode);

                    string[] script = ((EntityDataAttribute)DataAttribute.SystemParentID).DropScript(parent).ToArray();
                    SchemeClass.Instance.DDLDatabase.RunScript(script);
                    if (parent.Attributes.ContainsKey(DataAttribute.ParentIDColumnName))
                        parent.Attributes.Remove(DataAttribute.ParentIDColumnName);
                    Trace.WriteLine("Атрибут ParentID успешно удален.");

                    // Удаляем атрибут CubeParentID
                    if (parent.IsDivided)
                    {
                        script = ((EntityDataAttribute)DataAttribute.SystemCubeParentID).DropScript(parent).ToArray();
                        SchemeClass.Instance.DDLDatabase.RunScript(script);
                        if (parent.Attributes.ContainsKey(DataAttribute.CubeParentIDColumnName))
                            parent.Attributes.Remove(DataAttribute.CubeParentIDColumnName);
                        Trace.WriteLine("Атрибут CubeParentID успешно удален.");
                    }
                }
            }

            // Применяем изменения к объекту
            this.Clear();
            foreach (DimensionLevel level in ((DimensionLevelCollection)toObject).Values)
            {
                if (level.State == ServerSideObjectStates.New)
                    level.State = ServerSideObjectStates.Consistent;
                level.IsClone = false;
                this.Add(KeyIdentifiedObject.GetKey(level.ObjectKey, level.Name), level);
            }
        }

        #endregion

        #region IModifiable Members

        public IModificationItem GetChanges(IModifiable toObject)
        {
            bool changed = false;

            DimensionLevelCollection toHierarchy = (DimensionLevelCollection)toObject;

            if (this.AllHidden != toHierarchy.AllHidden)
                changed = true;

            if (this.HierarchyType != toHierarchy.HierarchyType)
                changed = true;
            else
            {
                foreach (DimensionLevel level in this.Values)
                {
                    if (!toHierarchy.ContainsKey(KeyIdentifiedObject.GetKey(level.ObjectKey, level.Name)))
                    {
                        changed = true;
                        foreach (IDimensionLevel item in toHierarchy.Values)
                        {
                            if (item.Name == level.Name)
                            {
                                changed = false;
                            }
                        }
                        if (changed)
                            break;
                    }
                    
                    if (level.CloneObject != null && !toHierarchy.ContainsKey(
                        KeyIdentifiedObject.GetKey(((DimensionLevel)level.CloneObject).ObjectKey, ((DimensionLevel)level.CloneObject).Name)))
                    {
                        changed = true;
                        break;
                    }

                    IDimensionLevel toLevel = null;
                    if (level.ObjectKey == Guid.Empty.ToString() || !toHierarchy.ContainsKey(level.ObjectKey))
                    {
                        foreach (IDimensionLevel itemLevel in toHierarchy.Values)
                        {
                            if (itemLevel.Name == level.Name)
                            {
                                toLevel = itemLevel;
                                break;
                            }
                        }
                    }
                    else
                    {
                        toLevel = toHierarchy[level.ObjectKey];
                    }

                    if (toLevel == null)
                        throw new Exception("[БАГ]");
                    
                    if (level.CompareTo(toLevel) != 0)
                    {
                        changed = true;
                        break;
                    }
                }

                if (this.Count != toHierarchy.Count)
                    changed = true;
            }

            if (changed)
                return new HierarchyModificationItem("Модификация иерархии", this, toObject);
            else
                return null;
        }

        #endregion

        #region ServerSideObject

        /// <summary>
        /// Возвращает экземпляр объекта с которым должен работать текущий пользователь
        /// </summary>
        /// <returns>Экземпляр объекта</returns>
        protected new DimensionLevelCollection Instance
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return (DimensionLevelCollection)GetInstance(); }
        }

        /// <summary>
        /// Возвращает экземпляр объекта с которым должен работать текущий пользователь, для установки значений свойств
        /// </summary>
        /// <returns>Экземпляр объекта</returns>
        protected DimensionLevelCollection SetInstance
        {
            get
            {
                if (SetterMustUseClone())
                    return (DimensionLevelCollection)CloneObject;
                else
                    return this;
            }
        }

        public override IServerSideObject Lock()
        {
            Classifier cloneClassifier = (Classifier)Owner.Lock();
            return (ServerSideObject)cloneClassifier.Levels;
        }

        #endregion ServerSideObject
    }
}
