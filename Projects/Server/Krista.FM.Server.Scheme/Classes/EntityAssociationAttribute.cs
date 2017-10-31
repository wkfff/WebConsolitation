using System;
using System.Collections.Generic;

using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.ScriptingEngine.Classes;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Classes
{
    /// <summary>
    /// ������� ����������
    /// </summary>
    [Serializable]
    internal class EntityAssociationAttribute : EntityDataAttribute
    {
        public EntityAssociationAttribute(string key, string name, ServerSideObject owner, ServerSideObjectStates state)
            : base(key, name, owner, state)
        {
        }

        /// <summary>
        /// ������������� �������� �� ��������� ��� ��������
        /// </summary>
        /// <param name="entity">������� � ������� ������ �������</param>
        /// <param name="script">������ SQL-��������</param>
        public override void UpdateSystemRowsSetDefaultValue(Entity entity, List<string> script)
        {
            if (((EntityAssociation)Owner).RoleB.ClassType == ClassTypes.clsBridgeClassifier)
            {
				ScriptingEngine.UpdateTableSetDefaultValue(
					this,
					entity.FullDBName,
					"-1",
					String.Format("{0} IS NULL and {1} = -1", Name, IDColumnName),
					script);
            }
        }

        internal override void UpdateTableSetDefaultValue(Entity entity, List<string> script)
        {
            if (((EntityAssociation)Owner).RoleB.ClassType == ClassTypes.clsBridgeClassifier ||
                ((EntityAssociation)Owner).RoleA.ClassType == ClassTypes.Table)
            {
                base.UpdateTableSetDefaultValue(entity, script);
            }
            else if (((EntityAssociation)Owner).RoleA is DataSourceDividedClass && ((EntityAssociation)Owner).RoleB is DataSourceDividedClass)
            {
                if (((DataSourceDividedClass)((EntityAssociation)Owner).RoleA).IsDivided && ((DataSourceDividedClass)((EntityAssociation)Owner).RoleB).IsDivided)
                {
                    List<int> sourcesID = ((DataSourceDividedClass)((Association)Owner).RoleA).GetDataSourcesID(SchemeClass.Instance.DDLDatabase);
                    if (sourcesID.Count == 0)
                        base.UpdateTableSetDefaultValue(entity, script);

                    foreach (int sourceID in sourcesID)
                    {
                        script.Add(String.Format("UPDATE {0} set {1} = {2} where {1} IS NULL and SourceID = {3}", entity.FullDBName, Name, DataClassifier.GetFixedRowID(sourceID), sourceID));
                    }

                    // ����������� �������� �� ��������� ��� ��������� -1
                    ScriptingEngine.UpdateTableSetDefaultValue(
                    this,
                    entity.FullDBName,
                    "-1",
                    String.Format("{0} IS NULL and {1} = -1", Name, SourceIDColumnName),
                    script);
                }
                else if (((DataSourceDividedClass)((EntityAssociation)Owner).RoleB).IsDivided)
                {
                    List<int> sourcesID = ((DataSourceDividedClass)((EntityAssociation)Owner).RoleB).GetDataSourcesID(SchemeClass.Instance.DDLDatabase);
                    if (sourcesID.Count > 0)
                    {
                        script.Add(String.Format("UPDATE {0} set {1} = {2} where {1} IS NULL", entity.FullDBName, Name, ((DataSourceDividedClass)((Association)Owner).RoleB).UpdateFixedRows(sourcesID[0])));
                    }
                }
                else
                    base.UpdateTableSetDefaultValue(entity, script);
            }
        }

        /// <summary>
        /// ������� �������������� ��� ��������.
        /// </summary>
        /// <param name="entity">��������.</param>
        internal override List<string> UpdateSetNullable(Entity entity)
        {
            List<string> script = new List<string>();

            EntityAssociationScriptingEngine se = (EntityAssociationScriptingEngine)((EntityAssociation)Owner).ScriptingEngine;

            // ������� �����������
			script.AddRange(se.DropReferenceConstraintScript((EntityAssociation)Owner));

            // ������� ��������������
            script.AddRange(ScriptingEngine.ModifyScript(this, entity, false, true, true));

            // ������� �����������
            script.AddRange(se.CreateReferenceConstraintScript((EntityAssociation)Owner));
            
            return script;
        }

        /// <summary>
        /// ������������� ���������� ������������
        /// </summary>
        /// <param name="oldValue">������ ������������</param>
        /// <param name="value">����� ������������</param>
        protected override void SetFullName(string oldValue, string value)
        {
            if (State == ServerSideObjectStates.New)
            {
                EntityAssociation entityAssociation = (EntityAssociation)Instance.Owner;
                entityAssociation.Name = String.Format("{0}.{1}", entityAssociation.RoleA.Name, value);
            }
        }

        #region ServerSideObject

        public override IServerSideObject Lock()
        {
            EntityAssociation cloneEntityAssociation = (EntityAssociation)Owner.Lock();
            return (ServerSideObject)cloneEntityAssociation.RoleDataAttribute;
        }

        #endregion ServerSideObject
    }
}
