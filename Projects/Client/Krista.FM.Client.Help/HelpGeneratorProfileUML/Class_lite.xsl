<?xml version="1.0" encoding="windows-1251"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="html" encoding="windows-1251"/>
	<xsl:include href="common.xsl"/>

	<xsl:template match="/">
		<html>
			<body>
				<div class="shortcut">
					<a class="sh" href="#attr">атрибуты</a> 
					<!--| <a class="sh" href="#hie">уровни иерархии</a>-->
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
		<xsl:call-template name="objCommonLite"/>
		<xsl:call-template name="DescrToBlock"/>
		<xsl:call-template name="DeveloperDescription">
			<xsl:with-param name ="string"  select="DeveloperDescription"/>
		</xsl:call-template>
		<xsl:apply-templates select="//Attributes"/>
		<!--<xsl:apply-templates select="//Hierarchy"/>-->
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
					<th>№</th>
					<!--<th>Имя в БД</th>-->
					<th></th>
					<th>Русское имя</th>
					<th>Тип</th>
					<th>Размер</th>
					<th>Обязательный</th>
					<th>Маска</th>
					<th>Описание</th>
					<th>Описание разработчика</th>
				</tr>
				<xsl:for-each select="./Attribute | ./RefAttribute">
					<xsl:sort select="concat(
						substring(@positionCalc, 1, string-length(@positionCalc) - string-length(substring-after(@positionCalc, '.') )), 
						translate(substring-after(@positionCalc,  '.'), '.', ''))" 
						data-type="number"/>
					<xsl:sort select="@position" data-type="number"/>
					<!--Разбор документа-->
					<xsl:if test="@caption != 'ID'">
						<tr>
							<td>
								<xsl:if test="@positionCalc != '0' and @positionCalc != '100'">
									<xsl:value-of select="@positionCalc"/>
								</xsl:if>
							</td>
							<td>
								<xsl:choose>
									<xsl:when test="@kind='0' and @class !='3'">
										<img src="../Resources/AttributeLock.gif" alt="Системный"></img>
									</xsl:when>
									<xsl:when test="@kind = '1' and @name !='ID'">
										<img src="../Resources/AttributeServ.gif" alt="Служебный"></img>
									</xsl:when>
									<xsl:when test="@kind='1' and @name = 'ID'">
										<img src="../Resources/AttributeKey.gif" alt="Первичный ключ"></img>
									</xsl:when>
									<!-- Ссылка -->
									<xsl:when test="@class ='3'">
										<img src="../Resources/AttributeLink.gif" alt="Ссылка"></img>
									</xsl:when>
									<xsl:otherwise>
										<img  src="../Resources/Attribute.gif" alt="Пользовательский"></img>
									</xsl:otherwise>
								</xsl:choose>
							</td>
							<!--<td>
							<xsl:value-of select="@name"/>
						</td>-->
							<td>
								<xsl:if test="string-length(substring-before((substring-after(@caption, '(')), ')')) = 0">
									<xsl:value-of select="@caption"/>
								</xsl:if>
								<xsl:if test="string-length(substring-before((substring-after(@caption, '(')), ')')) != 0">
									<xsl:value-of select="substring-before(@caption, '(')"/>
									<xsl:variable name="sub" select="substring-before((substring-after(@caption, '(')), ')')"/>
									<span>
										(
										<a href= "{@sourceEntityKey}.html">
											<xsl:value-of select="$sub"/>
										</a>
										)
									</span>
								</xsl:if>
							</td>
							<td>
								<xsl:call-template name="AttributeType"/>
							</td>
							<td>
								<xsl:if test="@scale">
									<xsl:value-of select="concat('(', @size, ',', @scale, ')')"/>
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
								<xsl:if test="@mask">Отображение:</xsl:if>
								<xsl:value-of select="@mask"/>
								<br/>
								<xsl:if test="@mask">Расщепление:</xsl:if>
								<xsl:value-of select="@divide"/>
							</td>
							<td>
								<xsl:value-of select="@description"/>
							</td>
							<td>
								<xsl:call-template name="DeveloperDescriptionLite">
									<xsl:with-param name ="string"  select="DeveloperDescription"/>
								</xsl:call-template>
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
					<tr>
						<td width="5%">
							<img src="../Resources/lev0.gif" alt=""/>
						</td>
						<td>
							(All)
						</td>
						<td>
							<xsl:choose>
								<xsl:when test="@allLevelName">
									<xsl:value-of select="@allLevelName"/>
								</xsl:when>
								<xsl:when test="not(@allLevelName)">нет</xsl:when>
							</xsl:choose>
						</td>
					</tr>
					<td>
						<img src="../Resources/hierarchy.gif" alt=""/>
					</td>
					<td>
						<xsl:value-of select="@name"/>
					</td>
				</tr>
				<tr>
					<td>
						<img src="../Resources/memberKey.gif" alt=""/>
					</td>
					<td>
						memberKey
					</td>
					<td>
						<xsl:value-of select="@memberKey"/>
					</td>
				</tr>
				<tr>
					<td>
						<img src="../Resources/parentKey.gif" alt=""/>
					</td>
					<td>
						parentKey
					</td>
					<td>
						<xsl:value-of select="@parentKey"/>
					</td>
				</tr>
				<tr>
					<td>
						<img src="../Resources/memberName.gif" alt=""/>
					</td>
					<td>
						memberName
					</td>
					<td>
						<xsl:value-of select="@memberName"/>
					</td>
				</tr>
			</tbody>
		</table>
		<xsl:if test="@levelNamingTemplate">
			Шаблон имени: <xsl:value-of select="@levelNamingTemplate"/>
		</xsl:if>
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
					<tr>
						<xsl:if test="@memberKey">
							<td>
								<img src="../Resources/memberKey.gif" alt=""/>
							</td>
							<td>
								<xsl:value-of select="@memberKey"/>
							</td>

						</xsl:if>
					</tr>
					<tr>
						<xsl:if test="@memberName">
							<td>
								<img src="../Resources/memberName.gif" alt=""/>
							</td>
							<td>
								<xsl:value-of select="@memberName"/>
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
					<!--<th>Имя в БД</th>-->
					<th>Русское имя</th>
					<th>Описание разработчика</th>
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
					<!--<td>
						<xsl:value-of select="@name"/>
					</td>-->
					<td>
						<xsl:value-of select="@caption"/>
					</td>
					<td>
						<xsl:call-template name="DeveloperDescriptionLite">
							<xsl:with-param name ="string"  select="DeveloperDescription"/>
						</xsl:call-template>
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
					<!--<th>Имя в БД</th>-->
					<th>Русское имя</th>
					<th>Описание разработчика</th>
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
						<!--<td>
							<xsl:value-of select="concat(@fullNameRoleA, '.', $AssocideName)"/>
						</td>-->
					</xsl:for-each>
					<td>
						<xsl:value-of select="@caption"/>
					</td>
					<td>
						<xsl:call-template name="DeveloperDescriptionLite">
							<xsl:with-param name ="string"  select="DeveloperDescription"/>
						</xsl:call-template>
					</td>
				</tr>

			</xsl:for-each>
		</table>
		<br/>
	</xsl:template>

</xsl:stylesheet>