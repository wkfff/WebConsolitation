<?xml version="1.0" encoding="windows-1251"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="text" encoding="windows-1251"/>

	<xsl:template match="/">
		<xsl:apply-templates select="/ServerConfiguration/Package"/>
	</xsl:template>

	<xsl:template match="/ServerConfiguration/Package">
		[OPTIONS]
		Compatibility=1.1 or later
		Compiled file= Scheme.chm
		Contents file=TOC.hhc
		Default topic=Intro.html
		Display compile progress=No
		Index file=Index.hhk
		Language=0x419 Русский
		Error log File=log.txt
		Title=Справочник по семантической структуре АС "Финансово-Экономический Анализ"

		[FILES]
		Intro.html
		index.html
		suppliers.html
		<xsl:apply-templates select="Packages"/>

	</xsl:template>

	<xsl:template match="Packages">
		<xsl:for-each select="./Package">
			packages/<xsl:value-of select="@name"/>.html
			<xsl:apply-templates select="Packages"/>
			<xsl:apply-templates select="Classes"/>
			<xsl:apply-templates select="Documents"/>
		</xsl:for-each>
	</xsl:template>


	<xsl:template match="Classes">
		<xsl:for-each select="./FixedClsDoc">
			classes/<xsl:value-of select="@objectKey"/>.html
		</xsl:for-each>
		<xsl:for-each select="./DataClsDoc">
			classes/<xsl:value-of select="@objectKey"/>.html
		</xsl:for-each>
		<xsl:for-each select="./DataTableDoc">
			classes/<xsl:value-of select="@objectKey"/>.html
		</xsl:for-each>
		<xsl:for-each select="./BridgeClsDoc">
			classes/<xsl:value-of select="@objectKey"/>.html
		</xsl:for-each>
		<xsl:for-each select="./TableDoc">
			classes/<xsl:value-of select="@objectKey"/>.html
		</xsl:for-each>
		<xsl:for-each select="./DocumentEntityDoc">
			classes/<xsl:value-of select="@objectKey"/>.html
		</xsl:for-each>
	</xsl:template>


	<xsl:template match="Documents">
		<xsl:for-each select="./Document">
			<xsl:if test="@name">
				diagrams/<xsl:value-of select="@name"/>.html
			</xsl:if>
		</xsl:for-each>

	</xsl:template>


</xsl:stylesheet>
