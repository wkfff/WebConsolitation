print N'Отключение триггеров'
GO

disable trigger t_f_Doc_Docum_aa on [DV].[f_Doc_Docum];
disable trigger t_f_F_OKVEDY_aa on [DV].[f_F_OKVEDY];
disable trigger t_f_F_Filial_aa on [DV].[f_F_Filial];
disable trigger t_f_F_Founder_aa on [DV].[f_F_Founder];
disable trigger t_f_Org_Passport_aa on [DV].[f_Org_Passport];
disable trigger t_f_F_ParameterDoc_aa on [DV].[f_F_ParameterDoc];

print N'Удаляем прикрепленые документы....'
GO

delete from [DV].[f_Doc_Docum]
where f_Doc_Docum.RefParametr = ANY (

		select ID
		from [DV].[f_F_ParameterDoc] pd
		where (pd.RefPartDoc = 1) and (pd.RefYearForm = 2013)
)

print N'Удаляем виды деятельности....'
GO

delete from [DV].[f_F_OKVEDY]
where f_F_OKVEDY.RefPassport = ANY (

		select ID
		from [DV].[f_Org_Passport] p
		where p.RefParametr = ANY (

		select ID
		from [DV].[f_F_ParameterDoc] pd
		where (pd.RefPartDoc = 1) and (pd.RefYearForm = 2013))
)

print N'Удаляем филиалы....'
GO

delete from [DV].[f_F_Filial]
where f_F_Filial.RefPassport = ANY (

		select ID
		from [DV].[f_Org_Passport] p
		where p.RefParametr = ANY (

		select ID
		from [DV].[f_F_ParameterDoc] pd
		where (pd.RefPartDoc = 1) and (pd.RefYearForm = 2013))
)

print N'Удаляем учредителей....'
GO

delete from [DV].[f_F_Founder]
where f_F_Founder.RefPassport = ANY (

		select ID
		from [DV].[f_Org_Passport] p
		where p.RefParametr = ANY (

		select ID
		from [DV].[f_F_ParameterDoc] pd
		where (pd.RefPartDoc = 1) and (pd.RefYearForm = 2013))
)

print N'Удаляем паспорта....'
GO

delete from [DV].[f_Org_Passport]
where f_Org_Passport.RefParametr = ANY (

		select ID
		from [DV].[f_F_ParameterDoc] pd
		where (pd.RefPartDoc = 1) and (pd.RefYearForm = 2013)
)

print N'Удаляем документ....'
GO

delete from [DV].[f_F_ParameterDoc]
where (f_F_ParameterDoc.RefYearForm = 2013) and
      (f_F_ParameterDoc.RefPartDoc = 1)


print N'Включение триггера'
GO

enable trigger t_f_Doc_Docum_aa on [DV].[f_Doc_Docum];
enable trigger t_f_F_OKVEDY_aa on [DV].[f_F_OKVEDY];
enable trigger t_f_F_Filial_aa on [DV].[f_F_Filial];
enable trigger t_f_F_Founder_aa on [DV].[f_F_Founder];
enable trigger t_f_Org_Passport_aa on [DV].[f_Org_Passport];
enable trigger t_f_F_ParameterDoc_aa on [DV].[f_F_ParameterDoc];
 
GO
