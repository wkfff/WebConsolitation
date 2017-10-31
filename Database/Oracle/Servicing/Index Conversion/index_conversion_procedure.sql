/*
    ��� "������ � ������������"
    ������  2.8.0
    ������
        index_conversion_procedure.sql - ������ ��� �������������� �������� 
                                                � ����� NORMAL � ���� BITMAP.
    ����    Oracle 9i � ����
*/

/*
������� ��������:
      ������ ��� �������������� �������� � ����� NORMAL � ���� BITMAP (���� �������).
    �������������� ������������ ������ �������, ��������� ��� ������� ������ ("����������")
    � ������-������ �� �������-�������������_��������������.
    �������������� �������� ������ ��� ������ ���� Oracle Enterprise Edition
    ���� �������������� - �������� ����������� ������� ��������, ��������� �������������� ��������.
    ���������� ������ ����������� � ������� ����� A_BITMAP_INDEX_CONVERSION_LOG.
      �������������� ����� - �� ������ ���������� �� ����������� ����������� ��������� ��,
    ���������� ������������� ������� ����� � �������. ���������� - ��� �� � ���������.

����������:
    ��� ������� ������� � SQL*Plus ���������� ����� ������ ��������� ���������:
    - ��� ���� ������
    - ������ � ����� SYS (��� �������������� ����������� ���������� �����-����������)
    - ������ � ����� DV (������ � �����-����������)
    - �������� ����� ������� ������� �� ����������
    - ��� �������������� (NORMAL->BITMAP ��� BITMAP->NORMAL)
    
    � ���������� ������ ������� ������� ����� ������� ��������� �������:
    - ������� ��������� A_BITMAP_INDEX_CONVERSION_LOG
    - ��������� P_BITMAP_INDEX_CONVERSION
    - ������� (job), ������� �������� ������������� � �������� �����
    
    ����� ��������� ������� ��������� ����� ������������� �������.
    � �� ��������� ������ ������� � ����������.
    
    ������ ������� �� ���������� ����������. ����� ���������� ��������������� ���������.

��������� ������:
    ��� ��������� ����������� �������������� �������� index_conversion_protocol.sql
    ���������: ������������ ������ ���������� ������� ����� �������� ������������ 
    ����� (��������� �����), ��� �� ������ ����������� ��������� ������
*/

host chcp 1251

SET ECHO OFF
SET VERIFY OFF
SET FEEDBACK OFF
WHENEVER SQLERROR EXIT ROLLBACK
WHENEVER OSERROR EXIT ROLLBACK

PROMPT ========================================================================
PROMPT ������ ��� �������� ��������� �������������� �������� � ���� bitmap
PROMPT ========================================================================

ACCEPT P_SERVER_NAME CHAR PROMPT '������� ��� ���� ������: '
ACCEPT P_PASSWORD_SYS CHAR DEFAULT 'sys' PROMPT '������� ������ ��� ������������ SYS (default sys): ' HIDE
ACCEPT P_PASSWORD_DV CHAR DEFAULT 'dv' PROMPT '������� ������ ��� ������������ DV (default dv): ' HIDE
ACCEPT P_JOB_START_TIME DATE FORMAT 'HH24:MI' DEFAULT '22:00' PROMPT '������� ����� ������� ������� � ������� ��:�� (default 22:00): '
ACCEPT P_UNDO_BITMAP CHAR DEFAULT 'No' PROMPT '�������� ��������� ����� ��������� BITMAP-��������? (Y/N, default N): '

PROMPT ������������ � �������...
connect sys/&P_PASSWORD_SYS@&P_SERVER_NAME as sysdba

grant execute on dbms_lock to dv;
PROMPT ����������� ���������� �������������

PROMPT 
PROMPT ������������ � �����-����������...
connect dv/&P_PASSWORD_DV@&P_SERVER_NAME

set serveroutput on

--�������� ������� �����
PROMPT ������� ������� ��� ���������...
declare
  E_TABLE_EXISTS EXCEPTION;
  pragma exception_init(E_TABLE_EXISTS,-00955);
begin
  execute immediate ( 'create table a_bitmap_index_conversion_log 
                        ( message varchar2(2000 char),
                          table_a_name varchar2(30),
                          column_a_name varchar2(30),
                          table_b_name varchar2(30),
                          constraint_fk_name varchar2(30),
                          index_fk_name varchar2(30),
                          index_size number(15), 
                          log_date timestamp(3) with local time zone
                         )'
                     );
  dbms_output.put_line('������� �������.');
exception
  when E_TABLE_EXISTS then
    dbms_output.put_line('������� � ����� ������ ��� ����������');
end;
/



PROMPT ������� ���������...
--����� ������������� ��������� ���������� �� ��������� 
--(���� ��������� � ������ ������ ����������� - ��� ����� �������������)
declare
  c_lock_name constant varchar2(128):='BITMAP INDEX CONVERSION';
  c_LOCK_STATUS_SUCCESS constant integer := 0;
  c_LOCK_STATUS_OWNED constant integer := 4;
  l_lock_handle varchar2(128);
  l_lock_status integer;
begin
  dbms_lock.allocate_unique( lockname => c_lock_name
                            ,lockhandle => l_lock_handle
                            );
  l_lock_status:=dbms_lock.request( lockhandle => l_lock_handle
                                   ,lockmode => dbms_lock.x_mode
                                   ,timeout => 0
                                   ,release_on_commit => true
                                   );
  if l_lock_status in (c_LOCK_STATUS_SUCCESS,c_LOCK_STATUS_OWNED) then
    null; --ok
  else
    raise_application_error(-20100, '� ������ ������ ��������� ����������� � ������ ������. ���������� �����.');
  end if;
  
end;
/

alter session set NLS_NUMERIC_CHARACTERS = '.,'
/

CREATE OR REPLACE 
PROCEDURE p_bitmap_index_conversion (p_in_undo_bitmap_conversion in varchar2 default 'N') is
  --��������� �������������� �������� � bitmap �� ������ ���������� �� ��
  type T_Constraint_List is table of varchar2(30);
  l_constraint_name_list T_Constraint_List;
  l_index_fk_name_list T_Constraint_List;
  l_table_a_name_list T_Constraint_List;
  l_table_b_name_list T_Constraint_List;
  i number;

  l_table_a_name varchar2(30);
  l_column_a_name varchar2(30);
  l_table_b_name varchar2(30);

  l_index_fk_name varchar2(30);
  l_index_fk_type varchar2(20);
  l_index_size number;
  l_index_size_total_old number;
  l_index_size_total_new number;

  c_LOCK_NAME constant varchar2(128):='BITMAP INDEX CONVERSION';
  c_LOCK_STATUS_SUCCESS constant integer := 0;
  c_LOCK_STATUS_OWNED constant integer := 4;
  l_lock_handle varchar2(128);
  l_lock_status integer;
  
  l_count_constraint_notfound number;
  l_count_index_notfound number;
  l_count_index_dropped number;
  l_count_index_created number;
  l_count_index_wrongname number;
  l_count_index_was_new_type number;

  l_undo_bitmap_conversion boolean;
--***************************************  
  procedure Logger ( p_in_message in varchar2
                    ,p_in_table_a_name in varchar2 default null
                    ,p_in_column_a_name in varchar2 default null
                    ,p_in_table_b_name in varchar2 default null
                    ,p_in_constraint_fk_name in varchar2 default null
                    ,p_in_index_fk_name in varchar2 default null
                    ,p_in_index_size in number default null
                    ) is
    pragma autonomous_transaction;
  begin
    insert into a_bitmap_index_conversion_log (message, table_a_name, column_a_name, table_b_name, 
                                               constraint_fk_name, index_fk_name, index_size,
                                               log_date)
      values ( substr(p_in_message,1,2000), p_in_table_a_name, p_in_column_a_name, p_in_table_b_name,
               p_in_constraint_fk_name, p_in_index_fk_name, p_in_index_size,
               current_timestamp
              );
      
    commit;
    
    --dbms_output.put_line(substr(p_in_message,1,250));
  end Logger;
--***************************************
  procedure Get_FK_Params ( p_in_fk_name in varchar2
                           ,p_out_table_a_name out varchar2
                           ,p_out_column_a_name out varchar2
                           ,p_out_table_b_name out varchar2
                           ) is
  begin
    select c.table_name, Cc.column_name, Cr.table_name
      into p_out_table_a_name, p_out_column_a_name, p_out_table_b_name
      from user_constraints C,
           user_cons_columns Cc,
           user_constraints Cr
     where C.owner = user
           and C.constraint_name = p_in_fk_name
           and C.constraint_type = 'R'
           and Cc.owner = C.owner
           and Cc.constraint_name = C.constraint_name
           and Cc.table_name = C.table_name
           and Cr.owner = C.r_owner
           and Cr.constraint_name = C.r_constraint_name;
  exception
    when no_data_found then
      p_out_table_a_name := null;
      p_out_column_a_name := null;
      p_out_table_b_name := null;
    
    when too_many_rows then
      raise_application_error(-20100,'� FK ����� ����� ��� �� ����� �������: '||p_in_fk_name);

  end Get_FK_Params;
--***************************************  
  procedure Get_Index_Params ( p_in_table_name in varchar2
                              ,p_in_column_name in varchar2
                              ,p_out_index_name out varchar2
                              ,p_out_index_type out varchar2
                              ) is
  begin
    select I.index_name, I.index_type
      into p_out_index_name, p_out_index_type
      from user_indexes I,
           user_ind_columns Ic
     where I.table_owner = user
           and I.table_name = p_in_table_name
           and Ic.index_name = I.index_name
           and Ic.table_name = I.table_name
           and Ic.column_name = p_in_column_name
           and not exists (select null  --������ ������ �� ����� ���� �������
                             from user_ind_columns Ic1
                            where Ic1.index_name = Ic.index_name
                                  and Ic1.table_name = Ic.table_name
                                  and Ic1.column_name <> Ic.column_name
                           );
    
  exception
    when no_data_found then
      p_out_index_name:=null;
      p_out_index_type:=null;

  end Get_Index_Params;
--***************************************
  Function Get_Index_Size (p_in_index_name in varchar2) return number is
    l_size number;
  begin
    select sum(S.bytes)
      into l_size
      from user_segments S
     where S.segment_name = p_in_index_name;
    return l_size;
  end Get_Index_Size;
--***************************************
  Function Is_Bitmap_Index_Enabled return boolean is
    l_enabled varchar2(100);
  begin
    select t.value 
      into l_enabled
      from sys.v_$option t 
     where t.parameter = 'Bit-mapped indexes';

    return (case when l_enabled='TRUE' then true else false end);
  exception
    when no_data_found then
      return null;
    when others then
      return null;
  end Is_Bitmap_Index_Enabled;
--***************************************
begin
  l_undo_bitmap_conversion := case when upper(p_in_undo_bitmap_conversion) in ('Y','YES') then true  else false end;
  
  Logger ('������ ��������� �������������� �������� '||case when l_undo_bitmap_conversion then 'BITMAP->NORMAL' else 'NORMAL->BITMAP' end );

  --������ ���������� ������ ������������� �������
  dbms_lock.allocate_unique( lockname => c_lock_name
                            ,lockhandle => l_lock_handle
                            );
  l_lock_status:=dbms_lock.request( lockhandle => l_lock_handle
                                   ,lockmode => dbms_lock.x_mode
                                   ,timeout => 0
                                   );
  if l_lock_status in (c_LOCK_STATUS_SUCCESS, c_LOCK_STATUS_OWNED) then
    null; --ok
  else
    raise_application_error(-20100, '��������� ��� ����������� � ������ ������');
  end if;
  
  --��������� ����������� ��������� bitmap-��������
  if not (nvl(Is_Bitmap_Index_Enabled,false) ) then
    raise_application_error(-20100,'���� �� ������������ BITMAP-������� (�� Enterprise Edition)');
  end if;
  
  --��������� ������ ������� ������ � ������ ������ �� ������������� ��������������
  --�� ������ ���������� ������ metalinks, metaobjects
  with Associations as
     (select substr(R.name, instr(R.name,'.',1,1)+1) association_name, 
             extractvalue(XMLType(R.configuration), '/DatabaseConfiguration/Reference/@shortName') association_shortname,
             TP.name table_a_name, 
             extractvalue(XMLType(TP.configuration), '/DatabaseConfiguration/DataTable/@shortName') table_a_shortname,
             TP.semantic table_a_semantic,
             TCh.name table_b_name,
             TCh.semantic table_b_semantic
        from metalinks R
            ,metaobjects TP 
            ,metaobjects TCh
       where R.class = 0 --AssociationClassTypes.Link  ���������� ������ (���������� ������ � ���������������� ������)
             and R.refparent = TP.id
             and R.refchild = TCh.id
             and TP.class = 3  --ClassTypes.clsFactData (������� ������)
             and TCh.class = 2 --ClassTypes.clsFixedClassifier (������������� �������������)
      )
  select upper(substr('FK'||'f_'||T.table_a_semantic||'_'
                      ||case when T.table_a_shortname is null then T.table_a_name else T.table_a_shortname end
                      ||case when T.association_shortname is null then T.association_name else T.association_shortname end
                      ,1,30)
               ) fk_name,
         upper(substr('I_'||'f_'||T.table_a_semantic||'_'
                      ||case when T.table_a_shortname is null then T.table_a_name else T.table_a_shortname end
                      ||case when T.association_shortname is null then T.association_name else T.association_shortname end
                      ,1,30)
               ) index_fk_name,
         upper(substr('f_'||T.table_a_semantic||'_'||T.table_a_name,1,30)) table_a_name,
         upper(substr('fx_'||T.table_b_semantic||'_'||T.table_b_name,1,30)) table_b_name
    
    bulk collect into l_constraint_name_list, 
                      l_index_fk_name_list,
                      l_table_a_name_list, 
                      l_table_b_name_list
    
    from Associations T
    order by table_a_name, table_b_name;
  
  Logger ('������� ������� ������ �������� ����������: '||l_constraint_name_list.count);

  l_count_constraint_notfound := 0;
  l_count_index_dropped := 0;
  l_count_index_notfound := 0;
  l_count_index_wrongname := 0;
  l_count_index_created := 0;
  l_count_index_was_new_type := 0;
  l_index_size_total_old := 0;
  l_index_size_total_new := 0;
  
  i := l_constraint_name_list.first;
  while (i is not null)
  loop
    --������� ��� �������+�������+�c��.������� �� ����� �������� �����
    Get_FK_Params( p_in_fk_name => l_constraint_name_list(i)
                  ,p_out_table_a_name => l_table_a_name
                  ,p_out_column_a_name => l_column_a_name
                  ,p_out_table_b_name => l_table_b_name
                  );
    
    --���� �� ������ ����� FK ������ �� �����, �� NEXT LOOP
    if l_table_a_name is null then
      Logger ( p_in_message => 'error by metadata: � �� �� ������ ������� ����'
              ,p_in_table_a_name => l_table_a_name_list(i)
              ,p_in_column_a_name => null
              ,p_in_table_b_name => l_table_b_name_list(i)
              ,p_in_constraint_fk_name => l_constraint_name_list(i)
              ,p_in_index_fk_name => null
              );
      l_count_constraint_notfound := l_count_constraint_notfound + 1;
      goto next_iteration;
    end if;

    --��������� ��� �� ��� FK (� ������������ � ����������� ������ ����� ���� �� ���������)
    if (l_table_a_name <> l_table_a_name_list(i))
       or (l_table_b_name <> l_table_b_name_list(i))
    then
      Logger ( p_in_message => 'error by metadata: ��������! � �� �� ������ ������� ����'
              ,p_in_table_a_name => l_table_a_name_list(i)
              ,p_in_column_a_name => null
              ,p_in_table_b_name => l_table_b_name_list(i)
              ,p_in_constraint_fk_name => l_constraint_name_list(i)
              ,p_in_index_fk_name => null
              );
      l_count_constraint_notfound := l_count_constraint_notfound + 1;
      goto next_iteration;
    end if;
    
    --������� ������������ ������
    Get_Index_Params( p_in_table_name => l_table_a_name
                     ,p_in_column_name => l_column_a_name
                     ,p_out_index_name => l_index_fk_name
                     ,p_out_index_type => l_index_fk_type
                     );
    
    --���� �� �����
    if l_index_fk_name is null then
      Logger ( p_in_message => 'warning: � �� �� ������ ������'
              ,p_in_table_a_name => l_table_a_name_list(i)
              ,p_in_column_a_name => l_column_a_name
              ,p_in_table_b_name => l_table_b_name_list(i)
              ,p_in_constraint_fk_name => l_constraint_name_list(i)
              ,p_in_index_fk_name => l_index_fk_name_list(i)
              );
      l_count_index_notfound := l_count_index_notfound + 1;

    --���� ������ ����� � �� �� ���� ���� ��� � ������ ������ - �� �������
    else                 
      --��������� ������������ �����
      if l_index_fk_name <> l_index_fk_name_list(i) then
        Logger ( p_in_message => 'warning: � �� ��� ������� ��� �������� ����� ���������� �� ����������: ����� '||l_index_fk_name
                ,p_in_table_a_name => l_table_a_name_list(i)
                ,p_in_column_a_name => l_column_a_name
                ,p_in_table_b_name => l_table_b_name_list(i)
                ,p_in_constraint_fk_name => l_constraint_name_list(i)
                ,p_in_index_fk_name => l_index_fk_name_list(i)
                );
        l_count_index_wrongname := l_count_index_wrongname + 1;
      end if;
            
      --������� ������ ���� �� �� ���� ���� ��� � ������ ������
      if (l_index_fk_type <> (case when l_undo_bitmap_conversion then 'NORMAL' else 'BITMAP' end) )
        or (l_index_fk_name <> l_index_fk_name_list(i))
      then
        begin
          l_index_size := Get_Index_Size(p_in_index_name => l_index_fk_name);
          execute immediate ('drop index '||l_index_fk_name);
          Logger ( p_in_message => 'Ok. ������ ������'
                  ,p_in_table_a_name => l_table_a_name_list(i)
                  ,p_in_column_a_name => l_column_a_name
                  ,p_in_table_b_name => l_table_b_name_list(i)
                  ,p_in_constraint_fk_name => l_constraint_name_list(i)
                  ,p_in_index_fk_name => l_index_fk_name_list(i)
                  ,p_in_index_size => l_index_size
                  );
          l_count_index_dropped := l_count_index_dropped + 1;
          l_index_size_total_old := l_index_size_total_old + l_index_size;
        exception
          when others then
            Logger ( p_in_message => 'error: ������ ��� �������� ������� '||l_index_fk_name||' :'||sqlerrm
                    ,p_in_table_a_name => l_table_a_name_list(i)
                    ,p_in_column_a_name => l_column_a_name
                    ,p_in_table_b_name => l_table_b_name_list(i)
                    ,p_in_constraint_fk_name => l_constraint_name_list(i)
                    ,p_in_index_fk_name => l_index_fk_name_list(i)
                    ,p_in_index_size => l_index_size
                    );
        end;
      end if;
    end if;
            
    --������� ��� ������������� ������ �������� ����������
    if (l_index_fk_type = (case when l_undo_bitmap_conversion then 'NORMAL' else 'BITMAP' end) )
       and (l_index_fk_name = l_index_fk_name_list(i)) then
       l_count_index_was_new_type := l_count_index_was_new_type + 1;
    else
      begin
        execute immediate ('CREATE '||(case when l_undo_bitmap_conversion then '' else 'BITMAP' end)
                           ||' INDEX '||l_index_fk_name_list(i)||' ON '||l_table_a_name_list(i)
                           ||' ('|| l_column_a_name||') TABLESPACE  dvindx COMPUTE STATISTICS'
                           );
        l_index_size := Get_Index_Size(p_in_index_name => l_index_fk_name_list(i));
        Logger ( p_in_message => 'Ok. ������ ������ ('||(case when l_undo_bitmap_conversion then 'NORMAL' else 'BITMAP' end)||')'
                ,p_in_table_a_name => l_table_a_name_list(i)
                ,p_in_column_a_name => l_column_a_name
                ,p_in_table_b_name => l_table_b_name_list(i)
                ,p_in_constraint_fk_name => l_constraint_name_list(i)
                ,p_in_index_fk_name => l_index_fk_name_list(i)
                ,p_in_index_size => l_index_size
                );
        l_count_index_created := l_count_index_created + 1;
        l_index_size_total_new := l_index_size_total_new + l_index_size;
      exception
        when others then
          Logger ( p_in_message => 'error: ������ ��� �������� �������: '||sqlerrm
                  ,p_in_table_a_name => l_table_a_name_list(i)
                  ,p_in_column_a_name => l_column_a_name
                  ,p_in_table_b_name => l_table_b_name_list(i)
                  ,p_in_constraint_fk_name => l_constraint_name_list(i)
                  ,p_in_index_fk_name => l_index_fk_name_list(i)
                  );
      end;
    end if;
      
    <<next_iteration>>
    i:= l_constraint_name_list.next(i);
  end loop;
  
  dbms_lock.sleep(0.1);
  Logger ('�� ������� ������� ������: '||l_count_constraint_notfound);
  dbms_lock.sleep(0.1);
  Logger ('�� ������� �������� � ������������ ������� ������: '||l_count_index_notfound);
  dbms_lock.sleep(0.1);
  Logger ('������� �������� � ����������������� ������: '||l_count_index_wrongname);
  dbms_lock.sleep(0.1);
  Logger ('������� ������������ ��������: '||l_count_index_dropped);
  dbms_lock.sleep(0.1);
  Logger ('������� ��������: '||l_count_index_created);
  dbms_lock.sleep(0.1);
  Logger ('��� ���� ������� ������� ����: '||l_count_index_was_new_type);
  dbms_lock.sleep(0.1);
  Logger ('������, ������� �������� ��������� �������: '||l_index_size_total_old);
  dbms_lock.sleep(0.1);
  Logger ('������, ���������� ��������� ����� ��������: '||l_index_size_total_new);
  dbms_lock.sleep(0.1);
  Logger ('���������� ��������� �������������� �������� '||case when l_undo_bitmap_conversion then 'BITMAP->NORMAL' else 'NORMAL->BITMAP' end );
  
  --������� ����������
  l_lock_status:=dbms_lock.release(lockhandle => l_lock_handle);
exception
  when others then
    Logger ('������ �� ����� ������ ��������� �������������� ��������: '||sqlerrm);
end P_Bitmap_Index_Conversion;
/

PROMPT ��������� �������.


PROMPT ������� job...
declare
  l_job_num number;
  l_job_start_date date;
begin
  --������� ����� ��������� job-� ��� �������
  for cr_job in (select J.job, J.broken, J.this_date, J.next_date
                   from user_jobs J
                  where upper(what) like '%P_BITMAP_INDEX_CONVERSION%'
                 )
  loop
    dbms_job.remove(JOB => cr_job.job);
    dbms_output.put_line('������������ job �'||cr_job.job||' (start_time='||to_char(cr_job.next_date,'DD.MM.YYYY HH24:MI:SS')||') ������.');
  end loop cr_job;

  --��������� ����� ��� ������� ������ job-�
  begin
    l_job_start_date := to_date( to_char(sysdate,'DD.MM.YYYY ')||'&P_JOB_START_TIME'
                                ,'DD.MM.YYYY HH24:MI'
                                );
  exception
    when others then
      dbms_output.put_line('������ ��� ���������� ������� ��� ���������� �������... ����� ����������� 22:00');
      l_job_start_date := trunc(sysdate)+22/24; --� ������ ������ ���������c� � 22:00
  end;
  
  dbms_job.submit( JOB => l_job_num
                  ,WHAT => 'execute immediate ''call P_Bitmap_Index_Conversion(''''&P_UNDO_BITMAP'''')'';
                            execute immediate ''drop procedure P_Bitmap_Index_Conversion'';'
                  ,NEXT_DATE => l_job_start_date
                  );
  commit;
  dbms_output.put_line('Job �'||l_job_num||' ������. ����� ������� ����������� �� '||to_char(l_job_start_date,'DD.MM.YYYY HH24:MI:SS'));
end;
/


PROMPT ��� ��������� ��������� ���������� ����������� index_conversion_protocol.sql
PROMPT ������ �������� �������.
PROMPT

disconnect

quit

