<?php
	require_once ("scripts.php");
	require_once ("helperDBData.php");

	session_start();
	
 	define('PASSWORD', 'barkovam');
 	//авторизация
 	if (!($_SESSION['isAuthorizedUser'] == "True"))
 	{
 		if (($_POST['login'] == PASSWORD) && ($_POST['pass'] == PASSWORD))
 		{
 			$_SESSION['isAuthorizedUser'] = "True";
 		}
 		else 
 		{
			header("Location: statistics_login.html");
 		}
 	}
?>
<html>
	<head>
		<script language="javascript" src="Scripts/calendar.js"></script>
		<script language="javascript" src="Scripts/calendar_conf.js"></script>
		<meta http-equiv="content-type" content="text/html; charset=UTF-8">
		<title>iMonitoring</title>
	</head>
	<body>
	<?php 
			echo '<h1 align="center">' . win1251ToUtf8('Статистика посещений') . '</h1>';
			$isConnectionCountCheck = ($_REQUEST['statisticsType'] == 'connectionCount') || !isset($_REQUEST['statisticsType']);
			echo '<h4>' . win1251ToUtf8('Период:') . '</h4>';
			echo '<form name="sampleform">';
				$startPeriod = isset($_REQUEST['firstinput']) ? $_REQUEST['firstinput'] : date("Y-m-d");
				$endPeriod = isset($_REQUEST['secondinput']) ? $_REQUEST['secondinput'] : date("Y-m-d");
				echo '<input type="button" value="C" onclick="javascript:showCal(\'Calendar1\')" /><input type="text" name="firstinput" size=20 value="'. $startPeriod . '" />';
				echo ' <input type="button" value="' . win1251ToUtf8('По') . '" onclick="javascript:showCal(\'Calendar2\')" /><input type="text" name="secondinput" size=20 value="' . $endPeriod . '"/>';
				
				$deviceType = isset($_REQUEST['deviceType']) ? $_REQUEST['deviceType'] : MobileDeviceTypes::ALL;
				echo '<h4>' . win1251ToUtf8('Тип устройства:') . '</h4>';
				echo '<p><select name="deviceType" style="width : 200">';
					$isSelected = ($deviceType == MobileDeviceTypes::ALL) ? "selected" : "";
					echo "<option $isSelected value=" . MobileDeviceTypes::ALL . ">All</option>"; 

					$isSelected = ($deviceType == MobileDeviceTypes::IPAD) ? "selected" : "";
					echo "<option $isSelected value=" . MobileDeviceTypes::IPAD . ">iPad</option>"; 
					
					$isSelected = ($deviceType == MobileDeviceTypes::IPHONE) ? "selected" : "";
					echo "<option $isSelected value=" . MobileDeviceTypes::IPHONE . ">iPhone</option>"; 
					
					$isSelected = ($deviceType == MobileDeviceTypes::WM240x320) ? "selected" : "";
					echo "<option $isSelected value=" . MobileDeviceTypes::WM240x320 . ">WM240x320</option>"; 
					
					$isSelected = ($deviceType == MobileDeviceTypes::WM480x640) ? "selected" : "";
					echo "<option $isSelected value=" . MobileDeviceTypes::WM480x640 . ">WM480x640</option>"; 
					
					$isSelected = ($deviceType == MobileDeviceTypes::WM_UNSUPPORTED) ? "selected" : "";
					echo "<option $isSelected value=" . MobileDeviceTypes::WM_UNSUPPORTED . ">WM_Unsupported</option>"; 
				echo '</select><p>';
				
				$isCountChecked = $isConnectionCountCheck ? 'checked="true"' : '';
				$isDateCheck =  $isConnectionCountCheck ? '' : 'checked="true"';
				echo "<p><input type='radio' $isCountChecked name='statisticsType' value='connectionCount'>" . win1251ToUtf8('Количество подключений');
				echo "<br><input type='radio' $isDateCheck name='statisticsType' value='dateStatistics'>" . win1251ToUtf8('Детальная информация');
				
				echo '<p><input type="submit" value="' . win1251ToUtf8('Запросить') . '">';
				//echo '<input type="submit" name="resetStatistics" value="' . win1251ToUtf8('Сбросить статистику') . '">';
			
				//если нажали очистить статистику, сделаем это
				if ($_REQUEST['resetStatistics'])
					resetAuthorizationStatistics();
	
				//добавляем один день к периоду, т.к. время дня начинается с 00:00:00 
				$endPeriod = date("Y-m-d", strtotime("+1 day", strtotime($endPeriod)));
				
				if ($isConnectionCountCheck)
				{
					if ($deviceType == MobileDeviceTypes::ALL)
					{
						PrintCountConnectionTable($startPeriod, $endPeriod, MobileDeviceTypes::ALL);
						PrintCountConnectionTable($startPeriod, $endPeriod, MobileDeviceTypes::IPAD);
						PrintCountConnectionTable($startPeriod, $endPeriod, MobileDeviceTypes::IPHONE);
						PrintCountConnectionTable($startPeriod, $endPeriod, MobileDeviceTypes::WM240x320);
						PrintCountConnectionTable($startPeriod, $endPeriod, MobileDeviceTypes::WM480x640);
					}
					else 
					{
						PrintCountConnectionTable($startPeriod, $endPeriod, $deviceType);
					}
				}
				else 
				{
					PrintDetailConnectionInfo($startPeriod, $endPeriod, $deviceType);
				}
			echo '</form>';
		echo '</body>';
	echo '</html>';
	
	function PrintCountConnectionTable($startPeriod, $endPeriod, $deviceType)
	{
		echo "<h4>$deviceType</h4>";
		$statistics = getAuthorizationCount($startPeriod, $endPeriod, $deviceType);
		$statisticsRow = mysql_fetch_assoc($statistics);
		if ($statisticsRow)
		{
			echo "<table BORDER=2 width=450px><tr><th>User name</th><th>Connection count</th><th>Last Connection</th></tr>";
			while ($statisticsRow)
			{
				echo '<tr><td>' . $statisticsRow['UserName'] . '</td><td>' . $statisticsRow['COUNT(UserName)'] . '</td><td>' . $statisticsRow['MAX(ConnectionDate)'] . '</td></tr>';
				$statisticsRow = mysql_fetch_assoc($statistics);
			}
			echo '</table>';
		}
		else 
		{
			echo "<p>Empty";
		}
	}
	
	function PrintDetailConnectionInfo($startPeriod, $endPeriod, $deviceType)
	{
		$statistics = getAuthorizationDate($startPeriod, $endPeriod, $deviceType);
		$statisticsRow = mysql_fetch_assoc($statistics);
		if ($statisticsRow)
		{
			echo "<table width=100%><tr>";
			echo "<td valign=top><table width=550px BORDER=2><tr><th>User name</th><th>Connection date</th><th>Device type</th><th>App version</th><th>View location</th></tr>";
			while ($statisticsRow)
			{
				$ip = $statisticsRow['UserIP'];
				echo '<tr><td>' . $statisticsRow['UserName'] . '</td><td>' . $statisticsRow['ConnectionDate'] . '</td><td>' . $statisticsRow['DeviceType'] . '</td><td>' . $statisticsRow['AppVersion'] . '</td><td align=center>'
					.'<input type="button" name="viewLocation" value="' . $ip . '" onClick="javascript:window.open(\'http://ip2geolocation.com/?ip=' . $ip . '\', \'displayWindow\', \'width=1050, height=900, status=no, toolbar=no, menubar=no\')" ></td></tr>';
				$statisticsRow = mysql_fetch_assoc($statistics);
			}
			echo '</table></td>';
			echo '</tr></table>';
		}
		else 
		{
			echo "<p>Empty";
		}
	}
?>