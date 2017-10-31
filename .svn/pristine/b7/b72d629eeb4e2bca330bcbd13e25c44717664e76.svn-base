using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Krista.FM.Common;
using Krista.FM.Common.Xml;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Server.Common;
using Krista.FM.Server.Users;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Server.TemplatesService
{
	public partial class TemplatesRepository : DisposableObject, ITemplatesRepository
	{
		private readonly IScheme scheme;
	    private readonly ILinqRepository<Templates> repository;
	    private readonly ILinqRepository<B_Territory_RFBridge> territoryRepository;
        private readonly ILinqRepository<FX_FX_ThemSection> themSectionRepository; 
	    private readonly IUnitOfWorkFactory unitOfWorkFactory;

	    #region Константы
		/// <summary>
		/// Выбор всех данных кроме документов
		/// </summary>
		private const string selectQuery =
            "select ID, Code, Name, Type, Description, DocumentFileName, ParentID, Editor, RefTemplatesTypes, SortIndex, Flags from Templates";

		/// <summary>
		/// Выбор всех данных вместе с документом
		/// </summary>
		private const string selectAllQuery =
            "select ID, Code, Name, Type, Description, DocumentFileName, Document, ParentID, Editor, RefTemplatesTypes, SortIndex, Flags from Templates";

		/// <summary>
		/// Выбор всех данных вместе с пустым документом
		/// </summary>
		private const string selectAllWithNullDocumentQuery =
            "select ID, Code, Name, Type, Description, DocumentFileName, null as Document, ParentID, Editor, RefTemplatesTypes, SortIndex, Flags from Templates";

		/// <summary>
		/// Удаление по ID
		/// </summary>
		private const string deleteQuery = "delete from Templates where ID = ?";

		private const string generatorName = "g_Templates";
		#endregion Константы

        public TemplatesRepository(
            IScheme scheme,
            ILinqRepository<Templates> repository,
            IUnitOfWorkFactory unitOfWorkFactory,
            ILinqRepository<B_Territory_RFBridge> territoryRepository,
            ILinqRepository<FX_FX_ThemSection> themSectionRepository)
        {
            this.scheme = scheme;
            this.repository = repository;
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.territoryRepository = territoryRepository;
            this.themSectionRepository = themSectionRepository;
        }

#if DEBUG        
        protected override void Dispose(bool disposing)
        {
            Trace.TraceVerbose("~" + this);

            base.Dispose(disposing);
        }
#endif

		#region Внутренние вспомогательные методы

		private IDataUpdater GetInternalDataUpdater(Database db)
		{
			IDbDataAdapter adapter = db.GetDataAdapter();

			adapter.SelectCommand = db.Connection.CreateCommand();
			adapter.SelectCommand.CommandText = selectAllWithNullDocumentQuery;

			DataUpdater upd = new DataUpdater(adapter, null, db);
			upd.Transaction = db.Transaction;
			upd.OnInsteadUpdate += OnInsteadUpdate;
			return upd;
		}

		private bool OnInsteadUpdate(IDatabase db, DataRow dataRow)
		{
			switch (dataRow.RowState)
			{
				case DataRowState.Added:
					return InsteadInsert(db, dataRow, scheme.UsersManager);
				case DataRowState.Modified:
					return InsteadUpdate(db, dataRow);
				case DataRowState.Deleted:
					return InsteadDelete(db, dataRow, scheme.UsersManager);
				default:
					Trace.TraceError("Запись не имеет изменений или имеет состояние Detached. ConversionTable.OnInsteadUpdate()");
					return false;
			}
		}

		private static bool InsteadInsert(IDatabase db, DataRow dataRow, IUsersManager usersManager)
		{
			int affectedRows = Convert.ToInt32(db.ExecQuery(
                "insert into Templates (ID, Code, Name, Type, Description, DocumentFileName, ParentID, Editor, RefTemplatesTypes, SortIndex, Flags) values (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
				QueryResultTypes.NonQuery,
				db.CreateParameter(TemplateFields.ID, dataRow[TemplateFields.ID]),
				db.CreateParameter(TemplateFields.Code, dataRow[TemplateFields.Code]),
				db.CreateParameter(TemplateFields.Name, dataRow[TemplateFields.Name]),
				db.CreateParameter(TemplateFields.Type, dataRow[TemplateFields.Type]),
				db.CreateParameter(TemplateFields.Description, dataRow[TemplateFields.Description]),
				db.CreateParameter(TemplateFields.DocumentFileName, dataRow[TemplateFields.DocumentFileName]),
				db.CreateParameter(TemplateFields.ParentID, dataRow[TemplateFields.ParentID]),
				db.CreateParameter(TemplateFields.Editor, dataRow[TemplateFields.Editor]),
				db.CreateParameter(TemplateFields.RefTemplatesTypes, dataRow[TemplateFields.RefTemplatesTypes]),
				db.CreateParameter(TemplateFields.SortIndex, dataRow[TemplateFields.SortIndex]),
				db.CreateParameter(TemplateFields.Flags, dataRow[TemplateFields.Flags])
				));

			// Создаем права для владельца
			DataTable dt = usersManager.GetUsersPermissionsForObject(Convert.ToInt32(dataRow[TemplateFields.ID]), (int)SysObjectsTypes.Template);
			DataRow[] rows = dt.Select(String.Format("ID = {0}", Authentication.UserID));
			rows[0][Convert.ToString((int)TemplateOperations.ViewTemplate)] = true;
			rows[0][Convert.ToString((int)TemplateOperations.EditTemplateAction)] = true;
			DataTable changes = dt.GetChanges();
			usersManager.ApplayUsersPermissionsChanges(Convert.ToInt32(dataRow[TemplateFields.ID]), (int)SysObjectsTypes.Template, changes);

			return affectedRows == 1;
		}

		private bool InsteadUpdate(IDatabase db, DataRow dataRow)
		{
			CheckTemplateLock(db, dataRow);

		    int affectedRows = Convert.ToInt32(db.ExecQuery(
		        "update Templates set Code = ?, Name = ?, Type = ?, Description = ?, DocumentFileName = ?, ParentID = ?, Editor = ?, RefTemplatesTypes = ?, SortIndex = ?, Flags = ? where ID = ?",
		        QueryResultTypes.NonQuery,
		        db.CreateParameter(TemplateFields.Code, dataRow[TemplateFields.Code]),
		        db.CreateParameter(TemplateFields.Name, dataRow[TemplateFields.Name]),
		        db.CreateParameter(TemplateFields.Type, dataRow[TemplateFields.Type]),
		        db.CreateParameter(TemplateFields.Description, dataRow[TemplateFields.Description]),
		        db.CreateParameter(TemplateFields.DocumentFileName, dataRow[TemplateFields.DocumentFileName]),
		        db.CreateParameter(TemplateFields.ParentID, dataRow[TemplateFields.ParentID]),
		        db.CreateParameter(TemplateFields.Editor, dataRow[TemplateFields.Editor]),
		        db.CreateParameter(TemplateFields.RefTemplatesTypes, dataRow[TemplateFields.RefTemplatesTypes]),
		        db.CreateParameter(TemplateFields.SortIndex, dataRow[TemplateFields.SortIndex]),
		        db.CreateParameter(TemplateFields.Flags, dataRow[TemplateFields.Flags]),		        
                db.CreateParameter(TemplateFields.ID, dataRow[TemplateFields.ID])
                ));
			
			return affectedRows == 1;
		}

		private bool InsteadDelete(IDatabase db, DataRow dataRow, IUsersManager usersManager)
		{
			if (0 == Convert.ToInt32(db.ExecQuery("select count(*) from Templates where ID = ?",
				QueryResultTypes.Scalar,
				db.CreateParameter(TemplateFields.ID, dataRow[TemplateFields.ID, DataRowVersion.Original]))))
			{
				Trace.TraceWarning("Попытка удалить несуществующую запись шаблона (ID = {0})", dataRow[TemplateFields.ID, DataRowVersion.Original]);
				return false;
			}
			
			CheckTemplateLock(db, dataRow);

			int affectedRows = Convert.ToInt32(db.ExecQuery(deleteQuery, QueryResultTypes.NonQuery,
				db.CreateParameter(TemplateFields.ID, dataRow[TemplateFields.ID, DataRowVersion.Original])));

			// Удаляем права на объект
			DataTable dt = usersManager.GetUsersPermissionsForObject(Convert.ToInt32(dataRow[TemplateFields.ID, DataRowVersion.Original]), (int)SysObjectsTypes.Template);
			foreach (DataRow row in dt.Rows)
				foreach (DataColumn column in dt.Columns)
					if (column.DataType.Name == typeof(Boolean).Name)
						row[column] = false;
			DataTable changes = dt.GetChanges();
			usersManager.ApplayUsersPermissionsChanges(Convert.ToInt32(dataRow[TemplateFields.ID, DataRowVersion.Original]), (int)SysObjectsTypes.Template, changes);

			return affectedRows == 1;
		}

		private void CheckTemplateLock(IDatabase db, DataRow dataRow)
		{
			int templateID = Convert.ToInt32(dataRow[TemplateFields.ID, DataRowVersion.Original]);
			
			// Ищем запись в базе
			object result = db.ExecQuery(
				"select Editor from Templates where ID = ?", QueryResultTypes.Scalar,
				db.CreateParameter(TemplateFields.ID, templateID));
			if (result == null)
				throw new TemplateLokedException(String.Format("Шаблон отчета (ID = {0}), не найден в базе данных.", templateID));
			int templateOwnerUserID = Convert.ToInt32(result);

			// Проверяем корректность блокировки
			if (templateOwnerUserID != -1 && templateOwnerUserID != Authentication.UserID)
			{
				throw new TemplateLokedException(String.Format(
					"Шаблон отчета (ID = {0}, Name = '{1}') не может быть изменен, т.к. он заблокирован пользователем {2}.",
					templateID, dataRow[TemplateFields.Name, DataRowVersion.Original],
					scheme.UsersManager.GetUserNameByID(templateOwnerUserID)));
			}
		}

        private void CheckTemplateLock(Templates template)
        {
            if (template == null)
            {
                throw new TemplateLokedException("Шаблон отчета, не найден в базе данных.");
            }

            if (template.Editor == null || template.Editor == -1)
                return;

            // Проверяем корректность блокировки
            if (template.Editor != null && template.Editor != Authentication.UserID)
            {
                throw new TemplateLokedException(String.Format(
                    "Шаблон отчета (ID = {0}, Name = '{1}') не может быть изменен, т.к. он заблокирован пользователем {2}.",
                    template.ID, template.Name,
                    scheme.UsersManager.GetUserNameByID(Convert.ToInt32(template.Editor))));
            }
        }

        #endregion

		#region методы ITemplatesRepository

		public DataTable GetTemplatesInfo(TemplateTypes templateType)
		{
			UsersManager um = (UsersManager)scheme.UsersManager;

			// определяем может ли пользователь просматривать все шаблоны
			bool canViewAll = um.HasUserPermissionForOperation((int)Authentication.UserID, (int)AllTemplatesOperations.ViewAllUsersTemplates);

			canViewAll |= um.CheckTemplateTypePermissionForTemplate(
				(int)Authentication.UserID, (int)templateType, TemplateTypeOperations.ViewAllUsersTemplates);

			// если пользователю доступны не все шаблоны - получаем вспомогательные списки
			ArrayList visibleTaskTypesForUser = null;
            ArrayList visibleTasksForUser = null;
            if (!canViewAll)
            {
                // получаем список типов задач которые может просматривать пользователь и все группы куда он входит
                visibleTaskTypesForUser = um.GetUserVisibleTemplateTypes((int)Authentication.UserID);
                // получаем список ID задач которые может просматривать пользователь и все группы куда он входит
                visibleTasksForUser = um.GetUserVisibleTemplates((int)Authentication.UserID);
            }

			using (IDatabase db = scheme.SchemeDWH.DB)
			{
				DataTable dt = (DataTable) db.ExecQuery(
					selectQuery + " where RefTemplatesTypes = ? order by SortIndex",
                   	QueryResultTypes.DataTable,
                   	db.CreateParameter(TemplateFields.RefTemplatesTypes, (int) templateType));
				
				DataColumn columnIsVisible = dt.Columns.Add(TemplateFields.IsVisible, typeof(bool));
				columnIsVisible.DefaultValue = true;

				foreach (DataRow row in dt.Rows)
				{
					row[columnIsVisible] = true;

					if ((Convert.ToString(row[TemplateFields.Name]) == "Источники финансирования" && Convert.ToInt32(row[TemplateFields.Type]) == (int)TemplateDocumentTypes.Group) ||
						(Convert.ToString(row[TemplateFields.Name]) == "Шаблоны отчетов" && Convert.ToInt32(row[TemplateFields.Type]) == (int)TemplateDocumentTypes.Group))
					{
						row[columnIsVisible] = false;
						continue;
					}

					// если пользователь может просматривать все шаблоны - ставим признак видимости
					if (canViewAll)
						continue;

					// если задача имеет тип который пользователь может просматривать - ставим признак видимости
					if ((visibleTaskTypesForUser != null) && 
						(visibleTaskTypesForUser.Contains(templateType)))
						continue;

					// если текущий пользователь имеет право на просмотр этой конкретной задачи - ставим признак видимости
					if (visibleTasksForUser.Contains(Convert.ToInt32(row[TemplateFields.ID])))
						continue;

					// иначе - задача не видна
					row[columnIsVisible] = false;
				}

				return dt;
			}
		}

        public IList<TemplateDTO> GetTemplatesInfo(TemplateTypes templateType, int? parentId)
        {
            using (new PersistenceContext())
            {
                IList<Templates> templateses =
                    repository.FindAll().Where(
                        x => (x.RefTemplatesTypes.ID == (int) templateType) && x.ParentID == parentId).ToList();

                IList<TemplateDTO> templateDtos = new List<TemplateDTO>();

                foreach (var template in templateses)
                {
                    TemplateDTO templateDto = new TemplateDTO
                                                  {
                                                      Code = template.Code,
                                                      Description = template.Description,
                                                      DocumentFileName = template.DocumentFileName,
                                                      Editor = template.Editor,
                                                      Flags = template.Flags,
                                                      ID = template.ID,
                                                      Name = template.Name,
                                                      ParentID = template.ParentID,
                                                      SortIndex = template.SortIndex,
                                                      Type = template.Type,
                                                      RefTemplatesTypes = template.RefTemplatesTypes.ID,
                                                  };

                    // Document
                    ParseDocument(templateDto, template);

                    templateDtos.Add(templateDto);
                }

                return templateDtos;
            }
        }

	    private static void ParseDocument(TemplateDTO templateDto, Templates template)
	    {
	        byte[] document = template.Document;
	        string xmlDescriptor = string.Empty;
	        IPhoteTemplateDescriptor descriptor;

	        //если данных нет, запишем свойства поумолчанию
	        if (document == null)
	        {
	            descriptor = new IPhoteTemplateDescriptor();
	            descriptor.SubjectDepended = false;
	            descriptor.IsNotScrollable = false;
	            descriptor.TemplateType = MobileTemplateTypes.None;
	            descriptor.LastDeployDate = DateTime.MinValue;
	            descriptor.IconByte = null;
	            descriptor.ForumDiscussionID = 0;
	            descriptor.TerritoryRF = String.Empty;
	            descriptor.ThemeSection = String.Empty;

	            xmlDescriptor = XmlHelper.Obj2XmlStr(descriptor);
	            document = Encoding.UTF8.GetBytes(xmlDescriptor);
	        }

	        xmlDescriptor = Encoding.UTF8.GetString(document);
	        descriptor = XmlHelper.XmlStr2Obj<IPhoteTemplateDescriptor>(xmlDescriptor);
	        templateDto.SubjectDepended = descriptor.SubjectDepended;
	        templateDto.IsNotScrollable = descriptor.IsNotScrollable;
	        templateDto.LastDeployDate = descriptor.LastDeployDate;
	        templateDto.TemplateType = descriptor.TemplateType;
	        templateDto.IconByte = descriptor.IconByte;
	        templateDto.ForumDiscussionID = descriptor.ForumDiscussionID;
	        templateDto.TerritoryRF = descriptor.TerritoryRF;
	        templateDto.ThemeSection = descriptor.ThemeSection;
	    }

	    public IDataUpdater GetDataUpdater()
		{
            using (Database db = (Database)scheme.SchemeDWH.DB)
            {
                return GetInternalDataUpdater(db);
            }
		}

        //[UnitOfWork]
        public virtual void LockTemplate(int templateId)
		{
            using (IUnitOfWork unitOfWork = unitOfWorkFactory.Create())
            {
                Trace.TraceVerbose("Блокировка шаблона (ID = {0}) пользователем {1}", templateId,
                                   Authentication.UserName);

                var template = repository.FindOne(templateId);
                CheckTemplateLock(template);
                template.Editor = Authentication.UserID;
                repository.Save(template);

                unitOfWork.Commit();
            }
		}

        //[UnitOfWork]
        public virtual void UnlockTemplate(int templateId)
		{
            using (IUnitOfWork unitOfWork = unitOfWorkFactory.Create())
            {
                var template = repository.FindOne(templateId);
                CheckTemplateLock(template);
                template.Editor = -1;
                repository.Save(template);

                unitOfWork.Commit();
            }
		}

		public int NewTemplateID()
		{
			using (IDatabase db = scheme.SchemeDWH.DB)
			{
				return db.GetGenerator(generatorName);
			}
		}

        //[UnitOfWork]
		public virtual byte[] GetDocument(int templateId)
		{
            using (new PersistenceContext())
            {
                var template = repository.FindOne(templateId);
                if (template == null)
                {
                    return null;
                }

                return template.Document;
            }
		}

		/// <summary>
		/// Проверяет прикреплен ли документ к шаблону.
		/// </summary>
		/// <param name="templateId">ID шаблона.</param>
        //[UnitOfWork]
        public virtual bool ExistDocument(int templateId)
		{
            using (new PersistenceContext())
            {
                return repository.FindAll().Any(x => x.ID == templateId && x.Document != null);
            }
		}

        //[UnitOfWork]
        public virtual void SetDocument(byte[] documentData, int templateId)
		{
            using (IUnitOfWork unitOfWork = unitOfWorkFactory.Create())
            {
                var template = repository.FindOne(templateId);
                if (template != null)
                {
                    template.Document = documentData;
                    repository.Save(template);

                    unitOfWork.Commit();
                }
            }
		}

	    public string GetTerritoryName(int code)
	    {
            using (new PersistenceContext())
            {
                return territoryRepository.FindAll().First(x => x.Code == code).Name;
            }
	    }

	    public string GetThemeSectionName(int code)
	    {
            using (new PersistenceContext())
            {
                return themSectionRepository.FindAll().First(x => x.Code == code).Name;
            }
	    }

	    #endregion
	}
}