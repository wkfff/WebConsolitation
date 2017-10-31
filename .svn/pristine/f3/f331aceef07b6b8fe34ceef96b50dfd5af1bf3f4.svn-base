using System;
using System.Diagnostics;

using Krista.FM.Server.Common;

using Krista.FM.ServerLibrary;
using System.Collections.Generic;


namespace Krista.FM.Server.Scheme.Classes
{
    /// <summary>
    /// Коллекция классификаторов
    /// </summary>
    internal class DocumentCollection : MajorObjecModifiableCollection<string, IDocument>, IDocumentCollection
    {
        public DocumentCollection(Package owner, ServerSideObjectStates state)
            : base(owner, state)
        {
        }

        public override IServerSideObject Lock()
        {
            Package clonePackage = (Package)Owner.Lock();
            return (ServerSideObject)clonePackage.Documents;
        }

		public override IDocument CreateItem()
		{
			return CreateItem(DocumentTypes.Diagram);
		}

    	public IDocument CreateItem(DocumentTypes documentType)
        {
            string name = String.Format("Новый документ {0}", Count + 1);
			Document document = new Document(Guid.NewGuid().ToString(), Owner, -1, name, documentType, ServerSideObjectStates.New);
            Trace.TraceEvent(TraceEventType.Verbose, "Инициализация документа {0}", document.FullName);
            document.DbObjectState = DBObjectStateTypes.New;
			document.Configuration = String.Format(
				"<?xml version=\"1.0\" encoding=\"windows-1251\"?><DatabaseConfiguration><Document objectKey=\"{0}\" type=\"{1}\" name=\"{2}\"><Diagram><Symbols/></Diagram></Document></DatabaseConfiguration>", 
				document.ObjectKey,
				Enum.GetName(typeof (DocumentTypes), documentType),
				name);
            this.Add(document.ObjectKey, document);
            return document;
        }

        /// <summary>
        /// Ищет объект в коллекции по ключу (ключем может быть как GUID так и FullName).
        /// </summary>
        /// <param name="item">Объект для поиска.</param>
        /// <returns>Если объект найден, то возвращает true.</returns>
        protected override IDocument ContainsObject(KeyValuePair<string, IDocument> item)
        {
            string key = item.Key;

            try
            {
                new Guid(item.Key);
                if (!this.ContainsKey(item.Key))
                {
                    foreach (IDocument document in this.Values)
                    {
                        if (document.Name == item.Value.Name)
                            return document;
                    }
                }
            }
            catch (FormatException)
            {
                key = item.Key;
            }

            if (this.ContainsKey(key))
                return this[key];
            else
            {
                return null;
            }
        }
    }
}
