<?xml version="1.0" encoding="windows-1251"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="text" encoding="windows-1251"/>

	<xsl:template match="/">
		<xsl:apply-templates select="//ServerConfiguration"/>
	</xsl:template>

	<xsl:template match="//ServerConfiguration">
		&lt;HTML>
		&lt;HEAD&gt;
		&lt;META name="GENERATOR" content="fm.HelpGenerator"&gt;
		<!-- Sitemap 1.0 -->
		&lt;/HEAD&gt;
		&lt;BODY&gt;


		&lt;OBJECT type="text/site properties"&gt;
		&lt;PARAM name="Window Styles" value="0x800027"&gt;
		&lt;PARAM name="ImageType" value="Books"&gt;
		&lt;/OBJECT>

		&lt;UL&gt;
		&lt;LI&gt;&lt;OBJECT type="text/sitemap"&gt;
		&lt;PARAM name="Name" value="Введение"&gt;
		&lt;PARAM name="Local" value="Index.html"&gt;
		&lt;/OBJECT&gt;&lt;/LI&gt;
		&lt;/UL&gt;

		<xsl:apply-templates select="./Package"/>
		<xsl:apply-templates select="./Suppliers"/>
		<xsl:apply-templates select="./Semantics"/>

	</xsl:template>

	<xsl:template match="Package">
		
		&lt;UL&gt;
		&lt;LI&gt;&lt;OBJECT type="text/sitemap"&gt;
		&lt;PARAM name="Name" value="<xsl:value-of select="@name"/>"&gt;
		&lt;PARAM name="Local" value="packages/<xsl:value-of select="@name"/>.html"&gt;
		&lt;PARAM name="ImageNumber" value="5"&gt;
		&lt;/OBJECT&gt;&lt;/LI&gt;
		<xsl:apply-templates select="./Packages"/>
		&lt;/UL&gt;
		
	</xsl:template>

	<xsl:template match="Suppliers">
		
		&lt;UL&gt;
		&lt;LI&gt;&lt;OBJECT type="text/sitemap"&gt;
		&lt;PARAM name="Name" value="Источники данных"&gt;
		&lt;PARAM name="Local" value="suppliers.html"&gt;
		&lt;/OBJECT&gt;&lt;/LI&gt;
		&lt;/UL&gt;
		
	</xsl:template>

	<xsl:template match="Semantics">

		&lt;UL&gt;
		&lt;LI&gt;&lt;OBJECT type="text/sitemap"&gt;
		&lt;PARAM name="Name" value="Список семантик"&gt;
		&lt;PARAM name="Local" value="semantics.html"&gt;
		&lt;/OBJECT&gt;&lt;/LI&gt;
		&lt;/UL&gt;

	</xsl:template>

	<xsl:template match="Packages">

		&lt;UL&gt;
		&lt;LI&gt;&lt;OBJECT type="text/sitemap"&gt;
		&lt;PARAM name="Name" value="Пакеты"&gt;
		&lt;PARAM name="Local" value=""&gt;
		&lt;/OBJECT&gt;&lt;/LI&gt;

		<xsl:for-each select="./Package">
			&lt;UL&gt;
			&lt;LI&gt;&lt;OBJECT type="text/sitemap"&gt;
			&lt;PARAM name="Name" value="<xsl:value-of select="@name"/>"&gt;
			&lt;PARAM name="Local" value="packages/<xsl:value-of select="@name"/>.html"&gt;
			&lt;PARAM name="ImageNumber" value="5"&gt;
			&lt;/OBJECT&gt;&lt;/LI&gt;
			<xsl:apply-templates select="Packages"/>
			<xsl:apply-templates select="Classes"/>
			<xsl:apply-templates select="Documents"/>
			&lt;/UL&gt;
		</xsl:for-each>

		&lt;/UL&gt;
	</xsl:template>

	<xsl:template match="Classes">
		&lt;UL&gt;
		&lt;LI&gt;&lt;OBJECT type="text/sitemap"&gt;
		&lt;PARAM name="Name" value="Классы"&gt;
		&lt;PARAM name="Local" value=""&gt;
		&lt;/OBJECT&gt;&lt;/LI&gt;

		&lt;UL&gt;
		<xsl:for-each select="./FixedClsDoc">

			&lt;LI&gt;&lt;OBJECT type="text/sitemap"&gt;
			&lt;PARAM name="Name" value="<xsl:value-of select="concat(@semanticCaption, '.', @caption)"/>"&gt;
			&lt;PARAM name="Local" value="classes/<xsl:value-of select="@objectKey"/>.html"&gt;
			&lt;PARAM name="ImageNumber" value="11"&gt;
			&lt;/OBJECT&gt;&lt;/LI&gt;

		</xsl:for-each>

		<xsl:for-each select="./BridgeClsDoc">

			&lt;LI&gt;&lt;OBJECT type="text/sitemap"&gt;
			&lt;PARAM name="Name" value="<xsl:value-of select="concat(@semanticCaption, '.', @caption)"/>"&gt;
			&lt;PARAM name="Local" value="classes/<xsl:value-of select="@objectKey"/>.html"&gt;
			&lt;PARAM name="ImageNumber" value="11"&gt;
			&lt;/OBJECT&gt;&lt;/LI&gt;

		</xsl:for-each>
		<xsl:for-each select="./DataClsDoc">

			&lt;LI&gt;&lt;OBJECT type="text/sitemap"&gt;
			&lt;PARAM name="Name" value="<xsl:value-of select="concat(@semanticCaption, '.', @caption)"/>"&gt;
			&lt;PARAM name="Local" value="classes/<xsl:value-of select="@objectKey"/>.html"&gt;
			&lt;PARAM name="ImageNumber" value="11"&gt;
			&lt;/OBJECT&gt;&lt;/LI&gt;

		</xsl:for-each>

		<xsl:for-each select="./DataTableDoc">

			&lt;LI&gt;&lt;OBJECT type="text/sitemap"&gt;
			&lt;PARAM name="Name" value="<xsl:value-of select="concat(@semanticCaption, '.', @caption)"/>"&gt;
			&lt;PARAM name="Local" value="classes/<xsl:value-of select="@objectKey"/>.html"&gt;
			&lt;PARAM name="ImageNumber" value="11"&gt;
			&lt;/OBJECT&gt;&lt;/LI&gt;

		</xsl:for-each>

		<xsl:for-each select="./TableDoc">

			&lt;LI&gt;&lt;OBJECT type="text/sitemap"&gt;
			&lt;PARAM name="Name" value="<xsl:value-of select="concat(@semanticCaption, '.', @caption)"/>"&gt;
			&lt;PARAM name="Local" value="classes/<xsl:value-of select="@objectKey"/>.html"&gt;
			&lt;PARAM name="ImageNumber" value="11"&gt;
			&lt;/OBJECT&gt;

		</xsl:for-each>

		<xsl:for-each select="./DocumentEntityDoc">

			&lt;LI&gt;&lt;OBJECT type="text/sitemap"&gt;
			&lt;PARAM name="Name" value="<xsl:value-of select="concat(@semanticCaption, '.', @caption)"/>"&gt;
			&lt;PARAM name="Local" value="classes/<xsl:value-of select="@objectKey"/>.html"&gt;
			&lt;PARAM name="ImageNumber" value="11"&gt;
			&lt;/OBJECT&gt;

		</xsl:for-each>
		
		&lt;/UL&gt;
		&lt;/UL&gt;

	</xsl:template>

	<xsl:template match="Documents">

		&lt;UL&gt;
		&lt;LI&gt;&lt;OBJECT type="text/sitemap"&gt;
		&lt;PARAM name="Name" value="Диаграммы"&gt;
		&lt;PARAM name="Local" value=""&gt;
		&lt;/OBJECT&gt;&lt;/LI&gt;
		&lt;UL&gt;
		<xsl:for-each select="./Document">
			&lt;LI&gt;&lt;OBJECT type="text/sitemap"&gt;
			&lt;PARAM name="Name" value="<xsl:value-of select="@name"/>"&gt;
			&lt;PARAM name="Local" value="diagrams/<xsl:value-of select="@name"/>.html"&gt;
			&lt;/OBJECT&gt;&lt;/LI&gt;
		</xsl:for-each>
		&lt;/UL&gt;

		&lt;/UL&gt;
	</xsl:template>

</xsl:stylesheet>
