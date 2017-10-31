<?xml version="1.0" encoding="windows-1251"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="html" encoding="windows-1251"/>
	<xsl:param name="index"/>
	<xsl:include href="common.xsl"/>

	<xsl:template match="/">
		<xsl:apply-templates select="//databaseheader"/>
	</xsl:template>

	<xsl:template match="//databaseheader">
		<html>
			<head>
				<title>
					<!--  Кривой способ выбора имени (нужно изучать XPath) -->
					<xsl:for-each select="//databaseheader/controlblock/content/dimensions/dimension[position() = $index]">
						Измерение <xsl:value-of select="@name"/>
					</xsl:for-each>
				</title>
				<link rel="stylesheet" href="../Resources/main.css" type="text/css"/>
			</head>

			<body>
				<div class="shortcut">
					<a class="sh" href="../index.htm#cl">список кубов</a> | <a class="sh" href="../index.htm#dl">список общих измерений</a>
				</div>
				<xsl:apply-templates select="//databaseheader/controlblock/content/dimensions/dimension[position() = $index]"/>
			</body>
		</html>
	</xsl:template>

	<xsl:template match="dimension">
		<xsl:variable name="dimname">
			<xsl:value-of select="@name"/>
		</xsl:variable>

		<table border="0" cellpadding="5">
			<tr valign="top">
				<td>
					<!-- Рисуем иконку для измерения (пока выделяем только Разделяемый и простой) -->
					<xsl:choose>
						<xsl:when test="@storagemode='Molap'">
							<img border="0" src="../Resources/DimSh32.gif" alt="общее измерение"/>
						</xsl:when>
						<xsl:otherwise>
							<img border="0" src="../Resources/Dim32.gif" alt="частное измерение"/>
						</xsl:otherwise>
					</xsl:choose>
				</td>
				<td class="h1">
					<xsl:value-of select="$dimname"/>
				</td>
			</tr>
		</table>

		<!-- описание -->
		<xsl:call-template name="DescrToBlock"/>

		<xsl:call-template name="DescrToBlock2"/>

		<h2>Уровни измерения</h2>
		<table border="0" cellpadding="3">
			<xsl:for-each select="hierarchies/hierarchy/levels/level">
				<tr>
					<!-- рисуем иконку -->
					<td>
						<xsl:variable name="levnum">
							<xsl:value-of select="position()"/>
						</xsl:variable>
						<img border="0" src="../Resources/lev{$levnum}.gif" alt=""/>
					</td>
					<td>
						<xsl:value-of select="@name"/>
					</td>
				</tr>
				<xsl:call-template name="DescrToTable"/>
			</xsl:for-each>
		</table>


		<br/>
		<h2>Используется в кубах</h2>
		<table border="0" cellpadding="3">

			<!-- ищем все вхождения данного измерения в кубы -->
			<xsl:for-each select="//databaseheader/controlblock/content/cubes/cube/cubedimensions/cubedimension[@name = $dimname]">
				<xsl:variable name="cubename">
					<xsl:value-of select="ancestor::cube/@name"/>
				</xsl:variable>

				<!--  Кривой способ выбора имени (нужно изучать XPath) -->
				<xsl:for-each select="//databaseheader/controlblock/content/cubes/cube">
					<xsl:if test="@name = $cubename">
						<xsl:variable name="cubenum">
							<xsl:value-of select="position()"/>
						</xsl:variable>
						<tr>
							<!-- рисуем иконку -->
							<td>
								<img border="0" src="../Resources/SimpleCube32.gif" alt="обычный куб"/>
							</td>
							<!-- рисуем ссылку на страницу -->
							<td>
								<a href="Cube{$cubenum}.htm">
									<xsl:value-of select="$cubename"/>
								</a>
							</td>
						</tr>
					</xsl:if>
				</xsl:for-each>
			</xsl:for-each>
		</table>



	</xsl:template>


</xsl:stylesheet>
