-- phpMyAdmin SQL Dump
-- version 2.9.1.1
-- http://www.phpmyadmin.net
-- 
-- ����: mysql.baze.krista.ru.postman.ru:63848
-- ����� ��������: ��� 23 2009 �., 14:40
-- ������ �������: 5.0.67
-- ������ PHP: 5.2.6
-- 
-- ���� ������: 'imon'
-- 

-- --------------------------------------------------------

-- 
-- ��������� ������� 'Groups'
-- 

create table Groups
(
	ID					int (10) not null,
	Name				varchar (50) not null,
	Description			varchar (2048),
	Blocked				int (1) default 0 not null,
	primary key ( ID ),
	unique key UKGroups ( Name )
);

-- --------------------------------------------------------

-- 
-- ��������� ������� 'Memberships'
-- 

create table Memberships 
(
	ID 					int (10) not null,
	RefUsers 			int (10) not null,
	RefGroups 			int (10) not null,
	primary key ( ID ),
	constraint FKMembershipsRefUsers foreign key ( RefUsers )
		references Users ( ID ),
	constraint FKMembershipsRefGroups foreign key ( RefGroups )
		references Groups ( ID )
);

-- --------------------------------------------------------

-- 
-- ��������� ������� 'Permissions'
-- 

create table Permissions 
(
	ID					int (10) not null,
	RefObjects			int (10) not null,
	RefGroups			int (10) default null,
	RefUsers 			int (10) default null,
	primary key ( ID ),
	constraint FKPermissionsRefObjects foreign key ( RefObjects )
		references Objects ( ID ),
	constraint FKPermissionsRefGroups foreign key ( RefGroups )
		references Groups ( ID ),
	constraint FKPermissionsRefUsers foreign key ( RefUsers )
		references Users ( ID )
);

-- --------------------------------------------------------

-- 
-- ��������� ������� 'Users'
-- 

create table Users 
(
	ID					int (10) not null,
	Name 				varchar (255) not null,
	Blocked				int (1) default 0 not null,
	PwdHashSHA 			varchar (255) default null,
	primary key ( ID ),
	unique key UKUsers ( Name )
);

-- --------------------------------------------------------

-- 
-- ��������� ������� 'Objects'
-- 

create table Objects
(
	ID					int (10) not null,
	ObjectKey			varchar (255),
	Name				varchar (100) not null,
	Caption				varchar (255) not null,
	Description			varchar (2048) default null,		
	ObjectType			int (10) not null,
	primary key ( ID )
);

-- --------------------------------------------------------

-- 
-- ��������� ������� 'Templates'
-- 

create table Templates
(
	ID					int (10) not null,
	ParentID			int (10) default null,
	Type				int (10) not null,
	Code				varchar (100) not null,
	Name				varchar (255) not null,
	Description			varchar (2048),
	Document 			text,
	SelfHashCode		int (20) default null,
	DescendantsHashCode	int (20) default null,
	ForumDiscussionID	int default null,
	TerritorialTagsID	varchar (2048),
	primary key ( ID )
);
