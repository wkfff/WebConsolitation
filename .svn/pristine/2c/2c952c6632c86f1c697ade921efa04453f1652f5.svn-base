using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.DiagramEditor
{
    public class UndoRedo : IUndoRedo
    {
        private StackLimited stackUndo;

        private Stack stackRedo;

        public UndoRedo()
        {
            // и глубина, и макс. размер стека 32  
            stackUndo = new StackLimited(32);

            stackRedo = new Stack(32);
        }

        /// <summary>
        /// Новая команда
        /// </summary>
        public void Do(ICommand cmd)
        {
            stackUndo.Push(cmd);

            stackRedo.Clear();
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
                return;

            ICommand cmd = stackUndo.Pop() as ICommand;

            if (cmd != null)
            {
                stackRedo.Push(cmd);
                cmd.Undo();
            }
        }

        public void Redo()
        {
            if (!CanRedo())
                return;

            ICommand cmd = stackRedo.Pop() as ICommand;

            if (cmd != null)
            {
                stackUndo.Push(cmd);
                cmd.Redo();
            }
        }

        public void Flush()
        {
            stackUndo.Clear();
            stackRedo.Clear();
        }

        #endregion
    }
}
