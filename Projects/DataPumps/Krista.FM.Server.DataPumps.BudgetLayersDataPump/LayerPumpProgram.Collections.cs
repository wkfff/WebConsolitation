// ******************************************************************
// Модуль содержит реализацию списков и коллекций используемых 
// для представления объектов программ закачек
// ******************************************************************

using System;
using System.Collections.Generic;
using System.Data;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.BudgetLayersDataPump
{
    #region Коллекции и списки

    #region LayersCollection
    /// <summary>
    /// Коллекция используемых слоев
    /// </summary>
    public sealed class LayersCollection : ObjectsCollection
    {
        public LayersCollection(PumpProgram parentProgram)
            : base(parentProgram, XmlConsts.layerNodeTag, typeof(Layer))
        {
        }

        public void LoadLayersFromDir(string dir)
        {
            foreach (Layer lay in this)
            {
                lay.LoadFromDir(dir);
            }
        }
    }
    #endregion

    #region FMObjectsCollection
    /// <summary>
    /// Коллекция используемых объектов системы (наших)
    /// </summary>
    public sealed class FMObjectsCollection : ObjectsCollection
    {
        public FMObjectsCollection(PumpProgram parentProgram)
            : base(parentProgram, XmlConsts.fmObjectTag, typeof(FMObject))
        {
        }

        /// <summary>
        /// Дополнительная проверка корректности внутреннего списка используемых объектов
        /// </summary>
        /// <returns>текст ошибки (если она была)</returns>
        public override string Validate()
        {
            string errors = base.Validate();
            if (!String.IsNullOrEmpty(errors))
                return errors;
            DataPumpModuleBase pumpModule = parentProgram.pumpModule;
            // инициализируем массивы объектов родительского модуля
            List<string> classifiers = new List<string>();
            List<string> factTables = new List<string>();
            // проходим по списку объектов и проверяем каждый на наличие в схеме
            foreach (FMObject obj in this)
            {
                switch (obj.objectType)
                {
                    case FmObjectsTypes.cls:
                        //case FmObjectsTypes.fixedCls:
                        if (pumpModule.Scheme.Classifiers[obj.name].IsDivided)
                            classifiers.Add(obj.name);
                        break;
                    case FmObjectsTypes.factTable:
                        if (pumpModule.Scheme.FactTables[obj.name].IsDivided)
                            factTables.Add(obj.name);
                        break;
                }
            }
            // формируем массивы испольхуемых объектов для родитедбского модуля
            // они там используются при выборке, сохранении и удалении данных
            if (classifiers.Count > 0)
            {
                pumpModule.UsedClassifiers = new IClassifier[classifiers.Count];
                for (int i = 0; i < classifiers.Count; i++)
                {
                    string clsName = classifiers[i];
                    pumpModule.UsedClassifiers[i] = pumpModule.Scheme.Classifiers[clsName];
                }
            }
            if (factTables.Count > 0)
            {
                pumpModule.UsedFacts = new IFactTable[factTables.Count];
                for (int i = 0; i < factTables.Count; i++)
                {
                    string clsName = factTables[i];
                    pumpModule.UsedFacts[i] = pumpModule.Scheme.FactTables[clsName];
                }
            }
            return String.Empty;
        }

        /// <summary>
        /// Найти классификатор на который ссылается атрибут. Вообще, место этому методу - в общих модулях,
        /// а еще лучше - сделать в коллекциях классификаторов (да и вообще во всех коллекциях) метод
        /// для регистронезависимого поиска
        /// </summary>
        /// <param name="dataObj"></param>
        /// <param name="refName"></param>
        /// <returns></returns>
        private static IEntity GetBridgeClsByRefName(IEntity dataObj, string refName)
        {
            IAssociationCollection assCol = (IAssociationCollection)(dataObj).Associations;
            string fullName = String.Concat(dataObj.Name, ".", refName);
            IEntity bridgeCls = null;
            foreach (IAssociation item in assCol.Values)
            {
                if (String.Compare(item.Name, fullName, true) == 0)
                {
                    bridgeCls = item.RoleBridge;
                    break;
                }
            }
            return bridgeCls;
        }

        /// <summary>
        /// Создание правил обработки аттрибутов - преобразование заданных пользователем 
        /// правил в конечный вид и создание правил действующих всегда (заполнение PumpID и т.п.)
        /// </summary>
        private void BuildProcessRules()
        {
            // формируем правила для каждого используемого объекта системы
            foreach (FMObject fmObject in this)
            {
                IEntity ent = fmObject.obj as IEntity;
                #region По заданным в XML правилам обработки получаем их фактические значения
                // обрабатываем правила обработки, заданные в XML
                fmObject.processRules.Build();
                // обработка для AttributeProcessRules.refCls - проставление фактического ID, значение
                // которого пойдет в ссылку
                foreach (ProcessRule pr in fmObject.processRules)
                {
                    if (pr.processRule != AttributeProcessRules.refCls)
                        continue;
                    string attrName = pr.name;
                    IDataAttribute attr = ent.Attributes[attrName];

                    if (attr.Class != DataAttributeClassTypes.Reference)
                        throw new Exception("Правило применимо только для ссылочных аттрибутов");
                    // получаем сущность на которую ссылается аттрибут
                    IEntity bridgeCls = GetBridgeClsByRefName(ent, attrName);
                    // формируем текст запроса-ограничения
                    string queryText = String.Format("select ID from {0} where {1}", bridgeCls.FullDBName, pr.dataValue);
                    // получаем ID записей, удовлетворяющих условиям
                    DataTable ids = (DataTable)parentProgram.pumpModule.DB.ExecQuery(queryText, 
                        QueryResultTypes.DataTable);
                    if ((ids != null) && (ids.Rows.Count > 0))
                    {
                        // пишем ID первой записи в параметр правила обработки
                        pr.dataValue = ids.Rows[0][0];
                    }
                    else
                    {
                        // если ни одной записи не найдено - лучше прекратить закачку с ошибкой
                        throw new Exception(
                            String.Format("Не удалось значение ссылки. Не найдены записи, удовлетворяющие условию '{0}'", queryText)
                        );
                    }
                }
                #endregion

                #region Теперь по аттрибутам объекта схемы формируем дополнительные правила
                #region генерация ID для классификаторов
                if (fmObject.objectType == FmObjectsTypes.cls)
                {
                    // для обычных классификаторов добавляем правило генерации ИД
                    ProcessRule pr = new ProcessRule(parentProgram);
                    pr.name = "ID";
                    pr.processRule = AttributeProcessRules.generatorValue;
                    pr.dataValue = ent.GeneratorName;
                    fmObject.processRules.AddNew(pr);
                }
                #endregion
                #region расставления ссылок на классификаторы для таблиц фактов
                // для таблиц фактов - расставляем ссылки на связанные классификаторы
                if (fmObject.objectType == FmObjectsTypes.factTable)
                {
                    IDataAttributeCollection attrs = ent.Attributes;
                    foreach (IDataAttribute attr in attrs.Values)
                    {
                        // если аттрибут - ссылка..
                        if (attr.Class == DataAttributeClassTypes.Reference)
                        {
                            string attrName = attr.Name;
                            IEntity bridgeCls = GetBridgeClsByRefName(ent, attrName);
                            string bridgeName = bridgeCls.ObjectKey;
                            //  в качестве значения правила обработки будет использоваться ID 
                            // созданной записи нашего объекта
                            FMObject fo = ObjectByName(bridgeName) as FMObject;
                            if (fo != null)
                            {
                                ProcessRule pr = new ProcessRule(parentProgram);
                                pr.processRule = AttributeProcessRules.fmObjectRef;
                                pr.name = attrName;
                                pr.dataValue = fo;
                                fmObject.processRules.AddNew(pr);
                            }
                        }
                    }
                }
                #endregion
                #region расставление PumpID, SourceID, TaskID для всего
                DataColumnCollection cl = fmObject.objData.Tables[0].Columns;
                if (cl.Contains("PumpID"))
                {
                    ProcessRule pr = new ProcessRule(parentProgram);
                    pr.name = "PumpID";
                    pr.processRule = AttributeProcessRules.constant;
                    pr.dataValue = parentProgram.pumpModule.PumpID;
                    fmObject.processRules.AddNew(pr);
                }
                if (cl.Contains("SourceID"))
                {
                    ProcessRule pr = new ProcessRule(parentProgram);
                    pr.name = "SourceID";
                    pr.processRule = AttributeProcessRules.constant;
                    // если источник объекта явно не указан - берем источник закачки
                    if (fmObject.dataSource != null)
                        pr.dataValue = fmObject.dataSource.id;
                    else
                        pr.dataValue = parentProgram.pumpModule.SourceID;
                    fmObject.processRules.AddNew(pr);
                }
                if (cl.Contains("TaskID"))
                {
                    ProcessRule pr = new ProcessRule(parentProgram);
                    pr.name = "TaskID";
                    pr.processRule = AttributeProcessRules.constant;
                    pr.dataValue = -1;
                    fmObject.processRules.AddNew(pr);
                }
                #endregion
                #endregion

            }

        }

        /// <summary>
        /// Запрос данных объекта
        /// </summary>
        internal void QueryData()
        {
            foreach (FMObject obj in this)
            {
                obj.QueryData();
            }
            // после иницилиазации датасетов и адаптеров можно сформировать правила обработки
            BuildProcessRules();
        }
    }
    #endregion

    #region ProcessRulesList
    /// <summary>
    /// Список правил обработки
    /// </summary>
    public sealed class ProcessRulesList : ObjectsList
    {
        public ProcessRulesList(PumpProgram parentProgram)
            : base(parentProgram, XmlConsts.processRuleTag, typeof(ProcessRule))
        {
        }

        /// <summary>
        /// Преобразовать правила к конечному виду
        /// </summary>
        internal void Build()
        {
            foreach (ProcessRule pr in this)
            {
                pr.Build();
            }
        }

        /// <summary>
        /// Применить правила обработки к строке данных
        /// </summary>
        /// <param name="row">Строка данных</param>
        internal void Applay(DataRow row)
        {
            foreach (ProcessRule pr in this)
            {
                pr.Applay(row);
            }
        }
    }
    #endregion

    #region StepsCollection
    /// <summary>
    /// Коллекция шагов закачки
    /// </summary>
    public sealed class StepsCollection : ObjectsCollection
    {
        public StepsCollection(PumpProgram parentProgram)
            : base(parentProgram, XmlConsts.stepNodeTag, typeof(PumpStep))
        {
        }

        internal void BuildProcessRules()
        {
            foreach (PumpStep ps in this)
                ps.fieldsGroups.BuildProcessRules();
        }
    }
    #endregion

    #region LayerFieldsList
    /// <summary>
    /// Список полей слоя
    /// </summary>
    public sealed class LayerFieldsList : ObjectsList
    {
        public LayerFieldsList(PumpProgram parentProgram)
            : base(parentProgram, XmlConsts.layerFieldNodeTag, typeof(LayerField))
        {
        }

        internal void BuildProcessRules()
        {
            foreach (LayerField lf in this)
            {
                if (lf.processRules.Count != 0)
                    lf.processRules.Build();
            }
        }

    }
    #endregion

    #region FieldsGroupsList
    /// <summary>
    /// Список групп полей
    /// </summary>
    public sealed class FieldsGroupsList : ObjectsList
    {
        public FieldsGroupsList(PumpProgram parentProgram)
            : base(parentProgram, XmlConsts.fieldsGroupTag, typeof(FieldsGroup))
        {
        }

        internal void BuildProcessRules()
        {
            foreach (FieldsGroup fp in this)
                fp.layerFields.BuildProcessRules();
        }

    }
    #endregion
    #endregion

}