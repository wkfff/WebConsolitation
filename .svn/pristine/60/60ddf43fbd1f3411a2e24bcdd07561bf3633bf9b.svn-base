using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.DiagramEditor
{
    /// <summary>
    /// Базовый класс для текстового описания на диаграмме
    /// </summary>
    public class UMLLabel : DiagramRectangleEntity
    {
        #region Fields

        /// <summary>
        /// Скрывать границы и заливку комментария
        /// </summary>
        private bool isFormatted = true;

        #region ToolStripMenuItems

        /// <summary>
        /// По-умолчанию граница и заливка у комментария отсутствуют 
        /// </summary>
        private ToolStripMenuItem defaultLineFillColor = new ToolStripMenuItem();

        #endregion

        #endregion

        #region Constructor

        public UMLLabel(Guid id, AbstractDiagram diagram, int x, int y)
            : this(id, diagram, x, y, Color.LightYellow)
        {
        }

        public UMLLabel(Guid id, AbstractDiagram diagram, int x, int y, Color color)
            : base(String.Empty, id, diagram, x, y, color)
        {
            EntityRectangle = new Rectangle(x, y, 75, 20);
            Text = "Label";

            AutoSizeRec();

            this.defaultLineFillColor.Click += new EventHandler(DefaultLineFillColor_Click);
        }

        #endregion

        #region Properties
        
        public bool IsFormatted
        {
            get { return isFormatted; }
            set { isFormatted = value; }
        }

        /// <summary>
        /// Количество точек выделения
        /// </summary>
        [Browsable(false)]
        public override int HandleCount
        {
            get
            {
                return 4;
            }
        }

        #endregion

        #region Methods

        public override void InitializeDefault()
        {
            base.InitializeDefault();

            this.isFormatted = true;
        }
       
        public override void AutoSizeRec()
        {
            Graphics g = Diagram.Site.CreateGraphics();
            try
            {
                Rectangle rectangle = EntityRectangle;
                rectangle.Width = (int)g.MeasureString(Text, this.Font).Width + 5;
                rectangle.Height = (int)g.MeasureString(Text, this.Font).Height + 5;

                EntityRectangle = rectangle;

                Diagram.Site.Invalidate();
            }
            finally
            {
                g.Dispose();
            }
        }

        /// <summary>
        /// Редактирование описания
        /// </summary>
        public override void CreateHeaderTextBox(Point point)
        {
            Size scrollOffset = ScaleTransform.TransformSize(new Size(Diagram.Site.AutoScrollPosition), Diagram.Site.ZoomFactor);

            // Редактируем только комментарии, не являющиеся стереотипом
            base.CreateHeaderTextBox(point);

            HeaderText.Text = Text;

            HeaderText.Location =
                ScaleTransform.SimpleTransformPoint(
                    new Point(
                        this.X + (IndentTextBox / 2) + scrollOffset.Width,
                        this.Y + (IndentTextBox / 2) + scrollOffset.Height),
                    Diagram.Site.ZoomFactor);

            HeaderText.Width = ScaleTransform.TransformInt(this.Width - IndentTextBox, Diagram.Site.ZoomFactor);
            HeaderText.Height = ScaleTransform.TransformInt(this.Height, Diagram.Site.ZoomFactor);

            HeaderText.BackColor = !isFormatted ? this.FillColor : Diagram.Site.BackColor;

            HeaderText.Enabled = true;
            HeaderText.Visible = true;

            if (HeaderText.CanFocus)
            {
                HeaderText.Focus();
            }
        }

        public override void ContextMenuInitialize()
        {
            base.ContextMenuInitialize();

            this.Options.Visible = false;
            this.ShadowVisible.Visible = false;
            this.ColorShadow.Visible = false;

            // DefaultParemeters
            this.defaultLineFillColor.Name = "DefaultLineFillColor";

            // this.DefaultLineFillColor.Size = new Size(194, 22);
            this.defaultLineFillColor.Text = "Не отображать границы и заливку.";
            if (this.isFormatted)
            {
                this.defaultLineFillColor.Checked = true;

                this.ColorEntity.Enabled = false;
                this.LColor.Enabled = false;
                this.LWidht.Enabled = false;
            }
            else
            {
                this.defaultLineFillColor.Checked = false;

                this.LColor.Enabled = true;
                this.LWidht.Enabled = true;
                this.ColorEntity.Enabled = true;
            }

            this.DeleteFromScheme.Visible = false;
            this.TsmiSelectInBrowser.Visible = false;

            this.TsmiFormat.DropDownItems.Add(defaultLineFillColor);
        }

        public override void RemoveEntity()
        {
            Diagram.Entities.Remove(this);
            List<DiagramEntity> collection = new List<DiagramEntity>();
            foreach (DiagramEntity entity in Diagram.Entities)
            {
                if (entity is UMLAnchorEntityToNote)
                {
                    if (((UMLAnchorEntityToNote)entity).ParentDiagramEntity.ID == this.ID || ((UMLAnchorEntityToNote)entity).ChildDiagramEntity.ID == this.ID)
                    {
                        collection.Add(entity);
                    }
                }
            }

            foreach (DiagramEntity entity in collection)
            {
                Diagram.Entities.Remove(entity);
            }

            base.RemoveEntity();
        }

        public override void RestoreEntity()
        {
            Diagram.Entities.Add(this);
            base.RestoreEntity();
        }

        /// <summary>
        /// Отрисовка комментария
        /// </summary>
        public override void Draw(System.Drawing.Graphics g, System.Drawing.Size scrollOffset)
        {
            SolidBrush fillBrush = new SolidBrush(FillColor);
            SolidBrush textBrush = new SolidBrush(TextColor);
            try
            {
                Pen.Color = LineColor;
                Pen.Width = LineWidth;

                Rectangle r = new Rectangle(EntityRectangle.X, EntityRectangle.Y, EntityRectangle.Width, EntityRectangle.Height);

                r.X -= IndentTextBox / 2;
                r.Y += IndentTextBox / 2;

                Rectangle rectForFormatting = new Rectangle(EntityRectangle.X, EntityRectangle.Y, EntityRectangle.Width, EntityRectangle.Height);
                rectForFormatting.Offset(scrollOffset.Width, scrollOffset.Height);
                if (!isFormatted)
                {
                    g.FillRectangle(fillBrush, rectForFormatting);
                    g.DrawRectangle(Pen, rectForFormatting);
                }

                r.Offset(scrollOffset.Width, scrollOffset.Height);
                g.DrawString(Text, Font, textBrush, rectForFormatting, Format);
            }
            finally
            {
                fillBrush.Dispose();
                textBrush.Dispose();
            }
        }

        /// <summary>
        /// 4 положения курсора
        /// </summary>
        public override System.Windows.Forms.Cursor GetHandleCursor(int handleNumber)
        {
            switch (handleNumber)
            {
                case 1:
                    return Cursors.SizeNWSE;
                case 2:
                    return Cursors.SizeNESW;
                case 3:
                    return Cursors.SizeNWSE;
                case 4:
                    return Cursors.SizeNESW;
                default:
                    return Cursors.Default;
            }
        }

        /// <summary>
        /// Перемещение описания
        /// </summary>
        public override void Move(Point p)
        {
            base.Move(p);
        }

        /// <summary>
        /// Изменение размеров
        /// </summary>
        /// <param name="point">Точка, за которую растягиваем</param>
        /// <param name="handleNumber">Точки растяжения пронумерованы</param>
        public override void MoveHandleTo(System.Drawing.Point point, int handleNumber)
        {
            point = ScaleTransform.TransformPoint(point, Diagram.Site.ZoomFactor);

            int left = EntityRectangle.Left;
            int top = EntityRectangle.Top;
            int right = EntityRectangle.Right;
            int bottom = EntityRectangle.Bottom;

            switch (handleNumber)
            {
                case 1:
                    left = point.X;
                    top = point.Y;
                    if (right - left < MinWidht)
                    {
                        left = right - MinWidht;
                    }

                    if (bottom - top < MinHeight)
                    {
                        top = bottom - MinHeight;
                    }

                    break;
                case 2:
                    right = point.X;
                    top = point.Y;
                    if (bottom - top < MinHeight)
                    {
                        top = bottom - MinHeight;
                    }

                    break;
                case 3:
                    right = point.X;
                    bottom = point.Y;
                    break;
                case 4:
                    left = point.X;
                    bottom = point.Y;
                    if (right - left < MinWidht)
                    {
                        left = right - MinWidht;
                    }

                    break;
            }

            SetRectangle(left, top, right - left, bottom - top);
        }
        
        /// <summary>
        /// Координаты точки выделения
        /// </summary>
        public override System.Drawing.Point GetHandle(int handleNumber)
        {
            int x, y;

            x = EntityRectangle.X;
            y = EntityRectangle.Y;

            switch (handleNumber)
            {
                case 1:
                    x = EntityRectangle.X;
                    y = EntityRectangle.Y;
                    break;
                case 2:
                    x = EntityRectangle.Right;
                    y = EntityRectangle.Y;
                    break;
                case 3:
                    x = EntityRectangle.Right;
                    y = EntityRectangle.Bottom;
                    break;
                case 4:
                    x = EntityRectangle.X;
                    y = EntityRectangle.Bottom;
                    break;
            }

            return new Point(x, y);
        }

        /// <summary>
        /// Определяет размер текста
        /// </summary>
        protected override void SetMinSize(Graphics g, bool auto)
        {
            // return TextRenderer.MeasureText(text, this.Font, new Size(rectangle.Width, rectangle.Height), flags);
        }

        #endregion Methods

        #region Events

        /// <summary>
        /// Форматирование комментария
        /// </summary>
        private void DefaultLineFillColor_Click(object sender, EventArgs e)
        {
            foreach (DiagramEntity entity in Diagram.Site.SelectedEntities)
            {
                if (entity is UMLLabel)
                {
                    if (((UMLLabel)entity).defaultLineFillColor.Checked)
                    {
                        ((UMLLabel)entity).defaultLineFillColor.Checked = false;
                        ((UMLLabel)entity).isFormatted = false;
                    }
                    else
                    {
                        ((UMLLabel)entity).defaultLineFillColor.Checked = true;
                        ((UMLLabel)entity).isFormatted = true;
                    }
                }
            }

            Diagram.Site.Invalidate();
        }

        #endregion
    }
}
