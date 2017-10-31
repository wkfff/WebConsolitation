<?xml version="1.0" encoding="windows-1251"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="text" encoding="windows-1251"/>

<xsl:template match="/">
  <xsl:apply-templates select="//Database"/>
</xsl:template>

<xsl:template match="//Database">[OPTIONS]
Compatibility=1.1 or later
Compiled file=Cubes.chm
Contents file=TOC.hhc
Default topic=Resources/Intro.htm
Display compile progress=No
Index file=Index.hhk
Language=0x419 Русский
Title=Справочник по многомерной базе АС "Финансово-Экономический Анализ"

[FILES]
Resources/Intro.htm
index.htm
<xsl:for-each select="//Databases/Database/Cubes/Cube">pages/Cube<xsl:value-of select="position()"/>.htm
</xsl:for-each>
<xsl:for-each select="//Databases/Database/DatabaseDimensions/DatabaseDimension">pages/Dim<xsl:value-of select="position()"/>.htm
</xsl:for-each>
</xsl:template>
</xsl:stylesheet>
