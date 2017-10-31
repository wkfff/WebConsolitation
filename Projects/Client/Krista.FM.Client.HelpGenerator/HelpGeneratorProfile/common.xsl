<?xml version="1.0" encoding="windows-1251"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="html" encoding="windows-1251"/>

<!-- Выводит примечание и/или источник данных в отдельный блок -->
<xsl:template name="DescrToBlock">
  <xsl:if test="CustomProperties/Property[(@name='help_source') or (@name='help_descr')]">
    <table border="0" cellpadding="10" width="80%"><tr><td class="descr">
      <xsl:if test="CustomProperties/Property[@name='help_source']">
        <h3>Источник данных</h3>
        <xsl:value-of select="CustomProperties/Property[@name='help_source']"/>
      </xsl:if>

      <xsl:if test="CustomProperties/Property[@name='help_descr']">
        <h3>Описание</h3>
        <xsl:value-of select="CustomProperties/Property[@name='help_descr']"/>
      </xsl:if>
    </td></tr></table>
    <br/>
  </xsl:if>
</xsl:template>

<!-- Выводит примечание в строку таблицы.
  Таблица состоит из двух ячеек, первая пустая -->
<xsl:template name="DescrToTable">
  <xsl:if test="CustomProperties/Property[@name='help_descr']">
    <tr>
      <td/>
      <td class="descr"><xsl:value-of select="CustomProperties/Property[@name='help_descr']"/></td>
    </tr>
  </xsl:if>
</xsl:template>

	<xsl:template name="DescrToBlock2">
		<xsl:if test="@Description or @description">
			<table class ="note" border="0" cellpadding="10" width="80%">
				<tr>
					<td class="contents">
						<xsl:if test="@Description">
							<xsl:value-of select="@Description"/>
						</xsl:if>
						<xsl:if test="@description">
							<xsl:value-of select="@description"/>
						</xsl:if>
					</td>
				</tr>
			</table>
		</xsl:if>

	</xsl:template>

</xsl:stylesheet>