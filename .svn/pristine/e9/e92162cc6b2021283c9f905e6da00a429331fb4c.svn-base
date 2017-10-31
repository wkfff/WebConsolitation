<?php	
	include_once ("../scripts.php");
	/*//доступ к БД на локалке
	define('DB_USER_NAME', 'root');
	define('DB_USER_PASSWORD', '982324');
	define('HOST', 'localhost');
	define('HOST_PORT', '');
	define('CHARACTER_SET_CLIENT', "set character_set_client='cp1251'");
	define('CHARACTER_SET_RESULTS', "set character_set_results='utf8'");
	define('COLLATION_CONNECTION', "set collation_connection='cp1251_general_ci'");*/
	
	
	//доступ к БД на www.imon.krista.ru
	define('DB_USER_NAME', 'root');
	define('DB_USER_PASSWORD', '3IvW2dVTQG3K');
	define('HOST_PORT', '63848');
	define('HOST', 'mysql.baze.krista.ru.postman.ru');
	define('CHARACTER_SET_CLIENT', "set character_set_client='cp1251'");
	define('CHARACTER_SET_RESULTS', "set character_set_results='utf8'");
	define('COLLATION_CONNECTION', "set collation_connection='utf8_general_ci'");
	
	//домашний каталог сайта
	define('HOME_DIR', getHomeDir());
	
	//имя базы данной
	//define('DB_NAME', 'imon');
	//define('DB_NAME', 'imon_novosib');
	define('DB_NAME', 'imon_samara');
	//define('DB_NAME', 'imon_yaroslavl');
	//define('DB_NAME', 'imon_test');
	//define('DB_NAME', 'imon_mincomsvyaz');
	
	//бэкап базы
	define('BACKUP_DB_NAME', DB_NAME . '_backup');
	
	//имя xml в которой указываются соседние хосты
	define('NEIGHBOR_HOSTS_FILE_NAME', 'neighborHosts.xml');
	
	//имя запакованного пакета с обновленными данными
	define('ARCHIVE_DATA_BURST_NAME', 'DataBurst.tar.gz');
	//имя пакета с обновленными данными
	define('DATA_BURST_NAME', 'DataBurst');
	//бэкап старого пакета
	define('BACKUP_DATA_BURST_NAME', 'DataBurst_Backup');
	//имя папки со скриптами для БД в пакете
	define('DATA_BURST_DATABASE_NAME', 'DataBase');
	//имя папки с отчетами
	define('REPORTS_FOLDER_NAME', 'Reports');
	//бэкап папки со старыми отчетами
	define('BACKUP_REPORTS_FOLDER_NAME', 'Reports_Backup');
	//путь к отчетам в пакете данных
	define('DATA_BURST_REPORTS_PATH', HOME_DIR . DATA_BURST_NAME . '/' . REPORTS_FOLDER_NAME . '/');
	//путь к отчетам на сайте
	define('REPORTS_PATH', HOME_DIR . REPORTS_FOLDER_NAME . '/');
	//путь к бэкапу отчетов на сайте
	define('BACKUP_REPORTS_PATH', HOME_DIR . BACKUP_REPORTS_FOLDER_NAME . '/');
	//путь к скриптам для БД в пакете данных
	define('DATA_BURST_DATABASE_PATH', HOME_DIR . DATA_BURST_NAME . '/' . DATA_BURST_DATABASE_NAME . '/');
	
	//признак успешного выполнения операции
	define('SUCCESS_INDICATOR', 'successIndicator');
	//IP по которым не ведется статистика
	define('IGNORE_STATTISTIC_IP', '217.15.145.70');
	//администратор
	define('ADMIN_USER', 'fm');
	
	//типы мобильных устройств
	class MobileDeviceTypes
	{
		const ALL = "All";
		const IPHONE = "IPhone";
		const IPAD = "IPad";
		const WM240x320 = "WM240x320";
		const WM480x640 = "WM480x640";
		const WM_UNSUPPORTED = "WM";
	}
	
	//Тип записи в базе даных
	class RecordType
	{
		//категория
		const CATEGORY = 0;
		//отчет
		const REPORT = 10;
	}
?>