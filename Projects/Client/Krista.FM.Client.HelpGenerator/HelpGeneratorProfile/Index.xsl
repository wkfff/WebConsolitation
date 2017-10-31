<?xml version="1.0" encoding="windows-1251"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="html" encoding="windows-1251"/>

<xsl:template match="/">
  <xsl:apply-templates select="//Database"/>
</xsl:template>

<xsl:template match="//Database">
  <html>
   <head>
      <title>
         Список кубов
      </title>
	   <link rel="stylesheet" href="../Resources/main.css" type="text/css"/>
   </head>

  <body>
    <a name="cl"/>
    <h1>Список кубов</h1>
    <table border="0" cellpadding="3">
      <xsl:for-each select="//Databases/Database/Cubes/Cube">
        <xsl:variable name="cubenum">
          <xsl:value-of select="position()"/>
        </xsl:variable>
        <tr>
          <!-- Рисуем иконку для куба (пока выделяем только виртуальный и простой) -->
          <td valign="bottom">

            <xsl:choose>
              <xsl:when test="@SubClassType='1'">
                <img border="0" src="Resources/VirtualCube16.gif" alt="виртуальный куб"/>
              </xsl:when>
              <xsl:otherwise>
                <img border="0" src="Resources/SimpleCube16.gif" alt="обычный куб"/>
              </xsl:otherwise>
            </xsl:choose>

          </td>
          <td>
            <a href="Pages/cube{$cubenum}.htm"><xsl:value-of select="@name"/></a>
          </td>
        </tr>
      </xsl:for-each>
    </table>
    <br/><br/>

    <a name="dl"/>
    <h1>Список общих измерений</h1>
    <table border="0" cellpadding="3">
      <xsl:for-each select="//Databases/Database/DatabaseDimensions/DatabaseDimension">
        <xsl:if test="@IsShared='true'">
          <xsl:variable name="dimnum">
            <xsl:value-of select="position()"/>
          </xsl:variable>
          <tr>
            <td><img border="0" src="Resources/DimSh16.gif" alt=""/></td>
            <td><a href="Pages/Dim{$dimnum}.htm"><xsl:value-of select="@name"/></a></td>
          </tr>
        </xsl:if>
      </xsl:for-each>
    </table>

  </body>
  </html>
</xsl:template>

</xsl:stylesheet>
