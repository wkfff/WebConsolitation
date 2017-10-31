using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.Components.Components
{
    public enum FloatStyle
    {
        Left,
        Right,
        None
    }

    
    public partial class TagCloud : UserControl
    {
        public Color ForeColor = Color.Red;
        public string fontName = "Arial";
        public Collection<Color> ForeColors;
        public int groupCount = 5;
        public int startFontSize = 12;
        public int fontStep = 2;
        public FloatStyle floatStyle = FloatStyle.Left;
		public string displayStyle = "block";
    	public string whiteSpaceStyle = "normal";
        public string PaddingTop = "0px";

        private string GetFloatStyle()
        {
            string style = "left";

            switch(floatStyle)
            {
                case FloatStyle.Left:
                    {
                        style = "left";
                        break;
                    }
                case FloatStyle.Right:
                    {
                        style = "right";
                        break;
                    }
                case FloatStyle.None:
                    {
                        style = "none";
                        break;
                    }
            }

            return style;
        }

        public void Render(Dictionary<string, int> tags)
        {
            
            Dictionary<string, int> result = RecalculateTags(tags);
            int colorsCount = 0;
            foreach (KeyValuePair<string, int> tag in result)
            {
                Label label = GetCloudLabel(tag);
                colorsCount = SetTagColor(colorsCount, label);
                HtmlGenericControl div = GetTagDiv(label);
                this.Controls.Add(div);
            }
        }

        private HtmlGenericControl GetTagDiv(Label label)
        {
            HtmlGenericControl div = new HtmlGenericControl("div");
            
			div.Style.Add("padding-right", "25px");
            div.Style.Add("padding-top", PaddingTop);
			div.Style.Add("vertical-align", "middle");

			if (floatStyle != FloatStyle.None)
			{
				div.Style.Add("float", GetFloatStyle());
			}
			if (!displayStyle.Equals("block"))
			{
				div.Style.Add("display", displayStyle);
			}
			if (!whiteSpaceStyle.Equals("normal"))
			{
				div.Style.Add("white-space", whiteSpaceStyle);
			}

        	div.Controls.Add(label);
            
			return div;
        }

        private Label GetCloudLabel(KeyValuePair<string, int> tag)
        {
            Label label = new Label();
            label.Font.Size = tag.Value * fontStep + startFontSize;
            label.Text = tag.Key;
            label.Font.Name = fontName;
            return label;
        }

        private int SetTagColor(int colorsCount, Label label)
        {
            if (ForeColors != null && ForeColors.Count > 0)
            {
                if (colorsCount >= ForeColors.Count)
                {
                    colorsCount = 0;
                }
                label.ForeColor = ForeColors[colorsCount];
                colorsCount++;
            }
            else
            {
                label.ForeColor = ForeColor;
            }
            return colorsCount;
        }

        public void Render(Dictionary<string, Tag> tags)
        {
            Dictionary<string, int> tagsWork = new Dictionary<string, int>();
            foreach (Tag tag in tags.Values)
            {
                tagsWork.Add(tag.key, tag.weight);
            }

            Dictionary<string, int> result = RecalculateTags(tagsWork);
            
            int i = 0;
            int colorsCount = 0;
            foreach (KeyValuePair<string, int> tag in result)
            {
                Label label = GetCloudLabel(tag);
                colorsCount = SetTagColor(colorsCount, label);

                HtmlGenericControl div = GetTagDiv(label);

                div.ID = String.Format("{0}_tag{1}", this.ID, i);
                i++;
                TooltipHelper.AddToolTip(div, tags[tag.Key].toolTip, this.Page);
                this.Controls.Add(div);
            }
        }

        private Dictionary<string, int> RecalculateTags(Dictionary<string, int> tags)
        {
            int max = 0;
            int min = int.MaxValue;
            int[] weights = new int[tags.Count];
            int tagsCount = 0;
            foreach (KeyValuePair<string, int> tag in tags)
            {
                if (max < tag.Value)
                {
                    max = tag.Value;
                }
                if (min > tag.Value)
                {
                    min = tag.Value;
                }
                weights[tagsCount] = tag.Value;
                tagsCount++;
            }
                        
            // Примерно равномерно распределяем веса по кластерам
            Cluster[] clusters = new Cluster[groupCount];
            int step = min + (max - min) / groupCount;
            for (int i = 0; i < groupCount; i++)
            {
                clusters[i] = new Cluster();
                
                clusters[i].massCenter = min + step * i;
                
            }

            for (int j = 0; j < 10; j++)
            {
                CalculateDistances(clusters, tags);
                FindClosestPosition(clusters, tags);
                RecalculateClastrers(clusters);

                // Очищаем словари.
                for (int i = 0; i < clusters.Length; i++)
                {
                    clusters[i].tagDistance = new Dictionary<string, int>();
                    clusters[i].tagIncludes = new Dictionary<string, int>();
                }
            }
            CalculateDistances(clusters, tags);
            FindClosestPosition(clusters, tags);
            RecalculateClastrers(clusters);
            clusters = SortClusters(clusters);

            for (int i = 0; i < clusters.Length; i++)
            {
                clusters[i].weight = i;
            }
            //clusters = MixClusters(clusters);

            for (int i = 0; i < clusters.Length; i++)
            {
                foreach (KeyValuePair<string, int> tag in clusters[i].tagIncludes)
                {
                    tags[tag.Key] = clusters[i].weight;
                }
            }

            return tags;
        }

        private static void RecalculateClastrers(Cluster[] clusters)
        {
            for (int i = 0; i < clusters.Length; i++)
            {
                int massCenter = 0;
                foreach (string key in clusters[i].tagIncludes.Keys)
                {
                    massCenter += clusters[i].tagIncludes[key];
                }
                // Переписываем центр масс
                if (clusters[i].tagIncludes.Count > 0)
                {
                    clusters[i].massCenter = massCenter / clusters[i].tagIncludes.Count;
                }
                else
                {
                    clusters[i].massCenter = clusters[0].massCenter / 2;
                }
            }
        }

        private static void CalculateDistances(Cluster[] clusters, Dictionary<string, int> tags)
        {
            for (int i = 0; i < clusters.Length; i++)
            {
                foreach (KeyValuePair<string, int> tag in tags)
                {
                    clusters[i].tagDistance.Add(tag.Key, Math.Abs(clusters[i].massCenter - tag.Value));
                }
            }
        }

        public static void FindClosestPosition(Cluster[] clusters, Dictionary<string, int> tags)
        {
            clusters = SortClusters(clusters);
            // Проходим по тэгам
            foreach (KeyValuePair<string, int> tag in tags) 
            {
                int clusterNum = 0;
                int distance = int.MaxValue;
                // для них по всем кластерам
                for (int i = 0; i < clusters.Length; i++)
                {
                    // ищем наименьшую дистанцию
                    if (clusters[i].tagDistance[tag.Key] < distance)
                    {
                        distance = clusters[i].tagDistance[tag.Key];
                        clusterNum = i;
                    }
                }
                // Добавляем тэг в ближний кластер
                clusters[clusterNum].tagIncludes.Add(tag.Key, tag.Value);
            }
        }

        private static Cluster[] SortClusters(Cluster[] clusters)
        {
            bool notSorted = true;
            while (notSorted)
            {
                notSorted = false;
                for (int i = 0; i < clusters.Length - 1; i++)
                {
                    if (clusters[i].massCenter > clusters[i + 1].massCenter)
                    {
                        Cluster cluster = clusters[i];
                        clusters[i] = clusters[i + 1];
                        clusters[i + 1] = cluster;
                        notSorted = true;
                    }
                }
            }
            return clusters;
        }

        private static Cluster[] MixClusters(Cluster[] clusters)
        {
            Cluster[] result = new Cluster[clusters.Length];
            int clusterCounter = 0;
            int resultCounter = 0;

            for (; clusterCounter < clusters.Length; clusterCounter += 2)
            {
                result[resultCounter] = clusters[clusterCounter];
                resultCounter++;
            }
             
            // Перескочили конец на 1
            if ((clusters.Length - 1) == clusterCounter - 1)
            {
                clusterCounter -= 1;
                for (; clusterCounter > 0; clusterCounter -= 2)
                {
                    result[resultCounter] = clusters[clusterCounter];
                    resultCounter++;
                }
            }
            else
            {
                clusterCounter -= 2;
                for (; clusterCounter > 1; clusterCounter -= 2)
                {
                    result[resultCounter] = clusters[clusterCounter];
                    resultCounter++;
                }
            }
            
            return result;
        }
    }

    public class Cluster
    {
        public int massCenter;
        public Dictionary<string, int> tagDistance = new Dictionary<string, int>();
        public Dictionary<string, int> tagIncludes = new Dictionary<string, int>();
        public int weight;
    }

    public class Tag
    {
        public int weight;
        public string key;
        public string toolTip;
    }
}