<?php
	session_start();
	include_once ("helperDBData.php");
	include_once ("scripts.php");
	include_once ("helperDeviceType.php");
	define ( 'DETALIZATION', 'detalization' );
	
	$result = "";
	$result .= '<?xml version="1.0" encoding="UTF-8"?>';
	$result .= '<iMonitoring>';
	//если в сессии есть список групп в которых пользователь авторизирован, учтем это
	$autorizedGroups = isset($_SESSION['authorizedGroups']) ? $_SESSION['authorizedGroups'] : "";
	//заменим имена фиксированных груп, на ID, т.к. они уникальны на каждом из серверах
	$autorizedGroups = replaceFixedGroupNamesToId($autorizedGroups);

	$userReports = getUserReports ( $_SESSION ['userID'], $autorizedGroups);
	$isNewSnapshotMode = $_SESSION ["isNewSnapshotMode"];
	if ($userReports) {
		//тип устройства, от которого идет запрос
		$mobileDeviceType = $_SESSION ["mobileDeviceType"];
		$categoryCodes = array ();
		$result .= echoCategories ( $mobileDeviceType, $userReports, &$categoryCodes, $isNewSnapshotMode );
		//возвратим точку поиска обратно
		mysql_data_seek ( $userReports, 0 );
		$result .= echoReports ( $mobileDeviceType, $userReports, $categoryCodes, $isNewSnapshotMode );
	}
	$userInfo = $_SESSION ['userInfo'];
	
	if (isset ( $userInfo )) {
		$result .= '<info text="' . $userInfo . '" />';
	}
	$result .= '</iMonitoring>';

	//если в сессии стоит признак сохранить в xml доступные отчеты, то сделаем это, иначе отдадим браузеру
	if ($_SESSION ['isSaveAvailableReportsXml'])
		$_SESSION ['availableReportsXml'] = $result;
	else
		echo $result;
		
	//добавим категории
	function echoCategories($mobileDeviceType, $userReports, $categoryCodes, $isNewSnapshotMode) {
		$categoriesRow = mysql_fetch_assoc ( $userReports );
		$result = '<categories>';
		while ( $categoriesRow ) {
			$rowType = $categoriesRow ['Type'];
			//в данную секцию должны попасть только категории
			if ($rowType == RecordType::CATEGORY) {
				//!!!оставим категории только для запрашиваемого устройства
				if ($mobileDeviceType == extractDeviceTypeFromReport ( $categoriesRow )) {
					//для того, что бы небыло разногласий, уникальные имена категорий всегда в нижнем регистре
					$id = strtolower ( $categoriesRow ['ID'] );
					$code = strtolower ( $categoriesRow ['Code'] );
					$name = $categoriesRow ['Name'];
					$description = $categoriesRow ['Description'];
					$categoryCodes [$id] = $code;
					
					$document = $categoriesRow ['Document'];
					if ($document) {
						$xml = stringToXml ( $document );
						$icon = $xml->IconByte;
						$timeSpan = strtotime ( $xml->LastDeployDate );
						$updateDate = date ( 'd.m.Y H:i:s P', $timeSpan );
					}
					//если в название категории присутствует ключевое слова "detalization"
					//значит она детализирующая
					$isDetalization = str_contains ( $code, DETALIZATION ) ? "YES" : "NO";
					
					//версии клиента с новым режимом загрузки отдаем все категории,
					//старые клиенты ничего не знают от детализирующих категориях, их не отдаем
					if ($isNewSnapshotMode || (! $isNewSnapshotMode && ($isDetalization == "NO"))) {
						$result .= '<category id="' . $code . '" name="' . $name . '" description="' . $description . '" updateDate="' . $updateDate . '" isDetalization="' . $isDetalization . '" iconByte="' . $icon . '" />';
					}
				}
			}
			$categoriesRow = mysql_fetch_assoc ( $userReports );
		}
		$result .= '</categories>';
		return $result;
	}
	
	//добавим отчеты
	function echoReports($mobileDeviceType, $userReports, $categoryCodes, $isNewSnapshotMode) {
		$reportsRow = mysql_fetch_assoc ( $userReports );
		$result = '<reports>';
		while ( $reportsRow ) {
			$rowType = $reportsRow ['Type'];
			//в данную секцию должны попасть только отчеты
			if ($rowType == RecordType::REPORT) {
				//!!!оставим отчеты только для запрашиваемого устройства
				if ($mobileDeviceType == extractDeviceTypeFromReport ( $reportsRow )) {
					//для того, что бы небыло разногласий, уникальные имена отчетов всегда в нижнем регистре
					$uniqueName = strtolower ( $reportsRow ['Code'] );
					$name = $reportsRow ['Name'];
					$categoryID = $reportsRow ['ParentID'];
					$categoryCode = $categoryCodes [$categoryID];
					
					$document = $reportsRow ['Document'];
					if ($document) {
						$xml = stringToXml ( $document );
						$subjectDependence = $xml->SubjectDepended;
						$subjectDependence = ($subjectDependence == "false") ? "NO" : "YES";
						$isNotScrollable = $xml->IsNotScrollable;
						$isNotScrollable = (($isNotScrollable == "false") || ($isNotScrollable == "")) ? "NO" : "YES";
						$timeSpan = strtotime ( $xml->LastDeployDate );
						if (!$timeSpan)
							//если не удалось узнать время, ставим текущее
							$updateDate = date ( 'd.m.Y H:i:s P' );
						else
							$updateDate = date ( 'd.m.Y H:i:s P', $timeSpan );
					} else {
						$subjectDependence = "NO";
						$isNotScrollable = "false";
						$updateDate = date ( 'd.m.Y H:i:s P' );
					}
					
					//если в название категории присутствует ключевое слова "detalization"
					//значит все отчеты принадлежещие ей, тоже детализирующие
					$isDetalization = str_contains ( $categoryCode, DETALIZATION ) ? "YES" : "NO";
					
					//отображать отчет на весь экран
					$isFullScreen = ($uniqueName == 'fo_0035_0005') || ($uniqueName == 'fo_0035_0005_horizontal') || ($uniqueName == 'it_0001_0004') || ($uniqueName == 'it_0001_0004_horizontal') || ($uniqueName == 'it_0001_0004_white') || ($uniqueName == 'it_0001_0004_white_horizontal') || ($uniqueName == 'se_0001_0001') || ($uniqueName == 'se_0001_0001_horizontal') || ($uniqueName == 'fo_0035_0016') || ($uniqueName == 'fo_0035_0016_horizontal') || ($uniqueName == 'fo_0035_0017') || ($uniqueName == 'fo_0035_0017_horizontal') || ($uniqueName == 'fo_0035_0018') || ($uniqueName == 'fo_0035_0018_horizontal') || ($uniqueName == 'fo_0035_0019') || ($uniqueName == 'fo_0035_0019_horizontal');
					$isFullScreen = $isFullScreen ? "YES" : "NO";
					
					//версии клиента с новым режимом загрузки отдаем все отчеты,
					//старые клиенты ничего не знают от детализирующих отчетах, их не отдаем
					if ($isNewSnapshotMode || (! $isNewSnapshotMode && ($isDetalization == "NO"))) {
						$result .= '<report id="' . $uniqueName . '" categoryID="' . $categoryCode . '" name="' . $name . '" subjectDependence="' . $subjectDependence . '" updateDate="' . $updateDate . '" isDetalization="' . $isDetalization . '" isFullScreen="' . $isFullScreen . '" isNotScrollable="' . $isNotScrollable . '" />';
					}
				}
			}
			$reportsRow = mysql_fetch_assoc ( $userReports );
		}
		$result .= '</reports>';
		return $result;
	}
?>