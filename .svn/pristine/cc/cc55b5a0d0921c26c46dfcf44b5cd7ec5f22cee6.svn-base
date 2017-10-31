using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Infragistics.Win.UltraWinTree;

namespace Krista.FM.Client.Components
{
    /// <summary>
    /// Тип сортировки
    /// </summary>
	public enum SortTypeEnum
	{
        /// <summary>
        /// по тексту
        /// </summary>
		ByText, 
        /// <summary>
        /// по ключу
        /// </summary>
		ByKey
	}

    /// <summary>
    /// Расширенный Infragistics.Win.UltraWinTree.UltraTree
    /// </summary>
    public class UltraTreeEx : Infragistics.Win.UltraWinTree.UltraTree
    {
        #region Сортировка в дереве

        private TreeSortComparer treeLevelSortComparer = new TreeSortComparer();

        /// <summary>
        /// Constructor
        /// </summary>
        public UltraTreeEx()
        {
            this.Override.SortComparer = treeLevelSortComparer;
        }

        /// <summary>
        /// Сортировка по тексту по возрастанию 
        /// </summary>
        public void Sort()
        {
            this.Override.Sort = SortType.Ascending;
            this.RefreshSort();
        }

        /// <summary>
        /// Сортировка с указанными пакраметрами
        /// </summary>
        /// <param name="sortTypeEnum"> Тип сортировки по какому признаку сортировать</param>
        /// <param name="sortType"> Тип сортировки</param>
        public void Sort(SortTypeEnum sortTypeEnum, SortType sortType)
        {
            this.Override.Sort = sortType;
            this.treeLevelSortComparer.SortType = sortTypeEnum;
            this.RefreshSort();
        }

        /// <summary>
        /// Сравнение узлов в дереве
        /// </summary>
        class TreeSortComparer : IComparer
        {
            public TreeSortComparer(SortTypeEnum InitialSortType)
            {
                this.SortType = InitialSortType;
            }

            public TreeSortComparer()
            {
                this.SortType = SortTypeEnum.ByText;
            }

            private SortTypeEnum sortType;
            public SortTypeEnum SortType
            {
                get
                {
                    return sortType;
                }
                set
                {
                    sortType = value;
                }
            }

            int IComparer.Compare(object x, object y)
            {
                int returnValue = 0;

                Infragistics.Win.UltraWinTree.UltraTreeNode TreeNodeX = (Infragistics.Win.UltraWinTree.UltraTreeNode)x;
                Infragistics.Win.UltraWinTree.UltraTreeNode TreeNodeY = (Infragistics.Win.UltraWinTree.UltraTreeNode)y;

                switch (sortType)
                {
                    case SortTypeEnum.ByKey:
                        {
                            returnValue = TreeNodeX.Key.CompareTo(TreeNodeY.Key);
                            break;
                        }
                    case SortTypeEnum.ByText:
                        {
                            returnValue = TreeNodeX.Text.CompareTo(TreeNodeY.Text);
                            break;
                        }
                }

                return returnValue;
            }
        }

        #endregion Сортировка в дереве
    }
}
