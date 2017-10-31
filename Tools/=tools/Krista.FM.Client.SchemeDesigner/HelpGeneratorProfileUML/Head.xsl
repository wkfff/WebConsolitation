<?xml version="1.0" encoding="windows-1251"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

	<xsl:output method="html" encoding="windows-1251"/>
	<xsl:include href="common.xsl"/>
	
	<xsl:template match="/">
		<html>
			<head>
				<META http-equiv="Content-Style-Type" content="text/css"/>
				<link rel="stylesheet" href="Resources/main.css" type="text/css"/>
			</head>
			<body>
				<xsl:apply-templates select="ServerConfiguration"/>
			</body>
		</html>
	</xsl:template>

	<xsl:template match="ServerConfiguration">
		<table width="100%" height="95%">
			<tr>
				<td align="center" class="contents" valign="top">
					<img border="0" src="Resources/logo_Krista2.jpg"/>
				</td>
			</tr>
			<tr>
				<td class="contents" valign="top" align="center">
					<font size="6" color="#008080" face="Arial">
						<xsl:value-of select="Caption"/>
					</font>
				</td>
			</tr>
			<tr>
				<td align="left" class="contents" valign="bottom" >
					<table>
						<tr>
							<td class="contents">
								<font size="3" color="#000000" face="Verdana">
									<xsl:value-of select="CompanyName"/>
								</font>
							</td>
						</tr>
						<tr>
							<td class="contents">
								<xsl:value-of select="MainVersion"/>
							</td>
						</tr>
						<tr>
							<td class="contents">
								<xsl:value-of select="DBVersion"/>
							</td>
						</tr>
						<tr>
							<td class="contents">
								<xsl:value-of select="ProductName"/>

								<span>
									&#160;&#160;&#160;&#160;
									<a href="http://www.krista.ru">
										http://www.krista.ru
									</a>
								</span>
							</td>
						</tr>
						<tr>
							<td class="contents">
								<a href ="mailto:fmsupport@krista.ru">
									<xsl:value-of select="Coordinate"/>
								</a>
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</xsl:template>
</xsl:stylesheet>