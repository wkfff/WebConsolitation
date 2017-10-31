<?xml version="1.0" encoding="windows-1251"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="html" encoding="windows-1251"/>
	<xsl:include href="common.xsl"/>

	<xsl:template match="/">
		<xsl:apply-templates select="//Document"/>
	</xsl:template>

	<xsl:template match="//Document">
		<html>
			<head>
				<h2>
					<xsl:value-of select="@name"/>
					<head>
						<META http-equiv="Content-Style-Type" content="text/css"/>
						<link rel="stylesheet" href="../Resources/main.css" type="text/css"/>
					</head>
				</h2>
			</head>
			<body>
				<xsl:call-template name="DescrToBlock"/>
				<xsl:call-template name="DeveloperDescription">
					<xsl:with-param name ="string"  select="DeveloperDescription"/>
				</xsl:call-template>
				<xsl:variable name="имя_док">
					<xsl:value-of select="@name"></xsl:value-of>
				</xsl:variable>
				<p>
					<img border="0" src="{$имя_док}.emf" alt="{$имя_док}"/>
				</p>

				<xsl:apply-templates select="//Diagram"/>
			</body>
		</html>
	</xsl:template>

	<xsl:template match="Diagram">
		<table>
			<xsl:for-each select="./EntitySymbol">
				<!--Отсортировано по типу-->
				<xsl:sort select="@typeIndex" order="ascending"/>
				<tr>
					<td>
						<xsl:call-template name="SetEntityPictureByType"/>
					</td>
					<td>
						<a href="../classes/{@objectKey}.html">
							<xsl:value-of select="@caption"/>
						</a>
					</td>
				</tr>
			</xsl:for-each>
		</table>
	</xsl:template>

</xsl:stylesheet>
