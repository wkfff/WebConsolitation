
/*Связывает указанный аргумент user в текущей базе данных с существующим 
аргументом SQL Server login. Аргументы user и login должны быть указаны. 
Аргумент password должен иметь значение NULL или не должен быть указан.
*/

USE demo
GO
EXEC sp_change_users_login 'update_one', 'demo', 'MikhailMamonov'
GO