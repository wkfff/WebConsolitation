/********************************************************************
	��������� ���� Oracle �� ������ 2.X.X � ��������� ������ 2.X.X 
********************************************************************/

/* ������ ���������� �������: */ 
/* Start - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */ 
	/* ��� SQL-������ */
/* End - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */ 


/* Start - ����������� ����� */ 

/* ������� �� ����� ������ */ 
whenever SQLError exit rollback; 
/* End   - ����������� ����� */ 


/* Start - 13201 - ��������� ������������� �� ��������� ���������� (������������� �������� �� ������� ��������) - zaharchenko - 18.03.2010 */

CREATE OR REPLACE VIEW DV_KD_EXCTCACHPL
(id, rowtype, pumpid, sourcekey, name, codestr, sourceid, refbridgekd, RefKVSR, RefKVSRBridge, datasourcename, short_name)
AS
SELECT T.ID, T.RowType, T.PumpID, T.SourceKey, T.Name, T.CodeStr, T.SourceID, T.RefBridgeKD, T.RefKVSR, T.RefKVSRBridge,
DS."SUPPLIERCODE" || ' ' || DS."DATANAME" || CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' || cast((CASE DS."KINDSOFPARAMS" WHEN 0 THEN DS."NAME" || ' ' || cast(DS."YEAR" as varchar(4)) WHEN 1 THEN cast(DS."YEAR" as varchar(4)) WHEN 2 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 3 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) || ' ' || DS."VARIANT" WHEN 4 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."VARIANT" WHEN 5 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) WHEN 6 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."TERRITORY" WHEN 7 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 9 THEN DS."VARIANT" END ) as varchar(1000)) END
, cast(T.Name as VARCHAR2(255))
FROM d_KD_ExctCachPl T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID);

CREATE OR REPLACE VIEW DV_MeansType_ExctCachPl
(id, rowtype, pumpid, sourcekey, CODE, CODE1, CODE2, CODE3, name, RefBridgeMT, sourceid, datasourcename, short_name)
AS
SELECT T.ID, T.RowType, T.PumpID, T.SourceKey, T.CODE, T.CODE1, T.CODE2, T.CODE3, T.Name, T.RefBridgeMT, T.SourceID, 
DS."SUPPLIERCODE" || ' ' || DS."DATANAME" || CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' || cast((CASE DS."KINDSOFPARAMS" WHEN 0 THEN DS."NAME" || ' ' || cast(DS."YEAR" as varchar(4)) WHEN 1 THEN cast(DS."YEAR" as varchar(4)) WHEN 2 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 3 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) || ' ' || DS."VARIANT" WHEN 4 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."VARIANT" WHEN 5 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) WHEN 6 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."TERRITORY" WHEN 7 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 9 THEN DS."VARIANT" END ) as varchar(1000)) END
, cast(T.Name as VARCHAR2(255))
FROM d_MeansType_ExctCachPl T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID);

CREATE OR REPLACE VIEW DV_Regions_ExctCachPl
(id, rowtype, pumpid, sourcekey, OKATO, CODE1, CODE2, CODE3, CODE4, name, RefBridgeOKATO, sourceid, datasourcename, short_name)
AS
SELECT T.ID, T.RowType, T.PumpID, T.SourceKey, T.OKATO, T.CODE1, T.CODE2, T.CODE3, T.CODE4, T.Name, T.RefBridgeOKATO, T.SourceID, 
DS."SUPPLIERCODE" || ' ' || DS."DATANAME" || CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' || cast((CASE DS."KINDSOFPARAMS" WHEN 0 THEN DS."NAME" || ' ' || cast(DS."YEAR" as varchar(4)) WHEN 1 THEN cast(DS."YEAR" as varchar(4)) WHEN 2 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 3 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) || ' ' || DS."VARIANT" WHEN 4 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."VARIANT" WHEN 5 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) WHEN 6 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."TERRITORY" WHEN 7 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 9 THEN DS."VARIANT" END ) as varchar(1000)) END
, cast(T.Name as VARCHAR2(255))
FROM d_Regions_ExctCachPl T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID);

CREATE OR REPLACE VIEW DV_R_ExctCachPl
(id, rowtype, pumpid, sourcekey, CODE, CODE1, CODE2, CODE3, CODE4, CODE5, CODE6, name, RefFKR, RefKCSR, RefKVR, RefRBridge, sourceid, datasourcename, short_name)
AS
SELECT T.ID, T.RowType, T.PumpID, T.SourceKey, T.CODE, T.CODE1, T.CODE2, T.CODE3, T.CODE4, T.CODE5, T.CODE6, T.Name, T.RefFKR, T.RefKCSR, T.RefKVR, T.RefRBridge, T.SourceID, 
DS."SUPPLIERCODE" || ' ' || DS."DATANAME" || CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' || cast((CASE DS."KINDSOFPARAMS" WHEN 0 THEN DS."NAME" || ' ' || cast(DS."YEAR" as varchar(4)) WHEN 1 THEN cast(DS."YEAR" as varchar(4)) WHEN 2 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 3 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) || ' ' || DS."VARIANT" WHEN 4 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."VARIANT" WHEN 5 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) WHEN 6 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."TERRITORY" WHEN 7 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 9 THEN DS."VARIANT" END ) as varchar(1000)) END
, cast(T.Name as VARCHAR2(255))
FROM d_R_ExctCachPl T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID);

CREATE OR REPLACE VIEW DV_EKR_ExctCachPl
(id, rowtype, pumpid, sourcekey, CODE, name, RefKOSGU, sourceid, datasourcename, short_name)
AS
SELECT T.ID, T.RowType, T.PumpID, T.SourceKey, T.CODE, T.Name, T.RefKOSGU, T.SourceID, 
DS."SUPPLIERCODE" || ' ' || DS."DATANAME" || CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' || cast((CASE DS."KINDSOFPARAMS" WHEN 0 THEN DS."NAME" || ' ' || cast(DS."YEAR" as varchar(4)) WHEN 1 THEN cast(DS."YEAR" as varchar(4)) WHEN 2 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 3 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) || ' ' || DS."VARIANT" WHEN 4 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."VARIANT" WHEN 5 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) WHEN 6 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."TERRITORY" WHEN 7 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 9 THEN DS."VARIANT" END ) as varchar(1000)) END
, cast(T.Name as VARCHAR2(255))
FROM d_EKR_ExctCachPl T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID);

CREATE OR REPLACE VIEW DV_SubKESR_ExctCachPl
(id, rowtype, pumpid, sourcekey, CODE, name, RefSEKRBridge, sourceid, datasourcename, short_name)
AS
SELECT T.ID, T.RowType, T.PumpID, T.SourceKey, T.CODE, T.Name, T.RefSEKRBridge, T.SourceID, 
DS."SUPPLIERCODE" || ' ' || DS."DATANAME" || CASE KindsOfParams WHEN 8 THEN '' ELSE ' - ' || cast((CASE DS."KINDSOFPARAMS" WHEN 0 THEN DS."NAME" || ' ' || cast(DS."YEAR" as varchar(4)) WHEN 1 THEN cast(DS."YEAR" as varchar(4)) WHEN 2 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 3 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."MONTH" as varchar(2)) || ' ' || DS."VARIANT" WHEN 4 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."VARIANT" WHEN 5 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) WHEN 6 THEN cast(DS."YEAR" as varchar(4)) || ' ' || DS."TERRITORY" WHEN 7 THEN cast(DS."YEAR" as varchar(4)) || ' ' || cast(DS."QUARTER" as varchar(1)) || ' ' || cast(DS."MONTH" as varchar(2)) WHEN 9 THEN DS."VARIANT" END ) as varchar(1000)) END
, cast(T.Name as VARCHAR2(255))
FROM d_SubKESR_ExctCachPl T LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID);

commit;

/* End - 13201 - ��������� ������������� �� ��������� ���������� (������������� �������� �� ������� ��������) - zaharchenko - 18.03.2010 */