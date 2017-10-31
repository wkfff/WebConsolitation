using System;
using System.Diagnostics;
using System.Xml;

using Krista.FM.Common.Xml;
using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.Modifications;

using Krista.FM.ServerLibrary;

using NUnit.Framework;


namespace Krista.FM.Server.Scheme.Classes
{
    /// <summary>
    /// Объекты идентифицируемые по уникальному ключу при помощи GUID.
    /// </summary>
    public abstract class KeyIdentifiedObject : SMOSerializable, IKeyIdentifiedObject, IModifiable
    {
        /// <summary>
        /// Уникальный ключ объекта.
        /// </summary>
        private string key;

        public KeyIdentifiedObject(string key, ServerSideObject owner)
            : base(owner)
        {
            this.key = key;
        }

        public KeyIdentifiedObject(string key, ServerSideObject owner, ServerSideObjectStates state)
            : base(owner, state)
        {
            this.key = key;
        }

        /// <summary>
        /// Уникальный ключ объекта.
        /// </summary>
        public virtual string ObjectKey
        {
            get { return key; }
            set
            {
                if (state == ServerSideObjectStates.New)
                {
                    key = value;
                }
                else
                {
                    throw new InvalidOperationException(
                        "Свойство ObjectKey можно устанавливать только для вновь создаваемых объектов.");
                }
            }
        }

        /// <summary>
        /// Уникальное наименование объекта (старый ключ).
        /// Это свойство после выпуска версии 2.4.1 необходимо удалить.
        /// </summary>
        public abstract string ObjectOldKeyName
        {
            get;
        }

        internal virtual void Save2Xml(XmlNode node)
        {
            if (!String.IsNullOrEmpty(key) && key != Guid.Empty.ToString())
            {
                XmlHelper.SetAttribute(node, "objectKey", key);
            }
        }

        #region IModifiable Members

        public virtual IModificationItem GetChanges(IModifiable toObject)
        {
            if (this.ObjectKey == Guid.Empty.ToString() && ((IKeyIdentifiedObject)toObject).ObjectKey != Guid.Empty.ToString())
            {
                return new KeyObjectModificationItem(((IKeyIdentifiedObject) toObject).ObjectKey, this, null);
            }
            else if (this.ObjectKey == ((IKeyIdentifiedObject)toObject).ObjectKey)
            {
                return null;
            }
            else
            {
                return new InapplicableModificationItem(
                    String.Format("Невозможно изменить ключ объекта: {0}", this.ToString()), null, null, null);
            }
        }

        #endregion

        /// <summary>
        /// Если параметр key правильный непустой GUID, то возвращает значение key, 
        /// иначе вернет значение параметра name.
        /// </summary>
        /// <param name="key">GUID объекта.</param>
        /// <param name="name">Английское наименование объекта.</param>
        /// <returns>Уникальный ключ объекта.</returns>
        internal static string GetKey(string key, string name)
        {
            Guid guig = Guid.Empty;
            
            if (!String.IsNullOrEmpty(key))
            {
                guig = new Guid(key);
            }

            return guig == Guid.Empty ? name : guig.ToString(); 
        }

        [DebuggerStepThrough()]
        public override string ToString()
        {
            return String.Format("{0} : {1}", ObjectKey, base.ToString());
        }
    }

    [TestFixture]
    public class KeyIdentifiedObjectTest
    {
        [Test(Description = "Проверка на пустой GUID")]
        public void GetKeyTest1()
        {
            Assert.AreEqual("Name", KeyIdentifiedObject.GetKey(String.Empty, "Name"));
        }

        [Test(Description = "Проверка на null")]
        public void GetKeyTest2()
        {
            Assert.AreEqual("Name", KeyIdentifiedObject.GetKey(null, "Name"));
        }

        [Test(Description = "Проверка некорректный GUID")]
        public void GetKeyTest3()
        {
            string guid = Guid.NewGuid().ToString();
            Assert.AreEqual(guid, KeyIdentifiedObject.GetKey(guid, "Name"));
        }

        [Test(Description = "Проверка некорректный GUID")]
        [ExpectedException(typeof(FormatException))]
        public void GetKeyTest4()
        {
            KeyIdentifiedObject.GetKey("0001343-2343-HDKRO-D76J", "Name");
        }
    }

    internal sealed class KeyObjectModificationItem : ModificationItem
    {
        /// <summary>
        /// Наименование модифицируемого свойства
        /// </summary>
        private readonly string key;


        public KeyObjectModificationItem(string newKey, KeyIdentifiedObject fromObject, ModificationItem parent)
            : base(ModificationTypes.Modify, newKey, fromObject, null, parent)
        {
            key = newKey;
        }

        public override int ImageIndex
        {
            get { return 54; }
        }

        protected override void OnBeforeChildApplay(ModificationContext context, out bool isAppliedPartially)
        {

            ((KeyIdentifiedObject) FromObject).ObjectKey = key;
            isAppliedPartially = false;
        }
    }
}
