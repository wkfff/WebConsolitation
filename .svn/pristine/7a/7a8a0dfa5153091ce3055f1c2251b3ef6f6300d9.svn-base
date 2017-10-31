<?php
	include_once ("hostInfo.php");
	
	//Класс для работы с соседними серверами
	class NeighborHosts 
	{
		//все соседние сервера
		public $hosts;
		
		//загрузим из xml урл-ы серверов которые будем опрашивать
		function loadHosts($settingsPath)
		{
			$xml = simplexml_load_file($settingsPath);
			if (isset($xml))
			{
				foreach ($xml->host as $item)
				{
					$host = new HostInfo();
					$host->load($item);
					$this->hosts[] = $host;
				}
			}
		}
		
		//инициализация всех хостов
		function initHosts($login, $password)
		{	
			if (!$this->IsEmpty())
			{
				foreach ($this->hosts as $host)
				{
					$host->initInfo($login, $password);
				}
			}
		}
		
		//установим куки с сессиями, нужны iPhonу - что бы соседние сервера принимали 
		//его за "своего"
		function setCookies()
		{
			if (!$this->IsEmpty())
			{
				foreach ($this->hosts as $host)
				{
					$host->setCookie(FALSE);
				}
			}
		}
		
		function IsEmpty()
		{
			return (count($this->hosts) == 0);
		}
	}
?>