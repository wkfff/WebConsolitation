using System.Data;
using System.Xml;
using System.Reflection;
using Krista.FM.Common.Validations.Messages;
using Krista.FM.ServerLibrary.Validations;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations
{
	public class MasterValidator : AbstractMasterValidator
	{
		private XmlNode configuration;

		public MasterValidator(XmlNode configuration)
		{
			this.configuration = configuration;
		}

		public override IValidatorMessageHolder Validate(DataRow masterRow, string masterKey)
		{
			ValidationMessages vmh = new ValidationMessages();
			vmh.Add(CreateValidatorInstance(configuration).Validate(masterRow, masterKey));
			return vmh;
		}

		/// <summary>
		/// Создает экземпляр валидатора.
		/// </summary>
		/// <param name="node">Xml-описание валидатора.</param>
		private AbstractMasterValidator CreateValidatorInstance(XmlNode node)
		{
			// Создаем экземпляр выщвав конструктор вез параметров
			AbstractMasterValidator v =
				(AbstractMasterValidator)GetType().Assembly.CreateInstance(node.Attributes["class"].Value);

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