using System;
using System.Xml;

using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.ScriptingEngine.Classes;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Classes
{
    /// <summary>
    /// Фиксированный классификатор (доступен только для чтения)
    /// </summary>
    internal class FixedClassifier : Classifier
    {
        /// <summary>
        /// Конструктор объекта
        /// </summary>
        /// <param name="key"></param>
        /// <param name="owner"></param>
        /// <param name="semantic"></param>
        /// <param name="name">Имя объекта</param>
        /// <param name="state"></param>
        public FixedClassifier(string key, ServerSideObject owner, string semantic, string name, ServerSideObjectStates state)
            : base(key, owner, semantic, name, ClassTypes.clsFixedClassifier, SubClassTypes.Regular, false, state, SchemeClass.ScriptingEngineFactory.FixedClassifierEntityScriptingEngine)
        {
            tagElementName = "FixedCls";
        }

        /// <summary>
        /// Префикс классификатора
        /// </summary>
        public override string TablePrefix
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return "fx"; }
        }

        /// <summary>
        /// Полное имя объекта
        /// </summary>
        public override string FullName
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return TablePrefix + "." + base.FullName; }
        }

        /// <summary>
        /// Возврашает следующее значение генератора
        /// </summary>
        public override int GetGeneratorNextValue
        {
            get
            {
                throw new Exception("Фиксированный классификатор не может быть изменен.");
            }
        }

        /// <summary>
        /// Определяет делится ли объект по источникам или нет
        /// </summary>
        public override bool IsDivided
        {
            get { return false; }
        }

        /// <summary>
        /// Фиксированный классификотор не делиться по источникам
        /// Используем переопределенное свойство при сериализации объекта
        /// </summary>
        public override string DataSourceKinds
        {
            get
            {
                return String.Empty;
            }
            set
            {
                throw new Exception("У данного класса нельзя менять деление по источникам");
            }
        }

        /// <summary>
        /// Обновляет фиксированные записи в БД
        /// </summary>
        /// <param name="sourceID"></param>
        public override int UpdateFixedRows(int sourceID)
        {
            return -1;
        }

        internal override void Create(Modifications.ModificationContext context)
        {
            base.Create(context);
            InsertData(context);
        }

        internal override XmlDocument PostInitialize()
        {
            XmlDocument doc = base.PostInitialize();

            #region Удаление триггеров авторизации
            if (ID > 0)
            {
                Database db = null;
                try
                {
                    db = (Database)SchemeClass.Instance.SchemeDWH.DB;
                    if (_scriptingEngine.ExistsObject(AuditTriggerName, Krista.FM.Server.Scheme.ScriptingEngine.ObjectTypes.Trigger)
                        && (SchemeDWH.Instance.DatabaseVersion == "2.3.1.0" && SchemeClass.Instance.NeedUpdateScheme))
                    {
                        db.RunScript(new string[] { ((EntityScriptingEngine)_scriptingEngine).DropAuditTriggersScript(this) }, false);
                    }
                }
                catch (Exception e)
                {
                    Trace.TraceError("Ошибка при удалении триггера аудита {0}: {1}", AuditTriggerName, e.Message);
                }
                finally
                {
                    if (db != null) 
                        db.Dispose();
                }
            }

            #endregion Удаление триггеров авторизации

            return doc;
        }

        /// <summary>
        /// Проверяет права на просмотр объекта для текущего пользователя
        /// </summary>
        /// <returns>true - если у пользлвателя есть права на просмотр</returns>
        public override bool CurrentUserCanViewThisObject()
        {
            return true;
        }
    }
}
