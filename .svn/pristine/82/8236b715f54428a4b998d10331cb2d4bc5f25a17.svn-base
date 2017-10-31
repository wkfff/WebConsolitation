using System.Data;
using System.Xml;
using System.Reflection;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.Common.Validations.Messages;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.Validations;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations
{
	public class DetailValidator : AbstractDetailValidator
	{
		private XmlNode configuration;

		public DetailValidator(XmlNode configuration)
		{
			this.configuration = configuration;
		}

		public override IValidatorMessageHolder Validate(DataTable detailTable)
		{
			ValidationMessages vmh = new ValidationMessages();
			vmh.Add(CreateValidatorInstance(configuration).Validate(detailTable));
			return vmh;
		}

		/// <summary>
		/// Создает экземпляр валидатора.
		/// </summary>
		/// <param name="node">Xml-описание валидатора.</param>
		private AbstractDetailValidator CreateValidatorInstance(XmlNode node)
		{
			// Создаем экземпляр выщвав конструктор вез параметров
			AbstractDetailValidator v =
				(AbstractDetailValidator)GetType().Assembly.CreateInstance(node.Attributes["class"].Value);

			IEntityAssociation entityAssociation = WorkplaceSingleton.Workplace.ActiveScheme.RootPackage.FindAssociationByName(
				node.ParentNode.Attributes["associationKey"].Value);
			v.Association = entityAssociation;

			if (node.Attributes["secondAssociationKey"] != null)
			{
				entityAssociation = WorkplaceSingleton.Workplace.ActiveScheme.RootPackage.FindAssociationByName(
					node.Attributes["secondAssociationKey"].Value);
				v.SecondAssociation = entityAssociation;
			}

			// Инициалицируем свойства валидатора
			foreach (XmlNode propertyNode in node.ChildNodes)
			{
				if (propertyNode.NodeType != XmlNodeType.Element)	
					continue;

				foreach (MethodInfo mi in v.GetType().GetMethods())
				{
					if (mi.MemberType == MemberTypes.Method && 
					    mi.IsPublic && 
					    mi.Name == string.Format("set_{0}", propertyNode.Name))
					{
						mi.Invoke(v, BindingFlags.SetProperty, null, new object[] { propertyNode.InnerText }, null);
						break;
					}
				}
			}
			
			return v;
		}
	}
}