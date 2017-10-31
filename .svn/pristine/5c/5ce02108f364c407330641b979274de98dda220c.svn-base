using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using System.Web.Hosting;
using System.Web.UI.WebControls.WebParts;
using Oracle.DataAccess.Client;
using System.Configuration;

namespace Krista.FM.Providers.OraclePersonalizationProvider
{
//CREATE TABLE personalization (
//  CONSTRAINT un_pers UNIQUE (username, application),
//  username        varchar2(255),
//  application     varchar2(1000),
//  pagesettings    blob default empty_blob()
//)

	public class OraclePersonalizationProvider : PersonalizationProvider
	{
		private string _connectionString;

		private string _dataSource = @"
            (DESCRIPTION =
                (ADDRESS_LIST =
                (ADDRESS = (PROTOCOL = TCP)(HOST = krista-fm3.krista.ru)(PORT = 1521))
                )
                (CONNECT_DATA =
                (SERVICE_NAME = DVTEST)
                )
            )
                                     ";

		private string _applicationName;
		public override string ApplicationName
		{
			get
			{
				if (string.IsNullOrEmpty(_applicationName))
					_applicationName = HostingEnvironment.ApplicationVirtualPath;
				return _applicationName;
			}
			set
			{
				_applicationName = value;
			}
		}

		public override void Initialize(string name,
			NameValueCollection configSettings)
		{

			if (configSettings == null)
				throw new ArgumentNullException("configSettings");
			if (string.IsNullOrEmpty(name))
				name = "OraclePersonalizationProvider";
			if (string.IsNullOrEmpty(configSettings["description"]))
			{
				configSettings.Remove("description");
				configSettings.Add("description",
					"Oracle-based personalization provider");
			}

			base.Initialize(name, configSettings);

			_applicationName = configSettings["applicationName"];
			if (_applicationName != null)
				configSettings.Remove("applicationName");

			if (!String.IsNullOrEmpty(configSettings["connectionStringName"]))
			{
				_connectionString = ConfigurationManager.ConnectionStrings[configSettings["connectionStringName"]].ConnectionString;
			}
		}


		public override PersonalizationStateInfoCollection FindState(
				PersonalizationScope scope,
				PersonalizationStateQuery query,
				int pageIndex,
				int pageSize,
				out int totalRecords)
		{

			totalRecords = 1;
			throw new NotImplementedException();
		}

		public override int GetCountOfState(PersonalizationScope scope,
				PersonalizationStateQuery query)
		{


			throw new NotImplementedException();
		}

		public override int ResetUserState(string path,
				DateTime userInactiveSinceDate)
		{

			throw new NotImplementedException();
		}

		protected override void ResetPersonalizationBlob(
				WebPartManager webPartManager,
				string path,
				string userName)
		{
		}

		public override int ResetState(
				PersonalizationScope scope,
				string[] paths,
				string[] usernames)
		{

			throw new NotImplementedException();
		}

		protected override void SavePersonalizationBlob(
				WebPartManager webPartManager,
				string path,
				string userName,
				byte[] dataBlob)
		{
			if (userName == null)
			{
				userName = "Guest";
			}

			OracleConnection con = new OracleConnection();
			con.ConnectionString = _connectionString;
			con.Open();

			StringBuilder sql = new StringBuilder();

			sql.Append("BEGIN ");
			sql.Append("  DELETE FROM personalization WHERE username = :U1");
			sql.Append("    AND application = :A1; ");
			sql.Append("  INSERT INTO personalization ");
			sql.Append("    VALUES (:U2, :A2, :BLOBDATA); ");
			sql.Append("END;");

			if (String.IsNullOrEmpty(userName))
			{
				sql = sql.Replace("= :U1", "IS NULL");
				sql = sql.Replace(":U2", "NULL");
			}

			OracleCommand cmd = new OracleCommand(sql.ToString());
			cmd.BindByName = true;
			cmd.Connection = con;
			cmd.CommandType = CommandType.Text;

			cmd.Parameters.Add(new OracleParameter("A1", OracleDbType.Varchar2,
				1000, this.ApplicationName, ParameterDirection.Input));
			cmd.Parameters.Add(new OracleParameter("A2", OracleDbType.Varchar2,
				1000, this.ApplicationName, ParameterDirection.Input));

			OracleParameter p = new OracleParameter("BLOBDATA", OracleDbType.Blob);
			p.Value = dataBlob;
			cmd.Parameters.Add(p);

			if (userName.Length > 0)
			{
				cmd.Parameters.Add(new OracleParameter("U1", OracleDbType.Varchar2,
					 255, userName, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("U2", OracleDbType.Varchar2,
					 255, userName, ParameterDirection.Input));
			}

			cmd.ExecuteNonQuery();
			cmd.Dispose();
			con.Close();
			con.Dispose();
		}


		protected override void LoadPersonalizationBlobs(
				WebPartManager webPartManager,
				string path,
				string userName,
				ref byte[] sharedDataBlob,
				ref byte[] userDataBlob)
		{
			if (userName == null)
			{
				userName = "Guest";
			}
			
			try
			{
				OracleConnection con = new OracleConnection();
				con.ConnectionString = _connectionString;
				con.Open();

				string sql = "SELECT pagesettings FROM personalization ";
				if (userName.Length > 0)
					sql += "WHERE username = :U1 and application = :A1";
				else
					sql += "WHERE username IS NULL and application = :A1";

				OracleCommand cmd = new OracleCommand(sql);
				cmd.BindByName = true;
				cmd.Connection = con;
				cmd.CommandType = CommandType.Text;

				if (userName.Length > 0)
				{
					cmd.Parameters.Add(new OracleParameter("U1", OracleDbType.Varchar2,
														   255, userName, ParameterDirection.Input));
				}

				cmd.Parameters.Add(new OracleParameter("A1", OracleDbType.Varchar2,
													   1000, this.ApplicationName, ParameterDirection.Input));

				OracleDataReader reader = cmd.ExecuteReader();
				if (reader.Read())
				{
					long blob_length = reader.GetBytes(0, 0, null, 0, Int32.MaxValue);
					if (blob_length > 0)
					{
						byte[] buffer = new byte[blob_length];
						reader.GetBytes(0, 0, buffer, 0, buffer.Length);

						if (string.IsNullOrEmpty(userName))
							sharedDataBlob = buffer;
						else
							userDataBlob = buffer;
					}
				}

				cmd.Dispose();
				con.Close();
				con.Dispose();
			}
			catch (Exception e)
			{
				throw new Exception(e.Message, e);
			}
		}
	}
}
