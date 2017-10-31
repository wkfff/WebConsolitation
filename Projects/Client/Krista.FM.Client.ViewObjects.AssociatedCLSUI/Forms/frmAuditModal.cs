using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Krista.FM.Client.Common;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI
{
    public partial class frmAuditModal : BaseCls.frmModalTemplate
    {
        public static bool ShowAudit(IWorkplace workplace, string objectName, string objectRusCaption, int rowID, AuditShowObjects auditObject)
        {
            string filter = string.Empty;

            IDbDataParameter[] filterParams = null;

            string frmCaption = string.Empty;

            if (auditObject == AuditShowObjects.RowObject)
            {
                filter = "((OBJECTNAME = ?) and (RECORDID = ?))";
                filterParams = new IDbDataParameter[2];
                filterParams[0] = new DbParameterDescriptor("p0", objectName);
                filterParams[1] = new DbParameterDescriptor("p1", rowID);

                frmCaption = String.Format("История изменения записи с ID = {0} объекта '{1}'", rowID, objectRusCaption);
            }
            else if (auditObject == AuditShowObjects.ClsObject)
            {
                filter = "(OBJECTNAME = ?)";
                filterParams = new IDbDataParameter[1];
                filterParams[0] = new DbParameterDescriptor("p0", objectName);
                frmCaption = String.Format("История изменения записей объекта '{0}'", objectRusCaption);
            }
            frmAuditModal frmAudit = new frmAuditModal();
            workplace.ProtocolsInplacer.AttachAudit(frmAudit.spcContainer.Panel1, frmCaption, auditObject, filter, filterParams);
            frmAudit.Text = frmCaption;
            Application.DoEvents();
            IWin32Window tmp = (IWin32Window)(Control)workplace;
            //.. .ShowDialog(tmp)
            if (frmAudit.ShowDialog(tmp) == DialogResult.OK)
                return true;
            return false;
        }

        public static bool ShowAudit(IWorkplace workplace, string objectName, string objectRusCaption, int rowID, int pumpId, ClassTypes classType)
        {
            string filter = "((OBJECTNAME = ?) and (RECORDID = ?))";
            IDbDataParameter[] filterParams = new IDbDataParameter[2];
            filterParams[0] = new DbParameterDescriptor("p0", objectName);
            filterParams[1] = new DbParameterDescriptor("p1", rowID);

            string frmCaption = String.Format("История изменения записи с ID = {0} объекта '{1}'", rowID, objectRusCaption);
            frmAuditModal frmAudit = new frmAuditModal();
            workplace.ProtocolsInplacer.AttachAudit(frmAudit.spcContainer.Panel1, frmCaption, classType, objectName, pumpId, filter, filterParams);
            frmAudit.Text = frmCaption;
            Application.DoEvents();

            if (frmAudit.ShowDialog((IWin32Window)(Control)workplace) == DialogResult.OK)
            {
                return true;
            }
            return false;
        }

        public frmAuditModal()
        {
            InitializeComponent();
        }
    }
}