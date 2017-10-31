using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Krista.FM.Domain.Reporitory
{
    public sealed class DomainRepository
    {
        private readonly IDomainDataService domainDataService;

        public DomainRepository(IDomainDataService domainDataService)
        {
            this.domainDataService = domainDataService; 
        }

        public T Get<T>(int id) where T : DomainObject, new()
        {
            Type objectType = typeof (T);

            DataRow[] rows = domainDataService.GetObjectData(objectType, String.Format("ID = {0}", id));

            return rows.GetLength(0) == 1
                ? (T)CreateObject<T>(rows[0])
                : null;
        }

        public IList<T> GetAll<T>() where T : DomainObject, new()
        {
            Type objectType = typeof(T);

            DataRow[] rows = domainDataService.GetObjectData(objectType, String.Empty);

            List<T> list = new List<T>(rows.GetLength(0));
            foreach (DataRow row in rows)
            {
                list.Add((T)CreateObject<T>(row));
            }

            return list;
        }

        /// <summary>
        /// Сохраняет объект модели в базе данных.
        /// В случае, если он новый, выполняется операция INSERT, иначе - UPDATE.
        /// </summary>
        /// <typeparam name="T">Тип объекта.</typeparam>
        /// <param name="obj">Объект модели.</param>
        public void Save<T>(T obj) where T : DomainObject
        {
            if (obj.IsNew())
            {
                domainDataService.Create(obj);
                obj.SetPersisted();
            }
            else
            {
                domainDataService.Update(obj);
            }
        }

        /// <summary>
        /// Удаляет объект модели из базы данных.
        /// </summary>
        /// <typeparam name="T">Тип объекта.</typeparam>
        /// <param name="id">Id удаляемой записи.</param>
        public void Delete<T>(int id) where T : DomainObject
        {
            
        }

        /// <summary>
        /// Удаляет объект модели из базы данных.
        /// </summary>
        /// <typeparam name="T">Тип объекта.</typeparam>
        /// <param name="obj">Объект модели.</param>
        public void Delete<T>(T obj) where T : DomainObject
        {

        }

        /// <summary>
        /// Создает объект указанного типа и наполняет его данными.
        /// </summary>
        /// <param name="row">Данные объекта.</param>
        private static object CreateObject<T>(DataRow row) where T : DomainObject, new()
        {
            // Создаем экземпляр объекта
            T obj = new T();

            // Устанавливаем признак того, что объект взят из хранилища
            obj.SetPersisted();

            // Наполняем экземпляр данными);
            foreach (PropertyInfo propertyInfo in obj.GetType().GetProperties())
            {
                object value = row[propertyInfo.Name.ToUpper()];
                if (propertyInfo.PropertyType.FullName.StartsWith("Krista"))
                {
                    value = null;
                }
                else
                {
                    value = value is DBNull ? null : Convert.ChangeType(value, propertyInfo.PropertyType);
                }
                propertyInfo.SetValue(obj, value, null);
            }
            return obj;
        }
    }
}
