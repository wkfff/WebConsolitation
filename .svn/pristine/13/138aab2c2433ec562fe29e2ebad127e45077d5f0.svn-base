using System;
using System.IO;
using System.Text;

namespace Krista.FM.Common.TaskDocuments.OfficeCustomPropertiesAdapters
{
    public abstract class OfficeCustomPropertiesAdapter : IDisposable
    {
        public abstract object GetProperty(string name);
        public abstract void SetProperty(string name, object value);
        public abstract void Clear();
        public abstract void Save();
        public abstract void Dispose();

        public virtual void WriteData(string name, Stream stream)
        {
            byte[] buff = new byte[stream.Length];
            stream.Read(buff, 0, buff.Length);
            string base64 = Convert.ToBase64String(buff);
            stream.Close();
                
            TextReader reader = new StringReader(base64);
                
            const int maxBlockSize = 255;
            char[] buffer = new char[maxBlockSize];
            int currentPos = 0;
            int propertyIndex = 0;
            while (true)
            {

                int readedBlockSize = reader.Read(buffer, 0, maxBlockSize);
                if (readedBlockSize == 0)
                    break;

                
                SetProperty(String.Format("{0}.{1}", name, propertyIndex++), 
                    new StringBuilder().Append(buffer, 0, readedBlockSize).ToString());

                currentPos += readedBlockSize;
            }

            // Размер данных
            SetProperty(String.Format("{0}.Size", name), currentPos);
        }
    }
}
