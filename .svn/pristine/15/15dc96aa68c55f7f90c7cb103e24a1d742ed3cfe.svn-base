/********************************************************************
	Переводит базу Oracle из версии 3.0 в следующую версию 3.1 
********************************************************************/

/* Шаблон оформления альтера: */ 
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */ 
	/* Ваш SQL-скрипт */
/* End - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */ 


/* Start - Стандартная часть */ 

/* Выходим по любой ошибке */ 
whenever SQLError exit rollback; 
/* End   - Стандартная часть */ 


/* Start - Feature #21083 - Альтер на включение интерфейса "Сообщения" и подписка всех групп на сообщения от администратора системы - tsvetkov - <Дата: 16.04.2012> */

/*!!! Альтер необходим применить после перезапуска службы третьего звена !!! */

DECLARE
  objectMessageUI NUMBER;
  objectMessageFromAdmin NUMBER;
  objectAllMessage NUMBER;
  
  CURSOR groupsCursor IS
    SELECT ID, NAME FROM GROUPS;
  v_g groupsCursor%ROWTYPE;
BEGIN
  BEGIN
    SELECT id INTO objectMessageUI FROM objects o WHERE  objectkey = 'MessagesUI';
    SELECT id INTO objectAllMessage FROM objects o WHERE  objectkey = 'AllMessages';
    SELECT id INTO objectMessageFromAdmin FROM objects o WHERE  objectkey = '3133843A-10EE-424F-A4F1-80F403384CC6';
    
    DELETE FROM permissions WHERE refobjects IN (objectMessageUI, objectAllMessage, objectMessageFromAdmin) AND refgroups IS NOT NULL;
    
    OPEN groupsCursor;
    FETCH groupsCursor INTO v_g;
    -- для определенных или всех групп делаем операцию
    WHILE(groupsCursor%FOUND) LOOP
      -- даем права на отображение интерфейса
      INSERT INTO permissions
      (    
        refobjects,
        refgroups,
        refusers,
        allowedaction
      )
      VALUES
      (
        objectMessageUI,
        v_g.ID,
        NULL,
        3001
      );
      
      -- даем права группе Администраторы на получение ссобщений от всех подсистем
      IF (v_g.NAME = 'Администраторы') THEN
      	INSERT INTO permissions
      (    
        refobjects,
        refgroups,
        refusers,
        allowedaction
      )
      VALUES
      (
        objectAllMessage,
        v_g.ID,
        NULL,
        32001
      );
      ELSE 
      	-- даем права на получение сообщений от администратора системы
      	INSERT INTO permissions
      (    
        refobjects,
        refgroups,
        refusers,
        allowedaction
      )
      VALUES
      (
        objectMessageFromAdmin,
        v_g.ID,
        NULL,
        33001
      );
      END IF;
      FETCH groupsCursor INTO v_g;
    END LOOP;
    CLOSE groupsCursor;
    EXCEPTION
      WHEN no_data_found then
      DBMS_OUTPUT.PUT_LINE('Не применен альтер Alter3_0_23.sql или не перезапущена служба третьего звена');
    END; 
	
END;
/

COMMIT;

/* End - Feature #21083 - Альтер на включение интерфейса "Сообщения" и подписка всех групп на сообщения от администратора системы - tsvetkov - <Дата: 16.04.2012> */
