<?php
	include_once ("scripts.php");
	include_once (getConfigPath());
	
	//����������� � �����
	function hostConnect($host=HOST, $port=HOST_PORT, $user=DB_USER_NAME, $password=DB_USER_PASSWORD)
	{
		if ($port != '')
			$host .= ":" . $port;
		if (!mysql_connect($host, $user, $password))
			die('I cannot connect to db: ' . mysql_error());
		
		mysql_query(CHARACTER_SET_CLIENT);
		mysql_query(CHARACTER_SET_RESULTS);
		mysql_query(COLLATION_CONNECTION);
	}
	
	//����������� � ��
	function dbConnect($host=HOST, $port=HOST_PORT, $user=DB_USER_NAME, $password=DB_USER_PASSWORD, $db=DB_NAME)
	{
		hostConnect($host, $port, $user, $password);
		mysql_select_db($db);
	}
	
	//���������� �� �� � ������� ������
	function isExistDataBase($dbName)
	{
		hostConnect();
		//������ ��� �� �������
		$dataBaseList = mysql_list_dbs();
		while ($dataBase = mysql_fetch_row($dataBaseList))
		{
			$dataBaseStr = $dataBase[0];
	        if ($dataBaseStr == $dbName)
				return TRUE;
	    }
	    return FALSE;
	}
	
	//��������������� ��
	function renameDB($oldName, $newName)
	{
		hostConnect();
		//���� �� ����� ���� ��� ����������� �������
		if (!isExistDataBase($oldName))
		{
			return "";
		}
		//���� ���� ���� ������ � ������ ������� �� ����� ����, ������ ��
		$sqlForRename = 'DROP DATABASE IF EXISTS `' . $newName . '`;';
		mysql_query($sqlForRename);
		if (mysql_error() != "")
			return mysql_error();
		//�������� ����� ��
		$sqlForRename = 'CREATE DATABASE `' . $newName . '` DEFAULT CHARACTER SET cp1251 COLLATE cp1251_general_ci;';
		mysql_query($sqlForRename);
		if (mysql_error() != "")
			return mysql_error();
		//������ ������ � ���� ������
		$listTables = mysql_list_tables($oldName);
		while ($tableName = mysql_fetch_row($listTables)) 
		{
			$tableNameStr = $tableName[0];
	        $sqlForRename = 'RENAME TABLE `' . $oldName . '`.`' . $tableNameStr . '` TO `' . $newName . '`.`' . $tableNameStr . '`;';
	        mysql_query($sqlForRename);
	        if (mysql_error() != "")
				return mysql_error();
		}
	    //������ ������ ��
		$sqlForRename = 'DROP DATABASE `' . $oldName . '`;';
		mysql_query($sqlForRename);
		return mysql_error();
	}
	
	function createDataBase($name)
	{
		hostConnect();
		$query = 'CREATE DATABASE IF NOT EXISTS `' . $name . '` DEFAULT CHARACTER SET cp1251 COLLATE cp1251_general_ci;';

		mysql_query($query);
		return mysql_error();
	}
	
	function copyTable($fromTable, $toTable)
	{
		hostConnect();
		$query = 'INSERT INTO ' . $fromTable . ' SELECT * FROM ' . $toTable;
		
		mysql_query($query);
		return mysql_error();
	}
	
	//��������� ������������� ������
	function multiQuery($query, $host=HOST, $port=HOST_PORT, $user=DB_USER_NAME, $password=DB_USER_PASSWORD, $db=DB_NAME)
	{
		if (HOST_PORT == '')
			$mysqli = new mysqli($host, $user, $password, $db);
		else
			$mysqli = new mysqli($host, $user, $password, $db, $port);
		$mysqli->multi_query(CHARACTER_SET_CLIENT);
		$mysqli->multi_query(CHARACTER_SET_RESULTS);
		$mysqli->multi_query(COLLATION_CONNECTION);

		if (mysqli_connect_errno())
		{
		    printf("Connect failed: %s\n", mysqli_connect_error());
		    exit();
		}
		$mysqli->multi_query($query);
		$result = $mysqli->error;
		$mysqli->close();
		return $result;
	}
?>