using System.Windows.Forms;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.TasksUI
{
    public partial class FormSelectExportMode : Form
    {
        public FormSelectExportMode()
        {
            InitializeComponent();
        }

        public static TaskExportType SelectTasksExportType()
        {
            TaskExportType result = TaskExportType.teUndefined;
            FormSelectExportMode tmpFrm = new FormSelectExportMode();
            if (tmpFrm.ShowDialog() == DialogResult.OK)
            {
                if (tmpFrm.uosExportMode.CheckedIndex == 0)
                    result = TaskExportType.teSelectedOnly;
                else
                    result = TaskExportType.teIncludeChild;
            }
            tmpFrm.Dispose();
            return result;
        }
    }
}