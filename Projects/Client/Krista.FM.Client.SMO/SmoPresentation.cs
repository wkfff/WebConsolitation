using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO
{
    /// <summary>
    /// Smo объект для представления объекта
    /// </summary>
    public class SmoPresentation : SmoKeyIdentifiedObject<IPresentation>, IPresentation
    {
        public SmoPresentation(IPresentation serverObject)
            : this(serverObject, false)
        {
        }

        public SmoPresentation(IPresentation serverObject, bool cached)
            : base(serverObject, cached)
        {
        }


        public SmoPresentation(SMOSerializationInfo cache) 
            : base(cache)
        {
        }

        #region IPresentation Members

        public string Name
        {
            get
            {
                return serverControl.Name;
            }
            set
            {
                serverControl.Name = value;
            }
        }


        public IDataAttributeCollection Attributes
        {
            get
            {
                return
                    cached
                        ? (IDataAttributeCollection)
                          SmoObjectsCache.GetSmoObject(typeof (SMODataAttributeCollection),
                                                       GetCachedObject("Attributes"))
                        :
                            serverControl.Attributes;
            }
        }


        #endregion

        #region IPresentation Members

        public const string levelNamingConst = "LevelNamingTemplate";

        /// <summary>
        /// 
        /// </summary>
        public string LevelNamingTemplate
        {
            get
            {
                return serverControl.LevelNamingTemplate;
            }
            set
            {
                serverControl.LevelNamingTemplate = value;
            }
        }

        public virtual string Configuration
        {
            get
            {
                return serverControl.Configuration;
            }
        }

        public IDataAttributeCollection GroupedAttributes
        {
            get { return serverControl.GroupedAttributes; }
        }

        #endregion
    }
}
