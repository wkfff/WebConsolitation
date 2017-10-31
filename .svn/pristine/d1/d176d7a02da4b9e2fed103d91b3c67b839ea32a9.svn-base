using System;
using System.Windows.Forms;
using System.Data;
using System.Text;
using System.Collections.Generic;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.ServerLibrary;
using CC = Krista.FM.Client.Components;
using Krista.FM.Client.Common;

using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using HierarchyType=Krista.FM.ServerLibrary.HierarchyType;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls
{

    public abstract partial class BaseClsUI : BaseViewObj, IInplaceClsView
    {
        #region Методы и свойства для работы с иерархией и уровнями
        // Кэш параметров иерархии. Должен очищаться при перегрузке списка объектов в дереве навигации
        private Dictionary<string, CC.HierarchyInfo> cashedHierarchyInfo = new Dictionary<string, CC.HierarchyInfo>();

        /// <summary>
        /// Получение и кэширование всех нужных параметров иерархии
        /// </summary>
        public HierarchyInfo GetHierarchyInfo(object sender)
        {
            HierarchyInfo hi = GetHierarchyInfoFromClsObject(ActiveDataObj, vo.ugeCls);
            maxHeight = new int[hi.LevelsCount];
            return hi;
        }

        private DataRelation GetHierarchyRelation(object sender)
        {
            HierarchyInfo hi = ((UltraGridEx)sender).HierarchyInfo;
            // создаем связь (relation) для построения иерархии
            if ((hi.ParentRefClmnName != String.Empty) &&
                (hi.ParentClmnName != String.Empty))
            {
                DataColumn parentColumn = dsObjData.Tables[0].Columns[hi.ParentClmnName];
                DataColumn refparentColumn = dsObjData.Tables[0].Columns[hi.ParentRefClmnName];
                if (parentColumn.DataType != refparentColumn.DataType)
                {
                    string errStr = "Невозможно построить иерархию." + Environment.NewLine +
                        "Типы родительского и ссылочного полей неодинаковы:" + Environment.NewLine +
                        parentColumn.Caption + ": " + parentColumn.DataType.ToString() + Environment.NewLine +
                        refparentColumn.Caption + "  " + refparentColumn.DataType.ToString();
                   throw new InvalidOperationException(errStr);
                }
                DataRelation hr = new DataRelation(
                    "HierarchyRelation",
                        parentColumn,
                        refparentColumn,
                        false
                );
                return hr;
            }
            return null;
        }

        protected virtual void OnNeedLoadChildRows(object sender, int parentID)
        {
            IDataUpdater upd = null;
            try
            {
                string filterStr;
                upd = GetActiveUpdater(parentID, out filterStr);
                DataTable childRows = new DataTable();
                DataTable allRows = dsObjData.Tables[0];
                upd.Fill(ref childRows);
				LookupManager.Instance.InitLookupsCash(ActiveDataObj, dsObjData);
                allRows.BeginLoadData();
                foreach (DataRow row in childRows.Rows)
                {
                    DataRow addedRow = allRows.Rows.Add(row.ItemArray);
                    addedRow.AcceptChanges();
                }
                allRows.EndLoadData();
            }
            finally
            {
                if (upd != null)
                    upd.Dispose();
            }
        }

        /// <summary>
        /// Обработка изменения вида грида
        /// </summary>
        /// <param name="inHierarchy"></param>
        void ugeCls_OnChangeHierarchyView(object sender, bool inHierarchy)
        {

        }

        public virtual HierarchyInfo GetHierarchyInfoFromClsObject(IEntity clsObject, UltraGridEx gridEx)
        {
            #region Инициализация
            // если объект уже был обработан - возвращаем нужные значения
            string objKey = String.Format("{0}.{1}", clsObject.ObjectKey, GetCurrentPresentation(clsObject));

            if (cashedHierarchyInfo.ContainsKey(objKey))
            {
                return cashedHierarchyInfo[objKey];
            }

            HierarchyInfo newHrInfo = new CC.HierarchyInfo();
            #endregion

            #region Тип иерахии
            // для таблиц фактов ничего более не делаем
            if (clsObject.ClassType == ClassTypes.clsFactData || clsObject.ClassType == ClassTypes.Table)
            {
                newHrInfo.LevelsNames = new string[] { clsObject.Caption };
                newHrInfo.LevelsCount = 1;
                newHrInfo.FlatLevelName = newHrInfo.LevelsNames[0];
                gridEx.SingleBandLevelName = newHrInfo.LevelsNames[0];
                return newHrInfo;
            }

            // какая-то важная переменная
            string levelWithTemplateName = string.Empty;

            IClassifier activeCls = (IClassifier)clsObject;
            switch (activeCls.Levels.HierarchyType)
            {
                // Для рдевовидной иерархии получим названия полей для ссылок
                case HierarchyType.ParentChild:
                    // ищем первый элемент с уровнем != All
                    foreach (IDimensionLevel item in activeCls.Levels.Values)
                    {
                        if (item.LevelType != LevelTypes.All)
                        {
                            levelWithTemplateName = item.ObjectKey;
                            newHrInfo.ParentRefClmnName = item.ParentKey.Name;
                            newHrInfo.ParentClmnName = item.MemberKey.Name;
                            break;
                        }
                    }
                    // если не найден - генерируем исключение
                    if (String.IsNullOrEmpty(levelWithTemplateName))
                        throw new InvalidProgramException("Не найден уровень для построения иерархии");

                    newHrInfo.ObjectHierarchyType = Components.HierarchyType.ParentChild;
                    // для больших иерархических таблиц ставим признак загрузки по раскрытию элемента
                    if (clsObject.RecordsCount(GetDataSourceID()) > HierarchyInfo.MaxRowsCount)
                        newHrInfo.loadMode = LoadMode.OnParentExpand;
                    break;
                case HierarchyType.Regular:
                    newHrInfo.ObjectHierarchyType = Components.HierarchyType.Regular;
                    int ind = 0;
                    foreach (IDimensionLevel item in activeCls.Levels.Values)
                    {
                        if (item.LevelType != LevelTypes.All)
                        {
                            levelWithTemplateName = item.ObjectKey;
                            ind++;
                        }
                    }
                    // если индекс меньше или равен единице, классификатор - плоский, иерархия не нужна
                    if (ind <= 1) break;
                    // иначе - получаем интерфейс искомого уровня
                    IDimensionLevel lvl = activeCls.Levels[levelWithTemplateName];
                    // если уровень не найден - генерируем исключение
                    if (lvl == null) throw
                        new InvalidProgramException("Не найден уровень для построения иерархии");
                    // определяем имена колонок для построения иерархии
                    newHrInfo.ParentRefClmnName = "PARENTID";
                    newHrInfo.ParentClmnName = lvl.MemberKey.Name;
                    break;
            }
            #endregion

            #region Имена уровней
            // получим количество уровней по атрибуту Divide
            string Divide = string.Empty;
            foreach (IDataAttribute item in clsObject.Attributes.Values)
            {
                Divide = item.Divide;
                if (Divide != string.Empty)
                    break;
            }
            int divideLevelCount = 0;
            if (Divide != string.Empty)
            {
                divideLevelCount = Divide.Split('.').Length;
                newHrInfo.isDivideCode = true;
            }

            // переменная для определения позиции (индекса) искомого уровня
            int index = 0;
            // имя ключа искомого уровня
            string lastLevelKey = string.Empty;
            // ищем последний уровень с типом != All, запомниаем его индекс и ключ
            foreach (IDimensionLevel item in activeCls.Levels.Values)
            {
                if (item.LevelType != LevelTypes.All)
                {
                    index++;
                    lastLevelKey = item.ObjectKey;
                }
            }

            if (lastLevelKey != string.Empty)
                newHrInfo.FlatLevelName = activeCls.Levels[lastLevelKey].Name;

            // В зависимости от типа иерархии, определяем количество уровней и их имена
            if (activeCls.Levels.HierarchyType != HierarchyType.Regular)
            {

                IDimensionLevel lvl = activeCls.Levels[lastLevelKey];
                string levelTemplate = string.Empty;

                // пробуем брать имена уровней с учетом версии классификатора
                if (activeCls.Presentations.ContainsKey(GetCurrentPresentation(clsObject)))
                    levelTemplate = activeCls.Presentations[GetCurrentPresentation(clsObject)].LevelNamingTemplate;

                if (String.IsNullOrEmpty(levelTemplate))
                    levelTemplate = lvl.LevelNamingTemplate;

                if (String.IsNullOrEmpty(levelTemplate))
                {
                    newHrInfo.LevelsCount = 6;
                    newHrInfo.LevelsNames = new string[6];
                    for (int i = 1; i <= newHrInfo.LevelsCount; i++)
                        newHrInfo.LevelsNames[i - 1] = "Уровень " + i.ToString();
                }
                else
                {
                    string[] names = levelTemplate.Split(';');
                    // если количество уровней в divide больше заданных имен уровней...
                    if (divideLevelCount > names.Length)
                    {
                        newHrInfo.LevelsCount = divideLevelCount;
                        newHrInfo.LevelsNames = new string[divideLevelCount];
                        // пишем сначал заданные имена уровней
                        for (int i = 0; i < names.Length; i++)
                        {
                            newHrInfo.LevelsNames[i] = names[i];
                        }
                        // дописываем недостающие
                        for (int i = names.Length; i < divideLevelCount; i++)
                        {
                            newHrInfo.LevelsNames[i] = String.Format("Уровень {0}", i + 1);
                        }
                    }
                    else
                    {
                        newHrInfo.LevelsCount = names.Length;
                        newHrInfo.LevelsNames = names;
                    }
                }
            }
            else
            {
                newHrInfo.LevelsCount = index;
                if (newHrInfo.LevelsCount <= 1)
                {
                    newHrInfo.LevelsCount = 1;
                    newHrInfo.LevelsNames = new string[newHrInfo.LevelsCount];
                    if (lastLevelKey == string.Empty)
                        newHrInfo.LevelsNames[0] = GetDataObjSemanticRus(clsObject);
                    else
                        newHrInfo.LevelsNames[0] = lastLevelKey;
                }
                else
                {
                    newHrInfo.LevelsNames = new string[newHrInfo.LevelsCount];
                    if (lastLevelKey != string.Empty)
                        newHrInfo.FlatLevelName = activeCls.Levels[lastLevelKey].Name;
                    index = 0;
                    foreach (IDimensionLevel item in activeCls.Levels.Values)
                    {
                        if (item.LevelType != LevelTypes.All)
                        {
                            index++;
                            newHrInfo.LevelsNames[index - 1] = item.Name;
                        }
                    }
                }
            }
            #endregion

            if (newHrInfo.LevelsCount > 1)
                newHrInfo.CurViewState = ViewState.Hierarchy;
            else
                newHrInfo.CurViewState = ViewState.Flat;

            cashedHierarchyInfo.Add(objKey, newHrInfo);
            return newHrInfo;
        }

        #endregion
    }
}
