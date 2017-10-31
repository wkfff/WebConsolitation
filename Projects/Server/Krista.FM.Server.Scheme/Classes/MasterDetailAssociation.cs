using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.ScriptingEngine;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.Classes
{
    /// <summary>
    /// Ассоциация мастер-деталь.
    /// </summary>
    internal class MasterDetailAssociation : EntityAssociation
    {
        internal const string TagElementName = "MasterDetail";
        
        /// <summary>
        /// Конструктор объекта.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="semantic">Семантика</param>
        /// <param name="name">Наименование</param>
        /// <param name="state"></param>
        public MasterDetailAssociation(string key, ServerSideObject owner, string semantic, string name, ServerSideObjectStates state)
            : base(key, owner, semantic, name, AssociationClassTypes.MasterDetail, state, SchemeClass.ScriptingEngineFactory.EntityAssociationScriptingEngine)
		{
            tagElementName = TagElementName;
            MandatoryRoleData = true;
            onDeleteAction = OnDeleteAction.Cascade;
        }

    }
}
