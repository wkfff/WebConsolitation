using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.Classes
{
    internal class GroupedAttributeCollection : DataAttributeCollection
    {
        private IDataAttributeCollection attributes;

        public GroupedAttributeCollection(Entity parent, ServerSideObjectStates state, IDataAttributeCollection attributes)
            : base(parent, state)
        {
            this.attributes = attributes;
        }

        protected override System.Collections.IEnumerator Enumerator()
        {
            return new GroupedAttributesEnumerator(attributes, Parent);
        }

        private class GroupedAttributesEnumerator : IEnumerator<KeyValuePair<string, IDataAttribute>>
        {
            private SortedList<int, IDataAttribute> sortedAttributes;
            private Dictionary<string, IDataAttribute> systemAttributes;
            private IEnumerator enumerator;
            private IEnumerator systemEnumerator;

            private IEntity parent;

            /// <summary>
            /// Коструктор энумератора
            /// </summary>
            /// <param name="dictionary"></param>
            public GroupedAttributesEnumerator(IDataAttributeCollection dictionary, IEntity parent)
            {
                this.parent = parent;

                InitializeArray(dictionary);
                Reset();
            }

            /// <summary>
            /// Инициализация массива атрибутов
            /// </summary>
            /// <param name="dictionary"></param>
            private void InitializeArray(IDataAttributeCollection dictionary)
            {
                Dictionary<string, IDataAttribute> dataAttributes = CollectAttributes(dictionary, 0, "");

                sortedAttributes = new SortedList<int, IDataAttribute>(dataAttributes.Count);
                systemAttributes = new Dictionary<string, IDataAttribute>();

                int i = 0;

                foreach (KeyValuePair<string, IDataAttribute> pair in dataAttributes)
                {
                    if (sortedAttributes.ContainsKey(pair.Value.Position))
                    {
                        systemAttributes.Add(pair.Key, pair.Value);
                    }
                    else
                    {
                        sortedAttributes.Add(pair.Value.Position, pair.Value);
                    }
                    i++;
                }
                enumerator = sortedAttributes.GetEnumerator();
                systemEnumerator = systemAttributes.GetEnumerator();
            }

            private Dictionary<string, IDataAttribute> CollectAttributes(IDataAttributeCollection dictionary, int level, string lname)
            {
                Dictionary<string, IDataAttribute> innerDictionary = new Dictionary<string, IDataAttribute>();

                foreach (IDataAttribute dataAttribute in dictionary.Values)
                {
                    IDataAttribute attribute = GetAttribute(dataAttribute, level, lname);

                    if (attribute == null)
                        continue;

                    if (attribute is IGroupAttribute)
                    {
                        if (!innerDictionary.ContainsKey(attribute.Name))
                        {
                            ((IGroupAttribute)attribute).Attributes = CollectAttributes(dictionary, level + 1, attribute.Name);
                            innerDictionary.Add(attribute.Name, attribute);
                        }
                    }
                    else
                        innerDictionary.Add(attribute.Name, attribute);
                }

                return innerDictionary;
            }

            private IDataAttribute GetAttribute(IDataAttribute dataAttribute, int level, string lname)
            {
                if (String.IsNullOrEmpty(dataAttribute.GroupTags) && level == 0)
                    return dataAttribute;

                if (!String.IsNullOrEmpty(dataAttribute.GroupTags))
                {
                    string[] levelsName = dataAttribute.GroupTags.Split(new Char[] {';'});

                    if (levelsName.Length == level && levelsName[level - 1] == lname)
                        return dataAttribute;

                    if (level == 0 || (levelsName.Length > level && levelsName[level - 1] == lname))
                    {
                        string levelName = levelsName[level];

                        // создаем атрибут-группу, если ее нет
                        // наполняем коллекцию атрибутов для группы
                        // добавляем группу в коллекцию
                        GroupAttribute groupAttribute = new GroupAttribute(new Guid().ToString(), levelName,
                                                                           (Entity) parent,
                                                                           dataAttribute.State);
                        groupAttribute.Position = dataAttribute.Position;

                        return groupAttribute;
                    }
                }

                return null;
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
                        KeyValuePair<string, IDataAttribute> sysCur = (KeyValuePair<string, IDataAttribute>)systemEnumerator.Current;
                        return new KeyValuePair<string, IDataAttribute>(sysCur.Key, sysCur.Value);
                    }

                    KeyValuePair<int, IDataAttribute> cur = (KeyValuePair<int, IDataAttribute>)enumerator.Current;
                    return new KeyValuePair<string, IDataAttribute>(cur.Value.ObjectKey, cur.Value);
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
                        KeyValuePair<string, IDataAttribute> sysCur = (KeyValuePair<string, IDataAttribute>)systemEnumerator.Current;
                        return new KeyValuePair<string, IDataAttribute>(sysCur.Key, sysCur.Value);
                    }

                    KeyValuePair<int, IDataAttribute> cur = (KeyValuePair<int, IDataAttribute>)enumerator.Current;
                    return new KeyValuePair<string, IDataAttribute>(cur.Value.ObjectKey, cur.Value);
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

                return enumerator.MoveNext();
            }

            /// <summary>
            /// Сброс параметров
            /// </summary>
            public void Reset()
            {
                systemEnumerator.Reset();
                enumerator.Reset();
            }

            #endregion
        }
    }
}
