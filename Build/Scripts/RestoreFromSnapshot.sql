----Make Database to single user Mode
ALTER DATABASE RIAIntegrationTests
SET SINGLE_USER WITH
ROLLBACK IMMEDIATE

----Restore Database
RESTORE DATABASE RIAIntegrationTests from
DATABASE_SNAPSHOT = 'ssRIAIntegrationTests';
GO

/*If there is no error in statement before database will be in multiuser
mode.
If error occurs please execute following command it will convert
database in multi user.*/
ALTER DATABASE RIAIntegrationTests SET MULTI_USER
GO