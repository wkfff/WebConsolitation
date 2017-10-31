using System;
using System.Collections.Generic;
using System.Text;
using DSO;

namespace Krista.FM.Server.OLAP.Processor
{
    /// <summary>
    /// Класс для обработки ошибок
    /// </summary>
    public class ErrorHandler
    {
        private Dictionary<int, Type> errors;

        private static ErrorHandler instance;

        public ErrorHandler()
        {
            this.errors = new Dictionary<int, Type>();
            InitializeErrors();
        }

        /// <summary>
        /// Инизиализация списка обрабатываемых ошибок
        /// </summary>
        private void InitializeErrors()
        {
            errors.Add(-2147221406, typeof(MderrDimensionMemberNotFound));
        }

        /// <summary>
        /// Экземпляр класса обработки ошибок
        /// </summary>
        public static ErrorHandler Instance
        {
            get
            {
                if (instance == null)
                    instance = new ErrorHandler();

                return instance;
            }
        }

        public Dictionary<int, Type> Errors
        {
            get { return errors; }
        }
    }

   
}
