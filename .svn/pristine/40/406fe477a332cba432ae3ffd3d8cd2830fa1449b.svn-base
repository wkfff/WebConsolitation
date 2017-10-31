/********************************************************************
	Переводит базу Oracle из версии 3.0 в следующую версию 3.1 
********************************************************************/

/* Шаблон оформления альтера: */ 
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */ 
	/* Ваш SQL-скрипт */
/* End - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */ 


/* Start - Стандартная часть */ 

/* Выходим по любой ошибке */ 
:on error exit
/* End   - Стандартная часть */ 


/* Start - Feature #21083 - Альтер на включение интерфейса "Сообщения" и подписка всех групп на сообщения от администратора системы - tsvetkov - <Дата: 16.04.2012> */

/*!!! Альтер необходим применить после перезапуска службы третьего звена !!! */

DECLARE @objectMessageUI INT,  @objectMessageFromAdmin INT, @objectAllMessage int;
  
DECLARE groupsCursor CURSOR FOR
    SELECT ID, NAME FROM GROUPS;
DECLARE  @curId INT, @curName VARCHAR(30);

BEGIN

	SELECT @objectMessageUI = id FROM objects o WHERE  objectkey = 'MessagesUI';
	SELECT @objectAllMessage = id FROM objects o WHERE  objectkey = 'AllMessages';
	SELECT @objectMessageFromAdmin = id FROM objects o WHERE  objectkey = '3133843A-10EE-424F-A4F1-80F403384CC6';

	IF (@objectMessageUI IS NULL)
	BEGIN
		PRINT('Не применен альтер Alter3_0_23.sql или не перезапущена служба третьего звена')
		RETURN
	END

	IF (@objectAllMessage IS NULL)
	BEGIN
		PRINT('Не применен альтер Alter3_0_23.sql или не перезапущена служба третьего звена')
		RETURN
	END

	IF (@objectMessageFromAdmin IS NULL)
	BEGIN
		PRINT('Не применен альтер Alter3_0_23.sql или не перезапущена служба третьего звена')
		RETURN
	END

	DELETE FROM permissions WHERE refobjects IN (@objectMessageUI, @objectAllMessage, @objectMessageFromAdmin) AND refgroups IS NOT NULL;

	OPEN groupsCursor;
	FETCH NEXT FROM groupsCursor INTO @curId, @curName;
	-- для определенных или всех групп делаем операцию
	WHILE(@@FETCH_STATUS <> -1) 
	BEGIN
      PRINT('Обрабатываем группу: ' + CAST (@curId AS VARCHAR) + ' ' + @curName)
	  -- даем права на отображение интерфейса
	  INSERT INTO g.permissions default values;
	  DELETE FROM g.permissions WHERE ID = @@IDENTITY

	  INSERT INTO permissions
	  (   
	  	id, 
		refobjects,
		refgroups,
		refusers,
		allowedaction
	  )
	  VALUES
	  (
	  	@@identity,
		@objectMessageUI,
		@curID,
		NULL,
		3001
	  );
	  
	  -- даем права группе Администраторы на получение ссобщений от всех подсистем
	  IF (@curName = 'Администраторы')
	  BEGIN
	   	INSERT INTO g.permissions default values;
	    DELETE FROM g.permissions WHERE ID = @@IDENTITY
  		INSERT INTO permissions
	    (    
	    	id,
			refobjects,
			refgroups,
			refusers,
			allowedaction
	    )
		VALUES
		(
			@@identity,
			@objectAllMessage,
			@curId,
			NULL,
			32001
		)	 
		END 
	  ELSE 
	  	BEGIN

  			-- даем права на получение сообщений от администратора системы
  			INSERT INTO g.permissions default values;
			DELETE FROM g.permissions WHERE ID = @@IDENTITY
  			INSERT INTO permissions
			(    
	  			id,
				refobjects,
				refgroups,
				refusers,
				allowedaction
			)
			VALUES
			(
	  			@@identity,
				@objectMessageFromAdmin,
				@curId,
				NULL,
				33001
			);
		END;
	  FETCH NEXT FROM groupsCursor INTO  @curId, @curName;
	END;
	CLOSE groupsCursor;

END;

GO

/* End - Feature #21083 - Альтер на включение интерфейса "Сообщения" и подписка всех групп на сообщения от администратора системы - tsvetkov - <Дата: 16.04.2012> */
