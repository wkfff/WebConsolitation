using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Common;

namespace Krista.FM.Client.ViewObjects.TasksUI
{
    public partial class FormTasksRenaming : Form
    {
        /// <summary>
        /// форма
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="isAllTasks"></param>
        /// <returns></returns>
        public static bool ShowTasksRenamingParams(IWin32Window parent, UltraGrid grid, ref bool isAllTasks, ref bool childsTaskRename, ref string renameListFile)
        {
            FormTasksRenaming frm = new FormTasksRenaming();
            if (grid.Selected.Rows.Count == 0)
            {
                frm.rbAllTasks.Checked = true;
                frm.rbSelectedTasks.Enabled = false;
            }
            if (frm.ShowDialog(parent) == DialogResult.OK)
            {
                isAllTasks = frm.rbAllTasks.Checked;
                childsTaskRename = frm.cbChildsRename.Checked;
                renameListFile = frm.uteFileName.Value.ToString();
                return true;
            }
            return false;
        }

        private Infragistics.Win.ToolTip toolTipText;

        public FormTasksRenaming()
        {
            InitializeComponent();

            toolTipText = new Infragistics.Win.ToolTip(this);
			toolTipText.DisplayShadow = true;
			toolTipText.AutoPopDelay = 0;
			toolTipText.InitialDelay = 0;
        }

        private void ubtnOpenRenameList_Click(object sender, System.EventArgs e)
        {
            string fileName = string.Empty;
            if (ExportImportHelper.GetFileName("Список переименований.xml", ExportImportHelper.fileExtensions.xml, false, ref fileName))
            {
                uteFileName.Value = fileName;
                ultraButton2.Enabled = true;
            }
        }

        private void ubtnOpenRenameList_MouseMove(object sender, MouseEventArgs e)
        {
            UltraButton button = (UltraButton) sender;
            toolTipText.ToolTipText = "Загрузить список переименований";
            Point tooltipPos = new Point(0, button.Width);
            toolTipText.Show(button.PointToScreen(tooltipPos));
        }

        private void ubtnOpenRenameList_MouseLeave(object sender, System.EventArgs e)
        {
            toolTipText.Hide();
        }

        private void rbSelectedTasks_CheckedChanged(object sender, System.EventArgs e)
        {
            cbChildsRename.Enabled = rbSelectedTasks.Checked;
        }
    }
}