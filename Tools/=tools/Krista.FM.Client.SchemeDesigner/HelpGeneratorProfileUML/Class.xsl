<?xml version="1.0" encoding="windows-1251"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="html" encoding="windows-1251"/>
	<xsl:include href="common.xsl"/>

	<xsl:template match="/">
		<html>
			<body>
				<div class="shortcut">
					<a class="sh" href="#attr">атрибуты</a> | <a class="sh" href="#hie">уровни иерархии</a>
					| <a class="sh" href="#data">фиксированные строки</a>
					| <a class="sh" href="#associations">ассоциации</a>
					| <a class="sh" href="#associated">используется в</a>
				</div>
				<head>
					<META http-equiv="Content-Style-Type" content="text/css"/>
					<link rel="stylesheet" href="../Resources/main.css" type="text/css"/>
				</head>
				<br/>
				<xsl:apply-templates select="//Class"/>
			</body>
		</html>
	</xsl:template>

	<xsl:template match="//Class" >
		<h1>
			<xsl:value-of select="concat(@semanticCaption, '.', @caption)"/>
		</h1>

		<h2>Информация об объекте</h2>
		<xsl:call-template name="objCommon"/>
		<xsl:call-template name="DescrToBlock"/>
		<xsl:apply-templates select="//Attributes"/>
		<xsl:apply-templates select="//Hierarchy"/>
		<xsl:apply-templates select="//Data"/>
		<xsl:apply-templates select="//AssociationsCls"/>
		<xsl:apply-templates select="//AssociatedCls"/>
	</xsl:template>

	<xsl:template match="//Attributes">
		<a name="attr"/>
		<h2>Атрибуты</h2>
		<table cellspacing="3" cellpadding="2">
			<tbody>
				<tr>
					<th></th>
					<th>Имя в БД</th>
					<th>Русское имя</th>
					<th>Тип</th>
					<th>Размер</th>
					<th>Обязательный</th>
					<th>Описание</th>
				</tr>
				<xsl:for-each select="./Attribute">
					<xsl:if test="not(@kind) or @class = '3' or @name = 'ID'">
						<tr>
							<td>
								<xsl:choose>
									<xsl:when test="@name = 'ID'">
										<img src="../Resources/AttributeKey.gif" alt="Первичный ключ"></img>
									</xsl:when>
									<xsl:when test="@class ='3'">
										<img src="../Resources/AttributeLink.gif" alt="Ссылка"></img>
									</xsl:when>
									<xsl:otherwise>
										<img  src="../Resources/Attribute.gif" alt="Обычный"></img>
									</xsl:otherwise>
								</xsl:choose>
							</td>
							<td>
								<xsl:value-of select="@name"/>
							</td>
							<td>
								<xsl:value-of select="@caption"/>
							</td>
							<td>
								<xsl:call-template name="AttributeType"/>
							</td>
							<td>
								<xsl:if test="@scale">
									<xsl:value-of select="concat(@size, ',', @scale)"/>
								</xsl:if>
								<xsl:if test="not(@scale)">
									<xsl:value-of select="@size"/>
								</xsl:if>
							</td>
							<td>
								<xsl:if test="@nullable">
									<xsl:choose>
										<xsl:when test="@nullable='true'">нет</xsl:when>
										<xsl:when test="@nullable='false'">да</xsl:when>
									</xsl:choose>
								</xsl:if>
								<xsl:if test="not(@nullable)">
									да
								</xsl:if>
							</td>
							<td>
								<xsl:value-of select="@description"/>
							</td>
						</tr>
					</xsl:if>
				</xsl:for-each>

			</tbody>
		</table>
		<br/>
	</xsl:template>

	<xsl:template match="//Hierarchy">
		<h2>Уровни иерархии</h2>
		<a name="hie"/>
		<xsl:apply-templates select="ParentChild"/>
		<xsl:apply-templates select="Regular"/>
		<br/>
	</xsl:template>

	<xsl:template match="ParentChild">
		<h4>
			Тип иерархии: Иерархическая
		</h4>
		<table cellspacing = "3">
			<tbody>
				<tr>
					<td width="5%">
						<img src="../Resources/lev0.gif" alt=""/>
					</td>
					<td>
						(All)
					</td>
					<td align="left">
						<xsl:choose>
							<xsl:when test="@allLevelName">
								<xsl:value-of select="@allLevelName"/>
							</xsl:when>
							<xsl:when test="not(@allLevelName)">нет</xsl:when>
						</xsl:choose>
					</td>
				</tr>
				<xsl:if test="@levelNamingTemplate">
					<xsl:call-template name ="links">
						<xsl:with-param name="str" select="@levelNamingTemplate"/>
						<xsl:with-param name = "num" select = "1"/>
					</xsl:call-template>
				</xsl:if>
			</tbody>
		</table>
	</xsl:template>

	<xsl:template name ="links">
		<xsl:param name="str"/>
		<xsl:param name="num"/>
		<xsl:choose>
			<xsl:when test="contains($str,';')">
				<tr>
					<td>
						<img src="../Resources/lev{$num}.gif" alt=""/>
					</td>
					<td>
						<xsl:value-of select="substring-before($str,';')"/>
					</td>
				</tr>
				<xsl:call-template name="links">
					<xsl:with-param name="str" select="substring-after($str,'; ')"/>
					<xsl:with-param name="num" select="number($num) + 1"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<tr>
					<td>
						<img src="../Resources/lev{$num}.gif" alt=""/>
					</td>
					<td>
						<xsl:value-of select="$str"/>
					</td>
				</tr>
			</xsl:otherwise>

		</xsl:choose>
	</xsl:template>

	<xsl:template match="Regular">
		<h4>
			Тип иерархии: Фиксированная
		</h4>
		<table cellspacing ="3">
			<tbody>
				<xsl:for-each select="./Level">
					<xsl:if test="position()=1">
						<tr>
							<td width="5%">
								<img src="../Resources/lev0.gif" alt=""/>
							</td>
							<td>(All)</td>
							<td>
								<xsl:if test="@all">
									<xsl:value-of select="@all"/>
								</xsl:if>
								<xsl:if test="not(@all)">
									нет
								</xsl:if>
							</td>
						</tr>
					</xsl:if>
					<tr>
						<xsl:if test="@name">
							<td width="5%">
								<xsl:variable name="level">
									<xsl:value-of select="position()-1"/>
								</xsl:variable>
								<img src="../Resources/lev1.gif" alt=""/>
							</td>
							<td>
								<xsl:value-of select="@name"/>
							</td>
						</xsl:if>
					</tr>
				</xsl:for-each>
			</tbody>
		</table>
	</xsl:template>

	<xsl:template match="//Data">
		<a name="data"/>
		<h2>Фиксированные строки</h2>
		<xsl:apply-templates select="Values"/>
	</xsl:template>

	<xsl:template match="Values">
		<table cellspacing="3">
			<tbody>
				<xsl:for-each select="./Row">
					<xsl:if test="position()=1">
						<tr>
							<th>ID</th>
							<xsl:for-each select="./Column">
								<th>
									<xsl:call-template name="attrCaption">
										<xsl:with-param name="name" select="@name"/>
									</xsl:call-template>
								</th>
							</xsl:for-each>
						</tr>
					</xsl:if>
					<tr>
						<td>
							<xsl:value-of select="@id"/>
						</td>
						<xsl:for-each select="./Column">
							<td>
								<xsl:value-of select="@value"/>
							</td>
						</xsl:for-each>
					</tr>
				</xsl:for-each>
			</tbody>
		</table>
	</xsl:template>

	<xsl:template match="//AssociationsCls">
		<a name="associations"/>
		<h2>Ассоциации</h2>
		<table  cellpadding="3" border="0">
			<tbody>
				<tr>
					<th></th>
					<th></th>
					<th>Ссылка</th>
					<th>Имя в БД</th>
					<th>Русское имя</th>
				</tr>
			</tbody>
			<xsl:for-each select="./AssociationCls">
				<xsl:call-template name="DescrToBlock"/>
				<tr>
					<td>
						<xsl:choose>
							<xsl:when test="@associationType='0'">
								<img border="0" src="../Resources/association.gif" alt="Ссылка"/>
							</xsl:when>
							<xsl:when test="@associationType = '2'">
								<img border="0" src="../Resources/ASSOCIATIONBRIDGE.gif" alt="Ассоциация сопоставления"/>
							</xsl:when>
							<xsl:when test="@associationType = '4'">
								<img border="0" src="../Resources/ASSOCIATIONBRIDGE.gif" alt="Ассоциация сопоставления"/>
							</xsl:when>
							<xsl:when test="@associationType = '5'">
								<img border="0" src="../Resources/masterdetail.gif" alt="Ассоциация мастер-деталь"/>
							</xsl:when>
						</xsl:choose>
					</td>
					<xsl:variable name="AssociatName">
						<xsl:value-of select="@fullCaption"/>
					</xsl:variable>
					<xsl:for-each select="./RoleBCls">
						<td>
							<xsl:choose>
								<xsl:when test="@typeClsRoleB='0'">
									<img border="0" src="../Resources/bridgeCls.gif" alt="Сопоставимый классификатор"/>
								</xsl:when>
								<xsl:when test="@typeClsRoleB = '1'">
									<img border="0" src="../Resources/kd.gif" alt="Классификатор данных"/>
								</xsl:when>
								<xsl:when test="@typeClsRoleB = '2'">
									<img border="0" src="../Resources/fixedCls.gif" alt="Фиксированный классификатор"/>
								</xsl:when>
								<xsl:when test="@typeClsRoleB = '3'">
									<img border="0" src="../Resources/factCls.gif" alt="Таблица фактов"/>
								</xsl:when>
								<xsl:when test="@typeClsRoleB = '10'">
									<img border="0" src="../Resources/tableCls.gif" alt="Таблица"/>
								</xsl:when>
							</xsl:choose>
						</td>
						<xsl:variable name="className">
							<xsl:value-of select="@fullCaptionRoleB"/>
						</xsl:variable>
						<xsl:variable name="classPath">
							<xsl:value-of select="@objectKey"/>
						</xsl:variable>
						<td>
							<xsl:value-of select="concat(substring-before($AssociatName, '&gt;'), '&gt; ')"/>
							<a href="../classes/{$classPath}.html">
								<xsl:value-of select="$className"/>
							</a>
						</td>

					</xsl:for-each>
					<td>
						<xsl:value-of select="@name"/>
					</td>
					<td>
						<xsl:value-of select="@caption"/>
					</td>
				</tr>
			</xsl:for-each>
		</table>
		<br/>
	</xsl:template>

	<xsl:template match="//AssociatedCls">
		<a name="associated"/>
		<h2>Используется в</h2>
		<table  cellpadding="3" border="0">
			<tbody>
				<tr>
					<th></th>
					<th></th>
					<th>Ссылка</th>
					<th>Имя в БД</th>
					<th>Русское имя</th>
				</tr>
			</tbody>
			<xsl:for-each select="./AssociateCls">
				<xsl:call-template name="DescrToBlock"/>
				<!--Разбор документа-->
				<tr>
					<td>
						<xsl:choose>
							<xsl:when test="@associationType='0'">
								<img border="0" src="../Resources/association.gif" alt="Ссылка"/>
							</xsl:when>
							<xsl:when test="@associationType = '2'">
								<img border="0" src="../Resources/ASSOCIATIONBRIDGE.gif" alt="Ассоциация сопоставления"/>
							</xsl:when>
							<xsl:when test="@associationType = '4'">
								<img border="0" src="../Resources/ASSOCIATIONBRIDGE.gif" alt="Ассоциация сопоставления"/>
							</xsl:when>
							<xsl:when test="@associationType = '5'">
								<img border="0" src="../Resources/masterdetail.gif" alt="Ассоциация мастер-деталь"/>
							</xsl:when>
						</xsl:choose>
					</td>
					<xsl:variable name="AssocideDCap">
						<xsl:value-of select="@fullCaption"/>
					</xsl:variable>
					<xsl:variable name="AssocideName">
						<xsl:value-of select="@name"/>
					</xsl:variable>
					<xsl:for-each select="./RoleACls">
						<td align="right">
							<xsl:choose>
								<xsl:when test="@typeClsRoleA='0'">
									<img border="0" src="../Resources/bridgeCls.gif" alt="Сопоставимый классификатор"/>
								</xsl:when>
								<xsl:when test="@typeClsRoleA = '1'">
									<img border="0" src="../Resources/kd.gif" alt="Классификатор данных"/>
								</xsl:when>
								<xsl:when test="@typeClsRoleA = '2'">
									<img border="0" src="../Resources/fixedCls.gif" alt="Фиксированный классификатор"/>
								</xsl:when>
								<xsl:when test="@typeClsRoleA = '3'">
									<img border="0" src="../Resources/factCls.gif" alt="Таблица фактов"/>
								</xsl:when>
								<xsl:when test="@typeClsRoleA = '10'">
									<img border="0" src="../Resources/tableCls.gif" alt="Таблица"/>
								</xsl:when>
							</xsl:choose>
						</td>
						<td>
							<a href="../classes/{@objectKey}.html">
								<xsl:value-of select="@fullCaptionRoleA"/>
							</a>
							<xsl:value-of select="concat(' -&gt;', substring-after($AssocideDCap, '-&gt;'))"/>
						</td>
						<td>
							<xsl:value-of select="concat(@fullNameRoleA, '.', $AssocideName)"/>
						</td>
					</xsl:for-each>
					<td>
						<xsl:value-of select="@caption"/>
					</td>
				</tr>

			</xsl:for-each>
		</table>
		<br/>
	</xsl:template>

</xsl:stylesheet>