namespace Krista.FM.RIA.Extensions.E86N.Services.Reports
{
    public class Docs
    {
        /// <summary>
        /// ������� ����� ������ ���(������)
        /// </summary>
        public bool Root { get; set; }

        public bool Numerable { get; set; }

        /// <summary>
        /// ������������ "��� ��� ����"
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ����� ����������
        /// </summary>
        public int InstitutionCount { get; set; }

        /// <summary>
        /// ����� ����������
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// ����������� ����������
        /// </summary>
        public int NotCreated { get; set; }

        /// <summary>
        /// ���������� � ��������� ������
        /// </summary>
        public int Created { get; set; }

        /// <summary>
        /// ���������� �� ������������
        /// </summary>
        public int UnderConsideration { get; set; }

        /// <summary>
        /// ���������� �� ���������
        /// </summary>
        public int OnEditing { get; set; }

        /// <summary>
        /// ���������� ����������
        /// </summary>
        public int Finished { get; set; }

        /// <summary>
        /// ����� ������������� ����������
        /// </summary>
        public int TotalIncomplete { get; set; }

        /// <summary>
        /// �������������� ����������
        /// </summary>
        public int Exported { get; set; }

        /// <summary>
        /// ���� ������������� ����������
        /// </summary>
        public double PartOfIncomplete { get; set; }

        /// <summary>
        /// ���������� ���������� � ������ ����������
        /// </summary>
        public int NumberOfPreparation { get; set; }

        /// <summary>
        /// ������� ����������� ��� ���������������� ���������� 
        /// </summary>
        public int PresenceOfCompleted { get; set; }

        /// <summary>
        /// ����������� ����������� ��� �� �����������
        /// </summary>
        public int NotInspectionActivity { get; set; }

        /// <summary>
        /// ����������� �����������(���������) ��� �� �����������
        /// </summary>
        public int NotInspectionActivityFinished { get; set; }

        /// <summary>
        /// �� ��������� ��
        /// </summary>
        public int NotBring { get; set; }

        /// <summary>
        /// �� ��������� ��(���������)
        /// </summary>
        public int NotBringFinished { get; set; }

        /// <summary>
        /// ���������� �������� ���������� � ������� �� ������� ��
        /// </summary>
        public int GovernmentCountNotCreatedDocs { get; set; }
    }
}