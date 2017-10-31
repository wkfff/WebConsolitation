using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Common.Handling
{
    /// <summary>
    /// Класс общих функции для работы с событиями
    /// </summary>
    public static class EventsProcessing
    {
        #region Так должно бы было быть

        // *******************************************************************************
        // Борисов: Вообще, обработка событий должна была бы выглядеть так.
        // Но при тестировании мне показалось что DynamicInvoke работает чуть медленнее,
        // нужно провести более подробное профилирование.
        // Также нужно рассмотреть вопрос о полном отказе от текущей схемы в пользу опроса
        // состояния закачки по таймеру
        // *******************************************************************************

        /*
        private static object ProcessInvocationList(Delegate dlg, 
            Type resultType, params object[] parameters)
        {
            object result = null;
            if (dlg == null)
                return result;
            Delegate[] invocationList = dlg.GetInvocationList();
            foreach (Delegate curDlg in invocationList)
            {
                try
                {
                    object curResult = curDlg.DynamicInvoke(parameters);

                    if (resultType == null)
                        continue;
                    switch (resultType.FullName)
                    {
                        case "System.String":
                            result += curResult + Environment.NewLine;
                            break;
                        case "System.Boolean":
                            if (result == null)
                                result = curResult;
                            else
                                result = (bool)result || (bool)curResult;
                            break;
                    }
                }
                catch
                {
                    Delegate.Remove(dlg, curDlg);
                }
            }
            return result;
        }

        public static void OnGetStringDelegateEvent(GetStringDelegate evt, string progID)
        {
            ProcessInvocationList(evt, null, progID);
        }

        public static void OnGetPumpStateDelegateEvent(GetPumpStateDelegate evt, PumpProcessStates state)
        {
            ProcessInvocationList(evt, null, state);
        }

        public static void OnPumpProcessStateChangedDelegateEvent(PumpProcessStateChangedDelegate evt,
            PumpProcessStates prevState, PumpProcessStates currState)
        {
            ProcessInvocationList(evt, null, prevState, currState);
        }

        public static void OnGetVoidDelegateEvent(GetVoidDelegate evt)
        {
            ProcessInvocationList(evt, null);
        }

        public static string OnDeleteDataDelegateEvent(DeleteDataDelegate evt, 
            int pumpID, int sourceID)
        {
            object result = ProcessInvocationList(evt, typeof(string), pumpID, sourceID);
            if (result == null)
                return String.Empty;
            else
                return result.ToString();
        }

        public static bool OnGetBoolDelegateEvent(GetBoolDelegate evt)
        {
            object result = ProcessInvocationList(evt, typeof(bool));
            if (result == null)
                return false;
            else
                return (bool)result;
        }
         */
        #endregion

        #region Так есть сейчас. Дублирование кода
        
        /// <summary>
        /// Метод, вызываемый при генерировании события
        /// </summary>
        public static void OnGetStringDelegateEvent(ref GetStringDelegate evt, string progID)
        {
            // Все обработчики события нужно вызывать вручную для обнаружения отключившихся клиентов
            if (evt != null)
            {
                Delegate[] delegate_list = evt.GetInvocationList();
                foreach (Delegate dlg in delegate_list)
                {
                    GetStringDelegate handler = (GetStringDelegate)dlg;
                    try
                    {
                        handler.Invoke(progID);
                    }
                    catch
                    {
                        evt -= handler;
                    }
                }
            }
        }

        /// <summary>
        /// Метод, вызываемый при генерировании события
        /// </summary>
        public static void OnGetPumpStateDelegateEvent(ref GetPumpStateDelegate evt, PumpProcessStates state)
        {
            // Все обработчики события нужно вызывать вручную для обнаружения отключившихся клиентов
            if (evt != null)
            {
                Delegate[] delegate_list = evt.GetInvocationList();
                foreach (Delegate dlg in delegate_list)
                {
                    GetPumpStateDelegate handler = (GetPumpStateDelegate)dlg;
                    try
                    {
                        handler.Invoke(state);
                    }
                    catch
                    {
                        evt -= handler;
                    }
                }
            }
        }

        /// <summary>
        /// Метод, вызываемый при генерировании события
        /// </summary>
        public static void OnPumpProcessStateChangedDelegateEvent(ref PumpProcessStateChangedDelegate evt,
            PumpProcessStates prevState, PumpProcessStates currState)
        {
            // Все обработчики события нужно вызывать вручную для обнаружения отключившихся клиентов
            if (evt != null)
            {
                Delegate[] delegate_list = evt.GetInvocationList();
                foreach (Delegate dlg in delegate_list)
                {
                    PumpProcessStateChangedDelegate handler = (PumpProcessStateChangedDelegate)dlg;
                    try
                    {
                        handler.Invoke(prevState, currState);
                    }
                    catch
                    {
                        evt -= handler;
                    }
                }
            }
        }

        /// <summary>
        /// Метод, вызываемый при генерировании события
        /// </summary>
        public static void OnGetVoidDelegateEvent(ref GetVoidDelegate evt)
        {
            // Все обработчики события нужно вызывать вручную для обнаружения отключившихся клиентов
            if (evt != null)
            {
                Delegate[] delegate_list = evt.GetInvocationList();
                foreach (Delegate dlg in delegate_list)
                {
                    GetVoidDelegate handler = (GetVoidDelegate)dlg;
                    try
                    {
                        handler.Invoke();
                    }
                    catch
                    {
                        evt -= handler;
                    }
                }
            }
        }

        /// <summary>
        /// Вызов событий удаления закачанных данных
        /// </summary>
        /// <param name="pumpID">ИД закачки</param>
        /// <param name="sourceID">ИД источника</param>
        /// <returns>Строка ошибки</returns>
        public static string OnDeleteDataDelegateEvent(ref DeleteDataDelegate evt, int pumpID, int sourceID)
        {
            string result = string.Empty;

            // Все обработчики события нужно вызывать вручную для обнаружения отключившихся клиентов
            if (evt != null)
            {
                Delegate[] delegate_list = evt.GetInvocationList();
                foreach (Delegate dlg in delegate_list)
                {
                    DeleteDataDelegate handler = (DeleteDataDelegate)dlg;
                    try
                    {
                        result += handler.Invoke(pumpID, sourceID) + "\n";
                    }
                    catch
                    {
                        evt -= handler;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Метод, вызываемый при генерировании события
        /// </summary>
        public static bool OnGetBoolDelegateEvent(ref GetBoolDelegate evt)
        {
            bool result = false;

            // Все обработчики события нужно вызывать вручную для обнаружения отключившихся клиентов
            if (evt != null)
            {
                Delegate[] delegate_list = evt.GetInvocationList();
                foreach (Delegate dlg in delegate_list)
                {
                    GetBoolDelegate handler = (GetBoolDelegate)dlg;
                    try
                    {
                        result = result || handler.Invoke();
                    }
                    catch
                    {
                        evt -= handler;
                    }
                }
            }

            return result;
        }
         
        #endregion
    }
}
