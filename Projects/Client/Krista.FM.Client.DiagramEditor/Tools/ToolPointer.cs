#region User Difinishions
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
#endregion

namespace Krista.FM.Client.DiagramEditor
{
    /// <summary>
    /// Точки выделения
    /// </summary>
    public class ToolPointer : Tool
    {
        #region Fields

        // Keep state about last and current point (used to move and resize objects)
        private Point lastPoint = new Point(0, 0);
        private Point startPoint = new Point(0, 0);

        private int handleNumber;

        #endregion

        #region Constructor

        public ToolPointer()
        {
        }

        #endregion

        public override void OnMouseDown(DiargamEditor editor, System.Windows.Forms.MouseEventArgs e, Size scrollOffset, DiagramEntity selectedEntity)
        {
            Point point = new Point(e.X - scrollOffset.Width, e.Y - scrollOffset.Height);

            // Test for resizing (only if control is selected, cursor is on the handle)
            handleNumber = selectedEntity.HitTest(point);
        }

        public override void OnMouseMove(DiargamEditor editor, System.Windows.Forms.MouseEventArgs e, Size scrollOffset, DiagramEntity selectedEntity)
        {
            Point point = new Point(e.X - scrollOffset.Width, e.Y - scrollOffset.Height);

            if (e.Button == MouseButtons.None)
            {
                if (selectedEntity != null)
                {
                    handleNumber = selectedEntity.HitTest(point);
                    editor.Cursor = selectedEntity.GetHandleCursor(handleNumber);
                }
            }

            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            // resize
            if (selectedEntity != null)
            {
                selectedEntity.MoveHandleTo(point, handleNumber);

                editor.Cursor = selectedEntity.GetHandleCursor(handleNumber);
            }
        }

        public override void OnMouseUp(DiargamEditor editor, System.Windows.Forms.MouseEventArgs e, Size scrollOffset, DiagramEntity selectedEntity)
        {
            Point point = new Point(e.X - scrollOffset.Width, e.Y - scrollOffset.Height);

            editor.Cursor = Cursors.Default;
            if (selectedEntity is UMLAssociationBase)
            {
                ((UMLAssociationBase)selectedEntity).AfterMouseUp(point, handleNumber);
            }

            if (selectedEntity != null)
            {
                selectedEntity.Normalize();
            }
        }
    }
}
