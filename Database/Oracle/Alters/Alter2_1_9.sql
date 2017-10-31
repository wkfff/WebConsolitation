/*******************************************************************
 ��������� ���� Oracle �� ������ 2.1.9 � ��������� ������ 2.1.10
*******************************************************************/

/* ������ ���������� �������: */
/* Start - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */
    /* ��� SQL-������ */
/* End   - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */


/* Start - ����������� ����� */

/* ������� �� ����� ������ */
whenever SQLError exit rollback;
/* End   - ����������� ����� */


/* Start - 2765 - ���_0002_������ - ��������� ������� - mik-a-el - 9.06.2005 */

update PUMPREGISTRY
set PROGRAMCONFIG =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbMoveFilesToArchive" Text="�������� ������������ ����� � ����� ������." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
	<ControlsGroup Type="Control" ParamsKind="Individual">
		<Check Name="ucbFinalOverturn" Text="������� �������������� ��������." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
	<ControlsGroup Type="Control" ParamsKind="Individual">
		<Control Name="lbComment" Type="Label" Text="��������� ��������� ������������ ������������ ���� ������������ ������ ��� ������� ��������� ������." LocationX="13" LocationY="40" Width="600" Height="20" Value=""/>
	</ControlsGroup>
	<ControlsGroup Name="gbYear" Text="���" LocationX="13" LocationY="70" Width="232" Height="59" Type="GroupBox" ParamsKind="Individual">
		<Control Name="umeYears" Text="" Type="Combo_Years" Value="-1" LocationX="6" LocationY="20" Width="220" Height="20"/>
	</ControlsGroup>
	<ControlsGroup Name="gbMonth" Text="�����" LocationX="251" LocationY="70" Width="232" Height="59" Type="GroupBox" ParamsKind="Individual">
		<Control Name="ucMonths" Text="" Type="Combo_Months" Value="-1" LocationX="6" LocationY="19" Width="220" Height="21"/>
	</ControlsGroup>
</DataPumpParams>'
where PROGRAMIDENTIFIER = 'FUVaultPump';

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
	<PumpData State="InQueue" Comment="����������� ������� ������ �� ����������, ����������� �������� ���������������."/>
	<ProcessData State="InQueue" Comment="����������� ��������� ������������ ������������ ���� �� ���������� ������."/>
	<AssociateData State="InQueue" Comment="����������� ������������� ������ ��������������� �� ���������� ����������."/>
	<ProcessCube State="InQueue" Comment="����������� �������� ��� ���������� ��������� �� ������ ���������� ����������."/>
	<CheckData State="InQueue" Comment="������� �������� �� �����������."/>
</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'FUVaultPump';

commit;

/* End   - 2765 - ���_0002_������ - ��������� ������� - mik-a-el - 9.06.2005 */


/* Start - 2887 - ����� ���� ������� - ��������������� �������� - mik-a-el - 21.06.2006 */

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
	<PreviewData State="InQueue" Comment="��������������� �������� ������ ��� �������."/>
	<PumpData State="InQueue" Comment="����������� ������� ������ �� ����������, ����������� �������� ���������������."/>
	<ProcessData State="InQueue" Comment="������� �������� �� �����������."/>
	<AssociateData State="InQueue" Comment="����������� ������������� ������ ��������������� �� ���������� ����������."/>
	<ProcessCube State="InQueue" Comment="����������� �������� ��� ���������� ��������� �� ������ ���������� ����������."/>
	<CheckData State="InQueue" Comment="������� �������� �� �����������."/>
</DataPumpStagesParams>';

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
	<PreviewData State="InQueue" Comment="��������������� �������� ������ ��� �������."/>
	<PumpData State="InQueue" Comment="����������� ������� ������ �� ����������, ����������� �������� ���������������."/>
	<ProcessData State="InQueue" Comment="����������� ��������� ������������ ������������ ���� �� ���������� ������."/>
	<AssociateData State="InQueue" Comment="����������� ������������� ������ ��������������� �� ���������� ����������."/>
	<ProcessCube State="InQueue" Comment="����������� �������� ��� ���������� ��������� �� ������ ���������� ����������."/>
	<CheckData State="InQueue" Comment="������� �������� �� �����������."/>
</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'FUVaultPump';

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
	<PreviewData State="InQueue" Comment="������� �������� �� �����������."/>
	<PumpData State="InQueue" Comment="����������� ������� ������ �� ����������, ����������� �������� ���������������."/>
	<ProcessData State="InQueue" Comment="����������� ������������� ���� ������ �� �������� ���������������."/>
	<AssociateData State="InQueue" Comment="����������� ������������� ������ ��������������� �� ���������� ����������."/>
	<ProcessCube State="InQueue" Comment="����������� �������� ��� ���������� ��������� �� ������ ���������� ����������."/>
	<CheckData State="InQueue" Comment="������� �������� �� �����������."/>
</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'SKIFMonthRepPump' or PROGRAMIDENTIFIER = 'SKIFYearRepPump' or PROGRAMIDENTIFIER = 'FKMonthRepPump';

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
	<PreviewData State="InQueue" Comment="������� �������� �� �����������."/>
	<PumpData State="InQueue" Comment="����������� ������� ������ �� ����������, ����������� �������� ���������������."/>
	<ProcessData State="InQueue" Comment="����������� ����������� ���� ������ �� ���������� ���������� �������."/>
	<AssociateData State="InQueue" Comment="����������� ������������� ������ ��������������� �� ���������� ����������."/>
	<ProcessCube State="InQueue" Comment="����������� �������� ��� ���������� ��������� �� ������ ���������� ����������."/>
	<CheckData State="InQueue" Comment="������� �������� �� �����������."/>
</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'FNS28nDataPump' or PROGRAMIDENTIFIER = 'Form10Pump';

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
	<PreviewData State="InQueue" Comment="������� �������� �� �����������."/>
	<PumpData State="InQueue" Comment="����������� ������� ������ �� ����������, ����������� �������� ���������������."/>
	<ProcessData State="InQueue" Comment="������� �������� �� �����������."/>
	<AssociateData State="InQueue" Comment="����������� ������������� ������ ��������������� �� ���������� ����������."/>
	<ProcessCube State="InQueue" Comment="����������� �������� ��� ���������� ��������� �� ������ ���������� ����������."/>
	<CheckData State="InQueue" Comment="������� �������� �� �����������."/>
</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'LocalBudgetsDataPump' or PROGRAMIDENTIFIER = 'LeasePump' or PROGRAMIDENTIFIER = 'BudgetDataPump';

update PUMPREGISTRY
set PROGRAMCONFIG =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbMoveFilesToArchive" Text="�������� ������������ ����� � ����� ������." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbDeleteEarlierData" Text="������� ���������� ����� ������ �� ���� �� ���������." LocationX="13" LocationY="20" Width="500" Height="20" Value="True"/>
	</ControlsGroup>
	<ControlsGroup Type="Control" ParamsKind="Individual">
		<Control Name="lbComment" Type="Label" Text="��������� ����������� ������ ������������ ������ ��� ������� ��������� (�����������) ������." LocationX="13" LocationY="40" Width="600" Height="20" Value=""/>
	</ControlsGroup>
	<ControlsGroup Name="gbDisintegrationMode" Text="����� �����������" LocationX="13" LocationY="70" Width="232" Height="70" Type="GroupBox" ParamsKind="Individual">
		<Radio Name="rbtnDisintegratedOnly" Text="������ ��������������" LocationX="13" LocationY="20" Width="400" Height="20" Value="true" FontBold="false"/>
		<Radio Name="rbtnDisintAll" Text="���������� ���" LocationX="13" LocationY="40" Width="400" Height="20" Value="false" FontBold="false"/>
	</ControlsGroup>
	<ControlsGroup Name="gbYear" Text="���" LocationX="251" LocationY="70" Width="232" Height="59" Type="GroupBox" ParamsKind="Individual">
		<Control Name="umeYears" Text="" Type="Combo_Years" Value="-1" LocationX="6" LocationY="20" Width="220" Height="20"/>
	</ControlsGroup>
	<ControlsGroup Name="gbMonth" Text="�����" LocationX="489" LocationY="70" Width="232" Height="59" Type="GroupBox" ParamsKind="Individual">
		<Control Name="ucMonths" Text="" Type="Combo_Months" Value="-1" LocationX="6" LocationY="19" Width="220" Height="21"/>
	</ControlsGroup>
</DataPumpParams>'
where PROGRAMIDENTIFIER = 'Form10Pump';

update PUMPREGISTRY
set PROGRAMCONFIG =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbMoveFilesToArchive" Text="�������� ������������ ����� � ����� ������." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
</DataPumpParams>'
where PROGRAMIDENTIFIER = 'Form13Pump' or PROGRAMIDENTIFIER = 'Form16Pump';

update PUMPREGISTRY
set PROGRAMCONFIG =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbMoveFilesToArchive" Text="�������� ������������ ����� � ����� ������." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbDeleteEarlierData" Text="������� ���������� ����� ������ �� ���� �� ���������." LocationX="13" LocationY="20" Width="500" Height="20" Value="True"/>
	</ControlsGroup>
</DataPumpParams>'
where PROGRAMIDENTIFIER = 'Form1OBLPump' or PROGRAMIDENTIFIER = 'Form5NIOPump' or PROGRAMIDENTIFIER = 'Form14Pump';

commit;

/* End   - 2887 - ����� ���� ������� - ��������������� �������� - mik-a-el - 21.06.2006 */



/* Start   - 2934 - ������� ���� "���������� ������" - gbelov - 22.06.2006 */

delete from RegisteredUIModules where Name = 'AdminConsole';

commit;

/* End   - 2934 - ������� ���� "���������� ������" - gbelov - 22.06.2006 */


/* Start   -  - ��������� ������� �������� "�������������" - gbelov - 22.06.2006 */

create or replace view datasources
(id, suppliercode, datacode, dataname, kindsofparams, name, year, month, variant, quarter, territory, datasourcename)
as
select
	ID, SupplierCode, DataCode, DataName, KindsOfParams,
	Name, Year, Month, Variant, Quarter, Territory,
	SupplierCode ||' '|| DataName ||' - '|| CASE KindsOfParams WHEN 0 THEN Name || ' ' || Year WHEN 1 THEN cast(Year as varchar(4)) WHEN 2 THEN Year || ' ' || Month  WHEN 3 THEN Year || ' ' || Month || ' ' || Variant WHEN 4 THEN Year || ' ' || Variant WHEN 5 THEN Year || ' ' || Quarter WHEN 6 THEN Year || ' ' || territory END
from HUB_DataSources;

/* End   -  - ��������� ������� �������� "�������������" - gbelov - 22.06.2006 */


insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (15, '2.1.10', SYSDATE, SYSDATE, '');

commit;
