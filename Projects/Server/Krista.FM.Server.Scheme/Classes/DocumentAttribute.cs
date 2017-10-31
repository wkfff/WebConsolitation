using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Common.Xml;
using Krista.FM.Server.Common;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.Classes
{
    /// <summary>
    /// Атрибут-документ с подчиненными полями.
    /// </summary>
    internal class DocumentAttribute : EntityDataAttribute, IDocumentAttribute
    {
        private readonly EntityDataAttribute[] childAttributes = new EntityDataAttribute[3];

        public DocumentAttribute(string key, string name, ServerSideObject owner, ServerSideObjectStates state)
            : base(key, name, owner, state)
        {
            Initialize();
        }

        public DocumentAttribute(string key, ServerSideObject owner, XmlNode xmlAttribute, ServerSideObjectStates state)
            : base(key, owner, xmlAttribute, state)
        {
            Initialize();
        }

        private void Initialize()
        {
            childAttributes[0] = CreateAttribute(Guid.Empty.ToString(), String.Format(DocumentNameColumnName, Name), Owner, AttributeClass.Regular, this.State);
            childAttributes[0].Type = DataAttributeTypes.dtString;
            childAttributes[0].Size = 255;
            childAttributes[0].Caption = String.Format(DocumentNameColumnCaption, Caption);
            childAttributes[0].Description = childAttributes[0].Name;
            childAttributes[0].Class = DataAttributeClassTypes.System;
            childAttributes[0].Kind = DataAttributeKindTypes.Regular;
            childAttributes[0].Visible = true;
            childAttributes[0].IsReadOnly = false;
            childAttributes[0].IsNullable = this.IsNullable;

            childAttributes[1] = CreateAttribute(Guid.Empty.ToString(), String.Format(DocumentTypeColumnName, Name), Owner, AttributeClass.Regular, this.State);
            childAttributes[1].Type = DataAttributeTypes.dtInteger;
            childAttributes[1].Size = 5;
            childAttributes[1].Caption = String.Format(DocumentTypeColumnCaption, Caption); 
            childAttributes[1].Description = childAttributes[1].Name;
            childAttributes[1].Class = DataAttributeClassTypes.System;
            childAttributes[1].Kind = DataAttributeKindTypes.Regular;
            childAttributes[1].Visible = false;
            childAttributes[1].IsReadOnly = false;
            childAttributes[1].IsNullable = this.IsNullable;

            childAttributes[2] = CreateAttribute(Guid.Empty.ToString(), String.Format(DocumentFileNameColumnName, Name), Owner, AttributeClass.Regular, this.State);
            childAttributes[2].Type = DataAttributeTypes.dtString;
            childAttributes[2].Size = 255;
            childAttributes[2].Caption = String.Format(DocumentFileNameColumnCaption, Caption);
            childAttributes[2].Description = childAttributes[2].Name;
            childAttributes[2].Class = DataAttributeClassTypes.System;
            childAttributes[2].Kind = DataAttributeKindTypes.Regular;
            childAttributes[2].Visible = false;
            childAttributes[2].IsReadOnly = true;
            childAttributes[2].IsNullable = this.IsNullable;
        }

        internal override void Save2Xml(XmlNode node)
        {
            base.Save2Xml(node);
            XmlHelper.SetAttribute(node, "isDocumentAttribute", "true");
        }

        internal override void OnAfterAdd()
        {
            ((Entity)Owner).Attributes.Add(childAttributes[0]);
            ((Entity)Owner).Attributes.Add(childAttributes[1]);
            ((Entity)Owner).Attributes.Add(childAttributes[2]);
        } 

        internal override void OnBeforeRemove()
        {
            ((Entity)Owner).Attributes.Remove(GetKey(childAttributes[0].ObjectKey, childAttributes[0].Name));
            ((Entity)Owner).Attributes.Remove(GetKey(childAttributes[1].ObjectKey, childAttributes[1].Name));
            ((Entity)Owner).Attributes.Remove(GetKey(childAttributes[2].ObjectKey, childAttributes[2].Name));
        }

        internal override List<string> AddScript(Entity entity, bool withNullClause, bool generateDependendScripts)
        {
            List<string> script = base.AddScript(entity, withNullClause, generateDependendScripts);

            script.AddRange(childAttributes[0].AddScript(entity, withNullClause, generateDependendScripts));
            script.AddRange(childAttributes[1].AddScript(entity, withNullClause, generateDependendScripts));
            script.AddRange(childAttributes[2].AddScript(entity, withNullClause, generateDependendScripts));

            return script;
        }

        internal override List<string> DropScript(Entity entity)
        {
            List<string> script = base.DropScript(entity);
            
            script.AddRange(ScriptingEngine.DropScript(childAttributes[0], entity));
            script.AddRange(ScriptingEngine.DropScript(childAttributes[1], entity));
            script.AddRange(ScriptingEngine.DropScript(childAttributes[2], entity));

            return script;
        }

        public override ServerSideObjectStates State
        {
            [DebuggerStepThrough]
            get { return base.State; }
            set
            {
                base.State = value;
                childAttributes[0].State = value;
                childAttributes[1].State = value;
                childAttributes[2].State = value;
            }
        }

        /// <summary>
        /// Имя атрибута в БД
        /// </summary>
        public override string Name
        {
            [DebuggerStepThrough]
            get { return base.Name; }
            set
            {
                base.Name = value;
                childAttributes[2].Name = String.Format(DocumentFileNameColumnName, value);
                childAttributes[1].Name = String.Format(DocumentTypeColumnName, value);
                childAttributes[0].Name = String.Format(DocumentNameColumnName, value);
            }
        }

        public override string Caption
        {
            [DebuggerStepThrough]
            get { return base.Caption; }
            set
            {
                base.Caption = value;
                if (childAttributes[0] != null)
                {
                    LogicalCallContextData userContext = LogicalCallContextData.GetContext();
                    bool needRestoreUserContext = false;
                    try
                    {
                        if ((int)userContext["UserID"] != (int)FixedUsers.FixedUsersIds.System)
                        {
                            needRestoreUserContext = true;
                            SessionContext.SetSystemContext();
                        }
                        childAttributes[2].Caption = String.Format(DocumentFileNameColumnCaption, value);
                        childAttributes[1].Caption = String.Format(DocumentTypeColumnCaption, value);
                        childAttributes[0].Caption = String.Format(DocumentNameColumnCaption, value);
                    }
                    finally
                    {
                        if (needRestoreUserContext)
                        {
                            LogicalCallContextData.SetContext(userContext);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Определяет обязательность значения.
        /// </summary>
        public override bool IsNullable
        {
            [DebuggerStepThrough]
            get { return base.IsNullable; }
            set
            {
                base.IsNullable = value;

                if (childAttributes[0] != null)
                {
                    LogicalCallContextData userContext = LogicalCallContextData.GetContext();
                    bool needRestoreUserContext = false;
                    try
                    {
                        if ((int)userContext["UserID"] != (int)FixedUsers.FixedUsersIds.System)
                        {
                            needRestoreUserContext = true;
                            SessionContext.SetSystemContext();
                        }

                        childAttributes[0].IsNullable = value;
                        childAttributes[1].IsNullable = value;
                        childAttributes[2].IsNullable = value;
                    }
                    finally
                    {
                        if (needRestoreUserContext)
                        {
                            LogicalCallContextData.SetContext(userContext);
                        }
                    }
                }
            }
        }

        #region IDocumentAttribute Members

        /// <summary>
        /// Дополнительный атрибут "Наименование".
        /// </summary>
        public IDataAttribute DocumentName
        {
            get { return childAttributes[0]; }
        }

        /// <summary>
        /// Дополнительный атрибут "Тип документа".
        /// </summary>
        public IDataAttribute DocumentType
        {
            get { return childAttributes[1]; }
        }

        /// <summary>
        /// Дополнительный атрибут "Имя файла".
        /// </summary>
        public IDataAttribute DocumentFileName
        {
            get { return childAttributes[2]; }
        }

        /// <summary>
        /// Сохраняет данные документа в БД.
        /// </summary>
        /// <param name="documentData">Документа.</param>
        /// <param name="tableName">Имя таблицы.</param>
        /// <param name="rowID">ID записи.</param>
        public void SaveDocumentDataToDataBase(byte[] documentData, string tableName, int rowID)
        {
            using (IDatabase db = SchemeClass.Instance.SchemeDWH.DB)
            {
                string updateDocumentQuery = string.Format("update {0} set {1} = ? where ID = ?",
                    tableName, Name);
                IDbDataParameter[] queryParams = new IDbDataParameter[2];
                queryParams[0] = db.CreateBlobParameter(Name, ParameterDirection.Input, documentData);
                queryParams[1] = db.CreateParameter("ID", rowID);
                db.ExecQuery(updateDocumentQuery, QueryResultTypes.NonQuery, queryParams);
            }
        }

        /// <summary>
        /// Выбирает данные документа из БД.
        /// </summary>
        /// <param name="tableName">Имя таблицы.</param>
        /// <param name="rowID">ID записи.</param>
        /// <returns>Документ.</returns>
        public byte[] GetDocumentDataFromDataBase(string tableName, int rowID)
        {
            using (IDatabase db = SchemeClass.Instance.SchemeDWH.DB)
            {
                string getDocumentQuery = string.Format("select {0} from {1} where ID = ?",
                    Name, tableName);
                return (byte[])db.ExecQuery(getDocumentQuery, QueryResultTypes.Scalar,
                    db.CreateParameter("ID", rowID));
            }
        }

        #endregion

        /// <summary>
        /// Снимает обязательность для атрибута.
        /// </summary>
        /// <param name="entity">Сущность.</param>
        internal override List<string> UpdateSetNullable(Entity entity)
        {
            List<string> script = base.UpdateSetNullable(entity);
            script.AddRange(childAttributes[0].UpdateSetNullable(entity));
            script.AddRange(childAttributes[1].UpdateSetNullable(entity));
            script.AddRange(childAttributes[2].UpdateSetNullable(entity));
            return script;
        }

        /// <summary>
        /// Устанавливает обязательность для атрибута.
        /// </summary>
        /// <param name="entity">Сущность.</param>
        internal override List<string> UpdateSetNotNull(Entity entity)
        {
            List<string> script = base.UpdateSetNotNull(entity);
            script.AddRange(childAttributes[0].UpdateSetNotNull(entity));
            script.AddRange(childAttributes[1].UpdateSetNotNull(entity));
            script.AddRange(childAttributes[2].UpdateSetNotNull(entity));
            return script;
        }
    }
}
