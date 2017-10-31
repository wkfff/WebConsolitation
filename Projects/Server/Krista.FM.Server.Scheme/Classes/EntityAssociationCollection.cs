using System;
using System.Collections.Generic;
using System.Data;

using Krista.FM.Server.Common;

using Krista.FM.ServerLibrary;
using Krista.FM.Common;

namespace Krista.FM.Server.Scheme.Classes
{
    /// <summary>
    /// Коллекция ассоциаций
    /// </summary>
    internal class EntityAssociationCollection : MajorObjecModifiableCollection<string, IEntityAssociation>, IEntityAssociationCollection
    {
        public EntityAssociationCollection(ServerSideObject owner, ServerSideObjectStates state)
            : base(owner, state)
        {
        }

        public override IServerSideObject Lock()
        {
            Package clonePackage = (Package)Owner.Lock();
            return (ServerSideObject)clonePackage.Associations;
        }

        #region IEntityAssociationCollection Members

        public IEntityAssociation CreateItem(IEntity roleA, IEntity roleB, AssociationClassTypes associationClassType)
        {
           if (PackageDependents.GetInnerDependents(roleB.ParentPackage).Contains(roleA.ParentPackage.Name)
                && !roleB.ParentPackage.Name.Equals(roleA.ParentPackage.Name))
                throw new Exception(
                    string.Format(
                        "Невозможно создать ассоциацию между объектами {0} и {1}, поскольку уже существует связь между пакетами {2} и {3}",
                        roleA.FullCaption, roleB.FullCaption, roleB.ParentPackage.Name, roleA.ParentPackage.Name));

            EntityAssociation entityAssociation;

            associationClassType = SchemeClass.EntityAssociationFactory.GetAssociationClassType(roleA, roleB, associationClassType);

            string newObjectKey = Guid.NewGuid().ToString();
            string refFieldName = GetNewName(newObjectKey);

            /*
            if (roleB.Semantic == "Date" && roleB.ClassType == ClassTypes.clsFixedClassifier)
                refFieldName = String.Format("Ref{0}", roleB.Name);
            else if (roleB.ClassType == ClassTypes.clsDataClassifier || roleB.ClassType == ClassTypes.clsFixedClassifier)
            {
                refFieldName = String.Format("Ref{0}", roleB.Semantic);
                if (DataAttributeCollection.GetAttributeByKeyName(roleA.Attributes, refFieldName, refFieldName) != null)
                    refFieldName = String.Format("Ref{0}{1}", roleB.Semantic, roleB.Name);
            }
            else if (roleB.ClassType == ClassTypes.clsBridgeClassifier)
                refFieldName = String.Format("Ref{0}{1}", roleB.Semantic, roleB.Name);
             */

            if (!Owner.IsLocked)
                Owner.Lock();

            entityAssociation = (EntityAssociation)SchemeClass.EntityAssociationFactory.CreateAssociation(
                newObjectKey,
                Owner, -1,
                roleA.Semantic,
                String.Format("{0}.{1}", roleA.Name, refFieldName),
                associationClassType,
                roleA as Entity,
                roleB as Entity,
                String.Empty,
                ServerSideObjectStates.New);

            entityAssociation.DbObjectState = DBObjectStateTypes.New;
            entityAssociation.Caption = roleB.FullCaption;
            entityAssociation.Description = String.Format("Ссылка на классификатор \"{0}\".", roleB.OlapName);

            this.Add(entityAssociation.ObjectKey, entityAssociation);

            entityAssociation.LinkObject(String.Empty, String.Empty, String.Empty,
                                         (entityAssociation.AssociationClassType != AssociationClassTypes.MasterDetail),
                                         false);

            return entityAssociation;
        }

        [Obsolete("Использовать метод CreateItem(IEntity roleA, IEntity roleB, AssociationClassTypes associationClassType).")]
        public IEntityAssociation CreateItem(IEntity roleA, IEntity roleB)
        {
            return CreateItem(roleA, roleB, AssociationClassTypes.Link);
        }

        #endregion

        #region ICollection2DataTable Members

        public DataTable GetDataTable()
        {
            DataTable dt = new DataTable(GetType().Name);
			dt.Columns.Add(new DataColumn("FullName", typeof(String)));
			dt.Columns.Add(new DataColumn("DataSemantic", typeof(String)));
			dt.Columns.Add(new DataColumn("DataCaption", typeof(String)));
			dt.Columns.Add(new DataColumn("BridgeSemantic", typeof(String)));
			dt.Columns.Add(new DataColumn("BridgeCaption", typeof(String)));
			dt.Columns.Add(new DataColumn("ObjectKey", typeof(String)));
			dt.Columns.Add(new DataColumn("AssociatedRecordsPercent", typeof(Decimal)));

            foreach (IEntityAssociation item in Values)
            {
                if (item.AssociationClassType != AssociationClassTypes.Bridge && item.AssociationClassType != AssociationClassTypes.BridgeBridge)
                    continue;

                if (((CommonDBObject)item).CurrentUserCanViewThisObject())
                {
                    DataRow row = dt.NewRow();
                    row[0] = KeyIdentifiedObject.GetKey(item.ObjectKey, item.FullName); 
                    IAssociation association = (IAssociation)item;
                    row[1] = ((Entity)association.RoleData).SemanticCaption;
                    row[2] = association.RoleData.Caption;
                    row[3] = ((Entity)association.RoleBridge).SemanticCaption;
                    row[4] = association.RoleBridge.Caption;
                    row[5] = KeyIdentifiedObject.GetKey(item.ObjectKey, item.FullName);
					if (association is BridgeAssociation)
					{
						decimal? percent = ((BridgeAssociation) association).AssociatedRecordsPercent;
						row[6] = percent != null ? (object)Math.Round((decimal)percent, 0) : DBNull.Value;
					}
                    dt.Rows.Add(row);
                }
            }

            return dt;
        }

        #endregion

        /// <summary>
        /// Ищет объект в коллекции по ключу (ключем может быть как GUID так и FullName).
        /// </summary>
        /// <param name="item">Объект для поиска.</param>
        /// <returns>Если объект найден, то возвращает true.</returns>
        protected override IEntityAssociation ContainsObject(KeyValuePair<string, IEntityAssociation> item)
        {
            string key = item.Key;

            try
            {
                new Guid(item.Key);
                if (!this.ContainsKey(item.Key))
                {
                    foreach (IEntityAssociation entityAssociation in this.Values)
                    {
                        if (entityAssociation.FullName == item.Value.FullName)
                            return entityAssociation;
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
