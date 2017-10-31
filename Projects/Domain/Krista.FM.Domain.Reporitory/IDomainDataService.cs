using System;
using System.Data;

namespace Krista.FM.Domain.Reporitory
{
    public interface IDomainDataService
    {
        /// <summary>
        /// ���������� ������� � ������� ��� ���������� ���� �������.
        /// </summary>
        /// <param name="objectType">��� �������.</param>
        /// <param name="selectFilter">������� ���������� �������.</param>
        DataRow[] GetObjectData(Type objectType, string selectFilter);

        /// <summary>
        /// ������� ����� ������ � ���� ������.
        /// </summary>
        /// <param name="obj"></param>
        void Create(DomainObject obj);

        /// <summary>
        /// ��������� ������ � ���� ������.
        /// </summary>
        /// <param name="obj"></param>
        void Update(DomainObject obj);
    }
}