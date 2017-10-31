using System;

using Krista.FM.Server.Common;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Classes
{
	/// <summary>
    /// Ассоциация между фактами и классификаторами данных
	/// </summary>
    internal class FactAssociation : Association
	{
        /// <summary>
        /// Ассоциация между фактами и классификаторами данных
        /// </summary>
        /// <param name="semantic">Семантика</param>
        /// <param name="name">Наименование</param>
        public FactAssociation(string key, ServerSideObject owner, string semantic, string name, ServerSideObjectStates state)
            : base(key, owner, semantic, name, AssociationClassTypes.Link, state, SchemeClass.ScriptingEngineFactory.EntityAssociationScriptingEngine)
        {
            tagElementName = Association.TagElementName;
            MandatoryRoleData = true;
        }
    }
}
