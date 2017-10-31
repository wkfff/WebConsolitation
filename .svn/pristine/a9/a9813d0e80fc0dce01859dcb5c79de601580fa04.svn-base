using System.Windows.Forms;

using Krista.FM.Client.OLAPResources;

namespace Krista.FM.Client.OLAPAdmin
{
	public partial class frmPropGrid : Form
	{
		private FormClosingEventHandler formClosingHandler;

		private void Init()
		{
			InitializeComponent();
			btnOK.Image = Utils.GetResourceImage32("Ok");
			btnCancel.Image = Utils.GetResourceImage32("Error");
		}

		private void Init(string title)
		{
			Init();
			this.Text = title;
		}

		public frmPropGrid()
		{
			Init();
		}

		public frmPropGrid(string title)
		{			
			Init(title);			
		}

		public frmPropGrid(FormClosingEventHandler _formClosingHandler, string title)
		{
			Init(title);
			formClosingHandler = _formClosingHandler;
		}

		private void frmGenerateNew_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (formClosingHandler != null)
			{
				formClosingHandler(sender, e);
			}			
		}
	}
}