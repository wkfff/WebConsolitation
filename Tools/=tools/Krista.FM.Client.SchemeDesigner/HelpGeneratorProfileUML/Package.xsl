<?xml version="1.0" encoding="windows-1251"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="html" encoding="windows-1251"/>
	<xsl:include href="common.xsl"/>
	<xsl:param name="mode"/>

	<xsl:template match="/">
		<html>
			<head>
				<META http-equiv="Content-Style-Type" content="text/css"/>
				<link rel="stylesheet" href="../Resources/main.css" type="text/css"/>
			</head>
			<body>
				<xsl:apply-templates select="ServerConfiguration/Package"/>
			</body>
		</html>
	</xsl:template>

	<xsl:template match="ServerConfiguration/Package">
		<table border="0" cellpadding="2">
			<tbody>
			</tbody>
			<tr>
				<td class="contents">
					<h2>
						<xsl:value-of select="@name"/>
					</h2>
				</td>
			</tr>
			<tr>
				<td class="contents">
					<xsl:value-of select="@privatePath"/>
				</td>
			</tr>
		</table>

		<xsl:call-template name="DescrToBlock"/>
		
		<xsl:if test="$mode = 'developerMode' or $mode = 'liteMode'">
			<xsl:call-template name="DeveloperDescription">
				<xsl:with-param name ="string"  select="DeveloperDescription"/>
			</xsl:call-template>
		</xsl:if>
		
		<xsl:for-each select="./Packages">
			<xsl:apply-templates select="//Packages"/>
		</xsl:for-each>
		<xsl:for-each select="./Classes">
			<xsl:apply-templates select="//Classes"/>
		</xsl:for-each>
		<xsl:for-each select="./Associations">
			<xsl:apply-templates select="//Associations"/>
		</xsl:for-each>
		<xsl:for-each select="./Documents">
			<xsl:apply-templates select="//Documents"/>
		</xsl:for-each>
		<link rel="stylesheet" type="text/css" href="../Resources/style.css"/>
	</xsl:template>


	<!-- ������ ���������� -->
	<xsl:template match="//Packages">
		<h2>������ �������</h2>
		<table border="0" cellpadding="5" width="80%">
			<tbody>
				<xsl:for-each select="./Package" >
					<tr>
						<td width ="5%">
							<img src="../Resources/package.gif" alt="�����"/>
						</td>
						<!--<xsl:apply-templates select="//Package"/>-->
						<td>
							<xsl:variable name="���_������">
								<xsl:value-of select="@name"></xsl:value-of>
							</xsl:variable>
							<a href="{$���_������}.html">
								<xsl:value-of select="@name"/>
							</a>
							<br/>
						</td>
					</tr>
				</xsl:for-each>
			</tbody>
		</table>
	</xsl:template>


	<!-- ������ ������� -->
	<xsl:template match="//Classes">
		<h2>������</h2>
		<table  border="0" cellpadding="3" width="90%">
			<tbody>
				<tr>
					<xsl:if test="$mode = 'developerMode' or $mode = 'userMode'">
						<th width="5%"></th>
					</xsl:if>
					<xsl:if test="$mode = 'liteMode'">
						<th width="3%"></th>
					</xsl:if>
					<xsl:if test="$mode = 'developerMode' or $mode = 'userMode'">
						<th width="20%">���������� ���</th>
					</xsl:if>
					<th width="20%">������� ���</th>
					<xsl:if test="$mode = 'developerMode' or $mode = 'userMode'">
						<th width="15%">��� � ��</th>
					</xsl:if>
					<th width="40%">��������</th>
				</tr>
			</tbody>
			<!--������������� ��������������-->
			<xsl:for-each select="./FixedClsDoc">
				<tr>
					<td>
						<img src="../Resources/fixedCls.gif" alt="������������� �������������"/>
					</td>
					<xsl:if test="$mode = 'developerMode' or $mode = 'userMode'">
						<td>
							<xsl:value-of select="concat(@tablePrefix, '.', @semantic, '.', @name)"/>
						</td>
					</xsl:if>
					<td>
						<a href="../classes/{@objectKey}.html">
							<xsl:value-of select="concat(@semanticCaption, '.', @caption)"/>
						</a>
					</td>
					<xsl:if test="$mode = 'developerMode' or $mode = 'userMode'">
						<td>
							<xsl:value-of select="@fullDBName"/>
						</td>
					</xsl:if>
					<td>
						<xsl:value-of select="@description"/>
					</td>
				</tr>
			</xsl:for-each>
			<!--������������ ��������������-->
			<xsl:for-each select="./BridgeClsDoc">
				<tr>
					<td>
						<img src="../Resources/bridgeCls.gif" alt="������������ �������������"/>
					</td>
					<xsl:if test="$mode = 'developerMode' or $mode = 'userMode'">
						<td>
							<xsl:value-of select="concat(@tablePrefix, '.', @semantic, '.', @name)"/>
						</td>
					</xsl:if>
					<td>
						<a href="../classes/{@objectKey}.html">
							<xsl:value-of select="concat(@semanticCaption, '.', @caption)"/>
						</a>
					</td>
					<xsl:if test="$mode = 'developerMode' or $mode = 'userMode'">
						<td>
							<xsl:value-of select="@fullDBName"/>
						</td>
					</xsl:if>
					<td>
						<xsl:value-of select="@description"/>
					</td>
				</tr>
			</xsl:for-each>
			<!--�������������� ������-->
			<xsl:for-each select="./DataClsDoc">
				<tr>
					<td>
						<img src="../Resources/kd.gif" alt="������������� ������"/>
					</td>
					<xsl:if test="$mode = 'developerMode' or $mode = 'userMode'">
						<td>
							<xsl:value-of select="concat(@tablePrefix, '.', @semantic, '.', @name)"/>
						</td>
					</xsl:if>
					<td>
						<a href="../classes/{@objectKey}.html">
							<xsl:value-of select="concat(@semanticCaption, '.', @caption)"/>
						</a>
					</td>
					<xsl:if test="$mode = 'developerMode' or $mode = 'userMode'">
						<td>
							<xsl:value-of select="@fullDBName"/>
						</td>
					</xsl:if>
					<td>
						<xsl:value-of select="@description"/>
					</td>
				</tr>
			</xsl:for-each>
			<!--������� ������-->
			<xsl:for-each select="./DataTableDoc">
				<tr>
					<td>
						<img src="../Resources/factCls.gif" alt="������� ������"/>
					</td>
					<xsl:if test="$mode = 'developerMode' or $mode = 'userMode'">
						<td>
							<xsl:value-of select="concat(@tablePrefix, '.', @semantic, '.', @name)"/>
						</td>
					</xsl:if>
					<td>
						<a href="../classes/{@objectKey}.html">
							<xsl:value-of select="concat(@semanticCaption, '.', @caption)"/>
						</a>
					</td>
					<xsl:if test="$mode = 'developerMode' or $mode = 'userMode'">
						<td>
							<xsl:value-of select="@fullDBName"/>
						</td>
					</xsl:if>
					<td>
						<xsl:value-of select="@description"/>
					</td>
				</tr>
			</xsl:for-each>
			<!--�������-->
			<xsl:for-each select="./TableDoc">
				<tr>
					<td>
						<img src="../Resources/tableCls.gif" alt="�������"/>
					</td>
					<xsl:if test="$mode = 'developerMode' or $mode = 'userMode'">
						<td>
							<xsl:value-of select="concat(@tablePrefix, '.', @semantic, '.', @name)"/>
						</td>
					</xsl:if>
					<td>
						<a href="../classes/{@objectKey}.html">
							<xsl:value-of select="concat(@semanticCaption, '.', @caption)"/>
						</a>
					</td>
					<xsl:if test="$mode = 'developerMode' or $mode = 'userMode'">
						<td>
							<xsl:value-of select="@fullDBName"/>
						</td>
					</xsl:if>
					<td>
						<xsl:value-of select="@description"/>
					</td>
				</tr>
			</xsl:for-each>
			<!--��������� ���������-->
			<xsl:for-each select="./DocumentEntityDoc">
				<tr>
					<td>
						<img src="../Resources/tableCls.gif" alt="�������"/>
					</td>
					<xsl:if test="$mode = 'developerMode' or $mode = 'userMode'">
						<td>
							<xsl:value-of select="concat(@tablePrefix, '.', @semantic, '.', @name)"/>
						</td>
					</xsl:if>
					<td>
						<a href="../classes/{@objectKey}.html">
							<xsl:value-of select="concat(@semanticCaption, '.', @caption)"/>
						</a>
					</td>
					<xsl:if test="$mode = 'developerMode' or $mode = 'userMode'">
						<td>
							<xsl:value-of select="@fullDBName"/>
						</td>
					</xsl:if>
					<td>
						<xsl:value-of select="@description"/>
					</td>
				</tr>
			</xsl:for-each>			
		</table>
	</xsl:template>

	<!--���� �� �������
<xsl:template match="//Associations">
<h2>������ ����������</h2>
<table bgcolor="green" border="1" cellpadding="1">
	<tbody>
		<tr>
			<th>��� ����������</th>
		</tr>
	</tbody>
	<xsl:for-each select="./Reference">
	<tr>
		<td><xsl:value-of select="@name"/></td>
	</tr>
	</xsl:for-each>
	<xsl:for-each select="./Data2Bridge">
	<tr>
		<td><xsl:value-of select="@name"/></td>
	</tr>
	</xsl:for-each>
	<xsl:for-each select="./MasterDetail">
	<tr>
		<td><xsl:value-of select="@name"/></td>
	</tr>
	</xsl:for-each>
	</table>
</xsl:template>
-->

	<!-- ������ ���������� -->
	<xsl:template match="//Documents">
		<h2>���������</h2>
		<table cellpadding="1" border="0">
			<tbody>
			</tbody>

			<xsl:for-each select="./Document">
				<!--������ ���������-->
				<tr>
					<td>
						<img src="../Resources/documents.gif" alt="���������"/>
					</td>
					<td>
						<a href="../diagrams/{@name}.html">
							<xsl:value-of select="@name"/>
						</a>
					</td>
				</tr>
			</xsl:for-each>
		</table>
	</xsl:template>

</xsl:stylesheet>
