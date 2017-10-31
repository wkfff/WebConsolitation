<?php
	$result = '';
	$phpSESSID = $_COOKIE['PHPSESSID'];
	if (!isset($phpSESSID))
		$phpSESSID = $_GET['PHPSESSID'];
		
	$isCreateNew = !isset($_GET['isCreateNew']) || ($_GET['isCreateNew'] == "TRUE");
	
	//������� ����� ������
	if ($isCreateNew)
	{
		if ($phpSESSID != "")
		{
			//���� ���������� ������ ������, ��������� ��
			session_start();
			session_destroy();
		}
		session_start();
		$result = session_id();
	}
	//�������� �������� ��� ������������ ������
	else 
	{
		if ($phpSESSID != "")
		{
			session_start();
			if ($_SESSION['isAuthentication'] == TRUE)
			{
				include_once ("helperDBData.php");
				//���� ������ ���������, ���������� ������
				if (isExistActualSessionTableEntry($phpSESSID))
				{
					$result = $phpSESSID;
					//���� ���� �������� �������, �������� ������������ ������ � �� ���
					if (isset($_SESSION['neighborHosts']))
					{
						include_once ("neighborHosts.php");
						$neighborHosts = unserialize(gzuncompress($_SESSION['neighborHosts']));
						if (count($neighborHosts->hosts) > 0)
						{
							foreach ($neighborHosts->hosts as $host)
							{
								if(!$host->isEmptyReports() && !$host->checkValidSession())
								{
									$result = '';
									break;
								}
							}
						}
						$_SESSION["neighborHosts"] = gzcompress(serialize($neighborHosts));
					}
					//�.�. �������� ���������� ���������� ��� ��������� ���������, ����� 
					//��������������� ��� ��� ��� ��� �����������
					if($result != '')
					{
						include_once ("helperDBData.php");
						include_once ("scripts.php");
						updateAuthorizationStatistics($_SESSION["userLogin"], getIP(), 
							$_SESSION["mobileDeviceType"], $_SESSION['appVersion']);
					}
				}
			}
		}
	}
	
	echo '<?xml version="1.0" encoding="UTF-8"?>';	
	echo '<iMonitoring>';
		echo '<sessionID value="' . $result . '" />';
	echo '</iMonitoring>';
?>