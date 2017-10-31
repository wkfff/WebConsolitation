<?php
	include_once ("hostInfo.php");
	
	//����� ��� ������ � ��������� ���������
	class NeighborHosts 
	{
		//��� �������� �������
		public $hosts;
		
		//�������� �� xml ���-� �������� ������� ����� ����������
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
		
		//������������� ���� ������
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
		
		//��������� ���� � ��������, ����� iPhon� - ��� �� �������� ������� ��������� 
		//��� �� "������"
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