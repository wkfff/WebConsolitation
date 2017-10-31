namespace Krista.FM.Domain.Services.FinSourceDebtorBook
{
    public interface IVariantProtocolService
    {
        T_S_ProtocolTransfer GetStatus(int variantId, int regionId);

        /// <summary>
        /// ��������� ��� ������ ��� ������� �� �������� � �������.
        /// </summary>
        /// <param name="variantId"></param>
        /// <param name="regionId"></param>
        /// <param name="comment"></param>
        T_S_ProtocolTransfer ToTest(int variantId, int regionId, string comment);

        void Reject(int variantId, int regionId, string comment);
        void Accept(int variantId, int regionId, string comment);
    }
}