


SELECT ID, Code
  FROM [RIATest].[DV].[d_Org_VidOrg] uch
  where (select COUNT(ID) FROM [RIATest].[DV].[d_Org_VidOrg] u where u.Code = uch.Code ) > 1
  order by Code
GO