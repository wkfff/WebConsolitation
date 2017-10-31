using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using Krista.FM.Common.Xml;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.Classes.PresentationLayer
{
	internal class DocumentEntityAttribute : EntityDataAttribute, IDocumentEntityAttribute
	{
		private string sourceEntityKey;
		private string sourceEntityAttributeKey;

        public DocumentEntityAttribute(string key, string name, ServerSideObject owner, ServerSideObjectStates state)
            : base(key, name, owner, state)
        {
        }

		public DocumentEntityAttribute(string key, ServerSideObject owner, XmlNode xmlAttribute, ServerSideObjectStates state)
            : base(key, owner, xmlAttribute, state)
        {
			sourceEntityKey = xmlAttribute.Attributes["sourceEntityKey"].Value;
			sourceEntityAttributeKey = xmlAttribute.Attributes["sourceEntityAttributeKey"].Value;
		}

		#region IDocumentEntityAttribute Members

		public string SourceEntityKey
		{
			get { return Instance.sourceEntityKey; }
			set { SetInstance.sourceEntityKey = value; }
		}

		public string SourceEntityAttributeKey
		{
			get { return Instance.sourceEntityAttributeKey; }
			set { SetInstance.sourceEntityAttributeKey = value; }
		}

		public void SetSourceAttribute(IDataAttribute sourceAttribute)
		{
			SourceEntityKey = ((IEntity) sourceAttribute.OwnerObject).ObjectKey;
			SourceEntityAttributeKey = sourceAttribute.ObjectKey;
			Name = sourceAttribute.Name;
			Caption = sourceAttribute.Caption;
			Description = sourceAttribute.Description;
			Type = sourceAttribute.Type;
			Size = sourceAttribute.Size;
			Scale = sourceAttribute.Scale;
			Mask = sourceAttribute.Mask;
			Visible = sourceAttribute.Visible;
			IsNullable = sourceAttribute.IsNullable;
			IsReadOnly = sourceAttribute.IsReadOnly;
			StringIdentifier = sourceAttribute.StringIdentifier;
			DeveloperDescription = sourceAttribute.DeveloperDescription;
		}

		#endregion

		internal override void Save2Xml(XmlNode node)
		{
			base.Save2Xml(node);
			XmlHelper.SetAttribute(node, "sourceEntityKey", SourceEntityKey);
			XmlHelper.SetAttribute(node, "sourceEntityAttributeKey", SourceEntityAttributeKey);
		}

		#region DDL

		public override void UpdateSystemRowsSetDefaultValue(Entity entity, List<string> script)
		{
		}

		internal override void UpdateTableSetDefaultValue(Entity entity, List<string> script)
		{
		}

		internal override List<string> AddScript(Entity entity, bool withNullClause, bool generateDependendScripts)
		{
			return new List<string>();
		}

		internal override string[] ModifyScript(Entity entity, bool withTypeModification, bool withNullClause, bool generateDependendScripts)
		{
			return new string[0];
		}

		internal override List<string> DropScript(Entity entity)
		{
			return new List<string>();
		}

		#endregion DDL

		#region ServerSideObject

		/// <summary>
		/// Возвращает экземпляр объекта с которым должен работать текущий пользователь
		/// </summary>
		/// <returns>Экземпляр объекта</returns>
		protected new DocumentEntityAttribute Instance
		{
			[DebuggerStepThrough]
			get { return (DocumentEntityAttribute)GetInstance(); }
		}

		/// <summary>
		/// Возвращает экземпляр объекта с которым должен работать текущий пользователь, для установки значений свойств
		/// </summary>
		/// <returns>Экземпляр объекта</returns>
		protected new DocumentEntityAttribute SetInstance
		{
			get
			{
				if (SetterMustUseClone())
					return (DocumentEntityAttribute)CloneObject;
				else
					return this;
			}
		}

		public override IServerSideObject Lock()
		{
			Entity cloneEntity = (Entity)Owner.Lock();
			return (ServerSideObject)cloneEntity.Attributes[ObjectKey];
		}

		#endregion ServerSideObject
	}
}
