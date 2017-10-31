using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.SchemeEditor.Gui
{
    /// <summary>
    /// ������� ��������� ��� ����, ������� ����������
    /// ���������� ����������� ��������� IViewContent.
    /// </summary>
    public interface IWorkbenchWindow
    {
        /// <summary>
        /// ���������� ����.
        /// </summary>
        IViewContent ViewContent { get; }

        /// <summary>
        /// ������� ���������� ����.
        /// </summary>
        IBaseViewContent ActiveViewContent { get; }

        /// <summary>
        /// ��������� ����.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// ��������� ����, ���� force == true, �� ���������
        /// ��� ������� � ���������� ��������� ����������� ����.
        /// </summary>
        /// <returns>true, ���� ���� �������.</returns>
        bool CloseWindow(bool force);

        /// <summary>
        /// ������ ���� ������� � �������� ��� �����.
        /// </summary>
        void SelectWindow();

        /// <summary>
        /// ����������������� � ��������� �����������.
        /// </summary>
        void RedrawContent();

        /// <summary>
        /// ���������� ����� �������� ����.
        /// </summary>
        event EventHandler CloseEvent;
    }
}
