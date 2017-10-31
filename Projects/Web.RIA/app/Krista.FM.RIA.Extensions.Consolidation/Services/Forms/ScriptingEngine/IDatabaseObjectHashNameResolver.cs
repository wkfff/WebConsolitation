namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ScriptingEngine
{
    public interface IDatabaseObjectHashNameResolver
    {
        /// <summary>
        /// ���������� ������������ ���-���, ��� ���� ������� 30 ��������, ��� null, ���� ���-��� �� ����������.
        /// ������ �����: [������ 20 �������� �� ������������� ����]$[��� (7 �������� base64)]$[���������� ��������]
        /// </summary>
        string Get(string longName, ObjectTypes objectType);

        /// <summary>
        /// ������� ���-���, ��� ���� ������� 30 ��������. 
        /// ���� ����� ��� ��� ����������������, �� ������ ���������� InvalidOperationException.
        /// ������ �����: [������ 20 �������� �� ������������� ����]$[��� (7 �������� base64)]$[���������� ��������]
        /// </summary>
        string Create(string longName, ObjectTypes objectType);

        /// <summary>
        /// ������� ���-���, ��� ���� ������� 30 ��������.
        /// </summary>
        void Delete(string longName, ObjectTypes objectType);
    }
}