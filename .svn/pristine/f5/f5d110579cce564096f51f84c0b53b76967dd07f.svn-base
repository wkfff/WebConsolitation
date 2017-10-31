using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI
{
	/// <summary>
	/// 
	/// </summary>
	public class BaseDetailTableUI : BaseClsUI
	{
		private IEntityAssociation entityAssociation;
		private BaseDetailTableView viewControl;

		public BaseDetailTableUI(IEntityAssociation entityAssociation)
			: base(entityAssociation.RoleBridge)
		{
			this.entityAssociation = entityAssociation;
		}

		public override void Initialize()
		{
			viewControl = new BaseDetailTableView();
		}

		public override string Key
		{
			get { return entityAssociation.ObjectKey; }
		}

		public override System.Windows.Forms.Control Control
		{
			get { return viewControl; }
		}

		public string Caption
		{
			get { return entityAssociation.RoleBridge.FullCaption; }
		}

		/// <summary>
		/// настройка грида детали
		/// </summary>
		private void DetailGridSetup(UltraGridEx ugeDetail)
		{
			/*ugeDetail.OnGetHierarchyInfo += new GetHierarchyInfo(ugeDetail_OnGetHierarchyInfo);
			ugeDetail.OnSaveChanges += new SaveChanges(ugeDetail_OnSaveChanges);
			ugeDetail.OnRefreshData += new RefreshData(ugeDetail_OnRefreshData);
			ugeDetail.OnClearCurrentTable += new DataWorking(ugeDetail_OnClearCurrentTable);
			ugeDetail.OnCancelChanges += new DataWorking(ugeDetail_OnCancelChanges);
			ugeDetail.OnGetGridColumnsState += new GetGridColumnsState(ugeDetail_OnGetGridColumnsState);
			ugeDetail.OnInitializeRow += new InitializeRow(ugeDetail_OnInitializeRow);
			ugeDetail.ToolClick += new ToolBarToolsClick(ugeDetail_ToolClick);
			ugeDetail.OnClickCellButton += new ClickCellButton(ugeDetail_OnClickCellButton);
			ugeDetail.OnAfterRowInsert += new AfterRowInsert(ugeDetail_OnAfterRowInsert);

			ugeDetail.OnBeforeRowDeactivate += new BeforeRowDeactivate(ugeDetail_OnBeforeRowDeactivate);

			ugeDetail.OnGridInitializeLayout += new GridInitializeLayout(ugeDetail_OnGridInitializeLayout);

			ugeDetail.OnGetLookupValue += new GetLookupValueDelegate(ugeDetail_OnGetLookupValue);
			ugeDetail.OnCheckLookupValue += new CheckLookupValueDelegate(ugeDetail_OnCheckLookupValue);

			ugeDetail.OnMouseEnterGridElement += new MouseEnterElement(ugeCls_OnMouseEnterGridElement);
			ugeDetail.OnMouseLeaveGridElement += new MouseLeaveElement(ugeCls_OnMouseLeaveGridElement);

			ugeDetail.GridDragDrop += new MainMouseEvents(ugeDetail_GridDragDrop);
			ugeDetail.GridDragEnter += new MainMouseEvents(ugeDetail_GridDragEnter);
			*/
			//ugeDetail.ugData.AfterColPosChanged += new AfterColPosChangedEventHandler(ugData_AfterColPosChanged);

		}

		private void ugeDetail_OnInitializeRow(object sender, InitializeRowEventArgs e)
		{
			UltraGridRow tmpRow = UltraGridHelper.GetRowCells(e.Row);
			if (tmpRow.Cells["ID"].Value == DBNull.Value || tmpRow.Cells["ID"].Value == null)
				return;

			//SetDocumentRow(tmpRow, viewControl.Grid.CurrentStates);
		}

		// сделать выделенной/активной Select();

	}
}
