using System.Collections.Generic;
using Krista.FM.RIA.Core.ViewModel;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.DebtBook
{
    public interface IChangesCalcService
    {
        /// <summary>
        /// �������� ���� �������� ��������.
        /// </summary>
        /// <param name="entity">�������� ��������.</param>
        /// <param name="documentId">Id ��������.</param>
        /// <param name="formules">������ ������.</param>
        void Recalc(IEntity entity, int documentId, Dictionary<string, ColumnState> formules);
    }
}