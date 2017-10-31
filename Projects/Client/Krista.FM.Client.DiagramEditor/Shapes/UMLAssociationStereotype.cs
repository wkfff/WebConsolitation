using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.DiagramEditor
{
    public class UMLAssociationStereotype : UMLLabel
    {
        #region Fields

        /// <summary>
        /// Связь с ассоциацией
        /// </summary>
        private UMLAssociation association;

        /// <summary>
        /// Смещение, относительно центра ассоциации
        /// </summary>
        private Size offset;

        private bool isFirst = true;

        /// <summary>
        /// Расположение стереотипа по-умолчанию
        /// </summary>
        private ToolStripMenuItem defaulPosition = new ToolStripMenuItem();

        #endregion

        #region Constructor

        public UMLAssociationStereotype(Guid id, AbstractDiagram diagram, int x, int y, UMLAssociation association)
            : base(id, diagram, x, y)
        {
            this.association = association;
            offset = Size.Empty;
        }

        #endregion

        #region Properties

        /// <summary>
        /// расстояния до цинтра ассоциации
        /// </summary>
        public int OffsetWidht
        {
            get { return offset.Width; }
            set { offset.Width = value; }
        }

        public int OffsetHeight
        {
            get { return offset.Height; }
            set { offset.Height = value; }
        }

        public UMLAssociation Association
        {
            get { return association; }
        }

        #endregion

        #region Method

        public override void ContextMenuInitialize()
        {
            base.ContextMenuInitialize();

            this.defaulPosition.Text = "Позиция по-умолчанию";
            this.defaulPosition.Name = "DefPos";
            this.defaulPosition.Image = Diagram.Site.ImageList[Images.imgStereotypeDefPos];
            this.defaulPosition.ImageTransparentColor = Color.FromArgb(Krista.FM.Client.Design.Command.transparentColor);
            this.defaulPosition.Click += new EventHandler(Menu_Click);

            this.ContextMenu.Items.AddRange(new ToolStripItem[]
                                                {
                                                    defaulPosition
                                                });
        }

        public override void Draw(Graphics g, Size scrollOffset)
        {
            // при восстановлении объекта из xml-описания
            if (isFirst && offset.IsEmpty && !this.EntityRectangle.Location.IsEmpty)
            {
                offset.Width = EntityRectangle.X + (EntityRectangle.Width / 2) - association.Center.X;
                offset.Height = EntityRectangle.Y + (EntityRectangle.Height / 2) + this.Font.Height - association.Center.Y;

                isFirst = false;
            }

            // определение позиции в зависимости от смещения
            Rectangle rectangle = EntityRectangle;
            rectangle.X = association.Center.X + offset.Width - (EntityRectangle.Width / 2);
            rectangle.Y = association.Center.Y + offset.Height - (EntityRectangle.Height / 2) - this.Font.Height;
            EntityRectangle = rectangle;

            base.Draw(g, scrollOffset);
        }

        public override void Move(Point p)
        {
            if (Diagram.Site.SelectedEntities.Count == 1)
            {
                // Вызов базового метода для перемещения прямоугольника
                base.Move(p);

                // смещение
                offset.Width = EntityRectangle.X + (EntityRectangle.Width / 2) - association.Center.X;
                offset.Height = EntityRectangle.Y + (EntityRectangle.Height / 2) - association.Center.Y + this.Font.Height;

                // признак изменения диаграммы
                Diagram.IsChanged = true;
            }
        }

        /// <summary>
        /// Стереотипы не редактируем
        /// </summary>
        public override void CreateHeaderTextBox(Point point)
        {
        }
       
        public override void RemoveEntity()
        {
            if (Diagram.Entities.Contains(this))
            {
                Diagram.Entities.Remove(this);
                this.association.StereotypeEntity = null;
            }

            base.RemoveEntity();
        }

        public override void RestoreEntity()
        {
            if (Diagram.FindDiagramEntityByFullName(this.Association.Key) != null
                && ((UMLAssociation)Diagram.FindDiagramEntityByFullName(this.Association.Key)).StereotypeEntity == null)
            {
                this.Association.StereotypeEntity = this;
                Diagram.Entities.Add(this);
            }

            Diagram.Site.Invalidate();
        }

        /// <summary>
        /// Дополнительные обработчики для стереотипов
        /// </summary>
        private void Menu_Click(object sender, EventArgs e)
        {
            ToolStripItem item = sender as ToolStripItem;

            if (item != null)
            {
                switch (item.Name)
                {
                    case "DefPos":
                        offset = Size.Empty;
                        Diagram.Site.Invalidate();
                        break;

                    default:
                        throw new Exception("Обработчик не реализован!");
                }
            }
        }

        #endregion
    }
}
