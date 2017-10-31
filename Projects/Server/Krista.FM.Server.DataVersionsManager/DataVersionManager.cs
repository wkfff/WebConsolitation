using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;
using System.Diagnostics;

namespace Krista.FM.Server.DataVersionsManager
{
    /// <summary>
    /// Ошибка при заполнение таблицы с версиями
    /// </summary
    [Serializable]
    public class DataVersionManagerException : Exception
    {
        public DataVersionManagerException()
        {
        }

        public DataVersionManagerException(string message, Exception innerException)
            : base(String.Format("{0}. Внутреннее исключение: {1}", message, innerException.Message), innerException)
        {
        }
    }

    /// <summary>
    /// Менеджер управления версиями
    /// </summary>
    public class DataVersionManager : DisposableObject, IDataVersionManager
    {
        /// <summary>
        /// Ссылка на объект схемы
        /// </summary>
        private IScheme scheme;
        /// <summary>
        /// Коллекция версий классификаторов
        /// </summary>
        private DataVersionsCollection dataVersionsCollection;

        /// <summary>
        /// Конструктор менеджера управления версиями классификатора
        /// </summary>
        /// <param name="scheme"></param>
        public DataVersionManager(IScheme scheme)
        {
            if (scheme == null)
				throw new ArgumentNullException("scheme");

            this.scheme = scheme;
            Initialize();
        }

        /// <summary>
        /// Инициализация параметров менеджера
        /// </summary>
        private void Initialize()
        {
            this.dataVersionsCollection = new DataVersionsCollection(this);
        }

        #region IDataVersionManager Members

        /// <summary>
        /// Ссылка на объект схемы
        /// </summary>
        public IScheme Scheme
        {
            get { return scheme; }
        }

        /// <summary>
        /// Заполнение таблицы с версиями классификаторов
        /// </summary>
        public void FillObjectVersionTable()
        {
            IDatabase db = scheme.SchemeDWH.DB;
            try
            {
                IPackage root = scheme.RootPackage;

                FillVersionsFromPackage(root);
            }
            catch (DataVersionManagerException ex)
            {
                Trace.WriteLine(String.Format("При заполнени таблицы с версиями возникла ошибка : {0}", ex));
                throw new ServerException(String.Format("При заполнени таблицы с версиями возникла ошибка : {0}", ex));
            }
            finally
            {
                db.Dispose();
            }
        }

        private void FillVersionsFromPackage(IPackage root)
        {
            try
            {
                foreach (IPackage package in root.Packages.Values)
                {
                    FillVersionsFromPackage(package);
                }

                foreach (IEntity entity in root.Classes.Values)
                {
                    IClassifier classifier = entity as IClassifier;
                    if (classifier != null)
                    {
                        if (classifier.IsDivided)
                            CreateVersion(classifier);
                    }
                }
            }
            catch (InvalidCastException ex)
            {
                throw new DataVersionManagerException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Создание версий классификатора на основе набора источников, по котрым заведены данные
        /// </summary>
        /// <param name="entity"></param>
        private void CreateVersion(IClassifier entity)
        {
            // получаем список источников, по которым заведены данные
            Dictionary<int, string> sources = scheme.DataSourceManager.GetDataSourcesNames(entity.FullDBName);

            int i = 1;
            foreach (KeyValuePair<int, string> pair in sources)
            {
                if (pair.Key != 0 && pair.Key != -1 && pair.Value != "Источник не найден")
                {
                    DataVersion dataVersion = (DataVersion)this.DataVersions.Create();
                    dataVersion.ObjectKey = entity.ObjectKey;
                    dataVersion.SourceID = pair.Key;
                    dataVersion.IsCurrent = false;
                    if (!String.IsNullOrEmpty(entity.Presentations.DefaultPresentation))
                    {
                        dataVersion.PresentationKey = entity.Presentations.DefaultPresentation;
                        dataVersion.Name = String.Format("{0}.{1}", entity.FullCaption,
                                                         entity.Presentations[entity.Presentations.DefaultPresentation].
                                                             Name);
                    }
                    else
                    {
                        dataVersion.Name = String.Format("{0}.Version{1}", entity.FullCaption, i);
                    }

                    if (!this.DataVersions.Containts(dataVersion))
                    {
                        this.DataVersions.Add(dataVersion);
                        i++;
                    }
                }
            }
        }

        #endregion

        #region IDataVersionManager Members

        /// <summary>
        /// Управление набором версий
        /// </summary>
        public IDataVersionsCollection DataVersions
        {
            get { return dataVersionsCollection; }
        }

        #endregion
    }
}
