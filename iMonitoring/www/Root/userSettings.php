<?php
	session_start();
	
	include_once ("neighborHosts.php");

	$request = '<?xml version="1.0" encoding="UTF-8"?>';	
	$request .= '<iMonitoring>';
		$request .= '<settings>';
			$request .= '<startPage url="getReport.php" />';
			$request .= '<notSuccessfulHTMLText text="' . win1251ToUtf8("The system cannot find the file specified.|The resource cannot be found.|«апрошенный ресурс не может быть доставлен|Ќевозможно отобразить страницу|ќшибка выполнени€|Server Error in|NAME=&quot;ROBOTS&quot;|Object not found!|was not found on this server.") . '" />';
		$request .= '</settings>';

		if (isset($_SESSION["currentHostInfo"]))
			$currentHostInfo = unserialize(gzuncompress($_SESSION["currentHostInfo"]));
		if (isset($_SESSION["neighborHosts"]))
			$neighborHosts = unserialize(gzuncompress($_SESSION["neighborHosts"]));
			
		//категории
		$request .= echoCategoriesSection($currentHostInfo, $neighborHosts);
		//отчеты
		$request .= echoReportsSection($currentHostInfo, $neighborHosts);
			
		$userInfo = $_SESSION['userInfo'];
		if (isset($userInfo))
		{
			$request .= '<info text="' . $userInfo . '" />';
		}
		
		$isAuthentication = ((bool)$_SESSION['isAuthentication']) ? "TRUE" : "FALSE";
		$request .= '<isAuthentication value="' . $isAuthentication . '" />';
	$request .= '</iMonitoring>';
	
	$isNewSnapshotMode = $_SESSION['isNewSnapshotMode'];
	
	if ($isNewSnapshotMode)
		//дл€ новых версий будем сжимать поток данных
		echo gzcompress($request);
	else
		echo $request;
	
	//пока не пон€тное дл€ мен€ €вление, но на зеноновском хостинге,
	//после востановлени€ класса он удал€етс€ из сессии, по этому 
	//сейчас записываем его туда повторно
	if (isset($currentHostInfo))
		$_SESSION["currentHostInfo"] = gzcompress(serialize($currentHostInfo));
	if (isset($neighborHosts))
		$_SESSION["neighborHosts"] = gzcompress(serialize($neighborHosts));

	//отдадим доступные категории
	function echoCategoriesSection($currentHostInfo, $neighborHosts)
	{
		$result = '<categories rowHeight="135" >';
		if ($currentHostInfo)
			$result .= echoAvailableCategories($currentHostInfo);
		
		if ($neighborHosts)
		{
			if (!$neighborHosts->IsEmpty())
			{
				foreach ($neighborHosts->hosts as $host)
				{
					$result .= echoAvailableCategories($host);
				}
			}
		}
		$result .=  '</categories>';
		return $result;
	}
	
	//отдадим доступные отчеты
	function echoReportsSection($currentHostInfo, $neighborHosts)
	{
		$result = '<reports>';
		if ($currentHostInfo)
			$result .= echoAvailableReports($currentHostInfo);
		
		if ($neighborHosts)
		{
			if (!$neighborHosts->IsEmpty())
			{
				foreach ($neighborHosts->hosts as $host)
				{
					$result .= echoAvailableReports($host);
				}
			}
		}
		$result .=  '</reports>';
		return $result;
	}
	
	//добавим отчеты
	function echoAvailableReports($hostInfo)
	{
		$result = ''; 
		if ($hostInfo)
		{
			$availableReports = $hostInfo->AvailableReports;
			foreach ($availableReports as $report)
			{
				$result .= '<report id="' . $report->Id . 
					'" categoryID="' . $report->CategoryID . 
					'" name="' . $report->Name . 
					'" subjectDependence="' . $report->SubjectDependence . 
					'" updateDate="' . $report->UpdateDate . 
					'" isDetalization="' . $report->IsDetalization . 
					'" isFullScreen="' . $report->IsFullScreen . 
					'" isNotScrollable="' . $report->IsNotScrollable . '" />';
			}
		}
		return $result;
	}
	
	//добавим категории
	function echoAvailableCategories($hostInfo)
	{
		$result = ''; 
		if ($hostInfo)
		{
			$availableCategories = $hostInfo->AvailableCategories;
			foreach ($availableCategories as $category)
			{
				$result .= '<category id="' . $category->Id . 
					'" name="' . $category->Name . 
					'" description="' . $category->Description . 
					'" updateDate="' . $category->UpdateDate . 
					'" isDetalization="' . $category->IsDetalization .
					'" iconSize="' . strlen($category->Icon) . '" />';
			}
		}
		return $result;
	}	
?>