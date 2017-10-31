using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Krista.FM.Server.Dashboards.Components;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Reports.Report.Section;

namespace Krista.FM.Server.Dashboards.reports.SGM
{
    public class SGMPdfExportHelper
    {
        private const string fontName = "Verdana";
        private const int captionFontSize = 16;
        private const int subCaptionFontSize = 14;
        private const int mainTextFontSize = 10;
        private const int commentTextFontSize = 8;

        private int totalHeight;

        public string TruncHTMLTags(string text)
        {
            string result = text;
            result = result.Replace("&nbsp;", " ");
            result = result.Replace("&nbsp", " ");
            result = result.Replace("\r", string.Empty);
            result = result.Replace("\n", string.Empty);
            result = result.Replace("<br>", "\r");
            result = result.Replace("<nobr>", string.Empty);
            result = result.Replace("</nobr>", string.Empty);
            result = result.Replace("<b>", string.Empty);
            result = result.Replace("</b>", string.Empty);
            return result;
        }

        public void ClearInnerHeight()
        {
            totalHeight = 0;
        }

        public void CalcInnerHeight(IText title)
        {
            totalHeight += (int)title.Measure().Height + 10;
        }

        public void CalcInnerHeight(int height)
        {
            totalHeight += height + 10;
        }

        public void ExportCaptionText(DocumentExportEventArgs e, string text)
        {
            var title = e.Section.AddText();
            var font = new Font(fontName, captionFontSize, FontStyle.Bold);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Alignment = Infragistics.Documents.Reports.Report.TextAlignment.Left;
            title.AddContent(TruncHTMLTags(text));

            ClearInnerHeight();
            CalcInnerHeight(title);
        }

        #region Subcaption

        private void InternalExportSubCaptionText(IText title, string text)
        {
            var font = new Font(fontName, subCaptionFontSize);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Alignment = Infragistics.Documents.Reports.Report.TextAlignment.Left;
            title.AddContent(TruncHTMLTags(text));
            CalcInnerHeight(title);
        }

        public void ExportSubCaptionText(EndExportEventArgs e, string text)
        {
            var title = e.Section.AddText();
            InternalExportSubCaptionText(title, text);
        }

        public void ExportSubCaptionText(DocumentExportEventArgs e, string text)
        {
            var title = e.Section.AddText();
            InternalExportSubCaptionText(title, text);
        }

        #endregion

        #region Comment

        private void InternalExportCommentText(IText title, string text)
        {
            var font = new Font(fontName, commentTextFontSize);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Alignment = Infragistics.Documents.Reports.Report.TextAlignment.Left;
            title.AddContent(TruncHTMLTags(text));
            CalcInnerHeight(title);
        }

        public void ExportCommentText(EndExportEventArgs e, string text)
        {
            IText title = e.Section.AddText();
            InternalExportCommentText(title, text);
        }

        public void ExportCommentText(DocumentExportEventArgs e, string text)
        {
            IText title = e.Section.AddText();
            InternalExportCommentText(title, text);
        }

        #endregion

        #region MainText

        private void InternalExportMainText(IText title, string text)
        {
            var font = new Font(fontName, mainTextFontSize);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Alignment = Infragistics.Documents.Reports.Report.TextAlignment.Left;
            GetTextWithImages(title, TruncHTMLTags(text));
            CalcInnerHeight(title);
        }

        public void ExportMainText(EndExportEventArgs e, string text)
        {
            IText title = e.Section.AddText();
            InternalExportMainText(title, text);
        }

        public void ExportMainText(DocumentExportEventArgs e, string text)
        {
            IText title = e.Section.AddText();
            InternalExportMainText(title, text);
        }

        #endregion

        #region Map

        public void ExportMap(EndExportEventArgs e, Dundas.Maps.WebControl.MapControl map)
        {
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromMap(map);
            e.Section.AddImage(img);
            CalcInnerHeight(img.Height);
        }

        public void ExportMap(DocumentExportEventArgs e, Dundas.Maps.WebControl.MapControl map)
        {
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromMap(map);
            e.Section.AddImage(img);
            CalcInnerHeight(img.Height);
        }

        #endregion

        #region Chart

        public void ExportChart(EndExportEventArgs e, UltraChart chart)
        {            
            ISection section = e.Section;
            IText title = section.AddText();
            SplitLargeImages(section, title, GetImageFromChart(chart), chart);
        }

        public void ExportChart(DocumentExportEventArgs e, UltraChart chart)
        {            
            var section = e.Section;
            var title = section.AddText();
            SplitLargeImages(section, title, GetImageFromChart(chart), chart);
        }

        public void SplitLargeImages(ISection section, IText title, Infragistics.Documents.Reports.Graphics.Image image, UltraChart chart)
        {
            if (chart.ChartType != Infragistics.UltraChart.Shared.Styles.ChartType.BarChart)
            {
                title.AddContent(image);
                CalcInnerHeight(image.Height);
                return;
            }

            var pageHeight = (int)section.PageSize.Height;
            int part1Height = pageHeight - totalHeight % pageHeight;
            
            if (part1Height < 300) part1Height = pageHeight;
            if (image.Height < part1Height) part1Height = image.Height;

            if (part1Height >= image.Height)
            {
                title.AddContent(image);
                CalcInnerHeight(image.Height);
            }
            else
            {
                int imageCount = Convert.ToInt32((image.Height - part1Height) / pageHeight) + 2;

                var imgPart = new Infragistics.Documents.Reports.Graphics.Image(image.Width, part1Height);
                for (int x = 0; x < image.Width; x++)
                {
                    for (int y = 0; y < part1Height; y++)
                    {
                        imgPart.SetPixel(x, y, image.GetPixel(x, y));
                    }
                }
                
                title.AddContent(imgPart);
                CalcInnerHeight(imgPart.Height);
                title = section.AddText();

                for (int i = 1; i < imageCount; i++)
                {
                    int partHeight = pageHeight;
                    if (i + 1 == imageCount) partHeight = image.Height - pageHeight * (i - 1) - part1Height;
                    imgPart = new Infragistics.Documents.Reports.Graphics.Image(image.Width, partHeight);
                    for (int x = 0; x < image.Width; x++)
                    {
                        for (int y = 0; y < partHeight; y++)
                        {
                            imgPart.SetPixel(x, y, image.GetPixel(x, part1Height + (i - 1) * pageHeight + y));
                        }
                    }
                    title.AddContent(imgPart);
                    CalcInnerHeight(imgPart.Height);
                    title = section.AddText();
                }
            }
        }

        public Infragistics.Documents.Reports.Graphics.Image GetImageFromChart(UltraChart chart)
        {
            var imageStream = new MemoryStream();
            chart.SaveTo(imageStream, ImageFormat.Png);
            var img = new Infragistics.Documents.Reports.Graphics.Image(imageStream);
            img.Preferences.Compressor = Infragistics.Documents.Reports.Graphics.ImageCompressors.Flate;
            return img;
        }

        #endregion

        public void GetTextWithImages(IText title, string fullText)
        {
            string text = fullText;
            string deletedTags = string.Empty;
            bool hasImage = true;
            while (hasImage)
            {
                int imageStartPosition = text.IndexOf("<img");
                int imageEndPosition = text.IndexOf(">");
                string textPart;
                if (imageStartPosition >= 0)
                {
                    textPart = text.Substring(0, imageStartPosition);
                    deletedTags = text.Substring(imageStartPosition, imageEndPosition - imageStartPosition + 2);
                    text = text.Remove(0, imageEndPosition + 1);
                }
                else
                {
                    hasImage = false;
                    textPart = text;
                }
                title.AddContent(textPart);
                if (hasImage)
                {
                    imageStartPosition = deletedTags.IndexOf("src=");
                    imageEndPosition = deletedTags.IndexOf("'", imageStartPosition + 6);
                    var imagePath = deletedTags.Substring(imageStartPosition + 5, imageEndPosition - imageStartPosition - 5);
                    title.AddContent(UltraGridExporter.GetImage(imagePath));
                }
            }            
        }
    }
}
