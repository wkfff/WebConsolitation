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
					<xsl:for-each select="//databaseheader/controlblock/content/cubes/cube[position() = $index]">
						<tr>
							<xsl:variable name="cubename">
								Куб <xsl:value-of select="@name"/>
							</xsl:variable>
						</tr>

					</xsl:for-each>
				</title>
				<link rel="stylesheet" href="../Resources/main.css" type="text/css"/>
			</head>

			<body>
				<div class="shortcut">
					<a class="sh" href="../index.htm#cl">список кубов</a> | <a class="sh" href="../index.htm#dl">список общих измерений</a>
				</div>

				<xsl:apply-templates select="//databaseheader/controlblock/content/cubes/cube[position() = $index]"/>
			</body>
		</html>
	</xsl:template>

	<xsl:template match="cube">
		<xsl:variable name="cubename">
			<xsl:value-of select="@name"/>
		</xsl:variable>

		<table border="0" cellpadding="5">
			<tr valign="top">
				<td class ="contents">
					<!-- Рисуем иконку для куба (пока выделяем только виртуальный и простой) -->
					<img border="0" src="../Resources/SimpleCube32.gif" alt="обычный куб"/>
				</td>
				<td class="contents">
					<h1>
						<xsl:value-of select="@name"/>
					</h1>
				</td>
			</tr>
		</table>

		<!-- описание -->
		<xsl:call-template name="DescrToBlock"/>
		<a name="cubes"/>
		<xsl:call-template name="DescrToBlock2"/>

		<!-- Используемые измерения -->
		<xsl:call-template name="DescrToBlock"/>

		<a name="dims"/>
		<h2>Измерения</h2>
		<table border="0" cellpadding="3">
			<xsl:for-each select="cubedimensions/cubedimension">
				<tr>
					<xsl:variable name="dimname">
						<xsl:value-of select="@name"/>
					</xsl:variable>

					<!--  Кривой способ выбора имени (нужно изучать XPath) -->
					<xsl:for-each select="//databaseheader/controlblock/content/dimensions/dimension">
						<xsl:if test="@name = $dimname">
							<xsl:variable name="dimnum">
								<xsl:value-of select="position()"/>
							</xsl:variable>

							<!-- рисуем иконку -->
							<td>
								<img border="0" src="../Resources/DimSh16.gif" alt="общее измерение"/>
							</td>

							<!-- рисуем ссылку на страницу -->
							<td>
								<a href="Dim{$dimnum}.htm">
									<xsl:value-of select="$dimname"/>
								</a>
							</td>
						</xsl:if>
					</xsl:for-each>
					<xsl:call-template name="DescrToTable"/>
				</tr>
			</xsl:for-each>
		</table>

		<br/>

		<a name="measures"/>
		<h2>Меры</h2>
		<table border="0" cellpadding="3">
			<!--меры-->
			<xsl:for-each select="measuregroups/measuregroup/measures/measure">
				<tr>
					<td>
						<img src="../Resources/Measure16.gif" alt="Итог"/>
					</td>
					<td>
						<xsl:value-of select="@name"/>
					</td>
				</tr>
				<xsl:call-template name="DescrToTable"/>
			</xsl:for-each>

			<!--вычислимые меры-->
			<xsl:for-each select="mdxscripts/mdxscript/commands/command[@CALCUCATE]">
				<tr>
					<td>
						<img src="../Resources/Calc16.gif" alt="Вычислимый итог"/>
					</td>
					<td>
						<xsl:value-of select="@name"/>
					</td>
				</tr>
				<xsl:call-template name="DescrToTable"/>
			</xsl:for-each>
		</table>


		<br/>
		<h2>Используются кубы</h2>
		<table border="0" cellpadding="3">
			<!-- ищем все вхождения данного измерения в кубы -->
			<xsl:for-each select="measuregroups/measuregroup[@name != $cubename]">
				<xsl:variable name="measname">
					<xsl:value-of select="@name"/>
				</xsl:variable>

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
									<xsl:value-of select="$measname"/>
								</a>
							</td>
						</tr>
					</xsl:if>
				</xsl:for-each>
			</xsl:for-each>
		</table>

	</xsl:template>


</xsl:stylesheet>
