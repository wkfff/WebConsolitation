using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml.Serialization;

namespace Krista.FM.Server.FinSourcePlanning.Services.IndicatorsService
{
    class XmlDeserealizeHelper
    {
        public static object GetHandler(byte[] objectData, Type objectType)
        {
            MemoryStream msCompressed = new MemoryStream();
            msCompressed.Write(objectData, 0, objectData.Length);
            DeflateStream decompressedzipStream = new DeflateStream(msCompressed, CompressionMode.Decompress, true);
            decompressedzipStream.BaseStream.Position = 0;
            MemoryStream msDecompressed = new MemoryStream();
            int dataByte = decompressedzipStream.ReadByte();
            while(dataByte != -1)
            {
                msDecompressed.WriteByte((byte)dataByte);
                dataByte = decompressedzipStream.ReadByte();
            }
            StreamReader reader = new StreamReader(msDecompressed, Encoding.UTF8);
            reader.BaseStream.Position = 0;
            XmlSerializer serializer = new XmlSerializer(objectType);
            return serializer.Deserialize(reader);
        }
    }
}
