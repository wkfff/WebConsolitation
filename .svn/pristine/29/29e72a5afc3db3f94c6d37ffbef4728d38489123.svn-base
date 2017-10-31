using System.Collections;
using System.Net;
using System.Runtime.Remoting.Channels;
using System.Security.Permissions;

namespace Krista.FM.Common
{
    /// <summary>
    /// Клиентский провайдер
    /// Подключается настроикой в конфигурационном файле клиента
    /// </summary>
    public class ReplaceIPAddressClientChannelSinkProvider : IClientChannelSinkProvider
    {
        private IClientChannelSinkProvider nextProvider;
        private readonly string ip;

        public ReplaceIPAddressClientChannelSinkProvider(IDictionary props,
                                            ICollection providerData)
        {
            // лучаем ip-адресс сервера, таким каким его видит клиентское приложение
            IPHostEntry ihe = Dns.GetHostByName((string) props["serverName"]);
            ip = ihe.AddressList.Length > 0
                ? ihe.AddressList[0].ToString()
                : "127.0.0.1";
        }

        public IClientChannelSinkProvider Next
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
            get
            {
                return (nextProvider);
            }
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
            set
            {
                nextProvider = value;
            }
        }

        public IClientChannelSink CreateSink(IChannelSender channel, string url, object remoteChannelData)
        {
            IClientChannelSink nextSink = Next.CreateSink(channel, url, remoteChannelData);

            return (new ReplaceIPAddressClientChannelSink(nextSink, ip));
        }
    }
}
