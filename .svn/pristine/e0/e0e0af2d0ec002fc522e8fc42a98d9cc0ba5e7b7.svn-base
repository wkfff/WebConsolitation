using System;
using System.Data;
using System.IO;
using Krista.FM.Common;
using Krista.FM.Common.Templates;
using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Client.ViewObjects.TemplatesUI
{
	internal partial class TemplatestUIBase
	{
		/// <summary>
		/// Получает запись документа по ID.
		/// </summary>
		private DataRow GetDocumentRow(int id)
		{
			DataRow[] rows = dtTemplates.Select(String.Format("ID = {0}", id));
			if (rows.Length > 0)
				return rows[0];
			return null;
		}

		/// <summary>
		/// Возвращает имя файла документа.
		/// </summary>
		internal string GetDocumentFileName(int id)
		{
			DataRow row = GetDocumentRow(id);
			if (row != null)
			{
				string docName = row[TemplateFields.Name].ToString();
				docName += Path.GetExtension(row[TemplateFields.DocumentFileName].ToString());
				return docName;
			}
			return string.Empty;
		}

		/// <summary>
		/// Возвращает полный путь к файлу документа.
		/// </summary>
		internal string GetDocumentFullFilePath(int id)
		{
			return TemplatesDocumentsHelper.GetFullFileName(id, GetDocumentFileName(id));
		}

		/// <summary>
		/// Получение документа по ID.
		/// </summary>
		internal byte[] GetDocument(int templateID)
		{
			DataRow[] docRows = dtTemplates.Select(string.Format("ID = {0}", templateID));
			byte[] document;
			if (docRows.Length == 0)
			{
				DataRow row = dtTemplates.NewRow();
				row[TemplateFields.ID] = templateID;
				document = DocumentsHelper.DecompressFile(Repository.GetDocument(templateID));
				row[TemplateFields.Document] = document;
			}
			else
			{
				if (docRows[0][TemplateFields.Document] == DBNull.Value)
				{
					byte[] data = Repository.GetDocument(templateID);
					document = data != null ? DocumentsHelper.DecompressFile(data) : null;
				}
				else
					document = DocumentsHelper.DecompressFile((byte[])docRows[0][TemplateFields.Document]);
			}
			return document;
		}

		/// <summary>
		/// Сохраняет документ во временный каталог.
		/// </summary>
		internal void SaveDocumentToTempFolder(int id)
		{
			string filePath = GetDocumentFullFilePath(id);
			byte[] document = GetDocument(id);
			
			if (document == null)
				throw new Exception("У шаблона нет прикрепленного документа.");

			TemplatesDocumentsHelper.SaveDocument(filePath, document);
		}

		internal TemplateDocumentTypes GetTemplateType(string fileExt)
		{
			string str = fileExt.ToLower();
			switch (str)
			{
				case ".xls":
					return TemplateDocumentTypes.MSExcel;
				case ".doc":
					return TemplateDocumentTypes.MSWord;
				case ".dot":
					return TemplateDocumentTypes.MSWordTemplate;
				case ".xlt":
					return TemplateDocumentTypes.MSExcelTemplate;
				case ".exd":
					return TemplateDocumentTypes.MDXExpert;
				case ".exd3":
					return TemplateDocumentTypes.MDXExpert3;
				default:
					return TemplateDocumentTypes.Arbitrary;
			}
		}

		/// <summary>
		/// Возвращает тип документа используемый по умолчанию.
		/// </summary>
		internal virtual TemplateDocumentTypes DefaultDocumentTypes
		{
			get { return TemplateDocumentTypes.Arbitrary; }
		}
	}
}
