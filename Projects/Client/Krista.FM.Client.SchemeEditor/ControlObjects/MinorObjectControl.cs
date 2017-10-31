using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    /// <summary>
    /// Абстрактный шаблонный класс для второстепенных объектов
    /// </summary>
    /// <typeparam name="T">Тип элемента</typeparam>
    public abstract class MinorObjectControl<T> : CustomTreeNodeControl<T> where T : IServerSideObject
    {

        /// <summary>
        /// Абстрактный шаблонный класс для второстепенных объектов
        /// </summary>
        /// <param name="name">Уникальный ключ элемента в дереве</param>
        /// <param name="text">Наименование узла видимое в интерфейсе</param>
        /// <param name="controlObject">Управляемый объект</param>
        /// <param name="parent">Родительский узел элемента управления</param>
        public MinorObjectControl(string name, string text, T controlObject, CustomTreeNodeControl parent, int imageIndex)
            : base(name, text, controlObject, parent, imageIndex)
        {
        }

        protected override string GetCaption()
        {
            string _caption = base.GetCaption();

            /*if (TypedControlObject.IsClone || TypedControlObject.LockedByUserID == Krista.FM.Common.ClientAuthentication.UserID)
                _caption = String.Format("[Edit] {0}", _caption);
            else if (TypedControlObject.IsLocked)
                _caption = String.Format("[Locked] {0}", _caption);*/

            return _caption;
        }

        [MenuAction("Свойства", Images.Properties)]
        public void ShowProperties()
        {
            SchemeEditor.Instance.ShowObjectProperties(this.ControlObject);
        }
    }
}
