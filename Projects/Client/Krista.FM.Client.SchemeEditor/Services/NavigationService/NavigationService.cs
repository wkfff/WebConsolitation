using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.Client.Design;
using Krista.FM.Client.SchemeEditor.Gui;


namespace Krista.FM.Client.SchemeEditor.Services.NavigationService
{
    /// <summary>
    /// ��������� �������� ����� ���������.
    /// </summary>
    public partial class NavigationService
    {
        private LinkedList<string> listBackward;
        private LinkedList<string> listForward;
        private string clientState;
        private int histCapacity = 100;
        private static NavigationService singleInstance;
        private bool whileNavigate;

        public bool WhileNavigate
        {
            get { return whileNavigate; }
        }   

        /// <summary>
        /// ���������� ���������� ��������� ������� �����
        /// </summary>
        public int CountBack
        {
            get { return listBackward.Count; }
        }

        /// <summary>
        /// ���������� ���������� ��������� ������� �����
        /// </summary>
        public int CountForward
        {
            get { return listForward.Count; }
        }

        public NavigationService()
        {
            this.listBackward = new LinkedList<string>();
            this.listForward = new LinkedList<string>();
            whileNavigate = true;
            Reset();
        }

        public static NavigationService Instance
        {
            get
            {
                if (singleInstance == null)
                {
                    singleInstance = new NavigationService();
                }
                return singleInstance;
            }
        }

        /// <summary>
        /// ������������ ������� �����.
        /// </summary>
        /// <param name="client">������, ��� �������� ���������� ���������.</param>
        /// <param name="index">�� ������� ��������� ����������</param>
        public void Backward(object client, int index)
        {           
            for (int i = 0; i < (index); i++)
            {
                if (CanBackward())
                {
                    this.listForward.AddLast(this.clientState);
                    this.clientState = this.listBackward.Last.Value;
                    this.listBackward.RemoveLast();                    
                }
             }
            this.whileNavigate = false;  // ������� �� ������ ���������    
            if (client is INavigation) // ���� ������ ��������� ���������
            {
                // �������� �������
                ((INavigation)client).SelectObject(this.clientState, true);
            }            
        }
         
        /// <summary>
        /// ������ ����� ������ �����.
        /// </summary>
        /// <returns>����� ������ �����.</returns>
        public string[] initHistBackArray()
        {
            string[] tempHist = new string[histCapacity]; 
           
            this.listBackward.CopyTo(tempHist, 0);
            return tempHist;           
        }
        
        /// <summary>
        /// ������ ����� ������ ������.
        /// </summary>
        /// <returns>����� ������ ������</returns>
        public string[] initHistForwardArray()
        {
            string[] tempHist = new string[histCapacity];
            this.listForward.CopyTo(tempHist, 0);
            return tempHist;
        }

        /// <summary>
        /// ���������, ���� �� ������� ��������� �����.
        /// </summary>
        /// <returns>true, ���� ���� ������� ���������, false  � ��������� ������</returns>
        public bool CanBackward()
        {
            return this.listBackward.Count > 0;
        }

        /// <summary>
        /// ������������ ������� ������.
        /// </summary>
        /// <param name="client">������, ��� �������� ���������� ���������.</param>
        /// <param name="index">�� ������� ��������� ����������</param>
        public void Forward(object client, int index)
        {   
            for (int i = 0; i < (index); i++)
            {
                if (CanForward())
                {
                    this.listBackward.AddLast(this.clientState);
                    this.clientState = this.listForward.Last.Value;
                    this.listForward.RemoveLast();                        
                }
            }
            this.whileNavigate = false;  // ������� �� ������ ���������
            if (client is INavigation)   // ���� ������ ��������� ��������� 
            {
                // �������� �������
                ((INavigation)client).SelectObject(this.clientState, true) ;
            }
            
        }

        /// <summary>
        /// ���������, ���� �� ������� ��������� ������.
        /// </summary>
        /// <returns>true, ���� ���� ������� ���������, false  � ��������� ������.</returns>
        public bool CanForward()
        {
            return this.listForward.Count > 0;
        }

        /// <summary>
        /// ������� ������� � �������� �������.
        /// </summary>
        private void Reset()
        {
            this.listBackward.Clear();
            this.listForward.Clear();
            this.clientState = null;
        }

        /// <summary>
        /// ������������ ��������� ��������.
        /// </summary>
        /// <param name="state">��������� �������.</param>
        public void OnStateChange(string state)
        {
            // ���� ��������� ��������� � ���������� ������� �� ������ ���������
            // �� ��������� �������� � ������.
            if (!this.WhileNavigate)
            {
                this.whileNavigate = true;
                return;
            }
            // ���� ��� �������� ��������
            if (this.clientState == null)
            {
                this.clientState = state; // �������� ����������� ����������
            }
            else
            {
                // ���� ���������� ������� �� ��������� � �������
                if (state != this.clientState)
                {
                    this.listBackward.AddLast(this.clientState); // �������� ������� ������� � ������
                    this.clientState = state; // �������� ����������� ����������
                    this.listForward.Clear(); // ������� ������� ������
                }
                if (this.listBackward.Count > histCapacity) // ���� ���������� ��������� ����������
                {
                    this.listBackward.RemoveFirst(); // ������� ������ �������
                }
            }
        }
    }
}
