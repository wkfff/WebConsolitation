<?xml version="1.0" encoding="windows-1251"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="text" encoding="windows-1251"/>

<xsl:template match="/">
  <xsl:apply-templates select="//databaseheader"/>
</xsl:template>

<xsl:template match="//databaseheader">

&lt;!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN"&gt;
&lt;HTML>
 &lt;HEAD&gt;
  &lt;META name="GENERATOR" content="fm.HelpGenerator"&gt;
  <!-- Sitemap 1.0 -->
 &lt;/HEAD&gt;
&lt;BODY&gt;
&lt;UL&gt;
<xsl:for-each select="//databaseheader/controlblock/content/cubes/cube">
 &lt;LI&gt;&lt;OBJECT type="text/sitemap"&gt;
  &lt;PARAM name="Name" value="<xsl:value-of select="@name"/> [куб]"&gt;
  &lt;PARAM name="Local" value="pages/Cube<xsl:value-of select="position()"/>.htm"&gt;
  &lt;/OBJECT&gt;
</xsl:for-each>

<xsl:for-each select="//databaseheader/controlblock/content/dimensions/dimension">
 &lt;LI&gt;&lt;OBJECT type="text/sitemap"&gt;
  &lt;PARAM name="Name" value="<xsl:value-of select="@name"/> [измерение]"&gt;
  &lt;PARAM name="Local" value="pages/Dim<xsl:value-of select="position()"/>.htm"&gt;
  &lt;/OBJECT&gt;
</xsl:for-each>

&lt;/BODY&gt;
&lt;/HTML&gt;

</xsl:template>
</xsl:stylesheet>
