using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace Krista.FM.Domain.Reporitory.NHibernate.MappingOverride
{
    public class F_S_SchBNote_Mapping : IAutoMappingOverride<F_S_SchBNote> 
	{
		public void Override(AutoMapping<F_S_SchBNote> mapping)
        {
            mapping.Map(x => x.Note).CustomType("BinaryBlob");
        }
	}
}
