using System.Windows.Forms;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    /// <summary>
    /// Работа с клавиатурой
    /// </summary>
    public class CommandKeyPress : DiagramEditorCommand
    {
        public CommandKeyPress(AbstractDiagram diagram)
            : base(diagram)
        {
            Diagram.Site.KeyDown += new KeyEventHandler(Site_KeyDown);
            Diagram.Site.KeyUp += new KeyEventHandler(Site_KeyUp);
            Diagram.Site.KeyPress += new KeyPressEventHandler(Site_KeyPress);
            Diagram.Site.MouseWheel += new MouseEventHandler(Site_MouseWheel);
        }

        public override void Execute()
        {
        }

        private void Site_MouseWheel(object sender, MouseEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control)
            {
                // e.Delta == 120
                Diagram.Site.ScaleControl.ScaleChange(e.Delta / 12);
            }
        }

        private void Site_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '+')
            {
                Diagram.Site.ScaleControl.ScaleChange(10);
            }

            if (e.KeyChar == '-')
            {
                Diagram.Site.ScaleControl.ScaleChange(-10);
            }
        }

        private void Site_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 107 || e.KeyValue == 109)
            {
                Diagram.Site.ScaleControl.FinalizeOperation();
            }
        }

        private void Site_KeyDown(object sender, KeyEventArgs e)
        {
            // нажата 'a'
            bool a = false;

            bool z = false;

            bool r = false;

            bool f = false;

            bool s = false;

            // нажат ctrl
            bool ctrl = false;

            // нажат delete
            bool del = false;

            // нажат shift
            bool shift = false;

            bool alt = false;

            if (e.KeyCode == Keys.Delete)
            {
                if (Diagram.Site.SelectedEntities.Count == 0)
                {
                    return;
                }

                Diagram.Site.SelectedEntities[0].CmdDeleteSymbol.Execute();

                del = true;
            }

            if (e.Control)
            {
                ctrl = true;
            }

            if (e.Alt)
            {
                alt = true;
            }

            if (e.KeyCode == Keys.A)
            {
                a = true;
            }

            if (e.KeyCode == Keys.F)
            {
                f = true;
            }

            if (e.KeyCode == Keys.Shift)
            {
                shift = true;
            }

            if (ctrl && a)
            {
                Diagram.Site.CmdHelper.SelectAll();
            }

            if (shift && del)
            {
                if (Diagram.Site.SelectedEntities[0] != null)
                {
                    Diagram.Site.SelectedEntities[0].CmdDeletFromScheme.Execute();
                }
            }

            if (e.KeyValue == 107)
            {
                Diagram.Site.ScaleControl.StartUpOperation();
            }

            if (e.KeyValue == 109)
            {
                Diagram.Site.ScaleControl.StartDownOperation();
            }

            if (e.KeyCode == Keys.Z)
            {
                z = true;
            }

            if (e.KeyCode == Keys.R)
            {
                r = true;
            }

            if (e.KeyCode == Keys.S)
            {
                s = true;
            }

            if (ctrl && z)
            {
                Diagram.Site.UndoredoManager.Undo();
            }

            if (ctrl && r)
            {
                Diagram.Site.UndoredoManager.Redo();
            }

            if (ctrl && alt && f)
            {
                Diagram.Site.UndoredoManager.Flush();
            }

            if (ctrl && s)
            {
                Diagram.Site.CmdSave.Execute();
            }

            if (e.KeyCode == Keys.Escape)
            {
                Diagram.Site.CmdHelper.FinalizeCreateAssociation();
            }
        }
    }
}
