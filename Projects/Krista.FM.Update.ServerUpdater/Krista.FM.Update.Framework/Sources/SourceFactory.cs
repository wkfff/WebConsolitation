using System;
using Krista.FM.Update.Framework.Utils;

namespace Krista.FM.Update.Framework.Sources
{
    public class SourceFactory
    {
        public static FileDownloaderAbstract CreateSourceFactory(ConnectionType connectionType, string proxy, string proxyPort, string user, string password)
        {
            switch (connectionType)
            {
                case ConnectionType.http:
                case ConnectionType.tcp:
                    return new FileDownloader(proxy, proxyPort, user, password);
                case ConnectionType.ftp:
                    return new FtpDownloader();
                default:
                    throw new Exception(String.Format("Не известный тип соединения: {0}", connectionType));
            }
        }
    }
}
