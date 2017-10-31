<?xml version="1.0" encoding="windows-1251"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="html" encoding="windows-1251"/>

<xsl:template name="DescrToBlock">
    <xsl:if test="@description or @DeveloperDescription">
    <table border="0" cellpadding="10" width="80%"><tr><td class="descr" bgcolor="#CCFFFF">
    <xsl:if test="@description">
        <xsl:value-of select="@description"/>
        </xsl:if>
        
        <xsl:if test="@DeveloperDescription">
        <xsl:value-of select="@DeveloperDescription"/>
        </xsl:if>
    </td></tr></table>
    </xsl:if>

</xsl:template>

</xsl:stylesheet>
