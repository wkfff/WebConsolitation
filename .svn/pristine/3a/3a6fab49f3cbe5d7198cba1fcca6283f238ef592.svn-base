using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Core.Controllers.Helpers
{
    public static class EntityAuthorizationExtensions
    {
        public static bool CanViewData(this IEntity entity, IUsersManager usersManager)
        {
            return entity.CanViewData(usersManager, false);
        }

        public static bool CanViewData(this IEntity entity, IUsersManager usersManager, bool raiseException)
        {
            switch (entity.ClassType)
            {
                case ClassTypes.clsBridgeClassifier:
                    return usersManager.CheckPermissionForSystemObject(entity.ObjectKey, (int)AssociatedClassifierOperations.ViewClassifier, true);
                case ClassTypes.clsDataClassifier:
                    return usersManager.CheckPermissionForSystemObject(entity.ObjectKey, (int)DataClassifiesOperations.ViewClassifier, true);
                case ClassTypes.clsFactData:
                    return usersManager.CheckPermissionForSystemObject(entity.ObjectKey, (int)FactTableOperations.ViewClassifier, true);
                default:
                    return true;
            }
        }

        public static bool CanEditRecord(this IEntity entity, IUsersManager usersManager, bool raiseException)
        {
            switch (entity.ClassType)
            {
                case ClassTypes.clsBridgeClassifier:
                    return usersManager.CheckPermissionForSystemObject(entity.ObjectKey, (int)AssociatedClassifierOperations.EditRecord, true);
                case ClassTypes.clsDataClassifier:
                    return usersManager.CheckPermissionForSystemObject(entity.ObjectKey, (int)DataClassifiesOperations.EditRecord, true);
                case ClassTypes.clsFactData:
                    return usersManager.CheckPermissionForSystemObject(entity.ObjectKey, (int)FactTableOperations.EditRecord, true);
                default:
                    return true;
            }
        }

        public static bool CanAddRecord(this IEntity entity, IUsersManager usersManager, bool raiseException)
        {
            switch (entity.ClassType)
            {
                case ClassTypes.clsBridgeClassifier:
                    return usersManager.CheckPermissionForSystemObject(entity.ObjectKey, (int)AssociatedClassifierOperations.AddRecord, true);
                case ClassTypes.clsDataClassifier:
                    return usersManager.CheckPermissionForSystemObject(entity.ObjectKey, (int)DataClassifiesOperations.AddRecord, true);
                case ClassTypes.clsFactData:
                    return usersManager.CheckPermissionForSystemObject(entity.ObjectKey, (int)FactTableOperations.EditRecord, true);
                default:
                    return true;
            }
        }

        public static bool CanDeleteRecord(this IEntity entity, IUsersManager usersManager, bool raiseException)
        {
            switch (entity.ClassType)
            {
                case ClassTypes.clsBridgeClassifier:
                    return usersManager.CheckPermissionForSystemObject(entity.ObjectKey, (int)AssociatedClassifierOperations.DelRecord, true);
                case ClassTypes.clsDataClassifier:
                    return usersManager.CheckPermissionForSystemObject(entity.ObjectKey, (int)DataClassifiesOperations.DelRecord, true);
                case ClassTypes.clsFactData:
                    return usersManager.CheckPermissionForSystemObject(entity.ObjectKey, (int)FactTableOperations.EditRecord, true);
                default:
                    return true;
            }
        }
    }
}
