
select *  FROM [RIAYar].[DV].[d_Org_VidOrg] o
where ID > 804 and (EXISTS (select *
                             FROM [RIAYar].[DV].[f_Org_Passport] pp
                             where pp.RefVid = o.ID))