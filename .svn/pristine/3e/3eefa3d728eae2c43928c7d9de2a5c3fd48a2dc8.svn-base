<?php	

	function writeLog($text)
	{
		$text = "\r\n\r\n=====================" . date('d.m.Y H:i:s P') . "=====================\r\n" . $text;
		$filename = "log.txt";
		if(!file_exists($filename)) 
			$fh = fopen($filename, "w");
		else
			$fh = fopen($filename, "a");
			
		fwrite($fh, $text);	
		fclose($fh); 
	}

	//получить сумму символов (ASCI таблицы) строки 
	function getCharSumm($string)
	{
		$array = str_split($string);
		$sum = 0;
		foreach ($array as $char)
		{
			$sum += ord($char);
		}
		return $sum;
	}
	
	//Пулучаем произведение сумм пароля и сесии
	function getSessionPassword($password, $sessionID)
	{
		$passSum = getCharSumm($password);
		$sessionIdSum = getCharSumm($sessionID);
		return $passSum * $sessionIdSum;
	}
	
	//Перекодирует из win1251 в utf8
	function win1251ToUtf8($s)
	{
		$t = '';
		for($i=0, $m=strlen($s); $i<$m; $i++)
		{
			$c=ord($s[$i]);
			if ($c<=127)
			{
				$t.=chr($c); 
				continue; 
			}
			if ($c>=192 && $c<=207)
			{
				$t.=chr(208).chr($c-48); 
				continue; 
			}
			if ($c>=208 && $c<=239)
			{
				$t.=chr(208).chr($c-48); 
				continue; 
			}
			if ($c>=240 && $c<=255)
			{
				$t.=chr(209).chr($c-112); 
				continue; 
			}
			if ($c==184) 
			{ 
				$t.=chr(209).chr(209);
				continue; 
			}
			if ($c==168) 
			{ 
				$t.=chr(208).chr(129);
				continue; 
			}
		}
		return $t;
	}
	
	function getIP() 
	{
	  if (getenv("HTTP_CLIENT_IP") && strcasecmp(getenv("HTTP_CLIENT_IP"), "unknown"))
	     $ip = getenv("HTTP_CLIENT_IP");
	  elseif (getenv("HTTP_X_FORWARDED_FOR") && strcasecmp(getenv("HTTP_X_FORWARDED_FOR"), "unknown"))
	     $ip = getenv("HTTP_X_FORWARDED_FOR");
	  elseif (getenv("REMOTE_ADDR") && strcasecmp(getenv("REMOTE_ADDR"), "unknown"))
	     $ip = getenv("REMOTE_ADDR");
	  elseif (isset($_SERVER['REMOTE_ADDR']) && $_SERVER['REMOTE_ADDR'] && strcasecmp($_SERVER['REMOTE_ADDR'], "unknown"))
	     $ip = $_SERVER['REMOTE_ADDR'];
	  else
	     $ip = "unknown";
	     
	  $commaPos = strpos($ip, ',');
	  if ($commaPos > 0)
	  	$ip = substr($ip, 0, $commaPos);
	  	
	  return $ip;
	}
	
	function getCurrentUriPath()
	{
		$uri = "http://" . $_SERVER['HTTP_HOST'] . $_SERVER['REQUEST_URI'];
		$uriParts = pathinfo($uri);
		return $uriParts['dirname'] . '/';
	}
	
	function getHomeDir()
	{
		$pathParts = pathinfo($_SERVER['SCRIPT_FILENAME']);
		$homeDir = $pathParts['dirname'];
		return addSlashe($homeDir);
	}
	
	function getFullFilePath($fileName)
	{
		return getHomeDir() . $fileName;
	}
	
	function getConfigPath()
	{
		return getFullFilePath("config.php");
	}
	
	function getCurrentDir()
	{
		$path_parts = pathinfo($_SERVER['PHP_SELF']);
		$path = str_replace("/", "", $path_parts['dirname']);
		if (count($path) > 0)
			$path .= "/";
		echo $path;
	}
	
	function copyDir($from_path, $to_path) 
	{ 
 		copyDirEX($from_path, $to_path, '');
 	}
 	
 	function copyReport($from_path, $to_path)
 	{
 		copyDirEX($from_path, $to_path, '.files');
 	}
 	
 	//Расширеная функция копирования директорий, параметрами передаются 
 	//пути от куда и куда копировать, а так же часть названия директории,  
 	//при нахождении которой директория будет удалена, и только потом в нее скопируются свежие файлы
 	function copyDirEX($from_path, $to_path, $deletteeDirNamePart)
 	{
 	 	if (is_dir($from_path)) 
 		{
 			error_reporting(0);
 			//Если директория содержит deletteeDirNamePart, то сначала ее удалим
 			if (($deletteeDirNamePart != '') && (stripos($to_path, $deletteeDirNamePart) > 0))
 				removeDir($to_path);
 			mkdir($to_path, 0777);
 			error_reporting(E_ALL);
  			chdir($from_path); 
  			$handle = opendir('.'); 
  			while (($file = readdir($handle)) !== false) 
  			{
   				if (($file != ".") && ($file != "..")) 
   				{
    				if (is_dir($file)) 
    				{
     					copyDirEX($from_path . $file . "/" , $to_path . $file . "/", $deletteeDirNamePart); 
     					chdir($from_path); 
    				}
    				if (is_file($file)) 
    					copy($from_path.$file, $to_path.$file);
   				}
  			}
  			closedir($handle);
 		}
 	}
	
 	function isEmptyDir($path)
 	{
 		$result = true; 	
 		if (is_dir($path)) 
 		{
 			chdir($path);
 			$countFiles = count(glob("*"));
 			$result = ($countFiles == 0);
 		}
 		return $result;
 	}
 	
 	//Удаляет директорию
 	function removeDir($directory) 
 	{
		if (is_dir($directory))
		{
			$dir = opendir($directory);
			while(($file = readdir($dir)))
			{
				if (is_file($directory ."/". $file))
				{
					unlink($directory . "/" . $file);
				}
				elseif (is_dir($directory . "/" . $file) && ($file != ".") && ($file != ".."))
				{
					removeDir($directory . "/" . $file);
				}
			}
			closedir($dir);
			rmdir($directory);
		}
 	}
 	
 	//Установить Cookie
 	function set_cookie($name, $value, $expires = 0, $path = '/', $domain = '') 
	{
	    //$domain = ($_SERVER['REMOTE_ADDR'] == '127.0.0.1') ? false : $domain;
		$path = addSlashe($path);
	    setcookie($name, $value, $expires, $path, $domain);
	}
	
	//Добавляет в конец строки слэш, если его там нет
	function addSlashe($string)
	{
		if (($string != "") && (substr($string, -1) != '/'))
	    	$string .= '/';
	    return $string;
	}
	
	//получить исходный id отчета без признаков горизонтальности/вертикальности
	function getSourceReportID($id)
	{
		$startPostfix = strlen($id) - 2;
		$postfix = substr($id, $startPostfix);
		switch ($postfix) 
		{
			case "_V":
			case "_H":
				{
					return substr($id, 0, $startPostfix);
				}
			
			default:
				return $id;
		}
	}
	
	function stringToXml($str)
	{
		$reult = null;
		if (isset($str))
		{
			//в наших файлах в бд неоправданно ставится кодировка utf-16, 
			//что не нравится SimpleXML
			$str = str_replace('utf-16', 'utf-8', $str);
			$reult = new SimpleXMLElement($str);
		}
		return $reult;
	}
	
	/////////////////////////////////////////////////////////////////////////////////////
	//Функции для удаления дублирующихся отчетов на соседних серверах
	/////////////////////////////////////////////////////////////////////////////////////
	function validReports($currentHostInfo, $neighborHosts)
	{
		$existReportsId = array();
		deleteDuplicate(&$existReportsId, $currentHostInfo);
		
		if (!$neighborHosts->IsEmpty())
		{
			foreach ($neighborHosts->hosts as $host)
			{
				deleteDuplicate(&$existReportsId, $host);
			}
		}
	}
	
	function deleteDuplicate($existReportsId, $hostInfo)
	{
		$countReports = count($hostInfo->AvailableReports);
		for($i = 0; $i < $countReports; $i++)
		{
			$report = $hostInfo->AvailableReports[$i];
			if (isExistValue($existReportsId, $report->Id))
			{
				$hostInfo->deleteAvailableReport($i);
			}
			else 
			{
				$existReportsId[] = $report->Id;
			}
		}
	}
	
	function isExistValue($array, $value)
	{
		foreach ($array as $item)
		{
			if ($item == $value)
				return TRUE;
		}
		return FALSE;
	}
	
	function str_contains($str, $content, $ignorecase=true)
	{
		$retval = false;
		
		if ($ignorecase)
		{
			$str = strtolower($str);
			$content = strtolower($content);
		}
		
		$_strpos = strpos($str, $content);
		if ( $_strpos === 0 || $_strpos > 0 ) 
			$retval = true;
		
		return $retval;
	}
?>