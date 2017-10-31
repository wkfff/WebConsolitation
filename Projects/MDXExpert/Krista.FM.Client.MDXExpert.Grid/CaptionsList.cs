using System.Collections.Generic;
using System.Drawing;
using Krista.FM.Client.MDXExpert.Grid.Style;
using Krista.FM.Common.Xml;
using Krista.FM.Client.MDXExpert.Data;
using System.Xml;

namespace Krista.FM.Client.MDXExpert.Grid
{
    /// <summary>
    /// ��������� ����������
    /// </summary>
    public abstract class CaptionsList : GridCollection<CaptionCell>
    {
        private CaptionType _type;
        private CellStyle _style;
        private bool _visible;

        /// <summary>
        /// ����������� ��������� ����������
        /// </summary>
        /// <param name="grid">������ �� ����</param>
        /// <param name="type">��� ���������</param>
        public CaptionsList(ExpertGrid grid, CaptionType type)
        {
            this.Grid = grid;
            this._type = type;

            this.Style = new CellStyle(this.Grid, GridConsts.gridRowsBackColorStart, GridConsts.gridRowsBackColorEnd,
                GridConsts.gridAxisForeColor, GridConsts.gridCommonBorderColor);
                
            //���� ����� �� ���������� � ������, �������� ��� � ������ ����������
            this.Style.StringFormat.Trimming = StringTrimming.EllipsisCharacter;
            this.Style.StringFormat.FormatFlags = StringFormatFlags.LineLimit;
            this.Visible = true;
        } 

        /// <summary>
        /// ��������� ���� ���������� � ���������
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="painter"></param>
        public override void Draw(Graphics graphics, Painter painter)
        {
            if (this.IsEmpty)
                return;
            Region oldClip = graphics.Clip;
            try
            {
                this.VisibleBounds = this.GetVisibleBounds();
                graphics.Clip = new Region(this.VisibleBounds);
                foreach (CaptionCell caption in this)
                {
                    caption.OnPaint(graphics, painter);
                }
            }
            finally
            {
                graphics.Clip = oldClip;
                graphics = null;
            }
        }

        /// <summary>
        /// �� ��������� ����������� ���� ������ � ���������
        /// </summary>
        /// <param name="mousePoint">���������� ��� ������</param>
        /// <returns>CaptionCell (���������)</returns>
        public CaptionCell FindCaption(Point mousePoint)
        {
            foreach (CaptionCell caption in this)
            {
                if (caption.GetHitTest(mousePoint))
                    return caption;
            }
            return null;
        }

        /// <summary>
        /// ���� ������ ���������� � �������
        /// </summary>
        /// <param name="searchBounds">������� ��� ������</param>
        /// <returns>������ �����, ���������� � �������</returns>
        public List<GridControl> FindCaptions(Rectangle searchBounds)
        {
            List<GridControl> result = new List<GridControl>();

            foreach (CaptionCell caption in this)
            {
                if (caption.GetHitTest(searchBounds))
                {
                    result.Add(caption);
                }
            }
            return result;
        }


        /// <summary>
        /// ���������� ������ ��� ������� ���������
        /// </summary>
        /// <param name="widths"></param>
        public void SetWidths(int[] widths)
        {
            for (int i = 0; (i < this.Count) && (i < widths.Length); i++)
            {
                this[i].Width = widths[i];
            }
        }

        /// <summary>
        /// �������� ������ ������� ���������
        /// </summary>
        /// <returns></returns>
        public int[] GetWidths()
        {
            int[] result = new int[this.Count];
            for (int i = 0; i < this.Count; i++)
            {
                result[i] = this[i].OriginalSize.Width;
            }
            return result;
        }

        /// <summary>
        /// ���������� ������ ��� ������� ���������
        /// </summary>
        /// <param name="widths"></param>
        public void SetHeights(int[] heights)
        {
            for (int i = 0; (i < this.Count) && (i < heights.Length); i++)
            {
                this[i].SetUncheckHeight(heights[i]);
            }
        }

        /// <summary>
        /// �������� ������ ������� ���������
        /// </summary>
        /// <returns></returns>
        public int[] GetHeights()
        {
            int[] result = new int[this.Count];
            for (int i = 0; i < this.Count; i++)
            {
                result[i] = this[i].OriginalSize.Height;
            }
            return result;
        }

        /// <summary>
        /// �������� ��������� ����������
        /// </summary>
        public new void Clear()
        {
            foreach (CaptionCell caption in this)
            {
                caption.Clear();
            }
            base.Clear();
            base.VisibleBounds = Rectangle.Empty;
        }

        /// <summary>
        /// ��������� ����� ��������� ���������
        /// </summary>
        /// <param name="captionsNode"></param>
        public override void Load(XmlNode collectionNode, bool isLoadTemplate)
        {
            if (collectionNode == null)
                return;
            this.Style.Load(collectionNode.SelectSingleNode(GridConsts.style), isLoadTemplate);
            this.LoadSizes(collectionNode.SelectSingleNode(GridConsts.sizes));
            this.Visible = XmlHelper.GetBoolAttrValue(collectionNode, GridConsts.visible, true);
        }

        /// <summary>
        /// ��������� ��������� ���������
        /// </summary>
        /// <param name="captionsNode"></param>
        public override void Save(XmlNode collectionNode)
        {
            if (collectionNode == null)
                return;
            this.Style.Save(XmlHelper.AddChildNode(collectionNode, GridConsts.style));
            this.SaveSizes(XmlHelper.AddChildNode(collectionNode, GridConsts.sizes));
            XmlHelper.SetAttribute(collectionNode, GridConsts.visible, this.Visible.ToString());
        }

        /// <summary>
        /// ���������� ����� ������ ���������� ��������� �����
        /// </summary>
        /// <param name="style"></param>
        public override void SetStyle(CellStyle style)
        {
            style.Grid = this.Grid;
            this.Style.CopyDefaultStyle(style);
            this.Style = style;
            foreach (CaptionCell caption in this)
            {
                caption.Style = this.Style;
            }
        }

        /// <summary>
        /// ��� ���������
        /// </summary>
        public CaptionType Type
        {
            get { return this._type; }
            set { this._type = value; }
        }

        /// <summary>
        /// ����� ������ ���������, ��������������� �� ��� �������� ���������
        /// </summary>
        public CellStyle Style
        {
            get { return this._style; }
            set { this._style = value; }
        }

        /// <summary>
        /// ���������
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        /// <summary>
        /// ������������� ���������
        /// </summary>
        /// <param name="clsInfo">�������� ������</param>
        public abstract void Initialize(CellSetInfo clsInfo);

        /// <summary>
        /// ������������� ���������
        /// </summary>
        /// <param name="pivotData">�������� ������</param>
        public abstract void Initialize(Data.PivotData pivotData);

        /// <summary>
        /// ��������� ������� ��������� ���������
        /// </summary>
        /// <param name="sizesNode"></param>
        protected abstract void LoadSizes(XmlNode sizesNode);
        
        /// <summary>
        /// ��������� ������� ��������� ���������
        /// </summary>
        /// <param name="sizesNode"></param>
        protected abstract void SaveSizes(XmlNode sizesNode);

        /// <summary>
        /// ������������� ������ � ������� � PivotData
        /// </summary>
        /// <param name="pivotData"></param>
        public abstract void SynchronizePivotData(Data.PivotData pivotData);
    }
}
