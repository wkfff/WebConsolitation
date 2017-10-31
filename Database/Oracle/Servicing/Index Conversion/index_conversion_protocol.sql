/*
    АИС "Анализ и планирование"
    ВЕРСИЯ  2.8.0
    МОДУЛЬ
        index_conversion_protocol.sql - Скрипт для получения/просмотра протокола
                           преобразования индексов с типом NORMAL к типу BITMAP.
    СУБД    Oracle 9i и выше
*/

/*
КРАТКОЕ ОПИСАНИЕ:
      Скрипт предназначен для просмотра/получения протокола, формируемого 
    процедурой преобразования индексов (index_conversion_procedure.sql).
    Протокол хранится в таблице A_BITMAP_INDEX_CONVERSION_LOG  

ВЫПОЛНЕНИЕ:
    При запуске скрипта в SQL*Plus необходимо будет ввести следующие параметры:
    - Имя базы данных
    - Пароль к схеме DV (пароль к схеме-контейнеру)
    - Имя формируемого файла
    - Удалить ли после формирования протокола таблицу в БД

РЕЗУЛЬТАТ РАБОТЫ:
    Листинг с данными будет сохранен в текстовый файл
*/

host chcp 1251

SET ECHO OFF
SET VERIFY OFF
SET FEEDBACK OFF
WHENEVER SQLERROR EXIT ROLLBACK
WHENEVER OSERROR EXIT ROLLBACK

PROMPT ========================================================================
PROMPT Скрипт для просмотра данных протокола преобразования индексов
PROMPT ========================================================================

ACCEPT P_SERVER_NAME CHAR PROMPT 'Введите имя базы данных: '
ACCEPT P_PASSWORD_DV CHAR DEFAULT 'dv' PROMPT 'Введите пароль для пользователя DV (default dv): ' HIDE
ACCEPT P_LOG_FILE_NAME CHAR DEFAULT 'index_conversion[&P_SERVER_NAME].txt' PROMPT 'Введите имя файла-протокола (default index_conversion[&P_SERVER_NAME].txt): '
ACCEPT P_DELETE_LOG_TABLE CHAR DEFAULT 'N' PROMPT 'Удалить в БД таблицу с протоколом после окончания работы ? (Y/N, default N): '


PROMPT 
PROMPT Подключаемся к схеме-контейнеру...
connect dv/&P_PASSWORD_DV@&P_SERVER_NAME

set serveroutput on;

PROMPT

SPOOL &P_LOG_FILE_NAME
SET LINES 100
--Проверяем состояние job-а
declare 
 l_job_status varchar2(100);
 l_job_count number;
begin
  l_job_count := 0;
  dbms_output.put_line('Состояние очереди заданий: ');
  for cr_job in ( select J.job, J.broken, J.this_date, J.next_date, J.failures
                    from user_jobs J
                   where upper(what) like '%P_BITMAP_INDEX_CONVERSION%'
                 )
  loop
    l_job_status := case 
                      when cr_job.broken='Y' then 'СЛОМАНО'
                      else 
                        case 
                          when cr_job.this_date is null then 'Ожидает выполнения в '||to_char(cr_job.next_date, 'DD.MM.YYYY HH24:MI:SS')
                                                              ||'  Число неудачных попыток: '||nvl(cr_job.failures,0)
                          else 'Выполняется.'
                        end
                    end;
    
    dbms_output.put_line(' ЗАДАНИЕ №'||cr_job.job||' Состояние: '||l_job_status);
    l_job_count := l_job_count+1;
  end loop cr_job;
  
  if l_job_count=0 then
    dbms_output.put_line('...задания отсутствуют.');  
  end if;
end;
/

--Проверяем наличие таблицы
declare
  l_dummy number;
begin
  select 1 
    into l_dummy
    from user_tables T
   where T.table_name = 'A_BITMAP_INDEX_CONVERSION_LOG';
exception
  when no_data_found then
    raise_application_error(-20100,'Не найдена таблица с протоколом.');
end;
/

PROMPT =========================================================================

COLUMN message         format a60      heading "Сообщение"
COLUMN table_a_name    format a30      heading "Таблица фактов"
COLUMN column_a_name   format a30      heading "Столбец таблицы фактов"
COLUMN table_b_name    format a30      heading "Таблица фикс.классификатор"
COLUMN constraint_fk_name format a30   heading "Внешний ключ"
COLUMN index_fk_name   format a30      heading "Индекс внешнего ключа"
COLUMN index_size      format 999,999,999,999,999 heading "Размер индекса (byte)"
COLUMN log_date        format a24      heading "Дата записи"
SET LINES 275
SET PAGESIZE 50000
SET COLSEP |

select message, table_a_name, column_a_name,
       table_b_name, constraint_fk_name, index_fk_name, index_size, 
       to_char(log_date,'DD.MM.YYYY HH24:MI:SSxFF') log_date
  from a_bitmap_index_conversion_log 
order by log_date;

PROMPT =========================================================================

--Удаление таблицы
declare
  l_drop_table boolean;
begin
  if upper('&P_DELETE_LOG_TABLE') = 'Y' 
    or upper('&P_DELETE_LOG_TABLE') = 'YES' 
  then
    execute immediate ('drop table a_bitmap_index_conversion_log');
    dbms_output.put_line('Таблица с протоколом удалена из БД.');
  else
    dbms_output.put_line('Таблица с протоколом не удалена из БД.');
  end if;
end;
/

SPOOL OFF

PROMPT Скрипт выполнен успешно.

disconnect

quit

