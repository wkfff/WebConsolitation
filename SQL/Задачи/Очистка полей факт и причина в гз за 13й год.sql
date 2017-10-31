/* Start - #5923 - Чистка данных в ГЗ - shelpanov - 19.04.2013 */

print N'Отключение триггера'
GO

disable trigger t_f_F_PNRZnach_aa on [DV].[f_F_PNRZnach];

print('Чистим')
GO

update [DV].[f_F_PNRZnach]
set  ActualValue = null,
	 Protklp = null
where RefFactGZ = ANY (SELECT gz.ID
				FROM [DV].[f_F_GosZadanie] gz join [DV].[f_F_ParameterDoc] vp ON (gz.RefParametr = vp.ID)
				where vp.RefYearForm = 2013
			   )

print N'Включение триггера'
GO

enable trigger t_f_F_PNRZnach_aa on [DV].[f_F_PNRZnach];

/* End - #5923 - Меняем статусы для документов - shelpanov - 19.04.2013 */