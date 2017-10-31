using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataSourcesManager
{
    /// <summary>
    /// Статический класс содержащий различные функции для работы с исчниками данных и не только
    /// </summary>
    public static class DataSourceUtils
    {
        public static string TakeMethod2String(TakeMethodTypes takeMethod)
        {
            switch (takeMethod)
            {
                case TakeMethodTypes.Import:
                    return "ИМПОРТ";
                case TakeMethodTypes.Receipt:
                    return "СБОР";
                case TakeMethodTypes.Input:
                    return "ВВОД";
                default:
                    throw new Exception("Неверный метод получения информации " + takeMethod);
            }
        }

        public static string ParamKind2String(ParamKindTypes paramKind)
        {
            switch (paramKind)
            {
                case ParamKindTypes.Budget:
                    return "ГодФинорган";
                case ParamKindTypes.Year:
                    return "Год";
                case ParamKindTypes.YearMonthVariant:
                    return "ГодМесяцВариант";
                case ParamKindTypes.YearMonth:
                    return "ГодМесяц";
                case ParamKindTypes.YearVariant:
                    return "ГодВариант";
                case ParamKindTypes.YearQuarter:
                    return "ГодКвартал";
                case ParamKindTypes.YearTerritory:
                    return "ГодТерритория";
                case ParamKindTypes.YearQuarterMonth:
                    return "ГодКварталМесяц";
                case ParamKindTypes.WithoutParams:
                    return "БезПараметров";
                case ParamKindTypes.Variant:
                    return "Вариант";
                case ParamKindTypes.YearVariantMonthTerritory:
                    return "ГодВариантМесяцТерритория";
                default:
                    throw new Exception("Неверный вид параметра поступающей информации " + paramKind);
            }
        }

        public static ParamKindTypes String2ParamKind(string paramKindString)
        {
            switch (paramKindString)
            {
                case "ГодФинорган":
                    return ParamKindTypes.Budget;
                case "Год":
                    return ParamKindTypes.Year;
                case "ГодМесяцВариант":
                    return ParamKindTypes.YearMonthVariant;
                case "ГодМесяц":
                    return ParamKindTypes.YearMonth;
                case "ГодВариант":
                    return ParamKindTypes.YearVariant;
                case "ГодКвартал":
                    return ParamKindTypes.YearQuarter;
                case "ГодТерритория":
                    return ParamKindTypes.YearTerritory;
                case "ГодКварталМесяц":
                    return ParamKindTypes.YearQuarterMonth;
                case "БезПараметров":
                    return ParamKindTypes.WithoutParams;
                case "Вариант":
                    return ParamKindTypes.Variant;
                case "ГодВариантМесяцТерритория":
                    return ParamKindTypes.YearVariantMonthTerritory;
                default:
                    throw new Exception("Неверный вид параметра поступающей информации " + paramKindString);
            }
        }
    }
}
