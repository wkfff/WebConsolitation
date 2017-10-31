using System.IO;

namespace Krista.FM.Extensions
{
    public static class StreamExtensions
    {
        public static Stream CopyTo(this Stream input, Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }

            return input;
        }
    }
}
