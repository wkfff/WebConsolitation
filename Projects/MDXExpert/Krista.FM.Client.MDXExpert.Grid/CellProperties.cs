using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Grid.Style;
using Microsoft.AnalysisServices.AdomdClient;

namespace Krista.FM.Client.MDXExpert.Grid
{
    /// <summary>
    /// ��������� ������������ MemberProperties
    /// </summary>
    public class CellProperties
    {
        private ExpertGrid _grid;
        private Point _location;
        private DimensionCell _dimCell;
        private List<PropertyElement> _propertyNames;
        private List<PropertyElement> _propertyValues;

        public CellProperties(DimensionCell dimCell)
        {
            this.DimCell = dimCell;
            this.Grid = dimCell.Grid;
            this.PropertyNames = new List<PropertyElement>();
            this.PropertyValues = new List<PropertyElement>();
        }

        /// <summary>
        /// ������ ���������� ������� �� ������ - null
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
        /// ������ ��������� �������, ������� ������������� ����� ������� ������ ������ � ������
        /// </summary>
        public void RecalulateCoordinate()
        {
            if (((this.MemberProperties == null) || (this.MemberProperties.Count == 0)) || this.DimCell.IsTotal
                || !this.DimCell.Axis.IsAppearPropInDimCells)
                return;
            Point location = this.GetLocation();
            int width = this.DimCell.Bounds.Right - location.X;

            MemberPropertyCollection memberProperties = this.MemberProperties;
            string[] nameList = this.GetPropertiesNames(memberProperties);
            //������ ������������ ������ �����, +4 - ��� ����� ��� ":"
            int maxNameWidth = CommonUtils.GetStringMaxWidth(this.DimCell.Grid.GridGraphics, nameList,
                this.DimCell.Axis.MemProperNameStyle.Font) + 4;

            int nameHeight = this.DimCell.Axis.MemProperNameStyle.TextHeight;
            int valueHeight = this.DimCell.Axis.MemProperValueStyle.TextHeight;
            int heightOdds = (int)(Math.Abs(nameHeight - valueHeight) / 2);

            for (int i = 0; i < memberProperties.Count; i++)
            {
                if (memberProperties[i].Value == null)
                    continue;

                PropertyElement propertyName = this.PropertyNames[i];
                propertyName.Location = new Point(location.X, (nameHeight < valueHeight) ? location.Y + heightOdds :
                    location.Y);
                propertyName.Size = new Size(maxNameWidth, nameHeight);

                PropertyElement propertyValue = this.PropertyValues[i];
                propertyValue.Location = new Point(location.X + maxNameWidth + this.DimCell.Axis.MemPropSeparatorWidth,
                    (nameHeight > valueHeight) ? location.Y + heightOdds : location.Y);
                propertyValue.Size = new Size(width - maxNameWidth, valueHeight);

                location.Y += Math.Max(nameHeight, valueHeight) + this.DimCell.Axis.MemPropSeparatorHeight;
            }
        }

        /// <summary>
        /// ������������� �������
        /// </summary>
        /// <param name="memberPropertys"></param>
        public void Initialization(MemberPropertyCollection memberPropertys)
        {
            this.ClearProperties();
            
            if ((memberPropertys == null) || this.DimCell.IsTotal)
                return;

            for (int i = 0; i < memberPropertys.Count; i++)
            {
                this.PropertyNames.Add(new PropertyElement(this));
                this.PropertyValues.Add(new PropertyElement(this));
            }
        }

        /// <summary>
        /// ��������� �������� ��� ��������� ��������� �������, ��� ������������ ���
        /// ��������� �� ������������� ���������
        /// </summary>
        /// <param name="dimensionCell">������ � ������� ����������� ��������</param>
        /// <returns></returns>
        private Point GetOffsetLocation(DimensionCell dimensionCell)
        {
            Point result = Point.Empty;

            if (dimensionCell.IsBelongToColumnsAxis)
            {
                result.X = ((dimensionCell.OffsetBounds.X < dimensionCell.Axis.VisibleBounds.X) ?
                    (dimensionCell.Axis.VisibleBounds.X - dimensionCell.OffsetBounds.X) : 0)
                    - dimensionCell.Grid.HScrollBarState.Offset;
            }
            else
            {
                result.Y = ((dimensionCell.OffsetBounds.Y < dimensionCell.Axis.VisibleBounds.Y) ?
                    (dimensionCell.Axis.VisibleBounds.Y - dimensionCell.OffsetBounds.Y) : 0)
                    - dimensionCell.Grid.VScrollBarState.Offset;
            }
            return result;
        }

        public void OnPaint(Graphics graphics, Painter painter)
        {
            if (this.DimCell.Axis.IsAppearPropInDimCells 
                && (this.DimCell != null)
                && (this.DimCell.ClsMember != null) 
                && (this.MemberProperties.Count > 0)
                && !this.DimCell.IsTotal)
            {
                CellStyle nameStyle = this.DimCell.Axis.MemProperNameStyle;
                CellStyle valueStyle = this.DimCell.Axis.MemProperValueStyle;

                //�������� �������� ��� ��������� �������
                Point offsetLocation = this.GetOffsetLocation(this.DimCell);

                MemberPropertyCollection memberPropertys = this.MemberProperties;
                for (int i = 0; i < memberPropertys.Count; i++)
                {
                    MemberProperty memberProperty = memberPropertys[i];

                    if (memberProperty.Value == null)
                        continue;

                    string strValue = GetMemberPropertyValue(memberProperty.Value);

                    graphics.DrawString(memberProperty.Name + ":", nameStyle.Font, nameStyle.ForeColorBrush,
                        this.PropertyNames[i].GetOffsetBounds(offsetLocation));
                    graphics.DrawString(strValue, valueStyle.Font, valueStyle.ForeColorBrush,
                        //memberProperty.Value.ToString(), valueStyle.Font, valueStyle.ForeColorBrush,
                        this.PropertyValues[i].GetOffsetBounds(offsetLocation));
                }
            }
        }

        /// <summary>
        /// ������� ��������� ������� � ������
        /// </summary>
        public void Clear()
        {
            this.DimCell = null;
            this.ClearProperties();
        }

        /// <summary>
        /// ������� ��������� � ��������� �������
        /// </summary>
        public void ClearProperties()
        {
            this.PropertyNames.Clear();
            this.PropertyValues.Clear();
        }

        /// <summary>
        /// �������� ��������� ������������ �������
        /// </summary>
        /// <returns></returns>
        private Point GetLocation()
        {
            return new Point(this.DimCell.Location.X + (this.DimCell.ExistCollapseButton ? 20 : 0)
                + this.DimCell.Axis.MemPropPostTextLeftSpacing,
                this.DimCell.Location.Y + this.DimCell.TextHeight + this.DimCell.Axis.MemPropPostTextSpacing);
        }

        /// <summary>
        /// �������� ������ ���� � �������
        /// </summary>
        /// <param name="memberPropertys"></param>
        /// <returns></returns>
        private string[] GetPropertiesNames(MemberPropertyCollection memberPropertys)
        {
            string[] result = new string[memberPropertys.Count];
            for (int i = 0; i < memberPropertys.Count; i++)
            {
                if (memberPropertys[i].Value != null)
                    result[i] = memberPropertys[i].Name;
            }
            return result;
        }

        /// <summary>
        /// �������� �����������
        /// </summary>
        /// <returns></returns>
        public string GetComment()
        {
            StringBuilder result = new StringBuilder();
            if (this.MemberProperties == null)
                return result.ToString();

            foreach(MemberProperty property in this.MemberProperties)
            {
                if (property.Value != null)
                    result.AppendLine(string.Format("{0}: {1}", property.Name, 
                        GetMemberPropertyValue(property.Value)));
            }
            return result.ToString();
        }

        /// <summary>
        /// ��� ������ � MASS2005 ��� �������� �������� � member propertys ������������ � 
        /// ���������������� ����, ����� ������������� �� � ����������� ���
        /// </summary>
        /// <param name="memberProperty"></param>
        /// <returns></returns>
        public static string GetMemberPropertyValue(object value)
        {
            string result = String.Empty;
            if (value == null)
            {
                return result;
            }
            else
            {
                result = value.ToString();
            }

            if (((Data.PivotData.AnalysisServicesVersion == Data.AnalysisServicesVersion.v2005)||
                (Data.PivotData.AnalysisServicesVersion == Data.AnalysisServicesVersion.v2008))
                && (result.Contains("E")))
            {
                string temp = result.Replace('.', ',');
                double nValue;
                if (double.TryParse(temp, out nValue))
                {
                    result = nValue.ToString();
                }
            }
            return result;
        }

        public ExpertGrid Grid
        {
            get { return _grid; }
            set { _grid = value; }
        }

        /// <summary>
        /// ������������ �������
        /// </summary>
        public Point Location
        {
            get { return _location; }
            set { _location = value; }
        }

        /// <summary>
        /// ������ ���������, � ������� ����������� ������ ����� �������
        /// </summary>
        public DimensionCell DimCell
        {
            get { return this._dimCell; }
            set { this._dimCell = value; }
        }

        /// <summary>
        /// ��������� ��������� ���� �������
        /// </summary>
        internal List<PropertyElement> PropertyNames
        {
            get { return _propertyNames; }
            set { _propertyNames = value; }
        }

        /// <summary>
        /// ��������� ��������� �������� �������
        /// </summary>
        internal List<PropertyElement> PropertyValues
        {
            get { return _propertyValues; }
            set { _propertyValues = value; }
        }

        /// <summary>
        /// ���� �������� 
        /// </summary>
        internal MemberPropertyCollection MemberProperties
        {
            get
            {
                if (this.DimCell.ClsMember != null)
                    return this.DimCell.ClsMember.MemberProperties;
                else
                    return null;
            }
        }

        /// <summary>
        /// ���� �������� 
        /// </summary>
        internal LevelPropertyCollection LevelProperties
        {
            get
            {
                if (this.DimCell.ClsMember != null)
                    return this.DimCell.ClsMember.ParentLevel.LevelProperties;
                else
                    return null;
            }
        }


        /// <summary>
        /// ������ �������
        /// </summary>
        public int Height
        {
            get
            {
                if (this.IsEmpty || this.DimCell.IsTotal || !this.DimCell.Axis.IsAppearPropInDimCells)
                    return 0;

                int nameHeight = this.DimCell.Axis.MemProperNameStyle.TextHeight;
                int valueHeight = this.DimCell.Axis.MemProperValueStyle.TextHeight;
                int maxHeight = Math.Max(nameHeight, valueHeight);
                int memPropCount = this.GetMemberPropertiesCount(this.MemberProperties);
                //���� � ������� ��� ������, ������ � ����� ������� ���...
                if (memPropCount == 0)
                    return 0;
                return memPropCount * (maxHeight + this.DimCell.Axis.MemPropSeparatorHeight)
                    + this.DimCell.Axis.MemPropPostTextSpacing;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return (this.PropertyNames.Count == 0);
            }
        }

    }

    public class PropertyElement
    {
        private Point _location;
        private Size _size;
        private CellProperties _cellProperties;

        public PropertyElement(CellProperties cellProperties)
        {
            this.CellProperties_ = cellProperties;
        }

        public Rectangle GetOffsetBounds()
        {
            DimensionCell dimCell = this.CellProperties_.DimCell;
            Point offsetLocation = Point.Empty;
            if (dimCell.IsBelongToRowAxis)
            {
                offsetLocation.X = ((dimCell.OffsetBounds.X < dimCell.Axis.VisibleBounds.X) ?
                    (dimCell.Axis.VisibleBounds.X - dimCell.OffsetBounds.X) : 0) 
                    - dimCell.Grid.HScrollBarState.Offset;
            }
            else
            {
                offsetLocation.Y = ((dimCell.OffsetBounds.Y < dimCell.Axis.VisibleBounds.Y) ?
                    (dimCell.Axis.VisibleBounds.Y - dimCell.OffsetBounds.Y) : 0) 
                    - dimCell.Grid.VScrollBarState.Offset;
            }
            return this.GetOffsetBounds(offsetLocation);
        }

        public Rectangle GetOffsetBounds(Point offsetLocation)
        {
            Rectangle result = this.Bounds;

            if (this.CellProperties_.DimCell.IsBelongToRowAxis)
                result.Y += offsetLocation.Y;
            else
                result.X += offsetLocation.X;
            return result;
        }

        public Size Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public Point Location
        {
            get { return _location; }
            set { _location = value; }
        }

        public CellProperties CellProperties_
        {
            get { return _cellProperties; }
            set { _cellProperties = value; }
        }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(this.Location, this.Size);
            }
        }
    }
}
