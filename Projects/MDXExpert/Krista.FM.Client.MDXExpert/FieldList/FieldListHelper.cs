using Infragistics.Win.UltraWinTree;
using Krista.FM.Client.MDXExpert.Data;
using Microsoft.AnalysisServices.AdomdClient;

namespace Krista.FM.Client.MDXExpert.FieldList
{
    public struct FieldListHelper
    {
        /// <summary>
        /// Получение данных из узла
        /// </summary>
        /// <param name="node">узел</param>
        /// <returns>данные узла</returns>
        public static ItemData GetItemData(UltraTreeNode node)
        {
            if (node != null)
            {
                return ((ItemData)node.Tag);
            }
            return null;
        }

        /// <summary>
        /// Получение юникнейма из данных поля
        /// </summary>
        /// <param name="iData">Данные поля</param>
        /// <returns>юникнейм поля</returns>
        public static string GetUniqueNameFromItemData(ItemData iData)
        {
            string result = "";

            switch (iData.ItemType)
            {
                case ItemType.ntHierarch:
                    result = ((Hierarchy)iData.AdomdObj).UniqueName;
                    break;
                case ItemType.ntLevel:
                    result = ((Level)iData.AdomdObj).UniqueName;
                    break;
                case ItemType.ntMeasure:
                    if (!((PivotTotal)iData.PivotObj).IsCustomTotal)
                    {
                        result = ((Member) iData.AdomdObj).UniqueName;
                    }
                    else
                    {
                        result = ((PivotTotal)iData.PivotObj).UniqueName;
                    }
                    break;
            }
            return result;
        }

        /// <summary>
        /// Получение юникнейма из узла
        /// </summary>
        /// <param name="node">Узел</param>
        /// <returns>юникнейм поля</returns>
        public static string GetUniqueNameFromNode(UltraTreeNode node)
        {
            return GetUniqueNameFromItemData(GetItemData(node));
        }

        public static bool IsMeasuresNode(UltraTreeNode node)
        {
            ItemData iData = GetItemData(node);

            if (iData.ItemType == ItemType.ntHierarch)
            {
                if (((Hierarchy)iData.AdomdObj).Name == "Measures")
                {
                    return true;
                }
            }

            return false;
        }

        public static UltraTreeNode GetMeasuresNode(UltraTreeNode root)
        {
            foreach(UltraTreeNode node in root.Nodes)
            {
                if (IsMeasuresNode(node))
                {
                    return node;
                }
            }
            return null;
        }

    }
}
