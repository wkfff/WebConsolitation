using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.SchemeEditor.Gui
{
    /// <summary>
    /// ��������� ��� �������, ������� ��������� �������� IsDirty � ������� DirtyChanged.
    /// </summary>
    public interface ICanBeDirty
    {
        /// <summary>
        /// ���� �������� ���������� true, �� ���������� ���� ��������
        /// ����� �������� ��������/����������.
        /// </summary>
        bool IsDirty { get; set; }

        /// <summary>
        /// ����������� ����� ��������� ����������� � �������, 
        /// ��� ��������� ������ ���� ���������.
        /// </summary>
        event EventHandler DirtyChanged;
    }
}
