using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
//
// ���������� ��� ���������� ����������� ������ � �������� ���a���
//
namespace Krista.FM.ServerLibrary
{
	/// <summary>
	/// �������� ������
	/// </summary>
	public interface IDataSource
	{
		/// <summary>
		/// ������� ��� ������ ���������� �� ����� ���������
		/// </summary>
		void DeleteData();

		/// <summary>
		/// ID ��������� ������
		/// </summary>
		int ID { get; }

		/// <summary>
		/// ��� ���������� ������
		/// </summary>
		string SupplierCode { get; set; }

		/// <summary>
		/// ���������� ����� ����������� ����������
		/// </summary>
		string DataCode { get; set; }

		/// <summary>
		/// ������������ ����������� ����������
		/// </summary>
		string DataName { get; set; }

		/// <summary>
		/// ��� ���������� ��������� ������
		/// </summary>
        ParamKindTypes ParametersType { get; set; }

		/// <summary>
		/// ��� ����������: ������������ �������
		/// </summary>
		string BudgetName { get; set; }

        /// <summary>
        /// ��� ����������: ����������
        /// </summary>
        string Territory { get; set; }

		/// <summary>
		/// ��� ����������: ���
		/// </summary>
		int Year { get; set; }

		/// <summary>
		/// ��� ����������: �����
		/// </summary>
		int Month { get; set; }

		/// <summary>
		/// ��� ����������: �������
		/// </summary>
		string Variant { get; set; }

		/// <summary>
		/// ��� ����������: �������
		/// </summary>
		int Quarter { get; set; }

	    /// <summary>
	    /// ����������(��������) ���������.
	    /// </summary>
	    void LockDataSource();

	    /// <summary>
	    /// �������� ���������.
	    /// </summary>
	    void UnlockDataSource();

        /// <summary>
        /// ������� ������ �� ��������� � ������ ��������� ������� ��������.
        /// </summary>
        /// <param name="dependedObjects">������� ��������� ��������.</param>
	    /// <returns>���������� ��������.</returns>
	    DataTable RemoveWithData(DataTable dependedObjects);

        /// <summary>
        /// ���� � ��������� ���������� �������� � ����� �� ������� ����������, ��� �������
        /// </summary>
        /// <returns></returns>
	    int? FindInDatabase();

        /// <summary>
        /// ��������� ����� �������� � ���� � � ���������
        /// </summary>
        /// <returns></returns>
	    int Save();

        /// <summary>
        /// ��������� �������� � ��������� "���������"
        /// </summary>
	    void ConfirmDataSource();

        /// <summary>
        /// ��������� �������� � ��������� "�� ��������"
        /// </summary>
	    void UnConfirmDataSource();
	}


	/// <summary>
	/// ��������� ���������� ������
	/// </summary>
	public interface IDataSourceCollection : IEnumerable 
	{
        /// <summary>
        /// ���� �������� ������ �� ��� ����������
        /// </summary>
        /// <param name="obj">������ ���������</param>
        /// <returns>�� ��������� (null - �� ������)</returns>
        int? FindDataSource(object obj);

        /// <summary>
        /// ���������� �������� �� ��������� ��������� ����
        /// </summary>
        /// <param name="key">ID ��������� ������</param>
        /// <returns>true ���� �������� ��������� ����; ����� false</returns>
        bool Contains(int key);

		/// <summary>
		/// ����������, �������� �� ��������� ��������� �������� (����� ���� ����������)
		/// </summary>
		/// <param name="obj">������ ���������</param>
		/// <returns>true - ��������</returns>
		bool Contains(object obj);

        /// <summary>
        /// ���������� ��������� ������ (ID ����������)
        /// </summary>
        ICollection Keys { get; }

        /// <summary>
        /// ��������� �������� � ���� ������
        /// </summary>
        /// <param name="value">DataSource �������� ������</param>
		/// <returns>�� ���������</returns>
        int Add(Object value);

        /// <summary>
        /// ��������� �������� � ���� ������ � ��������� � �������
        /// </summary>
        /// <param name="value">DataSource �������� ������</param>
        /// <param name="phe"></param>
        /// <returns>�� ���������</returns>
        int Add(Object value, IPumpHistoryElement phe);

        /// <summary>
        /// ������� �������� �� ����
        /// </summary>
        /// <param name="index">�� ���������</param>
		/// <returns>������ ������</returns>
		string RemoveAt(int index);

		/// <summary>
		/// ������� ������� ���������
		/// </summary>
		/// <returns>��������� �������</returns>
		IDataSource CreateElement();

        /// <summary>
        /// ���������� ���������� � ���� ������
        /// </summary>
		int Count { get; }

		/// <summary>
		/// ���������� ���������� �������� ������ � ��������� ������,
		/// ���� ����� ���, �� ���������� null
		/// </summary>
		IDataSource this[int key] { get; }
	}

    /// <summary>
    /// ����� ��������� ����������
    /// </summary>
    public enum TakeMethodTypes
    {
        /// <summary>
        /// ������
        /// </summary>
        [Description("������")]
        Import,

        /// <summary>
        /// ����
        /// </summary>
        [Description("����")]
        Input,

        /// <summary>
        /// ���� (�� ������������)
        /// </summary>
        [Description("���� (�� ������������)")]
        Receipt

    }

    /// <summary>
    /// ��� ���������� ���������
    /// </summary>
    public enum ParamKindTypes : int
    {
        [Description("�� �������")]
        NoDivide = -1,
        
        [Description("������")]
        Budget = 0,
        
        [Description("���")]
        Year = 1,
        
        [Description("��� �����")]
        YearMonth = 2,
        
        [Description("��� ����� �������")]
        YearMonthVariant = 3,
        
        [Description("��� �������")]
        YearVariant = 4,
        
        [Description("��� �������")]
        YearQuarter = 5,
        
        [Description("��� ����������")]
        YearTerritory = 6,
        
        [Description("��� ������� �����")]
        YearQuarterMonth = 7,
        
        [Description("��� ����������")]
        WithoutParams = 8,

        [Description("�������")]
        Variant = 9,

        [Description("��� ����� ����������")]
        YearMonthTerritory = 10,

        [Description("��� ������� ����������")]
        YearQuarterTerritory = 11,

        [Description("��� ������� ����� ����������")]
        YearVariantMonthTerritory = 12,
    }

    /// <summary>
    /// ��������� ������
    /// </summary>
    public interface IDataSupplier : IServerSideObject, ICloneable, IDisposable
    {
        string Name { get; set;}
        string Description { get; set; }
        IDataKindCollection DataKinds { get; }
    }

    /// <summary>
    /// ��������� ����������� ������
    /// </summary>
    public interface IDataSupplierCollection : IDictionaryBase<string, IDataSupplier>
    {
        IDataSupplier New();
        void Add(IDataSupplier dataSupplier);

        void EndEdit();
        void CancelEdit();
    }

    /// <summary>
    /// ��� ����������� ����������
    /// </summary>
    public interface IDataKind : IServerSideObject, ICloneable
    {
        /// <summary>
        /// ��������� ������.
        /// </summary>
        IDataSupplier Supplier { get; }

        /// <summary>
        /// ��� ����������� ����������
        /// </summary>
        string Code { get; set;}

        /// <summary>
        /// ������������ ����������� ����������
        /// </summary>
        string Name { get; set;}

        /// <summary>
        /// �������� ����������� ����������
        /// </summary>
        string Description { get; set;}

        /// <summary>
        /// ��� ���������� ����������� ����������
        /// </summary>
        ParamKindTypes ParamKind { get; set;}

        /// <summary>
        /// ����� ��������� ������
        /// </summary>
        TakeMethodTypes TakeMethod { get; set;}
    }


    /// <summary>
    /// ��������� ����� ����������� ����������
    /// </summary>
    public interface IDataKindCollection : IDictionaryBase<string, IDataKind>
    {
        IDataKind New();
        void Add(IDataKind dataKind);
    }

    /// <summary>
    /// �������� ����������, ������� � �������� �������
    /// </summary>
    public interface IDataSourceManager : IDisposable
    {
        /// <summary>
        /// ���� � �������� � ��������� �������
        /// </summary>
        string BaseDirectory { get; }

        /// <summary>
        /// ���� � �������� � ��������� �������
        /// </summary>
        string ArchiveDirectory { get; }

        /// <summary>
        /// ��������� ������
        /// </summary>
        IDataSourceCollection DataSources { get; }

        /// <summary>
        /// ��������� ��� ������� � �������� �����
        /// </summary>
        IScheme Scheme { get; }

        /// <summary>
        /// ��������� ����������� ������
        /// </summary>
        IDataSupplierCollection DataSuppliers { get; }

        /// <summary>
        /// ��������� ������ �� ����������
        /// </summary>
        /// <returns></returns>
        DataTable GetDataSourcesInfo();

        /// <summary>
        /// ��������� ������ �� ����������
        /// </summary>
        /// <param name="dataSourceKinds">����� ����� ����������� ����������, 
        /// ����� �� ������� ����� ���� ������ ������ �������� ���������</param>
        /// <returns></returns>
        DataTable GetDataSourcesInfo(string dataSourceKinds);

        /// <summary>
        /// ���������� �������� � ��������� ��������� ������
        /// </summary>
        /// <param name="SourceID">ID ��������� ������</param>
        /// <returns>�������� � ��������� ��������� ������</returns>
        string GetDataSourceName(int SourceID);

        /// <summary>
        /// ���������� ������ ���������� �� ������� ����������� ������
        /// </summary>
        /// <param name="tableName">��� ������������� � ���� ������. ������ ���������� �� �������� ICommonObject.FullDBName</param>
        /// <returns>Key - ID ��������� ������; Value - �������� ���������</returns>
        Dictionary<int, string> GetDataSourcesNames(string tableName);

		/// <summary>
		/// ���������� IDataUpdater ��������� ������ ��� ������ ��� ��������� ���� ���������� ������
		/// </summary>
		IDataUpdater DataSourcesDataUpdater { get; }
    }
}