using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Common.Consolidation.Calculations.Expressions;
using Krista.FM.Common.Consolidation.Calculations.Helpers;
using Krista.FM.Common.Consolidation.Data;
using Krista.FM.Domain;
using Krista.FM.Extensions;

namespace Krista.FM.Common.Consolidation.Calculations.Visitors
{
    public class EvaluationVisitor : ConsRelationVisitor
    {
        /// <summary>
        /// Предоставляет доступ к данным адресуемым левой частью соотношения.
        /// </summary>
        private readonly IPrimaryDataProvider primaryDataProvider;

        /// <summary>
        /// Предоставляет доступ к данным адресуемым правой частью соотношения.
        /// </summary>
        private readonly IDataProvider dataProvider;

        private static decimal epsilon = 0.001m;

        private static Type[] commonTypes = new[] { typeof(string), typeof(decimal), typeof(int), typeof(long), typeof(bool) };

        /// <summary>
        /// Левая часть выражения выступающая в роли контекста для правой части выражения.
        /// </summary>
        private Context context;
        
        /// <summary>
        /// Локальный контекст для селектора строк.
        /// </summary>
        private Context localContext;

        public EvaluationVisitor()
        {
            context = new Context();
            localContext = new Context();
        }

        public EvaluationVisitor(IPrimaryDataProvider primaryDataProvider, IDataProvider dataProvider)
            : this()
        {
            this.primaryDataProvider = primaryDataProvider;
            this.dataProvider = dataProvider;
        }

        /// <summary>
        /// Результат вычисления выражения.
        /// </summary>
        public object Result { get; private set; }

        public void SetContext(string sectionCode, string formCode, IList<D_Form_TableColumn> metaColumns)
        {
            context.SectionName = sectionCode;
            context.FormName = formCode;
            context.MetaColumns = metaColumns;
        }

        public override void Visit(VerifyRelation expression)
        {
            expression.Left.Accept(this);

            if (Result is IList)
            {
            }
            else if (IsNumeric(Result) || Result is bool || Result is string)
            {
                new BinaryExpression(expression.Type, new ValueExpression(Result), expression.Right)
                    .Accept(this);
            }
            else
            {
                throw new EvaluationException("Неподходящий тип выражения левой части");
            }
        }

        public override void Visit(CheckRelation expression)
        {
            expression.UndependRowSelector.Accept(this);
            if (!(Result is IList))
            {
                throw new EvaluationException("Результатом вычисления UndependRowSelector должен быть список.");
            }
        }

        public override void Visit(AssignRelation expression)
        {
            expression.LValueSelector.Accept(this);

            var leftValueResult = (LeftValueSelectorResult)Result;
            foreach (var row in leftValueResult.Rows)
            {
                context.Row = row;
                context.Columns = leftValueResult.Columns;

                expression.Expr.Accept(this);

                // ReSharper disable HeuristicUnreachableCode
                if (Result is RightValueSelectorResult)
                {
                    // Строка
                    var rightSelectorResult = (RightValueSelectorResult)Result;
                    if (rightSelectorResult.Rows.Count > 1)
                    {
                        throw new EvaluationException("Правая часть выражения присваивания должна возвращать одну строку либо скалярное значение.");
                    }

                    if (rightSelectorResult.Rows.Count == 1)
                    {
                        // Если в правой части всего одна графа, то ее значение присваиваем 
                        // всем графам левой части
                        if (rightSelectorResult.Columns.Count == 1)
                        {
                            var value = rightSelectorResult.Rows[0].Get(rightSelectorResult.Columns[0]);
                            foreach (var column in leftValueResult.Columns)
                            {
                                row.Set(column, value);
                            }
                        }
                        else
                        {
                            int rightColumnIndex = 0;
                            var rightResultRow = rightSelectorResult.Rows[0];
                            foreach (var column in leftValueResult.Columns)
                            {
                                if (rightColumnIndex < rightSelectorResult.Columns.Count)
                                {
                                    var rightColumnName = rightSelectorResult.Columns[rightColumnIndex];
                                    var value = rightResultRow.Get(rightColumnName);
                                    row.Set(column, value);
                                }

                                rightColumnIndex++;
                            }
                        }
                    }
                }
                else
                {
                    // ReSharper restore HeuristicUnreachableCode
                    // Скалярное значение
                    foreach (var column in leftValueResult.Columns)
                    {
                        row.Set(column, Result);
                    }
                }
            }
        }

        public override void Visit(TotalRowGenRelation expression)
        {
            expression.LvalueSelector.Accept(this);

            var lvalue = (LeftValueSelectorResult)Result;
            
            // Селектор левой части должен выбирать только множимые строки, 
            // так что дополнительно отфильтруем их.
            List<IRecord> leftMultiplicityRows = lvalue.Rows.Where(row => row.IsMultiplicity()).ToList();

            HashSet<IRecord> groupedRows = new HashSet<IRecord>(
                leftMultiplicityRows, 
                new NaturalKeyRecordComparer(lvalue.Columns));

            // Получаем строку шаблон. На основе этого шаблона будем создавать остальные строки.
            IRecord template = null;
            IList<IRecord> templates = primaryDataProvider.GetMultipliesRowsTemplates();
            foreach (var record in templates)
            {
                localContext.Row = record;
                expression.LvalueSelector.UndependRowSelector.UndependCond.Accept(this);

                if (Result != null && Convert.ToBoolean(Result))
                {
                    template = record;
                    break;
                }
            }

            if (template == null)
            {
                throw new EvaluationException("Не найден подходяший шаблон множимой строки.");
            }

            // Вычисляем правую часть соотношения
            expression.ConsSelector.Accept(this);
            var rvalue = (RightValueSelectorResult)Result;

            // Строки правой части для вставки в левую чать
            List<IRecord> candidateRows = rvalue.Rows;

            foreach (var row in candidateRows)
            {
                // Создаем новую запись для дальнейшей обработки и возможной вставки
                IRecord newRow = primaryDataProvider.CreateRecord(template);

                // Заполняем новую запись значениями из строки правой части
                int columnIndex = 0;
                foreach (var column in lvalue.Columns)
                {
                    newRow.Set(column, row.Get(rvalue.Columns[columnIndex++]));
                }

                // Применяем к созданной записи условие фильтрации левой части
                localContext.Row = newRow;
                expression.LvalueSelector.UndependRowSelector.UndependCond.Accept(this);
                if (Result != null && Convert.ToBoolean(Result))
                {
                    if (groupedRows.Add(newRow))
                    {
                        primaryDataProvider.AppendRecord(newRow);
                    }
                }
            }
        }

        public override void Visit(ConsRowGenRelation expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(LeftValueSelector expression)
        {
            expression.UndependRowSelector.Accept(this);

            List<string> columns = new List<string>();
            foreach (var columnCode in expression.ColsSelector)
            {
                var column = context.MetaColumns.FirstOrDefault(x => x.Code.ToUpper() == columnCode);
                if (column == null)
                {
                    throw new EvaluationException(String.Format("Графа с именем {0} не найдена.", columnCode));
                }

                columns.Add(column.InternalName);
            }

            Result = new LeftValueSelectorResult
            {
                Rows = (List<IRecord>)Result,
                Columns = columns
            };
        }

        public override void Visit(UndependRowSelector expression)
        {
            IList<IRecord> rows = primaryDataProvider.GetSectionRows();
            List<IRecord> selectedRows = new List<IRecord>();

            // Возвращаем все записи, т.к. нет условия фильтрации
            if (expression.UndependCond == null)
            {
                Result = rows;
            }
            else
            {
                localContext.MetaColumns = context.MetaColumns;

                // Применяем условия фильтра для каждой записи
                foreach (var row in rows)
                {
                    localContext.Row = row;

                    expression.UndependCond.Accept(this);

                    if (Result != null && Convert.ToBoolean(Result))
                    {
                        selectedRows.Add(row);
                    }
                }

                Result = selectedRows;
            }
        }

        public override void Visit(ConsSelector expression)
        {
            expression.DependSelector.Accept(this);

            var result = (RightValueSelectorResult)Result;

            result.Columns = new List<string>();
            foreach (var valueExpr in expression.RValueExpressions)
            {
                valueExpr.Accept(this);

                result.Columns.Add(Convert.ToString(Result));
            }

            Result = result;
        }

        public override void Visit(RightValueSelector expression)
        {
            expression.DependSelector.Accept(this);

            var selectorResult = (RightValueSelectorResult)Result;

            if (expression.Expr != null)
            {
                // Здесь, результатом может быть или имя графы или список граф
                expression.Expr.Accept(this);

                if (Result is IList<string>)
                {
                    selectorResult.Columns = Result as List<string>;
                }
                else
                {
                    selectorResult.Columns = new List<string> { Convert.ToString(Result) };
                }
            }
            else
            {
                // Скорее всего здесь нужно взять перечень столбцов из контекста
                selectorResult.Columns = context.Columns;
            }

            Result = selectorResult;
        }

        public override void Visit(DependSelector expression)
        {
            if (expression.LayerSelector != null)
            {
                expression.LayerSelector.Accept(this);
            }
            
            if (expression.FormSelector != null)
            {
                expression.FormSelector.Accept(this);
            }

            if (expression.SectionSelector != null)
            {
                expression.SectionSelector.Accept(this);
            }

            expression.DependRowSelector.Accept(this);
        }

        public override void Visit(DependRowSelector expression)
        {
            dataProvider.SetContext(context.SectionName, context.FormName, context.IsSlave);
            IList<IRecord> rows = dataProvider.GetSectionRows();
            List<IRecord> selectedRows = new List<IRecord>();

            localContext.MetaColumns = dataProvider.GetMetaColumns();

            // Применяем условия фильтра для каждой записи
            foreach (var row in rows)
            {
                localContext.Row = row;

                expression.DependCond.Accept(this);

                if (Convert.ToBoolean(Result))
                {
                    selectedRows.Add(row);
                }
            }

            Result = new RightValueSelectorResult { Rows = selectedRows };
        }

        public override void Visit(Subject expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(LayerSelector expression)
        {
            context.IsSlave = true;
        }

        public override void Visit(FormSelector expression)
        {
            context.FormName = expression.Name;
        }

        public override void Visit(SectionSelector expression)
        {
            context.SectionName = expression.Name;
        }

        public override void Visit(ColsSelector expression)
        {
            List<string> columns = new List<string>();
            foreach (var columnName in expression.Columns)
            {
                var column = localContext.MetaColumns.FirstOrDefault(x => x.Code.ToUpper() == columnName);
                if (column == null)
                {
                    throw new EvaluationException(String.Format("Графа с именем {0} не найдена.", columnName));
                }

                columns.Add(column.InternalName);
            }

            Result = columns;
        }

        public override void Visit(DependContextParamColumn expression)
        {
            var column = context.MetaColumns.FirstOrDefault(x => x.Code.ToUpper() == expression.Column);
            if (column == null)
            {
                throw new EvaluationException(String.Format("Графа с именем {0} не найдена.", expression.Column));
            }

            Result = context.Row.Get(column.InternalName);
            if (Result is string)
            {
                Result = ((string)Result).ToUpper();
            }
        }

        public override void Visit(DependParamColumn expression)
        {
            var column = localContext.MetaColumns.FirstOrDefault(x => x.Code.ToUpper() == expression.Column);
            if (column == null)
            {
                throw new EvaluationException(String.Format("Графа с именем {0} не найдена.", expression.Column));
            }

            Result = localContext.Row.Get(column.InternalName);
            if (Result is string)
            {
                Result = ((string)Result).ToUpper();
            }
        }

        public override void Visit(UndependParamColumn expression)
        {
            var column = context.MetaColumns.FirstOrDefault(x => x.Code.ToUpper() == expression.Column);
            if (column == null)
            {
                throw new EvaluationException(String.Format("Графа с именем {0} не найдена.", expression.Column));
            }

            Result = localContext.Row.Get(column.InternalName);
            if (Result is string)
            {
                Result = ((string)Result).ToUpper();
            }
        }

        public override void Visit(ParamColumn expression)
        {
            if (localContext.MetaColumns == null)
            {
                Result = expression.Column;
                return;
            }

            var column = localContext.MetaColumns.FirstOrDefault(x => x.Code.ToUpper() == expression.Column);
            if (column == null)
            {
                throw new EvaluationException(String.Format("Графа с именем {0} не найдена.", expression.Column));
            }

            Result = column.InternalName;
        }

        public override void Visit(LogicFunction expression)
        {
            foreach (var parameter in expression.Parameters)
            {
                parameter.Accept(this);

                switch (expression.Type)
                {
                    case ExistFunctionTypes.IsNotNull:
                        if (Result == null)
                        {
                            Result = false;
                            return;
                        }

                        break;
                    case ExistFunctionTypes.IsNull:
                        if (Result != null)
                        {
                            Result = false;
                            return;
                        }

                        break;
                }
            }

            Result = true;
        }

        public override void Visit(ExistFunction expression)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Результатов вычисления групповой функции может быть либо скаляр либо структура RightValueSelectorResult.
        /// Для групповой функции СЧЕТ (Count) результат всегда скаляр.
        /// </summary>
        public override void Visit(GroupFunction expression)
        {
            expression.RValueSelector.Accept(this);

            var selectorResult = (RightValueSelectorResult)Result;

            if (expression.Type == GroupFunctionTypes.Count)
            {
                Result = selectorResult.Rows.Count;
            }
            else
            {
                var groupRecord = GroupFunc(selectorResult.Rows, selectorResult.Columns, expression.Type);
                Result = new GroupFuncResult
                {
                    Rows = new List<IRecord> { groupRecord }, 
                    Columns = selectorResult.Columns
                };
            }
        }

        public override void Visit(CalcFunction expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(InStatement expression)
        {
            expression.UndependParam.Accept(this);

            var parameter = Result;

            foreach (var constant in expression.Constants)
            {
                constant.Accept(this);

                if (CompareUsingMostPreciseType(parameter, Result) == 0)
                {
                    Result = true;
                    return;
                }
            }

            Result = false;
        }

        public override void Visit(ClassProperty expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(ClassMethod expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(BinaryExpression expression)
        {
            // Если один из операндов является групповой функцией
            if (expression.Left is GroupFunction || expression.Right is GroupFunction)
            {
                BinaryGroupsVisitor(expression.Type, expression.Left, expression.Right);
                return;
            }

            object leftValue = null;
            Func<object> left = () =>
            {
                if (leftValue == null)
                {
                    expression.Left.Accept(this);
                    if (Result is RightValueSelectorResult)
                    {
                        var rvsr = (RightValueSelectorResult)Result;
                        if (rvsr.Rows.Count == 0)
                        {
                            leftValue = null;
                        }
                        else
                        {
                            leftValue = rvsr.Rows[0].Get(rvsr.Columns[0]);
                        }
                    }
                    else if (expression.Left.GetType() == typeof(ParamColumn) && Result is string)
                    {
                        // в правой части не указан селектор строк, поэтому значение графы
                        // берем из левой части
                        leftValue = context.Row.Get(Convert.ToString(Result));
                    }
                    else
                    {
                        leftValue = Result;
                    }
                }

                return leftValue;
            };

            object rightValue = null;
            Func<object> right = () =>
            {
                if (rightValue == null)
                {
                    expression.Right.Accept(this);
                    if (Result is RightValueSelectorResult)
                    {
                        var rvsr = (RightValueSelectorResult)Result;
                        if (rvsr.Rows.Count == 0)
                        {
                            rightValue = null;
                        }
                        else
                        {
                            rightValue = rvsr.Rows[0].Get(rvsr.Columns[0]);
                        }
                    }
                    else if (expression.Right.GetType() == typeof(ParamColumn) && Result is string)
                    {
                        // в правой части не указан селектор строк, поэтому значение графы
                        // берем из левой части
                        rightValue = context.Row.Get(Convert.ToString(Result));
                    }
                    else
                    {
                        rightValue = Result;
                    }
                }

                return rightValue;
            };

            switch (expression.Type)
            {
                case BinaryExpressionTypes.And:
                    Result = Convert.ToBoolean(left()) && Convert.ToBoolean(right());
                    break;

                case BinaryExpressionTypes.Or:
                    Result = Convert.ToBoolean(left()) || Convert.ToBoolean(right());
                    break;

                case BinaryExpressionTypes.Equal:
                    Result = CompareUsingMostPreciseType(left(), right()) == 0;
                    break;

                case BinaryExpressionTypes.NotEqual:
                    var value = CompareUsingMostPreciseType(left(), right());
                    Result = value != null ? value != 0 : (bool?)null;
                    break;

                case BinaryExpressionTypes.Greater:
                    Result = CompareUsingMostPreciseType(left(), right()) > 0;
                    break;

                case BinaryExpressionTypes.GreaterOrEqual:
                    Result = CompareUsingMostPreciseType(left(), right()) >= 0;
                    break;

                case BinaryExpressionTypes.Lesser:
                    Result = CompareUsingMostPreciseType(left(), right()) < 0;
                    break;

                case BinaryExpressionTypes.LesserOrEqual:
                    Result = CompareUsingMostPreciseType(left(), right()) <= 0;
                    break;

                case BinaryExpressionTypes.Minus:
                    Result = Numbers.Subtract(left(), right());
                    break;

                case BinaryExpressionTypes.Plus:
                    if (left() is string)
                    {
                        Result = String.Concat(left(), right());
                    }
                    else
                    {
                        Result = Numbers.Add(left(), right());
                    }

                    break;

                case BinaryExpressionTypes.Div:
                    if (left() == null || right() == null)
                    {
                        Result = null;
                    }
                    else
                    {
                        Result = IsReal(left()) || IsReal(right())
                                ? Numbers.Divide(left(), right())
                                : Numbers.Divide(Convert.ToDecimal(left()), right());
                    }

                    break;

                case BinaryExpressionTypes.Modulo:
                    Result = Numbers.Modulo(left(), right());
                    break;

                case BinaryExpressionTypes.Times:
                    Result = Numbers.Multiply(left(), right());
                    break;
            }
        }

        public virtual void BinaryGroupsVisitor(BinaryExpressionTypes type, ConsRelationExpression left, ConsRelationExpression right)
        {
            var leftValues = GetGroupFuncResult(left);
            var rightValues = GetGroupFuncResult(right);

            if (!(type == BinaryExpressionTypes.Plus || type == BinaryExpressionTypes.Times ||
                  type == BinaryExpressionTypes.Minus || type == BinaryExpressionTypes.Div ||
                  type == BinaryExpressionTypes.Modulo))
            {
                var visitor = new SerializationVisitor();
                visitor.Visit(new BinaryExpression(type, left, right));
                throw new EvaluationException(
                    "Над результатами групповых функций можно применять только следующие операторы: +, -, *, / и %. Выражение: " +
                    visitor.Result);
            }

            // Если у правой части больше колонок, то для операций + и * меняем местами правую и левую части операции
            // т.е. выражения [X] + [Y, Z] заменяем на [Y, Z] + [X], в итоге получится [Y + X, Z + X]
            if (rightValues.Columns.Count > leftValues.Columns.Count &&
                (type == BinaryExpressionTypes.Plus || type == BinaryExpressionTypes.Times))
            {
                var temp = rightValues;
                rightValues = leftValues;
                leftValues = temp;
            }

            var result = new GroupFuncResult
            {
                Rows = new List<IRecord> { new Record() },
                Columns = new List<string>()
            };

            // Выполняем арифметические операции над векторами.
            // Здесь важна последовательность колонок
            var maxColumnCount = Math.Max(leftValues.Columns.Count, rightValues.Columns.Count);
            for (int leftColIndex = 0; leftColIndex < maxColumnCount; leftColIndex++)
            {
                if (leftColIndex < leftValues.Columns.Count)
                {
                    object leftValue = leftValues.Rows[0].Get(leftValues.Columns[leftColIndex]);

                    object rightValue = null;
                    if (leftColIndex < rightValues.Columns.Count)
                    {
                        rightValue = rightValues.Rows[0].Get(rightValues.Columns[leftColIndex]);
                    }
                    else
                    {
                        switch (type)
                        {
                            case BinaryExpressionTypes.Plus:
                            case BinaryExpressionTypes.Minus:
                                rightValue = 0;
                                break;
                            case BinaryExpressionTypes.Times:
                            case BinaryExpressionTypes.Div:
                            case BinaryExpressionTypes.Modulo:
                                rightValue = 1;
                                break;
                        }
                    }

                    new BinaryExpression(type, new ValueExpression(leftValue), new ValueExpression(rightValue))
                        .Accept(this);

                    result.Columns.Add(Convert.ToString(leftColIndex));
                    result.Rows[0].Set(Convert.ToString(leftColIndex), Result);
                }
                else
                {
                    object rightValue = rightValues.Rows[0].Get(rightValues.Columns[leftColIndex]);
                    result.Columns.Add(Convert.ToString(leftColIndex));
                    result.Rows[0].Set(Convert.ToString(leftColIndex), rightValue);
                }
            }

            Result = result;
        }

        public override void Visit(UnaryExpression expression)
        {
            expression.Exp.Accept(this);

            if (Result == null)
            {
                Result = null;
            }
            else
            {
                switch (expression.Type)
                {
                    case UnaryExpressionTypes.Not:
                        Result = !Convert.ToBoolean(Result);
                        break;

                    case UnaryExpressionTypes.Negate:
                        Result = Numbers.Subtract(0, Result);
                        break;
                }
            }
        }

        public override void Visit(ValueExpression expression)
        {
            Result = expression.Value;
        }

        public override void Visit(ConsRelationExpression expression)
        {
            throw new NotImplementedException("Метод object EvaluationVisitor.Visit(ConsRelationExpression expression) не реализован.");
        }

        /// <summary>
        /// Получает наиболее точный тип.
        /// </summary>
        private static Type GetMostPreciseType(Type a, Type b)
        {
            foreach (Type t in commonTypes)
            {
                if (a == t || b == t)
                {
                    return t;
                }
            }

            return a;
        }

        private static int? CompareUsingMostPreciseType(object a, object b)
        {
            if (a == null || b == null)
            {
                return null;
            }

            Type mpt = GetMostPreciseType(a.GetType(), b.GetType());
            if (Type.GetTypeCode(mpt) == TypeCode.Decimal)
            {
                return CompareDecimal(Convert.ToDecimal(a) - Convert.ToDecimal(b));
            }

            return Comparer.Default.Compare(Convert.ChangeType(a, mpt), Convert.ChangeType(b, mpt));
        }

        private static int CompareDecimal(decimal eps)
        {
            if (Math.Abs(eps) < epsilon)
            {
                return 0;
            }

            if (eps > 0)
            {
                return 1;
            }

            return -1;
        }

        private static bool IsReal(object value)
        {
            if (value == null)
            {
                return false;
            }

            var typeCode = Type.GetTypeCode(value.GetType());

            return typeCode == TypeCode.Decimal || typeCode == TypeCode.Double || typeCode == TypeCode.Single;
        }

        private static bool IsNumeric(object s)
        {
            double result;
            return double.TryParse(Convert.ToString(s), out result);
        }

        private static decimal GroupMax(IEnumerable<IRecord> rows, string column)
        {
            return rows.Select(record => record.GetDecimal(column)).Concat(new[] { Decimal.MinValue }).Max();
        }

        private static decimal GroupMin(IEnumerable<IRecord> rows, string column)
        {
            return rows.Select(record => record.GetDecimal(column)).Concat(new[] { Decimal.MaxValue }).Min();
        }

        private static decimal GroupSumm(IEnumerable<IRecord> rows, string column)
        {
            return rows.Sum(record => record.GetDecimal(column));
        }

        /// <summary>
        /// Вычисляет выражение и пытается результат вычисления преобразовать в тип GroupFuncResult.
        /// </summary>
        /// <param name="expr">Вычисляемое выражение.</param>
        /// <exception cref="EvaluationException">
        /// Выдает исключение, если нет возможности преобразовать результат к типу GroupFuncResult.
        /// </exception>
        /// <returns>
        /// Вычисленное значение преобразованное в тип GroupFuncResult.
        /// </returns>
        private GroupFuncResult GetGroupFuncResult(ConsRelationExpression expr)
        {
            expr.Accept(this);

            var groupFuncResult = Result as GroupFuncResult;
            if (groupFuncResult != null)
            {
                return groupFuncResult;
            }

            // Преобразуем результат в GroupFuncResult
            if (expr.GetType() == typeof(ParamColumn) && Result is string)
            {
                return new GroupFuncResult
                {
                    Columns = new List<string> { ((ParamColumn)expr).Column },
                    Rows = new List<IRecord> { context.Row }
                };
            }

            throw new EvaluationException(
                "Преобразование выражения {0} в тип GroupFuncResult не реализовано.".FormatWith(expr.GetType().Name));
        }

        private IRecord GroupFunc(List<IRecord> rows, List<string> columns, GroupFunctionTypes type)
        {
            Dictionary<string, decimal> columnSums = new Dictionary<string, decimal>();
            foreach (var column in columns)
            {
                columnSums.Add(column, 0);
            }

            // TODO: Определьть тип колонки, пока все групповые функции будут иметь тип результата decimal
            foreach (var column in columns)
            {
                decimal result = 0;
                switch (type)
                {
                    case GroupFunctionTypes.Sum:
                        result = GroupSumm(rows, column);
                        break;
                    case GroupFunctionTypes.Avg:
                        result = GroupSumm(rows, column);
                        result = result / rows.Count;
                        break;
                    case GroupFunctionTypes.Max:
                        result = GroupMax(rows, column);
                        break;
                    case GroupFunctionTypes.Min:
                        result = GroupMin(rows, column);
                        break;
                }

                columnSums[column] = result;
            }

            IRecord groupRecord = new Record();
            foreach (var column in columns)
            {
                groupRecord.Set(column, columnSums[column]);
            }

            return groupRecord;
        }

        private class Context
        {
            /// <summary>
            /// Подотчетный уровень.
            /// </summary>
            public bool IsSlave { get; set; }

            public string FormName { get; set; }

            public string SectionName { get; set; }

            public IList<D_Form_TableColumn> MetaColumns { get; set; }

            public IRecord Row { get; set; }

            public List<string> Columns { get; set; }
        }

        private abstract class ExpressionResult
        {
        }

        private abstract class SelectorResult : ExpressionResult
        {
            public List<IRecord> Rows { get; set; }

            public List<string> Columns { get; set; }
        }

        private class LeftValueSelectorResult : SelectorResult
        {
        }

        private class RightValueSelectorResult : SelectorResult
        {
        }

        private class GroupFuncResult : RightValueSelectorResult
        {
        }

        private class Record : IRecord
        {
            private Dictionary<string, object> values = new Dictionary<string, object>();

            public int MetaRowId
            {
                get { throw new NotImplementedException(); }
            }

            public ReportDataRecordState State
            {
                get { throw new NotImplementedException(); }
            }

            public object Value
            {
                get { throw new NotImplementedException(); }
            }

            public bool IsMultiplicity()
            {
                throw new NotImplementedException();
            }

            public object Get(string column)
            {
                return values[column];
            }

            public void Set(string column, object value)
            {
                if (!values.ContainsKey(column))
                {
                    values.Add(column, value);
                }
                else
                {
                    values[column] = value;
                }
            }

            public void AssignRecord(object assignRecord)
            {
                throw new NotImplementedException();
            }

            public void Delete()
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Позволяет сравнивать записи на равенство по натуральному ключу.
        /// </summary>
        private class NaturalKeyRecordComparer : IEqualityComparer<IRecord>
        {
            private readonly List<string> naturalKey;

            public NaturalKeyRecordComparer(List<string> naturalKey)
            {
                this.naturalKey = naturalKey;
            }

            /// <summary>
            /// Определяет, равны ли два указанных объекта.
            /// </summary>
            /// <param name="x">Первый сравниваемый объект.</param>
            /// <param name="y">Второй сравниваемый объект.</param>
            /// <returns> Значение true, если указанные объекты равны; в противном случае — значение false. </returns>
            public bool Equals(IRecord x, IRecord y)
            {
                if (x == null || y == null)
                {
                    return true;
                }

                return GetHashCode(x) == GetHashCode(y);
            }

            /// <summary>
            /// Возвращает хэш-код указанного объекта.
            /// </summary>
            /// <param name="obj">Объект <see cref="T:System.Object"/>, для которого должен быть возвращен хэш-код.</param><exception cref="T:System.ArgumentNullException">Тип <paramref name="obj"/> является ссылочным типом, значением <paramref name="obj"/> является null.</exception>
            /// <returns> Хэш-код указанного объекта. </returns>
            public int GetHashCode(IRecord obj)
            {
                unchecked
                {
                    int hashCode = 1;
                    foreach (var key in naturalKey)
                    {
                        var value = obj.Get(key);
                        hashCode = (hashCode * 397) ^ (value != null ? value.GetHashCode() : 0);
                    }
                    
                    return hashCode;
                }
            }
        }
    }
}
