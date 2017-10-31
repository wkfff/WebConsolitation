using System;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.Security.Permissions;

namespace Krista.FM.Common
{
    /// <summary>
    /// Реализация серверного приемника.
    /// Задача - достать имя хоста и поместить его в контекст вызова.
    /// </summary>
    public class ReplaceIPAddressServerChannelSink : BaseChannelSinkWithProperties, IServerChannelSink
    {
        private Object serverHostNameOrIp;
        private readonly IServerChannelSink nextSink;

        [SecurityPermission(SecurityAction.LinkDemand)]
        public ReplaceIPAddressServerChannelSink(IServerChannelSink sink)
        {
            if (sink == null)
                throw new ArgumentNullException("sink");

            nextSink = sink;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
        public ServerProcessing ProcessMessage(IServerChannelSinkStack sinkStack, IMessage requestMsg,
            ITransportHeaders requestHeaders, Stream requestStream, out IMessage responseMsg,
            out ITransportHeaders responseHeaders, out Stream responseStream)
        {
            serverHostNameOrIp = requestHeaders["serverHostNameOrIp"];

            if (serverHostNameOrIp != null)
            {
                CallContext.SetData("serverHostNameOrIp", serverHostNameOrIp);
            }
            else
            {
                CallContext.FreeNamedDataSlot("serverHostNameOrIp");
            }

            sinkStack.Push(this, null);
            ServerProcessing status = NextChannelSink.ProcessMessage(sinkStack, requestMsg, requestHeaders,
                requestStream, out responseMsg, out responseHeaders, out responseStream);

            CallContext.FreeNamedDataSlot("serverHostNameOrIp");

            return status;
        }

        public void AsyncProcessResponse(IServerResponseChannelSinkStack sinkStack, object state, IMessage msg, ITransportHeaders headers, Stream stream)
        {
        }

        public Stream GetResponseStream(IServerResponseChannelSinkStack sinkStack, object state, IMessage msg, ITransportHeaders headers)
        {
            return null;
        }

        public IServerChannelSink NextChannelSink
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
            get
            {
                return (nextSink);
            }
        }
    }
}
