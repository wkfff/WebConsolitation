using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace Krista.FM.Domain.Reporitory.NHibernate.MappingOverride
{
    public class T_CD_Files_Mapping : IAutoMappingOverride<T_CD_Files> 
	{
        public void Override(AutoMapping<T_CD_Files> mapping)
        {
            mapping.Map(x => x.FileBody).CustomType("BinaryBlob");
        }
	}
}
