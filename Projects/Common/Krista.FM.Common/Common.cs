using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Lifetime;
using System.Runtime.Remoting.Messaging;
using System.Security.Principal;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.InteropServices;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Common
{
    #region Работа с серверным логом
    /// <summary>
    /// Класс для поддержки серверного лога
    /// </summary>
    public class ListenerContainer
    {
        /// <summary>
        /// Объект для ведения серверного лога
        /// </summary>
        public static System.Diagnostics.TextWriterTraceListener textWriter;

        static ListenerContainer()
        {
            string str = System.Configuration.ConfigurationManager.AppSettings["ServerLog"];
            textWriter = new System.Diagnostics.TextWriterTraceListener("ServerLog.log", "ServerLog");
        }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        public ListenerContainer()
        {
        }
    }
    #endregion

    #region Работа со строками
    /// <summary>
    /// Вспомогательный класс для работы со строками
    /// </summary>
    public class StrUtils
	{
		/// <summary>
		/// Конструктор класса
		/// </summary>
        public StrUtils()
		{
		}

		/// <summary>
		/// Преобразовать строку в булевское значение (регистронечувствительно)
		/// </summary>
		/// <param name="str">"True" или "False"</param>
		/// <returns></returns>
        public static bool StringToBool(string str)
		{
			string upperString = str.ToUpper();
			if (upperString == "TRUE")
				return true;
			else if (upperString == "FALSE")
				return false;
			else
				throw new ArgumentException("Can't convert " + str + " to boolean type.");
		}
    }
    #endregion

    #region Классы для контроля версий системы

    /// <summary>
    /// Класс выполняющий сканирования директории и получение версий сборок
    /// Для внутреннего использования
    /// </summary>
    [Serializable]
    internal class AssemblyesScaner : MarshalByRefObject
    {
        /// <summary>
        /// Найти в директории сборки, удовлетворяющие условию поиска и получить их версии
        /// </summary>
        /// <param name="scanDir">Директория в которой осуществляется поиск</param>
        /// <param name="scanMask">Маска поиска</param>
        /// <returns>Названия и версии сборок</returns>
        public Dictionary<string, string> ScanAssemblyes(string scanDir, string scanMask)
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            // получаем все сборки из запрошенной директории, удовлетворяющие условию поиска
            string[] dlls = Directory.GetFiles(scanDir, scanMask);
            // каждую пытаемся загрузить и получить версию
            foreach (string dll in dlls)
            {
                try
                {
                    Assembly ass = Assembly.LoadFile(dll);
                    res.Add(ass.ManifestModule.Name, AppVersionControl.GetAssemblyVersion(ass));
                }
                catch { };
            }
            // возвращаем список названий и версий
            return res;
        }
    }
    
    /// <summary>
    /// Структура получения информации о версиях сборок системы
    /// </summary>
    public struct AppVersionControl
    {
        /// <summary>
        /// Название общей сборки с декларациями интерфейсов
        /// </summary>
        public const string ServerLibraryAssemblyName = "Krista.FM.ServerLibrary.dll";

        /// <summary>
        /// Маска поиска для серверных сборок
        /// </summary>
        public const string ServerAssemblyesSearchMaskDll = "Krista.FM.*.dll";
        public const string ServerAssemblyesSearchMaskExe = "Krista.FM.*.exe";

        /// <summary>
        /// Маска поиска для клиентских сборок
        /// </summary>
        public const string ClientAssemblyesSearchMaskDll = "Krista.FM.*.dll";
        public const string ClientAssemblyesSearchMaskExe = "Krista.FM.*.exe";

        /// <summary>
        /// Ключ командной строки отключающий контроль версий
        /// </summary>
        private const string IgnoreVersionsKey = "IgnoreVersions";

        /// <summary>
        /// Включен ли режим игнорирования версий сборок
        /// </summary>
        /// <returns></returns>
        public static bool IgnoreVersionsModeOn()
        {
            return CommandLineUtils.ParameterPresent(IgnoreVersionsKey);
        }

        /// <summary>
        /// Найти ServerLibrary в памяти или на диске
        /// </summary>
        /// <returns>ServerLibrary</returns>
        public static Assembly GetServerLibraryAssembly()
        {
            // ServerLibrary как правило всегда загружена, так что создавать новый домен нецелесообразно
            Assembly serverLibraryAssembly = null;
            // есть ли сборка в памяти?
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly ass in assemblies)
            {
                if (ass.ManifestModule.Name == ServerLibraryAssemblyName)
                {
                    serverLibraryAssembly = ass;
                    break;
                }
            }
            // есть ли сборка на диске?
            if (serverLibraryAssembly == null)
            {
                string[] assemblyFile = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, ServerLibraryAssemblyName);
                if ((assemblyFile != null) && (assemblyFile.Length != 0))
                {
                    serverLibraryAssembly = Assembly.LoadFile(assemblyFile[0]);
                }
            }
            // если сборки нигде не обнаружено - генерируем исключение
            if (serverLibraryAssembly == null)
                throw new Exception(String.Format("Сборка '{0}' не найдена", ServerLibraryAssemblyName));
            return serverLibraryAssembly;
        }

        /// <summary>
        /// Получить версию общей сборки с декларациями интерфейсов
        /// </summary>
        /// <returns>Версия</returns>
        public static string GetServerLibraryVersion()
        {
            return GetAssemblyVersion(GetServerLibraryAssembly());
        }

        /// <summary>
        /// Получить версию сборки
        /// </summary>
        /// <param name="ass">Сборка</param>
        /// <returns>Версия</returns>
        public static string GetAssemblyVersion(Assembly ass)
        {
            object[] attr = ass.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
            if (attr.Length > 0)
                return ((AssemblyFileVersionAttribute)attr[0]).Version;
            else
                return String.Empty;
        }

        /// <summary>
        /// Возвращает первые два разряда версии
        /// </summary>
        /// <param name="version">Полная версия сборки</param>
        /// <returns></returns>
        public static string GetAssemblyBaseVersion(string version)
        {
            string[] parts = version.Split('.');
            string[] result = new string[2];
            for (int i = 0; i < 2; i++)
            {
                result[i] = parts[i];
            }
            return string.Join(".", result);
        }

        /// <summary>
        /// Получить версии сборок из директории текущего домена приложения, удовлетворяющих условию поиска
        /// </summary>
        /// <param name="searchMask">Маска поиска</param>
        /// <returns>Список названий и версий сборок</returns>
        public static Dictionary<string, string> GetAssemblyesVersions(string searchMask)
        {
            // в целях предотвращения расходов памяти для загрузки сборок создаем новый домен
            AppDomain scanDomain = AppDomain.CreateDomain("ScanDomain");
            try
            {
                string curDir = AppDomain.CurrentDomain.BaseDirectory;
                string curAssemblyName = curDir + Assembly.GetExecutingAssembly().ManifestModule.Name;
                // получаем Proxy объекта, осуществляющего сканирование
                AssemblyesScaner sc = (AssemblyesScaner)scanDomain.CreateInstanceFromAndUnwrap(
                    curAssemblyName, 
                    "Krista.FM.Common.AssemblyesScaner");
                // получаем список названий и версий сборок
                return sc.ScanAssemblyes(curDir, searchMask);
            }
            finally
            {
                // выгружаем вспомогательный домен
                AppDomain.Unload(scanDomain);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseVesion"></param>
        /// <param name="assemblyFilter"></param>
        /// <param name="badAssemblies"></param>
        /// <param name="outputTrace"></param>
        public static void CheckAssemblyVersions(string baseVesion, string assemblyFilter, Dictionary<string, string> badAssemblies, bool outputTrace)
        {
            Dictionary<string, string> versions = AppVersionControl.GetAssemblyesVersions(assemblyFilter);
            int maxLength = 0;
            foreach (string itemKey in versions.Keys)
            {
                if (itemKey.Length > maxLength)
                    maxLength = itemKey.Length;
            }

            string formatString = String.Format("{{0,-{0}}} {{1}} {{2}}", maxLength);
            foreach (KeyValuePair<string, string> item in versions)
            {
                string baseAssemblyVesion = AppVersionControl.GetAssemblyBaseVersion(item.Value);
                string badSign = String.Empty;
                if (baseAssemblyVesion != baseVesion)
                {
                    badAssemblies.Add(item.Key, item.Value);
                    badSign = "Версия не соответствует базовой";
                }
                if (outputTrace)
                    Trace.WriteLine(String.Format(formatString, item.Key, item.Value, badSign));
            }
        }

        #region Unused
        /*
        /// <summary>
        /// Сравнить версии серверных и клиентских сборок, если есть несоответсвие - сформировать предупреждение
        /// </summary>
        public static bool CheckIntegrity(Dictionary<string, string> serverAssemblyes, Dictionary<string, string> clientAssemblyes,
           out StringBuilder errors)
        {
            errors = null;
            Dictionary<string, int> versionCounters = new Dictionary<string, int>();
            // считаем количество версий в серверной части
            foreach (string assName in serverAssemblyes.Keys)
            {
                string curVersion = serverAssemblyes[assName];
                if (versionCounters.ContainsKey(curVersion))
                    versionCounters[curVersion]++;
                else
                    versionCounters.Add(curVersion, 0);
            };
            // считаем количество версий в клиентской части
            foreach (string assName in clientAssemblyes.Keys)
            {
                string curVersion = clientAssemblyes[assName];
                if (versionCounters.ContainsKey(curVersion))
                    versionCounters[curVersion]++;
                else
                    versionCounters.Add(curVersion, 0);
            }
            // если версия одна - возвращаем true и выходим
            if (versionCounters.Count == 1)
                return true;

            // определяем превалирующую версию
            int maxNumber = 0;
            string mainVersion = String.Empty;
            foreach (string version in versionCounters.Keys)
            {
                if (versionCounters[version] > maxNumber)
                {
                    maxNumber = versionCounters[version];
                    mainVersion = version;
                }
            }
            // формируем список неправильных сборок
            errors = new StringBuilder();
            errors.Append("Основная версия: ");
            errors.Append(mainVersion);
            errors.AppendLine(); errors.AppendLine();
            errors.AppendLine("Отличающиеся модули серверной части:");
            int appended = 0;
            foreach (string assName in serverAssemblyes.Keys)
            {
                if (serverAssemblyes[assName] != mainVersion)
                {
                    errors.AppendLine(String.Concat(assName, ": ", serverAssemblyes[assName]));
                    appended++;
                }
            }
            if (appended == 0)
                errors.AppendLine("<Нет>");

            errors.AppendLine();

            appended = 0;
            errors.AppendLine("Отличающиеся модули клиентской части:");
            foreach (string assName in clientAssemblyes.Keys)
            {
                if (clientAssemblyes[assName] != mainVersion)
                {
                    errors.AppendLine(String.Concat(assName, ": ", clientAssemblyes[assName]));
                    appended++;
                }
            }
            if (appended == 0)
                errors.AppendLine("<Нет>");

            return false;
        }*/

        /*
        public static string FindMainVersion(Dictionary<string, string> serverAssemblyes, Dictionary<string, string> clientAssemblyes)
        {
            Dictionary<string, int> versionCounters = new Dictionary<string, int>();
            // считаем количество версий в серверной части
            foreach (string assName in serverAssemblyes.Keys)
            {
                string curVersion = serverAssemblyes[assName];
                if (versionCounters.ContainsKey(curVersion))
                    versionCounters[curVersion]++;
                else
                    versionCounters.Add(curVersion, 0);
            };
            // считаем количество версий в клиентской части
            foreach (string assName in clientAssemblyes.Keys)
            {
                string curVersion = clientAssemblyes[assName];
                if (versionCounters.ContainsKey(curVersion))
                    versionCounters[curVersion]++;
                else
                    versionCounters.Add(curVersion, 0);
            }
            // определяем превалирующую версию
            int maxNumber = 0;
            string mainVersion = String.Empty;
            foreach (string version in versionCounters.Keys)
            {
                if (versionCounters[version] > maxNumber)
                {
                    maxNumber = versionCounters[version];
                    mainVersion = version;
                }
            }
            return mainVersion;
        }
        */
        #endregion
    }
    #endregion

    #region Вспомогательнвый класс для вычисления контрольных сумм (CRC)
    /// <summary>
    /// Вспомогательнвый класс для вычисления контрольных сумм (CRC)
    /// </summary>
    public class CRCHelper
	{
		internal static readonly uint[] CRC_Table = new uint[]
		{
			0x00000000u,0x77073096u,0xee0e612cu,0x990951bau,0x076dc419u,0x706af48fu,0xe963a535u,0x9e6495a3u,
			0x0edb8832u,0x79dcb8a4u,0xe0d5e91eu,0x97d2d988u,0x09b64c2bu,0x7eb17cbdu,0xe7b82d07u,0x90bf1d91u,
			0x1db71064u,0x6ab020f2u,0xf3b97148u,0x84be41deu,0x1adad47du,0x6ddde4ebu,0xf4d4b551u,0x83d385c7u,
			0x136c9856u,0x646ba8c0u,0xfd62f97au,0x8a65c9ecu,0x14015c4fu,0x63066cd9u,0xfa0f3d63u,0x8d080df5u,
			0x3b6e20c8u,0x4c69105eu,0xd56041e4u,0xa2677172u,0x3c03e4d1u,0x4b04d447u,0xd20d85fdu,0xa50ab56bu,
			0x35b5a8fau,0x42b2986cu,0xdbbbc9d6u,0xacbcf940u,0x32d86ce3u,0x45df5c75u,0xdcd60dcfu,0xabd13d59u,
			0x26d930acu,0x51de003au,0xc8d75180u,0xbfd06116u,0x21b4f4b5u,0x56b3c423u,0xcfba9599u,0xb8bda50fu,
			0x2802b89eu,0x5f058808u,0xc60cd9b2u,0xb10be924u,0x2f6f7c87u,0x58684c11u,0xc1611dabu,0xb6662d3du,
			0x76dc4190u,0x01db7106u,0x98d220bcu,0xefd5102au,0x71b18589u,0x06b6b51fu,0x9fbfe4a5u,0xe8b8d433u,
			0x7807c9a2u,0x0f00f934u,0x9609a88eu,0xe10e9818u,0x7f6a0dbbu,0x086d3d2du,0x91646c97u,0xe6635c01u,
			0x6b6b51f4u,0x1c6c6162u,0x856530d8u,0xf262004eu,0x6c0695edu,0x1b01a57bu,0x8208f4c1u,0xf50fc457u,
			0x65b0d9c6u,0x12b7e950u,0x8bbeb8eau,0xfcb9887cu,0x62dd1ddfu,0x15da2d49u,0x8cd37cf3u,0xfbd44c65u,
			0x4db26158u,0x3ab551ceu,0xa3bc0074u,0xd4bb30e2u,0x4adfa541u,0x3dd895d7u,0xa4d1c46du,0xd3d6f4fbu,
			0x4369e96au,0x346ed9fcu,0xad678846u,0xda60b8d0u,0x44042d73u,0x33031de5u,0xaa0a4c5fu,0xdd0d7cc9u,
			0x5005713cu,0x270241aau,0xbe0b1010u,0xc90c2086u,0x5768b525u,0x206f85b3u,0xb966d409u,0xce61e49fu,
			0x5edef90eu,0x29d9c998u,0xb0d09822u,0xc7d7a8b4u,0x59b33d17u,0x2eb40d81u,0xb7bd5c3bu,0xc0ba6cadu,
			0xedb88320u,0x9abfb3b6u,0x03b6e20cu,0x74b1d29au,0xead54739u,0x9dd277afu,0x04db2615u,0x73dc1683u,
			0xe3630b12u,0x94643b84u,0x0d6d6a3eu,0x7a6a5aa8u,0xe40ecf0bu,0x9309ff9du,0x0a00ae27u,0x7d079eb1u,
			0xf00f9344u,0x8708a3d2u,0x1e01f268u,0x6906c2feu,0xf762575du,0x806567cbu,0x196c3671u,0x6e6b06e7u,
			0xfed41b76u,0x89d32be0u,0x10da7a5au,0x67dd4accu,0xf9b9df6fu,0x8ebeeff9u,0x17b7be43u,0x60b08ed5u,
			0xd6d6a3e8u,0xa1d1937eu,0x38d8c2c4u,0x4fdff252u,0xd1bb67f1u,0xa6bc5767u,0x3fb506ddu,0x48b2364bu,
			0xd80d2bdau,0xaf0a1b4cu,0x36034af6u,0x41047a60u,0xdf60efc3u,0xa867df55u,0x316e8eefu,0x4669be79u,
			0xcb61b38cu,0xbc66831au,0x256fd2a0u,0x5268e236u,0xcc0c7795u,0xbb0b4703u,0x220216b9u,0x5505262fu,
			0xc5ba3bbeu,0xb2bd0b28u,0x2bb45a92u,0x5cb36a04u,0xc2d7ffa7u,0xb5d0cf31u,0x2cd99e8bu,0x5bdeae1du,
			0x9b64c2b0u,0xec63f226u,0x756aa39cu,0x026d930au,0x9c0906a9u,0xeb0e363fu,0x72076785u,0x05005713u,
			0x95bf4a82u,0xe2b87a14u,0x7bb12baeu,0x0cb61b38u,0x92d28e9bu,0xe5d5be0du,0x7cdcefb7u,0x0bdbdf21u,
			0x86d3d2d4u,0xf1d4e242u,0x68ddb3f8u,0x1fda836eu,0x81be16cdu,0xf6b9265bu,0x6fb077e1u,0x18b74777u,
			0x88085ae6u,0xff0f6a70u,0x66063bcau,0x11010b5cu,0x8f659effu,0xf862ae69u,0x616bffd3u,0x166ccf45u,
			0xa00ae278u,0xd70dd2eeu,0x4e048354u,0x3903b3c2u,0xa7672661u,0xd06016f7u,0x4969474du,0x3e6e77dbu,
			0xaed16a4au,0xd9d65adcu,0x40df0b66u,0x37d83bf0u,0xa9bcae53u,0xdebb9ec5u,0x47b2cf7fu,0x30b5ffe9u,
			0xbdbdf21cu,0xcabac28au,0x53b39330u,0x24b4a3a6u,0xbad03605u,0xcdd70693u,0x54de5729u,0x23d967bfu,
			0xb3667a2eu,0xc4614ab8u,0x5d681b02u,0x2a6f2b94u,0xb40bbe37u,0xc30c8ea1u,0x5a05df1bu,0x2d02ef8du
		};

		/// <summary>
		/// Вычисление CRC32 для файла
		/// </summary>
        /// <param name="sourceFileName">путь к файлу</param>
		/// <returns>CRC32</returns>
        public static uint CRC32(string sourceFileName)
		{
            byte[] fileData = FileUtils.FileHelper.ReadFileData(sourceFileName);
            uint crc = CRC32(fileData, 0, fileData.Length);
			return crc;
		}

		/// <summary>
        /// Вычисление CRC32 для массива байт
		/// </summary>
		/// <param name="bytes">массив байт</param>
		/// <param name="offset">смещение от начала (откуда начинать считать)</param>
		/// <param name="length">кол-во байт (от смещения)</param>
		/// <returns>CRC32</returns>
        public static uint CRC32(byte[] bytes, int offset, int length)
		{
			return CRC32(bytes, offset, length, 0u);
		}

		/// <summary>
        /// Вычисление CRC32 для массива байт
		/// </summary>
		/// <param name="bytes">массив байт</param>
		/// <param name="offset">смещение</param>
		/// <param name="length">длина</param>
		/// <param name="baseCRC32">базовое CRC</param>
		/// <returns>CRC32</returns>
        public static unsafe uint CRC32(byte[] bytes, int offset, int length, uint baseCRC32)
		{
			if (bytes == null || length == 0)
				return baseCRC32;
			fixed (byte* pBytes = &bytes[offset])
			fixed (uint* pTable = &CRC_Table[0])
			{
				byte* p = pBytes;
				uint* t = pTable;
				uint x = ~baseCRC32;
				while (length >= 8)
				{
					x = (x >> 8) ^ t[(x & 0xFF) ^ p[0]];
					x = (x >> 8) ^ t[(x & 0xFF) ^ p[1]];
					x = (x >> 8) ^ t[(x & 0xFF) ^ p[2]];
					x = (x >> 8) ^ t[(x & 0xFF) ^ p[3]];
					x = (x >> 8) ^ t[(x & 0xFF) ^ p[4]];
					x = (x >> 8) ^ t[(x & 0xFF) ^ p[5]];
					x = (x >> 8) ^ t[(x & 0xFF) ^ p[6]];
					x = (x >> 8) ^ t[(x & 0xFF) ^ p[7]];
					length -= 8;
					p += 8;
				}
				switch (length)
				{
					case 1:
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[0]];
						break;
					case 2:
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[0]];
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[1]];
						break;
					case 3:
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[0]];
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[1]];
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[2]];
						break;
					case 4:
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[0]];
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[1]];
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[2]];
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[3]];
						break;
					case 5:
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[0]];
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[1]];
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[2]];
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[3]];
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[4]];
						break;
					case 6:
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[0]];
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[1]];
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[2]];
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[3]];
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[4]];
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[5]];
						break;
					case 7:
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[0]];
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[1]];
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[2]];
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[3]];
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[4]];
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[5]];
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[6]];
						break;
				}
				return ~x;
			}
		}

		/// <summary>
        /// Вычисление CRC32 для массива целых чисел
		/// </summary>
		/// <param name="values">массив целых чисел</param>
		/// <param name="offset">смещение</param>
		/// <param name="length">длина</param>
		/// <returns>CRC32</returns>
        public static uint CRC32(int[] values, int offset, int length)
		{
			return CRC32(values, offset, length, 0u);
		}

		/// <summary>
        /// Вычисление CRC32 для массива целых чисел
		/// </summary>
		/// <param name="values">массив целых чисел</param>
		/// <param name="offset">смещение</param>
		/// <param name="length">длина</param>
		/// <param name="baseCRC32">базовое значение</param>
		/// <returns>CRC32</returns>
		public static unsafe uint CRC32(int[] values, int offset, int length, uint baseCRC32)
		{
			if (values == null || length == 0)
				return baseCRC32;
			fixed (int* pValues = &values[offset])
			fixed (uint* pTable = &CRC_Table[0])
			{
				byte* p = (byte*)pValues;
				uint* t = pTable;
				length <<= 2;
				uint x = ~baseCRC32;
				while (length >= 8)
				{
					x = (x >> 8) ^ t[(x & 0xFF) ^ p[0]];
					x = (x >> 8) ^ t[(x & 0xFF) ^ p[1]];
					x = (x >> 8) ^ t[(x & 0xFF) ^ p[2]];
					x = (x >> 8) ^ t[(x & 0xFF) ^ p[3]];
					x = (x >> 8) ^ t[(x & 0xFF) ^ p[4]];
					x = (x >> 8) ^ t[(x & 0xFF) ^ p[5]];
					x = (x >> 8) ^ t[(x & 0xFF) ^ p[6]];
					x = (x >> 8) ^ t[(x & 0xFF) ^ p[7]];
					length -= 8;
					p += 8;
				}
				if (length > 0)
				{
					x = (x >> 8) ^ t[(x & 0xFF) ^ p[0]];
					x = (x >> 8) ^ t[(x & 0xFF) ^ p[1]];
					x = (x >> 8) ^ t[(x & 0xFF) ^ p[2]];
					x = (x >> 8) ^ t[(x & 0xFF) ^ p[3]];
				}
				return ~x;
			}
		}

		//unsafe
		/*public static unsafe uint CRC32(string s)
		{
			int length = 0;
			if (s != null)
				length = s.Length;
			if (length == 0)
				return 0;
			fixed (char* pChars = s)
			fixed (uint* pTable = &CRC_Table[0])
			{
				byte* p = (byte*)pChars;
				uint* t = pTable;
				length <<= 1;
				uint x = 0xFFFFFFFFu;
				while (length >= 8)
				{
					x = (x >> 8) ^ t[(x & 0xFF) ^ p[0]];
					x = (x >> 8) ^ t[(x & 0xFF) ^ p[1]];
					x = (x >> 8) ^ t[(x & 0xFF) ^ p[2]];
					x = (x >> 8) ^ t[(x & 0xFF) ^ p[3]];
					x = (x >> 8) ^ t[(x & 0xFF) ^ p[4]];
					x = (x >> 8) ^ t[(x & 0xFF) ^ p[5]];
					x = (x >> 8) ^ t[(x & 0xFF) ^ p[6]];
					x = (x >> 8) ^ t[(x & 0xFF) ^ p[7]];
					length -= 8;
					p += 8;
				}
				switch (length)
				{
					case 2:
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[0]];
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[1]];
						break;
					case 4:
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[0]];
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[1]];
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[2]];
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[3]];
						break;
					case 6:
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[0]];
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[1]];
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[2]];
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[3]];
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[4]];
						x = (x >> 8) ^ t[(x & 0xFF) ^ p[5]];
						break;
				}
				return ~x;
			}
		}*/

    }
    #endregion

    #region Получение хэша пароля
    /// <summary>
    /// Вспомогательный класс для вычисления хэша пароля
    /// </summary>
    public sealed class PwdHelper
    {
        /// <summary>
        /// Вычислить хэш (SHA512Managed) пароля
        /// </summary>
        /// <param name="password">Пароль</param>
        /// <returns>хэш (Base64String)</returns>
        public static string GetPasswordHash(string password)
        {
            byte[] pwdByteBuff = null;
            if (String.IsNullOrEmpty(password))
                pwdByteBuff = new byte[1] { 0 };
            else
            {
                pwdByteBuff = new byte[password.Length];
                for (int i = 0; i < pwdByteBuff.Length; i++)
                {
                    pwdByteBuff[i] = (byte)password[i];
                }
            }
            SHA512Managed crypter = new SHA512Managed();
            byte[] pwdHashBuff = crypter.ComputeHash(pwdByteBuff);
            crypter.Clear();
            return Convert.ToBase64String(pwdHashBuff);
        }
    }
    #endregion

    #region Работа с командной строкой
    /// <summary>
    /// Общие методы для работы с командной строкой
    /// </summary>
    public struct CommandLineUtils
    {
        /// <summary>
        /// Присутствует ли параметр с заданным именем
        /// </summary>
        /// <param name="parameterName">Имя параметра</param>
        /// <returns>true/false</returns>
        public static bool ParameterPresent(string parameterName)
        {
            string paramValue = String.Empty;
            return ParameterPresent(parameterName, out paramValue);
        }

        /// <summary>
        /// Присутствует ли параметр с заданным именем
        /// </summary>
        /// <param name="parameterName">Имя параметра</param>
        /// <param name="paramValue">Значение параметра</param>
        /// <returns>true/false</returns>
        public static bool ParameterPresent(string parameterName, out string paramValue)
        {
            string cmdLine = Environment.CommandLine;
            string[] parameters = cmdLine.Split(new char[] { '/', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            bool isPresent = false;
            paramValue = String.Empty;
            foreach (string parameter in parameters)
            {
                if (parameter.IndexOf(parameterName) != -1)
                {
                    isPresent = true;
                    paramValue = parameter.Substring(parameterName.Length).Trim('"');
                    break;
                }
            }
            return isPresent;
        }

    }
    #endregion

    #region Структура для минимизации кода использования объектов через Reflection
    public struct ReflectionHelper
    {
        /// <summary>
        /// Получить значение свойства
        /// </summary>
        /// <param name="obj">объект содержащий свойство</param>
        /// <param name="propName">название свойства</param>
        /// <returns>значение свойства</returns>
        public static object GetProperty(object obj, string propName, params object[] prms)
        {
            return obj.GetType().InvokeMember(propName, BindingFlags.GetProperty, null, obj, prms);
        }

        /// <summary>
        /// Установить значение свойства
        /// </summary>
        /// <param name="obj">объект содрежащий свойство</param>
        /// <param name="propName">название свойства</param>
        /// <param name="value">значение свойства</param>
        public static void SetProperty(object obj, string propName, object value)
        {
            obj.GetType().InvokeMember(propName, BindingFlags.SetProperty, null, obj, new object[1] { value });
        }

        /// <summary>
        /// Вызвать метод
        /// </summary>
        /// <param name="obj">объект содержащий метод</param>
        /// <param name="methodName">название метода</param>
        /// <param name="prms">параметры метода</param>
        /// <returns>результат выполнения</returns>
        public static object CallMethod(object obj, string methodName, params object[] prms)
        {
            return obj.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, obj, prms);
        }
    }
    #endregion

    #region Получение информации об исключении
    public struct ExceptionHelper
    {
        // шаблон заголовка лог-файла
        private static readonly string CRASH_TITLE_TEMPLATE =
            "{0} в приложении '{1}' возникла критическая ошибка '{2}'" +
            Environment.NewLine + Environment.NewLine +
            "Поля объекта Exception:" + Environment.NewLine + Environment.NewLine;

        public static string DumpException(Exception e)
        {
            StringBuilder sb = new StringBuilder(1024);
            sb.AppendFormat(
                CRASH_TITLE_TEMPLATE, 
                DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                Assembly.GetExecutingAssembly().ManifestModule.Name, 
                e.GetType().FullName);

            // получаем все поля класса
            PropertyInfo[] properties = e.GetType().GetProperties();
            foreach (PropertyInfo pi in properties)
            {
                try
                {
                    object propVal = ReflectionHelper.GetProperty(e, pi.Name);
                    sb.Append(pi.Name);
                    sb.Append(": ");
                    sb.Append(propVal.ToString());
                    sb.AppendLine();
                    sb.AppendLine();
                }
                catch
                {
                    // некоторые свойства прочитать невозможно, т.к. они требуют обращения к серверным сборкам
                }
            }
            return sb.ToString();
        }
    }
    #endregion

    #region Проверка имени объекта на зарезервированные слова

    /// <summary>
    /// Список зарезервированных слов для PL\SQL, MS SQl
    /// </summary>
    public class ReservedWordsClass
    {
        private static readonly List<string> reservedWords;

        static ReservedWordsClass()
        {
            reservedWords = new List<string>();

            ResourceManager resManager = new ResourceManager("Krista.FM.Common.KeywordOracleSQL", Assembly.GetExecutingAssembly());
            IDictionaryEnumerator resEnumerator = resManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true).GetEnumerator();
            while (resEnumerator.MoveNext())
            {
                reservedWords.Add(resEnumerator.Value.ToString());
            }
        }

        public static bool CheckName(string name)
        {
            if (!reservedWords.Contains(name.ToUpper()))
                return true;

            throw new Exception(
                string.Format("Вкачестве имени объекта не может быть использовано зарезервированное слово {0}", name));
        }

        public static List<string> ReservedWords
        {
            get { return reservedWords; }
        }
    } 

    #endregion


    /// <summary>
    /// объект для получения информации о копировании вариантов
    /// </summary>
    public class RemoteMetod : MarshalByRefObject
    {
        public delegate void GetSring(string message);
        public delegate void GetDataTable(DataTable table);

        public RemoteMetod()
        {
            messages = new List<string>();
        }

        private List<string> messages;

        public void GetStrFromRemote(string message)
        {
            messages.Add(message);
            if (_onGetString != null)
                _onGetString(message);
        }

        public List<string> Messages
        {
            get { return messages; }
        }

        private GetSring _onGetString;

        public event GetSring OnGetString
        {
            add { _onGetString += value; }
            remove { _onGetString -= value; }
        }

        private GetDataTable _onGetDataTable;

        public event GetDataTable OnGetDataTable
        {
            add { _onGetDataTable += value; }
            remove { _onGetDataTable -= value; }
        }

        public void GetDataTableFromRemoting(DataTable table)
        {
            if (_onGetDataTable != null)
                _onGetDataTable(table);
        } 
    }

    /// <summary>
    /// Объект для получения информации об объединении дубликатов.
    /// </summary>
    public class MergeDuplicatesInformer : MarshalByRefObject
    {
        private MergeDuplicatesListener _onSendMergeMessage;

        public event MergeDuplicatesListener OnSendMergeMessage
        {
            add { _onSendMergeMessage += value; }
            remove { _onSendMergeMessage -= value; }
        }

        public void SendMergeMessage(string message)
        {
            if (_onSendMergeMessage != null)
                _onSendMergeMessage(message);
        }
    }
}
