namespace Krista.FM.Domain
{
    public class FX_FX_ChangeLogActionType : ClassifierTable
    {
        /// <summary>
        ///   ���������� ���������
        /// </summary>
        public const int AddDocument = 13;

        /// <summary>
        ///   �������� ���������
        /// </summary>
        public const int DeleteDocument = 2;

        /// <summary>
        ///   �� ������� ������� ��������
        /// </summary>
        public const int DeleteDocumentAbort = 3;

        /// <summary>
        ///   ��������� �� ������������
        /// </summary>
        public const int OnUnderConsiderationState = 4;

        /// <summary>
        ///   ��������� �� ���������
        /// </summary>
        public const int OnEditingState = 5;

        /// <summary>
        ///   �������� �������������
        /// </summary>
        public const int OnExportedState = 6;

        /// <summary>
        ///   �������� ��������
        /// </summary>
        public const int OnFinishedState = 7;

        /// <summary>
        ///   � �������� ������� ����� ����������.
        /// </summary>
        public const int DocumentBodyChange = 8;

        /// <summary>
        ///   ���������� ������� �� ���������
        /// </summary>
        public const int DocumentBodyDelete = 9;

        /// <summary>
        ///   �� ������� ������� ���������� �� ���������
        /// </summary>
        public const int DocumentBodyDeleteAbort = 10;

        /// <summary>
        ///   ���������� ����� ����
        /// </summary>
        public const int FileCreate = 11;

        /// <summary>
        ///   ������������� ���� ������
        /// </summary>
        public const int FileDelete = 12;

        public static readonly string Key = "abf6f24d-0935-474c-be50-74ee332339d7";

        public virtual int RowType { get; set; }

        public virtual string Name { get; set; }
    }
}
