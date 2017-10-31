using System;
using Krista.FM.Utils.Common;
using NConsoler;

namespace Krista.FM.Utils.AnalysisFMMD_All
{
    class Program
    {
        public static bool flag;

        static int Main(string[] args)
        {
            Consolery.Run(typeof(Program), args);
            return flag ? 1 : 0;
        }

        [Action("Split FMMD_All\n")]
        public static void Split(
            [Optional("", "sourcePackage", AltNames = new[] { "sp" }, Description = "Полный путь к разбиваемому файлу FMMD_All. Если параметр не задан, ищем в текущей папке.")]
            string sourcePackage,

            [Optional("", "destPackage", AltNames = new[] { "dp" }, Description = "Каталог сохраненя измерений и кубов. Если параметр не задан, создаем в текущей папке.")]
		    string destPackage)
        {

            BaseOperation operation = new SplitOperation(sourcePackage, destPackage);
            if (!operation.Execute())
                flag = true;
        }

        [Action("Join FMMD_All\n")]
        public static void Join(

            [Optional("", "configPackage", AltNames = new[] { "cp" }, Description = "Путь к файлу конфирурации многомерной базы. Если параметр не задан, создаем без файла конфигурации.")]
		    string configPakage,

            [Optional("", "sourcePackage", AltNames = new[] { "sp" }, Description = "Путь к файлам с кубами и измерениями. Если параметр не задан, ищем в текущей папке.")]
            string sourcePackage,

            [Optional("", "destPackage", AltNames = new[] { "dp" }, Description = "Каталог сохраненя FMMD_All и UpdateCalculations. Если параметр не задан, создаем в текущей папке.")]
		    string destPackage,
            
            [Optional("", "c", Description = "Список кубов для восстановления. Востанавливаются с измерениями")]
		    string cubes,

            [Optional("", "d", Description = "Список измерений для восстановления. * - восстановить все измерения")]
		    string dimensions,

            [Optional(false, "excludeCube", AltNames = new[] {"ec"}, Description = "Создать указанные измерения без кубов. Если параметр не указан, создаем с кубами.")]
		    bool excludeCubes)
        {
            BaseOperation operation = new JoinOperation(configPakage, sourcePackage, destPackage, cubes, dimensions, excludeCubes);
            if (!operation.Execute())
                flag = true;
        }
    }
}
