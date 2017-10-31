using System;
using System.Data;
using System.Windows.Forms;

namespace Krista.FM.Client.Common.Forms
{
    public partial class FormAudit : Form
    {
        public static bool ShowAudit(IWorkplace workplace, string objectName, string objectRusCaption, int rowID, AuditShowObjects auditObject)
        {
            string filter = string.Empty;

            IDbDataParameter[] filterParams = null;

            string frmCaption = string.Empty;

            if (auditObject == AuditShowObjects.RowObject)
            {
                filter = "((UPPER(OBJECTNAME) = UPPER(?)) and (RECORDID = ?))";
                filterParams = new IDbDataParameter[2];
                filterParams[0] = CreateDataParams("OBJECTNAME", objectName);
                filterParams[1] = CreateDataParams("RECORDID", rowID);

                frmCaption = String.Format("История изменения записи с ID = {0} объекта '{1}'", rowID, objectRusCaption);
            }
            else if (auditObject == AuditShowObjects.ClsObject)
            {
                filter = "(OBJECTNAME = ?)";
                filterParams = new IDbDataParameter[1];
                filterParams[0] = CreateDataParams("OBJECTNAME", objectName);
                frmCaption = String.Format("История изменения записей объекта '{0}'", objectRusCaption);
            }
            else if (auditObject == AuditShowObjects.TaskObject)
            {
                filter = "(TASKID = ?)";
                filterParams = new IDbDataParameter[1];
                filterParams[0] = CreateDataParams("TASKID", rowID);
                frmCaption = String.Format("История изменения данных по задаче с ID = '{0}'", rowID);
            }
            FormAudit frmAudit = new FormAudit();
            workplace.ProtocolsInplacer.AttachAudit(frmAudit.spcContainer.Panel1, frmCaption, auditObject, filter, filterParams);
            frmAudit.Text = frmCaption;

            if (frmAudit.ShowDialog() == DialogResult.OK)
                return true;

            return false;
        }

        private static IDbDataParameter CreateDataParams(string paramName, object paramValue)
        {
            return new System.Data.OleDb.OleDbParameter(paramName, paramValue);
        }

        public FormAudit()
        {
            InitializeComponent();
        }
    }
}