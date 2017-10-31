using Krista.FM.Domain.MappingAttributes;

namespace Krista.FM.Domain
{
	public class D_Form_Relations : ClassifierTable
	{
		public static readonly string Key = "22a2998a-8498-4f20-807f-46dd884a6790";

		public virtual int RowType { get; set; }
		public virtual int ActivationType { get; set; }
		public virtual string LeftPart { get; set; }
		public virtual string RalationType { get; set; }
        [AnsiString]
		public virtual string RightPart { get; set; }
		public virtual string Description { get; set; }
		public virtual string ErrorMessage { get; set; }
		public virtual int Ord { get; set; }
		public virtual D_Form_Part RefPart { get; set; }
	}
}
