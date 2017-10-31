using Krista.FM.Server.Scheme.Classes;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Modifications
{
    /// <summary>
    /// �������� �� �������� ����� ��������
    /// </summary>
    internal sealed class CreateModificationItem : MajorObjectModificationItem
    {
        /// <summary>
        /// �������� �� �������� ����� ��������
        /// </summary>
        /// <param name="name">������������ �������� (��� ������������ �������)</param>
        /// <param name="ownerObject">������������ ������ � ������� ������ ���������� ��������� ������</param>
        /// <param name="toObject">����������� ������</param>
        /// <param name="parent">������������ �������� �����������</param>
        public CreateModificationItem(string name, object ownerObject, object toObject, ModificationItem parent)
            : base(ModificationTypes.Create, name, ownerObject, toObject, parent)
        {
        }

        protected override void OnBeforeChildApplay(ModificationContext context, out bool isAppliedPartially)
        {
            base.OnBeforeChildApplay(context, out isAppliedPartially);

            if (ToObject is CommonDBObject)
            {
                // ���������� ������ � ���������� �������,
                // �� ����� ��������� ��� ������ ��������� �� xml �������� �� �����������.
                ((CommonDBObject)ToObject).SetParent((CommonDBObject)FromObject);
                ((CommonDBObject)ToObject).Create(context);
            }
        }
    }
}
