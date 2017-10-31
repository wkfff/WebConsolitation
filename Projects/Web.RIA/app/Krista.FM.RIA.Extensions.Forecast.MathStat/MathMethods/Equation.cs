using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Krista.FM.Extensions;
using Microsoft.CSharp;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public struct EquationError
    {
        public int Group;
        public string Text;
        public object Data;
    }
    
    public static class Equation
    {
        public static double[] Calc(
                                    Dictionary<int, double[]> inData,
                                    Dictionary<int, double[]> inRegs, 
                                    Dictionary<string, double[]> calculatedDatas,
                                    ForecastStruct forecastStruct, 
                                    int fparamid,
                                    int group,
                                    int method,
                                    SortedList<int, bool> arrYear,
                                    int predCount,
                                    SortedList<string, double> coeff,
                                    MathGroups loadedMathGroups,
                                    out List<EquationError> errors)
        {
            errors = new List<EquationError>();

            int yearCount = inData[fparamid].Length;
            double[] pred = new double[yearCount + predCount];

            switch (group)
            {
                case FixedMathGroups.FirstOrderRegression:
                case FixedMathGroups.SecondOrderRegression:
                    if ((method == FirstOrderRegression.Optimal) || (method == SecondOrderRegression.Optimal))
                    {
                        bool[] weight = arrYear.Values.ToArray(); ///// new bool[data.Count()];

                        Regression reg = new Regression(inData[fparamid], weight, loadedMathGroups);
                        Method met = reg.GetOptimal(group);
                        ////reg.
                        reg.Predict(predCount, out pred);

                        coeff.Clear();
                        int j = 0;
                        foreach (double d in reg.OptimalH)
                        {
                            coeff.Add(met.Coeff[j], d);
                            j++;
                        }
                    }
                    else
                    {
                        bool[] weight = arrYear.Values.ToArray();

                        Regression reg = new Regression(inData[fparamid], weight, loadedMathGroups);
                        Method met = reg.GetConcrete(group, method).Value;
                        reg.Predict(predCount, out pred);

                        coeff.Clear();
                        int j = 0;
                        foreach (double d in reg.OptimalH)
                        {
                            coeff.Add(met.Coeff[j], d);
                            j++;
                        }
                    }

                    break;

                case FixedMathGroups.ARMAMethod:
                    RegressionWithARMA regARMA = null;
                    if ((method == ARMAwithRegression.ARMA22FirstOrder) ||
                        (method == ARMAwithRegression.ARMA22SecondOrder))
                    {
                        regARMA = new RegressionWithARMA(inData[fparamid], 2, 2, group, loadedMathGroups);
                    }

                    if ((method == ARMAwithRegression.ARMA33FirstOrder) ||
                        (method == ARMAwithRegression.ARMA33SecondOrder))
                    {
                        regARMA = new RegressionWithARMA(inData[fparamid], 3, 3, group, loadedMathGroups);
                    }

                    if ((method == ARMAwithRegression.ARMA23FirstOrder) ||
                        (method == ARMAwithRegression.ARMA23SecondOrder))
                    {
                        regARMA = new RegressionWithARMA(inData[fparamid], 2, 3, group, loadedMathGroups);
                    }

                    if ((method == ARMAwithRegression.ARMA32FirstOrder) ||
                        (method == ARMAwithRegression.ARMA32SecondOrder))
                    {
                        regARMA = new RegressionWithARMA(inData[fparamid], 3, 2, group, loadedMathGroups);
                    }

                    pred = regARMA.Predict(predCount);
                    break;

                case FixedMathGroups.MultiRegression:
                    bool[] weights = arrYear.Values.ToArray();

                    double[] data = new double[yearCount];
                    double[,] factors = new double[yearCount, inData.Count - 1 + inRegs.Count];

                    int k = 0;
                    foreach (KeyValuePair<int, double[]> pair in inData)
                    {
                        if (pair.Key == fparamid)
                        {
                            data = inData[pair.Key];
                        }
                        else
                        {
                            double[] row = inData[pair.Key];

                            for (int j = 0; j < row.Count(); j++)
                            {
                                factors[j, k] = row[j];       
                            }
                            
                            k++;
                        }
                    }

                    foreach (KeyValuePair<int, double[]> pair in inRegs)
                    {
                        double[] row = inData[pair.Key];

                        for (int j = 0; j < row.Count(); j++)
                        {
                            if (j < yearCount)
                            {
                                factors[j, k] = row[j];
                            }
                        }

                        k++;
                    }

                    MultiRegr mr = new MultiRegr(factors, data, weights);
                    mr.CalcRegression();

                    pred = mr.Predict(factors);

                    string[] coefNames = loadedMathGroups.GetGroupByCode(group).Value.Methods.GetMethodByCode(method).Value.Coeff;

                    for (int i = 0; i < mr.UsedInModel.Length; i++)
                    {
                        if (!mr.UsedInModel[i])
                        {
                            var err = new EquationError { Group = FixedMathGroups.MultiRegression, Text = "Парметр {0} включенный в модель множественной регрессии при текущих значениях не влияет на прогнозируемую величину. Его необходимо удалить из модели!", Data = inData.ElementAt(i).Key };

                            errors.Add(err);
                        }
                    }

                    coeff.Clear();
                    
                    int j2 = 0;
                    foreach (double d in mr.Coeff)
                    {
                        coeff.Add(coefNames[j2], d);
                        j2++;
                    }

                    break;

                case FixedMathGroups.PCAForecast:
                    /*bool[] indexes_exist = new bool[rowNum];
                    for (int ii = 0; ii < rowNum; ii++)
                    {
                        indexes_exist[ii] = true;
                    }

                    double[] expert_values = new double[] { 3, 5 };

                    PCAForecast a = new PCAForecast(allData, indexes_exist, expert_values);*/
                    break;
                case FixedMathGroups.ComplexEquation:
                    inData[fparamid].CopyTo(pred, 0);
                    
                    string expression = forecastStruct.Expression; ////"x+(A*x/1000)-(B*x/1000)+(C*x/10000)";
                    
                    for (int i = 1; i < yearCount + predCount; i++)
                    {
                        Dictionary<string, double> param = new Dictionary<string, double>();

                        foreach (KeyValuePair<string, double[]> pair in calculatedDatas)
                        {
                            param.Add(pair.Key, pair.Value[i - 1]);
                        }

                        foreach (ForecastParameter foreParam in forecastStruct.UsedParams)
                        {
                            if (i - 1 < inData[foreParam.ParamId].Length)
                            {
                                param.Add(foreParam.Name, inData[foreParam.ParamId][i - 1]);
                            }
                            else
                            {
                                param.Add(foreParam.Name, 0);
                            }
                        }

                        pred[i] = MathCalc(param, pred[i - 1], expression);
                        /*pred[i] = pred[i - 1] +
                            (calculatedDatas["A"][i - 1] * pred[i - 1] / 1000) -
                            (calculatedDatas["B"][i - 1] * pred[i - 1] / 1000) +
                            calculatedDatas["C"][i - 1];*/
                    }

                    break;
            }

            return pred;
        }

        public static double MathCalc(Dictionary<string, double> param, double x, string expression)
        {
            foreach (KeyValuePair<string, double> pair in param)
            {
                /*var value = pair.Value;
                var values = value.ToString();
                var valuesr = values.Replace(",", ".");*/
                expression = expression.Replace(pair.Key, "({0})".FormatWith(pair.Value.ToString("E").Replace(",", ".")));
            }

            expression = expression.Replace("x", "({0})".FormatWith(x.ToString("E").Replace(",", ".")));

            string source = String.Format(
@"using System;
namespace Krista.FM.RIA.Extensions.Forecast.Equation
{{
  public class MathEquation
  {{
    public static double Equation()
    {{
      return {0};
    }}
  }}
}}", 
                expression);

            Dictionary<string, string> providerOptions = new Dictionary<string, string> { { "CompilerVersion", "v3.5" } };
            CSharpCodeProvider provider = new CSharpCodeProvider(providerOptions);

            // Указываем GenerateInMemory
            CompilerParameters compilerParams = new CompilerParameters { GenerateInMemory = true, GenerateExecutable = false };

            CompilerResults results = provider.CompileAssemblyFromSource(compilerParams, source);

            // Assembly берем из результата
            Type type = results.CompiledAssembly.GetType("Krista.FM.RIA.Extensions.Forecast.Equation.MathEquation");
            MethodInfo method = type.GetMethod("Equation");
            object o = method.Invoke(null, null);

            return Convert.ToDouble(o);
        }

        public static double MathCalc(Dictionary<string, double> param, string expression)
        {
            foreach (KeyValuePair<string, double> pair in param)
            {
                expression = expression.Replace(pair.Key, "({0})".FormatWith(pair.Value.ToString("E").Replace(",", ".")));
            }
            
            ////expression = expression.Replace("x", x.ToString().Replace(",", "."));

            string source = String.Format(
@"using System;
namespace Krista.FM.RIA.Extensions.Forecast.Equation
{{
  public class MathEquation
  {{
    public static double Equation()
    {{
      return {0};
    }}
  }}
}}",
                expression);

            Dictionary<string, string> providerOptions = new Dictionary<string, string> { { "CompilerVersion", "v3.5" } };
            CSharpCodeProvider provider = new CSharpCodeProvider(providerOptions);

            // Указываем GenerateInMemory
            CompilerParameters compilerParams = new CompilerParameters { GenerateInMemory = true, GenerateExecutable = false };

            CompilerResults results = provider.CompileAssemblyFromSource(compilerParams, source);

            // Assembly берем из результата
            Type type = results.CompiledAssembly.GetType("Krista.FM.RIA.Extensions.Forecast.Equation.MathEquation");
            MethodInfo method = type.GetMethod("Equation");
            object o = method.Invoke(null, null);

            return Convert.ToDouble(o);
        }
    }
}
