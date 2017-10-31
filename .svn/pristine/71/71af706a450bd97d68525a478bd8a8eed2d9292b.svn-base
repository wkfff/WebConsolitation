using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Krista.FM.Extensions;
using Microsoft.CSharp;

namespace Krista.FM.RIA.Extensions.Forecast.Forma2p
{
    public struct EquationError
    {
        public int Group;
        public string Text;
        public object Data;
    }
    
    public static class Equation
    {
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
