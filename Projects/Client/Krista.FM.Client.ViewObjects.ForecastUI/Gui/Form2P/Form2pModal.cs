using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.ViewObjects.ForecastUI
{
	class SelectForm2pUI : ForecastFactTablesUI
	{
		public SelectForm2pUI(IEntity dataObject)
			: base(dataObject)
		{

		}

		
	}

	class Form2pModal : FactTableModal
	{
		public int ShowForm2pModal()
		{
			return ShowFactTableModal(SchemeObjectsKeys.f_S_Form2p_Key);
		}

		public override void CreateFactTableUI(IFactTable cls)
		{
			clsUI = new SelectForm2pUI(cls);
		}
	}
}
