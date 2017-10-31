using System;

using Krista.FM.Server.Common;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Classes
{
	/// <summary>
    /// ���������� ����� ������� � ���������������� ������
	/// </summary>
    internal class FactAssociation : Association
	{
        /// <summary>
        /// ���������� ����� ������� � ���������������� ������
        /// </summary>
        /// <param name="semantic">���������</param>
        /// <param name="name">������������</param>
        public FactAssociation(string key, ServerSideObject owner, string semantic, string name, ServerSideObjectStates state)
            : base(key, owner, semantic, name, AssociationClassTypes.Link, state, SchemeClass.ScriptingEngineFactory.EntityAssociationScriptingEngine)
        {
            tagElementName = Association.TagElementName;
            MandatoryRoleData = true;
        }
    }
}
