using System.Collections.Generic;
using System.Collections.ObjectModel;
using Krista.FM.Common.Calculator.Exceptions;
using NCalc;

namespace Krista.FM.Common.Calculator.Utils
{
    /// <summary>
    /// Анализирует выражение и выделяет из него требуемые для вычисления параметры
    /// </summary>
    public class ArgumentsParser
    {
        private ArgumentsParser(string expression)
        {
            if (string.IsNullOrEmpty(expression))
            {
                throw new InvalidConfigurationException("Выражение формулы не может быть пустым");
            }

            Results = new Collection<string>();
            var parser = new Expression(expression);
            parser.EvaluateParameter += OnParseArgument;
            parser.EvaluateFunction += OnParseFunction;
            parser.Evaluate();
        }

        /// <summary>
        /// Возвращает список параметров, которые потребуются при вычислении указанного выражения
        /// </summary>
        public static ICollection<string> Parse(string expression)
        {
            return (new ArgumentsParser(expression)).Results;
        }

        private ICollection<string> Results { get; set; }

        private void OnParseArgument(string name, ParameterArgs args)
        {
            if (!Results.Contains(name))
            {
                Results.Add(name);
            }

            // Заглушка, так как должно быть возвращено какое-то значение
            args.Result = 0;
        }

        private void OnParseFunction(string name, FunctionArgs args)
        {
            foreach (Expression param in args.Parameters)
            {
                param.Evaluate();
            }

            // Заглушка, так как должно быть возвращено какое-то значение
            args.Result = 0;
        }
    }
}
