using System;
using System.Runtime.Serialization;

namespace Krista.FM.Common
{
	/// <summary>
	/// ���������� ����������� ��� �������� ������������ ������� �� ������.
	/// </summary>
	[Serializable]
	public class InvalidateOlapObjectException : OlapProcessorException
	{
		/// <summary>
		/// ������ ����� ������ ������.
		/// </summary>
		public InvalidateOlapObjectException()
		{
		}

		/// <summary>
		/// ������ ����� ������ ������ � �������� ���������� �� ������.
		/// </summary>
		/// <param name="message">��������� �� ������.</param>
		public InvalidateOlapObjectException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// ����������� ��� ��������������.
		/// </summary>
		/// <param name="info">�������� ������ ��� ��������������.</param>
		/// <param name="context">���������� �� ��������� ������.</param>
		protected InvalidateOlapObjectException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		/// <summary>
		/// ������ ����� ������ ������ � ��������� ���������� �� ������ � ������� �� ���������� ����������.
		/// </summary>
		/// <param name="message">��������� �� ������.</param>
		/// <param name="innerException">������ �� ���������� ����������.</param>
		public InvalidateOlapObjectException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}