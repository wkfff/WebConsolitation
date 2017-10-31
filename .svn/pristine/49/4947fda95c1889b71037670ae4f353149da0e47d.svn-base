using System;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;

using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win;

using Krista.FM.Client.ViewObjects.AssociatedCLSUI.Association;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls
{
    public abstract partial class BaseClsUI : BaseViewObj, IInplaceClsView
    {
        protected IUsersManager um = null;

        protected bool allowAddRecord = true;
        protected bool allowChangeHierarchy = true;
        protected bool allowClearClassifier = true;
        protected bool allowDelRecords = true;
        protected bool allowEditRecords = true;
        protected bool allowImportClassifier = true;
        protected bool allowSetHierarchy = true;
        protected bool viewOnly = false;
        protected bool allowAddNewDataSource = true;

        protected string currentClassifierName;

        /// <summary>
        /// проверка прав для классификаторов и таблиц фактов
        /// </summary>
        public virtual void CheckClassifierPermissions()
        {
            um = this.Workplace.ActiveScheme.UsersManager;
            currentClassifierName = this.ActiveDataObj.ObjectKey; // currentClassifierName = this.ActiveDataObj.FullName; 
        }

        public virtual void SetPermissionsToClassifier(UltraGridEx gridEx)
        {

        }
    }
}
