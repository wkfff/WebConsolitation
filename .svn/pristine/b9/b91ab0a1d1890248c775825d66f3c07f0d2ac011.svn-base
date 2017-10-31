using System;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Services;

namespace Krista.FM.Common
{
    /// <summary>
    /// В контекст вызова помещается имя или адрес ремоутинг-сервера, видимый с клиента.
    /// Т.е., для одного клиента — "192.168...", для другого — "10.0..." и т.п.
    /// Когда объект маршаллится, достаете эту информацию из контекста.
    /// Подключается настроикой в конфигурационном файле сервера
    /// </summary>
    public class ReplaceIPAddressTrackingHandler : ITrackingHandler
    {
        private string defafaultIPAddressOrName;

        public void DisconnectedObject(object obj)
        {
        }

        public void MarshaledObject(object obj, System.Runtime.Remoting.ObjRef or)
        {
            if (String.IsNullOrEmpty(defafaultIPAddressOrName))
            {
                defafaultIPAddressOrName = GetDefaultIPAddressOrName(or);
            }

            Object serverHostNameOrIp = CallContext.GetData("serverHostNameOrIp") 
                ?? defafaultIPAddressOrName;

            if (serverHostNameOrIp != null)
            {
                foreach (Object channelData in or.ChannelInfo.ChannelData)
                {
                    if (channelData is ChannelDataStore)
                    {
                        ReplaceHostName((ChannelDataStore)channelData, serverHostNameOrIp.ToString());
                    }
                }
            }
        }

        public void UnmarshaledObject(object obj, System.Runtime.Remoting.ObjRef or)
        {
        }

        private void ReplaceHostName(ChannelDataStore dataStore, String serverHostNameOrIp)
        {
            for (Int32 i = 0; i < dataStore.ChannelUris.Length; i++)
            {
                UriBuilder ub = new UriBuilder(dataStore.ChannelUris[i]);
                if (String.Compare(ub.Host, serverHostNameOrIp, true) != 0)
                {
                    ub.Host = serverHostNameOrIp;
                    dataStore.ChannelUris[i] = ub.ToString();
                }
            }
        }

        private string GetDefaultIPAddressOrName(System.Runtime.Remoting.ObjRef or)
        {
            string ip = string.Empty;
            foreach (Object channelData in or.ChannelInfo.ChannelData)
            {
                if (channelData is ChannelDataStore)
                {
                    for (Int32 i = 0; i < ((ChannelDataStore)channelData).ChannelUris.Length; i++)
                    {
                        UriBuilder ub = new UriBuilder(((ChannelDataStore)channelData).ChannelUris[i]);
                        ip = ub.Host;
                    }
                }
            }

            return ip;
        }
    }
}
