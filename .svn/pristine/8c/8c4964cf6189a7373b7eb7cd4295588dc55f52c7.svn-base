using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Krista.FM.Common.Calculator.Exceptions;
using Krista.FM.Common.Calculator.Utils;

namespace Krista.FM.Common.Calculator.Implementation
{
    public class Preprocessor : IPreprocessor
    {
        public Preprocessor()
        {
            RequiredConstants = new Collection<IConstant>();
        }

        #region IPreprocessor
       
        public IFormulaRepository FormulaRepository { get; set; }

        public IList<IFormula> CalculationSequence { get; private set; }

        public IList<IConstant> RequiredConstants
        {
            get; private set;
        }

        public void AnalizeFormulas()
        {
            if (FormulaRepository == null)
            {
                throw new InvalidConfigurationException("Должен быть указан источник формул");
            }

            CalculationSequence = new List<IFormula>();

            var formulas = FormulaRepository.FindAll().ToList();

            FindDependencies(formulas);
            BuildCalculationSequence(formulas, CalculationSequence, 0);
        }

        public ICollection<IConstant> GetRequiredConstants(IList<IFormula> formulas)
        {
            var result = new Collection<IConstant>();

            foreach (var formula in formulas)
            {
                CollectDependsOnConstantsDeep(formula, result, 0);
            }

            return result;
        }

        public IList<IFormula> GetDependentSequence(IConstant constant)
        {
            var result = new List<IFormula>();

            foreach (var formula in CalculationSequence)
            {
                if (formula.DependOnConstants.Contains(constant))
                {
                    result.Union(GetDependentFormulasDeep(formula));
                }
            }

            return SortForCalculation(result);
        }

        public IList<IFormula> GetDependentSequence(IFormula formula)
        {
            return SortForCalculation(GetDependentFormulasDeep(formula));
        }

        public IList<IFormula> GetDependentSequence(IList<IFormula> formulas)
        {
            var result = new List<IFormula>();

            foreach (var formula in formulas)
            {
                result.Union(GetDependentFormulasDeep(formula));
            }

            return SortForCalculation(result);
        }

        public IList<IFormula> GetDependOnSequence(IFormula formula)
        {
            return SortForCalculation(GetDependOnFormulasDeep(formula));
        }

        public IList<IFormula> GetRecalculateSequence(IFormula formula)
        {
            // Определим, какие формулы потребуются для указанной
            var dependOnFormulas = GetDependOnFormulasDeep(formula);

            // Определим, на какие формулы может повлиять пересчет всех требуемых формул
            var result = new Collection<IFormula>();
            foreach (var dependOnFormula in dependOnFormulas)
            {
                result.Union(GetDependentFormulasDeep(dependOnFormula));
            }

            return SortForCalculation(result);
        }

        public IList<IFormula> GetRecalculateSequence(IEnumerable<IFormula> formulas)
        {
            var result = new Collection<IFormula>();
            foreach (var formula in formulas)
            {
                result.Union(GetRecalculateSequence(formula)); /* TODO: Многократный вызов SortForCalculation - устранить */
            }
            return SortForCalculation(result);
        }

        #endregion

        private static IEnumerable<string> ParseIdentifiers(string expression)
        {
            return ArgumentsParser.Parse(expression);
        }

        private void FindDependencies(ICollection<IFormula> formulas)
        {
            foreach (var formula in formulas)
            {
                if (string.IsNullOrEmpty(formula.Expression))
                {
                    throw new InvalidConfigurationException(string.Format("Для формулы '{0}' не задано выражение рассчета", formula.Name));
                }

                IList<string> identifiers;
                try
                {
                    identifiers = ParseIdentifiers(formula.Expression).ToList();
                }
                catch (Exception e)
                {
                    throw new InvalidConfigurationException(String.Format("Ошибка синтаксического разбора формулы {0} = {1}: {2}", formula.Name, formula.Expression, e.Message));
                }

                foreach (var identifier in identifiers)
                {
                    var usedFormula = formulas.FirstOrDefault(f => f.Name == identifier);

                    if (usedFormula != null)
                    {
                        if (!formula.DependOnFormulas.Contains(usedFormula))
                        {
                            formula.DependOnFormulas.Add(usedFormula);
                        }

                        if (!usedFormula.DependentFormulas.Contains(formula))
                        {
                            usedFormula.DependentFormulas.Add(formula);
                        }

                        continue;
                    }

                    var constant = RequiredConstants.FirstOrDefault(c => c.Name == identifier);
                    if (constant == null)
                    {
                        constant = new Constant(identifier);
                        RequiredConstants.Add(constant);
                    }

                    if (!formula.DependOnConstants.Contains(constant))
                    {
                        formula.DependOnConstants.Add(constant);
                    }

                    if (!constant.DependentFormulas.Contains(formula))
                    {
                        constant.DependentFormulas.Add(formula);
                    }
                }
            }
        }

        private static void BuildCalculationSequence(IEnumerable<IFormula> todoNodes, IList<IFormula> outputBuffer, int level)
        {
            if (level > CalculatorExtensions.MaxDependencyDepth)
            {
                throw new CircularReferenceException();
            }

            var todoNodesNext = new Collection<IFormula>();

            foreach (var node in todoNodes)
            {
                if (!outputBuffer.Contains(node))
                {
                    outputBuffer.Insert(0, node);
                }

                foreach (var usedNode in node.DependOnFormulas)
                {
                    if (outputBuffer.Contains(usedNode))
                    {
                        outputBuffer.Remove(usedNode);
                    }

                    if (!todoNodesNext.Contains(usedNode))
                    {
                        todoNodesNext.Add(usedNode);
                    }
                }
            }

            if (todoNodesNext.Count > 0)
            {
                BuildCalculationSequence(todoNodesNext, outputBuffer, level + 1);
            }
        }

        private static void CollectDependsOnConstantsDeep(IFormula root, ICollection<IConstant> outputBuffer, int level)
        {
            if (level > CalculatorExtensions.MaxDependencyDepth)
            {
                throw new CircularReferenceException();
            }

            foreach (var dependOnConstant in root.DependOnConstants)
            {
                if (!outputBuffer.Contains(dependOnConstant))
                {
                    outputBuffer.Add(dependOnConstant);
                }
            }

            foreach (var dependOnFormula in root.DependOnFormulas)
            {
                CollectDependsOnConstantsDeep(dependOnFormula, outputBuffer, level + 1);
            }
        }

        private static ICollection<IFormula> GetDependentFormulasDeep(IFormula root)
        {
            var dependentFormulas = new List<IFormula>();
            CollectDeepDependentFormulas(root, dependentFormulas, 0);
            return dependentFormulas;
        }

        private static void CollectDeepDependentFormulas(IFormula root, ICollection<IFormula> outputBuffer, int level)
        {
            if (level > CalculatorExtensions.MaxDependencyDepth)
            {
                throw new CircularReferenceException();
            }

            if (!outputBuffer.Contains(root))
            {
                outputBuffer.Add(root);
            }

            foreach (var dependentFormula in root.DependentFormulas)
            {
                CollectDeepDependentFormulas(dependentFormula, outputBuffer, level + 1);
            }
        }

        private static ICollection<IFormula> GetDependOnFormulasDeep(IFormula root)
        {
            var dependOnFormulas = new List<IFormula>();
            CollectDeepDependOnFormulas(root, dependOnFormulas, 0);
            return dependOnFormulas;
        }

        private static void CollectDeepDependOnFormulas(IFormula root, ICollection<IFormula> outputBuffer, int level)
        {
            if (level > CalculatorExtensions.MaxDependencyDepth)
            {
                throw new CircularReferenceException();
            }

            if (!outputBuffer.Contains(root))
            {
                outputBuffer.Add(root);
            }

            foreach (var dependOnFormula in root.DependOnFormulas)
            {
                CollectDeepDependOnFormulas(dependOnFormula, outputBuffer, level + 1);
            }
        }

        private IList<IFormula> SortForCalculation(ICollection<IFormula> sourceFormulas)
        {
            var result = new List<IFormula>();

            /* Просматриваем полную последовательнрость расчета по порядку и если среди переданных формул
             * есть текущая формула из полной последовательности, то вносим эту формулу в выходной буфер.
             * Т.о. по окончании в буфере будут все формулы из исходной последовательности, 
             * расставленные в порядке полной последовательности рассчета */
            foreach (var formula in CalculationSequence)
            {
                if (sourceFormulas.Contains(formula))
                {
                    result.Add(formula);
                }
            }

            return result;
        }
    }
}
