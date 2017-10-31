using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Common
{
    /// <summary>
    /// Правило проверки
    /// </summary>
    public abstract class CheckRule : ICheck
    {
        /// <summary>
        /// Уникальное имя параметра
        /// </summary>
        private string parametr;
        /// <summary>
        /// Условия корректность
        /// </summary>
        private string value;
        /// <summary>
        /// Строка предупреждения
        /// </summary>
        private string errorMessage;
        /// <summary>
        /// 
        /// </summary>
        private bool invalid;        
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="parametr"></param>
        /// <param name="className"></param>
        /// <param name="value"></param>
        /// <param name="errorMessage"></param>
        public CheckRule(string parametr, string value, string errorMessage, bool invalid)
            : this (parametr, value, errorMessage)
        {
            
            this.invalid = invalid;
        }

        public CheckRule(string parametr, string value, string errorMessage)
        {
            this.parametr = parametr;
            this.value = value;
            this.errorMessage = errorMessage;
        }

        /// <summary>
        /// Конструктор по-умолчанию
        /// </summary>
        public CheckRule()
        { 
        }
        /// <summary>
        /// Уникальное имя параметра
        /// </summary>
        public string Parametr
        {
            get { return parametr; }
        }
        /// <summary>
        /// Условие корректности
        /// </summary>
        public string Value
        {
            get { return value; }
        }
        /// <summary>
        /// Строка предупреждения
        /// </summary>
        public string ErrorMessage
        {
            get { return errorMessage; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool Invalid
        {
            get { return invalid; }
            set { invalid = value; }
        }

        #region ICheck Members

        public virtual bool Execute(string value)
        {
            return true;
        }

        #endregion
    }
}
