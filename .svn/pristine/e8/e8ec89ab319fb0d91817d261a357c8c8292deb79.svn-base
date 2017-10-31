using System.Windows.Forms;
using Krista.FM.Client.Common;

namespace Krista.FM.Common.TaskParamEditors
{
    public partial class TaskParamDimChooser : Form
    {
        public TaskParamDimChooser(bool mayApply)
        {
            InitializeComponent();
            buttonOk.Enabled = mayApply;
        }

        public string SelectDimension(IWorkplace workplace, string selectedDimensionName)
        {
            workplace.ProgressObj.Caption = "Загрузка измерений";
            workplace.ProgressObj.Text = "";
            workplace.ProgressObj.StartProgress();
            try
            {
                dimTree.Load(workplace.ActiveScheme, selectedDimensionName);
            }
            finally
            {
                workplace.ProgressObj.StopProgress();
            }
            
            return ShowDialog(workplace.WindowHandle) == DialogResult.Cancel ? string.Empty : dimTree.GetSelectedDimensionName();
        }

        public string LastError
        {
            get { return dimTree.LastError; }
        }
    }
}
