using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Server.OLAP.Processor
{
    /// <summary>
    /// Обработчик ошибки -2147221406
    /// </summary>
    public class MderrDimensionMemberNotFound : BaseError
    {
        public MderrDimensionMemberNotFound(int code)
            : base(code)
        {
            this.description = "A member was found in the fact table, but not in the dimension.";
        }

        public override string Execute(string errorMessage)
        {
            string[] parts = errorMessage.Split('\a');

            if (parts.Length == 3)
            {
                return String.Format("A member with key '{0}' was found in the fact table but was not found in the level {1} of the dimension {2}. Код ошибки: {3}",
                    parts[0], parts[2], parts[1], code);
            }
            if (parts.Length == 2)
            {
                return String.Format("A member with key '{0}' was found in the fact table but was not found in the dimension {1}. Код ошибки: {3}",
                    parts[0], parts[2], code);
            }

            return String.Format("{0}. Код ошибки: {1}", errorMessage, code);
        }
    }
}