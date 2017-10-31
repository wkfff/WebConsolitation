using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using Krista.FM.Client.Common;
using Krista.FM.Common;
using Krista.FM.Common.FileUtils;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.TemplatesUI
{
    public partial class FormTemplatesMaster : Form
    {
        private bool _addToTopLevel = false;

        internal static bool ShowTemplatesMaster(ref bool addToTopLevel)
        {
            FormTemplatesMaster templatesMaster = new FormTemplatesMaster();
            if (templatesMaster.ShowDialog() == DialogResult.OK)
            {
                addToTopLevel = templatesMaster._addToTopLevel;
                return true;
            }
            return false;
        }

        public FormTemplatesMaster()
        {
            InitializeComponent();
        }
        /*
        private void CreateNewDocuments()
        {
            string folder = TemplatesDocumentsHelper.GetDocsFolder();
            string docName = tbNewDocName.Text;
            if (string.IsNullOrEmpty(docName))
                docName = "Новый документ";
            docName = folder + Path.DirectorySeparatorChar + this._id.ToString() + "_" + docName;
            object obj = null;
            try
            {
                switch (newTemplateType)
                {
                    case TemplateTypes.Excel:
                        ExcelHelper exlhlp = new ExcelHelper(false);
                        docName = docName + ".xls";
                        obj = exlhlp.CreateEmptyDocument(docName);
                        OfficeHelper.SetObjectVisible(obj, true);
                        break;
                    case TemplateTypes.Word:
                        WordHelper wrdhlp = new WordHelper(false);
                        docName = docName + ".doc";
                        obj = wrdhlp.CreateEmptyDocument(docName);
                        OfficeHelper.SetObjectVisible(obj, true);
                        break;
                    case TemplateTypes.MDXExpert:
                        break;
                }
                _fileName = docName;
            }
            finally
            {
                if ((obj != null) && (Marshal.IsComObject(obj)))
                {
                    Marshal.ReleaseComObject(obj);
                    GC.GetTotalMemory(true);
                }
            }
        }
        */
        private void cbFirstLevelElem_CheckedChanged(object sender, EventArgs e)
        {
            _addToTopLevel = !rbChild.Checked;
        }
    }
}