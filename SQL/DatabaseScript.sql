/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	2.6
	МОДУЛЬ
		Скрипт для выполнения скрипта на СУБД	SQL Server 2005
*/

:Error Error.txt

SELECT @@VERSION As 'Версия сервера'
SELECT @@SERVERNAME AS 'Имя сервера'
GO

print('Подключение к БД')

:Connect $(SERVER_NAME) -U $(UserName) -P $(Password)

USE $(DatabaseName)
GO


print('Начало выполнения скриптов...')

:r Add_AuditDB.sql

print('Завершение выполнения скриптов')

GO
