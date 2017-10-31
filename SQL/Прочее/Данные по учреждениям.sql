SELECT	DISTINCT org.[INN],
		org.[KPP],
	   	org.[Name],
	   	grbs.Name GRBS,
	   	pas.Mail
FROM [DV].[d_Org_Structure] org join [DV].[d_Org_GRBS] grbs on (org.RefOrgGRBS = grbs.ID)
     join [DV].[f_F_ParameterDoc] doc on (org.ID = doc.RefUchr)
     join [DV].[f_Org_Passport] pas on (pas.RefParametr = doc.ID)
where (org.CloseDate is null) and (doc.RefPartDoc = 1) and (org.RefTipYc in (3, 10, 8))