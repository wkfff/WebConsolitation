using System;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.Classes
{
    /// <summary>
    /// Интерфейс фабрики
    /// </summary>
    internal abstract  class EntityAssociationFactoryAbstract
    {
        internal abstract IEntityAssociation CreateAssociation(string key, ServerSideObject owner, int ID, string semantic, string name,
                                  AssociationClassTypes associationClassType, Entity parent, Entity child,
                                  string configuration, ServerSideObjectStates state);

        internal abstract AssociationClassTypes GetAssociationClassType(IEntity roleA, IEntity roleB,
                                                      AssociationClassTypes associationClassType);
    }

    /// <summary>
    /// Фабрика для создания ассоциации
    /// </summary>
    internal class EntityAssociationFactoryBase : EntityAssociationFactoryAbstract
    {
        internal override IEntityAssociation CreateAssociation(string key, ServerSideObject owner, int ID, string semantic, string name, AssociationClassTypes associationClassType, Entity parent, Entity child, string configuration, ServerSideObjectStates state)
        {
            EntityAssociation association;

            if (parent == null)
                throw new Exception("Не указан родительский объект");
            if (child == null)
                throw new Exception("Не указан дочерний объект");

            switch (associationClassType)
            {
                case AssociationClassTypes.Link:
                    association = new FactAssociation(key, owner, semantic, name, state);
                    break;
                case AssociationClassTypes.Bridge:
                    if (IsBridgeRep(key))
                        association = new BridgeAssosiationReport(key, owner, semantic, name, state);
                    else
                        association = new BridgeAssociation(key, owner, semantic, name, state);
                    break;
                case AssociationClassTypes.MasterDetail:
                    association = new MasterDetailAssociation(key, owner, semantic, name, state);
                    break;
                case AssociationClassTypes.BridgeBridge:
                    if (IsBridgeRep(key))
                    {
                        association = new BridgeAssosiationReportItSelf(key, owner, semantic, name, state);
                        break;
                    }

                    association = new BridgeAssociationItSelf(key, owner, semantic, name, state);
                    break;
                default:
                    throw new Exception(String.Format("Объекты с классом {0} не поддерживаются.", associationClassType));
            }
            association.ID = ID;
            association.Configuration = configuration;
            association.RoleA = parent;
            association.RoleB = child;

            return association;
        }

        internal static bool IsBridgeRep(string key)
        {
            return    key == SchemeClassKeys.refBridgeRepUFK 
                   || key == SchemeClassKeys.refBridgeRepAS
                   || key == SchemeClassKeys.refRBridge
                   || key == SchemeClassKeys.refRBridgePlan
                   || key == SchemeClassKeys.refRRepMonthRep;
        }

        /// <summary>
        /// Определяет класс ассоциации по ее ролям.
        /// </summary>
        /// <param name="roleA">Роль A.</param>
        /// <param name="roleB">Роль B.</param>
        /// <param name="associationClassType">Класс ассоциации.</param>
        /// <returns>Класс ассоциации.</returns>
        internal override AssociationClassTypes GetAssociationClassType(IEntity roleA, IEntity roleB, AssociationClassTypes associationClassType)
        {
            SchemeClass.CheckRolesClasses(associationClassType, roleA.ClassType, roleB.ClassType, "Новая ассоциация");

            if ((roleA.ClassType == ClassTypes.clsFactData || roleA.ClassType == ClassTypes.clsDataClassifier || roleA.ClassType == ClassTypes.clsBridgeClassifier)
                && (roleB.ClassType == ClassTypes.clsDataClassifier || roleB.ClassType == ClassTypes.clsFixedClassifier))
            {
                associationClassType = AssociationClassTypes.Link;
            }
            else if (roleA.ClassType == ClassTypes.clsFactData && roleB.ClassType == ClassTypes.clsBridgeClassifier)
            {
                throw new Exception(String.Format("Невозможно создать ассоциацию между объектами {0} и {1}. Ассоциация между таблицей фактов и сопоставимым запрещена.", roleA.FullName, roleB.FullName));
            }
            else if (associationClassType == AssociationClassTypes.MasterDetail &&
                (roleA.ClassType == ClassTypes.Table) &&
                (roleB.ClassType == ClassTypes.clsBridgeClassifier ||
                 roleB.ClassType == ClassTypes.clsDataClassifier ||
                 roleB.ClassType == ClassTypes.clsFactData ||
                 roleB.ClassType == ClassTypes.clsFixedClassifier ||
                 roleB.ClassType == ClassTypes.Table))
            {
                foreach (IEntityAssociation item in roleA.Associations.Values)
                {
                    if (item.RoleBridge.FullName == roleB.FullName)
                        throw new Exception("Между одними и теми же таблицами можно создавать только одну ассоциацию \"Мастер-Деталь\"");
                }
            }
            else if (associationClassType == AssociationClassTypes.Link
                || associationClassType == AssociationClassTypes.BridgeBridge)
            {
            }
            else if (associationClassType == AssociationClassTypes.Bridge && (roleB.ClassType == ClassTypes.clsDataClassifier || roleB.ClassType == ClassTypes.clsBridgeClassifier) && (roleB.ClassType == ClassTypes.clsBridgeClassifier))
            {
            }
            else
                throw new Exception(String.Format("Невозможно создать ассоциацию между объектами {0} и {1}.", roleA.FullName, roleB.FullName));

            return associationClassType;
        }
    }
}
