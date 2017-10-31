using System;
using Antlr.Runtime;
using Krista.FM.Common.Consolidation.Calculations.Expressions;
using Krista.FM.Common.Consolidation.Calculations.Parser;
using Krista.FM.Domain;
using Krista.FM.Extensions;

namespace Krista.FM.Common.Consolidation.Calculations
{
    [CLSCompliant(false)]
    public class Expression
    {
        public Expression(D_Form_Relations expression)
        {
            if (expression == null)
            {
                throw new ArgumentException("Expression не может быть null", "expression");
            }

            OriginalExpression = expression;
        }

        /// <summary>
        /// Текстовое представление вычисляемого выражения.
        /// </summary>
        protected D_Form_Relations OriginalExpression { get; set; }
    
        public static ConsRelationExpression Compile(D_Form_Relations relation)
        {
            string expression = relation.LeftPart + relation.RalationType + relation.RightPart;

            var lexer = new ConsRelationLexer(new ANTLRStringStream(expression.ToUpper()));
            var parser = new ConsRelationParser(new CommonTokenStream(lexer));
            var parserFunc = GetRelationParser(parser, relation);
            ConsRelationExpression consRelationExpression = parserFunc();

            if (parser.Errors != null && parser.Errors.Count > 0)
            {
                throw new EvaluationException(String.Join(Environment.NewLine, parser.Errors.ToArray()));
            }

            return consRelationExpression;
        }

        private static Func<ConsRelationExpression> GetRelationParser(ConsRelationParser parser, D_Form_Relations relation)
        {
            if (relation.ActivationType == 2)
            {
                if (relation.RalationType == "<-")
                {
                    return () => parser.consRowGenRelation().Value;
                }
                
                if (relation.RalationType == "^-")
                {
                    return () => parser.totalRowGenRelation().Value;
                }
            }
            
            if (relation.RalationType.IsNullOrEmpty())
            {
                return () => parser.checkRelation().Value;
            }
            
            if (relation.RalationType == ":=")
            {
                return () => parser.assignRelation().Value;
            }

            return () => parser.verifyRelation().Value;
        }
    }
}
