using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Common.Handling
{
    /// <summary>
    /// ����� ��� ��������������� ������� ������� �������.
    /// </summary>
    /// <remarks>
    /// ������ ������ ������� ��������� ������.
    /// ��� ������� ������� ���������������� ���������� OnServerModificationMessage.
    /// ��� ��������� ������� �� ������� ����� ����������� �� OnClientModificationMessage.
    /// </remarks>
    public class ModificationMessageHandling : DisposableObject
    {
        /// <summary>
        /// ���������� ��� ������� �������.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e"></param>
        public void OnServerModificationMessage(object sender, ModificationMessageEventArgs args)
        {
            if (OnClientModificationMessage != null)
            {
                OnClientModificationMessage(sender, args);
                System.Windows.Forms.Application.DoEvents();
            }
        }

        /// <summary>
        /// ������� � �������� ������������� ������.
        /// </summary>
        public event ModificationMessageEventHandler OnClientModificationMessage;
    }
}
