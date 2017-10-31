using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Services.ChangeLog;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Services
{
    public class StateSystemService : NewRestService, IStateSystemService
    {
        #region IStateSystemService Members

        /// <summary>
        /// Группа Admin
        /// </summary>        
        public string Admin
        {
            get { return "Admin"; }
        }

        /// <summary>
        /// Группа PPO
        /// </summary>        
        public string Ppo
        {
            get { return "PPO"; }
        }

        /// <summary>
        /// Группа GRBS
        /// </summary>        
        public string Grbs
        {
            get { return "GRBS"; }
        }

        /// <summary>
        /// Группа User
        /// </summary>
        public string User
        {
            get { return "User"; }
        }

        /// <summary>
        ///   Получение типа документа
        /// </summary>
        /// <param name="docId"> Идентификатор документа </param>
        /// <returns> Идентификатор типа документа </returns>
        public int GetTypeDocID(int docId)
        {
            try
            {
                return GetItem<F_F_ParameterDoc>(docId).RefPartDoc.ID;
            }
            catch (Exception e)
            {
                throw new Exception("Не удалось получить тип документа.  " + e.Message);
            }
        }

        /// <summary>
        ///   Получение схемы переходов состояний по типу документа
        /// </summary>
        /// <param name="typeDocID"> Идентификатор типа документа </param>
        /// <returns> Идентификатор схемы </returns>
        public int? GetSchemStateTransitionsID(int typeDocID)
        {
            try
            {
                return (from p in GetItems<D_State_SchemTransitions>()
                        where p.RefPartDoc.ID == typeDocID
                        select p).First().ID;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///   Получение начального состояния из схемы состояний
        /// </summary>
        /// <param name="schemStateTransitionsID"> Идентификатор схемы состояний </param>
        /// <returns> Идентификатор состояния </returns>
        public int GetStartStateID(int schemStateTransitionsID)
        {
            try
            {
                return (from p in GetItems<D_State_SchemStates>()
                        where (p.RefSchemStateTransitions.ID == schemStateTransitionsID) &&
                              (p.IsStart == true)
                        select p).First().RefStates.ID;
            }
            catch (Exception e)
            {
                throw new Exception(
                    "Не удалось получить начальное состояние схемы переходов состояний.  " +
                    e.Message);
            }
        }

        /// <summary>
        ///   Получение эгземпляра состояния по ID
        /// </summary>
        /// <param name="stateID"> Идентификатор состояния </param>
        /// <returns> объект состояния </returns>
        public FX_Org_SostD GetState(int stateID)
        {
            try
            {
                return GetItem<FX_Org_SostD>(stateID);
            }
            catch (Exception e)
            {
                throw new Exception("Не удалось получить объект-состояние.  " + e.Message);
            }
        }

        /// <summary>
        ///   Получение эгземпляра состояния схемы по ID состояния
        /// </summary>
        /// <param name="stateID"> ID состояния </param>
        /// <param name="schemTransitions"> идентификатор схемы </param>
        /// <returns> эгземпляр состояния схемы </returns>
        public D_State_SchemStates GetSchemState(int stateID, int schemTransitions)
        {
            try
            {
                return (from p in GetItems<D_State_SchemStates>()
                        where (p.RefStates.ID == stateID) &&
                              (p.RefSchemStateTransitions.ID == schemTransitions)
                        select p).First();
            }
            catch (Exception e)
            {
                throw new Exception("Не удалось получить состояние схемы.  " + e.Message);
            }
        }

        /// <summary>
        ///   Получение схемы состояний по ID
        /// </summary>
        /// <param name="schemStateTransitionsID"> идентификатор схемы </param>
        public D_State_SchemTransitions GetSchemStateTransitions(int schemStateTransitionsID)
        {
            try
            {
                return GetItem<D_State_SchemTransitions>(schemStateTransitionsID);
            }
            catch (Exception e)
            {
                throw new Exception("Не удалось получить объект-схемы.  " + e.Message);
            }
        }

        /// <summary>
        ///   Получение переходов системы состояний
        /// </summary>
        /// <param name="schemStateTransitionsID"> идентификатор системы состояний </param>
        /// <returns> коллекция переходов </returns>
        public IQueryable<D_State_Transitions> GetTransitions(int schemStateTransitionsID)
        {
            try
            {
                return from p in GetItems<D_State_Transitions>()
                       where p.RefSchemStateTransitions.ID == schemStateTransitionsID
                       select p;
            }
            catch (Exception e)
            {
                throw new Exception("Не удалось получить переходы системы состояний.  " + e.Message);
            }
        }

        /// <summary>
        ///   Получение разрешенных переходов для состояния
        /// </summary>
        /// <param name="stateID"> идентификатор состояния </param>
        /// <returns> коллекция переходов </returns>
        public IQueryable<D_State_Transitions> GetAllowTransitions(int stateID)
        {
            try
            {
                return (from p in GetItems<D_State_OptionsTransition>()
                        where p.RefSchemStates.RefStates.ID == stateID
                        select p).Select(x => x.RefTransitions);
            }
            catch (Exception e)
            {
                throw new Exception("Не удалось получить переходы для состояния.  " + e.Message);
            }
        }

        /// <summary>
        /// Получение ролей пользователей для которых разрешен переход заданного состояния
        /// </summary>
        /// <param name="stateID">
        /// идентификатор состояния 
        /// </param>
        /// <param name="transitionID">
        /// идентификатор перехода 
        /// </param>
        /// <returns>
        /// коллекция ролей пользователей 
        /// </returns>
        public IQueryable<D_State_RightsTransition> GetRightsTransition(int stateID, int transitionID)
        {
            try
            {
                return from p in GetItems<D_State_RightsTransition>()
                       where (p.RefOptionsTransition.RefSchemStates.RefStates.ID == stateID) &&
                             (p.RefOptionsTransition.RefTransitions.ID == transitionID)
                       select p;
            }
            catch (Exception e)
            {
                throw new Exception("Не удалось получить роли для перехода.  " + e.Message);
            }
        }

        /// <summary>
        ///   Получение текущего состояния документа
        /// </summary>
        /// <param name="docId"> Идентификатор документа </param>
        /// <returns> Идентификатор состояния </returns>
        public int GetCurrentStateID(int docId)
        {
            try
            {
                return GetItem<F_F_ParameterDoc>(docId).RefSost.ID;
            }
            catch (Exception e)
            {
                throw new Exception("Не удалось получить текущее состояние документа.  " + e.Message);
            }
        }

        /// <summary>
        ///   Проверка на разрешенность перехода для состояния.
        ///   Возвращает true если переход присутствует среди разрешенных переходов для состояния
        /// </summary>
        /// <param name="stateID"> идентификатор состояния </param>
        /// <param name="transitionID"> идентификатор перехода </param>
        public bool CheckAllowTranstion(int stateID, int transitionID)
        {
            return GetAllowTransitions(stateID).Any(x => x.ID == transitionID);
        }

        /// <summary>
        /// Проверка прав на переход текущего пользователя
        /// </summary>
        /// <param name="stateID">
        /// идентификатор состояния 
        /// </param>
        /// <param name="transitionID">
        /// идентификатор перехода 
        /// </param>
        /// <returns>
        /// true если переход разрешен для текущего пользователя 
        /// </returns>
        public bool CheckRightsTransition(int stateID, int transitionID)
        {
            IQueryable<D_State_RightsTransition> rightsTransition = GetRightsTransition(stateID, transitionID);
            if (!rightsTransition.Any())
            {
                return true;
            }

            var auth = Resolver.Get<IAuthService>();
            string rightUser;
            if (auth.IsAdmin())
            {
                rightUser = Admin;
            }
            else
            {
                if (auth.IsSpectator())
                {
                    rightUser = "Spectator";
                }
                else if (auth.IsGrbsUser() || auth.IsPpoUser())
                {
                    rightUser = auth.IsGrbsUser() ? Grbs : Ppo;
                }
                else
                {
                    rightUser = User;
                }
            }

            return rightsTransition.Any(x => x.AccountsRole == rightUser);
        }

        /// <summary>
        ///   Выполнить переход для документа
        /// </summary>
        /// <param name="docId"> идентификатор документа </param>
        /// <param name="transitionID"> идентификатор перехода </param>
        public void Jump(int docId, int transitionID)
        {
            try
            {
                int stateID = GetItem<D_State_Transitions>(transitionID).RefSchemStates.RefStates.ID;
                SetState(docId, stateID);
            }
            catch (Exception e)
            {
                throw new Exception("Не удалось выполнить переход.  " + e.Message);
            }
        }

        /// <summary>
        ///   Выполнить смену состояний документа
        /// </summary>
        /// <param name="docId"> идентификатор документа </param>
        /// <param name="stateID"> идентификатор состояния </param>
        public void SetState(int docId, int stateID)
        {
            try
            {
                var log = Resolver.Get<IChangeLogService>();
                //// меняем состояние документу
                var doc = GetItem<F_F_ParameterDoc>(docId);
                doc.RefSost = GetState(stateID);
                Save(doc);
                CommitChanges();
                switch (stateID)
                {
                    case FX_Org_SostD.ExportedStateID:
                        log.WriteChange(doc, FX_FX_ChangeLogActionType.OnExportedState);
                        break;
                    case FX_Org_SostD.FinishedStateID:
                        log.WriteChange(doc, FX_FX_ChangeLogActionType.OnFinishedState);
                        break;
                    case FX_Org_SostD.OnEditingStateID:
                        log.WriteChange(doc, FX_FX_ChangeLogActionType.OnEditingState);
                        break;
                    case FX_Org_SostD.UnderConsiderationStateID:
                        log.WriteChange(doc, FX_FX_ChangeLogActionType.OnUnderConsiderationState);
                        break;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Не удалось сменить состояние.  " + e.Message);
            }
        }

        /// <summary>
        ///   Изменение примечания документа
        /// </summary>
        /// <param name="docId"> идентификатор документа </param>
        /// <param name="note"> примечание документа</param>
        /// <param name="add"> флаг, добавления или замены примечания </param>
        public void ChangeNotes(int docId, string note, bool add = true)
        {
            // предел длинны строки в хибернейте(почему 4000!?)
            const int MaxLength = 4000;

            try
            {
                // меняем примечание документу
                var doc = GetItem<F_F_ParameterDoc>(docId);
                if (add)
                {
                    doc.Note += " " + note;
                }
                else
                {
                    doc.Note = note;
                }

                if (doc.Note.Length > MaxLength)
                {
                    doc.Note = doc.Note.Remove(MaxLength);
                }

                Save(doc);
                CommitChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Не удалось изменить примечание документа.  " + e.Message);
            }
        }

        /// <summary>
        ///   Экспорт схемы в XML
        /// </summary>
        public Stream Export(int recId)
        {
            try
            {
                D_State_SchemTransitions schemTransitions = GetSchemStateTransitions(recId);

                IQueryable<D_State_SchemStates> states = from p in GetItems<D_State_SchemStates>()
                                                         where p.RefSchemStateTransitions.ID == recId
                                                         select p;

                IQueryable<D_State_Transitions> transitions = GetTransitions(recId);

                IQueryable<D_State_OptionsTransition> optionsTransitions =
                    from p in GetItems<D_State_OptionsTransition>()
                    where p.RefSchemStates.RefSchemStateTransitions.ID == recId
                    select p;

                var doc = new XmlDocument();

                doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", null));
                XmlElement elem = doc.CreateElement("StateSystem");
                elem.SetAttribute("Name", schemTransitions.Name);
                elem.SetAttribute("Note", schemTransitions.Note);
                elem.SetAttribute("InitAction", schemTransitions.InitAction);
                elem.SetAttribute(
                    "PartDocID", 
                    schemTransitions.RefPartDoc.ID.ToString(CultureInfo.InvariantCulture));

                XmlNode rootNode = doc.AppendChild(elem);

                XmlNode statesNode = rootNode.AppendChild(doc.CreateElement("States"));

                foreach (D_State_SchemStates state in states)
                {
                    XmlElement stateElem = doc.CreateElement("State");
                    stateElem.SetAttribute("ID", state.RefStates.ID.ToString(CultureInfo.InvariantCulture));
                    stateElem.SetAttribute(
                        "IsStart",
                        state.IsStart.HasValue ? state.IsStart.ToString() : "False");
                    statesNode.AppendChild(stateElem);
                }

                XmlNode transitionsNode = rootNode.AppendChild(doc.CreateElement("Transitions"));

                foreach (D_State_Transitions transition in transitions)
                {
                    XmlElement transitionElem = doc.CreateElement("Transition");
                    transitionElem.SetAttribute("Name", transition.Name);
                    transitionElem.SetAttribute("Note", transition.Note);
                    transitionElem.SetAttribute("Ico", transition.Ico);
                    transitionElem.SetAttribute("Action", transition.Action);
                    transitionElem.SetAttribute(
                        "FinalState", 
                        transition.RefSchemStates.RefStates.ID.ToString(CultureInfo.InvariantCulture));
                    transitionElem.SetAttribute("TransitionClass", transition.TransitionClass);
                    transitionsNode.AppendChild(transitionElem);
                }

                if (optionsTransitions.Count() != 0)
                {
                    XmlNode optionsTransitionNode =
                        rootNode.AppendChild(doc.CreateElement("OptionsTransitions"));
                    foreach (D_State_OptionsTransition optionsTransition in optionsTransitions)
                    {
                        XmlElement optionsTransitionElem = doc.CreateElement("OptionsTransition");
                        optionsTransitionElem.SetAttribute(
                            "StateCode", 
                            optionsTransition.RefSchemStates.RefStates.ID.ToString(CultureInfo.InvariantCulture));
                        optionsTransitionElem.SetAttribute("Transition", optionsTransition.RefTransitions.Name);
                        optionsTransitionNode.AppendChild(optionsTransitionElem);

                        IEnumerable<D_State_RightsTransition> rights =
                            GetRightsTransition(
                                optionsTransition.RefSchemStates.RefStates.ID, 
                                optionsTransition.RefTransitions.ID);
                        if (rights.Count() != 0)
                        {
                            XmlNode rightNode = optionsTransitionElem.AppendChild(doc.CreateElement("Rights"));
                            foreach (D_State_RightsTransition right in rights)
                            {
                                XmlElement rightElem = doc.CreateElement("Right");
                                rightElem.SetAttribute("AccountsRole", right.AccountsRole);
                                rightNode.AppendChild(rightElem);
                            }
                        }
                    }
                }

                Stream stream = new MemoryStream();
                doc.Save(stream);
                stream.Position = 0;

                return stream;
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка при формировании XML файла.  " + e.Message);
            }
        }

        /// <summary>
        ///   Импорт схемы из XML
        /// </summary>
        public void Import(XmlTextReader xmlFile)
        {
            try
            {
                var schemTransitions = new D_State_SchemTransitions { ID = 0 };
                var optionsTransition = new D_State_OptionsTransition { ID = 0 };

                xmlFile.WhitespaceHandling = WhitespaceHandling.None; // пропускаем пустые узлы
                while (xmlFile.Read())
                {
                    if (xmlFile.NodeType == XmlNodeType.Element)
                    {
                        switch (xmlFile.Name)
                        {
                            case "StateSystem":
                                schemTransitions.Name = xmlFile.GetAttribute("Name");
                                schemTransitions.Note = xmlFile.GetAttribute("Note");
                                schemTransitions.InitAction = xmlFile.GetAttribute("InitAction");
                                schemTransitions.RefPartDoc = GetItem<FX_FX_PartDoc>(
                                    Convert.ToInt32(xmlFile.GetAttribute("PartDocID")));
                                Save(schemTransitions);
                                break;
                            case "State":
                                var state = new D_State_SchemStates
                                    {
                                        ID = 0, 
                                        RefStates = GetState(Convert.ToInt32(xmlFile.GetAttribute("ID"))), 
                                        IsStart = Convert.ToBoolean(xmlFile.GetAttribute("IsStart")), 
                                        RefSchemStateTransitions = schemTransitions
                                    };
                                Save(state);
                                break;
                            case "Transition":
                                var transition = new D_State_Transitions
                                    {
                                        ID = 0, 
                                        Name = xmlFile.GetAttribute("Name"), 
                                        Note = xmlFile.GetAttribute("Note"), 
                                        Action = xmlFile.GetAttribute("Action"), 
                                        Ico = xmlFile.GetAttribute("Ico"), 
                                        TransitionClass = xmlFile.GetAttribute("TransitionClass"), 
                                        RefSchemStates = GetSchemState(
                                            Convert.ToInt32(xmlFile.GetAttribute("FinalState")), 
                                            schemTransitions.ID), 
                                        RefSchemStateTransitions = schemTransitions
                                    };
                                Save(transition);
                                break;
                            case "OptionsTransition":
                                optionsTransition = new D_State_OptionsTransition
                                    {
                                        ID = 0, 
                                        RefSchemStates = GetSchemState(
                                            Convert.ToInt32(
                                                xmlFile.GetAttribute("StateCode")), 
                                            schemTransitions.ID), 
                                        RefTransitions = (from p in GetItems<D_State_Transitions>()
                                                          where
                                                              p.Name.Equals(
                                                                  xmlFile.GetAttribute("Transition"))
                                                              &&
                                                              (p.RefSchemStateTransitions.ID
                                                               == schemTransitions.ID)
                                                          select p).First()
                                    };

                                Save(optionsTransition);
                                break;
                            case "Right":
                                var rightsTransition = new D_State_RightsTransition
                                    {
                                        ID = 0, 
                                        AccountsRole = xmlFile.GetAttribute("AccountsRole"), 
                                        RefOptionsTransition = optionsTransition
                                    };

                                Save(rightsTransition);
                                break;
                        }
                    }
                }
            }
            finally
            {
                if (xmlFile != null)
                {
                    xmlFile.Close();
                }
            }
        }

        /// <summary>
        ///   Открыт или закрыт документ
        /// </summary>
        /// <param name="docId"> Идентификатор документа </param>
        /// <returns> Идентификатор типа документа </returns>
        public bool GetDocClosure(int docId)
        {
            try
            {
                return GetItem<F_F_ParameterDoc>(docId).CloseDate.HasValue;
            }
            catch (Exception e)
            {
                throw new Exception("Не удалось получить дату закрытия документа.  " + e.Message);
            }
        }

        #endregion
    }
}
