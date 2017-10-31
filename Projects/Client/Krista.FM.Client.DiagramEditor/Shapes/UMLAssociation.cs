using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.DiagramEditor
{
    /// <summary>
    /// Связь между классами
    /// </summary>
    public class UMLAssociation : UMLAssociationBase
    {
        #region Поля

        /// <summary>
        /// Стереотип объекта
        /// </summary>
        private UMLAssociationStereotype associateStereotype;

        /// <summary>
        /// Карандаш c наконечником
        /// </summary>
        private Pen penWithCap = new Pen(Color.Black);

        #region ToolStripMenuItems

        #endregion

        #endregion

        #region Constructor

        public UMLAssociation(string key, Guid id, AbstractDiagram diagram, DiagramEntity start, DiagramEntity fin, List<Point> selPoints)
            : base(key, id, diagram, start, fin, selPoints)
        {
            penWithCap.CustomEndCap = new AdjustableArrowCap(8, 11, false);
        }

        #endregion

        #region Свойства

        /// <summary>
        /// Cтереотип объекта
        /// </summary>
        public UMLAssociationStereotype StereotypeEntity
        {
            get { return associateStereotype; }
            set { associateStereotype = value; }
        }

        /// <summary>
        /// Изменение цвета ассоциации
        /// </summary>
        public Pen PenWithCap
        {
            get { return penWithCap; }
            set { penWithCap = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Базовый метод инициализации контекстного меню
        /// </summary>
        public override void ContextMenuInitialize()
        {
            base.ContextMenuInitialize();
        }

        /// <summary>
        /// Получаем стереотип к ассоциации
        /// </summary>
        public UMLAssociationStereotype GetUMLAssociationStereotype()
        {
            // отсутствует одна из ролей
            if (ParentDiagramEntity == null || ChildDiagramEntity == null)
            {
                return null;
            }

            IEntityAssociation association =
                Diagram.Site.SchemeEditor.Scheme.GetObjectByKey(this.Key) as IEntityAssociation;

            if (association != null)
            {
                UMLAssociationStereotype stereotype =
                        new UMLAssociationStereotype(
                        Guid.NewGuid(), Diagram, 0, 0, this);

                stereotype.Text = String.Format("<<{0}>>", GetStereotypeNameByAssociationClassType(association.AssociationClassType));
                stereotype.AutoSizeRec();

                stereotype.Visible = true;
                return stereotype;
            }

            return null;
        }

        /// <summary>
        /// Удаление ассоциации
        /// </summary>
        public override void RemoveEntity()
        {
            if (this.StereotypeEntity != null)
            {
                Diagram.Entities.Remove(this.StereotypeEntity);
                this.StereotypeEntity = null;
            }

            Diagram.Entities.Remove(this);

            base.RemoveEntity();
        }

        /// <summary>
        /// Восстановление объекта
        /// </summary>
        public override void RestoreEntity()
        {
            if (Diagram.FindDiagramEntityByFullName(this.Key) != null)
            {
                return;
            }

            Diagram.Entities.Add(this);

            base.RestoreEntity();
        }

        /// <summary>
        /// Отрисовка ассоциации
        /// </summary>
        public override void Draw(System.Drawing.Graphics g, Size scrollOffset)
        {
            if (SelectedPoints.Count < 2)
            {
                return;
            }

            base.Draw(g, scrollOffset);

            penWithCap.Width = this.LineWidth;
            penWithCap.Color = Pen.Color;

            Point preLastPoint = this.GetPrePoint(SelectedPoints[SelectedPoints.Count - 2], SelectedPoints.Count);

            if (!preLastPoint.IsEmpty)
            {
                preLastPoint.Offset(scrollOffset.Width, scrollOffset.Height);
                Point p = SelectedPoints[SelectedPoints.Count - 1];
                p.Offset(scrollOffset.Width, scrollOffset.Height);
                g.DrawLine(penWithCap, preLastPoint, p);
            }
        }
               
        #endregion

        #region Events

        /// <summary>
        /// Добавляем/убираем стереотип
        /// </summary>
        /// <param name="offon">Отображать или скрывать</param>
        public override void DisplayStereotype(bool offon)
        {
            if (offon)
            {
                if (this.StereotypeEntity == null)
                {
                    // Получаем стереотип
                    this.StereotypeEntity = this.GetUMLAssociationStereotype();

                    if (this.StereotypeEntity == null)
                    {
                        return;
                    }

                    Diagram.Entities.Add(this.StereotypeEntity);
                    this.StereotypeVisible = true;
                }
            }
            else
            {
                if (this.StereotypeEntity != null)
                {
                    this.StereotypeEntity.RemoveEntity();
                    this.StereotypeVisible = false;
                }
            }
        }
        #endregion
    }
}
