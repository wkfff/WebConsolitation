using System;
using System.Collections;
using System.Data;
using System.Runtime.CompilerServices;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.Server.Users;


namespace Krista.FM.Server.DataPumpManagement
{
	/// <summary>
	/// ��������� ��������� ������� �������
	/// </summary>
	public class PumpRegistryCollection : DisposableObject, IPumpRegistryCollection
	{
        #region ����
        
        // ������ ���������
		private SortedList list;
		// �����
		private IScheme scheme;

        #endregion ����


        #region �������������

        /// <summary>
		/// ����������� �������
		/// </summary>
        public PumpRegistryCollection(IScheme scheme)
		{
            this.scheme = scheme;
			list = new SortedList(100);
        }

        #endregion �������������


        #region ����� �������

        /// <summary>
        /// ����������� ������� � ������� ������������
        /// </summary>
        /// <param name="progID">�� �������</param>
        /// <param name="description">��������</param>
		/// <param name="usersManager"></param>
		public static void RegisterPumpProgram(string progID, string description, UsersManager usersManager)
        {
            if (usersManager != null)
            {
                usersManager.RegisterSystemObject(progID, description, SysObjectsTypes.DataPump);
            }
        }

        /// <summary>
        /// �������� ����������� ������� � ������� ������������
        /// </summary>
        /// <param name="progID">�� �������</param>
		/// <param name="usersManager"></param>
		public static void UnregisterPumpProgram(string progID, UsersManager usersManager)
        {
            if (usersManager != null)
            {
                usersManager.UnregisterSystemObject(progID);
            }
        }

		private PumpRegistryElement GetRegistryElement(DataRow row)
		{
			PumpRegistryElement item = (PumpRegistryElement)CreateElement();

			item.ID = Convert.ToInt32(row[0]);
			item.SupplierCode = Convert.ToString(row[1]);
			item.DataCode = Convert.ToString(row[2]);
			item.ProgramIdentifier = Convert.ToString(row[3]);
			//if (!row.IsNull(4)) item.ProgramConfig = Convert.ToString(row[4]);
			if (!row.IsNull(5)) item.Description = Convert.ToString(row[5]);
			if (!row.IsNull(6)) item.Name = Convert.ToString(row[6]);
			if (!row.IsNull(7)) item.PumpProgram = Convert.ToString(row[7]);

			item.Initialize();
			item.RequestPumpHistory();
			return item;
		}

		#endregion ����� �������


        #region ���������� IPumpRegistryCollection

		/// <summary>
		/// ������� ������� ������� �������
		/// </summary>
		/// <returns>��������� �������</returns>
		public IPumpRegistryElement CreateElement()
		{
			return new PumpRegistryElement(scheme);
		}

		/// <summary>
		/// ���������� ���������� ������� ������� ������� � ��������� ������,
		/// ���� ����� ���, �� ���������� null. key - ProgramIdentifier �������.
		/// </summary>
		public IPumpRegistryElement this[string key]
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			get
			{
				if (list.ContainsKey(key))
					return (IPumpRegistryElement) list[key];

				using (IDatabase db = scheme.SchemeDWH.DB)
				{
					DataTable dt = DataPumpInfo.PumpRegistryDataTable(db, String.Format("ProgramIdentifier = '{0}'", key));
					if (dt.Rows.Count == 0)
						return null;

					IPumpRegistryElement pre = GetRegistryElement(dt.Rows[0]);
					list.Add(pre.ProgramIdentifier, pre);

					return pre;
				}
			}
		}

		#endregion ���������� IPumpRegistryCollection
	}
}
