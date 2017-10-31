
print N'Отключение триггера'
GO

disable trigger t_f_F_GosZadanie_aa on [DV].[f_F_GosZadanie];

print N'Удаление цен'
GO

UPDATE [DV].[f_F_GosZadanie]
   SET CenaEd = null

WHERE ID = ANY (SELECT gz.ID
				FROM [DV].[f_F_GosZadanie] gz join [DV].[d_Services_VedPer] vp ON (gz.RefVedPch = vp.ID)
				where (gz.CenaEd is not Null) and (vp.RefPl = 1)
			   )

 
print N'Включение триггера'
GO

enable trigger t_f_F_GosZadanie_aa on [DV].[f_F_GosZadanie];
 
GO


