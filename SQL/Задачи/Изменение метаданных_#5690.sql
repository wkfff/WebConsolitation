/* Start - #5690 - ��������� ���������� - shelpanov - 10.06.2013 */

print N'���������� ����������'
GO

UPDATE [DV].MetaObjects SET Configuration = '<?xml version="1.0" encoding="windows-1251"?>
	<DatabaseConfiguration>
	<FixedCls objectKey="248409d3-3c19-4979-8799-8e52eb7a0ec5" semantic="Fin" name="TypeIzmen" caption="��� ���������">
	<Attributes>
		<Attribute objectKey="8cf44526-b546-4d5c-b1af-e17cd4f56638" name="Name" caption="������������" type="ftString" size="255" lookupType="1" position="1" />
	</Attributes>
	<Hierarchy><Regular>
				<Level objectKey="45af11dd-b763-40c5-9657-e08e03267974" all="���" />
				<Level objectKey="4d9c4c90-a122-444d-a42d-ea721ee8afe4" name="�������������" memberKey="ID" memberName="Name" />
				</Regular>
	</Hierarchy>
	<Data>
		<Values>
			<Row id="0"><Column name="Name" value="''�������� �� �������''" /></Row>
			<Row id="1"><Column name="Name" value="����������" /></Row>
			<Row id="2"><Column name="Name" value="����������" /></Row>
			<Row id="3"><Column name="Name" value="��� ���������" /></Row>
		</Values>
	</Data>
	</FixedCls></DatabaseConfiguration>'
WHERE objectkey = '248409d3-3c19-4979-8799-8e52eb7a0ec5';
GO

/* End - #5690 - ��������� ���������� - shelpanov - 10.06.2013 */