disable trigger t_d_Org_OrgYchr_aa on [DV].[d_Org_OrgYchr];


delete from [DV].[d_Org_OrgYchr]
where ID = 6700;

enable trigger t_d_Org_OrgYchr_aa on [DV].[d_Org_OrgYchr];
