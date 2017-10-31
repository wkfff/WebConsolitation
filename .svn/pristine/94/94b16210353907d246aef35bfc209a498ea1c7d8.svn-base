using System.Collections.Generic;

namespace Krista.FM.Common.Calculator
{
    public interface IValueStoreGroup : IValueStore
    {
        /// <summary>
        /// Введено для обратной совместимости с IValueStore.Get, подволяет переключать хранилище, которое будет возвращать значения в Get.
        /// </summary>
        string CurrentStore { get; set; }

        IValueItem Get(string valueName, string valueStoreId);

        void RegisterStore(string sourceId, IValueStore store);

        void UnregisterStore(string sourceId);

        IValueStore GetStore(string sourceId);

        ICollection<string> GetStoresIndex();
    }
}
