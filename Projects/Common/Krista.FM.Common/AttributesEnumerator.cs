using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Common
{
    /// <summary>
    /// Упорядочивает коллекцию атрибутов согласно позиции.
    /// При работе с Dictionary<TKey, TValue> была замечена неприятная особенность его работы:
    /// после удаления элемента, вновь созданный, не встает вниз, а занимает позицию удаленного
    /// </summary>
    [Serializable]
    public class AttributesEnumerator : IEnumerator<KeyValuePair<string, IDataAttribute>>
    {
        private SortedList<int, IDataAttribute> sortedAttributes;
        private SortedList<int, IDataAttribute> systemAttributes;
        private Dictionary<string, IDataAttribute> dublicatePosAttributes;
        private IEnumerator enumerator;
        private IEnumerator systemEnumerator;
        private IEnumerator dublicateEnumerator;

        /// <summary>
        /// Коструктор энумератора
        /// </summary>
        /// <param name="dictionary"></param>
        public AttributesEnumerator(Dictionary<string, IDataAttribute> dictionary)
        {
            InitializeArray(dictionary);
            Reset();
        }

        /// <summary>
        /// Инициализация массива атрибутов
        /// </summary>
        /// <param name="dictionary"></param>
        private void InitializeArray(Dictionary<string, IDataAttribute> dictionary)
        {
            sortedAttributes = new SortedList<int, IDataAttribute>(dictionary.Count);
            systemAttributes = new SortedList<int, IDataAttribute>();
            dublicatePosAttributes = new Dictionary<string, IDataAttribute>();

            int i = 0;

            foreach (KeyValuePair<string, IDataAttribute> pair in dictionary)
            {
                if (sortedAttributes.ContainsKey(pair.Value.Position) 
                    || (systemAttributes.ContainsKey(pair.Value.Position))
                    || pair.Value.Position == 0)
                {
                    dublicatePosAttributes.Add(pair.Key, pair.Value);
                }
                else if (pair.Value.Class == DataAttributeClassTypes.System || pair.Value.Class == DataAttributeClassTypes.Fixed)
                {
                    systemAttributes.Add(pair.Value.Position, pair.Value);
                }
                else
                {
                    sortedAttributes.Add(pair.Value.Position, pair.Value);
                }
                i++;
            }
            enumerator = sortedAttributes.GetEnumerator();
            systemEnumerator = systemAttributes.GetEnumerator();
            dublicateEnumerator = dublicatePosAttributes.GetEnumerator();
        }

        #region IEnumerator<KeyValuePair<string,IDataAttribute>> Members

        /// <summary>
        /// Текущий объект
        /// </summary>
        public KeyValuePair<string, IDataAttribute> Current
        {
            get
            {
                if (systemEnumerator != null)
                {
                    KeyValuePair<int, IDataAttribute> sysCur = (KeyValuePair<int, IDataAttribute>)systemEnumerator.Current;
                    return new KeyValuePair<string, IDataAttribute>(sysCur.Value.Name, sysCur.Value);
                }

                if (enumerator != null)
                {
                    KeyValuePair<int, IDataAttribute> cur = (KeyValuePair<int, IDataAttribute>) enumerator.Current;
                    return new KeyValuePair<string, IDataAttribute>(cur.Value.ObjectKey, cur.Value);
                }

                KeyValuePair<string , IDataAttribute> dub = (KeyValuePair<string, IDataAttribute>)dublicateEnumerator.Current;
                return new KeyValuePair<string, IDataAttribute>(dub.Key, dub.Value);
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Освобождаем ресурсы
        /// </summary>
        public void Dispose()
        {
            // Освобождать нечего
        }

        #endregion

        #region IEnumerator Members

        /// <summary>
        /// Текущий объект
        /// </summary>
        object IEnumerator.Current
        {
            get
            {
                if (systemEnumerator != null)
                {
                    KeyValuePair<int, IDataAttribute> sysCur = (KeyValuePair<int, IDataAttribute>)systemEnumerator.Current;
                    return new KeyValuePair<string, IDataAttribute>(sysCur.Value.Name, sysCur.Value);
                }

                if (enumerator != null)
                {
                    KeyValuePair<int, IDataAttribute> cur = (KeyValuePair<int, IDataAttribute>)enumerator.Current;
                    return new KeyValuePair<string, IDataAttribute>(cur.Value.ObjectKey, cur.Value);
                }

                KeyValuePair<string, IDataAttribute> dub = (KeyValuePair<string, IDataAttribute>)dublicateEnumerator.Current;
                return new KeyValuePair<string, IDataAttribute>(dub.Key, dub.Value);
            }
        }

        /// <summary>
        /// Получает следущий элемент
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            if (systemEnumerator != null && systemEnumerator.MoveNext())
            {
                return true;
            }
            systemEnumerator = null;

            if (enumerator != null && enumerator.MoveNext())
            {
                return true;
            }
            enumerator = null;

            return dublicateEnumerator.MoveNext();
        }

        /// <summary>
        /// Сброс параметров
        /// </summary>
        public void Reset()
        {
            systemEnumerator.Reset();
            enumerator.Reset();
            dublicateEnumerator.Reset();
        }

        #endregion
    }
}
