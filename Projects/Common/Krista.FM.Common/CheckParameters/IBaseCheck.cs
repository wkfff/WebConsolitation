using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Common
{
    /// <summary>
    /// Базовый интерфейс для всех типов проверки корректности параметра
    /// </summary>
    interface IBaseCheck
    {
        /// <summary>
        /// Метод проверки корректности параметра
        /// </summary>
        void Execute();
    }
}
