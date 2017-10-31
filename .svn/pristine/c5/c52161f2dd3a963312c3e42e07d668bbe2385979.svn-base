using System.Web.Routing;
using Krista.FM.RIA.Core.ExtensionModule.Services;
using Krista.FM.RIA.Core.Extensions;
using Microsoft.Practices.Unity;

namespace Krista.FM.RIA.Core
{
    /// <summary>
    /// Инсталятор мудуля расширения.
    /// </summary>
    public interface IExtensionInstaller : IExtensionConfig
    {
        /// <summary>
        /// Идентификатор модуля.
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// Регистрация маршрутов.
        /// </summary>
        void InstallRoutes(RouteCollection routes);

        void RegisterTypes(IUnityContainer container);
        
        void RegisterParameters(IParametersService parametersService);

        void ConfigureClientExtension(ClientExtensionService clientExtensionService);

        void ConfigureNavigation(NavigationService navigationService, IParametersService parametersService);
        
        void ConfigureWindows(WindowService windowService);
        
        void ConfigureViews(ViewService viewService, IParametersService parametersService);
    }
}
