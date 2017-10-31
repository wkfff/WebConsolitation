using System;
using System.Collections.Generic;

using Krista.FM.Domain;

namespace Krista.FM.Common.Consolidation.Forms
{
    [CLSCompliant(false)]
    public interface IFormValidationService
    {
        /// <summary>
        /// ������ �������� �����.
        /// </summary>
        /// <param name="form">����������� �����.</param>
        /// <returns>��������� ��������. ������ ������.</returns>
        List<string> Validate(D_CD_Templates form);

        /// <summary>
        /// �������� ��������� �����.
        /// </summary>
        /// <param name="form">����������� �����.</param>
        /// <returns>��������� ��������. ������ ������.</returns>
        List<string> ValidateForm(D_CD_Templates form);

        /// <summary>
        /// �������� ������������ �������� ��������� �����.
        /// </summary>
        /// <param name="form">����������� �����.</param>
        /// <returns>��������� ��������. ������ ������.</returns>
        List<string> ValidateMapping(D_CD_Templates form);

        /// <summary>
        /// �������� �������� ����� Excel � ������������ �������� ��������.
        /// </summary>
        /// <param name="form">����������� �����.</param>
        /// <returns>��������� ��������. ������ ������.</returns>
        List<string> ValidateLayout(D_CD_Templates form);
    }
}