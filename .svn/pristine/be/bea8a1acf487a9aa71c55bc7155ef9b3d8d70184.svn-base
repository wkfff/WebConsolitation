using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using SharpSvn;

namespace Krista.FM.Utils.Common
{
    /// <summary>
    /// Операция по слиянию FMMD_All
    /// </summary>
    public class JoinOperation : BaseOperation
    {
        private const string configurationName = "OlapConfiguration.xml";

        /// <summary>
        /// Каталог сохранения
        /// </summary>
        private string destPackage;
        /// <summary>
        /// Путь к файлам с кубами и измерениями.
        /// </summary>
        private string sourcePackage;
        /// <summary>
        /// Каталог с конфигурацией. Если не задан, создаем без ее использования
        /// </summary>
        private readonly string configPakage;
        /// <summary>
        /// Пользовательский набор кубов
        /// </summary>
        private readonly string cubeNames;
        /// <summary>
        /// Пользовательский набор измерений
        /// </summary>
        private readonly string dimensionNames;
        /// <summary>
        /// Признак - восстанавливать измерения без подцепки кубов
        /// </summary>
        private readonly bool excludeCubes;
        /// <summary>
        /// Список партиций для удаления
        /// </summary>
        private readonly List<string> deletePartitionList = new List<string>();
        /// <summary>
        /// Список нужных вычислений
        /// </summary>
        private readonly List<string> updateCalculations = new List<string>();

        public JoinOperation(string configPakage, string sourcePackage, string destPackage, string cubeNames, string dimensionNames, bool excludeCubes)
        {
            this.destPackage = SplitHelperClass.CheckPath(destPackage);
            this.sourcePackage = SplitHelperClass.CheckPath(sourcePackage);
            this.cubeNames = cubeNames;
            this.dimensionNames = dimensionNames;
            this.excludeCubes = excludeCubes;
            this.configPakage = configPakage;
        }

        public void SaveDocument(XDocument document)
        {
            if (String.IsNullOrEmpty(destPackage))
                destPackage = AppDomain.CurrentDomain.BaseDirectory;

            if (!Directory.Exists(destPackage))
                Directory.CreateDirectory(destPackage);

            document.Save(string.Format("{0}\\FMMD_All.xml", destPackage));

            SavePackageFiles();
            CopyUpdateCalculation();
        }
        
        public XDocument HeaderLoad()
        {
            try
            {
                XDocument headerDocument = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"windows-1251\"?><XMLDSOConverter version=\"1.0\"><CustomProperties><Property name=\"RESERVED_RESTORE_IN_PROGRESS\" datatype=\"11\" /></CustomProperties><Databases><Database method=\"None\" name=\"/*#DatabaseName#*/\" ClassType=\"2\" SubClassType=\"0\" Description=\"\" OlapMode=\"0\" LastUpdated=\"12:00:00 AM\" IsVisible=\"true\"><DatabaseDimensions /><DatabaseCommands /><Cubes /></Database></Databases></XMLDSOConverter>");

                return headerDocument;

            }
            catch (Exception e)
            {
                throw new JoinException(e.Message);
            }
        }

        /// <summary>
        /// Загрузка конфигурации многомерных объектов
        /// </summary>
        /// <returns></returns>
        public XDocument ConfigurationLoad()
        {
            try
            {
                return
                    XDocument.Load(String.Format("{0}\\{1}", configPakage, configurationName));
            }
            catch (ArgumentNullException e)
            {
                throw new OpenFMMDAllExeption(string.Format("Не задан путь к {0}: {1}", configurationName, e.Message));
            }
            catch (FileNotFoundException e)
            {
                throw new OpenFMMDAllExeption(string.Format("Не найден {0}: {1}", configurationName, e.Message));
            }
        }

        public XDocument JoinDocument()
        {
            try
            {
                if (!JoinHelperClass.CheckDirectories(sourcePackage))
                    throw new JoinException("Не найдены директории DatabaseDimensions и Cubes");

                XDocument document = HeaderLoad();
                
                XElement databaseDimensionsNode =
                    (from el in document.Descendants("DatabaseDimensions") select el).First();

                XElement databaseCubesNode =
                    (from el in document.Descendants("Cubes") select el).First();

                List<string> cubes = GetCubes();
                List<string> databaseDemensions = GetDatabaseDimensions(cubes);

                AddUpdateCalculations(cubes);
                AddDatabaseDimensions(databaseDemensions, databaseDimensionsNode);
                AddCubes(cubes, databaseCubesNode);

                return document;
            }
            catch (OpenFMMDAllExeption e)
            {
                throw new JoinException(e.Message);
            }
        }
       
        private void SavePackageFiles()
        {
            ReplacesSave();
            DatasourceSave();
            DeletePartitionsSave();
            PackageSave();
        }

        #region Получение кубов и измерений для создания Fmmd_all

        private List<string> GetCubes()
        {
            var cubes = new List<string>();

            try
            {
                if (!String.IsNullOrEmpty(configPakage))
                    GetCubesByConfig(cubes);

                if (!String.IsNullOrEmpty(cubeNames))
                    GetCubesByCubesNames(cubes);

                if (!excludeCubes)
                {
                    if (!String.IsNullOrEmpty(dimensionNames))
                        GetCubesByDimensionNames(cubes);
                }
            }
            catch (Exception e)
            {
                throw new JoinException(e.Message);
            }

            return cubes;
        }

        private void GetCubesByDimensionNames(List<string> cubes)
        {
            var names = dimensionNames.Split(';');
            foreach (XDocument cubeDoc in from name in names
                                          from file in Directory.GetFiles(string.Format("{0}\\Cubes", sourcePackage))
                                          let cubeDoc = XDocument.Load(file)
                                          where cubeDoc.Descendants("CubeDimension").Where(descendant => descendant.Attribute("name").Value == name && !cubes.Contains(cubeDoc.Descendants("Cube").First().Attribute("name").Value)).Count() > 0
                                          select cubeDoc)
            {
                cubes.Add(cubeDoc.Descendants("Cube").First().Attribute("name").Value);
            }
        }

        private void GetCubesByCubesNames(List<string> cubes)
        {
            var names = cubeNames.Split(';');
            foreach (var name in names)
            {
                AddObjectToList(cubes, name, "Cubes");
            }
        }

        private void GetCubesByConfig(List<string> cubes)
        {
            XDocument configuration = ConfigurationLoad();

            IEnumerable<XElement> databaseCubes =
                (from el in configuration.Descendants("Items") select el);

            foreach (var items in from databaseCube in databaseCubes
                                  select (from el in databaseCube.Descendants("Item") select el)
                                  into items from xElement in items select items)
            {
                foreach (var xElement in items)
                {
                    string xmlPath = JoinHelperClass.GetXmlPath(string.Format("{0}OLAPPackages\\", sourcePackage), xElement);

                    if (!File.Exists(xmlPath))
                    {
                        Console.WriteLine("Файла не существует: {0}\n", xmlPath);
                        continue;
                    }

                    if (xmlPath != null) JoinHelperClass.AddCubesFromXmlPackage(cubes, xmlPath);
                }
            }
        }

        private void AddUpdateCalculations(List<string> cubes)
        {
            if (!Directory.Exists(string.Format("{0}\\Cubes\\Calculations", sourcePackage)))
            {
                Console.WriteLine(string.Format("Не найдена директория с вычислениями: {0}",
                                                string.Format("{0}\\Cubes\\Calculations", sourcePackage)));
                return;
            }

            string[] updateCalculationNames = Directory.GetFiles(string.Format("{0}\\Cubes\\Calculations", sourcePackage));
            GetUpdateCalculatons(cubes, updateCalculationNames);
        }

        /// <summary>
        /// Получает список нужных вычислений
        /// </summary>
        /// <param name="cubes"></param>
        /// <param name="updateCalculationNames"></param>
        private void GetUpdateCalculatons(List<string> cubes, string[] updateCalculationNames)
        {
            foreach (var updateCalculationName in from cube in cubes
                                                  from updateCalculationName in updateCalculationNames
                                                  where updateCalculationName.Contains(cube)
                                                  select updateCalculationName)
            {
                if (!updateCalculations.Contains(updateCalculationName))
                    updateCalculations.Add(updateCalculationName);
            }
        }

        private void AddObjectToList(List<string> list, string name, string folderName)
        {
            try
            {
                if (name[name.Length - 1] == '*')
                {
                    string[] files =
                        Directory.GetFiles(sourcePackage + folderName, name);

                    foreach (var file in files)
                    {
                        string c = file.Split('\\')[file.Split('\\').Length - 1].Split('.')[0];
                        if (!list.Contains(c))
                            list.Add(c);    
                    }
                }
                else
                {
                    if (!list.Contains(name))
                        list.Add(name);    
                }
            }
            catch (Exception e)
            {
                throw new JoinException(e.Message);
            }
        }

        /// <summary>
        /// Формируем список измерений по набору кубов
        /// </summary>
        /// <param name="cubes"></param>
        /// <returns></returns>
        private List<string> GetDatabaseDimensions(List<string> cubes)
        {
            var databaseDimensions = new List<string>();

            try
            {
                foreach (var cube in cubes)
                {
                    if (File.Exists(String.Format("{0}Cubes\\{1}.xml", sourcePackage, cube)))
                    {
                        XDocument cubeDocument = XDocument.Load(String.Format("{0}Cubes\\{1}.xml", sourcePackage, cube));

                        if (JoinHelperClass.CheckPartition(cubeDocument.Descendants("Cube").First()))
                            deletePartitionList.Add(cubeDocument.Descendants("Cube").First().Attribute("name").Value);

                        IEnumerable<XElement> cubeDimensions = cubeDocument.Descendants("CubeDimension");
                        foreach (var cubeDimension in cubeDimensions.Where
                            (cubeDimension => !databaseDimensions.Contains(cubeDimension.Attribute("name").Value)))
                        {
                            if (!databaseDimensions.Contains(cubeDimension.Attribute("name").Value))
                                databaseDimensions.Add(cubeDimension.Attribute("name").Value);
                        }
                    }
                }

                if (!String.IsNullOrEmpty(dimensionNames))
                    AddDimensionsByDimNames(databaseDimensions);

            }
            catch (Exception e)
            {
                throw new JoinException(e.Message);
            }

            return databaseDimensions;
        }

        private void AddDimensionsByDimNames(List<string> databaseDimensions)
        {
            if (dimensionNames.Equals("*"))
            {
                foreach (var file in Directory.GetFiles(string.Format("{0}\\DatabaseDimensions", sourcePackage)))
                {
                    string dimName = file.Split('\\')[file.Split('\\').Length - 1].Substring(0,
                                                                                             file.Split('\\')[
                                                                                                 file.Split('\\').Length -
                                                                                                 1].Length - 4);
                    if (!databaseDimensions.Contains(dimName))
                        databaseDimensions.Add(dimName);
                }
            }
            else
            {
                var names = dimensionNames.Split(';');
                foreach (var name in names)
                {
                    AddObjectToList(databaseDimensions, name, "DatabaseDimensions");
                }
            }
        }

        #endregion


        private void AddCubes(List<string> cubes, XElement databaseCubesNode)
        {
            foreach (var databaseCube in cubes)
            {
                AddCube(databaseCube, databaseCubesNode);
            }
        }

        private void AddCube(string cubeName, XElement databaseCubesNode)
        {
            try
            {
                string cubeFullName = String.Format("{0}Cubes\\{1}.xml", sourcePackage,
                                    cubeName);

                if (!File.Exists(cubeFullName))
                {
                    Trace.TraceWapning(
                        String.Format("Куб с именем {0} есть в Olap-конфигурации, но не найден в списке кубов", cubeName));
                    return;
                }

                string revision = JoinHelperClass.GetCubeRevision(cubeFullName);

                XDocument cubeDocument =
                        XDocument.Load(cubeFullName);
                SplitHelperClass.Validate(cubeDocument.Root, true);
                if (!String.IsNullOrEmpty(revision))
                {
                    XElement element =
                        XElement.Parse(
                            String.Format(@"<Property name=""Revision"" datatype=""8""><![CDATA[{0}]]></Property>",
                                          revision));
                    if (cubeDocument.Descendants("CustomProperties").First().Descendants("Property")
                        .Where(el => el.Attribute("name").Value == "Revision").Count() == 0)
                    {
                        cubeDocument.Descendants("CustomProperties").First().Add(element);
                    }
                    else
                    {
                        cubeDocument.Descendants("CustomProperties").First().Descendants("Property").Where(
                            el => el.Attribute("name").Value == "Revision").First().Value = revision;
                    }
                }

                Console.WriteLine(String.Format("Добавляем куб - {0}", cubeName));

                databaseCubesNode.Add(cubeDocument.Element("Cube"));
            }
            catch (Exception e)
            {
                throw new JoinException(e.Message);
            }
        }

        private void AddDatabaseDimensions(List<string> databaseDimensions, XElement databaseDimensionsNode)
        {
            
            foreach (var databaseDimension in databaseDimensions)
            {
                AddDatabaseDimension(databaseDimension, databaseDimensionsNode);
            }
        }

        private void AddDatabaseDimension(string databaseDimensionName, XElement databaseDimensionsNode)
        {
            try
            {
                XDocument dimDocument =
                    XDocument.Load(String.Format("{0}DatabaseDimensions\\{1}.xml", sourcePackage,
                                                 databaseDimensionName));
                SplitHelperClass.Validate(dimDocument.Root, true);

                Console.WriteLine(String.Format("Добавляем измерение - {0}", databaseDimensionName));

                databaseDimensionsNode.Add(dimDocument.Element("DatabaseDimension"));
            }
            catch (Exception e)
            {
                throw new JoinException(string.Format("{0}: {1}", e.Message, databaseDimensionName));
            }
        }

        #region MAIN

        public override bool Execute()
        {
            try
            {
                Console.WriteLine("Считывание аргументов...\n");
                Console.WriteLine(String.Format("Каталог с файлом конфигурации: {0}\n", configPakage));
                Console.WriteLine((String.IsNullOrEmpty(configPakage))
                                      ? String.Format("Каталог с файлом конфигурации: {0}\n", configPakage)
                                      : "Создаем без файла конфигурации");
                Console.WriteLine(String.Format("Каталог с файлами: {0}\n", sourcePackage));
                Console.WriteLine(String.Format("Каталог сохранения: {0}\n", destPackage));
                Console.WriteLine(String.Format("Список кубов заданный параметром: {0}\n", cubeNames));
                Console.WriteLine(String.Format("Список измерений заданный параметром: {0}\n", dimensionNames));
                Console.WriteLine(String.Format("Включать к измерениям кубы: {0}\n", excludeCubes));

                Console.WriteLine("Создание FMMD_All.xml\n");
                XDocument document = JoinDocument();
                SaveDocument(document);
                Console.WriteLine("FMMD_All.xml создан\n");
            }
            catch (JoinException e)
            {
                Trace.TraceError(e.Message);
                return false;
            }
            catch(OpenFMMDAllExeption e)
            {
                Trace.TraceError(e.Message);
                return false;
            }

            Trace.TraceSuccess("Создание FMMD_All.xml завершилось без ошибок.");
            return true;
        }

        #endregion

        #region Сохранение файлов, входящих в артефакт

        public void ReplacesSave()
        {
            try
            {
                Console.WriteLine("Сохранение FMMD_Replaces.xml");

                XDocument replacesDocument = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"windows-1251\"?><XMLDSOConverter version=\"1.0\"><replacelist><replace find=\"_win1251\" replace=\"\" case=\"1\"/></replacelist></XMLDSOConverter>");

                replacesDocument.Save(string.Format("{0}\\FMMD_Replaces.xml", destPackage));
            }
            catch (Exception e)
            {
                throw new JoinException(e.Message);
            }
        }

        public void DatasourceSave()
        {
            try
            {
                Console.WriteLine("Сохранение FMMD_Datasource.xml");

                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                string versionStr = String.Format("{0}.{1}.0.0", version.Major, version.Minor);

                Console.WriteLine(String.Format("Базовая версия - {0}", versionStr));

                XDocument datasourceDocument = XDocument.Parse(
                    String.Format(@"<?xml version=""1.0"" encoding=""windows-1251""?> 
                    <XMLDSOConverter version=""1.0"">
	                    <CustomProperties>
		                    <Property name=""RESERVED_RESTORE_IN_PROGRESS"" datatype=""11""><![CDATA[0]]>
		                    </Property>
	                    </CustomProperties>
	                <Databases>
		                <Database method=""Add"" name=""/*#DatabaseName#*/"" ClassType=""2"" SubClassType=""0"" Description="""" OlapMode=""0"" LastUpdated=""12:00:00 AM"" IsVisible=""true"">
			                <CustomProperties>
				                <Property name=""RESERVED_DATABASE_LOCALE_UPDATED2"" datatype=""0""><![CDATA[B]]>
				                </Property>
				                <Property name=""Версия"" datatype=""8""><![CDATA[{0}]]>
				                </Property>
				                <Property name=""Регион"" datatype=""8""><![CDATA[/*#Регион#*/]]>
				                </Property>
			                </CustomProperties>
			                <DataSources>
				                <DataSource method=""Add"" name=""dv"" ClassType=""6"" SubClassType=""0"" Description="""">
					                <property name=""ConnectionString""><![CDATA[/*#ConnectionString#*/]]>
					                </property>
				                </DataSource>
			                </DataSources>
		                </Database>
	                </Databases>
                </XMLDSOConverter>", versionStr));

                datasourceDocument.Save(string.Format("{0}\\FMMD_Datasource.xml", destPackage));

            }
            catch (Exception e)
            {
                throw new JoinException(e.Message);
            }
        }

        public void DeletePartitionsSave()
        {
            try
            {
                if (deletePartitionList.Count == 0)
                {
                    Console.WriteLine("НЕ сохраняем FMMD_Delete_Partitions.xml. Нет кубов, делящихся на партиции");
                    return;
                }

                Console.WriteLine("Сохранение FMMD_Delete_Partitions.xml");
                
                XDocument deletePartitionsDocument = XDocument.Parse(
                    @"<?xml version=""1.0"" encoding=""windows-1251""?>
                    <XMLDSOConverter version=""1.0"">
	                    <Databases>
		                    <Database method=""none"" name=""/*#DatabaseName#*/"" ClassType=""2"" SubClassType=""0"">
			                <Cubes>
			                </Cubes>
		                </Database>
	                </Databases>
                </XMLDSOConverter>");

                foreach (var deletePartition in deletePartitionList)
                {
                    XElement cubesNode = deletePartitionsDocument.Descendants("Cubes").First();
                    XElement element = XElement.Parse(
                        @"<Cube method=""none"" name="""" ClassType=""9"" SubClassType=""0"">
					        <Partitions>
						        <Partition method=""Remove"" name="""" ClassType=""19"" SubClassType=""0""/>
					        </Partitions>
				        </Cube>");

                    element.Attribute("name").Value = deletePartition;
                    element.Descendants("Partition").First().Attribute("name").Value = deletePartition;

                    cubesNode.Add(element);
                }

                deletePartitionsDocument.Save(string.Format("{0}\\FMMD_Delete_Partitions.xml", destPackage));

            }
            catch (Exception e)
            {
                throw new JoinException(e.Message);
            }
        }

        public void PackageSave()
        {
            try
            {
                XDocument packageDocument = null;

                Console.WriteLine("Сохранение FMMD_Package.xml");

                if (deletePartitionList.Count != 0)
                {
                    packageDocument = XDocument.Parse(
                        @"<?xml version=""1.0"" encoding=""windows-1251""?>
                        <XMLDSOConverter version=""1.0"">
	                        <package>
		                        <macros file=""FMMD_Macros.xml""/>
		                        <replaces file=""FMMD_Replaces.xml""/>
		                        <packets>
			                        <packet order=""1"" file=""FMMD_Datasource.xml""/>
			                        <packet order=""2"" file=""FMMD_All.xml""/>
 	  	                            <packet order=""3"" file=""FMMD_Delete_Partitions.xml""/>
		                        </packets>
	                        </package>
                        </XMLDSOConverter>");
                }
                else
                {
                    packageDocument = XDocument.Parse(
                        @"<?xml version=""1.0"" encoding=""windows-1251""?>
                        <XMLDSOConverter version=""1.0"">
	                        <package>
		                        <macros file=""FMMD_Macros.xml""/>
		                        <replaces file=""FMMD_Replaces.xml""/>
		                        <packets>
			                        <packet order=""1"" file=""FMMD_Datasource.xml""/>
			                        <packet order=""2"" file=""FMMD_All.xml""/>
		                        </packets>
	                        </package>
                        </XMLDSOConverter>");
                }

                packageDocument.Save(string.Format("{0}\\FMMD_Package.xml", destPackage));
            }
            catch (Exception e)
            {
                throw new JoinException(e.Message);
            }
        }

        #endregion

        #region Работа с UpdateCalculations

        private void CopyUpdateCalculation()
        {
            try
            {
                DeleteDirectory();

                Directory.CreateDirectory(string.Format("{0}\\UpdateCalculations", destPackage));
                foreach (var updateCalculation in updateCalculations)
                {
                    string fromFile = Path.GetFileName(updateCalculation);
                    File.Copy(updateCalculation, string.Format("{0}\\UpdateCalculations\\{1}", destPackage, fromFile));
                }
            }
            catch (Exception e)
            {
                throw new JoinException(e.Message);
            }
        }

        /// <summary>
        /// Удаляем существующую директорию UpdateCalculations
        /// </summary>
        private void DeleteDirectory()
        {
            string pathDirectory = string.Format("{0}\\UpdateCalculations", destPackage);
            if (Directory.Exists(pathDirectory))
            {
                try
                {
                    JoinHelperClass.ClearAttributes(pathDirectory);
                    Directory.Delete(pathDirectory, true);
                }
                catch (IOException e)
                {
                    throw new JoinException(e.Message);
                }
            }
        }

        #endregion
    }
}
