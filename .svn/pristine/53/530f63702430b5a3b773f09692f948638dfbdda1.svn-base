using System.IO.Compression;
using System.Web;
using System.Web.Mvc;

namespace Krista.FM.RIA.Core.Filters
{
    public class CompressAttribute : ActionFilterAttribute
    {
        private enum CompressionScheme
        {
            /// <summary>
            /// Метод сжатия Deflate.
            /// </summary>
            Deflate,
            
            /// <summary>
            /// Метод сжатия Gzip.
            /// </summary>
            Gzip,

            /// <summary>
            /// Без сжатия.
            /// </summary>
            None
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var preferred = GetPreferredEncoding(filterContext.HttpContext.Request);
            
            var response = filterContext.HttpContext.Response;

            if (preferred == CompressionScheme.Gzip)
            {
                response.AppendHeader("Content-encoding", "gzip");
                response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
            }

            if (preferred == CompressionScheme.Deflate)
            {
                response.AppendHeader("Content-encoding", "deflate");
                response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
            }
        }

        private static CompressionScheme GetPreferredEncoding(HttpRequestBase request)
        {
            var acceptableEncoding = request.Headers["Accept-Encoding"];
            if (acceptableEncoding == null)
            {
                // Некоторые прокси-сервера режут заголовки запросов, заголовка Accept-Encoding может не быть
                // bug #22681
                return CompressionScheme.None;
            }
                
            acceptableEncoding = SortEncodings(acceptableEncoding);

            // Get the preferred encoding format 
            if (acceptableEncoding.Contains("gzip"))
            {
                return CompressionScheme.Gzip;
            }

            if (acceptableEncoding.Contains("deflate"))
            {
                return CompressionScheme.Deflate;
            }

            return CompressionScheme.None;
        }

        private static string SortEncodings(string header)
        {
            return header;
        }
    }
}
