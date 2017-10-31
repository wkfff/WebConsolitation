<?php
	session_start();
	include_once ("hostInfo.php");
	
	if (!isset($_SESSION['userID']))
	{
		echo win1251ToUtf8("Пользователь не прошел аутентификацию");
		exit();
	}
	
	$query = $_SERVER['QUERY_STRING'];
	$paramArray = array();
	parse_str($query, $paramArray);
	$command = $paramArray['command'];
	
	switch ($command) {
		case 'categoryIcon':
			{
				//информация о текущем сервере
				$currentHostInfo = unserialize(gzuncompress($_SESSION['currentHostInfo']));
				$id = $paramArray['id'];
				$category = $currentHostInfo->getCategoryById($id);
				if (isset($category))
				{
					echo $category->Icon;
				}
				else 
				{
					include_once ("neighborHosts.php");
					if (isset($_SESSION['neighborHosts']))
					{
						$neighborHosts = unserialize(gzuncompress($_SESSION['neighborHosts']));
						foreach ($neighborHosts->hosts as $host)
						{
							if ($host->isExistsCategory($id))
							{
								$category = $host->getCategoryById($id);
								echo $category->Icon;
								break;
							}
						}
						$_SESSION["neighborHosts"] = gzcompress(serialize($neighborHosts));
					}
				}
				
				$_SESSION["currentHostInfo"] = gzcompress(serialize($currentHostInfo));
				return;
			}
	}
?>