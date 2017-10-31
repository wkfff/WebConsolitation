/* Start - #6520 - Добавляем группу E86N_Spectator - shelpanov - 12.08.2013 */

print N'Добавляем группу E86N_Spectator'
GO

INSERT INTO [DV].[Groups]
			([ID]
			,[Name]
			,[Description]
			,[DNSName]
			,[Blocked])
     VALUES
           (6, 'E86N_Spectator', 'Наблюдатель. На все только чтение', null, 0)

GO

/* End - - #6520 - Добавляем группу E86N_Spectator - shelpanov - 12.08.2013 */




