using System;
using System.Data;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.Server.Common;

namespace Krista.FM.Server.Users
{
    public sealed partial class UsersManager : DisposableObject, IUsersManager
    {
        #region Инициализация DataUpdater's для различных типов объектов
        private DataUpdater GetUsersUpdater()
        {
            return GetUsersUpdater(String.Empty);
        }

        private DataUpdater GetUsersUpdater(string selectFilter)
        {
			using (Database db = (Database)_scheme.SchemeDWH.DB)
            {
                IDbDataAdapter adapter = db.GetDataAdapter();

                // выборка
                string queryText = "select ID, NAME, DESCRIPTION, USERTYPE, " +
                    "BLOCKED, DNSNAME, LASTLOGIN, FIRSTNAME, LASTNAME, PATRONYMIC, " +
                    "JOBTITLE, REFDEPARTMENTS, REFORGANIZATIONS, REFREGION, ALLOWDOMAINAUTH, ALLOWPWDAUTH from USERS";
                if (!String.IsNullOrEmpty(selectFilter))
                    queryText = String.Format("{0} where {1}", queryText, selectFilter);
                queryText = String.Concat(queryText, " ORDER BY ID");
                adapter.SelectCommand = db.Connection.CreateCommand();
                adapter.SelectCommand.CommandText = queryText;

                // удаление
                queryText = "delete from USERS where ID = ?";
                adapter.DeleteCommand = db.Connection.CreateCommand();
                IDbDataParameter prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "ID";
                adapter.DeleteCommand.Parameters.Add(prm);
                adapter.DeleteCommand.CommandText = db.GetQuery(queryText, adapter.DeleteCommand.Parameters);

                // обновление 
                queryText = "update USERS set NAME = ?, DESCRIPTION = ?, USERTYPE = ?, " +
                    "BLOCKED = ?, DNSNAME = ?, LASTLOGIN = ?, FIRSTNAME = ?, LASTNAME = ?, " +
                    "PATRONYMIC = ?, JOBTITLE = ?, REFDEPARTMENTS = ?, REFORGANIZATIONS = ?, " +
                    "REFREGION = ?, ALLOWDOMAINAUTH = ?, ALLOWPWDAUTH = ? where ID = ?";
                adapter.UpdateCommand = db.Connection.CreateCommand();
                // NAME
                prm = db.CreateParameter("NAME", DataAttributeTypes.dtString, 255);
                prm.SourceColumn = "NAME";
                adapter.UpdateCommand.Parameters.Add(prm);
                // DESCRIPTION
                prm = db.CreateParameter("DESCRIPTION", DataAttributeTypes.dtString, 2048);
                prm.SourceColumn = "DESCRIPTION";
                adapter.UpdateCommand.Parameters.Add(prm);
                // REFSYSUSERTYPE
                prm = db.CreateParameter("USERTYPE", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "USERTYPE";
                adapter.UpdateCommand.Parameters.Add(prm);
                // BLOCKED
                prm = db.CreateParameter("BLOCKED", DataAttributeTypes.dtInteger, 1);
                prm.SourceColumn = "BLOCKED";
                adapter.UpdateCommand.Parameters.Add(prm);
                // DNSNAME
                prm = db.CreateParameter("DNSNAME", DataAttributeTypes.dtString, 255);
                prm.SourceColumn = "DNSNAME";
                adapter.UpdateCommand.Parameters.Add(prm);
                // LASTLOGIN
                prm = db.CreateParameter("LASTLOGIN", DataAttributeTypes.dtDateTime, 0);
                prm.SourceColumn = "LASTLOGIN";
                adapter.UpdateCommand.Parameters.Add(prm);
                // FIRSTNAME
                prm = db.CreateParameter("FIRSTNAME", DataAttributeTypes.dtString, 100);
                prm.SourceColumn = "FIRSTNAME";
                adapter.UpdateCommand.Parameters.Add(prm);
                // LASTNAME
                prm = db.CreateParameter("LASTNAME", DataAttributeTypes.dtString, 100);
                prm.SourceColumn = "LASTNAME";
                adapter.UpdateCommand.Parameters.Add(prm);
                // PATRONYMIC
                prm = db.CreateParameter("PATRONYMIC", DataAttributeTypes.dtString, 100);
                prm.SourceColumn = "PATRONYMIC";
                adapter.UpdateCommand.Parameters.Add(prm);
                // JOBTITLE
                prm = db.CreateParameter("JOBTITLE", DataAttributeTypes.dtString, 255);
                prm.SourceColumn = "JOBTITLE";
                adapter.UpdateCommand.Parameters.Add(prm);
                // REFDEPARTMENTS
                prm = db.CreateParameter("REFDEPARTMENTS", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "REFDEPARTMENTS";
                adapter.UpdateCommand.Parameters.Add(prm);
                // REFORGANIZATIONS
                prm = db.CreateParameter("REFORGANIZATIONS", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "REFORGANIZATIONS";
                adapter.UpdateCommand.Parameters.Add(prm);
                // refregion
                prm = db.CreateParameter("REFREGION", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "REFREGION";
                adapter.UpdateCommand.Parameters.Add(prm);
                // ALLOWDOMAINAUTH
                prm = db.CreateParameter("ALLOWDOMAINAUTH", DataAttributeTypes.dtInteger, 1);
                prm.SourceColumn = "ALLOWDOMAINAUTH";
                adapter.UpdateCommand.Parameters.Add(prm);
                // ALLOWPWDAUTH
                prm = db.CreateParameter("ALLOWPWDAUTH", DataAttributeTypes.dtInteger, 1);
                prm.SourceColumn = "ALLOWPWDAUTH";
                adapter.UpdateCommand.Parameters.Add(prm);


                // ID
                prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "ID";
                adapter.UpdateCommand.Parameters.Add(prm);

                adapter.UpdateCommand.CommandText = db.GetQuery(queryText, adapter.UpdateCommand.Parameters);

                // вставка
                queryText = "insert into USERS (ID, NAME, DESCRIPTION, USERTYPE, " +
                    "BLOCKED, DNSNAME, LASTLOGIN, FIRSTNAME, LASTNAME, PATRONYMIC, " +
                    "JOBTITLE, REFDEPARTMENTS, REFORGANIZATIONS, REFREGION, PWDHASHSHA, ALLOWDOMAINAUTH, ALLOWPWDAUTH) " + 
                    "values (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
                adapter.InsertCommand = db.Connection.CreateCommand();

                // ID
                prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "ID";
                adapter.InsertCommand.Parameters.Add(prm);
                // NAME
                prm = db.CreateParameter("NAME", DataAttributeTypes.dtString, 255);
                prm.SourceColumn = "NAME";
                adapter.InsertCommand.Parameters.Add(prm);
                // DESCRIPTION
                prm = db.CreateParameter("DESCRIPTION", DataAttributeTypes.dtString, 2048);
                prm.SourceColumn = "DESCRIPTION";
                adapter.InsertCommand.Parameters.Add(prm);
                // REFSYSUSERTYPE
                prm = db.CreateParameter("USERTYPE", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "USERTYPE";
                adapter.InsertCommand.Parameters.Add(prm);
                // BLOCKED
                prm = db.CreateParameter("BLOCKED", DataAttributeTypes.dtInteger, 1);
                prm.SourceColumn = "BLOCKED";
                adapter.InsertCommand.Parameters.Add(prm);
                // DNSNAME
                prm = db.CreateParameter("DNSNAME", DataAttributeTypes.dtString, 255);
                prm.SourceColumn = "DNSNAME";
                adapter.InsertCommand.Parameters.Add(prm);
                // LASTLOGIN
                prm = db.CreateParameter("LASTLOGIN", DataAttributeTypes.dtDateTime, 0);
                prm.SourceColumn = "LASTLOGIN";
                adapter.InsertCommand.Parameters.Add(prm);
                // FIRSTNAME
                prm = db.CreateParameter("FIRSTNAME", DataAttributeTypes.dtString, 100);
                prm.SourceColumn = "FIRSTNAME";
                adapter.InsertCommand.Parameters.Add(prm);
                // LASTNAME
                prm = db.CreateParameter("LASTNAME", DataAttributeTypes.dtString, 100);
                prm.SourceColumn = "LASTNAME";
                adapter.InsertCommand.Parameters.Add(prm);
                // PATRONYMIC
                prm = db.CreateParameter("PATRONYMIC", DataAttributeTypes.dtString, 100);
                prm.SourceColumn = "PATRONYMIC";
                adapter.InsertCommand.Parameters.Add(prm);
                // JOBTITLE
                prm = db.CreateParameter("JOBTITLE", DataAttributeTypes.dtString, 255);
                prm.SourceColumn = "JOBTITLE";
                adapter.InsertCommand.Parameters.Add(prm);
                // REFDEPARTMENTS
                prm = db.CreateParameter("REFDEPARTMENTS", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "REFDEPARTMENTS";
                adapter.InsertCommand.Parameters.Add(prm);
                // REFORGANIZATIONS
                prm = db.CreateParameter("REFORGANIZATIONS", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "REFORGANIZATIONS";
                adapter.InsertCommand.Parameters.Add(prm);
                //REFREGION
                prm = db.CreateParameter("REFREGION", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "REFREGION";
                adapter.InsertCommand.Parameters.Add(prm);
                // PWDHASHSHA
                prm = db.CreateParameter("PWDHASHSHA", (object)PwdHelper.GetPasswordHash(String.Empty));
                adapter.InsertCommand.Parameters.Add(prm);
                // ALLOWDOMAINAUTH
                prm = db.CreateParameter("ALLOWDOMAINAUTH", DataAttributeTypes.dtInteger, 1);
                prm.SourceColumn = "ALLOWDOMAINAUTH";
                adapter.InsertCommand.Parameters.Add(prm);
                // ALLOWPWDAUTH
                prm = db.CreateParameter("ALLOWPWDAUTH", DataAttributeTypes.dtInteger, 1);
                prm.SourceColumn = "ALLOWPWDAUTH";
                adapter.InsertCommand.Parameters.Add(prm);

                adapter.InsertCommand.CommandText = db.GetQuery(queryText, adapter.InsertCommand.Parameters);

				return new DataUpdater(adapter, null, db);
            }
        }

        private DataUpdater GetGroupsUpdater()
        {
            using (Database db = (Database)_scheme.SchemeDWH.DB)
            {
                IDbDataAdapter adapter = db.GetDataAdapter();

                // выборка
				string queryText = "SELECT ID, NAME, DESCRIPTION, BLOCKED, DNSNAME " +
                    "FROM GROUPS ORDER BY ID";
                adapter.SelectCommand = db.Connection.CreateCommand();
                adapter.SelectCommand.CommandText = queryText;

                // удаление
                queryText = "delete from GROUPS where ID = ?";
                adapter.DeleteCommand = db.Connection.CreateCommand();
                IDbDataParameter prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "ID";
                adapter.DeleteCommand.Parameters.Add(prm);
                adapter.DeleteCommand.CommandText = db.GetQuery(queryText, adapter.DeleteCommand.Parameters);

                // обновление
                queryText = "update GROUPS set NAME = ?, DESCRIPTION = ?, " +
                    "BLOCKED = ?, DNSNAME = ? where ID = ?";
                adapter.UpdateCommand = db.Connection.CreateCommand();
                // NAME
                prm = db.CreateParameter("NAME", DataAttributeTypes.dtString, 255);
                prm.SourceColumn = "NAME";
                adapter.UpdateCommand.Parameters.Add(prm);
                // DESCRIPTION
                prm = db.CreateParameter("DESCRIPTION", DataAttributeTypes.dtString, 2048);
                prm.SourceColumn = "DESCRIPTION";
                adapter.UpdateCommand.Parameters.Add(prm);
                // BLOCKED
                prm = db.CreateParameter("BLOCKED", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "BLOCKED";
                adapter.UpdateCommand.Parameters.Add(prm);
                // DNSNAME
                prm = db.CreateParameter("DNSNAME", DataAttributeTypes.dtString, 255);
                prm.SourceColumn = "DNSNAME";
                adapter.UpdateCommand.Parameters.Add(prm);
                // ID
                prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "ID";
                adapter.UpdateCommand.Parameters.Add(prm);

                adapter.UpdateCommand.CommandText = db.GetQuery(queryText, adapter.UpdateCommand.Parameters);

                // вставка
                queryText = "insert into GROUPS (ID, NAME, DESCRIPTION, BLOCKED, " +
                    " DNSNAME) values (?, ?, ?, ?, ?)";
                adapter.InsertCommand = db.Connection.CreateCommand();
                // ID
                prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "ID";
                adapter.InsertCommand.Parameters.Add(prm);
                // NAME
                prm = db.CreateParameter("NAME", DataAttributeTypes.dtString, 255);
                prm.SourceColumn = "NAME";
                adapter.InsertCommand.Parameters.Add(prm);
                // DESCRIPTION
                prm = db.CreateParameter("DESCRIPTION", DataAttributeTypes.dtString, 2048);
                prm.SourceColumn = "DESCRIPTION";
                adapter.InsertCommand.Parameters.Add(prm);
                // BLOCKED
                prm = db.CreateParameter("BLOCKED", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "BLOCKED";
                adapter.InsertCommand.Parameters.Add(prm);
                // DNSNAME
                prm = db.CreateParameter("DNSNAME", DataAttributeTypes.dtString, 255);
                prm.SourceColumn = "DNSNAME";
                adapter.InsertCommand.Parameters.Add(prm);

                adapter.InsertCommand.CommandText = db.GetQuery(queryText, adapter.InsertCommand.Parameters);

				return new DataUpdater(adapter, null, db);
            }
        }

        private DataUpdater GetObjectUpdaterForUpdateDescription()
        {
			using (Database db = (Database)_scheme.SchemeDWH.DB) 
            {
                IDbDataAdapter adapter = db.GetDataAdapter();

                // обновление
				string queryText = "update OBJECTS set DESCRIPTION = ? where ID = ?";
                adapter.UpdateCommand = db.Connection.CreateCommand();
                // DESCRIPTION
                IDbDataParameter prm = db.CreateParameter("DESCRIPTION", DataAttributeTypes.dtString, 2048);
                prm.SourceColumn = "DESCRIPTION";
                adapter.UpdateCommand.Parameters.Add(prm);
                // ID
                prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "ID";
                adapter.UpdateCommand.Parameters.Add(prm);

                adapter.UpdateCommand.CommandText = db.GetQuery(queryText, adapter.UpdateCommand.Parameters);

				DataUpdater du = new DataUpdater(adapter, null, db);
                du.Transaction = db.Transaction;
				return du;
			}
        }

        private DataUpdater GetObjectsUpdater(IDatabase externalDb)
        {
            DataUpdater du = null;
            string queryText = String.Empty;
            Database db = (Database)externalDb;
            if (db == null)
                db = (Database)_scheme.SchemeDWH.DB;
            try
            {
                IDbDataAdapter adapter = db.GetDataAdapter();

                // выборка
                queryText = "SELECT ID, OBJECTKEY, NAME, CAPTION, DESCRIPTION, OBJECTTYPE " +
                    "FROM OBJECTS ORDER BY ID";
                adapter.SelectCommand = db.Connection.CreateCommand();
                adapter.SelectCommand.CommandText = queryText;

                // удаление
                queryText = "delete from OBJECTS where ID = ?";
                adapter.DeleteCommand = db.Connection.CreateCommand();
                IDbDataParameter prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "ID";
                adapter.DeleteCommand.Parameters.Add(prm);
                adapter.DeleteCommand.CommandText = db.GetQuery(queryText, adapter.DeleteCommand.Parameters);

                // обновление
                queryText = "update OBJECTS set OBJECTKEY = ?, NAME = ?, CAPTION = ?, DESCRIPTION = ?, OBJECTTYPE = ? " +
                    "where ID = ?";
                adapter.UpdateCommand = db.Connection.CreateCommand();
                // OBJECTKEY
                prm = db.CreateParameter("OBJECTKEY", DataAttributeTypes.dtString, 255);
                prm.SourceColumn = "OBJECTKEY";
                adapter.UpdateCommand.Parameters.Add(prm);
                // NAME
                prm = db.CreateParameter("NAME", DataAttributeTypes.dtString, 255);
                prm.SourceColumn = "NAME";
                adapter.UpdateCommand.Parameters.Add(prm);
                // CAPTION
                prm = db.CreateParameter("CAPTION", DataAttributeTypes.dtString, 255);
                prm.SourceColumn = "CAPTION";
                adapter.UpdateCommand.Parameters.Add(prm);
                // DESCRIPTION
                prm = db.CreateParameter("DESCRIPTION", DataAttributeTypes.dtString, 2048);
                prm.SourceColumn = "DESCRIPTION";
                adapter.UpdateCommand.Parameters.Add(prm);
                // REFSYSOBJECTTYPE
                prm = db.CreateParameter("OBJECTTYPE", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "OBJECTTYPE";
                adapter.UpdateCommand.Parameters.Add(prm);
                // ID
                prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "ID";
                adapter.UpdateCommand.Parameters.Add(prm);

                adapter.UpdateCommand.CommandText = db.GetQuery(queryText, adapter.UpdateCommand.Parameters);

                // вставка
                queryText = "insert into OBJECTS (ID, OBJECTKEY, NAME, CAPTION, DESCRIPTION, OBJECTTYPE) values (?, ?, ?, ?, ?, ?)";
                adapter.InsertCommand = db.Connection.CreateCommand();
                // ID
                prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "ID";
                adapter.InsertCommand.Parameters.Add(prm);
                // OBJECTKEY
                prm = db.CreateParameter("OBJECTKEY", DataAttributeTypes.dtString, 255);
                prm.SourceColumn = "OBJECTKEY";
                adapter.InsertCommand.Parameters.Add(prm);
                // NAME
                prm = db.CreateParameter("NAME", DataAttributeTypes.dtString, 255);
                prm.SourceColumn = "NAME";
                adapter.InsertCommand.Parameters.Add(prm);
                // CAPTION
                prm = db.CreateParameter("CAPTION", DataAttributeTypes.dtString, 255);
                prm.SourceColumn = "CAPTION";
                adapter.InsertCommand.Parameters.Add(prm);
                // DESCRIPTION
                prm = db.CreateParameter("DESCRIPTION", DataAttributeTypes.dtString, 2048);
                prm.SourceColumn = "DESCRIPTION";
                adapter.InsertCommand.Parameters.Add(prm);
                // REFSYSOBJECTTYPE
                prm = db.CreateParameter("OBJECTTYPE", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "OBJECTTYPE";
                adapter.InsertCommand.Parameters.Add(prm);

                adapter.InsertCommand.CommandText = db.GetQuery(queryText, adapter.InsertCommand.Parameters);

                du = new DataUpdater(adapter, null, db);
                du.Transaction = db.Transaction;
            }
            finally
            {
                // удалдяем db только если сами ее создали
                if (externalDb == null)
                    db.Dispose();
            }
            return du;
        }

        private DataUpdater GetPermissionsUpdater()
        {
            using (Database db = (Database)_scheme.SchemeDWH.DB)
            {
                IDbDataAdapter adapter = db.GetDataAdapter();

                // выборка
				string queryText = "select ID, REFOBJECTS, REFGROUPS, REFUSERS, ALLOWEDACTION from PERMISSIONS ORDER BY ID";
                adapter.SelectCommand = db.Connection.CreateCommand();
                adapter.SelectCommand.CommandText = queryText;

                // удаление
                queryText = "delete from PERMISSIONS where ID = ?";
                adapter.DeleteCommand = db.Connection.CreateCommand();
                IDbDataParameter prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "ID";
                adapter.DeleteCommand.Parameters.Add(prm);
                adapter.DeleteCommand.CommandText = db.GetQuery(queryText, adapter.DeleteCommand.Parameters);
                // обновление
                //adapter.UpdateCommand = db.Connection.CreateCommand();
                // вставка
                queryText = "insert into PERMISSIONS (ID, REFOBJECTS, REFGROUPS, REFUSERS, ALLOWEDACTION) " +
                    "values (?, ?, ?, ?, ?)";
                adapter.InsertCommand = db.Connection.CreateCommand();
                //ID 
                prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "ID";
                adapter.InsertCommand.Parameters.Add(prm);
                //REFOBJECTS
                prm = db.CreateParameter("REFOBJECTS", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "REFOBJECTS";
                adapter.InsertCommand.Parameters.Add(prm);
                //REFGROUPS
                prm = db.CreateParameter("REFGROUPS", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "REFGROUPS";
                adapter.InsertCommand.Parameters.Add(prm);
                //REFUSERS
                prm = db.CreateParameter("REFUSERS", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "REFUSERS";
                adapter.InsertCommand.Parameters.Add(prm);
                //ALLOWEDACTION
                prm = db.CreateParameter("ALLOWEDACTION", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "ALLOWEDACTION";
                adapter.InsertCommand.Parameters.Add(prm);

                adapter.InsertCommand.CommandText = db.GetQuery(queryText, adapter.InsertCommand.Parameters);

				return new DataUpdater(adapter, null, db);
            }
        }

        private DataUpdater GetMembershipUpdater()
        {
            using (Database db = (Database)_scheme.SchemeDWH.DB)
            {
                IDbDataAdapter adapter = db.GetDataAdapter();

                // выборка
                string queryText = "select ID, REFUSERS, REFGROUPS from MEMBERSHIPS ORDER BY ID";
                adapter.SelectCommand = db.Connection.CreateCommand();
                adapter.SelectCommand.CommandText = queryText;
                // удаление
                queryText = "delete from MEMBERSHIPS where ID = ?";
                adapter.DeleteCommand = db.Connection.CreateCommand();
                IDbDataParameter prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "ID";
                adapter.DeleteCommand.Parameters.Add(prm);
                adapter.DeleteCommand.CommandText = db.GetQuery(queryText, adapter.DeleteCommand.Parameters);
                // обновление
                queryText = "update MEMBERSHIPS set REFUSERS = ?, REFGROUPS = ? where ID = ?";
                adapter.UpdateCommand = db.Connection.CreateCommand();
                // REFUSERS
                prm = db.CreateParameter("REFUSERS", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "REFUSERS";
                adapter.UpdateCommand.Parameters.Add(prm);
                // REFGROUPS
                prm = db.CreateParameter("REFGROUPS", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "REFGROUPS";
                adapter.UpdateCommand.Parameters.Add(prm);
                // ID
                prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "ID";
                adapter.UpdateCommand.Parameters.Add(prm);

                adapter.UpdateCommand.CommandText = db.GetQuery(queryText, adapter.UpdateCommand.Parameters);
                // вставка
                queryText = "insert into MEMBERSHIPS (ID, REFUSERS, REFGROUPS) values (?, ?, ?)";
                adapter.InsertCommand = db.Connection.CreateCommand();
                // ID
                prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "ID";
                adapter.InsertCommand.Parameters.Add(prm);
                // REFUSERS
                prm = db.CreateParameter("REFUSERS", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "REFUSERS";
                adapter.InsertCommand.Parameters.Add(prm);
                // REFGROUPS
                prm = db.CreateParameter("REFGROUPS", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "REFGROUPS";
                adapter.InsertCommand.Parameters.Add(prm);

                adapter.InsertCommand.CommandText = db.GetQuery(queryText, adapter.InsertCommand.Parameters);

                return new DataUpdater(adapter, null, db);
            }
        }

        private DataUpdater GetOrganizationsUpdater()
        {
            using (Database db = (Database)_scheme.SchemeDWH.DB)
            {
                IDbDataAdapter adapter = db.GetDataAdapter();

                // выборка 
                string queryText = "select ID, NAME, DESCRIPTION from ORGANIZATIONS";
                adapter.SelectCommand = db.Connection.CreateCommand();
                adapter.SelectCommand.CommandText = queryText;
                // удаление
                queryText = "delete from ORGANIZATIONS where ID = ?";
                adapter.DeleteCommand = db.Connection.CreateCommand();
                IDbDataParameter prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "ID";
                adapter.DeleteCommand.Parameters.Add(prm);
                adapter.DeleteCommand.CommandText = db.GetQuery(queryText, adapter.DeleteCommand.Parameters);
                // обновление 
                queryText = "update ORGANIZATIONS set NAME = ?, DESCRIPTION = ? where ID = ?";
                adapter.UpdateCommand = db.Connection.CreateCommand();
                // NAME
                prm = db.CreateParameter("NAME", DataAttributeTypes.dtString, 500);
                prm.SourceColumn = "NAME";
                adapter.UpdateCommand.Parameters.Add(prm);
                // DESCRIPTION
                prm = db.CreateParameter("DESCRIPTION", DataAttributeTypes.dtString, 2048);
                prm.SourceColumn = "DESCRIPTION";
                adapter.UpdateCommand.Parameters.Add(prm);
                // ID
                prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "ID";
                adapter.UpdateCommand.Parameters.Add(prm);
                adapter.UpdateCommand.CommandText = db.GetQuery(queryText, adapter.UpdateCommand.Parameters);
                // вставка
                queryText = "insert into ORGANIZATIONS (ID, NAME, DESCRIPTION) values (?, ?, ?)";
                adapter.InsertCommand = db.Connection.CreateCommand();
                // ID
                prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "ID";
                adapter.InsertCommand.Parameters.Add(prm);
                // NAME
                prm = db.CreateParameter("NAME", DataAttributeTypes.dtString, 500);
                prm.SourceColumn = "NAME";
                adapter.InsertCommand.Parameters.Add(prm);
                // DESCRIPTION
                prm = db.CreateParameter("DESCRIPTION", DataAttributeTypes.dtString, 2048);
                prm.SourceColumn = "DESCRIPTION";
                adapter.InsertCommand.Parameters.Add(prm);

                adapter.InsertCommand.CommandText = db.GetQuery(queryText, adapter.InsertCommand.Parameters);

                return new DataUpdater(adapter, null, db);
            }
        }

        private DataUpdater GetDepartmentsUpdater()
        {
            using (Database db = (Database)_scheme.SchemeDWH.DB)
            {
                IDbDataAdapter adapter = db.GetDataAdapter();

                // выборка 
                string queryText = "select ID, NAME, DESCRIPTION from DEPARTMENTS";
                adapter.SelectCommand = db.Connection.CreateCommand();
                adapter.SelectCommand.CommandText = queryText;
                // удаление
                queryText = "delete from DEPARTMENTS where ID = ?";
                adapter.DeleteCommand = db.Connection.CreateCommand();
                IDbDataParameter prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "ID";
                adapter.DeleteCommand.Parameters.Add(prm);
                adapter.DeleteCommand.CommandText = db.GetQuery(queryText, adapter.DeleteCommand.Parameters);
                // обновление 
                queryText = "update DEPARTMENTS set NAME = ?, DESCRIPTION = ? where ID = ?";
                adapter.UpdateCommand = db.Connection.CreateCommand();
                // NAME
                prm = db.CreateParameter("NAME", DataAttributeTypes.dtString, 500);
                prm.SourceColumn = "NAME";
                adapter.UpdateCommand.Parameters.Add(prm);
                // DESCRIPTION
                prm = db.CreateParameter("DESCRIPTION", DataAttributeTypes.dtString, 2048);
                prm.SourceColumn = "DESCRIPTION";
                adapter.UpdateCommand.Parameters.Add(prm);
                // ID
                prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "ID";
                adapter.UpdateCommand.Parameters.Add(prm);
                adapter.UpdateCommand.CommandText = db.GetQuery(queryText, adapter.UpdateCommand.Parameters);
                // вставка
                queryText = "insert into DEPARTMENTS (ID, NAME, DESCRIPTION) values (?, ?, ?)";
                adapter.InsertCommand = db.Connection.CreateCommand();
                // ID
                prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "ID";
                adapter.InsertCommand.Parameters.Add(prm);
                // NAME
                prm = db.CreateParameter("NAME", DataAttributeTypes.dtString, 500);
                prm.SourceColumn = "NAME";
                adapter.InsertCommand.Parameters.Add(prm);
                // DESCRIPTION
                prm = db.CreateParameter("DESCRIPTION", DataAttributeTypes.dtString, 2048);
                prm.SourceColumn = "DESCRIPTION";
                adapter.InsertCommand.Parameters.Add(prm);

                adapter.InsertCommand.CommandText = db.GetQuery(queryText, adapter.InsertCommand.Parameters);

                return new DataUpdater(adapter, null, db);
            }
        }

        private DataUpdater GetTasksTypesUpdater(IDatabase externalDb)
        {
            DataUpdater du = null;
            string queryText = String.Empty;
            Database db = (Database)externalDb;
            if (db == null)
                db = (Database)_scheme.SchemeDWH.DB;
            try
            {
                IDbDataAdapter adapter = db.GetDataAdapter();

                // выборка 
                queryText = "select ID, CODE, NAME, DESCRIPTION, TASKTYPE from TASKSTYPES";
                adapter.SelectCommand = db.Connection.CreateCommand();
                adapter.SelectCommand.CommandText = queryText;
                // удаление
                queryText = "delete from TASKSTYPES where ID = ?";
                adapter.DeleteCommand = db.Connection.CreateCommand();
                IDbDataParameter prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "ID";
                adapter.DeleteCommand.Parameters.Add(prm);
                adapter.DeleteCommand.CommandText = db.GetQuery(queryText, adapter.DeleteCommand.Parameters);
                // обновление 
                queryText = "update TASKSTYPES set CODE = ?, NAME = ?, DESCRIPTION = ?, TASKTYPE = ? where ID = ?";
                adapter.UpdateCommand = db.Connection.CreateCommand();
                // CODE
                prm = db.CreateParameter("CODE", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "CODE";
                adapter.UpdateCommand.Parameters.Add(prm);
                // NAME
                prm = db.CreateParameter("NAME", DataAttributeTypes.dtString, 500);
                prm.SourceColumn = "NAME";
                adapter.UpdateCommand.Parameters.Add(prm);
                // DESCRIPTION
                prm = db.CreateParameter("DESCRIPTION", DataAttributeTypes.dtString, 2048);
                prm.SourceColumn = "DESCRIPTION";
                adapter.UpdateCommand.Parameters.Add(prm);
                // TASKTYPE
                prm = db.CreateParameter("TASKTYPE", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "TASKTYPE";
                adapter.UpdateCommand.Parameters.Add(prm);
                // ID
                prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "ID";
                adapter.UpdateCommand.Parameters.Add(prm);
                adapter.UpdateCommand.CommandText = db.GetQuery(queryText, adapter.UpdateCommand.Parameters);
                // вставка
                queryText = "insert into TASKSTYPES (ID, CODE, NAME, DESCRIPTION, TASKTYPE) values (?, ?, ?, ?, ?)";
                adapter.InsertCommand = db.Connection.CreateCommand();
                // ID
                prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "ID";
                adapter.InsertCommand.Parameters.Add(prm);
                // CODE
                prm = db.CreateParameter("CODE", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "CODE";
                adapter.InsertCommand.Parameters.Add(prm);
                // NAME
                prm = db.CreateParameter("NAME", DataAttributeTypes.dtString, 500);
                prm.SourceColumn = "NAME";
                adapter.InsertCommand.Parameters.Add(prm);
                // DESCRIPTION
                prm = db.CreateParameter("DESCRIPTION", DataAttributeTypes.dtString, 2048);
                prm.SourceColumn = "DESCRIPTION";
                adapter.InsertCommand.Parameters.Add(prm);
                // TAKSTYPE
                prm = db.CreateParameter("TASKTYPE", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "TASKTYPE";
                adapter.InsertCommand.Parameters.Add(prm);

                adapter.InsertCommand.CommandText = db.GetQuery(queryText, adapter.InsertCommand.Parameters);

                du = new DataUpdater(adapter, null, db);
                du.Transaction = db.Transaction;
            }
            finally
            {
                // освобождаем Db только если сами ее получили
                if (externalDb == null)
                    db.Dispose();
            }
            return du;
        }

        #endregion
    }
}