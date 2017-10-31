using System;
using System.Data;
using System.Reflection;
using Krista.FM.Extensions;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Domain.Reporitory
{
    /// <summary>
    /// Предоставляет доступ к данным объектов схемы.
    /// </summary>
    public class SchemeDataService : IDomainDataService
    {
        private readonly IScheme scheme;

        public SchemeDataService(IScheme scheme)
        {
            this.scheme = scheme;
        }

        /// <summary>
        /// Возвращает таблицу с данными для указанного типа объекта.
        /// </summary>
        /// <param name="objectType">Тип объекта.</param>
        /// <param name="selectFilter">Условие фильтрации даннных.</param>
        public DataRow[] GetObjectData(Type objectType, string selectFilter)
        {
            // Получаем метаданные объекта по его уникальному ключу
            string objectKey = Convert.ToString(objectType.InvokeMember("Key",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField,
                null, objectType, null));
            IEntity entity = scheme.RootPackage.FindEntityByName(objectKey);

            // Получаем данные
            DataTable table = new DataTable();
            using (IDataUpdater du = entity.GetDataUpdater(selectFilter, null, null))
            {
                du.Fill(ref table);
            }
            return table.Select();
        }

        public void Create(DomainObject obj)
        {
            IEntity entity = scheme.RootPackage.FindEntityByName(obj.GetKey());

            // Получаем данные
            DataTable table = new DataTable();
            using (IDataUpdater du = entity.GetDataUpdater("1=0", null, null))
            {
                du.Fill(ref table);
                table.AddRow(obj, entity.GetGeneratorNextValue);
                du.Update(ref table);
            }
        }

        public void Update(DomainObject obj)
        {
            IEntity entity = scheme.RootPackage.FindEntityByName(obj.GetKey());

            // Получаем данные
            DataTable table = new DataTable();
            using (IDataUpdater du = entity.GetDataUpdater(
                "ID={0}".FormatWith(obj.ID), null, null))
            {
                du.Fill(ref table);
                table.Rows[0].UpdateRow(obj);
                du.Update(ref table);
            }
        }
    }
}
