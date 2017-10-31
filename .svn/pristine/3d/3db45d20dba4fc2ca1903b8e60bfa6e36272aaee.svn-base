using Krista.FM.Server.Scheme.Classes;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Modifications
{
    /// <summary>
    /// Отвечает за создание новых объектов
    /// </summary>
    internal sealed class CreateModificationItem : MajorObjectModificationItem
    {
        /// <summary>
        /// Отвечает за создание новых объектов
        /// </summary>
        /// <param name="name">Наименование операции (имя создаваемого объекта)</param>
        /// <param name="ownerObject">Родительский объект в котором должен находиться созданный объект</param>
        /// <param name="toObject">Создаваемый объект</param>
        /// <param name="parent">Родительская операция модификации</param>
        public CreateModificationItem(string name, object ownerObject, object toObject, ModificationItem parent)
            : base(ModificationTypes.Create, name, ownerObject, toObject, parent)
        {
        }

        protected override void OnBeforeChildApplay(ModificationContext context, out bool isAppliedPartially)
        {
            base.OnBeforeChildApplay(context, out isAppliedPartially);

            if (ToObject is CommonDBObject)
            {
                // Прицепляем объект к серверному объекту,
                // до этого родителем был объект созданный по xml описанию из репозирория.
                ((CommonDBObject)ToObject).SetParent((CommonDBObject)FromObject);
                ((CommonDBObject)ToObject).Create(context);
            }
        }
    }
}
