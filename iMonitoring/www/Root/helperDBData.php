<?php
	include_once ("scripts.php");
	include_once (getConfigPath());
	include_once ("scriptsDBData.php");
	
	dbConnect();
	
	//получить из таблицы свойств, значение по имени
	function getSettingValue($name)
	{
		if (!isset($name))
			return NULL;
		
		$sql = "SELECT value FROM Settings WHERE name='" . $name . "'";
		$result = mysql_query($sql);
		$row = mysql_fetch_assoc($result);
		return $row['value'];
	}
	
	//получить массив id групп, в которых состоит пользователь
	function getUserGroupsID($userID)
	{
		if (!isset($userID))
			return NULL;
		//получим id групп в которых состоит пользователь
		$sql = "SELECT RefGroups FROM Memberships WHERE RefUsers='" . $userID . "'";
		$result =  mysql_query($sql);	
		$row = mysql_fetch_assoc($result);
		$groupList = null;
		while ($row)
		{
			$groupList[] .= $row['RefGroups']; 	
			$row = mysql_fetch_assoc($result);
		}
	    return $groupList;
	}
	
	//если пользователь состоит в фиксированных группах, получим их
	function getFixedGroupNames($userID)
	{
		$result = null;
		$groupList = getUserGroupsID($userID);
		
		foreach ($groupList as $id)
		{
			switch ($id) {
				case MOBILE_GUEST_GROUP_ID:
				$result[] .= MOBILE_GUEST_GROUP_NAME;
				break;
				
				case MOBILE_VIP_GROUP_ID:
				$result[] .= MOBILE_VIP_GROUP_NAME;
				break;
			}
		}
		return $result;
	}
	
	function getStrFixedGroupNames($userID)
	{
		$groupList = getFixedGroupNames($userID);
		$result = "";
		foreach ($groupList as $name)
		{
			$result .= ($result != "") ? ', ' . $name : $name;
		}
		return $result;
	}
	
	//заменим имена фиксированных груп, на ID, т.к. они уникальны на каждом из серверах
	function replaceFixedGroupNamesToId($groupNameList)
	{
		$result = str_replace(MOBILE_GUEST_GROUP_NAME, MOBILE_GUEST_GROUP_ID, $groupNameList);
		$result = str_replace(MOBILE_VIP_GROUP_NAME, MOBILE_VIP_GROUP_ID, $result);
		return  $result;
	}
	
	function getUserGroups($userID)
	{
		if (!isset($userID))
			return NULL;
		
		if ($_SESSION['isAdmin'])
		{
			$sql = "SELECT * FROM Groups";
			return mysql_query($sql);
		}
		
		$groupsID = getUserGroupsID($userID);
		$groupList = "";
		foreach ($groupsID as $id)
		{
			$groupList .= ($groupList != "") ? ', ' . $id : $id;
		}

    	$sql = "SELECT * FROM Groups WHERE ID IN (" . $groupList . ")";
	    return mysql_query($sql);
	}
	
	//получить отчеты доступные пользователю
	function getUserReports($userID, $autorizedGroups)
	{
		if (!isset($userID))
			return NULL;
		
		if ($_SESSION['isAdmin'])
		{
			$sql = "SELECT * FROM Templates";
			return mysql_query($sql);
		}

		//получим список групп в которых состоит пользователь
		$groupsID = getUserGroupsID($userID);
		$groupList = $autorizedGroups;
		if ($groupsID)
		{
			foreach ($groupsID as $id)
			{
				$groupList .= ($groupList != "") ? ', ' . $id : $id;
			}
		}
		if ($groupList == "")
			$groupList = "''";

		$sql = "SELECT RefObjects FROM Permissions WHERE RefUsers = '$userID' OR RefGroups IN (" . $groupList . ")";
		$result =  mysql_query($sql);
		$row = mysql_fetch_assoc($result);
		$objectsList = "";
		while ($row)
		{
			$objectsList .= ($objectsList != "") ? ', ' . $row['RefObjects'] : $row['RefObjects'];	 	
			$row = mysql_fetch_assoc($result);
		}
		if ($objectsList == "")
			return NULL;
		
		$sql = "SELECT ObjectKey FROM Objects WHERE ID IN (" . $objectsList . ")";
		$result =  mysql_query($sql);
		$row = mysql_fetch_assoc($result);
		$reportList = "";
		while ($row)
		{
			$reportList .= ($reportList != "") ? ', ' . $row['ObjectKey'] : $row['ObjectKey'];	 	
			$row = mysql_fetch_assoc($result);
		}
		
    	$sql = "SELECT * FROM Templates WHERE ID IN (" . $reportList . ")";
	    return mysql_query($sql);
	}	
	
	//ведет статистику посещений пользователей
	function updateAuthorizationStatistics($userName, $userIP, $deviceType, $appVersion)
	{
		if ($userIP != IGNORE_STATTISTIC_IP)
		{
			$sql = "INSERT INTO Authorization_statistics (ID, UserName, UserIP, DeviceType, AppVersion, ConnectionDate) VALUES (NULL , '$userName', '$userIP', '$deviceType', '$appVersion', CURRENT_TIMESTAMP);";
			mysql_query($sql);
		}
	}
	
	//вернет количество посещений каждым из пользователей, и дату их последнего
	//посещения (данные ищутся в указанном перриоде), также можно указать тип устройства, 
	//для которого происходит выборка(можно не узазывать)
	function getAuthorizationCount($startPeriod, $endPeriod, $deviceType)
	{
		//строка с фильтром для подсчета посещений для конкретного устройства
		$deviceTypeFilter = isset($deviceType) && ($deviceType != MobileDeviceTypes::ALL) ? 
			"DeviceType = '$deviceType' AND " : "";
		$sql = "SELECT UserName, COUNT(UserName), MAX(ConnectionDate)  FROM Authorization_statistics WHERE $deviceTypeFilter ConnectionDate >= '$startPeriod' AND ConnectionDate <= '$endPeriod' GROUP BY UserName";
		return mysql_query($sql);
	}
	
	//вернет время посещения пользователей, также можно указать тип устройства, 
	//для которого происходит выборка(можно не узазывать)
	function getAuthorizationDate($startPeriod, $endPeriod, $deviceType)
	{
		//строка с фильтром для подсчета посещений для конкретного устройства
		$deviceTypeFilter = isset($deviceType) && ($deviceType != MobileDeviceTypes::ALL) ? 
			"DeviceType = '$deviceType' AND " : "";
		$sql = "SELECT UserName, UserIP, DeviceType, AppVersion, ConnectionDate FROM Authorization_statistics WHERE $deviceTypeFilter ConnectionDate >= '$startPeriod' AND ConnectionDate <= '$endPeriod' ORDER BY ConnectionDate DESC";
		return mysql_query($sql);
	}
	
	//сбросить статистику о посещении пользователей
	function resetAuthorizationStatistics()
	{
		$sql = "TRUNCATE TABLE Authorization_statistics";
	    mysql_query($sql);
	}
	/*
	//создать таблицу с актуальными сессиями
	function createActualSessionsTable()
	{
		$sql = "CREATE TABLE Actual_sessions (ID VARCHAR(50) NULL DEFAULT NULL , UNIQUE (ID)) ENGINE = MYISAM ;";
		mysql_query($sql);
	}*/
	
	//вставим запись в таблицу актуальных сессий
	function updateActualSession($sessionID)
	{
		$sql = "INSERT INTO Actual_sessions (ID) VALUES ('$sessionID');";
		mysql_query($sql);
	}
	
	//существует ли запись в таблице актуальных сессий
	function isExistActualSessionTableEntry($sessionID)
	{
		$sql = "SELECT ID FROM Actual_sessions WHERE ID = '$sessionID'";
		$result = mysql_query($sql);
		return mysql_num_rows($result) > 0 ? TRUE : FALSE;
	}
?>