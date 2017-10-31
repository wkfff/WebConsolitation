SELECT *
  FROM [RIATest].[DV].[d_Org_OrgYchr] uch
  where (select COUNT(ID) FROM [RIATest].[DV].[d_Org_OrgYchr] u where u.Name = uch.Name ) > 1
GO


