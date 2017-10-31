using System;
using System.Runtime.Serialization;
using Krista.FM.ServerLibrary;
using Krista.FM.Common;

namespace Krista.FM.Client.SMO
{
    /// <summary>
    /// По сути это SMO представление, как и для простого атрибута, только тут мы
    /// скрываем свойства, которые для представления не редактируем + добавляем контроль
    /// при изменении значений свойств
    /// </summary>
    public class SmoAttributePresentattion : SmoAttribute
    {
        public SmoAttributePresentattion(IDataAttribute serverObject)
            : this(serverObject, false)
        {
        }

        public SmoAttributePresentattion(IDataAttribute serverObject, bool cached)
            : base(serverObject, cached)
        {
        }


        public SmoAttributePresentattion(SMOSerializationInfo cache) 
            : base(cache)
        {
        }

        /// <summary>
        /// Фабричный метод, для создания SMO атрибутов.
        /// </summary>
        /// <param name="attribute">Атрибут.</param>
        /// <returns>SMO атрибут конкретного типа.</returns>
        new public static SmoAttribute SmoAttributeFactory(IDataAttribute attribute)
        {
            if (attribute.Class == DataAttributeClassTypes.Typed)
            {
                if (attribute is IDocumentAttribute)
                {
                    return new SmoAttributeDocument(attribute);
                }

                return new SmoAttributePresentattion(attribute);
            }
            return new SmoAttributeReadOnly(attribute);
        }


        #region IDataAttribute Members

        new public string Name
        {
            get { return cached ? (string)GetCachedValue("Name") : serverControl.Name; }
            set
            {
                if (ReservedWordsClass.CheckName(value))
                {
                    serverControl.Name = value;
                    CallOnChange();
                }
            }
        }

        new public string Caption
        {
            get { return cached ? (string)GetCachedValue("Caption") : serverControl.Caption; }
            set { serverControl.Caption = value; CallOnChange(); }
        }

        new public string Description
        {
            get { return cached ? (string)GetCachedValue("Description") : serverControl.Description; }
            set { serverControl.Description = value; CallOnChange(); }
        }

        new public int Size
        {
            // При смене значения 
            get { return serverControl.Size; }
            set
            {
                //ограничение на изменение размера у атрибута представления
                IEntity entity = serverControl.OwnerObject.OwnerObject as IEntity;
                if (entity != null)
                {
                    if (entity.Attributes.ContainsKey(serverControl.ObjectKey))
                        if (entity.Attributes[serverControl.ObjectKey].Size < value)
                            throw new Exception(
                                String.Format(
                                    "Невозможно изменить размерность атрибута представления {0} c {1} на {2}. Размер сожно только уменьшить",
                                    serverControl.Name, serverControl.Size, value));

                    serverControl.Size = value;
                    CallOnChange();
                }
            }
        }

        
        new public string Default
        {
            get { return cached ? Convert.ToString((string)GetCachedValue("DefaultValue")) : Convert.ToString(serverControl.DefaultValue); }
            set 
            {
                // TODO ограничение на изменение значения по умолчанию у атрибута представления

                serverControl.DefaultValue = value;
                CallOnChange(); 
            }
        }


        new public string Mask
        {
            get { return cached ? (string)GetCachedValue("Mask") : serverControl.Mask; }
            set
            {
                serverControl.Mask = value;
                CallOnChange();
            }
        }
        
        new public string Divide
        {
            get { return cached ? (string)GetCachedValue("Divide") : serverControl.Divide; }
            set 
            {
                IEntity entity = serverControl.OwnerObject.OwnerObject as IEntity;
                try
                {
                    if (entity != null)
                    {
                        if (entity.Attributes.ContainsKey(serverControl.ObjectKey))
                            CompareMask(entity.Attributes[serverControl.ObjectKey].Divide, value);

                        serverControl.Divide = value;
                        CallOnChange();
                    }
                }
                catch (CompareDivideException e)
                {
                    throw new Exception(String.Format("Невозможно изменить маску расщепления атрибута представления {0} c {1} на {2}. Оригинальная маска расщепления {3}. {4}",
                                        serverControl.Name, serverControl.Divide, value, entity.Attributes[serverControl.ObjectKey].Divide, e.Message));
                }
            }
        }

        /// <summary>
        /// Сравнение маски расщепления
        /// </summary>
        /// <param name="originalMask"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private void CompareMask(string originalMask, string value)
        {
            try
            {
                if (String.IsNullOrEmpty(value))
                    return;

                if (String.IsNullOrEmpty(originalMask))
                    throw new CompareDivideException("Оригинальная маска расщепления не задана");
                    
                string[] originalParts = originalMask.Split('.');
                string[] valueParts = value.Split('.');

                if (originalParts.Length < valueParts.Length)
                    throw new CompareDivideException("Количесво частей маски расщепления атрибута представления превышает число частей оригинальной маски");

                for (int i = 0; i < originalParts.Length; i++)
                {
                    if (valueParts.Length > i)
                        if (Math.Abs(Int32.Parse(valueParts[i])) > Math.Abs(Int32.Parse(originalParts[i])))
                            throw new CompareDivideException("Часть маски расщепления атрибута представления больше части оригинальгой маски");
                }

            }
            catch (ArgumentNullException e)
            {
                throw new Exception(e.Message);
            }
            catch (ArgumentException e)
            {
                throw new Exception(e.Message);
            }
        }

        private class CompareDivideException : Exception
        {
            public CompareDivideException()
            {
            }

            public CompareDivideException(string message) : base(message)
            {
            }

            public CompareDivideException(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected CompareDivideException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }

        new public bool StringIdentifier
        {
            get { return cached ? (bool)GetCachedValue("StringIdentifier") : serverControl.StringIdentifier; }
            set { serverControl.StringIdentifier = value; CallOnChange(); }
        }

        new public bool Visible
        {
            get { return cached ? (bool)GetCachedValue("Visible") : serverControl.Visible; }
            set { serverControl.Visible = value; CallOnChange(); }
        }

        new public bool IsNullable
        {
            get { return cached ? (bool)GetCachedValue("IsNullable") : serverControl.IsNullable; }
            set { serverControl.IsNullable = value; CallOnChange(); }
        } 

        new public bool IsReadOnly
        {
            get { return cached ? (bool)GetCachedValue("IsReadOnly") : serverControl.IsReadOnly; }
            set { serverControl.IsReadOnly = value; CallOnChange(); }
        }
       
        new public string SQLDefinition
        {
            get { return cached ? (string)GetCachedValue("SQLDefinition") : serverControl.SQLDefinition; }
        }

        new public int Scale
        {
            get { return cached ? (int)GetCachedValue("Scale") : serverControl.Scale; }
            set { serverControl.Scale = value; CallOnChange(); }
        }

        new public LookupAttributeTypes LookupType
        {
            get { return cached ? (LookupAttributeTypes)GetCachedValue("LookupType") : serverControl.LookupType; }
            set { serverControl.LookupType = value; CallOnChange(); }
        }

        new public DataAttributeTypes Type
        {
            get { return cached ? (DataAttributeTypes)GetCachedValue("Type") : serverControl.Type; }
            set { serverControl.Type = value; CallOnChange(); }
        }

        new public string PositionCalc
        {
            get { return serverControl.GetCalculationPosition(); }
        }

        new public string GroupParentAttribute
        {
            get { return serverControl.GroupParentAttribute; }
            set { serverControl.GroupParentAttribute = value; CallOnChange(); }
        }

        #endregion
    }
}
