using System;
using System.IO;

namespace Krista.FM.Common
{
    /// <summary>
    /// Класс для обработки строки подключения OLEDB провайдера
    /// </summary>
    public class ConnectionString
    {
        /// <summary>
        /// Провайдер
        /// </summary>
        public string Provider;

        /// <summary>
        /// Имя сервера
        /// </summary>
        public string DataSource;

        /// <summary>
        /// Имя базы данных
        /// </summary>
        public string InitialCatalog = String.Empty;

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string UserID;

        /// <summary>
        /// Пароль
        /// </summary>
        public string Password;

        /// <summary>
        /// Максимальное время ожидания подключения
        /// </summary>
        public int? ConnectTimeout;

        /// <summary>
        /// Версия сервера
        /// </summary>
        public string ConnectTo;
        
        /// <summary>
        /// Оригинальная строка подключения к базе
        /// </summary>
        public string OriginalString = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public bool PersistSecurityInfo;

        /// <summary>
        /// Тип встроенной аутентификации.
        /// </summary>
        public string IntegratedSecurity;

        /// <summary>
        /// Тайм-аут
        /// </summary>
        public string Timeout = string.Empty;

        /// <summary>
        /// таймаут на выполнение запросов
        /// </summary>
        public int? QueryTimeout;

        public ConnectionString()
        {
        }

        /// <summary>
        /// Разбор строки подключения на составляющие
        /// </summary>
        /// <param name="cs">строка подключения</param>
        public virtual void Parse(String cs)
        {
            string[] parameters = cs.Split(';');
            foreach (string prm in parameters)
            {
                string[] keyValue = prm.Split('=');
                switch (keyValue[0].TrimStart())
                {
                    case "Provider":
                        this.Provider = keyValue[1];
                        break;
                    case "Data Source":
                        this.DataSource = keyValue[1];
                        break;
                    case "Initial Catalog":
                        this.InitialCatalog = keyValue[1];
                        break;
                    case "User ID":
                        this.UserID = keyValue[1];
                        break;
                    case "Integrated Security":
                        this.IntegratedSecurity = keyValue[1];
                        break;
                    case "Persist Security Info":
                        this.PersistSecurityInfo = Convert.ToBoolean(keyValue[1]);
                        break;
                    case "Password":
                        this.Password = keyValue[1];
                        break;
                    case "Connect Timeout":
                        this.ConnectTimeout = Convert.ToInt32(keyValue[1]);
                        break;
                    case "Query Timeout":
                        QueryTimeout = Convert.ToInt32(keyValue[1]);
                        break;
                    case "ConnectTo":
                        this.ConnectTo = keyValue[1];
                        break;
                    case "Timeout":
                        this.Timeout = keyValue[1];
                        break;
                    case "Extended Properties":
                        string[] properties = cs.Split(new string[] {"\""}, StringSplitOptions.RemoveEmptyEntries);
                        if (properties.Length > 0)
                            Parse(properties[1]);
                        break;
                }
            }
        }

        /// <summary>
        /// Возвращает строку подключения
        /// </summary>
        /// <returns>Строка подключения</returns>
        public override string ToString()
        {
            string connectionString = String.Format(
                "Data Source={0};", DataSource);

            if (!String.IsNullOrEmpty(UserID))
                connectionString += String.Format("User ID={0};", UserID);

            if (!String.IsNullOrEmpty(InitialCatalog))
                connectionString += String.Format("Initial Catalog={0};", InitialCatalog);

            if (!String.IsNullOrEmpty(IntegratedSecurity))
                connectionString += String.Format("Integrated Security={0};", IntegratedSecurity);

            if (ConnectTimeout != null)
                connectionString += String.Format("Connect Timeout={0};", ConnectTimeout);

            if (PersistSecurityInfo)
                return String.Format("{0}Password={1};Persist Security Info=True", connectionString, Password);
            else
                return String.Format("{0}Persist Security Info=False", connectionString);
        }

        /// <summary>
        /// Считывает строку подключения из файла
        /// </summary>
        /// <param name="file">UDL-файл со строкой подключения</param>
        public void ReadConnectionString(string file)
        {
            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(file))
                {
                    String line;
                    // Read and display lines from the file until the end of 
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line == "[oledb]")
                            continue;
                        if (line == "; Everything after this line is an OLE DB initstring")
                            continue;

                        if (line.Contains("Provider"))
                        {
                            OriginalString = line;

                            Parse(line);
                            return;
                        }
                    }
                    throw new Exception("Неверная строка подключения.");
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("The file could not be read:");
                Trace.WriteLine(e.Message);
                throw new Exception(e.Message, e);
            }
        }
    }
}
