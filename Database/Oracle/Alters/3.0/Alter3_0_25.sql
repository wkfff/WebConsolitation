/********************************************************************
	��������� ���� Oracle �� ������ 3.0 � ��������� ������ 3.1 
********************************************************************/

/* ������ ���������� �������: */ 
/* Start - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */ 
	/* ��� SQL-������ */
/* End - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */ 


/* Start - ����������� ����� */ 

/* ������� �� ����� ������ */ 
whenever SQLError exit rollback; 
/* End   - ����������� ����� */ 


/* Start - Feature #20206 - ������ �� ���������� �������������� �������.������� ���. ������ ���� �������� � Seconds �������. - tsvetkov - 10.02.2012') */

begin
  DBMS_SESSION.SET_IDENTIFIER('script_identifier');
end;
/

begin
  -- Call the procedure
  dvcontext.setvalue('SESSIONID',
                     'script_session',
                     'DV',
                      'script_identifier');
end;
/
begin
  -- Call the procedure
  dvcontext.setvalue('USERNAME',
                     'script_user',
                     'DV',
                     'script_identifier');
end;
/


SELECT SYS_CONTEXT ('DVContext', 'USERNAME'), SYS_CONTEXT('DVContext', 'SESSIONID') from dual;

delete from d_Date_WorkingDays;
commit;

DECLARE
   CURSOR DaysWorkingCursor
     IS SELECT CAST(TO_CHAR(DayoffDate, 'YYYYMMDD') AS NUMBER) as DayoffDate 
     FROM d_Fact_Holidays 
    WHERE DayOff = 0;
   CURSOR ConversionFKCursor
     IS SELECT RefFODate 
     FROM d_Date_ConversionFK con
    WHERE NOT EXISTS ( SELECT NULL 
                       FROM d_Fact_Holidays 
                        WHERE TO_CHAR(DayoffDate, 'YYYYMMDD') = CAST(con.RefFODate AS VARCHAR2(8))
                                AND DayOff = 1 )
     AND TO_CHAR(RefFODate) >= (SELECT TO_CHAR(MIN(DayoffDate), 'YYYYMMDD') FROM d_Fact_Holidays)
     AND TO_CHAR(RefFODate) <= (SELECT TO_CHAR(MAX(DayoffDate), 'YYYYMMDD') FROM d_Fact_Holidays);
   ------------------------------------------------------------

BEGIN
   FOR r IN ConversionFKCursor LOOP
         INSERT INTO d_Date_WorkingDays 
           ( DaysWorking )
           VALUES ( r.RefFODate );
   END LOOP;
   FOR r IN DaysWorkingCursor LOOP
         INSERT INTO d_Date_WorkingDays 
           ( DaysWorking )
           VALUES ( r.DayoffDate );
   END LOOP;
   ------------------------------------------------------------
   -- ������ ���������
   DELETE d_Date_WorkingDays 

    WHERE ( id NOT IN ( SELECT MAX(t2.id) 
                        FROM d_Date_WorkingDays  t2
                          GROUP BY t2.DaysWorking )
           )
            AND ( id NOT IN ( SELECT MAX(t2.id) 
                              FROM d_Date_WorkingDays  t2
                                GROUP BY t2.DaysWorking )
           ); 
     
END;
/

commit;

INSERT INTO DatabaseVersions ( ID, NAME, Released, Updated, Comments, NeedUpdate )
    VALUES ( 120, '3.0.0.25', To_Date('10.02.2012', 'dd.mm.yyyy'), SYSDATE, '���������� �������������� �������.������� ���"', 0 );
commit;

/* 'End - Feature #20206 - ������ �� ���������� �������������� �������.������� ��� - tsvetkov - 10.02.2012');*/ 
   

