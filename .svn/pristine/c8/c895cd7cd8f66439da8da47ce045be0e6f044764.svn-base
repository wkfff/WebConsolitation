<?xml version="1.0" encoding="windows-1251"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="html" encoding="windows-1251"/>
<xsl:include href="common.xsl"/>

<xsl:template match="/">
		<xsl:apply-templates select="//Document"/>
</xsl:template> 

<xsl:template match="//Document">
<html>
	<head>
		<h2><xsl:value-of select="@name"/></h2>
	</head>
	<body>
	<xsl:variable name="имя_док">
		<xsl:value-of select="@name"></xsl:value-of>
		</xsl:variable>
		<p><img border="0" src="{$имя_док}.emf" alt="{$имя_док}"/></p>
	</body>
</html>
</xsl:template>

</xsl:stylesheet>
