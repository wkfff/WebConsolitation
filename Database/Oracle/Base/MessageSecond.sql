/*
	��� "������ � ������������"
	������	3.1
	������
		TablesCatalog.sql - ������ �������� ����������
	����	Oracle 9.2
*/

pro ==============================================================================================
pro ���������. ���������� ���� ���� �� �������� ���������� � ��������� ��������� �� ��������������
pro ==============================================================================================

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
    -- ��� ������������ ��� ���� ����� ������ ��������
    WHILE(groupsCursor%FOUND) LOOP
      -- ���� ����� �� ����������� ����������
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
      
      -- ���� ����� ������ �������������� �� ��������� ��������� �� ���� ���������
      IF (v_g.NAME = '��������������') THEN
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
      	-- ���� ����� �� ��������� ��������� �� �������������� �������
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
      DBMS_OUTPUT.PUT_LINE('�� �������� ������ Alter3_0_23.sql ��� �� ������������ ������ �������� �����');
    END; 
	
END;
/

COMMIT;