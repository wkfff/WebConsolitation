using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Krista.FM.Client.Common.Forms;
using Krista.FM.ServerLibrary;
using VSSFileStatus = Krista.FM.ServerLibrary.VSSFileStatus;

namespace Krista.FM.Client.Help
{
    /// <summary>
    /// Проверка на соответствие между объектами на диаграмме и объектами в схеме
    /// </summary>
    public class ValidateDiagramObjects
    {
        private IScheme scheme;
        private Operation operation;
        private IVSSFacade vssUtils;


        public ValidateDiagramObjects(IScheme scheme)
        {
            this.scheme = scheme;
            vssUtils = new Providers.VSS.VSSFacade();

            operation = new Operation();

            try
            {
                operation.Text = "Старт валидации";
                operation.StartOperation();
                ValidatePackage(scheme.RootPackage);
            }
            finally
            {
                operation.StopOperation();
                operation.ReleaseThread();
            }
        }

        private void ValidatePackage(IPackage package)
        {
            bool needChange = false;

            operation.Text = string.Format("Валидация пакета {0}", package.Name);

            foreach (KeyValuePair<string, IPackage> pair in package.Packages)
            {
                ValidatePackage(pair.Value);
            }

            foreach (IDocument document in package.Documents.Values)
            {
                if (ValidateDocument(document))
                    needChange = true;
            }

            if (needChange)
                SaveChange(package);
        }

        private bool ValidateDocument(IDocument document)
        {
            bool needChange = false;

            operation.Text = string.Format("Валидация документа {0}", document.Name);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(document.Configuration);

            XmlNode xmlSymbols = doc.SelectSingleNode("/DatabaseConfiguration/Document/Diagram/Symbols");
            foreach (XmlNode xmlNode in xmlSymbols.ChildNodes)
            {
                if (!LoadSymbol(xmlNode))
                {
                    if (!document.ParentPackage.IsLocked)
                    {
                        document.ParentPackage.Lock();

                        document.Configuration = doc.InnerXml;
                        needChange = true;
                    }
                }
            }

            return needChange;
        }

        /// <summary>
        /// Сохранение изменений  в VSS
        /// </summary>
        private static void SaveChange(IPackage package)
        {
            if (package.IsLocked && Krista.FM.Common.ClientAuthentication.UserID == package.LockedByUserID)
            {
                package.EndEdit("Изменения в диаграмме в связи с переносом классификаторов");

            }
            else
            {
                if (Krista.FM.Common.ClientAuthentication.UserID != package.LockedByUserID)
                {
                    throw new Exception(
                        String.Format("Пакет {0} заблокированр другим пользователем {1}", package.Name,
                                      package.LockedByUserName)); 
                }
            }
        }

        

        private bool LoadSymbol(XmlNode xmlNode)
        {
            switch (xmlNode.Name)
            {
                case "EntitySymbol":
                    return LoadEntitySymbol(xmlNode);
                case "LinkSymbol": 
                    return LoadLinkSymbol(xmlNode);
            }

            return true;
        }

        private bool LoadLinkSymbol(XmlNode node)
        {
            string key = node.SelectSingleNode("Object").InnerText;

            ICommonObject commomObj = scheme.GetObjectByKey(key);

            if (commomObj == null)
            {
                FindLinkObjectByLastGuid(node);
                return false;
            }

            return true;
        }

        private void FindLinkObjectByLastGuid(XmlNode node)
        {
            string key = node.SelectSingleNode("Object").InnerText;

            string[] parts = key.Split(new string[] { ":" }, StringSplitOptions.None);

            string objectGuid = parts[parts.Length - 1];

            IEntityAssociation association = scheme.RootPackage.FindAssociationByName(objectGuid);

            if (association != null)
            {
                node.SelectSingleNode("Object").InnerText = association.Key;
            }
        }

        private bool LoadEntitySymbol(XmlNode node)
        {
            string key = node.SelectSingleNode("Object").InnerText;

            ICommonObject commomObj = scheme.GetObjectByKey(key);

            if (commomObj == null)
            {
                FindEntityObjectByLastGuid(node);
                return false;
            }

            return true;
        }

        private void FindEntityObjectByLastGuid(XmlNode node)
        {
            string key = node.SelectSingleNode("Object").InnerText;

            string[] parts = key.Split(new string[] {":"}, StringSplitOptions.None);

            string objectGuid = parts[parts.Length - 1];

            IEntity entity =  scheme.RootPackage.FindEntityByName(objectGuid);

            if (entity != null)
            {
                node.SelectSingleNode("Object").InnerText = entity.Key;
            }
        }
    }
}
