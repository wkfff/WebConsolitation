using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace Krista.FM.Domain.Reporitory.NHibernate.MappingOverride
{
    public class D_ExcCosts_Subsidy_Mapping : IAutoMappingOverride<D_ExcCosts_Subsidy>
    {
        public void Override(AutoMapping<D_ExcCosts_Subsidy> mapping)
        {
            mapping.Map(x => x.Doc).CustomType("BinaryBlob");
        }
    }
}
