using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DisintRules
{
    internal class NormativesUtils
    {
        internal IEntity GetNormativeEntity(NormativesKind normative, IScheme scheme)
        {
            string normativeKey = string.Empty;
            switch (normative)
            {
                case NormativesKind.NormativesBK:
                    normativeKey = NormativesObjectKeys.f_Norm_BK;
                    break;
                case NormativesKind.NormativesRegionRF:
                    normativeKey = NormativesObjectKeys.f_Norm_Region;
                    break;
                case NormativesKind.NormativesMR:
                    normativeKey = NormativesObjectKeys.f_Norm_MR;
                    break;
                case NormativesKind.VarNormativesRegionRF:
                    normativeKey = NormativesObjectKeys.f_Norm_VariedRegion;
                    break;
                case NormativesKind.VarNormativesMR:
                    normativeKey = NormativesObjectKeys.f_Norm_VariedMR;
                    break;
            }
            return scheme.RootPackage.FindEntityByName(normativeKey);
        }

        internal static int GetBudgetLevel(TerritoryType territoryType)
        {
            switch (territoryType)
            {
                case TerritoryType.Unknown:
                    return -1;
                case TerritoryType.GO:
                    return 15;
                case TerritoryType.GP:
                    return 16;
                case TerritoryType.MR:
                    return 5;
                case TerritoryType.MT:
                    return 5;
                case TerritoryType.SB:
                    return 3;
                case TerritoryType.SP:
                    return 17;
                case TerritoryType.RC:
                    return 5;
            }
            return -1;
        }

        /// <summary>
        /// тип территории для получения нужного норматива
        /// </summary>
        /// <param name="refRegion"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        internal static TerritoryType GetTerritoryType(int refRegion, IDatabase db)
        {
            object queryResult = db.ExecQuery(
                string.Format(
                    @"select regionBridge.RefTerrType from b_Regions_Bridge regionBridge, D_REGIONS_PLAN regionplan 
                    where regionplan.ID = ? and regionplan.RefBridge = regionBridge.ID"),
                    QueryResultTypes.Scalar, new DbParameterDescriptor("p0", refRegion));

            if (queryResult == DBNull.Value || queryResult == null)
                return TerritoryType.Unknown;

            switch (Convert.ToInt32(queryResult))
            {
                case 3:
                    return TerritoryType.SB;
                case 4:
                    return TerritoryType.MR;
                case 5:
                    return TerritoryType.GP;
                case 6:
                    return TerritoryType.SP;
                case 7:
                    return TerritoryType.GO;
                case 9:
                    return TerritoryType.MT;
                case 10:
                    return TerritoryType.RC;
                default:
                    return TerritoryType.Unknown;
            }
        }

        /// <summary>
        /// возвращает тип выбранного варианта
        /// </summary>
        /// <param name="refVariant"></param>
        /// <returns></returns>
        internal static VariantType GetVariantType(int refVariant)
        {
            switch (refVariant)
            {
                case 1:
                    return VariantType.BudgetLevels;
                case 2:
                    return VariantType.QuotaToAllLevels;
                case 3:
                    return VariantType.QuotaToConsRegion;
                case 4:
                    return VariantType.QuotaToConsMR;
            }
            return VariantType.Unknown;
        }

        /// <summary>
        /// запись сообщения в протокол
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="message"></param>
        /// <param name="variant"></param>
        /// <param name="classifiersEventKind"></param>
        internal static void SendMessageToProtocol(IClassifiersProtocol protocol, string message, ClassifiersEventKind classifiersEventKind, int variant)
        {
            protocol.WriteEventIntoClassifierProtocol(classifiersEventKind, "ФО_Результат доходов с расщеплением",
                -1, variant, (int)ClassTypes.clsFactData, message);
        }
    }
}
