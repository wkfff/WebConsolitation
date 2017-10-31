<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Register assembly="Ext.Net" namespace="Ext.Net" tagprefix="ext" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" 
    "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title></title>
    
    <style type="text/css">
        div.item-wrap {
	        float: left;
	        border : 1px solid transparent;
	        margin: 5px;
	        width: 140px;
	        cursor: pointer;
	        height: 120px;
	        text-align: center;
        }

        div.item-wrap img {
	        /*margin: 5px 0px 0px 5px;
	        width: 58px;
	        height: 48px;
	        background: url(/Content/images/icon.png) no-repeat;*/
        }

        div.item-wrap h6 {
	        font-size: 14px;
	        color: #3A4B5B;
	        font-family: tahoma,arial,san-serif;
        }
        
        .items-view .x-view-over { border: solid 1px silver; }

        #items-ct { padding: 20px 30px 24px 30px; }

        #items-ct h2 {
            border-bottom: 2px solid #3A4B5B;           
            cursor:pointer;      
        }

        #items-ct h2 div {
            background:transparent url(/Content/images/group-expand-sprite.gif) no-repeat 3px -47px;
            padding:4px 4px 4px 17px;
            font-family: tahoma,arial,san-serif;
            font-size: 12px;
            color: #3A4B5B;
        }

        #items-ct .collapsed h2 div { background-position: 3px 3px; }
        #items-ct dl { margin-left: 2px; }
        #items-ct .collapsed dl { display:none; }

    </style>
    
    <script type="text/javascript">

        var selectionChanged = function (dv, nodes) {
            if (nodes.length > 0) {
                var panel = nodes[0].getAttribute('ext:panel');
                var menu = nodes[0].getAttribute('ext:menu');
                parent.menuItem = { id: menu };
                var action = nodes[0].getAttribute('ext:action');

                if (!Ext.isEmpty(panel, false)) {
                    parent.window[panel].expand();
                }

                if (!Ext.isEmpty(parent.menuItem.id, false)) {
                    parent.menuItem.text = parent.window[menu].text;
                    parent.menuItem.url = parent.window[menu].url;
                    parent.menuItem.iconCls = parent.window[menu].iconCls;
                    parent.menuItem.passParentSize = false;
                    parent.eval(action);
                }
            }
        }
        
        var viewClick = function(dv, e) {
            var group = e.getTarget('h2', 3, true);
            if (group) {
                group.up('div').toggleClass('collapsed');
            }
        }
    </script>
</head>
<body>
    <ext:ResourceManager ID="resourceManager" runat="server" EnableTheming="true" DisableViewState="true"/>
    
    <ext:Store runat="server" ID="Store1" AutoLoad="true">
        <Proxy>
            <ext:HttpProxy Url="/Home/GetHomeSchema/" />
        </Proxy>
        <Reader>
            <ext:JsonReader Root="data">
                <Fields>
                    <ext:RecordField Name="Title" />
                    <ext:RecordField Name="Items" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    
    <ext:Panel ID="DashBoardPanel" runat="server" Cls="items-view" AutoHeight="true" Border="false">
        <Content>
            <ext:FitLayout ID="FitLayout1" runat="server">
                <Items>
                    <ext:DataView ID="DataView1" 
                        runat="server" 
                        StoreID="Store1" 
                        SingleSelect="true"
                        OverClass="x-view-over" 
                        ItemSelector="div.item-wrap" 
                        AutoHeight="true" 
                        EmptyText="No items to display">
                        <Template ID="Template1" runat="server">
                            <Html>
                                <div id="items-ct">
                                    <tpl for=".">
                                        <div>
                                            <h2><div>{Title}</div></h2>
                                            <dl>
                                                <tpl for="Items">
                                                    <div class="item-wrap" ext:panel="{Accordion}" ext:menu="{MenuItem}" ext:action="{Action}">
                                                        <img src="{Icon}"/>
                                                        <div>
                                                            <H6>{Title}</H6>                                                    
                                                        </div>
                                                    </div>
                                                </tpl>
                                                <div style="clear:left"></div>
                                             </dl>
                                        </div>
                                    </tpl>
                                </div>
                            </Html>
                        </Template>
                        <Listeners>
                            <SelectionChange Fn="selectionChanged" />
                            <ContainerClick Fn="viewClick" />
                        </Listeners>
                    </ext:DataView>
                </Items>
            </ext:FitLayout>
        </Content>
    </ext:Panel>
</body>
</html>