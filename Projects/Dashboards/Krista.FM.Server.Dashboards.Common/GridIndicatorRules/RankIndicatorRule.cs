using System;
using Infragistics.WebUI.UltraWebGrid;

namespace Krista.FM.Server.Dashboards.Common.GridIndicatorRules
{
    public class RankIndicatorRule : IndicatorRule
    {
        #region Поля

        private int rankColumnIndex = -1;
        private int worseRankColumnIndex = -1;
        private string rankColumnName = String.Empty;
        private string worseRankColumnName = String.Empty;

        private string bestRankHint = String.Empty;
        private string worseRankHint = String.Empty;

        private string bestRankImg = "~/images/starYellowBB.png";
        private string worseRankImg = "~/images/starGrayBB.png";

        #endregion

        #region Свойства

        private bool UseColumnName
        {
            get { return rankColumnName != String.Empty; }
        }

        public string BestRankHint
        {
            get { return bestRankHint; }
            set { bestRankHint = value; }
        }

        public string WorseRankHint
        {
            get { return worseRankHint; }
            set { worseRankHint = value; }
        }

        public string BestRankImg
        {
            get { return bestRankImg; }
            set { bestRankImg = value; }
        }

        public string WorseRankImg
        {
            get { return worseRankImg; }
            set { worseRankImg = value; }
        }

        #endregion

        public RankIndicatorRule(int rankColumnIndex, int worseRankColumnIndex)
        {
            this.rankColumnIndex = rankColumnIndex;
            this.worseRankColumnIndex = worseRankColumnIndex;
        }

        public RankIndicatorRule(string rankColumnName, string worseRankColumnName)
        {
            this.rankColumnName = rankColumnName;
            this.worseRankColumnName = worseRankColumnName;
        }

        public override void SetRowStyle(UltraGridRow row)
        {
            for (int i = 0; i < row.Cells.Count; i++)
            {
                UltraGridCell cell = row.Cells[i];
                string columnCaption = row.Band.Grid.Columns[i].Header.Caption;

                int rank = Int32.MinValue;
                int worseRank = Int32.MinValue;
                if (UseColumnName && columnCaption.Contains(rankColumnName))
                {
                    rank = GetIntDtValue(row, rankColumnName);
                    worseRank = GetIntDtValue(row, worseRankColumnName);
                }
                else if (!UseColumnName && i == rankColumnIndex)
                {
                    rank = GetIntDtValue(row, rankColumnIndex);
                    worseRank = GetIntDtValue(row, worseRankColumnIndex);
                }

                if (rank != Int32.MinValue && worseRank != Int32.MinValue)
                {
                    if (rank  == 1)
                    {
                        cell.Style.BackgroundImage = BestRankImg;
                        cell.Title = BestRankHint;
                    }
                    else if (rank == worseRank)
                    {
                        cell.Style.BackgroundImage = WorseRankImg;
                        cell.Title = WorseRankHint;
                    }

                    cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }
            }
        }
    }
}
