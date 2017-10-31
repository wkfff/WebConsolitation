<?xml version="1.0" encoding="windows-1251"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="text" encoding="windows-1251"/>

<xsl:template match="/">
  <xsl:apply-templates select="//ServerConfiguration/Package"/>
</xsl:template>

<xsl:template match="//ServerConfiguration">

&lt;!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN"&gt;
&lt;HTML>
 &lt;HEAD&gt;
  &lt;META name="GENERATOR" content="fm.HelpGenerator"&gt;

 &lt;/HEAD&gt;
&lt;BODY&gt;


 &lt;OBJECT type="text/site properties"&gt;
  &lt;PARAM name="Window Styles" value="0x800027"&gt;
  &lt;PARAM name="ImageType" value="Books"&gt;
 &lt;/OBJECT>
 
 
 
&lt;LI&gt;&lt;OBJECT type="text/sitemap"&gt;
&lt;PARAM name="Name" value="Введение"&gt;
&lt;PARAM name="Local" value="Intro.html"&gt;
&lt;/OBJECT&gt;

<xsl:apply-templates select="Package"/>



&lt;/BODY&gt;
&lt;/HTML&gt;

</xsl:template> 

<xsl:template match="Packages">

 &lt;LI&gt;&lt;OBJECT type="text/sitemap"&gt;
  &lt;PARAM name="Name" value="Пакеты"&gt;
    &lt;PARAM name="Local" value=""&gt;
  &lt;/OBJECT&gt;
  
<xsl:for-each select="./Package">
 &lt;LI&gt;&lt;OBJECT type="text/sitemap"&gt;
  &lt;PARAM name="Name" value="<xsl:value-of select="@name"/> [Пакет]"&gt;
  &lt;PARAM name="Local" value="packages/<xsl:value-of select="@name"/>.html"&gt;
  &lt;/OBJECT&gt;
	<xsl:apply-templates select="Packages"/>
	<xsl:apply-templates select="Classes"/>
	<xsl:apply-templates select="Documents"/>
</xsl:for-each>
</xsl:template>


<xsl:template match="Classes">

 &lt;LI&gt;&lt;OBJECT type="text/sitemap"&gt;
  &lt;PARAM name="Name" value="Классы"&gt;
    &lt;PARAM name="Local" value=""&gt;
  &lt;/OBJECT&gt;
  
	<xsl:for-each select="./FixedClsDoc">
	 &lt;LI&gt;&lt;OBJECT type="text/sitemap"&gt;
	&lt;PARAM name="Name" value="<xsl:value-of select="concat(@semanticCaption, '.', @caption)"/> [Фиксированный класификатор]"&gt;
  &lt;PARAM name="Local" value="classes/<xsl:value-of select="@fullDBName"/>.html"&gt;
    &lt;/OBJECT&gt;
	</xsl:for-each>
	
	<xsl:for-each select="./DataClsDoc">
	 &lt;LI&gt;&lt;OBJECT type="text/sitemap"&gt;
	&lt;PARAM name="Name" value="<xsl:value-of select="concat(@semanticCaption, '.', @caption)"/> [Классификатор данных]"&gt;
  &lt;PARAM name="Local" value="classes/<xsl:value-of select="@fullDBName"/>.html"&gt;
    &lt;/OBJECT&gt;
</xsl:for-each>

<xsl:for-each select="./DataTableDoc">
 &lt;LI&gt;&lt;OBJECT type="text/sitemap"&gt;
&lt;PARAM name="Name" value="<xsl:value-of select="concat(@semanticCaption, '.', @caption)"/> [Таблица фактов]"&gt;
  &lt;PARAM name="Local" value="classes/<xsl:value-of select="@fullDBName"/>.html"&gt;
    &lt;/OBJECT&gt;
</xsl:for-each>

<xsl:for-each select="./BridgeClsDoc">
 &lt;LI&gt;&lt;OBJECT type="text/sitemap"&gt;
&lt;PARAM name="Name" value="<xsl:value-of select="concat(@semanticCaption, '.', @caption)"/> [Сопоставимый класификатор]"&gt;
  &lt;PARAM name="Local" value="classes/<xsl:value-of select="@fullDBName"/>.html"&gt;
    &lt;/OBJECT&gt;
</xsl:for-each>

<xsl:for-each select="./TableDoc">
 &lt;LI&gt;&lt;OBJECT type="text/sitemap"&gt;
&lt;PARAM name="Name" value="<xsl:value-of select="concat(@semanticCaption, '.', @caption)"/> [Таблица]"&gt;
  &lt;PARAM name="Local" value="classes/<xsl:value-of select="@fullDBName"/>.html"&gt;
    &lt;/OBJECT&gt;
</xsl:for-each>
</xsl:template>

<xsl:template match="Documents">


 &lt;LI&gt;&lt;OBJECT type="text/sitemap"&gt;
  &lt;PARAM name="Name" value="Диаграммы"&gt;
    &lt;PARAM name="Local" value=""&gt;
  &lt;/OBJECT&gt;

  <xsl:for-each select="./Document">
 &lt;LI&gt;&lt;OBJECT type="text/sitemap"&gt;
&lt;PARAM name="Name" value="<xsl:value-of select="@name"/> [Диаграмма]"&gt;
  &lt;PARAM name="Local" value="diagrams/<xsl:value-of select="@name"/>.html"&gt;
    &lt;/OBJECT&gt;
</xsl:for-each>

</xsl:template>

</xsl:stylesheet>
