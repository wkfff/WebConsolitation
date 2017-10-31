using System;

using FluentNHibernate.Automapping;

namespace Krista.FM.Domain.Reporitory.NHibernate.Automapping
{
    /// <summary>
    /// You should create your own that either implements 
    /// IAutomappingConfiguration directly, 
    /// or inherits from DefaultAutomappingConfiguration.
    /// Overriding methods in this class will alter how the automapper behaves.
    /// </summary>
    public class AutomappingConfiguration : DefaultAutomappingConfiguration
    {
        public override bool ShouldMap(Type type)
        {
            // specify the criteria that types must meet in order to be mapped
            // any type for which this method returns false will not be mapped.
            return type.Namespace == "Krista.FM.Domain" && type.IsClass && !type.IsAbstract && type.Name != "HashObjectsNames";
        }

        public override bool IsDiscriminated(Type type)
        {
            // Необходим для определения подклассов (joined-subclass)
            if (type.IsSubclassOf(typeof(D_Report_Row)))
            {
                return true;
            }

            return false;
        }
    }
}
