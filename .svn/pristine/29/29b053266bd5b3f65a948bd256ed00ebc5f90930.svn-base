using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Krista.FM.Common
{
    public class DocumentsHelper
    {
        public static byte[] CompressFile(byte[] compressedFileBufer)
        {
            MemoryStream ms = new MemoryStream();
            DeflateStream compressStream = new DeflateStream(ms, CompressionMode.Compress, true);
            compressStream.Write(compressedFileBufer, 0, compressedFileBufer.Length);
            compressStream.Close();
            return ms.ToArray();
        }

        private static byte[] GetBuferFromStream(MemoryStream stream)
        {
            return stream.ToArray();
        }

        public static int ReadAllBytesFromStream(Stream stream, List<byte> list)
        {
            try
            {
                int offset = 0;
                int totalCount = 0;
                while (true)
                {
                    int byteRead = stream.ReadByte();

                    if (byteRead == -1)
                        break;
                    list.Add((byte)byteRead);
                    offset++;
                    totalCount++;
                }
                return totalCount;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static byte[] DecompressFile(byte[] compressedFileBufer)
        {
            MemoryStream ms = null;
            DeflateStream decompressedzipStream = null;
            try
            {
                ms = new MemoryStream(compressedFileBufer);
                ms.Write(compressedFileBufer, 0, compressedFileBufer.Length);
                decompressedzipStream = new DeflateStream(ms, CompressionMode.Decompress, true);
                ms.Position = 0;
                List<byte> list = new List<byte>();
                ReadAllBytesFromStream(decompressedzipStream, list);
                return list.ToArray();
            }
            finally
            {
                decompressedzipStream.Close();
            }
        }
    }
}
