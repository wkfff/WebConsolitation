using System;
using System.Collections;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Security;
using System.Security.Principal;

namespace Krista.FM.Update.Framework
{
    public static class UpdateManagerFactory
    {
        private static bool _isClientNotRegistered = true;
        private static bool _isServerNotRegistered = true;
        private const string _dataSourceUri = "B7167E88-14FC-4023-AF96-CB1E50E7CE5A";
        private static readonly object _threadSafetyObject = new object();

        public static IUpdateManager CreateServerUpdateManager()
        {
            Console.WriteLine("Старт");
            UpdateManager updateManager = UpdateManager.Instance;

            lock (UpdateManagerFactory._threadSafetyObject)
            {
                if (UpdateManagerFactory._isServerNotRegistered)
                {
                    try
                    {
                        var si = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                        IdentityReference ir = si.Translate(typeof(NTAccount));

                        IDictionary properties = new Hashtable();
                        properties["machineName"] = System.Environment.MachineName;
                        properties["port"] = "9090";
                        properties.Add("authorizedGroup", ir.Value);//"Everyone");
                        properties.Add("impersonationLevel", "Identification");
                        properties.Add("exclusiveAddressUse", "false");

                        BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider();
                        serverProvider.TypeFilterLevel = TypeFilterLevel.Full;

                        if (0 == ChannelServices.RegisteredChannels.Where(t => t.ChannelName == "remote").Count())
                        {
                            TcpChannel channel = new TcpChannel(properties, null, serverProvider);
                            ChannelServices.RegisterChannel(channel, true);
                        }

                        RemotingConfiguration.RegisterWellKnownServiceType(
                            typeof(UpdateManager),
                            _dataSourceUri, WellKnownObjectMode.Singleton);

                        RemotingServices.Marshal(updateManager,
                                                 _dataSourceUri, typeof(UpdateManager));

                        _isServerNotRegistered = false;
                        _isClientNotRegistered = true;
                    }
                    catch (SecurityException e)
                    {
                        throw new FrameworkRemotingException(e.Message, e);
                    }
                    catch(RemotingException e)
                    {
                        throw new FrameworkRemotingException(e.Message, e);
                    }
                }
            }

            return updateManager;
        }

        public static IUpdateManager CreateUpdateManager(bool isRemote)
        {
            if (isRemote)
            {
                return UpdateManagerFactory.CreateRemoteUpdateManager();
            }

            return UpdateManagerFactory.CreateLocalUpdateManager();
        }

        private static IUpdateManager CreateLocalUpdateManager()
        {
            return UpdateManager.Instance;
        }

        private static IUpdateManager CreateRemoteUpdateManager()
        {
            IUpdateManager manager = null;
            try
            {
                string serverUrl = String.Empty;
                lock (_threadSafetyObject)
                {
                    IChannel channel = CheckRegisterChannel();

                    // сервер и клиент работают на одной машине
                    const string server = "localhost";

                    serverUrl = string.Format("tcp://{0}:9090/{1}",
                                                server, _dataSourceUri);

                    WellKnownClientTypeEntry entry = null;

                    foreach (WellKnownClientTypeEntry registeredWellKnownClientType in RemotingConfiguration.GetRegisteredWellKnownClientTypes())
                    {
                        if (registeredWellKnownClientType.ObjectUrl == serverUrl)
                        {
                            entry = registeredWellKnownClientType;
                        }
                    }

                    if (entry == null)
                    {
                        RemotingConfiguration.RegisterWellKnownClientType(
                            typeof (UpdateManager), serverUrl);
                    }
                }

                manager = (UpdateManager)Activator.GetObject(typeof(UpdateManager), serverUrl);
                // тестовое обращение, чтоб удостовериться что подключение прошло успешно
                manager.Activate();
            }
            catch (SecurityException e)
            {
                throw new FrameworkRemotingException(e.Message, e);
            }
            catch(RemotingException e)
            {
                throw new FrameworkRemotingException(e.Message, e);
            }

            return manager;
        }

        /// <summary>
        /// Регистрируем на клиенте канал, чтобы была возможность
        /// взаимодействия сервера c клиентами
        /// </summary>
        /// <returns></returns>
        private static IChannel CheckRegisterChannel()
        {
            BinaryClientFormatterSinkProvider clientProvider = new BinaryClientFormatterSinkProvider();
            BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider
                                                                   {TypeFilterLevel = TypeFilterLevel.Full};

            if (ChannelServices.RegisteredChannels.Where(t => t.ChannelName == "IPC Client Channel").Count() != 0)
                return ChannelServices.RegisteredChannels.Where(t => t.ChannelName == "IPC Client Channel").First();

            var si = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            IdentityReference ir = si.Translate(typeof(NTAccount));

            IDictionary properties = new Hashtable
                                         {
                                             {"exclusiveAddressUse", "false"},
                                             {"port", "0"}
                                         };

            TcpChannel channel = new TcpChannel(properties, clientProvider, serverProvider);
            ChannelServices.RegisterChannel(channel, true);

            return channel;
        }
    }
}
