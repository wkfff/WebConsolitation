using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace Krista.FM.Domain.Reporitory.NHibernate.MappingOverride
{
    public class D_InvArea_Visual_Mapping : IAutoMappingOverride<D_InvArea_Visual> 
	{
        public void Override(AutoMapping<D_InvArea_Visual> mapping)
        {
            mapping.Map(x => x.Document).CustomType("BinaryBlob");
        }
	}
}
