using System;
using System.Collections.Generic;
using System.Globalization;

namespace Krista.FM.Server.FMService
{
    public class InstallParametrs
    {
        #region Поля

        /// <summary>
        /// Имя файла параметров.
        /// </summary>
		public const string ParametersFileName = "ServiceInstallerParameters.txt";

        /// <summary>
        /// Единственный экземпляр класса
        /// </summary>
        private static InstallParametrs serviceInstallParametrs;

        /// <summary>
        /// Путь к файлу с параметрами
        /// </summary>
        private static string filePath;

        /// <summary>
        /// Имя службы
        /// </summary>
        private string serviceName;

        /// <summary>
        /// Зависимости
        /// </summary>
        private string[] serviceDependedOn;

        #endregion Поля

        #region  Свойства

        /// <summary>
        /// Имя службы (только чтение)
        /// </summary>
        public string ServiceName
        {
            get { return serviceName; }
        }

        /// <summary>
        /// Зависимости (только чтение)
        /// </summary>
        public string[] GetServicesDependedOn()
        {
            return serviceDependedOn;
        }

        #endregion Свойства

        #region Конструктор

        /// <summary>
        /// Конструктор
        /// </summary>
        private InstallParametrs()
        {
            Initialize();
        }

        #endregion Конструктор

        #region Методы

		/// <summary>
		/// Устанавливает путь к файлу с параметрами
		/// </summary>
		public static void SetParametersBaseDirectory(string baseDirectory)
		{
			filePath = System.IO.Path.Combine(baseDirectory, ParametersFileName);
		}

		/// <summary>
        /// Инициализация параметров запуска службы
        /// </summary>
        private void Initialize()
        {
            try
            {
                Dictionary<string, object> parameters = ReadInstallerParameters(filePath);

                if (parameters.ContainsKey("ServiceName"))
                {
                    serviceName = Convert.ToString(parameters["ServiceName"], CultureInfo.InvariantCulture);
                }

                if (parameters.ContainsKey("ServicesDependedOn"))
                {
					string parameter = Convert.ToString(parameters["ServicesDependedOn"], CultureInfo.InvariantCulture);

                    serviceDependedOn = parameter.Split(',');
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("InitializeInstaller(): " + e.Message);
            	throw;
            }
        }

        /// <summary>
        /// Единственный экземпляр класса
        /// </summary>
        /// <returns></returns>
        public static InstallParametrs Instance()
        {
			if (serviceInstallParametrs == null)
			{
				// путь к файлу с параметрами запуска
				if (String.IsNullOrEmpty(filePath))
				{
					// сюда попадаем только при инсталяции\деинсталяции службы
					SetParametersBaseDirectory(Environment.CurrentDirectory);
				}

				serviceInstallParametrs = new InstallParametrs();
			}

        	return serviceInstallParametrs;
        }

        /// <summary>
        /// Параметры из файла ServiceInstallerParameters.txt
        /// </summary>
        /// <param name="fileName">Полный путь к файлу параметров</param>
        /// <returns>Коллекция параметров (имя + зависимости)</returns>
        private static Dictionary<string, object> ReadInstallerParameters(string fileName)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            try
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(fileName))
                {
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] lineParts = line.Split('=');
                        parameters.Add(lineParts[0], lineParts[1]);
                        Console.WriteLine(line);
                    }
                }
                return parameters;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        #endregion Методы
    }
}
