using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.ScriptingEngine;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.Classes
{
    /// <summary>
    /// ���������� ������-������.
    /// </summary>
    internal class MasterDetailAssociation : EntityAssociation
    {
        internal const string TagElementName = "MasterDetail";
        
        /// <summary>
        /// ����������� �������.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="semantic">���������</param>
        /// <param name="name">������������</param>
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
