using System;

using Krista.FM.ServerLibrary;
using Krista.FM.Common;

namespace Krista.FM.Server.Scheme
{
    public sealed class SchemeStub : MarshalByRefObject, ISchemeStub
    {
        #region Поля

        /// <summary>
        /// Работающий экземпляр схемы.
        /// </summary>
        private IServer server;

        /// <summary>
        /// Работающий экземпляр схемы.
        /// </summary>
        private IScheme scheme;

        /// <summary>
        /// Наименование схемы
        /// </summary>
        private string name;

        /// <summary>
        /// Путь к конфигурационному файлу схемы
        /// </summary>
        private string path;

        #endregion Поля

        /// <summary>
        /// Конструктор объекта
        /// </summary>
        /// <param name="server"></param>
        /// <param name="name">Наименование схемы</param>
        /// <param name="path">Путь к конфигурационному файлу схемы</param>
        public SchemeStub(IServer server, string name, string path)
        {
            this.server = server;
            this.name = name;
            this.path = path;
        }


        /// <summary>
        /// Запускает схему. Позволет подключаться к схеме только администратору.
        /// Для того, чтобы обычные пользователи могли подключиться к схеме необходимо вызвать метод Open.
        /// </summary>
        public void Startup()
        {
            try
            {
                UnityStarter.Initialize();

                SchemeClass startScheme = (SchemeClass)Resolver.Get<IScheme>();

                startScheme.Server = server;
                startScheme.Name = name;

                if (startScheme.Initialize(this.path))
                    scheme = startScheme;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Закрывает и выгружает схему
        /// </summary>
        public void Shutdown()
        {
            try
            {
                if (scheme != null)
                {
                    scheme.Dispose();
                    scheme = null;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(String.Format("Ошибка освобождения схемы {0}: {1}", name, e));
            }
        }

        public string Connect()
        {
            return ((SchemeClass)scheme).Connect();
        }

        public void Disconnect()
        {
            ((SchemeClass)scheme).Disconnect();
        }

        /// <summary>
        /// Реальный объект схемы
        /// </summary>
        public IScheme Scheme
        {
            get { return scheme; }
        }
    }
}