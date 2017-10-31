using System;
using System.Collections.Generic;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    /// <summary>
    /// Абстрактный шаблонный класс для пакетов, отношений, ассоциаций и документов
    /// </summary>
    /// <typeparam name="T">Тип элемента</typeparam>
    public abstract class MajorObjectControl<T> : CustomTreeNodeControl<T> where T : ICommonDBObject
    {
        /// <summary>
        /// Абстрактный шаблонный класс для пакетов, отношений, ассоциаций и документов
        /// </summary>
        /// <param name="controlObject">Управляемый объект</param>
        /// <param name="parent">Родительский узел элемента управления</param>
        /// <param name="imageIndex"></param>
        public MajorObjectControl(T controlObject, CustomTreeNodeControl parent, int imageIndex)
            : base(
                controlObject.ObjectKey,
                String.Format("{0}.{1} ({2})", controlObject.SemanticCaption, controlObject.Caption, controlObject.FullName),
                controlObject,
                parent, imageIndex)
        {
            Text = GetCaption();
        }

        public override string Caption
        {
            get 
            {
                ICommonDBObject co = (ICommonDBObject)ControlObject;
                return String.Format("{0}.{1} ({2})", co.SemanticCaption, co.Caption, co.FullName); 
            }
        }

        protected override string GetCaption()
        {
            return AddStateModificators(TypedControlObject, base.GetCaption()); 
        }

        [MenuAction("Отменить изменения", Images.CancelChange, CheckMenuItemEnabling = "IsLockedByCurrentUser")]
        public void CancelEdit()
        {
            ((ICommonDBObject)ControlObject).CancelEdit();
            this.Refresh();
            RefreshCaptionForChildren();
        }

        [MenuAction("XML конфигурация", Images.XMLConfigaration)]
        public void ShowXmlConfiguration()
        {
            Design.Editors.XmlViewEditorForm xve = new Design.Editors.XmlViewEditorForm(TypedControlObject.ConfigurationXml);
            SchemeEditor.ShowDialog(xve);
        }

        [MenuAction("SQL скрипт")]
        public void ShowSQLDefinitionScript()
        {
            Design.Editors.SQLScriptEditorForm sse = new Design.Editors.SQLScriptEditorForm();
            sse.Text = ((ICommonDBObject)ControlObject).FullName;
            sse.SetText(((ICommonDBObject)ControlObject).GetSQLDefinitionScript());
            SchemeEditor.ShowMdiForm(sse);
        }

        [MenuAction("Свойства", Images.Properties)]
        public void ShowProperties()
        {
            SchemeEditor.Instance.ShowObjectProperties(this.ControlObject);
        }
    }
}
