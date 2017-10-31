namespace Krista.FM.Client.ViewObjects.DataPumpUI
{
	/// <summary>
	/// ����� ��������������� ���������� � �������� ��� �����
	/// </summary>
	public class FileSystemEntry
	{
		#region ����

		private FileSystemEntryType					fileSystemEntryType = FileSystemEntryType.File;
		private string								fullyQualifiedName = "";

        #endregion ����


        #region �������������

        /// <summary>
		/// �����������
		/// </summary>
		/// <param name="fileSystemEntryType">���� ��� �������</param>
		/// <param name="fullyQualifiedName">������ ����</param>
		public FileSystemEntry(FileSystemEntryType fileSystemEntryType, string fullyQualifiedName)
		{
			this.fileSystemEntryType	= fileSystemEntryType;
			this.fullyQualifiedName		= fullyQualifiedName;
        }

        #endregion �������������


        #region ������ ������

        /// <summary>
		/// ��� �������� ��� �����
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Name;
        }

        #endregion ������ ������


        #region �������� ������

        /// <summary>
		/// ��� ��������: ���� ��� �������
		/// </summary>
		public FileSystemEntryType EntryType
		{
			get	{ return fileSystemEntryType; }
		}

		/// <summary>
		/// ������ ���
		/// </summary>
		public string FullName
		{
			get	{ return fullyQualifiedName; }
		}

		/// <summary>
		/// ���
		/// </summary>
		public string Name
		{
			get	{ return FileManager.RemovePath(fullyQualifiedName); }
        }

        #endregion �������� ������
    }


	/// <summary>
	/// ��� �������� �������� �������
	/// </summary>
	public enum FileSystemEntryType
	{
        /// <summary>
        /// ����
        /// </summary>
		File,

        /// <summary>
        /// �������
        /// </summary>
		Directory
	}
}
