using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

using Krista.FM.Common;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme
{
    internal class ParametersCollection : DictionaryBase<string, string>, IParametersCollection
    {
        public string path;

        /// <summary>
        /// Вызывае конструктор базавого класса и создаёт незаполненную коллекцию    
        /// </summary>
        public ParametersCollection()
        {
            this.path = "Parameters.xml";
            Initialize();
        }

        /// <summary>
        /// Вызывае конструктор базавого класса и создаёт коллекцию ,
        /// сам конструктор с присвоением пути к XML-документу и 
        /// инициализацией коллекции
        /// </summary>
        /// <param name="path">Путь к XML с параметрами коллекции</param>
        public ParametersCollection(string path)
        {
            this.path = path;
            Initialize();
        }

        /// <summary>
        /// Заполнение коллекции из XML-документа с предварительной очисткой
        /// </summary>
        public void Initialize()
        {
            try
            {
                list.Clear();

                XmlDocument doc = new XmlDocument();
                doc = Krista.FM.Common.Xml.XmlHelper.Load(this.path);

                XmlNode xmlParameters = doc.SelectSingleNode("/ServerConfiguration/Parameters");
                foreach (XmlNode item in xmlParameters.SelectNodes("Parameter"))
                    list.Add(item.Attributes["name"].Value, item.Attributes["value"].Value);

            }
            catch (Exception e)
            {
                Trace.WriteLine("Ошибка при инициализации коллекции параметров сервера : " + e.Message);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// свойство для работы с атрибутом 'readOnly' Semantics.xml 
        /// </summary>
        public bool ReadWrite
        {
            get
            {
                return Krista.FM.Common.FileUtils.FileHelper.IsFileReadOnly(this.path);
            }
            set
            {
                Krista.FM.Common.FileUtils.FileHelper.SetFileReadAccess(this.path, value);
            }
        }

        /// <summary>
        /// Индексатор для обращения к элементам коллекции как к массиву
        /// </summary>
        /// <param name="key">Имя элемента коллекции</param>
        /// <returns>Возвращает значение элемента с именем key</returns>
        public override string this[string key]
        {
            set
            {
                try
                {
                    if (ReadWrite)
                    {
                        throw new Exception(String.Format("Файл {0} открыт только для чтения", this.path));
                    }
                    XmlDocument doc = new XmlDocument();
                    doc = Krista.FM.Common.Xml.XmlHelper.Load(this.path);

                    XmlNode xmlParameters = doc.SelectSingleNode(String.Format("/ServerConfiguration/Parameters/Parameter[@name = '{0}']", key));
                    xmlParameters.Attributes["value"].Value = value;

                    Krista.FM.Common.Xml.XmlHelper.Save(doc, this.path);

                    list[key] = value;
                }
                catch (Exception e)
                {
                    Trace.WriteLine(String.Format("При изменении элемента с именем {1} на {2} возникла ошибка: {0} ", e.Message, key, value));
                    throw new Exception(e.Message, e);
                }
            }
        }
    }
}