using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Server.OLAP.Processor
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseError
    {
        /// <summary>
        /// Код ошибки
        /// </summary>
        protected int code;
        /// <summary>
        /// Текст ошибки из документации
        /// </summary>
        protected string description;

        public BaseError(int code)
        {
            this.code = code;
        }

        public virtual string Execute(string errorMessage)
        {
            return errorMessage;
        }
    }
}
