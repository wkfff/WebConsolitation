namespace Krista.FM.Domain
{
    public class FX_FX_typeFinSupport : ClassifierTable
    {
        /// <summary>
        /// ����������� ������ ����������
        /// </summary>
        public const int OwnRevenuesID = 0;
        
        /// <summary>
        /// �������� �� ���������� ���������������� (��������������) �������
        /// </summary>
        public const int SubsidyID = 1;

        /// <summary>
        /// �������� �� ���� ����
        /// </summary>
        public const int OtherPurposesID = 2;

        /// <summary>
        /// ��������� ����������
        /// </summary>
        public const int BudgetaryInvestmentID = 3;

        /// <summary>
        /// �������� �� ������������� ������������ �����������
        /// </summary>
        public const int FundsID = 4;
        
        public static readonly string Key = "16cfdeed-361d-480a-945b-486835324fc3";

        public virtual int RowType { get; set; }

        public virtual string Code { get; set; }

        public virtual string Name { get; set; }
    }
}
