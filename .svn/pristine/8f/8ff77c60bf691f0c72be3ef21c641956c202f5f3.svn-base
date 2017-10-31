using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.Client.Design;
using Krista.FM.Client.SchemeEditor.Gui;


namespace Krista.FM.Client.SchemeEditor.Services.NavigationService
{
    /// <summary>
    /// Управляет историей смены состояний.
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
        /// Возвращает количество элементов истории назад
        /// </summary>
        public int CountBack
        {
            get { return listBackward.Count; }
        }

        /// <summary>
        /// Возвращает количество элементов истории впред
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
        /// Обрабатывает нажатие назад.
        /// </summary>
        /// <param name="client">Клиент, для которого происходит обработка.</param>
        /// <param name="index">На сколько элементов сместиться</param>
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
            this.whileNavigate = false;  // переход по кнопке навигации    
            if (client is INavigation) // если клиент реализует интерфейс
            {
                // вызываем переход
                ((INavigation)client).SelectObject(this.clientState, true);
            }            
        }
         
        /// <summary>
        /// Делаем копию списка назад.
        /// </summary>
        /// <returns>Копия списка назад.</returns>
        public string[] initHistBackArray()
        {
            string[] tempHist = new string[histCapacity]; 
           
            this.listBackward.CopyTo(tempHist, 0);
            return tempHist;           
        }
        
        /// <summary>
        /// Делаем копию списка вперед.
        /// </summary>
        /// <returns>Копия списка вперед</returns>
        public string[] initHistForwardArray()
        {
            string[] tempHist = new string[histCapacity];
            this.listForward.CopyTo(tempHist, 0);
            return tempHist;
        }

        /// <summary>
        /// Проверяет, есть ли история переходов назад.
        /// </summary>
        /// <returns>true, если есть история переходов, false  в противном случае</returns>
        public bool CanBackward()
        {
            return this.listBackward.Count > 0;
        }

        /// <summary>
        /// Обрабатывает нажатие вперед.
        /// </summary>
        /// <param name="client">Клиент, для которого происходит обработка.</param>
        /// <param name="index">На сколько элементов сместиться</param>
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
            this.whileNavigate = false;  // переход по кнопке навигации
            if (client is INavigation)   // если клиент реализует интерфейс 
            {
                // вызываем переход
                ((INavigation)client).SelectObject(this.clientState, true) ;
            }
            
        }

        /// <summary>
        /// Проверяет, есть ли история переходов вперед.
        /// </summary>
        /// <returns>true, если есть история переходов, false  в противном случае.</returns>
        public bool CanForward()
        {
            return this.listForward.Count > 0;
        }

        /// <summary>
        /// Очищает историю и активный элемент.
        /// </summary>
        private void Reset()
        {
            this.listBackward.Clear();
            this.listForward.Clear();
            this.clientState = null;
        }

        /// <summary>
        /// Обрабатывает выделение элемента.
        /// </summary>
        /// <param name="state">Состояние клиента.</param>
        public void OnStateChange(string state)
        {
            // Если выделение произошло в результате нажатия на кнопку навигации
            // не добавляем элементы в списки.
            if (!this.WhileNavigate)
            {
                this.whileNavigate = true;
                return;
            }
            // Если нет текущего элемента
            if (this.clientState == null)
            {
                this.clientState = state; // текущему присваиваем выделенный
            }
            else
            {
                // если выделенный элемент не совпадает с текущим
                if (state != this.clientState)
                {
                    this.listBackward.AddLast(this.clientState); // помещаем текущий элемент в список
                    this.clientState = state; // текущему присваиваем выделенный
                    this.listForward.Clear(); // очищаем историю вперед
                }
                if (this.listBackward.Count > histCapacity) // если количество превышает допустимое
                {
                    this.listBackward.RemoveFirst(); // удаляем первый элемент
                }
            }
        }
    }
}
