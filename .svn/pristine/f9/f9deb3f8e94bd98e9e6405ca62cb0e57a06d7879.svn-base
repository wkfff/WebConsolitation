using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Krista.FM.Client.Design;
using Krista.FM.Client.SchemeEditor.ControlObjects;
using Krista.FM.Client.SMO.Design;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SchemeEditor
{
    public partial class ModificationsTreeControl : UserControl
    {
        /// <summary>
        /// Определяет, что дерево изменений доступно только для просмотра
        /// </summary>
        private bool readOnly = false;

        /// <summary>
        /// Инициализация компонента.
        /// </summary>
        public ModificationsTreeControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Определяет, что дерево изменений доступно только для просмотра
        /// </summary>
        internal bool ReadOnly
        {
            get { return readOnly; }
            set 
            {
                this.modificationsTreeView.ReadOnly = value;
                this.ultraToolbarsManager.Enabled = !value;
                readOnly = value; 
            }
        }

        /// <summary>
        /// Обновление дерева изменений
        /// </summary>
        public override void Refresh()
        {
            try
            {
                SchemeEditor.Instance.Operation.Text = "Поиск отличий...";
                SchemeEditor.Instance.Operation.StartOperation();

                base.Refresh();

                IModificationItem mi = null;

                if (SchemeEditor.Instance.ModifiableObject != null)
                {
                    mi = SchemeEditor.Instance.ModifiableObject.GetChanges();
                }

                if (mi != null)
                {
                    ModificationItemControl mic = new ModificationItemControl(mi, null);
                    modificationsTreeView.SetRootNode(mic);
                    modificationsTreeView.Nodes[mic.Key].Text = mi.Name;
                }
                else
                    modificationsTreeView.ClearNodes();

            }
            finally
            {
                SchemeEditor.Instance.Operation.StopOperation();
            }
        }

        /// <summary>
        /// Применение всех изменений
        /// </summary>
        internal void Applay()
        {
            Applay(String.Empty);
        }

        /// <summary>
        /// Применение всех изменений
        /// </summary>
        internal void Applay(string comments)
        {
            if (modificationsTreeView.Nodes.Count > 0 && modificationsTreeView.Nodes[0] is ModificationItemControl)
            {
                SchemeEditor.Instance.StartModificationsApplayProcess(((ModificationItemControl)modificationsTreeView.Nodes[0]).TypedControlObject, comments);
            }
        }

        /// <summary>
        /// Применение для текущего узла.
        /// </summary>
        internal void ApplayForNode(string comments)
        {
            // Если выделен один узел, он является ModificationItemControl и согласны применять. 
            if (modificationsTreeView.SelectedNodes.Count == 1 && modificationsTreeView.SelectedNodes[0] is ModificationItemControl && 
                    MessageBox.Show("Применить изменения?", "Внимание", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                // Применяем  к выделенному узлу.
                SchemeEditor.Instance.StartModificationsApplayProcess(((ModificationItemControl) modificationsTreeView.SelectedNodes[0]).TypedControlObject, comments);
            }
        }

        /// <summary>
        /// Разворачивает дерево изменений
        /// </summary>
        internal void ExpandTree()
        {
            this.modificationsTreeView.ExpandAll(Infragistics.Win.UltraWinTree.ExpandAllType.Always);
        }

        /// <summary>
        /// Текущий элемент в дереве. 
        /// </summary>
        private CustomTreeNodeControl currentNode;

        /// <summary>
        /// Текущий уровень вложенности.
        /// </summary>
        private int currentIndentLevel;

        /// <summary>
        /// Очищает дерево изменений.
        /// </summary>
        internal void ClearTree()
        {
            currentNode = null;

            if (modificationsTreeView.InvokeRequired)
            {
                modificationsTreeView.Invoke((MethodInvoker)(() => modificationsTreeView.Nodes.Clear()));
            }
            else
            {
                modificationsTreeView.Nodes.Clear();
            }
        }

        internal void SetCurrentNode(IModificationItem modificationItem, int indentLevel)
        {
            try
            {
                CustomTreeNodeControl parent = null;
                SmoModificationItemDesign smoMI = new SmoModificationItemDesign(modificationItem);

                if (currentNode != null)
                {
                    if (currentIndentLevel < indentLevel)
                    {
                        parent = (CustomTreeNodeControl)currentNode;
                    }
                    else if (currentIndentLevel == indentLevel)
                    {
                        parent = (CustomTreeNodeControl)currentNode.Parent;
                    }
                    else if (currentIndentLevel > indentLevel)
                    {
                        if (currentNode.Parent != null)
                            parent = (CustomTreeNodeControl)currentNode.Parent.Parent;
                    }
                }

                ModificationItemProcessControl mi = new ModificationItemProcessControl(
                    modificationItem.Key, modificationItem.Name, smoMI, parent);

                if (currentNode == null)
                {
                    modificationsTreeView.SetRootNode(mi);
                    currentNode = mi;
                }
                else
                {
                    if (smoMI.State != ModificationStates.NotApplied)
                    {
                        if (((IModificationItem)currentNode.ControlObject).Key == smoMI.Key)
                        {
                            try
                            {
                                currentNode.Refresh();
                            }
                            catch { }
                        }
                        else if (((IModificationItem)((CustomTreeNodeControl)currentNode.Parent).ControlObject).Key == smoMI.Key)
                        {
                            try
                            {
                                currentNode = (CustomTreeNodeControl)currentNode.Parent;
                                currentIndentLevel = indentLevel;
                            }
                            catch (Exception e)
                            {
                                Services.MessageService.ShowErrorFormatted("Произошло исключение: ", e.Message);
                            }
                            try
                            {
                                currentNode.Refresh();
                            }
                            catch { }
                        }
                    }
                    else if (parent != null)
                    {
                        parent.Nodes.Add(mi);
                        currentNode = mi;
                        currentIndentLevel = indentLevel;
                        parent.Expanded = true;

                        //SchemeEditor.Instance.Form.Invoke(new Krista.FM.Client.Common.VoidDelegate(mi.BringIntoView));

                    }
                }
            }
            catch (Exception e)
            {
                Services.MessageService.ShowErrorFormatted("Произошло исключение: ", e.Message);
            }
        }

        private void ultraToolbarsManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "Refresh":
                    {
                        Refresh();
                        break;
                    }
                case "Applay":
                    {
                        Applay();
                        break;
                    }
                case "ApplayForNode":
                    {
                        ApplayForNode(string.Empty);
                        break;
                    }
            }
        }
    }
}
