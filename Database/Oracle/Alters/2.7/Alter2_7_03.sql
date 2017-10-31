/* Start - 13246 - ���6 - �������� ������������ ������� �� ������� datasources2pumphistory - shahov - 24.03.2010 */

whenever SQLError exit rollback;

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

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (83, '2.7.0.3', To_Date('24.03.2010', 'dd.mm.yyyy'), SYSDATE, '���6 - �������� ������������ ������� �� ������� datasources2pumphistory', 0);

commit;

/* End - 13246 - ���6 - �������� ������������ ������� �� ������� datasources2pumphistory - shahov - 24.03.2010 */
