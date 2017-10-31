using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Core
{
    /// <summary>
    /// Системный модуль (расширение).
    /// </summary>
    public class CoreExtension
    {
        private readonly IScheme scheme;

        private string[] allowedObjects;

        private bool initialized;

        public CoreExtension(IScheme scheme)
        {
            this.scheme = scheme;
        }

        public string[] AllowedObjects
        {
            get { return this.allowedObjects; }
        }

        public void Initialize()
        {
            if (initialized)
            {
                return;
            }

            this.allowedObjects = this.scheme.UsersManager.GetViewObjectsNamesAllowedForCurrentUser();
            initialized = true;
        }
    }
}
