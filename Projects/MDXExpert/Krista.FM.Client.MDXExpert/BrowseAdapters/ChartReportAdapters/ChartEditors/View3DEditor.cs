using System.Windows.Forms;
using Infragistics.UltraChart.Resources.Editor;

namespace Krista.FM.Client.MDXExpert
{
    public class View3DEditor : ChartUIEditorBase
    {
        // Methods
        public View3DEditor()
            : base(typeof(View3DCtrl))
        {
            Localize(base.EditorControl);
        }

        private void Localize(Control editorControl)
        {
            foreach (Control control in editorControl.Controls)
            {
                switch (control.Text)
                {
                    case "Reset":
                        control.Text = "�����";
                        break;
                    case "View Transform":
                        control.Text = "��������� 3D ����";
                        control.Left = 120;
                        break;
                    case "P":
                        control.Text = "�"; //�����������
                        break;
                    case "S":
                        control.Text = "�"; //�������
                        break;
                }
            }
        }
    }
}