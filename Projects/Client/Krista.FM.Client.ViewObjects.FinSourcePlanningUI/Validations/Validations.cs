using System.Xml;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Validations
{
	public class FinSourceMasterValidator : MasterValidator
	{
		public FinSourceMasterValidator(XmlNode configuration) : base(configuration)
		{
		}
	}

	public class FinSourceMasterDetailValidator : MasterDetailValidator
	{
		public FinSourceMasterDetailValidator(XmlNode configuration) : base(configuration)
		{
		}
	}

	public class FinSourceMasterDetailDetailValdator : MasterDetailDetailValdator
	{
		public FinSourceMasterDetailDetailValdator(XmlNode configuration) : base(configuration)
		{
		}
	}

	public class FinSourceDetailValidator : DetailValidator
	{
		public FinSourceDetailValidator(XmlNode configuration) : base(configuration)
		{
		}
	}
}
