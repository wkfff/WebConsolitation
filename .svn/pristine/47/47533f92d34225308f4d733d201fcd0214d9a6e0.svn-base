/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	2.4.0
	МОДУЛЬ
		ForeignKeyIndices.sql - Создание индексов по внешним ключам
	СУБД	Oracle 9.2
*/

create index i_FKACTIONSREFTASKS on ACTIONS (REFTASKS) tablespace DVINDX compute statistics;
create index i_FKACTIONSREFUSERS on ACTIONS (REFUSERS) tablespace DVINDX compute statistics;
create index i_FKDS2PHREFDATASOURCES on DATASOURCES2PUMPHISTORY (REFDATASOURCES) tablespace DVINDX compute statistics;
create index i_FKDS2PHREFPUMPHISTORY on DATASOURCES2PUMPHISTORY (REFPUMPHISTORY) tablespace DVINDX compute statistics;
create index i_FKDISINTRULES_ALTKDREFDISTKD on DISINTRULES_ALTKD (REFDISINTRULES_KD) tablespace DVINDX compute statistics;
create index i_FKDISINTRULESEXREFDISINTKD on DISINTRULES_EX (REFDISINTRULES_KD) tablespace DVINDX compute statistics;
create index i_FKDOCUMENTSREFTASKS on DOCUMENTS (REFTASKS) tablespace DVINDX compute statistics;
create index i_FKEVENTPROTOCOLREFKINDSOFVNT on HUB_EVENTPROTOCOL (REFKINDSOFEVENTS) tablespace DVINDX compute statistics;
create index i_FKMEMBERSHIPSREFGROUPS on MEMBERSHIPS (REFGROUPS) tablespace DVINDX compute statistics;
create index i_FKMEMBERSHIPSREFUSERS on MEMBERSHIPS (REFUSERS) tablespace DVINDX compute statistics;
create index i_FKNOTIFICATIONSREFUSERS on NOTIFICATIONS (REFUSERS) tablespace DVINDX compute statistics;
create index i_FKPERMISSIONSREFGROUPS on PERMISSIONS (REFGROUPS) tablespace DVINDX compute statistics;
create index i_FKPERMISSIONSREFOBJECTS on PERMISSIONS (REFOBJECTS) tablespace DVINDX compute statistics;
create index i_FKPERMISSIONSREFUSERS on PERMISSIONS (REFUSERS) tablespace DVINDX compute statistics;
commit work;
