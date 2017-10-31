<?php
	include_once ("reportInfo.php");
	include_once ("categoryInfo.php");
	include_once ("scripts.php");
	include_once ("helperDBData.php");
	
	class HostInfo 
	{
		//uri хоста
		private $_uri;
		//массив доступных категорий
		private $_availableCategories;
		//массив доступных отчетов для пользователя
		private $_availableReports;
		//id сессии
		private $_sessionID;
		//логин пользователя
		private $login;
		//хэшированный пароль пользователя
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
		
		//ID сессии
		private function getSessionID()
		{
			return $this->_sessionID;
		}
		private function setSessionID($value)
		{
			$this->_sessionID = $value;
		}
		
		//Конструктор класса NeighborServer
		function __construct()
		{
			$this->setUri("");
			$this->login = "";
			$this->password = "";
			$this->setSessionID("");
			$this->setAvailableCategories(array());
			$this->setAvailableReports(array());
		}

		//Загружаем свойства сервера
		function load($xmlNode)
		{
			if (isset($xmlNode))
			{
				$value = (string)($xmlNode->attributes()->uri);
				$this->setUri($value);
			}
		}

		//Инициализируем всю нужную информацию хоста
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

		//получаем id сессии
		function initSessionID($isCreateNew = true)
		{
			$address = $this->Uri . "getSessionID.php";
			$sessionID = $this->SessionID;
			if (!$isCreateNew)
				//если инициализируем старую сессию пошлем еще и список соответсвтующих параметров 
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
			//если id сессии инициализированно значит сессия валидна
			return $this->SessionID == '' ? false : true;
		}

		//авторизируемся
		private function loginToHost()
		{
			//сгенерим пароль для сесии
			$hostSessionPassword = getSessionPassword($this->password, $this->SessionID);
			$mobileDeviceType = $_SESSION["mobileDeviceType"];
			$isNewSnapshotMode = $_SESSION["isNewSnapshotMode"];
			$isNewSnapshotMode = $isNewSnapshotMode ? "TRUE" : "FALSE";
			$authorizedFixedGroups = getStrFixedGroupNames($_SESSION["userID"]);
			
			$ch = curl_init();
			//что бы не получилось рекурсии, соседнему серверу сообщаем, что не надо смотреть 
			//его соседнии сервера
			$data = "login=$this->login&password=$hostSessionPassword&mobileDeviceType=$mobileDeviceType&isLookNeighborHost=FALSE&isMobileUser=FALSE&isNewSnapshotMode=$isNewSnapshotMode";
			//если пользователь состоит в фиксированных группах, учтем это
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
		
		//получаем список доступных для пользовател отчетов
		function initAvailableUserReports()
		{	
			$url = $this->Uri . "availableUserReports.php?PHPSESSID=" . $this->SessionID;    	
			$availableReportsXml = simplexml_load_file($url);
			$this->parseAvailableUserRepots($availableReportsXml);
		}
		
		//смотрим собственные доступные отчеты
		function initSelfAvailableUserReports()
		{
			//поставим признак что надо сохранить результат в сессии
			$_SESSION['isSaveAvailableReportsXml'] = TRUE;
			//получаем в виде Xml
			require ("availableUserReports.php");
			//после получения результата уберем признак
			unset($_SESSION['isSaveAvailableReportsXml']);
			//вытащим из сессии результат
			$result = $_SESSION['availableReportsXml'];
			//и сразу сотрем его из сессии, там он больше не нужен
			unset($_SESSION['availableReportsXml']);
			
			$availableReportsXml = simplexml_load_string($result);
			$this->parseAvailableUserRepots($availableReportsXml);
		}
		
		//устанавливаем куку для данной сессии
		function setCookie($isForceSet)
		{
			$reportsCount = count($this->_availableReports);
			//устанавливать будем только в случае наличия у хоста отчетов для пользователя
			//или признака принудительной установки 
			if (($reportsCount > 0) || $isForceSet)
			{
				$uriParsed = parse_url($this->Uri);
				$path = $uriParsed['path'] == '' ? '/' : $uriParsed['path'];
				//$date = time() + 60 * 24;
				//из-за разности во времени у часовых поясов, выставляем не на определенное время
				//а пока не закроется браузер
				$date = 0;
				set_cookie("PHPSESSID", $this->SessionID, $date, $path, $uriParsed['host']);
			}
		}
		
		//Получить массив доступных отчетов для пользователей на этом хосте
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
		
		//колекция отчетов - пуста
		function isEmptyReports()
		{
			return (count($this->_availableReports) <= 0);
		}
		
		//проверяет, существует ли катергория с таким id  в списке доступных для пользователя
		function isExistsCategory($id)
		{
			return $this->getCategoryById($id) != null;
		}
		
		//проверяет, существует ли отчет с таким id в списке доступных для пользователя
		function isExistsReport($id)
		{
			return $this->getReportById($id) != null;
		}
	}
?>