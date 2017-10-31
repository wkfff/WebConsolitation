using System.Collections.Generic;

namespace Krista.FM.RIA.Core.ExtensionModule.Services
{
    public class ClientExtensionService
    {
        private readonly List<string> clientExtensions = new List<string>();

        public void AddClientExtension(string clientExtension)
        {
            clientExtensions.Add(clientExtension);
        }

        public IEnumerable<string> GetClientExtensions()
        {
            return clientExtensions;
        }

        public void Clear()
        {
            clientExtensions.Clear();
        }
    }
}
