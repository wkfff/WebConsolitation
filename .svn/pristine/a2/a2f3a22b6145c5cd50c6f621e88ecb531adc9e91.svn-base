<?xml version="1.0" encoding="windows-1251"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="html" encoding="windows-1251"/>
<xsl:include href="common.xsl"/>

<xsl:template match="/">
	<html>
		<body>
		    <div class="shortcut">
      <a class="sh" href="#attr">атрибуты</a> | <a class="sh" href="#hie">уровни иерархии</a> | <a class="sh" href="#associations">ассоциации</a>
       | <a class="sh" href="#associated">используется в</a>
    </div>
    <br/>
		<xsl:apply-templates select="//Class"/>
		</body>
	</html>
</xsl:template> 

<xsl:template match="//Class" >
<table>
	<tbody>
		<tr>
			<td><h1><xsl:value-of select="concat(@semanticCaption, '.', @caption)"/></h1></td>
		</tr>
	</tbody>
	  <xsl:call-template name="DescrToBlock"/>
</table>
<xsl:for-each select="./Attributes">
			<xsl:apply-templates select="//Attributes"/>
	</xsl:for-each>
		<xsl:for-each select="./Hierarchy">
			<xsl:apply-templates select="//Hierarchy"/>
	</xsl:for-each>
		<xsl:for-each select="./AssociationsCls">
			<xsl:apply-templates select="//AssociationsCls"/>
	</xsl:for-each>
		<xsl:for-each select="./AssociatedCls">
			<xsl:apply-templates select="//AssociatedCls"/>
	</xsl:for-each>
</xsl:template>

<xsl:template match="//Attributes">
<a name="attr"/>
<h2>Атрибуты</h2>
<table  cellpadding="3" border="1" width="80%" id="top">
	<xsl:for-each select="./Attribute">
	<!--Разбор документа-->
		<tr>
	<td width ="5%"><img  src="../Resources/Attribute.gif" alt="Атрибут"></img></td>
		<td>
			<xsl:value-of select="@name"/>
		</td>
		<td>
			<xsl:value-of select="@caption"/>
		</td>
		<td>
			<xsl:value-of select="@type"/>
		</td>
		<td>
			<xsl:value-of select="@size"/>
		</td>
			<td width="50%">
				<xsl:value-of select="@description"/>
			</td>
	</tr>
	</xsl:for-each>
	</table>

</xsl:template>

<xsl:template match="//Hierarchy">

<h2>Уровни иерархии</h2>
<a name="hie"/>
<table  cellpadding="3" border="0" width="80%">
	<tbody>
	</tbody>
	<xsl:for-each select="./ParentChild">
	<!--Разбор документа-->
	<xsl:call-template name="DescrToBlock"/>
		<tr>
			<td width ="5%">
			<img src="../Resources/hierarchy.gif" alt=""/>
		</td>
		<td><xsl:value-of select="@name"/></td>
	</tr>
	<tr>
		<td><img src="../Resources/all.gif" alt=""/></td>
		<td><b>Уровень all</b></td>
		<td align="left"><xsl:choose>
			<xsl:when test="@allLevelName"><xsl:value-of select="@allLevelName"/></xsl:when>
			<xsl:when test="not(@allLevelName)">НЕТ</xsl:when>
		</xsl:choose></td>
	</tr>
	<tr>
		<td><img src="../Resources/memberKey.gif" alt=""/></td>
		<td><b>memberKey</b></td>
		<td><xsl:value-of select="@memberKey"/></td>
	</tr>
	<tr>
		<td><img src="../Resources/parentKey.gif" alt=""/></td>
		<td><b>parentKey</b></td>
		<td><xsl:value-of select="@parentKey"/></td>
	</tr>
	<tr>
		<td><img src="../Resources/memberName.gif" alt=""/></td>
		<td><b>memberName</b></td>
		<td><xsl:value-of select="@memberName"/></td>
	</tr>
	</xsl:for-each>
	<xsl:for-each select="./Regular">
		<xsl:for-each select="./Level">
	
	<tr>
		<xsl:if test="@all"><td><img src="../Resources/hierarchy.gif" alt=""/></td><td>Уровень all</td><td><xsl:value-of select="@all"/></td></xsl:if>
	</tr>
	<tr><xsl:if test="@name">
	<td>
			<img src="../Resources/hierarchy.gif" alt=""/>
	</td>
		<td><xsl:value-of select="@name"/></td>
	
	</xsl:if>
	</tr>
		<tr><xsl:if test="@memberKey">
	<td>
			<img src="../Resources/memberKey.gif" alt=""/>
	</td>
		<td><xsl:value-of select="@memberKey"/></td>
	
	</xsl:if>
	</tr>
		<tr><xsl:if test="@memberName">
	<td>
			<img src="../Resources/memberName.gif" alt=""/>
	</td>
		<td><xsl:value-of select="@memberName"/></td>
	
	</xsl:if>
	</tr>
				</xsl:for-each>
	</xsl:for-each>
	</table>
</xsl:template>

<xsl:template match="//AssociationsCls">
<a name="associations"/>
<h2>Ассоциации</h2>
<table  cellpadding="3" border="0">
	<tbody>
	</tbody>
	<xsl:for-each select="./AssociationCls">
	<xsl:call-template name="DescrToBlock"/>
	<tr>
		<td width ="5%">
		 <xsl:choose>
              <xsl:when test="@associationType='0'">
                <img border="0" src="../Resources/association.gif" alt="ссылка"/>
              </xsl:when>
              <xsl:when test="@associationType = '2'">
                <img border="0" src="../Resources/ASSOCIATIONBRIDGE.gif" alt="ассоциация сопоставления"/>
              </xsl:when>
                            <xsl:when test="@associationType = '4'">
                <img border="0" src="../Resources/ASSOCIATIONBRIDGE.gif" alt="ассоциация сопоставления"/>
              </xsl:when>
                    <xsl:when test="@associationType = '5'">
                <img border="0" src="../Resources/masterdetail.gif" alt="ассоциация мастер-деталь"/>
              </xsl:when>
            </xsl:choose></td>
		<td><xsl:value-of select="@name"/></td>
		<td><xsl:value-of select="@caption"/></td>
	</tr>
		<xsl:for-each select="./RoleBCls">
		<xsl:call-template name="DescrToBlock"/>
			<tr>
				<td></td>
				<td align="right"><xsl:choose>
              <xsl:when test="@typeClsRoleB='0'">
                <img border="0" src="../Resources/bridgeCls.gif" alt="Сопоставимый"/>
              </xsl:when>
              <xsl:when test="@typeClsRoleB = '1'">
                <img border="0" src="../Resources/kd.gif" alt="КД"/>
              </xsl:when>
                    <xsl:when test="@typeClsRoleB = '2'">
                <img border="0" src="../Resources/fixedCls.gif" alt="Фиксированный"/>
              </xsl:when>
               <xsl:when test="@typeClsRoleB = '3'">
                <img border="0" src="../Resources/factCls.gif" alt="Таблица фактов"/>
              </xsl:when>
               <xsl:when test="@typeClsRoleB = '10'">
                <img border="0" src="../Resources/tableCls.gif" alt="Таблица"/>
              </xsl:when>
            </xsl:choose>
            </td>
            <td><a href="../classes/{@fullNameRoleB}.html"><xsl:value-of select="@fullCaptionRoleB"/></a></td>
			</tr>
		</xsl:for-each>
	</xsl:for-each>
	</table>
</xsl:template>

<xsl:template match="//AssociatedCls">
<a name="associated"/>
<h2>Используется в</h2>
<table  cellpadding="3" border="0">
	<tbody>
	</tbody>
	<xsl:for-each select="./AssociateCls">
	<xsl:call-template name="DescrToBlock"/>
	<!--Разбор документа-->
	<tr>
		<td>
		 <xsl:choose>
              <xsl:when test="@associationType='0'">
                <img border="0" src="../Resources/association.gif" alt="ссылка"/>
              </xsl:when>
              <xsl:when test="@associationType = '2'">
                <img border="0" src="../Resources/ASSOCIATIONBRIDGE.gif" alt="ассоциация сопоставления"/>
              </xsl:when>
                                          <xsl:when test="@associationType = '4'">
                <img border="0" src="../Resources/ASSOCIATIONBRIDGE.gif" alt="ассоциация сопоставления"/>
              </xsl:when>
                    <xsl:when test="@associationType = '5'">
                <img border="0" src="../Resources/masterdetail.gif" alt="ассоциация мастер-деталь"/>
              </xsl:when>
            </xsl:choose></td>
		<td><xsl:value-of select="@name"/></td>
		<td><xsl:value-of select="@caption"/></td>
	</tr>
		<xsl:for-each select="./RoleACls">
		<xsl:call-template name="DescrToBlock"/>
			<tr>
				<td></td>
				<td align="right"><xsl:choose>
              <xsl:when test="@typeClsRoleA='0'">
                <img border="0" src="../Resources/bridgeCls.gif" alt="Сопоставимый"/>
              </xsl:when>
              <xsl:when test="@typeClsRoleA = '1'">
                <img border="0" src="../Resources/kd.gif" alt="КД"/>
              </xsl:when>
                    <xsl:when test="@typeClsRoleA = '2'">
                <img border="0" src="../Resources/fixedCls.gif" alt="Фиксированный"/>
              </xsl:when>
               <xsl:when test="@typeClsRoleA = '3'">
                <img border="0" src="../Resources/factCls.gif" alt="Таблица фактов"/>
              </xsl:when>
               <xsl:when test="@typeClsRoleA = '10'">
                <img border="0" src="../Resources/tableCls.gif" alt="Таблица"/>
              </xsl:when>
            </xsl:choose>
            </td>
            <td><a href="../classes/{@fullNameRoleA}.html"><xsl:value-of select="@fullCaptionRoleA"/></a></td>
			</tr>
		</xsl:for-each>
	</xsl:for-each>
	</table>
</xsl:template>

</xsl:stylesheet>