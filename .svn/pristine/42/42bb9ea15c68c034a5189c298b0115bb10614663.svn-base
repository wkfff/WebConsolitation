using System;
using Krista.FM.RIA.Extensions.DebtBook.Services.DAL;
using NCalc;

namespace Krista.FM.RIA.Extensions.DebtBook.Services.ControlRelationships
{
    public class ControlRelationship
    {
        private readonly string expression;
        private readonly string errorMessage;
        private bool isChecked;
        private bool isCorrect;
        
        public ControlRelationship(string expression, string errorMessage)
        {
            this.expression = expression;
            this.errorMessage = errorMessage;
            this.isChecked = false;
            this.isCorrect = false;
        }

        public bool IsCorrect
        {
            get { return this.isCorrect; }
        }
        
        public ControlRelationship Check(object record, IObjectRepository repository)
        {
            var checker = new Checker(record, repository);

            bool result = false;

            try
            {
                result = checker.Check(expression);
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("Ошибка во время вычисления контрольного соотношения: {0}", expression), e);
            }

            this.isChecked = true;
            this.isCorrect = result;
            return this;
        }

        public string GetResultMessage()
        {
            if (!this.isChecked)
            {
                throw new Exception("Контрольное соотношение не было вычислено!");
            }

            if (!IsCorrect)
            {
                return errorMessage;
            }

            return "Контрольное соотношение соблюдено.";
        }

        private class Checker
        {
            private readonly object record;
            private readonly IObjectRepository repository;

            public Checker(object record, IObjectRepository repository)
            {
                this.record = record;
                this.repository = repository;
            }

            public bool Check(string expression)
            {
                Expression expr = new Expression(expression);
                expr.EvaluateParameter += ParseParameters;
                expr.EvaluateFunction += ParseFunction;
                object resultObj = expr.Evaluate();
                bool result = Convert.ToBoolean(resultObj);
                return result;
            }

            private void ParseParameters(string name, ParameterArgs args)
            {
                args.Result = GetFieldValue(this.record, name) ?? 0;
                args.HasResult = true;
            }

            private void ParseFunction(string name, FunctionArgs args)
            {
                if (name.ToUpper() == "PREV")
                {
                    var param = args.Parameters[0].ParsedExpression.ToString().Trim().Replace("[", string.Empty).Replace("]", string.Empty);
                    var prevRow = repository.GetPrevious(this.record);
                    if (prevRow != null)
                    {
                        var prevParamValue = GetFieldValue(prevRow, param);
                        args.Result = prevParamValue ?? 0;
                        args.HasResult = true;
                    }
                    else
                    {
                        args.HasResult = true;
                        args.Result = 0;
                    }
                }
                else if (name.ToUpper() == "DATE")
                {
                    var result = args.Parameters[0].Evaluate();
                    if (result.ToString() == "0")
                    {
                        result = DateTime.MinValue;
                    }

                    args.Result = Convert.ToDateTime(result);
                    args.HasResult = true;
                }
                else if (name.ToUpper() == "MONTH")
                {
                    var result = args.Parameters[0].Evaluate();
                    args.Result = Convert.ToDateTime(result).Month;
                }
                else if (name.ToUpper() == "DAY")
                {
                    var result = args.Parameters[0].Evaluate();
                    args.Result = Convert.ToDateTime(result).Day;
                }
            }

            /// <summary>
            /// Находит значение атрибута по его имени через рефлексию
            /// </summary>
            private object GetFieldValue(object obj, string propertyName)
            {
                try
                {
                    var dotPosition = propertyName.IndexOf(".", StringComparison.Ordinal);
                    if (dotPosition < 0)
                    {
                        var property = obj.GetType().GetProperty(propertyName);
                        var val = property.GetValue(obj, null);
                        return val;
                    }
                    else
                    {
                        var basePropertyName = propertyName.Substring(0, dotPosition);
                        var property = obj.GetType().GetProperty(basePropertyName);
                        var baseObject = property.GetValue(obj, null);

                        if (baseObject == null)
                        {
                            return null;
                        }

                        var secondPropertyName = propertyName.Substring(dotPosition + 1);
                        return GetFieldValue(baseObject, secondPropertyName);
                    }
                }
                catch (Exception)
                {
                    throw new Exception(String.Format("Не найдено значение атрибута {0}", propertyName));
                }
            }
        }
    }
}
