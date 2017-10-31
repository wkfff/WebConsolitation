using System;
using NCalc;

namespace Krista.FM.Common.Calculator.Implementation
{
    public class CalculatorParallel : CalculatorAbstract
    {
        public IValueStoreGroup ValueStoreGroup { get; set; }

        #region ICalculator

        protected override void EvaluateExpression(IFormula formula)
        {
            foreach (var storeId in ValueStoreGroup.GetStoresIndex())
            {
                ValueStoreGroup.CurrentStore = storeId;
                object result;
                try
                {
                    result = formula.ParsedExpression.Evaluate();
                }
                catch (Exception e)
                {
                    Trace.TraceError("Ошибка вычисления выражения {0} = {1}: {2}", formula.Name, formula.Expression, e.Message);
                    result = null;
                }
                
                if (ValueStoreGroup.CurrentStore != storeId)
                {
                    throw new InvalidOperationException(string.Format("В процессе рассчета в групповом хранилище изменилось текущее хранилище с '{0}' на '{1}'", storeId, ValueStoreGroup.CurrentStore));
                }

                ValueStoreGroup.Get(formula.Name).Value = result;
            }
        }

        #endregion

        protected override void OnEvaluateParameter(string name, ParameterArgs args)
        {
            //// args.Result = ValueStoreGroup.Get(name).Value; // TODO: Так все результаты null оказываются - разобраться
            var value = ValueStoreGroup.Get(name).Value;
            if (value == null)
            {
                args.Result = null;
            }
            else
            {
                args.Result = Convert.ToDouble(ValueStoreGroup.Get(name).Value);
            }
        }
    }
}
