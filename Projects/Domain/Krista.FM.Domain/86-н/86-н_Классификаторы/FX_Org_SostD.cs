namespace Krista.FM.Domain
{
    public class FX_Org_SostD : ClassifierTable
    {
        /// <summary>
        /// ��������� "������������"
        /// </summary>
        public const int NewStateID = 1;

        /// <summary>
        /// ��������� "������"
        /// </summary>
        public const int CreatedStateID = 2;

        /// <summary>
        /// ��������� "�� ������������"
        /// </summary>
        public const int UnderConsiderationStateID = 4;

        /// <summary>
        /// ��������� "�� ���������"
        /// </summary>
        public const int OnEditingStateID = 5;

        /// <summary>
        /// ��������� "���������"
        /// </summary>
        public const int FinishedStateID = 7;

        /// <summary>
        /// ��������� "�������������"
        /// </summary>
        public const int ExportedStateID = 8;

        public static readonly string Key = "243f0a9e-5946-41ff-8b75-8aa75f9e820c";

        public virtual int RowType { get; set; }

        public virtual string Name { get; set; }
    }
}
