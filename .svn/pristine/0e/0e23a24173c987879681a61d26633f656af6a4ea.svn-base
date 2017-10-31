using System;
using System.Windows.Forms;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.Common.Forms;
using Infragistics.Win.UltraWinTree;
using Krista.FM.Common;
using System.Collections;

namespace Krista.FM.Client.Common.Controls
{
    public partial class DataSourcesTreeControl : UserControl
    {
        /// <summary>
        /// Менеджер управления источниками данных
        /// </summary>
        IDataSourceManager dataSourcesManager;
   
        public DataSourcesTreeControl()
        {
            InitializeComponent();
            this.dataSourcesTree.AfterCheck += dataSourcesTree_AfterCheck;
        }

        static void dataSourcesTree_AfterCheck(object sender, NodeEventArgs e)
        {
            CheckState check = e.TreeNode.CheckedState;
            if (e.TreeNode.HasNodes)
            {
                foreach (UltraTreeNode node in e.TreeNode.Nodes)
                {
                    node.CheckedState = check;
                }
            }
        }

        /// <summary>
        /// Инициализация дерева
        /// </summary>
        private void Initialize()
        {
            int nodeKey = 0;
            try
            {
                dataSourcesTree.BeginUpdate();

                // Идем по коллекции поставщиков данных
                foreach (
                    IDataSupplier dataSupplier in dataSourcesManager.DataSuppliers.Values)
                {
                    string nodeText = dataSupplier.Name + " - " + dataSupplier.Description;
                    UltraTreeNode node = new UltraTreeNode(nodeText);
                    node.Override.NodeStyle = NodeStyle.CheckBox;

                    // Идем по коллекции вида поступающей информации
                    foreach (IDataKind dataKind in dataSupplier.DataKinds.Values)
                    {
                        if (dataKind.TakeMethod != TakeMethodTypes.Import)
                        {
                            string childNodeText = String.Format("{0} - {1} - {2} - {3}",
                                 dataKind.Code,
                                 dataKind.Name,
                                 EnumHelper.GetEnumItemDescription(typeof(TakeMethodTypes), dataKind.TakeMethod),
                                 EnumHelper.GetEnumItemDescription(typeof(ParamKindTypes), dataKind.ParamKind)).Replace(" (НЕ ИСПОЛЬЗОВАТЬ)", String.Empty);

                            UltraTreeNode childNode = new UltraTreeNode();
                            childNode.Key = nodeKey.ToString();
                            childNode.Text = childNodeText;
                            childNode.Tag = dataKind;
                            childNode.Override.NodeStyle = NodeStyle.CheckBox;
                            node.Nodes.Add(childNode);
                            nodeKey++;
                        }
                    }

                    if (node.Nodes.Count > 0)
                        dataSourcesTree.Nodes.Add(node);
                }

                dataSourcesTree.Sort();
                dataSourcesTree.EndUpdate();
            }
            catch(ArgumentException e)
            {
                throw new Exception(e.Message);
            }
        }

        public IDataSourceManager DataSourcesManager
        {
            set
            {
                this.dataSourcesManager = value;
                Initialize();
            }
        }

        public string CheckedNodes()
        {
            try
            {
                string dataKindsCollection = String.Empty;
                int i = 0;
                foreach (UltraTreeNode node in dataSourcesTree.Nodes)
                {
                    foreach (UltraTreeNode childNode in node.Nodes)
                    {
                        if (childNode.CheckedState == CheckState.Checked)
                        {
                            IDataKind dataKind = (IDataKind)childNode.Tag;

                            if (i == 0)
                                dataKindsCollection = String.Format("{0}\\{1:D4}",
                                    dataKind.Supplier.Name, dataKind.Code);
                            else
                                dataKindsCollection += String.Format(";{0}\\{1:D4}",
                                    dataKind.Supplier.Name, dataKind.Code);

                            i++;
                        }
                    }
                }
                return dataKindsCollection;
            }
            catch (NullReferenceException ex)
            {
                throw new NullReferenceException(ex.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataKindCollection"></param>
        public void InitializeCheckedNodes(string dataKindCollection)
        {
            Operation operation = new Operation();
            try
            {
                operation.Text = "Инициализация видов поступающей информации";
                operation.StartOperation();

                Unchecked();

                if (!String.IsNullOrEmpty(dataKindCollection))
                {
                    string[] kinds = dataKindCollection.Split(';');

                    for (int i = 0; i < kinds.Length; i++)
                    {
                        FindNode(kinds[i]);
                    }
                }
            }
            finally
            {
                operation.StopOperation();
                operation.ReleaseThread();
            }
        }

        private void FindNode(string kind)
        {
            foreach (UltraTreeNode node in dataSourcesTree.Nodes)
            {
                if (node.Text.Split(' ')[0] == kind.Split('\\')[0])
                {
                    foreach (UltraTreeNode childNode in node.Nodes)
                    {
                        if (childNode.Text.Split(' ')[0] == kind.Split('\\')[1])
                        {
                            childNode.CheckedState = CheckState.Checked;
                            node.Expanded = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Unchecked()
        {
            foreach (UltraTreeNode node in dataSourcesTree.Nodes)
            {
                node.CheckedState = CheckState.Unchecked;
                foreach (UltraTreeNode childNode in node.Nodes)
                    childNode.CheckedState = CheckState.Unchecked;

                node.Expanded = false;
            }
        }
    }
}
