// ******************************************************************
// Модуль содержит реализацию корневого элемента объектной модели -  
// программы закачки
// ******************************************************************
using System;
using System.Xml;
using System.Text;
using System.Data;

using Krista.FM.ServerLibrary;
using Krista.FM.Common.Xml;
using Krista.FM.Server.DataPumps;


namespace Krista.FM.Server.DataPumps.BudgetLayersDataPump
{
    #region Перечисления
    /// <summary>
    /// Режим подключения к серверу слоев (откуда брать данные)
    /// </summary>
    public enum ConnectMode : int
    {
        // подключаться к серверу слоев
        directConnection = 0,
        // искать данные слоев в каталоге источника
        fromAdoXml = 1
    }
    #endregion

    #region PumpProgram
    /// <summary>
    /// Класс для представления программы закачки
    /// </summary>
    public sealed class PumpProgram : ProcessObject
    {
        // Версия программы
        private string version;
        // Используемые слои
        internal LayersCollection usedLayers;
        // Используемые объекты системы
        internal FMObjectsCollection fmObjects;
        // Шаги закачки
        internal StepsCollection steps;
        // Источник данных по умолчанию
        internal DataSource defaultDataSource;
        // Родительский модуль закачки (для доступа к общей объектной модели)
        internal DataPumpModuleBase pumpModule = null;

        private PumpProgram()
        {
        }

        public PumpProgram(DataPumpModuleBase pumpModule)
        {
            this.pumpModule = pumpModule;
            usedLayers = new LayersCollection(this);
            fmObjects = new FMObjectsCollection(this);
            steps = new StepsCollection(this);
            defaultDataSource = new DataSource(this);
        }

        /// <summary>
        /// Загрузить программу из XML
        /// </summary>
        /// <param name="node">Корневой узел</param>
        internal override void  LoadFromXml(XmlNode node)
        {
 	        base.LoadFromXml(node);
            // загружаем параметры программы (версию)
            version = XmlHelper.GetStringAttrValue(node, XmlConsts.versionAttr, String.Empty);
            // .. источник данных по умолчанию
            XmlNode xn = node.SelectSingleNode(XmlConsts.defaultDataSourceNodeTag);
            defaultDataSource.LoadFromXml(xn);
            // .. используемые слои
            xn = node.SelectSingleNode(XmlConsts.layersNodeTag);
            usedLayers.LoadFromXml(xn);
            // .. используемые объекты системы
            xn = node.SelectSingleNode(XmlConsts.fmObjectsTag);
            fmObjects.LoadFromXml(xn);
            // .. шаги закачки
            xn = node.SelectSingleNode(XmlConsts.stepsNodeTag);
            steps.LoadFromXml(xn);
        }

        /// <summary>
        /// Проверка программы закачки на валидность (втроая, первая - проверка на соответствие XSD-схеме)
        /// </summary>
        /// <param name="errors">Текст ошибок (если были)</param>
        /// <returns>true/false</returns>
        public bool Validate(out string errors)
        {
            StringBuilder sb = new StringBuilder();

            string err = defaultDataSource.Validate();
            if (!String.IsNullOrEmpty(err))
            {
                sb.AppendFormat("Проверка источника данных по умолчанию: {0} {1}", err, Environment.NewLine);
            }

            err = usedLayers.Validate();
            if (!String.IsNullOrEmpty(err))
            {
                sb.AppendFormat("Проверка секции описания слоев: {0} {1}", err, Environment.NewLine);
            }

            err = fmObjects.Validate();
            if (!String.IsNullOrEmpty(err))
            {
                sb.AppendFormat("Проверка секции описания объектов схемы: {0} {1}", err, Environment.NewLine);
            }

            err = steps.Validate();
            if (!String.IsNullOrEmpty(err))
            {
                sb.AppendFormat("Проверка секции описания шагов закачки: {0} {1}", err, Environment.NewLine);
            }

            errors = sb.ToString();
            return String.IsNullOrEmpty(errors);
        }

        /// <summary>
        /// Загрузка данных слоев из каталога
        /// </summary>
        /// <param name="dir">Путь к каталогу</param>
        public void LoadLayersFromDir(string dir)
        {
            usedLayers.LoadLayersFromDir(dir);
        }

        /// <summary>
        /// Запрос данных объектов, компиляция правил обработки атрибутов
        /// </summary>
        public void QueryData()
        {
            fmObjects.QueryData();
            steps.BuildProcessRules();
        }

        /// <summary>
        /// Преобразовать значение из слоя к значению атрибуты схемы без преобразования значения
        /// </summary>
        /// <param name="sourceRow">строка с данными слоя</param>
        /// <param name="sourceField">имя поля слоя</param>
        /// <param name="destinationTable">DataTable с данными объекта схемы (приемника)</param>
        /// <param name="field">поле объекта схемы</param>
        /// <returns>значение</returns>
        private static object GetValueForDestinationAttr(DataRow sourceRow, string sourceField,
            DataTable destinationTable, LayerField field)
        {
           return  GetValueForDestinationAttr(sourceRow, sourceField, destinationTable, field, true);
        }

        /// <summary>
        /// Преобразовать значение из слоя к значению атрибуты схемы
        /// </summary>
        /// <param name="sourceRow">строка с данными слоя</param>
        /// <param name="sourceField">имя поля слоя</param>
        /// <param name="destinationTable">DataTable с данными объекта схемы (приемника)</param>
        /// <param name="field">поле объекта схемы</param>
        /// <param name="useConvertValue">Использовать ли параметры конвертации значения</param>
        /// <returns>значение</returns>
        private static object GetValueForDestinationAttr(DataRow sourceRow, string sourceField, 
            DataTable destinationTable, LayerField field, bool useConvertValue)
        {
            // запоминаем значение из слоя
            object sourceValue = sourceRow[sourceField];
            if (sourceValue == DBNull.Value)
                sourceValue = 0;
            // если нужно - преобразуем его в соответствии с правилами обработки
            if (useConvertValue)
            {
                switch (field.convertValueMode)
                {
                    // режим удаления маски
                    case ConvertValueModes.removeMask:
                        // удаляем все символы маски. Если будет сильно тормозить 
                        // - переделать на StringBuilder
                        sourceValue = sourceValue.ToString().Replace(".", String.Empty).
                            Replace(",", String.Empty).
                            Replace(" ", String.Empty);
                        break;
                    // режим части значения
                    case ConvertValueModes.partValue:
                        // получаем подстроку в соответствии с параметрами преобразования
                        string valStr = sourceValue.ToString();
                        int firstPos = Convert.ToInt32(field.convertValueModeParam1);
                        int length = Convert.ToInt32(field.convertValueModeParam2);
                        sourceValue = valStr.Substring(firstPos, length);
                        break;
                    // режим добавления к значению фикс строки
                    case ConvertValueModes.concatValue:
                        sourceValue = string.Format("{0}{1}", sourceValue, field.convertValueModeParam1);
                        break;
                    // режим дополнения строки слева
                    case ConvertValueModes.padLeft:
                        sourceValue = sourceValue.ToString().PadLeft(Convert.ToInt32(field.convertValueModeParam1), 
                                                                     Convert.ToChar(field.convertValueModeParam2));
                        break;
                }
            }
            // пытаемся преобразовать значение к типу атрибута-приемника
            Type destinationType = destinationTable.Columns[field.destinationAttributeName].DataType;
            object destinationValue = Convert.ChangeType(sourceValue, destinationType);
            return destinationValue;
        }

        /// <summary>
        /// Добавить значение в фильтр (в формате ограничения на DataTable)
        /// </summary>
        /// <param name="filter">буфер для выражения</param>
        /// <param name="attrName">имя атрибута</param>
        /// <param name="val">значение</param>
        private static void AppendValueToFilter(StringBuilder filter, string attrName, object val)
        {
            if (filter.Length != 0)
                filter.Append(" and ");
            if (val.GetType().FullName == "System.String")
            {
                filter.AppendFormat("({0} = '{1}')", attrName, val);
            }
            else
                filter.AppendFormat("({0} = {1})", attrName, val);
        }

        /// <summary>
        /// Была ли закачана строка классификатора
        /// </summary>
        /// <param name="filterStr">выражение для поиска строки</param>
        /// <param name="objData">DataTable с данными объекта</param>
        /// <param name="fmObj">объект системы</param>
        /// <returns></returns>
        private static bool ClsRowAlreadyPumped(string filterStr, DataTable objData, FMObject fmObj)
        {
            if (String.IsNullOrEmpty(filterStr))
                return false;
            // пытаемся найти строку
            DataRow[] selected = objData.Select(filterStr);
            if (selected.Length > 0)
            {
                // если нашли - запоминаем ее ID (будем считать что ссылки всегда идут по этому полю)
                fmObj.pumpedValueForCurrentLayerRow = selected[0]["ID"];
                return true;
            }
            return false;
        }

        /// <summary>
        /// Применить правила обработки к строке данных объекта схемы
        /// </summary>
        /// <param name="fmObj">объект схемы</param>
        /// <param name="group">группа полей</param>
        /// <param name="row">строка данных</param>
        private static void ApplayProcessRulesForRow(FMObject fmObj, FieldsGroup group, DataRow row)
        {
            //применяем правила обработки для объекта
            fmObj.processRules.Applay(row);
            //применяем правила обработки для каждого поля
            foreach (LayerField field in group.layerFields)
            {
                field.processRules.Applay(row);
            }
        }

        /// <summary>
        /// Закачать строку классификатора в режиме непосредственной закачки
        /// </summary>
        /// <param name="row">строка данных слоя</param>
        /// <param name="group">группа полей</param>
        /// <param name="currentStep">текущий шаг закачки</param>
        /// <param name="currentLayer">текущий слой</param>
        private void ProcessClsRowFieldsGroupDirectPump(DataRow row, FieldsGroup group, 
            PumpStep currentStep, Layer currentLayer)
        {
            // получаем указатели на нужные объекты
            FMObject fmObject = fmObjects[((LayerField)group.layerFields[0]).destinationObjectSynonym] as FMObject;
            DataTable objData = fmObject.objData.Tables[0];

            // резервируем массив для значений
            object[] values = new object[group.layerFields.Count];
            StringBuilder filter = new StringBuilder();
            // считываем значения из слоя, формируем фильтр
            int ind = 0;
            foreach (LayerField field in group.layerFields)
            {
                values[ind] = GetValueForDestinationAttr(row, field.name, objData, field);
                if (field.destinationAttributeUseInSearch)
                {
                    AppendValueToFilter(filter, field.destinationAttributeName, values[ind]);
                }
                ind++;
            }
            // если фильтр есть - попытаемся найти такую запись в данных классификатора
            if (ClsRowAlreadyPumped(filter.ToString(), objData, fmObject))
                return;
            // если дошли до сюда - нужно добавить новую запись
            DataRow newRow = objData.NewRow();
            // пишем данные 
            ind = 0;
            foreach (LayerField field in group.layerFields)
            {
                newRow[field.destinationAttributeName] = values[ind];
                ind++;
            }
            //применяем правила обработки
            ApplayProcessRulesForRow(fmObject, group, newRow);
            objData.Rows.Add(newRow);
            // запоминам ID записи
            fmObject.pumpedValueForCurrentLayerRow = newRow["ID"];
        }

        /// <summary>
        /// Закачать строку классификатора для случая ссылки на другой слой
        /// </summary>
        /// <param name="row">строка данных слоя</param>
        /// <param name="group">группа полей</param>
        /// <param name="currentStep">текущий шаг закачки</param>
        /// <param name="currentLayer">текущий слой</param>
        private void ProcessClsRowFieldsGroupLayerRef(DataRow row, FieldsGroup group,
            PumpStep currentStep, Layer currentLayer)
        {
            // получаем указатели на нужные объекты
            LayerField firstField = (LayerField) group.layerFields[0];
            FMObject fmObject = fmObjects[firstField.destinationObjectSynonym] as FMObject;
            DataTable objData = fmObject.objData.Tables[0];

            // пытаемся найти значение в закачанных данных
            StringBuilder destinationFilter = new StringBuilder();
            StringBuilder sourceFilter = new StringBuilder();

            // считываем значения из слоя, формируем фильтр
            foreach (LayerField field in group.layerFields)
            {
                if (!field.destinationAttributeUseInSearch)
                    continue;
                object refVal = GetValueForDestinationAttr(row, field.name, objData, field);
                AppendValueToFilter(destinationFilter, field.destinationAttributeName, refVal);
                AppendValueToFilter(sourceFilter, field.refLayerFieldName, row[field.name]);
            }
            // была ли такая строка закачана?
            if (ClsRowAlreadyPumped(destinationFilter.ToString(), objData, fmObject))
                return;
            
            // если нет - формируем данные для объекта из слоя на который идет ссылка
            Layer refLayer = (Layer)usedLayers[firstField.refLayerSynonym];
            // пытаемся получить строку-ссылку
            DataRow[] references = refLayer.layerData.Select(sourceFilter.ToString());
            // если не получилось - закачка не может быть продолжена
            if (references.Length == 0)
                throw new Exception(String.Format(
                    "В слое '{0}' не удалось найти значение удовлетворяющее условиям поиска '{1}'",
                    refLayer.name, sourceFilter));
            // если получилось - формируем новую строку классификатора
            DataRow newRow = objData.NewRow();
            foreach (LayerField field in group.layerFields)
            {
                object val = GetValueForDestinationAttr(references[0], field.refLayerFieldName, objData, field);
                newRow[field.destinationAttributeName] = val;
            }
            //применяем правила обработки
            ApplayProcessRulesForRow(fmObject, group, newRow);
            objData.Rows.Add(newRow);
            fmObject.pumpedValueForCurrentLayerRow = newRow["ID"];
        }


        /// <summary>
        /// Закачать строку классификатора
        /// </summary>
        /// <param name="row">строка данных слоя</param>
        /// <param name="group">группа полей</param>
        /// <param name="currentStep">текущий шаг закачки</param>
        /// <param name="currentLayer">текущий слой</param>
        private void ProcessClsRowFieldsGroup(DataRow row, FieldsGroup group, 
            PumpStep currentStep, Layer currentLayer)
        {
            // все данные объекта получаются из текущего слоя или из ссылки на другой слоя?
            LayerField firstField = (LayerField)group.layerFields[0];
            // в зависимости от типа обработки первого поля в группе - закачиваем строку классификатора..
            switch (firstField.processType)
            {
                // .. в режиме непосредственной закачки
                case ProcessTypes.directPump:
                    ProcessClsRowFieldsGroupDirectPump(row, group, currentStep, currentLayer);
                    break;
                // .. в режиме ссылки на слой
                case ProcessTypes.layerRef:
                    ProcessClsRowFieldsGroupLayerRef(row, group, currentStep, currentLayer);
                    break;
            }

        }

        /// <summary>
        /// Закачать строку таблицы фактов
        /// </summary>
        /// <param name="row">строка данных слоя</param>
        /// <param name="group">группа полей</param>
        /// <param name="currentStep">текущий шаг закачки</param>
        /// <param name="currentLayer">текущий слой</param>
        private void ProcessFactTableRowFieldsGroup(DataRow row, FieldsGroup group,
            PumpStep currentStep, Layer currentLayer)
        {
            // получаем указатели на нужные объекты
            FMObject fmObject = fmObjects[((LayerField)group.layerFields[0]).destinationObjectSynonym] as FMObject;
            DataTable objData = fmObject.objData.Tables[0];
            // резервируем массив для значений
            object[] values = new object[group.layerFields.Count];
            // считываем значения из слоя, формируем фильтр
            int ind = 0;
            foreach (LayerField field in group.layerFields)
            {
                values[ind] = GetValueForDestinationAttr(row, field.name, objData, field);
                ind++;
            }
            // нулевые суммы не качаем
            ind = 0;
            bool zeroSums = true;
            foreach (LayerField field in group.layerFields)
            {
                if (Convert.ToDecimal(values[ind]) != 0)
                    zeroSums = false;
                ind++;
            }
            if (zeroSums)
                return;

            DataRow newRow = objData.NewRow();
            // пишем данные 
            ind = 0;
            foreach (LayerField field in group.layerFields)
            {
                newRow[field.destinationAttributeName] = values[ind];
                ind++;
            }
            //применяем правила обработки
            ApplayProcessRulesForRow(fmObject, group, newRow);
            objData.Rows.Add(newRow);
        }

        
        /// <summary>
        /// Запуск процесса закачки данных
        /// </summary>
        public void PumpData()
        {
            // для каждого шага..
            foreach (PumpStep step in steps)
            {
                string msgStr = String.Format("Выполняется шаг: {0}", step.name);
                pumpModule.SetProgress(0, 0, msgStr, String.Empty);
                pumpModule.WriteToTrace(msgStr, DataPumpModuleBase.TraceMessageKind.Information);
                // .. получаем слой и его данные
                Layer lay = (Layer)usedLayers[step.synonym];
                DataTable dt = lay.layerData;
                int curPos = 0;
                // .. для каждой записи данных слоя
                foreach (DataRow row in dt.Rows)
                {
                    pumpModule.SetProgress(dt.Rows.Count, curPos, String.Empty, String.Empty, true);
                    // .. прохожим в цикле по всем группам полей
                    foreach (FieldsGroup group in step.fieldsGroups)
                    {
                        // .. в зависимости от типа группы закачиваем строку том или ином режиме
                        switch (group.groupType)
                        {
                            case FieldsGroupsType.clsRow:
                                ProcessClsRowFieldsGroup(row, group, step, lay);
                                break;
                            case FieldsGroupsType.factTableRow:
                                ProcessFactTableRowFieldsGroup(row, group, step, lay);
                                break;
                        }
                    }
                    curPos++;
                }
            }
            pumpModule.SetProgress(0, 0, String.Empty, String.Empty);
        }

        /// <summary>
        /// Сохранить данные используемых объектов
        /// </summary>
        public void UpdateData()
        {
            string msgStr = "Сохранение данных..";
            pumpModule.SetProgress(0, 0, msgStr, String.Empty);
            pumpModule.WriteToTrace(msgStr, DataPumpModuleBase.TraceMessageKind.Information);
            int curPos = 0;
            // проходим по всем используемым объектам
            foreach (FMObject fmObject in fmObjects)
            {
                msgStr = fmObject.name;
                pumpModule.SetProgress(fmObjects.Count, curPos, String.Empty, msgStr);
                // если объект имеет адаптер ждя работы с базой - вызываем метод сохранения данных
                if (fmObject.adapter != null)
                {
					pumpModule.UpdateDataSet(fmObject.adapter, fmObject.objData, (IEntity)fmObject.obj);
                }
                curPos++;
            }
        }

        /// <summary>
        /// Очичтить используемые ресурсы
        /// </summary>
        public void ClearData()
        {
            steps.Clear();
            fmObjects.Clear();
            usedLayers.Clear();
            defaultDataSource.Clear();
        }
    }
    #endregion

}