<?php	
	include_once ("../scripts.php");
	/*//������ � �� �� �������
	define('DB_USER_NAME', 'root');
	define('DB_USER_PASSWORD', '982324');
	define('HOST', 'localhost');
	define('HOST_PORT', '');
	define('CHARACTER_SET_CLIENT', "set character_set_client='cp1251'");
	define('CHARACTER_SET_RESULTS', "set character_set_results='utf8'");
	define('COLLATION_CONNECTION', "set collation_connection='cp1251_general_ci'");*/
	
	
	//������ � �� �� www.imon.krista.ru
	define('DB_USER_NAME', 'root');
	define('DB_USER_PASSWORD', '3IvW2dVTQG3K');
	define('HOST_PORT', '63848');
	define('HOST', 'mysql.baze.krista.ru.postman.ru');
	define('CHARACTER_SET_CLIENT', "set character_set_client='cp1251'");
	define('CHARACTER_SET_RESULTS', "set character_set_results='utf8'");
	define('COLLATION_CONNECTION', "set collation_connection='utf8_general_ci'");
	
	//�������� ������� �����
	define('HOME_DIR', getHomeDir());
	
	//��� ���� ������
	//define('DB_NAME', 'imon');
	//define('DB_NAME', 'imon_novosib');
	define('DB_NAME', 'imon_samara');
	//define('DB_NAME', 'imon_yaroslavl');
	//define('DB_NAME', 'imon_test');
	//define('DB_NAME', 'imon_mincomsvyaz');
	
	//����� ����
	define('BACKUP_DB_NAME', DB_NAME . '_backup');
	
	//��� xml � ������� ����������� �������� �����
	define('NEIGHBOR_HOSTS_FILE_NAME', 'neighborHosts.xml');
	
	//��� ������������� ������ � ������������ �������
	define('ARCHIVE_DATA_BURST_NAME', 'DataBurst.tar.gz');
	//��� ������ � ������������ �������
	define('DATA_BURST_NAME', 'DataBurst');
	//����� ������� ������
	define('BACKUP_DATA_BURST_NAME', 'DataBurst_Backup');
	//��� ����� �� ��������� ��� �� � ������
	define('DATA_BURST_DATABASE_NAME', 'DataBase');
	//��� ����� � ��������
	define('REPORTS_FOLDER_NAME', 'Reports');
	//����� ����� �� ������� ��������
	define('BACKUP_REPORTS_FOLDER_NAME', 'Reports_Backup');
	//���� � ������� � ������ ������
	define('DATA_BURST_REPORTS_PATH', HOME_DIR . DATA_BURST_NAME . '/' . REPORTS_FOLDER_NAME . '/');
	//���� � ������� �� �����
	define('REPORTS_PATH', HOME_DIR . REPORTS_FOLDER_NAME . '/');
	//���� � ������ ������� �� �����
	define('BACKUP_REPORTS_PATH', HOME_DIR . BACKUP_REPORTS_FOLDER_NAME . '/');
	//���� � �������� ��� �� � ������ ������
	define('DATA_BURST_DATABASE_PATH', HOME_DIR . DATA_BURST_NAME . '/' . DATA_BURST_DATABASE_NAME . '/');
	
	//������� ��������� ���������� ��������
	define('SUCCESS_INDICATOR', 'successIndicator');
	//IP �� ������� �� ������� ����������
	define('IGNORE_STATTISTIC_IP', '217.15.145.70');
	//�������������
	define('ADMIN_USER', 'fm');
	
	//���� ��������� ���������
	class MobileDeviceTypes
	{
		const ALL = "All";
		const IPHONE = "IPhone";
		const IPAD = "IPad";
		const WM240x320 = "WM240x320";
		const WM480x640 = "WM480x640";
		const WM_UNSUPPORTED = "WM";
	}
	
	//��� ������ � ���� �����
	class RecordType
	{
		//���������
		const CATEGORY = 0;
		//�����
		const REPORT = 10;
	}
?>