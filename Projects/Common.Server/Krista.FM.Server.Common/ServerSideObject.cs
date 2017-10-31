using System;
using System.Diagnostics;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Common
{
    /// <summary>
    /// Базовый клас для всех серверных объектов. 
    /// Определяет повение серверных объектов на основе паттерна "Компоновщик"
    /// </summary>
    //[DebuggerStepThrough()]
    public class ServerSideObject : SecurityContextBoundObject, ICloneable, IServerSideObject
    {
        /// <summary>
        /// Объект-целое, владелец
        /// </summary>
        private ServerSideObject owner;

        /// <summary>
        /// Состояние серверного объекта во времени его существования
        /// </summary>
        protected ServerSideObjectStates state;

        /// <summary>
        /// true - Объект является целой частью, false - является частью составного объекта 
        /// </summary>
        private bool isEndPoint = false;

        /// <summary>
        /// ID пользователя заблокировавшего объект
        /// </summary>
        private int lockedByUserID;

        /// <summary>
        /// Имя пользователя заблокировавшего объект
        /// </summary>
        private string lockedByUserName;

        /// <summary>
        /// true - объект является клоном
        /// </summary>
        private bool isClone = false;

        /// <summary>
        /// Копия объекта с которым работаем польхователь заблокировавший его
        /// </summary>
        private ServerSideObject cloneObject;

        /// <summary>
        /// Определяет поведение серверных объектов на основе паттерна "Компоновщик".
        /// Конструктор по умолчанию.
        /// </summary>
        [DebuggerStepThrough()]
        public ServerSideObject(ServerSideObject owner)
            : this (owner, ServerSideObjectStates.Consistent)
        {
        }

        /// <summary>
        /// Определяет поведение серверных объектов на основе паттерна "Компоновщик".
        /// Конструктор по умолчанию.
        /// </summary>
        /// <param name="owner">Объект-владелец</param>
        /// <param name="state">Состояние объекта</param>
        [DebuggerStepThrough()]
        public ServerSideObject(ServerSideObject owner, ServerSideObjectStates state)
        {
            identifier = GetNewIdentifier();

            this.state = state;
            this.owner = owner;

            if (!Authentication.IsSystemRole())
            {
                lockedByUserID = (int)Authentication.UserID;
                lockedByUserName = Authentication.UserName;
            }
        }

        /// <summary>
        /// Время жизни объекта не ограничено
        /// </summary>
        /// <returns></returns>
        [DebuggerStepThrough()]
        public override object InitializeLifetimeService()
        {
            return null;
        }

        #region Identifier

        /// <summary>
        /// Счетчик для генерации идентификаторов
        /// </summary>
        private static object g_identifier = 0;
        
        /// <summary>
        /// Идентификатор текущего объекта
        /// </summary>
        protected int identifier = -1;

        /// <summary>
        /// Идентификатор текущего объекта
        /// </summary>
        public int Identifier
        {
            [DebuggerStepThrough()]
            get { return identifier; }
            //set { identifier = value; }
        }
        
        /// <summary>
        /// Возвращает следующий идентификатор из счетчика
        /// </summary>
        /// <returns>Новый идентификатор</returns>
        [DebuggerStepThrough()]
        protected static int GetNewIdentifier()
        {
            int localIdentifier;
            lock (g_identifier)
            {
                g_identifier = localIdentifier = Convert.ToInt32(g_identifier) + 1;
            }
            return localIdentifier;
        }

        #endregion

        /// <summary>
        /// Объект-целое, владелец
        /// </summary>
        public ServerSideObject Owner
        {
            [DebuggerStepThrough()]
            get { return owner; }
            [DebuggerStepThrough()]
            set { owner = value; }
        }

        /// <summary>
        /// Объект-целое, владелец
        /// </summary>
        public IServerSideObject OwnerObject
        {
            [DebuggerStepThrough()]
            get { return owner; }
        }

        /// <summary>
        /// Состояние серверного объекта во времени его существования
        /// </summary>
        public virtual ServerSideObjectStates State
        {
            [DebuggerStepThrough()]
            get { return state; }
            [DebuggerStepThrough()]
            set { state = value; }
        }

        /// <summary>
        /// true - Объект является целой частью, false - является частью составного объекта 
        /// </summary>
        public virtual bool IsEndPoint
        {
            [DebuggerStepThrough()]
            get { return isEndPoint; }
            [DebuggerStepThrough()]
            set { isEndPoint = value; }
        }

        /// <summary>
        /// true - объект является клоном
        /// </summary>
        public bool IsClone
        {
            [DebuggerStepThrough()]
            get { return isClone; }
            [DebuggerStepThrough()]
            set { isClone = value; }
        }

        /// <summary>
        /// Копия объекта с которым работаем польхователь заблокировавший его
        /// </summary>
        public ServerSideObject CloneObject
        {
            [DebuggerStepThrough()]
            get { return cloneObject; }
        }

        /// <summary>
        /// Копия объекта с которым работаем польхователь заблокировавший его
        /// </summary>
        public IServerSideObject ICloneObject
        {
            [DebuggerStepThrough()]
            get { return cloneObject; }
        }

        /// <summary>
        /// Возвращает true, если объект заблокирован
        /// </summary>
        public bool IsLocked
        {
            [DebuggerStepThrough()]
            get { return cloneObject != null || lockedByUserID == Authentication.UserID || State == ServerSideObjectStates.New; }
        }

        /// <summary>
        /// ID пользователя заблокировавшего объект
        /// </summary>
        public int LockedByUserID
        {
            [DebuggerStepThrough()]
            get { return lockedByUserID; }
        }

        /// <summary>
        /// Имя пользователя заблокировавшего объект
        /// </summary>
        public string LockedByUserName
        {
            [DebuggerStepThrough()]
            get { return lockedByUserName; }
        }

        //[DebuggerStepThrough()]
        public virtual IServerSideObject Lock()
        {
            if (Authentication.IsSupervisor())
                throw new Exception("Супервизору запрещено изменять схему.");

            int? userID = Authentication.UserID;
            if (IsLocked)
            {
                if (LockedByUserID == userID)
                    if (state == ServerSideObjectStates.New)
                        return this;
                    else
                        return cloneObject;
                else
                    throw new Exception(String.Format("Объект заблокирован пользователем {0}", lockedByUserName));
            }
            else
            {
                if (IsEndPoint)
                {
                    lockedByUserID = (int)Authentication.UserID;
                    lockedByUserName = Authentication.UserName;
                    if (state == ServerSideObjectStates.New)
                        return this;
                    else
                        cloneObject = cloneObject != null ? cloneObject : Clone() as ServerSideObject;
                    return cloneObject;
                }
                else
                {
                    return Owner.Lock();
                }
            }
        }

        /// <summary>
        /// Снимает блокировку с объекта
        /// </summary>
        //[DebuggerStepThrough()]
        public virtual void Unlock()
        {
            if (IsLocked || state == ServerSideObjectStates.New)
            {
                if (IsEndPoint)
                {
                    if (cloneObject != null)
                        cloneObject.Dispose();
                    cloneObject = null;
                    lockedByUserID = 0;
                    lockedByUserName = String.Empty;
                }
                else
                {
                    cloneObject = null;
                    lockedByUserID = 0;
                    lockedByUserName = String.Empty;
                    //if (Owner != null)
                    //    Owner.Unlock();
                }
            }
        }

        /// <summary>
        /// true - для обращения к свойству необходимо использовать клон
        /// </summary>
        /// <returns>true/false</returns>
        [DebuggerStepThrough()]
        protected bool GetterMustUseClone()
        {
            return CloneObject != null && (LockedByUserID == Authentication.UserID || Authentication.IsSupervisor());
        }

        /// <summary>
        /// true - для установки значения свойства необходимо использовать клон
        /// </summary>
        /// <returns>true/false</returns>
        //[DebuggerStepThrough()]
        protected bool SetterMustUseClone()
        {
            return CheckEditMode() && CloneObject != null;
        }

        /// <summary>
        /// Возвращает экземпляр объекта с которым должен работать текущий пользователь
        /// </summary>
        /// <returns>Экземпляр объекта</returns>
        [DebuggerStepThrough()]
        protected virtual ServerSideObject GetInstance()
        {
            return GetterMustUseClone() ? CloneObject : this;
        }

        //[DebuggerStepThrough()]
        protected bool CheckEditMode()
        {
            if (Authentication.IsSystemRole())
                return false;

            if (Authentication.IsSupervisor())
                throw new Exception("Супервизору запрещено изменять схему.");

            if (IsLocked)
            {
                if (LockedByUserID != Authentication.UserID)
                    throw new InvalidOperationException(String.Format("Объект заблокирован пользователем \"{0}\"", LockedByUserName));
            }
            else
            {
                if (state == ServerSideObjectStates.New)
                {
                    return false;
                }
                else
                {
                    if (!IsClone)
                    {
                        //Lock();

                        // Пытаемся получить имя пакета
                        string packageName = String.Empty;
                        if (this is IPackage)
                        {
                            packageName = ((IPackage) this).Name;
                        }
                        else if (this.owner is IPackage)
                        {
                            packageName = ((IPackage)this.owner).Name;
                        }
                        else if (this.owner != null && this.owner.owner is IPackage)
                        {
                            packageName = ((IPackage)this.owner.owner).Name;
                        }

                        throw new InvalidOperationException(
                            String.Format("Для внесения изменений сначала необходимо заблокировать пакет \"{0}\".\nВстаньте на пакет и в контекстном меню выберите пункт \"Редактировать\".", packageName));//\nОсобо догадливые увидят этот пункт, так как они обновили версию клиента.
                    }
                }
            }
            return true;
        }

        #region ICloneable Members

        /// <summary>
        /// Создает новый объект, являющийся копией текущего экземпляра. 
        /// </summary>
        /// <returns>Новый объект, являющийся копией текущего экземпляра.</returns>
        //[DebuggerStepThrough()]
        public virtual Object Clone()
        {
            ServerSideObject clone = (ServerSideObject)MemberwiseClone(false);
            lockedByUserID = (int)Authentication.UserID;
            lockedByUserName = Authentication.UserName;
            clone.identifier = GetNewIdentifier();
            clone.IsClone = true;
            cloneObject = clone;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Trace.TraceVerbose("Clone({0}) {1}", this.ToString(), clone.identifier);
            Console.ResetColor();
            return clone;
        }

        #endregion

        [DebuggerStepThrough()]
        public override string ToString()
        {
            return String.Format("{0} State({1}) {2} I{3} : {4}", IsClone ? "(Clone)" : String.Empty, state, lockedByUserName, identifier, base.ToString());
        }

        /// <summary>
        /// Возвращает псевдо-уникальное имя по указанному гвиду.
        /// </summary>
        public static string GetNewName(string newObjectKey)
        {
            string g = new Guid(newObjectKey).ToString("N");
            while (Char.IsDigit(g[0]))
                g = Guid.NewGuid().ToString("N");
            return g.Substring(0, 7);
        }
    }
}
