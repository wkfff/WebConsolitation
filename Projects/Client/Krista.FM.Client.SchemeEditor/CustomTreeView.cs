using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Infragistics.Win.IGControls;
using Infragistics.Win.UltraWinTree;

using Krista.FM.Client.SchemeEditor.ControlObjects;
using Krista.FM.Client.SMO.Design;

namespace Krista.FM.Client.SchemeEditor
{
    public class CustomTreeView : UltraTree
    {
		public CustomTreeView()
            : base()
        {
            HideSelection = false;
            PathSeparator = CustomPathSeparator;
            MouseDown += new MouseEventHandler(this.OnMouseDown);
            BeforeExpand += new BeforeNodeChangedEventHandler(OnBeforeExpand);
            AfterExpand += new AfterNodeChangedEventHandler(OnAfterExpand);
            AfterSelect += new AfterNodeSelectEventHandler(OnAfterSelect);
            DoubleClick += new EventHandler(OnDoubleClick);

			SelectionDragStart += new EventHandler(CustomTreeView_SelectionDragStart);
        }

        /// <summary>
        /// Start a DragDrop operation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CustomTreeView_SelectionDragStart(object sender, EventArgs e)
        {
            foreach (UltraTreeNode node in SelectedNodes)
            {
                //Если установлено свойство, запрещающее перетаскивание, пиши пропало всей Selection-группе
                if ( !((CustomTreeNodeControl)node).Draggable )
                {
                    return;
                }
            }
            this.DoDragDrop(SelectedNodes, DragDropEffects.Move);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.SuspendLayout();
                ClearNodes();
                this.ResumeLayout();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Разделитель узлов дерева
        /// </summary>
        internal static string CustomPathSeparator
        {
            get
            {
                return "::";
            }
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
			UltraTree treeControl = sender as UltraTree;
            if (e.Button == MouseButtons.Right)
            {
                CustomTreeNodeControl nodeAtPoint = (CustomTreeNodeControl)treeControl.GetNodeFromPoint(e.X, e.Y);
                if (nodeAtPoint != null)
                {
                    IGContextMenu contextMenu = nodeAtPoint.GetContextMenu();

                    treeControl.ContextMenu = contextMenu;
                }
            }
        }

        private void OnDoubleClick(object sender, EventArgs e)
        {
            UltraTree treeControl = sender as UltraTree;
            if (treeControl.SelectedNodes.Count > 0)
            {
                CustomTreeNodeControl nodeAtPoint = (CustomTreeNodeControl)treeControl.SelectedNodes[0];
                if (nodeAtPoint != null)
                    nodeAtPoint.OnDoubleClick(new NodeEventArgs(nodeAtPoint));
            }
        }

        /// <summary>
        /// Удаляет элементы дерева
        /// </summary>
        internal void ClearNodes()
        {
            foreach (CustomTreeNodeControl item in Nodes)
                item.Dispose();
            Nodes.Clear();
        }

        /// <summary>
        /// Обработка события перед разворачиванием узла. 
        /// Вызывает соответствующий обработчик разворачиваемого узла.
        /// </summary>
        private void OnBeforeExpand(object sender, CancelableNodeEventArgs e)
        {
            BeginUpdate();
            try
            {
                ((CustomTreeNodeControl)e.TreeNode).OnBeforeExpand(e);
            }
            finally
            {
                EndUpdate();
            }
        }

        /// <summary>
        /// Обработка события перед сворачивания узла. 
        /// Вызывает соответствующий обработчик сворачивоемого узла.
        /// </summary>
        private void OnAfterExpand(object sender, NodeEventArgs e)
        {
            BeginUpdate();
            try
            {
                ((CustomTreeNodeControl)e.TreeNode).OnAfterExpand(e);
            }
            finally
            {
                EndUpdate();
            }
        }

        private void OnAfterSelect(object sender, SelectEventArgs e)
        {
            BeginUpdate();
            try
            {
                if (e.NewSelections.Count == 1)
                    ((CustomTreeNodeControl)e.NewSelections[0]).OnAfterSelect(e);
            }
            finally
            {
                EndUpdate();
            }
        }

        /// <summary>
        /// Устанавливает корневой элемент дерева. Прежние элементы удаляются.
        /// </summary>
		/// <param name="nodeControl">Корневой элемент</param>
        internal void SetRootNode(CustomTreeNodeControl nodeControl)
        {
            // Очищаем дерево
            ClearNodes();

            // Устанавливаем корневой элемент
            Nodes.Add(nodeControl);
        }
    }
}
