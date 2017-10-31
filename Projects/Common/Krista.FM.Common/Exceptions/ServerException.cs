using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Krista.FM.Common
{
    /// <summary>
    /// ���������� �� ������� �������.
    /// </summary>
    [Serializable]
    public class ServerException : Exception
    {
        /// <summary>
        /// ������ ����� ������ ������.
        /// </summary>
        public ServerException()
            : base()
        {
        }

        /// <summary>
        /// ������ ����� ������ ������ � �������� ���������� �� ������.
        /// </summary>
        /// <param name="message">��������� �� ������.</param>
        public ServerException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// ����������� ��� ��������������.
        /// </summary>
        /// <param name="info">�������� ������ ��� ��������������.</param>
        /// <param name="context">���������� �� ��������� ������.</param>
        protected ServerException(SerializationInfo info, StreamingContext context)
            : base (info, context)
        {
        }

        /// <summary>
        /// ������ ����� ������ ������ � ��������� ���������� �� ������ � ������� �� ���������� ����������.
        /// </summary>
        /// <param name="message">��������� �� ������.</param>
        /// <param name="innerException">������ �� ���������� ����������.</param>
        public ServerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
