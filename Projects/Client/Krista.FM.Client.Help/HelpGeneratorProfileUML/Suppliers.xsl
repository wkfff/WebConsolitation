<?xml version="1.0" encoding="windows-1251"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="html" encoding="windows-1251"/>
	<xsl:include href="common.xsl"/>

	<xsl:template match="/">
		<xsl:apply-templates select="//Suppliers"/>
	</xsl:template>

	<xsl:template match="//Suppliers">
		<html>
			<head>
				<META http-equiv="Content-Style-Type" content="text/css"/>
				<link rel="stylesheet" href="Resources/main.css" type="text/css"/>
				<h1>Источники данных</h1>
			</head>
			<body>
				<xsl:apply-templates select="./Supplier"/>
			</body>
		</html>
	</xsl:template>

	<xsl:template match="Supplier">
		<h2>
			<xsl:value-of select="@name"/>
		</h2>
		<xsl:call-template name="DescrToBlock"/>
		<table cellspacing ="2" width="90%">
			<tbody>
				<tr>
					<th>
						Код
					</th>
					<th>
						Имя
					</th>
					<th>
						Метод
					</th>
					<th>
						Вид параметров
					</th>
					<th>
						Описание
					</th>
				</tr>
				<xsl:for-each select="./DataKind">
					<tr>
						<td width="5%">
							<xsl:value-of select="@code"/>
						</td>
						<td width="30%">
							<xsl:value-of select="@name"/>
						</td>
						<td width="10%">
							<xsl:value-of select="@takeMethod"/>
						</td>
						<td width="15%">
							<xsl:value-of select="@paramKind"/>
						</td>
						<td width="40%">
							<xsl:value-of select="@description"/>
						</td>
					</tr>
				</xsl:for-each>
			</tbody>
		</table>
	</xsl:template>

</xsl:stylesheet>