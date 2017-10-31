using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

using Infragistics.Win.UltraWinGrid;

using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Common;
using Krista.FM.Common.FileUtils;

using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.TemplatesService;
using Krista.FM.Client.ViewObjects.TemplatesUI.Commands;

namespace Krista.FM.Client.ViewObjects.TemplatesUI
{
	internal abstract partial class TemplatestUIBase : BaseViewObj
	{
		private ITemplatesRepository repository;
		protected DataTable dtTemplates;
		private readonly TemplateTypes templateType;


		protected TemplatestUIBase(string key, TemplateTypes templateType)
            : base(key)
        {
			Trace.TraceVerbose("Создание экземпляра объекта просмотра \"{0}\"", GetType().FullName);
			this.templateType = templateType;
			Caption = "Репозиторий отчетов";
			 
        }

		protected override void SetViewCtrl()
		{
			fViewCtrl = new TemplatesView();
		}

		protected TemplatesView ViewObject
		{
			[System.Diagnostics.DebuggerStepThrough]
			get { return (TemplatesView)fViewCtrl; }
		}

		internal ITemplatesRepository Repository
		{
			get { return repository; }
		}

		public override void Initialize()
		{
			Trace.TraceVerbose("Инициализация объекта просмотра \"{0}\"", FullCaption);

			base.Initialize();

			repository = Workplace.ActiveScheme.TemplatesService.Repository;

			ViewObject.ugeTemplates.SaveLoadFileName = Caption;
			
			// Устанавливаем сортировку по полю SortIndex
			ViewObject.ugeTemplates.sortColumnName = TemplateFields.SortIndex;
			
			ViewObject.ugeTemplates.OnGetGridColumnsState += OnGetGridColumnsState;
			ViewObject.ugeTemplates.OnGridInitializeLayout += OnGridInitializeLayout;
			ViewObject.ugeTemplates.OnInitializeRow += OnInitializeRow;
			
			ViewObject.ugeTemplates.OnRefreshData += OnRefreshData;
			ViewObject.ugeTemplates.OnSaveChanges += OnSaveChanges;
			ViewObject.ugeTemplates.OnCancelChanges += OnCancelChanges;

			ViewObject.ugeTemplates.OnAfterRowActivate += OnAfterRowActivate;
			ViewObject.ugeTemplates.OnAfterRowInsert += OnAfterRowInsert;
			ViewObject.ugeTemplates.OnBeforeRowsDelete += OnBeforeRowsDelete;
			ViewObject.ugeTemplates.OnCellChange += OnCellChange;
			ViewObject.ugeTemplates.ugData.AfterCellUpdate += OnAfterCellUpdate;
            ViewObject.ugeTemplates.ugData.BeforeCellUpdate += new BeforeCellUpdateEventHandler(ugData_BeforeCellUpdate);

			ViewObject.ugeTemplates.GridSelectionDrag += OnGridSelectionDrag;
			ViewObject.ugeTemplates.GridDragEnter += OnGridDragEnter;
			ViewObject.ugeTemplates.GridDragLeave += OnGridDragLeave;
			ViewObject.ugeTemplates.GridDragDrop += OnGridDragDrop;
			ViewObject.ugeTemplates.GridDragOver += OnGridDragOver;

			ViewObject.ugeTemplates.OnLoadFromXML += OnLoadFromXML;
			ViewObject.ugeTemplates.OnSaveToXML += OnSaveToXML;

			commandList.Add(typeof(Commands.CreateRootTemplateCommand).Name, new Commands.CreateRootTemplateCommand());
			commandList.Add(typeof(Commands.CreateChildTemplateCommand).Name, new Commands.CreateChildTemplateCommand());

			// добавляем кнопочки на основной тулбар
			ViewObject.InitializeGridToolBarCommands(CommandList);

			SetPermissions();
		}

		protected override void Dispose(bool disposing)
		{
			Trace.TraceVerbose("Уничтожение (disposing == {0}) объекта просмотра \"{1}\"", disposing, FullCaption);
			base.Dispose(disposing);
		}

		private readonly Dictionary<string, TemplatesCommand> commandList = new Dictionary<string, TemplatesCommand>();

		public Dictionary<string, TemplatesCommand> CommandList
		{
			get { return commandList; }
		}

		public override void InitializeData()
		{
			ReloadData();
		}

		protected virtual DataTable GetTemplatesInfo()
		{
			DataTable dt = Repository.GetTemplatesInfo(templateType);
			dt.Columns.Add(TemplateFields.LastEditData, typeof(DateTime));
			dt.Columns.Add(TemplateFields.Document, typeof(byte[]));
			return dt;
		}

		public override void ReloadData()
		{
			UltraGridStateSettings gridSettings = UltraGridStateSettings.SaveUltraGridStateSettings(Grid);

			if (dtTemplates != null)
				dtTemplates.Clear();

			dtTemplates = GetTemplatesInfo();
			dtTemplates.AcceptChanges();

			Grid.DisplayLayout.MaxBandDepth = 10;

			// создаем объект для хранения иерархических данных
			DataSet ds = new DataSet("TemplatesDataSet");
			ds.Tables.Add(dtTemplates);
			ds.Relations.Add("TempateRelation", 
				ds.Tables[0].Columns[TemplateFields.ID], 
				ds.Tables[0].Columns[TemplateFields.ParentID]);
			ds.Tables[0].Constraints.Clear();
			ViewObject.ugeTemplates.DataSource = ds;
			ViewObject.ugeTemplates.ugData.DisplayLayout.AddNewBox.Hidden = true;
			ViewObject.ugeTemplates.ugData.DisplayLayout.Bands[0].ColumnFilters[TemplateFields.ParentID].FilterConditions.Add(FilterComparisionOperator.Equals, DBNull.Value);
			ViewObject.ugeTemplates.utmMain.Tools["btnVisibleAddButtons"].SharedProps.Visible = false;

			// делаем активной первую строку в гриде
			ActiveateFirstGridRow(Grid);

			gridSettings.RestoreUltraGridStateSettings(Grid);
		}

		public override void SaveChanges()
		{
			SaveData();
		}

		public bool SaveData()
		{
			if (Grid.ActiveRow != null)
				Grid.ActiveRow.Update();

			if (!CheckNotNullFields())
				return false;

			DataTable changes = dtTemplates.GetChanges();
			if (changes == null)
				return true;

            List<int> savedTemplates = new List<int>();

			using (IDataUpdater du = Repository.GetDataUpdater())
			{
				Trace.TraceInformation("Сохранение изменений репозитория шаблонов");
				Trace.Indent();
				try
				{
					du.Update(ref changes);

					foreach (DataRow row in changes.Rows)
					{
						if (row.RowState == DataRowState.Deleted)
							continue;

						int templateId = Convert.ToInt32(row[TemplateFields.ID]);
					    savedTemplates.Add(templateId);
                        // сохраняем документ в том случае, если он есть в данных);
						if (row[TemplateFields.Document] != DBNull.Value)
						{
							Trace.TraceVerbose("Передача на сервер документа \"{0}\" ({1} байт)", 
								row[TemplateFields.DocumentFileName], 
								((byte[])row[TemplateFields.Document]).GetLength(0));
							
							Repository.SetDocument((byte[])row[TemplateFields.Document], templateId);
							row[TemplateFields.LastEditData] = DBNull.Value;
							row[TemplateFields.Document] = DBNull.Value;
							dtTemplates.Select("ID = " + templateId)[0][TemplateFields.Document] = DBNull.Value;
							row.AcceptChanges();
						}
						else
						{
							// Сохраняем документ в базу
							if (row[TemplateFields.LastEditData] != DBNull.Value)
							{
								System.IO.FileInfo fi = new System.IO.FileInfo(row[TemplateFields.DocumentFileName].ToString());

								// документ был открыт, закрыт и там были какие то изменения... записываем его в базу
								// или документ был добавлен... записываем его в базу.
								if (fi.LastWriteTimeUtc != Convert.ToDateTime(row[TemplateFields.LastEditData]) ||
									!Repository.ExistDocument(templateId))
								{
									byte[] fileData 
										= fi.LastWriteTimeUtc != Convert.ToDateTime(row[TemplateFields.LastEditData])
									    ? DocumentsHelper.CompressFile(FileHelper.ReadFileData(fi.FullName))
									    : (byte[]) row[TemplateFields.Document];

									Trace.TraceVerbose("Передача на сервер документа \"{0}\" ({1} байт)", fi.FullName, fileData.GetLength(0));
									Repository.SetDocument(fileData, templateId);
								}
								else
								{
									Trace.TraceVerbose("Нет изменений для файла {0}", fi.FullName);
								}

								row[TemplateFields.LastEditData] = DBNull.Value;
								row[TemplateFields.Document] = DBNull.Value;
							}
						}
					}

					changes.AcceptChanges();
					dtTemplates.AcceptChanges();

					Trace.Unindent();
					Trace.TraceInformation("Изменения репозитория шаблонов сохранены успешно");
				}
				catch (ServerException e)
				{
					Trace.Unindent();
					if (e.InnerException is TemplateLokedException)
						MessageBox.Show((IWin32Window)Workplace, e.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
					else
						throw new ServerException(e.Message, e);
				}
			}
            // разблокируем все отчеты, по которым были сохранены изменения
            foreach (int templateId in savedTemplates)
            {
                UnlockTemplate(templateId);
                DataRow templateRow = dtTemplates.Select(string.Format("ID = {0}", templateId))[0];
                templateRow[TemplateFields.Editor] = -1;
                templateRow[TemplateFields.LastEditData] = DBNull.Value;
            }
            dtTemplates.AcceptChanges();

		    return true;
		}

		private bool OnRefreshData(object sender)
		{
			ReloadData();
			return true;
		}

		private bool OnSaveChanges(object sender)
		{
			return SaveData();
		}

		private void OnCancelChanges(object sender)
		{
			if (Grid.ActiveRow != null)
				Grid.ActiveRow.Update();

			// Отменяем блокировку измененных документов
			DataTable changes = dtTemplates.GetChanges();
			if (changes != null)
			{
				foreach (DataRow row in changes.Rows)
				{
					if (row.RowState == DataRowState.Added)
						continue;
					Repository.UnlockTemplate(Convert.ToInt32(
						row[TemplateFields.ID, DataRowVersion.Original]));
				}
			}

			dtTemplates.RejectChanges();

			ReloadData();
		}

		/// <summary>
		/// Получение записи документа по ID.
		/// </summary>
		internal DataRow GetTemplateRow(int templateId)
		{
			DataRow[] rows = dtTemplates.Select(string.Format("ID = {0}", templateId));
			if (rows.Length > 0)
				return rows[0];
			return null;
		}

		internal bool IsNewTemplate(int templateId)
		{
			DataRow dataRow = GetTemplateRow(templateId);

			if (dataRow == null)
			{
				UltraGridRow row = GetUltraGridRowById(Grid, templateId);
				return row.IsAddRow;
			}
			return dataRow.RowState == DataRowState.Added;
		}

		internal void LockTemplate(int templateId)
		{
			DataRow dataRow = GetTemplateRow(templateId);
			if (dataRow == null)
				return;
			if (dataRow.RowState == DataRowState.Deleted || dataRow.RowState == DataRowState.Added)
				return;

			UltraGridRow row = GetUltraGridRowById(Grid, templateId);

			if (row.IsDeleted || row.IsAddRow)
				return;

            if (row.Cells[TemplateFields.Editor].Value == DBNull.Value ||
                row.Cells[TemplateFields.Editor].Value == null)
                return;

			if (Convert.ToInt32(row.Cells[TemplateFields.Editor].Value) != ClientAuthentication.UserID)
			{
				Trace.TraceVerbose("Блокировка шаблона (ID = {0})", templateId);
				
				Repository.LockTemplate(templateId);
				row.Cells[TemplateFields.Editor].Value = ClientAuthentication.UserID;
			}
		}

		private void UnlockTemplate(int templateId)
		{
			DataRow row = GetTemplateRow(templateId);

			if (row.RowState == DataRowState.Deleted || row.RowState == DataRowState.Added)
				return;

			if (Convert.ToInt32(row[TemplateFields.Editor]) == ClientAuthentication.UserID)
			{
				Trace.TraceVerbose("Снимаем блокировку шаблона (ID = {0})", templateId);
				// достаточно разблокировки отчета в базе
				Repository.UnlockTemplate(templateId);
				UltraGridRow gridRow = GetUltraGridRowById(Grid, templateId);
				//gridRow.Cells[TemplateFields.Editor].Value = -1;
				//gridRow.Cells[TemplateFields.LastEditData].Value = DBNull.Value;
			}
		}

		private void SetDetailVisible(bool isVisible)
		{
			ViewObject.sc1.Panel2.Enabled = isVisible;
		}
	}
}