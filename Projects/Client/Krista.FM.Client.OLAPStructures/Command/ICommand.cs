using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Krista.FM.Client.Common.Forms;
using Microsoft.AnalysisServices;
using System.ComponentModel;

namespace Krista.FM.Client.OLAPStructures
{
    /// <summary>
    /// Базовый интерфейс для всех команд
    /// </summary>
    public interface ICommand
    {
        void Execute();
    }

    /// <summary>
    /// Базовый класс для одиночных команд 
    /// </summary>
    public class BaseCommand
    {
        /// <summary>
        /// Объект, кот надо заскриптить
        /// </summary>
        private NamedComponent obj;

        public BaseCommand(NamedComponent obj)
        {
            this.obj = obj;
        }

        public NamedComponent Obj
        {
            get { return obj; }
            set { obj = value; }
        }
    }

    /// <summary>
    /// Скрипт создания альтера для одного объекта
    /// </summary>
    public class AlterScriptCommand : BaseCommand, ICommand
    {
        public AlterScriptCommand(NamedComponent obj) 
            : base(obj)
        {

        }

        #region ICommand Members

        public void Execute()
        {
            if (Obj is MajorObject)
            {
                SaveFileDialog fileDialog = new SaveFileDialog();
                fileDialog.Filter = "xmla files (*.xmla)|*.xmla|All files (*.*)|*.*";
                fileDialog.FilterIndex = 2;
                fileDialog.RestoreDirectory = true;

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    Scripter scripter = new Scripter();

                    XmlTextWriter writer = new XmlTextWriter(fileDialog.FileName, Encoding.UTF8);

                    writer.Formatting = Formatting.Indented;

                    Scripter.WriteStartBatch(writer, true, false);

                    scripter.ScriptAlter(new MajorObject[] {(MajorObject)Obj}, writer, true );

                    Scripter.WriteEndBatch(writer);

                    writer.Close();
                }
            }
            else
            {
                throw new Exception(String.Format("Для данного типа объекта {0} невозможно создать скрипт обновления", Obj.GetType()));
            }
        }

        #endregion
    }

    /// <summary>
    /// Скрипт создания команды создания для одного объекта
    /// </summary>
    public class CreateScriptCommand : BaseCommand, ICommand
    {
        public CreateScriptCommand(NamedComponent obj) : base(obj)
        {
        }

        #region ICommand Members

        public void Execute()
        {
            if (Obj is MajorObject)
            {
                SaveFileDialog fileDialog = new SaveFileDialog();
                fileDialog.Filter = "xmla files (*.xmla)|*.xmla|All files (*.*)|*.*";
                fileDialog.FilterIndex = 2;
                fileDialog.RestoreDirectory = true;

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    Scripter scripter = new Scripter();

                    XmlTextWriter writer = new XmlTextWriter(fileDialog.FileName, Encoding.UTF8);

                    writer.Formatting = Formatting.Indented;

                    Scripter.WriteStartBatch(writer, true, false);

                    scripter.ScriptCreate(new MajorObject[] { (MajorObject)Obj }, writer, true);

                    Scripter.WriteEndBatch(writer);

                    writer.Close();
                }
            }
            else
            {
                throw new Exception(String.Format("Для данного типа объекта {0} невозможно создать скрипт обновления", Obj.GetType()));
            }
        }

        #endregion
    }

    /// <summary>
    /// Скрипт создания команды удаления для одного объекта
    /// </summary>
    public class DeleteScriptCommand : BaseCommand, ICommand
    {
        public DeleteScriptCommand(NamedComponent obj)
            : base(obj)
        {
        }

        #region ICommand Members

        public void Execute()
        {
            if (Obj is MajorObject)
            {
                SaveFileDialog fileDialog = new SaveFileDialog();
                fileDialog.Filter = "xmla files (*.xmla)|*.xmla|All files (*.*)|*.*";
                fileDialog.FilterIndex = 2;
                fileDialog.RestoreDirectory = true;

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    Scripter scripter = new Scripter();

                    XmlTextWriter writer = new XmlTextWriter(fileDialog.FileName, Encoding.UTF8);

                    writer.Formatting = Formatting.Indented;

                    Scripter.WriteStartBatch(writer, true, false);

                    scripter.ScriptDelete(new MajorObject[] { (MajorObject)Obj }, writer, true);

                    Scripter.WriteEndBatch(writer);

                    writer.Close();
                }
            }
            else
            {
                throw new Exception(String.Format("Для данного типа объекта {0} невозможно создать скрипт обновления", Obj.GetType()));
            }
        }

        #endregion
    }

    /// <summary>
    /// Тип команды
    /// </summary>
    public enum CommandType
    {
        [Description("Создать")]
        create = 0,
        [Description("Изменить")]
        alter = 1,
        [Description("Удалить")]
        delete = 2
    }

    /// <summary>
    /// Объект для скрипта
    /// </summary>
    public class ObjectForScript
    {
        /// <summary>
        /// Тип операци над объектом
        /// </summary>
        private CommandType commandType;
        /// <summary>
        /// Управляющий объект
        /// </summary>
        private MajorObject obj;
        /// <summary>
        /// Тип объекта
        /// </summary>
        private string objectType;

        public ObjectForScript(CommandType commandType, MajorObject obj, string objectType)
        {
            this.commandType = commandType;
            this.obj = obj;
            this.objectType = objectType;
        }

        public CommandType CommandType
        {
            get { return commandType; }
            set { commandType = value; }
        }

        public MajorObject Obj
        {
            get { return obj; }
            set { obj = value; }
        }

        public string ObjectType
        {
            get { return objectType; }
            set { objectType = value; }
        }
    }

    /// <summary>
    /// Комплексный скрипт
    /// </summary>
    public class ComplexScriptCommand : ICommand
    {
        /// <summary>
        /// Подставляемое значение вместо имени многомерной БД
        /// </summary>
        private const string databaseName = "/*#DatabaseName#*/";

        private Dictionary<string, ObjectForScript> objects = new Dictionary<string, ObjectForScript>();

        public ComplexScriptCommand(Dictionary<string, ObjectForScript> objects)
        {
            this.objects = objects;
        }

        public Dictionary<string, ObjectForScript> Objects
        {
            get { return objects; }
            set { objects = value; }
        }

        #region ICommand Members

        public void Execute()
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "xmla files (*.xmla)|*.xmla";
            fileDialog.FilterIndex = 2;
            fileDialog.RestoreDirectory = true;
            fileDialog.FileName = "script";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                Operation op = new Operation();
                try
                {
                    op.Text = "Создание патча...";
                    op.StartOperation();

                    XmlTextWriter writer = new XmlTextWriter(fileDialog.FileName, Encoding.UTF8);

                    writer.Formatting = Formatting.Indented;

                    Scripter scripter = new Scripter();

                    Scripter.WriteStartBatch(writer, false);

                    // Добавляем в патч представления данных
                    List<ObjectForScript> dsv = new List<ObjectForScript>();
                    foreach (ObjectForScript obj in objects.Values)
                    {
                        if (obj.Obj is DataSourceView)
                            dsv.Add(obj);
                    }

                    AddToWriter(writer, dsv, scripter);

                    // Добавляем в патч измерения);
                    List<ObjectForScript> dimensions = new List<ObjectForScript>();
                    foreach (ObjectForScript obj in objects.Values)
                    {
                        if (obj.Obj is Dimension)
                            dimensions.Add(obj);
                    }
                    AddToWriter(writer, dimensions, scripter);

                    // Добавляем в патч кубы
                    List<ObjectForScript> cubes = new List<ObjectForScript>();
                    foreach (ObjectForScript obj in objects.Values)
                    {
                        if (obj.Obj is Cube)
                            cubes.Add(obj);
                    }
                    AddToWriter(writer, cubes, scripter);

                    Scripter.WriteEndBatch(writer);

                    writer.Close();

                    ClearDatabaseName(fileDialog.FileName);
                }
                finally
                {
                    op.StopOperation();
                    op.ReleaseThread();
                }
            }
        }

        private static void AddToWriter(XmlTextWriter writer, List<ObjectForScript> dsv, Scripter scripter)
        {
            int i = 0;
            ScriptInfo[] info = new ScriptInfo[dsv.Count];
            foreach (ObjectForScript majorObject in dsv)
            {
                i = ADD(info, i, majorObject);
            }
            scripter.Script(info, writer);
        }

        private static int ADD(ScriptInfo[] scripts, int i, ObjectForScript obj)
        {
            bool scriptDependents = GetScriptAction(obj.CommandType) == ScriptAction.CreateWithAllowOverwrite
                                        ? false
                                        : true;
            ScriptInfo info =
                new ScriptInfo(obj.Obj, GetScriptAction(obj.CommandType), ScriptOptions.Default, scriptDependents);
            scripts.SetValue(info, i);
            i++;
            return i;
        }

        private static void ClearDatabaseName(string name)
        {
            XmlDocument doc =  new XmlDocument();
            doc.Load(name);

            XmlNamespaceManager xmlns = new XmlNamespaceManager(doc.NameTable);
            xmlns.AddNamespace("as", "http://schemas.microsoft.com/analysisservices/2003/engine");

            //коллекция альтеров
            XmlNodeList alterList = doc.SelectNodes("as:Batch//as:Alter", xmlns);
            Clear(alterList, xmlns);

            // коллекция объектов для удаления
            XmlNodeList deleteList = doc.SelectNodes("as:Batch//as:Delete", xmlns);
            Clear(deleteList, xmlns);

            // коллекция объектов для создания
            XmlNodeList createList = doc.SelectNodes("as:Batch//as:Create", xmlns);
            Clear(createList, xmlns);

            doc.Save(name);
        }

        /// <summary>
        /// Вычищаем имя конкретной базы и подставляем макропеременную
        /// </summary>
        /// <param name="list"></param>
        /// <param name="xmlns"></param>
        private static void Clear(XmlNodeList list, XmlNamespaceManager xmlns)
        {
            foreach (XmlNode node in list)
            {
                // для alter и delete
                if (node.SelectSingleNode("as:Object/as:DatabaseID", xmlns) != null)
                    node.SelectSingleNode("as:Object/as:DatabaseID", xmlns).InnerText = databaseName;
                // для команды create
                if (node.SelectSingleNode("as:ParentObject/as:DatabaseID", xmlns) != null)
                    node.SelectSingleNode("as:ParentObject/as:DatabaseID", xmlns).InnerText = databaseName;
            }
        }

        private static ScriptAction GetScriptAction(CommandType type)
        {
            switch(type)
            {
                case CommandType.alter:
                    return ScriptAction.AlterWithAllowCreate;
                case CommandType.create:
                    return ScriptAction.CreateWithAllowOverwrite;
                case CommandType.delete:
                    return ScriptAction.Delete;
                default:
                    throw new Exception("");
            }
        }

        #endregion
    }
}
