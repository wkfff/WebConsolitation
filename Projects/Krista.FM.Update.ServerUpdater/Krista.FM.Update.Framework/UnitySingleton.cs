using Krista.FM.Update.Framework.FeedReaders;
using Krista.FM.Update.Framework.Sources;
using Microsoft.Practices.Unity;

namespace Krista.FM.Update.Framework
{
    /// <summary>
    /// Cинглтон контейнера Unity
    /// </summary>
    public static class UnitySingleton
    {
        static readonly IUnityContainer _instance = new UnityContainer();

        static UnitySingleton()
        {
            //Здесь будем конфигурировать контейнер
            _instance
                .RegisterType<IUpdateFeedReader, XmlFeedReader>()
                .RegisterType<IUpdateSource, SimpleWebSource>();
        }

        public static IUnityContainer Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}
