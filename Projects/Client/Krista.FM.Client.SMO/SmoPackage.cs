using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO
{
    public class SmoPackage : SmoCommonDBObject, IPackage
    {
        public SmoPackage(IPackage serverControl)
            : base(serverControl)
        {
        }

		public SmoPackage(SMOSerializationInfo cache)
            : base(cache)
        {
        }

        public bool Search(SearchParam searchParam, ref DataTable searchTable)
        {
            return ServerControl.Search(searchParam, ref searchTable);
        }

        public new IPackage ServerControl
        {
            get { return (IPackage)serverControl; }
        }

        public new string Name
        {
            get
            {
                return serverControl.Name;
            }
            set
            {
                if (State == ServerSideObjectStates.New)
                    if (ReservedWordsClass.CheckName(value))
                    {
                        PrivatePath = string.Format("{0}.xml", value);
                        serverControl.Name = value;
                    }
            }
        }

        #region IPackage Members

        public string PrivatePath
        {
            get
            {
                if (string.IsNullOrEmpty(ServerControl.PrivatePath))
                {
                    return (string.Format("{0}.xml", serverControl.Name));
                }
                return ServerControl.PrivatePath;
            }
            set
            {
                if (State != ServerSideObjectStates.New)
                    throw new Exception("Путь к файлу пакета можно изменять только у вновь созданных пакетов.");
                ServerControl.PrivatePath = value; CallOnChange();
            }
        }

        public DataTable GetSourcesDependedData(int sID)
        {
        	return ServerControl.GetSourcesDependedData(sID);
        }

        public DataTable GetConflictPackageDependents()
        {
			return ServerControl.GetConflictPackageDependents();
		}

        public List<string> GetDependentsByPackage()
        {
			return ServerControl.GetDependentsByPackage();
		}

        public List<IEntity> GetSortEntitiesByPackage()
        {
			return ServerControl.GetSortEntitiesByPackage();
		}

        public override string Semantic
        {
            get { return base.Semantic; }
            set { base.Semantic = value; }
        }

        public override string FullName
        {
            get { return base.FullName; }
        }

        public override string FullDBName
        {
            get { return base.FullDBName; }
        }

        public IPackageCollection Packages
        {
            get { return ServerControl.Packages; }
        }

        public IEntityCollection<IEntity> Classes
        {
            get { return ServerControl.Classes; }
        }

        public IEntityAssociationCollection Associations
        {
            get { return ServerControl.Associations; }
        }

        public IDocumentCollection Documents
        {
            get { return ServerControl.Documents; }
        }

		private Type GetItemValueSmoObjectType(object obj)
		{
            if (obj is IBridgeClassifier)
                return typeof(SmoBridgeClassifier);
            else if (obj is IVariantDataClassifier)
                return typeof(SmoVariantDataClassifier);
            else if (obj is IFactTable)
                return typeof(SmoFactTable);
            else if (obj is IBridgeAssociationReport)
                return typeof (SmoBridgeAssociationReport);
            else if (obj is IBridgeAssociation)
                return typeof(SmoBridgeAssociation);
            else if (obj is IAssociation)
                return typeof(SmoAssociation);
            else if (obj is IEntityAssociation)
                return typeof(SmoAssociation);
            else if (obj is IClassifier)
                return typeof(SmoClassifier);
            else if (obj is IEntity)
                return typeof(SmoEntity);
            else
                return typeof(SmoClassifier);
		}

		public IEntity FindEntityByName(string name)
        {
        	IEntity entity = ServerControl.FindEntityByName(name);
            // если не нашли объект, возвращаем пустое значение 
            if (entity == null)
                return null;
			return
				cached
					? (IEntity)SmoObjectsCache.GetSmoObject(GetItemValueSmoObjectType(entity), entity)
					: entity;
        }

        public IEntityAssociation FindAssociationByName(string name)
        {
			IEntityAssociation entity = ServerControl.FindAssociationByName(name);
            // если не нашли объект, возвращаем пустое значение 
            if (entity == null)
                return null;
            if (entity is IBridgeAssociationReport)
                return entity;
			return
				cached
					? (IEntityAssociation)SmoObjectsCache.GetSmoObject(GetItemValueSmoObjectType(entity), entity)
					: entity;
        }

        public override string Caption
        {
            get
            {
                return base.Caption;
            }
            set
            {
                base.Caption = value;
            }
        }

        public VSSFileStatus IsCheckedOut
        {
            get { return ServerControl.IsCheckedOut; }
        }

        public DataTable Validate()
        {
            return ServerControl.Validate();
        }

        public void SaveToDisk()
        {
            ServerControl.SaveToDisk();
        }

        #endregion

        #region Общие для пакета виды источников

        /// <summary>
        /// Общие для пакета виды источников, это свойство не храним, берем из первого заплненного
        /// классификатора, делящегося по источникам. В большинстве случаев этот параметр будет
        /// устанавливаться для пакета, и меняться для каждого отдельного объекта не будет
        /// </summary>
        public virtual string DataSourceKinds
        {
            get
            {
                foreach (IEntity entity in Classes.Values)
                {
                    if ((entity is IDataSourceDividedClass && ((IDataSourceDividedClass)entity).IsDivided) ||
                        entity is IFactTable)
                        if (!String.IsNullOrEmpty(((IDataSourceDividedClass)entity).DataSourceKinds))
                            return ((IDataSourceDividedClass)entity).DataSourceKinds;
                }
                return String.Empty;
            }
            set
            {
                foreach (IEntity entity in Classes.Values)
                {
                    if ((entity is IDataSourceDividedClass && ((IDataSourceDividedClass)entity).IsDivided) ||
                        entity is IFactTable)
                        ((IDataSourceDividedClass)entity).DataSourceKinds = value;
                }

                CallOnChange();
            }
        }

        #endregion Общие для пакета виды источников
    }
}
