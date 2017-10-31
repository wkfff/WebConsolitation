<?xml version="1.0" encoding="windows-1251"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="html" encoding="windows-1251"/>
	<xsl:include href="common.xsl"/>

	<xsl:template match="/">
		<xsl:apply-templates select="//Semantics"/>
	</xsl:template>

	<xsl:template match="//Semantics">
		<html>
			<head>
				<META http-equiv="Content-Style-Type" content="text/css"/>
				<link rel="stylesheet" href="Resources/main.css" type="text/css"/>
				<h1>Список семантик</h1>
			</head>
			<body>
				<table>
					<tr>
						<th>Английское имя</th>
						<th>Русское имя</th>
					</tr>
					<tbody>
						<xsl:for-each select="./Semantic">
							<xsl:sort select="@name"/>
							<tr>
								<td>
									<xsl:value-of select="@name"/>
								</td>
								<td>
									<xsl:value-of select="@caption"/>
								</td>
							</tr>
						</xsl:for-each>
					</tbody>
				</table>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>