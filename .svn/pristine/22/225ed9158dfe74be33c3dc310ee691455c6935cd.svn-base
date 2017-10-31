using System.IO;
using System.Linq;
using System.Xml;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Services
{
    public interface IStateSystemService : INewRestService
    {
        /// <summary>
        /// Admin - константа
        /// </summary>
        string Admin { get; }

        /// <summary>
        /// PPO - константа
        /// </summary>
        string Ppo { get; }

        /// <summary>
        /// GRBS - константа
        /// </summary>
        string Grbs { get; }

        /// <summary>
        /// User - константа
        /// </summary>
        string User { get; }
        
        /// <summary>
        /// Получение типа документа
        /// </summary>
        /// <param name="docId">Идентификатор документа</param>
        /// <returns>Идентификатор типа документа</returns>
        int GetTypeDocID(int docId);

        /// <summary>
        /// Получение схемы переходов состояний по типу документа
        /// </summary>
        /// <param name="typeDocID">Идентификатор типа документа</param>
        /// <returns>Идентификатор схемы</returns>
        int? GetSchemStateTransitionsID(int typeDocID);

        /// <summary>
        /// Получение начального состояния из схемы состояний
        /// </summary>
        /// <param name="schemStateTransitionsID">Идентификатор схемы состояний</param>
        /// <returns>Идентификатор состояния</returns>
        int GetStartStateID(int schemStateTransitionsID);

        /// <summary>
        /// Получение эгземпляра состояния по ID
        /// </summary>
        /// <param name="stateID">Идентификатор состояния</param>
        /// <returns>объект состояния</returns>
        FX_Org_SostD GetState(int stateID);

        /// <summary>
        /// Получение эгземпляра состояния схемы по ID состояния
        /// </summary>
        /// <param name="stateID">ID состояния</param>
        /// <param name="schemTransitions">идентификатор схемы</param>
        /// <returns>эгземпляр состояния схемы</returns>
        D_State_SchemStates GetSchemState(int stateID, int schemTransitions);

        /// <summary>
        /// Получение схемы состояний по ID
        /// </summary>
        /// <param name="schemStateTransitionsID">идентификатор схемы</param>
        /// <returns>схема состояний</returns>
        D_State_SchemTransitions GetSchemStateTransitions(int schemStateTransitionsID);

        /// <summary>
        /// Получение переходов системы состояний
        /// </summary>
        /// <param name="schemStateTransitionsID">идентификатор системы состояний</param>
        /// <returns>коллекция переходов</returns>
        IQueryable<D_State_Transitions> GetTransitions(int schemStateTransitionsID);

        /// <summary>
        /// Получение разрешенных переходов для состояния
        /// </summary>
        /// <param name="stateID">идентификатор состояния</param>
        /// <returns>коллекция переходов</returns>
        IQueryable<D_State_Transitions> GetAllowTransitions(int stateID);

        /// <summary>
        /// Получение ролей пользователей для которых разрешен переход с заданного состояния
        /// </summary>
        /// <param name="stateID">идентификатор состояния</param>
        /// <param name="transitionID">идентификатор перехода</param>
        /// <returns>коллекция ролей пользователей</returns>
        IQueryable<D_State_RightsTransition> GetRightsTransition(int stateID, int transitionID);

        /// <summary>
        /// Получение текущего состояния документа
        /// </summary>
        /// <param name="docId">Идентификатор документа</param>
        /// <returns>Идентификатор состояния</returns>
        int GetCurrentStateID(int docId);

        /// <summary>
        /// Проверка на разрешенность перехода для состояния.
        /// Возвращает true если переход присутствует среди разрешенных переходов для состояния
        /// </summary>
        /// <param name="stateID">идентификатор состояния</param>
        /// <param name="transitionID">идентификатор перехода</param>
        /// <returns>true - если разрешен</returns>
        bool CheckAllowTranstion(int stateID, int transitionID);

        /// <summary>
        /// Проверка прав на переход из заданного состояния для текущего пользователя
        /// </summary>
        /// <param name="stateID">
        /// идентификатор состояния
        /// </param>
        /// <param name="transitionID">
        /// идентификатор перехода
        /// </param>
        /// <returns>
        /// true - если переход разрешен для текущего пользователя
        /// </returns>
        bool CheckRightsTransition(int stateID, int transitionID);

        /// <summary>
        /// Выполнить переход для документа
        /// </summary>
        /// <param name="docId">идентификатор документа</param>
        /// <param name="transitionID">идентификатор перехода</param>
        void Jump(int docId, int transitionID);

        /// <summary>
        /// Выполнить смену состояний документа
        /// </summary>
        /// <param name="docId">идентификатор документа</param>
        /// <param name="stateID">идентификатор состояния</param>
        void SetState(int docId, int stateID);

        /// <summary>
        /// Изменение примечания документа
        /// </summary>
        /// <param name="docId">идентификатор документа</param>
        /// <param name="note">примечание документа</param>
        /// <param name="add">флаг добавления или замены примечания</param>
        void ChangeNotes(int docId, string note, bool add = true);

        /// <summary>
        /// Экспорт схемы в XML
        /// </summary>
        /// <param name="recId">идентификатор схемы</param>
        /// <returns>поток с XMLем</returns>
        Stream Export(int recId);

        /// <summary>
        /// Импорт схемы из XML
        /// </summary>
        /// <param name="xmlFile">xml файл</param>
        void Import(XmlTextReader xmlFile);

        bool GetDocClosure(int docId);
    }
}