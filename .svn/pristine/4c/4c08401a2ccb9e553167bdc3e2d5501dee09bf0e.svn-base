using System;
using System.Xml;

using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.Modifications;

using Krista.FM.ServerLibrary;
using Krista.FM.Common.Xml;
using System.Data;


namespace Krista.FM.Server.Scheme.Classes
{
    /// <summary>
    /// Класс определяет доступ к документу привязанному к пакету
    /// </summary>
    internal class Document : CommonDBObject, IDocument
    {
        #region Поля

        /// <summary>
        /// Тип документа
        /// </summary>
        readonly DocumentTypes documentType;

        #endregion Поля

        #region Конструктор

        /// <summary>
        /// Класс определяет доступ к документу привязанному к пакету
        /// </summary>
        /// <param name="key"></param>
        /// <param name="owner"></param>
        /// <param name="name">Наименование документа</param>
        /// <param name="documentType"></param>
        /// <param name="state"></param>
        public Document(string key, ServerSideObject owner, string name, DocumentTypes documentType, ServerSideObjectStates state)
            : base(key, owner, documentType.ToString(), name, state)
        {
            this.documentType = documentType;
        }

        /// <summary>
        /// Класс определяет доступ к документу привязанному к пакету
        /// Данный конструктор вызывается при создании объекта из базы данных.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="owner">Родительский пакет.</param>
        /// <param name="ID">ID пакета.</param>
        /// <param name="name">Наименование.</param>
        /// <param name="documentType">Тип документа.</param>
        /// <param name="state"></param>
        public Document(string key, ServerSideObject owner, int ID, string name, DocumentTypes documentType, ServerSideObjectStates state)
            : this(key, owner, name, documentType, state)
        {
            this.ID = ID;
        }

        #endregion Конструктор

        #region ServerSideObject

        /// <summary>
        /// Возвращает экземпляр объекта с которым должен работать текущий пользователь
        /// </summary>
        /// <returns>Экземпляр объекта</returns>
        protected new Document Instance
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return (Document)GetInstance(); }
        }

        /// <summary>
        /// Возвращает экземпляр объекта с которым должен работать текущий пользователь, для установки значений свойств
        /// </summary>
        /// <returns>Экземпляр объекта</returns>
        protected new Document SetInstance
        {
            get
            {
                if (SetterMustUseClone())
                    return (Document)CloneObject;
                else
                    return this;
            }
        }

        public override IServerSideObject Lock()
        {
            Package clonePackage = (Package)Owner.Lock();
            return (ServerSideObject)clonePackage.Documents[this.ObjectKey];
        }

        #endregion ServerSideObject
        
        /// <summary>
        /// Создание записи в метаданных
        /// </summary>
        private void InsertMetaData(ModificationContext context)
        {
            Database db = context.Database;

            int? PackageID = Parent.ID;
            if (PackageID == null)
                throw new Exception(String.Format("У объекта {0} указан несуществующий пакет.", FullName));

            // Проверка на уникальность
            long count = Convert.ToInt64(db.ExecQuery("select Count(*) from MetaDocuments where Name = ?",
                QueryResultTypes.Scalar,
                db.CreateParameter("Name", this.Name)));
            if (count > 0)
                throw new Exception("Документ с именем " + this.FullName + " уже создан в вазе данных.");

            // получаем значение генератора
            if (this.ID <= 0)
                this.ID = db.GetGenerator("g_MetaDocuments");

            db.ExecQuery("insert into MetaDocuments (ID, ObjectKey, Name, DocType, Configuration, RefPackages) values (?, ?, ?, ?, ?, ?)",
                QueryResultTypes.NonQuery,
                db.CreateParameter("ID", this.ID),
                db.CreateParameter("ObjectKey", this.ObjectKey),
                db.CreateParameter("Name", this.Name),
                db.CreateParameter("DocType", (int)this.documentType),
                db.CreateParameter("Config", this.Configuration, System.Data.DbType.AnsiString),
                db.CreateParameter("RefPackages", PackageID));
        }

        /// <summary>
        /// Удаляет из каталога метаданных информацию о пакете
        /// </summary>
        private void RemoveMetaData(ModificationContext context)
        {
            Database db = context.Database;

            db.ExecQuery("delete from MetaDocuments where ID = ?", QueryResultTypes.NonQuery,
                db.CreateParameter("ID", this.ID));
        }

        /// <summary>
        /// Сохранение XML-конфигурации в базе данных
        /// </summary>
        internal override void SaveConfigurationToDatabase(ModificationContext context)
        {
            IDatabase db = context.Database;

            string confXmlString = Configuration;
            confXmlString = confXmlString.Replace("objectKey=\"00000000-0000-0000-0000-000000000000\"", String.Format("objectKey=\"{0}\"", ObjectKey));
            confXmlString = confXmlString.Replace("<DatabaseConfiguration xmlns=\"xmluml\">", "<DatabaseConfiguration>");

            // вставляем запись в каталог метаданных
            db.ExecQuery(
                "update MetaDocuments set Configuration = ?, ObjectKey = ? where ID = ?",
                QueryResultTypes.NonQuery,
                db.CreateParameter("Config", confXmlString, System.Data.DbType.AnsiString),
                db.CreateParameter("ObjectKey", ObjectKey),
                db.CreateParameter("ID", this.ID));

            this.Configuration = confXmlString;
        }

        internal override void Save2Xml(XmlNode node)
        {
            base.Save2Xml(node);

            if (String.IsNullOrEmpty(Configuration))
                return;
            XmlDocument doc = new XmlDocument();
            doc.InnerXml = Configuration;
            string tagName;
			tagName = Enum.GetName(typeof (DocumentTypes), documentType);

            string xPath = "/DatabaseConfiguration/Document/Diagram";

            XmlHelper.SetAttribute(node, "objectKey", ObjectKey);
            XmlHelper.SetAttribute(node, "type", tagName);
            XmlHelper.SetAttribute(node, "name", Name);

            node.InnerXml = doc.SelectSingleNode(xPath).OuterXml;
        }

        internal override void Save2XmlDocumentation(XmlNode node)
        {
            Save2Xml(node);
            
            // Специфичные для документации свойства
            
        } 

        /// <summary>
        /// Создает новый документ в базе данных
        /// </summary>
        /// <param name="context"></param>
        internal override void Create(ModificationContext context)
        {
            bool updatingState = InUpdating;
            InUpdating = true;
            try
            {
                Trace.TraceInformation("Создание {0} {1}", this, DateTime.Now);

                base.Create(context);

                InsertMetaData(context);

                DbObjectState = DBObjectStateTypes.InDatabase;

                if (!Parent.Documents.ContainsKey(GetKey(this.ObjectKey, this.FullName)))
                    Parent.Documents.Add(GetKey(this.ObjectKey, this.FullName), this);
            }
            finally
            {
                InUpdating = updatingState;
            }
        }

        /// <summary>
        /// Удаление объекта из базы и метаданных
        /// </summary>
        internal override void Drop(ModificationContext context)
        {
            bool updatingState = InUpdating;
            InUpdating = true;
            try
            {
                Trace.WriteLine(String.Format("Удаление документа {0}", this));
                
                RemoveMetaData(context);

                if (Parent.Documents.ContainsKey(this.ObjectKey))
                    Parent.Documents.Remove(this.ObjectKey);

                this.DbObjectState = DBObjectStateTypes.Unknown;

                base.Drop(context);
            }
            finally
            {
                InUpdating = updatingState;
            }
        }

        /// <summary>
        /// Устанавливает английское наименование
        /// </summary>
        /// <param name="oldValue">Старое наименование</param>
        /// <param name="value">Новое наименование</param>
        protected override void SetFullName(string oldValue, string value)
        {
            if (State == ServerSideObjectStates.New)
            {
                Package package = (Package)Owner;

                Document newObject = Instance;
                Document oldObject = package.Documents[oldValue] as Document;
                try
                {
                    package.Documents.Remove(oldValue);
                    package.Documents.Add(newObject.FullName, newObject);
                }
                catch (Exception e)
                {
                    package.Documents.Add(oldValue, oldObject);
                    throw new Exception(e.Message, e);
                }
            }
        }

        public override IModificationItem GetChanges()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Формирует список отличий (операций изменения) текущего объекта от toObject
        /// </summary>
        /// <param name="toObject">Объект с которым будет производиться сравнение</param>
        /// <returns>список отличий (операций изменения)</returns>
        public override IModificationItem GetChanges(IModifiable toObject)
        {
            UpdateMajorObjectModificationItem root = (UpdateMajorObjectModificationItem)base.GetChanges(toObject);

            if (State == ServerSideObjectStates.New)
                return root;

            Document toDocument = (Document)toObject;

            //
            // Основные свойства
            //

            if (this.Semantic != toDocument.Semantic)
                throw new Exception(string.Format("У документа {0} свойство Semantic недоступно для изменения.", this.FullName));

            if (this.Name != toDocument.Name)
                throw new Exception(string.Format("У документа {0} свойство Name недоступно для изменения.", this.FullName));

            if (this.Configuration != toDocument.Configuration)
            {
                ModificationItem item = new PropertyModificationItem("Configuration", this.Configuration, toDocument.Configuration, root);
                root.Items.Add(item.Key, item);
            }

            return root;
        }

#warning Удалить метод после выпуска версии 2.4.1
        internal void ConvertDiagramToGuidKeys(IDatabase db)
        {
            try
            {
                if (documentType == DocumentTypes.Diagram)
                {
                    Trace.TraceInformation(this.Name);

                    XmlDocument doc = Validator.LoadDocument(Configuration);

                    foreach (XmlNode node in doc.SelectNodes("//Symbols/*/Object"))
                    {
                        string key = SchemeClass.Instance.GetGuidPathByPathName(node.InnerText);
                        node.InnerText = key;
                    }

                    foreach (XmlNode node in doc.SelectNodes("//Symbols/*/AssociateKey"))
                    {
                        string key = SchemeClass.Instance.GetGuidPathByPathName(node.InnerText);
                        node.InnerText = key;
                    }


                    string confXmlString = doc.OuterXml;
                    confXmlString =
                        confXmlString.Replace("<DatabaseConfiguration xmlns=\"xmluml\">", "<DatabaseConfiguration>");

                    // вставляем запись в каталог метаданных
                    db.ExecQuery(
                        "update MetaDocuments set Configuration = ?, ObjectKey = ? where ID = ?",
                        QueryResultTypes.NonQuery,
                        db.CreateParameter("Config", confXmlString, System.Data.DbType.AnsiString),
                        db.CreateParameter("ObjectKey", ObjectKey),
                        db.CreateParameter("ID", this.ID));

                    this.Configuration = confXmlString;
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(Krista.Diagnostics.KristaDiagnostics.ExpandException(e));
            }
        }

        #region Права

        /// <summary>
        /// Проверяет права на просмотр объекта для текущего пользователя
        /// </summary>
        /// <returns>true - если у пользлвателя есть права на просмотр</returns>
        public override bool CurrentUserCanViewThisObject()
        {
            return true;
        }

        #endregion Права

        #region Свойства

        /// <summary>
        /// Тип документа
        /// </summary>
        public DocumentTypes DocumentType
        {
            get { return documentType; }
        }

        private string ConfigurationWrap
        {
            get { return base.Configuration; }
            set { base.Configuration = value; }
        }

        /// <summary>
        /// Представление документа в виде XML
        /// </summary>
        public override string Configuration
        {
            get
            {
                if (String.IsNullOrEmpty(Instance.ConfigurationWrap))
                    Instance.ConfigurationWrap = GetDocumentConfigurationFromDB();
                return Instance.ConfigurationWrap;
            }
            set 
            {
                SetInstance.ConfigurationWrap = value;
            }
        }

        /// <summary>
        /// Сохранение диаграммы в базу данных
        /// </summary>
        /// <param name="value"></param>
        private void SetDocumentConfigurationToDB(string value)
        {
			using (IDatabase db = SchemeClass.Instance.SchemeDWH.DB)
			{
				db.ExecQuery("update MetaDocuments set Configuration = ? where objectKey = ?",
							 QueryResultTypes.NonQuery,
							 db.CreateParameter("Configuration", value),
							 db.CreateParameter("ObjectKey", ObjectKey));
			}
        }

        /// <summary>
        /// Чтение информаци о диаграмме из базы данных
        /// </summary>
        /// <returns></returns>
        private string GetDocumentConfigurationFromDB()
        {
            IDatabase db = SchemeClass.Instance.SchemeDWH.DB;
            DataTable dt;
            try
            {
                dt =
                    (DataTable)
                    db.ExecQuery("select Configuration from MetaDocuments where objectKey = ?",
                                 QueryResultTypes.DataTable, db.CreateParameter("ObjectKey", ObjectKey));
                if (dt.Rows.Count == 1)
                {
                    return Convert.ToString(dt.Rows[0]["Configuration"]);
                }

                return String.Empty;
            }
            finally
            {
                db.Dispose();
            }
        }

        #endregion Свойства

        [System.Diagnostics.DebuggerStepThrough]
        public override string ToString()
        {
            return String.Format("{0} : {1}", Name, base.ToString());
        }
    }
}
