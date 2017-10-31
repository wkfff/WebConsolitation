using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using Krista.FM.Common.Xml;
using Krista.FM.Server.Common;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Classes
{
    /// <summary>
    /// Коллекция пакетов
    /// </summary>
    internal class PackageCollection : MajorObjecModifiableCollection<string, IPackage>, IPackageCollection
    {
        /// <summary>
        /// Коллекция пакетов.
        /// </summary>
        /// <param name="owner">Родительский пакет.</param>
        /// <param name="state">Состояние объекта.</param>
        public PackageCollection(ServerSideObject owner, ServerSideObjectStates state)
            : base(owner, state)
        {
        }

        public void Initialize(XmlNodeList xmlPackages)
        {
            foreach (XmlNode xmlPackage in xmlPackages)
            {
                string name;
                string privatePath = String.Empty;
                string config;
                string objectKey = Guid.Empty.ToString();

                if (xmlPackage.Attributes["objectKey"] != null)
                {
                    objectKey = xmlPackage.Attributes["objectKey"].Value;
                }

                if (xmlPackage.Attributes["name"] != null)
                {
                    name = xmlPackage.Attributes["name"].Value;
                    config = xmlPackage.OuterXml;
                }
                else if (xmlPackage.Attributes["privatePath"] != null)
                {
                    privatePath = xmlPackage.Attributes["privatePath"].Value;
                    name = Path.GetFileNameWithoutExtension(privatePath);
                    string packageFileName = String.Format("{0}\\{1}", ((Package)Owner).GetDir(), privatePath);
                    string errorMessage;
                    XmlDocument packageDoc = Validator.LoadValidated(
                        packageFileName,
                        "ServerConfiguration.xsd", "xmluml", out errorMessage);
                    if (packageDoc == null)
                        throw new Exception(errorMessage);
                    config = packageDoc.DocumentElement.InnerXml;
                    
                    XmlNode xmlPackageName = packageDoc.SelectSingleNode("/ServerConfiguration/Package/@name");
                    if (xmlPackageName != null)
                        name = xmlPackageName.Value;

                    XmlNode xmlPackageKey = packageDoc.SelectSingleNode("/ServerConfiguration/Package/@objectKey");
                    if (xmlPackageKey != null)
                        objectKey = xmlPackageKey.Value;
                }
                else
                    throw new Exception("У пакета отсутствует обязательный атрибут name или privatePath");

                Package package = Package.CreatePackage(objectKey, Owner, -1, name, SchemeClass.Enclose2DatabaseConfiguration(config), ServerSideObjectStates.New);
                package.PrivatePath = privatePath;
                this.Add(KeyIdentifiedObject.GetKey(package.ObjectKey, package.Name), package);
                package.Initialize();
                package.DbObjectState = DBObjectStateTypes.New;
            }
        }

        public void Initialize(string configFile, Package rootPackage)
        {
            string errMsg;

            // Открываем и разбираем файл настроек схемы
            XmlDocument xmlDoc = Validator.LoadValidated(configFile, "ServerConfiguration.xsd", "xmluml", out errMsg);
            if (xmlDoc == null)
                throw new Exception(String.Format("Can't load scheme configuration file: {0}{1}Error: {2}",
                    configFile, Environment.NewLine, errMsg));

            // Обрабатываем список пакетов схемы
            Initialize(xmlDoc.SelectNodes("/ServerConfiguration/Package/Packages/Package"));
        }

        protected override void CloneItems(DictionaryBase<string, IPackage> clon, bool cloneItems)
        {
            foreach (KeyValuePair<string, IPackage> item in this.list)
            {
                KeyValuePair<string, IPackage> newItem;
                if (!((Package)item.Value).IsEndPoint)
                    newItem = new KeyValuePair<string, IPackage>(item.Key, cloneItems ? (IPackage)item.Value.Clone() : item.Value);
                else
                    newItem = new KeyValuePair<string, IPackage>(item.Key, item.Value);

                clon.Add(newItem);
            }
        }

        public override ServerSideObjectStates State
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return base.State; }
            set
            {
                if (base.State != value)
                {
                    state = value;
                    foreach (Package item in Values)
                    {
                        if (String.IsNullOrEmpty(item.PrivatePath))
                        {
                            item.State = value;
                        }
                    }
                }
            }
        }

        public override IServerSideObject Lock()
        {
            Package clonePackage = (Package)Owner.Lock();
            return (ServerSideObject)clonePackage.Packages;
        }

        public override IPackage CreateItem()
        {
            Package package = Package.CreatePackage(Guid.NewGuid().ToString(), Owner, -1, String.Format("Новый пакет {0}", this.Count + 1), SchemeClass.Enclose2DatabaseConfiguration("<Package></Package>"), ServerSideObjectStates.New);
            package.DbObjectState = DBObjectStateTypes.New;
            package.Initialize();
            package.NeedFlash();
            this.Add(package.ObjectKey, package);
            return package;
        }

        /// <summary>
        /// Ищет объект в коллекции по ключу (ключем может быть как GUID так и FullName).
        /// </summary>
        /// <param name="item">Объект для поиска.</param>
        /// <returns>Если объект найден, то возвращает true.</returns>
        protected override IPackage ContainsObject(KeyValuePair<string, IPackage> item)
        {
            string key = item.Key;

            try
            {
                new Guid(item.Key);
                if (!this.ContainsKey(item.Key))
                {
                    foreach (IPackage package in this.Values)
                    {
                        if (package.Name == item.Value.Name)
                            return package;
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
