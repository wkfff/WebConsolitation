<?xml version="1.0" encoding="windows-1251"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="html" encoding="windows-1251"/>
<xsl:include href="common.xsl"/>

<xsl:template match="//ServerConfiguration">
  <html>
   <head>
      <title>
         Схема
      </title>
      <link rel="stylesheet" type="text/css" href="Resources/style.css"/>
   </head>

  <body>
    <a name="cl"/>
        <h1>Список пакетов</h1>
        <xsl:call-template name="DescrToBlock"/>
    <table border="0" cellpadding="5" bgcolor="#CCFFFF" width="80%">
      <xsl:for-each select="//ServerConfiguration/Package/Packages/Package">
        <xsl:variable name="имя_пакета">
                      <xsl:value-of select="@name"/>
        </xsl:variable>
        <tr>
          <td valign="bottom">
			<img border="0" src="../Resources/package.gif" alt="пакет"/>
         </td>
          <td>
            <a href="packages/{$имя_пакета}.html"><xsl:value-of select="@name"/></a>
          </td>
          
          <td>  
       			  <xsl:value-of select="@privatePath"/>
          </td> 
          
        </tr>
      </xsl:for-each>
    </table>
    <br/><br/>
      
  </body>
  </html>
</xsl:template>

</xsl:stylesheet>
