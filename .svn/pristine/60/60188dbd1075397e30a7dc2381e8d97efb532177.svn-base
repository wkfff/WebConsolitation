using System;
using System.Collections.Generic;
using System.Xml;

using Krista.FM.Common.Exceptions;
using Krista.FM.Common.Xml;
using Krista.FM.Server.Scheme;

using Krista.FM.ServerLibrary;
using System.Xml.XPath;

namespace Krista.FM.Server.Scheme.Classes
{
    internal partial class Package
    {
        /// <summary>
        /// Метод сохранения классов и асоциаций
        /// </summary>
        /// <param name="node">Текущий узел</param>
        internal override void Save2Xml(XmlNode node)
        {
            BasePackageSave(node);

            // Пакеты
            XmlNode packagesNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, "Packages", null);

            SaveCollectionPackage2Xml(Packages.Values, packagesNode);

            if (packagesNode.ChildNodes.Count > 0)
                node.AppendChild(packagesNode);

            // Классы
            XmlNode classesNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, "Classes", null);

            SaveCollectionEntity2Xml(ClassTypes.clsFixedClassifier, Classes.Values, classesNode);
            SaveCollectionEntity2Xml(ClassTypes.clsBridgeClassifier, Classes.Values, classesNode);
            SaveCollectionEntity2Xml(ClassTypes.clsDataClassifier, Classes.Values, classesNode);
            SaveCollectionEntity2Xml(ClassTypes.clsFactData, Classes.Values, classesNode);
            SaveCollectionEntity2Xml(ClassTypes.Table, Classes.Values, classesNode);
			SaveCollectionEntity2Xml(ClassTypes.DocumentEntity, Classes.Values, classesNode);

            if (classesNode.ChildNodes.Count > 0)
                node.AppendChild(classesNode);

            // Ассоциации
            XmlNode associationsNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, "Associations", null);

            SaveCollectionAssociation2Xml(AssociationClassTypes.Bridge, Associations.Values, associationsNode);
            SaveCollectionAssociation2Xml(AssociationClassTypes.BridgeBridge, Associations.Values, associationsNode);
            SaveCollectionAssociation2Xml(AssociationClassTypes.Link, Associations.Values, associationsNode);
            SaveCollectionAssociation2Xml(AssociationClassTypes.MasterDetail, Associations.Values, associationsNode);

            if (associationsNode.ChildNodes.Count > 0)
                node.AppendChild(associationsNode);

            // Документы
            XmlNode documentsNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, "Documents", null);

            SaveDocumentCollection2Xml(Documents.Values, documentsNode);

            if (documentsNode.ChildNodes.Count > 0)
                node.AppendChild(documentsNode);
        }

        private void BasePackageSave(XmlNode node)
        {
            base.Save2Xml(node);
            /*if (privatePath != null)
            {
                XmlHelper.SetAttribute(node, "privatePath", privatePath);
                //return;
            }
            else */
            if (!String.IsNullOrEmpty(Name))
            {
                XmlHelper.SetAttribute(node, "name", Name == "Корневой пакет" ? SchemeClass.Instance.Name : Name);
            }
            else
                throw new Exception("У пакета отсутствуют обязательные атрибуты.");

            if (!String.IsNullOrEmpty(Description))
                XmlHelper.SetAttribute(node, "description", Description);
        }

        /// <summary>
        /// Создание базового узла
        /// </summary>
        public override string ConfigurationXml
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                doc.InnerXml = "<?xml version=\"1.0\" encoding=\"windows-1251\"?><ServerConfiguration/>";

               
                XmlNode element = doc.CreateElement("Package");
                doc.DocumentElement.AppendChild(element);
                                
                Save2Xml(element);
                return doc.InnerXml;
            }
        }
        
        /// <summary>
        /// Метод сохранения пакета для документации
        /// </summary>
        /// <param name="node"></param>
        /// <exception cref="Krista.FM.Common.Exceptions.HelpException"></exception>
        internal override void Save2XmlDocumentation(XmlNode node)
        {
            try
            {
                BasePackageSave(node);

                // Специфичные для документации свойства пакета

                // Пакеты
                XmlNode packagesNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, "Packages", null);

                this.SaveCollectionPackage2XmlDocumentation(Packages.Values, packagesNode);

                if (packagesNode.ChildNodes.Count > 0)
                    node.AppendChild(packagesNode);

                // Классы
                XmlNode classesNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, "Classes", null);

                this.SaveCollectionEntity2XmlDocumentation(ClassTypes.clsFixedClassifier, Classes.Values, classesNode);
                this.SaveCollectionEntity2XmlDocumentation(ClassTypes.clsBridgeClassifier, Classes.Values, classesNode);
                this.SaveCollectionEntity2XmlDocumentation(ClassTypes.clsDataClassifier, Classes.Values, classesNode);
                this.SaveCollectionEntity2XmlDocumentation(ClassTypes.clsFactData, Classes.Values, classesNode);
                this.SaveCollectionEntity2XmlDocumentation(ClassTypes.Table, Classes.Values, classesNode);
                this.SaveCollectionEntity2XmlDocumentation(ClassTypes.DocumentEntity, Classes.Values, classesNode);

                if (classesNode.ChildNodes.Count > 0)
                    node.AppendChild(classesNode);

                // Ассоциации
                XmlNode associationsNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, "Associations", null);

                this.SaveCollectionAssociation2XmlDocumentation(AssociationClassTypes.Bridge, Associations.Values, associationsNode);
                this.SaveCollectionAssociation2XmlDocumentation(AssociationClassTypes.BridgeBridge, Associations.Values, associationsNode);
                this.SaveCollectionAssociation2XmlDocumentation(AssociationClassTypes.Link, Associations.Values, associationsNode);
                this.SaveCollectionAssociation2XmlDocumentation(AssociationClassTypes.MasterDetail, Associations.Values, associationsNode);

                if (associationsNode.ChildNodes.Count > 0)
                    node.AppendChild(associationsNode);

                // Документы
                XmlNode documentsNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, "Documents", null);

                SaveDocumentCollection2XmlDocumentation(Documents.Values, documentsNode);

                if (documentsNode.ChildNodes.Count > 0)
                    node.AppendChild(documentsNode);
                if (IsEndPoint)
                {
                    XmlHelper.SetAttribute(node, "privatePath", PrivatePath);
                }

                // дополнительные для документации свойства пакета
            }
            catch (XmlException ex)
            {
                throw new HelpException(ex.ToString());
            }
            catch (ArgumentException ex)
            {
                throw new HelpException(ex.ToString());
            }
            catch (InvalidOperationException ex)
            {
                throw new HelpException(ex.ToString());
            }
            catch (XPathException ex)
            {
                throw new HelpException(ex.ToString());
            }
        }

        /// <summary>
        /// Сериализация коллекции документов для документации
        /// </summary>
        /// <param name="iCollection"></param>
        /// <param name="documentsNode"></param>
        /// <exception cref="Krista.FM.Common.Exceptions.HelpException"></exception>
        private void SaveDocumentCollection2XmlDocumentation(ICollection<IDocument> iCollection, XmlNode documentsNode)
        {
            try
            {
                foreach (Document document in iCollection)
                {
                    // в XML сохраняем корректные имема диаграмм
                    string s = CheckName(document.Name);

                    XmlNode xmlNode = documentsNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Document", null);
                    document.Save2XmlDocumentation(xmlNode);
                    documentsNode.AppendChild(xmlNode);
                }
            }
            catch (ArgumentException ex)
            {
                throw new HelpException(ex.ToString());
            }
            catch (InvalidOperationException ex)
            {
                throw new HelpException(ex.ToString());
            }
        }

        private string CheckName(string p)
        {
            char[] illegalCharacters = new char[] { ':', '/', '\\', '|', '*', '<', '>', '?', '"' };
            for (int i = 0; i < illegalCharacters.Length; i++)
            {
                if (p.IndexOf(illegalCharacters[i]) > -1)
                {
                    p = p.Replace(illegalCharacters[i], '_');
                }
            }
            return p;
        }

        /// <summary>
        /// Сериализация коллекции ассоциаций для документации
        /// </summary>
        /// <param name="associationClassTypes"></param>
        /// <param name="iCollection"></param>
        /// <param name="associationsNode"></param>
        /// <exception cref="Krista.FM.Common.Exceptions.HelpException"></exception>
        private void SaveCollectionAssociation2XmlDocumentation(AssociationClassTypes associationClassTypes, ICollection<IEntityAssociation> iCollection, XmlNode associationsNode)
        {
            try
            {
                foreach (EntityAssociation association in iCollection)
                {
                    if (association.AssociationClassType == associationClassTypes)
                    {
                        XmlNode xmlNode = associationsNode.OwnerDocument.CreateNode(XmlNodeType.Element, SchemeClass.GetNameByAssociationClassType(associationClassTypes), null);
                        association.Save2XmlDocumentation(xmlNode);
                        associationsNode.AppendChild(xmlNode);
                    }
                }
            }
            catch (ArgumentException ex)
            {
                throw new HelpException(ex.ToString());
            }
            catch (InvalidOperationException ex)
            {
                throw new HelpException(ex.ToString());
            }
        }

        /// <summary>
        /// Сериализация коллекции объектов для документации
        /// </summary>
        /// <param name="classTypes"></param>
        /// <param name="iCollection"></param>
        /// <param name="classesNode"></param>
        /// <exception cref="Krista.FM.Common.Exceptions.HelpException"></exception>
        private void SaveCollectionEntity2XmlDocumentation(ClassTypes classTypes, ICollection<IEntity> iCollection, XmlNode classesNode)
        {
            try
            {
                foreach (Entity entity in iCollection)
                {
                    if (entity.ClassType == classTypes)
                    {
                        XmlNode xmlNode = classesNode.OwnerDocument.CreateNode(XmlNodeType.Element,
                                                                               SchemeClass.GetNameByClassType(classTypes) +
                                                                               "Doc", null);
                        entity.Save2XmlDocumentation(xmlNode);
                        entity.SaveAssociationDoсumentation(xmlNode);
                        classesNode.AppendChild(xmlNode);
                    }
                }
            }
            catch(ArgumentException ex)
            {
                throw new HelpException(ex.ToString());
            }
            catch (InvalidOperationException ex)
            {
                throw new HelpException(ex.ToString());
            }
        }

        /// <summary>
        /// Сериализация коллекции пакетов для документации
        /// </summary>
        /// <param name="iCollection"></param>
        /// <param name="packagesNode"></param>
        /// <exception cref="Krista.FM.Common.Exceptions.HelpException"></exception>
        private void SaveCollectionPackage2XmlDocumentation(ICollection<IPackage> iCollection, XmlNode packagesNode)
        {
            try
            {
                foreach (Package package in iCollection)
                {
                    XmlNode xmlNode = packagesNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Package", null);

                    if (package.IsEndPoint)
                    {
                        XmlHelper.SetAttribute(xmlNode, "privatePath", package.PrivatePath);
                    }

                    package.Save2XmlDocumentation(xmlNode);


                    packagesNode.AppendChild(xmlNode);
                }
            }
            catch (ArgumentException ex)
            {
                throw new HelpException(ex.ToString());
            }
            catch(InvalidOperationException ex)
            {
                throw new HelpException(ex.ToString());
            }
        }

        /// <summary>
        /// Сохраняет коллекцию пакетов в XML
        /// </summary>
        /// <param name="packageCollection">Коллекция пакетов</param>
        /// <param name="node"></param>
        private void SaveCollectionPackage2Xml(ICollection<IPackage> packageCollection, XmlNode node)
        {
            foreach (Package package in packageCollection)
            {
                if (package.FullName == SystemPackageName)
                    continue;

                XmlNode xmlNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, "Package", null);
                
                if (package.IsEndPoint)
                {
                    XmlHelper.SetAttribute(xmlNode, "privatePath", package.PrivatePath);
                }
                else
                    package.Save2Xml(xmlNode);
                

                node.AppendChild(xmlNode);
            }
        }

        /// <summary>
        /// Заполняет ассоциации по типу
        /// </summary>
        /// <param name="associationClassType">Тип ассоциации</param>
        /// <param name="entityCollection">Коллекция ассоциаций</param>
        /// <param name="node">Узел "Associations"</param>
        private void SaveCollectionAssociation2Xml(AssociationClassTypes associationClassType, ICollection<IEntityAssociation> entityCollection, XmlNode node)
        {
            foreach (EntityAssociation association in entityCollection)
            {
                if (association.AssociationClassType == associationClassType)
                {
                    XmlNode xmlNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, SchemeClass.GetNameByAssociationClassType(associationClassType), null);
                    association.Save2Xml(xmlNode);
                    node.AppendChild(xmlNode);
                }
            }
        }

        /// <summary>
        /// Заполняет классы по типу
        /// </summary>
        /// <param name="classType">Тип класса</param>
        /// <param name="entityCollection">коллекция классов</param>
        /// <param name="node">узел "Classes"</param>
        private void SaveCollectionEntity2Xml(ClassTypes classType, ICollection<IEntity> entityCollection, XmlNode node)
        {
            foreach (Entity entity in entityCollection)
            {
                if (entity.ClassType == classType)
                {
                    XmlNode xmlNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, SchemeClass.GetNameByClassType(classType), null);
                    entity.Save2Xml(xmlNode);
                    node.AppendChild(xmlNode);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentCollection"></param>
        /// <param name="node"></param>
        private void SaveDocumentCollection2Xml(ICollection<IDocument> documentCollection, XmlNode node)
        {
            foreach (Document document in documentCollection)
            {
                XmlNode xmlNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, "Document", null);
                document.Save2Xml(xmlNode);
                node.AppendChild(xmlNode);
            }
        }
    }
}
