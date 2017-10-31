using System;
using System.Drawing;

using Krista.FM.ServerLibrary;
using Krista.FM.Client.Common.Forms;
using System.Windows.Forms;


namespace Krista.FM.Client.Design
{
    public abstract class BaseCommand
    {
         /// <summary>
        /// Прозрачный цвет
        /// </summary>
        public static readonly int transparentColor = Color.Lime.ToArgb();

        #region Поля

        /// <summary>
        /// Определяет доступна команда или нет
        /// </summary>
        private bool enabled = true;
        
        /// <summary>
        /// Событие при изменении свойства доступности
        /// </summary>
        public event EventHandler OnChangeEnabled;

        /// <summary>
        /// Изображение для команды
        /// </summary>
        private Bitmap image;

       
        #endregion Поля

        #region Методы

        private void OnChange(EventArgs args)
        {
            if (OnChangeEnabled != null)
                OnChangeEnabled(this, args);
        }

        private void Change()
        {
            EventArgs args = new EventArgs();
            OnChange(args);
        }

        #endregion Методы

        #region Свойства

        public Bitmap Image
        {
            get { return image; }
            set { image = value; }
        }

        public bool Enabled
        {
            get { return enabled; }
            set 
            {
                if (enabled != value)
                {
                    enabled = value;
                    Change();
                }
            }
        }

        #endregion Свойства
    }

    /// <summary>
    /// Базовый класс для всех команд без параметра
    /// </summary>
    public abstract class Command : BaseCommand, ICommand
    {
        public abstract void Execute();
    }

    /// <summary>
    /// Базовый класс для всех команд с параметром
    /// </summary>
    public abstract class CommandWithPrm : BaseCommand, ICommamdWithPrm
    {
        /// <summary>
        /// Параметр команды
        /// </summary>
        private object parameter;

        /// <summary>
        /// Выполнение команды с параметром
        /// </summary>
        /// <param name="parameter"></param>
        public abstract void Execute(object parameter);

        /// <summary>
        /// Параметр команды
        /// </summary>
        public object Parameter
        {
            get
            {
                if (parameter == null)
                    throw new Exception("Параметр команды не задан");
                return parameter;
            }
            set
            {
                parameter = value;
            }
        }
    }

    /// <summary>
    /// Базовый интерфейс для действий
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Выполнить действие
        /// </summary>
        void Execute();
    }

    /// <summary>
    /// Команда с параметром
    /// </summary>
    public interface ICommamdWithPrm
    {
        /// <summary>
        /// Выполнение команды с параментром
        /// </summary>
        /// <param name="obj"></param>
        void Execute(object obj);
    }

    /// <summary>
    /// Интерфейс для команд, реализующих команду Undo
    /// </summary>
    public interface IUndoCommand
    {
        void Undo();
    }

    /// <summary>
    /// Интерфейс команды создания новой ассоциации
    /// </summary>
    public interface INewAssociationCommand : ICommand
    {
        /// <summary>
        /// Создать новую ассоциацию
        /// </summary>
        /// <param name="roleA">Откуда</param>
        /// <param name="roleB">Куда</param>
        /// <returns>Созданная ассоциация</returns>
		IEntityAssociation Execute(IEntity roleA, IEntity roleB, AssociationClassTypes type);
    }

   
    /// <summary>
    /// Интерфейс создания нового класса
    /// </summary>
    public interface INewEntityCommand : ICommand
    {
        /// <summary>
        /// Создать новый класс с параметром ClassType
        /// </summary>
        /// <returns></returns>
        IEntity Execute(ClassTypes classTypes);
    }

    /// <summary>
    /// Интерфейс создания нового пакета
    /// </summary>
    public interface INewPackageCommand : ICommand
    {
        /// <summary>
        /// Создать новый пакет
        /// </summary>
        /// <returns></returns>
        IPackage Execute();
    }

    /// <summary>
    /// Интерфейс команды открытия документа
    /// </summary>
    public interface IOpenDocumentCommand : ICommand
    {
        /// <summary>
        /// Открыть документ
        /// </summary>
        /// <param name="document">Серверный документ</param>
        void Execute(IDocument document);
    }

    /// <summary>
    /// Интерфейс редактора схемы
    /// </summary>
    public interface ISchemeEditor
    {
        /// <summary>
        /// Серверная схема
        /// </summary>
        IScheme Scheme { get; set; }

        /// <summary>
        /// Название отображаемое в заголовке формы
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Возвращает серверный объект по квалифицированному пути
        /// </summary>
        /// <param name="pathName">Квалифицированный путь объекта</param>
        /// <returns>Серверный объект</returns>
        ICommonObject GetObjectByPathName(string pathName);

        /// <summary>
        /// Объект для индикации длительных операций (создается при первом обращении)
        /// </summary>
        Operation Operation { get; }

        /// <summary>
        /// Shows the specified System.Windows.Forms.Form.
        /// </summary>
        /// <param name="dialog">The System.Windows.Forms.Form to display.</param>
        /// <returns>A System.Windows.Forms.DialogResult indicating the result code returned by the System.Windows.Forms.Form.</returns>
        DialogResult ShowDialog(Form dialog);

        /// <summary>
        /// Удаляет объект из схемы
        /// </summary>
        /// <param name="serverKey">Квалифицированный путь объекта</param>
        void DeleteObject(string serverKey);

        /// <summary>
        /// Выделить объект в дереве
        /// </summary>
        /// <param name="serverKey">Имя может быть как серверное, так и уже готовое имя в дереве</param>
        /// <param name="isCorrectName">Серверное - false, в дереве - true</param>
        void SelectObject(string serverKey, bool isCorrectName);

        /// <summary>
        /// Обновление пакета
        /// </summary>
        /// <param name="objectName"> Объект, вызвавший обновление родителя</param>
        /// <returns></returns>
        void RefreshPackage(string objectName);

        /// <summary>
        /// Получаем имя в дереве по полному серверному имени
        /// </summary>
        /// <param name="objectName"></param>
        /// <returns></returns>
        string GetNameByServerName(string objectName);

        /// <summary>
        /// 
        /// </summary>
        PropertyGrid PropertyGrid { get; set;}
    }

    /// <summary>
    /// Интерфейс дизайнера UI
    /// </summary>
    public interface ISchemeDesigner
    {
        /// <summary>
        /// Редактор схем
        /// </summary>
        ISchemeEditor SchemeEditor { get; }
    }
}
