using System;
using Krista.FM.Utils.MakeConfigurationLibrary;

namespace Krista.FM.Utils.MakeConfiguration
{
    /// <summary>
    /// Создает файл конфигуорации по полному описанию пакетов с кубами
    /// Аргументы:
    ///     -path - путь к папке OLAPPackages
    /// </summary>
    class Program
    {
        private static string path;

        static void Main(string[] args)
        {
            GetArguments(args);

            ConfigurationManager configurationManager = new ConfigurationManager(path);

            Console.WriteLine(configurationManager.MakeConfiguration(AppDomain.CurrentDomain.BaseDirectory, configurationManager.GetRootPackage())
                                  ? "Создание конфигурационного файла завершено УСПЕШНО."
                                  : "Создание конфигурационного файла завершено С ОШИБКОЙ.");

            Console.ReadLine();
        }

        private static void GetArguments(string[] args)
        {
            if (args.Length == 0)
                path = AppDomain.CurrentDomain.BaseDirectory;
        }

        
    }
}
