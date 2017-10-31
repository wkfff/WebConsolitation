using System;
using System.Drawing;

using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.FixedCls
{
	public class FixedClsUI : BaseClsUI
	{
        public FixedClsUI(IEntity dataObject)
            : base(dataObject)
		{
			Index = 1;
			Caption = "������������� ��������������";
			clsClassType = ClassTypes.clsFixedClassifier;
		}

        public override Icon Icon
        {
            get { return Icon.FromHandle(Properties.Resources.cls_Fixed_16.GetHicon()); }
        }

        public override System.Drawing.Image TypeImage16
		{
			get { return Krista.FM.Client.ViewObjects.AssociatedCLSUI.Properties.Resources.cls_Fixed_16; }
		}

		public override System.Drawing.Image TypeImage24
		{
			get { return Krista.FM.Client.ViewObjects.AssociatedCLSUI.Properties.Resources.cls_Fixed_24; }
		}

        public override void InitializeUI()
        {
            base.InitializeUI();
            // ������ �������� ����������, ������������� � �����
            vo.utcDataCls.Tabs[1].Visible = false;
            vo.utcDataCls.Tabs[2].Visible = false;
            //vo.utcDataCls.Tabs[3].Visible = false;
            // ..��������� ����������� ��������������
            vo.ugeCls.IsReadOnly = true;
            vo.utbToolbarManager.Tools["disin"].SharedProps.Visible = false;
            vo.ugeCls.StateRowEnable = false;
        }

        public override bool HasDataSources()
        {
            return false;
        }

        protected override void SetViewObjectCaption()
        {
            if (ActiveDataObj == null)
                return;
            Workplace.ViewObjectCaption = string.Format("{0}: {1}", "������������� �������������", GetClsRusName());
        }

        public override void CheckClassifierPermissions()
        {
            base.CheckClassifierPermissions();
            vo.ugeCls.AllowAddNewRecords = false;
            vo.ugeCls.AllowClearTable = false;
            vo.ugeCls.AllowDeleteRows = false;
            vo.ugeCls.AllowEditRows = false;
            vo.ugeCls.AllowImportFromXML = false;
            vo.ugeCls.IsReadOnly = true;
            vo.ugeCls.StateRowEnable = false;
        }

        protected override IDataUpdater GetActiveUpdater(int? parentID, out string filterStr)
        {
            dataQuery = string.Empty;
            // �������� ��������� ���������� ������
            dataQuery = "(RowType = 0)";
            return base.GetActiveUpdater(parentID, out filterStr);
        }

	}
}