using System.Collections.Generic;
using System.Linq;

namespace Krista.FM.RIA.Core.ExtensionModule.Services
{
    public class NavigationService
    {
        private readonly List<Navigation> navigation = new List<Navigation>();

        public void AddNavigation(params Navigation[] navs)
        {
            navigation.AddRange(navs);
        }

        public IEnumerable<Navigation> GetNavigations()
        {
            // Ideally it would have a different navigation tree for each page
            return navigation.OrderBy(x => x.OrderPosition);
        }

        public void Clear()
        {
            navigation.Clear();
        }
    }
}
