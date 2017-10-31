using System.Reflection;

using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DomainStore
{
    public interface IDomainFormsAssembliesStore
    {
        /// <summary>
        /// ������� � ��������� � ��������� ����� �������� ������ ��� ��������� �����.
        /// </summary>
        /// <param name="form">����� ��� ������� ����� ������� �������� ������.</param>
        void Register(D_CD_Templates form);

        /// <summary>
        /// ���������� ��� ������������������ �������� ������.
        /// </summary>
        Assembly[] GetAllAssemblies();
    }
}