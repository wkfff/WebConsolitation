using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Microsoft.AnalysisServices.AdomdClient;

using Krista.FM.Client.MDXExpert.Common;

namespace Krista.FM.Client.MDXExpert.Grid
{
    /// <summary>
    /// Ячейка измерения
    /// </summary>
    public class DimensionCell : GridCell
    {
        private Axis _axis;
        private DimensionCell _parent;
        private List<DimensionCell> _children;
        private bool _isRoot;
        private bool _isTotal;
        private bool _isGrandTotal;
        private bool _isVisible = true;
        private bool _expand;
        private bool _isExistTotal;
        private bool _isDummy;
        private bool _isAverage;
        private bool _isStandartDeviation;
        private bool _isMedian;

        private bool _existCollapseButton;
        private int _depthLevel = 0;
        private int _tupleIndex = -1;
        private int _leafIndex = -1;
        private int _textHeight;
        private int _allParentHashCode;
        private CollapseButton collapseButton;
        private Rectangle _textBounds;
        private Member  _clsMember;
        private CellProperties _cellProperties;
        private string _tupleUN;

        public DimensionCell(Axis axis, DimensionCell parent)
            : base(axis.Grid, GridObject.DimensionCell)
        {
            this.Axis = axis;
            this.Parent = parent;
            this.Children = new List<DimensionCell>();
            if (parent != null)
            {
                parent.Children.Add(this);
            }
            base.Style = Axis.Style;
            this.collapseButton = new CollapseButton(this);
            this.CellProperties_ = new CellProperties(this);
        }

        public override void OnClick(Point mousePosition)
        {
            if (this.Grid.SelectionFrame.IsDrag)
                this.Grid.SelectionFrame.EndDrag(true);

            if (this.ExistCollapseButton)
            {
                if (this.collapseButton.Contain(mousePosition))
                {
                    this.collapseButton.OnClick(mousePosition);
                    return;
                }
            }

        }

        /// <summary>
        /// Если нажат дочерний элемент контрола вернет true
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override bool IsClickChildElement(Point point)
        {
            if (this.ExistCollapseButton)
            {
                if (this.collapseButton.Contain(point))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Получить относительные координаты ячейки
        /// </summary>
        /// <returns>Rectangle</returns>
        public override Rectangle GetOffsetBounds()
        {
            Rectangle result = this.Bounds;
            if (this.IsBelongToRowAxis)
                result.Y -= this.Grid.VScrollBarState.Offset;
            else
                result.X -= this.Grid.HScrollBarState.Offset;
            return result;
        }

        /// <summary>
        /// Получить видимую область родителя
        /// </summary>
        /// <returns></returns>
        public override Rectangle GetParentVisibleBounds()
        {
            return this.Axis.GetVisibleBounds();
        }

        public override Rectangle GetVisibleBounds()
        {
            return Rectangle.Intersect(this.Axis.GetVisibleBounds(), this.GetOffsetBounds());
        }

        /// <summary>
        /// К основному комментрию + комментарий от свойств
        /// </summary>
        /// <returns></returns>
        public override string GetComment()
        {
            string result = base.GetComment();
            if (!this.CellProperties_.IsEmpty && !this.IsGrandTotal && this.Axis.IsAppearPropInComments)
                result += "\n\n" + this.CellProperties_.GetComment();
            return result;
        }

        /// <summary>
        /// Хеш код ячейки
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.GetAllHashCode();
        }

        /// <summary>
        /// Получить координаты Текста в ячейке(т.к. в ячейке может еще рисовать крестик(схлапывание дочерник ячеек)
        /// то текст смещается)
        /// </summary>
        /// <param name="offsetBounds"></param>
        /// <returns>Point</returns>
        public Rectangle GetTextOffsetBounds(Rectangle offsetBounds)
        {
            Rectangle result = offsetBounds;
            if (this.IsBelongToRowAxis)
            {
                result.X += this.ExistCollapseButton ? this.Grid.GridScale.GetScaledValue(20) : 0;
                result.Y += (offsetBounds.Y < this.Axis.VisibleBounds.Y) ? 
                    (this.Axis.VisibleBounds.Y - offsetBounds.Y) : 0;
            }
            else
            {
                result.X += ((offsetBounds.X < this.Axis.VisibleBounds.X) ?
                    (this.Axis.VisibleBounds.X - offsetBounds.X) : 0) + (this.ExistCollapseButton ? this.Grid.GridScale.GetScaledValue(20) : 0);
            }
            result.Height = this.TextHeight;
            result.Width = offsetBounds.Right - result.X;
            return result;
        }

        public override void OnPaint(Graphics graphics, Painter painter)
        {
            //получаем координаты ячейки с учетом смещения
            base.OffsetBounds = this.GetOffsetBounds();

            //Если данный мембер фиктивный или лежит за пределами видимой части контрола, рисовать его не будем
            if (this.IsDummy || !this.Axis.VisibleBounds.IntersectsWith(base.OffsetBounds))
                return;

            this.TextOffsetBounds = this.GetTextOffsetBounds(base.OffsetBounds);
            painter.DrawDimensionCell(graphics, base.OffsetBounds, this.State, this.Style, this.Text,
                this.TextOffsetBounds);
            if (this.ExistCollapseButton)
            {
                this.collapseButton.OnPaint(graphics, painter);
            }
            this.CellProperties_.OnPaint(graphics, painter);
        }

        /// <summary>
        /// Очистка всех ссылок у ячеек
        /// </summary>
        public new void Clear()
        {
            this.collapseButton.Clear();
            this.collapseButton = null;
            this.CellProperties_.Clear();
            this.CellProperties_ = null;
            this.Axis = null;
            this.Parent = null;
            this.ClsMember = null;
            foreach (DimensionCell cell in this.Children)
            {
                cell.Clear();
            }
            this.Children.Clear();
        }

        /// <summary>
        /// Найти среди дочерних элементов ИТОГ
        /// </summary>
        /// <returns>DimensionCell(Итог)</returns>
        public DimensionCell GetTotal()
        {
            if (this.HasChilds)
            {
                //начинаем искать с конца, т.к. чаще всего итоги размещаются именно там... В дальнейшем если будет 
                //заведен признак размещения итогов в начале или конце, то обязатально будем учитыватье его здесь!!!
                for (int i = this.Children.Count - 1; i > -1; i--)
                {
                    if (this.Children[i].IsTotal)
                        return this.Children[i];
                    //Если есть фиктивные ячейки, тогда итог будем смотреть у них
                    if (this.Children[i].IsDummy)
                        return this.Children[i].GetTotal();
                }
            }
            return null;
        }

        /// <summary>
        /// Найти среди дочерних элементов среднее значение
        /// </summary>
        /// <returns>DimensionCell(Среднее значение)</returns>
        public DimensionCell GetAverage()
        {
            if (this.HasChilds)
            {
                //начинаем искать с конца, т.к. чаще всего среднее значение размещается именно там... В дальнейшем если будет 
                //заведен признак размещения среднего в начале или конце, то обязатально будем учитывать его здесь!!!
                for (int i = this.Children.Count - 1; i > -1; i--)
                {
                    if (this.Children[i].IsAverage)
                        return this.Children[i];
                    //Если есть фиктивные ячейки, тогда среднее значение будем смотреть у них
                    if (this.Children[i].IsDummy)
                        return this.Children[i].GetAverage();
                }
            }
            return null;
        }

        /// <summary>
        /// Найти среди дочерних элементов медиану
        /// </summary>
        /// <returns>DimensionCell(Медиана)</returns>
        public DimensionCell GetMedian()
        {
            if (this.HasChilds)
            {
                //начинаем искать с конца, т.к. чаще всего медиана размещается именно там... В дальнейшем если будет 
                //заведен признак размещения медианы в начале или конце, то обязатально будем учитывать его здесь!!!
                for (int i = this.Children.Count - 1; i > -1; i--)
                {
                    if (this.Children[i].IsMedian)
                        return this.Children[i];
                    //Если есть фиктивные ячейки, тогда медиану будем смотреть у них
                    if (this.Children[i].IsDummy)
                        return this.Children[i].GetMedian();
                }
            }
            return null;
        }

        /// <summary>
        /// Найти среди дочерних элементов стандартное отклонение
        /// </summary>
        /// <returns>DimensionCell(стандартное отклонение)</returns>
        public DimensionCell GetStandartDeviation()
        {
            if (this.HasChilds)
            {
                //начинаем искать с конца, т.к. чаще всего стандартное отклонение размещается именно там... В дальнейшем если будет 
                //заведен признак размещения стандартное отклонение в начале или конце, то обязатально будем учитывать его здесь!!!
                for (int i = this.Children.Count - 1; i > -1; i--)
                {
                    if (this.Children[i].IsStandartDeviation)
                        return this.Children[i];
                    //Если есть фиктивные ячейки, тогда стандартное отклонение будем смотреть у них
                    if (this.Children[i].IsDummy)
                        return this.Children[i].GetStandartDeviation();
                }
            }
            return null;
        }


        /// <summary>
        /// Определяет является ли ячейка видимой
        /// </summary>
        /// <returns>bool(видимость)</returns>
        public bool GetVisibility()
        {
            if (this.IsTotal)
            {
                return this.Parent.GetVisibility();
            }
            else
            {
                DimensionCell parent = this.Parent;
                bool parentCollapsed = true;

                do
                {
                    parentCollapsed = (parent == null) ? false : parent.HaveNotDummyChild() ? !parent.Expanded : false;
                    if (parent != null)
                        parent = parent.Parent;
                }
                while ((parent != null) && (!parentCollapsed));
                return !parentCollapsed;
            }
        }

        /// <summary>
        /// Получить первого видимого родителя
        /// </summary>
        /// <returns>DimensionCell</returns>
        public DimensionCell GetVisibleParent()
        {
            DimensionCell result = this.Parent;

            while ((result != null) && (!result.GetVisibility()))
            {
                result = result.Parent;
            }

            return result;
        }

        /// <summary>
        /// Инициализируем наличие кнопки
        /// </summary>
        public void InitCollapseButton()
        {
            this.ExistCollapseButton = this.HaveNotDummyChild();
            if (!this.ExistCollapseButton)
            {
                if (this.IsPossibleAddLevel())
                {
                    this.ExistCollapseButton = true;
                    //выставляем только признак, без запоминания состояния
                    this.SetExpaned(false, false);
                }
            }
        }

        /// <summary>
        /// Возможно ли добавить следующий уровень
        /// </summary>
        /// <returns></returns>
        public bool IsPossibleAddLevel()
        {
            bool result = false;
            if ((this.Grid.PivotData != null) && !this.Grid.PivotData.IsCustomMDX)
            {
                if (!this.IsTotal && !this.IsDummy && !this.HaveNotDummyChild() && (this.ClsMember != null) && !this.IsAverage && !this.IsStandartDeviation && !this.IsMedian)
                {
                    CaptionsList captionList = this.IsBelongToRowAxis ? (CaptionsList)this.Grid.RowCaptions
                        : (CaptionsList)this.Grid.ColumnCaptions;

                    Krista.FM.Client.MDXExpert.Data.PivotField pivotField = null;
                    if (captionList.Count > 0)
                    {
                        string levelUN = captionList[this.DepthLevel].UniqueName;
                        pivotField = this.Grid.PivotData.GetPivotField(levelUN);
                    }

                    if ((pivotField != null) && pivotField.IsLastFieldInSet)
                    {
                        //если элемент листовой, имеет детей, и уровень которому он принаделжит не последний,
                        //значит возможно вывести дочерний уровень
                        Level parentLevel = this.ClsMember.ParentLevel;
                        bool isHasChild = this.ClsMember.ChildCount > 0;
                        result = isHasChild && (parentLevel.LevelNumber < parentLevel.ParentHierarchy.Levels.Count - 1);
                    }
                    /*string fieldSetUN = captionList[this.DepthLevel].HierarchyUN;
                    Krista.FM.Client.MDXExpert.Data.FieldSet fieldSet = this.Grid.PivotData.GetFieldSet(fieldSetUN);
                    result = (fieldSet != null) && !fieldSet.IsLastSetInAxis;*/
                    //так же раскрытие возможно если уровень следующий за текущем еще не включен в запрос
                    if (!result)
                    {
                        Krista.FM.Client.MDXExpert.Data.PivotField nextField = (pivotField != null) ?
                            pivotField.GetNextField() : null;
                        result = (nextField != null) && !nextField.IsIncludeToQuery;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Если у ячейки есть хотя бы один не фиктивный ребенок, вернет true
        /// </summary>
        public bool HaveNotDummyChild()
        {
            if (this.HasChilds)
            {
                DimensionCell firstChild = this;
                do
                {
                    firstChild = firstChild.Children[0];
                    if (!firstChild.IsDummy)
                        return true;
                }
                while (firstChild.HasChilds);
            }
            return false;
        }

        /// <summary>
        /// Вернет последнего потомка
        /// </summary>
        /// <returns></returns>
        public DimensionCell GetLastDescendant()
        {
            DimensionCell result = this;
            while (result.HasChilds)
            {
                result = result.Children[result.Children.Count - 1];
            }
            return result;
        }

        /// <summary>
        /// Вернет последнего потомка из того же измерения
        /// </summary>
        /// <returns></returns>
        public DimensionCell GetLastDescendantInDimension()
        {
            DimensionCell result = this;
            DimensionCell child = this;
            while (result.HasChilds)
            {
                child = result.Children[result.Children.Count - 1];
                if (child.ClsMember.ParentLevel.ParentHierarchy == result.ClsMember.ParentLevel.ParentHierarchy)
                {
                    result = child;
                }
                else
                    return result;
            }
            return result;
        }


        /// <summary>
        /// Получает хеш код всех родителей ячейки и самой себя
        /// </summary>
        /// <param name="dimCell"></param>
        /// <returns></returns>
        public string GetAllHashCodeStr()
        {
            return this.GetAllHashCode().ToString();
        }

        /// <summary>
        /// Получает хеш код всех родителей ячейки и самой себя
        /// </summary>
        /// <param name="dimCell"></param>
        /// <returns></returns>
        public int GetAllHashCode()
        {
            return this.GetAllUniqueName().GetHashCode();
        }

        /// <summary>
        /// Получает свой и всех своих родителей UN 
        /// </summary>
        /// <returns></returns>
        public string GetAllUniqueName()
        {
            return this.GetAllUniqueName(string.Empty);
        }

        /// <summary>
        /// Получает свой и всех своих родителей UN 
        /// </summary>
        /// <param name="splitter">разделитель между UN</param>
        /// <returns></returns>
        public string GetAllUniqueName(string splitter)
        {
            string result = string.Empty;
            DimensionCell dimCell = this;

            while (dimCell != null)
            {
                if (!dimCell.IsDummy && !dimCell.IsRoot)
                {
                    if (dimCell.ClsMember != null)
                    {
                        string un = dimCell.ClsMember.UniqueName;
                        result += (result == string.Empty) ? un : splitter + un;
                    }
                }

                dimCell = dimCell.Parent;
            }
            return result;
        }

        /// <summary>
        /// Развернуть/свернуть элемент
        /// </summary>
        public void ExpandMember()
        {
            this.ExpandMember(!this.Expanded);
        }

        /// <summary>
        /// Развернуть/свернуть элемент
        /// </summary>
        public void ExpandMember(bool state)
        {
            if (this.IsPossibleAddLevel())
                //если выводим новый уровень, договорились все остальные элементы на уровне
                //скрывать
                this.Axis.ExpandAllMember(this.DepthLevel, false);

            this.Expanded = state;

            if (this.ClsMember != null)
            {
                Level level = this.ClsMember.ParentLevel;
                string hierarchyUN = level.ParentHierarchy.UniqueName;
                string levelUN = level.UniqueName;
                this.Grid.OnExpandedMember(hierarchyUN, levelUN, this.Expanded);
            }
            if (this.Grid != null)
                this.Grid.RecalculateGrid();
        }

        /*
         * Из за того что у итогов не могли собрать все UN кортежа, было решено не собирать
         * каждый раз имя кортежа, а при инициализации оси просто записывать в поле элемента TupleUN
        /// <summary>
        /// Вернет уникальные имена элементов кортежа CellSet, в котором был расположен данный элемент
        /// </summary>
        /// <returns></returns>
        public string GetTupleUN()
        {
            string result = string.Empty;
            string prepareDimUN = string.Empty;
            DimensionCell dimCell = this;


                while (dimCell != null)
                {
                    string currentDimUN = dimCell.ClsMember != null ? dimCell.ClsMember.ParentLevel.ParentHierarchy.UniqueName : string.Empty;
                    if (!dimCell.IsDummy && !dimCell.IsRoot && (prepareDimUN != currentDimUN))
                    {
                        result += dimCell.ClsMember != null ? dimCell.ClsMember.UniqueName : string.Empty;
                        prepareDimUN = currentDimUN;
                    }
                    dimCell = dimCell.Parent;
                }
            return result;
        }*/

        /// <summary>
        /// Родитель
        /// </summary>
        public DimensionCell Parent
        {
            get { return this._parent; }
            set { this._parent = value; }
        }

        /// <summary>
        /// Дети
        /// </summary>
        public List<DimensionCell> Children
        {
            get { return this._children; }
            set { this._children = value; }
        }

        /// <summary>
        /// Является корнем
        /// </summary>
        public bool IsRoot
        {
            get { return this._isRoot; } 
            set { this._isRoot = value; }
        }

        /// <summary>
        /// Имеет детей
        /// </summary>
        public bool HasChilds
        {
            get
            {
                return (this.Children != null) && (this.Children.Count > 0);
            }
        }

        /// <summary>
        /// Развернута
        /// </summary>
        public bool Expanded
        {
            get { return this._expand; }
            set { this.SetExpaned(value, true); }
        }

        /// <summary>
        /// Выставить признак раскрытости
        /// </summary>
        /// <param name="state">состояние</param>
        /// <param name="isSaveStateMember">запоминать ли состояние раскрытости</param>
        public void SetExpaned(bool state, bool isSaveStateMember)
        {
            this._expand = state;
            if (!this.IsDummy && isSaveStateMember)
                this.Axis.StateMembersExpand.SetState(this);
            //Всем фиктивным ячейкам выставляем такой же признак
            if (this.HasChilds && this.Children[0].IsDummy)
                this.Children[0].Expanded = state;
        }

        /// <summary>
        /// Уровень вложености
        /// </summary>
        public int DepthLevel
        {
            get { return this._depthLevel; }
            set { this._depthLevel = value; }
        }

        /// <summary>
        /// Границы текста в ячейке
        /// </summary>
        public Rectangle TextOffsetBounds
        {
            get { return this._textBounds; }
            set { this._textBounds = value; }
        }

        /// <summary>
        /// Ось
        /// </summary>
        public Axis Axis
        {
            get { return this._axis; }
            set { this._axis = value; }
        }

        /// <summary>
        /// Видимость ячейки
        /// </summary>
        public bool IsVisible
        {
            get { return this._isVisible; }
            set { this._isVisible = value; }
        }

        /// <summary>
        /// Является ли итогом
        /// </summary>
        public bool IsTotal
        {
            get
            {
                return (this._isTotal || this.IsGrandTotal);
            }
            set
            {
                if (value)
                {
                    DimensionCell parent = this.Parent;
                    while (parent != null)
                    {
                        parent.IsExistTotal = true;
                        //если родительская ячейка Фиктивная, ищем действительного родителя и 
                        //выставляем этот признак ему
                        parent = parent.IsDummy ? parent.Parent : null;
                    }
                }
                this._isTotal = value;
            }
        }

        /// <summary>
        /// Является ли "Общим итогом"
        /// </summary>
        public bool IsGrandTotal
        {
            get { return _isGrandTotal; }
            set { _isGrandTotal = value; }
        }

        /// <summary>
        /// Является ли листовым элементом
        /// </summary>
        public bool IsLeaf
        {
            get
            {
                return !this.IsRoot && (this.IsTotal || !this.HasChilds || this.IsAverage || this.IsStandartDeviation || this.IsMedian);
            }
        }

        /// <summary>
        /// Является ли ячейка видимым листом
        /// </summary>
        public bool IsVisibleLeaf
        {
            get
            {
                return (!this.HasChilds && this.IsVisible)
                    || ((this.IsTotal) && (this.Parent != null) && this.Parent.IsVisible);
            }
        }
        
        /// <summary>
        /// Существует ли у ячейки среди детей итог
        /// </summary>
        public bool IsExistTotal
        {
            get { return this._isExistTotal; }
            set { this._isExistTotal = value; }
        }

        /// <summary>
        /// Признак фиктивности ячейки(т.к. при составлении дерева оси, существует моменты когда для правильного
        /// его отображения приходится вставлять фиктивные узлы)
        /// </summary>
        public bool IsDummy
        {
            get { return this._isDummy; }
            set { this._isDummy = value; }
        }

        /// <summary>
        /// Требует схлапывающей кнопки
        /// </summary>
        public bool ExistCollapseButton
        {
            get { return this._existCollapseButton; }
            set { this._existCollapseButton = value; }
        }

        /// <summary>
        /// Индекс в кортеже, которому соответсвует данный мембер
        /// </summary>
        public int TupleIndex
        {
            get { return this._tupleIndex; }
            set { this._tupleIndex = value; }
        }

        /// <summary>
        /// Индекс мембера в списке листов
        /// </summary>
        public int LeafIndex
        {
            get { return this._leafIndex; }
            set { this._leafIndex = value; }
        }

        /// <summary>
        /// Уникальное имя мембера из селсета, котороый принадлежит ячейке
        /// </summary>
        public Member ClsMember
        {
            get { return this._clsMember; }
            set 
            {
                this._clsMember = value;
                if (value != null)
                    this.CellProperties_.Initialization(value.MemberProperties);
            }
        }

        /// <summary>
        /// Высота текста в ячейке
        /// </summary>
        public int TextHeight
        {
            get { return this._textHeight; }
            set { this._textHeight = value; }
        }

        /// <summary>
        /// Хеш код юник неймов всех родителей
        /// </summary>
        public int AllParentHashCode
        {
            get { return _allParentHashCode; }
            set { _allParentHashCode = value; }
        }

        /// <summary>
        /// Свойства мембера
        /// </summary>
        public CellProperties CellProperties_
        {
            get { return this._cellProperties; }
            set { this._cellProperties = value; }
        }

        /// <summary>
        /// Ячейка принадлежит к оси строк
        /// </summary>
        public bool IsBelongToRowAxis
        {
            get
            {
                return this.Axis.AxisType == AxisType.Rows;
            }
        }

        /// <summary>
        /// Ячейка принадлежит к оси колонок
        /// </summary>
        public bool IsBelongToColumnsAxis
        {
            get { return this.Axis.AxisType == AxisType.Columns; }
        }

        /// <summary>
        /// Последний ли ребенок у родителя
        /// </summary>
        public bool IsLastChild
        {
            get
            {
                bool result = false;
                if (this.Parent != null)
                {
                    result = (this.Parent.Children[this.Parent.Children.Count - 1] == this);
                }
                return result;
            }
        }

        /// <summary>
        /// Уникальное имя кортежа, в котором располагается элемент
        /// </summary>
        public string TupleUN
        {
            get { return _tupleUN; }
            set { _tupleUN = value; }
        }

        /// <summary>
        /// Ячейка заголовка к которой относится данный элемент измеренеия
        /// </summary>
        public CaptionCell CaptionCell
        {
            get
            {
                CaptionsList captionList = this.IsBelongToRowAxis ? (CaptionsList)this.Grid.RowCaptions
                    : (CaptionsList)this.Grid.ColumnCaptions;
                return captionList[this.DepthLevel];
            }
        }

        public bool IsAverage
        {
            get { return _isAverage; }
            set { _isAverage = value; }
        }

        public bool IsStandartDeviation
        {
            get { return _isStandartDeviation; }
            set { _isStandartDeviation = value; }
        }

        public bool IsMedian
        {
            get { return _isMedian; }
            set { _isMedian = value; }
        }


        /// <summary>
        /// Кнопка схлапывания ячейки
        /// </summary>
        private class CollapseButton : GridCell
        {
            private DimensionCell _dimCell;

            public DimensionCell DimCell
            {
                get { return _dimCell; }
                set { _dimCell = value; }
            }

            public CollapseButton(DimensionCell dimCell)
                : base(dimCell.Grid, GridObject.CollapseButton)
            {
                this.DimCell = dimCell;
                base.Size = new Size(10, 10);
                base.Location = new Point(2, 2);
            }

            public bool Contain(Point mousePosition)
            {
                return (base.GetHitTest(mousePosition, true) ||
                    this.GetTriangleBounds(this.GetOffsetBounds()).Contains(mousePosition));
            }

            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }

            public override void OnClick(Point mousePosition)
            {
                if (base.GetHitTest(mousePosition, true))
                {
                    this.DimCell.ExpandMember();
                }
                else
                {
                    //Если кликнули по треугольнику, фокусируем вид таблицы на начале этой ячейки
                    if (this.GetTriangleBounds(this.GetOffsetBounds()).Contains(mousePosition))
                        this.ClickByTriangle(mousePosition);
                }
            }

            private void ClickByTriangle(Point mousePosition)
            {
                if (this.DimCell.IsBelongToRowAxis)
                {
                    this.Grid.VScrollBarState.FocusCell = this.DimCell;
                    this.Grid.VScrollBarState.ScrollDirection = Direction.BottomUp;
                }
                else
                {
                    this.Grid.HScrollBarState.FocusCell = this.DimCell;
                    this.Grid.HScrollBarState.ScrollDirection = Direction.RightToLeft;
                }
                this.Grid.SetScrollPosition();
                this.Grid.DrawGrid(AreaSet.All);
            }

            public override void OnPaint(Graphics graphics, Painter painter)
            {
                if (this.DimCell != null)
                {
                    base.OffsetBounds = this.GetOffsetBounds();
                    //если ячейка большая и ее начало не видно, будем рисовать треугольничек с намеком на продолжение 
                    //этой ячейки
                    if (this.DimCell.Axis.VisibleBounds.IntersectsWith(base.OffsetBounds))
                    {
                        painter.DrawCollapseButton(graphics, base.OffsetBounds, this.DimCell.Expanded);
                    }
                    else
                    {
                        painter.DrawShiftButton(graphics, this.GetTriangleBounds(base.OffsetBounds),
                            (this.DimCell.IsBelongToRowAxis ? Direction.BottomUp : Direction.RightToLeft));
                    }
                }
            }

            public new void Clear()
            {
                this.DimCell = null;
                base.Clear();
            }

            public override Rectangle GetOffsetBounds()
            {
                Rectangle bounds = this.DimCell.OffsetBounds;
                bounds.Size = this.Size; 
                bounds.Y += base.Location.X;
                bounds.X += base.Location.Y;
                return bounds;
            }

            public override Rectangle GetVisibleBounds()
            {
                return Rectangle.Intersect(this.DimCell.GetVisibleBounds(), this.GetOffsetBounds());
            }

            /// <summary>
            /// Получить видимую область родителя
            /// </summary>
            /// <returns></returns>
            public override Rectangle GetParentVisibleBounds()
            {
                return this.DimCell.GetVisibleBounds();
            }

            /// <summary>
            /// Получить границы треугольника(признак что ячейка отображается не с начала)
            /// </summary>
            /// <param name="offsetBounds">Границы схлапывающей кнопки(CollapseButton)</param>
            /// <returns>Rectangle</returns>
            private Rectangle GetTriangleBounds(Rectangle offsetBounds)
            {
                offsetBounds.Y = this.DimCell.TextOffsetBounds.Y + 1;
                offsetBounds.X = this.DimCell.TextOffsetBounds.X - 18;
                return offsetBounds;
            }

            public override bool IsClickChildElement(Point point)
            {
                return false;
            }
        }
    }
}
