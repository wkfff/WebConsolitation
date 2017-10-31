<?xml version="1.0" encoding="windows-1251"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="html" encoding="windows-1251"/>
	<xsl:param name="index"/>
	<xsl:include href="common.xsl"/>

	<xsl:template match="/">
		<xsl:apply-templates select="//Database"/>
	</xsl:template>

	<xsl:template match="//Database">
		<html>
			<head>
				<title>
					<!--  Кривой способ выбора имени (нужно изучать XPath) -->
					<xsl:for-each select="//Databases/Database/Cubes/Cube[position() = $index]">
						Куб <xsl:value-of select="@name"/>
					</xsl:for-each>
				</title>
				<link rel="stylesheet" href="../Resources/main.css" type="text/css"/>
			</head>

			<body>
				<div class="shortcut">
					<a class="sh" href="#dims">измерения</a> | <a class="sh" href="#measures">меры</a> | <a class="sh" href="../index.htm#cl">список кубов</a> | <a class="sh" href="../index.htm#dl">список общих измерений</a>
				</div>

				<xsl:apply-templates select="//Databases/Database/Cubes/Cube[position() = $index]"/>
			</body>
		</html>
	</xsl:template>

	<xsl:template match="Cube">
		<table  border="0" cellpadding="5">
			<tr valign="top">
				<td class ="contents">
					<!-- Рисуем иконку для куба (пока выделяем только виртуальный и простой) -->
					<xsl:choose>
						<xsl:when test="@SubClassType='1'">
							<img border="0" src="../Resources/VirtualCube32.gif" alt="виртуальный куб"/>
						</xsl:when>
						<xsl:otherwise>
							<img border="0" src="../Resources/SimpleCube32.gif" alt="обычный куб"/>
						</xsl:otherwise>
					</xsl:choose>
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
		<a name="dims"/>

		<xsl:call-template name="DescrToBlock2"/>

		<br/>
		<!-- описание -->
		<xsl:call-template name="DescrToBlock"/>

		<a name="dims"/>
		<h2>Измерения</h2>
		<table border="0" cellpadding="3">
			<xsl:for-each select="CubeDimensions/CubeDimension">
				<tr>
					<xsl:variable name="dimname">
						<xsl:value-of select="@name"/>
					</xsl:variable>

					<!--  Кривой способ выбора имени (нужно изучать XPath) -->
					<xsl:for-each select="//Databases/Database/DatabaseDimensions/DatabaseDimension">
						<xsl:if test="@name = $dimname">
							<xsl:variable name="dimnum">
								<xsl:value-of select="position()"/>
							</xsl:variable>

							<!-- рисуем иконку -->
							<td>
								<xsl:choose>
									<xsl:when test="@IsShared='true'">
										<img border="0" src="../Resources/DimSh16.gif" alt="общее измерение"/>
									</xsl:when>
									<xsl:otherwise>
										<img border="0" src="../Resources/Dim16.gif" alt="частное измерение"/>
									</xsl:otherwise>
								</xsl:choose>
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
			<xsl:for-each select="CubeMeasures/CubeMeasure">
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
			<xsl:for-each select="CubeCommands/CubeCommand">
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

	</xsl:template>


</xsl:stylesheet>
