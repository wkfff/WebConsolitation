using System;
using System.Globalization;
using System.Text;
using Krista.FM.Common.Consolidation.Calculations.Expressions;

namespace Krista.FM.Common.Consolidation.Calculations.Visitors
{
    /// <summary>
    /// Преобразует дерево в строковое представление.
    /// </summary>
    public class SerializationVisitor : ConsRelationVisitor
    {
        private readonly NumberFormatInfo numberFormatInfo;

        public SerializationVisitor()
        {
            Result = new StringBuilder();
            numberFormatInfo = new NumberFormatInfo { NumberDecimalSeparator = "." };
        }

        public StringBuilder Result { get; protected set; }

        public override void Visit(ConsRelationExpression expression)
        {
            throw new NotImplementedException("The method object SerializationVisitor.Visit(ConsRelationExpression expression) is not implemented.");
        }

        public override void Visit(VerifyRelation expression)
        {
            expression.Left.Accept(this);

            switch (expression.Type)
            {
                case BinaryExpressionTypes.Equal:
                    Result.Append(" = ");
                    break;
                case BinaryExpressionTypes.NotEqual:
                    Result.Append(" != ");
                    break;
                case BinaryExpressionTypes.Greater:
                    Result.Append(" > ");
                    break;
                case BinaryExpressionTypes.GreaterOrEqual:
                    Result.Append(" >= ");
                    break;
                case BinaryExpressionTypes.Lesser:
                    Result.Append(" < ");
                    break;
                case BinaryExpressionTypes.LesserOrEqual:
                    Result.Append(" <= ");
                    break;
            }

            expression.Right.Accept(this);
        }

        public override void Visit(CheckRelation expression)
        {
            expression.UndependRowSelector.Accept(this);
        }

        public override void Visit(AssignRelation expression)
        {
            expression.LValueSelector.Accept(this);
            Result.Append(" := ");
            expression.Expr.Accept(this);
        }

        public override void Visit(TotalRowGenRelation expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(ConsRowGenRelation expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(LeftValueSelector expression)
        {
            expression.UndependRowSelector.Accept(this);
            Result.Append(".ГРАФА[");
            Result.Append(String.Join(", ", expression.ColsSelector.ToArray()));
            Result.Append("]");
        }

        public override void Visit(UndependRowSelector expression)
        {
            Result.Append("СТРОКА");
            Result.Append("[");
            if (expression.UndependCond != null)
            {
                expression.UndependCond.Accept(this);
            }

            Result.Append("]");
        }

        public override void Visit(ConsSelector expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(RightValueSelector expression)
        {
            expression.DependSelector.Accept(this);

            if (expression.Expr != null)
            {
                expression.Expr.Accept(this);
            }
        }

        public override void Visit(DependSelector expression)
        {
            if (expression.LayerSelector != null)
            {
                expression.LayerSelector.Accept(this);
                Result.Append(".");
            }

            if (expression.FormSelector != null)
            {
                expression.FormSelector.Accept(this);
                Result.Append(".");
            }

            if (expression.SectionSelector != null)
            {
                expression.SectionSelector.Accept(this);
                Result.Append(".");
            }

            expression.DependRowSelector.Accept(this);
        }

        public override void Visit(DependRowSelector expression)
        {
            Result.Append("СТРОКА");
            Result.Append("[");
            expression.DependCond.Accept(this);
            Result.Append("]");
        }

        public override void Visit(Subject expression)
        {
            Result.Append("СУБЪЕКТ");

            if (expression.Refs != null)
            {
                foreach (var @ref in expression.Refs)
                {
                    Result.Append(".");
                    @ref.Accept(this);
                }
            }
        }

        public override void Visit(LayerSelector expression)
        {
            Result.Append("ПОДОТЧЕТНЫЙ")
                .Append("[");

            if (expression.LayerCond != null)
            {
                expression.LayerCond.Accept(this);
            }

            Result.Append("]");
        }

        public override void Visit(FormSelector expression)
        {
            Result.Append("ФОРМА")
                .Append("[")
                .Append("'")
                .Append(expression.Name)
                .Append("'")
                .Append("]");
        }

        public override void Visit(SectionSelector expression)
        {
            Result.Append("РАЗДЕЛ")
                .Append("[")
                .Append("'")
                .Append(expression.Name)
                .Append("'")
                .Append("]");
        }

        public override void Visit(ColsSelector expression)
        {
            Result.Append("ГРАФЫ[");

            for (int i = 0; i < expression.Columns.Count; i++)
            {
                Result.Append(expression.Columns[i]);
                if (i < expression.Columns.Count - 1)
                {
                    Result.Append(",");
                }
            }

            Result.Append("]");
        }

        public override void Visit(DependContextParamColumn expression)
        {
            Result
                .Append("$")
                .Append(".")
                .Append(expression.Column);
        }

        public override void Visit(DependParamColumn expression)
        {
            Result.Append(".")
                .Append(expression.Column);
        }

        public override void Visit(UndependParamColumn expression)
        {
            Result.Append(".")
                .Append(expression.Column);
        }

        public override void Visit(ParamColumn expression)
        {
            Result
                .Append("ГРАФА[")
                .Append(expression.Column)
                .Append("]");
        }

        public override void Visit(LogicFunction expression)
        {
            if (expression.Type == ExistFunctionTypes.IsNull)
            {
                Result.Append("ПУСТО");
            }
            else if (expression.Type == ExistFunctionTypes.IsNotNull)
            {
                Result.Append("НЕПУСТО");
            }

            Result.Append("(");

            for (int i = 0; i < expression.Parameters.Count; i++)
            {
                expression.Parameters[i].Accept(this);
                if (i < expression.Parameters.Count - 1)
                {
                    Result.Append(",");
                }
            }

            Result.Append(")");
        }

        public override void Visit(ExistFunction expression)
        {
            Result.Append("НЕПУСТО");
            Result.Append("(");

            expression.DependSelector.Accept(this);

            Result.Append(")");
        }

        public override void Visit(GroupFunction expression)
        {
            switch (expression.Type)
            {
                case GroupFunctionTypes.Sum:
                    Result.Append("СУММА");
                    break;
                case GroupFunctionTypes.Count:
                    Result.Append("СЧЕТ");
                    break;
                case GroupFunctionTypes.Min:
                    Result.Append("МИНИМУМ");
                    break;
                case GroupFunctionTypes.Max:
                    Result.Append("МАКСИМУМ");
                    break;
                case GroupFunctionTypes.Avg:
                    Result.Append("СРЕДНЕЕ");
                    break;
            }

            Result.Append("(");
            expression.RValueSelector.Accept(this);
            Result.Append(")");
        }

        public override void Visit(CalcFunction expression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(InStatement expression)
        {
            expression.UndependParam.Accept(this);

            Result.Append(" В ");
            Result.Append(" (");

            for (int i = 0; i < expression.Constants.Count; i++)
            {
                expression.Constants[i].Accept(this);
                if (i < expression.Constants.Count - 1)
                {
                    Result.Append(",");
                }
            }

            Result.Append(")");
        }

        public override void Visit(ClassProperty expression)
        {
            Result.Append(expression.Name);
        }

        public override void Visit(ClassMethod expression)
        {
            Result.Append(expression.Name);
            Result.Append("(");
            
            if (expression.Parameters != null)
            {
                for (int i = 0; i < expression.Parameters.Count; i++)
                {
                    expression.Parameters[i].Accept(this);
                    if (i < expression.Parameters.Count - 1)
                    {
                        Result.Append(",");
                    }
                }
            }

            Result.Append(")");
        }

        public override void Visit(BinaryExpression expression)
        {
            expression.Left.Accept(this);

            switch (expression.Type)
            {
                case BinaryExpressionTypes.And:
                    Result.Append(" И ");
                    break;

                case BinaryExpressionTypes.Or:
                    Result.Append(" ИЛИ ");
                    break;

                case BinaryExpressionTypes.Div:
                    Result.Append(" / ");
                    break;

                case BinaryExpressionTypes.Equal:
                    Result.Append(" = ");
                    break;

                case BinaryExpressionTypes.Greater:
                    Result.Append(" > ");
                    break;

                case BinaryExpressionTypes.GreaterOrEqual:
                    Result.Append(" >= ");
                    break;

                case BinaryExpressionTypes.Lesser:
                    Result.Append(" < ");
                    break;

                case BinaryExpressionTypes.LesserOrEqual:
                    Result.Append(" <= ");
                    break;

                case BinaryExpressionTypes.Minus:
                    Result.Append(" - ");
                    break;

                case BinaryExpressionTypes.Modulo:
                    Result.Append(" % ");
                    break;

                case BinaryExpressionTypes.NotEqual:
                    Result.Append(" != ");
                    break;

                case BinaryExpressionTypes.Plus:
                    Result.Append(" + ");
                    break;

                case BinaryExpressionTypes.Times:
                    Result.Append(" * ");
                    break;
            }

            expression.Right.Accept(this);
        }

        public override void Visit(UnaryExpression expression)
        {
            if (expression.Type == UnaryExpressionTypes.Negate)
            {
                Result.Append(" -");
            }
            else if (expression.Type == UnaryExpressionTypes.Not)
            {
                Result.Append(" НЕ ");
            }

            expression.Exp.Accept(this);
        }

        public override void Visit(ValueExpression expression)
        {
            switch (expression.Type)
            {
                case ValueTypes.Boolean:
                    Result.Append(expression.Value.ToString()).Append(" ");
                    break;

                case ValueTypes.DateTime:
                    Result.Append("#").Append(expression.Value.ToString()).Append("#").Append(" ");
                    break;

                case ValueTypes.Float:
                    Result.Append(decimal.Parse(expression.Value.ToString()).ToString(numberFormatInfo)).Append(" ");
                    break;

                case ValueTypes.Integer:
                    Result.Append(expression.Value.ToString()).Append(" ");
                    break;

                case ValueTypes.String:
                    Result.Append("'").Append(expression.Value.ToString()).Append("'").Append(" ");
                    break;
            }
        }
    }
}
