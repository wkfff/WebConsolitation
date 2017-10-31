using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.MDXExpert
{
    public delegate void TablePagerChangeEventHandler();
    public delegate void PageSizeChangeEventHandler();

    public partial class TablePager : UserControl
    {
        /// <summary>
        /// максимальный размер страницы, значение по умолчанию
        /// </summary>
        public const int DefaultPageSize = 1000;
        //пользователь переключил страницу
        private TablePagerChangeEventHandler _pageChanged = null;
        //пользователь изменил максимальное количество строк отображаемое в таблице
        private PageSizeChangeEventHandler _pageSizeChanged = null;
        //предыдущий номер страницы
        int preparePageNumber = 1;
    
        //Общее кол-во записей в таблице
        private int recordCount = 0;
                
        //Кол-во записей на одной странице
        private int _pageSize = DefaultPageSize;

        public TablePager()
        {
            InitializeComponent();
        }

        public void SetControlState()
        {
            buttonFirst.Enabled = (((int)pageEditor.MinValue) != ((int)pageEditor.Value));
            buttonPrev.Enabled = buttonFirst.Enabled;
            buttonLast.Enabled = (((int)pageEditor.MaxValue) != ((int)pageEditor.Value));
            buttonNext.Enabled = buttonLast.Enabled;   
        }

        public void DoPageChagned()
        {
            if (_pageChanged != null)
            {        
                _pageChanged();
                this.SetControlState();
            }
        }

        public void DoPageSizeChanged()
        {
            if (_pageSizeChanged != null)
            {
                _pageSizeChanged();
            }
        }

        private void buttonFirst_Click(object sender, EventArgs e)
        {
            this.preparePageNumber = this.CurrentPageNumber;
            CurrentPageNumber = 1;
        }

        private void buttonPrev_Click(object sender, EventArgs e)
        {
            this.preparePageNumber = this.CurrentPageNumber;
            CurrentPageNumber--;            
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            this.preparePageNumber = this.CurrentPageNumber;
            CurrentPageNumber++;
        }

        private void buttonLast_Click(object sender, EventArgs e)
        {
            this.preparePageNumber = this.CurrentPageNumber;
            CurrentPageNumber = PageCount;
        }

        private void pageEditor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsWhiteSpace(e.KeyChar)) DoPageChagned();
        }

        /// <summary>
        /// Выставляет максимальное количество записей на одной странице, без последующего обновления
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void SetPageSizeWithoutRefresh(int value)
        {
            if (value > 0)
            {
                _pageSize = value;
            }
        }

        /// <summary>
        /// Выстваляет текущий номер страницы без обновления
        /// </summary>
        /// <param name="value"></param>
        public void SetPageNumberWithoutRefresh(int value)
        {
            if (value > 0)
                this.pageEditor.Value = value;
        }

        /// <summary>
        /// Направление последнего передвижение по страницам
        /// </summary>
        public DirectionMode Direction
        {
            get
            {
                int currentPageNumber = (int)pageEditor.Value;
                DirectionMode direction = DirectionMode.Never;
                if (currentPageNumber > this.preparePageNumber)
                    direction = DirectionMode.Up;
                else
                    if (currentPageNumber < this.preparePageNumber)
                        direction = DirectionMode.Down;
                
                return direction;
            }
        }

        /// <summary>
        /// Общее кол-во записей в таблице
        /// </summary>
        public int RecordCount
        {
            get { return recordCount; }
            set
            {
                recordCount = value;
                labelPageCount.Text = string.Format("Всего страниц {0}. Всего строк {1}.",
                                                    PageCount.ToString(), value.ToString());
                pageEditor.MaxValue = PageCount;
                //Если все умещается на одну страницу, то соответсвенно и номер страницу будет = 1
                if (recordCount <= this.PageSize)
                    pageEditor.Value = 1;
            }
        }

        private void RefreshPager()
        {
            this.RecordCount = this.RecordCount;
            if (this.CurrentPageNumber > this.PageCount)
                this.pageEditor.Value = this.PageCount;
            this.SetControlState();
        }

        /// <summary>
        /// Кол-во записей на одной странице, во время выставленние данного свойства 
        /// происходит обновление таблицы, что бы выставляеть без обновления использовать
        /// SetPageSizeWithoutRefresh
        /// </summary>
        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                if ((value > 0) && (_pageSize != value))
                {
                    _pageSize = value;
                    this.RefreshPager();
                    this.DoPageSizeChanged();
                }
            }
        }

        /// <summary>
        /// Общее кол-во страниц
        /// </summary>
        public int PageCount
        {
            get
            {
                //!!!Math.Ceiling почему-то не так работаеть                                 
                decimal quotient = RecordCount / PageSize;
                return ((int)Math.Ceiling(quotient)) + 1;
            }
        }

        /// <summary>
        /// Номер текущей страницы
        /// </summary>
        public int CurrentPageNumber
        {
            get { return ((int)pageEditor.Value); }
            set
            {
                if (((int)pageEditor.Value) != value)
                {
                    if ((((int)pageEditor.MinValue) <= value) && (((int)pageEditor.MaxValue) >= value))
                    {
                        pageEditor.Value = value;
                        DoPageChagned();
                    }
                }
            }
        }

        /// <summary>
        /// Возвращает номер первой записи страницы
        /// </summary>
        public int LowNumberOnPage
        {
            get
            {
                return (CurrentPageNumber - 1) * PageSize;
            }
        }

        /// <summary>
        /// Возвращает номер последней записи страницы
        /// </summary>
        public int HighNumberOnPage
        {
            get
            {
                return (LowNumberOnPage + PageSize);
            }
        }

        public event TablePagerChangeEventHandler PageChanged
        {
            add { _pageChanged += value; }
            remove { _pageChanged -= value; }
        }

        public event PageSizeChangeEventHandler PageSizeChanged
        {
            add { _pageSizeChanged += value; }
            remove { _pageSizeChanged -= value; }
        }
    }

    public enum DirectionMode
    {
        Down,
        Never,
        Up
    }
}
