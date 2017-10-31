using System;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.FactTables;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.ForecastUI
{

	class ForecastFactTablesUI : FactTablesUI
	{
		public ForecastFactTablesUI(IEntity dataObject)
			: base(dataObject)
		{

		}

		public override void Initialize()
		{
			base.Initialize();
			vo.ugeCls.OnGridInitializeLayout += new GridInitializeLayout(OnGridFactTableInitializeLayout);
			vo.ugeCls.OnInitializeRow += new InitializeRow(OnInitializeRow);
		}

		public virtual void OnInitializeRow(object sender, InitializeRowEventArgs e)
		{
			
		}

		public virtual void OnGridFactTableInitializeLayout(object sender, InitializeLayoutEventArgs e)
		{
			
		}

		public override void UpdateToolbar()
		{
			//base.UpdateToolbar();
			//vo.utbToolbarManager.Toolbars["utbFilters"].Visible = false;
			//vo.utbToolbarManager.Toolbars["SelectTask"].Visible = false;
			vo.utbToolbarManager.Toolbars["utbFilters"].Visible = false;
			vo.utbToolbarManager.Toolbars["SelectTask"].Visible = false;
			vo.ugeCls.utmMain.Tools["menuSave"].SharedProps.Enabled = true;
			//vo.ugeCls.utmMain.Tools["deleteSelectedRows"].SharedProps.Visible = false;
			vo.ugeCls.utmMain.Tools["ClearCurrentTable"].SharedProps.Visible = false;
			vo.ugeCls.utmMain.Tools["menuLoad"].SharedProps.Visible = false;
			vo.ugeCls.utmMain.Tools["menuSave"].SharedProps.Visible = false;
			vo.ugeCls.utmMain.Tools["ShowHierarchy"].SharedProps.Visible = false;
			vo.ugeCls.utmMain.Tools["PasteRow"].SharedProps.Visible = false;
			((Infragistics.Win.UltraWinToolbars.StateButtonTool)vo.ugeCls.utmMain.Tools["ShowGroupBy"]).Checked = false;
		}

		public override void SetTaskId(ref UltraGridRow row)
		{
			row.Cells["TASKID"].Value = -1;
			//base.SetTaskId(ref row);
		}
	}

	class FactTableModal
	{
		private IInplaceClsView currentViewObject;
		protected BaseClsUI clsUI;

		public virtual Int32 ShowFactTableModal(String key)
		{
			frmModalTemplate modalClsForm = new frmModalTemplate();

			IFactTable cls = (IFactTable)ForecastNavigation.Instance.Workplace.ActiveScheme.FactTables[key];
						 
			CreateFactTableUI(cls);
			if (clsUI != null)
			{
				modalClsForm.FormCaption = String.Format("{0}: {1}", "Таблица фактов", cls.OlapName);

				clsUI.Workplace = ForecastNavigation.Instance.Workplace;
				clsUI.RestoreDataSet = false;

				clsUI.Initialize();

				clsUI.InitModalCls(-1);

				currentViewObject = (IInplaceClsView)clsUI;

				currentViewObject.InitModalCls(-1);
				modalClsForm.SuspendLayout();
				// и присоединяем к форме
				try
				{
					modalClsForm.AttachCls(currentViewObject);
					ComponentCustomizer.CustomizeInfragisticsControls(modalClsForm);
				}
				finally
				{
					modalClsForm.ResumeLayout();
				}
				// ...загружаем данные
				currentViewObject.RefreshAttachedData();

				AdditionsFilters(clsUI);

				modalClsForm.Shown += new EventHandler(modalClsForm_Shown);
				if (modalClsForm.ShowDialog((Form)ForecastNavigation.Instance.Workplace) == DialogResult.OK)
				{
					int clsID = modalClsForm.AttachedCls.GetSelectedID();
					// если ничего не выбрали - считаем что функция завершилась неудачно
					if (clsID == -10)
						return -1;
					else return clsID;
				}
				else return -1;
			}
			else return -1;
		}

		public virtual void CreateFactTableUI(IFactTable cls)
		{
			clsUI = new ForecastFactTablesUI(cls);
		}

		public virtual void AdditionsFilters(BaseClsUI clsUI)
		{
		}
		
		public virtual void modalClsForm_Shown(object sender, EventArgs e)
		{
			((Infragistics.Win.UltraWinToolbars.StateButtonTool)((BaseClsView)((BaseClsUI)currentViewObject).ViewCtrl).ugeCls.utmMain.Toolbars["utbColumns"].Tools["ShowFilter"]).Checked = false;
		}
	}
}