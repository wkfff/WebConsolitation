using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace Krista.FM.Domain.Reporitory.NHibernate.MappingOverride
{
    public class D_ExcCosts_NPA_Mapping : IAutoMappingOverride<D_ExcCosts_NPA>
    {
        public void Override(AutoMapping<D_ExcCosts_NPA> mapping)
        {
            mapping.Map(x => x.Doc).CustomType("BinaryBlob");
        }
    }
}
