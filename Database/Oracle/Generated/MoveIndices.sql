/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	2.4.0
	МОДУЛЬ
		ForeignKeyIndices.sql - Перенос автоматически созданных индексов в табличное пространство индексов DVINDX
	СУБД	Oracle 9.2
*/

alter index PKACTIONS rebuild tablespace DVINDX compute statistics;
alter index PKCONVERSIONTABLE rebuild tablespace DVINDX compute statistics;
alter index PKDATABASEVERSIONS rebuild tablespace DVINDX compute statistics;
alter index PKDATASOURCES2PUMPHISTORY rebuild tablespace DVINDX compute statistics;
alter index PKDISINTRULESEX rebuild tablespace DVINDX compute statistics;
alter index PKDISINTRULES_ALTKD rebuild tablespace DVINDX compute statistics;
alter index PKDISINTRULES_KD rebuild tablespace DVINDX compute statistics;
alter index PKDOCUMENTS rebuild tablespace DVINDX compute statistics;
alter index PKEVENTPROTOCOL rebuild tablespace DVINDX compute statistics;
alter index PKGROUPS rebuild tablespace DVINDX compute statistics;
alter index PKKINDSOFEVENTS rebuild tablespace DVINDX compute statistics;
alter index PKMEMBERSHIPS rebuild tablespace DVINDX compute statistics;
alter index PKNOTIFICATIONS rebuild tablespace DVINDX compute statistics;
alter index PKOBJECTS rebuild tablespace DVINDX compute statistics;
alter index PKPERMISSIONS rebuild tablespace DVINDX compute statistics;
alter index PKPUMPHISTORY rebuild tablespace DVINDX compute statistics;
alter index PKPUMPREGISTRY rebuild tablespace DVINDX compute statistics;
alter index PKSAT_BRIDGEOPERATIONS rebuild tablespace DVINDX compute statistics;
alter index PKSAT_DATAPUMP rebuild tablespace DVINDX compute statistics;
alter index PKSAT_DELETEDATA rebuild tablespace DVINDX compute statistics;
alter index PKSAT_MDPROCESSING rebuild tablespace DVINDX compute statistics;
alter index PKSAT_PROCESSDATA rebuild tablespace DVINDX compute statistics;
alter index PKSAT_REVISEDATA rebuild tablespace DVINDX compute statistics;
alter index PKSAT_SYSTEMEVENTS rebuild tablespace DVINDX compute statistics;
alter index PKSAT_USERSOPERATIONS rebuild tablespace DVINDX compute statistics;
alter index PKTASKS rebuild tablespace DVINDX compute statistics;
alter index PKUSERS rebuild tablespace DVINDX compute statistics;
alter index UKDISINTRULES_EX rebuild tablespace DVINDX compute statistics;
alter index UKDISINTRULES_KD rebuild tablespace DVINDX compute statistics;
commit work;
