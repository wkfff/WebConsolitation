<?xml version="1.0" encoding="windows-1251"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="html" encoding="windows-1251"/>
<xsl:include href="common.xsl"/>

<xsl:template match="/">
<html>
	<body>
		<xsl:apply-templates select="ServerConfiguration/Package"/>
	</body>
</html>
</xsl:template>

<xsl:template match="ServerConfiguration/Package">
<table border="0" cellpadding="2">
	<tbody>
	</tbody>
	<tr>
		<td>
	<h2><xsl:value-of select="@name"/></h2>
	</td>
	</tr>
	<tr>
	<td>
		<xsl:value-of select="@privatePath"/>
	</td>
	</tr>
	
<xsl:call-template name="DescrToBlock"/>
	</table>
	<xsl:for-each select="./Packages">
			<xsl:apply-templates select="//Packages"/>
	</xsl:for-each>
		<xsl:for-each select="./Classes">
			<xsl:apply-templates select="//Classes"/>
	</xsl:for-each>
		<xsl:for-each select="./Associations">
			<xsl:apply-templates select="//Associations"/>
	</xsl:for-each>
		<xsl:for-each select="./Documents">
			<xsl:apply-templates select="//Documents"/>
	</xsl:for-each>
	      <link rel="stylesheet" type="text/css" href="../Resources/style.css"/>
</xsl:template>


<!-- Разбор подпакетов -->
<xsl:template match="//Packages">
	<h2>Список пакетов</h2>
	<table border="0" cellpadding="5" width="80%">
		<tbody>
			<xsl:for-each select="./Package" >
				<tr>
					<td width ="5%"><img src="../Resources/package.gif" alt="пакет"/></td>
		<!--<xsl:apply-templates select="//Package"/>-->
		<td>
		<xsl:variable name="имя_пакета">
		<xsl:value-of select="@name"></xsl:value-of>
		</xsl:variable>
		<a href="{$имя_пакета}.html"><xsl:value-of select="@name"/></a><br/>
		</td></tr>
	</xsl:for-each>
	</tbody>
	</table>
</xsl:template>


<!-- Разбор классов -->
<xsl:template match="//Classes">
<h2>Список классификаторов</h2>
	<table  border="0" cellpadding="3" frame="above" width="80%">
		<tbody>
		<!--
			<tr>
				<th><img src="../Resources/classes.gif"/> </th>
				<th>Имя</th>
				<th>Наименование</th>
				<th>Имя в БД</th>
			</tr>
-->
		</tbody>
	<!--Фиксированные классификаторы-->
	<xsl:for-each select="./FixedClsDoc">
	<tr>
		<td width ="5%"><img src="../Resources/fixedCls.gif" alt="Фиксированный классификатор"/></td>
		<td><xsl:value-of select="@name"/></td>
		<td><a href="../classes/{@fullDBName}.html"><xsl:value-of select="concat(@semanticCaption, '.', @caption)"/></a></td>
		<td><xsl:value-of select="@fullDBName"/></td>

	</tr>
	</xsl:for-each>
	<!--Сопоставимые классификаторы-->
	<xsl:for-each select="./BridgeClsDoc">
	<tr>
		<td width ="5%">
			<img src="../Resources/bridgeCls.gif" alt="Сопоставимый классификатор"/>
		</td>
		<td>
			<xsl:value-of select="@name"/>
		</td>
		<td>
			<a href="../classes/{@fullDBName}.html">
				<xsl:value-of select="concat(@semanticCaption, '.', @caption)"/>
			</a>
		</td>
		<td>
			<xsl:value-of select="@fullDBName"/>
		</td>
	</tr>
</xsl:for-each>
<!--Классификаторы данных-->
<xsl:for-each select="./DataClsDoc">
	<tr>
		<td width ="5%"><img src="../Resources/kd.gif" alt="Классификатор данных"/></td>
		<td><xsl:value-of select="@name"/></td>
		<td><a href="../classes/{@fullDBName}.html"><xsl:value-of select="concat(@semanticCaption, '.', @caption)"/></a></td>
		<td><xsl:value-of select="@fullDBName"/></td>
	</tr>
	</xsl:for-each>
	<!--Таблицы фактов-->
	<xsl:for-each select="./DataTableDoc">
	<tr>
		<td width ="5%">
			<img src="../Resources/factCls.gif" alt="Таблица фактов"/>
		</td>
		<td>
			<xsl:value-of select="@name"/>
		</td>
		<td>
			<a href="../classes/{@fullDBName}.html">
				<xsl:value-of select="concat(@semanticCaption, '.', @caption)"/>
			</a>
		</td>
		<td>
			<xsl:value-of select="@fullDBName"/>
		</td>
	</tr>
</xsl:for-each>
<!--Таблицы-->
<xsl:for-each select="./TableDoc">
	<tr>
		<td width ="5%"><img src="../Resources/tableCls.gif" alt="Таблица"/></td>
		<td><xsl:value-of select="@name"/></td>
		<td><a href="../classes/{@fullDBName}.html"><xsl:value-of select="concat(@semanticCaption, '.', @caption)"/></a></td>
		<td><xsl:value-of select="@fullDBName"/></td>
	</tr>
	</xsl:for-each>
		</table>
</xsl:template>

<!--Пока не выводим
<xsl:template match="//Associations">
<h2>Список ассоциаций</h2>
<table bgcolor="green" border="1" cellpadding="1">
	<tbody>
		<tr>
			<th>Имя ассоциации</th>
		</tr>
	</tbody>
	<xsl:for-each select="./Reference">
	<tr>
		<td><xsl:value-of select="@name"/></td>
	</tr>
	</xsl:for-each>
	<xsl:for-each select="./Data2Bridge">
	<tr>
		<td><xsl:value-of select="@name"/></td>
	</tr>
	</xsl:for-each>
	<xsl:for-each select="./MasterDetail">
	<tr>
		<td><xsl:value-of select="@name"/></td>
	</tr>
	</xsl:for-each>
	</table>
</xsl:template>
-->

<!-- Разбор документов -->
<xsl:template match="//Documents">
<h2>Список документов</h2>
<table cellpadding="1" border="0">
	<tbody>
	</tbody>

	<xsl:for-each select="./Document">
	<!--Разбор документа-->
	<tr>
			<td>	
				<img src="../Resources/documents.gif" alt="Диаграмма"/>
			</td>
		<td>
			<a href="../diagrams/{@name}.html"><xsl:value-of select="@name"/></a>
		</td>
	</tr>
	</xsl:for-each>
	</table>
</xsl:template>

</xsl:stylesheet>
