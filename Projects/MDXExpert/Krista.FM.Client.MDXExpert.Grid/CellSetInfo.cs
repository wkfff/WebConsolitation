using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AnalysisServices.AdomdClient;

namespace Krista.FM.Client.MDXExpert.Grid
{
    /// <summary>
    /// Парсит информацию взятую из селсета, имеет много полезных методов
    /// </summary>
    public class CellSetInfo
    {
        private ExpertGrid _grid;
        private List<DimensionInfo> _rowsInfo;
        private List<DimensionInfo> _columnsInfo;
        private List<MeasuresSectionInfo> _measuresSectionsInfo;

        public CellSetInfo(ExpertGrid grid)
        {
            this.Grid = grid;
            this.RowsInfo = new List<DimensionInfo>();
            this.ColumnsInfo = new List<DimensionInfo>();
            this.MeasuresSectionsInfo = new List<MeasuresSectionInfo>();
        }

        public void Clear()
        {
            this.RowsInfo.Clear();
            this.ColumnsInfo.Clear();
            this.MeasuresSectionsInfo.Clear();
        }

        public void Initialize(CellSet cls)
        {
            if (cls != null)
            {
                this.ColumnsInfo = this.GetDimensionsInfo(cls.Axes[0].Positions, AxisType.Columns);
                if (cls.Axes.Count > 1)
                {
                    this.RowsInfo = this.GetDimensionsInfo(cls.Axes[1].Positions, AxisType.Rows);
                }
                this.MeasuresSectionsInfo = this.GetMeasuresSectionsInfo(cls);
            }
        }

        private bool IsExistMeasures(OlapInfoHierarchyCollection hiers)
        {
            foreach (OlapInfoHierarchy hier in hiers)
            {
                //не выводим измерение мер, т.к оно считается заголовками показателей
                if (hier.Name == "[Measures]")
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Получает информацию о показателях
        /// </summary>
        /// <param name="cls"></param>
        /// <returns></returns>
        private List<MeasuresSectionInfo> GetMeasuresSectionsInfo(CellSet cls)
        {
            List<MeasuresSectionInfo> measuresSectionsInfo = new List<MeasuresSectionInfo>();
            MeasuresSectionInfo currentSection = null;
            Member measure = null;
            string prepareTupleUN = "newTupleUN";
            bool isNewSection = true;

            /* В селсете меры является как правило последним сетом в множестве столбцов             
             * В оси могут быть и другие измерения, но их может и небыть.
             * Будем исходить из этого
             */
            if (cls.Axes[0] != null)
            {
                if ((cls.Axes[0].Positions.Count > 0) && this.IsExistMeasures(cls.OlapInfo.AxesInfo.Axes[0].Hierarchies))
                {
                    //собираем список мер
                    foreach (Position pos in cls.Axes[0].Positions)
                    {
                        measure = pos.Members[pos.Members.Count - 1];
                        if (measure.UniqueName == " ")
                            break;

                        string currentTupleUN = GetTupleUN(pos);

                        isNewSection = (prepareTupleUN != currentTupleUN) || currentSection.IsExistMeasure(measure.UniqueName);
                        if (isNewSection)
                        {
                            currentSection = new MeasuresSectionInfo(currentTupleUN);
                            measuresSectionsInfo.Add(currentSection);
                            prepareTupleUN = currentTupleUN;
                        }
                        currentSection.MeasuresInfo.Add(new MeasureInfo(measure.Caption, measure.UniqueName));
                    }
                }
            }
            return measuresSectionsInfo;
        }

        public static string GetTupleUN(Position pos)
        {
            string result = string.Empty;
            if (pos != null)
            {
                for (int i = pos.Members.Count - 2; i >= 0; i--)
                {
                    result += pos.Members[i].UniqueName;
                }
            }
            return result;
        }

        /// <summary>
        /// Получить измерение по uniqueName
        /// </summary>
        /// <param name="uniqueName"></param>
        /// <returns></returns>
        public DimensionInfo GetDimInfoByUName(string uniqueName)
        {
            foreach (DimensionInfo dim in this.ColumnsInfo)
            {
                if (dim.UniqueName == uniqueName)
                    return dim;
            }
            foreach (DimensionInfo dim in this.RowsInfo)
            {
                if (dim.UniqueName == uniqueName)
                    return dim;
            }

            return null;
        }

        /// <summary>
        /// Получить уровень по uniqueName
        /// </summary>
        /// <param name="uniqueName"></param>
        /// <returns></returns>
        public LevelInfo GetLevInfoByUName(string uniqueName)
        {

            foreach (DimensionInfo dim in this.ColumnsInfo)
            {
                foreach (LevelInfo lev in dim.LevelsInfo)
                {
                    if (lev.UniqueName == uniqueName)
                        return lev;
                }
            }
            foreach (DimensionInfo dim in this.RowsInfo)
            {
                foreach (LevelInfo lev in dim.LevelsInfo)
                {
                    if (lev.UniqueName == uniqueName)
                        return lev;
                }
            }

            return null;
        }

        /// <summary>
        /// Получить количество отображаемых уровней в измерении, следующих после уровня указанного мембера 
        /// </summary>
        /// <param name="mem"></param>
        /// <returns></returns>
        public int GetFollowUpLevelCount(Member mem)
        {
            int result = 0;
            if (mem != null)
            {
                string hierName = mem.ParentLevel.ParentHierarchy.UniqueName;
                DimensionInfo dim = this.GetDimInfoByUName(hierName);
                if (dim == null)
                    return 0;
                foreach (LevelInfo lev in dim.LevelsInfo)
                {
                    if (lev.AbsoluteDepth > mem.LevelDepth)
                        result++;
                }
            }
            return result;
        }

        /// <summary>
        /// Существуют ли у измерения итоги. (Вернет true, если среди всех следующих за ним измерениях 
        /// есть уровень All)
        /// </summary>
        /// <param name="uniqueName"></param>
        /// <returns></returns>
        public bool IsExistsTotals(string uniqueName)
        {
            bool result = this.IsExistTotals(uniqueName, AxisType.Rows);
            if (!result)
                result = this.IsExistTotals(uniqueName, AxisType.Columns);
            return result;
        }

        /// <summary>
        /// Существуют ли у измерения итоги. (Вернет true, если среди всех следующих за ним измерениях 
        /// есть уровень All)
        /// </summary>
        /// <returns></returns>
        private bool IsExistTotals(string uniqueName, AxisType axisType)
        {
            bool result = false;

            foreach (DimensionInfo dimInfo in (axisType == AxisType.Rows) ? this.RowsInfo : this.ColumnsInfo)
            {
                if (result)
                {
                    if (!dimInfo.IsExistLevelAll)
                    {
                        return false;
                    }
                }

                if (dimInfo.UniqueName == uniqueName)
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// Получает информацию об измерениях
        /// </summary>
        /// <param name="pc"></param>
        /// <param name="axisType"></param>
        /// <returns></returns>
        private List<DimensionInfo> GetDimensionsInfo(PositionCollection pc, AxisType axisType)
        {
            List<DimensionInfo> result = new List<DimensionInfo>();
            
            if (pc.Count == 0)
                return result;
            
            int dimensionCount = pc[0].Members.Count;
            int positionCount = pc.Count;
            //заведем признак, существует ли "Общий итог"
            bool isExistGrandTotal;

            if (axisType == AxisType.Columns)
            {
                if (dimensionCount == 1)
                    return result;
                if (dimensionCount > 1)
                {
                    dimensionCount--;
                }
                isExistGrandTotal = this.Grid.PivotData.ColumnAxis.GrandTotalExists;
            }
            else
            {
                isExistGrandTotal = this.Grid.PivotData.RowAxis.GrandTotalExists;
            }

            for (int i = 0; i < dimensionCount; i++)
            {
                result.Add(new DimensionInfo(pc[0].Members[i].ParentLevel.ParentHierarchy.UniqueName));
                DimensionInfo currentDimension = result[i];

                currentDimension.IsExistLevelAll = this.IsExistLevelAll(pc[0].Members[i]);

                List<string> levelsNames = new List<string>();

                for (int j = 0; j < positionCount; j++)
                {
                    Position position = pc[j];
                    //по внутреннему соглашению принято, что последняя позиция в CellSet является "Общим итогом"
                    //(если он вообще присутствует)
                    bool isGrandTotal = isExistGrandTotal && (j == (positionCount - 1));
                    bool isAllLevel = position.Members[i].LevelName.Contains(Common.Consts.allLevel);
                    //если еще нет уровня с таким именем, и это не "Общий итог", добавляем информацию об уровне
                    if ((!levelsNames.Contains(position.Members[i].LevelName) && !isGrandTotal && !isAllLevel))
                    {
                        levelsNames.Add(position.Members[i].LevelName);
                        currentDimension.LevelsInfo.Add(new LevelInfo(position.Members[i].LevelDepth,
                            position.Members[i].ParentLevel.Caption,
                            position.Members[i].ParentLevel.UniqueName,
                            this.GetMemberPropertiesCount(position.Members[i].MemberProperties)));
                    }
                }
                
                //сортируем уровни по возростанию
                for (int j = 1; j < currentDimension.LevelsInfo.Count; j++)
                {
                    int absDepth = currentDimension.LevelsInfo[j].AbsoluteDepth;
                    int k = j - 1;
                    while ((k >= 0) && (absDepth < currentDimension.LevelsInfo[k].AbsoluteDepth))
                    {
                        k--;
                    }
                    k++;
                    if ((k >= 0) && (k != j))
                    {
                        currentDimension.LevelsInfo.Insert(k, currentDimension.LevelsInfo[j]);
                        currentDimension.LevelsInfo.RemoveAt(j + 1);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Существует ли у измерения мембера, уровень All
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        private bool IsExistLevelAll(Member member)
        {
            if (member == null)
                return false;
            return member.ParentLevel.ParentHierarchy.Levels[0].Name == "(All)";
        }

        /// <summary>
        /// Вернет количество свойств не равных - null
        /// </summary>
        /// <param name="memberPropertys"></param>
        /// <returns></returns>
        private int GetMemberPropertiesCount(MemberPropertyCollection memberPropertys)
        {
            int result = 0;
            if (memberPropertys == null)
                return result;

            foreach (MemberProperty property in memberPropertys)
            {
                if (property.Value != null)
                    result++;
            }
            return result;
        }

        /// <summary>
        /// Получает секцию показателй по совокупности uniqueName - ов всех элементов в кортеже
        /// </summary>
        /// <param name="parentUN"></param>
        /// <returns></returns>
        public MeasuresSectionInfo GetSectionInfoByTupleUN(string tupleUN)
        {
            foreach (MeasuresSectionInfo sectionInfo in this.MeasuresSectionsInfo)
            {
                if (sectionInfo.TupleUN == tupleUN)
                {
                    return sectionInfo;
                }
            }
            return null;
        }

        /// <summary>
        /// Вернет последнию секцию показателей
        /// </summary>
        public MeasuresSectionInfo LastMeasureSectionInfo
        {
            get 
            {
                int sectionCount = this.MeasuresSectionsInfo.Count;
                return sectionCount > 0 ? this.MeasuresSectionsInfo[sectionCount - 1] : null;
            }
        }

        internal List<DimensionInfo> RowsInfo
        {
            get 
            { 
                return this._rowsInfo; 
            }
            set 
            { 
                this._rowsInfo = value; 
            }
        }

        internal List<DimensionInfo> ColumnsInfo
        {
            get 
            { 
                return this._columnsInfo; 
            }
            set 
            { 
                this._columnsInfo = value; 
            }
        }

        public List<MeasuresSectionInfo> MeasuresSectionsInfo
        {
            get 
            { 
                return this._measuresSectionsInfo; 
            }
            set 
            {
                this._measuresSectionsInfo = value; 
            }
        }

        public ExpertGrid Grid
        {
            get { return _grid; }
            set { _grid = value; }
        }
    }

    /// <summary>
    /// Секция показателей
    /// </summary>
    public class MeasuresSectionInfo
    {
        private List<MeasureInfo> _measuresInfo;
        private string _tupleUN;

        public MeasuresSectionInfo(string tupleUN)
        {
            this.TupleUN = tupleUN;
            this.MeasuresInfo = new List<MeasureInfo>();
        }

        public void Clear()
        {
            this.MeasuresInfo.Clear();
        }

        /// <summary>
        /// Ищем меру по uniqueName
        /// </summary>
        /// <param name="uniqueName"></param>
        /// <returns></returns>
        public MeasureInfo GetMeasureByUN(string uniqueName)
        {
            foreach (MeasureInfo measure in this.MeasuresInfo)
            {
                if (measure.UniqueName == uniqueName)
                    return measure;
            }
            return null;
        }

        /// <summary>
        /// Существет ли в выборке мера с указанным uniqueName 
        /// </summary>
        /// <param name="uniqueName"></param>
        /// <returns></returns>
        public bool IsExistMeasure(string uniqueName)
        {
            return (this.GetMeasureByUN(uniqueName) != null ? true : false);
        }

        /// <summary>
        /// Показатели
        /// </summary>
        public List<MeasureInfo> MeasuresInfo
        {
            get
            {
                return this._measuresInfo;
            }
            set
            {
                this._measuresInfo = value;
            }
        }

        /// <summary>
        /// uniqueName всех элементов кортежа
        /// </summary>
        public string TupleUN
        {
            get 
            { 
                return this._tupleUN; 
            }
            set 
            { 
                this._tupleUN = value; 
            }
        }
    }

    /// <summary>
    /// Информация об показателе
    /// </summary>
    public class MeasureInfo
    {
        private string _caption;
        private string _uniqueName;        

        public string UniqueName
        {
            get { return _uniqueName; }
            set { _uniqueName = value; }
        }

        public string Caption
        {
            get { return _caption; }
            set { _caption = value; }
        }

        public MeasureInfo(string caption, string uniqueName)
        {
            this.Caption = caption;
            this.UniqueName = uniqueName;
        }
    }

    /// <summary>
    /// Информация об уровне
    /// </summary>
    public class LevelInfo
    {
        private int _absoluteDepth;
        private string _caption;
        private string _uniqueName;
        private int _propertiesCount;

        public LevelInfo(int absoluteDepth, string caption, string uniqueName, int propertiesCount)
        {
            this.AbsoluteDepth = absoluteDepth;
            this.Caption = caption;
            this.UniqueName = uniqueName;
            this.PropertiesCount = propertiesCount;
        }

        public int AbsoluteDepth
        {
            get { return _absoluteDepth; }
            set { _absoluteDepth = value; }
        }

        public string Caption
        {
            get { return _caption; }
            set { _caption = value; }
        }

        public string UniqueName
        {
            get { return _uniqueName; }
            set { _uniqueName = value; }
        }

        public int PropertiesCount
        {
            get { return _propertiesCount; }
            set { _propertiesCount = value; }
        }
    }

    /// <summary>
    /// Информация об измерении
    /// </summary>
    public class DimensionInfo
    {
        private string _uniqueName;
        private List<LevelInfo> _levelsInfo;
        private bool _isExistLevelAll;

        public DimensionInfo(string uniqueName)
        {
            this.UniqueName = uniqueName;
            this.LevelsInfo = new List<LevelInfo>();
        }

        /// <summary>
        /// Юник нейм иерархии/измерения
        /// </summary>
        public string UniqueName
        {
            get { return this._uniqueName; }
            set { this._uniqueName = value; }
        }

        /// <summary>
        /// Уровни
        /// </summary>
        public List<LevelInfo> LevelsInfo
        {
            get { return this._levelsInfo; }
            set { this._levelsInfo = value; }
        }

        /// <summary>
        /// Существует ли у измерения уровень All
        /// </summary>
        public bool IsExistLevelAll
        {
            get { return _isExistLevelAll; }
            set { _isExistLevelAll = value; }
        }
    }
}
