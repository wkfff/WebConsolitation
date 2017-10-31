<?php
	session_start();
	
	include_once ("hostInfo.php");
	
	if (!isset($_SESSION['userID']))
	{
		echo win1251ToUtf8("������������ �� ������ ��������������");
		exit();
	}
	
	$reportID = $_REQUEST['reportID'];
	$subjectID = $_REQUEST['subjectID'];
	
	//���������� � ������� �������
	$currentHostInfo = unserialize(gzuncompress($_SESSION['currentHostInfo']));
	//������� �������� id �����
	$sourceID = getSourceReportID($reportID);
	$isNewSnapshotMode = $_SESSION['isNewSnapshotMode'];
	$url = "";
	$report = $currentHostInfo->getReportById($sourceID);
	//���� ����� ���������� �� ���� ������� �������� ����� �� �� ����
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
	//����� ����� �� �������� �������, ������������ �� ����
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
	
	//���� �� ���� ������ �� ���������, ��� � ���� ���� ����� �����, ���������� ������
	if ($url == "")
		$url = "error.html";
	
	//���� �� �������� ��� ���� �������, �� �� ����������� ��������,
	//����� ������������� ������ �� ��������� �� ������, �� ����� 
	//������ ���������� ��� ���� ��������
	$_SESSION["currentHostInfo"] = gzcompress(serialize($currentHostInfo));
	
	header("Location: $url");
?>