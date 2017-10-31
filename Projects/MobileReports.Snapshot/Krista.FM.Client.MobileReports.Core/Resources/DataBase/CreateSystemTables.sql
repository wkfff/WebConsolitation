-- phpMyAdmin SQL Dump
-- version 2.9.1.1
-- http://www.phpmyadmin.net
-- 
-- Хост: mysql.baze.krista.ru.postman.ru:63848
-- Время создания: Апр 23 2009 г., 14:39
-- Версия сервера: 5.0.67
-- Версия PHP: 5.2.6
-- 
-- База данных: 'imon'
-- 

-- --------------------------------------------------------

-- 
-- Структура таблицы 'Authorization_statistics'
-- 

create table Authorization_statistics 
(
	ID					int (10) not null auto_increment,
	UserName			varchar (50) not null,
	UserIP 				varchar (50) not null,
	DeviceType 			varchar (20),
	AppVersion			int (10),
	ConnectionDate 		timestamp not null default CURRENT_TIMESTAMP,
	PRIMARY KEY  ( ID )
);


-- 
-- Структура таблицы 'Actual_sessions'
-- 

create table Actual_sessions
(
	ID					varchar (50) null default null, 
	UNIQUE (ID)
);
