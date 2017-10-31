using System.Windows.Forms;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.TasksUI
{
    public partial class FormSelectImportMode : Form
    {
        public FormSelectImportMode()
        {
            InitializeComponent();
        }

        public static TaskImportType SelectTasksImportType()
        {
            TaskImportType result = TaskImportType.tiUndefined;
            FormSelectImportMode tmpFrm = new FormSelectImportMode();
            if (tmpFrm.ShowDialog() == DialogResult.OK)
            {
                if (tmpFrm.uosImportMode.CheckedIndex == 0)
                    result = TaskImportType.tiAsRootTasks;
                else
                    result = TaskImportType.tiAsChildForSelected;
            }
            tmpFrm.Dispose();
            return result;
        }

    }
}