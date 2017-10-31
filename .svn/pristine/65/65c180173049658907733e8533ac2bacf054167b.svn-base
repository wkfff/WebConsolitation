using System;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.Security.Permissions;

namespace Krista.FM.Common
{
    /// <summary>
    /// Реализация клиентского приемника. 
    /// Задача - положить имя хоста в заголовок запроса
    /// </summary>
    public class ReplaceIPAddressClientChannelSink : BaseChannelSinkWithProperties,
       IClientChannelSink
    {
        private readonly String serverHostNameOrIp;
        private readonly IClientChannelSink nextSink;

        [SecurityPermission(SecurityAction.LinkDemand)]
        public ReplaceIPAddressClientChannelSink(IClientChannelSink sink, String serverHostNameOrIp)
        {
            if (sink == null)
                throw new ArgumentNullException("sink");

            this.serverHostNameOrIp = serverHostNameOrIp;
            nextSink = sink;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
        public void ProcessMessage(IMessage msg, ITransportHeaders requestHeaders, Stream requestStream,
            out ITransportHeaders responseHeaders, out Stream responseStream)
        {
            requestHeaders["serverHostNameOrIp"] = serverHostNameOrIp;

            nextSink.ProcessMessage(msg, requestHeaders, requestStream,
                out responseHeaders, out responseStream);
        }

        public void AsyncProcessRequest(IClientChannelSinkStack sinkStack, IMessage msg, ITransportHeaders headers, Stream stream)
        {
        }

        public void AsyncProcessResponse(IClientResponseChannelSinkStack sinkStack, object state, ITransportHeaders headers, Stream stream)
        {
        }

        public Stream GetRequestStream(IMessage msg, ITransportHeaders headers)
        {
            return null;
        }

        public IClientChannelSink NextChannelSink
        {
            get { return nextSink; }
        }
    }
}
