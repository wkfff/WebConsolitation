using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;
using Krista.FM.Common.Exceptions.DbExceptions;
using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.Modifications;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.Classes
{
    internal class UniqueKeyCollection : ModifiableCollection<string, IUniqueKey>, IUniqueKeyCollection
    {
        private readonly IEntity _parent;


        public UniqueKeyCollection(ServerSideObject owner, ServerSideObjectStates state)
            : base(owner, state)
        {
            this._parent = (IEntity)owner;
        }

        public IModificationItem GetChanges(IModifiable toObject)
        {
           ModificationItem uniqueKeyModificationItem = base.GetChanges((UniqueKeyCollection)toObject);
           return uniqueKeyModificationItem;
        }

        protected override ModificationItem GetRemoveModificationItem(KeyValuePair<string, IUniqueKey> item, CollectionModificationItem root)
        {
            return new RemoveMinorModificationItem(String.Format("UK: {0}", item.Value.Caption), item.Value, root);
        }

        protected override ModificationItem GetCreateModificationItem(KeyValuePair<string, IUniqueKey> item, CollectionModificationItem root)
        {
            return new CreateMinorModificationItem(String.Format("UK: {0}", item.Value.Caption), _parent, item.Value, root);
        }


        public IUniqueKey CreateItem(string caption, List<string> fields, bool hashable)
        {
            UniqueKey uniqueKey = new UniqueKey(Guid.NewGuid().ToString(), this.Owner, ServerSideObjectStates.New);
            uniqueKey.Caption = caption;
            uniqueKey.Fields = fields;
            uniqueKey.Hashable = hashable;
            this.Add(uniqueKey.ObjectKey, uniqueKey);
            return uniqueKey;
        }



        public void Initialize(XmlNode xmlUKListNode)
        {
            if (xmlUKListNode == null)
            { return; }

            try
            {
                XmlNodeList xmlUKeys = xmlUKListNode.SelectNodes("UniqueKey");
                if (xmlUKeys == null)
                {
                    throw new XmlSchemaValidationException("Неверная структура xml: отсутствуют элементы UniqueKey");
                }
                //Проходим по всем уникальным ключам
                foreach (XmlNode xmlUKey in xmlUKeys)
                {
                    string uniqKeyObjectKey = xmlUKey.Attributes["objectKey"].Value;
                    
                    string uniqKeyCaption = null;
                    if (xmlUKey.Attributes["caption"] != null ) uniqKeyCaption = xmlUKey.Attributes["caption"].Value;

                    string uniqKeyHashable = null;
                    if (xmlUKey.Attributes["hashable"] != null) uniqKeyHashable = xmlUKey.Attributes["hashable"].Value;
                    
                    //Проходим по всем полям ключа
                    int fieldCount = xmlUKey.ChildNodes.Count;
                    if (fieldCount == 0)
                    {
                        throw new XmlSchemaValidationException("Неверная структура xml: отсутствуют элементы Field в UniqueKey");
                    }

                    List<string> uKFieldList = new List<string>(fieldCount);
                    foreach (XmlNode xmlUKField in xmlUKey.ChildNodes)
                    {
                        uKFieldList.Add(xmlUKField.Attributes["name"].Value);
                    }

                    //Создаем ключ
                    UniqueKey uniqueKey = new UniqueKey(uniqKeyObjectKey, this.Owner, ServerSideObjectStates.Consistent);
                    uniqueKey.Caption = uniqKeyCaption;
                    uniqueKey.Fields = uKFieldList;
                    
                    if (uniqKeyHashable != null)
                    {
                        uniqueKey.Hashable = Boolean.Parse(uniqKeyHashable);
                    }

                    //Добавляем ключ в коллекцию)
                    this.Add(uniqueKey.ObjectKey, uniqueKey);
                }

            }
            catch (Exception e)
            {
                Trace.TraceEvent(TraceEventType.Error, "Ошибка при инициализации списка уникальных ключей объекта {0}", ((Entity)this.Owner).FullName);
            }

        }



        internal void Save2Xml(XmlNode uniqueKeysNode)
        {
            foreach (UniqueKey uniqueKey in this.Values)
            {
                XmlNode uniqueKeyNode = uniqueKeysNode.OwnerDocument.CreateNode(XmlNodeType.Element, "UniqueKey", null);
                uniqueKey.Save2Xml(uniqueKeyNode);
                uniqueKeysNode.AppendChild(uniqueKeyNode);
            }
        }

        /// <summary>
        /// Удаление уникального ключа
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ukKey"></param>
        internal void RemoveUniqueKey(ModificationContext context, string ukKey)
        {
            if (this.ContainsKey(ukKey))
            {
                UniqueKey uniqueKey = (UniqueKey)this[ukKey];

                if (uniqueKey != null)
                {

                    // Удаляем уникальный ключ из БД, без порождения исключений при его отсутсвии в БД
                    try
                    {
                        SchemeClass.Instance.DDLDatabase.RunScript(uniqueKey.GetDropUniqConstraintScript().ToArray(), false);
                    }
                    catch (DbNonExistentConstraintException)
                    {
                        Trace.TraceWarning(String.Format("Уникальный ключ \"{0}\" отсутствовал в базе данных", uniqueKey));
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError(String.Format("Ошибка при удалении уникального ключа \"{0}\" из базы данных", uniqueKey), e);
                        throw new InvalidOperationException("Ошибка при удалении уникального ключа из БД", e);
                    }
                    
                    //Удаляем связанное хэш-поле и триггер его вычисления
                    //if (uniqueKey.Hashable)
                    //{
                        try
                        {
                            //Без порождения исключений из БД, т.к. лень разносить на две процедуры по отдельности для столбца и триггера :-)
                            SchemeClass.Instance.DDLDatabase.RunScript(uniqueKey.GetDropHashScript().ToArray(), true);
                        }
                        catch (Exception e)
                        {
                            Trace.TraceError(String.Format("Ошибка при удалении хэш-поля по уникальному ключу \"{0}\" в БД", uniqueKey));
                            throw new InvalidOperationException("Ошибка при удалении хэш-поля по уникальному ключу в БД", e);
                        }                        
                    //}

                    // Удаляем связанное хэш-поле из коллекции атрибутов
                    if (uniqueKey.Hashable)
                    {
                        this._parent.Attributes.Remove(KeyIdentifiedObject.GetKey(DataAttribute.SystemHashUK.ObjectKey, DataAttribute.SystemHashUK.Name));
                    }

                    // Удаляем уникальный ключ из коллекции уникальных ключей
                    this.Remove(ukKey);

                    Trace.WriteLine(String.Format("Уникальный ключ \"{0}\" успешно удален", uniqueKey));
                }
            }
        }


        /// <summary>
        /// Добавление уникального ключа
        /// </summary>
        /// <param name="context"></param>
        /// <param name="uniqueKey"></param>
        /// <param name="isAppliedPartially"></param>
        internal void AddUniqueKey(ModificationContext context, UniqueKey uniqueKey, out bool isAppliedPartially)
        {
            if (! this.ContainsKey(uniqueKey.ObjectKey))
            {
                //Создаем уникальный ключ в БД
                try
                {
                    //Может и имеет смысл попытаться его сначала дропнуть в БД с перехватом ошибки,
                    //но если будет совпадение по имени с другим ключом(из-за длинного имени таблицы) - то тот ключ будет просто потерян.
                    //В общем ситуация нештатная полюбому, поэтому пусть пока ругается на дубликат - если чё придется удалять ключ руками
                    SchemeClass.Instance.DDLDatabase.RunScript(uniqueKey.GetCreateUniqConstraintScript().ToArray(), false);
                }
                catch (DbDuplicateKeysFoundException e)
                {
                    Trace.TraceError(String.Format("Ошибка при создании уникального ключа \"{0}\" в БД. Обнаружены неуникальные записи", uniqueKey));
                    //throw new InvalidOperationException("Ошибка при создании уникального ключа в БД: в таблице обнаружены неуникальные записи.", e);
                    isAppliedPartially = true;
                    return;
                }
                catch(DbSuchUniqueKeyAlreadyExistsInTableException e)
                {
                    Trace.TraceError(String.Format("Ошибка при создании уникального ключа \"{0}\" в БД. Ключ с таким набором столбцов уже существует", uniqueKey));
                    throw new InvalidOperationException("Ошибка при создании уникального ключа в БД: Ключ с таким набором столбцов уже существует.", e);
                }
                catch(DbNameAlreadyUsedByExistingConstraintException e)
                {
                    Trace.TraceError(String.Format("Ошибка при создании уникального ключа \"{0}\" в БД. Ключ с таким именем уже существует", uniqueKey));
                    throw new InvalidOperationException("Ошибка при создании уникального ключа в БД: Ключ с таким именем уже существуе.", e);
                }

                catch (Exception e)
                {
                    Trace.TraceError(String.Format("Ошибка при создании уникального ключа \"{0}\" в БД", uniqueKey));
                    throw new InvalidOperationException("Ошибка при создании уникального ключа в БД", e);
                }



                //Создаем доп.объекты для хэш ключа - поле в таблице, уникальный индекс по нему и триггер для вычисления хэша
                if (uniqueKey.Hashable)
                {
                    try
                    {
                        SchemeClass.Instance.DDLDatabase.RunScript(uniqueKey.GetCreateHashScript().ToArray(), false);
                    }
                    catch(Exception e)
                    {
                        Trace.TraceError(String.Format("Ошибка при создании хэш-поля по уникальному ключу \"{0}\" в БД", uniqueKey));
                        
                        //Cносим то, что создалось (ключ, хэш-поле и триггер могут оставаться, коли завалимся на уникальном индексе по хэш )
                        try
                        {
                            SchemeClass.Instance.DDLDatabase.RunScript(uniqueKey.GetDropUniqConstraintScript().ToArray(), false);
                            SchemeClass.Instance.DDLDatabase.RunScript(uniqueKey.GetDropHashScript().ToArray(), false);
                        }
                        catch (Exception)
                        {
                            Trace.TraceError(String.Format("Ошибка при удалении созданного ключа, хэш-поля и триггера  \"{0}\" в БД", uniqueKey));
                        }

                        //throw new InvalidOperationException("Ошибка при создании хэш-поля по уникальному ключу в БД", e);
                        isAppliedPartially = true;
                        return;
                    }
                }

                // Добавляем хэш-поле в коллекцию атрибутов
                if (uniqueKey.Hashable)
                {
                    this._parent.Attributes.Add(DataAttribute.SystemHashUK);
                }

                //Добавляем уникальный ключ в коллекцию
                this.Add(uniqueKey.ObjectKey, uniqueKey);

                uniqueKey.State = ServerSideObjectStates.Consistent;

                Trace.WriteLine(String.Format("Уникальный ключ \"{0}\" успешно добавлен", uniqueKey));
            }

            isAppliedPartially = false;
        }

        /// <summary>
        /// Удаляет из списка полей уникальных ключей данное поле, либо удаляет ключ целиком, если поле единственное
        /// </summary>
        internal void RemoveFieldFromUniqueKeys(string field)
        {
            List<UniqueKey> keysForRemove = new List<UniqueKey>();

            //находим ключи, которые содержат в списке полей данный атрибут
            foreach (UniqueKey uniqueKey in this.Values)
            {
                if (uniqueKey.ContainField(field))
                {
                    if (uniqueKey.Fields.Count == 1)
                    {
                        keysForRemove.Add(uniqueKey);   
                    }
                    else
                    {
                        List<string> newFields = uniqueKey.Fields;
                        newFields.Remove(field);
                        uniqueKey.Fields = newFields;
                    }
                     
                }
            }

            //Удаляем эти ключи из коллекции
            foreach (UniqueKey uniqueKey in keysForRemove)
            {
                this.Remove(uniqueKey.ObjectKey);
            }
        }
        


    }
}
