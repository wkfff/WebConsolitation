<?xml version="1.0" encoding="windows-1251"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="text" encoding="windows-1251"/>

<xsl:template match="/">
  <xsl:apply-templates select="//databaseheader"/>
</xsl:template>

<xsl:template match="//databaseheader">
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
&lt;PARAM name="Local" value="Resources/Intro.htm"&gt;
&lt;/OBJECT&gt;

&lt;LI&gt;&lt;OBJECT type="text/sitemap"&gt;
&lt;PARAM name="Name" value="Список кубов"&gt;
&lt;PARAM name="Local" value="Index.htm"&gt;
&lt;/OBJECT&gt;

&lt;LI&gt;
&lt;UL&gt;

<xsl:for-each select="//databaseheader/controlblock/content/cubes/cube">
 &lt;LI&gt;&lt;OBJECT type="text/sitemap"&gt;
  &lt;PARAM name="Name" value="<xsl:value-of select="@name"/>"&gt;
  &lt;PARAM name="Local" value="pages/Cube<xsl:value-of select="position()"/>.htm"&gt;
  &lt;/OBJECT&gt;
</xsl:for-each>

&lt;/UL&gt;

&lt;LI&gt;&lt;OBJECT type="text/sitemap"&gt;
&lt;PARAM name="Name" value="Общие измерения"&gt;
&lt;PARAM name="Local" value="Index.htm#dl"&gt;
&lt;/OBJECT&gt;

&lt;UL&gt;
<xsl:for-each select="/databaseheader/controlblock/content/dimensions/dimension">
  <xsl:if test="@type='Regular'">
   &lt;LI&gt;&lt;OBJECT type="text/sitemap"&gt;
    &lt;PARAM name="Name" value="<xsl:value-of select="@name"/>"&gt;
    &lt;PARAM name="Local" value="pages/Dim<xsl:value-of select="position()"/>.htm"&gt;
    &lt;/OBJECT&gt;
  </xsl:if>
</xsl:for-each>
&lt;/UL&gt;

&lt;/UL&gt;

&lt;/BODY&gt;
&lt;/HTML&gt;

</xsl:template>
</xsl:stylesheet>
