using System;
using System.Data;

using Krista.FM.Common;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.TemplatesService
{
	[Serializable]
	internal class Template : DisposableObject, ITemplate
	{
		private TemplatesRepository repository;

		private readonly int templateID;

		internal Template(TemplatesRepository repository, int ID)
		{
			this.repository = repository;
			templateID = ID;
		}

		#region реализация ITemplate

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				repository = null;
			}
			base.Dispose(disposing);
		}

		public int ID
		{
			get { return Convert.ToInt32(repository.GetTemplateRow(templateID)["ID"]); }
		}

		public string Code
		{
			get { return repository.GetTemplateRow(templateID)["Code"].ToString(); }
			set
			{
				if (Code == value)
					return;
				repository.GetTemplateRow(templateID)["Code"] = value;
			}
		}

		public string Name
		{
			get { return repository.GetTemplateRow(templateID)["NAME"].ToString(); }
			set
			{
				if (Name == value)
					return;
				repository.GetTemplateRow(templateID)["NAME"] = value;
			}
		}

		public string Description
		{
			get { return repository.GetTemplateRow(templateID)["DESCRIPTION"].ToString(); }
			set { repository.GetTemplateRow(templateID)["DESCRIPTION"] = value; }
		}

		public TemplateTypes TemplateType
		{
			get
			{
				int typeTemplate = Convert.ToInt32(repository.GetTemplateRow(templateID)["TYPE"]);
				return (TemplateTypes)typeTemplate;
			}
			set
			{
				repository.GetTemplateRow(templateID)["TYPE"] = Convert.ToInt32(value);
			}
		}

		public byte[] Document
		{
			get
			{
				return repository.GetDocument(templateID);
			}
			set
			{
				repository.UpdateDocument(value, ID);
			}
		}

		public string DocumentFileName
		{
			get { return repository.GetTemplateRow(templateID)["DOCUMENTFILENAME"].ToString(); }
			set { repository.GetTemplateRow(templateID)["DOCUMENTFILENAME"] = value; }
		}

		public int ParentID
		{
			get { return Convert.ToInt32(repository.GetTemplateRow(templateID)["PARENTID"]); }
			set { repository.GetTemplateRow(templateID)["PARENTID"] = value; }
		}

		public TemplateState GetTemplateState(int currentUser)
		{
			DataRow row = repository.GetTemplateRow(templateID);
			int templateUser = Convert.ToInt32(row["Editor"]);
			if (templateUser == currentUser)
				return TemplateState.EditingByCurrent;
			if (templateUser == -1)
				return TemplateState.EasyAccessState;
			return TemplateState.EditingState;
		}

		public bool SetTemplateState(int currentUser, TemplateState templateState)
		{
			if (GetTemplateState(currentUser) == TemplateState.EditingState)
				return false;
			int editor = -1;
			switch (templateState)
			{
				case TemplateState.EasyAccessState:
					editor = -1;
					break;
				case TemplateState.EditingState:
					editor = currentUser;
					break;
			}
			repository.GetTemplateRow(templateID)["Editor"] = editor;
			repository.UpdateTemplateState(editor, this.ID);
			// сохранить эти изменения в базу
			return true;
		}

		#endregion
	}
}
