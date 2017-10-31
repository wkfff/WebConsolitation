using System;
using System.Windows.Forms;

using Infragistics.Win.UltraWinGrid;

using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.Workplace.Gui;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.Navigation
{
	public partial class EntityNavigationListView : BaseView
	{
		public EntityNavigationListView()
		{
			InitializeComponent();

			grid.AllowClearTable = false;
			grid.AllowDeleteRows = false;
			grid.AllowImportFromXML = false;
			grid.AllowAddNewRecords = false;
			grid.ExportImportToolbarVisible = false;
			grid.SaveMenuVisible = false;
			grid.LoadMenuVisible = false;
			grid.StateRowEnable = false;
			grid._utmMain.Tools["ShowHierarchy"].SharedProps.Visible = false;
			grid._utmMain.Tools["ColumnsVisible"].SharedProps.Visible = true;
			grid._utmMain.Tools["ClearCurrentTable"].SharedProps.Visible = false;
			grid._utmMain.Tools["DeleteSelectedRows"].SharedProps.Visible = false;

			Grid.ugData.MouseEnterElement += ugData_MouseEnterElement;
			Grid.ugData.MouseClick += ugData_MouseClick;
			cmsAuditSchemeObject.ItemClicked += cmsAuditSchemeObject_ItemClicked;
		}

		internal UltraGridEx Grid
		{
			get { return grid; }
		}

		internal void AddGoToButton()
		{
			UltraGridColumn clmn = grid.ugData.DisplayLayout.Bands[0].Columns.Add("GoToObject", "");
			UltraGridHelper.SetLikelyButtonColumnsStyle(clmn, -1);
			clmn.CellButtonAppearance.Image = imageList.Images[0];
			clmn.Header.VisiblePosition = 1;
		}


		#region Обработчики и методы, связанные с отображением аудита

		// показывает, что курсор находится на элементе выбора записей в навигационном гриде
		private bool _activeUIElementIsSchemeObject = false;

		private void ugData_MouseEnterElement(object sender, Infragistics.Win.UIElementEventArgs e)
		{
			_activeUIElementIsSchemeObject = e.Element is RowSelectorUIElement;
		}

		/// <summary>
		/// Обработчик нажатия мышки навигационного грида
		/// </summary>
		private void ugData_MouseClick(object sender, MouseEventArgs e)
		{
			// при клике мышки на панель навигации ставим на нее фокус
			Grid.ugData.Focus();

			if (e.Button == MouseButtons.Right)
			{
				// и если кликнули правой кнопкой и стоим на элементе выбора записей,
				// где находится курсор активной записи
				if (_activeUIElementIsSchemeObject)
				{
					// показываем меню, по которому можно вызвать аудит для всего объекта
					//_auditShowObject = AuditShowObject.SchemeObject;
					cmsAuditSchemeObject.Show(Grid.ugData.PointToScreen(e.Location));
				}
			}
		}

		/// <summary>
		/// Вызов аудита для всего классификатора.
		/// </summary>
		private void cmsAuditSchemeObject_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Name == "аудитSchemeObjectToolStripMenuItem")
			{
				cmsAuditSchemeObject.Hide();
				
				IEntity entity =
					WorkplaceSingleton.Workplace.ActiveScheme.RootPackage.FindEntityByName(
						Convert.ToString(grid.ugData.ActiveRow.Cells["ObjectKey"].Value));
				
				frmAuditModal.ShowAudit(
					WorkplaceSingleton.Workplace,
					entity.FullName, entity.FullCaption, -1, AuditShowObjects.ClsObject);
			}
		}
		#endregion Обработчики и методы, связанные с отображением аудита
	}
}