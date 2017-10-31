using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Krista.FM.Client.OLAPStructures;
using Krista.FM.Client.OLAPResources;

namespace Krista.FM.Client.OLAPAdmin
{
	public class Wizard
	{
		protected IWin32Window owner;
		protected List<WizardPage> pages = new List<WizardPage>();
		private bool prevPageFinishEnabled = false;
		public bool WizardCancelled = false;
		
		protected void AddPage(WizardPage newPage)
		{
			if (pages.Count > 0)
			{
				pages[pages.Count - 1].NextPage = newPage;
				newPage.PrevPage = pages[pages.Count - 1];
				
				newPage.PrevPage.FinishButton.Enabled = prevPageFinishEnabled;
				prevPageFinishEnabled = newPage.FinishButton.Enabled;
			}
			newPage.FinishButton.Enabled = true;
			pages.Add(newPage);
		}

		public Wizard(IWin32Window _owner)
		{
			owner = _owner;
		}

		public virtual bool Execute()
		{
			if (pages.Count > 0)
			{
				pages[0].Show(owner);
				Dispose();				
			}
			return !WizardCancelled;
		}

		public virtual void Dispose()
		{
			for (int i = 0; i < pages.Count; i++)
				if (pages[i] != null) { pages[i].Close(); }
		}
	}
	
	public class ButtonParams
	{		
		public WizardPageHandlerDelegate Handler = null;

		public ButtonParams(WizardPageHandlerDelegate handler)
		{	
			Handler = handler;
		}
	}

	public class Captions
	{
		private string window = "Выберите объекты";
		private string page = "Переместите ненужные вам объекты в левый список";

		public Captions()
		{			
		}

		public Captions(string _window, string _page)
		{
			window = _window;
			page = _page;			
		}

		public string Window
		{
			get { return window; }
			set { window = value; }
		}

		public string Page
		{
			get { return page; }
			set { page = value; }
		}
	}

	public class CaptionsTwoLists : Captions
	{	
		private string leftList = "Ненужные объекты";
		private string rightList = "Нужные объекты";

		public CaptionsTwoLists() : base()
		{ }

		public CaptionsTwoLists(string _window, string _page, string _leftList, string _rightList):
			base(_window, _page)
		{			
			leftList = _leftList;
			rightList = _rightList;
		}

		public string LeftList
		{
			get { return leftList; }
			set { leftList = value; }
		}

		public string RightList
		{
			get { return rightList; }
			set { rightList = value; }
		}
	}

	public class Buttons
	{
		public ButtonParams Prev = new ButtonParams(null);
		public ButtonParams Next = new ButtonParams(null);
		public ButtonParams Finish = new ButtonParams(null);
	}

	public class WizardPageParams
	{
		public Captions Captions = new Captions();
		public Buttons Buttons = new Buttons();
	}

	public class WizardPageParamsTwoLists : WizardPageParams
	{
		public new CaptionsTwoLists Captions = new CaptionsTwoLists();		
	}

	public delegate bool WizardPageHandlerDelegate(WizardPage page);

	public class WizardPage
	{
		protected frmWizardPage frmPage;
		protected WizardPage nextPage;
		protected WizardPage prevPage;

		protected WizardPageParams parameters;

		private void Init(
			Wizard _wizard, WizardPageParams _parameters, WizardPage _nextPage, WizardPage _prevPage)
		{
			frmPage = new frmWizardPage(_wizard);
			parameters = _parameters;

			frmPage.Text = parameters.Captions.Window;
			frmPage.textBoxHeader.Text = parameters.Captions.Page;
						
			frmPage.btnPrev.Enabled = prevPage != null;
			frmPage.btnPrev.Click += new EventHandler(PrevButtonHandler);

			frmPage.btnNext.Enabled = nextPage != null;
			frmPage.btnNext.Click += new EventHandler(NextButtonHandler);

			frmPage.btnFinish.Enabled = parameters.Buttons.Finish.Handler != null;
			frmPage.btnFinish.Click += new EventHandler(FinishButtonHandler);
		}		

		public WizardPage(Wizard _wizard, WizardPageParams _parameters)
		{			
            Init(_wizard, _parameters, null, null);
		}

		public WizardPage(
			Wizard _wizard, WizardPageParams _parameters, WizardPage _nextPage, WizardPage _prevPage)
		{
			Init(_wizard, _parameters, _nextPage, _prevPage);
		}

		public void Show(IWin32Window owner)
		{
			frmPage.StartPosition = FormStartPosition.CenterScreen;
			if (!frmPage.Modal) { frmPage.ShowDialog(owner); }
			else frmPage.Visible = true;
		}

		public void Hide()
		{
			frmPage.Hide();
		}

		public virtual void Close()
		{	
			frmPage.Close();
			frmPage.Dispose();
		}

		public WizardPage PrevPage
		{
			get { return prevPage; }
			set
			{ 
				prevPage = value;
				frmPage.btnPrev.Enabled = prevPage != null;				
			}
		}

		public WizardPage NextPage
		{
			get { return nextPage; }
			set
			{ 
				nextPage = value;				
				frmPage.btnNext.Enabled = nextPage != null;				
			}
		}		

		public Button PrevButton
		{
			get { return frmPage.btnPrev; }
		}

		public Button NextButton
		{
			get { return frmPage.btnNext; }
		}

		public Button FinishButton
		{
			get { return frmPage.btnFinish; }
		}		

		private void PrevButtonHandler(object sender, EventArgs e)
		{
			bool noErrors = true;
			if (parameters.Buttons.Prev.Handler != null)
				noErrors = parameters.Buttons.Prev.Handler(this);
			if (noErrors)
			{
				this.Hide();
				if (prevPage != null) { prevPage.Show(frmPage.Owner); }
			}
		}

		private void NextButtonHandler(object sender, EventArgs e)
		{
			bool noErrors = true;
			if (parameters.Buttons.Next.Handler != null)
				noErrors = parameters.Buttons.Next.Handler(this);
			if (noErrors)
			{				
				this.Hide();
				if (nextPage != null) { nextPage.Show(frmPage.Owner); }				
			}			
		}

		private void FinishButtonHandler(object sender, EventArgs e)
		{
			bool noErrors = true;
			if (parameters.Buttons.Finish.Handler != null)
				noErrors = parameters.Buttons.Finish.Handler(this);
			if (noErrors) { this.Hide(); }
		}
	}

	internal class WizardPageTwoLists : WizardPage
	{	
		private ctrlTwoLists twoLists;
		private List<object> leftList;
		private List<object> rightList;

		public WizardPageTwoLists(Wizard _wizard, List<object> items, WizardPageParamsTwoLists parameters,
			CustomBtnParams leftBtn, CustomBtnParams rightBtn)
			: base(_wizard, parameters)
		{			
			twoLists = new ctrlTwoLists();
			Utils.InsertControl(frmPage.pnlBody, twoLists);
			twoLists.Init(items, parameters.Captions.Window, parameters.Captions.Page,
				parameters.Captions.LeftList, parameters.Captions.RightList, leftBtn, rightBtn);
		}		

		public void RefreshItems(List<object> items)
		{
			twoLists.RefreshItems(items);			
		}		

		public List<object> LeftList
		{
			get
			{
				if (frmPage != null && frmPage.LeftList != null)
					return frmPage.LeftList.SortedItems;
				else
					return leftList;
			}
		}

		public List<object> RightList
		{
			get
			{
				if (frmPage != null && frmPage.RightList != null)
					return frmPage.RightList.SortedItems;
				else
					return rightList;
			}
		}		

		public override void Close()
		{
			FlashLists();
			base.Close();
		}

		private void FlashLists()
		{
			leftList = frmPage.LeftList.SortedItems;
			rightList = frmPage.RightList.SortedItems;
		}
	}

	internal class WizardPagePropertyGrid : WizardPage
	{
		TabControl tabControl = new TabControl();
		PropertyGrid ObjectGrid = new PropertyGrid();
		PropertyGrid ControlGrid = new PropertyGrid();
		DatabaseHeader package;

		public WizardPagePropertyGrid(Wizard _wizard, DatabaseHeader _package, WizardPageParams parameters)
			: base(_wizard, parameters)
		{
			package = _package;
			Utils.InsertControl(frmPage.pnlBody, tabControl);
			tabControl.TabPages.Add("Объект");
			tabControl.TabPages.Add("Управляющая информация");
			Utils.InsertControl(tabControl.TabPages[0], ObjectGrid);
			Utils.InsertControl(tabControl.TabPages[1], ControlGrid);			
			ObjectGrid.SelectedObject = package.DatabaseInfo;
			ControlGrid.SelectedObject = package.ControlBlock;			
		}
	}

	internal class WizardPageObjectExplorer : WizardPage
	{		
		private ctrlObjectExplorer objectExplorer;
		private List<object> names;
		//private DualAccessDictionary headers;

		public WizardPageObjectExplorer(Wizard _wizard, List<object> _headers, WizardPageParams parameters)
			: base(_wizard, parameters)
		{
			objectExplorer = new ctrlObjectExplorer();
			Utils.InsertControl(frmPage.pnlBody, objectExplorer);
			if (_headers != null)
			{	
				objectExplorer.Init(_headers, parameters.Captions.Window, null, null, null);
			}
		}

		public void RefreshItems(List<object> _headers)
		{	
			objectExplorer.RefreshItems(_headers);
		}		

		public override void Close()
		{
			FlashLists();
			base.Close();
		}		

		private void FlashLists()
		{
			names = objectExplorer.objectList.SortedItems;			
		}
	}
}
