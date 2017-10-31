<?php
	session_start();
	
	include_once ("scripts.php");
	include_once (getConfigPath());
	include_once ("scriptsDBData.php");

	ignore_user_abort(TRUE);
	set_time_limit(0);
	
	//0 - распаковать архив
	//1 - перенести отчеты из архива
	//2 - обновим информацю в базе данных
	$commandID = $_GET['commandID'];
	
	
	switch ($commandID) 
	{
		case 0:
			{
				initStartOperation();
				extractDataBurst();
				break;
			}
		case 1:
			{
				initStartOperation();
				updateReports();
				break;
			}
		case 2:
			{
				initStartOperation();
				updateDB();
				break;
			}
		case 10:
			{
				echo $_SESSION['RollOutOperationState'];
				break;
			}
		case 11:
			{
				echo $_SESSION['RollOutLog'];
				break;
			}
		case 12:
		{
			$_SESSION['RollOutOperationState'] = OperationState::READY;
			echo $_SESSION['RollOutOperationState'];
			break;
		}
	}
	
	//инициализация начала операции
	function initStartOperation()
	{
		$_SESSION['RollOutOperationState'] = OperationState::START;
		$_SESSION['RollOutLog'] = '';
	}
	
	//распакуем архив с пакетом данных
	function extractDataBurst()
	{
		error_reporting(E_ALL);
		include_once 'PEAR/Archive/Tar.php';
		try 
		{
			$_SESSION['RollOutOperationState'] = OperationState::EXECUTING;
			$_SESSION['RollOut'] .= "Удаляем старый бэкап архива.";
			removeDir(BACKUP_DATA_BURST_NAME);
			if (is_dir(DATA_BURST_NAME))
			{
				$_SESSION['RollOutLog'] .= "Делаем бэкап текущего архива.";
				rename(DATA_BURST_NAME, BACKUP_DATA_BURST_NAME);
			}
			$_SESSION['RollOutLog'] .= "Разархивируем пакет с данными.";
			$tar = new Archive_Tar(ARCHIVE_DATA_BURST_NAME, 'gz');
			$tar->extract('.');
			$_SESSION['RollOutOperationState'] = OperationState::SUCCESS_ENDED;
			echo SUCCESS_INDICATOR;
		}
		catch (Exception $e)
		{
			$_SESSION['RollOutLog'] .= $e->getMessage();
			$_SESSION['RollOutOperationState'] = OperationState::FAILED_ENDED;
		}
		error_reporting(0);
	}
	
	//из раcпакованого архива в корневую папку копируем новые отчеты
	function updateReports()
	{
		error_reporting(E_ALL);
		try 
		{
			$_SESSION['RollOutOperationState'] = OperationState::EXECUTING;
			if (isEmptyDir(DATA_BURST_REPORTS_PATH))
			{
				$_SESSION['RollOutLog'] .= "Отчетов в пакете нет, видимо обновляем только базу данных.";
			}
			//Если директроия с отчетами не пустая, то выполним все ниже следующие действия
			else
			{
				$_SESSION['RollOutLog'] .= "Удаляем старый бэкап отчетов.";
				removeDir(BACKUP_REPORTS_PATH);
				//echo "Делаем бэкап текущих отчетов.";
				//copyDir(REPORTS_PATH, BACKUP_REPORTS_PATH);
				$_SESSION['RollOutLog'] .= "Загружаем из пакета в рабочию директорию новые отчеты.";
				copyReport(DATA_BURST_REPORTS_PATH, REPORTS_PATH);
			}
			$_SESSION['RollOutOperationState'] = OperationState::SUCCESS_ENDED;
			echo SUCCESS_INDICATOR;
		}
		catch (Exception $e)
		{
			$_SESSION['RollOutLog'] .= $e->getMessage();
			$_SESSION['RollOutOperationState'] = OperationState::FAILED_ENDED;
		}
		error_reporting(0);
	}
	
	//обновим информацию в базе данных
	function updateDB()
	{
		error_reporting(E_ALL);
		$_SESSION['RollOutOperationState'] = OperationState::EXECUTING;
		$_SESSION['RollOutLog'] .= "Делаем бэкап существующей базы.";
		$error = renameDB(DB_NAME, BACKUP_DB_NAME);
		if ($error != "")
		{
			$_SESSION['RollOutLog'] .= $error;
			exit;
		}
		$_SESSION['RollOutLog'] .= "Создаем базу данных.";
		$error = createDataBase(DB_NAME);
		if ($error != "")
		{
			$_SESSION['RollOutLog'] .= $error;
			exit;
		}

		$_SESSION['RollOutLog'] .= "Выполним запросы для формирования и заполнения новой БД.";
		$query = file_get_contents(DATA_BURST_DATABASE_PATH . 'CreateExpImpTables.sql');
		$query .= file_get_contents(DATA_BURST_DATABASE_PATH . 'CreateSystemTables.sql');
		$query .= file_get_contents(DATA_BURST_DATABASE_PATH . 'DataBaseDump.sql');
		$_SESSION['RollOutLog'] .= "Переносим таблицу со статистикой из старой БД.";
		$query .= 'INSERT INTO ' . DB_NAME . '.Authorization_statistics SELECT * FROM ' . BACKUP_DB_NAME . '.Authorization_statistics;';
		$error = multiQuery($query);
		if ($error != "")
		{
			$_SESSION['RollOutLog'] .= $error;
			$_SESSION['RollOutOperationState'] = OperationState::FAILED_ENDED;
			exit;
		}
		$_SESSION['RollOutOperationState'] = OperationState::SUCCESS_ENDED;
		echo SUCCESS_INDICATOR;
		error_reporting(0);
	}
?>