using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Common.Xml;
using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.Modifications;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Classes
{
    /// <summary>
	/// Summary description for Association.
	/// </summary>
    internal abstract class Association : EntityAssociation, IAssociation 
    {
        internal const string TagElementName = "Reference";

        /// <summary>
        /// Конструктор объекта
        /// </summary>
        /// <param name="semantic">Семантика</param>
        /// <param name="name">Наименование</param>
        /// <param name="subClassType">Подкласс объекта</param>
        public Association(string key, ServerSideObject owner, string semantic, string name, AssociationClassTypes associationClassType, ServerSideObjectStates state, ScriptingEngine.ScriptingEngineAbstraction scriptingEngine)
            : base(key, owner, semantic, name, associationClassType, state, scriptingEngine)
		{
        }

    }

    /// <summary>
    /// Коллекция ассоциаций
    /// </summary>
    internal class AssociationCollection : EntityAssociationCollection, IAssociationCollection
    {
        public AssociationCollection(ServerSideObject owner, ServerSideObjectStates state)
            : base(owner, state)
        {
        }

        public override IServerSideObject Lock()
        {
            Entity cloneEntity = (Entity)Owner.Lock();
            return (ServerSideObject)cloneEntity.Associations;
        }
    }

    /// <summary>
    /// Коллекция ассоциаций
    /// </summary>
    internal class AssociatedCollection : EntityAssociationCollection, IAssociationCollection
    {
        public AssociatedCollection(ServerSideObject owner, ServerSideObjectStates state)
            : base(owner, state)
        {
        }

        public override IServerSideObject Lock()
        {
            Entity cloneEntity = (Entity)Owner.Lock();
            return (ServerSideObject)cloneEntity.Associated;
        }
    }
}
