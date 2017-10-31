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
        /// ����� � �����������
        /// </summary>
        private UMLAssociation association;

        /// <summary>
        /// ��������, ������������ ������ ����������
        /// </summary>
        private Size offset;

        private bool isFirst = true;

        /// <summary>
        /// ������������ ���������� ��-���������
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
        /// ���������� �� ������ ����������
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

            this.defaulPosition.Text = "������� ��-���������";
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
            // ��� �������������� ������� �� xml-��������
            if (isFirst && offset.IsEmpty && !this.EntityRectangle.Location.IsEmpty)
            {
                offset.Width = EntityRectangle.X + (EntityRectangle.Width / 2) - association.Center.X;
                offset.Height = EntityRectangle.Y + (EntityRectangle.Height / 2) + this.Font.Height - association.Center.Y;

                isFirst = false;
            }

            // ����������� ������� � ����������� �� ��������
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
                // ����� �������� ������ ��� ����������� ��������������
                base.Move(p);

                // ��������
                offset.Width = EntityRectangle.X + (EntityRectangle.Width / 2) - association.Center.X;
                offset.Height = EntityRectangle.Y + (EntityRectangle.Height / 2) - association.Center.Y + this.Font.Height;

                // ������� ��������� ���������
                Diagram.IsChanged = true;
            }
        }

        /// <summary>
        /// ���������� �� �����������
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
        /// �������������� ����������� ��� �����������
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
                        throw new Exception("���������� �� ����������!");
                }
            }
        }

        #endregion
    }
}
