using Krista.FM.ServerLibrary;

namespace Krista.FM.Common
{
    public class DataAttributeHelper
    {
        /// <summary>
        /// ���������� �������� �� ��������� �� ����� ��� �� ������������.
        /// </summary>
        /// <param name="collection">��������� ���������.</param>
        /// <param name="key">���������� ���� �������.</param>
        /// <param name="name">������������ �������.</param>
        public static IDataAttribute GetAttributeByKeyName(IDataAttributeCollection collection, string key, string name)
        {
            if (collection.ContainsKey(key))
            {
                return collection[key];
            }
            else
            {
                return GetByName(collection, name);
            }
        }

        /// <summary>
        /// ���������� �������� �� ��������� �� ����� ��� �� ������������.
        /// </summary>
        /// <param name="collection">��������� ���������.</param>
        /// <param name="name">������������ �������.</param>
        public static IDataAttribute GetByName(IDataAttributeCollection collection, string name)
        {
            IDataAttribute attr = null;
            foreach (IDataAttribute item in collection.Values)
            {
                if (item.Name == name)
                {
                    attr = item;
                    break;
                }
            }
            return attr;
        }
    }
}
