using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Krista.FM.Common;
using Krista.FM.Common.ObjectTree;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Server.ReportsService
{
    public class ReportsTreeService : DisposableObject, IReportsTreeService
    {
        private IScheme scheme;

        public ReportsTreeService(IScheme scheme)
        {
            this.scheme = scheme;
        }

        public ITree<ITemplateReport> GetReportsTree(string topNodeCode)
        {
            ObjectTree<ITemplateReport> tree = new ObjectTree<ITemplateReport>();

            DataTable dtAllReports = scheme.TemplatesService.Repository.GetTemplatesInfo(
                TemplateTypes.System);

            DataRow[] rows = dtAllReports.Select(string.Format("Code = '{0}'", topNodeCode));
            // нет отчетов системы, выходим, возвращая пустой список
            if (rows == null || rows.Length == 0)
                return tree;
            var report = new TemplateReport(rows[0]);
            ITreeNode<ITemplateReport> node = tree.AddNode(report.Key, report);
            FillReports(node, dtAllReports);

            return tree;
        }

        public ITree<ITemplateReport> GetSystemReportsTree()
        {
            return GetReportsTree("SystemReports");
        }

        public ITree<ITemplateReport> GetReportsTree(TemplateTypes templateType)
        {
            ObjectTree<ITemplateReport> tree = new ObjectTree<ITemplateReport>();
            DataTable dtAllReports = scheme.TemplatesService.Repository.GetTemplatesInfo(templateType);
            // нет отчетов системы, выходим, возвращая пустой список
            if (dtAllReports.Rows.Count == 0)
                return tree;
            foreach (var row in dtAllReports.Rows.Cast<DataRow>().Where(w => w.IsNull("ParentID")))
            {
                var report = new TemplateReport(row);
                ITreeNode<ITemplateReport> node = tree.AddNode(report.Key, report);
                FillReports(node, dtAllReports);
            }
            return tree;
        }


        private void FillReports(ITreeNode<ITemplateReport> parent, DataTable dtTeplates)
        {

            foreach (DataRow childRow in dtTeplates.Rows.Cast<DataRow>().
                Where(w => (!w.IsNull("ParentID") && Convert.ToInt32(w["ParentID"]) == parent.DataValue.Id)))
            {
                var report = new TemplateReport(childRow);
                ITreeNode<ITemplateReport> node = parent.AddChild(report.Key, report);
                FillReports(node, dtTeplates);
            }
        }
    }
}
