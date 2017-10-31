using System.IO;
using System.Security.Cryptography.Xml;
using System.Xml.XPath;

namespace Krista.FM.DigitalSignature
{
    public static class XmlC14NTransformAlgorithm
    {
        /// <summary>
        /// Преобразует xml-документ к канонической форме в соответствии с алгоритмом 
        /// </summary>
        /// <param name="xmlDoc">Исходный xml-документ.</param>
        /// <returns>Преобразованный xml-документ представленный в виде строки.</returns>
        public static string Convert(IXPathNavigable xmlDoc)
        {
            var xmlTransform = new XmlDsigC14NTransform { Algorithm = SignedXml.XmlDsigC14NTransformUrl };

            // Передаем документ в трансформатор
            xmlTransform.LoadInput(xmlDoc);

            MemoryStream outputStream = (MemoryStream)xmlTransform.GetOutput(typeof(Stream));
            using (StreamReader streamReader = new StreamReader(outputStream))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}