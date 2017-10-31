
using System;
using System.Linq;
using Krista.FM.Client.Reports.MOFO.Helpers;

namespace Krista.FM.Client.Reports.MOFO0022.Helpers
{
    internal class MOFO0022AteGrouping : MOFOAteGrouping
    {
        public const string Title = "По крупнейшим недоимщикам";

        public MOFO0022AteGrouping(int style): base(style)
        {
            SetFixedValues(GetRegionsId());
        }
    }

    enum ContractType
    {
        Municipal,         // договора, заключенные муниципальными образованиями
        AssetDepartment,   // договора, заключенные минмособлимуществом
        All
    }

    class ReportMOFO0022Helper
    {
        public const string FixOrgRef = "-1";

        public const int MunicipalNonDifRealtyIncome = 0;   // доходы местного бюджета от земли, собственность на кот. от неразграничена
        public const int MunicipalDifRealtyIncome = 1;      // доходы местного бюджета от земли, собственность на кот. от неразграничена
        public const int AssetRentIncome = 2;               // доходы от сдачи в аренду имущество
        public const int AssetDepartmentIncome = 3;         // доходы минмособлимущества от земли, собственность на кот. от неразграничена

        public static string GetContractTypeCodes(ContractType type, bool withAssetRent = false)
        {
            int[] result;

            switch (type)
            {
                case ContractType.Municipal:
                    result = withAssetRent
                        ? new[] { MunicipalNonDifRealtyIncome, MunicipalDifRealtyIncome, AssetRentIncome }
                        : new[] { MunicipalNonDifRealtyIncome, MunicipalDifRealtyIncome };
                    break;
                case ContractType.AssetDepartment:
                    result = new[] { AssetDepartmentIncome };
                    break;
                default:
                    result = withAssetRent
                        ? new[] { MunicipalNonDifRealtyIncome, MunicipalDifRealtyIncome, AssetRentIncome, AssetDepartmentIncome }
                        : new[] { MunicipalNonDifRealtyIncome, MunicipalDifRealtyIncome, AssetDepartmentIncome };
                    break;
            }

            return String.Join(", ", result.Select(i => Convert.ToString(i)).ToArray());
        }
    }
}
