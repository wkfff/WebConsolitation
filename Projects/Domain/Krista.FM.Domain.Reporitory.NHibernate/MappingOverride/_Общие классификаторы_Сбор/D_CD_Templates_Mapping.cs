using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace Krista.FM.Domain.Reporitory.NHibernate.MappingOverride
{
    public class D_CD_Templates_Mapping : IAutoMappingOverride<D_CD_Templates> 
	{
        public void Override(AutoMapping<D_CD_Templates> mapping)
        {
            mapping.Map(x => x.TemplateFile).CustomType("BinaryBlob");
            mapping.Map(x => x.TemplateMarkup).CustomType("BinaryBlob");
        }
	}
}
