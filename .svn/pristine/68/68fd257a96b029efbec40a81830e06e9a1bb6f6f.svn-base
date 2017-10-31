/* Выходим по любой ошибке */
whenever SQLError exit rollback;


/* Устанавливаем генератор РайоныКонс на максимальное значение ИД Районы */
rem Настройки отображения/протоколирования
set echo off;
set Heading off;
set LineSize 250;
set PageSize 2000;
set SQLPrompt "";
set Autoprint off;
set Feedback off;
set Verify off;

spool AlterSequences.sql;

define tableName = 'D_REGIONS_MONTHREP';
define unitTableName = 'D_REGIONS_MONTHREPUNIT';
prompt drop sequence g_&unitTableName;;
select 'create sequence g_' || '&unitTableName' || ' start with ' || case (select count(ID) from &tableName) when 0 then 1 else (select max(ID) + 1 from &tableName) end || ';' from Dual;

prompt

spool off;

set echo on;
set Heading on;
set Feedback on;
set Autoprint on;
set Verify on;
set LineSize 500;
set PageSize 20;
set SQLPrompt "SQL>";

spool Form128DataTransfer.log;

whenever SQLError exit rollback;

alter trigger T_D_REGIONS_MONTHREP_AA disable;

@AlterSequences.sql

/* Меняем английскую К на русскую */
alter trigger T_D_REGIONS_FORPUMPSKIF_AA disable;
update D_REGIONS_FORPUMPSKIF set CODESTR = 'K' where CODESTR = 'К';
alter trigger T_D_REGIONS_FORPUMPSKIF_AA enable;

/* Копируем строки с соответствующим типом документа из Районов в РайоныКонс */
insert into D_REGIONS_MONTHREPUNIT (ID, SOURCEID, PUMPID, CODESTR, NAME, BUDGETKIND)
select ID, SOURCEID, PUMPID, CODESTR, NAME, BUDGETKIND
from D_REGIONS_MONTHREP where CODESTR in (select CODESTR from D_REGIONS_FORPUMPSKIF where REFDOCTYPE = 3);


/*-------------------------------------------------------- ДефПроф ---------------------------------------------------*/

alter trigger T_F_F_MONTHREPDEFPROF_AA disable;

/* Переносим факты с конс. районом */
insert into F_DP_MRDEFPROFUNIT (
	SOURCEID,
	PUMPID,
	SOURCEKEY,
	YEARPLANREPORT,
	QUARTERPLANREPORT,
	MONTHPLANREPORT,
	FACTREPORT,
	YEARPLAN,
	QUARTERPLAN,
	MONTHPLAN,
	FACT,
	SPREADFACTYEARPLAN,
	SPREADFACTMONTHPLAN,
	SPREADFACTYEARPLANREPORT,
	SPREADFACTMONTHPLANREPORT,
	REFMEANSTYPE,
	REFYEARMONTH,
	REFBDGTLEVELS,
	REFREGIONS)
select
	SOURCEID,
	PUMPID,
	SOURCEKEY,
	YEARPLANREPORT,
	QUARTERPLANREPORT,
	MONTHPLANREPORT,
	FACTREPORT,
	YEARPLAN,
	QUARTERPLAN,
	MONTHPLAN,
	FACT,
	SPREADFACTYEARPLAN,
	SPREADFACTMONTHPLAN,
	SPREADFACTYEARPLANREPORT,
	SPREADFACTMONTHPLANREPORT,
	REFMEANSTYPE,
	REFYEARMONTH,
	REFBDGTLEVELS,
	REFREGIONS
from F_F_MONTHREPDEFPROF ff
where ff.REFREGIONS in (select ID from D_REGIONS_MONTHREPUNIT);


/* Удаляем данные конс. фактов из неконс. таблицы */
delete from F_F_MONTHREPDEFPROF
where REFREGIONS in (select ID from D_REGIONS_MONTHREPUNIT);

alter trigger T_F_F_MONTHREPDEFPROF_AA enable;

/*-------------------------------------------------------- ДефПроф ---------------------------------------------------*/

/*-------------------------------------------------------- Доходы ---------------------------------------------------*/

alter trigger T_F_F_MONTHREPINCOMES_AA disable;

/* Переносим факты с конс. районом */
insert into F_D_MRINCOMESUNIT (
	SOURCEID,
	PUMPID,
	SOURCEKEY,
	YEARPLANREPORT,
	QUARTERPLANREPORT,
	FACTREPORT,
	YEARPLAN,
	QUARTERPLAN,
	FACT,
	MONTHPLANREPORT,
	MONTHPLAN,
	SPREADFACTYEARPLAN,
	SPREADFACTMONTHPLAN,
	SPREADFACTYEARPLANREPORT,
	SPREADFACTMONTHPLANREPORT,
	REFKDBRIDGE,
	REFKD2004,
	REFKD2005,
	REFYEARMONTH,
	REFBDGTLEVELS,
	REFMEANSTYPE,
	REFREGIONS)
select
	SOURCEID,
	PUMPID,
	SOURCEKEY,
	YEARPLANREPORT,
	QUARTERPLANREPORT,
	FACTREPORT,
	YEARPLAN,
	QUARTERPLAN,
	FACT,
	MONTHPLANREPORT,
	MONTHPLAN,
	SPREADFACTYEARPLAN,
	SPREADFACTMONTHPLAN,
	SPREADFACTYEARPLANREPORT,
	SPREADFACTMONTHPLANREPORT,
	REFKDBRDG,
	REFKD2004,
	REFKD2005,
	REFYEARMONTH,
	REFBDGTLEVELS,
	REFMEANSTYPE,
	REFREGIONS
from F_F_MONTHREPINCOMES ff
where ff.REFREGIONS in (select ID from D_REGIONS_MONTHREPUNIT);


/* Удаляем данные конс. фактов из неконс. таблицы */
delete from F_F_MONTHREPINCOMES
where REFREGIONS in (select ID from D_REGIONS_MONTHREPUNIT);

alter trigger T_F_F_MONTHREPINCOMES_AA enable;

/*-------------------------------------------------------- Доходы ---------------------------------------------------*/

/*-------------------------------------------------------- ИстВнешФин ---------------------------------------------------*/

alter trigger T_F_F_MONTHREPOUTFIN_AA disable;

/* Переносим факты с конс. районом */
insert into F_SOF_MRUNIT (
	SOURCEID,
	PUMPID,
	SOURCEKEY,
	YEARPLANREPORT,
	QUARTERPLANREPORT,
	MONTHPLANREPORT,
	FACTREPORT,
	YEARPLAN,
	QUARTERPLAN,
	MONTHPLAN,
	FACT,
	SPREADFACTYEARPLAN,
	SPREADFACTMONTHPLAN,
	SPREADFACTYEARPLANREPORT,
	SPREADFACTMONTHPLANREPORT,
	REFKIFBRIDGE,
	REFMEANSTYPE,
	REFYEARMONTH,
	REFBDGTLEVELS,
	REFREGIONS,
	REFSRCOUTFIN2004,
	REFSRCOUTFIN2005)
select
	SOURCEID,
	PUMPID,
	SOURCEKEY,
	YEARPLANREPORT,
	QUARTERPLANREPORT,
	MONTHPLANREPORT,
	FACTREPORT,
	YEARPLAN,
	QUARTERPLAN,
	MONTHPLAN,
	FACT,
	SPREADFACTYEARPLAN,
	SPREADFACTMONTHPLAN,
	SPREADFACTYEARPLANREPORT,
	SPREADFACTMONTHPLANREPORT,
	REFKIFBRIDGE,
	REFMEANSTYPE,
	REFYEARMONTH,
	REFBDGTLEVELS,
	REFREGIONS,
	REFSRCOUTFIN2004,
	REFSRCOUTFIN2005
from F_F_MONTHREPOUTFIN ff
where ff.REFREGIONS in (select ID from D_REGIONS_MONTHREPUNIT);


/* Удаляем данные конс. фактов из неконс. таблицы */
delete from F_F_MONTHREPOUTFIN
where REFREGIONS in (select ID from D_REGIONS_MONTHREPUNIT);

alter trigger T_F_F_MONTHREPOUTFIN_AA enable;

/*-------------------------------------------------------- ИстВнешФин ---------------------------------------------------*/

/*-------------------------------------------------------- ИстВнутрФин ---------------------------------------------------*/

alter trigger T_F_F_MONTHREPINFIN_AA disable;

/* Переносим факты с конс. районом */
insert into F_SIF_MRUNIT (
	SOURCEID,
	PUMPID,
	SOURCEKEY,
	YEARPLANREPORT,
	QUARTERPLANREPORT,
	MONTHPLANREPORT,
	FACTREPORT,
	YEARPLAN,
	QUARTERPLAN,
	MONTHPLAN,
	FACT,
	SPREADFACTYEARPLAN,
	SPREADFACTMONTHPLAN,
	SPREADFACTYEARPLANREPORT,
	SPREADFACTMONTHPLANREPORT,
	REFKIFBRIDGE,
	REFMEANSTYPE,
	REFYEARMONTH,
	REFBDGTLEVELS,
	REFREGIONS,
	REFSRCINFIN2005,
	REFSRCINFIN2003,
	REFSRCINFIN2002)
select
	SOURCEID,
	PUMPID,
	SOURCEKEY,
	YEARPLANREPORT,
	QUARTERPLANREPORT,
	MONTHPLANREPORT,
	FACTREPORT,
	YEARPLAN,
	QUARTERPLAN,
	MONTHPLAN,
	FACT,
	SPREADFACTYEARPLAN,
	SPREADFACTMONTHPLAN,
	SPREADFACTYEARPLANREPORT,
	SPREADFACTMONTHPLANREPORT,
	REFKIFBRIDGE,
	REFMEANSTYPE,
	REFYEARMONTH,
	REFBDGTLEVELS,
	REFREGIONS,
	REFSRCINFIN2005,
	REFSRCINFIN2003,
	REFSRCINFIN2002
from F_F_MONTHREPINFIN ff
where ff.REFREGIONS in (select ID from D_REGIONS_MONTHREPUNIT);


/* Удаляем данные конс. фактов из неконс. таблицы */
delete from F_F_MONTHREPINFIN
where REFREGIONS in (select ID from D_REGIONS_MONTHREPUNIT);

alter trigger T_F_F_MONTHREPINFIN_AA enable;

/*-------------------------------------------------------- ИстВнутрФин ---------------------------------------------------*/

/*-------------------------------------------------------- Расходы ---------------------------------------------------*/

alter trigger T_F_F_MONTHREPOUTCOMES_AA disable;

/* Переносим факты с конс. районом */
insert into F_R_MROUTCOMESUNIT (
	SOURCEID,
	PUMPID,
	SOURCEKEY,
	YEARPLANREPORT,
	QUARTERPLANREPORT,
	FACTREPORT,
	YEARPLAN,
	QUARTERPLAN,
	FACT,
	MONTHPLANREPORT,
	MONTHPLAN,
	SPREADFACTYEARPLAN,
	SPREADFACTMONTHPLAN,
	SPREADFACTYEARPLANREPORT,
	SPREADFACTMONTHPLANREPORT,
	REFFKR,
	REFEKR,
	REFYEARMONTH,
	REFBDGTLEVELS,
	REFMEANSTYPE,
	REFREGIONS)
select
	SOURCEID,
	PUMPID,
	SOURCEKEY,
	YEARPLANREPORT,
	QUARTERPLANREPORT,
	FACTREPORT,
	YEARPLAN,
	QUARTERPLAN,
	FACT,
	MONTHPLANREPORT,
	MONTHPLAN,
	SPREADFACTYEARPLAN,
	SPREADFACTMONTHPLAN,
	SPREADFACTYEARPLANREPORT,
	SPREADFACTMONTHPLANREPORT,
	REFFKR,
	REFEKR,
	REFYEARMONTH,
	REFBDGTLEVELS,
	REFMEANSTYPE,
	REFREGIONS
from F_F_MONTHREPOUTCOMES ff
where ff.REFREGIONS in (select ID from D_REGIONS_MONTHREPUNIT);


/* Удаляем данные конс. фактов из неконс. таблицы */
delete from F_F_MONTHREPOUTCOMES
where REFREGIONS in (select ID from D_REGIONS_MONTHREPUNIT);

alter trigger T_F_F_MONTHREPOUTCOMES_AA enable;

/*-------------------------------------------------------- Расходы ---------------------------------------------------*/

/*-------------------------------------------------------- СправВнешДолг ---------------------------------------------------*/

alter trigger T_F_F_MONTHREPOUTDEBTBOOKS_AA disable;

/* Переносим факты с конс. районом */
insert into F_GOD_MRBOOKS (
	SOURCEID,
	PUMPID,
	SOURCEKEY,
	FACTREPORT,
	FACT,
	REFYEARMONTH,
	REFBDGTLEVELS,
	REFREGIONS,
	REFMARKSOUTDEBT)
select
	SOURCEID,
	PUMPID,
	SOURCEKEY,
	FACTREPORT,
	FACT,
	REFYEARMONTH,
	REFBDGTLEVELS,
	REFREGIONS,
	REFMARKSOUTDEBT
from F_F_MONTHREPOUTDEBTBOOKS ff
where ff.REFREGIONS in (select ID from D_REGIONS_MONTHREPUNIT);


/* Удаляем данные конс. фактов из неконс. таблицы */
delete from F_F_MONTHREPOUTDEBTBOOKS
where REFREGIONS in (select ID from D_REGIONS_MONTHREPUNIT);

alter trigger T_F_F_MONTHREPOUTDEBTBOOKS_AA enable;

/*-------------------------------------------------------- СправВнешДолг ---------------------------------------------------*/

/*-------------------------------------------------------- СправВнутрДолг ---------------------------------------------------*/

alter trigger T_F_F_MONTHREPINDEBTBOOKS_AA disable;

/* Переносим факты с конс. районом */
insert into F_GID_MRBOOKS (
	SOURCEID,
	PUMPID,
	SOURCEKEY,
	FACTREPORT,
	FACT,
	REFYEARMONTH,
	REFBDGTLEVELS,
	REFREGIONS,
	REFMARKSINDEBT)
select
	SOURCEID,
	PUMPID,
	SOURCEKEY,
	FACTREPORT,
	FACT,
	REFYEARMONTH,
	REFBDGTLEVELS,
	REFREGIONS,
	REFMARKSINDEBT
from F_F_MONTHREPINDEBTBOOKS ff
where ff.REFREGIONS in (select ID from D_REGIONS_MONTHREPUNIT);


/* Удаляем данные конс. фактов из неконс. таблицы */
delete from F_F_MONTHREPINDEBTBOOKS
where REFREGIONS in (select ID from D_REGIONS_MONTHREPUNIT);

alter trigger T_F_F_MONTHREPINDEBTBOOKS_AA enable;

/*-------------------------------------------------------- СправВнутрДолг ---------------------------------------------------*/

/*-------------------------------------------------------- СправДоходы ---------------------------------------------------*/

alter trigger T_F_F_MONTHREPINCOMESBOOKS_AA disable;

/* Переносим факты с конс. районом */
insert into F_D_MRBOOK (
	SOURCEID,
	PUMPID,
	SOURCEKEY,
	FACTREPORT,
	FACT,
	REFYEARMONTH,
	REFBDGTLEVELS,
	REFREGIONS,
	REFKVSR)
select
	SOURCEID,
	PUMPID,
	SOURCEKEY,
	FACTREPORT,
	FACT,
	REFYEARMONTH,
	REFBDGTLEVELS,
	REFREGIONS,
	REFKVSR
from F_F_MONTHREPINCOMESBOOKS ff
where ff.REFREGIONS in (select ID from D_REGIONS_MONTHREPUNIT);


/* Удаляем данные конс. фактов из неконс. таблицы */
delete from F_F_MONTHREPINCOMESBOOKS
where REFREGIONS in (select ID from D_REGIONS_MONTHREPUNIT);

alter trigger T_F_F_MONTHREPINCOMESBOOKS_AA enable;

/*-------------------------------------------------------- СправДоходы ---------------------------------------------------*/

/*-------------------------------------------------------- СправЗадолженность ---------------------------------------------------*/

alter trigger T_F_F_MONTHREPARREARSBOOKS_AA disable;

/* Переносим факты с конс. районом */
insert into F_ARREARS_MRBOOK (
	SOURCEID,
	PUMPID,
	SOURCEKEY,
	FACTREPORT,
	FACT,
	REFYEARMONTH,
	REFBDGTLEVELS,
	REFREGIONS,
	REFMARKSARREARS)
select
	SOURCEID,
	PUMPID,
	SOURCEKEY,
	FACTREPORT,
	FACT,
	REFYEARMONTH,
	REFBDGTLEVELS,
	REFREGIONS,
	REFMARKSARREARS
from F_F_MONTHREPARREARSBOOKS ff
where ff.REFREGIONS in (select ID from D_REGIONS_MONTHREPUNIT);


/* Удаляем данные конс. фактов из неконс. таблицы */
delete from F_F_MONTHREPARREARSBOOKS
where REFREGIONS in (select ID from D_REGIONS_MONTHREPUNIT);

alter trigger T_F_F_MONTHREPARREARSBOOKS_AA enable;

/*-------------------------------------------------------- СправЗадолженность ---------------------------------------------------*/

/*-------------------------------------------------------- СправРасходы ---------------------------------------------------*/

alter trigger T_F_F_MONTHREPOUTCOMESBOOKS_AA disable;

/* Переносим факты с конс. районом */
insert into F_R_MRBOOK (
	SOURCEID,
	PUMPID,
	SOURCEKEY,
	FACTREPORT,
	FACT,
	REFYEARMONTH,
	REFBDGTLEVELS,
	REFREGIONS,
	REFFKR,
	REFEKR)
select
	SOURCEID,
	PUMPID,
	SOURCEKEY,
	FACTREPORT,
	FACT,
	REFYEARMONTH,
	REFBDGTLEVELS,
	REFREGIONS,
	REFFKR,
	REFEKR
from F_F_MONTHREPOUTCOMESBOOKS ff
where ff.REFREGIONS in (select ID from D_REGIONS_MONTHREPUNIT);


/* Удаляем данные конс. фактов из неконс. таблицы */
delete from F_F_MONTHREPOUTCOMESBOOKS
where REFREGIONS in (select ID from D_REGIONS_MONTHREPUNIT);

alter trigger T_F_F_MONTHREPOUTCOMESBOOKS_AA enable;

/*-------------------------------------------------------- СправРасходы ---------------------------------------------------*/

/*-------------------------------------------------------- СправРасходыДоп ---------------------------------------------------*/

alter trigger T_F_F_MROUTCOMESBKSEX_AA disable;

/* Переносим факты с конс. районом */
insert into F_R_MRBOOKADD (
	SOURCEID,
	PUMPID,
	SOURCEKEY,
	FACTREPORT,
	FACT,
	REFYEARMONTH,
	REFBDGTLEVELS,
	REFREGIONS,
	REFMARKSOUTCOMES)
select
	SOURCEID,
	PUMPID,
	SOURCEKEY,
	FACTREPORT,
	FACT,
	REFYEARMONTH,
	REFBDGTLEVELS,
	REFREGIONS,
	REFMARKSOUTCOMES
from F_F_MONTHREPOUTCOMESBOOKSADD ff
where ff.REFREGIONS in (select ID from D_REGIONS_MONTHREPUNIT);


/* Удаляем данные конс. фактов из неконс. таблицы */
delete from F_F_MONTHREPOUTCOMESBOOKSADD
where REFREGIONS in (select ID from D_REGIONS_MONTHREPUNIT);

alter trigger T_F_F_MROUTCOMESBKSEX_AA enable;

/*-------------------------------------------------------- СправРасходыДоп ---------------------------------------------------*/

delete from f_F_MonthRepDefProf f
where f.RefRegions in (
	select ID from D_REGIONS_MONTHREP mr
	where mr.CODESTR in (select fps.CODESTR from D_REGIONS_FORPUMPSKIF fps where fps.REFDOCTYPE = 3 or fps.REFDOCTYPE = 4));

delete from f_F_MonthRepIncomes f
where f.RefRegions in (
	select ID from D_REGIONS_MONTHREP mr
	where mr.CODESTR in (select fps.CODESTR from D_REGIONS_FORPUMPSKIF fps where fps.REFDOCTYPE = 3 or fps.REFDOCTYPE = 4));

delete from f_F_MonthRepOutFin f
where f.RefRegions in (
	select ID from D_REGIONS_MONTHREP mr
	where mr.CODESTR in (select fps.CODESTR from D_REGIONS_FORPUMPSKIF fps where fps.REFDOCTYPE = 3 or fps.REFDOCTYPE = 4));

delete from f_F_MonthRepInFin f
where f.RefRegions in (
	select ID from D_REGIONS_MONTHREP mr
	where mr.CODESTR in (select fps.CODESTR from D_REGIONS_FORPUMPSKIF fps where fps.REFDOCTYPE = 3 or fps.REFDOCTYPE = 4));

delete from f_F_MonthRepOutcomes f
where f.RefRegions in (
	select ID from D_REGIONS_MONTHREP mr
	where mr.CODESTR in (select fps.CODESTR from D_REGIONS_FORPUMPSKIF fps where fps.REFDOCTYPE = 3 or fps.REFDOCTYPE = 4));

delete from f_F_MonthRepOutDebtBooks f
where f.RefRegions in (
	select ID from D_REGIONS_MONTHREP mr
	where mr.CODESTR in (select fps.CODESTR from D_REGIONS_FORPUMPSKIF fps where fps.REFDOCTYPE = 3 or fps.REFDOCTYPE = 4));

delete from f_F_MonthRepInDebtBooks f
where f.RefRegions in (
	select ID from D_REGIONS_MONTHREP mr
	where mr.CODESTR in (select fps.CODESTR from D_REGIONS_FORPUMPSKIF fps where fps.REFDOCTYPE = 3 or fps.REFDOCTYPE = 4));

delete from f_F_MonthRepIncomesBooks f
where f.RefRegions in (
	select ID from D_REGIONS_MONTHREP mr
	where mr.CODESTR in (select fps.CODESTR from D_REGIONS_FORPUMPSKIF fps where fps.REFDOCTYPE = 3 or fps.REFDOCTYPE = 4));

delete from f_F_MonthRepArrearsBooks f
where f.RefRegions in (
	select ID from D_REGIONS_MONTHREP mr
	where mr.CODESTR in (select fps.CODESTR from D_REGIONS_FORPUMPSKIF fps where fps.REFDOCTYPE = 3 or fps.REFDOCTYPE = 4));

delete from f_F_MonthRepOutcomesBooks f
where f.RefRegions in (
	select ID from D_REGIONS_MONTHREP mr
	where mr.CODESTR in (select fps.CODESTR from D_REGIONS_FORPUMPSKIF fps where fps.REFDOCTYPE = 3 or fps.REFDOCTYPE = 4));

delete from f_F_MonthRepOutcomesBooksAdd f
where f.RefRegions in (
	select ID from D_REGIONS_MONTHREP mr
	where mr.CODESTR in (select fps.CODESTR from D_REGIONS_FORPUMPSKIF fps where fps.REFDOCTYPE = 3 or fps.REFDOCTYPE = 4));

/* Удаляем строки из Районов с конс. типом документа */
delete from D_REGIONS_MONTHREP mr
where mr.CODESTR in (select fps.CODESTR from D_REGIONS_FORPUMPSKIF fps where fps.REFDOCTYPE = 3 or fps.REFDOCTYPE = 4);

commit;

alter trigger T_D_REGIONS_MONTHREP_AA enable;

spool off;

commit;