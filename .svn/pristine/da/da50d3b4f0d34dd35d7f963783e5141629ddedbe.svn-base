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
					<xsl:for-each select="//Databases/Database/DatabaseDimensions/DatabaseDimension[position() = $index]">
						Измерение <xsl:value-of select="@name"/>
					</xsl:for-each>
				</title>
				<link rel="stylesheet" href="../Resources/main.css" type="text/css"/>
			</head>

			<body>
				<div class="shortcut">
					<a class="sh" href="../index.htm#cl">список кубов</a> | <a class="sh" href="../index.htm#dl">список общих измерений</a>
				</div>
				<xsl:apply-templates select="//Databases/Database/DatabaseDimensions/DatabaseDimension[position() = $index]"/>
			</body>
		</html>
	</xsl:template>

	<xsl:template match="DatabaseDimension">
		<xsl:variable name="dimname">
			<xsl:value-of select="@name"/>
		</xsl:variable>

		<table border="0" cellpadding="5">
			<tr valign="top">
				<td class="contents">
					<!-- Рисуем иконку для измерения (делим их по типу иерархиии как в аналазисе) -->
					<xsl:choose>
						<xsl:when test="DatabaseLevels//DatabaseLevel/child::property/attribute::name='ParentKeyColumn'">
							<img border="0" src="../Resources/DimPC32.gif" alt="parent-child"/>
						</xsl:when>
						<xsl:otherwise>
							<img border="0" src="../Resources/DimSh32.gif" alt="общее измерение"/>
						</xsl:otherwise>
					</xsl:choose>
				</td>
				<td class="contents">
					<h1>
						<xsl:value-of select="$dimname"/>
					</h1>
				</td>
			</tr>
		</table>

		<!-- описание -->
		<xsl:call-template name="DescrToBlock"/>
		<a name="dims"/>
		<xsl:call-template name="DescrToBlock2"/>

		<!-- описание -->
		<xsl:call-template name="DescrToBlock"/>

		<h2>Уровни измерения</h2>
		<xsl:choose>
			<xsl:when test="DatabaseLevels//DatabaseLevel/child::property/attribute::name='ParentKeyColumn'">
				<h4>Тип иерархии: Иерархическая</h4>
			</xsl:when>
			<xsl:otherwise>
				<h4>Тип иерархии: Фиксированная</h4>
			</xsl:otherwise>
		</xsl:choose>
		<table border="0" cellpadding="3">
			<xsl:for-each select="DatabaseLevels/DatabaseLevel">
				<tr>
					<!-- рисуем иконку -->
					<xsl:if test="not(@LevelNamingTemplate)">
					<td>
						<xsl:variable name="levnum">
							<xsl:value-of select="position()"/>
						</xsl:variable>
						<img border="0" src="../Resources/lev{$levnum}.gif" alt=""/>
					</td>
					</xsl:if>

					<!--Раскладываем по уровням-->

					<xsl:if test="@LevelNamingTemplate">
						<xsl:call-template name ="links">
							<xsl:with-param name="str" select="@LevelNamingTemplate"/>
							<xsl:with-param name = "num" select = "2"/>
						</xsl:call-template>
					</xsl:if>
					<xsl:if test="not(@LevelNamingTemplate)">
						<td>
							<xsl:value-of select="@name"/>
						</td>
					</xsl:if>
					<xsl:if test="@name = '(All)'">
						<td>
							<xsl:value-of select="child::property[@name = 'MemberKeyColumn']/node()"/>
						</td>
					</xsl:if>
				</tr>
				<xsl:call-template name="DescrToTable"/>
			</xsl:for-each>
		</table>

		<xsl:if test="@IsShared='true'">
			<br/>
			<h2>Используется в кубах</h2>
			<table border="0" cellpadding="3">

				<!-- ищем все вхождения данного измерения в кубы -->
				<xsl:for-each select="//Databases/Database/Cubes/Cube/CubeDimensions/CubeDimension[@name = $dimname]">
					<xsl:variable name="cubename">
						<xsl:value-of select="ancestor::Cube/@name"/>
					</xsl:variable>

					<!--  Кривой способ выбора имени (нужно изучать XPath) -->
					<xsl:for-each select="//Databases/Database/Cubes/Cube">
						<xsl:if test="@name = $cubename">
							<xsl:variable name="cubenum">
								<xsl:value-of select="position()"/>
							</xsl:variable>
							<tr>
								<!-- рисуем иконку -->
								<td>
									<xsl:choose>
										<xsl:when test="@SubClassType='1'">
											<img border="0" src="../Resources/VirtualCube16.gif" alt="виртуальный куб"/>
										</xsl:when>
										<xsl:otherwise>
											<img border="0" src="../Resources/SimpleCube16.gif" alt="обычный куб"/>
										</xsl:otherwise>
									</xsl:choose>
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
		</xsl:if>


	</xsl:template>

	<xsl:template name ="links">
		<xsl:param name="str"/>
		<xsl:param name="num"/>
		<xsl:choose>
			<xsl:when test="contains($str,';')">
				<tr>
					<td>
						<img src="../Resources/lev{$num}.gif" alt=""/>
					</td>
					<td>
						<xsl:value-of select="substring-before($str,';')"/>
					</td>
				</tr>
				<xsl:call-template name="links">
					<xsl:with-param name="str" select="substring-after($str,';')"/>
					<xsl:with-param name="num" select="number($num) + 1"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<tr>
					<td>
						<img src="../Resources/lev{$num}.gif" alt=""/>
					</td>
					<td>
						<xsl:value-of select="$str"/>
					</td>
				</tr>
			</xsl:otherwise>

		</xsl:choose>
	</xsl:template>


</xsl:stylesheet>
