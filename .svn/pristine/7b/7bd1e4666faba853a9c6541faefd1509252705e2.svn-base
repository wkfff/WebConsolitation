<?php
	class CategoryInfo 
	{
		//id отчета
		private $_id;
		//им€
		private $_name;
		//описание
		private $_description;
		//признак что категори€ €л€етс€ детализирующей
		private $_isDetalization;
		//иконка
		private $_icon;
		//дата обновлени€
		private $_updateDate;
	
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
		
		//»м€
		function getName()
		{
			return $this->_name;
		}
		function setName($value)
		{
			$this->_name = $value;
		}
		
		//ќписание
		function getDescription()
		{
			return $this->_description;
		}
		function setDescription($value)
		{
			$this->_description = $value;
		}
		
		//»конка
		function getIcon()
		{
			return $this->_icon;
		}
		function setIcon($value)
		{
			$this->_icon = $value;
		}
		
		//ѕризнак что категори€ €л€етс€ детализирующей
		function getIsDetalization()
		{
			return $this->_isDetalization;
		}
		function setIsDetalization($value)
		{
			$this->_isDetalization = $value;
		}
		
		//ƒата обновлени€
		function getUpdateDate()
		{
			return $this->_updateDate;
		}
		function setUpdateDate($value)
		{
			$this->_updateDate = $value;
		}
		
		// онструктор класса
		function __construct($xmlNode)
		{
			if (isset($xmlNode))
				$this->load($xmlNode);
		}
		
		//«агружаем свойства сервера
		function load($xmlNode)
		{
			if (isset($xmlNode))
			{
				//id
				$value = (string)($xmlNode->attributes()->id);
				$this->setId($value);
				//им€
				$value = (string)($xmlNode->attributes()->name);
				$this->setName($value);
				//описание
				$value = (string)($xmlNode->attributes()->description);
				$this->setDescription($value);
				//признак что категори€ €л€етс€ детализирующей
				$value = (string)($xmlNode->attributes()->isDetalization);
				$this->setIsDetalization($value);
				//иконка
				$value = (string)($xmlNode->attributes()->iconByte);
				$this->setIcon($value);
				//дата обновлени€
				$value = (string)($xmlNode->attributes()->updateDate);
				$this->setUpdateDate($value);
			}
		}
	}
?>