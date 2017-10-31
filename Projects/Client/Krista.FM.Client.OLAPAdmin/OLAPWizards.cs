using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Krista.FM.Client.OLAPStructures;
using Krista.FM.Client.OLAPResources;

namespace Krista.FM.Client.OLAPAdmin
{	
	internal class WizardSelectOLAPObjects : Wizard
	{
		protected WizardPageTwoLists cubesPage;
		protected WizardPageParamsTwoLists cubesPageParams;

		protected WizardPageTwoLists dimsPage;
		protected WizardPageParamsTwoLists dimsPageParams;

		protected WizardPagePropertyGrid databasePage;
		protected WizardPageParams databasePageParams;

		protected DatabaseHeader package;
		protected bool omitCubesPage = false;
		
		protected virtual void CreatePages()
		{
			InitPropertiesPageParams();
			databasePage = new WizardPagePropertyGrid(this, package, databasePageParams);

			if (!omitCubesPage)
			{
				InitCubesPageParams();				
				cubesPage = new WizardPageTwoLists(
					this, package.ControlBlock.Cubes.GetItems(), cubesPageParams, null, null);
			}

			InitDimsPageParams();
			CustomBtnParams rightBtn =
				new CustomBtnParams("Только используемые в кубах", CustomFilterHandler);
			dimsPage = new WizardPageTwoLists(this, null, dimsPageParams, rightBtn, rightBtn);
		}

		protected virtual void AddPages()
		{
			AddPage(databasePage);
			if (!omitCubesPage) { AddPage(cubesPage); }
			AddPage(dimsPage);
		}

		protected void Init(IWin32Window _owner, DatabaseHeader _package, bool _omitCubesPage)
		{
			owner = _owner;
			package = _package;
			omitCubesPage = _omitCubesPage;

			CreatePages();
			AddPages();			
		}

		public WizardSelectOLAPObjects(IWin32Window _owner, DatabaseHeader _package)
			: base(_owner)
		{
			Init(_owner, _package, false);			
		}

		public WizardSelectOLAPObjects(IWin32Window _owner,
			DatabaseHeader _package, List<string> dimNames, bool omitCubesPage)
			: base(_owner)
		{
			Init(_owner, _package, true);
		}

		protected void CustomFilterHandler(object sender, EventArgs e)
		{
			ToolStripButton btn = sender as ToolStripButton;
			ctrlFiltratedList filtratedList = btn.GetCurrentParent().Parent as ctrlFiltratedList;			
			if (btn.Checked)
			{				
				filtratedList.SetChosenItems(
					package.ControlBlock.GetUsedDimensions(cubesPage.RightList));
			}
			else
			{
				filtratedList.SetChosenItems(null);
			}
		}

		protected virtual void InitPropertiesPageParams()
		{
			databasePageParams = new WizardPageParams();
			databasePageParams.Captions.Window = "Параметры пакета";
			databasePageParams.Captions.Page = "Отредактируйте параметры пакета";			
			databasePageParams.Buttons.Next.Handler = PropertiesPageNextButtonHandler;
		}

		protected virtual void InitCaptions(CaptionsTwoLists captions, string objectName)
		{
			captions.Window = string.Format("Выберите {0} для дальнейшей обработки", objectName);
			captions.Page = string.Format(
				"Переместите ненужные вам {0} в левый список, нужные в правый", objectName);
			captions.LeftList = string.Format("Ненужные {0}", objectName);
			captions.RightList = string.Format("Нужные {0}", objectName);
		}

		protected virtual void InitCubesPageParams()
		{
			cubesPageParams = new WizardPageParamsTwoLists();
			InitCaptions(cubesPageParams.Captions, "кубы");			
			cubesPageParams.Buttons.Next.Handler = CubesPageNextButtonHandlerWithCheck;
		}

		protected virtual void InitDimsPageParams()
		{
			dimsPageParams = new WizardPageParamsTwoLists();
			InitCaptions(dimsPageParams.Captions, "измерения");
			dimsPageParams.Buttons.Finish.Handler = DimsPageFinishButtonHandler;
		}

		protected bool PropertiesPageNextButtonHandler(WizardPage page)
		{
			if (string.IsNullOrEmpty(package.ControlBlock.FileName))
			{
				Messanger.Error("Не задано имя файла!");
				return false;
			}
			else { return true; }
		}

		protected bool CheckCubesPage()
		{
			if (cubesPage.RightList.Count == 0)
			{
				Messanger.Error("Правый список не содержит элементов!");
				return false;
			}
			return true;
		}

		protected bool CubesPageNextButtonHandlerWithCheck(WizardPage page)
		{
			//if (CheckCubesPage()) { return CubesPageNextButtonHandler(page); }
			//return false;
			return CubesPageNextButtonHandler(page);			
		}

		protected bool CubesPageNextButtonHandler(WizardPage page)
		{	
			dimsPage.RefreshItems(package.ControlBlock.Dimensions.GetItems());
			return true;
		}

		protected bool DimsPageFinishButtonHandler(WizardPage page)
		{
			//if (dimsPage.RightList.Count == 0)
			//{
			//    Messanger.Error("Правый список не содержит элементов!");
			//    return false;
			//}
			//else { return true; }
			return true;
		}

		public DatabaseHeader DatabaseHeader
		{
			get { return package; }
		}

		public WizardPageTwoLists CubesPage
		{
			get { return cubesPage; }
		}

		public WizardPageTwoLists DimensionsPage
		{
			get { return dimsPage; }
		}
	}

	internal class WizardSelectOLAPObjectsAndProperties : WizardSelectOLAPObjects
	{
		private WizardPageObjectExplorer cubesPropPage;
		private WizardPageParams cubesPropParams;

		private WizardPageObjectExplorer dimsPropPage;
		private WizardPageParams dimsPropParams;

		protected override void CreatePages()
		{	
			base.CreatePages();			

			cubesPropParams = new WizardPageParams();
			cubesPropParams.Captions.Window = "Настройте объекты";			
			cubesPropPage = new WizardPageObjectExplorer(this, cubesPage.RightList, cubesPropParams);

			dimsPropParams = new WizardPageParams();
			dimsPropParams.Captions.Window = "Настройте объекты";
			dimsPropParams.Buttons.Finish.Handler = dimsPageParams.Buttons.Finish.Handler;
			dimsPropPage = new WizardPageObjectExplorer(this, dimsPage.RightList, dimsPropParams);
		}

		protected override void AddPages()
		{
			base.AddPages();			
			AddPage(cubesPropPage);
			AddPage(dimsPropPage);
		}

		protected override void InitCubesPageParams()
		{
			base.InitCubesPageParams();
			cubesPageParams.Buttons.Next.Handler = CubesPageNextButtonHandler;
		}

		protected override void InitDimsPageParams()
		{
			base.InitDimsPageParams();
			dimsPageParams.Buttons.Next.Handler = DimsPageNextButtonHandler;
			dimsPageParams.Buttons.Finish.Handler = null;
		}

		protected virtual void InitPackage()
		{
			package.ControlBlock.PackageFormat = PackageFormat.FullPack;
			package.ControlBlock.UpdateMode = Microsoft.AnalysisServices.UpdateMode.UpdateOrCreate;			
		}

		public WizardSelectOLAPObjectsAndProperties(IWin32Window _owner, DatabaseHeader _package)
			: base(_owner, _package)
		{
			//InitPackage();
		}

		public WizardSelectOLAPObjectsAndProperties(IWin32Window _owner,
			DatabaseHeader _package, List<string> dimNames, bool omitCubesPage)
			: base(_owner, _package, dimNames, omitCubesPage)
		{
			//InitPackage();
		}
		
		protected bool DimsPageNextButtonHandler(WizardPage page)
		{
			cubesPropPage.RefreshItems(cubesPage.RightList);
			dimsPropPage.RefreshItems(dimsPage.RightList);
			return true;
		}		
	}	
}