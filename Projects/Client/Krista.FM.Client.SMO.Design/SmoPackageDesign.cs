using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
using Krista.FM.Client.Design.Editors;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    public class SmoPackageDesign : SmoCommonDBObjectDesign, IPackage
    {
        public SmoPackageDesign(IPackage serverControl)
            : base(serverControl)
        {
        }

        public bool Search(SearchParam searchParam, ref DataTable searchTable)
        {
            return ServerControl.Search(searchParam, ref searchTable);
        }

        [Browsable(false)]
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

        [Category("CommonObject.Naming")]
        [DisplayName(@"Файл пакета (PrivatePath)")]
        [Description("Путь к файлу пакета относительно каталога, в котором находится родительскай объект. Если имя файла не указано, то пакет будет сохранен в файле родительскаго пакета.")]
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

        [Browsable(false)]
        public DataTable GetSourcesDependedData(int sID)
        {
            return ServerControl.GetSourcesDependedData(sID);
        }

        [Browsable(false)]
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

        [Browsable(false)]
        public override string Semantic
        {
            get { return base.Semantic; }
            set { base.Semantic = value; }
        }

        [Browsable(false)]
        public override string FullName
        {
            get { return base.FullName; }
        }

        [Browsable(false)]
        public override string FullDBName
        {
            get { return base.FullDBName; }
        }

        [Browsable(false)]
        public IPackageCollection Packages
        {
            get { return ServerControl.Packages; }
        }

        [Browsable(false)]
        public IEntityCollection<IEntity> Classes
        {
            get { return ServerControl.Classes; }
        }

        [Browsable(false)]
        public IEntityAssociationCollection Associations
        {
            get { return ServerControl.Associations; }
        }

        [Browsable(false)]
        public IDocumentCollection Documents
        {
            get { return ServerControl.Documents; }
        }

        public IEntity FindEntityByName(string name)
        {
            IEntity entity = ServerControl.FindEntityByName(name);
            // если не нашли объект, возвращаем пустое значение 
            if (entity == null)
                return null;
            return
                 entity;
        }

        public IEntityAssociation FindAssociationByName(string name)
        {
            IEntityAssociation entity = ServerControl.FindAssociationByName(name);
            // если не нашли объект, возвращаем пустое значение 
            if (entity == null)
                return null;
            return
                entity;
        }

        [Browsable(false)]
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
        [DisplayName(@"Коллекция общих видов источника для пакета")]
        [Description("Коллекция общих видов источника для пакета, по которым делятся данные. Свойство также можно отдельно установить для каждого объекта, делящегося по источникам")]
        [Editor(typeof(DataSourcesEditor), typeof(UITypeEditor))]
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
