<?xml version="1.0" encoding="windows-1251"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="html" encoding="windows-1251" cdata-section-elements="DeveloperDescription"/>

	<xsl:template name="DescrToBlock">
		<xsl:if test="@description or @DeveloperDescription">
			<table border="0" cellpadding="10" width="80%">
				<tr>
					<td class="contents">
						<xsl:if test="@description">
							<xsl:value-of select="@description"/>
						</xsl:if>
						<xsl:if test="@DeveloperDescription">
							<xsl:value-of select="@DeveloperDescription"/>
						</xsl:if>
					</td>
				</tr>
			</table>
		</xsl:if>
	</xsl:template>
	
	<xsl:template name="AttributeType">
		<xsl:choose>
			<xsl:when test="@type='ftUnknown'">
				Неизвестный
			</xsl:when>
			<xsl:when test="@type='ftInteger'">
				Целый
			</xsl:when>
			<xsl:when test="@type='ftDouble'">
				Вещественный
			</xsl:when>
			<xsl:when test="@type='ftChar'">
				Символьный
			</xsl:when>
			<xsl:when test="@type='ftString'">
				Строковый
			</xsl:when>
			<xsl:when test="@type='ftBoolean'">
				Логический
			</xsl:when>
			<xsl:when test="@type='ftDate'">
				Дата
			</xsl:when>
			<xsl:when test="@type='ftDateTime'">
				Дата и время
			</xsl:when>
			<xsl:when test="@type='ftBLOB'">
				<xsl:if test="@isDocumentAttribute and @isDocumentAttribute='true'">
					Атрибут-документ
				</xsl:if>
				<xsl:if test="not(@isDocumentAttribute)">
					BLOB
				</xsl:if>
			</xsl:when>
		</xsl:choose>
	</xsl:template>

	<!-- Создание иконки по типу класса -->
	<xsl:template name="SetEntityPictureByType">
		<xsl:choose>
			<xsl:when test="@typeCls ='0'">
				<img border="0" src="../Resources/bridgeCls.gif" alt="Сопоставимый классификатор"/>
			</xsl:when>
			<xsl:when test="@typeCls = '1'">
				<img border="0" src="../Resources/kd.gif" alt="Классификатор данных"/>
			</xsl:when>
			<xsl:when test="@typeCls = '2'">
				<img border="0" src="../Resources/fixedCls.gif" alt="Фиксированный классификатор"/>
			</xsl:when>
			<xsl:when test="@typeCls = '3'">
				<img border="0" src="../Resources/factCls.gif" alt="Таблица фактов"/>
			</xsl:when>
			<xsl:when test="@typeCls = '10'">
				<img border="0" src="../Resources/tableCls.gif" alt="Таблица"/>
			</xsl:when>
		</xsl:choose>
	</xsl:template>

	<!-- Русское имя объекта по его префиксу -->
	<xsl:template name = "ObjectNameByPrefix">
		<xsl:choose>
			<xsl:when test="attribute::classType = 'DataClsDoc'">
				Классификатор данных
			</xsl:when>
			<xsl:when test="attribute::classType = 'BridgeClsDoc'">
				Сопоставимый классификатор
			</xsl:when>
			<xsl:when test="attribute::classType = 'FixedClsDoc'">
				Фиксированный классификатор
			</xsl:when>
			<xsl:when test="attribute::classType = 'TableDoc'">
				Таблица
			</xsl:when>
			<xsl:when test="attribute::classType = 'DataTableDoc'">
				Таблица фактов
			</xsl:when>
			<xsl:when test="attribute::classType = 'DocumentEntityDoc'">
				Табличный документ
			</xsl:when>
		</xsl:choose>
	</xsl:template>

	<xsl:template name="objCommon">
		<TABLE class="note" width="80%">
			<tr>
				<td>
					<img src="../Resources/package.gif" alt="пакет"/><font color="#FFFFFF">г</font><a href="../packages/{@packageName}.html">
							<xsl:value-of select="@packageName"/>
						</a><br/>
						Тип объекта:&#32;<xsl:call-template name="ObjectNameByPrefix"/><br/>
						Английское имя:&#32;<xsl:value-of select ="concat(@tablePrefix, '.', @semantic, '.', @name)"/><br/>
						Имя в БД:&#32;<xsl:value-of select="@fullDBName"/><br/>
						Уникальный идентификатор:&#32;<xsl:value-of select="@objectKey"/>
				</td>
			</tr>
		</TABLE>
	</xsl:template>

	<xsl:template name="objCommonLite">
		<TABLE class="note" width="80%">
			<tr>
				<td>
					<img src="../Resources/package.gif" alt="пакет"/><font color="#FFFFFF">г</font><a href="../packages/{@packageName}.html">
						<xsl:value-of select="@packageName"/>
					</a><br/>
					Тип объекта:&#32;<xsl:call-template name="ObjectNameByPrefix"/><br/>
				</td>
			</tr>
		</TABLE>
	</xsl:template>

	<!-- Получаем русское имя атрибута по его имени в БД -->
	<xsl:template name="attrCaption">
		<xsl:param name="name"/>
		<xsl:for-each select="preceding::Attributes/Attribute">
			<xsl:if test="@name=$name">
				<xsl:value-of select="concat(attribute::caption, ' (', $name, ')')"/>
			</xsl:if>
		</xsl:for-each>
	</xsl:template>

	<!-- Описание разработчика -->
	<xsl:template name ="DeveloperDescription">
		<xsl:param name="string"/>
		<xsl:if test="string-length($string) != '0'">
			<h2>Описание разработчика</h2>
				<xsl:value-of select="$string"/>
		</xsl:if>
	</xsl:template>

	<xsl:template name ="DeveloperDescriptionLite">
		<xsl:param name="string"/>
		<xsl:if test="string-length($string) != '0'">
			<xsl:value-of select="$string"/>
		</xsl:if>
	</xsl:template>

</xsl:stylesheet>
