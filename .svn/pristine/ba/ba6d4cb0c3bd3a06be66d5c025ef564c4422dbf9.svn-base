using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Krista.FM.Extensions
{
    public static class Monad
    {
        /// <summary>
        /// Проверки на null. Если this не null, то выполняет выражение, иначе возвращает null.
        /// </summary>
        /// <example>
        /// string postCode = this.With(x => person)
        ///                       .With(x => x.Address)
        ///                       .With(x => x.PostCode);
        /// </example>
        public static TResult With<TInput, TResult>(this TInput o, Func<TInput, TResult> evaluator)
            where TResult : class
            where TInput : class
        {
            if (o == null) return null;
            return evaluator(o);
        }

        /// <summary>
        /// Метод возвращает значение выражения, если this не null, иначе вернет значение failureValue.
        /// </summary>
        /// <example>
        /// string postCode = this.With(x => person).With(x => x.Address)
        ///     .Return(x => x.PostCode, string.Empty);
        /// </example>
        public static TResult Return<TInput, TResult>(this TInput o,
            Func<TInput, TResult> evaluator, TResult failureValue) where TInput : class
        {
            if (o == null) return failureValue;
            return evaluator(o);
        }

        /// <summary>
        /// "Прокидывает" объект дальше по цепочке, если выполняется выражение.
        /// </summary>
        public static TInput If<TInput>(this TInput o, Func<TInput, bool> evaluator)
            where TInput : class
        {
            if (o == null) return null;
            return evaluator(o) ? o : null;
        }

        /// <summary>
        /// "Прокидывает" объект дальше по цепочке, если выражение не выполняется.
        /// </summary>
        public static TInput Unless<TInput>(this TInput o, Func<TInput, bool> evaluator)
            where TInput : class
        {
            if (o == null) return null;
            return evaluator(o) ? null : o;
        }

        /// <summary>
        /// Метод выполняет делегат над объектом и "прокидывает" объект дальше по цепочке.
        /// </summary>
        public static TInput Do<TInput>(this TInput o, Action<TInput> action)
            where TInput : class
        {
            if (o == null) return null;
            action(o);
            return o;
        }

    }
}
