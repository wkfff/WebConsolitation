using System.Data;
using System.Xml;
using System.Reflection;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Validations
{
	/// <summary>
	/// Составной валидатор, состоящий из нескольких дочерних валидаторов.
	/// Выполняет проверки для связки мастер-деталь.
	/// </summary>
	public class MasterDetailCompositeValidator : AbstractMasterDetailValidator
	{
		private XmlNode configuration;

		public MasterDetailCompositeValidator(XmlNode configuration)
		{
			this.configuration = configuration;
		}

		public override IValidatorMessageHolder Validate(DataRow masterRow, DataTable detailTable)
		{
			ValidationMessages vmh = new ValidationMessages();

			XmlNodeList nodeList = configuration.SelectNodes("MasterDetailValidator");
			if (nodeList != null)
			{
				foreach (XmlNode node in nodeList)
				{
					vmh.Add(CreateValidatorInstance(node).Validate(masterRow, detailTable));
				}
			}
			return vmh;
		}

		/// <summary>
		/// Создает экземпляр валидатора.
		/// </summary>
		/// <param name="node">Xml-описание валидатора.</param>
		private AbstractMasterDetailValidator CreateValidatorInstance(XmlNode node)
		{
			// Создаем экземпляр выщвав конструктор вез параметров
			AbstractMasterDetailValidator v =
				(AbstractMasterDetailValidator) GetType().Assembly.CreateInstance(node.Attributes["class"].Value);

			IEntityAssociation entityAssociation = WorkplaceSingleton.Workplace.ActiveScheme.RootPackage.FindAssociationByName(
				node.ParentNode.Attributes["associationKey"].Value);
			v.Association = entityAssociation;

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
