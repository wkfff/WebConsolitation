using System.Windows.Forms;
using Krista.FM.Client.Common;

namespace Krista.FM.Common.TaskParamEditors
{
    public partial class TaskParamDimEditor : Form
    {
        public TaskParamDimEditor(bool mayApply)
        {
            InitializeComponent();
            buttonOK.Enabled = mayApply;
        }

        public string EditDimension(IWorkplace workplace, string dimName, string oldValue)
        {
            workplace.ProgressObj.Caption = "Загрузка элементов измерения";
            workplace.ProgressObj.Text = "";
            workplace.ProgressObj.StartProgress();
            try
            {
                buttonOK.Enabled = buttonOK.Enabled & membersTree1.Load(workplace.ActiveScheme, dimName, oldValue);
                 
            }
            finally
            {
                workplace.ProgressObj.StopProgress();
            }

            return ShowDialog(workplace.WindowHandle) == DialogResult.Cancel ? string.Empty : membersTree1.GetXmlValue();
        }

        public static string GetMembersText(string value)
        {
            return MembersTree.GetMembersText(value);
        }

        public string LastError
        {
            get { return membersTree1.LastError; }
        }
    }
}
