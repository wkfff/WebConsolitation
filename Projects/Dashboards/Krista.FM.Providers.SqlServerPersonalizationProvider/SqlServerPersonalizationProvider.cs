using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Web.Hosting;
using System.Web.UI.WebControls.WebParts;

namespace Krista.FM.Providers.SqlServerPersonalizationProvider
{
	public sealed class SqlServerPersonalizationProvider : PersonalizationProvider
	{
		private string connectionString;

		private string applicationName;
		
		public override string ApplicationName
		{
			get
			{
				if (string.IsNullOrEmpty(applicationName))
					applicationName = HostingEnvironment.ApplicationVirtualPath;
				return applicationName;
			}
			set
			{
				applicationName = value;
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

			applicationName = configSettings["applicationName"];
			if (applicationName != null)
				configSettings.Remove("applicationName");

			if (!String.IsNullOrEmpty(configSettings["connectionStringName"]))
			{
				connectionString = ConfigurationManager.ConnectionStrings[configSettings["connectionStringName"]].ConnectionString;
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

		public override int GetCountOfState(PersonalizationScope scope, PersonalizationStateQuery query)
		{
			throw new NotImplementedException();
		}

		public override int ResetUserState(string path, DateTime userInactiveSinceDate)
		{
			throw new NotImplementedException();
		}

		protected override void ResetPersonalizationBlob(WebPartManager webPartManager, string path, string userName)
		{
		}

		public override int ResetState(PersonalizationScope scope, string[] paths, string[] usernames)
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

			SqlConnection con = new SqlConnection();
			con.ConnectionString = connectionString;
			con.Open();

			StringBuilder sqlDelete = new StringBuilder();
			sqlDelete.Append("DELETE FROM personalization WHERE username = @U1 AND application = @A1");

			StringBuilder sqlInsert = new StringBuilder();
			sqlDelete.Append("INSERT INTO personalization VALUES (@U2, @A2, @BLOBDATA)");

			if (String.IsNullOrEmpty(userName))
			{
				sqlDelete = sqlDelete.Replace("= @U1", "IS NULL");
				sqlInsert = sqlInsert.Replace("@U2", "NULL");
			}

			SqlCommand cmd = new SqlCommand(sqlDelete.ToString());
			//cmd.BindByName = true;
			cmd.Connection = con;
			cmd.CommandType = CommandType.Text;

			SqlParameter a1 = new SqlParameter("A1", SqlDbType.VarChar, 645);
			a1.Value = ApplicationName;
			cmd.Parameters.Add(a1);

			if (userName.Length > 0)
			{
				SqlParameter u1 = new SqlParameter("U1", SqlDbType.VarChar, 255);
				u1.Value = userName;
				cmd.Parameters.Add(u1);
			}

			cmd.ExecuteNonQuery();
			cmd.Dispose();

			cmd = new SqlCommand(sqlInsert.ToString());
			//cmd.BindByName = true;
			cmd.Connection = con;
			cmd.CommandType = CommandType.Text;

			SqlParameter a2 = new SqlParameter("A2", SqlDbType.VarChar, 645);
			a2.Value = ApplicationName;
			cmd.Parameters.Add(a2);

			SqlParameter p = new SqlParameter("BLOBDATA", SqlDbType.VarBinary);
			p.Value = dataBlob;
			cmd.Parameters.Add(p);

			if (userName.Length > 0)
			{
				SqlParameter u2 = new SqlParameter("U2", SqlDbType.VarChar, 255);
				u2.Value = userName;
				cmd.Parameters.Add(u2);
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
				SqlConnection con = new SqlConnection();
				con.ConnectionString = connectionString;
				con.Open();

				string sql = "SELECT pagesettings FROM personalization ";
				if (userName.Length > 0)
					sql += "WHERE username = @U1 and application = @A1";
				else
					sql += "WHERE username IS NULL and application = @A1";

				SqlCommand cmd = new SqlCommand(sql);
				//cmd.BindByName = true;
				cmd.Connection = con;
				cmd.CommandType = CommandType.Text;

				if (userName.Length > 0)
				{
					SqlParameter u1 = new SqlParameter("U1", SqlDbType.VarChar, 255);
					u1.Value = userName;
					cmd.Parameters.Add(u1);
				}

				SqlParameter a1 = new SqlParameter("A1", SqlDbType.VarChar, 645);
				a1.Value = ApplicationName;
				cmd.Parameters.Add(a1);

				SqlDataReader reader = cmd.ExecuteReader();
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
