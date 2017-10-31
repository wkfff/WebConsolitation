using System;
using System.Xml;

using Krista.FM.Server.Common;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.Classes.PresentationLayer
{
	/// <summary>
	/// Табличный документ.
	/// </summary>
	internal class DocumentEntity : Entity, IDocumentEntity
	{
		public DocumentEntity(string key, ServerSideObject owner, string semantic, string name, ServerSideObjectStates state)
			: base(key, owner, semantic, name, ClassTypes.DocumentEntity, SubClassTypes.Regular, state, SchemeClass.ScriptingEngineFactory.NullScriptingEngine)
        {
			tagElementName = "DocumentEntity";
        }

		/// <summary>
		/// Префикс таблицы.
		/// </summary>
		public override string TablePrefix
		{
			[System.Diagnostics.DebuggerStepThrough]
			get { return "r"; }
		}

		/// <summary>
		/// Инициализирует коллекцию атрибутов объекта по информации из XML настроек 
		/// </summary>
		/// <param name="doc">Документ с XML настройкой</param>
		/// <param name="atagElementName">наименование тега с настройками объекта</param>
		protected override void InitializeAttributes(XmlDocument doc, string atagElementName)
		{
			// Инициализируем атрибуты-ссылки 
			XmlNodeList xmlAttributes = doc.SelectNodes(String.Format("/DatabaseConfiguration/{0}/Attributes/RefAttribute", atagElementName));
			foreach (XmlNode xmlAttribute in xmlAttributes)
			{
				XmlNode keyNode = xmlAttribute.Attributes["objectKey"];
				Attributes.Add(new DocumentEntityAttribute(keyNode.Value, this, xmlAttribute, state));
			}

			// Инициализируем стандактные атрибуты
			xmlAttributes = doc.SelectNodes(String.Format("/DatabaseConfiguration/{0}/Attributes/Attribute", atagElementName));
			foreach (XmlNode xmlAttribute in xmlAttributes)
			{
				EntityDataAttribute attr = EntityDataAttribute.CreateAttribute(this, xmlAttribute, State);
				Attributes.Add(attr);
			}
		}

		internal override void Save2Xml(XmlNode node)
		{
		    base.Save2Xml(node);

		    Save2XmlRefAttributes(node);
		}

        /// <summary>
        /// Атрибуты ссылки переносим в конец коллекции.
        /// В Xml для справки уже эту операцию делали, но поскольку в справку включены атрибуты ассоциаций,
        /// то операцию переноса производим повторно
        /// </summary>
        /// <param name="node"></param>
	    private void Save2XmlRefAttributes(XmlNode node)
	    {
            //
	        // Атрибуты ссылки
	        //

	        XmlNode attributesNode = node.SelectSingleNode("Attributes");
	        foreach (EntityDataAttribute attr in Attributes.Values)
	        {
	            if (attr.Class == DataAttributeClassTypes.Typed && attr is DocumentEntityAttribute)
	            {
	                // удаляем тег Attribute соответствующего атрибута, для того, чтобы заменить его на RefAttribute
	                XmlNode attributeNode =
	                    (attributesNode.SelectSingleNode(String.Format("Attribute[@objectKey='{0}']", attr.ObjectKey)) !=
	                     null)
	                        ? attributesNode.SelectSingleNode(String.Format("Attribute[@objectKey='{0}']", attr.ObjectKey))
	                        : attributesNode.SelectSingleNode(String.Format("RefAttribute[@objectKey='{0}']",
	                                                                        attr.ObjectKey));
	                attributesNode.RemoveChild(attributeNode);

	                attributeNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, "RefAttribute", null);
	                attr.Save2Xml(attributeNode);
	                attributesNode.AppendChild(attributeNode);
	            }
	        }
	    }

	    public override bool CurrentUserCanViewThisObject()
		{
			return false;
		}

        internal override void Save2XmlDocumentation(XmlNode node)
        {
            base.Save2XmlDocumentation(node);

            //
            // Атрибуты ссылки
            //

            XmlNode attributesNode = node.SelectSingleNode("Attributes");
            foreach (EntityDataAttribute attr in Attributes.Values)
            {
                if (attr.Class == DataAttributeClassTypes.Typed && attr is DocumentEntityAttribute)
                {
                    // удаляем тег Attribute соответствующего атрибута, для того, чтобы заменить его на RefAttribute
                    XmlNode attributeNode =
                        (attributesNode.SelectSingleNode(String.Format("Attribute[@objectKey='{0}']", attr.ObjectKey)) !=
                         null)
                            ? attributesNode.SelectSingleNode(String.Format("Attribute[@objectKey='{0}']", attr.ObjectKey))
                            : attributesNode.SelectSingleNode(String.Format("RefAttribute[@objectKey='{0}']",
                                                                            attr.ObjectKey));
                    attributesNode.RemoveChild(attributeNode);

                    attributeNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, "RefAttribute", null);
                    attr.Save2XmlDocumentation(attributeNode);
                    attributesNode.AppendChild(attributeNode);
                }
            }
        }
	}
}
