using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Krista.FM.Common
{
    /// <summary>
    /// Проверка на основе регулярного выражения.
    /// </summary>
    public class CheckRegEx : CheckRule
    {
        public CheckRegEx(string parametr, string value, string errorMessage, bool invalid)
            : base (parametr, value, errorMessage, invalid)
        {
        }

        #region ICheck Members

        /// <summary>
        /// Метод проверки
        /// </summary>
        /// <returns></returns>
        public override bool Execute(string value)
        {
            bool result = false;

            Regex rx = new Regex(this.Value);

            if (rx.IsMatch(value))
                result = true;

            return Invalid ? !result : result;
        }

        #endregion
    }
}
