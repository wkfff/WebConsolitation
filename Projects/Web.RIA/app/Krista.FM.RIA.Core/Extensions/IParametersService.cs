using System;
using System.IO;

namespace Krista.FM.RIA.Core.Extensions
{
    public interface IParametersService
    {
        /// <summary>
        /// ������������ ���������������� ���������� ����������.
        /// </summary>
        /// <param name="stream">������������ ����������.</param>
        void RegisterExtensionConfigParameters(Stream stream);

        /// <summary>
        /// ������������ ���������������� ���������� ����������.
        /// </summary>
        /// <param name="name">��� ��������������� ���������.</param>
        /// <param name="type">��� ���������.</param>
        void RegisterExtensionConfigParameter(string name, Type type);

        /// <summary>
        /// ���������� ����������� �������� ���������.
        /// </summary>
        /// <param name="parameterName">��� ���������.</param>
        /// <returns>�������� ���������.</returns>
        string GetParameterValue(string parameterName);

        void Clear();
    }
}