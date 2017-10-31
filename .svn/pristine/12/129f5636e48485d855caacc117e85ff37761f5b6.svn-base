using System;
using System.Data;
using System.Globalization;
using System.Management;
using Microsoft.Win32;

namespace Krista.FM.Common
{
    /// <summary>
    /// Класс для сбора информации о системе.
    /// </summary>
    public class SystemInfo
    {
        #region Fields
        /// <summary>
        /// Используется для кеширования информации.
        /// </summary>
        private DataTable collectedInfo;
        /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        private DataColumn dc_id = new DataColumn();
        /// <summary>
        /// Вторичный ключ
        /// </summary>
        private DataColumn dc_parentID = new DataColumn();
        /// <summary>
        /// Приложение
        /// </summary>
        private DataColumn dc_application = new DataColumn();
        /// <summary>
        /// категория
        /// </summary>
        private DataColumn dc_category = new DataColumn();
        /// <summary>
        /// Уникальное английское имя
        /// </summary>
        private DataColumn dc_uniqueName = new DataColumn();
        /// <summary>
        /// имя
        /// </summary>
        private DataColumn dc_name = new DataColumn();
        /// <summary>
        /// значение
        /// </summary>
        private DataColumn dc_value = new DataColumn();
        /// <summary>
        /// вызывает/не вызывает подозрение
        /// </summary>
        private DataColumn dc_suspect = new DataColumn();
        /// <summary>
        /// Коммментарии к подозрительному объекту
        /// </summary>
        private DataColumn dc_suspectDescription = new DataColumn();
        /// <summary>
        /// Экземпляр класса, отвечающего за сбор информации о системе
        /// </summary>
        private RemouteSysInformation systeminfo;
        /// <summary>
        /// Кто вызывает: клиент или сервер
        /// </summary>
        private string applicationType;

        #endregion Fields

        public SystemInfo(string applicationType)
        {
            this.applicationType = applicationType;
        }

        /// <summary>
        /// Собирает информацию и сохраняет ее во внутреннюю таблицу.
        /// </summary>
        /// <returns>Таблица содержащая собранную информацию.</returns>
        protected virtual DataTable CollectInfo()
        {
            InitializeTable();

            // TODO Собираем информацию и заполняем collectInfo
            Collect();

            return collectedInfo;
        }

        private void Collect()
        {
            try
            {
                string categoryName = "Окружение (Параметры ОС)";

                if (systeminfo == null)
                    systeminfo = new RemouteSysInformation();

                AddRow(categoryName, "ComputerName", "Имя узла", systeminfo.ComputerName, null);
                AddRow(categoryName, "OSCaption", "Название ОС", systeminfo.OSCaption, null);
                AddRow(categoryName, "OSVersion", "Версия ОС ", systeminfo.OSVersion, null);
                AddRow(categoryName, "OSManufacturer", "Изготовитель ОС ", systeminfo.OSManufacturer, null);
                AddRow(categoryName, "OSBuildType", "Сборка ОС ", systeminfo.OSBuildType, null);
                AddRow(categoryName, "RegisteredUser", "Зарегистрированный пользователь ", systeminfo.RegisteredUser, null);
                AddRow(categoryName, "Organization", "Зарегистрированная организация ", systeminfo.Organization, null);
                AddRow(categoryName, "SerialNumber", "Код продукта ", systeminfo.SerialNumber, null);
                AddRow(categoryName, "InstallDate", "Дата установки ", systeminfo.InstallDate, null);
                AddRow(categoryName, "WorkingTime", "Время работы ", systeminfo.WorkingTime, null);
                AddRow(categoryName, "SysManufacturer", "Изготовитель системы ", systeminfo.SysManufacturer, null);
                AddRow(categoryName, "SysModel", "Модель системы ", systeminfo.SysModel, null);
                AddRow(categoryName, "Systype", "Тип системы ", systeminfo.Systype, null);

                Guid processorID = AddRow(categoryName, "Processors", "Процессор(ы) ", String.Format("число процессоров - {0}", systeminfo.ProcessorCount), null);
                foreach (Processor processor in systeminfo.Processors)
                {
                    AddRow(categoryName, String.Format("Processor{0}", processor.number), String.Format("[{0}]", processor.number), String.Format("{0} {1} ~{2} МГц", processor.caption, processor.manufacturer, processor.speed), processorID);
                }
                AddRow(categoryName, "BiosVersion", "Версия BIOS ", systeminfo.BiosVersion, null);
                AddRow(categoryName, "CurTimeZone", "Часовой пояс ", systeminfo.CurTimeZone, null);
                AddRow(categoryName, "WindowsDirectory", "Папка Windows", systeminfo.WindowsDirectory, null);
                AddRow(categoryName, "SystemFolder", "Системная папка", systeminfo.SystemFolder, null);
                AddRow(categoryName, "SystemDevice", "Устройство загрузки", systeminfo.SystemDevice, null);
                AddRow(categoryName, "OSLanguage", "Язык системы", systeminfo.OSLanguage, null);
                AddRow(categoryName, "TotalVisibleMemorySize", "Полный объем физической памяти", systeminfo.TotalVisibleMemorySize + " МБ", null);
                AddRow(categoryName, "FreePhysicalMemory", "Доступная физическая память", systeminfo.FreePhysicalMemory + " МБ", null);
                AddRow(categoryName, "TotalVirtualMemorySize", "Виртуальная память: макс. размер", systeminfo.TotalVirtualMemorySize + " МБ", null);
                AddRow(categoryName, "FreeVirtualMemory", "Виртуальная память: доступно", systeminfo.FreeVirtualMemory + " МБ", null);
                AddRow(categoryName, "TotalVirtualMemorySize", "Виртуальная память: используется", systeminfo.TotalVirtualMemorySize - systeminfo.FreeVirtualMemory + " МБ", null);
                AddRow(categoryName, "Domain", "Домен", systeminfo.Domain, null);

                Guid updateID = AddRow(categoryName, "Overpatchings", "Исправление(я)", String.Format("Число установленных исправлений - {0}", systeminfo.UpdatesCount), null);
                foreach (Update update in systeminfo.Updates)
                {
                    if (String.IsNullOrEmpty(update.fixComment))
                        AddRow(categoryName, String.Format("Overpatching{0}", update.number), String.Format("[{0}]", update.number), String.Format("{0}", update.hotFixID), updateID);
                    else
                        AddRow(categoryName, String.Format("Overpatching{0}", update.number), String.Format("[{0}]", update.number), String.Format("{0} - {1}",update.hotFixID, update.fixComment), updateID);
                }

                Guid networkAdapterID = AddRow(categoryName, "Adapters", "Сетевые адаптеры", "число адаптеров - " + systeminfo.Networks.Length, null);
                foreach (NetworkAdapter adapter in systeminfo.Networks)
                {
                    Guid adapterID = AddRow(categoryName, String.Format("Adapter{0}", adapter.Number), String.Format("[{0}]", adapter.Number), String.Format("{0} ", adapter.Name), networkAdapterID);
                    foreach (NetworkAdapterConfiguration config in adapter.networkAdapterConfiguration)
                    {
                        AddRow(categoryName, "DHCPEnabled", "DHCP включен", config.DHCPEnabled, adapterID);
                        AddRow(categoryName, "DhcpServer", "DHCP сервер", config.DhcpServer, adapterID);
                        Guid IPID = AddRow(categoryName, "IPAdrresses", "IP адрес", String.Format("число адресов - {0}", config.IPCollection.Length), adapterID);
                        foreach (IpCollection s in config.IPCollection)
                        {
                            AddRow(categoryName, String.Format("IPAdrress{0}", s.Number), String.Format("[{0}]", s.Number), String.Format("{0}", s.IP), IPID);
                        }
                    }
                }

                Guid regionalSettingsID = AddRow(categoryName, "RegionalSettings", "Региональные настройки", null, null);
                AddRow(categoryName, "sLanguage", "Язык, используемый в диалогах", systeminfo.SLanguage, regionalSettingsID);
                AddRow(categoryName, "sDecimal", "Разделитель целой и дробной части", systeminfo.SDecimal, regionalSettingsID);
                AddRow(categoryName, "sCurrency", "Обозначение денежной единицы", systeminfo.SCurrency, regionalSettingsID);
            }
            catch
            { }
        }

        private void InitializeTable()
        {
            //
            //
            //
            collectedInfo = new DataTable();
            collectedInfo.TableName = "CollectionInfo";
            //
            // id
            //
            this.dc_id.ColumnName = "ID";
            this.dc_id.DataType = typeof(Guid);
            this.dc_id.AllowDBNull = true;
            //
            // parentID
            //
            this.dc_parentID.ColumnName = "ParentID";
            this.dc_parentID.AllowDBNull = true;
            this.dc_parentID.DataType = typeof(Guid);
            //
            // category
            //
            this.dc_category.ColumnName = "Category";
            this.dc_category.DataType = System.Type.GetType("System.String");
            //
            // uniqueName
            //
            this.dc_uniqueName.ColumnName = "UniqueName";
            this.dc_uniqueName.DataType = typeof(string);
            //
            // name
            //
            this.dc_name.ColumnName = "Name";
            this.dc_name.DataType = System.Type.GetType("System.String");
            //
            // value
            //
            this.dc_value.ColumnName = "Value";
            this.dc_value.DataType = System.Type.GetType("System.String");
            //
            // suspect
            //
            this.dc_suspect.ColumnName = "Suspect";
            this.dc_suspect.DataType = System.Type.GetType("System.Boolean");
            this.dc_suspect.DefaultValue = false;
            //
            // application
            //
            this.dc_application.ColumnName = "Application";
            this.dc_application.DataType = typeof(string);
            //
            // suspectDescription
            //
            this.dc_suspectDescription.ColumnName = "SuspectDescription";
            this.dc_suspectDescription.DataType = typeof(string);
            this.dc_suspectDescription.AllowDBNull = true;

            collectedInfo.Columns.AddRange(new DataColumn[] { dc_id, dc_application, dc_category, dc_uniqueName, dc_name, dc_value, dc_parentID, dc_suspect, dc_suspectDescription});
        }

        /// <summary>
        /// Возвращает собранную информацию из внутренней таблицы, 
        /// если таблица пустая, то вновь собирает информацию.
        /// </summary>
        /// <returns>Таблица содержащая собранную информацию.</returns>
        public DataTable GetInfo()
        {
            if (collectedInfo == null)
            {
                DataTable table = CollectInfo();
                CheckRows(collectedInfo);
                return table;
            }
            else
                return collectedInfo;
        }

        /// <summary>
        /// Проверка параметров
        /// </summary>
        /// <param name="table"> Заполненная таблица параметров</param>
        protected static void CheckRows(DataTable table)
        {
            try
            {
                // для каждого правила...
                foreach (CheckRule rule in CheckParametrsCollection.RuleCollection)
                {
                    if (rule is CheckDataSourceMD)
                        CheckDataSourceMD.CheckMDDataSource(rule, table);

                    // Строка параметра в table
                    DataRow[] rows = table.Select(String.Format("uniqueName = '{0}'", rule.Parametr));
                    for (int i = 0; i < rows.Length; i++)
                    {
                        if (!rule.Execute(rows[i]["value"].ToString()))
                        {
                            rows[i]["suspect"] = true;
                            rows[i]["suspectDescription"] = String.Format(rule.ErrorMessage, rows[i]["value"]); // строку предупреждения
                        }
                    }
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

       
        /// <summary>
        /// Удаление собранной информации.
        /// При следующем вызове GetInfo() информация будет собрана затово.
        /// </summary>
        public void ClearInfo()
        {
            // TODO Сброс данных
        }

        /// <summary>
        /// Индексатор для доступа к параметрам по имени.
        /// </summary>
        /// <param name="parameterName">Имя параметра.</param>
        /// <returns>Значение параметра.</returns>
        public object this[string parameterName]
        {
            get
            {
                DataRow[] rows = GetInfo().Select(String.Format("Name = {0}", parameterName));
                return rows[0]["Value"];
            }
        }

        protected virtual Guid AddRow(string category, string unique, string name, string value, object parentID)
        {
            DataRow row = GetNewRow(category, unique, name, value, parentID); 
            collectedInfo.Rows.Add(row);
            row[0] = Guid.NewGuid();
            return (Guid)row[0];
        }

        private DataRow GetNewRow(string category, string unique, string name, string value)
        {
            DataRow row = collectedInfo.NewRow();
            row[dc_application] = applicationType;
            row[dc_category] = category;
            row[dc_uniqueName] = unique;
            row[dc_name] = name;
            row[dc_value] = value;

            return row;
        }

        private DataRow GetNewRow(string category, string unique, string name, string value, object parentIDNumber)
        {
            DataRow row = GetNewRow(category, unique, name, value);
            if (parentIDNumber != null)
                row[dc_parentID] = (Guid)parentIDNumber;

            return row;
        }
    }

    /// <summary>
    /// Информация о процессоре
    /// </summary>  
    public struct Processor
    {
        public int number;
        public String caption;
        public uint speed;
        public String manufacturer;
    }

    /// <summary>
    /// Информация об исправлении
    /// </summary>
    public struct Update
    {
        public int number;
        public string hotFixID;
        public string fixComment;
    }

    /// <summary>
    /// Информация о сетевом подключении
    /// </summary>
    public struct NetworkAdapter
    {
        public int Number;
        public string Name;
        public int ID;
        public NetworkAdapterConfiguration[] networkAdapterConfiguration; 
    }

    /// <summary>
    /// Информация о IP-адресах
    /// </summary>
    public struct NetworkAdapterConfiguration
    {
        public string DHCPEnabled;
        public string DhcpServer;
        public IpCollection[] IPCollection;
    }

    public struct IpCollection
    {
        public int Number;
        public string IP;
    }

    /// <summary>
    /// Класс для сбора системной информации
    /// </summary>
    public class RemouteSysInformation
    {
        #region Fields
        /// <summary>
        /// Имя компьютера
        /// </summary>
        private string computerName;
        /// <summary>
        /// Название ОС
        /// </summary>
        private string oSCaption;
        /// <summary>
        /// версия ОС
        /// </summary>
        private string oSVersion;
        /// <summary>
        /// Изготовитель ОС
        /// </summary>
        private string oSManufacturer;
        /// <summary>
        /// Сборка ОС
        /// </summary>
        private string oSBuildType;
        /// <summary>
        /// Зарегистрированный пользователь
        /// </summary>
        private string registeredUser;
        /// <summary>
        /// Зарегистрированная организация
        /// </summary>
        private string organization;
        /// <summary>
        /// Код продукта
        /// </summary>
        private string serialNumber;
        /// <summary>
        /// Дата установки
        /// </summary>
        private string installDate;
        /// <summary>
        /// Время работы системы
        /// </summary>
        private string workingTime;
        /// <summary>
        /// Часовоя пояс
        /// </summary>
        private string timeZone;
        /// <summary>
        /// версия BIOS
        /// </summary>
        private string biosVersion;
        /// <summary>
        /// Изготовитель системы
        /// </summary>
        private string sysManufacturer;
        /// <summary>
        /// Модель системы
        /// </summary>
        private string sysModel;
        /// <summary>
        /// Тип системы
        /// </summary>
        private string systype;
        /// <summary>
        /// Количество процессоров
        /// </summary>
        private int processorCount;
        /// <summary>
        /// Коллекция процессоров
        /// </summary>
        private Processor[] processors;
        /// <summary>
        /// Количество обновлений
        /// </summary>
        private int updatesCount;
        /// <summary>
        /// Коллекция обновлений
        /// </summary>       
        private Update[] updates;
        /// <summary>
        /// Папка Windows
        /// </summary>
        private string windowsDirectory;
        /// <summary>
        /// Системная папка
        /// </summary>
        private string systemFolder;
        /// <summary>
        /// Устройство загрузки
        /// </summary>
        private string systemDevice;
        /// <summary>
        /// Полный объем физической памяти
        /// </summary>
        private int totalVisibleMemorySize;
        /// <summary>
        /// Доступно физической памяти
        /// </summary>
        private int freePhysicalMemory;
        /// <summary>
        /// Виртуальная память: Макс. размер
        /// </summary>
        private int totalVirtualMemorySize;
        /// <summary>
        /// Виртуальная память: доступно 
        /// </summary>
        private int freeVirtualMemory;
        /// <summary>
        /// Домен
        /// </summary>
        private string domain;
        /// <summary>
        /// Сетевые адаптеры
        /// </summary>
        private NetworkAdapter[] networks;
        /// <summary>
        /// Язык OS
        /// </summary>
        private string oSLanguage;
        /// <summary>
        /// Разделитель целой и дробной части
        /// </summary>
        private string sDecimal;
        /// <summary>
        /// Языковой стандарт
        /// </summary>
        private string sLanguage;
        /// <summary>
        /// Денежная единица
        /// </summary>
        private string sCurrency;

        #endregion

        #region Properties
        public string ComputerName
        {
            get { return computerName; }
        }
        public string OSCaption
        {
            get { return oSCaption; }
        }
        public string OSVersion
        {
            get { return oSVersion; }
        }
        public string OSManufacturer
        {
            get { return oSManufacturer; }
        }
        public string OSBuildType
        {
            get { return oSBuildType; }
        }
        public string RegisteredUser
        {
            get { return registeredUser; }
        }
        public string Organization
        {
            get { return organization; }
        }
        public string SerialNumber
        {
            get { return serialNumber; }
        }
        public string InstallDate
        {
            get { return installDate; }
        }
        public string WorkingTime
        {
            get { return workingTime; }
        }
        public string CurTimeZone
        {
            get { return timeZone; }
        }
        public string BiosVersion
        {
            get { return biosVersion; }
        }
        public string SysManufacturer
        {
            get { return sysManufacturer; }
        }
        public string SysModel
        {
            get { return sysModel; }
        }
        public string Systype
        {
            get { return systype; }
        }
        public int ProcessorCount
        {
            get { return processorCount; }
        }
        public Processor[] Processors
        {
            get { return processors; }
        }
        public string WindowsDirectory
        {
            get { return windowsDirectory; }
        }
        public string SystemFolder
        {
            get { return systemFolder; }
        }
        public string SystemDevice
        {
            get { return systemDevice; }
        }
        public int TotalVisibleMemorySize
        {
            get { return totalVisibleMemorySize; }
        }
        public Update[] Updates
        {
            get { return updates; }
        }
        public int UpdatesCount
        {
            get { return updatesCount; }
        }
        public int FreePhysicalMemory
        {
            get { return freePhysicalMemory; }
        }
        public int TotalVirtualMemorySize
        {
            get { return totalVirtualMemorySize; }
        }
        public int FreeVirtualMemory
        {
            get { return freeVirtualMemory; }
        }
        public string Domain
        {
            get { return domain; }
        }
        public NetworkAdapter[] Networks
        {
            get { return networks; }
        }
        public string OSLanguage
        {
            get { return oSLanguage; }
        }

        public string SDecimal
        {
            get { return sDecimal; }
        }

        public string SLanguage
        {
            get { return sLanguage; }
        }

        public string SCurrency
        {
            get { return sCurrency; }
        }

        #endregion
        
        public RemouteSysInformation()
        {
            Initialize();
        }

        #region Methods

        
        private void Initialize()
        {
            GetSystemOSInformation();
            GetSystemBIOSInformation();
            GetComputerSystemInformation();
            GetProcessorsInformation();
            GetTimeZoneInformation();
            GetUpdatesInformation();
            GetNetworkAdapterInformation();
            GetRegionSettingsInformation();
        }

        /// <summary>
        /// Информация о региональных настройках
        /// </summary>
        private void GetRegionSettingsInformation()
        {
            try
            {
                using (RegistryKey lang = Registry.CurrentUser.OpenSubKey("Control Panel\\International", true))
                {
                    if (lang != null)
                    {
                        sDecimal = lang.GetValue("sDecimal").ToString();
                        sLanguage = lang.GetValue("sLanguage").ToString();
                        sCurrency = lang.GetValue("sCurrency").ToString();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("{0}", e));
            }
        }

        /// <summary>
        /// Информация о сетевых адаптерах
        /// </summary>
        private void GetNetworkAdapterInformation()
        {
            try
            {
                ManagementObjectSearcher mos = new ManagementObjectSearcher(new SelectQuery("Select * From Win32_NetworkAdapter"));
                ManagementObjectCollection moc = mos.Get();

                networks = new NetworkAdapter[moc.Count];

                int i = 0;
                foreach (ManagementObject mo in moc)
                {
                    networks[i].Number = i;
                    networks[i].Name = String.Format("{0}", mo["Name"]);
                    networks[i].ID = int.Parse(String.Format("{0}", mo["Index"]));

                    mos = new ManagementObjectSearcher(new SelectQuery("Select Index, DHCPEnabled, DHCPServer, IPAddress From Win32_NetworkAdapterConfiguration Where Index = " + networks[i].ID));
                    moc = mos.Get();

                    networks[i].networkAdapterConfiguration = new NetworkAdapterConfiguration[moc.Count];
                    int j = 0;
                    foreach (ManagementObject m in moc)
                    {
                        networks[i].networkAdapterConfiguration[j].DHCPEnabled = ((bool)m["DHCPEnabled"]) ? "Да" : "Нет";
                        networks[i].networkAdapterConfiguration[j].DhcpServer = String.Format("{0}", m["DHCPServer"]);

                        if (m["IPAddress"] != null)
                        {
                            networks[i].networkAdapterConfiguration[j].IPCollection = new IpCollection[((string[])m["IPAddress"]).Length];
                            int k = 0;
                            foreach (string ipstring in (string[])m["IPAddress"])
                            {
                                networks[i].networkAdapterConfiguration[j].IPCollection[k].Number = k;
                                networks[i].networkAdapterConfiguration[j].IPCollection[k].IP = ipstring;
                                k++;
                            }
                        }
                        else
                        {
                            networks[i].networkAdapterConfiguration[j].IPCollection = new IpCollection[0];
                        }
                        j++;
                    }
                    i++;
                }
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("{0}", e));
            }
        }

        /// <summary>
        /// Информация об операционной системе
        /// </summary>
        private void GetSystemOSInformation()
        {
            foreach (ManagementObject mo in new ManagementClass(new ManagementPath("Win32_OperatingSystem"), null).GetInstances())
            {
                try
                {
                    computerName = String.Format("{0}", mo["CSName"]);
                    oSCaption = String.Format("{0}", mo["Caption"]);
                    oSVersion = String.Format("{0} {1}", mo["Version"], mo["CSDVersion"]);
                    oSManufacturer = String.Format("{0}", mo["Manufacturer"]);
                    oSBuildType = String.Format("{0}", mo["BuildType"]);
                    registeredUser = String.Format("{0}", mo["RegisteredUser"]);
                    organization = String.Format("{0}", mo["Organization"]);
                    serialNumber = String.Format("{0}", mo["SerialNumber"]);
                    installDate = GetInstallTime(String.Format("{0}", mo["InstallDate"]));
                    workingTime = GetWorkingTime(String.Format("{0}", mo["LastBootUpTime"]));
                    windowsDirectory = String.Format("{0}", mo["WindowsDirectory"]);
                    systemFolder = String.Format("{0}", mo["SystemDirectory"]);
                    systemDevice = String.Format("{0}", mo["SystemDevice"]);
                    totalVisibleMemorySize = GetMemorySize(mo["TotalVisibleMemorySize"].ToString());
                    freePhysicalMemory = GetMemorySize(String.Format("{0}", mo["FreePhysicalMemory"]));
                    totalVirtualMemorySize = GetMemorySize(String.Format("{0}", mo["TotalVirtualMemorySize"]));
                    freeVirtualMemory = GetMemorySize(String.Format("{0}", mo["FreeVirtualMemory"]));
                    oSLanguage = GetLanguage(String.Format("{0}", mo["OSLanguage"]));
                }
                catch (Exception e)
                {
                    throw new Exception(String.Format("Ошибка при извлечении параметра ОС: {0}", e));
                }
            }
        }

        private void GetSystemBIOSInformation()
        {
            foreach (ManagementObject mo in new ManagementClass(new ManagementPath("Win32_BIOS"), null).GetInstances())
            {
                try
                {
                    biosVersion = String.Format("{0}", mo["Version"]);
                }
                catch (Exception e)
                {
                    throw new Exception(String.Format("Ошибка при извлечении параметра BIOS: {0}", e));
                }
            }
        }

        private void GetComputerSystemInformation()
        {
            foreach (ManagementObject mo in new ManagementClass(new ManagementPath("Win32_ComputerSystem"), null).GetInstances())
            {
                try
                {
                    sysManufacturer = String.Format("{0}", mo["Manufacturer"]);
                    sysModel = String.Format("{0}", mo["Model"]);
                    systype = String.Format("{0}", mo["SystemType"]);
                    processorCount = int.Parse(mo["NumberOfProcessors"].ToString());
                    domain = String.Format("{0}", mo["Domain"]);
                }
                catch (Exception e)
                {
                    throw new Exception(String.Format("Ошибка при извлечении параметра системы: {0}", e));
                }
            }
        }

        /// <summary>
        /// Информация о процессорах
        /// </summary>
        private void GetProcessorsInformation()
        {
            try
            {
                ManagementObjectSearcher moSearch = new ManagementObjectSearcher(new ObjectQuery("Select Caption, CurrentClockSpeed, Manufacturer from Win32_Processor"));
                ManagementObjectCollection moReturn = moSearch.Get();

                processors = new Processor[processorCount];

                int i = 0;
                foreach (ManagementObject mo in moReturn)
                {
                    processors[i].number = i;
                    processors[i].caption = mo["Caption"].ToString().Trim();
                    processors[i].manufacturer = mo["Manufacturer"].ToString();
                    processors[i].speed = uint.Parse(mo["CurrentClockSpeed"].ToString());
                    i++;
                }
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("Ошибка при извлечении параметра процессора: {0}", e));
            }
        }

        /// <summary>
        /// Информация о часовом поясе
        /// </summary>
        private void GetTimeZoneInformation()
        {
            try
            {
                foreach (ManagementObject mo in new ManagementClass(new ManagementPath("Win32_TimeZone"), null).GetInstances())
                {
                    timeZone = String.Format("{0}", mo["Caption"]);
                }
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("Ошибка при извлечении параметра часового пояса: {0}", e));
            }
        }

        private void GetUpdatesInformation()
        {
            try
            {
                ManagementObjectSearcher moSearch = new ManagementObjectSearcher(new ObjectQuery("Select HotFixID, FixComments from Win32_QuickFixEngineering"));
                ManagementObjectCollection moReturn = moSearch.Get();

                updatesCount = moReturn.Count;
                updates = new Update[updatesCount];
                int i = 0;
                foreach (ManagementObject mo in moReturn)
                {
                    updates[i].number = i;
                    updates[i].hotFixID = String.Format("{0}", mo["HotFixID"]);
                    updates[i].fixComment = String.Format("{0}", mo["FixComments"]);
                    i++;
                }

            }
            catch (Exception e)
            {
                throw new Exception(String.Format("Ошибка при извлечении параметра исправления: {0}", e));
            }
        }

        #endregion

        #region Helper Func

        int GetMemorySize(string size)
        {
            return (int)(int.Parse(size) / 1024);
        }


        /// <summary>
        /// Время инсталяции системы
        /// </summary>
        /// <param name="installTime"></param>
        /// <returns></returns>
        string GetInstallTime(string installTime)
        {
            installTime = installTime.Split(".".ToCharArray())[0];

            DateTime installDateTime = GetTimeByString(installTime);

            return String.Format("{0}.{1}.{2}, {3}:{4}:{5}", installDateTime.Day, installDateTime.Month, installDateTime.Year, installDateTime.Hour, installDateTime.Minute, installDateTime.Second);
        }

        /// <summary>
        /// Время работы системы
        /// </summary>
        /// <param name="lastBoot"></param>
        /// <returns></returns>
        string GetWorkingTime(string lastBoot)
        {
            lastBoot = lastBoot.Split(".".ToCharArray())[0];

            DateTime lastBootTime = GetTimeByString(lastBoot);

            TimeSpan differens = DateTime.Now.Subtract(lastBootTime);

            return String.Format("{0}дн., {1}час., {2}мин., {3}сек.", differens.Days, differens.Hours, differens.Minutes, differens.Seconds);
        }

        /// <summary>
        /// Получаем datetime из строки
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        DateTime GetTimeByString(string str)
        {
            try
            {
                int year = int.Parse(str.Substring(0, 4));
                int month = int.Parse(str.Substring(4, 2));
                int day = int.Parse(str.Substring(6, 2));
                int hour = int.Parse(str.Substring(8, 2));
                int min = int.Parse(str.Substring(10, 2));
                int sec = int.Parse(str.Substring(12, 2));

                return new DateTime(year, month, day, hour, min, sec);
            }
            catch
            {
                throw new Exception("Ошибка при переводе!");
            }
        }
        /// <summary>
        /// Получение языка
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private string GetLanguage(string p)
        {
            try
            {
                using (RegistryKey lang = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Classes\\MIME\\Database\\Rfc1766", true))
                {
                    foreach (string str in lang.GetValueNames())
                    {
                        if (UInt32.Parse(str, NumberStyles.AllowHexSpecifier) == UInt32.Parse(p))
                            return lang.GetValue(str).ToString();
                    }
                }
                return "unknow";
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("При получения языка возникла ошибка: {0}", e));
            }
        }

        #endregion
    }
}
