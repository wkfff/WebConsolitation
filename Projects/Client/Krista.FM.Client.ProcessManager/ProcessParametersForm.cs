using System.Windows.Forms;
using Krista.FM.Server.ProcessorLibrary;

namespace Krista.FM.Client.ProcessManager
{
    public partial class ProcessParametersForm : Form
    {
        public ProcessParametersForm()
        {
            InitializeComponent();
            Common.DefaultFormState.Load(this);
            this.Closed += new System.EventHandler(ProcessParametersForm_Closed);
        }

        void ProcessParametersForm_Closed(object sender, System.EventArgs e)
        {
            Common.DefaultFormState.Save(this);
        }

        public RadioButton RbSeparate
        {
            get { return rbSeparate; }
        }
        public RadioButton RbTogether
        {
            get { return rbTogether; }
        }
        public RadioButton RbSelected
        {
            get { return rbSelected; }
        }
        public RadioButton RbNeedProcess
        {
            get { return rbNeedProcess; }
        }
    }

    public struct ProcessParameters
    {
        public static bool Show(out ProcessBatchMode batchMode, out ProcessObjectMode objectMode)
        {
            // значения по умолчанию для параметров расчета
            batchMode = ProcessBatchMode.Together;
            objectMode = ProcessObjectMode.Selected;

            ProcessParametersForm form = new ProcessParametersForm();

            if (DialogResult.OK == form.ShowDialog())
            {
                batchMode = form.RbSeparate.Checked ? ProcessBatchMode.Separate : ProcessBatchMode.Together;
                objectMode = form.RbSelected.Checked ? ProcessObjectMode.Selected : ProcessObjectMode.AllNeedProcess;

                return true;
            }

            return false;
        }
    }
}