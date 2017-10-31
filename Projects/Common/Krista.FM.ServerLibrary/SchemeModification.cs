using System;
using System.Collections.Generic;

namespace Krista.FM.ServerLibrary
{
    /// <summary>
    /// ���� �����������.
    /// </summary>
    public enum ModificationTypes : int
    {
        /// <summary>
        /// �������� ������ �������.
        /// </summary>
        Create = 0,

        /// <summary>
        /// ��������� �������.
        /// </summary>
        Modify = 1,

        /// <summary>
        /// �������� ������������� �������.
        /// </summary>
        Remove = 2,

        /// <summary>
        /// ������������ �������� �����������.
        /// </summary>
        Inapplicable = 3
    }

    /// <summary>
    /// ��������� �������� �����������.
    /// </summary>
    public enum ModificationStates
    {
        NotApplied,
        Applied,
        AppliedWithErrors,
        AppliedPartially
    }

    public delegate void ModificationMessageEventHandler(object sender, ModificationMessageEventArgs e);

    /// <summary>
    /// ��� ������� �����������.
    /// </summary>
    public enum ModificationEventTypes
    {
        StartOperation,
        EndOperation
    }

    /// <summary>
    /// ������ ������� �����������.
    /// </summary>
    [Serializable]
    public class ModificationMessageEventArgs : EventArgs
    {
        private string message;
        private IModificationItem item;
        private ModificationEventTypes type;
        private int indentLevel;

        /// <summary>
        /// ������������� ���������� ������.
        /// </summary>
        /// <param name="message">����� ���������.</param>
        public ModificationMessageEventArgs(string message)
        {
            this.message = message;
        }

        /// <summary>
        /// ������������� ���������� ������.
        /// </summary>
        /// <param name="message">����� ���������.</param>
        /// <param name="item">�������� ����������� ���������.</param>
        /// <param name="type">��� �������� �����������.</param>
        /// <param name="indentLevel">������� ����������� ��������.</param>
        public ModificationMessageEventArgs(string message, IModificationItem item, ModificationEventTypes type, int indentLevel)
            : this(message)
        {
            this.item = item;
            this.type = type;
            this.indentLevel = indentLevel;
        }

        /// <summary>
        /// �������������� ���������.
        /// </summary>
        public string Message
        {
            get { return message; }
        }

        /// <summary>
        /// �������� ����������� ���������.
        /// </summary>
        public IModificationItem Item
        {
            get { return item; }
        }

        /// <summary>
        /// ��� ������� �����������.
        /// </summary>
        public ModificationEventTypes Type
        {
            get { return type; }
        }

        /// <summary>
        /// ������� ����������� ��������.
        /// </summary>
        public int IndentLevel
        {
            get { return indentLevel; }
        }
    }

    /// <summary>
    /// �������� � ������� ����������� ��������� ��������� �����.
    /// </summary>
    public interface IModificationContext : IDisposable
    {
        /// <summary>
        /// ������������� � ������ �������� ����������.
        /// </summary>
        /// <returns>ID ������ ��������� "�������� ������������".</returns>
        int BeginUpdate();
        
        /// <summary>
        /// ������������� � ������ �������� ����������.
        /// </summary>
        void EndUpdate();

        event ModificationMessageEventHandler OnModificationMessage;
    }

    /// <summary>
    /// �������� ����������� ���������.
    /// </summary>
    public interface IModificationItem : IDisposable
    {
        /// <summary>
        /// �������� �������� �����������.
        /// </summary>
        Dictionary<string, IModificationItem> Items { get; }

        /// <summary>
        /// ���������� ���� � ������ ��������.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// ��� �������� ����������� ������������ � ����������.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// ��� �������� �����������.
        /// </summary>
        ModificationTypes Type { get; }

        /// <summary>
        /// ��������� �������� �����������.
        /// </summary>
        ModificationStates State { get; }

        /// <summary>
        /// ���������� ��������� ��� ���������� ��������.
        /// </summary>
        Exception Exception { get; }

        /// <summary>
        /// ImageIndex
        /// </summary>
        int ImageIndex { get; }

        /// <summary>
        /// ���������� ���������.
        /// </summary>
        /// <param name="context">�������� � ������� ����������� ��������� ��������� �����.</param>
        /// <param name="isAppliedPartially">������� ��������-��������� ���������� ���������</param>
        void Applay(IModificationContext context, out bool isAppliedPartially);
    }

    /// <summary>
    /// ���������� ������ ��� ��������� � ���������� ���������
    /// </summary>
    public interface IModifiable
    {
        /// <summary>
        /// ��������� ������ ������� (�������� ���������) �������� ������� �� toObject
        /// </summary>
        /// <param name="toObject">������ � ������� ����� ������������� ���������</param>
        /// <returns>������ ������� (�������� ���������)</returns>
        IModificationItem GetChanges(IModifiable toObject);
    }

    /// <summary>
    /// ���������� ������ ��� ��������� � ���������� ���������
    /// </summary>
    public interface IMinorModifiable : IModifiable
    {
        /// <summary>
        /// ���������� ���������. �������� ������� ������ � ���� ������� toObject
        /// </summary>
        /// <param name="toObject">������ � ���� �������� ����� �������� ������� ������</param>
        //void Update(IModifiable toObject);
    }

    /// <summary>
    /// ���������� ������ ��� ��������� � ���������� ���������
    /// </summary>
    public interface IMajorModifiable : IMinorModifiable
    {
        IModificationItem GetChanges();
    }

    /// <summary>
    /// ���������� ��������� ��������.
    /// </summary>
    /// <typeparam name="TKey">���������� ����.</typeparam>
    /// <typeparam name="TValue">��������.</typeparam>
    public interface IModifiableCollection<TKey, TValue> : IDictionaryBase<TKey, TValue>, IDisposable
    {
        /// <summary>
        /// ������� ����� ������� �� �������� � ���������.
        /// </summary>
        /// <returns>��������� ������.</returns>
        TValue CreateItem();
    }
}
