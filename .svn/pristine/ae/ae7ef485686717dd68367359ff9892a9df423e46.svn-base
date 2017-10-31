using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace Krista.FM.Domain.Reporitory.NHibernate.MappingOverride
{
    public class D_InvProject_Vizual_Mapping : IAutoMappingOverride<D_InvProject_Vizual> 
	{
        public void Override(AutoMapping<D_InvProject_Vizual> mapping)
        {
            mapping.Map(x => x.Document).CustomType("BinaryBlob");
        }
	}
}
