using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Krista.FM.Client.Design;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    public class UndoRedo : IUndoRedo
    {
        private StackLimited stackUndo;

        private Stack stackRedo;

        private DiargamEditor site;

        public UndoRedo()
        {
            // и глубина, и макс. размер стека 32  
            stackUndo = new StackLimited(64);

            stackRedo = new Stack(64);
        }

        public DiargamEditor Site
        {
            get { return site; }
            set { site = value; }
        }

        /// <summary>
        /// Новая команда
        /// </summary>
        public void Do(BaseCommand cmd)
        {
            stackUndo.Push(cmd);

            stackRedo.Clear();

            site.CmdUndo.Enabled = true;
        }

        public bool CanUndo()
        {
            return stackUndo.Count > 0;
        }

        public bool CanRedo()
        {
            return stackRedo.Count > 0;
        }

        #region IUndoRedo Members

        public void Undo()
        {
            if (!CanUndo())
            {
                return;
            }

            BaseCommand cmd = stackUndo.Pop() as BaseCommand;

            if (cmd != null)
            {
                if (cmd is IUndoCommand)
                {
                    stackRedo.Push(cmd);
                    ((IUndoCommand)cmd).Undo();
                }
            }

            site.CmdRedo.Enabled = true;

            if (stackUndo.Count == 0)
            {
                site.CmdUndo.Enabled = false;
            }
        }

        public void Redo()
        {
            if (!CanRedo())
            {
                return;
            }

            BaseCommand cmd = stackRedo.Pop() as BaseCommand;

            if (cmd is Command)
            {
                stackUndo.Push(cmd);
                ((Command)cmd).Execute();
            }

            if (cmd is CommandWithPrm)
            {
                stackUndo.Push(cmd);
                ((CommandWithPrm)cmd).Execute(((CommandWithPrm)cmd).Parameter);
            }

            site.CmdUndo.Enabled = true;

            if (stackRedo.Count == 0)
            {
                site.CmdRedo.Enabled = false;
            }
        }

        public void Flush()
        {
            stackUndo.Clear();
            stackRedo.Clear();

            site.CmdRedo.Enabled = false;
            site.CmdUndo.Enabled = false;
        }

        #endregion
    }
}
