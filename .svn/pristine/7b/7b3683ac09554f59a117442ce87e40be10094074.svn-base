using System;
using System.Collections.Generic;
using System.Drawing;

namespace Krista.FM.Client.DiagramEditor
{
    /// <summary>
    /// Связь между классом и комментарием
    /// </summary>
    public class UMLAnchorEntityToNote : UMLAssociationBase
    {
        public UMLAnchorEntityToNote(Guid id, AbstractDiagram diagram, DiagramEntity start, DiagramEntity end, List<Point> selPoints)
            : base(String.Empty, id, diagram, start, end, selPoints)
        {
            Pen.Color = Color.Gray;
            Pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            Pen.DashPattern = new float[] { 4F, 4f };
        }

        public override void Draw(Graphics g, Size scrollOffset)
        {
            base.Draw(g, scrollOffset);
        }

        /// <summary>
        /// Контекстное меню
        /// </summary>
        public override void ContextMenuInitialize()
        {
            base.ContextMenuInitialize();

            this.DeleteFromScheme.Visible = false;
            this.TsmiSelectInBrowser.Visible = false;
            this.Options.Visible = false;
        }

        /// <summary>
        /// Удаление объекта
        /// </summary>
        public override void RemoveEntity()
        {
            Diagram.Entities.Remove(this);
            base.RemoveEntity();
        }

        /// <summary>
        /// Восстановление объекта
        /// </summary>
        public override void RestoreEntity()
        {
            Diagram.Entities.Add(this);
            base.RestoreEntity();
        }
    }
}
