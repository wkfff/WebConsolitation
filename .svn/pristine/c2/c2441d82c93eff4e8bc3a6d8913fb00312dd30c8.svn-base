using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Controllers;
using Krista.FM.RIA.Core.Controllers.Binders;
using Krista.FM.RIA.Core.ExtensionModule.Services;
using Krista.FM.RIA.Core.Principal;
using Krista.FM.RIA.Core.Progress;

using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.Web.Mvc;
using Unity.AutoRegistration;
using Krista.FM.RIA.Core.Extensions;

namespace Krista.FM.RIA
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class RiaMvcApplication : MvcApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{exclude}/{extnet}/ext.axd");

            routes.Add(
                new Route(
                    "View/{viewId}",
                    new RouteValueDictionary(new { controller = "View", action = "Show", viewId = "" }),
                    new DynamicRouteHandler())
            );

            routes.Add(
                new Route(
                    "{controller}/{action}/{id}",
                    new RouteValueDictionary(new { controller = "Home", action = "Index", id = "" }),
                    new DynamicRouteHandler())
            );
        }

        protected override void Application_Start()
        {
            // Настройка системы логирования
            log4net.Config.XmlConfigurator.Configure();
            log4net.LogManager.GetRepository().PluginMap.Add(new log4net.Plugin.RemoteLoggingServerPlugin("LoggingSink"));
            Trace.TraceInformation("Application_Start");

            base.Application_Start();

            // Регистрация провайдера виртуальных ресурсов
            HostingEnvironment.RegisterVirtualPathProvider(new AssemblyResourceVirtualPathProvider());

            RegisterRoutes(RouteTable.Routes);

            // Регистрируем фабрику провайдера значений JsonValueProviderFactory
            ValueProviderFactories.Factories.Add(new JsonValueProviderFactoryWraper());

            ModelBinders.Binders.Add(typeof(int[]), new IntegerArrayModelBinder());

            // Создаем IoC контейнер
            var container = new UnityContainer();
            RegisterTypes(container);
            RegisterControllerFactory(container);
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));

            // Инициализация подключаемых модулей расширений
            InitializeExtensions(
                container.ResolveAll<IExtensionInstaller>(),
                container);
        }

        private static void RegisterControllerFactory(IUnityContainer container)
        {
            var factory = new UnityControllerFactory(container);
            ControllerBuilder.Current.SetControllerFactory(factory);
        }

        /// <summary>
        /// Рекистрация типов с IoC контейнере.
        /// </summary>
        /// <param name="container">IoC контейнер.</param>
        protected override void RegisterTypes(IUnityContainer container)
        {
            base.RegisterTypes(container);

            container.RegisterInstance(container);
            container.RegisterType<IMembershipService, AccountMembershipService>();
            container.RegisterType<IFormsAuthentication, FormsAuthenticationService>();
            container.RegisterType<NavigationService, NavigationService>(new SessionLifetimeManager<NavigationService>());
            container.RegisterType<ViewService, ViewService>(new SessionLifetimeManager<ViewService>());
            container.RegisterType<WindowService, WindowService>(new SessionLifetimeManager<WindowService>());
            container.RegisterType<ClientExtensionService, ClientExtensionService>(new SessionLifetimeManager<ClientExtensionService>());
            container.RegisterType<IParametersService, ParametersService>(new SessionLifetimeManager<IParametersService>());
            container.RegisterType<IEntityDataService, EntityDataService>(new PerResolveLifetimeManager());
            container.RegisterType<IDomainDataService, SchemeDataService>(new PerResolveLifetimeManager());
            container.RegisterType<IPrincipalProvider, PrincipalProvider>(new SessionLifetimeManager<PrincipalProvider>());
            container.RegisterType<IProgressDataProvider, AspnetProgressProvider>();
            container.RegisterType<IProgressManager, ProgressManager>(new TransientLifetimeManager());

            try
            {
                // Автоматическая регистрация типов
                container
                    .ConfigureAutoRegistration()
                    .LoadAssembliesFrom(Directory.GetFiles(HttpRuntime.BinDirectory, "Krista.FM.RIA.Extensions.*.dll"))
                    .ExcludeSystemAssemblies()
                    .ExcludeAssemblies(a => a.GetName().FullName.StartsWith("Accessibility"))
                    .ExcludeAssemblies(a => a.GetName().FullName.StartsWith("Anonymously"))
                    .ExcludeAssemblies(a => a.GetName().FullName.StartsWith("Antlr3"))
                    .ExcludeAssemblies(a => a.GetName().FullName.StartsWith("Antlr4"))
                    .ExcludeAssemblies(a => a.GetName().FullName.StartsWith("App_"))
                    .ExcludeAssemblies(a => a.GetName().FullName.StartsWith("Castle"))
                    .ExcludeAssemblies(a => a.GetName().FullName.StartsWith("CppCodeProvider"))
                    .ExcludeAssemblies(a => a.GetName().FullName.StartsWith("Ext.Net"))
                    .ExcludeAssemblies(a => a.GetName().FullName.StartsWith("DynamicProxyGenAssembly"))
                    .ExcludeAssemblies(a => a.GetName().FullName.StartsWith("FluentNHibernate"))
                    .ExcludeAssemblies(a => a.GetName().FullName.StartsWith("ICSharpCode"))
                    .ExcludeAssemblies(a => a.GetName().FullName.StartsWith("Iesi"))
                    .ExcludeAssemblies(a => a.GetName().FullName.StartsWith("Interop"))
                    .ExcludeAssemblies(a => a.GetName().FullName.StartsWith("Krista.Diagnostics"))
                    .ExcludeAssemblies(a => a.GetName().FullName.StartsWith("Krista.FM.Client.SMO"))
                    .ExcludeAssemblies(a => a.GetName().FullName.StartsWith("Krista.FM.Common"))
                    .ExcludeAssemblies(a => a.GetName().FullName.StartsWith("Krista.FM.Domain"))
                    .ExcludeAssemblies(a => a.GetName().FullName.StartsWith("Krista.FM.Extensions"))
                    .ExcludeAssemblies(a => a.GetName().FullName.StartsWith("Krista.FM.ServerLibrary"))
                    .ExcludeAssemblies(a => a.GetName().FullName.StartsWith("log4net"))
                    .ExcludeAssemblies(a => a.GetName().FullName.StartsWith("Microsoft"))
                    .ExcludeAssemblies(a => a.GetName().FullName.StartsWith("NCalc"))
                    .ExcludeAssemblies(a => a.GetName().FullName.StartsWith("Newtonsoft"))
                    .ExcludeAssemblies(a => a.GetName().FullName.StartsWith("NHibernate"))
                    .ExcludeAssemblies(a => a.GetName().FullName.StartsWith("NPOI"))
                    .ExcludeAssemblies(a => a.GetName().FullName.StartsWith("protobuf"))
                    .ExcludeAssemblies(a => a.GetName().FullName.StartsWith("Remotion"))
                    .ExcludeAssemblies(a => a.GetName().FullName.StartsWith("SMDiagnostics"))
                    .ExcludeAssemblies(a => a.GetName().FullName.StartsWith("VJSharpCodeProvider"))
                    .ExcludeAssemblies(a => a.GetName().FullName.StartsWith("Unity"))
                    .ExcludeAssemblies(a => a.GetName().FullName.StartsWith("WebDev"))
                    .ExcludeAssemblies(a => a.GetName().FullName.StartsWith("zlib"))
                    .ExcludeAssemblies(a =>a.GetName().FullName.StartsWith("Quartz"))
                    .Include(If.Implements<IController>, Then.Register().UsingPerCallMode().WithTypeName())
                    .Include(If.Implements<IExtensionInstaller>, Then.Register()
                        .UsingSingletonMode()
                        .WithTypeName())
                    .Exclude(If.Is<SchemeBoundController>)
                    .ApplyAutoRegistration();

                // Регистрируем обобщенные репозитории
                container.RegisterType(typeof(ILinqRepository<>), typeof(NHibernateLinqRepository<>));
                container.RegisterType(typeof(IRepository<>), typeof(NHibernateRepository<>));
            }
            catch (ReflectionTypeLoadException reflectionTypeLoadException)
            {
                var ex = reflectionTypeLoadException.LoaderExceptions;

                foreach (Exception exception in ex)
                {
                    Trace.TraceCritical("Автоматическая регистрация типов: {0}",
                    Diagnostics.KristaDiagnostics.ExpandException(exception));
                }

                throw new ApplicationException(reflectionTypeLoadException.Message, reflectionTypeLoadException);
            }
            catch (Exception e)
            {
                Trace.TraceCritical("Автоматическая регистрация типов: {0}",
                    Diagnostics.KristaDiagnostics.ExpandException(e));
                
                throw new ApplicationException(e.Message, e);
            }
        }

        private void InitializeExtensions(IEnumerable<IExtensionInstaller> extensionInstallers, IUnityContainer container)
        {
            foreach (IExtensionInstaller installer in extensionInstallers)
            {
                installer.InstallRoutes(RouteTable.Routes);
                installer.RegisterTypes(container);
            }    
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            string url = HttpContext.Current.Request.RawUrl.ToLower();
            if (url.Contains("ext.axd"))
            {
                HttpContext.Current.SkipAuthorization = true;
            }
            else if (url.Contains("returnurl=/default.aspx") || url.Contains("returnurl=%2fdefault.aspx"))
            {
                Response.Redirect(url.Replace("returnurl=/default.aspx", "r=/").Replace("returnurl=%2fdefault.aspx", "r=/"));
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            Trace.TraceVerbose("Session_Start");
        }

        protected void Session_End(object sender, EventArgs e)
        {
            Trace.TraceVerbose("Session_End");

            ConnectionHelper.Disconnect(Session);
        }

        protected override void Application_End()
        {
            base.Application_End();

            Trace.TraceInformation("Application_End");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();

            Trace.TraceError("Application_Error {0}", Diagnostics.KristaDiagnostics.ExpandException(ex));
        }
    }
}