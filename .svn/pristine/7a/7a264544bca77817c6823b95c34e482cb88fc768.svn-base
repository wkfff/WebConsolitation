<?php
	include_once ("scripts.php");
	include_once (getConfigPath());

	//получаем тип устройства
	function getMobileDeviceType($strDeviceType)
	{	
		if (!isset($strDeviceType))
			return MobileDeviceTypes::IPHONE;
			
		switch ($strDeviceType)
		{
			case "All":
				return MobileDeviceTypes::All;
				
			case "IPad":
				return MobileDeviceTypes::IPAD;
				
			case "IPhone": 
				return MobileDeviceTypes::IPHONE;
			
			case "WM240x320":
				return MobileDeviceTypes::WM240x320;
						
			case "WM480x640": 
			case "WM480x800": 
				return MobileDeviceTypes::WM480x640;
			
			case "WM":
				return MobileDeviceTypes::WM_UNSUPPORTED;
		}
		return MobileDeviceTypes::IPHONE;
	}
	
	//Извлекаем из картежа отчета, информацию о типе устройства, для которого 
	//он был сгенерирован, по умолчанию это iPhone
	function extractDeviceTypeFromReport($reportRow)
	{
		$result = MobileDeviceTypes::IPHONE;
		if(isset($reportRow))
		{
			$document = $reportRow['Document'];
			if ($document)
			{
				$xml = stringToXml($document);
		
				$templateType = (string)$xml->TemplateType;
				$result = getMobileDeviceType($templateType);
			}
		}
		return $result;
	}
?>