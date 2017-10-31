using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class ExportService : IExportService
    {
        private readonly IForecastVariantsRepository variantsRepository;
        private readonly IForecastParamsRepository paramsRepository;
        private readonly IForecastValuesRepository valuesRepository;
        private readonly IRepository<FX_Date_YearDayUNV> yearRepository;

        public ExportService(
            IForecastVariantsRepository variantsRepository,
            IForecastParamsRepository paramsRepository,
            IForecastValuesRepository valuesRepository,
            IRepository<FX_Date_YearDayUNV> yearRepository)
        {
            this.variantsRepository = variantsRepository;
            this.paramsRepository = paramsRepository;
            this.valuesRepository = valuesRepository;
            this.yearRepository = yearRepository;
        }
        
        public Stream ExportPlanResult(int varid)
        {
            Stream template = new MemoryStream(Resources.Resource.PlanResult);

            HSSFWorkbook wb = new HSSFWorkbook(template);

            var sheet = wb.GetSheetAt(0);

            var variant = variantsRepository.FindOne(varid);

            XDocument xDoc = XDocument.Parse(variant.XMLString);

            int progFrom = Convert.ToInt32(xDoc.Root.Attribute("from").Value);
            int progTo = Convert.ToInt32(xDoc.Root.Attribute("to").Value);

            var usedDatas = xDoc.Root.Element("UsedDatas");
            var usedData = usedDatas.Element("UsedData");

            int paramId = Convert.ToInt32(usedData.Attribute("fparamid").Value);

            var usedYears = xDoc.Root.Element("UsedYears");

            int dataFrom = Convert.ToInt32(usedYears.Attribute("from").Value);
            int dataTo = Convert.ToInt32(usedYears.Attribute("to").Value);

            for (int i = dataFrom; i <= dataTo; i++)
            {
                NPOIHelper.SetCellValue(sheet, 0, i - dataFrom + 2, i);
            }

            for (int i = dataFrom; i <= progTo; i++)
            {
                NPOIHelper.SetCellValue(sheet, 20, i - dataFrom + 2, i);
            }

            int j = 1;
            foreach (XElement data in usedData.Elements("Data"))
            {
                int id = Convert.ToInt32(data.Attribute("paramid").Value);
                var param = paramsRepository.FindOne(id);
                NPOIHelper.SetCellValue(sheet, j, 0, param.Name);
                NPOIHelper.SetCellValue(sheet, j, 1, param.RefOKEI.Designation);

                var valuesStat = (from t in valuesRepository.FindAll()
                                 where (t.RefVar == variant) && (t.RefStat.ID == 1) && (t.RefParam == param)
                                 select new
                                 {
                                     t.Value,
                                     Year = (int)(t.RefDate.ID / 10000)
                                 }).ToList();
                foreach (var value in valuesStat)
                {
                    NPOIHelper.SetCellValue(sheet, j, value.Year - dataFrom + 2, value.Value);
                }
                
                j++;
            }

            var progParam = paramsRepository.FindOne(paramId);
            NPOIHelper.SetCellValue(sheet, 21, 0, progParam.Name);
            NPOIHelper.SetCellValue(sheet, 21, 1, progParam.RefOKEI.Designation);

            var valuesProg = (from t in valuesRepository.FindAll()
                              where (t.RefVar == variant) && (t.RefStat.ID == 0) && (t.RefParam == progParam)
                              select new
                              {
                                  t.Value,
                                  Year = (int)(t.RefDate.ID / 10000)
                              }).ToList();

            foreach (var value in valuesProg)
            {
                NPOIHelper.SetCellValue(sheet, 21, value.Year - dataFrom + 2, value.Value);
            }

            return WriteToStream(wb);
        }

        private static MemoryStream WriteToStream(HSSFWorkbook wb)
        {
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI";
            wb.DocumentSummaryInformation = dsi;
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI";
            wb.SummaryInformation = si;

            MemoryStream resultStream = new MemoryStream();
            wb.Write(resultStream);

            resultStream.Seek(0, SeekOrigin.Begin);
            return resultStream;
        }
    }
}
