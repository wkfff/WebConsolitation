using System;
using Microsoft.SqlServer.Dts.Runtime;
using System.Globalization;

namespace Krista.FM.Utils.DTSGenerator
{
    /// <summary>
    /// Обрабатываемые события, возникающие при создании схемы переноса данных
    /// </summary>
    public sealed class PackageEvents : IDTSEvents
    {
        #region IDTSEvents Members

        void IDTSEvents.OnBreakpointHit(IDTSBreakpointSite breakpointSite, BreakpointTarget breakpointTarget)
        {
            // TODO:  Add PackageEvents.OnBreakpointHit implementation
        }

        bool IDTSEvents.OnQueryCancel()
        {
            // TODO:  Add PackageEvents.OnQueryCancel implementation
            return false;
        }

        void IDTSEvents.OnTaskFailed(TaskHost taskHost)
        {
            Console.WriteLine("Неуспешно выполненная задача " + taskHost.Name + "\n");
        }

        void IDTSEvents.OnWarning(DtsObject source, int warningCode, string subComponent, string description, string helpFile, int helpContext, string idofInterfaceWithError)
        {
            string sourceName = ((IDTSName)source).Name;
            Console.WriteLine("Предупреждение " + "\nWarningCode " + warningCode
                              + "\nSource  " + sourceName + "\nSubComponent " + subComponent
                              + "\nDescription " + description + "IDOfInterfaceWithError "
                              + idofInterfaceWithError + "\n");
        }

        void IDTSEvents.OnCustomEvent(TaskHost taskHost, string eventName, string eventText, ref object[] arguments, string subComponent, ref bool fireAgain)
        {
            // TODO:  Add PackageEvents.OnCustomEvent implementation
        }

        void IDTSEvents.OnProgress(TaskHost taskHost, string progressDescription, int percentComplete, int progressCountLow, int progressCountHigh, string subComponent, ref bool fireAgain)
        {
            Console.WriteLine("Выполнение " + "\nTaskHost " + taskHost.Name
                              + "\nProgressDescription " + progressDescription
                              + "\nPercentComplete " + percentComplete.ToString(
                                                           CultureInfo.InvariantCulture) + "\n");
        }

        void IDTSEvents.OnInformation(DtsObject source, int informationCode, string subComponent, string description, string helpFile, int helpContext,
                                      string idofInterfaceWithError, ref bool fireAgain)
        {
            Console.WriteLine("Информационной сообщение" + "\nSubComponent: "
                              + subComponent + "\nDescription: " + description);
        }

        void IDTSEvents.OnVariableValueChanged(DtsContainer dtsContainer, Variable variable, ref bool fireAgain)
        {
            // TODO:  Add PackageEvents.OnVariableValueChanged implementation
        }

        bool IDTSEvents.OnError(DtsObject source, int errorCode, string subComponent, string description, string helpFile, int helpContext, string idofInterfaceWithError)
        {
            string sourceName = ((IDTSName)source).Name;

            Console.WriteLine("Ошибка " + "\nErrorCode " + errorCode
                              + "\nSource " + sourceName + "\nSubComponent " + subComponent
                              + "\nDescription " + description + "\nIDOfInterfaceWithError "
                              + idofInterfaceWithError + "\n");

            return false;
        }

        void IDTSEvents.OnPreExecute(Executable exec, ref bool fireAgain)
        {
            TaskHost th = exec as TaskHost;
            Package p = exec as Package;

            if (th == null)
            {
                Console.WriteLine("Событие перед выполнением пакета\n" + p.Name + "\n");
            }
            else if (p == null)
            {
                Console.WriteLine("Событие перед выполнением задачи\n" + th.Name + "\n");
            }
        }

        void IDTSEvents.OnPostExecute(Executable exec, ref bool fireAgain)
        {
            TaskHost th = exec as TaskHost;
            Package p = exec as Package;

            if (th == null)
            {
                Console.WriteLine("Событие после выполнения пакета \n" + p.Name + ": " + p.Execute(null, null, null, null, null)+ "\n");
            }
            else if (p == null)
            {
                Console.WriteLine("Событие после выполнения задачи\n" + th.Name + "\n");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exec"></param>
        /// <param name="fireAgain"></param>
        void IDTSEvents.OnPostValidate(Executable exec, ref bool fireAgain)
        {
            TaskHost th = exec as TaskHost;
            Package p = exec as Package;

            if (th == null)
            {
                Console.WriteLine("Событие после валидации пакета " + p.Name + "\n");
            }
            else if (p == null)
            {
                Console.WriteLine("Событие после валидации задачи" + th.Name + "\n");
            }
        }

        /// <summary>
        /// Событие перед валидацией пакета
        /// </summary>
        /// <param name="exec"></param>
        /// <param name="fireAgain"></param>
        void IDTSEvents.OnPreValidate(Executable exec, ref bool fireAgain)
        {
            TaskHost th = exec as TaskHost;
            Package p = exec as Package;

            if (th == null)
            {
                Console.WriteLine("OnPreValidate\n" + p.Name + "\n");
            }
            else if (p == null)
            {
                Console.WriteLine("OnPreValidate\n" + th.Name + "\n");
            }
        }

        void IDTSEvents.OnExecutionStatusChanged(Executable exec, DTSExecStatus newStatus, ref bool fireAgain)
        {
            // TODO:  Add PackageEvents.OnExecutionStatusChanged implementation
        }

        #endregion
    }

    public class ComponentEvents : IDTSComponentEvents
    {
        #region Implementation of IDTSComponentEvents

        void IDTSComponentEvents.FireBreakpointHit(BreakpointTarget breakpointTarget)
        {
        }

        bool IDTSComponentEvents.FireQueryCancel()
        {
            return false;
        }

        void IDTSComponentEvents.FireWarning(int warningCode, string subComponent, string description, string helpFile, int helpContext)
        {
            Console.WriteLine("OnWarning" + "\nWarningCode "
                              + warningCode.ToString(CultureInfo.InvariantCulture)
                              + "\nSubComponent " + subComponent
                              + "\nDescription " + description + "\n");
        }

        void IDTSComponentEvents.FireCustomEvent(string eventName, string eventText, ref object[] arguments, string subComponent, ref bool fireAgain)
        {
        }

        void IDTSComponentEvents.FireProgress(string progressDescription, int percentComplete, int progressCountLow, int progressCountHigh, string subComponent, ref bool fireAgain)
        {
            Console.WriteLine("OnProgress " + progressDescription
                              + "\nPercentComplete " + percentComplete);
        }

        bool IDTSComponentEvents.FireError(int errorCode, string subComponent, string description, string helpFile, int helpContext)
        {
            Console.WriteLine("OnError" + "\nErrorCode "
                              + errorCode.ToString(CultureInfo.InvariantCulture)
                              + "\nSubComponent " + subComponent
                              + "\nDescription " + description);

            return false;
        }

        void IDTSComponentEvents.FireInformation(int informationCode, string subComponent, string description, string helpFile, int helpContext, ref bool fireAgain)
        {
            Console.WriteLine("FireInformation" + "\nSubComponent: "
                              + subComponent + "\nDescription: " + description
                              + "\nInformationCode: " + informationCode);
        }


        #endregion
    }
}