using System;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SMO
{
    public abstract class SmoKeyIdentifiedObject<T> : SMOSerializableObject<T>, IKeyIdentifiedObject where T : IKeyIdentifiedObject
    {
        public SmoKeyIdentifiedObject(T serverObject)
            : base(serverObject)
        {
        }

        public SmoKeyIdentifiedObject(T serverObject, bool cached)
            : base(serverObject, cached)
        {
        }

        public SmoKeyIdentifiedObject(SMOSerializationInfo cache)
            : base(cache)
        {
        }

        #region IKeyIdentifiedObject Members

        public string ObjectKey
        {
            get { return cached ? (string)GetCachedValue("ObjectKey") : serverControl.ObjectKey; }
            set
            {
                if (serverControl.State == ServerSideObjectStates.New)
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

        public string ObjectOldKeyName
        {
            get { return serverControl.ObjectOldKeyName; }
        }

        #endregion
    }
}
