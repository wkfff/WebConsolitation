/* Start - #4352 - ����������� ����������. � ������ ��������. - shelpanov - 03.10.2012 */

UPDATE [DV].[fx_Org_SostD]
   SET [Name] = '�������� �� �������'
where ID = 0;

UPDATE [DV].[MetaObjects]
   SET [Configuration] =
    '<?xml version="1.0" encoding="windows-1251"?>
    <DatabaseConfiguration>
     <FixedCls objectKey="243f0a9e-5946-41ff-8b75-8aa75f9e820c" semantic="Org" name="SostD" caption="��������� ������">
       <Attributes>
         <Attribute objectKey="b4e71c4e-534c-40e5-8fb1-d2f8a7684cfb" name="Name" caption="������������" type="ftString" size="255" lookupType="1" position="1" />
       </Attributes>
       <Hierarchy>
         <Regular>
           <Level objectKey="b0835532-6293-44c3-a7e0-8e0e88ef8ea6" all="��� ���������" />
           <Level objectKey="a340f92f-8d38-4759-8dbb-173b6ffb364a" name="������������" memberKey="ID" memberName="Name" />
         </Regular>
       </Hierarchy>
       <Data>
        <Values>
          <Row id="0"><Column name="Name" value="�������� �� �������" /></Row>
          <Row id="1"><Column name="Name" value="������������" /></Row>
          <Row id="2"><Column name="Name" value="������" /></Row>
          <Row id="3"><Column name="Name" value="�� ��������������" /></Row>
          <Row id="4"><Column name="Name" value="�� ������������" /></Row>
          <Row id="5"><Column name="Name" value="�� ���������" /></Row>
          <Row id="6"><Column name="Name" value="�� ����������" /></Row>
          <Row id="7"><Column name="Name" value="���������" /></Row>
          <Row id="8"><Column name="Name" value="�������������" /></Row>
        </Values>
       </Data>
     </FixedCls>
    </DatabaseConfiguration>'
      
 WHERE [ObjectKey] = '243f0a9e-5946-41ff-8b75-8aa75f9e820c'
GO


