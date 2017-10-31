<?php
	session_start();
	
	include_once ("hostInfo.php");
	
	if (!isset($_SESSION['userID']))
	{
		echo win1251ToUtf8("ѕользователь не прошел аутентификацию");
		exit();
	}
	
	$reportID = $_REQUEST['reportID'];
	$subjectID = $_REQUEST['subjectID'];
	
	//информаци€ о текущем сервере
	$currentHostInfo = unserialize(gzuncompress($_SESSION['currentHostInfo']));
	//получим исходный id отчет
	$sourceID = getSourceReportID($reportID);
	$isNewSnapshotMode = $_SESSION['isNewSnapshotMode'];
	$url = "";
	$report = $currentHostInfo->getReportById($sourceID);
	//если отчет расположен на этом сервере перешлем сразу на не него
	if ($report != null)
	{
		$reportsName = (isset($subjectID) && ($subjectID != "")) ? $reportID . "_" . $subjectID : $reportID;
		
		$url = $currentHostInfo->Uri . "Reports/";
		if ($isNewSnapshotMode)
		{
			$url .= "NewSnapshotMode/";
			$reportsName .= ".zip";
		}
		else
		{
			$url .= "OldSnapshotMode/";
			$reportsName .= ".php";
		}
		$url .= $report->CategoryID . "/" . $sourceID . "/" . $reportID . "/" . $reportsName;
	}
	//значи отчет на соседнем сервере, переадресуем на него
	else
	{
		include_once ("neighborHosts.php");
		if (isset($_SESSION['neighborHosts']))
		{
			$neighborHosts = unserialize(gzuncompress($_SESSION['neighborHosts']));
			foreach ($neighborHosts->hosts as $host)
			{
				if ($host->isExistsReport($sourceID))
				{
					$url = $host->Uri . "getReport.php?reportID=$reportID&subjectID=$subjectID";
					break;
				}
			}
			$_SESSION["neighborHosts"] = gzcompress(serialize($neighborHosts));
		}
	}
	
	//если ни один сервер не призналс€, что у него есть такой отчет, отправл€ем ошибку
	if ($url == "")
		$url = "error.html";
	
	//пока не пон€тное дл€ мен€ €вление, но на зеноновском хостинге,
	//после востановлени€ класса он удал€етс€ из сессии, по этому 
	//сейчас записываем его туда повторно
	$_SESSION["currentHostInfo"] = gzcompress(serialize($currentHostInfo));
	
	header("Location: $url");
?>