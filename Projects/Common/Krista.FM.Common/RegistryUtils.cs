using System;
using Microsoft.Win32;
//using System.Security.AccessControl;
using System.IO;


namespace Krista.FM.Common.RegistryUtils
{
	/// <summary>
	/// Класс для рхранения настроек в системном реестре
	/// Общий принцип такой - каждый объект работает с ключом, сформированным 
	/// в соответствии со своим Namespace.
	/// Родительский ключ - либо HKEY_CURRENT_USER, либо HKEY_LOCAL_MACHINE
	/// 
	/// Т.е., например, объекты пространства имен Krista.FM.Common могут работать
	/// с ключами HCKU\SOFTWARE\Krista\FM\Common и HCLM\SOFTWARE\Krista\FM\Common
	/// (в зависимости от типа настроек)
	/// </summary>
	public sealed class Utils
	{
		// пространство имен вызывающего объекта
		private string CallerNamespace;
		// ключ рееста для вызывающего объекта
		private RegistryKey CallerKey;
		//Корневой ключ (HKLM или HKCU)
		private RegistryKey RootKey;

        /// <summary>
		/// Конструктор класса
		/// </summary>
		/// <param name="CallerType">Тип вызывающего объекта</param>
		/// <param name="UseCurrentUserKey">Использовать-ли в качестве родительского ключ HKCU?</param>
		public Utils(System.Type CallerType, bool UseCurrentUserKey)
		{
			// Получаем пространство имен вызывающего объекта.
			// Тип параметра Type выбран намеренно - чтобы не подсовывали
			// несущесвующих пространств имен
			CallerNamespace = CallerType.ToString();
		
            // Инициализируем корневой ключ
			if (UseCurrentUserKey)
			{
				RootKey = Registry.CurrentUser;
			}
			else 
			{
				RootKey = Registry.LocalMachine;
			}
			// Формируем или получаем ключ для вызывающего объекта
			BuildRegistryKey();
		}

		/// <summary>
		/// Деструктор класса
		/// </summary>
		~Utils()
		{
			if (CallerKey != null) CallerKey.Close();
			if (RootKey != null) RootKey.Close();
		}

        /// <summary>
        /// Метод проверки наличия в системе COM-объекта c заданным ProgID
        /// </summary>
        /// <param name="progID">ProgID объекта</param>
        /// <param name="isInprocServer">Является ли объект Inproc-сервером</param>
        /// <returns>результат проверки</returns>
        public static bool CheckLibByProgID(string progID, bool isInprocServer)
        {
            string filter = String.Format("{0}\\Clsid", progID);
            RegistryKey key = Registry.ClassesRoot.OpenSubKey(filter, false);
            if (key == null)
                return false;
            string clsid = key.GetValue(String.Empty).ToString();
            key.Close();
            if (isInprocServer)
                filter = String.Format("CLSID\\{0}\\InprocServer32", clsid);
            else
                filter = String.Format("CLSID\\{0}\\LocalServer32", clsid); 
            if (PlatformDetect.Is64BitProcess)
                filter = "Wow6432Node\\" + filter;
            key = Registry.ClassesRoot.OpenSubKey(filter, false);
            
            if (key == null)
                return false;            
            
            object keyValue = key.GetValue(String.Empty);
            if (keyValue == null)
                return false;
            string dllPath = keyValue.ToString();
            key.Close();
            bool res = File.Exists(dllPath);
            return File.Exists(dllPath);
        }
        
		/// <summary>
        /// Проверка существования ключа, в случае отсутсвия - создается, если createKey = true
		/// </summary>
		/// <param name="keyName">Название ключа</param>
        /// <param name="createKey">Если true и ключ не найден, то ключ создается</param>
        /// <returns>Найденный/Созданный ключ</returns>
		private RegistryKey CheckKey(string keyName, bool createKey)
		{
            return CheckKey(RootKey, keyName, createKey);
		}

        /// <summary>
        /// Проверка существования ключа, в случае отсутсвия - создается, если createKey = true
        /// </summary>
        /// <param name="rootKey">Корневой ключ (HKLM или HKCU)</param>
        /// <param name="keyName">Название ключа</param>
        /// <param name="createKey">Если true и ключ не найден, то ключ создается</param>
        /// <returns>Найденный/Созданный ключ</returns>
        private static RegistryKey CheckKey(RegistryKey rootKey, string keyName, bool createKey)
        {
            RegistryKey currentKey = rootKey.OpenSubKey(keyName, true);
            if (currentKey == null && createKey)
                currentKey = rootKey.CreateSubKey(keyName);
            return currentKey;
        }

        /// <summary>
        /// Проверка существования ключа, в случае отсутсвия - создается
        /// </summary>
        /// <param name="KeyName">Название ключа</param>
        /// <returns>Найденный/Созданный ключ</returns>
        private RegistryKey CheckKey(string KeyName)
		{
            return CheckKey(KeyName, true);
        }

        /// <summary>
		/// Проверка наличия параметра. Все параметры хранятся в строковом виде.
        /// В случае отсутсвия и если createParameter true - создается пустой параметр.
		/// </summary>
		/// <param name="ValueName">Название параметра</param>
        /// <param name="createParameter">Если true и параметр не найден, то параметр создается</param>
        /// <returns>Значение параметра (если параметра нет - возвращается пустая строка),
		/// это нужно учитывать при конвертации в численные типы
		/// </returns>
		private string CheckValue(string ValueName, bool createParameter)
		{
			object tmpValue = CallerKey.GetValue(ValueName);
            if (tmpValue == null && createParameter) 
			{
				tmpValue = "";
				CallerKey.SetValue(ValueName, "");
			}
            if (tmpValue == null)
                return null;
            else
			    return tmpValue.ToString();
		}

        /// <summary>
        /// Проверка наличия параметра. Все параметры хранятся в строковом виде.
        /// В случае отсутсвия - создается пустой параметр.
        /// </summary>
        /// <param name="ValueName">Название параметра</param>
        /// <returns>Значение параметра (если параметра нет - возвращается пустая строка),
        /// это нужно учитывать при конвертации в численные типы
        /// </returns>
        private string CheckValue(string ValueName)
        {
            return CheckValue(ValueName, true);
        }

        /// <summary>
		/// Получение/создание ключа реестра для вызывающего объекта
		/// </summary>
		private void BuildRegistryKey()
		{
            CallerKey = BuildRegistryKey(RootKey, CallerNamespace);
		}

        /// <summary>
        /// Получение/создание ключа реестра для вызывающего объекта
        /// </summary>
        public static RegistryKey BuildRegistryKey(RegistryKey rootKey, string objectNnamespace)
        {
            // разбираем пространство имен вызывающего объекта на составляющие
            string[] splitNamespace = objectNnamespace.Split(new Char[] { '.' });
            string callerKeyName = "SOFTWARE";
            // последовательно проходим по всей ветке и проверяем наличие ключей,
            // в случае отсутсвия - создаем
            foreach (string s in splitNamespace)
            {
                callerKeyName = callerKeyName + '\\' + s;
                CheckKey(rootKey, callerKeyName, true);
            };
            // Возвращаем ключ для вызывающего объекта
            return rootKey.OpenSubKey(callerKeyName, true);
        }

        /// <summary>
		/// Получение значению ключа
		/// </summary>
		/// <param name="KeyName">Имя значения</param>
		/// <returns>Значение</returns>
        public string GetKeyValue(string KeyName)
		{
			return CheckValue(KeyName);
		}

		/// <summary>
		/// Установка значения ключа
		/// </summary>
		/// <param name="KeyName">Имя значения</param>
		/// <param name="Value">Значение</param>
		public void SetKeyValue(string KeyName, object Value)
		{
			CallerKey.SetValue(KeyName, Value.ToString());
		}

        /// <summary>
        /// Поиск ключа по имени
        /// </summary>
        /// <param name="keyName">Имя ключа</param>
        /// <returns>true - ключ найден, иначе false</returns>
        public bool FindKey(string keyName)
        {
            return CheckKey(CallerKey.Name + "\\" + keyName, false) != null;
        }

        /// <summary>
        /// Поиск ключа по имени
        /// </summary>
        /// <param name="keyName">Имя ключа</param>
        /// <returns>true - ключ найден, иначе false</returns>
        public bool FindParameter(string keyName)
        {
            return CheckValue(keyName, false) != null;
        }

        /// <summary>
        /// Поиск и запись в массив всех подключей.
        /// </summary>
        /// <returns>Массив подключей.</returns>
        public string[] GetNames()
        {
            string[] names = CallerKey.GetSubKeyNames();
            return names;
        }

        /// <summary>
        /// Открывает ключ по полному пути.
        /// </summary>
        /// <param name="s">Путь</param>
        public RegistryKey OpenSubKeys(string s)
        {
            return CallerKey.OpenSubKey(s);
        }
    }
}
