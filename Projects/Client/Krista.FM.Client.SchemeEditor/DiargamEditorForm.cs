using System.Windows.Forms;


namespace Krista.FM.Client.SchemeEditor.DiargamEditor
{
    public partial class DiargamEditorForm : Form
    {
        public DiargamEditorForm()
        {
            InitializeComponent();

        	diargamEditor.ToolbarsManager = ultraToolbarsManager;
        }

        public DiagramEditor.DiargamEditor DiargamEditor
        {
            get { return diargamEditor; }
        }

        private void DiargamEditorForm_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }
    }
}