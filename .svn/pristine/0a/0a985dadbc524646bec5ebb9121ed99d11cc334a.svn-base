using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinTree;
using Krista.FM.Client.Common.Wizards;
using Krista.FM.ServerLibrary;
using Krista.FM.Common;

namespace Krista.FM.Client.ViewObjects.DataSourcesUI.DataSourceWizard
{
    public partial class DataSourcesTreePage : Krista.FM.Client.Common.Wizards.WizardPageBase
    {
        private IDataKind selecterDataKind;

        public DataSourcesTreePage()
        {
            InitializeComponent();
        }

        public IDataKind SelecterDataKind
        {
            get { return selecterDataKind; }
        }

        protected override void OnLoad(EventArgs e)
        {
            if ((WizardPageParent.WizardButtons & WizardForm.TWizardsButtons.Next) == WizardForm.TWizardsButtons.Next)
                WizardPageParent.WizardButtons -= WizardForm.TWizardsButtons.Next;
            
            FillDataSourcesTree();
        }

        /// <summary>
        /// Построение дерева поставщиков данных и видов поступающей информации.
        /// </summary>
        private void FillDataSourcesTree()
        {
            DataSourcesNavigation.Instance.Workplace.OperationObj.Text = "Загрузка...";
            DataSourcesNavigation.Instance.Workplace.OperationObj.StartOperation();

            try
            {
                dataSourcesTree.BeginUpdate();

                int nodeKey = 0;

                // Идем по коллекции поставщиков данных
                foreach (
                    IDataSupplier dataSupplier in DataSourcesNavigation.Instance.DataSourceManager.DataSuppliers.Values)
                {
                    string nodeText = dataSupplier.Name + " - " + dataSupplier.Description;
                    UltraTreeNode node = new UltraTreeNode(nodeText);

                    // Идем по коллекции вида поступающей информации
                    foreach (IDataKind dataKind in dataSupplier.DataKinds.Values)
                    {
                        if (dataKind.TakeMethod != TakeMethodTypes.Import)
                        {
                            string childNodeText = String.Format("{0} - {1} - {2} - {3}",
                                 dataKind.Code,
                                 dataKind.Name,
                                 EnumHelper.GetEnumItemDescription(typeof(TakeMethodTypes), dataKind.TakeMethod).Replace(" (НЕ ИСПОЛЬЗОВАТЬ)", String.Empty),
                                 EnumHelper.GetEnumItemDescription(typeof(ParamKindTypes), dataKind.ParamKind));

                            UltraTreeNode childNode = node.Nodes.Add(nodeKey.ToString(), childNodeText);
                            childNode.Tag = dataKind;
                            nodeKey++;
                        }
                    }

                    if (node.Nodes.Count > 0)
                        dataSourcesTree.Nodes.Add(node);
                }
                dataSourcesTree.Sort();
                dataSourcesTree.EndUpdate();
            }
            finally
            {
                DataSourcesNavigation.Instance.Workplace.OperationObj.StopOperation();
            }
        }

        private void DataSourcesTree_AfterSelect(object sender, SelectEventArgs e)
        {
            if (e.NewSelections[0] != null && e.NewSelections[0].Parent != null)
            {
                selecterDataKind = (IDataKind)e.NewSelections[0].Tag;

                if ((WizardPageParent.WizardButtons & WizardForm.TWizardsButtons.Next) != WizardForm.TWizardsButtons.Next)
                    WizardPageParent.WizardButtons |= WizardForm.TWizardsButtons.Next;
            }
            else
            {
                if ((WizardPageParent.WizardButtons & WizardForm.TWizardsButtons.Next) == WizardForm.TWizardsButtons.Next)
                    WizardPageParent.WizardButtons -= WizardForm.TWizardsButtons.Next;
            }
        }
    }
}
