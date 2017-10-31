using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.ServerLibrary;
using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.Modifications;

namespace Krista.FM.Server.Scheme.Classes
{
    /// <summary>
    /// Коллекция представлений объекта сервера
    /// </summary>
    internal class PresentationCollection : ModifiableCollection<string, IPresentation>, IPresentationCollection
    {                

        #region Fields

        /// <summary>
        /// Уникальный идентификатор представления по умолчанию
        /// </summary>
        private string defaultPresentation;
        /// <summary>
        /// Родительский объект
        /// </summary>
        private readonly Entity parent;

        #endregion Fields

        #region Constructor

        public PresentationCollection(Entity owner, ServerSideObjectStates state)
            : base(owner, state)
        {
            this.parent = owner;
        }

        /// <summary>
        /// Конструктор со значением по умолчанию для коллекции представлений у объекта сервера
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="state"></param>
        /// <param name="defaultPresentation"></param>
        public PresentationCollection(Entity owner, ServerSideObjectStates state, string defaultPresentation)
            : this(owner, state)
        {
            this.defaultPresentation = defaultPresentation;
        }

        #endregion Constructor

        #region IPresentationCollection Members

        /// <summary>
        /// Уникальный идентификатор представления по умолчанию
        /// </summary>
        public string DefaultPresentation
        {
            get
            {
                return GetterMustUseClone()
                           ? ((PresentationCollection) CloneObject).DefaultPresentation
                           : defaultPresentation;
            }
            set
            {
                if (SetterMustUseClone())
                    ((PresentationCollection)CloneObject).DefaultPresentation = value;
                else
                    defaultPresentation = value;
            }
        }
        
        #endregion

        #region IPresentationCollection Members

        /// <summary>
        /// Создание представления
        /// </summary>
        /// <param name="key"></param>
        /// <param name="name"></param>
        /// <param name="attributes"></param>
        /// <param name="levelNamingTemplate"></param>
        /// <returns></returns>
        public IPresentation CreateItem(string key, string name, List<IDataAttribute> attributes, string levelNamingTemplate)
        {
            Presentation presentation = new Presentation(key, name, attributes,
                levelNamingTemplate, this.Owner, ServerSideObjectStates.New);

            this.Add(presentation.ObjectKey, presentation);

            this.defaultPresentation = presentation.ObjectKey;

            return presentation;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Родительский объект
        /// </summary>
        public Entity Parent
        {
            get { return parent; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Блокировка
        /// </summary>
        /// <returns></returns>
        public override IServerSideObject Lock()
        {
            Entity cloneEntity = (Entity)Owner.Lock();
            return cloneEntity.Presentations;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clon"></param>
        /// <param name="cloneItems"></param>
        protected override void CloneItems(DictionaryBase<string, IPresentation> clon, bool cloneItems)
        {
            foreach (KeyValuePair<string, IPresentation> item in this.list)
            {
                ((PresentationCollection)clon).list.Add(item.Key, (IPresentation)item.Value.Clone());
            }
        }

        protected override ModificationItem GetCreateModificationItem(KeyValuePair<string, IPresentation> item, CollectionModificationItem root)
        {
            return new CreateMinorModificationItem(Convert.ToString(item.Key), Parent, item.Value, root);
        }

        protected override ModificationItem GetRemoveModificationItem(KeyValuePair<string, IPresentation> item, CollectionModificationItem root)
        {
            return new RemoveMinorModificationItem(Convert.ToString(item.Key), item.Value, root);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toCollection"></param>
        /// <returns></returns>
        public override ModificationItem GetChanges(IModifiableCollection<string, IPresentation> toCollection)
        {
            ModificationItem root = base.GetChanges(toCollection);

            PresentationCollection collection = (PresentationCollection)toCollection;

            if (this.DefaultPresentation != collection.DefaultPresentation)
            {
                ModificationItem item = new PropertyModificationItem("DefaultPresentation", this.DefaultPresentation, collection.DefaultPresentation, root);
                root.Items.Add(item.Key, item);
            }

            return root;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="toPresentations"></param>
        public virtual void Update(Entity entity, PresentationCollection toPresentations)
        {
            if (DefaultPresentation != toPresentations.DefaultPresentation)
            {
                Trace.WriteLine(String.Format("У коллекции представлений у объекта \"{0}\" представление по умолчанию изменилось c \"{1}\" на \"{2}\"",
                    entity.FullName, DefaultPresentation, toPresentations.DefaultPresentation));
                DefaultPresentation = toPresentations.DefaultPresentation;
            }
        }

        /// <summary>
        /// Возвращаем представление по ключу
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="objectKey"></param>
        /// <returns></returns>
        internal static Presentation GetPresentationByKey(IPresentationCollection collection, string objectKey)
        {
            if (collection.ContainsKey(objectKey))
                return (Presentation)collection[objectKey];

            return null;
        }

        #endregion Methods
    }
}
