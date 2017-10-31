using Krista.FM.RIA.Core.ExtensionModule;
using Krista.FM.RIA.Core.Extensions;

namespace Krista.FM.RIA.Core.Gui
{
    public class NavigationFactoty
    {
        public Navigation CreateNavigation(string type, IParametersService parametersService)
        {
            Navigation navigation = null;
            switch (type)
            {
                case "tree": 
                    navigation = new NavigationTree(parametersService); 
                    break;
                case "list":
                    navigation = new NavigationList(parametersService); 
                    break;
            }

            return navigation;
        }
    }
}
