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
					<!--  ������ ������ ������ ����� (����� ������� XPath) -->
					<xsl:for-each select="//databaseheader/controlblock/content/dimensions/dimension[position() = $index]">
						��������� <xsl:value-of select="@name"/>
					</xsl:for-each>
				</title>
				<link rel="stylesheet" href="../Resources/main.css" type="text/css"/>
			</head>

			<body>
				<div class="shortcut">
					<a class="sh" href="../index.htm#cl">������ �����</a> | <a class="sh" href="../index.htm#dl">������ ����� ���������</a>
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
					<!-- ������ ������ ��� ��������� (���� �������� ������ ����������� � �������) -->
					<xsl:choose>
						<xsl:when test="@storagemode='Molap'">
							<img border="0" src="../Resources/DimSh32.gif" alt="����� ���������"/>
						</xsl:when>
						<xsl:otherwise>
							<img border="0" src="../Resources/Dim32.gif" alt="������� ���������"/>
						</xsl:otherwise>
					</xsl:choose>
				</td>
				<td class="h1">
					<xsl:value-of select="$dimname"/>
				</td>
			</tr>
		</table>

		<!-- �������� -->
		<xsl:call-template name="DescrToBlock"/>

		<xsl:call-template name="DescrToBlock2"/>

		<h2>������ ���������</h2>
		<table border="0" cellpadding="3">
			<xsl:for-each select="hierarchies/hierarchy/levels/level">
				<tr>
					<!-- ������ ������ -->
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
		<h2>������������ � �����</h2>
		<table border="0" cellpadding="3">

			<!-- ���� ��� ��������� ������� ��������� � ���� -->
			<xsl:for-each select="//databaseheader/controlblock/content/cubes/cube/cubedimensions/cubedimension[@name = $dimname]">
				<xsl:variable name="cubename">
					<xsl:value-of select="ancestor::cube/@name"/>
				</xsl:variable>

				<!--  ������ ������ ������ ����� (����� ������� XPath) -->
				<xsl:for-each select="//databaseheader/controlblock/content/cubes/cube">
					<xsl:if test="@name = $cubename">
						<xsl:variable name="cubenum">
							<xsl:value-of select="position()"/>
						</xsl:variable>
						<tr>
							<!-- ������ ������ -->
							<td>
								<img border="0" src="../Resources/SimpleCube32.gif" alt="������� ���"/>
							</td>
							<!-- ������ ������ �� �������� -->
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
