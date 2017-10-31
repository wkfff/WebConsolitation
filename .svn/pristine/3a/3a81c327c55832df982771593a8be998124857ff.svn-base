<?php
	class ReportInfo 
	{
		//id отчета
		private $_id;
		//id категории
		private $_categoryID;
		//название отчета
		private $_name;
		//субъектно зависимый ли отчет
		private $_subjectDependence;
		//дата обновления отчета
		private $_updateDate;
		//признак что отчет яляется детализирующим
		private $_isDetalization;
		//признак что отчет отображается на весь экран
		private $_isFullScreen;
		//признак что отчет не скроллируемый
		private $_isNotScrollable;
	
	    function __get($name)
	    {
	        $method = 'get'.ucfirst($name);
	        return method_exists($this, $method)
	                ? $this->$method()
	                : $this->{'_'.$name};
	    }
	    function __set($name, $value)
	    {
	        $method = 'set'.ucfirst($name);
	        if (method_exists($this, $method)) {
	            $this->$method($value);
	        } else {
	            $this->{'_'.$name} = $value;
	        }
	    }
		
	    //Id
		function getId()
		{
			return $this->_id;
		}
		function setId($value)
		{
			$this->_id = $value;
		}
		
		//СategoryID
		function getCategoryID()
		{
			return $this->_categoryID;
		}
		function setCategoryID($value)
		{
			$this->_categoryID = $value;
		}
		
		//Имя
		function getName()
		{
			return $this->_name;
		}
		function setName($value)
		{
			$this->_name = $value;
		}

		//Субъектно ли зависимый отчет
		function getSubjectDependence()
		{
			return $this->_subjectDependence;
		}
		function setSubjectDependence($value)
		{
			$this->_subjectDependence = $value;
		}
		
		//Дата обновления
		function getUpdateDate()
		{
			return $this->_updateDate;
		}
		function setUpdateDate($value)
		{
			$this->_updateDate = $value;
		}

		//Признак что отчет яляется детализирующим
		function getIsDetalization()
		{
			return $this->_isDetalization;
		}
		function setIsDetalization($value)
		{
			$this->_isDetalization = $value;
		}

		//Признак что отчет отображается на весь экран
		function getIsFullScreen()
		{
			return $this->_isFullScreen;
		}
		function setIsFullScreen($value)
		{
			$this->_isFullScreen = $value;
		}		
		
		//Признак что отчет не скроллируемый
		function getIsNotScrollable()
		{
			return $this->_isNotScrollable;
		}
		function setIsNotScrollable($value)
		{
			$this->_isNotScrollable = $value;
		}
		
		//Конструктор класса
		function __construct($xmlNode)
		{
			if (isset($xmlNode))
				$this->load($xmlNode);
		}
		
		//Загружаем свойства сервера
		function load($xmlNode)
		{
			if (isset($xmlNode))
			{
				//id
				$value = (string)($xmlNode->attributes()->id);
				$this->setId($value);
				//categoryID
				$value = (string)($xmlNode->attributes()->categoryID);
				$this->setCategoryID($value);
				//имя
				$value = (string)($xmlNode->attributes()->name);
				$this->setName($value);
				//субъектно ли зависимый
				$value = (string)($xmlNode->attributes()->subjectDependence);
				$this->setSubjectDependence($value);
				//дата обновления
				$value = (string)($xmlNode->attributes()->updateDate);
				$this->setUpdateDate($value);
				//признак что отчет яляется детализирующим
				$value = (string)($xmlNode->attributes()->isDetalization);
				$this->setIsDetalization($value);
				//признак что отчет отображается на весь экран
				$value = (string)($xmlNode->attributes()->isFullScreen);
				$this->setIsFullScreen($value);
				//признак что отчет не скроллируемый
				$value = (string)($xmlNode->attributes()->isNotScrollable);
				$this->setIsNotScrollable($value);
			}
		}
	}
?>