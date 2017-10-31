using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class MethodsLoader : IMethodsLoader
    {
        private readonly IForecastExtension extension;
        private readonly IForecastMethodsRepository methodsRepository;
        
        public MethodsLoader(IForecastExtension extension, IForecastMethodsRepository methodsRepository)
        {
            this.extension = extension;
            this.methodsRepository = methodsRepository;
        }

        public void LoadMethods()
        {
            var groups = (from t in methodsRepository.GetAllMethods()
                           where t.Code2 == 0
                          select t).ToList();

            foreach (D_Forecast_PMethods group in groups)
            {
                switch (group.Code1)
                {
                    case FixedMathGroups.ComplexEquation:
                    case FixedMathGroups.FirstOrderRegression:
                    case FixedMathGroups.SecondOrderRegression:
                    case FixedMathGroups.ARMAMethod:
                    case FixedMathGroups.MultiRegression:
                    case FixedMathGroups.PCAForecast:
                        extension.LoadedMathGroups.AddGroup(group.TextCode, group.Code1.Value, group.TextName);
                        break;
                }
            }

            var methods = (from t in methodsRepository.GetAllMethods()
                           where t.Code2 != 0
                          select t).ToList();

            foreach (D_Forecast_PMethods method in methods)
            {
                int groupCode = method.Code1.Value;
                var tmpGroup = extension.LoadedMathGroups.GetGroupByCode(groupCode);
                    
                if (tmpGroup.HasValue)
                {
                    Group group = tmpGroup.Value;

                    switch (groupCode)
                    {
                        case FixedMathGroups.FirstOrderRegression:
                        case FixedMathGroups.SecondOrderRegression:
                            switch (method.Code2)
                            {
                                case FirstOrderRegression.ExpReg:
                                case FirstOrderRegression.LogistReg:
                                case FirstOrderRegression.LogReg:
                                case FirstOrderRegression.Optimal:
                                case FirstOrderRegression.PolyExpReg:
                                case FirstOrderRegression.PolyPowReg:
                                case FirstOrderRegression.PolyReg:
                                case FirstOrderRegression.PowReg:
                                    group.Methods.AddMethod(
                                        method.TextCode, 
                                        method.Code2.Value, 
                                        method.TextName, 
                                        method.Descr, 
                                        DecompressFile(method.ImageFile), 
                                        method.Coeffs, 
                                        method.PCount,
                                        method.XMLString);
                                    break;
                            }

                            break;
                        
                        case FixedMathGroups.ARMAMethod: 
                            switch (method.Code2)
                            {
                                case ARMAwithRegression.ARMA22FirstOrder:
                                case ARMAwithRegression.ARMA22SecondOrder:
                                case ARMAwithRegression.ARMA23FirstOrder:
                                case ARMAwithRegression.ARMA23SecondOrder:
                                case ARMAwithRegression.ARMA32FirstOrder:
                                case ARMAwithRegression.ARMA32SecondOrder:
                                case ARMAwithRegression.ARMA33FirstOrder:
                                case ARMAwithRegression.ARMA33SecondOrder:
                                    group.Methods.AddMethod(
                                        method.TextCode,
                                        method.Code2.Value,
                                        method.TextName,
                                        method.Descr,
                                        DecompressFile(method.ImageFile),
                                        method.Coeffs,
                                        method.PCount,
                                        method.XMLString);
                                    break;
                            }

                            break;

                        case FixedMathGroups.ComplexEquation:
                            switch (method.Code2)
                            {
                                case ComplexEquation.EmbeddedEquation:
                                case ComplexEquation.LagEquation:
                                case ComplexEquation.NowEquation:
                                case ComplexEquation.PopulationEquation:
                                    group.Methods.AddMethod(
                                        method.TextCode,
                                        method.Code2.Value,
                                        method.TextName,
                                        method.Descr,
                                        DecompressFile(method.ImageFile),
                                        method.Coeffs,
                                        method.PCount,
                                        method.XMLString);
                                break;
                            }

                            break;
                        case FixedMathGroups.MultiRegression:
                            switch (method.Code2)
                            {
                                case MultiRegression.SimpleRegression:
                                    group.Methods.AddMethod(
                                        method.TextCode,
                                        method.Code2.Value,
                                        method.TextName,
                                        method.Descr,
                                        DecompressFile(method.ImageFile),
                                        method.Coeffs,
                                        method.PCount,
                                        method.XMLString);
                                break;
                            }

                        break;
                        case FixedMathGroups.PCAForecast:
                            ///////// not implemented!!!!!!!!!!!!!!!
                            break;
                    }
                }
            }

            return;
        }

        private static byte[] DecompressFile(byte[] compressedFileBuffer)
        {
            if ((compressedFileBuffer != null) && (compressedFileBuffer.Length > 0))
            {
                MemoryStream ms = null;

                DeflateStream decompressedzipStream = null;
                try
                {
                    ms = new MemoryStream(compressedFileBuffer);
                    ms.Write(compressedFileBuffer, 0, compressedFileBuffer.Length);
                    decompressedzipStream = new DeflateStream(ms, CompressionMode.Decompress, true);
                    ms.Position = 0;
                    List<byte> list = new List<byte>();

                    ////Используем этот метод т.к. поток DefalteStream не поддерживает свойство опеределения длинны потока.
                    ReadAllBytesFromStream(decompressedzipStream, list);
                    return list.ToArray();
                }
                finally
                {
                    decompressedzipStream.Close();
                }
            }
            else
            {
                return compressedFileBuffer;
            }
        }

        private static int ReadAllBytesFromStream(Stream stream, List<byte> list)
        {
            try
            {
                int offset = 0;
                int totalCount = 0;
                while (true)
                {
                    int byteRead = stream.ReadByte();

                    if (byteRead == -1)
                    {
                        break;
                    }

                    list.Add((byte)byteRead);
                    offset++;
                    totalCount++;
                }

                return totalCount;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
