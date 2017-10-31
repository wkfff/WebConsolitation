using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Data;
using Krista.FM.Client.MDXExpert.Grid.Style;
using Krista.FM.Common.Xml;
using Microsoft.AnalysisServices.AdomdClient;
using System.Xml;

namespace Krista.FM.Client.MDXExpert.Grid
{
    /// <summary>
    /// Ось грида
    /// </summary>
    abstract public class Axis
    {
        //растояние между свойствами в ячейке
        private const int _memPropSeparatorHeight = 0;
        //растояние между именем свойства и его значением в ячейке
        private const int _memPropSeparatorWidth = 2;
        //растояние между текстом в ячейке и свойствами
        private const int _memPropPostTextSpacing = 3;
        //растояние слева от текста ячейки до свойств
        private const int _memPropPostTextLeftSpacing = 10;

        private DimensionCell _root;
        private AxisType _axisType;
        private ExpertGrid _grid;
        private Point _location = Point.Empty;
        private Rectangle _visibleBounds = Rectangle.Empty;
        private Graphics _graphics;
        private Painter _painter;
        private int _leafCount = 0;
        private CellStyle _style;
        private CellStyle _memProperNameStyle;
        private CellStyle _memProperValueStyle;
        private MembersExpandDictionary _stateMembersExpand;
        private MembersExpandDictionary _prepareStateMembersExpand;

        private int _grandTotalPosition;

        public Axis(ExpertGrid grid, AxisType axisType)
        {
            this.Grid = grid;
            this.AxisType = axisType;

            this.SetDefaultStyle();

            this.MemProperNameStyle = new CellStyle(this.Grid, GridConsts.gridRowsBackColorStart, GridConsts.gridRowsBackColorEnd,
                Color.Gray, GridConsts.gridCommonBorderColor);
            this.MemProperNameStyle.Font = new Font("Arial", 8);
            this.MemProperValueStyle = new CellStyle(this.Grid, GridConsts.gridRowsBackColorStart, GridConsts.gridRowsBackColorEnd,
                Color.Black, GridConsts.gridCommonBorderColor);
            this.MemProperValueStyle.Font = new Font("Arial", 8);

            this.StateMembersExpand = new MembersExpandDictionary();
            this.PrepareStateMembersExpand = new MembersExpandDictionary();

            this.Root = new DimensionCell(this, null);
            this.Root.IsRoot = true;
            this.Root.Expanded = true;

            this.Root.SetUncheckHeight(0);
            this.Root.SetUncheckWidth(0);
        }

        // делегат исопльзуемый при поиске ячейки
        private delegate bool FindCondition(DimensionCell cell, Point clickPoint);

        private delegate bool FindConditionInBounds(DimensionCell cell, Rectangle searchBounds);

        // смотрит имеет ли смысл искать ячейку в в этой ветке дерева(тоесть у текущей ячейки или ее потомков)
        private bool ConditionForRows(DimensionCell cell, Point clickPoint)
        {
            Rectangle bounds = cell.Bounds;
            return ((bounds.Y <= clickPoint.Y) && (bounds.Bottom >= clickPoint.Y));
        }

        // смотрит имеет ли смысл искать ячейку в в этой ветке дерева(тоесть у текущей ячейки или ее потомков)
        private bool ConditionForColumns(DimensionCell cell, Point clickPoint)
        {
            Rectangle bounds = cell.Bounds;
            return ((bounds.X <= clickPoint.X) && (bounds.Right >= clickPoint.X));
        }

        // смотрит имеет ли смысл искать ячейку в в этой ветке дерева(тоесть у текущей ячейки или ее потомков)
        private bool ConditionForRows(DimensionCell cell, Rectangle searchBounds)
        {
            Rectangle bounds = cell.Bounds;
            return ((bounds.Y <= searchBounds.Bottom) && (bounds.Bottom >= searchBounds.Y));
        }

        // смотрит имеет ли смысл искать ячейку в в этой ветке дерева(тоесть у текущей ячейки или ее потомков)
        private bool ConditionForColumns(DimensionCell cell, Rectangle searchBounds)
        {
            Rectangle bounds = cell.Bounds;
            return ((bounds.X <= searchBounds.Right) && (bounds.Right >= searchBounds.X));
        }


        /// <summary>
        /// Определяет надо ли рисовать данную ячейку, т.к. она может быть скрытой, или лежать за областью видимости
        /// грида
        /// </summary>
        /// <param name="cell">Ячейка</param>
        /// <returns>результат</returns>
        private bool IsDrawChild(DimensionCell cell)
        {
            return (cell.IsRoot || ((cell.HasChilds) &&
                (cell.OffsetBounds.Top <= this.VisibleBounds.Bottom) && 
                (this.VisibleBounds.IntersectsWith(cell.OffsetBounds))));
        }

        /// <summary>
        /// Рисует ячейку, а так же всех ее потомков
        /// </summary>
        /// <param name="cell">Ячейка</param>
        private void DrawMember(DimensionCell cell)
        {
            if (cell == null)
                return;

            cell.OnPaint(this._graphics, this._painter);
            if (IsDrawChild(cell))
            {
                if (cell.Expanded)
                {
                    foreach (DimensionCell childNode in cell.Children)
                    {
                        DrawMember(childNode);
                        //если подчиненый узел вне зоны видимости, другие рисовать смысла нет
                        if ((childNode.OffsetBounds.Top > this.VisibleBounds.Bottom) ||
                            (childNode.OffsetBounds.Left > this.VisibleBounds.Right))
                            break;
                    }
                }
                else
                {
                    DimensionCell total = cell.GetTotal();
                    if (total != null)
                        total.OnPaint(this._graphics, this._painter);
                }
            }
        }

        /// <summary>
        /// Рисует ось
        /// </summary>
        /// <param name="graphics">Graphics</param>
        /// <param name="painter">Painter</param>
        public void DrawMembers(Graphics graphics, Painter painter)
        {
            this.Graphics = graphics;
            this.Painter = painter;
            Region oldClip = graphics.Clip;
            try
            {
                this.VisibleBounds = this.GetVisibleBounds();
                this.Graphics.Clip = new Region(this.VisibleBounds);

                this.DrawMember(this.Root);
            }
            finally
            {
                graphics.Clip = oldClip;
                this.Graphics = null;
            }
        }

        /// <summary>
        /// Получить индекс кортежа, с которого начинается общий итог
        /// </summary>
        /// <param name="pc"></param>
        /// <returns></returns>
        protected int GetGrandTotalPosition(PositionCollection pc)
        {
            switch (this.AxisType)
            {
                case AxisType.Rows: return pc.Count - 1;
                case AxisType.Columns:
                    {
                        int sectionCount = this.Grid.PivotData.ClsInfo.MeasuresSectionsInfo.Count;
                        if (sectionCount > 0)
                        {
                            int lastSectionMeasureCount = this.Grid.PivotData.ClsInfo.MeasuresSectionsInfo[sectionCount - 1].MeasuresInfo.Count;
                            return pc.Count - lastSectionMeasureCount;
                        }
                        break;
                    }
            }
            return -1;
        }

        /// <summary>
        /// Является ли указанный индекс, индексом картежа в котром размещен главный итог
        /// </summary>
        /// <param name="curPosition"></param>
        /// <returns></returns>
        private bool IsGrandTotal(int curPosition, Position pos)
        {
            if (curPosition == this.GrandTotalPosition)
            {
                switch (this.AxisType)
                {
                    case AxisType.Columns:
                        {
                            //в кортеже итога колонок элементы должны быть либо с уровня All, 
                            //либо элементами поумолчанию, либо мерами 
                            foreach (Member mem in pos.Members)
                            {
                                if (!mem.LevelName.Contains("[(All)]") && 
                                    !mem.LevelName.Contains("[Measures]") &&
                                    (mem.UniqueName != mem.ParentLevel.ParentHierarchy.DefaultMember))
                                {
                                    return false;
                                }
                            }
                            return this.Grid.PivotData.ColumnAxis.GrandTotalExists;
                        }
                    case AxisType.Rows: 
                        {
                            //в кортеже итога строк элементы должны быть либо с уровня All, 
                            //либо элементами поумолчанию
                            foreach (Member mem in pos.Members)
                            {
                                if (!mem.LevelName.Contains("[(All)]") &&
                                    (mem.UniqueName != mem.ParentLevel.ParentHierarchy.DefaultMember))
                                {
                                    return false;
                                }
                            }
                            return this.Grid.PivotData.RowAxis.GrandTotalExists;
                        }
                }
            }
            return false;
        }

        protected void InitAxisMembers(PositionCollection pc, int lowerPosNum,
            int upperPosNum, int memNum, DimensionCell rootCell)
        {
            int curMemPos;
            Member curMem;
            Member mem;
            DimensionCell DC;
            int j;

            bool isColumnsAxis = (this.AxisType == AxisType.Columns);

            //В оси КОЛОНОК - верхняя граница предпоследний мембер. Последний не трогаем, т.к это меры,
            //а они будут считаться заголовками показателей
            if ((pc.Count == 0) || (memNum >= pc[0].Members.Count - (isColumnsAxis ? 1 : 0)))
                return;


            for (int i = lowerPosNum; i <= upperPosNum; )
            {
                curMemPos = i;
                curMem = pc[i].Members[memNum];
                mem = curMem;

                if (this.IsGrandTotal(i, pc[i]))
                {
                    i = pc.Count;
                    //получаем родительский элемент меры, он находиться на предпоследнем уровне
                    mem = pc[0].Members.Count >= 2 ? pc[i - 1].Members[pc[0].Members.Count - 2] : mem;
                    DC = this.NewDC(rootCell, Common.Consts.grandTotalCaption, true, false, mem, pc[i - 1], false, false, false);
                    DC.IsGrandTotal = true;
                    break;
                }

                while ((i <= upperPosNum) && !this.IsGrandTotal(i, pc[i]))
                {
                    mem = pc[i].Members[memNum];
                    if (mem.UniqueName != curMem.UniqueName)
                    {
                        break;
                    }
                    i++;
                }
                //итог между измерениями
                bool isInterDimensionTotal = this.IsInterDimensionTotal(curMem, isColumnsAxis ?
                    this.Grid.PivotData.ColumnAxis : this.Grid.PivotData.RowAxis);

                //если встретили итог, а в таблице нет ни одной ячейки, значит этот итог выводить не надо
                //он был добавлен для корректного подсчета итогов по видимым
                if (isInterDimensionTotal && rootCell.IsRoot)
                    continue;

                bool isStandartDeviation = false;
                if (IsStandartDeviationMember(curMem))
                {
                    isStandartDeviation = true;
                }


                if (isInterDimensionTotal||isStandartDeviation)
                {
                    DC = rootCell;
                }
                else
                {
                    string memCaption = String.Empty;

                    bool isAverage = false;
                    bool isMedian = false;

                    if (IsAverageMember(curMem))
                    {
                        memCaption = GridConsts.average;
                        isAverage = true;
                    }
                    else
                        if (IsMedianMember(curMem))
                        {
                            memCaption = GridConsts.median;
                            isMedian = true;
                        }

                    if((!isAverage)&&(!isMedian))
                        memCaption = curMem.Caption != string.Empty ? CommonUtils.GetMemberCaptionWithoutID(curMem) : GridConsts.empty;

                    DC = this.NewDC(rootCell, memCaption, false, false, curMem, pc[i - 1], isAverage, false, isMedian);

                    if ((isAverage) || (isMedian))
                        continue;
                }

                //Проверяем принадлежность данного элемента к родителю, если не родной, предупреждаем об 
                //этом пользователя
                this.CheckParentAccessory(pc, upperPosNum, memNum, curMem, i);

                if ((!isStandartDeviation)&&((i > upperPosNum) || (curMem.LevelDepth >= pc[i].Members[memNum].LevelDepth) || IsCalculatedMember(curMem)))
                {
                    //в не сбалансированных измерениях, если требуется добавляем мемберу пустышки
                    DC = this.AddDummyCell(DC, curMem, pc[i - 1]);
                    this.InitAxisMembers(pc, curMemPos, i - 1, memNum + 1, DC);
                }
                else
                {
                    j = i;

                    if (isStandartDeviation)
                    {
                        j = rootCell.IsRoot? upperPosNum : upperPosNum + 1;

                    }

                    while (j <= upperPosNum)
                    {
                        if (pc[j].Members[memNum].LevelDepth <= curMem.LevelDepth)
                        {
                            break;
                        }
                        j++;
                    }
                    this.InitAxisMembers(pc, i, j - 1, memNum, DC);

                    if (isStandartDeviation)
                    {
                        if (i > upperPosNum)
                        {
                            i = pc.Count - 1;
                            AddStandartDeviation(pc, lowerPosNum, memNum, curMem, DC, j, i, i);
                            break;
                        }
                        else
                        {
                            AddStandartDeviation(pc, lowerPosNum, memNum, curMem, DC, j, i, i-1);
                            i = j;
                            continue;
                        }
                    }

                    curMem = AddTotal(pc, lowerPosNum, memNum, curMem, DC, j, isColumnsAxis, i,
                        isInterDimensionTotal);
                    i = j;
                }
            }
        }


        /// <summary>
        /// В таблице бывают ситуации, когда элемент без родителя, такая ситуации не 
        /// корректна, предупреждаем об этом пользователя
        /// </summary>
        /// <param name="pc"></param>
        /// <param name="upperPosNum"></param>
        /// <param name="memNum"></param>
        /// <param name="curMem"></param>
        /// <param name="i"></param>
        private void CheckParentAccessory(PositionCollection pc, int upperPosNum, int memNum, Member curMem, int i)
        {
            //Если один из элементов вычислимый - выходим
            if (IsCalculatedMember(curMem) || ((i <= upperPosNum) && IsCalculatedMember(pc[i].Members[memNum])))
                return;


            //элемент имеет чужих потомков
            bool hasAlienDescendant = ((i <= upperPosNum) && curMem.LevelDepth < pc[i].Members[memNum].LevelDepth)
                && !this.IsAncestor(curMem, pc[i].Members[memNum]);
            if (hasAlienDescendant)
            {
                string errorText = string.Format("В измерении \"{0}\" по элементу \"{1}\" нет данных на родительском" +
                    " уровне. Для корректного отображения структуры таблицы, отключите опцию  \"Скрывать пустые элемены\".",
                    curMem.ParentLevel.ParentHierarchy.Name, pc[i].Members[memNum].Name);
                throw new Exception(errorText);
            }
        }

        /// <summary>
        /// Если первый элемент является предком второго, вернет true
        /// </summary>
        /// <param name="ancestor">предок</param>
        /// <param name="descendant">предпологаемый потомок</param>
        /// <returns></returns>
        private bool IsAncestor(Member ancestor, Member descendant)
        {

            if (ancestor.UniqueName == descendant.UniqueName)
                return true;

            while ((descendant.Parent != null) && (ancestor.LevelDepth <= descendant.Parent.LevelDepth))
            {
                if (ancestor.UniqueName == descendant.Parent.UniqueName)
                    return true;
                descendant = descendant.Parent;
            }
            return false;
        }


        /// <summary>
        /// Добавим стандартное отклонение
        /// </summary>
        /// <returns></returns>
        private Member AddStandartDeviation(PositionCollection pc, int lowerPosNum, int memNum, Member curMem,
            DimensionCell DC, int j, int i, int stDevPosition)
        {

            int memNumLimit = pc[i].Members.Count;

                if (memNumLimit > memNum + 1)
                {
                    //если это не отклонение, тогда будем искать его
                    if (!this.IsStandartDeviationMember(pc[stDevPosition].Members[memNum + 1]))
                    {
                        for (int k = lowerPosNum; k <= j - 1; k++)
                        {
                            if (this.IsStandartDeviationMember(pc[k].Members[memNum + 1]))
                            {
                                curMem = pc[k].Members[memNum + 1];
                                stDevPosition = k;
                                break;
                            }
                        }
                    }
                }

            //добавляем отклонение
                this.NewDC(DC, GridConsts.stdev, false, false, curMem, pc[stDevPosition], false, true, false);
            return curMem;
        }



        /// <summary>
        /// Добавим итог
        /// </summary>
        /// <returns></returns>
        private Member AddTotal(PositionCollection pc, int lowerPosNum, int memNum, Member curMem,
            DimensionCell DC, int j, bool isColumnsAxis, int i, bool isInterDimensionTotal)
        {
            //UN уровня с которого выводим итог, если это итог между измерениями то берем UN уровня
            //предыдущего измерения
            string levelUn = isInterDimensionTotal ? pc[i].Members[memNum - 1].ParentLevel.UniqueName :
                curMem.ParentLevel.UniqueName;
            //если у уровня включена видимость итога, и вообще если итог есть(его может не быть,
            //если после этого измерения в оси есть измерения без уровян All) добавим его
            bool isAppendTotal = this.Grid.PivotData.IsVisibleLevelTotal(levelUn) &&
                this.Grid.PivotData.ClsInfo.IsExistsTotals(curMem.ParentLevel.ParentHierarchy.UniqueName);

            if (isAppendTotal)
            {
                int totalPosition = i - 1;
                //т.к. в оси колонок последними расположены меры, их не трогаем
                int memNumLimit = isColumnsAxis ? pc[i].Members.Count - 1 : pc[i].Members.Count;

                if (memNumLimit > memNum + 1)
                {
                    //если это не итог, тогда будем искать его
                    if (!this.IsGrandMember(pc[totalPosition].Members[memNum + 1]))
                    {
                        for (int k = lowerPosNum; k <= j - 1; k++)
                        {
                            if (this.IsGrandMember(pc[k].Members[memNum + 1]))
                            {
                                curMem = pc[k].Members[memNum + 1];
                                totalPosition = k;
                                break;
                            }
                        }
                    }
                }

                //добавляем итог
                this.NewDC(DC, GridConsts.totalCaption, true, false, curMem, pc[totalPosition], false, false, false);
            }
            return curMem;
        }

        /// <summary>
        /// Вернет true если мембер лежит на уровне All, если такого уровня нет, то если является 
        /// DefaultMember 
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        private bool IsGrandMember(Member member)
        {
            bool result = member.LevelName.Contains(Common.Consts.allLevel);
            
            if (!result && !(member.ParentLevel.ParentHierarchy.Levels[0].Name == "(All)"))
            {
                //если нет уровня All
                result = (member.UniqueName == member.ParentLevel.ParentHierarchy.DefaultMember);
            }
            
            return result;
        }

        /// <summary>
        /// Является ли данный мембер итогом между двумя измерениями
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        private bool IsInterDimensionTotal(Member member, PivotAxis pivotAxis)
        {
            bool result = false;
            if (member != null)
            {
                //Мембер будет считаться итогом если он лежит в уровне All, или мембер является мембером
                //по умолчанию, но уровень на котором он лежит не выводится в таблицу
                result = (member.LevelName.Contains(Common.Consts.allLevel) || 
                    (this.IsGrandMember(member) && 
                    !pivotAxis.FieldSets.FieldIsPreset(member.ParentLevel.UniqueName)))&&(!IsCalculatedMember(member));
            }
            return result;
        }

        /// <summary>
        /// Является ли элемент вычислимым
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        private bool IsCalculatedMember(Member member)
        {
            return member.ParentLevel.GetMembers().Find(member.Caption) == null;
        }

        /// <summary>
        /// Является ли элемент вычислимым средним значением
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        private bool IsAverageMember(Member member)
        {
            string memberName = PivotData.GetNameFromUniqueName(member.UniqueName);
            return (this.Grid.PivotData.AverageSettings.AverageMemberName == memberName);
        }


        /// <summary>
        /// Является ли элемент вычислимым средним значением
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        private bool IsStandartDeviationMember(Member member)
        {
            string memberName = PivotData.GetNameFromUniqueName(member.UniqueName);
            return (this.Grid.PivotData.AverageSettings.StandartDeviationName == memberName);
        }


        /// <summary>
        /// Является ли элемент вычислимой медианой
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        private bool IsMedianMember(Member member)
        {
            string memberName = PivotData.GetNameFromUniqueName(member.UniqueName);
            return (this.Grid.PivotData.MedianSettings.MedianMemberName == memberName);
        }


        /// <summary>
        /// Поиск ячейки
        /// </summary>
        /// <param name="cell">Текущая ячейка</param>
        /// <param name="position">Координаты для поиска ячейки</param>
        /// <param name="IsFindNext">Условие продолжениея поиска</param>
        /// <param name="isFindByOffsetBounds">Признак, что поиск ячейки производится по относительным координатам</param>
        /// <returns>Если нашли, то ячейку</returns>
        private DimensionCell FindCell(DimensionCell cell, Point position, FindCondition IsFindNext)
        {
            if (cell == null)
                return null;
            //при условии что выделенная ячейка находится в правильном направлении, продолжаем поиск
            if (IsFindNext(cell, position))
            {
                if (cell.GetHitTest(position))
                    return cell;
                else
                {
                    if (cell.HasChilds)
                    {
                        DimensionCell result = null;
                        if (cell.Expanded)
                        {
                            for (int i = 0; (i < cell.Children.Count) && (result == null); i++)
                                result = FindCell(cell.Children[i], position, IsFindNext);
                        }
                        else
                        {
                            result = FindCell(cell.GetTotal(), position, IsFindNext);
                        }
                        return result;
                    }
                    else
                        return null;
                }
            }
            else
                return null;
        }

        /// <summary>
        /// Поиск ячейки
        /// </summary>
        /// <param name="cell">Текущая ячейка</param>
        /// <param name="position">Координаты для поиска ячейки</param>
        /// <param name="IsFindNext">Условие продолжениея поиска</param>
        /// <param name="isFindByOffsetBounds">Признак, что поиск ячейки производится по относительным координатам</param>
        /// <returns>Если нашли, то ячейку</returns>
        private List<GridControl> FindCells(DimensionCell cell, Rectangle searchBounds, FindConditionInBounds IsFindNext)
        {
            List<GridControl> result = new List<GridControl>();
            if (cell == null)
                return result;
            //при условии что выделенная ячейка находится в правильном направлении, продолжаем поиск
            if (IsFindNext(cell, searchBounds))
            {
                if (cell.GetHitTest(searchBounds))
                {
                    if (!cell.IsRoot)
                        result.Add(cell);
                }

                if (cell.HasChilds)
                {
                    if (cell.Expanded)
                    {
                        for (int i = 0; (i < cell.Children.Count); i++)
                            result.AddRange(FindCells(cell.Children[i], searchBounds, IsFindNext));
                    }
                    else
                    {
                        result.AddRange(FindCells(cell.GetTotal(), searchBounds, IsFindNext));
                    }
                    return result;
                }
                else
                    return result;

            }
            else
                return result;
        }

        /// <summary>
        /// Ищем ячейку по координате
        /// </summary>
        /// <param name="position">Координаты для поиска ячейки</param>
        /// <returns>Если нашли то ячейку, иначе null</returns>
        public DimensionCell FindCell(Point position, bool useFindWithOffset)
        {
            switch (this.AxisType)
            {
                case AxisType.Rows:
                    {
                        if (useFindWithOffset)
                            position = new Point(position.X, position.Y + this.Grid.VScrollBarState.Offset);
                        return this.FindCell(this.Root, position, new FindCondition(this.ConditionForRows));
                    }
                case AxisType.Columns:
                    {
                        if (useFindWithOffset)
                            position = new Point(position.X + this.Grid.HScrollBarState.Offset, position.Y);
                        return this.FindCell(this.Root, position, new FindCondition(this.ConditionForColumns));
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        /// <summary>
        /// Ищем ячейки в области
        /// </summary>
        /// <param name="searchBounds">Координаты для поиска ячейки</param>
        /// <returns></returns>
        public List<GridControl> FindCells(Rectangle searchBounds, bool useFindWithOffset)
        {
            List<GridControl> result = new List<GridControl>();
            switch (this.AxisType)
            {
                case AxisType.Rows:
                    {
                        if (useFindWithOffset)
                        {
                            if (searchBounds.Y < this.Location.Y)
                            {
                                searchBounds.Height -= this.Location.Y - searchBounds.Y;
                                searchBounds.Y = this.Location.Y;
                            }
                            int offset = Math.Min(this.Grid.VScrollBarState.Offset, this.Grid.VScrollBarState.BeginOffset);
                            searchBounds = new Rectangle(searchBounds.X,
                                                         searchBounds.Y + offset,
                                                         searchBounds.Width, searchBounds.Height);
                            searchBounds.Height += Math.Abs(this.Grid.VScrollBarState.BeginOffset - this.Grid.VScrollBarState.Offset);

                        }
                        return this.FindCells(this.Root, searchBounds, new FindConditionInBounds(this.ConditionForRows));
                    }
                case AxisType.Columns:
                    {
                        if (useFindWithOffset)
                        {
                            if (searchBounds.X < this.Location.X)
                            {
                                searchBounds.Width -= this.Location.X - searchBounds.X;
                                searchBounds.X = this.Location.X;
                            }
                            searchBounds = new Rectangle(searchBounds.X + this.Grid.HScrollBarState.Offset,
                                                         searchBounds.Y, searchBounds.Width, searchBounds.Height);
                        }
                        return this.FindCells(this.Root, searchBounds, new FindConditionInBounds(this.ConditionForColumns));
                    }
                default:
                    {
                        return result;
                    }
            }
        }



        protected void GetLeafCount(DimensionCell cell, ref int leafCount, bool isOnlyVisible)
        {
            bool isLeaf = isOnlyVisible ? cell.IsVisibleLeaf : cell.IsLeaf;
            if (isLeaf)
            {
                leafCount++;
            }
            else
            {
                foreach (DimensionCell child in cell.Children)
                {
                    this.GetLeafCount(child, ref leafCount, isOnlyVisible);
                }
            }
        }

        /// <summary>
        /// Получить количество листовых узлов в оси
        /// </summary>
        /// <returns>Количестов листовых узлов в оси</returns>
        public int GetLeafCount()
        {
            int leafCount = 0;
            this.GetLeafCount(this.Root, ref leafCount, false);
            return leafCount;
        }

        /// <summary>
        /// Количестов видимых листовых узлов в оси
        /// </summary>
        /// <returns>Количество видимых листовых узлов в оси</returns>
        public int GetVisibleLeafCount()
        {
            int leafCount = 0;
            this.GetLeafCount(this.Root, ref leafCount, true);
            return leafCount;
        }

        /// <summary>
        /// Возвращает массив с сcылками на листовые элементы
        /// </summary>
        /// <returns>Масив с ссылками на листовые элементы</returns>
        public DimensionCell[] GetLeafList()
        {
            List<DimensionCell> result = new List<DimensionCell>();
            this.GetLeafList(this.Root, ref result);
            return result.ToArray();
        }

        private void GetLeafList(DimensionCell cell, ref List<DimensionCell> result)
        {
            if (cell == null)
                return;
            if (cell.IsLeaf)
            {
                result.Add(cell);
                cell.LeafIndex = result.IndexOf(cell);
            }
            foreach (DimensionCell childCell in cell.Children)
            {
                this.GetLeafList(childCell, ref result);
            }
        }

        /// <summary>
        /// Всем ячейкам измерения устанавливает указанный стиль
        /// </summary>
        /// <param name="style"></param>
        public void SetStyle(CellStyle style)
        {
            style.Grid = this.Grid;
            this.Style.CopyDefaultStyle(style);
            this.Style = style;
            this.SetStyle(this.Root);
        }

        /// <summary>
        /// Рекурсивный обход коллекции ячеек, и выставление им стиля оси
        /// </summary>
        /// <param name="cell"></param>
        private void SetStyle(DimensionCell cell)
        {
            if (cell == null)
                return;

            cell.Style = this.Style;
            foreach (DimensionCell childrenCell in cell.Children)
            {
                this.SetStyle(childrenCell);
            }
        }

        /// <summary>
        /// Очистка оси
        /// </summary>
        public void Clear()
        {
            foreach (DimensionCell childCell in this.Root.Children)
            {
                childCell.Clear();
            }
            this.Root.Children.Clear();
            this.Root.SetUncheckHeight(0);
            this.Root.SetUncheckWidth(0);
        }

        /// <summary>
        /// Сделано для удобства добавления новой ячейки, параметрами указывается, родитель, заголовок, 
        /// и признак итога
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="caption"></param>
        /// <param name="IsTotal"></param>
        /// <returns></returns>
        protected DimensionCell NewDC(DimensionCell parent, string caption, bool isTotal, bool isDummy, Member clsMember, 
            Position pos, bool isAverage, bool isStandartDeviation, bool isMedian)
        {
            DimensionCell DC = new DimensionCell(this, parent);
            DC.Text = caption;
            DC.IsTotal = isTotal;
            DC.IsDummy = isDummy;
            DC.IsAverage = isAverage;
            DC.IsStandartDeviation = isStandartDeviation;
            DC.IsMedian = isMedian;

            DC.TupleIndex = (pos != null) ? pos.Ordinal : -1;
            //имена кортежей нужны только элементам столбцов, для правильного нахождения индексов 
            //кортежей для показателей, для строк их запоминать не будем
            if (this.AxisType == AxisType.Columns)
                DC.TupleUN = CellSetInfo.GetTupleUN(pos);
            DC.ClsMember = clsMember;
            DC.AllParentHashCode = DC.GetAllHashCode();

            if (this.Grid.SelectedCells != null)
            {
                if (this.Grid.SelectedCells.HashCodes.Contains(DC.GetHashCode()))
                {
                    DC.State = ControlState.Selected;
                    if (!this.Grid.SelectedCells.CurrentCells.Contains(DC))
                    {
                        this.Grid.SelectedCells.CurrentCells.Add(DC);
                    }
                }
                else
                {
                    DC.State = ControlState.Normal;
                }

            }

            this.InitStateMemberExpand(DC);
            return DC;
        }

        private void InitStateMemberExpand(DimensionCell DC)
        {
            if (DC.IsDummy)
            {
                //если фиктивная ячейка, то значение равно не фиктивному родителю
                DC.Expanded = DC.Parent.Expanded;
            }
            else
            {
                bool state = true;
                //ищем состояние включенности мембера
                state = this.PrepareStateMembersExpand.GetState(DC, !this.Grid.PivotData.DynamicLoadData);
                //присвоим
                DC.Expanded = state;
            }
        }

        /// <summary>
        /// Если требуется(а требуется в случае, когда в измерении детализация мембера меньше количества уровней), 
        /// добавляет ячейке переданной параметром, нужное количество пустышек
        /// </summary>
        /// <param name="dimensionCell"></param>
        /// <param name="mem"></param>
        /// <returns></returns>
        protected DimensionCell AddDummyCell(DimensionCell dimensionCell, Member mem, Position pos)
        {
            int dummyCellCount = this.Grid.PivotData.ClsInfo.GetFollowUpLevelCount(mem);
            //Дело в том, что в объемных таблицах, в выборке моожет и не быть всех уровней которых она должна 
            //в себя включать, а значит мы будем подстраховываться и уточнать это количиство у PivotData
            dummyCellCount = Math.Max(dummyCellCount, this.Grid.PivotData.GetFollowUpLevelCount(mem.ParentLevel.ParentHierarchy.UniqueName,
                mem.ParentLevel.UniqueName));

            DimensionCell dummyCell = dimensionCell;
            for (int i = 0; i < dummyCellCount; i++)
            {
                dummyCell = this.NewDC(dummyCell, string.Empty, false, true, mem, pos, false, false, false);
            }
            return dummyCell;
        }

        /// <summary>
        /// Ищет родителя пустышки
        /// </summary>
        /// <param name="dummyCell"></param>
        /// <returns></returns>
        protected DimensionCell GetDummyCellParent(DimensionCell dummyCell)
        {
            DimensionCell result = dummyCell.Parent;
            while ((result != null) && (result.IsDummy))
            {
                result = result.Parent;
            }
            return result;
        }

        /// <summary>
        /// Загрузить настройки оси из Xml
        /// </summary>
        public virtual void Load(XmlNode axisNode, bool isLoadTemplate)
        {
            if (axisNode == null)
                return;
            this.Style.Load(axisNode.SelectSingleNode(GridConsts.style), isLoadTemplate);
            this.MemProperNameStyle.Load(axisNode.SelectSingleNode(GridConsts.memPropNameStyle), isLoadTemplate);
            this.MemProperValueStyle.Load(axisNode.SelectSingleNode(GridConsts.memPropValueStyle), isLoadTemplate);
            this.StateMembersExpand.Load(axisNode.SelectSingleNode(GridConsts.stateMembersExpand));
        }

        /// <summary>
        /// Сохранить настройки оси в Xml
        /// </summary>
        public virtual void Save(XmlNode axisNode)
        {
            if (axisNode == null)
                return;
            this.Style.Save(XmlHelper.AddChildNode(axisNode, GridConsts.style));
            this.MemProperNameStyle.Save(XmlHelper.AddChildNode(axisNode, GridConsts.memPropNameStyle));
            this.MemProperValueStyle.Save(XmlHelper.AddChildNode(axisNode, GridConsts.memPropValueStyle));
            //при динамическом режиме загрузки данных будем хранить состояния только открытых элементов,
            //в обыкновенном режиме только закрытых
            SaveMode meberSaveNode = this.Grid.PivotData.DynamicLoadData ? SaveMode.OnlyTrue : SaveMode.OnlyFalse;
            this.StateMembersExpand.Save(XmlHelper.AddChildNode(axisNode, GridConsts.stateMembersExpand), meberSaveNode);
        }

        /// <summary>
        /// По уникальному имени уровня вычиляет высоту MemberPropertys
        /// </summary>
        /// <param name="levelUN"></param>
        /// <returns></returns>
        public int GetMemPropHeight(string levelUN)
        {
            //Найдем информацию об этом уровне
            LevelInfo levelInfo = this.Grid.PivotData.ClsInfo.GetLevInfoByUName(levelUN);
            //Посмотрим сколько там свойств
            int propertysCount = (levelInfo == null) ? 0 : levelInfo.PropertiesCount;
            if ((propertysCount == 0) || !this.IsAppearPropInDimCells)
                return 0;

            int nameHeight = this.MemProperNameStyle.OriginalTextHeight;
            int valueHeight = this.MemProperValueStyle.OriginalTextHeight;
            int maxHeight = Math.Max(nameHeight, valueHeight);
            //Что бы узнать высоту свойств, умножим количество на высоту, и добавим растояние от текста до
            //свойств
            return propertysCount * (maxHeight + this.MemPropSeparatorHeight)
                + this.MemPropPostTextSpacing;
        }

        private void SetDefaultStyle()
        {
            this.Style = new CellStyle(this.Grid, GridConsts.gridRowsBackColorStart, GridConsts.gridRowsBackColorEnd,
                GridConsts.gridAxisForeColor, GridConsts.gridCommonBorderColor);
            //если текст не помещается в ячейку, обрезаем его и ставим многоточие   
            this.Style.StringFormat.Trimming = StringTrimming.EllipsisCharacter;
            this.Style.StringFormat.FormatFlags = StringFormatFlags.LineLimit;
        }

        /// <summary>
        /// Свернуть развернуть элементы
        /// </summary>
        /// <param name="levelNumber">номер уровня для которого применять</param>
        /// <param name="state">если развернуть то true, если свернуть то false</param>
        public void ExpandAllMember(int levelNumber, bool state)
        {
            CaptionsList captionList = (this.AxisType == AxisType.Rows) ? this.Grid.RowCaptions
                : (CaptionsList)this.Grid.ColumnCaptions;

            if ((levelNumber < 0) || (levelNumber > captionList.Count - 1))
                return;

            CaptionCell captionCell = captionList[levelNumber];
            this.ExpandAllMember(captionCell, state);
        }

        /// <summary>
        /// Свернуть развернуть элементы
        /// </summary>
        /// <param name="captionCell">заголовок, на чьем уровне требуется операция</param>
        /// <param name="state">если развернуть то true, если свернуть то false</param>
        public void ExpandAllMember(CaptionCell captionCell, bool state)
        {
            if (captionCell == null)
                return;

            int levelNumber = captionCell.Captions.IndexOf(captionCell);
            this.DoExpandMember(this.Root, levelNumber, state);

            //узнаем уникальные имена измерения и уровня, на котором разворачивали элементы
            string hierarchyUN = captionCell.HierarchyUN;
            string levelUN = captionCell.UniqueName;
            //после того как свернули/развернули элементы, возбудим событие
            this.Grid.OnExpandedMember(hierarchyUN, levelUN, state);
            if (this.Grid != null)
                this.Grid.RecalculateGrid();
        }

        /// <summary>
        /// Рекурсия с развертыванием/свертыванием элементов на нужном уровне
        /// </summary>
        /// <param name="dimCell">ячейка у которой выставляем состояние</param>
        /// <param name="levelNumber">номер уровня на котором это надо делать</param>
        /// <param name="state">состояние</param>
        private void DoExpandMember(DimensionCell dimCell, int levelNumber, bool state)
        {
            if (dimCell == null) 
                return;

            if (!dimCell.IsRoot)
            {
                if (dimCell.DepthLevel == levelNumber)
                {
                    dimCell.Expanded = state;
                    return;
                }
            }
            foreach (DimensionCell child in dimCell.Children)
            {
                this.DoExpandMember(child, levelNumber, state);
            }
        }

        /// <summary>
        /// список всех схлопнутых мемберов на оси
        /// </summary>
        /// <returns></returns>
        public List<Member> AllCollapsedMembers()
        {
            return AllCollapsedMembers(this.Root); 
        }

        private List<Member> AllCollapsedMembers(DimensionCell dc)
        {
            List<Member> expandedMembers = new List<Member>();

            if (dc == null)
                return expandedMembers;

            foreach (DimensionCell cell in dc.Children)
            {
                if ((cell.HasChilds) && (!this.StateMembersExpand.GetState(cell, false)))
                {
                    expandedMembers.Add(cell.ClsMember);
                }
                expandedMembers.AddRange(AllCollapsedMembers(cell));
            }
            return expandedMembers;
        }

        /// <summary>
        /// Корень оси(невидимый в гриде), к нему крепяться все остальные ячейки
        /// </summary>
        public DimensionCell Root
        {
            get { return this._root; }
            set { this._root = value; }
        }

        /// <summary>
        /// Ссылка на грид
        /// </summary>
        public ExpertGrid Grid
        {
            get { return this._grid; }
            set { this._grid = value; }
        }

        /// <summary>
        /// Границы оси
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                Size size = new Size(this.Width, this.Height);
                return new Rectangle(this.Location, size);
            }
        }

        /// <summary>
        /// Расположение оси
        /// </summary>
        public Point Location
        {
            get { return this._location; }
            set { this._location = value; }
        }

        /// <summary>
        /// Тип оси
        /// </summary>
        public AxisType AxisType
        {
            get { return this._axisType; }
            set { this._axisType = value; }
        }

        /// <summary>
        /// Количестов листовых элементов
        /// </summary>
        public int LeafCount
        {
            get { return this._leafCount; }
            set { this._leafCount = value; }
        }

        /// <summary>
        /// Пустая ли коллекция
        /// </summary>
        public bool IsEmpty
        {
            get { return !this._root.HasChilds; }
        }

        /// <summary>
        /// Видимая в гриде область оси
        /// </summary>
        public Rectangle VisibleBounds
        {
            get { return this._visibleBounds; }
            set { this._visibleBounds = value; }
        }

        public Graphics Graphics
        {
            get { return this._graphics; }
            set { this._graphics = value; }
        }

        public Painter Painter
        {
            get { return this._painter; }
            set { this._painter = value; }
        }

        /// <summary>
        /// Стиль ячейки, распростаняется на все элементы оси
        /// </summary>
        public CellStyle Style
        {
            get { return this._style; }
            set { this._style = value; }
        }

        /// <summary>
        /// Растояние между свойствами в ячейке
        /// </summary>
        public int MemPropSeparatorHeight
        {
            get { return _memPropSeparatorHeight; }
        }

        /// <summary>
        /// Растояние между именем свойства и его значением в ячейке
        /// </summary>
        public int MemPropSeparatorWidth
        {
            get { return _memPropSeparatorWidth; }
        }

        /// <summary>
        /// Растояние между текстом в ячейке и свойствами
        /// </summary>
        public int MemPropPostTextSpacing
        {
            get { return _memPropPostTextSpacing; }
        }

        /// <summary>
        /// Растояние слева от текста ячейки до свойств
        /// </summary>
        public int MemPropPostTextLeftSpacing
        {
            get { return _memPropPostTextLeftSpacing; }
        }

        /// <summary>
        /// Стиль имени, в свойствах ячейки
        /// </summary>
        public CellStyle MemProperNameStyle
        {
            get { return _memProperNameStyle; }
            set { _memProperNameStyle = value; }
        }

        /// <summary>
        /// Стиль значения, в свойствах ячейки
        /// </summary>
        public CellStyle MemProperValueStyle
        {
            get { return _memProperValueStyle; }
            set 
            {
                if (_memProperValueStyle != null)
                    _memProperValueStyle.CopyDefaultStyle(value);
                _memProperValueStyle = value; 
            }
        }

        /// <summary>
        /// Отображать свойсва элемента в ячейках измерения
        /// </summary>
        public bool IsAppearPropInDimCells
        {
            get
            {
                //тип отображения свойств элементов
                MemberPropertiesDisplayType displayType = (this.AxisType == AxisType.Rows)
                    ? this.Grid.PivotData.RowAxis.PropertiesDisplayType
                    : this.Grid.PivotData.ColumnAxis.PropertiesDisplayType;

                return (displayType == MemberPropertiesDisplayType.DisplayInReport)
                    || (displayType == MemberPropertiesDisplayType.DisplayOverall);
            }
        }

        /// <summary>
        /// Отображать свойсва элемента в комментариях
        /// </summary>
        public bool IsAppearPropInComments
        {
            get
            {
                //тип отображения свойств элементов
                MemberPropertiesDisplayType displayType = (this.AxisType == AxisType.Rows)
                    ? this.Grid.PivotData.RowAxis.PropertiesDisplayType
                    : this.Grid.PivotData.ColumnAxis.PropertiesDisplayType;

                return (displayType == MemberPropertiesDisplayType.DisplayInHint)
                    || (displayType == MemberPropertiesDisplayType.DisplayOverall);
            }
        }

        /// <summary>
        /// Индекс позиции кортежа, с которой начинаестя главный итог
        /// </summary>
        public int GrandTotalPosition
        {
            get { return _grandTotalPosition; }
            set { _grandTotalPosition = value; }
        }

        /// <summary>
        /// Состояние раскрытости элементов
        /// </summary>
        public MembersExpandDictionary StateMembersExpand
        {
            get { return _stateMembersExpand; }
            set { _stateMembersExpand = value; }
        }

        /// <summary>
        /// Состояние раскрытости элементов с предыдущего действия
        /// </summary>
        public MembersExpandDictionary PrepareStateMembersExpand
        {
            get { return _prepareStateMembersExpand; }
            set { _prepareStateMembersExpand = value; }
        }

        /// <summary>
        /// Перерасчет координат элементов
        /// </summary>
        /// <param name="location">Место с расположение оси</param>
        public abstract void RecalculateCoordinates(Point location);

        /// <summary>
        /// Получить видимую область оси
        /// </summary>
        /// <returns>Rectangle</returns>
        public abstract Rectangle GetVisibleBounds();
     
        /// <summary>
        /// Инициализация оси
        /// </summary>
        /// <param name="cls">Источник данных</param>
        public abstract void InitializeMembers(CellSet cls);
       
        /// <summary>
        /// Ширина
        /// </summary>
        public abstract int Width { get;}
       
        /// <summary>
        /// Высота
        /// </summary>
        public abstract int Height { get;}
    }
}
