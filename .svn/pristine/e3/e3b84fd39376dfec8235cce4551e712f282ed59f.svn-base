using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public struct Method 
    {
        public string CodeName;
        public int Code;
        public string TextName;
        public string Description;
        public byte[] FormulaImg;
        public int NOfPowers;
        public string[] Coeff;
        public int MaxParamCount;
        public string XMLString;
    }

    internal struct FirstOrderRegression
    {
        public const int MaxCode = 7;

        public const int Optimal = 99;
        public const int ExpReg = 1;
        public const int PowReg = 2;
        public const int LogReg = 3;
        public const int LogistReg = 4;
        public const int PolyReg = 5;
        public const int PolyExpReg = 6;
        public const int PolyPowReg = 7;
    }

    internal struct SecondOrderRegression
    {
        public const int MaxCode = 4;

        public const int Optimal = 99;
        public const int PolyExpReg = 1;
        public const int PolyPowReg = 2;
        public const int PolyLogReg = 3;
        public const int PolyLogistReg = 4;
    }

    internal struct ARMAwithRegression
    {
        public const int ARMA22FirstOrder = 1;
        public const int ARMA22SecondOrder = 2;
        public const int ARMA33FirstOrder = 3;
        public const int ARMA33SecondOrder = 4;
        public const int ARMA23FirstOrder = 5;
        public const int ARMA23SecondOrder = 6;
        public const int ARMA32FirstOrder = 7;
        public const int ARMA32SecondOrder = 8;
    }

    internal struct ComplexEquation
    {
        public const int EmbeddedEquation = 3;
        public const int NowEquation = 1;
        public const int LagEquation = 2;

        public const int PopulationEquation = 10;
    }

    internal struct MultiRegression
    {
        public const int SimpleRegression = 1;
    }

    public class MathMethods : IEnumerable<Method>
    {
        private List<Method> lstOfMethods = new List<Method>();
        
        public void AddMethod(string codeName, int code, string textName, string description, byte[] formulaImg, string coeffString, int? maxParamCount, string xmlString)
        {
            string[] coeffs = new string[] { };
            if (coeffString != null)
            {
                coeffs = coeffString.Replace(" ", String.Empty).Replace("\"", String.Empty).Split(',');
            }

            Method method = new Method
            {
                CodeName = codeName,
                Code = code,
                TextName = textName,
                Description = description,
                FormulaImg = formulaImg,
                NOfPowers = coeffs.Count(),
                Coeff = coeffs,
                MaxParamCount = maxParamCount.HasValue ? maxParamCount.Value : 100,
                XMLString = xmlString
            };

            lstOfMethods.Add(method);
        }

        public void ClearAll()
        {
            lstOfMethods.Clear();
        }

        public Method? GetMethodByCode(int code)
        {
            foreach (Method method in lstOfMethods)
            {
                if (method.Code == code)
                {
                    return method;
                }
            }

            return null;
        }

        public IEnumerator<Method> GetEnumerator()
        {
            foreach (Method method in lstOfMethods)
            {
                yield return method;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    
    /*internal struct FirstOrderRegression
    {
        public const int MaxCode = 7;

        public static Method Optimal = new Method { Name = "Optimal", Text = "Подбор оптимальной регресси 1-го рода", Code = 0, FormulaImgPath = String.Empty, NOfPowers = 4, MaxParamCount = 1 };
        public static Method ExpReg = new Method { Name = "ExpReg", Text = "Экспоненциальная регрессия 1-го рода", Code = 1, FormulaImgPath = "/Krista.FM.RIA.Extensions.Forecast/Presentation/Content/Formulas/exp.png/extention.axd", NOfPowers = 4, Coeff = new[] { "a", "b", "c", "alfa" }, MaxParamCount = 1 };
        public static Method PowReg = new Method { Name = "PowReg", Text = "Степенная регрессия 1-го рода", Code = 2, FormulaImgPath = "/Krista.FM.RIA.Extensions.Forecast/Presentation/Content/Formulas/pow.png/extention.axd", NOfPowers = 4, Coeff = new[] { "a", "b", "c", "alfa" }, MaxParamCount = 1 };
        public static Method LogReg = new Method { Name = "LogReg", Text = "Логарифмическая регрессия 1-го рода", Code = 3, FormulaImgPath = "/Krista.FM.RIA.Extensions.Forecast/Presentation/Content/Formulas/log.png/extention.axd", NOfPowers = 4, Coeff = new[] { "a", "c", "b", "d" }, MaxParamCount = 1 };
        public static Method LogistReg = new Method { Name = "LogistReg", Text = "Логистическая регрессия 1-го рода", Code = 4, FormulaImgPath = "/Krista.FM.RIA.Extensions.Forecast/Presentation/Content/Formulas/logist.png/extention.axd", NOfPowers = 4, Coeff = new[] { "a", "b", "c", "alfa" }, MaxParamCount = 1 };
        public static Method PolyReg = new Method { Name = "PolyReg", Text = "Полиномиальная регрессия 1-го рода", Code = 5, FormulaImgPath = "/Krista.FM.RIA.Extensions.Forecast/Presentation/Content/Formulas/poly.png/extention.axd", NOfPowers = 4, Coeff = new[] { "a", "b", "c", "d" }, MaxParamCount = 1 };
        public static Method PolyExpReg = new Method { Name = "PolyExpReg", Text = "Полиномиально-экспоненциальная регрессия 1-го рода", Code = 6, FormulaImgPath = "/Krista.FM.RIA.Extensions.Forecast/Presentation/Content/Formulas/polyexp.png/extention.axd", NOfPowers = 4, Coeff = new[] { "a", "b", "c", "d" }, MaxParamCount = 1 };
        public static Method PolyPowReg = new Method { Name = "PolyPowReg", Text = "Полиномиально-степенная регрессия 1-го рода", Code = 7, FormulaImgPath = "/Krista.FM.RIA.Extensions.Forecast/Presentation/Content/Formulas/polypow.png/extention.axd", NOfPowers = 4, Coeff = new[] { "a", "b", "c", "alfa" }, MaxParamCount = 1 };

        public static Method? GetMethod(int code)
        {
            if (Optimal.Code == code)
            {
                return Optimal;
            }

            if (ExpReg.Code == code)
            {
                return ExpReg;
            }

            if (PowReg.Code == code)
            {
                return PowReg;
            }

            if (LogReg.Code == code)
            {
                return LogReg;
            }

            if (LogistReg.Code == code)
            {
                return LogistReg;
            }

            if (PolyReg.Code == code)
            {
                return PolyReg;
            }

            if (PolyExpReg.Code == code)
            {
                return PolyExpReg;
            }

            if (PolyPowReg.Code == code)
            {
                return PolyPowReg;
            }

            return null;
        }
    }
    
    internal struct SecondOrderRegression
    {
        public const int MaxCode = 4;

        public static Method Optimal = new Method { Name = "Optimal", Text = "Подбор оптимальной регресси 2-го рода", Code = 0, FormulaImgPath = String.Empty, NOfPowers = 5, MaxParamCount = 1 };
        public static Method PolyExpReg = new Method { Name = "PolyExpReg", Text = "Полиномиально-экспоненциальная регрессия 2-го рода", Code = 1, FormulaImgPath = "/Krista.FM.RIA.Extensions.Forecast/Presentation/Content/Formulas/polyexp5.png/extention.axd", NOfPowers = 5, Coeff = new[] { "a", "b", "c", "d", "f" }, MaxParamCount = 1 };
        public static Method PolyPowReg = new Method { Name = "PolyPowReg", Text = "Полиномиально-степенная регрессия 2-го рода", Code = 2, FormulaImgPath = "/Krista.FM.RIA.Extensions.Forecast/Presentation/Content/Formulas/polypow5.png/extention.axd", NOfPowers = 5, Coeff = new[] { "a", "b", "c", "d", "alfa" }, MaxParamCount = 1 };
        public static Method PolyLogReg = new Method { Name = "PolyLogReg", Text = "Полиномиально-логарифмическая регрессия 2-го рода", Code = 3, FormulaImgPath = "/Krista.FM.RIA.Extensions.Forecast/Presentation/Content/Formulas/polylog5.png/extention.axd", NOfPowers = 5, Coeff = new[] { "a", "b", "c", "d", "f" }, MaxParamCount = 1 };
        public static Method PolyLogistReg = new Method { Name = "PolyLogistReg", Text = "Полиномиально-логистическая регрессия 2-го рода", Code = 4, FormulaImgPath = "/Krista.FM.RIA.Extensions.Forecast/Presentation/Content/Formulas/polylogist5.png/extention.axd", NOfPowers = 5, Coeff = new[] { "a", "b", "c", "d", "f" }, MaxParamCount = 1 };

        public static Method? GetMethod(int code)
        {
            if (code == Optimal.Code)
            {
                return Optimal;
            }

            if (code == PolyExpReg.Code)
            {
                return PolyExpReg;
            }

            if (code == PolyPowReg.Code)
            {
                return PolyPowReg;
            }

            if (code == PolyLogReg.Code)
            {
                return PolyLogReg;
            }

            if (code == PolyLogistReg.Code)
            {
                return PolyLogistReg;
            }

            return null;
        }
    }

    internal struct ARMAwithRegression
    {
        public static Method ARMA22FirstOrder = new Method { Name = "ARMA22FirstOrder", Text = "ARMA(2,2) по остаткам регрессии 1-го рода", Code = 1, FormulaImgPath = String.Empty, MaxParamCount = 1 };
        public static Method ARMA22SecondOrder = new Method { Name = "ARMA22SecondOrder", Text = "ARMA(2,2) по остаткам регрессии 2-го рода", Code = 2, FormulaImgPath = String.Empty, MaxParamCount = 1 };
        public static Method ARMA33FirstOrder = new Method { Name = "ARMA33FirstOrder", Text = "ARMA(3,3) по остаткам регрессии 1-го рода", Code = 3, FormulaImgPath = String.Empty, MaxParamCount = 1 };
        public static Method ARMA33SecondOrder = new Method { Name = "ARMA33SecondOrder", Text = "ARMA(3,3) по остаткам регрессии 2-го рода", Code = 4, FormulaImgPath = String.Empty, MaxParamCount = 1 };
        public static Method ARMA23FirstOrder = new Method { Name = "ARMA23FirstOrder", Text = "ARMA(2,3) по остаткам регрессии 1-го рода", Code = 5, FormulaImgPath = String.Empty, MaxParamCount = 1 };
        public static Method ARMA23SecondOrder = new Method { Name = "ARMA23SecondOrder", Text = "ARMA(2,3) по остаткам регрессии 2-го рода", Code = 6, FormulaImgPath = String.Empty, MaxParamCount = 1 };
        public static Method ARMA32FirstOrder = new Method { Name = "ARMA32FirstOrder", Text = "ARMA(3,2) по остаткам регрессии 1-го рода", Code = 7, FormulaImgPath = String.Empty, MaxParamCount = 1 };
        public static Method ARMA32SecondOrder = new Method { Name = "ARMA32SecondOrder", Text = "ARMA(3,2) по остаткам регрессии 2-го рода", Code = 8, FormulaImgPath = String.Empty, MaxParamCount = 1 };

        public static Method? GetMethod(int code)
        {
            if (code == ARMA22FirstOrder.Code)
            {
                return ARMA22FirstOrder;
            }

            if (code == ARMA22SecondOrder.Code)
            {
                return ARMA22SecondOrder;
            }

            if (code == ARMA23FirstOrder.Code)
            {
                return ARMA23FirstOrder;
            }

            if (code == ARMA23SecondOrder.Code)
            {
                return ARMA23SecondOrder;
            }

            if (code == ARMA32FirstOrder.Code)
            {
                return ARMA32FirstOrder;
            }

            if (code == ARMA32SecondOrder.Code)
            {
                return ARMA32SecondOrder;
            }

            if (code == ARMA33FirstOrder.Code)
            {
                return ARMA33FirstOrder;
            }

            if (code == ARMA33SecondOrder.Code)
            {
                return ARMA33SecondOrder;
            }

            return null;
        }
    }

    internal struct ComplexEquation
    {
        public static Method EmbeddedEquation = new Method { Name = "EmbeddedEquation", Text = "Вложенный расчет", Code = 0, FormulaImgPath = String.Empty };
        public static Method NowEquation = new Method { Name = "NowEquation", Text = "Расчет в текущем времени", Code = 1, FormulaImgPath = String.Empty };
        public static Method LagEquation = new Method { Name = "LagEquation", Text = "Расчет c лаговыми значениями", Code = 2, FormulaImgPath = String.Empty };
    }

    internal struct MultiRegression
    {
        public static Method SimpleRegression = new Method { Name = "SimpleRegression", Text = "Множественная регрессия", Code = 1, FormulaImgPath = String.Empty, MaxParamCount = 5 };

        public static Method? GetMethod(int code)
        {
            if (code == SimpleRegression.Code)
            {
                return SimpleRegression;
            }

            return null;
        }
    }*/
}
