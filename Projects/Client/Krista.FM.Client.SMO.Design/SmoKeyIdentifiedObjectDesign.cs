using System;
using System.ComponentModel;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    public abstract class SmoKeyIdentifiedObjectDesign<T> 
        : SmoServerSideObjectDesign<T>, IKeyIdentifiedObject where T : IKeyIdentifiedObject
    {
        protected SmoKeyIdentifiedObjectDesign(T serverControl)
            : base(serverControl)
        {
        }
        
        #region IKeyIdentifiedObject Members

        [Category("KeyIdentifiedObject")]
        [DisplayName(@"Уникальный ключ объекта (ObjectKey)")]
        [Browsable(true)]
        public string ObjectKey
        {
            get { return serverControl.ObjectKey; }
            set
            {
                if (((IKeyIdentifiedObject)serverControl).State == ServerSideObjectStates.New)
                {
                    try
                    {
                        Guid guid = new Guid(value);
                        if (guid == Guid.Empty)
                        {
                            throw new InvalidOperationException(
                                "Guid объекта не может быть пустым.");
                        }
                    }
                    catch (FormatException)
                    {
                        throw new InvalidOperationException(
                            "Значение свойства некорректно.");
                    }
                    serverControl.ObjectKey = value;
                }
                else
                {
                    throw new InvalidOperationException(
                        "Свойство ObjectKey можно устанавливать только для вновь создаваемых объектов.");
                }
            }
        }

        [Category("KeyIdentifiedObject")]
        [DisplayName(@"Уникальное наименование объекта (ObjectOldKeyName)")]
        [Browsable(true)]
        public string ObjectOldKeyName
        {
            get { return serverControl.ObjectOldKeyName; }
        }

        #endregion

        public SMOSerializationInfo GetSMOObjectData()
        {
            throw new NotImplementedException();
        }

        public SMOSerializationInfo GetSMOObjectData(LevelSerialization level)
        {
            throw new NotImplementedException();
        }
    }
}
    