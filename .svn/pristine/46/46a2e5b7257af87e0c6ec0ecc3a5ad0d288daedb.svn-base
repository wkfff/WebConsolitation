using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Infragistics.Win.UltraWinTree;
using Krista.FM.Client.Reports;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.ViewObjects.ReportsUI.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Client.ViewObjects.ReportsUI.Gui
{
    internal class ReportsUI : BaseViewObj
    {

        internal ReportsUI(string key)
            : base(key)
        {
            Caption = "Отчеты";
        }

        public override Control Control
        {
            get { return fViewCtrl; }
        }

        protected override void SetViewCtrl()
        {
            if (fViewCtrl == null)
                fViewCtrl = new ReportsView();
        }

        #region внутренние переменные

        protected ReportsView vo;

        private IReportsTreeService reportTreeService;

        private ReportService reportService;

        #endregion

        public override void Initialize()
        {
            base.Initialize();

            reportService = new ReportService();

            vo = (ReportsView) fViewCtrl;
            vo.tReports.MouseDoubleClick += tReports_MouseDoubleClick;
            reportTreeService = Workplace.ActiveScheme.ReportsTreeService;
            CreateTreeStructure();
        }

        void tReports_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            UltraTree treeControl = sender as UltraTree;
            UltraTreeNode node = treeControl.GetNodeFromPoint(e.Location);

            if (node != null)
            {
                TemplateDocumentTypes docType = (TemplateDocumentTypes) node.Tag;
                if (docType != TemplateDocumentTypes.Group)
                {
                    // выполняем действия по созданию отчета
                    reportService.CreateReport(node.Key, Workplace);
                }
            }
        }

        #region построение структуры дерева

        private void CreateTreeStructure()
        {
            ITree<ITemplateReport> tree = reportTreeService.GetSystemReportsTree();
            foreach (var parentNode in tree.Nodes.Values.Where(w => w.Parent == null))
            {
                UltraTreeNode node = vo.tReports.Nodes.Add(parentNode.DataValue.Key, parentNode.DataValue.Caption);
                PrepareTreeNode(node, parentNode.DataValue.DocumentType);
                CreateTreeStructure(node, parentNode);
            }
        }

        private static void CreateTreeStructure(UltraTreeNode parentNode, ITreeNode<ITemplateReport> parentDataNode)
        {
            foreach (var childDataNode in parentDataNode.Nodes.Values)
            {
                UltraTreeNode childNode = parentNode.Nodes.Add(childDataNode.DataValue.Key, childDataNode.DataValue.Caption);
                PrepareTreeNode(childNode, childDataNode.DataValue.DocumentType);
                CreateTreeStructure(childNode, childDataNode);
            }
        }

        /// <summary>
        /// настраиваем узел в дереве в зависимости от элемента репозитория, который он представляет
        /// </summary>
        /// <param name="node"></param>
        /// <param name="docType"></param>
        private static void PrepareTreeNode(UltraTreeNode node, TemplateDocumentTypes docType)
        {
            switch (docType)
            {
                case TemplateDocumentTypes.Group:
                    node.LeftImages.Add(Properties.Resources.Folder_icon);
                    break;
                case TemplateDocumentTypes.Arbitrary:
                    node.LeftImages.Add(Properties.Resources.Document_Blank_icon);
                    break;
                case TemplateDocumentTypes.MDXExpert:
                case TemplateDocumentTypes.MDXExpert3:
                    node.LeftImages.Add(Properties.Resources.Mdx_Expert_icon);
                    break;
                case TemplateDocumentTypes.MSWord:
                case TemplateDocumentTypes.MSWordPlaning:
                case TemplateDocumentTypes.MSWordTemplate:
                    node.LeftImages.Add(Properties.Resources.Document_Microsoft_Word_icon);
                    break;
                case TemplateDocumentTypes.MSExcel:
                case TemplateDocumentTypes.MSExcelPlaning:
                case TemplateDocumentTypes.MSExcelTemplate:
                    node.LeftImages.Add(Properties.Resources.Document_Microsoft_Excel_icon);
                    break;
            }
            node.Tag = docType;
        }

        #endregion
    }
}
