namespace Krista.FM.Server.Scheme.Classes.Visitors
{
    /// <summary>
    /// ��������� ��������� ������� �� ������������ ��  ���������.
    /// </summary>
    internal interface IValidable
    {
        void AcceptVisitor(IValidationVisitor visitor);
    }

}
