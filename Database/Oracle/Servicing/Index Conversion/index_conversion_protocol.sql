/*
    ��� "������ � ������������"
    ������  2.8.0
    ������
        index_conversion_protocol.sql - ������ ��� ���������/��������� ���������
                           �������������� �������� � ����� NORMAL � ���� BITMAP.
    ����    Oracle 9i � ����
*/

/*
������� ��������:
      ������ ������������ ��� ���������/��������� ���������, ������������ 
    ���������� �������������� �������� (index_conversion_procedure.sql).
    �������� �������� � ������� A_BITMAP_INDEX_CONVERSION_LOG  

����������:
    ��� ������� ������� � SQL*Plus ���������� ����� ������ ��������� ���������:
    - ��� ���� ������
    - ������ � ����� DV (������ � �����-����������)
    - ��� ������������ �����
    - ������� �� ����� ������������ ��������� ������� � ��

��������� ������:
    ������� � ������� ����� �������� � ��������� ����
*/

host chcp 1251

SET ECHO OFF
SET VERIFY OFF
SET FEEDBACK OFF
WHENEVER SQLERROR EXIT ROLLBACK
WHENEVER OSERROR EXIT ROLLBACK

PROMPT ========================================================================
PROMPT ������ ��� ��������� ������ ��������� �������������� ��������
PROMPT ========================================================================

ACCEPT P_SERVER_NAME CHAR PROMPT '������� ��� ���� ������: '
ACCEPT P_PASSWORD_DV CHAR DEFAULT 'dv' PROMPT '������� ������ ��� ������������ DV (default dv): ' HIDE
ACCEPT P_LOG_FILE_NAME CHAR DEFAULT 'index_conversion[&P_SERVER_NAME].txt' PROMPT '������� ��� �����-��������� (default index_conversion[&P_SERVER_NAME].txt): '
ACCEPT P_DELETE_LOG_TABLE CHAR DEFAULT 'N' PROMPT '������� � �� ������� � ���������� ����� ��������� ������ ? (Y/N, default N): '


PROMPT 
PROMPT ������������ � �����-����������...
connect dv/&P_PASSWORD_DV@&P_SERVER_NAME

set serveroutput on;

PROMPT

SPOOL &P_LOG_FILE_NAME
SET LINES 100
--��������� ��������� job-�
declare 
 l_job_status varchar2(100);
 l_job_count number;
begin
  l_job_count := 0;
  dbms_output.put_line('��������� ������� �������: ');
  for cr_job in ( select J.job, J.broken, J.this_date, J.next_date, J.failures
                    from user_jobs J
                   where upper(what) like '%P_BITMAP_INDEX_CONVERSION%'
                 )
  loop
    l_job_status := case 
                      when cr_job.broken='Y' then '�������'
                      else 
                        case 
                          when cr_job.this_date is null then '������� ���������� � '||to_char(cr_job.next_date, 'DD.MM.YYYY HH24:MI:SS')
                                                              ||'  ����� ��������� �������: '||nvl(cr_job.failures,0)
                          else '�����������.'
                        end
                    end;
    
    dbms_output.put_line(' ������� �'||cr_job.job||' ���������: '||l_job_status);
    l_job_count := l_job_count+1;
  end loop cr_job;
  
  if l_job_count=0 then
    dbms_output.put_line('...������� �����������.');  
  end if;
end;
/

--��������� ������� �������
declare
  l_dummy number;
begin
  select 1 
    into l_dummy
    from user_tables T
   where T.table_name = 'A_BITMAP_INDEX_CONVERSION_LOG';
exception
  when no_data_found then
    raise_application_error(-20100,'�� ������� ������� � ����������.');
end;
/

PROMPT =========================================================================

COLUMN message         format a60      heading "���������"
COLUMN table_a_name    format a30      heading "������� ������"
COLUMN column_a_name   format a30      heading "������� ������� ������"
COLUMN table_b_name    format a30      heading "������� ����.�������������"
COLUMN constraint_fk_name format a30   heading "������� ����"
COLUMN index_fk_name   format a30      heading "������ �������� �����"
COLUMN index_size      format 999,999,999,999,999 heading "������ ������� (byte)"
COLUMN log_date        format a24      heading "���� ������"
SET LINES 275
SET PAGESIZE 50000
SET COLSEP |

select message, table_a_name, column_a_name,
       table_b_name, constraint_fk_name, index_fk_name, index_size, 
       to_char(log_date,'DD.MM.YYYY HH24:MI:SSxFF') log_date
  from a_bitmap_index_conversion_log 
order by log_date;

PROMPT =========================================================================

--�������� �������
declare
  l_drop_table boolean;
begin
  if upper('&P_DELETE_LOG_TABLE') = 'Y' 
    or upper('&P_DELETE_LOG_TABLE') = 'YES' 
  then
    execute immediate ('drop table a_bitmap_index_conversion_log');
    dbms_output.put_line('������� � ���������� ������� �� ��.');
  else
    dbms_output.put_line('������� � ���������� �� ������� �� ��.');
  end if;
end;
/

SPOOL OFF

PROMPT ������ �������� �������.

disconnect

quit

