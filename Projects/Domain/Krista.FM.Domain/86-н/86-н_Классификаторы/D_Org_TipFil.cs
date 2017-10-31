namespace Krista.FM.Domain
{
    public class D_Org_TipFil : ClassifierTable
    {
        /// <summary> 
        /// ������
        /// </summary>
        public const int Branch = 1;

        /// <summary>
        /// ������������ ����������� �������������
        /// </summary>
        public const int SeparateUnit = 2;

        /// <summary>
        /// �����������������
        /// </summary>
        public const int Agency = 3;

        public static readonly string Key = "b3b43c0c-d8fc-4699-8563-9d57071443d9";
        
        public virtual int RowType { get; set; }

        public virtual int Code { get; set; }

        public virtual string Name { get; set; }
    }
}
