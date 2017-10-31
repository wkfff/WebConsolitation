/********************************************************************
	Переводит базу Oracle из версии 2.X.X в следующую версию 2.X.X 
********************************************************************/

/* Шаблон оформления альтера: */ 
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */ 
	/* Ваш SQL-скрипт */
/* End - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */ 


/* Start - Стандартная часть */ 

/* Выходим по любой ошибке */ 

/* End   - Стандартная часть */ 


/* Start - 13201 - Создаются представления не сделанные дизайнером (фиксированная иерархия на уровень источник) - zaharchenko - 18.03.2010 */

drop VIEW DV_KD_EXCTCACHPL;

go

CREATE VIEW DV_KD_EXCTCACHPL
(id, rowtype, pumpid, sourcekey, name, codestr, sourceid, refbridgekd, datasourcename, short_name)
AS
SELECT T.ID, T.RowType, T.PumpID, T.SourceKey, T.Name, T.CodeStr, T.SourceID, T.RefBridgeKD,
DS."SUPPLIERCODE" + ' ' + DS."DATANAME" + CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' + cast((CASE DS."KINDSOFPARAMS" WHEN 0 THEN DS."NAME" + ' ' + cast(DS."YEAR" as varchar(4)) WHEN 1 THEN cast(DS."YEAR" as varchar(4)) WHEN 2 THEN cast(DS."YEAR" as varchar(4)) + ' ' + cast(DS."MONTH" as varchar(2)) WHEN 3 THEN cast(DS."YEAR" as varchar(4)) + ' ' + cast(DS."MONTH" as varchar(2)) + ' ' + DS."VARIANT" WHEN 4 THEN cast(DS."YEAR" as varchar(4)) + ' ' + DS."VARIANT" WHEN 5 THEN cast(DS."YEAR" as varchar(4)) + ' ' + cast(DS."QUARTER" as varchar(1)) WHEN 6 THEN cast(DS."YEAR" as varchar(4)) + ' ' + DS."TERRITORY" WHEN 7 THEN cast(DS."YEAR" as varchar(4)) + ' ' + cast(DS."QUARTER" as varchar(1)) + ' ' + cast(DS."MONTH" as varchar(2)) WHEN 9 THEN DS."VARIANT" END ) as varchar(1000)) END
, cast(T.Name as VARCHAR(255))
FROM d_KD_ExctCachPl T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID);

go

drop VIEW DV_MeansType_ExctCachPl;

go

CREATE VIEW DV_MeansType_ExctCachPl
(id, rowtype, pumpid, sourcekey, CODE, CODE1, CODE2, CODE3, name, RefBridgeMT, sourceid, datasourcename, short_name)
AS
SELECT T.ID, T.RowType, T.PumpID, T.SourceKey, T.CODE, T.CODE1, T.CODE2, T.CODE3, T.Name, T.RefBridgeMT, T.SourceID, 
DS."SUPPLIERCODE" + ' ' + DS."DATANAME" + CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' + cast((CASE DS."KINDSOFPARAMS" WHEN 0 THEN DS."NAME" + ' ' + cast(DS."YEAR" as varchar(4)) WHEN 1 THEN cast(DS."YEAR" as varchar(4)) WHEN 2 THEN cast(DS."YEAR" as varchar(4)) + ' ' + cast(DS."MONTH" as varchar(2)) WHEN 3 THEN cast(DS."YEAR" as varchar(4)) + ' ' + cast(DS."MONTH" as varchar(2)) + ' ' + DS."VARIANT" WHEN 4 THEN cast(DS."YEAR" as varchar(4)) + ' ' + DS."VARIANT" WHEN 5 THEN cast(DS."YEAR" as varchar(4)) + ' ' + cast(DS."QUARTER" as varchar(1)) WHEN 6 THEN cast(DS."YEAR" as varchar(4)) + ' ' + DS."TERRITORY" WHEN 7 THEN cast(DS."YEAR" as varchar(4)) + ' ' + cast(DS."QUARTER" as varchar(1)) + ' ' + cast(DS."MONTH" as varchar(2)) WHEN 9 THEN DS."VARIANT" END ) as varchar(1000)) END
, cast(T.Name as VARCHAR(255))
FROM d_MeansType_ExctCachPl T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID);

go

drop VIEW DV_Regions_ExctCachPl;

go

CREATE VIEW DV_Regions_ExctCachPl
(id, rowtype, pumpid, sourcekey, OKATO, CODE1, CODE2, CODE3, CODE4, name, RefBridgeOKATO, sourceid, datasourcename, short_name)
AS
SELECT T.ID, T.RowType, T.PumpID, T.SourceKey, T.OKATO, T.CODE1, T.CODE2, T.CODE3, T.CODE4, T.Name, T.RefBridgeOKATO, T.SourceID, 
DS."SUPPLIERCODE" + ' ' + DS."DATANAME" + CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' + cast((CASE DS."KINDSOFPARAMS" WHEN 0 THEN DS."NAME" + ' ' + cast(DS."YEAR" as varchar(4)) WHEN 1 THEN cast(DS."YEAR" as varchar(4)) WHEN 2 THEN cast(DS."YEAR" as varchar(4)) + ' ' + cast(DS."MONTH" as varchar(2)) WHEN 3 THEN cast(DS."YEAR" as varchar(4)) + ' ' + cast(DS."MONTH" as varchar(2)) + ' ' + DS."VARIANT" WHEN 4 THEN cast(DS."YEAR" as varchar(4)) + ' ' + DS."VARIANT" WHEN 5 THEN cast(DS."YEAR" as varchar(4)) + ' ' + cast(DS."QUARTER" as varchar(1)) WHEN 6 THEN cast(DS."YEAR" as varchar(4)) + ' ' + DS."TERRITORY" WHEN 7 THEN cast(DS."YEAR" as varchar(4)) + ' ' + cast(DS."QUARTER" as varchar(1)) + ' ' + cast(DS."MONTH" as varchar(2)) WHEN 9 THEN DS."VARIANT" END ) as varchar(1000)) END
, cast(T.Name as VARCHAR(255))
FROM d_Regions_ExctCachPl T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID);

go

drop VIEW DV_R_ExctCachPl;

go

CREATE VIEW DV_R_ExctCachPl
(id, rowtype, pumpid, sourcekey, CODE, CODE1, CODE2, CODE3, CODE4, CODE5, CODE6, name, RefFKR, RefKCSR, RefKVR, RefRBridge, sourceid, datasourcename, short_name)
AS
SELECT T.ID, T.RowType, T.PumpID, T.SourceKey, T.CODE, T.CODE1, T.CODE2, T.CODE3, T.CODE4, T.CODE5, T.CODE6, T.Name, T.RefFKR, T.RefKCSR, T.RefKVR, T.RefRBridge, T.SourceID, 
DS."SUPPLIERCODE" + ' ' + DS."DATANAME" + CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' + cast((CASE DS."KINDSOFPARAMS" WHEN 0 THEN DS."NAME" + ' ' + cast(DS."YEAR" as varchar(4)) WHEN 1 THEN cast(DS."YEAR" as varchar(4)) WHEN 2 THEN cast(DS."YEAR" as varchar(4)) + ' ' + cast(DS."MONTH" as varchar(2)) WHEN 3 THEN cast(DS."YEAR" as varchar(4)) + ' ' + cast(DS."MONTH" as varchar(2)) + ' ' + DS."VARIANT" WHEN 4 THEN cast(DS."YEAR" as varchar(4)) + ' ' + DS."VARIANT" WHEN 5 THEN cast(DS."YEAR" as varchar(4)) + ' ' + cast(DS."QUARTER" as varchar(1)) WHEN 6 THEN cast(DS."YEAR" as varchar(4)) + ' ' + DS."TERRITORY" WHEN 7 THEN cast(DS."YEAR" as varchar(4)) + ' ' + cast(DS."QUARTER" as varchar(1)) + ' ' + cast(DS."MONTH" as varchar(2)) WHEN 9 THEN DS."VARIANT" END ) as varchar(1000)) END
, cast(T.Name as VARCHAR(255))
FROM d_R_ExctCachPl T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID);

go

drop VIEW DV_EKR_ExctCachPl;

go

CREATE VIEW DV_EKR_ExctCachPl
(id, rowtype, pumpid, sourcekey, CODE, name, RefKOSGU, sourceid, datasourcename, short_name)
AS
SELECT T.ID, T.RowType, T.PumpID, T.SourceKey, T.CODE, T.Name, T.RefKOSGU, T.SourceID, 
DS."SUPPLIERCODE" + ' ' + DS."DATANAME" + CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' + cast((CASE DS."KINDSOFPARAMS" WHEN 0 THEN DS."NAME" + ' ' + cast(DS."YEAR" as varchar(4)) WHEN 1 THEN cast(DS."YEAR" as varchar(4)) WHEN 2 THEN cast(DS."YEAR" as varchar(4)) + ' ' + cast(DS."MONTH" as varchar(2)) WHEN 3 THEN cast(DS."YEAR" as varchar(4)) + ' ' + cast(DS."MONTH" as varchar(2)) + ' ' + DS."VARIANT" WHEN 4 THEN cast(DS."YEAR" as varchar(4)) + ' ' + DS."VARIANT" WHEN 5 THEN cast(DS."YEAR" as varchar(4)) + ' ' + cast(DS."QUARTER" as varchar(1)) WHEN 6 THEN cast(DS."YEAR" as varchar(4)) + ' ' + DS."TERRITORY" WHEN 7 THEN cast(DS."YEAR" as varchar(4)) + ' ' + cast(DS."QUARTER" as varchar(1)) + ' ' + cast(DS."MONTH" as varchar(2)) WHEN 9 THEN DS."VARIANT" END ) as varchar(1000)) END
, cast(T.Name as VARCHAR(255))
FROM d_EKR_ExctCachPl T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID);

go

drop VIEW DV_SubKESR_ExctCachPl;

go

CREATE VIEW DV_SubKESR_ExctCachPl
(id, rowtype, pumpid, sourcekey, CODE, name, RefSEKRBridge, sourceid, datasourcename, short_name)
AS
SELECT T.ID, T.RowType, T.PumpID, T.SourceKey, T.CODE, T.Name, T.RefSEKRBridge, T.SourceID, 
DS."SUPPLIERCODE" + ' ' + DS."DATANAME" + CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' + cast((CASE DS."KINDSOFPARAMS" WHEN 0 THEN DS."NAME" + ' ' + cast(DS."YEAR" as varchar(4)) WHEN 1 THEN cast(DS."YEAR" as varchar(4)) WHEN 2 THEN cast(DS."YEAR" as varchar(4)) + ' ' + cast(DS."MONTH" as varchar(2)) WHEN 3 THEN cast(DS."YEAR" as varchar(4)) + ' ' + cast(DS."MONTH" as varchar(2)) + ' ' + DS."VARIANT" WHEN 4 THEN cast(DS."YEAR" as varchar(4)) + ' ' + DS."VARIANT" WHEN 5 THEN cast(DS."YEAR" as varchar(4)) + ' ' + cast(DS."QUARTER" as varchar(1)) WHEN 6 THEN cast(DS."YEAR" as varchar(4)) + ' ' + DS."TERRITORY" WHEN 7 THEN cast(DS."YEAR" as varchar(4)) + ' ' + cast(DS."QUARTER" as varchar(1)) + ' ' + cast(DS."MONTH" as varchar(2)) WHEN 9 THEN DS."VARIANT" END ) as varchar(1000)) END
, cast(T.Name as VARCHAR(255))
FROM d_SubKESR_ExctCachPl T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID);

go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (82, '2.7.0.2', CONVERT(datetime, '2010.03.18', 102), GETDATE(), 'Создаются представления не сделанные дизайнером', 0);

go

/* End - 13201 - Создаются представления не сделанные дизайнером (фиксированная иерархия на уровень источник) - zaharchenko - 18.03.2010 */