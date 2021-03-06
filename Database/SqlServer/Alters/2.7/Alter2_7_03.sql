/* Start - 13246 - ���6 - �������� ������������ ������� �� ������� datasources2pumphistory - shahov - 24.03.2010 */

delete from DATASOURCES2PUMPHISTORY
where
     REFDATASOURCES in (
         select ID from DATASOURCES
         where SUPPLIERCODE = '���' and DATACODE = 6
     )
     and
     not REFPUMPHISTORY in (
         select ID from PUMPHISTORY
         where PROGRAMIDENTIFIER = 'Form13Pump'
     );

go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (83, '2.7.0.3', CONVERT(datetime, '2010.03.24', 102), GETDATE(), '���6 - �������� ������������ ������� �� ������� datasources2pumphistory', 0);

go

/* End - 13246 - ���6 - �������� ������������ ������� �� ������� datasources2pumphistory - shahov - 24.03.2010 */
