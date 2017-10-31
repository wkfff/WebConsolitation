using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

using Krista.FM.Common.Xml;
using Krista.FM.Common;
using Krista.FM.Server.Scheme;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme
{
    internal class SemanticsCollection : DictionaryBase<string, string>, ISemanticsCollection, ISMOSerializable
    {
        public string path;
      
        /// <summary>
        /// Вызывает конструктор базавого класса и создаёт незаполненную коллекцию    
        /// </summary>
        public SemanticsCollection()
        {
            this.path = "Semantics.xml";
            Initialize();
        }

        /// <summary>
        /// Вызывает конструктор базавого класса и создаёт коллекцию ,
        /// сам конструктор с присвоением пути к XML-документу и 
        /// инициализацией коллекции
        /// </summary>
        /// <param name="path">Путь к XML с семантическими принадлежностями</param>
        public SemanticsCollection(string path)
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

                XmlDocument doc = Krista.FM.Common.Xml.XmlHelper.Load(this.path);

                XmlNode xmlSemantics = doc.SelectSingleNode("/ServerConfiguration/Semantics");
                foreach (XmlNode item in xmlSemantics.SelectNodes("Semantic"))
                    list.Add(item.Attributes["name"].Value, item.Attributes["caption"].Value);

            }
            catch (Exception e)
            {
                Trace.WriteLine("Ошибка при инициализации коллекции семантических принадлежностей: " + e.Message);
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
        /// Добавление новой записи в коллекцию семантических принадлежностей
        /// </summary>
        /// <param name="key">имя</param>
        /// <param name="value">значение</param>
        public override void Add(string key, string value)
        {
            try
            {
                ValidateKey(key);

                XmlDocument doc = XmlHelper.Load(this.path);
               
                XmlElement elem = doc.CreateElement("Semantic");
                              
                XmlNode attrName = doc.CreateNode(XmlNodeType.Attribute, "name", "");
                attrName.Value = key;
                elem.Attributes.SetNamedItem(attrName);

                XmlNode attrCap = doc.CreateNode(XmlNodeType.Attribute, "caption", "");
                attrCap.Value = value;
                elem.Attributes.SetNamedItem(attrCap);

                doc.SelectSingleNode("/ServerConfiguration/Semantics").AppendChild(elem);

                XmlHelper.Save(doc, this.path);

                list.Add(key, value);
            }
            catch (ArgumentException)
            {
                throw new Exception(String.Format("Запись с именем {0} уже существует", key));
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                throw new Exception(String.Format("Файл {0} не найден", this.path));
            }
        }

        /// <summary>
        /// Проверка имени семантики на уникальность независимо от регистра
        /// </summary>
        /// <param name="key">Имя семантики</param>
        private void ValidateKey(string key)
        {
            foreach (string item in list.Keys)
            {
                if (item.ToLower() == key.ToLower())
                    throw new ArgumentException();
            }
        }

        /// <summary>
        /// Удаление из коллекции семантических принадлежностей
        /// </summary>
        /// <param name="key">имя</param>
        /// <returns></returns>
        public override bool Remove(string key)
        {
            try
            {
                if (!list.ContainsKey(key))
                    return false;

                XmlDocument doc = XmlHelper.Load(this.path);
                XmlNode root = doc.SelectSingleNode("/ServerConfiguration/Semantics");
                root.RemoveChild(doc.SelectSingleNode(String.Format("/ServerConfiguration/Semantics/Semantic[@name = '{0}']", key)));
                Krista.FM.Common.Xml.XmlHelper.Save(doc, this.path);
                return list.Remove(key);
            }
            catch (ArgumentException)
            {
                throw new Exception(String.Format("Запись с именем {0} отсутствует", key));
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                throw new Exception(String.Format("Файл {0} не найден", this.path));
            }
        }

        // <summary>
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
                    XmlDocument doc = XmlHelper.Load(this.path);

                    XmlNode xmlSemantics = doc.SelectSingleNode(String.Format("/ServerConfiguration/Semantics/Semantic[@name = '{0}']", key));
                    xmlSemantics.Attributes["caption"].Value = value;

                    XmlHelper.Save(doc, this.path);

                    list[key] = value;
                }
                catch (Exception e)
                {
                    Trace.WriteLine(String.Format("При изменении элемента с именем {1} на {2} возникла ошибка : {0} ", e.Message, key, value));
                    throw new Exception(e.Message, e);
                }
            }
        }


        #region ISMOSerializable Members

        public SMOSerializationInfo GetSMOObjectData()
        {
            throw new NotImplementedException();
        }

        public SMOSerializationInfo GetSMOObjectData(LevelSerialization level)
        {
            SMOSerializationInfo info = new SMOSerializationInfo(new Dictionary<string, object>());

            info.Add("this", this);
            info.Add("Identifier", 100000);

            foreach (KeyValuePair<string, string> pair in list)
            {
                info.Add(pair.Key, pair.Value);
            }

            return info;
        }

        #endregion

        #region IServerSideObject Members

        public IServerSideObject Lock()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Unlock()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool IsLocked
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public int LockedByUserID
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public string LockedByUserName
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool IsEndPoint
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public ServerSideObjectStates State
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public IServerSideObject OwnerObject
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public int Identifier
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool IsClone
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public IServerSideObject ICloneObject
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}

