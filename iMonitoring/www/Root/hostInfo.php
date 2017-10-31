<?php
	include_once ("reportInfo.php");
	include_once ("categoryInfo.php");
	include_once ("scripts.php");
	include_once ("helperDBData.php");
	
	class HostInfo 
	{
		//uri �����
		private $_uri;
		//������ ��������� ���������
		private $_availableCategories;
		//������ ��������� ������� ��� ������������
		private $_availableReports;
		//id ������
		private $_sessionID;
		//����� ������������
		private $login;
		//������������ ������ ������������
		private $password;

	    function __get($name)
	    {
	        $method = 'get'.ucfirst($name);
	        return method_exists($this, $method) ? $this->$method() : $this->{'_'.$name};
	    }
	    function __set($name, $value)
	    {
	        $method = 'set'.ucfirst($name);
	        if (method_exists($this, $method)) 
	        {
	            $this->$method($value);
	        }
	        else 
	        {
	            $this->{'_'.$name} = $value;
	        }
	    }

	    //URI
		private function getUri()
		{
			return $this->_uri;
		}
		private function setUri($value)
		{
			$this->_uri = $value;
		}

		//AvailableCategories
		function getAvailableCategories()
		{
			return $this->_availableCategories;
		}
		function setAvailableCategories($value)
		{
			$this->_availableCategories = $value;
		}
		function addAvailableCategory($categoryInfo)
		{
			$this->_availableCategories[] = $categoryInfo;
		}
		function deleteAvailableCategory($index)
		{
			unset($this->_availableCategories[$index]);
		}
		
		//AvailableReports
		function getAvailableReports()
		{
			return $this->_availableReports;
		}
		function setAvailableReports($value)
		{
			$this->_availableReports = $value;
		}
		function addAvailableReport($reportInfo)
		{
			$this->_availableReports[] = $reportInfo;
		}
		function deleteAvailableReport($index)
		{
			unset($this->_availableReports[$index]);
		}
		
		//ID ������
		private function getSessionID()
		{
			return $this->_sessionID;
		}
		private function setSessionID($value)
		{
			$this->_sessionID = $value;
		}
		
		//����������� ������ NeighborServer
		function __construct()
		{
			$this->setUri("");
			$this->login = "";
			$this->password = "";
			$this->setSessionID("");
			$this->setAvailableCategories(array());
			$this->setAvailableReports(array());
		}

		//��������� �������� �������
		function load($xmlNode)
		{
			if (isset($xmlNode))
			{
				$value = (string)($xmlNode->attributes()->uri);
				$this->setUri($value);
			}
		}

		//�������������� ��� ������ ���������� �����
		function initInfo($login, $password)
		{
			$this->login = $login;
			$this->password = $password;
			
			$this->initSessionID();
			
			if ($this->SessionID != "")
			{
				$this->loginToHost();
				$this->initAvailableUserReports();
			}
		}

		//�������� id ������
		function initSessionID($isCreateNew = true)
		{
			$address = $this->Uri . "getSessionID.php";
			$sessionID = $this->SessionID;
			if (!$isCreateNew)
				//���� �������������� ������ ������ ������ ��� � ������ ��������������� ���������� 
				$address .= "?isCreateNew=FALSE&PHPSESSID=$sessionID";
			error_reporting(0);
			try 
			{
				$sessionIdXml = simplexml_load_file($address);
				if ($sessionIdXml)
				{
					$sessionID = (string)($sessionIdXml->sessionID->attributes()->value);
				}
			}
			catch (Exception $e)
			{
				$e->getMessage();
				$sessionID = "";
			}
			error_reporting(E_ALL);
			$this->SessionID = $sessionID;
		}
		
		function checkValidSession()
		{
			$this->initSessionID(false);
			//���� id ������ ����������������� ������ ������ �������
			return $this->SessionID == '' ? false : true;
		}

		//��������������
		private function loginToHost()
		{
			//�������� ������ ��� �����
			$hostSessionPassword = getSessionPassword($this->password, $this->SessionID);
			$mobileDeviceType = $_SESSION["mobileDeviceType"];
			$isNewSnapshotMode = $_SESSION["isNewSnapshotMode"];
			$isNewSnapshotMode = $isNewSnapshotMode ? "TRUE" : "FALSE";
			$authorizedFixedGroups = getStrFixedGroupNames($_SESSION["userID"]);
			
			$ch = curl_init();
			//��� �� �� ���������� ��������, ��������� ������� ��������, ��� �� ���� �������� 
			//��� �������� �������
			$data = "login=$this->login&password=$hostSessionPassword&mobileDeviceType=$mobileDeviceType&isLookNeighborHost=FALSE&isMobileUser=FALSE&isNewSnapshotMode=$isNewSnapshotMode";
			//���� ������������ ������� � ������������� �������, ����� ���
			if ($authorizedFixedGroups != "")
				$data .= "&authorizedGroups=" . $authorizedFixedGroups;
				
			curl_setopt($ch, CURLOPT_URL, $this->Uri . "login_action.php?PHPSESSID=" . $this->SessionID);
			curl_setopt($ch, CURLOPT_RETURNTRANSFER, false);
			curl_setopt($ch, CURLOPT_HEADER, false);
			curl_setopt($ch, CURLOPT_POST, 1);
			curl_setopt($ch, CURLOPT_POSTFIELDS, $data);
			
			curl_exec($ch);
			curl_close($ch);
		}

		private function parseAvailableUserRepots($availableReportsXml)
		{		
			unset($this->AvailableCategories);
			unset($this->AvailableReports);
			if ($availableReportsXml) 
			{
				if ($availableReportsXml->categories)
				{
					foreach ($availableReportsXml->categories->category as $categoryXml)
					{
						$this->addAvailableCategory(new CategoryInfo($categoryXml));
					}
				}
				if ($availableReportsXml->reports)
				{
					foreach ($availableReportsXml->reports->report as $reportXml)
					{
						$this->addAvailableReport(new ReportInfo($reportXml));
					}
				}
			}
		}
		
		//�������� ������ ��������� ��� ����������� �������
		function initAvailableUserReports()
		{	
			$url = $this->Uri . "availableUserReports.php?PHPSESSID=" . $this->SessionID;    	
			$availableReportsXml = simplexml_load_file($url);
			$this->parseAvailableUserRepots($availableReportsXml);
		}
		
		//������� ����������� ��������� ������
		function initSelfAvailableUserReports()
		{
			//�������� ������� ��� ���� ��������� ��������� � ������
			$_SESSION['isSaveAvailableReportsXml'] = TRUE;
			//�������� � ���� Xml
			require ("availableUserReports.php");
			//����� ��������� ���������� ������ �������
			unset($_SESSION['isSaveAvailableReportsXml']);
			//������� �� ������ ���������
			$result = $_SESSION['availableReportsXml'];
			//� ����� ������ ��� �� ������, ��� �� ������ �� �����
			unset($_SESSION['availableReportsXml']);
			
			$availableReportsXml = simplexml_load_string($result);
			$this->parseAvailableUserRepots($availableReportsXml);
		}
		
		//������������� ���� ��� ������ ������
		function setCookie($isForceSet)
		{
			$reportsCount = count($this->_availableReports);
			//������������� ����� ������ � ������ ������� � ����� ������� ��� ������������
			//��� �������� �������������� ��������� 
			if (($reportsCount > 0) || $isForceSet)
			{
				$uriParsed = parse_url($this->Uri);
				$path = $uriParsed['path'] == '' ? '/' : $uriParsed['path'];
				//$date = time() + 60 * 24;
				//��-�� �������� �� ������� � ������� ������, ���������� �� �� ������������ �����
				//� ���� �� ��������� �������
				$date = 0;
				set_cookie("PHPSESSID", $this->SessionID, $date, $path, $uriParsed['host']);
			}
		}
		
		//�������� ������ ��������� ������� ��� ������������� �� ���� �����
		function getArrayReports()
		{
			$reportsCount = count($this->_availableReports);
			$result = array();
			if ($reportsCount > 0)
			{
				foreach ($this->_availableReports as $report)
				{
					$result[] = $report->Id;
				}
			}
			return $result;
		}
				
		
		function getCategoryById($id)
		{
			if (count($this->_availableCategories) > 0)
			{
				foreach ($this->_availableCategories as $category)
				{
					if ($id == $category->Id)
						return $category;
				}
			}
			return null;
		}
		
		function getReportById($id)
		{
			if (!$this->isEmptyReports())
			{
				foreach ($this->_availableReports as $report)
				{
					if ($id == $report->Id)
						return $report;
				}
			}
			return null;
		}
		
		//�������� ������� - �����
		function isEmptyReports()
		{
			return (count($this->_availableReports) <= 0);
		}
		
		//���������, ���������� �� ���������� � ����� id  � ������ ��������� ��� ������������
		function isExistsCategory($id)
		{
			return $this->getCategoryById($id) != null;
		}
		
		//���������, ���������� �� ����� � ����� id � ������ ��������� ��� ������������
		function isExistsReport($id)
		{
			return $this->getReportById($id) != null;
		}
	}
?>