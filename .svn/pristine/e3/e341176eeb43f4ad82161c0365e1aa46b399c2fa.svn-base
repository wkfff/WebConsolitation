using System.Collections;
using System.Runtime.Remoting.Channels;
using System.Security.Permissions;

namespace Krista.FM.Common
{
    /// <summary>
    /// Серверный провайдер.
    /// Подключается настроикой в конфигурационном файле сервера
    /// </summary>
    public class ReplaceIPAddressServerChannelSinkProvider : IServerChannelSinkProvider
    {
        public ReplaceIPAddressServerChannelSinkProvider(IDictionary props,
                                            ICollection providerData)
        {
        }

        private IServerChannelSinkProvider nextProvider;

        public IServerChannelSinkProvider Next
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

        public IServerChannelSink CreateSink(IChannelReceiver channel)
        {
            IServerChannelSink nextSink = Next.CreateSink(channel);
            return new ReplaceIPAddressServerChannelSink(nextSink);
        }

        public void GetChannelData(IChannelDataStore channelData)
        {
        }
    }
}
