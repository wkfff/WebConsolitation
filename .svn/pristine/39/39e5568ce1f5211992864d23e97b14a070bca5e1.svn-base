using System.Runtime.InteropServices;

namespace Krista.FM.Update.Framework
{
    /// <summary>
    /// Используем wrapper для взаимодействия с COM
    /// </summary>
    [ComVisible(true)]
    [Guid("BE47FCD5-34A7-4B6E-BDAC-8E9A6C73649B"), ProgId("Krista.FM.UpdateManager")] //C0686889-854D-445f-8433-30BEF0E092B1
    public class UpdateManagerWrapper : IUpdateManagerWrapper
    {
        public void InitializeNotifyIconForm()
        {
            UpdateManager.Instance.InitializeNotifyIconForm();
        }

        /// <summary>
        /// Строка с версиями модулей сервера
        /// </summary>
        public string ServerModulesString
        {
            set { UpdateManager.Instance.ServerModulesString = value; }
        }
    }

    [ComVisible(true)]
    [Guid("4932ADA3-4D9D-4EEE-9AE9-286E3269F418")]
    public interface IUpdateManagerWrapper
    {
        void InitializeNotifyIconForm();
        /// <summary>
        /// Строка с версиями модулей сервера
        /// </summary>
        string ServerModulesString { set; }
    }
}
