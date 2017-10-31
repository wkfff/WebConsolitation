
function igtbl_onKeyDown(evnt,gn,my)
{var gs=igtbl_getGridById(gn);if(!evnt)evnt=window.event;if(!gs||!evnt||!gs.isLoaded())
return;gs.event=evnt;var processed=false;var canceled=false;if(!gs.Activation.AllowActivation)
return;var se=igtbl_srcElement(evnt);if(gs._focus==null&&se&&!igtbl_contains(gs.Element.parentNode.parentNode,se))
return;if(gs._focus===false||igtbl_inEditMode(gn))return;var te=gs.Element;var cell=gs.getActiveCell();var row=gs.getActiveRow();if(!my&&se&&se.tagName&&se.tagName.length>4&&(!cell||cell.Column.ColumnType!=7))
return;var elId,nextCell=null,nextRow=null,key=evnt.keyCode;if(cell)
elId=cell.Element.id;else if(row)
elId=row.Element.id;else
return;if(igtbl_fireEvent(gn,gs.Events.KeyDown,"(\""+gn+"\",\""+elId+"\","+key+")")==true)
return;switch(key)
{case 9:if(cell)
{if(evnt.ctrlKey)
nextRow=cell.Row;else
nextCell=cell.getNextTabCell(evnt.shiftKey,true,true);}
else
{if(evnt.ctrlKey)
nextCell=row.getCell(0);else
nextRow=row.getNextTabRow(evnt.shiftKey,false,true);}
if(evnt.shiftKey)
{if(nextCell)
{te.setAttribute("startPointCell",nextCell.Element.id);te.setAttribute("selectMethod","cell");te.setAttribute("selectTable",nextCell.Row.Element.parentNode.parentNode.id);te.setAttribute("startPointRow",nextCell.Row.Element.id);}
else if(nextRow)
{te.setAttribute("selectMethod","row");te.setAttribute("selectTable",nextRow.Element.parentNode.parentNode.id);te.setAttribute("startPointRow",nextRow.Element.id);}}
if(row&&(row.IsAddNewRow&&(!nextCell||nextCell.Row!=row)))
{if(row.commit())
{var nac=null;if(cell)
{nac=row.getCell(0);while(nac&&!nac.Column.getVisible())
nac=row.getCell(nac.Column.Index+1);if(nac)
nextCell=nac;}
if(!nac)
nextRow=row;processed=true;}
else if(row.OwnerCollection.length>0)
{if(row.Band.AddNewRowView==2&&evnt.shiftKey)
{var nac=null;var nar=row.OwnerCollection.getRow(row.OwnerCollection.length-1);while(nar.Rows&&nar.getExpanded())
{if(nar.Rows.AddNewRow&&nar.Band.AddNewRowView==2)
nar=nar.Rows.AddNewRow;else
nar=nar.Rows.getRow(nar.Rows.length-1);}
if(cell&&nar)
{nac=nar.getCell(nar.cells.length-1);while(nac&&!nac.Column.getVisible())
nac=nar.getCell(nac.Column.Index-1);if(nac)
nextCell=nac;}
if(!nac)
nextRow=nar;processed=true;}
else if(row.Band.AddNewRowView==1&&!evnt.shiftKey)
{var nac=null;var nar=row.OwnerCollection.getRow(0);if(cell)
{nac=nar.getCell(0);while(nac&&!nac.Column.getVisible())
nac=nar.getCell(nac.Column.Index+1);if(nac)
nextCell=nac;}
if(!nac)
nextRow=nar;processed=true;}}}
if(nextCell||nextRow)
processed=true;else
{if(row)
{if(row.processUpdateRow)
row.processUpdateRow();}}
break;case 13:var b=igtbl_getElementById(gn+"_bt");if(b&&b.style.display!="none")
{processed=true;igtbl_colButtonClick(evnt,gn);}
else if(cell)
{processed=true;if(cell.Column.ColumnType==3)
{if(cell.isEditable())
cell.setValue(!cell.getValue());}
else if(cell.Column.ColumnType==7)
{if(cell.Column.CellButtonDisplay==0)
b.fireEvent("onclick");else
{var bi=cell.Element.childNodes[0];if(bi.tagName=="NOBR")
bi=bi.childNodes[0];if(typeof(bi.fireEvent)!="undefined")
bi.fireEvent("onclick");}}
else if(cell.getTargetURL())
igtbl_navigateUrl(cell.getTargetURL());else
cell.beginEdit();}
else if(row&&row.GroupByRow)
{processed=true;row.toggleRow();}
break;case 16:processed=true;if(cell)
{if(!te.getAttribute("startPointCell"))
te.setAttribute("startPointCell",cell.Element.id);te.setAttribute("selectMethod","cell");row=cell.Row;if(igtbl_getSelectTypeCell(gn,row.Band.Index)==3)
{te.setAttribute("shiftSelect",true);te.setAttribute("startPointCell",cell.Element.id);}}
else
{te.setAttribute("selectMethod","row");if(igtbl_getSelectTypeRow(gn,row.Band.Index)==3)
{te.setAttribute("shiftSelect",true);if(!te.getAttribute("startPointRow"))
te.setAttribute("startPointRow",row.Element.id);}}
te.setAttribute("selectTable",row.Element.parentNode.parentNode.id);break;case 32:if(cell)
{if(igtbl_getSelectTypeCell(gn,cell.Column.Band.Index)==3)
{processed=true;cell.setSelected(!cell.getSelected());}
else if(cell.Column.ColumnType==3)
{processed=true;if(cell.isEditable())
{var val=cell.getValue();if(val==="false")val=false;cell.setValue(!val);}}}
else if(row)
{processed=true;if(igtbl_getSelectTypeRow(gn,row.Band.Index)==3)
row.setSelected(!row.getSelected());}
break;case 35:if(cell)
{nextCell=cell.Row.getCell(cell.Row.cells.length-1);if(!nextCell.Column.getVisible())
nextCell=nextCell.getPrevCell();if(nextCell==cell)
nextCell=null;}
else
{nextRow=row.OwnerCollection.getRow(row.OwnerCollection.length-1);if(nextRow.getHidden())
nextRow=nextRow.getPrevRow(true,true);if(nextRow==row)
nextRow=null;}
if(nextCell||nextRow)
processed=true;break;case 36:if(cell)
{nextCell=cell.Row.getCell(0);while(nextCell&&!nextCell.Element)
{nextCell=nextCell.getNextCell();}
if(nextCell&&!nextCell.Column.getVisible())
{nextCell=nextCell.getNextCell();}
if(nextCell==cell)
{nextCell=null;}}
else
{nextRow=row.OwnerCollection.getRow(0);if(nextRow.getHidden())
nextRow=nextRow.getNextRow(true,true);if(nextRow==row)
nextRow=null;}
if(nextCell||nextRow)
processed=true;break;case 37:if(cell)
{var possibleNextCell=cell;do
{nextCell=possibleNextCell.getPrevCell();if(!nextCell)
{var prevRow=possibleNextCell.Row.getPrevRow(true,true);if(prevRow)
{nextCell=prevRow.getCell(prevRow.cells.length-1);if(!nextCell.Column.getVisible()||!nextCell.getElement())
nextCell=nextCell.getPrevCell();}}
possibleNextCell=nextCell;}while(nextCell&&!nextCell.Element)
if(nextCell)
processed=true;else
gs.getDivElement().scrollLeft=0;}
else if(row.Band.getExpandable()==1)
{processed=true;row.setExpanded(false);}
break;case 39:if(cell)
{var possibleNextCell=cell;do
{nextCell=possibleNextCell.getNextCell();if(!nextCell)
{var nextRow=possibleNextCell.Row.getNextRow(true,true);if(nextRow)
{nextCell=nextRow.getCell(0);if(!nextCell.Column.getVisible()||!nextCell.getElement())
nextCell=nextCell.getNextCell();}}
possibleNextCell=nextCell;}while(nextCell&&!nextCell.getElement())
if(nextCell)
processed=true;}
else if(row.Band.getExpandable()==1)
{processed=true;row.setExpanded(true);}
break;case 38:if(cell&&cell.Row.getPrevRow(true,true))
{var nr=cell.Row.getPrevRow(true,true);while(!nextCell&&nr)
{nextCell=nr.getCellByColumn(cell.Column);nr=nr.getPrevRow(true,true);if(nextCell&&nextCell.Element==null)nextCell=null;}}
else if(row)
nextRow=row.getPrevRow(true,true);if(nextCell||nextRow)
processed=true;else if(row&&row.Band.Index==0)
gs.getDivElement().scrollTop=0;break;case 40:if(cell&&cell.Row.getNextRow(true,true))
{var nr=cell.Row.getNextRow(true,true);while(!nextCell&&nr)
{nextCell=nr.getCellByColumn(cell.Column);nr=nr.getNextRow(true,true);if(nextCell&&nextCell.Element==null)nextCell=null;}}
else if(row)
nextRow=row.getNextRow(true,true);if(!nextCell&&!nextRow&&gs.Node&&gs.RowsRange>0&&gs.RowsServerLength>gs.Rows.length&&gs.XmlLoadOnDemandType!=2)
if(gs.ReadyState==0)
gs.invokeXmlHttpRequest(gs.eReqType.MoreRows);if(nextCell||nextRow)
processed=true;break;default:if(evnt.ctrlKey&&(key==67||key==45||key==88||key==46||key==86))
{switch(key)
{case 67:case 45:gs.copy();break;case 88:case 46:gs.cut();break;case 86:gs.paste();break;}
canceled=true;}
else if(evnt.shiftKey&&key==45)
{gs.paste();canceled=true;}
else
if(key==46)
{processed=true;gs.deleteSelectedRows();}
else if(key>=48&&key<=57||key>=54&&key<=90||key>=96&&key<=111||key>=186&&key<=192||key>=219&&key<=222||key==113||key==107||key==109)
{if((key==107||key==109)&&(!cell||!cell.isEditable()))
{if(cell&&cell.Row.Band.getExpandable()==1)
{processed=true;cell.Row.setExpanded(key==107);}
else if(row&&row.Band.getExpandable()==1)
{processed=true;row.setExpanded(key==107);}
break;}
else if(cell)
{if(cell.isEditable()&&cell.Column.ColumnType!=3)
cell.beginEdit(key);else if(cell.Column.getAllowUpdate()==3)
cell.Row.editRow();}
else if(row&&key==113)
row.editRow();}
break;}
if(!canceled&&(nextCell||nextRow))
{if(nextCell)
{var stc=nextCell.Row.Band.getSelectTypeCell();if((!evnt.shiftKey||key==9)&&(!evnt.ctrlKey||stc!=3))
igtbl_clearSelectionAll(gn);if(evnt.shiftKey&&key!=9)
igtbl_selectRegion(gn,nextCell.Element);else if(!evnt.ctrlKey&&stc==3||stc==2)
nextCell.setSelected();nextCell.activate();nextCell.scrollToView();if(nextCell.hasButtonEditor(igtbl_cellButtonDisplay.OnMouseEnter))
igtbl_showColButton(gn,nextCell.Element);else if(key==9&&nextCell.Row.Band.getCellClickAction()==1)
igtbl_EnterEditMode(gn);}
else
{var str=nextRow.Band.getSelectTypeRow();if((!evnt.shiftKey||key==9)&&(!evnt.ctrlKey||str!=3))
igtbl_clearSelectionAll(gn);igtbl_setActiveRow(gn,nextRow.getFirstRow());if(evnt.shiftKey&&key!=9)
{var firstElem=igtbl_getFirstCell(gn,nextRow.Element);if(firstElem.previousSibling)firstElem=firstElem.previousSibling;igtbl_selectRegion(gn,firstElem);}
else if(!evnt.ctrlKey&&str==3||str==2)
nextRow.setSelected();nextRow.scrollToView();}
if(gs.NeedPostBack)
igtbl_doPostBack(gn);}
if(canceled)
return ig_cancelEvent(evnt);else
if(processed)
{if(key!=16&&!evnt.shiftKey)
{te.removeAttribute("selectMethod");te.removeAttribute("selectTable");te.removeAttribute("startPointRow");te.removeAttribute("startPointCell");}
ig_cancelEvent(evnt);}}
function igtbl_onKeyUp(evnt,gn)
{var gs=igtbl_getGridById(gn);if(!evnt)evnt=window.event;if(!gs||!evnt||!gs.Activation.AllowActivation)
return;gs.event=evnt;var se=igtbl_srcElement(evnt);if(gs._focus==null&&!igtbl_contains(gs.Element.parentNode.parentNode,se))
return;if(gs._focus===false||igtbl_inEditMode(gn))return;var te=gs.Element,cell=gs.oActiveCell;if(!cell)
cell=gs.oActiveRow;if(cell)
gs.fireEvent(gs.Events.KeyUp,[gs.Id,cell.Element.id,evnt.keyCode]);}
function igtbl_rowFromRows(rows,n)
{if(n<0||!rows)
return null;var i=0,j=0;var row=rows[0];while(row&&(row.getAttribute("filterRow")||row.getAttribute("addNewRow")))
row=rows[++j];while(row&&i<n)
{if(i>=rows.length-1)
return null;row=rows[++j];if(row&&(row.getAttribute("hiddenRow")||row.parentNode.tagName=="TFOOT"))
row=rows[++j];i++;}
return row;}
function igtbl_getFirstCell(gn,row)
{if(row.getAttribute("groupRow"))
return row.childNodes[0].childNodes[0].childNodes[0].rows[0].cells[0];else
return row.cells[igtbl_getBandFAC(gn,row)];}
function igtbl_getParentRow(gn,row)
{var l=igtbl_getRowLevel(row.id);if(l.length==1)
{delete l;return null;}
var pl=igtbl_copyArray(l,l.length-1);var pr=igtbl_getRow(gn,pl);delete pl;delete l;return pr;}
function igtbl_getCurRow(c)
{var r=null;while(c&&!r)
if(c.tagName=="TR"&&!c.getAttribute("hiddenRow"))
r=c;else
c=c.parentNode;if(r&&r.getAttribute("groupRow"))
r=r.parentNode.parentNode.parentNode.parentNode;return r;}
function igtbl_getFirstSibRow(gn,row)
{var rl=igtbl_getRowLevel(row.id);var rlns=igtbl_copyArray(rl);rlns[rlns.length-1]=0;var ns=igtbl_getRow(gn,rlns);while(ns&&(ns.getAttribute("deleted")||ns.style.display=="none"))
{rlns[rlns.length-1]++;ns=igtbl_getRow(gn,rlns);}
delete rlns;delete rl;return ns;}
function igtbl_getLastSibRow(gn,row)
{var lastRow=row;var ns=igtbl_getNextSibRow(gn,lastRow);while(ns)
{lastRow=ns;ns=igtbl_getNextSibRow(gn,lastRow);}
return lastRow;}
function igtbl_getFirstChildRow(gn,row)
{var rl=igtbl_getRowLevel(row.id);var rlc=igtbl_copyArray(rl);rlc[rlc.length]=0;var ns=igtbl_getRow(gn,rlc);if(ns&&(ns.getAttribute("deleted")||ns.style.display=="none"))
ns=igtbl_getNextSibRow(gn,ns);delete rlc;delete rl;return ns;}
function igtbl_getLastChildRow(gn,row)
{var ns=igtbl_getFirstChildRow(gn,row);if(ns)
{var r=igtbl_getNextSibRow(gn,ns);while(r)
{ns=r;r=igtbl_getNextSibRow(gn,ns);}}
return ns;}
function igtbl_ActivateNextCell(gn)
{var gs=igtbl_getGridById(gn);if(!gs||!gs.Activation.AllowActivation)
return null;var cell=gs.oActiveCell;if(!cell)
return null;var nextCell=cell.getNextTabCell(false,true);if(nextCell)
{igtbl_setActiveCell(gn,nextCell.Element);if(gs.getActiveCell()==nextCell)
{igtbl_clearSelectionAll(gn);igtbl_selectCell(gn,nextCell);nextCell.scrollToView();if(gs.NeedPostBack)
igtbl_doPostBack(gn);return nextCell.Element;}
else
return cell.Element;}
return null;}
function igtbl_ActivatePrevCell(gn)
{var gs=igtbl_getGridById(gn);if(!gs||!gs.Activation.AllowActivation)
return null;var cell=gs.oActiveCell;if(!cell)
return null;var prevCell=cell.getNextTabCell(true,true);if(prevCell)
{igtbl_setActiveCell(gn,prevCell.Element);if(gs.getActiveCell()==prevCell)
{igtbl_clearSelectionAll(gn);igtbl_selectCell(gn,prevCell);prevCell.scrollToView();if(gs.NeedPostBack)
igtbl_doPostBack(gn);return prevCell.Element;}
else
return cell.Element;}
return null;}
function igtbl_EnterEditMode(gn)
{var gs=igtbl_getGridById(gn);if(!gs||!gs.Activation.AllowActivation)
return;var cell=gs.oActiveCell;if(!cell)
return;cell.beginEdit();gs._exitEditCancel=false;}
function igtbl_EndEditMode(gn)
{igtbl_hideEdit(gn);}
function igtbl_getActiveCell(gn)
{var gs=igtbl_getGridById(gn);if(!gs||!gs.Activation.AllowActivation)
return null;return gs.getActiveCell();}
function igtbl_getActiveRow(gn)
{var gs=igtbl_getGridById(gn);if(!gs||!gs.Activation.AllowActivation)
return null;return gs.getActiveRow();}
function igtbl_getRowLevel(rowId)
{var rowObj=igtbl_getElementById(rowId);if(rowObj.getAttribute("level"))
rowId=rowObj.getAttribute("level");var rn=rowId.split("_");var fn=rn.length-1;while(fn>=0)
{if(!parseInt(rn[fn],10)&&rn[fn]!="0")
break;fn--;}
fn++;var res=new Array();for(var i=fn;i<rn.length;i++)
res[i-fn]=parseInt(rn[i],10);return res;}
function igtbl_getNextSibRow(gn,row)
{var rl=igtbl_getRowLevel(row.id);var rlns=igtbl_copyArray(rl);rlns[rlns.length-1]++;var ns=igtbl_getRow(gn,rlns);while(ns&&(ns.getAttribute("deleted")||ns.style.display=="none"))
{rlns[rlns.length-1]++;ns=igtbl_getRow(gn,rlns);}
delete rlns;delete rl;return ns;}
function igtbl_getPrevSibRow(gn,row)
{var rl=igtbl_getRowLevel(row.id);var rlps=igtbl_copyArray(rl);rlps[rlps.length-1]--;var ps=igtbl_getRow(gn,rlps);while(ps&&(ps.getAttribute("deleted")||ps.style.display=="none"))
{rlps[rlps.length-1]--;ps=igtbl_getRow(gn,rlps);}
delete rlps;delete rl;return ps;}
function igtbl_copyArray(src,count)
{if(!count)
count=src.length;var dest=new Array();for(var i=0;i<count;i++)
dest[i]=src[i];return dest;}
function igtbl_getRow(gn,l)
{if(!l.length||!l[0]&&l[0]!=0)
return null;var te=igtbl_getGridById(gn).Element;var clr=te.tBodies[0].rows;var row=igtbl_rowFromRows(clr,l[0]);if(row&&row.parentNode.tagName=="TFOOT")
return;for(var i=1;i<l.length;i++)
if(!row||!l[i]&&l[i]!=0)
break;else
{clr=igtbl_getChildRows(gn,row);row=igtbl_rowFromRows(clr,l[i]);}
return row;}
function _igtbl_determineEditorPosition(g,gn,cellObj,cElem,cell)
{var r=igtbl_getAbsBounds(cElem,g,true);if(g.IsXHTML&&!cellObj.Column.EditorControlID)
{r.x--;r.y--;}
var z=g._getZ(99998);var offsWidth=cElem.offsetWidth;var offsHeight=cElem.offsetHeight;var w=(g.DivElement.clientWidth&&g.DivElement.clientWidth<offsWidth?g.DivElement.clientWidth:offsWidth)-2,h=(g.DivElement.clientHeight&&g.DivElement.clientHeight<offsHeight?g.DivElement.clientHeight:offsHeight)-2,ch=cElem.clientHeight;if(ch==null||ch==0)
{w--;h--;}
if(w<5)
w=5;if(h<5)
h=18;r.w=w;r.h=h;r.z=z;if((z=ig_csom.AgentName.indexOf("netscape/7.0"))>0)
{r.x++;r.y++;}
if(cellObj.Row.IsFilterRow)
{var buttonShift=cell.childNodes[0];buttonShift=buttonShift.offsetLeft+buttonShift.offsetWidth;if(buttonShift<r.w)
{r.w-=buttonShift;r.x+=buttonShift;}}
return r;}
function igtbl_editCell(evnt,gn,cell,key)
{var g=igtbl_getGridById(gn);if(g==null||!g.isLoaded())
return;g.event=evnt;if(g._editorCurrent!=null)
{if(g._editorCurrent.getAttribute("currentCell")==cell.id)return;igtbl_hideEdit(null,null,g);}
var cellObj=igtbl_getCellById(cell.id);var cElem=cellObj.getElement();if(cellObj==null)
return;if((g._exitEditCancel&&!g._returnToEditModeFromScroll)||!cellObj.isEditable())
{return;}
if(cellObj.Row.Band.getCellClickAction()==1||cellObj.Row.Band.getCellClickAction()==3)
{if(cellObj!=g.oActiveCell)
g.setActiveCell(cellObj);}
else if(cellObj.Row.Band.getCellClickAction()==2)
{if(cellObj.Row!=g.oActiveRow)
g.setActiveRow(cellObj.Row);}
var col=cellObj.Column;if(col.ColumnType==3)
{var chBx=cell.firstChild;if(chBx.tagName!="INPUT")
chBx=chBx.firstChild;if(chBx.tagName=="INPUT")
try
{if(!chBx.getAttribute("gn"))
{ig_csom.addEventListener(chBx,"keydown",igtbl_inputEvt);chBx.setAttribute("gn",gn);chBx.setAttribute("cellId",cell.id);}}
catch(exception){;}
return;}
if(cellObj.hasButtonEditor())
return;if(igtbl_fireEvent(gn,g.Events.BeforeEnterEditMode,"(\""+gn+"\",\""+cell.id+"\")")==true)
return;cellObj.scrollToView();var r=_igtbl_determineEditorPosition(g,gn,cellObj,cElem,cell);var css=null,e=cell,i=0;while(++i<10&&(css==null||css==""))
if((e=e.parentNode)!=null)
css=e.getAttribute("bandNo");if(css!=null&&css!="")
css=igtbl_getEditCellClass(gn,css);if(!css)css="";var v=(key!=null&&key>0&&key!=113)?"":cellObj.getValue();if(v==null)
if((v=cell.getAttribute(igtbl_sUnmaskedValue))==null)
v="";var href=-1,nn=cElem.childNodes;for(z=0;z<nn.length;z++)
if(nn[z].tagName=="A")
href=z;g._editorCustom=null;var elem=igtbl_editCust(g,(key!=null&&key>0&&key!=113)?"":v,r,col,cElem);v=cellObj.Node?cellObj.getNodeValue():(cell.getAttribute(igtbl_sigDataValue)?unescape(cell.getAttribute(igtbl_sigDataValue)):v.toString());if(!elem)
elem=igtbl_editList(g,v,r,col,cElem);if(!elem)
{elem=igtbl_editDef(g,v,r,col,css,cell,cElem);if(elem)
{var keyupHandled=elem.getAttribute("_igkeyupevent");if(cellObj.Row.IsFilterRow)
{if(!keyupHandled)
{ig_csom.addEventListener(elem,"keyup",igtbl_stringEditorKeyPress);elem.setAttribute("_igkeyupevent","true");}}
else
{if(keyupHandled)
{ig_csom.removeEventListener(elem,"keyup",igtbl_stringEditorKeyPress);elem.removeAttribute("_igkeyupevent");}}}
else
{return;}}
cellObj._oldValue=v;if(href>=0)
elem.setAttribute("hasHref",href);g._editorCurrent=elem;elem.setAttribute("gn",gn);elem.setAttribute("currentCell",cell.id);igtbl_fireEvent(gn,g.Events.AfterEnterEditMode,"(\""+gn+"\",\""+cell.id+"\");");if(typeof(Page_Validators)!="object"||cellObj.Row.IsFilterRow)
return;var i,j,id=null,colV=col.Validators,pgV=Page_Validators;if((e=col.getEditorControl())!=null)
if((id=e.ID)==null)
if((id=e.id)==null)
id=e.Id;if(id==null)
id=elem.id;else
for(i=0;i<pgV.length;i++)
{if(pgV[i]&&pgV[i].controltovalidate==id)
{if(!colV)colV=col.Validators=new Array();for(j=colV.length-1;j>=0;j--)if(colV[j]==pgV[i].id)break;if(j<0)colV[colV.length]=pgV[i].id;}}
for(i=0;i<colV.length;i++)
{if((v=igtbl_getElementById(colV[i]))==null)
continue;if(v.parentNode!=document.body)
document.body.insertBefore(v,document.body.firstChild);s=v.style;s.zIndex=g._getZ(99999);s.position="absolute";v.style.left=r.x+"px";v.style.top=(r.y+r.h+2)+"px";try
{v.unselectable="on";}catch(ex){;}
if(e!=null)
continue;if(ig_csom.IsNetscape6)
v.controltovalidate=id;else
v.setAttribute("controltovalidate",id);ValidatorHookupControlID(id,v);}
for(i=0;i<pgV.length;i++)
if(pgV[i].controltovalidate==id)
for(j=0;j<colV.length;j++)
if(pgV[i].id==colV[j])
pgV[i].enabled=pgV[i].isvalid=true;}
function igtbl_editCust(g,v,r,col,cElem)
{var id=col.EditorControlID,editor=col.getEditorControl();if(editor==null&&id!=null)
{if((editor=igtbl_getElementById(id))!=null)
editor=editor.Object;}
if(editor==null)
return null;col.editorControl=editor;var elem=editor.Element;if(!editor._old_parent)
editor._old_parent=elem.parentNode;g._ensureValidParent(editor);editor.setValue(v,false);elem.style.zIndex=r.z;if(elem.tagName=="INPUT")
{if(g.IsXHTML)
{if(ig_csom.IsIE)
{r.x-=2;r.y-=2;}
var cStyle,eStyle;var wd=0,hd=0;if(ig_csom.IsIE)
{cStyle=cElem.currentStyle;eStyle=elem.currentStyle;wd=igtbl_parseInt(cStyle.borderLeftWidth)+igtbl_parseInt(cStyle.borderRightWidth)-igtbl_parseInt(eStyle.borderLeftWidth)-igtbl_parseInt(eStyle.borderRightWidth);hd=igtbl_parseInt(cStyle.borderTopWidth)+igtbl_parseInt(cStyle.borderBottomWidth)-igtbl_parseInt(eStyle.borderTopWidth)-igtbl_parseInt(eStyle.borderBottomWidth);}
else
{cStyle=cElem.style;eStyle=elem.style;wd=igtbl_parseInt(cStyle.borderLeftWidth)+igtbl_parseInt(cStyle.borderRightWidth)-igtbl_parseInt(eStyle.borderLeftWidth)-igtbl_parseInt(eStyle.borderRightWidth);hd=igtbl_parseInt(cStyle.borderTopWidth)+igtbl_parseInt(cStyle.borderBottomWidth)-igtbl_parseInt(eStyle.borderTopWidth)-igtbl_parseInt(eStyle.borderBottomWidth);}
if(r.w<0)r.w=1;if(r.h<0)r.h=1;}
else
{var frameBorderZero=(window.frameElement&&window.frameElement.frameBorder=="0");if(ig_csom.IsIE&&frameBorderZero)
{r.x+=2;r.y+=2;}
else if(ig_csom.IsFireFox)
{r.x+=1;r.y+=1;}}}
else if(ig_csom.IsIE&&g.IsXHTML)
{r.x--;r.y--;}
if(ig_csom.IsFireFox&&g.IsXHTML)
{r.y-=g.DivElement.scrollTop;r.x-=g.DivElement.scrollLeft;}
editor.setVisible(true,r.x,r.y,r.w,r.h);editor.webGrid=g;editor.addEventListener("blur",igtbl_hideEdit,g);editor.addEventListener("keydown",igtbl_hideEdit,g);elem.setAttribute("editType",4);g._editorCustom=editor;return elem;}
function igtbl_editDef(g,v,r,col,css,cell,cElem)
{var elem=g._editorInput,nn=cElem.childNodes;var area=(col.CellMultiline==1)?1:0;if(area==1)
elem=g._editorArea;var w=cElem.offsetWidth,h=cElem.offsetHeight,ie=ig_csom.IsIE;var s,i=ie?0:nn.length;while(i-->0)
{var curElem=nn[i];if(!cell.Object.Row.IsFilterRow||curElem.tagName!="BUTTON")
{if(curElem.style)
curElem.style.display="none";else
{if(g._oldElems==null)
g._oldElems=new Array();g._oldElems[g._oldElems.length]=curElem;cElem.removeChild(curElem);}}}
var justCreated=false;if(!elem)
{if(area==1){g._editorArea=elem=document.createElement("TEXTAREA");ig_csom.addEventListener(g._editorArea,"keypress",_igtbl_textareaEditorKeyPress);ig_csom.addEventListener(g._editorArea,"paste",_igtbl_textareaEditorPaste);ig_csom.addEventListener(g._editorArea,"input",_igtbl_textareaEditorInput);}
else
{g._editorInput=elem=document.createElement("INPUT");elem.type="text";}
if(ie)
document.body.insertBefore(elem,document.body.firstChild);else
cElem.appendChild(elem);ig_csom.addEventListener(elem,"keydown",igtbl_editEvt);ig_csom.addEventListener(elem,"keyup",igtbl_editEvt);ig_csom.addEventListener(elem,"blur",igtbl_editEvt);elem.setAttribute("editType",area);i=g.Id+((area==1)?"_ta":"_tb");elem.id=i;justCreated=true;}
else if(!ie)
cElem.appendChild(elem);elem.value="";if(css.length>0)
elem.className=css;s=elem.style;var eStyle=igtbl_getComputedStyle(elem);if(eStyle==null)eStyle=elem.style;s.zIndex=r.z;if(ig_csom.IsIE&&g.IsXHTML)
{r.x--;r.y--;var edLeftWidth=igtbl_parseInt(eStyle.borderLeftWidth);var edRightWidth=igtbl_parseInt(eStyle.borderRightWidth);var edTopWidth=igtbl_parseInt(eStyle.borderTopWidth);var edBottomWidth=igtbl_parseInt(eStyle.borderBottomWidth);if(edLeftWidth||edRightWidth)
{var wd=igtbl_parseInt(cElem.currentStyle.borderLeftWidth)+igtbl_parseInt(cElem.currentStyle.borderRightWidth)-edLeftWidth-edRightWidth;if(wd)
r.w+=wd;}
else
{var wd=igtbl_parseInt(cElem.currentStyle.borderLeftWidth);if(wd)
r.x+=wd;}
if(edTopWidth||edBottomWidth)
{var hd=igtbl_parseInt(cElem.currentStyle.borderTopWidth)+igtbl_parseInt(cElem.currentStyle.borderBottomWidth)-edTopWidth-edBottomWidth;if(hd)
r.h+=hd;}
else
{var hd=igtbl_parseInt(cElem.currentStyle.borderTopWidth);if(hd)
r.y+=hd;}}
if(g.IsXHTML)
{var wd=0;var hd=0;var pd=eStyle.paddingLeft;if(pd&&pd.length>2&&pd.substr(pd.length-2,2)=="px")
wd-=igtbl_parseInt(pd);pd=eStyle.paddingRight;if(pd&&pd.length>2&&pd.substr(pd.length-2,2)=="px")
wd-=igtbl_parseInt(pd);pd=eStyle.paddingTop;if(pd&&pd.length>2&&pd.substr(pd.length-2,2)=="px")
hd-=igtbl_parseInt(pd);pd=eStyle.paddingBottom;if(pd&&pd.length>2&&pd.substr(pd.length-2,2)=="px")
hd-=igtbl_parseInt(pd);if(wd)
r.w+=wd;if(hd)
r.h+=hd;if(r.w<0)r.w=1;if(r.h<0)r.h=1;}
if(ie)
{s.position="absolute";s.left=r.x+"px";s.top=r.y+"px";s.height=r.h+"px";}
else
{if(r.w>10)
{if(g.IsXHTML)
r.w-=igtbl_parseInt(igtbl_dom.css.getComputedStyle(cElem,"paddingLeft"))+igtbl_parseInt(igtbl_dom.css.getComputedStyle(cElem,"paddingRight"))+3;else
r.w-=6;}
s.height="";}
s.width=r.w+"px";if(area==0)
{i=col.FieldLength;elem.maxLength=(i!=null&&i>0)?i:2147483647;}
if(area==1)
s.overflow="auto";s.borderWidth=ig_csom.isEmpty(elem.className)?"0":"";s.display="";if(!ie)
{cell.style.height=h+"px";for(i=1;i<8;i++)
if(cElem.offsetWidth>w)
cell.style.width=(w-i)+"px";s.height=(r.h-1)+"px";if(cElem.offsetHeight>h)
s.height=(r.h-2)+"px";}
elem.value=v;try
{elem.select();elem.focus();}catch(e){;}
return elem;}
function _igtbl_textareaEditorKeyPress(evnt){var textbox=igtbl_srcElement(evnt);var grid=igtbl_getGridById(textbox.id.substring(0,textbox.id.length-3));var column=igtbl_getCellById(textbox.getAttribute("currentCell")).Column;if(column.FieldLength>0)
{if(textbox.selectionStart!=undefined)
{textbox.oldValue=textbox.value;textbox.oldSelectionStart=textbox.selectionStart;textbox.oldSelectionEnd=textbox.selectionEnd;}
if(textbox.value.length>=column.FieldLength&&!_igtbl_textareaHasSelection(textbox))
{if(evnt&&evnt.preventDefault)
{if(evnt.charCode!=0)
{evnt.preventDefault();}}
return false;}}}
function _igtbl_textareaEditorPaste(evnt){var textbox=igtbl_srcElement(evnt);var grid=igtbl_getGridById(textbox.id.substring(0,textbox.id.length-3));var column=igtbl_getCellById(textbox.getAttribute("currentCell")).Column;if(column.FieldLength>0)
{var text=window.clipboardData.getData("Text");var range=document.selection.createRange();var currentLength=textbox.value.length-range.text.length;if((currentLength+text.length)>column.FieldLength)
{if((column.FieldLength-currentLength)>0)
{range.text=text.substring(0,column.FieldLength-currentLength);}
evnt.returnValue=false;}}}
function _igtbl_textareaEditorInput(evnt){var textbox=igtbl_srcElement(evnt);var grid=igtbl_getGridById(textbox.id.substring(0,textbox.id.length-3));var column=igtbl_getCellById(textbox.getAttribute("currentCell")).Column;if(column.FieldLength>0)
{if(textbox.value.length>column.FieldLength&&textbox.value!=textbox.oldValue&&textbox.value.length>textbox.oldValue.length)
{var beginning=textbox.oldValue.substring(0,textbox.oldSelectionStart);var end=textbox.oldValue.substring(textbox.oldSelectionEnd);var middle=textbox.value.substring(beginning.length,textbox.value.length-end.length);var newLength=column.FieldLength-(beginning.length+end.length);middle=middle.substring(0,newLength);textbox.value=beginning+middle+end;textbox.selectionBegin=beginning.length+middle.length;textbox.selectionEnd=textbox.selectionBegin;}
textbox.removeAttributeNode("oldValue");textbox.removeAttributeNode("selectionStart");textbox.removeAttributeNode("SelectionEnd");}}
function _igtbl_textareaHasSelection(textbox){if(textbox.selectionStart!=undefined)
{if(textbox.selectionEnd>textbox.selectionStart)
{return true;}}
else if(document.selection){var selection=document.selection.createRange();if(selection&&selection.text.length>0)
{return true;}}}
function igtbl_editEvt(evt,gn,id)
{if(evt==null)
if((evt=window.event)==null)
return;var src=evt.srcElement;if(src==null)
if((src=evt.target)==null)
return;if(!gn||!gn.substring)
gn=src.getAttribute("gn");var g=igtbl_getGridById(gn);if(g==null)
return;g.event=evt;var b=g._editorButton;switch(evt.type)
{case"focus":return;case"blur":if(!src.getAttribute("noOnBlur"))
{igtbl_hideEdit(null,evt,g);}
else
ig_cancelEvent(evt);return;case"keydown":src.setAttribute("noOnBlur",true);window.setTimeout("igtbl_cancelNoOnBlurTB('"+gn+"','"+src.id+"')",100);var key=evt.keyCode;if(!(evt.ctrlKey&&key==67))
{ig_cancelEvent(evt);return;}
return;case"keyup":igtbl_fireEvent(gn,g.Events.EditKeyUp,"(\""+gn+"\",\""+src.getAttribute("currentCell")+"\","+evt.keyCode+")");return;}
if(b&&g.ActiveCell&&!igtbl_isFocus(gn,b))
igtbl_showColButton(gn,"hide",true);}
function igtbl_editList(g,v,r,col,cElem)
{var list=col.ValueList;if(list==null||list.length<1)return null;var s,elem=g._editorList;var h=cElem.offsetHeight;if(!ig_csom.IsIE)
{var i=cElem.childNodes.length;while(i-->0)
{var curElem=cElem.childNodes[i];if(!g.getActiveCell().Element.Object.Row.IsFilterRow||curElem.tagName!="BUTTON")
{if(curElem.style)
curElem.style.display="none";else
{if(g._oldElems==null)
g._oldElems=new Array();g._oldElems[g._oldElems.length]=curElem;cElem.removeChild(curElem);}}}}
if(elem==null)
{g._editorList=elem=document.createElement("SELECT");if(!ig_csom.IsIE)
cElem.appendChild(elem);else
{document.body.insertBefore(elem,document.body.firstChild);elem.style.position="absolute";}
ig_csom.addEventListener(elem,"keydown",igtbl_editEvt);ig_csom.addEventListener(elem,"keyup",igtbl_editEvt);ig_csom.addEventListener(elem,"blur",igtbl_editEvt);ig_csom.addEventListener(elem,"mousedown",igtbl_listMouseDown);elem.style.zIndex=r.z;elem.setAttribute("editType",3);elem.id=g.Id+"_vl";ig_csom.addEventListener(elem,"change",igtbl_dropDownChange);}
else if(!ig_csom.IsIE)
cElem.appendChild(elem);s=elem.style;var opt,css=col.ValueListClass;if(ig_csom.notEmpty(css))
elem.className=css;else
elem.style.fontSize=((r.h-=4)<15)?(((r.h<9)?9:r.h)+"px"):"";elem.value=v;var i=elem.options.length,prompt=col.ValueListPrompt;while(i-->0)
elem.removeChild(elem.options[i]);if(ig_csom.notEmpty(prompt))
{elem.appendChild(opt=document.createElement("OPTION"));opt.text=opt.value=prompt;}
while(++i<list.length)if(list[i]!=null)
{elem.appendChild(opt=document.createElement("OPTION"));opt.value=list[i][0];opt.text=list[i][1];if(v!=null&&(v==list[i][0])){opt.selected=true;v=null;}}
s.display="";s.visibility="hidden";var cStyle,eStyle;if(ig_csom.IsIE)
{eStyle=elem.currentStyle;cStyle=cElem.currentStyle;}
else
{eStyle=igtbl_getComputedStyle(elem);cStyle=igtbl_getComputedStyle(cElem);}
if(g.IsXHTML)
{var xd,yd,wd,hd;if(ig_csom.IsIE)
{yd=Math.floor((igtbl_parseInt(cStyle.borderTopWidth)+igtbl_parseInt(cStyle.borderBottomWidth)+igtbl_parseInt(eStyle.borderTopWidth)+igtbl_parseInt(eStyle.borderBottomWidth))/2);}
else
{r.x--;r.y--;if(ig_csom.IsFireFox)
{r.y-=g.DivElement.scrollTop;r.x-=g.DivElement.scrollLeft;}
var pd=igtbl_parseInt(eStyle.paddingLeft);if(pd)
r.w-=pd;pd=igtbl_parseInt(eStyle.paddingRight);if(pd)
r.w-=pd;pd=igtbl_parseInt(eStyle.paddingTop);if(pd)
r.h-=pd;pd=igtbl_parseInt(eStyle.paddingBottom);if(pd)
r.h-=pd;pd=igtbl_parseInt(cStyle.paddingLeft);if(pd)
r.w-=pd;pd=igtbl_parseInt(cStyle.paddingRight);if(pd)
r.w-=pd;pd=igtbl_parseInt(cStyle.paddingTop);if(pd)
r.h-=pd;pd=igtbl_parseInt(cStyle.paddingBottom);if(pd)
r.h-=pd;r.h+=3;if(cElem.offsetHeight>h)
r.h-=2;}
xd=igtbl_parseInt(eStyle.borderLeftWidth);if(xd)
r.x-=Math.floor(xd/2);if(yd)
r.y+=yd;wd=igtbl_parseInt(cStyle.borderLeftWidth)+igtbl_parseInt(cStyle.borderRightWidth);if(wd&&ig_csom.IsIE)
r.w+=wd;wd=igtbl_parseInt(eStyle.borderLeftWidth);if(wd&&ig_csom.IsIE)
r.w+=wd;if(hd)
r.h+=hd;}
if(ig_csom.IsIE)
{s.left="0px";s.top="0px";}
s.left=r.x+"px";s.width=r.w+"px";if(!(g.IsXHTML&&ig_csom.IsIE7))
{s.height=(cElem.offsetHeight-igtbl_parseInt(cStyle.borderTopWidth)-igtbl_parseInt(cStyle.borderBottomWidth))+"px";}
var so=igtbl_getStyleSheet(elem.className);if(so&&so.verticalAlign=="top")
s.top=r.y+"px";else if(so&&so.verticalAlign=="bottom")
s.top=r.y+r.h-elem.offsetHeight+"px";else
s.top=r.y+r.h/2-elem.offsetHeight/2+"px";s.visibility="visible";elem.focus();return elem;}
function igtbl_listMouseDown(evnt)
{if(!evnt&&event)evnt=event;if(!evnt)return;var list=igtbl_srcElement(evnt);list.setAttribute("noOnBlur","true");window.setTimeout("igtbl_clearNoOnBlurElem('"+list.id+"')",100);}
function igtbl_activate(gn)
{var x=0;var g=igtbl_getGridById(gn);if(g==null)
return;if(g._editorCurrent!=null)
{try
{if(g._editorCurrent.select)
{g._editorCurrent.select();}
g._editorCurrent.focus();return;}
catch(e){;}}
var b=igtbl_initButton(g),elem=g._focusElem;if(ig_csom.IsFireFox&&elem)
{var elem1=document.createElement("INPUT");document.body.insertBefore(elem1,document.body.firstChild);document.body.removeChild(elem1);setTimeout("var elem1=document.createElement(\"INPUT\");"+" document.body.insertBefore(elem1,document.body.firstChild); "+" document.body.removeChild(elem1);",0);}
if(elem==null)
try
{elem=document.createElement("INPUT");document.body.insertBefore(elem,document.body.firstChild);elem.setAttribute("gn",gn);ig_csom.addEventListener(elem,"keydown",igtbl_inputEvt);ig_csom.addEventListener(elem,"keyup",igtbl_inputEvt);ig_csom.addEventListener(elem,"focus",igtbl_inputEvt);ig_csom.addEventListener(elem,"blur",igtbl_inputEvt);ig_csom.addEventListener(document.body,"mouseup",igtbl_globalMouseUp);g._focusElem=elem;var s=elem.style;s.zIndex=-1;s.position="absolute";s.fontSize="2px";s.padding=s.width=s.height=s.border="0px";if(elem.offsetWidth>2)
s.width="1px";if(elem.offsetHeight>2)
s.height="1px";elem.tabIndex=g.DivElement.getAttribute("tabIndexPage");}
catch(ex){;}
if(elem==null)
return;g._lastKey=0;if(!igtbl_isOk(g))
return;igtbl_showColButton(gn,"hide",true);var cell=g.oActiveCell;if(cell!=null)
cell=cell.Element;if(cell==null)
cell=g.Element;var r=igtbl_getAbsBounds(cell,g,true);elem.style.left=(r.x-3)+"px";elem.style.top=(r.y-3)+"px";if(g._focus0)
return;if(g._mouseDown==1)
{g._mouseDown=0;try
{window.setTimeout("try{igtbl_isFocus('"+gn+"',null,true);}catch(ex){;}",0);}catch(ex){;}}
else
igtbl_isFocus(gn);}
function _igtbl_processUpdates(g,se)
{var ar=g.getActiveRow();if(ar&&!igtbl_inEditMode(g.Id)&&!igtbl_isAChildOfB(se,g.MainGrid))
{var retmpl=igtbl_getElementById(ar.Band.RowTemplate);if(!retmpl||!igtbl_isAChildOfB(se,retmpl))
{var combo;if(typeof(igcmbo_getComboByElement)!="undefined")combo=igcmbo_getComboByElement(se);if(!combo||!igtbl_isAChildOfB(combo.Element,retmpl))
{if(ar.IsAddNewRow)
ar.commit();else if(ar.processUpdateRow)
ar.processUpdateRow();}}}}
function igtbl_globalMouseUp(evt,gn)
{var g=igtbl_getGridById(gn?gn:igtbl_lastActiveGrid);if(!g||typeof(document)=="undefined"||(ig_csom.IsIE&&typeof(document.body)=="undefined"))return;var se=igtbl_srcElement(evt);_igtbl_processUpdates(g,se);var resizeDiv=document.body.igtbl_resizeDiv;if(resizeDiv)resizeDiv.style.display="none";g.Element.removeAttribute("mouseDown");}
function igtbl_isOk(g)
{var vis=true,e=g.Element;while((e=e.parentNode)!=null)
if(e.tagName!=(igtbl_isXHTML?"HTML":"BODY")&&e.style!=null&&e.style.display=="none")
vis=false;if((e=g._focusElem)!=null)if((e.style.display=="none")==vis)e.style.display=vis?"":"none";return vis;}
function igtbl_isFocus(gn,b,foc)
{var g=igtbl_getGridById(gn);if(g==null)
return false;if(b)
return g.oActiveCell!=null&&g.oActiveCell.Element.id==b.getAttribute("srcElement");var ae=g.oActiveCell;if(!ae)
ae=g.oActiveRow;var nn=null;var activeElement=null;try{activeElement=document.activeElement;if(!activeElement&&igtbl_browserWorkarounds.activeElement)
activeElement=igtbl_browserWorkarounds.activeElement;if((!activeElement||activeElement==document.body)&&g.event)
activeElement=igtbl_srcElement(g.event);if(activeElement)
nn=activeElement.nodeName;}catch(e){;}
var inputSelectButton=(nn=="INPUT"||nn=="TEXTAREA"||nn=="SELECT"||nn=="BUTTON");var internalObj=(activeElement!=null&&inputSelectButton&&(!ae||!ae.Column||(ae.Column.TemplatedColumn&2))&&(ig_isAChildOfB(activeElement,g.Element)||ig_isAChildOfB(activeElement,g.MainGrid)));var specObj=activeElement&&typeof(activeElement.getAttribute)=="function"&&activeElement.getAttribute("igtbl_active");var insideEl=(ae&&activeElement&&ig_isAChildOfB(activeElement,ae.Element))&&inputSelectButton&&(!ae.Column||(ae.Column.TemplatedColumn&2))||inputSelectButton&&activeElement&&activeElement.parentNode&&ig_isAChildOfB(activeElement,igtbl_getElementById(g.UniqueID+"_pager"));var templOpen=ae&&ae.Band.RowTemplate&&igtbl_getElementById(ae.Band.RowTemplate).style.display=="";if(g._editorCurrent==null&&!insideEl&&!templOpen&&!internalObj&&!specObj&&!(ig_csom.IsFireFox&&ae&&ae.Column&&(ae.Column.TemplatedColumn&2)&&ae.Element.childNodes.length>0))
try
{if(foc||nn!='DIV'||!activeElement||''+activeElement.contentEditable!='true')
g._focusElem.focus();}
catch(ex){;}
return false;}
function igtbl_initButton(g)
{var b=g._editorButton;if(!b)
if((g._editorButton=b=igtbl_getElementById(g.Id+"_bt"))!=null)
{if(b.parentNode!=document.body)
{var parentNode=b.parentNode;parentNode.removeChild(b);document.body.insertBefore(b,document.body.firstChild);}
b.unselectable="on";b.tabIndex=-1;b.hideFocus=true;b.setAttribute("gn",g.Id);g._mouseWait=0;ig_csom.addEventListener(b,"mouseout",igtbl_editEvt);}
if(b)
b.style.zIndex=g._getZ(99999);return b;}
function igtbl_inputEvt(evt,gn)
{if(gn!=null)
{var g=igtbl_getGridById(gn);if(g==null)
return;if(evt==null)
{g._focus=g._focus0;return;}
if(!g._focus)
return;var e=new Object();e.shiftKey=evt==1;e.ctrlKey=evt==2;e.keyCode=9;if(g.getActiveCell()==null&&g.getActiveRow()==null)
{if(g.Rows!=null)
if((e=g.Rows.getRow(0))!=null)
if((e=e.getCell(0))!=null)
e.activate();return;}
igtbl_onKeyDown(e,gn,true);return;}
if(evt==null)
if((evt=window.event)==null)
return;var src=evt.srcElement;if(src==null)
if((src=evt.target)==null)
return;if(typeof(src.getAttribute)=="undefined")
return;var g=igtbl_getGridById(gn=src.getAttribute("gn")),key=evt.keyCode;if(g==null)
return;switch(evt.type)
{case"focus":if(!igtbl_isOk(g))
{ig_cancelEvent(evt);return;}
g._focus0=g._focus=true;break;case"blur":g._focus0=false;try
{window.setTimeout("try{igtbl_inputEvt(null,'"+gn+"');}catch(ex){;}",0);}
catch(ex)
{g._focus=false;}
break;case"keydown":var click=false,ac=g.oActiveCell,b=g._editorButton;if(ac&&(key==32||key==13))
click=b?igtbl_isFocus(gn,b):(ac.Column.ColumnType==7&&ac.Column.CellButtonDisplay==1);g._mouseDown=0;if(click)
igtbl_colButtonClick(evt,gn,b,ac.Element);if(key==9)
try
{if(src.tagName=="INPUT"&&src.type=="checkbox")
{igtbl_processTab(gn,evt,key,igtbl_getCellById(src.getAttribute("cellId")));ig_cancelEvent(evt);break;}
src.removeAttribute("noOnBlur");if(g.oActiveCell&&g.oActiveCell.getNextTabCell(evt.shiftKey,true)||g.oActiveRow&&g.oActiveRow.getNextTabRow(evt.shiftKey))
{ig_cancelEvent(evt);window.setTimeout("try{igtbl_inputEvt("+(evt.shiftKey?"1":(evt.ctrlKey?"2":"0"))+",'"+gn+"');}catch(ex){;}",10);break;}}
catch(ex){;}
else if(key==13||key==27)
{src.removeAttribute("noOnBlur");if(igtbl_inEditMode(g.Id)&&!igtbl_hideEdit(null,evt,g))
{if(key!=27)
ig_cancelEvent(evt);return;}}
if(typeof(igtbl_onKeyDown)!="undefined")
igtbl_onKeyDown(evt,gn,true);break;case"keyup":if(typeof(igtbl_onKeyUp)!="undefined")
igtbl_onKeyUp(evt,gn);break;}}
function igtbl_hideEdit()
{var oEvent=null,g=null,i=arguments.length,gn=arguments[0];if(i==1&&(gn!=null&&gn.substring))
g=igtbl_getGridById(gn);if(i>2)
{oEvent=arguments[i-2];g=arguments[i-1];}
if(g==null)
return false;var evt=oEvent,elem=g._editorCurrent,key=g._lastKey;if(i==1)
{if(key==114)return false;if(key==9)
try
{window.setTimeout("try{igtbl_activate('"+gn+"');}catch(ex){;}",10);return false;}
catch(ex){;}
if(key==9||key==13||key==27)
{igtbl_activate(gn);return false;}
evt=null;}
if(elem==null||elem.getAttribute("noOnBlur"))
return false;var oEditor=(i==1||gn==null)?g._editorCustom:gn;if(oEditor&&oEditor._inArrowKeyNavigation)
return;gn=g.Id;if(oEditor!=null&&evt!=null)
evt=evt.event;key=-3;if(evt!=null&&evt.type=="keydown")
g._lastKey=key=evt.keyCode;if(evt!=null)
if((key==13&&(evt.shiftKey||evt.ctrlKey))||(key!=-3&&key!=9&&key!=13&&key!=27&&key!=113))
return false;var cell=igtbl_getElementById(elem.getAttribute("currentCell"));if(cell==null)
return false;var cellObj=igtbl_getCellById(cell.id);if(cellObj==null)
return false;var type=elem.getAttribute("editType"),v=(oEditor!=null)?oEditor.getValue():elem.value;var j,colV,pgV=null,valid=(typeof(Page_Validators)!="object"||cellObj.Row.IsFilterRow);if(!valid)
{valid=true;colV=cellObj.Column.Validators;pgV=Page_Validators;for(j=0;j<colV.length;j++)for(i=0;i<pgV.length;i++)if(pgV[i].id==colV[j])
{ValidatorValidate(pgV[i]);if(!pgV[i].isvalid)valid=false;}
if(!valid)
{var de=g.getDivElement();de.setAttribute("noOnScroll","true");de.setAttribute("oldSL",de.scrollLeft.toString());de.setAttribute("oldST",de.scrollTop.toString());}
else
igtbl_cancelNoOnScroll(gn);}
if(!valid&&typeof(Page_Validators)!="undefined")
{ValidatorUpdateIsValid();}
elem.setAttribute("noOnBlur",true);if(!valid||g.fireEvent(g.Events.BeforeExitEditMode,[gn,cell.id,v])==true)
{window.setTimeout("igtbl_clearNoOnBlurElem('"+elem.id+"')",100);if(!g._exitEditCancel&&!g._insideSetActive)
{g._insideSetActive=true;igtbl_setActiveCell(gn,cell);g._insideSetActive=false;}
g._exitEditCancel=true;return false;}
elem.removeAttribute("noOnBlur");if(pgV!=null)for(i=0;i<pgV.length;i++)for(j=0;j<colV.length;j++)
if(pgV[i].id==colV[j]&&pgV[i].enabled)
ValidatorEnable(pgV[i],false);elem.removeAttribute("currentCell");g._editorCustom=g._editorCurrent=null;var equalsPrompt=false;if(oEditor!=null)
{oEditor.setVisible(false);v=oEditor.getValue();oEditor.removeEventListener("blur",igtbl_hideEdit);oEditor.removeEventListener("keydown",igtbl_hideEdit);if(key==27)
window.setTimeout("try{igtbl_activate('"+gn+"');}catch(ex){;}",1);}
else
{elem.style.display="none";if(type==0||type==1||type==3)
{if(elem.style.position!="absolute")
{var p=elem.parentNode;p.removeChild(elem);var i,nn=p.childNodes;if(nn!=null)for(i=0;i<nn.length;i++)
if(nn[i].style!=null)nn[i].style.display="";i=((nn=g._oldElems)==null)?0:nn.length;while(i-->0)cell.appendChild(nn[i]);cell.style.width=cell.style.height="";}
if(type==3)
{if(!(cellObj.Column.AllowNull&&cellObj.Column.ValueListPrompt==cellObj.Column.getNullText()&&cellObj.Column.ValueListPrompt==elem.options[elem.selectedIndex].value||elem.options[elem.selectedIndex].value!=cellObj.Column.ValueListPrompt))
equalsPrompt=true;}}}
g._oldElems=null;g._exitEditCancel=false;if(key!=27&&cellObj._oldValue!==v&&!equalsPrompt)
cellObj.setValue(v);igtbl_fireEvent(gn,g.Events.AfterExitEditMode,"(\""+gn+"\",\""+cell.id+"\");");if(g.NeedPostBack)
{igtbl_doPostBack(gn);return true;}
if(key==9||key==13)
igtbl_processTab(gn,evt,key,cellObj);return true;}
function igtbl_processTab(gn,evt,key,cellObj)
{var g=igtbl_getGridById(gn);var start=null;if(typeof igtbl_ActivateNextCell=="function")
{var oldAc=g.oActiveCell;if(key==9&&evt.shiftKey)
start=igtbl_ActivatePrevCell(gn);else
start=igtbl_ActivateNextCell(gn);if(!start&&cellObj.Row.Band.getCellClickAction()==2)
{start=cellObj.getNextTabCell(evt.shiftKey);}
else if(!start)
{if(oldAc)
{if(oldAc.Row.IsAddNewRow)
{oldAc.Row.commit();var nac=oldAc.Row.getCell(0);while(nac&&!nac.Column.getVisible())
nac=oldAc.Row.getCell(nac.Column.Index+1);if(nac)
{nac.activate();nac.scrollToView();}}
else
{if(oldAc.Row.processUpdateRow)
oldAc.Row.processUpdateRow()
if(key==9&&evt.shiftKey)
start=igtbl_ActivatePrevCell(gn);else
start=igtbl_ActivateNextCell(gn);}}}
else if(g.oActiveCell&&oldAc&&oldAc.Row!=g.oActiveCell.Row&&oldAc.Row.IsAddNewRow&&(oldAc.Row.Band.Index>0&&oldAc.Row.Band.AddNewRowView==2||oldAc.Row.Band.AddNewRowView==1))
{var nac=oldAc.Row.getCell(0);while(nac&&!nac.Column.getVisible())
nac=oldAc.Row.getCell(nac.Column.Index+1);if(nac)
{nac.activate();nac.scrollToView();}}
if(!start)
delete g._lastKey;else if(evt!=null)
ig_cancelEvent(evt);}
if(start&&key==9&&igtbl_getCellClickAction(gn,cellObj.Column.Band.Index)==2)
{if(g.oActiveRow!=start.Row)
{start.Row.activate();if(start.Row.Band.getSelectTypeRow()==2)
start.Row.setSelected(true);}
start.beginEdit()}
else if(start&&key==9&&igtbl_getCellClickAction(gn,cellObj.Column.Band.Index)==1)
try
{window.setTimeout("try{igtbl_EnterEditMode('"+gn+"');}catch(ex){;}",100);}
catch(ex)
{igtbl_EnterEditMode(gn);}
else
igtbl_activate(gn);}
function igtbl_getOffsetX(evnt,e)
{if(ig_csom.IsIE)
return evnt.offsetX;else if(ig_csom.IsFireFox)
return(evnt.clientX+window.scrollX)-igtbl_getLeftPos(e);else
return evnt.clientX-igtbl_getLeftPos(e);}
function igtbl_getOffsetY(evnt,e)
{if(ig_csom.IsIE)
return evnt.offsetY;else
return evnt.clientY-igtbl_getTopPos(e);}
function igtbl_onResize(gn)
{if(typeof(igtbl_getGridById)=="undefined"||!ig_csom.IsIE55Plus)return;var gs=igtbl_getGridById(gn);if(!gs||!gs.isLoaded())return;var div=gs.Element.parentNode;if(!div||div.nodeName=="#document-fragment")return;var adjHeight=0;if(gs._scrElem)
{div=gs._scrElem;if(gs.MainGrid&&!gs.MainGrid.style.height)
adjHeight=div.scrollHeight-div.clientHeight;}
var oldX=div.getAttribute("oldXSize");var oldY=div.getAttribute("oldYSize");var oldTop=div.getAttribute("oldTop");var oldLeft=div.getAttribute("oldLeft");var elTop=igtbl_getTopPos(gs.Element);var elLeft=igtbl_getLeftPos(gs.Element);if(oldX==null)
{div.setAttribute("oldXSize",div.offsetWidth);div.setAttribute("oldYSize",div.offsetHeight);div.setAttribute("oldTop",elTop);div.setAttribute("oldLeft",elLeft);gs.alignStatMargins();gs.alignDivs(0,true);if(gs.StatHeader&&(gs.UseFixedHeaders||gs.XmlLoadOnDemandType!=0&&gs.XmlLoadOnDemandType!=4))
gs.StatHeader.ScrollTo(div.scrollLeft);return;}
if(oldX==div.offsetWidth&&oldY==div.offsetHeight+adjHeight&&oldTop==elTop&&oldLeft==elLeft)
return;div.setAttribute("oldXSize",div.offsetWidth);div.setAttribute("oldYSize",div.offsetHeight);div.setAttribute("oldTop",elTop);div.setAttribute("oldLeft",elLeft);if(gs.Element.getAttribute("noOnResize"))return;igtbl_hideEdit(gn);gs.alignStatMargins();gs.alignDivs(0,true);if(gs.StatHeader&&(gs.UseFixedHeaders||gs.XmlLoadOnDemandType!=0&&gs.XmlLoadOnDemandType!=4))
gs.StatHeader.ScrollTo(div.scrollLeft);gs.endEditTemplate();}
function igtbl_isDisabled(elem)
{if(!elem)return false;if(ig_csom.IsIE55Plus)
return elem.disabled;return elem.getAttribute("disabled")&&elem.getAttribute("disabled").toString()=="true";}
function igtbl_setDisabled(elem,b)
{if(!elem)
return;if(ig_csom.IsIE55Plus)
elem.disabled=b;else
{elem.setAttribute("disabled",b);if(b)
{if(typeof(elem.getAttribute("oldColor"))!="string"&&elem.style.color!="graytext")
elem.setAttribute("oldColor",elem.style.color);elem.style.color="graytext";}
else
{if(typeof(elem.getAttribute("oldColor"))=="string")
{elem.style.color=elem.getAttribute("oldColor");elem.removeAttribute("oldColor");}
else
elem.style.color="";}}}
function igtbl_button(gn,evnt)
{if(document.all)
{if(evnt.button==1)return 0;else if(evnt.button==4)return 1;else if(evnt.button==2)return 2;return-1;}
if(evnt.button==0&&gn)
{if(evnt.detail!=0)return 0;var gs=igtbl_getGridById(gn);if(gs.Element.getAttribute("mouseDown"))return 0;else return-1;}
else if(evnt.button==1)return 1;else if(evnt.button==2)return 2;return-1;}
function igtbl_srcElement(evt)
{var e=evt.srcElement;if(!e)e=evt.target;while(e&&!e.tagName)e=e.parentNode;return e;}
function igtbl_styleName(sn)
{var r=sn.toLowerCase();var sa=r.split("-");for(var i=1;i<sa.length;i++)
sa[i]=sa[i].charAt(0).toUpperCase()+sa[i].substr(1);r=sa.join("");return r;}
function igtbl_hasClassName(e,cn)
{return e.className.indexOf(cn)!=-1;}
function igtbl_setClassName(e,cn)
{var i=e.className.indexOf(cn);if(i==-1)
e.className+=(e.className.length==0?"":" ")+cn;}
function igtbl_removeClassName(e,cn)
{var i=e.className.indexOf(cn);if(i>=0)
{var leftPart="";var rightPart="";if(i>0)
{leftPart=e.className.substr(0,i);if(leftPart.substr(leftPart.length-1)==" ")
leftPart=leftPart.substr(0,leftPart.length-1);}
if(i+cn.length<e.className.length)
rightPart=e.className.substr(i+cn.length);e.className=leftPart+rightPart;}}
function igtbl_changeStyle(gn,se,style)
{var appldStyle=se.getAttribute("newClass");if(!style)
{if(appldStyle)
igtbl_removeClassName(se,appldStyle);se.removeAttribute("newClass");return;}
else
{var styleToApply=style;if(styleToApply==appldStyle)
return;if(appldStyle)
igtbl_changeStyle(gn,se,null);igtbl_setClassName(se,styleToApply);se.setAttribute("newClass",styleToApply);}}
function igtbl_initEvent(se){this.srcElement=this.target=se;}
function igtbl_adjustLeft(e){return document.all?igtbl_getLeftPos(e):0;}
function igtbl_adjustTop(e){return document.all?igtbl_getTopPos(e):0;}
function igtbl_clientWidth(e)
{var cw=e.clientWidth;if(!cw)
{cw=e.offsetWidth;if(e.scrollWidth)if(e.scrollWidth>cw)cw-=13;}
return(cw>0)?cw:0;}
function igtbl_clientHeight(e)
{var ch=e.clientHeight;if(!ch)
{ch=e.offsetHeight;if(e.scrollHeight)if(e.scrollHeight>ch)ch-=13;}
return(ch>0)?ch:0;}
function igtbl_getInnerText(elem)
{if(!elem)return"";if(elem.nodeName=="#text"){return elem.nodeValue;}
var txt="",nn=elem.childNodes;if(ig_csom.IsIEWin)try{return elem.innerText;}catch(ex){;}
if(elem.nodeName=="#text")txt=elem.nodeValue;else if(elem.nodeName=="BR")txt="\r\n";else if(nn)for(var i=0;i<nn.length;i++)txt+=igtbl_getInnerText(nn[i]);var sp=String.fromCharCode(160);while(txt.indexOf(sp)>=0)txt=txt.replace(sp," ");return txt;}
function igtbl_setInnerText(elem,txt,wrap)
{if(!elem)return;if(elem.nodeName=="#text")
{elem.nodeValue=txt;return;}
txt=(txt&&txt!="")?txt.toString():" ";if(ig_csom.IsIEWin)try{elem.innerText=txt;return;}catch(ex){;}
while(txt.indexOf("\r")>=0)txt=txt.replace("\r","");while(!wrap&&txt.indexOf(" ")>=0)txt=txt.replace(" ",String.fromCharCode(160));var te=null,ss=txt.split("\n"),nn=elem.childNodes;var j=-1,i=nn.length;while(i-->0)
{if(!te&&nn[i]&&nn[i].nodeName=="#text"){te=nn[i];te.nodeValue=te.data=ss[++j];}
if(nn[i]!=te)elem.removeChild(nn[i]);}
while(++j<ss.length)
{if(j>0)elem.appendChild(document.createElement("BR"));try{elem.appendChild(document.createTextNode(ss[j]));}catch(ex){;}}}
function igtbl_showColButton(gn,se,active)
{var gs=igtbl_getGridById(gn);if(!gs||se==null)return;var b=igtbl_initButton(gs),cell=gs.oActiveCell;if(!b)return;if(se=="hide")
{gs._mouseWait=0;gs._mouseIn=null;if(active&&cell&&cell.hasButtonEditor(igtbl_cellButtonDisplay.OnMouseEnter))
{if(b.getAttribute("srcElement")==cell.Element.id)return;try{window.setTimeout("try{igtbl_showColButton('"+gn+"','act');}catch(e){}",20);}catch(e){;}}
if(b.style.display=="")b.style.display="none";return;}
if(se=="act")
{if(!cell||(cell.Row.ParentRow&&!cell.Row.ParentRow.getExpanded())||b.style.display=="")return;se=cell.Element;}
igtbl_scrollToView(gn,se);var bandNo=null;var pNode=se;while(bandNo===null&&(pNode=pNode.parentNode)!=null)
{bandNo=pNode.getAttribute("bandNo");}
var columnNo=igtbl_getColumnNo(gn,se);var column=gs.Bands[bandNo].Columns[columnNo];b.style.width=igtbl_clientWidth(se)+"px";b.style.height=igtbl_clientHeight(se)+"px";if(ig_shared.IsIE)
{{var testValue=igtbl_getAbsBounds(se,gs,true);b.style.left=testValue.x+"px";b.style.top=testValue.y+"px";}}
else
{ig_csom.absPosition(se,b,ig_Location.MiddleCenter,null);}
b.className=column.ButtonClass;if(se.innerHTML==igtbl_getNullText(gn,bandNo,columnNo))
b.value=" ";else if(se.firstChild.tagName=="NOBR")
b.value=igtbl_getInnerText(se.firstChild);else
b.value=igtbl_getInnerText(se);b.setAttribute("srcElement",se.id);b.style.display="";}
function igtbl_getDocumentElement(elemID)
{if(ig_shared.IsIE)
{var obj;if(document.all)
obj=document.all[elemID];else
obj=document.getElementById(elemID);return obj;}
else
{var elem=document.getElementById(elemID);if(elem)
{var elems=document.getElementsByTagName(elem.tagName);var els=[];for(var i=0;i<elems.length;i++)
{if(elems[i].id==elemID)
els[els.length]=elems[i];}
return(els&&els.length==1)?els[0]:els;}
return null;}}
function igtbl_onScroll(evnt,gn)
{var gs=igtbl_getGridById(gn);if(!gs)return;var de=gs.getDivElement();if(de.getAttribute("noOnScroll"))
{if(de.getAttribute("oldSL"))
igtbl_scrollLeft(de,parseInt(de.getAttribute("oldSL")));if(de.getAttribute("oldST"))
igtbl_scrollTop(de,parseInt(de.getAttribute("oldST")));return igtbl_cancelEvent(evnt);}
if(!igtbl_hideEdit(gn)&&gs._exitEditCancel)
{var activeCell=gs.getActiveCell();if(activeCell)
{gs._returnToEditModeFromScroll=true;igtbl_editCell(evnt,gn,activeCell.Element);gs._returnToEditModeFromScroll=null;return;}}
igtbl_showColButton(gn,"hide");if(gs.FixedColumnScrollType!=2)
gs.alignStatMargins();gs.endEditTemplate();var isVertScroll=(typeof(gs._oldScrollTop)!="undefined"&&gs._oldScrollTop!=de.scrollTop||typeof(gs._oldScrollTop)=="undefined"&&de.scrollTop>0);if(gs.Node&&!gs.AllowPaging&&(gs.RowsServerLength>gs.Rows.length&&gs.XmlLoadOnDemandType!=2||gs.XmlLoadOnDemandType==2)&&isVertScroll)
igtbl_onScrollXml(evnt,gn);if(gs.UseFixedHeaders)
{if(typeof(gs.fhOldScrollLeft)=="undefined"&&typeof(gs.fhOldScrollTop)=="undefined"||gs.fhOldScrollLeft!=gs._scrElem.scrollLeft||gs.fhOldScrollTop!=gs._scrElem.scrollTop)
{gs.fhOldScrollLeft=gs._scrElem.scrollLeft;gs.fhOldScrollTop=gs._scrElem.scrollTop;if(gs.FixedColumnScrollType==2)
{if(gs.alignDivsTimeoutID)
window.clearTimeout(gs.alignDivsTimeoutID);gs.alignDivsTimeoutID=window.setTimeout("igtbl_doAlignDivs('"+gn+"')",250);}
else
gs.alignDivs();}}
else if(gs.XmlLoadOnDemandType==1||gs.XmlLoadOnDemandType>2||!isVertScroll)
{gs.alignDivs();}
gs._oldScrollLeft=de.scrollLeft;gs._oldScrollTop=de.scrollTop;gs._removeChange("ScrollLeft",gs);gs._recordChange("ScrollLeft",gs,de.scrollLeft);gs._removeChange("ScrollTop",gs);gs._recordChange("ScrollTop",gs,de.scrollTop);}
function igtbl_doAlignDivs(gn,force)
{var gs=igtbl_getGridById(gn);gs.alignDivsTimeoutID=null;gs.alignStatMargins();gs.alignDivs(0,force);}
function igtbl_filterMouseUp(evt)
{var src=ig_csom.IsIE?evt.srcElement:evt.target;while(src&&!src.getAttribute("filter"))
{src=src.parentNode;}
var filterDropObject=src.object;if(filterDropObject)
filterDropObject.show(false);return ig_cancelEvent(evt);}
function igtbl_filterMouseOver(evt)
{var src=ig_csom.IsIE?evt.srcElement:evt.target;while(src&&src.tagName!="TR")
{if(src.tagName=="DIV"&&(src.getAttribute("filter")||src.getAttribute("filterIconList")))
return;src=src.parentNode;}
if(src)
{var srcDiv=src;while(srcDiv&&!(srcDiv.getAttribute("filter")||srcDiv.getAttribute("filterIconList")))
{srcDiv=srcDiv.parentNode;}
if(srcDiv)
{var filterDropObject=srcDiv.object;if(filterDropObject)
{if(src.tagName=="TR")
src=src.childNodes[0];src.setAttribute("oldStyle",src.className);src.className=filterDropObject.getHighlightStyle()+" "+src.className;}}}}
function igtbl_filterMouseOut(evt)
{var src=ig_csom.IsIE?evt.srcElement:evt.target;while(src&&src.tagName!="TD")
{src=src.parentNode;}
if(src)
{var oldStyle=src.getAttribute("oldStyle");src.className=oldStyle?oldStyle:"";}}
function igtbl_filterMouseUpDocument(evt)
{for(var gridId in igtbl_gridState)
{var g=igtbl_getGridById(gridId);if(g._currentFilterDropped)
g._currentFilterDropped.show(false);}}
var igtbl_filterRequester;function igtbl_stringEditorKeyPress(evt)
{if(igtbl_filterRequester)
{window.clearTimeout(igtbl_filterRequester);igtbl_filterRequester=null;}
var src=evt.srcElement?evt.srcElement:evt.target;if(src)
{var cell=igtbl_getCellById(src.getAttribute("currentCell"));if(cell.Column.AllowRowFiltering==3&&cell.Column.Band.Grid.LoadOnDemand!=3)
{return;}
if(cell.Column.DataType==8)
igtbl_filterRequester=window.setTimeout("igtbl_filterRequest(\""+src.getAttribute("currentCell")+"\",\""+src.value+"\",\""+src.id+"\")",1000);}}
function igtbl_filterRequest(cellId,editorValue,srcId)
{igtbl_filterRequester=null;var oCell=igtbl_getCellById(cellId);if(oCell.Row.IsFilterRow)
{var columnFilter=oCell.Column._getFilterPanel(oCell.Row.Element);var filterOp=parseInt(oCell._getFilterTypeImage().getAttribute("operator"));if(editorValue!=null&&editorValue!=""&&oCell.Column.DataType==8)
{var re=new RegExp("^\\s+");editorValue=editorValue.replace(re,"");}
if(editorValue==null||editorValue=="")filterOp=igtbl_filterComparisionOperator.All;var g=oCell.Row.Band.Grid;var curEditor=g._editorCurrent;if(curEditor)
{curEditor.setAttribute("noOnBlur",true);window.setTimeout("igtbl_clearNoOnBlurElem('"+curEditor.id+"')",100);}
columnFilter.setFilter(filterOp,editorValue);columnFilter.applyFilter();window.setTimeout("_realignFilterRowEditor(\""+g.UniqueID+"\",\""+oCell.Id+"\")",250);}}
function _realignFilterRowEditor(gn,cellId)
{var g=igtbl_getGridById(gn);var oCell=igtbl_getCellById(cellId);var curEditor=g._editorCurrent;if(curEditor&&oCell&&curEditor.getAttribute("currentCell")==oCell.Id)
{var cell=oCell.getElement();var r=_igtbl_determineEditorPosition(g,g.UniqueID,oCell,cell,cell);curEditor.style.left=r.x+"px";curEditor.style.top=r.y+"px";}}
function igtbl_replaceChild(parent,newChild,oldChild)
{try
{parent.replaceChild(newChild,oldChild);}
catch(exc)
{var sibling=oldChild.nextSibling;parent.removeChild(oldChild);if(sibling)
parent.insertBefore(newChild,sibling);else
parent.appendChild(newChild);}}
function igtbl_getComputedStyle(elem)
{if(elem.currentStyle)
{return elem.currentStyle;}
else if(document.defaultView&&document.defaultView.getComputedStyle)
{return document.defaultView.getComputedStyle(elem,"");}
return null;}
function igtbl_sortGrid()
{if(this.Rows.Node&&this.LoadOnDemand==3)
this.Rows.sortXml();else
this.Rows.sort();}
function igtbl_columnCompareRows(row1,row2)
{if(!row1.GroupByRow||!row2.GroupByRow)
return;var res=0;var v1=row1.Value;var v2=row2.Value;if(v1!=null||v2!=null)
{switch(this.DataType)
{case 8:{if(ig_csom.IsIE55Plus||ig_csom.IsNetscape6||(v1&&v1.localeCompare))
{if(v1==null&&v2!=null)
res=-1;else if(v1!=null&&v2==null)
res=1;else
res=v1.localeCompare(v2);}
else
if(v1==null&&v2!=null)
res=-1;else if(v1!=null&&v2==null)
res=1;else if(v1<v2)
res=-1;else if(v1>v2)
res=1;break;}
default:if(v1==null&&v2!=null)
res=-1;else if(v1!=null&&v2==null)
res=1;else if(v1<v2)
res=-1;else if(v1>v2)
res=1;}
if(this.SortIndicator==2)
res=-res;}
return res;}
function igtbl_columnCompareCells(cell1,cell2)
{var res=0;var v1=cell1.getValue(this.ColumnType==5||this.WebComboId);var v2=cell2.getValue(this.ColumnType==5||this.WebComboId);if(v1!=null||v2!=null)
{if(!cell1.Column.SortCaseSensitive)
{if(typeof(v1)=="string")
v1=v1.toLowerCase();if(typeof(v2)=="string")
v2=v2.toLowerCase();}
switch(this.DataType)
{case 8:{if(ig_csom.IsIE55Plus||ig_csom.IsNetscape6||(v1&&v1.localeCompare))
{if(v1==null&&v2!=null)
res=-1;else if(v1!=null&&v2==null)
res=1;else
res=v1.localeCompare(v2);}
else
if(v1==null&&v2!=null)
res=-1;else if(v1!=null&&v2==null)
res=1;else if(v1<v2)
res=-1;else if(v1>v2)
res=1;break;}
default:if(v1==null&&v2!=null)
res=-1;else if(v1!=null&&v2==null)
res=1;else if(v1<v2)
res=-1;else if(v1>v2)
res=1;}
if(this.SortIndicator==2)
res=-res;}
return res;}
function igtbl_compare(av1,av2,caseSens,sort)
{return igtbl_compareRows(av1,av2,sort);}
function igtbl_compareRows(av1,av2,columns)
{var res=0;for(var i=0;i<columns.length&&res==0;i++)
{var v1=av1[i+1];var v2=av2[i+1];if(v1!=null||v2!=null)
{var t1=typeof(v1);var t2=typeof(v2);if(!columns[i].SortCaseSensitive)
{if(t1=="string")
v1=v1.toLowerCase();if(t2=="string")
v2=v2.toLowerCase();}
if(t1=="string"&&t2=="string")
{if(ig_csom.IsIE55Plus||ig_csom.IsNetscape6||(v1&&v1.localeCompare))
{if(v1==null&&v2!=null)
res=-1;else if(v1!=null&&v2==null)
res=1;else
{res=v1.localeCompare(v2);}}
else
if(v1==null&&v2!=null)
res=-1;else if(v1!=null&&v2==null)
res=1;else if(v1<v2)
res=-1;else if(v1>v2)
res=1;}
else
{if(v1==null&&v2!=null)
res=-1;else if(v1!=null&&v2==null)
res=1;else if(v1<v2)
res=-1;else if(v1>v2)
res=1;}
if(columns[i].SortIndicator&&columns[i].SortIndicator==2||typeof(columns[i])=="number"&&columns[i]==2)
{res=-res;}}}
return res;}
function igtbl_quickSort(cln,array,left,right)
{var i,j,comp,temp;i=left;j=right;comp=cln.getRow(array[Math.floor((left+right)/2)]);do
{while(cln.getRow(array[i]).compare(comp)<0&&i<right)
i++;while(cln.getRow(array[j]).compare(comp)>0&&j>left)
j--;if(i<=j)
{temp=array[i];array[i]=array[j];array[j]=temp;i++;j--;}}
while(i<=j);if(left<j)
igtbl_quickSort(cln,array,left,j);if(i<right)
igtbl_quickSort(cln,array,i,right);}
function igtbl_quickSort1(cln,array,colInfo,left,right)
{var i,j,comp,temp;i=left;j=right;comp=array[Math.floor((left+right)/2)];do
{while(igtbl_compareRows(array[i],comp,colInfo)<0&&i<right)
i++;while(igtbl_compareRows(array[j],comp,colInfo)>0&&j>left)
j--;if(i<=j)
{if(i<j)
{temp=array[i];array[i]=array[j];array[j]=temp;}
i++;j--;}}
while(i<=j);if(left<j)
igtbl_quickSort1(cln,array,colInfo,left,j);if(i<right)
igtbl_quickSort1(cln,array,colInfo,i,right);}
function igtbl_bubbleSort(cln,array,colInfo)
{var hasSwapped=true;while(hasSwapped)
{hasSwapped=false;for(var i=0;i<array.length-1;i++)
{if(igtbl_compareRows(array[i],array[i+1],colInfo)>0)
{hasSwapped=true;var swap=array[i];array[i]=array[i+1]
array[i+1]=swap;}}}}
function igtbl_insertionSort(cln,array,colInfo)
{var i=1;while(i<array.length)
{var row=array[i];var j=i-1;while(j>=0&&igtbl_compareRows(array[j],row,colInfo)>0)
{array[j+1]=array[j];j--;}
array[j+1]=row;i++;}}
function igtbl_binaryTreeNodeCreate()
{return{"values":[],"left":null,"right":null};}
function igtbl_binaryTreeInsert(tree,value,array,colInfo,caseSense)
{if(tree.values.length==0)
tree.values[0]=value;else
{var treeRow=tree.values[0];var compareResult=igtbl_compareRows(value,treeRow,colInfo);if(compareResult==0)
tree.values[tree.values.length]=value;else if(compareResult<0)
tree.left=igtbl_binaryTreeInsert(tree.left==null?igtbl_binaryTreeNodeCreate():tree.left,value,array,colInfo);else
tree.right=igtbl_binaryTreeInsert(tree.right==null?igtbl_binaryTreeNodeCreate():tree.right,value,array,colInfo);}
return tree;}
function igtbl_binaryTreeTraverse(tree,array,index)
{if(tree.left!=null)
index=igtbl_binaryTreeTraverse(tree.left,array,index);for(var i=0;i<tree.values.length;i++)
{array[index++]=tree.values[i];tree.values[i]=null;}
if(tree.right!=null)
index=igtbl_binaryTreeTraverse(tree.right,array,index);return index;}
function igtbl_binaryTreeSort(cln,array,colInfo)
{var tree=igtbl_binaryTreeNodeCreate();for(var i=0;i<array.length;i++)
tree=igtbl_binaryTreeInsert(tree,array[i],array,colInfo);igtbl_binaryTreeTraverse(tree,array,0);igtbl_dispose(tree);}
function igtbl_clctnSort(sortedCols)
{if(!this.Band.IsGrouped&&this.Grid.LoadOnDemand==3)
return this.sortXml();if(!sortedCols)
sortedCols=this.Band.SortedColumns;this.setLastRowId();var changed=true;var sortArray=new Array(this.length);var colInfo=new Array();var chkBoxArray=new Array();for(var i=0;i<this.length;i++)
sortArray[i]=[i];for(var j=0;j<this.Band.SortedColumns.length;j++)
{var column=igtbl_getColumnById(this.Band.SortedColumns[j]);if(column.IsGroupBy)
{if(this.length>0)
{var grCol=igtbl_getColumnById(this.getRow(0).GroupColId);if(grCol==column)
{for(var i=0;i<this.length;i++)
sortArray[i][j+1]=this.getRow(i).Value;colInfo[j]=column;}}}
else
{if(column.ColumnType==5)
{for(var i=0;i<this.length;i++)
{var srtCol=this.getRow(i).getCellByColumn(column);if(srtCol)sortArray[i][j+1]=srtCol.getValue(true);}}
else
{for(var i=0;i<this.length;i++)
{var srtCol=this.getRow(i).getCellByColumn(column);if(srtCol)sortArray[i][j+1]=srtCol.getValue();}}
colInfo[j]=column;}}
for(i=0;i<this.Band.Columns.length;i++)
{var col=this.Band.Columns[i];if(col.hasCells()&&col.ColumnType==3)
chkBoxArray[chkBoxArray.length]=i;}
if(sortedCols.length>0&&this.length>0)
{var firstSortCol=igtbl_getColumnById(sortedCols[0]);var sortAlg=firstSortCol.getSortingAlgorithm();var sortImpl=firstSortCol.getSortImplementation();if(sortAlg==5&&sortImpl!=null)
sortImpl(this,sortArray,colInfo);else
switch(sortAlg)
{default:case 1:igtbl_quickSort1(this,sortArray,colInfo,0,this.length-1);break;case 2:igtbl_bubbleSort(this,sortArray,colInfo);break;case 3:igtbl_insertionSort(this,sortArray,colInfo);break;case 4:igtbl_binaryTreeSort(this,sortArray,colInfo);break;}}
var cntnSort=false;for(var i=this.Band.Index+1;i<this.Grid.Bands.length&&!cntnSort;i++)
if(this.Grid.Bands[i].SortedColumns.length>0)
cntnSort=true;var alternateStyle=this.Band.getRowAltClassName();var rowStyle=this.Band.getRowStyleClassName();var useAlternateRowStyle=(alternateStyle!=""&&rowStyle!=alternateStyle);for(var i=0;i<this.length;i++)
{if(sortArray[i][0]!=i)
{var san=sortArray[i][0];this.insert(this.remove(san),i);igtbl_dontHandleChkBoxChange=true;for(var j=0;j<chkBoxArray.length;j++)
{var cell=this.getRow(i).getCell(chkBoxArray[j]);if(cell&&cell.Element.getAttribute("chkBoxState"))
{var chkBoxEl=cell.getElement().firstChild;if(chkBoxEl.tagName=="NOBR")
chkBoxEl=chkBoxEl.firstChild;chkBoxEl.checked=(cell.Element.getAttribute("chkBoxState")=="true");}}
igtbl_dontHandleChkBoxChange=false;sortArray[i][0]=i;for(j=i+1;j<sortArray.length;j++)
if(sortArray[j][0]<san)
sortArray[j][0]++;}
var curRow=this.getRow(i);var className="";if(useAlternateRowStyle)
className=i%2?alternateStyle:rowStyle;if(useAlternateRowStyle&&!curRow.GroupByRow)
{var e=curRow.Element;if(curRow.Band._optSelectRow)
{var oldClassName=className;var colCssClass;if(useAlternateRowStyle)
colCssClass=(i%2==0)?col.CssClass:col._AltCssClass;else
colCssClass=col.CssClass;if(colCssClass&&className.indexOf(colCssClass)==-1)
className=className+" "+colCssClass;var rowClass=e.getAttribute("rowClass");if(rowClass)
className=className+" "+rowClass;var selectedStyle=curRow.Band.getSelClass();if(e.className&&selectedStyle)
{if(e.className.indexOf(selectedStyle)!=-1)
{className+=(" "+selectedStyle);}}
var activationStyle=curRow.Band.Grid.Activation._cssClass;if(e.className&&activationStyle)
{if(e.className.indexOf(activationStyle)!=-1)
{className+=(" "+activationStyle);}}
if(e.className!=className)
e.className=className;className=oldClassName;}
else
{var j=curRow.Band.firstActiveCell;var colNo=0;var rowElem=curRow.Element;var nonFixed=false;while(j<rowElem.cells.length)
{var col=curRow.Band.Columns[colNo];while(col&&!col.hasCells())
col=curRow.Band.Columns[++colNo];if(colNo>=curRow.Band.Columns.length)
break;if(col.getFixed()===false&&!nonFixed)
{j=0;rowElem=curRow.nfElement;nonFixed=true;}
var e=rowElem.cells[j];if(e)
{var oldClassName=className;var colCssClass;if(useAlternateRowStyle)
colCssClass=(i%2==0)?col.CssClass:col._AltCssClass;else
colCssClass=col.CssClass;if(colCssClass)
className=className+" "+colCssClass;var rowClass=rowElem.getAttribute("rowClass");if(rowClass)
className=className+" "+rowClass;var cellClass=e.getAttribute("cellClass");if(cellClass)
className=className+" "+cellClass;if(e.className!=className)
e.className=className;className=oldClassName;}
j++;colNo++;}}}
if(curRow.Expandable)
{var col=sortedCols.length>0?igtbl_getColumnById(sortedCols[0]):null;if(col&&col.IsGroupBy)
{if(curRow.Rows)
curRow.Rows.sort(sortedCols.slice(1));}
else if(cntnSort&&curRow.Rows)
curRow.Rows.sort(this.Grid.Bands[this.Band.Index+1].SortedColumns);}}
if(this.Node)
this.reIndex(0);igtbl_dispose(sortArray);delete sortArray;igtbl_dispose(chkBoxArray);delete chkBoxArray;}
function igtbl_getCollapseImage(gn,bandNo)
{var g=igtbl_getGridById(gn);return g.Bands[bandNo].getCollapseImage();}
function igtbl_getExpandImage(gn,bandNo)
{var g=igtbl_getGridById(gn);return g.Bands[bandNo].getExpandImage();}
function igtbl_getCellClickAction(gn,bandNo)
{var g=igtbl_getGridById(gn);return g.Bands[bandNo].getCellClickAction();}
function igtbl_getSelectTypeCell(gn,bandNo)
{var g=igtbl_getGridById(gn);var res=g.SelectTypeCell;if(g.Bands[bandNo].SelectTypeCell!=0)
res=g.Bands[bandNo].SelectTypeCell;return res;}
function igtbl_getSelectTypeColumn(gn,bandNo)
{var g=igtbl_getGridById(gn);var res=g.SelectTypeColumn;if(g.Bands[bandNo].SelectTypeColumn!=0)
res=g.Bands[bandNo].SelectTypeColumn;return res;}
function igtbl_getSelectTypeRow(gn,bandNo)
{var g=igtbl_getGridById(gn);var res=g.SelectTypeRow;if(g.Bands[bandNo].SelectTypeRow!=0)
res=g.Bands[bandNo].SelectTypeRow;return res;}
function igtbl_getHeaderClickAction(gn,bandNo,columnNo)
{var g=igtbl_getGridById(gn);var res=g.HeaderClickAction;var band=g.Bands[bandNo];var column=band.Columns[columnNo];if(column.HeaderClickAction!=0)
res=column.HeaderClickAction;else if(band.HeaderClickAction!=0)
res=band.HeaderClickAction;if(res>1)
{if(band.AllowSort!=0)
{if(band.AllowSort==2)
res=0;}
else if(g.AllowSort==0||g.AllowSort==2)
res=0;}
return res;}
function igtbl_getAllowUpdate(gn,bandNo,columnNo)
{var g=igtbl_getGridById(gn);if(typeof(columnNo)!="undefined")
return g.Bands[bandNo].Columns[columnNo].getAllowUpdate();var res=g.AllowUpdate;if(g.Bands[bandNo].AllowUpdate!=0)
res=g.Bands[bandNo].AllowUpdate;return res;}
function igtbl_getAllowColSizing(gn,bandNo,columnNo)
{var g=igtbl_getGridById(gn);var res=g.AllowColSizing;if(g.Bands[bandNo].AllowColSizing!=0)
res=g.Bands[bandNo].AllowColSizing;if(g.Bands[bandNo].Columns[columnNo].AllowColResizing!=0)
res=g.Bands[bandNo].Columns[columnNo].AllowColResizing;return res;}
function igtbl_getRowSizing(gn,bandNo,row)
{var g=igtbl_getGridById(gn);var res=g.RowSizing;if(g.Bands[bandNo].RowSizing!=0)
res=g.Bands[bandNo].RowSizing;if(row.getAttribute("sizing"))
res=parseInt(row.getAttribute("sizing"),10);return res;}
function igtbl_getRowSelectors(gn,bandNo)
{var g=igtbl_getGridById(gn);return g.Bands[bandNo].getRowSelectors();}
function igtbl_getNullText(gn,bandNo,columnNo)
{var g=igtbl_getGridById(gn);if(g.Bands[bandNo].Columns[columnNo].NullText!="")
return g.Bands[bandNo].Columns[columnNo].NullText;if(g.Bands[bandNo].NullText!="")
return g.Bands[bandNo].NullText;return g.NullText;}
function igtbl_getEditCellClass(gn,bandNo)
{var g=igtbl_getGridById(gn);if(g.Bands[bandNo].EditCellClass!="")
return g.Bands[bandNo].EditCellClass;return g.EditCellClass;}
function igtbl_getFooterClass(gn,bandNo)
{var g=igtbl_getGridById(gn);return g.Bands[bandNo].getFooterClass();}
function igtbl_getGroupByRowClass(gn,bandNo)
{return g.Bands[bandNo].getGroupByRowClass();}
function igtbl_getHeadClass(gn,bandNo,columnNo)
{var g=igtbl_getGridById(gn);return g.Bands[bandNo].Columns[columnNo].getHeadClass();}
function igtbl_getRowLabelClass(gn,bandNo)
{var g=igtbl_getGridById(gn);return g.Bands[bandNo].getRowLabelClass();}
function igtbl_getSelGroupByRowClass(gn,bandNo)
{var g=igtbl_getGridById(gn);return g.Bands[bandNo].getSelGroupByRowClass();}
function igtbl_getSelHeadClass(gn,bandNo,columnNo)
{var g=igtbl_getGridById(gn);if(g.Bands[bandNo].Columns[columnNo].SelHeadClass!="")
return g.Bands[bandNo].Columns[columnNo].SelHeadClass;if(g.Bands[bandNo].SelHeadClass!="")
return g.Bands[bandNo].SelHeadClass;return g.SelHeadClass;}
function igtbl_getSelCellClass(gn,bandNo,columnNo)
{var g=igtbl_getGridById(gn);return g.Bands[bandNo].Columns[columnNo].getSelClass();}
function igtbl_getExpAreaClass(gn,bandNo)
{var g=igtbl_getGridById(gn);return g.Bands[bandNo].getExpAreaClass();}
function igtbl_getCurrentRowImage(gn,bandNo)
{var g=igtbl_getGridById(gn);var res=g.CurrentRowImage;var band=g.Bands[bandNo];if(band.CurrentRowImage!="")
res=band.CurrentRowImage;var au=igtbl_getAllowUpdate(gn,band.Index);if(band.RowTemplate!=""&&(au==1||au==3))
{res=g.CurrentEditRowImage;if(band.CurrentEditRowImage!="")
res=band.CurrentEditRowImage;}
return res;}
function igtbl_getCurrentRowAltText(gn,bandNo)
{var g=igtbl_getGridById(gn);var band=g.Bands[bandNo];var au=igtbl_getAllowUpdate(gn,bandNo);var alt=g._currentRowAltText;if(band.RowTemplate!=""&&(au==1||au==3))
alt=g._currentEditRowAltText;return alt;}
function igtbl_getBandFAC(gn,elem)
{var gs=igtbl_getGridById(gn);var bandNo=null;if(elem.tagName=="TD"||elem.tagName=="TH")
{if(elem.id!="")
{return igtbl_getBandById(elem.id).firstActiveCell;}
else{elem=elem.parentNode;}}
if(elem.tagName=="TR")
bandNo=elem.parentNode.parentNode.getAttribute("bandNo");if(elem.tagName=="TABLE")
bandNo=elem.getAttribute("bandNo");if(bandNo)
return gs.Bands[bandNo].firstActiveCell;return null;}
function igtbl_enumColumnCells(gn,column)
{var cellIndex=null;var colOffs=column.getAttribute("colOffs");if(colOffs!==null)
cellIndex=igtbl_parseInt(colOffs);else
{var i=0;while(i<column.parentNode.childNodes.length&&cellIndex===null)
{if(column.parentNode.childNodes[i]==column)
cellIndex=i;i++;}}
var nonFixed=false;i=0;var pn=column.parentNode;while(i<5&&pn&&!(pn.tagName=="DIV"&&pn.id==gn+"_drs"))
{pn=pn.parentNode;nonFixed=pn&&pn.tagName=="DIV"&&pn.id==gn+"_drs";i++;}
var ar=new Array();var colIdA=column.id.split("_");var fac=igtbl_getBandFAC(gn,column);var thead=column.parentNode;while(thead&&thead.tagName!="THEAD")
thead=thead.parentNode;if(thead)
for(var i=1;i<thead.parentNode.rows.length;i++)
{var row=thead.parentNode.rows[i];if(!row.getAttribute("hiddenRow")&&row.parentNode.tagName!="TFOOT")
{var visElem=null;if(!nonFixed)
visElem=row.cells[cellIndex];else
for(var j=fac;j<row.cells.length&&!visElem;j++)
{var cell=row.cells[j];if(cell.firstChild&&cell.firstChild.id==gn+"_drs")
{row=cell.firstChild.firstChild.rows[0];visElem=row.cells[cellIndex];}}
if(visElem&&visElem.id||!visElem)
{var visCol=null;if(visElem)
visCol=igtbl_getColumnById(visElem.id);if(visElem&&(!visCol||visCol.Id!=column.id)||!visElem)
{visElem=row.cells[0];visCol=null;if(visElem)
visCol=igtbl_getColumnById(visElem.id);while(visElem&&(!visCol||visCol.Id!=column.id))
{visElem=visElem.nextSibling;if(visElem)
visCol=igtbl_getColumnById(visElem.id);}}}
if(visElem)
{ar[ar.length]=visElem;}}}
return ar;}
function igtbl_getElemVis(cols,index)
{var i=0,j=-1;while(cols&&cols[i]&&j!=index)
{if(!ig_csom.IsIE||cols[i].style.display!="none")
j++;i++;}
return cols[i-1];}
function igtbl_hideColHeader(tBody,col,hide,fixedHeaders)
{var realIndex=-1;var trueRealIndex=-1;var tr=tBody.childNodes[0];for(var i=0;i<tr.cells.length;i++)
{var c=tr.cells[i];if(c.colSpan>1&&c.firstChild.tagName=="DIV"&&c.firstChild.id.substr(c.firstChild.id.length-4)=="_drs")
{tr=c.firstChild.firstChild.childNodes[1].rows[0];i=0;c=tr.cells[i];trueRealIndex=-1;}
if(c.style.display=="")
{realIndex++;trueRealIndex++;}
if(col.Id&&c.id==col.Id||col.fId&&c.id==col.fId)
{var h=(hide?"none":"");if(c.style.display==h)
return;c.style.display=h;var headerColGroup=null;var stationaryMarginsUsed=false;if(fixedHeaders)
{if(col.getFixed())
{headerColGroup=tBody.previousSibling.childNodes;}
else if(tr&&tr.parentNode&&tr.parentNode.previousSibling&&tr.parentNode.previousSibling.tagName=="COLGROUP")
{headerColGroup=tr.parentNode.previousSibling.childNodes;stationaryMarginsUsed=true;}
else if(tBody.nextSibling&&tBody.nextSibling.nextSibling)
{var childNodes=tBody.nextSibling.nextSibling.childNodes[0].childNodes;var i=0;while(i<childNodes.length)
{var col=childNodes[i];i++;if(col.colSpan>1&&col.firstChild.tagName=="DIV"&&col.firstChild.id.substr(col.firstChild.id.length-4)=="_drs")
{headerColGroup=col.childNodes[0].childNodes[0].childNodes[0].childNodes;break;}}}}
else if(tBody.nextSibling&&tBody.nextSibling.nextSibling)
{headerColGroup=tBody.nextSibling.nextSibling.childNodes[0].childNodes;}
if(headerColGroup&&!stationaryMarginsUsed)
{headerColGroup[i].style.display=h;}
var chn=tBody.previousSibling;while(chn.tagName!="COLGROUP")
chn=chn.previousSibling;chn=chn.childNodes;if(hide)
{var ch=chn[realIndex];col.Width=ch.width;ch.parentNode.appendChild(ch);ch.width="1px";ch.style.display="none";if(headerColGroup)
{if(stationaryMarginsUsed)
{var sch=headerColGroup[trueRealIndex];col.Width=sch.width;sch.parentNode.appendChild(sch);sch.width="1px";sch.style.display="none";}
else
headerColGroup[headerColGroup.length-1].width=col.Width;}}
else
{var ch=chn[chn.length-1];ch.style.cssText=col.Style;ch.width=col.Width+"px";if(chn[realIndex+1])
ch.parentNode.insertBefore(ch,chn[realIndex+1])
if(ch.style.display=="none")
ch.style.display="";if(headerColGroup)
{if(stationaryMarginsUsed)
{var sch=headerColGroup[headerColGroup.length-1];sch.style.cssText=col.Style;sch.width=col.Width+"px";if(headerColGroup[trueRealIndex+1])
sch.parentNode.insertBefore(sch,headerColGroup[trueRealIndex+1])
if(sch.style.display=="none")
sch.style.display="";}
else
headerColGroup[i].width=col.Width;}}
break;}}
return trueRealIndex;}
function igtbl_hideColumn(rows,col,hide)
{if(col&&col.Hidden==hide)
return;var g=col.Band.Grid;var ao=g.Activation;var realIndex=-1;igtbl_lineupHeaders(col.Id,col.Band);if(col.Band.Index==rows.Band.Index)
{if(col.Band.Index==0)
{if(g.StatHeader)
{var el=g.StatHeader.getElementByColumn(col);igtbl_hideColHeader(g.StatHeader.Element,col,hide,g.UseFixedHeaders);}
if(g.StatFooter)
{var el=g.StatFooter.getElementByColumn(col);igtbl_hideColHeader(g.StatFooter.Element,col,hide,g.UseFixedHeaders);}
if(g.StatHeader||g.StatFooter)
{var filterRow=rows.FilterRow;if(filterRow)
{var filterCell=filterRow.getCellByColumn(col);filterCell.Element.style.display=(hide?"none":"");}}}
var tBody=rows.Element.previousSibling;if(tBody)
{realIndex=igtbl_hideColHeader(tBody,col,hide,g.UseFixedHeaders);}
if(g.UseFixedHeaders&&!g.StatFooter&&rows.Element.nextSibling&&rows.Element.nextSibling.tagName=="TFOOT")
{igtbl_hideColHeader(rows.Element.nextSibling,col,hide,g.UseFixedHeaders);}}
for(var i=0;i<rows.length;i++)
{var row=rows.getRow(i);if(col.Band.Index==rows.Band.Index&&!row.GroupByRow)
{var cell=row.getCellByColumn(col);if(hide)
{if(cell.Element==null){}
else
{var cellElm=cell.Element;cellElm.style.display="none";if(g.UseFixedHeaders&&!col.getFixed()&&cellElm.parentNode&&cellElm.parentNode.parentNode&&cellElm.parentNode.parentNode.previousSibling&&cellElm.parentNode.parentNode.previousSibling.tagName=="COLGROUP")
{var headerColGroup=cellElm.parentNode.parentNode.previousSibling.childNodes;var sch=headerColGroup[realIndex];col.Width=sch.width;sch.parentNode.appendChild(sch);sch.width="1px";sch.style.display="none";}}
if(col.Band.Grid.getActiveRow()==row)
{if(igtbl_hasClassName(cell.Element,ao._cssClassL))
{igtbl_removeClassName(cell.Element,ao._cssClassL);for(var j=col.Index+1;j<col.Band.Columns.length;j++)
if(col.Band.Columns[j].getVisible()&&col.Band.Columns[j].hasCells())
{igtbl_setClassName(row.getCellByColumn(col.Band.Columns[j]).Element,ao._cssClassL);break;}}
if(igtbl_hasClassName(cell.Element,ao._cssClassR))
{igtbl_removeClassName(cell.Element,ao._cssClassR);for(var j=col.Index-1;j>=0;j--)
if(col.Band.Columns[j].getVisible()&&col.Band.Columns[j].hasCells())
{igtbl_setClassName(row.getCellByColumn(col.Band.Columns[j]).Element,ao._cssClassR);break;}}}}
else
{if(cell.Element==null){}
else
{var cellElm=cell.Element;if(g.UseFixedHeaders&&!col.getFixed()&&cellElm.parentNode&&cellElm.parentNode.parentNode&&cellElm.parentNode.parentNode.previousSibling&&cellElm.parentNode.parentNode.previousSibling.tagName=="COLGROUP")
{var headerColGroup=cellElm.parentNode.parentNode.previousSibling.childNodes;var sch=headerColGroup[headerColGroup.length-1];sch.style.cssText=col.Style;sch.width=col.getWidth()+"px";if(headerColGroup[realIndex+1])
sch.parentNode.insertBefore(sch,headerColGroup[realIndex+1])
if(sch.style.display=="none")
sch.style.display="";}
cellElm.style.display="";}
if(col.Band.Grid.getActiveRow()==row)
{var j=0;for(j=0;j<col.Band.Columns.length;j++)
if(col.Band.Columns[j].getVisible()&&col.Band.Columns[j].hasCells())
break;if(j>col.Index)
{igtbl_removeClassName(row.getCellByColumn(col.Band.Columns[j]).Element,ao._cssClassL);igtbl_setClassName(cell.Element,ao._cssClassL);}
for(j=col.Band.Columns.length-1;j>=0;j--)
if(col.Band.Columns[j].getVisible()&&col.Band.Columns[j].hasCells())
break;if(j<col.Index)
{igtbl_removeClassName(row.getCellByColumn(col.Band.Columns[j]).Element,ao._cssClassR);igtbl_setClassName(cell.Element,ao._cssClassR);}}}}
else if(col.Band.Index>=rows.Band.Index&&row.Expandable)
{if((row.GroupByRow||col.Band.Index>rows.Band.Index)&&row.Rows)
igtbl_hideColumn(row.Rows,col,hide);}}
if(ig_shared.IsFireFox)
igtbl_lineupHeaders(col.Id,col.Band);if(g.UseFixedHeaders&&col.Band.Index==0)
{var colWidthAdjustment=(hide)?col.getWidth()*-1:col.getWidth();var scrw=g._scrElem.firstChild.offsetWidth+colWidthAdjustment;if(scrw>=0)
{g._scrElem.firstChild.style.width=scrw+"px";}
g.alignStatMargins();g.alignDivs(0,true);}}
function igtbl_isColEqual(col1,col2)
{if(col1==null&&col2==null)
return true;if(col1==null||col2==null)
return false;if(col1.Band.Index==col2.Band.Index&&col1.Key==col1.Key&&col1.Index==col2.Index)
return true;return false;}
function igtbl_assignColumnElements(ce,band)
{if(ce)
{if(typeof(ce.getAttribute)!="undefined"&&ce.getAttribute("columnNo"))
{var colNo=igtbl_parseInt(ce.getAttribute("columnNo"));band.Columns[colNo].Element=ce;}
if(ce.childNodes)
for(var i=0;i<ce.childNodes.length;i++)
igtbl_assignColumnElements(ce.childNodes[i],band);}}
function _igtbl_sortNumber(a,b)
{return a[0]-b[0];}
function _igtbl_createXmlElement(doc,tagName,ns)
{return igtbl_xml.createXmlElement(doc,tagName,ns);}
function _igtbl_createXmlTextNode(doc,ns)
{return igtbl_xml.createXmlTextNode(doc,ns);}
function igtbl_rowGetValue(colId)
{}
if(ig_csom.IsIE6)
{var igtbl_mouseDownX;var igtbl_mouseDownY;}
function igtbl_headerClickDown(evnt,gn)
{if(!evnt&&event)
evnt=event;if(!gn&&igtbl_lastActiveGrid)
gn=igtbl_lastActiveGrid;if(!gn||!evnt)
return false;var gs=igtbl_getGridById(gn);if(!gs||gs.isDisabled())
return;gs.event=evnt;igtbl_lastActiveGrid=gn;var te=gs.Element;te.setAttribute("mouseDown",evnt.button);var se=igtbl_srcElement(evnt);if(se&&se.tagName=="IMG"&&(se.getAttribute("imgType")=="group"||se.getAttribute("imgType")=="fixed"))
return;while(se&&(se.tagName!="TH"||se.id.length<gn.length||se.id.substr(0,gn.length)!=gn)&&(se.tagName!="DIV"||!se.getAttribute("groupInfo")))
se=se.parentNode;if(!se)
return;if(se.tagName=="TH"&&se.parentNode.parentNode.tagName!="TFOOT")
{var colObj=igtbl_getColumnById(se.id);if(!colObj)return;if(igtbl_fireEvent(gn,gs.Events.MouseDown,"(\""+gn+"\",\""+se.id+"\","+igtbl_button(gn,evnt)+")")==true)
return true;if(igtbl_button(gn,evnt)!=0)
return;var bandNo=colObj.Band.Index;var band=colObj.Band;if(igtbl_getOffsetX(evnt,se)>igtbl_clientWidth(se)-4&&igtbl_getAllowColSizing(gn,bandNo,colObj.Index)==2)
{if(ig_csom.IsFireFox&&igtbl_inEditMode(gn))
gs.endEdit();te.setAttribute("elementMode","resize");te.setAttribute("resizeColumn",se.id);igtbl_lineupHeaders(se.id,band);var div,divr;if(!document.body.igtbl_resizeDiv)
{div=document.createElement("DIV");div.style.zIndex=gs._getZ(10000,1);div.style.position="absolute";div.style.left="0px";div.style.top="0px";div.style.width="0px";div.style.height="0px";document.body.insertBefore(div,document.body.firstChild);igtbl_addEventListener(div,"mouseup",igtbl_resizeDivMouseUp,false);igtbl_addEventListener(div,"mousemove",igtbl_resizeDivMouseMove,false);igtbl_addEventListener(div,"selectstart",igtbl_resizeDivSelectStart,false);document.body.igtbl_resizeDiv=div;divr=document.createElement("DIV");div.appendChild(divr);divr.style.position="absolute";if(igtbl_isXHTML||ig_csom.IsNetscape6)
{divr.style.borderLeftWidth="1px";divr.style.borderLeftColor="black";divr.style.borderLeftStyle="solid";divr.style.width="1px";}
else
{divr.style.borderWidth="1px";divr.style.borderColor="black";divr.style.borderStyle="solid";divr.style.width="2px";}}
else
{div=document.body.igtbl_resizeDiv;divr=div.firstChild;}
div.setAttribute("gn",gn);div.style.display="";div.style.cursor="w-resize";var divw=document.body.clientWidth,divh=document.body.clientHeight
div.style.width=divw+"px";div.style.height=divh+"px";div.style.backgroundColor="transparent";divr.style.top=igtbl_getTopPos(te.parentNode,false)+"px";divr.style.left=evnt.clientX
+igtbl_getBodyScrollLeft()
+"px";divr.style.height=te.parentNode.offsetHeight+"px";div.column=colObj;div.srcElement=se;div.initX=evnt.clientX;return true;}
se.setAttribute("justClicked",true);if(ig_csom.IsIE6)
{igtbl_mouseDownX=evnt.x;igtbl_mouseDownY=evnt.y;}
if(igtbl_getHeaderClickAction(gn,bandNo,colObj.Index)==1&&(gs.SelectedColumns[se.id]!=true||gs.ViewType!=2||igtbl_getSelectTypeColumn(gn,bandNo)==3))
{if(igtbl_getSelectTypeColumn(gn,bandNo)<2)
return true;te.setAttribute("elementMode","select");te.setAttribute("selectMethod","column");if(!(igtbl_getSelectTypeColumn(gn,bandNo)==3&&evnt.ctrlKey))
igtbl_clearSelectionAll(gn);if(te.getAttribute("shiftSelect")&&evnt.shiftKey)
{te.setAttribute("lastSelectedColumn","");igtbl_selectColumnRegion(gn,se);te.removeAttribute("shiftSelect");}
else
{te.setAttribute("startColumn",se.id);if(gs.SelectedColumns[se.id]&&evnt.ctrlKey)
igtbl_selectColumn(gn,se.id,false);else
igtbl_selectColumn(gn,se.id);te.removeAttribute("shiftSelect");if(!evnt.ctrlKey)
te.setAttribute("shiftSelect",true);}}
ig_cancelEvent(evnt);return true;}
else if(se.tagName=="DIV"&&se.getAttribute("groupInfo"))
{if(igtbl_button(gn,evnt)!=0)
return;if(igtbl_fireEvent(gn,gs.Events.MouseDown,"(\""+gn+"\",\""+se.id+"\","+igtbl_button(gn,evnt)+")")==true)
return;var groupInfo=se.getAttribute("groupInfo").split(":");if(groupInfo[0]!="band")
igtbl_changeStyle(gn,se,igtbl_getSelHeadClass(gn,groupInfo[1],groupInfo[2]));se.setAttribute("justClicked",true);return true;}}
function igtbl_resizeDivMouseUp(evnt)
{if(!evnt)evnt=event;if(!evnt)return;var se=document.body.igtbl_resizeDiv;if(!se)return;var gn=se.getAttribute("gn");var g=igtbl_getGridById(gn);if(g&&g.Element.getAttribute("mouseDown"))
g.Element.removeAttribute("mouseDown");se.style.display="none";if(se.initX!=evnt.clientX)
{var col=se.column;if(!col||!col.Width)
return;var oldWidth=-1;if(col.Width.length&&col.Width.charAt(col.Width.length-1)=="%")
{oldWidth=se.srcElement.offsetWidth;}
else if(col.Element&&col.Element.colSpan>1)
{var colTags=col._getColTags();if(colTags.length==4)
{oldWidth=igtbl_getAbsBounds(col.Element).w-colTags[1].width;}}
if(oldWidth==-1)
{oldWidth=parseInt(col.Width,10);}
var newWidth=oldWidth+evnt.clientX-se.initX;if(newWidth<=0)
newWidth=1;if(oldWidth!=newWidth)
col.setWidth(newWidth);}}
function igtbl_resizeDivMouseMove(evnt)
{if(!evnt)
evnt=event;if(!evnt)
return;var se=document.body.igtbl_resizeDiv;if(!se)
return;var gn=se.getAttribute("gn");var g=igtbl_getGridById(gn);var te=null;if(g)te=g.Element;if(igtbl_button(null,evnt)>0||!te||!te.getAttribute("mouseDown"))
return igtbl_resizeDivMouseUp(evnt);se.style.cursor="w-resize";if(!se.firstChild)
se=se.parentNode;if(se.initX!=evnt.clientX)
{var col=se.column;if(parseInt(col.Width,10)+evnt.clientX-se.initX>0)
se.firstChild.style.left=evnt.clientX+igtbl_getBodyScrollLeft()+"px";}}
function igtbl_resizeDivSelectStart(evnt)
{if(!evnt)evnt=event;if(!evnt)return;return igtbl_cancelEvent(evnt);}
function igtbl_headerClickUp(evnt,gn)
{if(!evnt&&event)
evnt=event;if(!gn&&igtbl_lastActiveGrid)
gn=igtbl_lastActiveGrid;if(!gn||!evnt)
return false;var gs=igtbl_getGridById(gn);if(!gs||gs.isDisabled())
return;gs.event=evnt;if(igtbl_button(gn,evnt)==2)
return;var te=gs.Element;if(te.getAttribute("mouseDown"))
te.removeAttribute("mouseDown");else
return;var se=igtbl_srcElement(evnt);if(igtbl_isTemplatedElement(se))
return;if(se&&se.tagName=="IMG")
{var imgType=se.getAttribute("imgType");if(imgType=="group"||imgType=="fixed"||imgType=="filter")
return;}
while(se&&(se.tagName!="TH"||se.id.length<gn.length||se.id.substr(0,gn.length)!=gn)&&(se.tagName!="DIV"||!se.getAttribute("groupInfo")))
se=se.parentNode;if(!se)
return;var seTemp=se;while(seTemp!=null)
{if(seTemp.tagName=="TFOOT")
{return;}
seTemp=seTemp.parentNode}
seTemp=null;if(se.tagName=="TH")
{var column=igtbl_getColumnById(se.id);if(!column)return;var bandNo=column.Band.Index;var columnNo=column.Index;var mode=te.getAttribute("elementMode");var headerClickNeedPost=false;if(mode!="resize")
{var oldNP=gs.NeedPostBack;igtbl_fireEvent(gn,gs.Events.ColumnHeaderClick,"(\""+gn+"\",\""+se.id+"\","+igtbl_button(gn,evnt)+")");if(gs.NeedPostBack&&gs.NeedPostBack!=oldNP)
headerClickNeedPost=true;}
if(igtbl_fireEvent(gn,gs.Events.MouseUp,"(\""+gn+"\",\""+se.id+"\","+igtbl_button(gn,evnt)+")")==true)
return true;var headerClickAction=igtbl_getHeaderClickAction(gn,bandNo,columnNo);if(headerClickAction!=1)
igtbl_changeStyle(gn,se,null);te.removeAttribute("elementMode");te.removeAttribute("resizeColumn");te.removeAttribute("selectMethod");if(!te.getAttribute("shiftSelect"))
te.removeAttribute("startColumn");if(mode!="resize"&&(headerClickAction==2||headerClickAction==3)&&column.SortIndicator!=3)
{if(gs.Bands[bandNo].ClientSortEnabled)
{gs._displayPI();gs.startHourGlass();gs.sortingColumn=se;gs.oldColCursor=se.style.cursor;window.setTimeout("igtbl_gridSortColumn('"+gn+"','"+se.id+"',"+evnt.shiftKey+")",1);}
else
gs.sortColumn(se.id,evnt.shiftKey);if(gs.NeedPostBack&&!headerClickNeedPost)
igtbl_doPostBack(gn,evnt.shiftKey?"shiftKey:true":"");}
else
{if(mode=="resize")
igtbl_resizeDivMouseUp(evnt);if((mode=="resize"||mode=="select")&&gs.NeedPostBack)
{igtbl_doPostBack(gn,'HeaderClick:'+se.id);}
te.removeAttribute("elementMode");}}
else if(se.tagName=="DIV"&&se.getAttribute("groupInfo"))
{igtbl_fireEvent(gn,gs.Events.ColumnHeaderClick,"(\""+gn+"\",\""+se.id+"\","+igtbl_button(gn,evnt)+")");if(igtbl_fireEvent(gn,gs.Events.MouseUp,"(\""+gn+"\",\""+se.id+"\","+igtbl_button(gn,evnt)+")")==true)
return;var groupInfo=se.getAttribute("groupInfo").split(":");if(groupInfo[0]!="band")
{igtbl_changeStyle(gn,se,null);var bandNo=igtbl_bandNoFromColId(se.id);var columnNo=igtbl_colNoFromColId(se.id);var column=gs.Bands[bandNo].Columns[columnNo];var headerClickAction=igtbl_getHeaderClickAction(gn,bandNo,columnNo);if((headerClickAction==2||headerClickAction==3)&&column.SortIndicator!=3)
{if(gs.Bands[bandNo].ClientSortEnabled)
{gs._displayPI();gs.startHourGlass();gs.sortingColumn=se;gs.oldColCursor=se.style.cursor;window.setTimeout("igtbl_gridSortColumn('"+gn+"','"+se.id+"',true)",1);}
else
gs.sortColumn(se.id,evnt.shiftKey);if(gs.NeedPostBack)
igtbl_doPostBack(gn,evnt.shiftKey?"shiftKey:true":"");}}}
if(gs.NeedPostBack)
igtbl_doPostBack(gn,'HeaderClick:'+se.id);return true;}
function igtbl_headerContextMenu(evnt,gn)
{if(!evnt&&event)
evnt=event;if(!gn&&igtbl_lastActiveGrid)
gn=igtbl_lastActiveGrid;if(!gn||!evnt)
return false;var gs=igtbl_getGridById(gn);if(!gs||gs.isDisabled())
return;gs.event=evnt;if(igtbl_button(gn,evnt)==2&&!ig_csom.IsFireFox)
return;var te=gs.Element;te.removeAttribute("mouseDown");var se=igtbl_srcElement(evnt);while(se&&(se.tagName!="TH"||se.id.length<gn.length||se.id.substr(0,gn.length)!=gn)&&(se.tagName!="DIV"||!se.getAttribute("groupInfo")))
se=se.parentNode;if(!se)
return;if(se.tagName=="TH"||se.tagName=="DIV")
{var column=igtbl_getColumnById(se.id);if(se.tagName=="TH"&&!column)return;igtbl_fireEvent(gn,gs.Events.ColumnHeaderClick,"(\""+gn+"\",\""+se.id+"\",2)");if(igtbl_fireEvent(gn,gs.Events.MouseUp,"(\""+gn+"\",\""+se.id+"\",2)")==true)
return igtbl_cancelEvent(evnt);}}
function igtbl_headerMouseOut(evnt,gn)
{if(!evnt&&event)
evnt=event;if(!gn&&igtbl_lastActiveGrid)
gn=igtbl_lastActiveGrid;if(!gn||!evnt)
return false;var gs=igtbl_getGridById(gn);var se=igtbl_srcElement(evnt);if(!gs||!se||gs.isDisabled())
return;gs.event=evnt;if(se.tagName=="NOBR"&&se.title)
{se.title="";if(se.removeAttribute)
{se.removeAttribute("title");}}
while(se&&(se.tagName!="TH"||se.id.length<gn.length||se.id.substr(0,gn.length)!=gn)&&(se.tagName!="DIV"||!se.getAttribute("groupInfo")))
se=se.parentNode;if(!se)
return;if(se.tagName=="TH")
{var column=igtbl_getColumnById(se.id);if(!column)return;var sep=se.parentNode;if(gs.Element.getAttribute("elementMode")=="select")
return true;if(!igtbl_isMouseOut(se,evnt))return true;if(igtbl_fireEvent(gn,gs.Events.MouseOut,"(\""+gn+"\",\""+se.id+"\",1)")==true)
return true;if(igtbl_getHeaderClickAction(gn,column.Band.Index,column.Index)!=1)
igtbl_changeStyle(gn,se,null);return true;}
else if(se.tagName=="DIV"&&se.getAttribute("groupInfo"))
{if(!igtbl_isMouseOut(se,evnt))return true;if(igtbl_fireEvent(gn,gs.Events.MouseOut,"(\""+gn+"\",\""+se.id+"\",1)")==true)
return true;var groupInfo=se.getAttribute("groupInfo").split(":");if(groupInfo[0]!="band")
igtbl_changeStyle(gn,se,null);return true;}}
function igtbl_isMouseOut(se,evnt)
{var te=evnt.toElement;if(te==null)
te=evnt.relatedTarget;while(te!=null)
{if(te==se)
return false;try
{te=te.parentNode;}
catch(exc)
{break;}}
se._hasMouse=false;return true;}
function igtbl_headerMouseOver(evnt,gn)
{if(!evnt&&event)
evnt=event;if(!evnt)
return false;var se=igtbl_srcElement(evnt);if(!se)
return;var column;if(se.tagName=="NOBR")
{column=igtbl_getColumnById(se.parentNode.id);if(column)
{var nobr=se;var showTitle=nobr.offsetWidth>se.parentNode.offsetWidth||nobr.offsetHeight>se.parentNode.offsetHeight;var titleMode=column.getTitleModeResolved();showTitle|=titleMode==igtbl_CellTitleMode.Always;showTitle&=titleMode!=igtbl_CellTitleMode.Never;if(showTitle)
{nobr.title=column.HeaderText;}}}
else
column=igtbl_getColumnById(se.id);if(!column)return;var gs=column.Band.Grid;if(!gn)
gn=gs.Id;gs.event=evnt;if(!igtbl_lastActiveGrid||igtbl_lastActiveGrid.length<0||!igtbl_getGridById(igtbl_lastActiveGrid)||!igtbl_inEditMode(igtbl_lastActiveGrid))
igtbl_lastActiveGrid=gn;while(se&&(se.tagName!="TH"||se.id.length<gn.length||se.id.substr(0,gn.length)!=gn)&&(se.tagName!="DIV"||!se.getAttribute("groupInfo")))
se=se.parentNode;if(!se)
return;if(se.tagName!="DIV")
{while(se&&(se.tagName!="TH"||se.id.length<gn.length||se.id.substr(0,gn.length)!=gn))
se=se.parentNode;if(!se)
return;}
if(se._hasMouse)return;if(se.tagName=="TH")
{var column=igtbl_getColumnById(se.id);if(!column)return;se._hasMouse=true;igtbl_fireEvent(gn,gs.Events.MouseOver,"(\""+gn+"\",\""+se.id+"\",1)");}
else if(se.tagName=="DIV"&&se.getAttribute("groupInfo"))
{se._hasMouse=true;igtbl_fireEvent(gn,gs.Events.MouseOver,"(\""+gn+"\",\""+se.id+"\",1)");}}
function igtbl_headerMouseMove(evnt,gn)
{if(!evnt&&event)
evnt=event;if(!gn&&igtbl_lastActiveGrid)
gn=igtbl_lastActiveGrid;if(!gn||!evnt)
return false;var gs=igtbl_getGridById(gn);var se=igtbl_srcElement(evnt);if(!gs||!se||gs.isDisabled())
return false;gs.event=evnt;while(se&&(se.tagName!="TH"||se.id.length<gn.length||se.id.substr(0,gn.length)!=gn)&&(se.tagName!="DIV"||!se.getAttribute("groupInfo")))
se=se.parentNode;if(!se)
return;if(se.tagName=="TH")
{var column=igtbl_getColumnById(se.id);if(!column)return;var bandNo=column.Band.Index;var columnNo=column.Index;if(igtbl_button(gn,evnt)==0||gs.Element.getAttribute("mouseDown"))
{if(ig_shared.IsIE6&&igtbl_mouseDownX==evnt.x&&igtbl_mouseDownY==evnt.y)
return;var mode=gs.Element.getAttribute("elementMode");if(mode!=null&&mode=="resize")
igtbl_resizeDivMouseMove(evnt);else if(mode=="select"&&igtbl_getHeaderClickAction(gn,bandNo,columnNo)==1&&!evnt.ctrlKey)
igtbl_selectColumnRegion(gn,se);else
{var cursorName=se.getAttribute("oldCursor");if(cursorName!=null)
{se.style.cursor=cursorName;se.removeAttribute("oldCursor");}
if(igtbl_getHeaderClickAction(gn,bandNo,columnNo)!=1||gs.SelectedColumns[se.id]||igtbl_getSelectTypeColumn(gn,bandNo)<2)
if(column.AllowGroupBy==1&&gs.ViewType==2&&gs.GroupByBox.Element||column.Band.AllowColumnMoving>1)
{if(se.getAttribute("justClicked"))
{if(typeof(igtbl_headerDragStart)!="undefined")
igtbl_headerDragStart(gn,se,evnt);}
else
igtbl_changeStyle(gn,se,null);}}
if(se.getAttribute("justClicked"))
se.removeAttribute("justClicked");if((column.TemplatedColumn&1)&&se!=igtbl_srcElement(evnt))
return;igtbl_cancelEvent(evnt);return true;}
else
{var c,te=gs.Element;te.removeAttribute("elementMode");te.removeAttribute("resizeColumn");te.removeAttribute("selectMethod");if(!te.getAttribute("shiftSelect"))
te.removeAttribute("startColumn");if(igtbl_getOffsetX(evnt,se)>igtbl_clientWidth(se)-4&&igtbl_getAllowColSizing(gn,bandNo,columnNo)==2)
{if(se.getAttribute("oldCursor")==null)
se.setAttribute("oldCursor",se.style.cursor);se.style.cursor="w-resize";if((c=se.firstChild)!=null)if((c=c.firstChild)!=null)if((c=c.style)!=null)c.cursor="w-resize";}
else
{var cursorName=se.getAttribute("oldCursor");if(cursorName!=null)
{se.style.cursor=cursorName;se.removeAttribute("oldCursor");if((c=se.firstChild)!=null)if((c=c.firstChild)!=null)if((c=c.style)!=null)c.cursor=cursorName;}}}
if(se.getAttribute("justClicked"))
se.removeAttribute("justClicked");if((column.TemplatedColumn&1)&&se!=igtbl_srcElement(evnt))
return;}
else if(se.tagName=="DIV"&&se.getAttribute("groupInfo"))
{var groupInfo=se.getAttribute("groupInfo").split(":");if(groupInfo[0]!="band")
{if(igtbl_button(gn,evnt)==0||gs.Element.getAttribute("mouseDown"))
{var cursorName=se.getAttribute("oldCursor");if(cursorName!=null)
{se.style.cursor=cursorName;se.removeAttribute("oldCursor");}
igtbl_changeStyle(gn,se,null);if(gs.ViewType==2&&se.getAttribute("justClicked")&&typeof(igtbl_headerDragStart)!="undefined")
igtbl_headerDragStart(gn,se,evnt);}}
if(se.getAttribute("justClicked"))
se.removeAttribute("justClicked");return true;}
return false;}
function igtbl_tableMouseMove(evnt,gn)
{var gs=igtbl_getGridById(gn);var se=igtbl_srcElement(evnt);if(!gs||!se||gs.isDisabled())
return false;gs.event=evnt;var te=gs.Element;if(igtbl_button(gn,evnt)==0&&te.getAttribute("elementMode")=="resize")
{if((se.id==gn+"_div"||se.id==gn+"_hdiv"||se.tagName=="TABLE"&&se.parentNode.parentNode.getAttribute("hiddenRow")))
{igtbl_resizeDivMouseMove(evnt);return igtbl_cancelEvent(evnt);}
else if(se.tagName=="TR"&&se.getAttribute("hiddenRow")||se.id==gn+"_drs")
{igtbl_resizeDivMouseMove(evnt);return igtbl_cancelEvent(evnt);}}
else if(te.parentNode&&typeof(te.parentNode.oldCursor)=="string")
{te.parentNode.style.cursor=te.parentNode.oldCursor;if(gs.StatHeader)
gs.StatHeader.Element.parentNode.parentNode.style.cursor=te.parentNode.oldCursor;te.parentNode.oldCursor=null;}
if(se==te||se==gs.DivElement||se.tagName=="TH")
igtbl_colButtonMouseOut(evnt,gn);}
function igtbl_resizeRowMouseMove(e)
{var evnt=igtbl_event.getEvent(e);var se=igtbl_srcElement(evnt);var gn=igtbl_lastActiveGrid;if(!gn)return;var gs=igtbl_getGridById(gn);var te=gs.Element;if(te.getAttribute("resizeRow"))
{if(typeof(te.parentNode.oldCursor)!="string")
{te.parentNode.oldCursor=te.parentNode.style.cursor;te.parentNode.style.cursor="n-resize";}
var rowId=te.getAttribute("resizeRow");var row=igtbl_getElementById(rowId);if(!row||row.getAttribute("hiddenRow"))
return;var scrollTop=(ig_csom.IsIE6||ig_csom.IsIE7)&&igtbl_isXHTML?gs.DivElement.scrollTop:0;var r1h=row.offsetHeight+(evnt.clientY-((igtbl_getTopPos(row)-scrollTop)+row.offsetHeight));igtbl_resizeRow(gn,rowId,r1h);return igtbl_cancelEvent(evnt);}}
function igtbl_resizeRowMouseUp(e)
{if(!igtbl_lastActiveGrid)return;var gs=igtbl_getGridById(igtbl_lastActiveGrid);ig_csom.removeEventListener(document,"mousemove",igtbl_resizeRowMouseMove);ig_csom.removeEventListener(document,"mouseup",igtbl_resizeRowMouseUp);}
function igtbl_clearResizeDiv(gs,evnt,noForce)
{gs.Element.removeAttribute("elementMode");gs.Element.removeAttribute("resizeColumn");var resizeDiv=document.body.igtbl_resizeDiv;if(resizeDiv)
{resizeDiv.style.display="none";if(!noForce)
igtbl_resizeDivMouseUp(evnt);}
gs.Element.removeAttribute("mouseDown");}
function igtbl_tableMouseUp(evnt,gn)
{var gs=igtbl_getGridById(gn);if(!gs||gs.isDisabled())
return false;if(ig_csom.IsFireFox&&gs.Element.getAttribute("elementMode")=="resize")
{igtbl_resizeDivMouseUp(evnt)
return true;}
gs.event=evnt;var se=igtbl_srcElement(evnt);if(!se)return;if(se==gs._editorCurrent)return;if(gs.Element.getAttribute("elementMode")=="resize")
{if(se.id==gn+"_div")
{igtbl_clearResizeDiv(gs,evnt);}
else if(se.tagName=="TR"&&se.getAttribute("hiddenRow")||se.id==gn+"_drs")
igtbl_resizeDivMouseUp(evnt);}
var ar=gs.getActiveRow();if(ar&&!igtbl_isAChildOfB(se,ar.Element))
{gs.endEdit();if(ar.IsAddNewRow)
ar.commit();else
if(ar._dataChanged&&ar._dataChanged>1)
ar.processUpdateRow();}
if(!ig_shared.IsFireFox||!gs._isComboGrid)
igtbl_activate(gn);}
function igtbl_cellClickDown(evnt,gn)
{var gs=igtbl_getGridById(gn);if(!gs||gs.isDisabled())
return;gs.event=evnt;igtbl_lastActiveGrid=gn;gs._mouseDown=1;gs.Element.setAttribute("mouseDown","1");var se=igtbl_srcElement(evnt);if(!se||se.tagName=="IMG"&&se.getAttribute("imgType")=="expand")
return;if(''+se.contentEditable=='true')
return;igtbl_filterMouseUpDocument();if(!se||se==gs._editorCurrent)return;if((se.id==gn+"_vl")||(se.parentNode&&se.parentNode.id==gn+"_vl")){if(gs._focusElem)ig_cancelEvent(evnt);return;}
if(se.id==gn+"_tb"||se.id==gn+"_ta")
return;var sel=igtbl_getElementById(gn+"_vl");if(sel&&sel.style.display==""&&sel.getAttribute("noOnBlur"))
return igtbl_cancelEvent(evnt);var parentCell=igtbl_getParentCell(se);if(!ig_csom.IsNetscape6||!((se.tagName=="INPUT"&&se.type=="text"||se.tagName=="TEXTAREA")&&parentCell&&(parentCell.Column.TemplatedColumn&2)))
ig_cancelEvent(evnt);var se=igtbl_dom.find.parentByTag(se,["TD","TH"]);if(!se)
return;var row;var cell=igtbl_getCellByElement(se);var id=gs._mouseID=se.id;if(cell)
{row=cell.Row;id=cell.Element.id;}
else row=igtbl_getRowById(id);if(!row&&!cell)return;var fac=row.Band.firstActiveCell;if(igtbl_fireEvent(gn,gs.Events.MouseDown,"(\""+gn+"\",\""+id+"\","+igtbl_button(gn,evnt)+")")==true)
{igtbl_cancelEvent(evnt);return true;}
var band=row.Band;var bandNo=band.Index;if(igtbl_hideEdit(gn)&&!((band.getSelectTypeCell()==2||band.getSelectTypeCell()==3)&&band.getCellClickAction()==1&&cell&&!cell.getSelected()))return;if(igtbl_button(gn,evnt)==0&&!cell&&igtbl_getOffsetY(evnt,se)>igtbl_clientHeight(se)-4&&igtbl_getRowSizing(gn,bandNo,row.Element)==2&&!se.getAttribute("groupRow"))
{gs.Element.setAttribute("elementMode","resize");gs.Element.setAttribute("resizeRow",row.Element.id);row.Element.style.height=row.Element.offsetHeight;ig_csom.addEventListener(document,"mousemove",igtbl_resizeRowMouseMove);ig_csom.addEventListener(document,"mouseup",igtbl_resizeRowMouseUp);}
else
{var te=gs.Element;var workTableId;if((row.IsAddNewRow||row.IsFilterRow)&&row.Band.Index==0)
workTableId=gs.Element.id;else
if(se.getAttribute("groupRow"))
workTableId=se.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.id;else
workTableId=row.Element.parentNode.parentNode.id;if(igtbl_button(gn,evnt)!=0)
return;if(workTableId=="")
return;te.removeAttribute("lastSelectedCell");var prevSelRow=gs.SelectedRows[igtbl_getWorkRow(row.Element,gn).id];if(prevSelRow&&igtbl_getLength(gs.SelectedRows)>1)
prevSelRow=false;var selPresent=(igtbl_getLength(gs.SelectedCells)>0?1:0)|(igtbl_getLength(gs.SelectedRows)>0?2:0)|(igtbl_getLength(gs.SelectedCols)>0?4:0);if(se.getAttribute("groupRow")||!cell||igtbl_getCellClickAction(gn,bandNo)==2)
{if(!(igtbl_getSelectTypeRow(gn,bandNo)==3&&evnt.ctrlKey)&&!(row.getSelected()&&igtbl_getLength(gs.SelectedRows)==1))
igtbl_clearSelectionAll(gn);}
else
{if(!(igtbl_getSelectTypeCell(gn,bandNo)==3&&evnt.ctrlKey)&&!(cell.getSelected()&&igtbl_getLength(gs.SelectedCells)==1))
igtbl_clearSelectionAll(gn);}
gs.Element.setAttribute("elementMode","select");if(se.getAttribute("groupRow"))
{te.setAttribute("selectTable",se.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.id);te.setAttribute("selectMethod","row");}
else
{te.setAttribute("selectTable",workTableId);if(!cell||igtbl_getCellClickAction(gn,bandNo)==2)
te.setAttribute("selectMethod","row");else
te.setAttribute("selectMethod","cell");}
if(te.getAttribute("shiftSelect")&&evnt.shiftKey)
igtbl_selectRegion(gn,se);else
{if(!cell||igtbl_getCellClickAction(gn,bandNo)==2||se.getAttribute("groupRow"))
{var seRow=igtbl_getRowById(row.Element.id);if(gs.SelectedRows[row.Element.id]&&evnt.ctrlKey)
{igtbl_selectRow(gn,seRow,false);gs.setActiveRow(seRow);}
else
{var showEdit=true;if(!gs._exitEditCancel)
{if(gs.Activation.AllowActivation)
{var ar=gs.oActiveRow;if(ar!=seRow)
{gs.setActiveRow(seRow);showEdit=false;}
else
showEdit=true;}
if(igtbl_getSelectTypeRow(gn,bandNo)>1)
igtbl_selectRow(gn,seRow,true,!prevSelRow);if(showEdit&&!se.getAttribute("groupRow")&&row)
row.editRow();}}}
else
{if(cell.getSelected()&&evnt.ctrlKey)
{cell.select(false);cell.activate();}
else
{if(band.getSelectTypeCell()>1&&band.getCellClickAction()>=1&&!gs._exitEditCancel)
cell.select();else if(selPresent)
{var gsNPB=gs.NeedPostBack;igtbl_fireEvent(gn,gs.Events.AfterSelectChange,"(\""+gn+"\",\""+id+"\");");if(!gsNPB&&!(gs.Events.AfterSelectChange[1]&selPresent))
gs.NeedPostBack=false;}
cell.activate();}}
if(se.getAttribute("groupRow"))
te.setAttribute("startPointRow",se.parentNode.parentNode.parentNode.parentNode.parentNode.id);else
te.setAttribute("startPointRow",row.Element.id);te.setAttribute("startPointCell",id);te.removeAttribute("shiftSelect");if(!evnt.ctrlKey)
te.setAttribute("shiftSelect",true);}}
if(typeof(igtbl_currentEditTempl)!="undefined"&&igtbl_currentEditTempl!=null)
igtbl_gRowEditMouseDown(evnt);if(typeof(igcmbo_currentDropped)!="undefined"&&igcmbo_currentDropped!=null)
igcmbo_mouseDown(evnt);}
function igtbl_cellClickUp(evnt,gn)
{var gs=igtbl_getGridById(gn);if(!gs||gs.isDisabled())
return;gs.event=evnt;gs._mouseDown=0;if(igtbl_button(gn,evnt)==2)
return;if(gs.Element.getAttribute("mouseDown"))
gs.Element.removeAttribute("mouseDown");else
return;var se=igtbl_srcElement(evnt);if(!se||se==gs._editorCurrent||(se.tagName&&se.tagName.length>4))
{if(se&&se!=gs._editorCurrent)
{while(se&&(!(se.tagName=="TD"||se.parentNode&&se.parentNode.tagName=="TR"&&se.tagName=="TH")||se.id.length<gn.length||se.id.substr(0,gn.length)!=gn))
se=se.parentNode;if(se)
{if(igtbl_fireEvent(gn,gs.Events.MouseUp,"(\""+gn+"\",\""+se.id+"\","+igtbl_button(gn,evnt)+")")==true)
{igtbl_cancelEvent(evnt);return true;}}}
return;}
if(!gs._editorCurrent&&gs._focusElem&&!gs._focus0)
igtbl_activate(gn);if(se.id==gn+"_vl"||se.id==gn+"_tb"||se.id==gn+"_ta")
return;var sel=igtbl_getElementById(gn+"_vl");if(sel&&sel.style.display==""&&sel.getAttribute("noOnBlur"))
return igtbl_cancelEvent(evnt);if(se.tagName=="IMG"&&se.getAttribute("imgType")=="expand")
{igtbl_toggleRow(evnt);return;}
while(se&&(!(se.tagName=="TD"||se.parentNode&&se.parentNode.tagName=="TR"&&se.tagName=="TH")||se.id.length<gn.length||se.id.substr(0,gn.length)!=gn))
se=se.parentNode;if(!se)
return;if(se.tagName!="TD"&&!(se.parentNode&&se.parentNode.tagName=="TR"&&se.tagName=="TH"))
return;if(se.id=="")
return;var row;var id=se.id;var cell=igtbl_getCellById(id);if(cell)
{row=cell.Row;id=cell.Element.id;}
else row=igtbl_getRowById(id);if(!row&&!cell)return;var te=gs.Element;var mode=gs.Element.getAttribute("elementMode");gs.Element.removeAttribute("elementMode");te.removeAttribute("selectTable");te.removeAttribute("selectMethod");te.removeAttribute("resizeRow");var resizeDiv=document.body.igtbl_resizeDiv;if(resizeDiv)resizeDiv.style.display="none";if(!te.getAttribute("shiftSelect"))
{te.removeAttribute("startPointRow");te.removeAttribute("startPointCell");}
var bandNo=row.Band.Index;var fac=row.Band.firstActiveCell;if(cell&&igtbl_getCellClickAction(gn,bandNo)==1&&gs._mouseID==id)
{if(igtbl_getAllowUpdate(gn,bandNo,cell.Column.Index)==3&&cell.getEditable()!="no")
row.editRow(true);else
cell.beginEdit();}
var oldNPB=gs.NeedPostBack;if(!se.getAttribute("groupRow")&&mode!="resize")
{if(!cell)
igtbl_fireEvent(gn,gs.Events.RowSelectorClick,"(\""+gn+"\",\""+row.Element.id+"\","+igtbl_button(gn,evnt)+")");else
igtbl_fireEvent(gn,gs.Events.CellClick,"(\""+gn+"\",\""+id+"\","+igtbl_button(gn,evnt)+")");}
gs._noCellChange=false;if(igtbl_fireEvent(gn,gs.Events.MouseUp,"(\""+gn+"\",\""+id+"\","+igtbl_button(gn,evnt)+")")==true)
{igtbl_cancelEvent(evnt);return true;}
if((mode=="resize"||mode=="select")&&oldNPB)
{se=igtbl_srcElement(evnt);if(!(se&&se.tagName=="INPUT"&&se.type=="checkbox"))
igtbl_doPostBack(gn);return;}
if(gs.NeedPostBack&&(!cell||igtbl_getCellClickAction(gn,bandNo)==2))
igtbl_doPostBack(gn,'RowClick:'+row.Element.id+(row.Element.getAttribute("level")?"\x05"+row.Element.getAttribute("level"):""));else if(gs.NeedPostBack)
igtbl_doPostBack(gn,'CellClick:'+id+(cell.Element.getAttribute("level")?"\x05"+cell.Element.getAttribute("level"):""));var ctd=false;for(var gId in igtbl_gridState)
if(gId!=gn)
{igtbl_globalMouseUp(evnt,gId);ctd=true;}
if(ctd&&!igtbl_inEditMode(gn))
window.setTimeout("igtbl_activate('"+gn+"');",0);return igtbl_cancelEvent(evnt);}
function igtbl_cellContextMenu(evnt,gn)
{var gs=igtbl_getGridById(gn);if(!gs||gs.isDisabled())
return;gs.event=evnt;var te=gs.Element;te.removeAttribute("mouseDown");te.removeAttribute("elementMode");te.removeAttribute("resizeColumn");te.removeAttribute("selectMethod");if(!te.getAttribute("shiftSelect"))
te.removeAttribute("startColumn");var se=igtbl_srcElement(evnt);if(!se||se.id==gn+"_vl"||se.id==gn+"_tb"||se.id==gn+"_ta")
return;while(se&&!(se.tagName=="TD"||se.parentNode&&se.parentNode.tagName=="TR"&&se.tagName=="TH"))
se=se.parentNode;if(!se||(se.tagName!="TD"&&se.tagName!="TH"))
return;var row;var cell=igtbl_getCellByElement(se);var id=se.id;if(cell)
{row=cell.Row;id=cell.Element.id;}
else row=igtbl_getRowById(id);if(!row&&!cell)return;if(!se.getAttribute("groupRow"))
{if(!cell)
igtbl_fireEvent(gn,gs.Events.RowSelectorClick,"(\""+gn+"\",\""+row.Element.id+"\",2)");else
igtbl_fireEvent(gn,gs.Events.CellClick,"(\""+gn+"\",\""+id+"\",2)");}
if(igtbl_fireEvent(gn,gs.Events.MouseUp,"(\""+gn+"\",\""+id+"\",2)")==true)
return igtbl_cancelEvent(evnt);if(gs.NeedPostBack&&(!cell||igtbl_getCellClickAction(gn,row.Band.Index)==2))
igtbl_doPostBack(gn,'RowClick:'+row.Element.id+(row.Element.getAttribute("level")?"\x05"+row.Element.getAttribute("level"):""));else if(gs.NeedPostBack)
igtbl_doPostBack(gn,'CellClick:'+id+(cell.Element.getAttribute("level")?"\x05"+cell.Element.getAttribute("level"):""));}
function igtbl_cellMouseOver(evnt,gn)
{var gs=igtbl_getGridById(gn);var se=igtbl_srcElement(evnt);if(!gs||!se||gs.isDisabled())
return;gs.event=evnt;try{if(se.nodeName=="TD"||(se.nodeName=="DIV"&&''+se.contentEditable!='true'))se.unselectable="on";}catch(ex){;}
if(se.tagName=="NOBR")
{var cell=igtbl_getCellByElement(se.parentNode);if(cell)
{var nobr=cell.Element.childNodes[0];if(cell.Element.title)
{nobr.title=cell.Element.title;}
else
{var showTitle=nobr.offsetWidth>cell.Element.offsetWidth||nobr.offsetHeight>cell.Element.offsetHeight||(cell.Element.style.textOverflow=="ellipsis"&&nobr.offsetWidth+6>cell.Element.offsetWidth)||(cell.Element.currentStyle&&cell.Element.currentStyle.textOverflow=="ellipsis"&&nobr.offsetWidth+6>cell.Element.offsetWidth);var titleMode=cell.getTitleModeResolved();showTitle|=titleMode==igtbl_CellTitleMode.Always;showTitle&=titleMode!=igtbl_CellTitleMode.Never;if(showTitle)
{if(gs.Section508Compliant&&titleMode!=igtbl_CellTitleMode.OnOverflow)
{var row=cell.Row;if(row)
{var fmtStr=(row.ParentRow)?gs._childRowToolTipFormatStr:gs._rowToolTipFormatStr;fmtStr=fmtStr.replace("{0}",(1+row.getIndex()).toString());fmtStr=fmtStr.replace("{1}",(cell.Column.HeaderText));if(igtbl_string.trim(cell.MaskedValue))
{nobr.title=fmtStr.replace("{2}",cell.MaskedValue);}
else
{nobr.title=fmtStr.replace("{2}",cell.getValue(true));}}}
else
if(igtbl_string.trim(cell.MaskedValue))
{nobr.title=cell.MaskedValue;}
else
{nobr.title=cell.getValue(true);}}}}
se=se.parentNode;}
while(se&&!(se.tagName=="TD"||se.parentNode&&se.parentNode.tagName=="TR"&&se.tagName=="TH"))
se=se.parentNode;if(!se||se.tagName!="TD"&&se.tagName!="TH")
return;var row;var cell=igtbl_getCellByElement(se);var id=se.id;if(cell)
{if(!cell.Element)return;row=cell.Row;id=cell.Element.id;}
else row=igtbl_getRowById(se.id);if(!row&&!cell)return;if(se._hasMouse)return;se._hasMouse=true;var te=gs.Element;if(evnt.shiftKey&&row.Band.getSelectTypeRow()==3&&!te.getAttribute("shiftSelect"))
te.setAttribute("shiftSelect",true);if(igtbl_fireEvent(gn,gs.Events.MouseOver,"(\""+gn+"\",\""+id+"\",0)")==true)
return;}
function igtbl_cellMouseMove(evnt,gn)
{var se=igtbl_srcElement(evnt);var gs=igtbl_getGridById(gn);if(!gs||!se||gs.isDisabled())
return;gs.event=evnt;var te=gs.Element;if(se.id==gn+"_vl"||se.id==gn+"_tb"||se.id==gn+"_ta")
return;if(te.getAttribute("resizeRow")&&(se.tagName=="TH"&&se.parentNode.parentNode.tagName=="TFOOT"||se.tagName=="TD"&&se.parentNode.getAttribute("hiddenRow")))
return igtbl_tableMouseMove(evnt,gn);while(se&&!(se.tagName=="TD"||se.parentNode&&se.parentNode.tagName=="TR"&&se.tagName=="TH"))
se=se.parentNode;if(!se||se.tagName!="TD"&&se.tagName!="TH")
return;var row;var cell=igtbl_getCellByElement(se);var id=se.id;if(cell)
{row=cell.Row;if(!cell||!cell.Element)return;id=cell.Element.id;}
else row=igtbl_getRowById(se.id);if(!row&&!cell)
return;if(cell&&cell.Row.IsFilterRow)return;var bandNo=row.Band.Index;var fac=row.Band.firstActiveCell;if(igtbl_button(gn,evnt)==0)
{var mode=te.getAttribute("elementMode");if(!cell)
{var cursorName=se.getAttribute("oldCursor");if(cursorName!=null)
{se.style.cursor=cursorName;se.removeAttribute("oldCursor");}}
if(mode&&mode=="select"&&!evnt.ctrlKey)
{var lsc=te.getAttribute("lastSelectedCell");if(!lsc||lsc!=se.id)
igtbl_selectRegion(gn,se);te.setAttribute("lastSelectedCell",id);}}
else if(igtbl_getOffsetY(evnt,se)>igtbl_clientHeight(se)-4&&!cell&&igtbl_getRowSizing(gn,bandNo,row.Element)==2)
{var cursorName=se.getAttribute("oldCursor");if(cursorName==null)
se.setAttribute("oldCursor",se.style.cursor);se.style.cursor="n-resize";igtbl_colButtonMouseOut(null,gn);}
else
{te.removeAttribute("elementMode");te.removeAttribute("resizeRow");var cursorName=se.getAttribute("oldCursor");if(cursorName!=null)
{se.style.cursor=cursorName;se.removeAttribute("oldCursor");}
if(!cell)
igtbl_colButtonMouseOut(null,gn);else
{var column=(cell?cell.Column:null);if(cell&&cell.hasButtonEditor(igtbl_cellButtonDisplay.OnMouseEnter))
{if(gs._editorButton&&gs._editorButton.style.display!="")
if(gs._mouseWait++>5)
gs._mouseWait=0;if(gs._mouseIn!=id)
igtbl_showColButton(gn,cell.Element);}
else
igtbl_colButtonMouseOut(null,gn);}}
gs._mouseIn=id;return false;}
function igtbl_cellMouseOut(evnt,gn)
{var gs=igtbl_getGridById(gn);var se=igtbl_srcElement(evnt);if(!gs||!se||gs.isDisabled())
return;gs.event=evnt;if(se.tagName=="NOBR")
{var cell=igtbl_getCellByElement(se.parentNode);if(cell)
cell.Element.childNodes[0].title="";se=se.parentNode;}
while(se&&!(se.tagName=="TD"||se.parentNode&&se.parentNode.tagName=="TR"&&se.tagName=="TH"))
se=se.parentNode;if(!se||se.tagName!="TD"&&se.tagName!="TH")
return;var row;var cell=igtbl_getCellByElement(se);var id=se.id;if(cell)
{if(!cell.Element)return;row=cell.Row;id=cell.Element.id;var btn=igtbl_getElementById(gn+"_bt")
if(btn&&btn.style.display==""&&btn.getAttribute("srcElement")==id&&evnt.toElement&&evnt.toElement.id!=id&&evnt.toElement.id!=gn+"_bt")
igtbl_colButtonMouseOut(null,gn);}
else row=igtbl_getRowById(id);if(!row&&!cell)return;if(igtbl_isMouseOut(se,evnt))
igtbl_fireEvent(gn,gs.Events.MouseOut,"(\""+gn+"\",\""+id+"\",0)");}
function igtbl_cellDblClick(evnt,gn)
{var gs=igtbl_getGridById(gn);if(!gs||gs.isDisabled())
return;gs.event=evnt;var se=igtbl_srcElement(evnt);if(!se||se.id==gn+"_vl"||se.id==gn+"_tb"||se.id==gn+"_ta")
return;while(se&&(se.tagName!="TD"&&se.tagName!="TH"||se.id.length<gn.length||se.id.substr(0,gn.length)!=gn))
se=se.parentNode;if(!se)
return;if(se.tagName!="TD"&&se.tagName!="TH")
return;var row;var id=se.id;var cell=igtbl_getCellById(id);if(cell)
{row=cell.Row;id=cell.Element.id;}
else row=igtbl_getRowById(se.id);var column=igtbl_getColumnById(se.id);if(!row&&!cell&&!column)return;if(se.tagName=="TD"||se.tagName=="TH"&&row)
{if(se.getAttribute("groupRow"))
{igtbl_toggleRow(gn,row.Element.id);return;}
if(igtbl_fireEvent(gn,gs.Events.DblClick,"(\""+gn+"\",\""+id+"\")")==true)
return;if(row&&!cell)
{if(gs.NeedPostBack)
igtbl_doPostBack(gn,'RowDblClick:'+row.Element.id+(row.getLevel(true)?"\x05"+row.getLevel(true):""));return;}
var bandNo=row.Band.Index;if(gs.NeedPostBack)
{if(igtbl_getCellClickAction(gn,bandNo)==2)
igtbl_doPostBack(gn,'RowDblClick:'+row.Element.id+(row.getLevel(true)?"\x05"+row.getLevel(true):""));else
igtbl_doPostBack(gn,'CellDblClick:'+id+(cell.getLevel(true)?"\x05"+cell.getLevel(true):""));return;}
if(igtbl_getCellClickAction(gn,bandNo)==0)
return;var cancelEdit=gs._exitEditCancel;var activeRow=gs.getActiveRow();if(activeRow&&activeRow!=row)
{cancelEdit|=gs.fireEvent(gs.Events.BeforeRowDeactivate,[gs.Id,activeRow.Element.id])==true;cancelEdit|=gs.fireEvent(gs.Events.BeforeRowActivate,[gs.Id,row.Element.id])==true;}
if(!cancelEdit)
{if(cell.Column.getAllowUpdate()==3&&cell.getEditable()!="no"&&!cell.Row.IsFilterRow)
row.editRow(true);else
cell.beginEdit();}}
else
{if(igtbl_fireEvent(gn,gs.Events.DblClick,"(\""+gn+"\",\""+se.id+"\")")==true)
return;if(gs.NeedPostBack)
igtbl_doPostBack(gn,'HeaderDblClick:'+se.id);}}
var igtbl_dontHandleChkBoxChange=false;function igtbl_chkBoxChange(evnt,gn)
{if(igtbl_dontHandleChkBoxChange||(ig_csom.IsIE&&evnt.propertyName!="checked")||(!ig_csom.IsIE&&evnt.type!="change"))
return false;var se=igtbl_srcElement(evnt);if(!se)return false;var c=se.parentNode;while(c&&!(c.tagName=="TD"&&c.id!=""))
c=c.parentNode;if(!c)return;var s=se;var cell=igtbl_getCellById(c.id);if(!cell)return;if(!evnt&&event)evnt=event;if(!gn)gn=cell.Band.Grid.Id;var column=cell.Column;var gs=igtbl_getGridById(gn);gs.event=evnt;var oldValue=!s.checked;if(gs._exitEditCancel||!cell.isEditable()||igtbl_fireEvent(gn,gs.Events.BeforeCellUpdate,"(\""+gn+"\",\""+c.id+"\",\""+s.checked+"\")"))
{igtbl_dontHandleChkBoxChange=true;s.checked=oldValue;igtbl_dontHandleChkBoxChange=false;return true;}
cell.Row._dataChanged|=2;if(typeof(cell._oldValue)=="undefined")
cell._oldValue=oldValue;igtbl_saveChangedCell(gs,cell,s.checked.toString());cell.Value=cell.Column.getValueFromString(s.checked);if(!c.getAttribute("oldValue"))
c.setAttribute("oldValue",s.checked);if(c.getAttribute(igtbl_sUnmaskedValue))
c.setAttribute(igtbl_sUnmaskedValue,s.checked.toString());c.setAttribute("chkBoxState",s.checked.toString());var cca=igtbl_getCellClickAction(gn,column.Band.Index);if(cca==1||cca==3)
igtbl_setActiveCell(gn,c);else if(cca==2)
igtbl_setActiveRow(gn,c.parentNode);if(cell.Node)
{cell.setNodeValue(!s.checked?"False":"True");var cdata=cell.Node.firstChild;if(s.checked)
cdata.text=cdata.text.replace("type='checkbox'","type='checkbox' checked");else
cdata.text=cdata.text.replace(" checked","");gs.invokeXmlHttpRequest(gs.eReqType.UpdateCell,cell,s.checked);}
else if(ig_csom.IsNetscape6)
gs.invokeXmlHttpRequest(gs.eReqType.UpdateCell,cell,s.checked);igtbl_fireEvent(gn,gs.Events.AfterCellUpdate,"(\""+gn+"\",\""+c.id+"\",\""+s.checked+"\")");if(gs.LoadOnDemand==3)
gs.NeedPostBack=false;if(gs.NeedPostBack)
igtbl_doPostBack(gn);return false;}
function igtbl_colButtonClick(evnt,gn,b,se)
{if(!b)b=igtbl_getElementById(gn+"_bt");if(b&&se==null)
se=igtbl_getElementById(b.getAttribute("srcElement"));if(se==null||!se.id)
{if(!se)
se=igtbl_srcElement(evnt).parentNode;else
se=se.parentNode;if(se&&se.tagName=="NOBR")
do
{se=se.parentNode;}while(se&&se.tagName!="TD");}
var gs=igtbl_getGridById(gn);if(gs==null||gs._exitEditCancel||se==null||se.id==""||gs.isDisabled())
return;gs.event=evnt;var cell=igtbl_getCellById(se.id);if(!cell)return;var sel=cell!=gs.oActiveCell;try
{if(sel&&igtbl_isChild(gn,cell.Element))cell.activate();}catch(e){}
igtbl_fireEvent(gn,gs.Events.ClickCellButton,"(\""+gn+"\",\""+se.id+"\")");if(gs.NeedPostBack)
{var cellInfo=cell.Row._generateUpdateRowSemaphore(false);if(igtbl_doPostBack(gn,'CellButtonClick:'+se.id+"\x05"+cell.getLevel(true)+(gs.LoadOnDemand==3?"\x02"+cellInfo+"\x02"+cell.Row.Band.Index+"\x02"+cell.Row.DataKey:"")))
return;}}
function igtbl_colButtonMouseOut(evnt,gn)
{var gs=igtbl_getGridById(gn);if(gs==null||gs.isDisabled())return;var b=igtbl_getElementById(gn+"_bt");if(!b||b.getAttribute("noOnBlur"))
return;if(evnt&&evnt.toElement&&evnt.toElement.id==b.getAttribute("srcElement"))
return;if(b.style.display=="")
{b.setAttribute("noOnBlur",true);b.style.display="none";b.removeAttribute("srcElement");if(!gs.Activation.AllowActivation)
return;if(gs.oActiveCell)
{if(gs.oActiveCell.hasButtonEditor(igtbl_cellButtonDisplay.OnMouseEnter))
igtbl_showColButton(gn,gs.oActiveCell.Element);}
window.setTimeout("igtbl_clearNoOnBlurBtn('"+gn+"')",100);gs._mouseIn=null;}}
function igtbl_colButtonEvent(evnt,gn)
{}
function igtbl_dropDownChange(evnt,gn)
{var sel=null;if(!gn)
{sel=igtbl_srcElement(evnt);gn=sel.id.substring(0,sel.id.length-3);}
else
{sel=igtbl_getElementById(gn+"_vl");}
igtbl_getGridById(gn).event=evnt;if(sel&&sel.style.display=="")
igtbl_fireEvent(gn,igtbl_getGridById(gn).Events.ValueListSelChange,"(\""+gn+"\",\""+gn+"_vl\",\""+sel.getAttribute("currentCell")+"\");");}
function igtbl_fixedClick(evnt)
{var se=igtbl_srcElement(evnt);if(!se)return;var pn=se.parentNode;while(pn&&pn.tagName!="TH")pn=pn.parentNode;if(!pn||!pn.id)return;var column=igtbl_getColumnById(pn.id);if(column.Band.Grid.UseFixedHeaders)
{if(column.getFixed())
igtbl_doPostBack(column.Band.Grid.Id,"Unfix:"+column.Band.Index+":"+column.Index);else
igtbl_doPostBack(column.Band.Grid.Id,"Fix:"+column.Band.Index+":"+column.Index);return igtbl_cancelEvent(evnt);}}
function igtbl_mouseWheel(evnt,gn)
{var gs=igtbl_getGridById(gn);if(!gs||!gs._scrElem||gs.isDisabled())return;if(evnt.wheelDelta)
{igtbl_hideEdit(gn);gs._scrElem.scrollTop-=evnt.wheelDelta/3;}}
if(typeof(ig_csom)!="undefined")
ig_csom.addEventListener(document.documentElement,"mousewheel",igtbl_globalMouseWheel);function igtbl_globalMouseWheel(evnt)
{var scrElem=evnt.srcElement?evnt.srcElement:evnt.target;var hideEdit=true;for(var gn in igtbl_gridState)
{if(scrElem==null||scrElem.id!=gn+"_vl")
{if(typeof(igcmbo_getComboById)=="function")
{var g=igtbl_getGridById(gn);if(igtbl_getElementById(g.Id)!=null)
{var comboId=igtbl_getElementById(g.Id).getAttribute("igComboId");var src=igcmbo_srcElement(evnt);if(comboId)
{if(igtbl_isAChildOfB(src,g.Element.parentNode))
{hideEdit=false;}}
else
{if(g._editorCurrent&&igcmbo_displaying&&igcmbo_displaying.Element.id==g._editorCurrent.id)
{if(igtbl_isAChildOfB(src,igcmbo_displaying.getGrid().Element.parentNode))
{hideEdit=false;}}}}}
if(hideEdit)
{igtbl_hideEdit(gn);var grid=igtbl_getGridById(gn);for(var b=0;b<grid.Bands.length;b++)
{for(var c=0;c<grid.Bands[b].Columns.length;c++)
for(var v=0;v<grid.Bands[b].Columns[c].Validators.length;v++)
{if(document.getElementById(grid.Bands[b].Columns[c].Validators[v])&&!document.getElementById(grid.Bands[b].Columns[c].Validators[v]).isvalid)
{return false;}}}}
hideEdit=true;}}}
function igtbl_onScrollFixed(evnt,gn)
{var g=igtbl_getGridById(gn)
if(!g||!g._scrElem)return;var s=g.Element.parentNode.scrollTop;igtbl_scrollTop(g.Element.parentNode,0);igtbl_scrollTop(g.Element.parentNode.parentNode,0);igtbl_scrollTop(g._scrElem,s);}
function igtbl_onResizeFixed(evnt,gn)
{var g=igtbl_getGridById(gn)
if(!g||!g._scrElem)return;if(g.Element.getAttribute("noOnResize"))
{if(g._scrElem.getAttribute("oldW"))
g._scrElem.style.width=g._scrElem.getAttribute("oldW");if(g._scrElem.getAttribute("oldH"))
g._scrElem.style.height=g._scrElem.getAttribute("oldH");return igtbl_cancelEvent(evnt);}
if(!g._scrElem.style.width||g._scrElem.style.width.charAt(g._scrElem.style.width.length-1)!="%")
g._scrElem.setAttribute("oldW",g._scrElem.offsetWidth);if(!g._scrElem.style.height||g._scrElem.style.height.charAt(g._scrElem.style.height.length-1)!="%")
g._scrElem.setAttribute("oldH",g._scrElem.offsetHeight);}
function igtbl_onStationaryMarginScroll(evnt,gn,marginId)
{var gs=igtbl_getGridById(gn);var marginElement=document.getElementById(marginId);if(marginElement&&marginElement.scrollLeft!=0&&!gs.UseFixedHeaders)
{var scrollLeft=marginElement.scrollLeft+-marginElement.childNodes[0].offsetLeft;marginElement.scrollLeft=0;gs.DivElement.scrollLeft=scrollLeft;}
else if(marginElement&&marginElement.scrollLeft!=0&&gs.UseFixedHeaders)
{var offsetDiv=marginElement.childNodes[0].childNodes[1].childNodes[0].childNodes[0].childNodes[0];if(offsetDiv.id!=gn+"_drs")
{offsetDiv=marginElement.childNodes[0].childNodes[1].childNodes[0].childNodes[2].childNodes[0];}
var scrollLeft=marginElement.scrollLeft+-offsetDiv.childNodes[0].offsetLeft;marginElement.scrollLeft=0;document.getElementById(gn+"_divscr").scrollLeft=scrollLeft;}}
var igtbl_oldOnBodyResize;function igtbl_onBodyResize()
{var result;if(igtbl_oldOnBodyResize)
{result=igtbl_oldOnBodyResize();}
if(!document.body.getAttribute("noOnBodyResize"))
for(var gridId in igtbl_gridState)
{var grid=igtbl_getGridById(gridId);if(!grid||!grid.MainGrid||!grid.MainGrid.parentNode)
continue;if(igtbl_inEditMode(gridId)&&!ig_shared.IsIE6)
{igtbl_hideEdit(gridId);}
grid.alignStatMargins();}
return result;}
igtbl_oldSelectTab=null;function igtbl_tabChanges(tab,index)
{var selectedTab=tab.getSelectedTab();if(selectedTab&&selectedTab.index!=index)
{for(var gId in igtbl_gridState)
{var g=igtbl_getGridById(gId);if(igtbl_isAChildOfB(g.MainGrid,selectedTab.elemDiv))
{var pn=g.MainGrid.parentNode;if(!pn.id||pn.id.length<=10||pn.id.substr(pn.id.length-10,10)!="_container")
g.hide();igtbl_showColButton(gId,"hide");}}}
igtbl_oldSelectTab(tab,index,arguments[2]);selectedTab=tab.getSelectedTab();if(selectedTab)
{for(var gId in igtbl_gridState)
{var g=igtbl_getGridById(gId);if(igtbl_isAChildOfB(g.MainGrid,selectedTab.elemDiv))
{g.show();}}}}
function igtbl_onPagerClick(gn,evnt)
{var g=igtbl_getGridById(gn);if(!g||!evnt)return;if(!g.isLoaded())
return ig_cancelEvent(evnt);}
function igtbl_showFilterOptions(columnId,evnt)
{var col=igtbl_getColumnById(columnId);if(col.Band.Grid.Element.getAttribute("elementMode")=="resize")return;var x=igtbl_button(null,evnt);var parentEl=evnt.srcElement;if(!parentEl)parentEl=evnt.target;do
{if(parentEl.getAttribute("GroupByHeaderFloatingDiv"))
return;parentEl=parentEl.parentNode;}while(parentEl&&parentEl.tagName!="BODY")
if(!col||!evnt||!(ig_csom.IsNetscape6&&evnt.button==0||ig_csom.IsIE&&evnt.button==1))return;var gs=col.Band.Grid;var ar=gs.getActiveRow();if(ar)
{gs.endEdit();if(ar.IsAddNewRow)
ar.commit();else
if(ar._dataChanged&&ar._dataChanged>1)
ar.processUpdateRow();}
col.showFilterDropDown();return ig_cancelEvent(evnt);}
function igtbl_filterOptionMouseUp(evnt)
{if(!evnt||(ig_csom.IsIE&&!evnt.srcElement)||(ig_csom.IsFireFox&&!evnt.target))return;var src=ig_csom.IsIE?evnt.srcElement:evnt.target;while(src&&!src.getAttribute("fo"))
src=src.parentNode;if(!src)return;var value=src.getAttribute("value");var filterObject=src.parentNode.parentNode._filterObject;var band=filterObject.Column.Band;var grid=band.Grid;if(filterObject._setFilter(value))return;filterObject.applyFilter();grid.alignStatMargins();grid.alignDivs();grid.fireEvent(grid.Events.AfterRowFilterApplied,[grid.Id,filterObject.Column]);}
function igtbl_GridCornerClick()
{if(ig_csom.IsIE)
{var elem=event.srcElement;var gridName=elem.getAttribute("gridName");var g=igtbl_getGridById(gridName);if(g)
{igtbl_fireEvent(g.Id,g.Events.GridCornerImageClick,'("'+g.Id+'");');}}}
var igtbl_oldGlobalMouseMove;function igtbl_globalMouseMove(evnt)
{if(!evnt)
evnt=event;if(igtbl_oldGlobalMouseMove)
{igtbl_oldGlobalMouseMove(evnt);}
if(typeof(igtbl_gridState)!="undefined"&&igtbl_gridState)
{for(var gId in igtbl_gridState)
{var gs=igtbl_gridState[gId];if(gs&&gs.Element)
{var scrElem=evnt.srcElement?evnt.srcElement:evnt.target;if(!ig_isAChildOfB(evnt.srcElement,gs.Element))
{if(gs.Element.getAttribute("elementMode")=="resize")
{if(igtbl_button(gId,evnt)==-1)
{igtbl_clearResizeDiv(gs,evnt,true);}}}}}}}
function _igtbl_processServerPassedColumnFilters(serverFilterInfo,g)
{var itrCount=serverFilterInfo.length;for(var itr=0;itr<itrCount;itr++)
{var filterInfo=serverFilterInfo[itr];if(!filterInfo)break;if(filterInfo[0])
{var row=igtbl_getRowById(filterInfo[0]);if(row)
{var parentTableId="";var workingBand;if(row.Rows&&row.Rows.Element)
{var parentTable=row.Rows.Element;do
{parentTable=parentTable.parentNode;}while(parentTable&&!(parentTable.tagName=="TABLE"&&parentTable.id.length>0))
if(!parentTable)continue;parentTableId=parentTable.id;workingBand=row.Rows.Band;}
else
{parentTableId=filterInfo[0].replace("_r","_t");workingBand=g.Bands[row.Band.Index+1];}
filterPanel=workingBand._filterPanels[parentTableId];if(filterPanel)
filterPanel=filterPanel[filterInfo[1]];else
{workingBand._filterPanels[parentTableId]=new Object();}
if(!filterPanel)
{filterPanel=workingBand._filterPanels[parentTableId][filterInfo[1]]=new igtbl_FilterDropDown(igtbl_getColumnById(filterInfo[1]));if(row)
filterPanel.RowIsland=row.Rows;}
filterPanel.setFilter(filterInfo[2],filterInfo[3],true);}}
else
{var band=g.Bands[0];var filteredColumn=igtbl_getColumnById(filterInfo[1]);if(!band._filterPanels[filteredColumn.Id])
{band._filterPanels[filteredColumn.Id]=new igtbl_FilterDropDown(filteredColumn);}
filterPanel=band._filterPanels[filteredColumn.Id];filterPanel.RowIsland=filteredColumn.Band.Grid.Rows;filterPanel.setFilter(filterInfo[2],filterInfo[3],true);}}}
function _igtbl_setFilterIndicators(colFilters,rows)
{if(colFilters&&colFilters.length>0)
{_igtbl_setFilterIndicators(undefined,rows);var itrCount=colFilters.length;for(var itr=0;itr<itrCount;itr++)
{var column=igtbl_getColumnById(colFilters[itr][1]);_igtbl_setColumnFilterIndicator(column,true,rows);}}
else if(rows)
{var band=rows.Band;for(var itr=0;itr<band.Columns.length;itr++)
{_igtbl_setColumnFilterIndicator(band.Columns[itr],false,rows);}}}
function _igtbl_setColumnFilterIndicator(column,isFiltered,rows)
{var filterImageSrc=column.Band.Grid.FilterDefaultImage;if(isFiltered)
filterImageSrc=column.Band.Grid.FilterAppliedImage;var band=column.Band;if(band.RowFilterMode==1||band.Index==0)
{for(var itr=0;itr<band.Columns.length;itr++)
{var headerTags=column._getHeadTags();if(headerTags)
{for(var cnt=0;cnt<headerTags.length;cnt++)
_igtbl_changeFilterImage(column,headerTags[cnt],filterImageSrc);}}}
else
{for(var itr=0;itr<band.Columns.length;itr++)
{var myDirectColumnHeader=igtbl_getChildElementById(rows.Element.parentNode,column.Id);if(myDirectColumnHeader)
_igtbl_changeFilterImage(column,myDirectColumnHeader,filterImageSrc);}}}
function _igtbl_changeFilterImage(column,header,newImageSrc)
{var filterImg=column._findFilterImage(header);if(filterImg)
{filterImg.src=newImageSrc;var alt=filterImg.getAttribute("alt");if(alt!=null)
{var clpsAlt=filterImg.getAttribute("igAltF1");if(clpsAlt!=null){filterImg.setAttribute("igAltF0",alt);filterImg.setAttribute("alt",clpsAlt);filterImg.removeAttribute("igAltF1");}}}}
function _igtbl_containsColumnFilter(column_Id,colFilters)
{if(!colFilters||colFilters.length==0)
return false;for(var x=0;x<colFilters.length;x++)
{if(column_Id==colFilters[x][1])
return true;}
return false;}
function igtbl_filterGridScroll()
{for(var gridId in igtbl_gridState)
{var g=igtbl_getGridById(gridId);if(g._currentFilterDropped)
g._currentFilterDropped.show(false);}}
function igtbl_filterTypeKeyDown(evnt)
{var src=evnt.srcElement?evnt.srcElement:evnt.target;while(src&&src.tagName!="TD")
{src=src.parentNode;}
var cell=src.Object;if(!cell)cell=igtbl_getCellById(src.id);switch(evnt.keyCode)
{case(27):{var col=cell.Column;if(col.FilterIconsList!=null)
{col.FilterIconsList.show();}
igtbl_cancelEvent(evnt);}
case(9):{}}}
function igtbl_filterTypeSelect(evnt)
{var src=evnt.srcElement?evnt.srcElement:evnt.target;while(src&&src.tagName!="TD")
{src=src.parentNode;}
var cell=src.Object;if(!cell)cell=igtbl_getCellById(src.id);var col=cell.Column;if(col.FilterIconsList==null)
{col.FilterIconsList=new igtbl_FilterIconsList(col);}
col.FilterIconsList.show(cell);igtbl_cancelEvent(evnt);}
function igtbl_filterIconsMouseUp(evnt)
{if(!evnt||(ig_csom.IsIE&&!evnt.srcElement)||(ig_csom.IsFireFox&&!evnt.target))return;var src=ig_csom.IsIE?evnt.srcElement:evnt.target;while(src&&!src.getAttribute("filterListOption"))
src=src.parentNode;if(!src)return;var value=src.getAttribute("operator");var filterIconSource=src;while(filterIconSource&&!filterIconSource.getAttribute("filterIconList"))
filterIconSource=filterIconSource.parentNode;filterIconSource=filterIconSource.object;var filterCell=filterIconSource._currentCell;filterCell._setFilterTypeImage(value);var columnFilter=filterCell.Column._getFilterPanel(filterCell.Row.Element);if(columnFilter.IsActive())
{var compareValue=columnFilter.getEvaluationValue();if(compareValue==null||compareValue=="")value=igtbl_filterComparisionOperator.All;columnFilter.setFilter(value,compareValue);columnFilter.applyFilter();var gs=filterCell.Column.Band.Grid;gs.fireEvent(gs.Events.AfterRowFilterApplied,[gs.Id,filterCell.Column]);}
else
{columnFilter.setOperator(value);}}
function igtbl_clearNoOnBlurBtn(gn)
{var b=igtbl_getElementById(gn+"_bt");b.removeAttribute("noOnBlur");}
function igtbl_clearNoOnBlurElem(id)
{var e=igtbl_getElementById(id);if(e)e.removeAttribute("noOnBlur");}
function igtbl_cancelNoOnBlurTB(gn,id)
{if(id)
{var src=igtbl_getElementById(id);if(src)
{src.removeAttribute("noOnBlur");return;}}
var textBox=igtbl_getElementById(gn+"_tb");if(textBox&&textBox.style.display=="")
textBox.removeAttribute("noOnBlur");var sel=igtbl_getElementById(gn+"_vl");if(sel&&sel.style.display=="")
sel.removeAttribute("noOnBlur");}
function igtbl_cancelNoOnBlurDD(gn)
{if(arguments.length==0)
gn=igtbl_lastActiveGrid;var gs=igtbl_getGridById(gn);if(gs&&(gs.editorControl||gs._editorCustom))
{if(gs.editorControl)
gs.editorControl.Element.removeAttribute("noOnBlur");else
gs._editorCustom.Element.removeAttribute("noOnBlur");}}
function igtbl_blur(gn)
{window.setTimeout("igtbl_blurTimeout('"+gn+"')",100);}
function igtbl_blurTimeout(gn)
{var g=igtbl_getGridById(gn);if(!g)return;var ar=g.getActiveRow();var activeElement=null;try
{activeElement=document.activeElement;}catch(e){;}
if(ar&&!igtbl_inEditMode(gn)&&activeElement&&!igtbl_isAChildOfB(activeElement,g.DivElement))
{if(ar.IsAddNewRow)
ar.commit();else
if(ar.processUpdateRow)
ar.processUpdateRow();}
if(g._focusElem)
if(g._lastKey==9||g._lastKey==13||g._lastKey==27)
igtbl_activate(gn);}
function igtbl_getGridById(gridId)
{if(typeof(igtbl_gridState)=="undefined")
return null;var grid=igtbl_gridState[gridId];if(!grid)
for(var gId in igtbl_gridState)
if(igtbl_gridState[gId].UniqueID==gridId||igtbl_gridState[gId].ClientID==gridId)
{grid=igtbl_gridState[gId];break;}
return grid;}
function igtbl_getBandById(tagId)
{if(!tagId)
return null;var parts=tagId.split("_");var gridId=parts[0];var el=igtbl_getElementById(tagId);var bandIndex=igtbl_getBandNo(el);var objTypeId=parts[1];if(objTypeId=="c"&&el&&el.tagName=="TH")
{bandIndex=parts[2];}
if(!igtbl_getGridById(gridId))
return null;var grid=igtbl_getGridById(gridId);return grid.Bands[bandIndex];}
function igtbl_getColumnById(tagId)
{if(!tagId)
return null;var parts=tagId.split("_");var bandIndex=parts.length-2;var gridId=parts[0];var objTypeId=parts[1];var el=igtbl_getElementById(tagId);if(objTypeId=="anc"&&el&&el.tagName=="TD")
{bandIndex=igtbl_getBandById(tagId).Index;}
else
if(objTypeId=="flc"&&el.tagName=="TD")
{bandIndex=igtbl_getBandById(tagId).Index;}
else
if(objTypeId=="rc"&&el&&el.tagName=="TD")
{bandIndex=igtbl_getBandById(tagId).Index;}
else if(objTypeId=="cf")
{if(el&&el.tagName!="TH")
return null;bandIndex=parts[2];}
else if(objTypeId=="cg")
{if(el&&el.tagName!="TH")
return null;bandIndex=parts[2];}
else if(objTypeId=="c")
{if(el&&el.tagName!="TH")
return;bandIndex=parts[2];}
else
return null;if(!igtbl_getGridById(gridId))
return null;var grid=igtbl_getGridById(gridId);var band=grid.Bands[bandIndex];var colIndex=parts[parts.length-1];return band.Columns[colIndex];}
function igtbl_getRowById(tagId)
{if(!tagId)
return null;var parts=tagId.split("_");var rowTypeId=parts[1];var gridId=parts[0];var row=null;var isGrouped=false;var gridIdStore=gridId;if(rowTypeId=="anfr")
{row=igtbl_getWorkRow(igtbl_getElementById(tagId).parentNode.parentNode.parentNode.parentNode.parentNode);if(!row||row.tagName!="TR")
row=null;if(row&&!_validateGrid(gridId))
{row=null
gridId=gridIdStore;}}
if(row==null&&rowTypeId=="grc")
{row=igtbl_getElementById(tagId);if(typeof(row)!="undefined"&&row)
row=row.parentNode;if(!row||!row.getAttribute("groupRow"))
row=null;isGrouped=true;if(row&&!_validateGrid(gridId))
{row=null
gridId=gridIdStore;isGrouped=false;}}
if(row==null&&rowTypeId=="sgr")
{row=igtbl_getWorkRow(igtbl_getElementById(tagId));if(!row||!row.getAttribute("groupRow"))
row=null;isGrouped=true;if(row&&!_validateGrid(gridId))
{row=null
gridId=gridIdStore;isGrouped=false;}}
if(row==null&&rowTypeId=="nfr")
{row=igtbl_getWorkRow(igtbl_getElementById(tagId).parentNode.parentNode.parentNode.parentNode.parentNode);if(!row||row.tagName!="TR")
row=null;if(row&&!_validateGrid(gridId))
{row=null
gridId=gridIdStore;isGrouped=false;}}
if(row==null&&rowTypeId=="anr")
{row=igtbl_getElementById(tagId);if(!row||row.tagName!="TR")
row=null;if(row&&!_validateGrid(gridId))
{row=null
gridId=gridIdStore;isGrouped=false;}}
if(row==null&&rowTypeId=="anl")
{row=igtbl_getElementById(tagId);if(typeof(row)!="undefined"&&row)
row=row.parentNode;if(!row||row.tagName!="TR")
row=null;if(row&&!_validateGrid(gridId))
{row=null
gridId=gridIdStore;isGrouped=false;}}
if(row==null&&rowTypeId=="anc")
{row=igtbl_getElementById(tagId);if(typeof(row)!="undefined"&&row)
row=row.parentNode;if(row&&row.id.substr(0,gridId.length+5)==gridId+"_anfr")
do
{row=row.parentNode}while(row&&row.tagName!="TR");if(!row||row.tagName!="TR")
row=null;if(row&&!_validateGrid(gridId))
{row=null
gridId=gridIdStore;isGrouped=false;}}
if(row==null&&rowTypeId=="gr")
{row=igtbl_getElementById(tagId);if(!row||!row.getAttribute("groupRow"))
row=null;isGrouped=true;if(row&&!_validateGrid(gridId))
{row=null
gridId=gridIdStore;isGrouped=false;}}
if(row==null&&rowTypeId=="rh")
{row=igtbl_getElementById(tagId);if(typeof(row)!="undefined"&&row)
row=row.previousSibling;if(!row||!row.getAttribute("hiddenRow"))
row=null;if(row&&!_validateGrid(gridId))
{row=null
gridId=gridIdStore;isGrouped=false;}}
if(row==null&&rowTypeId=="rc")
{row=igtbl_getElementById(tagId);if(typeof(row)!="undefined"&&row)
row=row.parentNode;if(row&&row.id.substr(0,gridId.length+1)==gridId.substr(0,gridId.length-2)+"_nfr")
do
{row=row.parentNode}while(row&&row.tagName!="TR");if(!row||row.tagName!="TR")
row=null;if(row&&!_validateGrid(gridId))
{row=null
gridId=gridIdStore;isGrouped=false;}}
if(row==null&&rowTypeId=="r")
{row=igtbl_getElementById(tagId);if(!row||row.tagName!="TR")
row=null;if(row&&!_validateGrid(gridId))
{row=null
gridId=gridIdStore;isGrouped=false;}}
if(row==null&&rowTypeId=="l")
{row=igtbl_getElementById(tagId);if(typeof(row)!="undefined"&&row)
row=row.parentNode;if(!row||row.tagName!="TR")
row=null;if(row&&!_validateGrid(gridId))
{row=null
gridId=gridIdStore;isGrouped=false;}}
if(row==null&&rowTypeId=="t")
{row=igtbl_getElementById(tagId);if(typeof(row)!="undefined"&&row)
row=row.parentNode.parentNode.previousSibling;if(row&&!_validateGrid(gridId))
{row=null
gridId=gridIdStore;isGrouped=false;}}
if(row==null&&rowTypeId=="flc")
{row=igtbl_getElementById(tagId);if(typeof(row)!="undefined"&&row)
row=row.parentNode;if(row&&row.id.substr(0,gridId.length+5)==gridId+"_flfr")
do
{row=row.parentNode}while(row&&row.tagName!="TR");if(!row||row.tagName!="TR")
row=null;if(row&&!_validateGrid(gridId))
{row=null
gridId=gridIdStore;isGrouped=false;}}
if(row==null&&rowTypeId=="flfr")
{row=igtbl_getWorkRow(igtbl_getElementById(tagId).parentNode.parentNode.parentNode.parentNode.parentNode);if(!row||row.tagName!="TR")
row=null;if(row&&!_validateGrid(gridId))
{row=null
gridId=gridIdStore;isGrouped=false;}}
if(row==null&&rowTypeId=="fll")
{row=igtbl_getElementById(tagId);if(typeof(row)!="undefined"&&row)
row=row.parentNode;if(!row||row.tagName!="TR")
row=null;if(row&&!_validateGrid(gridId))
{row=null
gridId=gridIdStore;isGrouped=false;}}
if(row==null&&rowTypeId=="flr")
{row=igtbl_getElementById(tagId);if(!row||row.tagName!="TR")
row=null;if(row&&!_validateGrid(gridId))
{row=null
gridId=gridIdStore;isGrouped=false;}}
if(row==null)
return null;var gs=igtbl_getGridById(gridId);if(!gs)
return null;if(typeof(row.Object)!="undefined"&&row.Object.Band)
return row.Object;else
{parts=new Array();while(true)
{row=igtbl_getWorkRow(row,gridId);var level=-1;var bandZero=gs.Bands[0];if(gs.Bands.length==1&&!bandZero.IsGrouped)
{level=row.sectionRowIndex;if(!gs.StatHeader&&(bandZero.AddNewRowVisible==1&&bandZero.AddNewRowView==1||bandZero.FilterRowView==1&&bandZero.FilterUIType==1))
level--;}
else
for(var i=0;i<row.parentNode.childNodes.length;i++)
{var r=row.parentNode.childNodes[i];if(!r.getAttribute("hiddenRow")&&!r.getAttribute("addNewRow")&&!r.getAttribute("filterRow"))
level++;if(r==row)
break;}
parts[parts.length]=level;if(row.parentNode.parentNode.id==gs.Element.id)
break;row=row.parentNode.parentNode.parentNode.parentNode.previousSibling;}
if(parts.length>1)
parts=parts.reverse();var rows=gs.Rows;for(var i=0;i<parts.length;i++)
{row=rows.getRow(parseInt(parts[i],10),row.Element?null:row);if(row&&row.Expandable&&i<parts.length-1)
rows=row.Rows;else if(i<parts.length-1)
{row=null;break;}}
if(!row)
return null;delete parts;row.Element.Object=row;return row;}}
function igtbl_getCellById(tagId)
{if(!tagId)
return null;var parts=tagId.split("_");var gridId=parts[0];var cellTypeId=parts[1];if(cellTypeId!="rc")
{if(cellTypeId!="anc")
if(cellTypeId!="flc")
return null;}
var gs=igtbl_getGridById(gridId);if(!gs)
return null;var row=igtbl_getRowById(igtbl_getRowIdFromCellId(tagId));if(!row)
return null;var column=row.Band.Columns[parseInt(parts[parts.length-1],10)];return row.getCellByColumn(column);}
function igtbl_getRowIdFromCellId(id)
{if(id==null||id.length==0)return;var rowIdAr=id.split("_");switch(rowIdAr[1])
{case("rc"):rowIdAr[1]="r";break;case("anc"):rowIdAr[1]="anr";break;case("flc"):rowIdAr[1]="flr";break;}
return rowIdAr.slice(0,rowIdAr.length-1).join("_");}
function igtbl_getCellByElement(td)
{td=igtbl_dom.find.parentByTag(td,"TD");if(!td)return null;if(td.id)
return igtbl_getCellById(td.id);var tr=td.parentNode;var row=null;while(!row&&tr)
{if(tr.tagName=="TR"&&tr.id)
row=igtbl_getRowById(tr.id);tr=tr.parentNode;}
if(row)
{if(td.id)
return igtbl_getCellById(td.id);while(td.parentNode&&(td.parentNode!=row.Element&&td.parentNode!=row.nfElement))
td=td.parentNode;if(td.parentNode&&td.tagName=="TD"&&td.id)
return igtbl_getCellById(td.id);}
return null;}
function igtbl_getColumnNo(gn,cell)
{if(cell)
{var column=igtbl_getColumnById(cell.id);if(column)
return column.Index;else
return-1;}}
function igtbl_getBandNo(cell)
{if(!cell)
return-1;var tbl=cell;while(tbl&&!tbl.getAttribute("bandNo"))
tbl=tbl.parentNode;if(tbl)
return parseInt(tbl.getAttribute("bandNo"));return-1;}
function igtbl_getFirstRow(row)
{if(row.getAttribute("groupRow"))
return row.childNodes[0].childNodes[0].childNodes[0].rows[0];else
return row;}
function igtbl_getWorkRow(row,gridId)
{if(!row)return;if(row.getAttribute("groupRow"))
{var id=row.id.split("_");if(!gridId)
{if(id[1]=="sgr")
return row.parentNode.parentNode.parentNode.parentNode;else
return row;}
else
{var rowId=id[1];if(rowId=="sgr")
return row.parentNode.parentNode.parentNode.parentNode;else
return row;}}
else
return row;}
function igtbl_getColumnByCellId(cellID)
{var cell=igtbl_getCellById(cellID);if(!cell)
return null;if(cell.Band.Grid.UseFixedHeaders&&!cell.Column.getFixed())
{var tbl;if(cell.Band.Index==0&&!cell.Band.IsGrouped&&cell.Band.ColHeadersVisible==1&&(cell.Band.Grid.StationaryMargins==1||cell.Band.Grid.StationaryMargins==3))
tbl=cell.Band.Grid.StatHeader.Element.parentNode;else
{tbl=cell.Element;while(tbl!=null&&(tbl.tagName!="TABLE"||!tbl.id))
tbl=tbl.parentNode;}
if(tbl)
{thCells=tbl.childNodes[1].rows[0].cells[cell.Element.parentNode.parentNode.parentNode.parentNode.parentNode.cellIndex].firstChild.firstChild.rows[0].cells;var i=0;while(i<thCells.length&&thCells[i].cellIndex!=cell.Element.cellIndex)
i++;if(i<thCells.length)
return thCells[i];}
return null;}
if(cell.Band.Index==0&&!cell.Band.IsGrouped&&cell.Band.ColHeadersVisible==1&&(cell.Band.Grid.StationaryMargins==1||cell.Band.Grid.StationaryMargins==3))
return igtbl_getElemVis(cell.Band.Grid.StatHeader.Element.rows[0].cells,cell.Element.cellIndex);if(cell.Element.parentNode.parentNode.parentNode.childNodes[1].tagName=="THEAD")
return igtbl_getElemVis(cell.Element.parentNode.parentNode.parentNode.childNodes[1].rows[0].cells,cell.Element.cellIndex);return null;}
function igtbl_bandNoFromColId(colId)
{var s=colId.split("_");if(s.length<3)
return null;return parseInt(s[s.length-2]);}
function igtbl_colNoFromColId(colId)
{var s=colId.split("_");if(s.length<3)
return null;return parseInt(s[s.length-1]);}
function igtbl_colNoFromId(id)
{if(!id)
return null;var s=id.split("_");if(s.length==0)
return null;return parseInt(s[s.length-1]);}
function igtbl_isCell(itemName)
{var parts=itemName.split("_");return(parts[1]=="rc"||parts[1]=="anc");}
function igtbl_isColumnHeader(itemName)
{var parts=itemName.split("_");return parts[1]=="c";}
function igtbl_isColumnFooter(itemName)
{var parts=itemName.split("_");return parts[1]=="f";}
function igtbl_isRowLabel(itemName)
{var parts=itemName.split("_");return parts[1]=="l";}
function igtbl_isTemplatedElement(element)
{if(element.imgType)return false;var column,columnElement=element;while(columnElement&&!column)
{column=igtbl_getColumnById(columnElement.id);if(!column)
columnElement=columnElement.parentNode;}
if(!column||column.TemplatedColumn==0||!columnElement.id||columnElement==element)return false;if(igtbl_isColumnHeader(columnElement.id))
{return(column.TemplatedColumn&1)==1;}
if(igtbl_isCell(columnElement.id))
{return(column.TemplatedColumn&2)==2;}
if(igtbl_isColumnFooter(columnElement.id))
{return column.TemplatedColumn&4;}
return false;}
function igtbl_isChild(gn,e)
{if(!e)return false;var ge=igtbl_getElementById(gn+"_main");var p=e.parentNode;while(p&&p!=ge)
p=p.parentNode;return p!=null;}
function igtbl_getParentCell(element)
{if(element&&element.parentNode)
{var parentElement=element.parentNode;while(parentElement!=null)
{if(parentElement.id)
{var parentCell=igtbl_getCellById(parentElement.id);if(parentCell)
return parentCell}
parentElement=parentElement.parentNode;}}
return null;}
function igtbl_getCurCell(se)
{var c=null;while(se&&!c)
if(se.tagName=="TD")
c=se;else
se=se.parentNode;return c;}
function igtbl_shGetElemByCol(col)
{if(!col.hasCells())
return null;var j=0;var cols=col.Band.Columns;for(var i=0;i<col.Index;i++)
{if(cols[i].hasCells())
j++;}
var headerElem=null;if(col.Band.Grid.UseFixedHeaders)
{var childNodes=this.Element.childNodes[0].childNodes;childNodes=childNodes[childNodes.length-1].childNodes[0].childNodes[0].childNodes[1].childNodes[0].childNodes;for(var nodesLen=0;nodesLen<childNodes.length;nodesLen++)
{if(childNodes[nodesLen].id==col.Id)
{headerElem=childNodes[nodesLen];break;}}}
if(!headerElem)
headerElem=this.Element.childNodes[0].childNodes[j+col.Band.firstActiveCell];return headerElem;}
function igtbl_sfGetElemByCol(col)
{if(!col.hasCells())
return null;var j=0;for(var i=0;i<col.Index;i++)
{if(col.Band.Columns[i].hasCells())
j++;}
return this.Element.childNodes[0].childNodes[j+col.Band.firstActiveCell];}
function igtbl_getCellsByColumn(columnId)
{var c=igtbl_getDocumentElement(columnId);if(!c)return;if(!c.length)c=[c];var cells=[];var colSplit=columnId.split("_");var colIndex=parseInt(colSplit[colSplit.length-1],10);for(var k=0;k<c.length;k++)
{var tbody=c[k].parentNode;while(tbody&&tbody.tagName!="THEAD"&&tbody.tagName!="TABLE")
tbody=tbody.parentNode;if(!tbody||tbody.tagName=="TABLE")
continue;tbody=tbody.nextSibling;if(!tbody)
continue;for(var i=0;i<tbody.rows.length;i++)
{if(tbody.rows[i].getAttribute("hiddenRow"))
continue;var cell=tbody.rows[i].cells[c[k].cellIndex];while(cell)
{var cellSplit=cell.id.split("_");var cellIndex=parseInt(cellSplit[cellSplit.length-1],10);if(cellIndex==colIndex)
break;cell=cell.nextSibling;}
if(cell)
cells[cells.length]=cell;}}
return cells;}
function igtbl_gridSortColumn(gn,colId,shiftKey)
{var gs=igtbl_getGridById(gn);gs._isSorting=true;gs.sortColumn(colId,shiftKey);if(gs.sortingColumn&&gs.oldColCursor)
gs.sortingColumn.style.cursor=gs.oldColCursor;gs.stopHourGlass();gs._hidePI();delete gs._isSorting;if(gs.NeedPostBack)
igtbl_doPostBack(gn,"shiftKey:"+shiftKey.toString());}
function igtbl_resizeColumn(gn,colId,width)
{var gs=igtbl_getGridById(gn);if(!gs)
return false;var col=igtbl_getColumnById(colId);if(!col)
return false;return col.setWidth(width);}
function igtbl_setActiveCell(gn,cell,force)
{var g=igtbl_getGridById(gn);if(g)
g.setActiveCell(cell?igtbl_getCellById(cell.id):null,force);return;}
function igtbl_setActiveRow(gn,row,force)
{var g=igtbl_getGridById(gn);if(g)
g.setActiveRow(row?igtbl_getRowById(row.id):null,force);return;}
function igtbl_pageGrid(evnt,gn,pageNo)
{var g=igtbl_getGridById(gn);if(!g||!g.goToPage)return;g.goToPage(pageNo);igtbl_cancelEvent(evnt);}
function igtbl_inEditMode(gn)
{var g=igtbl_getGridById(gn);if(g&&g._cb)return g._editorCurrent!=null;if(g.editorControl&&g.editorControl.getVisible())
return true;var sel=igtbl_getElementById(gn+"_vl");if(sel&&sel.style.display=="")
return true;var tb=igtbl_getElementById(gn+"_tb");if(tb&&tb.style.display=="")
return true;var ta=igtbl_getElementById(gn+"_ta");if(ta&&ta.style.display=="")
return true;return false;}
function igtbl_saveChangedCell(gs,cell,value)
{if(typeof(gs.ChangedRows[cell.Row.Element.id])=="undefined")
gs.ChangedRows[cell.Row.Element.id]=new Object();if(cell.Element)
gs.ChangedRows[cell.Row.Element.id][cell.Element.id]=true;gs._recordChange("ChangedCells",cell,value);}
function igtbl_endCustomEdit()
{if(arguments.length<3)
return;var oEditor=arguments[0];var oEvent=arguments[arguments.length-2];var oThis=arguments[arguments.length-1];var key=(oEvent&&oEvent.event)?oEvent.event.keyCode:0;if(oThis)oThis._lastKey=key;if(oEvent&&typeof(oEvent.event)!="undefined"&&key!=9&&key!=13&&key!=27&&key!=0)
return;var se=null;if(oEditor.Element)
se=oEditor.Element;if(se!=null)
{if(se.getAttribute("noOnBlur"))
return igtbl_cancelEvent(oEvent.event);if(se.getAttribute("editorControl"))
{if(!oEditor.getVisible())
return;var cell=igtbl_getElementById(se.getAttribute("currentCell"));if(!cell)
return;var gs=oThis;var cellObj=igtbl_getCellById(cell.id);if(key==27)
oEditor.setValue(cellObj.getValue(),false);if(typeof(oEditor.getValue())!="undefined")
cellObj.setValue(oEditor.getValue());if(igtbl_fireEvent(gs.Id,gs.Events.BeforeExitEditMode,"(\""+gs.Id+"\",\""+cell.id+"\")")==true)
{if(!gs._exitEditCancel&&!gs._insideSetActive)
{gs._insideSetActive=true;igtbl_setActiveCell(gs.Id,cell);gs._insideSetActive=false;}
gs._exitEditCancel=true;return true;}
oEditor.setVisible(false);oEditor.removeEventListener("blur",igtbl_endCustomEdit);oEditor.removeEventListener("keydown",igtbl_endCustomEdit);gs._exitEditCancel=false;se.removeAttribute("currentCell");se.removeAttribute("oldInnerText");gs.editorControl=null;se.removeAttribute("editorControl");igtbl_fireEvent(gs.Id,gs.Events.AfterExitEditMode,"(\""+gs.Id+"\",\""+cell.id+"\");");if(key==9||key==13)
{var res=null;if(typeof(igtbl_ActivateNextCell)!="undefined")
{if(oEvent.event.shiftKey&&key==9)
res=igtbl_ActivatePrevCell(gs.Id);else
res=igtbl_ActivateNextCell(gs.Id);}
if(res&&igtbl_getCellClickAction(gs.Id,cellObj.Column.Band.Index)==1)
igtbl_EnterEditMode(gs.Id);if(!res)
{igtbl_blur(gs.Id);return;}
igtbl_activate(gs.Id);oEvent.cancel=true;}
else
gs.alignGrid();igtbl_blur(gs.Id);if(gs.NeedPostBack)
igtbl_doPostBack(gs.Id);}}}
var igtbl_lastActiveGrid="";var igtbl_isXHTML=document.compatMode=="CSS1Compat";var testVariable=null;var igtbl_sUnmaskedValue="uV";var igtbl_sigCellText="iCT";var igtbl_sigDataValue="iDV";var igtbl_isAtlas=false;var igtbl_litPrefix="";function igtbl_initGrid(gridId,gridInitArray,bandsInitArray,colsInitArray,eventsInitArray,xmlInitProps,isInsideUpdatePanel,isInsideWARP,firefoxXml)
{var grid=null;var gridElement=igtbl_getElementById("G_"+gridId);igtbl_isAtlas=typeof(Sys)!="undefined"&&typeof(Sys.Application)!="undefined";var rm=null;if(igtbl_isAtlas&&typeof Sys.WebForms=='object'&&typeof Sys.WebForms.PageRequestManager=='function')try
{rm=Sys.WebForms.PageRequestManager.getInstance();}catch(e){}
if(rm&&!rm._ig_grid_onsubmit)
{rm._ig_grid_onsubmit=rm._onsubmit;if(!rm._ig_grid_onsubmit)
rm._ig_grid_onsubmit=2;rm._onsubmit=function()
{if(typeof igtbl_gridState=='object')for(var id in igtbl_gridState)try
{var o=igtbl_gridState[id];if(o&&o.update)
o.update();}catch(id){}
if(typeof this._ig_grid_onsubmit=='function')try
{if(this._ig_grid_onsubmit()===false)
return false;}catch(i){}
return true;}}
if(isInsideUpdatePanel)
{var metCur=false;for(var gi in igtbl_gridState)
{var g=igtbl_getGridById(gi);if(g&&metCur)
g.GridIsLoaded=false;if(gridId==gi)
metCur=true;}}
grid=igtbl_getGridById(gridId);if(grid)
{if(isInsideUpdatePanel&&gridElement==grid.Element)
{grid.GridIsLoaded=true;return;}
igtbl_unloadGrid(gridId,true);igtbl_clearGridsPost(grid);}
var xml;if(firefoxXml)
{var xmlDoc=igtbl_xml.createDocumentFromString(firefoxXml);if(xmlDoc)xml=xmlDoc.firstChild;}
grid=new igtbl_Grid(gridElement,xml,gridInitArray,bandsInitArray,colsInitArray,eventsInitArray,xmlInitProps);if(typeof(igtab_selectTab)!="undefined"&&igtab_selectTab&&igtab_selectTab!=igtbl_tabChanges)
{igtbl_oldSelectTab=igtab_selectTab;igtab_selectTab=igtbl_tabChanges;}
var scrollLeft=grid._AddnlProps[9];var scrollTop=grid._AddnlProps[10];if(grid.LoadOnDemand!=3||grid.XmlLoadOnDemandType==2||grid.XmlLoadOnDemandType==4)
{if(scrollLeft>0)
grid._recordChange("ScrollLeft",grid,scrollLeft);if(scrollTop>0)
grid._recordChange("ScrollTop",grid,scrollTop);}
var sortedColsIds=grid._AddnlProps[8];if(sortedColsIds)
grid.addSortColumn(sortedColsIds);if(grid.Rows.hasRowFilters())
grid.Rows.reapplyRowStyles();var expRowsIds=grid._AddnlProps[3];for(var i=0;i<expRowsIds.length;i++)
{var id;if(!xml)
{var splitId=expRowsIds[i].split(";");igtbl_stateExpandRow(grid.Id,null,true,splitId[0],splitId[1]);id=splitId[0];}
else
{id=expRowsIds[i];id=id.split(";")[0];igtbl_toggleRow(grid.Id,id,true);}
var row=igtbl_getRowById(id);if(row&&row.Rows&&row.Rows.hasRowFilters())
row.Rows.reapplyRowStyles();}
var selRowsIds=grid._AddnlProps[4];for(i=0;i<selRowsIds.length;i++)
igtbl_selectRow(grid.Id,selRowsIds[i]);var selCellsIds=grid._AddnlProps[5];for(i=0;i<selCellsIds.length;i++)
igtbl_selectCell(grid.Id,selCellsIds[i]);var selColsIds=grid._AddnlProps[13];for(i=0;selColsIds&&i<selColsIds.length;i++)
igtbl_selectColumn(grid.Id,selColsIds[i]);var activeCellId=grid._AddnlProps[6];var activeRowId=grid._AddnlProps[7];var de=grid.getDivElement();var mainGrid=grid.MainGrid;var percentageHeight=mainGrid.style.height.indexOf("%")!=-1;if(typeof(igtbl_oldOnBodyResize)=="undefined")
{igtbl_oldOnBodyResize=igtbl_addEventListener(window,"resize",igtbl_onBodyResize,false);}
if(!grid.UseFixedHeaders)
{if(scrollLeft)
igtbl_scrollLeft(de,scrollLeft);grid.alignStatMargins();}
if(!mainGrid.style.height&&de.clientHeight!=de.scrollHeight)
{var scDiv=document.createElement("DIV");scDiv.id=grid.Element.id+"_scd";scDiv.innerHTML="&nbsp;";scDiv.style.height=de.scrollHeight-de.clientHeight+1;de.appendChild(scDiv);de.style.overflowY="hidden";de.setAttribute("scdAdded","true");}
grid.alignDivs(scrollLeft);if(grid.UseFixedHeaders||grid.XmlLoadOnDemandType!=0&&grid.XmlLoadOnDemandType!=4)
{if(grid.StatHeader)
grid.StatHeader.ScrollTo(scrollLeft);if(grid.StatFooter)
grid.StatFooter.ScrollTo(scrollLeft);grid.alignStatMargins();}
if(grid.XmlLoadOnDemandType==2)
de.setAttribute("noOnScroll",true);if(scrollTop)
igtbl_scrollTop(de,scrollTop);if(scrollTop||scrollLeft)
{var st=de.scrollTop.toString();de.setAttribute("noOnScroll","true");de.setAttribute("oldSL",de.scrollLeft.toString());de.setAttribute("oldST",st);grid.cancelNoOnScrollTimeout=window.setTimeout("igtbl_cancelNoOnScroll('"+grid.Id+"')",100);}
if(grid.XmlLoadOnDemandType==2)
de.removeAttribute("noOnScroll");if(activeCellId)
{grid.setActiveCell(igtbl_getCellById(activeCellId));var cell=grid.oActiveCell;if(cell)
{cell.scrollToView();if(cell.Band.getSelectTypeCell()==3)
grid.Element.setAttribute("startPointCell",cell.Element.id);}}
else if(activeRowId)
{grid.setActiveRow(igtbl_getRowById(activeRowId));var row=grid.oActiveRow;if(row)
{row.scrollToView();if(row.Band.getSelectTypeRow()==3)
grid.Element.setAttribute("startPointRow",row.Element.id);}}
grid._cb=typeof igtbl_editEvt=="function";grid._fromServerActiveRow=grid.oActiveRow;ig_csom.addCBEventListener("igtbl_getGridById('"+gridId+"')");var rowsRetrieved=grid._AddnlProps[15];if(rowsRetrieved>=0)
{grid.RowsRetrieved=rowsRetrieved;grid._recordChange("RowsRetrieved",grid,rowsRetrieved);}
if(grid.UseFixedHeaders&&grid.Bands[0]&&grid.Bands[0].HasHeaderLayout)
{var tHead;if(grid.StatHeader)
{tHead=grid.StatHeader.Element;}
else
{tHead=grid.Element.tHead;}
var tBodyBounds=igtbl_getAbsBounds(grid.Element.tBodies[0]);var tHeadBounds=igtbl_getAbsBounds(tHead);var nfhHeight=tBodyBounds.y+grid.getDivElement().scrollTop-tHeadBounds.y;if(nfhHeight>=0)
{tHead.rows[0].lastChild.style.height=nfhHeight+"px";}
var c=0,someFixed=false;for(c=0;c<grid.Bands[0].Columns.length;c++)
{someFixed|=grid.Bands[0].Columns[c].getFixed();if(someFixed)
{break;}}
if(igtbl_isXHTML&&ig_csom.IsIE&&!someFixed)
{var divContent=grid.getDivElement().firstChild;var percentageWidth=divContent.style.width.indexOf("%")!=-1;var drs=igtbl_getElementById(gridId+"_drs");if(!percentageWidth&&drs)
{divContent.style.width=drs.scrollWidth+"px";}}}
grid.GridIsLoaded=true;igtbl_fireEvent(grid.Id,grid.Events.InitializeLayout,'("'+grid.Id+'");');igtbl_browserWorkarounds.addActiveElementTracking();try
{if(!document.activeElement&&(activeCellId||activeRowId))
igtbl_activate(gridId);}
catch(e){;}
if(typeof igtbl_currentEditTempl=='string')
igtbl_currentEditTempl=null;igtbl_getElementById(grid.ClientID).control=grid;return grid;}
function igtbl_initActivation(aa)
{this.AllowActivation=aa[0];this.BorderColor=aa[1];this.BorderStyle=aa[2];this.BorderWidth=aa[3];this.BorderDetails=new Object();var bd=this.BorderDetails;bd.ColorLeft=aa[4][0];bd.ColorTop=aa[4][1];bd.ColorRight=aa[4][2];bd.ColorBottom=aa[4][3];bd.StyleLeft=aa[4][4];bd.StyleTop=aa[4][5];bd.StyleRight=aa[4][6];bd.StyleBottom=aa[4][7];bd.WidthLeft=aa[4][8];bd.WidthTop=aa[4][9];bd.WidthRight=aa[4][10];bd.WidthBottom=aa[4][11];this.getValue=function(where,what)
{var res="";if(where)
res=this.BorderDetails[what+where];if(res==""||res=="NotSet")
res=this["Border"+what];return res;}
this.hasBorderDetails=function()
{var bd=this.BorderDetails;if(bd.ColorLeft||bd.ColorTop||bd.ColorRight||bd.ColorBottom||bd.StyleLeft||bd.StyleTop||bd.StyleRight||bd.StyleBottom||bd.WidthLeft||bd.WidthTop||bd.WidthRight||bd.WidthBottom)
return true;return false;}}
function igtbl_initGroupByBox(grid)
{this.Element=igtbl_getElementById(grid.Id+"_groupBox");this.pimgUp=igtbl_getElementById(grid.Id+"_pimgUp");if(this.pimgUp)
this.pimgUp.style.zIndex=grid._getZ(100000,1);this.pimgDn=igtbl_getElementById(grid.Id+"_pimgDn");if(this.pimgDn)
this.pimgDn.style.zIndex=grid._getZ(100000,1);this.postString="";this.moveString="";if(this.Element)
{this.groups=new Array();var gt=this.Element.childNodes[0];if(gt.tagName=="TABLE")
for(var i=0;i<gt.rows.length;i++)
this.groups[i]=new igtbl_initGroupMember(gt.rows[i].cells[i]);}}
function igtbl_initGroupMember(e)
{var d=e;if(!d.getAttribute("groupInfo"))
return null;this.Element=d;this.groupInfo=d.getAttribute("groupInfo").split(":");this.groupInfo[1]=parseInt(this.groupInfo[1],10);if(this.groupInfo[0]=="col")
this.groupInfo[2]=parseInt(this.groupInfo[2],10);}
function igtbl_lineupHeaders(colId,band)
{var gs=band.Grid;var te=gs.Element;var cg=new Array();var stat=false;if(band.Index==0&&!band.IsGrouped&&gs.StationaryMargins>0)
{cg[0]=te.childNodes[0];if(gs.StatHeader)
cg[1]=gs.StatHeader.Element.previousSibling;if(gs.StatFooter)
{if((gs.Rows.AddNewRow&&band.AddNewRowView==2)||(gs.Rows.FilterRow&&gs.Rows.FilterRow.isFixedBottom()))
cg[cg.length]=gs.StatFooter.Element.previousSibling.previousSibling;else
cg[cg.length]=gs.StatFooter.Element.previousSibling;}
stat=true;}
else
{var e=igtbl_getDocumentElement(colId);if(e&&e.length)
for(var i=0;i<e.length;i++)
cg[i]=e[i].parentNode.parentNode.previousSibling;else if(e&&e.parentNode.parentNode.previousSibling)
cg[0]=e.parentNode.parentNode.previousSibling;}
if(cg.length>0)
{for(var j=0;j<cg.length;j++)
{var hasPercW=false;for(var i=0;cg[j]&&i<cg[j].childNodes.length&&!hasPercW;i++)
{var w=cg[j].childNodes[i].width.toString();if(!w||w.substr(w.length-1)=="%")
hasPercW=true;}
if(hasPercW)
for(var i=0;i<cg[j].childNodes.length;i++)
cg[j].childNodes[i].oldWidth=cg[j].childNodes[i].offsetWidth;if(j>0&&stat||gs.TableLayout)
{var pn=cg[j].parentNode;if(ig_shared.IsFireFox)
{if(!pn.oldWidth)
pn.oldWidth=pn.style.width;if(pn.oldWidth)
{var colGWidth=0;for(var cs=0;cs<cg[j].childNodes.length;cs++)
if(!cg[j].childNodes[cs].style.display)
colGWidth+=igtbl_parseInt(cg[j].childNodes[cs].width);if(colGWidth)
pn.style.width=colGWidth+"px";else
pn.style.width="";}}
else
pn.style.width="";}
for(var i=0;i<cg[j].childNodes.length;i++)
{if(cg[j].childNodes[i].oldWidth)
{if(cg[j].nextSibling)
{var co=igtbl_getElemVis(cg[j].nextSibling.firstChild.childNodes,i);var column=igtbl_getColumnById(co.id);if(column)
{co.style.width="";co.width=cg[j].childNodes[i].oldWidth;column.Width=co.width;if(column.Node)column.Node.setAttribute(igtbl_litPrefix+"width",co.width);}}
cg[j].childNodes[i].style.width="";cg[j].childNodes[i].width=cg[j].childNodes[i].oldWidth;cg[j].childNodes[i].oldWidth=null;}}}}
igtbl_dispose(cg);delete cg;}
function igtbl_scrollToView(gn,child,childWidth,nfWidth,scrollDirection,childLeftAdj)
{if(!childLeftAdj)
childLeftAdj=0;if(!child)
return;var gs=igtbl_getGridById(gn);var parent=gs.Element.parentNode;var drsParent=null;var scrParent=parent;if(gs.UseFixedHeaders||gs.XmlLoadOnDemandType!=0&&gs.XmlLoadOnDemandType!=4)
{scrParent=gs._scrElem;if(child.tagName=="TD"||child.tagName=="TH")
{var prnt=child;var i=0;while(i<6&&prnt&&(prnt.tagName!="DIV"||!prnt.id||prnt.id.substr(prnt.id.length-4,4)!="_drs"))
{i++;prnt=prnt.parentNode;}
if(i<6&&prnt)
drsParent=prnt;}}
if(scrParent.scrollWidth<=scrParent.offsetWidth&&scrParent.scrollHeight<=scrParent.offsetHeight)
return;var childLeft=igtbl_getLeftPos(child)+childLeftAdj;var parentLeft=igtbl_getLeftPos(drsParent?drsParent:parent);var childTop=igtbl_getTopPos(child);var parentTop=igtbl_getTopPos(parent);var childRight=childLeft+child.offsetWidth;var childBottom=childTop+child.offsetHeight;var parentRight=scrParent?(igtbl_getLeftPos(scrParent)+scrParent.clientWidth):(parentLeft+parent.clientWidth);if(ig_csom.IsIE&&igtbl_isXHTML&&scrParent&&gs.UseFixedHeaders)
parentRight-=scrParent.scrollLeft;var parentBottom=parentTop+parent.clientHeight;var hsw=parent.offsetHeight-parent.clientHeight;var vsw=parent.offsetWidth-parent.clientWidth;if(!scrollDirection||scrollDirection==2)
{if(childBottom>parentBottom)
{if(childTop-(parentTop-childTop)>parentTop&&childBottom-childTop<parentBottom-parentTop)
igtbl_scrollTop(scrParent,scrParent.scrollTop+childBottom-parentBottom+hsw-1);else
igtbl_scrollTop(scrParent,scrParent.scrollTop+childTop-parentTop-1);}
if(childTop<parentTop)
igtbl_scrollTop(scrParent,scrParent.scrollTop-(parentTop-childTop)-1);}
if(!scrollDirection||scrollDirection==1)
{if(typeof(nfWidth)!="undefined"&&nfWidth!==null&&(childLeft==childRight||childRight-childLeft<childWidth))
{igtbl_scrollLeft(scrParent,nfWidth);return;}
if(childRight>parentRight)
{if(childLeft-(childRight-parentRight)>parentLeft&&childRight-childLeft<parentRight-parentLeft)
igtbl_scrollLeft(scrParent,scrParent.scrollLeft+childRight-parentRight+vsw);else
igtbl_scrollLeft(scrParent,scrParent.scrollLeft+childLeft-parentLeft);}
if(childLeft<parentLeft)
igtbl_scrollLeft(scrParent,scrParent.scrollLeft-parentLeft+childLeft-vsw-1);else if(gs.UseFixedHeaders&&childLeft==parentLeft)
igtbl_scrollLeft(scrParent,0);}}
function _validateGrid(gridId)
{return(igtbl_getGridById(gridId)!=null);}
function igtbl_needPostBack(gn)
{igtbl_getGridById(gn).NeedPostBack=true;}
function igtbl_cancelPostBack(gn)
{igtbl_getGridById(gn).CancelPostBack=true;}
function igtbl_moveBackPostField(gn,param)
{var gs=igtbl_getGridById(gn);gs.moveBackPostField=param;}
function igtbl_updatePostField(gn,param)
{}
function igtbl_doPostBack(gn,args)
{var gs=igtbl_getGridById(gn);if(gs.isLoaded()&&!gs.CancelPostBack)
{gs.GridIsLoaded=false;if(!args)
args="";window.setTimeout("var g=igtbl_getGridById('"+gn+"');if(g){g.GridIsLoaded=true;g.NeedPostBack=false;}",1000);if(ig_shared.IsSafari)
__doPostBack(gs.UniqueID,args);else
window.setTimeout("window.__doPostBack('"+gs.UniqueID+"','"+args+"');");return true;}
return false;}
function igtbl_unloadGrid(gn,self)
{if(typeof(self)=="undefined")
self=false;var grid=igtbl_gridState[gn];if(!grid||!grid.Events)return;grid.Events.unload();grid.editorControl=null;grid.eReqType=null;grid.eReadyState=null;grid.eError=null;grid.eFeatureRowView=null;grid.eFilterRowType=null;grid.eRowFilterMode=null;grid.eClipboardError=null;grid.eClipboardOperation=null;grid.eFilterComparisonType=null;grid.eFilterComparisionOperator=null;if(grid.dragDropDiv)
{var dragDropDiv=grid.dragDropDiv;dragDropDiv.parentNode.removeChild(dragDropDiv);grid.dragDropDiv=null;grid.GroupByBox.pimgUp.style.display="none";grid.GroupByBox.pimgDn.style.display="none";}
for(var b=0;b<grid.Bands.length;b++)
{for(var c=0;c<grid.Bands[b].Columns.length;c++)
grid.Bands[b].Columns[c].hideValidators();}
for(var i=0;i<grid.Bands.length;i++)
for(var j=0;j<grid.Bands[i].Columns.length;j++)
{var editor=grid.Bands[i].Columns[j].editorControl;if(editor)
{var elem=editor.Element,old=editor._old_parent;if(elem&&old&&(old!=elem.parentNode))
{elem.parentNode.removeChild(elem);old.appendChild(elem);}
grid.Bands[i].Columns[j].editorControl=null;}}
if(grid._editorArea)
{if(grid._editorArea.parentNode)
grid._editorArea.parentNode.removeChild(grid._editorArea);grid._editorArea=null;}
if(grid._editorInput)
{if(grid._editorInput.parentNode)
grid._editorInput.parentNode.removeChild(grid._editorInput);grid._editorInput=null;}
if(grid._editorList)
{if(grid._editorList.parentNode)
grid._editorList.parentNode.removeChild(grid._editorList);grid._editorList=null;}
var f=grid._thisForm;if(!f)
f=igtbl_getThisForm(grid.Element);if(f&&self&&!(typeof(f.igtblGrid)=="undefined"||f.igtblGrid==null))
{var g=f.igtblGrid,tg=null;while(g&&g!=grid)
{tg=g;g=g.oldIgtblGrid;}
if(tg==null)
f.igtblGrid=grid.oldIgtblGrid;else
tg.oldIgtblGrid=grid.oldIgtblGrid;grid.oldIgtblGrid=null;}
if(f&&(!self||(typeof(f.igtblGrid)=="undefined"||f.igtblGrid==null)))
{var old=grid.oldIgtblGrid;if(old)
{grid.oldIgtblGrid=null;igtbl_unloadGrid(old.Id);}
try
{f.igtblGrid=null;if(grid._thisForm&&grid._thisForm.removeEventListener)
grid._thisForm.removeEventListener('submit',igtbl_submit,false);if(f.onsubmit==igtbl_submit)
{f.onsubmit=f.igtbl_oldOnSubmit;f.igtbl_oldOnSubmit=null;}
if(typeof(f.igtbl_oldSubmit)!="undefined"&&f.igtbl_oldSubmit!=null&&f.submit==igtbl_formSubmit)
{f.submit=f.igtbl_oldSubmit;f.igtbl_oldSubmit=null;}
if(typeof(window._igtbl_doPostBackOld)!="undefined"&&window._igtbl_doPostBackOld!=null&&window.__doPostBack==igtbl_submit)
{window.__doPostBack=window._igtbl_doPostBackOld;window._igtbl_doPostBackOld=null;}
window._igtbl_thisForm=null;}
catch(ex)
{}}
grid.disposing=true;var state=igtbl_dom.find.rootNode(grid.StateChanges);var node=grid.Node;igtbl_dispose(grid);igtbl_removeState(state);if(node)igtbl_xml.disposeDocument(node);delete node;delete igtbl_gridState[gn];delete state;}
function igtbl_removeState(stateNode)
{if(!stateNode||!stateNode.childNodes)
return;while(stateNode.childNodes.length>0)
igtbl_removeState(stateNode.childNodes[0]);if(stateNode.parentNode)
{if(typeof(stateNode.parentNode.removeChild)!="undefined")
stateNode.parentNode.removeChild(stateNode);else
stateNode.parentNode.removeNode(stateNode);}
if(typeof(stateNode.outerHTML)!="undefined")
stateNode.outerHTML="";}
function igtbl_dispose(obj)
{if(ig_csom.IsNetscape||ig_csom.IsNetscape6)
return;for(var item in obj)
{if(typeof(obj[item])!="undefined"&&obj[item]!=null&&!obj[item].tagName&&!obj[item].disposing&&typeof(obj[item])!="string")
{try
{obj[item].disposing=true;igtbl_dispose(obj[item]);}catch(exc1){;}}
try
{delete obj[item];}catch(exc2)
{return;}}}
var igtbl_oldOnUnload;var igtbl_bInsideOldOnUnload=false;function igtbl_unload()
{igtbl_browserWorkarounds.removeActiveElementTracking();if(igtbl_oldOnUnload&&!igtbl_bInsideOldOnUnload)
{igtbl_bInsideOldOnUnload=true;igtbl_oldOnUnload();igtbl_bInsideOldOnUnload=false;}
for(var gridId in igtbl_gridState)
{try
{if(typeof(document)!=='unknown')
{var p=igtbl_getElementById(gridId);p.value=ig_ClientState.getText(igtbl_gridState[gridId].ViewState);}}
catch(e)
{}
if(igtbl_gridState[gridId].unloadGrid)
igtbl_gridState[gridId].unloadGrid();else
delete igtbl_gridState[gridId];}}
if(typeof igtbl_gridState!="object")
var igtbl_gridState=new Object();var igtbl_bInsideigtbl_oldOnSubmit=false;var igtbl_bInsideigtbl_oldDoPostBack=false;function igtbl_submit()
{var retVal=true;if(arguments.length==0||(ig_csom.IsNetscape||ig_csom.IsNetscape6)&&arguments.length==1)
{var form=this;if(form.tagName!="FORM")
form=window._igtbl_thisForm;if(form)
{if(form.igtbl_oldOnSubmit&&!igtbl_bInsideigtbl_oldOnSubmit)
{igtbl_bInsideigtbl_oldOnSubmit=true;if(arguments.length==0)
retVal=form.igtbl_oldOnSubmit();else
retVal=form.igtbl_oldOnSubmit(arguments[0]);igtbl_bInsideigtbl_oldOnSubmit=false;if(retVal===false)
{if(typeof(igtbl_gridState)!="undefined"&&igtbl_gridState!=null)
for(var gId in igtbl_gridState)
{var g=igtbl_getGridById(gId);if(g)g.GridIsLoaded=true;}
return retVal;}}
igtbl_updateGridsPost(form.igtblGrid);if((window.__smartNav)&&form.igtblGrid)
igtbl_unloadGrid(form.igtblGrid.Id);}}
else if(typeof(window._igtbl_doPostBackOld)!="undefined"&&!igtbl_bInsideigtbl_oldDoPostBack&&window._igtbl_thisForm)
{igtbl_updateGridsPost(window._igtbl_thisForm.igtblGrid);igtbl_bInsideigtbl_oldDoPostBack=true;retVal=window._igtbl_doPostBackOld(arguments[0],arguments[1]);igtbl_bInsideigtbl_oldDoPostBack=false;}
return retVal;}
function igtbl_formSubmit()
{igtbl_updateGridsPost(this.igtblGrid);var val;try
{val=this.igtbl_oldSubmit();}
catch(e){};return val;}
function igtbl_updateGridsPost(grid)
{if(!grid)return;igtbl_updateGridsPost(grid.oldIgtblGrid);grid.update();}
function igtbl_clearGridsPost(grid)
{if(!grid||!grid.ViewState||!grid.ViewState.parentNode)return;if(typeof(grid.ViewState.parentNode.removeChild)!="undefined")
grid.ViewState.parentNode.removeChild(grid.ViewState);else
grid.ViewState.parentNode.removeNode(grid.ViewState);}
if(window.addEventListener)
window.addEventListener('unload',igtbl_unload,false);else if(window.onunload!=igtbl_unload)
{igtbl_oldOnUnload=window.onunload;window.onunload=igtbl_unload;}
function igtbl_toggleRow()
{var srcRow,expand;if(arguments.length==1)
{var evnt=arguments[0];var se=igtbl_srcElement(evnt);if(!se||se.tagName!="IMG")
return;srcRow=se.parentNode.parentNode.id;}
else
{srcRow=arguments[1];expand=arguments[2];}
var sr=igtbl_getRowById(srcRow);if(!sr)return;igtbl_lastActiveGrid=sr.gridId;if(typeof(expand)=="undefined")
expand=!sr.getExpanded();if(expand!=false)
sr.setExpanded(true);else
sr.setExpanded(false);}
function igtbl_resizeRow(gn,rowId,height)
{var gs=igtbl_getGridById(gn);if(!gs)
return;var row=igtbl_getRowById(rowId);if(!row)
return;if(height>0)
{var cancel=false;if(igtbl_fireEvent(gn,gs.Events.BeforeRowSizeChange,"(\""+gn+"\",\""+row.Element.id+"\","+height+")")==true)
cancel=true;if(!cancel)
{var origOffsetHeight=row.Element.offsetHeight;if(!row._origHeight)
row._origHeight=row.Element.offsetHeight;row.Element.style.height=height+"px";gs._removeChange("ResizedRows",row);gs._recordChange("ResizedRows",row,height);var rowLabel=row.getRowSelectorElement();if(rowLabel)
rowLabel.style.height=height+"px";var expansionArea=row.getExpansionElement();if(expansionArea)
expansionArea.style.height=height+"px";if(gs.UseFixedHeaders)
{var i=0;var rowElCells=row.Element.cells;while(i<rowElCells.length&&(!rowElCells[i].firstChild||rowElCells[i].firstChild.id!=gn+"_drs"))i++;if(i<rowElCells.length)
{var td=rowElCells[i];var noneFixedRow=td.firstChild.firstChild.rows[0];var noneFixedRowHeight=noneFixedRow.style.height;td.style.height=height+"px";noneFixedRow.style.height=height;if(gs.IsXHTML&&height>row._origHeight)
{if(rowLabel)
{var calcHeight=(height+rowLabel.offsetHeight-rowLabel.clientHeight);td.style.height=calcHeight+"px";noneFixedRow.style.height=calcHeight+"px";}}
if(ig_csom.IsIE6)
{if(td.offsetHeight>noneFixedRow.offsetHeight)
{noneFixedRow.style.height=noneFixedRowHeight;td.style.height=td.offsetHeight+"px";}
if(td.clientHeight<noneFixedRow.offsetHeight)
{noneFixedRow.style.height=(igtbl_parseInt(noneFixedRow.style.height)-(noneFixedRow.offsetHeight-td.clientHeight))+"px";}}
else if(ig_csom.IsIE)
{if(td.offsetHeight>noneFixedRow.offsetHeight)
noneFixedRow.style.height=noneFixedRowHeight;}}}
if(gs.StatHeader&&(row.IsFilterRow||row.IsAddNewRow)&&row.Band.Index==0)
{var headerDiv=gs.StatHeader.Element.parentNode.parentNode;headerDiv.height="";headerDiv.style.height="";}
gs.alignGrid();gs.alignStatMargins();gs.alignDivs();igtbl_fireEvent(gn,gs.Events.AfterRowSizeChange,"(\""+gn+"\",\""+row.Element.id+"\","+height+")");}}}
function igtbl_setSelectedRowImg(gn,row,hide)
{var gs=igtbl_getGridById(gn);if(!gs)return;if(row)
igtbl_getRowById(row.id).setSelectedRowImg(hide);}
function igtbl_setNewRowImg(gn,row)
{var gs=igtbl_getGridById(gn);if(!gs)return;gs.setNewRowImg(row?igtbl_getRowById(row.id):null);}
function igtbl_stateExpandRow(gn,row,expandFlag,id,level)
{var gs=igtbl_getGridById(gn);if(!gs)
return;if(expandFlag)
{var dk=(row?row.DataKey:null)
var stateChange=gs._recordChange("ExpandedRows",row,dk,id);if(!row)
ig_ClientState.setPropertyValue(stateChange.Node,"Level",level);else if(gs.CollapsedRows[row.Element.id])
gs._removeChange("CollapsedRows",row);}
else
{if(!row)return;gs._recordChange("CollapsedRows",row,row.DataKey);gs._removeChange("ExpandedRows",row);}}
function igtbl_getChildRows(gn,row)
{var rows=null;if(!row||!row.id)
return rows;var rObj=igtbl_getRowById(row.id);if(!rObj)
return rows;return rObj.getChildRows();}
function igtbl_rowsCount(rows)
{var i=0,j=0;if(!rows)return i;while(j<rows.length)
{var r=rows[j];if(!r.getAttribute("hiddenRow")&&!r.getAttribute("addNewRow")&&!r.getAttribute("filterRow"))
i++;j++;}
return i;}
function igtbl_visRowsCount(rows)
{var i=0,j=0;if(!rows)return i;while(j<rows.length)
{var r=rows[j];if(!r.getAttribute("hiddenRow")&&r.style.display==""&&!r.getAttribute("addNewRow")&&!r.getAttribute("FilterRow"))
i++;j++;}
return i;}
function igtbl_sortGroupedRows(rows,bandNo,colId)
{if(rows.length<=0)
return;if(rows.Band.Index==bandNo&&rows.getRow(0).Element.getAttribute("groupRow")==colId)
{rows.sort();return;}
for(var i=0;i<rows.length;i++)
{var row=rows.getRow(i);if(row.Rows&&row.Rows.length>0)
igtbl_sortGroupedRows(row.Rows,bandNo,colId);}}
function _igtbl_getMoreRows(gn)
{var g=igtbl_getGridById(gn);if(g)
{if(g.ReadyState==0)
g.invokeXmlHttpRequest(g.eReqType.MoreRows);}}
function igtbl_deleteSelRows(gn)
{var gs=igtbl_getGridById(gn);var ar=gs.getActiveRow();if(ar&&ar.IsAddNewRow)return;var del=false;if(igtbl_inEditMode(gn))
{igtbl_hideEdit(gn);if(igtbl_inEditMode(gn))
return;}
if(gs.Node)
{var arOffs=ar?ar.getIndex():0;gs.isDeletingSelected=true;var arr=igtbl_sortRowIdsByClctn(gs.SelectedRows);for(var i=0;i<arr.length;i++)
{var row=gs.getRowByLevel(arr[i]);if(row.deleteRow())
{if(i==arr.length-1||arr[i].length!=arr[i+1].length||arr[i].length>1&&arr[i][arr[i].length-2]!=arr[i+1][arr[i+1].length-2])
{var rows=row.OwnerCollection;rows.SelectedNodes=rows.Node.selectNodes("R");if(!rows.SelectedNodes.length)
rows.SelectedNodes=rows.Node.selectNodes("Group");rows.reIndex(row.getIndex(true));rows.repaint();}}}
if(!arr.length&&ar)
{var rows=ar.OwnerCollection;if(ar.deleteRow())
{rows.SelectedNodes=rows.Node.selectNodes("R");if(!rows.SelectedNodes.length)
rows.SelectedNodes=rows.Node.selectNodes("Group");while(rows.length==0&&rows.ParentRow&&rows.ParentRow.GroupByRow)
rows=rows.ParentRow.OwnerCollection;rows.reIndex(arOffs);rows.repaint();}}
if(ar&&!gs.getActiveRow())
{var rows=ar.OwnerCollection;if(arOffs<rows.length)
rows.getRow(arOffs).activate();else if(rows.length>0)
rows.getRow(rows.length-1).activate();else if(rows.ParentRow)
rows.ParentRow.activate();ar=gs.getActiveRow();if(ar&&ar.Band.getSelectTypeRow()==2)
ar.setSelected();}
gs.isDeletingSelected=false;ig_dispose(arr);delete arr;}
else
{var r=null;if(ar&&!gs.getActiveCell())
{r=ar.getNextRow();while(r&&r.getSelected())
r=r.getNextRow();if(!r)
{r=ar.getPrevRow();while(r&&r.getSelected())
r=r.getPrevRow();}
if(!r)
r=ar.ParentRow;}
for(var rowId in gs.SelectedRows)
{if(gs.SelectedRows[rowId])
{var row=igtbl_getRowById(rowId);if(row&&row.deleteRow(true))
del=true;}}
ar=gs.getActiveRow();if(!del&&ar&&!gs.SelectedRows[ar.Element.id])
{del=ar.deleteRow(true);if(del)ar=null;}
if(del)
{if(r&&igtbl_getElementById(r.Element.id))
{if(r.Band.getSelectTypeRow()==2)
r.setSelected();r.activate();ar=r;}
else
ar=null;}
if(!ar)
gs.setActiveRow(null);}
gs.alignStatMargins();if(gs.NeedPostBack)
igtbl_doPostBack(gn);}
function igtbl_deleteRow(gn,rowId)
{var row=igtbl_getRowById(rowId);if(!row)
return false;return row.deleteRow();}
function igtbl_clearRowChanges(gs,row)
{if(!row)return;if(gs.SelectedRows[row.Element.id])
gs._removeChange("SelectedRows",row);if(gs.SelectedCellsRows[row.Element.id])
{for(var cell in gs.SelectedCellsRows[row.Element.id])
{gs._removeChange("SelectedCells",igtbl_getCellById(cell));delete gs.SelectedCellsRows[row.Element.id][cell];}
delete gs.SelectedCellsRows[row.Element.id];}
if(gs.ChangedRows[row.Element.id])
{for(var cell in gs.ChangedRows[row.Element.id])
{gs._removeChange("ChangedCells",igtbl_getCellById(cell));delete gs.ChangedRows[row.Element.id][cell];}
delete gs.ChangedRows[row.Element.id];}
if(gs.ResizedRows[row.Element.id])
gs._removeChange("ResizedRows",row);if(gs.ExpandedRows[row.Element.id])
gs._removeChange("ExpandedRows",row);if(gs.CollapsedRows[row.Element.id])
gs._removeChange("CollapsedRows",row);if(typeof(gs.AddedRows[row.Element.id])!="undefined")
row._Changes["AddedRows"].setFireEvent(false);}
function igtbl_cleanRow(row)
{if(row.cells)
for(var j=0;j<row.cells.length;j++)
{var cell=row.cells[j];if(cell)
{cell.Column=null;cell.Band=null;cell.Row=null;for(var change in cell._Changes)
{var ch=cell._Changes[change];try
{if(ch.length)
ch=ch[0];if(ch.Grid)
ch.Grid._removeChange(change,cell);}catch(e){;}}
if(cell.Element)
cell.Element.Object=null;}}
if(row._Changes)
for(var change in row._Changes)
{var ch=row._Changes[change];try
{if(ch.length)
ch=ch[0];if(ch.Grid)
ch.Grid._removeChange(change,row);}catch(e){;}}
row.OwnerCollection=null;row.Band=null;row.ParentRow=null;row.Element.Object=null;}
var igtbl_justAssigned=false;function igtbl_resetJustAssigned()
{igtbl_justAssigned=false;}
function igtbl_clearNoOnResize(gn)
{var g=igtbl_getGridById(gn);g.Element.removeAttribute("noOnResize");}
function igtbl_fillEditTemplate(row,childNodes)
{for(var i=childNodes.length-1;i>=0;i--)
{var el=childNodes[i];if(typeof(el.getAttribute)=="undefined")
continue;var colKey=el.getAttribute("columnKey");var column=row.Band.getColumnFromKey(colKey);if(column)
{var cell=row.getCellByColumn(column);if(!cell)
{if(!el.isDisabled)
{el.setAttribute("disabledBefore",true);el.disabled=true;}
el.value="";continue;}
else if(el.isDisabled&&el.getAttribute("disabledBefore"))
{el.disabled=false;el.removeAttribute("disabledBefore");}
var cellValue=cell.getValue();var cellText="";var nullText="";if(cellValue==null)
{nullText=cell.Column.getNullText();cellText=nullText;}
else
cellText=cellValue.toString();var ect=cellText.replace(/\r\n/g,"\\r\\n");ect=ect.replace(/\"/g,"\\\"");var s="(\""+row.gridId+"\",\""+el.id+"\",\""+(cell.Element?cell.Element.id:"")+"\",\""+ect+"\")";if(!igtbl_fireEvent(row.gridId,igtbl_getGridById(row.gridId).Events.TemplateUpdateControls,s))
{if(el.tagName=="SELECT")
{for(var j=0;j<el.childNodes.length;j++)
if(el.childNodes[j].tagName=="OPTION")
if(el.childNodes[j].value==cellText)
{el.childNodes[j].selected=true;break;}}
else if(el.tagName=="INPUT"&&el.type=="checkbox")
{if(!cellValue||cellText.toLowerCase()=="false")
el.checked=false;else
el.checked=true;}
else if(el.tagName=="DIV"||el.tagName=="SPAN")
{for(var j=0;j<el.childNodes.length;j++)
{if(el.childNodes[j].tagName=="INPUT"&&el.childNodes[j].type=="radio")
if(el.childNodes[j].value==cellText)
{el.childNodes[j].checked=true;break;}}}
else
el.value=cellText;if(!el.isDisabled)
igtbl_focusedElement=el;}}
else if(el.childNodes&&el.childNodes.length>0)
igtbl_fillEditTemplate(row,el.childNodes);}}
function igtbl_unloadEditTemplate(row,childNodes)
{for(var i=0;i<childNodes.length;i++)
{var el=childNodes[i];if(typeof(el.getAttribute)=="unknown"||!el.getAttribute)
continue;var colKey=el.getAttribute("columnKey");var column=row.Band.getColumnFromKey(colKey);if(column)
{var cell=row.getCellByColumn(column);if(cell&&!igtbl_fireEvent(row.gridId,igtbl_getGridById(row.gridId).Events.TemplateUpdateCells,"(\""+row.gridId+"\",\""+el.id+"\",\""+(cell.Element?cell.Element.id:"")+"\")"))
{if(cell.isEditable()||cell.Column.getAllowUpdate()==3)
{if(el.tagName=="SELECT")
cell.setValue(el.options[el.selectedIndex].value);else if(el.tagName=="INPUT"&&el.type=="checkbox")
cell.setValue(el.checked);else if(el.tagName=="DIV"||el.tagName=="SPAN")
{for(var j=0;j<el.childNodes.length;j++)
{if(el.childNodes[j].tagName=="INPUT"&&el.childNodes[j].type=="radio")
if(el.childNodes[j].checked)
{cell.setValue(el.childNodes[j].value);break;}}}
else if(typeof(el.value)!="undefined")
cell.setValue(el.value);}}}
else if(el.childNodes&&el.childNodes.length>0)
igtbl_unloadEditTemplate(row,el.childNodes);}}
function igtbl_gRowEditMouseDown(evnt)
{if(igtbl_justAssigned)
{igtbl_justAssigned=false;return;}
if(!evnt)
evnt=event;var src=igtbl_srcElement(evnt);var editTempl=igtbl_getElementById(igtbl_currentEditTempl);if(editTempl&&src&&!igtbl_contains(editTempl,src))
{var rId=editTempl.getAttribute("editRow");if(rId)
{var row=igtbl_getRowById(rId);row.Band.Grid.event=evnt;row.endEditRow();}}}
function igtbl_gRowEditButtonClick(evnt,saveChanges)
{if(!evnt)
evnt=event;var src=igtbl_srcElement(evnt);var editTempl=igtbl_getElementById(igtbl_currentEditTempl);if(editTempl)
{if(typeof(saveChanges)=="undefined")
saveChanges=(src.id.substring(src.id.length-13)=="igtbl_reOkBtn")||src.value.toUpperCase()=="OK";var rId=editTempl.getAttribute("editRow");if(rId)
{var row=igtbl_getRowById(rId);row.Band.Grid.event=evnt;row.endEditRow(saveChanges);}}}
function igtbl_RecalculateRowNumbers(rc,startingIndex,band,xmlNode)
{if(rc==null&&band==null)return startingIndex;var oRow;var iRowLbl=-1;var oFAC;var returnedIndex=-1;var workingIndex;var oBand=band?band:rc.Band;switch(oBand.AllowRowNumbering)
{case(2):workingIndex=startingIndex;break;case(3):workingIndex=1;break;case(4):var indexOffSet=(oBand.Grid.AllowPaging&&oBand.Index==0)?((oBand.Grid.CurrentPageIndex-1)*oBand.Grid.PageSize):oBand._currentRowNumber;workingIndex=indexOffSet+1;break;}
if(null!=rc)
{for(var i=0;i<rc.length;i++)
{iRowLbl=-1;oRow=rc.getRow(i);if(oRow.Band.AllowRowNumbering>=2)
iRowLbl=oRow._setRowNumber(workingIndex);if(iRowLbl>-1)
{var childRows=oRow.Rows;var childBand=childRows?childRows.Band:oRow.Band.Grid.Bands[oRow.Band.Index+1];var childXmlNode=childRows?childRows.Node:(oRow.Node?oRow.Node.selectSingleNode("Rs"):null);returnedIndex=igtbl_RecalculateRowNumbers(childRows,workingIndex+1,childBand,childXmlNode);}
switch(rc.Band.AllowRowNumbering)
{case(2):workingIndex=returnedIndex;break;case(3):workingIndex=++workingIndex;break;case(4):oRow.Band._currentRowNumber=workingIndex;workingIndex=++workingIndex;break;}}}
else if(band!=null&&xmlNode!=null)
{var oXmlRows=xmlNode.selectNodes("R");for(var i=0;i<oXmlRows.length;i++)
{iRowLbl=-1;oRow=oXmlRows[i];if(band.AllowRowNumbering>=2)
oRow.setAttribute(igtbl_litPrefix+"rowNumber",workingIndex);var childRows=null;var childBand=band.Grid.Bands[band.Index+1];var childXmlNode=oRow.selectSingleNode("Rs");returnedIndex=igtbl_RecalculateRowNumbers(childRows,workingIndex+1,childBand,childXmlNode);switch(band.AllowRowNumbering)
{case(2):workingIndex=returnedIndex;break;case(3):workingIndex=++workingIndex;break;case(4):band._currentRowNumber=workingIndex;workingIndex=++workingIndex;break;}}}
return workingIndex;}
function igtbl_swapCells(rows,bandNo,index,toIndex)
{if(!rows||rows.Band.Index>bandNo)
return;for(var i=0;i<rows.rows.length;i++)
{var row=rows.rows[i];if(row)
{if(!row.GroupByRow&&row.Band.Index==bandNo&&row.cells)
{var cell=row.cells[index];row.cells[index]=row.cells[toIndex];row.cells[toIndex]=cell;}
igtbl_swapCells(row.Rows,bandNo,index,toIndex);}}}
function igtbl_AdjustCheckboxDisabledState(column,bandIndex,rows,value)
{if(!rows)return;if(rows.Band.Index==bandIndex)
for(var i=0;i<rows.length;i++)
{var oC=rows.getRow(i).getCellByColumn(column);oC=igtbl_getCheckboxFromElement(oC.Element);if(oC)oC.disabled=!(1==value);}
else if(rows.Band.Index<bandIndex)
for(var i=0;i<rows.length;i++)igtbl_AdjustCheckboxDisabledState(column,bandIndex,rows.getRow(i).Rows,value);}
function igtbl_cancelNoOnScroll(gn)
{var g=igtbl_getGridById(gn);if(!g)return;var de=g.getDivElement();de.removeAttribute("noOnScroll");de.removeAttribute("oldST");de.removeAttribute("oldSL");g.cancelNoOnScrollTimeout=0;}
function igtbl_scrollLeft(e,left)
{e.scrollLeft=left;if((ig_csom.IsNetscape||ig_csom.IsNetscape6)&&e.onscroll&&!e._insideFFOnScroll)
{e._insideFFOnScroll=true;e.onscroll();e._insideFFOnScroll=false;}}
function igtbl_scrollTop(e,top)
{if(e.scrollTop==top)
return;e.scrollTop=top;if((ig_csom.IsNetscape||ig_csom.IsNetscape6)&&e.onscroll&&!e._insideFFOnScroll)
{e._insideFFOnScroll=true;e.onscroll();e._insideFFOnScroll=false;}}
function igtbl_getBodyScrollLeft()
{var isXHTML=document.compatMode=="CSS1Compat";if(isXHTML)
return document.body.parentNode.scrollLeft;else
return document.body.scrollLeft;}
function igtbl_getBodyScrollTop()
{if(igtbl_isXHTML)
return document.body.parentNode.scrollTop;else
return document.body.scrollTop;}
function igtbl_selectStart(evnt,gn)
{var se=igtbl_srcElement(evnt);if(se)
{var over=false,cell=null,column=null;while(se&&!over)
{if(se&&(se.tagName=="TABLE"&&se.id=="G_"+gn||se.tagName=="TH"&&(column=igtbl_getColumnById(se.id))!=null||se.tagName=="TD"&&(cell=igtbl_getCellById(se.id))!=null))
over=true;se=se.parentNode;}
if(cell)
{if(!(cell.Column.TemplatedColumn&2))
igtbl_cancelEvent(evnt);}
else if(column)
{if((!(column.TemplatedColumn&1)&&se.parentNode.parentNode.tagName=="THEAD")||(!(column.TemplatedColumn&4)&&se.parentNode.parentNode.tagName=="TFOOT"))
igtbl_cancelEvent(evnt);}
else
igtbl_cancelEvent(evnt);}}
function igtbl_selectColumnRegion(gn,se)
{var gs=igtbl_getGridById(gn);if(!gs)
return;var te=gs.Element;var lastSelectedColumn=te.getAttribute("lastSelectedColumn");var selMethod=te.getAttribute("selectMethod");if(selMethod=="column"&&se.id!=lastSelectedColumn)
{var startColumn=igtbl_getColumnById(te.getAttribute("startColumn"));if(startColumn==null)
startColumn=igtbl_getColumnById(se.id);var endColumn=igtbl_getColumnById(se.id);if(endColumn.Band.getSelectTypeColumn()==3)
gs.selectColRegion(startColumn,endColumn);else
{igtbl_clearSelectionAll(gn);igtbl_selectColumn(gn,se.id);}
gs.Element.setAttribute("lastSelectedColumn",se.id);}}
function igtbl_selectRegion(gn,se)
{var gs=igtbl_getGridById(gn);if(!gs||!se)
return;var rowContainer;var cell=igtbl_getCellById(se.id),row=null;if(!cell)
row=igtbl_getRowById(se.id);else
row=cell.Row;if(!row)
return;if(se.getAttribute("groupRow"))
rowContainer=se.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode;else
rowContainer=row.Element.parentNode;var te=gs.Element;var selTableId=te.getAttribute("selectTable");var workTableId;if(row.IsAddNewRow&&row.Band.Index==0)
workTableId=gs.Element.id;else
if(se.getAttribute("groupRow"))
workTableId=se.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.id;else
workTableId=row.Element.parentNode.parentNode.id;if(workTableId=="")
return;var bandNo=igtbl_getElementById(workTableId).getAttribute("bandNo");if(selTableId==workTableId)
{var selMethod=te.getAttribute("selectMethod");if(selMethod=="row"&&(!cell||igtbl_getCellClickAction(gn,bandNo)==2&&cell||se.getAttribute("groupRow")))
{var selRow=igtbl_getRowById(te.getAttribute("startPointRow"));var rowId;if(se.getAttribute("groupRow"))
rowId=se.parentNode.parentNode.parentNode.parentNode.parentNode.id;else
rowId=row.Element.id;var curRow=igtbl_getRowById(rowId);if(selRow&&igtbl_getSelectTypeRow(gn,bandNo)==3&&igtbl_getCellClickAction(gn,bandNo)>0)
{igtbl_setActiveRow(gn,curRow.getFirstRow());gs.selectRowRegion(selRow,curRow);}
else
{igtbl_setActiveRow(gn,igtbl_getFirstRow(igtbl_getElementById(rowId)));if(!(curRow.getSelected()&&igtbl_getLength(gs.SelectedRows)==1))
{igtbl_clearSelectionAll(gn);if(se.getAttribute("groupRow"))
rowId=igtbl_getWorkRow(row.Element,gn).id;if(igtbl_getSelectTypeRow(gn,bandNo)>1&&igtbl_getCellClickAction(gn,bandNo)>0)
igtbl_selectRow(gn,curRow);}}}
else if(selMethod=="cell"&&cell)
{var selCell=igtbl_getCellById(te.getAttribute("startPointCell"));var curCell=igtbl_getCellById(se.id);if(igtbl_getSelectTypeCell(gn,bandNo)==3&&igtbl_getCellClickAction(gn,bandNo)>0&&selCell)
{gs.selectCellRegion(selCell,curCell);curCell.activate();}
else
{if(!(curCell.getSelected()&&igtbl_getLength(gs.SelectedRows)==1))
{igtbl_clearSelectionAll(gn);if(igtbl_getSelectTypeCell(gn,bandNo)>1&&igtbl_getCellClickAction(gn,bandNo)>0)
igtbl_selectCell(gn,curCell);}
igtbl_setActiveCell(gn,se);}}}}
function igtbl_clearSelectionAll(gn)
{var gs=igtbl_getGridById(gn);if(igtbl_fireEvent(gn,gs.Events.BeforeSelectChange,"(\""+gn+"\",\"\")")==true)
return;var row,column,cell;gs._noCellChange=false;for(var row in gs.SelectedRows)
igtbl_selectRow(gn,row,false,false);for(var column in gs.SelectedColumns)
igtbl_selectColumn(gn,column,false,false);for(var row in gs.SelectedCellsRows)
{for(var cell in gs.SelectedCellsRows[row])
delete gs.SelectedCellsRows[row][cell];delete gs.SelectedCellsRows[row];}
for(var cell in gs.SelectedCells)
igtbl_selectCell(gn,cell,false,false);}
function igtbl_selectCell(gn,cellID,selFlag,fireEvent)
{var cell=cellID;if(typeof(cell)=="string")
cell=igtbl_getCellById(cellID);if(!cell)
return;cell.select(selFlag,fireEvent);}
function igtbl_selectRow(gn,rowID,selFlag,fireEvent)
{var rowObj=rowID;if(typeof(rowObj)=="string")
rowObj=igtbl_getRowById(rowID);else
rowID=rowObj.Element.id;if(!rowObj)
return false;return rowObj.select(selFlag,fireEvent);}
function igtbl_selColRI(gn,column,bandNo,colNo,nonFixed)
{var cellElems=igtbl_enumColumnCells(gn,column);for(var i=0;i<cellElems.length;i++)
{var visElem=cellElems[i];igtbl_changeStyle(gn,visElem,igtbl_getSelCellClass(gn,bandNo,colNo));}
igtbl_changeStyle(gn,column,igtbl_getSelHeadClass(gn,bandNo,colNo));igtbl_dispose(cellElems);}
function igtbl_selectColumn(gn,columnID,selFlag,fireEvent)
{var column=igtbl_getElementById(columnID);var colObj=igtbl_getColumnById(columnID);if(!colObj)return;var bandNo=colObj.Band.Index;if(igtbl_getSelectTypeColumn(gn,bandNo)<2)
return;var colNo=colObj.Index;var gs=igtbl_getGridById(gn);if(gs._exitEditCancel||gs._noCellChange)
return;if(fireEvent!=false)
if(igtbl_fireEvent(gn,gs.Events.BeforeSelectChange,"(\""+gn+"\",\""+columnID+"\")")==true)
return;var nonFixed=gs.UseFixedHeaders&&!colObj.getFixed();var aRow=null;var aCell=gs.getActiveCell();if(aCell&&aCell.Column!=colObj)
aCell=null;else if(!aCell)
aRow=gs.getActiveRow();if(selFlag!=false)
{var cols=igtbl_getDocumentElement(columnID);if(cols)
{if(cols.length)
for(var j=0;j<cols.length;j++)
igtbl_selColRI(gn,cols[j],bandNo,colNo,nonFixed);else
igtbl_selColRI(gn,column,bandNo,colNo,nonFixed);gs._recordChange("SelectedColumns",colObj,gs.GridIsLoaded.toString());colObj.Selected=true;gs.Element.setAttribute("lastSelectedColumn",columnID);}}
else
{var cols=igtbl_getDocumentElement(columnID);if(!cols.length)
cols=[cols];for(var j=0;j<cols.length;j++)
{var colsj=cols[j];igtbl_changeStyle(gn,colsj,null);var cellElems=igtbl_enumColumnCells(gn,cols[j]);for(var i=0;i<cellElems.length;i++)
{var cell=cellElems[i];var row=cell.parentNode;if(!row.getAttribute("hiddenRow")&&!gs.SelectedRows[row.id]&&!gs.SelectedCells[cell.id])
igtbl_changeStyle(gn,cell,null);}
igtbl_dispose(cellElems);}
gs._removeChange("SelectedColumns",colObj);colObj.Selected=false;}
if(aRow)
aRow.renderActive();if(aCell)
aCell.renderActive();if(fireEvent!=false)
{var gsNPB=gs.NeedPostBack;igtbl_fireEvent(gn,gs.Events.AfterSelectChange,"(\""+gn+"\",\""+columnID+"\");");if(!gsNPB&&!(gs.Events.AfterSelectChange[1]&4))
gs.NeedPostBack=false;if(gs.NeedPostBack)
igtbl_moveBackPostField(gn,"SelectedColumns");}}
function igtbl_gSelectArray(gn,elem,array)
{var gs=igtbl_getGridById(gn);gs._noCellChange=false;if(elem==0)
{var oldSelCells=gs.SelectedCells;gs.SelectedCells=new Object();for(var i=0;i<array.length;i++)
if(oldSelCells[array[i]])
gs.SelectedCells[array[i]]=true;var fireOnUnsel=true;for(var i=0;i<array.length;i++)
if(!oldSelCells[array[i]])
{igtbl_selectCell(gn,array[i]);fireOnUnsel=false;}
for(var cell in oldSelCells)
if(!gs.SelectedCells[cell])
igtbl_selectCell(gn,cell,false,fireOnUnsel);for(var cell in oldSelCells)
delete oldSelCells[cell];}
else if(elem==1)
{var oldSelRows=gs.SelectedRows;gs.SelectedRows=new Object();for(var i=0;i<array.length;i++)
if(oldSelRows[array[i]])
gs.SelectedRows[array[i]]=true;var fireOnUnsel=true;for(var i=0;i<array.length;i++)
if(!oldSelRows[array[i]])
{igtbl_selectRow(gn,array[i]);fireOnUnsel=false;}
for(var row in oldSelRows)
if(!gs.SelectedRows[row])
igtbl_selectRow(gn,row,false,fireOnUnsel);for(var row in oldSelRows)
delete oldSelRows[row];}
else
{var oldSelCols=gs.SelectedColumns;gs.SelectedColumns=new Object();for(var i=0;i<array.length;i++)
if(oldSelCols[array[i]])
gs.SelectedColumns[array[i]]=true;var fireOnUnsel=true;for(var i=0;i<array.length;i++)
if(!oldSelCols[array[i]])
{igtbl_selectColumn(gn,array[i]);fireOnUnsel=false;}
for(var col in oldSelCols)
if(!gs.SelectedColumns[col])
igtbl_selectColumn(gn,col,false,fireOnUnsel);for(var col in oldSelCols)
delete oldSelCols[col];}}
function igtbl_initStatHeader(gs)
{this.Type="statHeader";this.gridId=gs.Id;this.Element=gs._tdContainer.parentNode.previousSibling.childNodes[0].childNodes[0].childNodes[0].childNodes[1];this.ScrollTo=igtbl_scrollStatHeader;this.getElementByColumn=igtbl_shGetElemByCol;if(!gs.Bands[0].HasHeaderLayout)
_igtbl_headerOrFooterHeight(this.Element);var outlGB=false;if(gs.Rows&&gs.Rows.length>0&&(row=gs.Rows.getRow(0)).GroupByRow)
outlGB=true;if(!gs.UseFixedHeaders)
{var row;if(outlGB)
{while(row.GroupByRow&&row.Rows&&row.Rows.length>0)
row=row.Rows.getRow(0);if(row.GroupByRow)
{for(var i=0;i<this.Element.childNodes[0].childNodes.length;i++)
{var col=this.Element.childNodes[0].childNodes[i];if(col.getAttribute("columnNo"))
{var colNo=parseInt(col.getAttribute("columnNo"));gs.Bands[0].Columns[colNo].Element=col;}}
return;}}
for(var i=0;i<this.Element.childNodes[0].childNodes.length;i++)
{var col=this.Element.childNodes[0].childNodes[i];if(col.getAttribute("columnNo"))
{var colNo=parseInt(col.getAttribute("columnNo"));gs.Bands[0].Columns[colNo].Element=col;}}}
else
{var childNodes=this.Element.childNodes[0].childNodes;var i=0;while(i<childNodes.length)
{var col=childNodes[i];i++;if(col.getAttribute("columnNo"))
{var colNo=parseInt(col.getAttribute("columnNo"));gs.Bands[0].Columns[colNo].Element=col;}
else if(col.colSpan>1&&col.firstChild.tagName=="DIV"&&col.firstChild.id.substr(col.firstChild.id.length-4)=="_drs")
{childNodes=col.firstChild.firstChild.childNodes[1].rows[0].childNodes;i=0;}}}
var comWidth=gs.Element.offsetWidth==0?gs.Element.style.width:gs.Element.offsetWidth;var hasPercWidth=gs.Element.style.width.indexOf("%")>0;if(typeof(comWidth)=="number"||(typeof(comWidth)=="string"&&comWidth.indexOf("%")==-1))
{comWidth=(typeof(comWidth)=="string")?igtbl_parseInt(comWidth):comWidth;if((gs.AllowUpdate==1||gs.Bands[0].AllowUpdate==1)&&!hasPercWidth)
comWidth--;if(outlGB)
{comWidth-=_igtbl_headerRowIndentation(gs,gs._AddnlProps[8].split(";"));}
if(hasPercWidth&&igtbl_dom.table.hasPercentageColumns(gs.Element,gs.Bands[0].firstActiveCell))
this.Element.parentNode.style.width="100%"
else if(comWidth>0)
{comWidth=comWidth+"px";this.Element.parentNode.style.width=comWidth;if(!hasPercWidth&&gs.Element.style.width!=comWidth)
gs.Element.style.width=comWidth;}}
else if(comWidth>0)
{this.Element.parentNode.style.width=comWidth+"px";}}
function igtbl_scrollStatHeader(scrollLeft)
{var gs=igtbl_getGridById(this.gridId);var parentNodeStyle=this.Element.parentNode.style;if(!gs.UseFixedHeaders)
parentNodeStyle.left=-scrollLeft+"px";var hasPercWidth=gs.Element.style.width.indexOf("%")>0;var comWidth=gs.getDivElement().firstChild.offsetWidth;var hdrTblWidth=comWidth;if(gs.Rows&&gs.Rows.length>0&&(row=gs.Rows.getRow(0)).GroupByRow)
{var hdrRowInd=_igtbl_headerRowIndentation(gs,gs.Bands[0].SortedColumns);comWidth-=hdrRowInd;hdrTblWidth-=2*hdrRowInd;}
if(parentNodeStyle.width&&comWidth>0&&gs.Element.offsetWidth>comWidth)
{comWidth=comWidth+"px";hdrTblWidth=hdrTblWidth+"px";parentNodeStyle.width=hdrTblWidth;if(!hasPercWidth&&gs.Element.style.width&&gs.Element.style.width!=comWidth)
gs.Element.style.width=comWidth;}}
function igtbl_initStatFooter(gs)
{this.Type="statFooter";this.ScrollTo=igtbl_scrollStatFooter;this.Resize=igtbl_resizeStatFooter;this.getElementByColumn=igtbl_sfGetElemByCol;this.gridId=gs.Id;var tbl=gs._tdContainer.parentNode.nextSibling.firstChild.firstChild.firstChild;this.Element=tbl.rows[tbl.rows.length-1].parentNode;_igtbl_headerOrFooterHeight(this.Element);var comWidth=gs.Element.offsetWidth;var hasPercWidth=gs.Element.style.width.indexOf("%")>0;if((gs.AllowUpdate==1||gs.Bands[0].AllowUpdate==1)&&!hasPercWidth)
comWidth--;if(gs.Rows&&gs.Rows.length>0&&(row=gs.Rows.getRow(0)).GroupByRow)
{comWidth-=_igtbl_headerRowIndentation(gs,gs._AddnlProps[8].split(";"));}
if(comWidth>0)
{comWidth=comWidth+"px";this.Element.parentNode.style.width=comWidth;}}
function igtbl_scrollStatFooter(scrollLeft)
{var gs=igtbl_getGridById(this.gridId);if(!gs.UseFixedHeaders)
this.Element.parentNode.style.left=-scrollLeft+"px";var comWidth=gs.Element.offsetWidth;if(gs.Rows&&gs.Rows.length>0&&(row=gs.Rows.getRow(0)).GroupByRow)
{comWidth-=_igtbl_headerRowIndentation(gs,gs.Bands[0].SortedColumns);}
if(this.Element.parentNode.style.width&&comWidth>0)
{comWidth=comWidth+"px";this.Element.parentNode.style.width=comWidth;}}
function igtbl_resizeStatFooter(index,width)
{var c1w=width;var gs=igtbl_getGridById(this.gridId);var column=gs.Bands[0].Columns[index];var el=igtbl_getDocumentElement(column.fId);if(el&&el.length&&el.length>0)
{el=el[el.length-1];}
var spannedFooter=false;if(!el)
{el=igtbl_getElemVis(gs.StatFooter.Element.childNodes[0].childNodes,index);spannedFooter=true;}
if(el)
{var cg=el.parentNode.parentNode.previousSibling;var anCell=null;if(gs.Rows.AddNewRow&&gs.Bands[0].AddNewRowView==2)
{cg=cg.previousSibling;anCell=gs.Rows.AddNewRow.getCellByColumn(column);}
while(cg&&cg.tagName!='COLGROUP')
{cg=cg.previousSibling;}
var c;if(cg)
{c=cg.childNodes[anCell?anCell.getElement().cellIndex:el.cellIndex];}
else
c=el;c.style.width=c1w+"px";el.style.width=c1w+"px";if(gs.UseFixedHeaders&&column&&!column.getFixed())
{var d=c.style.display;c.style.display="none";c.style.display=d;}}}
function _igtbl_headerOrFooterHeight(el)
{if(el.parentNode.offsetHeight==0)
return;if(el.parentNode.offsetHeight==0)
{var chn=el.firstChild.firstChild;while(chn&&!chn.height)
{chn=chn.nextSibling;}
if(chn&&chn.height)
{var chnH;if(chn.currentStyle)
{chnH=parseInt(chn.currentStyle.height)+parseInt(chn.currentStyle.borderBottomWidth)+parseInt(chn.currentStyle.borderTopWidth)
if(isNaN(chnH))
{chnH=chn.height;}
else
{chnH+="px";}}
else
{chnH=chn.height;}
el.parentNode.parentNode.style.height=chnH;}
else
{el.parentNode.parentNode.style.height="20px";}}
else
{el.parentNode.parentNode.style.height=el.parentNode.offsetHeight+"px";}}
function _igtbl_headerRowIndentation(gs,sc)
{var indentation0=gs.Bands[0].getIndentation();var result=0;for(var i=0;i<sc.length;i++)
{var col=igtbl_getColumnById(sc[i]);if(!col||col.Band.Index>0||!col.IsGroupBy)
{break;}
result+=indentation0;}
return result;}
igtbl_browserWorkarounds={ieBorderCollapseArtifacts:function(row,h)
{var table=row.Element.parentNode.parentNode;if(!ig_csom.IsIE||table.currentStyle.borderCollapse!="collapse"||table.currentStyle.tableLayout=="fixed")
{return;}
for(var x=row.cells.length-1;x>=0;x--)
{var cell=row.getCell(x);var col=cell.Column;cell.Element.style.display=(col.Hidden||h?"none":"");}},ieTabScrollBarAdjustment:function(firstBand)
{if(!ig_csom.IsIE)return;if(firstBand)
{var firstColumn=firstBand.Columns[0];while(firstColumn&&firstColumn.getHidden())
firstColumn=firstBand.Columns[firstColumn.Index+1]
if(firstColumn)firstColumn.setWidth(firstColumn.getWidth());}},addActiveElementTracking:function()
{if(typeof(document.activeElement)=="undefined"&&!this.isTrackingActiveElement)
{ig_csom.addEventListener(document,"focus",this.trackActiveElement);this.isTrackingActiveElement=true;}},removeActiveElementTracking:function()
{if(this.isTrackingActiveElement)
{ig_csom.removeEventListener(document,"focus",this.trackActiveElement);igtbl_browserWorkarounds.activeElement=null;}},trackActiveElement:function(e)
{var evnt=igtbl_event.getEvent(e);igtbl_browserWorkarounds.activeElement=igtbl_srcElement(evnt);}};function igtbl_fixDOEXml()
{if(ig_csom.IsNetscape6)
{var doeElems=document.getElementsByName("_igdoe");for(var i=doeElems.length-1;i>=0;i--)
{var doe=doeElems[i];doe.innerHTML=doe.textContent;doe.removeAttribute("name");}}}
igtbl_string={stringToBool:function(value)
{if(value==="true"||value===true)
return true;return false;},trim:function(s)
{if(!s)
return s;s=s.toString();var result=s;for(var i=0;i<s.length;i++)
if(s.charAt(i)!=' ')
break;result=s.substr(i,s.length-i);for(var i=result.length-1;i>=0;i--)
if(result.charAt(i)!=' ')
break;result=result.substr(0,i+1);return result;},isNullOrEmpty:function(object,property)
{if(typeof(object[property])=="undefined")return true;if(object[property]==="")return true;if(object[property]===null)return true;return false;},toString:function(object)
{if(typeof(object)!=undefined&&object!=null&&typeof(object.toString)=="function")
return object.toString();return"";}};igtbl_number={fromString:function(number)
{if(number)
{var outValue=parseInt(number,10);if(!isNaN(outValue))
return outValue;}
return 0;},sortNumber:function(a,b)
{return a[0]-b[0];},isNumberType:function(dataType)
{switch(dataType)
{case igtbl_dataType.Int16:case igtbl_dataType.Int32:case igtbl_dataType.Byte:case igtbl_dataType.SByte:case igtbl_dataType.UInt16:case igtbl_dataType.UInt32:case igtbl_dataType.Int64:case igtbl_dataType.UInt64:case igtbl_dataType.Single:case igtbl_dataType.Double:case igtbl_dataType.Decimal:return true;}
return false;}};function igtbl_valueFromString(value,dataType)
{if(typeof(value)=="undefined"||value==null)
return value;switch(dataType)
{case igtbl_dataType.Int16:case igtbl_dataType.Int32:case igtbl_dataType.Byte:case igtbl_dataType.SByte:case igtbl_dataType.UInt16:case igtbl_dataType.UInt32:case igtbl_dataType.Int64:case igtbl_dataType.UInt64:if(typeof(value)=="number")
return value;if(typeof(value)=="boolean")
return(value?1:0);if(value.toString().toLowerCase()=="true")
return 1;value=parseInt(value.toString(),10);if(value.toString()=="NaN")
value=0;break;case igtbl_dataType.Single:case igtbl_dataType.Double:case igtbl_dataType.Decimal:if(typeof(value)=="float")
return value;value=parseFloat(value.toString());if(value.toString()=="NaN")
value=0.0;break;case igtbl_dataType.Boolean:if(!value||value.toString()=="0"||value.toString().toLowerCase()=="false")
value=false;else
value=true;break;case igtbl_dataType.DateTime:var d;if(typeof(value)=="string")
{var dtV=value.split(".");var ms=0,lastPart=dtV.length>1?dtV[1].substr(dtV[1].length-3).toUpperCase():"";if(dtV.length>1&&(lastPart==" AM"||lastPart==" PM"))
{ms=igtbl_parseInt(dtV[1]);dtV[0]+=lastPart;}
else
dtV[0]=value;d=new Date(dtV[0]);if(!isNaN(d))
d.setMilliseconds(ms);}
else
d=new Date(value);if(d.toString()!="NaN"&&d.toString()!="Invalid Date")
value=d;else
value=igtbl_string.trim(value.toString());delete d;break;case igtbl_dataType.String:break;default:value=igtbl_string.trim(value.toString());}
return value;}
function igtbl_dateToString(date)
{if(date==null)
return"";if(typeof(date.getFullYear)!="function")
return date.toString();var month=date.getMonth();var day=date.getDate();var year=date.getFullYear();var hour=date.getHours();var min=date.getMinutes();var sec=date.getSeconds();var ms=date.getMilliseconds();return(month+1).toString()+"/"+day.toString()+"/"+
(year.toString().length>4?year.toString().substr(0,4):year)+" "+
(hour==0?"12":(hour%12).toString())+":"+(min<10?"0":"")+
min+":"+(sec<10?"0":"")+sec+
igtbl_dateMsToString(date)+" "+(hour<12?"AM":"PM");}
function igtbl_dateMsToString(date)
{var ms=date.getMilliseconds();if(ms==0)
return"";if(ms<10)
return".00"+ms.toString();if(ms<100)
return".0"+ms.toString();return"."+ms.toString();}
function igtbl_parseInt(inValue)
{var outValue=parseInt(inValue,10);if(isNaN(outValue))
outValue=0;return outValue;}
function igtbl_trim(s)
{if(!s)
return s;s=s.toString();var result=s;for(var i=0;i<s.length;i++)
if(s.charAt(i)!=' ')
break;result=s.substr(i,s.length-i);for(var i=result.length-1;i>=0;i--)
if(result.charAt(i)!=' ')
break;result=result.substr(0,i+1);return result;}
igtbl_debug={writeLine:function(message)
{},writeStackTrace:function(startingPoint)
{}}
function igtbl_contains(e1,e2)
{if(e1.contains)
return e1.contains(e2);var contains=false;var p=e2;while(p&&p!=e1)
p=p.parentNode;return p==e1;}
function igtbl_getStyleSheet(name)
{var nameAr=name.split(".");if(nameAr.length>2)
return null;else if(nameAr.length==2)
{if(ig_csom.IsIE)
nameAr[0]=nameAr[0].toUpperCase();else
nameAr[0]=nameAr[0].toLowerCase();name=nameAr.join(".");}
else
name="."+name;for(var i=0;i<document.styleSheets.length;i++)
{var ssrules=null;try
{if(ig_csom.IsIE)
ssrules=document.styleSheets[i].rules;else
ssrules=document.styleSheets[i].cssRules;}catch(e){;}
if(ssrules)
for(var j=0;j<ssrules.length;j++)
if(ssrules[j].selectorText==name)
return ssrules[j].style;}
return null;}
function igtbl_getCurrentStyleProperty(e,propName,forceCalc)
{if(e&&e.tagName&&ig_csom.IsIE&&!forceCalc)
return e.currentStyle[propName];else
{if(e&&e.tagName&&e.style[propName])
return e.style[propName];var className=e;if(e&&e.tagName)
className=e.className;if(className)
{var clsNames=className.split(" ");clsNames=clsNames.reverse();for(var i=0;i<clsNames.length;i++)
{var style=igtbl_getStyleSheet(clsNames[i]);if(style&&style[propName])
return style[propName];}}}
return"";}
function igtbl_getArray(elem)
{if(!elem)return null;var a=new Array();if(!elem.length)
a[0]=elem;else
for(var i=0;i<elem.length;i++)
a[i]=elem[i];return a;}
function igtbl_getCheckboxFromElement(oCellE)
{var oChk=null;for(var i=0;i<oCellE.childNodes.length;i++)
{if(oCellE.childNodes[i].tagName=="INPUT"&&oCellE.childNodes[i].type=="checkbox")
oChk=oCellE.childNodes[i];else
oChk=igtbl_getCheckboxFromElement(oCellE.childNodes[i])
if(oChk)break;}
return oChk;}
igtbl_dom={isParent:function(child,parent)
{if(child==null||parent==null)return false;var possibleParent=child.parentNode;while(possibleParent!=null)
{if(possibleParent==parent)
return true;possibleParent=possibleParent.parentNode;}
return false;},isTag:function(element,type)
{if(!element)
return false;if(!igtbl_array.isList(type))
type=[type];for(var x=0;x<type.length;x++)
{if(element.tagName===type[x])
return true;else if(typeof(type[x].toUpperCase)!="undefined"&&element.tagName&&element.tagName.toUpperCase()===type[x].toUpperCase())
return true;}
return false;},hasVisibleStyle:function(elem)
{if(igtbl_dom.css.getComputedStyle(elem,"display")!="none"&&igtbl_dom.css.getComputedStyle(elem,"visibility")!="hidden")
{return true;}
return false;},isVisible:function(elem)
{while(elem&&elem.tagName!=(igtbl_isXHTML?"HTML":"BODY"))
{if(elem.style&&elem.style.display=="none"||elem.tagName!="FORM"&&elem.tagName!="BODY"&&!elem.offsetHeight)
return false;elem=elem.parentNode;}
return true;},find:{elementById:function(tagId)
{if(!document)return;var obj=ig_csom.getElementById(tagId);if(obj&&obj.length&&typeof(obj.tagName)=="undefined")
{var i=0;while(i<obj.length&&(obj[i].id!=tagId||!igtbl_isVisible(obj[i])))i++;if(i<obj.length)obj=obj[i];else obj=obj[0];}
return obj;},parentByTag:function(element,parentType)
{var parent=element;while(parent&&!igtbl_dom.isTag(parent,parentType))
parent=parent.parentNode;return parent;},parentForm:function(elem)
{if(!elem)return null;var thisForm=igtbl_dom.find.parentByTag(elem,"FORM");if(!thisForm&&document.forms&&document.forms.length==1)
thisForm=document.forms[0];return thisForm;},childByTag:function(element,childType)
{if(element)
{for(var x=0;x<element.childNodes.length;x++)
{var child=element.childNodes[x];if(igtbl_dom.isTag(child,childType))
return child;var foundChild=igtbl_dom.find.childByTag(child,childType);if(foundChild)
return foundChild;}}
return null;},childrenByPath:function(element,path)
{var pathElements=path.split("/");var matches=[];if(pathElements.length>0)
{var elementToFind=pathElements[0];for(var x=0;x<element.childNodes.length;x++)
{var childNode=element.childNodes[x];if(igtbl_dom.isTag(childNode,elementToFind))
{if(elementToFind==path)
matches.push(childNode);else
return igtbl_dom.find.childrenByPath(childNode,path.substring(elementToFind.length+1));}}}
return matches;},childById:function(parent,id)
{if(!id||!parent.childNodes||!parent.childNodes.length)
return null;for(var i=0;i<parent.childNodes.length;i++)
try
{if(parent.childNodes[i].id&&parent.childNodes[i].id==id)
return parent.childNodes[i];var che=igtbl_dom.find.childById(parent.childNodes[i],id);if(che)
return che;}catch(ex){;}
return null;},childrenById:function(parent,id)
{if(!id||!parent.childNodes||!parent.childNodes.length)
return null;var array=[];for(var i=0;i<parent.childNodes.length;i++)
{try
{if(parent.childNodes[i].id&&parent.childNodes[i].id==id)
array[array.length]=parent.childNodes[i];var ches=igtbl_dom.find.childrenById(parent.childNodes[i],id);if(ches)
array=array.concat(ches);}
catch(ex){;}}
if(array.length)
return array;return null;},rootNode:function(element)
{if(!element)return;while(element.parentNode)
element=element.parentNode;return element;}},css:{getComputedStyle:function(element,property)
{if(typeof(window.getComputedStyle)!="undefined")
return window.getComputedStyle(element,"")[property];if(typeof(element.currentStyle)!="undefined")
return element.currentStyle[property];return element.style[property];},removeClass:function(element,className)
{element.className=element.className.replace(className,"");},replaceClass:function(element,oldClassName,newClassName)
{igtbl_dom.css.removeClass(element,oldClassName);if(element.className.indexOf(newClassName)==-1)
element.className+=" "+newClassName;}},dimensions:{bordersWidth:function(element,includePadding)
{var width=0;if(element.offsetWidth&&element.clientHeight)
width+=element.offsetWidth-element.clientWidth;if(includePadding&&(!ig_csom.IsIE||igtbl_isXHTML))
{width+=igtbl_number.fromString(igtbl_dom.css.getComputedStyle(element,"paddingLeft"));width+=igtbl_number.fromString(igtbl_dom.css.getComputedStyle(element,"paddingRight"));}
return width;},bordersHeight:function(element,includePadding)
{var height=0;if(element.offsetHeight&&element.clientHeight)
height+=element.offsetHeight-element.clientHeight;if(includePadding&&(!ig_csom.IsIE||igtbl_isXHTML))
{height+=igtbl_number.fromString(igtbl_dom.css.getComputedStyle(element,"paddingTop"));height+=igtbl_number.fromString(igtbl_dom.css.getComputedStyle(element,"paddingBottom"));}
return height;},hasPercentageWidth:function(elem)
{var width=igtbl_dom.css.getComputedStyle(elem,"width");if(width&&width.indexOf("%")>0)
return true;if(elem.width&&elem.width.indexOf("%")>0)
return true;},hasEmptyWidth:function(elem)
{var width=igtbl_dom.css.getComputedStyle(elem,"width");if(width=="auto")
return true;return false;}},table:{allPercentageColumns:function(elem,startIndex)
{if(!igtbl_dom.isTag(elem,"TABLE"))return false;if(startIndex==undefined)startIndex=0;var cols=igtbl_dom.find.childrenByPath(elem,"colgroup/col");for(var x=startIndex;x<cols.length;x++)
{var col=cols[x];if(igtbl_dom.hasVisibleStyle(col))
{if(igtbl_dom.dimensions.hasEmptyWidth(col))
return true;if(!igtbl_dom.dimensions.hasPercentageWidth(col))
return false;}}
return true;},hasPercentageColumns:function(elem,startIndex)
{if(!igtbl_dom.isTag(elem,"TABLE"))return false;if(startIndex==undefined)startIndex=0;var cols=igtbl_dom.find.childrenByPath(elem,"colgroup/col");for(var x=startIndex;x<cols.length;x++)
{var col=cols[x];if(igtbl_dom.hasVisibleStyle(col))
{if(igtbl_dom.dimensions.hasEmptyWidth(col))
return true;if(igtbl_dom.dimensions.hasPercentageWidth(col))
return true;}}
return false;}}};igtbl_stylesheet={addRule:function(rule)
{if(typeof(document.styleSheets)!="undefined"&&document.styleSheets.length>0)
{document.styleSheets[0].insertRule(rule,0);}}};igtbl_event={getEvent:function(evnt)
{if(typeof(evnt)!="undefined")
return evnt;return event;},addEventListener:function(obj,eventName,fRef,dispatch)
{if(typeof(dispatch)=="undefined")
dispatch=true;if(obj.addEventListener)
return obj.addEventListener(eventName,fRef,dispatch);else
{var oldHandler=obj["on"+eventName];eval("obj.on"+eventName+"=fRef;");return oldHandler;}},removeEventListener:function(obj,eventName,fRef,oldfRef,dispatch)
{if(typeof(dispatch)=="undefined")
dispatch=true;if(obj.removeEventListener)
return obj.removeEventListener(eventName,fRef,dispatch);else
eval("obj.on"+eventName+"=oldfRef;");}};igtbl_browser={}
function igtbl_getClipboardData()
{if(window.clipboardData)
{return window.clipboardData.getData("Text");}
else if(ig_shared.IsFireFox||ig_shared.IsNetscape)
{netscape.security.PrivilegeManager.enablePrivilege('UniversalXPConnect');var clip=Components.classes["@mozilla.org/widget/clipboard;1"].createInstance(Components.interfaces.nsIClipboard);var trans=Components.classes["@mozilla.org/widget/transferable;1"].createInstance(Components.interfaces.nsITransferable);trans.addDataFlavor("text/unicode");clip.getData(trans,clip.kGlobalClipboard);var str=new Object();var len=new Object();trans.getTransferData("text/unicode",str,len);if(str)
return str.value.QueryInterface(Components.interfaces.nsISupportsString).toString();}}
function igtbl_setClipboardData(copytext)
{if(window.clipboardData)
{window.clipboardData.setData("Text",copytext);}
else if(ig_shared.IsFireFox||ig_shared.IsNetscape)
{netscape.security.PrivilegeManager.enablePrivilege('UniversalXPConnect');var clip=Components.classes['@mozilla.org/widget/clipboard;1'].createInstance(Components.interfaces.nsIClipboard);var trans=Components.classes['@mozilla.org/widget/transferable;1'].createInstance(Components.interfaces.nsITransferable);trans.addDataFlavor('text/unicode');var str=Components.classes["@mozilla.org/supports-string;1"].createInstance(Components.interfaces.nsISupportsString);str.data=copytext;trans.setTransferData("text/unicode",str,copytext.length*2);var clipid=Components.interfaces.nsIClipboard;clip.setData(trans,null,clipid.kGlobalClipboard);}
else
return false;return true;}
function igtbl_isAChildOfB(a,b)
{if(a==null||b==null)return false;while(a!=null)
{if(a==b)return true;a=a.parentNode;}
return false;}
function igtbl_getThisForm(elem)
{if(!elem)
return null;var thisForm=elem.parentNode;while(thisForm&&thisForm.tagName!="FORM")
thisForm=thisForm.parentNode;if(!thisForm&&document.forms&&document.forms.length==1)
thisForm=document.forms[0];return thisForm;}
function igtbl_addEventListener(obj,eventName,fRef,dispatch)
{if(typeof(dispatch)=="undefined")
dispatch=true;if(obj.addEventListener)
return obj.addEventListener(eventName,fRef,dispatch);else
{var oldHandler=obj["on"+eventName];eval("obj.on"+eventName+"=fRef;");return oldHandler;}}
function igtbl_removeEventListener(obj,eventName,fRef,oldfRef,dispatch)
{if(typeof(dispatch)=="undefined")
dispatch=true;if(obj.removeEventListener)
return obj.removeEventListener(eventName,fRef,dispatch);else
eval("obj.on"+eventName+"=oldfRef;");}
function igtbl_isVisible(elem)
{while(elem&&elem.tagName!=(igtbl_isXHTML?"HTML":"BODY"))
{if(elem.style&&elem.style.display=="none"||elem.tagName!="FORM"&&elem.tagName!="BODY"&&!elem.offsetHeight)
return false;elem=elem.parentNode;}
return true;}
function igtbl_getElementById(tagId)
{if(!document)return;var obj=ig_csom.getElementById(tagId);if(obj&&obj.length&&typeof(obj.tagName)=="undefined")
{var i=0;while(i<obj.length&&(obj[i].id!=tagId||!igtbl_isVisible(obj[i])))i++;if(i<obj.length)obj=obj[i];else obj=obj[0];}
return obj;}
function igtbl_getChildElementsById(parent,id)
{if(!id||!parent.childNodes||!parent.childNodes.length)
return null;var array=[];for(var i=0;i<parent.childNodes.length;i++)
try
{if(parent.childNodes[i].id&&parent.childNodes[i].id==id)
array[array.length]=parent.childNodes[i];var ches=igtbl_getChildElementsById(parent.childNodes[i],id);if(ches)
array=array.concat(ches);}
catch(ex){;}
if(array.length)
return array;return null;}
function igtbl_getChildElementById(parent,id)
{if(!id||!parent.childNodes||!parent.childNodes.length)
return null;for(var i=0;i<parent.childNodes.length;i++)
try
{if(parent.childNodes[i].id&&parent.childNodes[i].id==id)
return parent.childNodes[i];var che=igtbl_getChildElementById(parent.childNodes[i],id);if(che)
return che;}catch(ex){;}
return null;}
igtbl_array={contains:function(array,element)
{for(var x in array)
{if(array[x]===element)
return true;}
return false;},exclude:function(array,exclude)
{var newArray=[];for(var x in array)
{if(!igtbl_array.contains(exclude,array[x]))
newArray.push(array[x]);}
return newArray;},isList:function(value)
{return value!=null&&typeof(value)=="object"&&typeof(value.length)=="number"&&(value.length==0||typeof(value[0])!="undefined");},hasElements:function(array)
{if(!array)
return false;for(element in array)
if(array[element]!=null)
return true;return false;},getLength:function(obj)
{var count=0;for(var item in obj)
count++;return count;}};function igtbl_arrayHasElements(array)
{if(!array)
return false;for(element in array)
if(array[element]!=null)
return true;return false;}
function igtbl_getLength(obj)
{var count=0;for(var item in obj)
count++;return count;}
igtbl_nav={splitUrl:function(url)
{var targetFrame=null;if(url.substr(0,1)=="@")
{targetFrame="_blank";url=url.substr(1);var cb=-1;if(url.substr(0,1)=="["&&(cb=url.indexOf("]"))>1)
{targetFrame=url.substr(1,cb-1);url=url.substr(cb+1);}}
return[url,targetFrame];},navigateUrl:function(url)
{var urls=igtbl_splitUrl(url);ig_csom.navigateUrl(urls[0],urls[1]);igtbl_dispose(urls);}}
function igtbl_escape(text)
{text=escape(text);return text.replace(/\+/g,"%2b");}
function igtbl_splitUrl(url)
{var targetFrame=null;if(url.substr(0,1)=="@")
{targetFrame="_blank";url=url.substr(1);var cb=-1;if(url.substr(0,1)=="["&&(cb=url.indexOf("]"))>1)
{targetFrame=url.substr(1,cb-1);url=url.substr(cb+1);}}
return[url,targetFrame];}
function igtbl_navigateUrl(url)
{var urls=igtbl_splitUrl(url);ig_csom.navigateUrl(urls[0],urls[1]);igtbl_dispose(urls);}
function igtbl_getLeftPos(e,cc,oe)
{return igtbl_getAbsolutePos("Left",e,cc,oe);}
function igtbl_getTopPos(e,cc,oe)
{return igtbl_getAbsolutePos("Top",e,cc,oe);}
function igtbl_getAbsolutePos(where,e,cc,oe)
{if(ig_csom.IsIE&&igtbl_isXHTML&&e.getBoundingClientRect)
{switch(where)
{case"Left":return igtbl_getAbsBounds(e).x;case"Top":return igtbl_getAbsBounds(e).y;}}
return igtbl_getAbsolutePos2(where,e,cc,oe);}
function igtbl_getAbsolutePos2(where,e,cc,oe)
{var offs="offset"+where,cl="client"+where,bw="border"+where+"Width",sl="scroll"+where;var crd=e[offs];if(e[cl]&&cc!=false)
crd+=e[cl];if(typeof(oe)=="undefined")
oe=null;var tmpE=e.offsetParent,cSb=true;while(tmpE!=null&&tmpE!=oe)
{crd+=tmpE[offs];if((tmpE.tagName=="DIV"||tmpE.tagName=="TD")&&tmpE.style[bw])
{var bwv=parseInt(tmpE.style[bw],10);if(!isNaN(bwv))
crd+=bwv;}
if(cSb&&typeof(tmpE[sl])!="undefined")
{var op=tmpE.offsetParent,t=tmpE;while(t&&t!=op&&t.tagName!=(igtbl_isXHTML?"HTML":"BODY"))
{if(t[sl])
crd-=t[sl];t=t.parentNode;}}
if(tmpE[cl]&&cc!=false)
crd+=tmpE[cl];tmpE=tmpE.offsetParent;}
if(tmpE&&tmpE[cl]&&cc!=false)
crd+=tmpE[cl];return crd;}
function igtbl_getAbsBounds(elem,g,forAbsPos)
{var r=new Object();if(ig_csom.IsIE&&elem.getBoundingClientRect)
{var rect=elem.getBoundingClientRect();r.x=rect.left;r.y=rect.top;r.w=rect.right-rect.left;r.h=rect.bottom-rect.top;}
else if(document.getBoxObjectFor && ig_csom.IsFireFox&&document.getBoxObjectFor(elem))
{var rect=document.getBoxObjectFor(elem);r.x=rect.x;r.y=rect.y;r.w=rect.width;r.h=rect.height;}
else
{return igtbl_getAbsBounds2(elem,g);}
var tmpE=elem;var passedMain=false;while(tmpE&&tmpE!=document.documentElement)
{passedMain|=g!=null&&tmpE==g.MainGrid;if(forAbsPos)
{}
else
{if(tmpE.scrollLeft)
{r.x+=tmpE.scrollLeft;}
if(tmpE.scrollTop)
{r.y+=tmpE.scrollTop;}
if(!igtbl_isXHTML)
{var left=parseInt(tmpE.style.left);if(!isNaN(left))
{if(left<0)
r.x-=left;}}}
if(tmpE==elem)
{if(typeof(tmpE.currentStyle)!="undefined")
{if(!igtbl_isXHTML)
{var cs=tmpE.currentStyle;var bw=0;var b=cs.borderLeftWidth.toLowerCase();if(b=="thin")
bw++;else if(b=="medium")
bw+=3;else if(b=="thick")
bw+=5;else
bw+=igtbl_parseInt(b);b=cs.borderRightWidth.toLowerCase();if(b=="thin")
bw++;else if(b=="medium")
bw+=3;else if(b=="thick")
bw+=5;else
bw+=igtbl_parseInt(b);r.w-=bw;bw=0;b=cs.borderTopWidth.toLowerCase();if(b=="thin")
bw++;else if(b=="medium")
bw+=3;else if(b=="thick")
bw+=5;else
bw+=igtbl_parseInt(b);b=cs.borderBottomWidth.toLowerCase();if(b=="thin")
bw++;else if(b=="medium")
bw+=3;else if(b=="thick")
bw+=5;else
bw+=igtbl_parseInt(b);r.h-=bw;}}
else
{if(tmpE.offsetWidth&&tmpE.clientWidth)
{var xDiff=tmpE.offsetWidth-tmpE.clientWidth;r.x-=xDiff/2;r.w-=xDiff;}
if(tmpE.offsetHeight&&tmpE.clientHeight)
{var yDiff=tmpE.offsetHeight-tmpE.clientHeight;r.y-=yDiff/2;r.h-=yDiff;}}}
tmpE=tmpE.offsetParent;}
if(!igtbl_isXHTML&&ig_csom.IsFireFox&&forAbsPos&&passedMain&&g!=null)
{var divElement=g.getDivElement();r.x-=divElement.scrollLeft;r.y-=divElement.scrollTop;}
if((tmpE=document.documentElement)&&!ig_csom.IsFireFox)
{try
{var frameEl=tmpE.document.parentWindow.frameElement;if(frameEl&&(frameEl.tagName=="IFRAME"||frameEl.tagName=="FRAME"))
{var fb=frameEl.getAttribute("frameBorder");if((fb&&(fb==="0"||fb.toLowerCase()==="no"))||(fb===""&&frameEl.tagName=="FRAME"))
{r.x+=2;r.y+=2;}}}
catch(exc){;}
if(tmpE.scrollLeft)
{r.x+=tmpE.scrollLeft;}
if(tmpE.scrollTop)
{r.y+=tmpE.scrollTop;}
if(!igtbl_isXHTML&&(tmpE=document.body))
{if(tmpE.scrollLeft)
{r.x+=tmpE.scrollLeft;}
if(tmpE.scrollTop)
{r.y+=tmpE.scrollTop;}}}
return r;}

function igtbl_getAbsBounds2(elem,g)
{
    var e = elem;
	var pos = {};
                pos.x = e.offsetLeft;
                pos.y = e.offsetTop;
                parent = e.offsetParent;
                if (parent != e) {
                    while (parent) {
                        pos.x += parent.offsetLeft;
                        pos.y += parent.offsetTop;
                        parent = parent.offsetParent;
                    }
                }
               
                parent = e.offsetParent;
                while (parent && parent != document.body) {
                    pos.x -= parent.scrollLeft;                   
                    if (parent.tagName != 'TR') {
                        pos.y -= parent.scrollTop;
                    }
                    parent = parent.offsetParent;
                }

                return pos;}

function igtbl_getRelativePos(gn,e,where,ignoreTableBorder)
{var g=igtbl_getGridById(gn);var mainGrid=igtbl_getElementById(gn+"_main");var passedMainGrid=false;var offs="offset"+where,bw="border"+where+"Width";var ovfl="overflow",ovflC=ovfl+(where=="Left"?"X":"Y");var crd=e[offs];var parent=e.offsetParent;if(!parent)
{if(e.tagName=="TD"||e.tagName=="TH")
{parent=e.parentNode;while(parent&&parent.tagName!="TABLE")
{parent=parent.parentNode;}}}
while((parent!=null&&parent.tagName!=(igtbl_isXHTML?"HTML":"BODY")&&(!passedMainGrid||parent.style.position!="relative"||(parent.style.position=="relative"&&parent.id=="G_"+gn))))
{passedMainGrid=passedMainGrid||igtbl_isAChildOfB(mainGrid.parentNode,parent);if(passedMainGrid&&(parent.style.position=="absolute"||parent.style[ovflC]&&parent.style[ovflC]!="visible"||parent.style[ovfl]&&parent.style[ovfl]!="visible"))
break;crd+=parent[offs];if(ig_csom.IsIE&&(parent.tagName=="DIV"||parent.tagName=="TD"||parent.tagName=="FIELDSET")&&parent.style[bw])
{var bwv=parseInt(parent.style[bw],10);if(!isNaN(bwv))
crd+=bwv;}
if(parent==mainGrid)
passedMainGrid=true;parent=parent.offsetParent;}
var deductScroll=true;if(where=="Top"&&g.StatHeader)
{while(e)
{if(e==g.StatHeader.Element)
{deductScroll=false;break;}
e=e.parentNode;}}
if(deductScroll)
crd-=g.Element.offsetParent["scroll"+where]
return crd;}
igtbl_regExp={escape:function(text,exclusions)
{if(typeof(text)=="undefined"||text==null)return"";var characters=["\\","^","$","*","+","?","!","-","=",":",",",".","|","(",")","{","}","[","]"];var includedCharacters=characters;if(exclusions)
includedCharacters=igtbl_array.exclude(characters,exclusions);for(var x in includedCharacters)
includedCharacters[x]="\\"+includedCharacters[x];return text.replace(new RegExp("("+includedCharacters.join("|")+")","g"),'\\$1');}};function igtbl_getRegExpSafe(val)
{if(typeof(val)=="undefined"||val==null)
return"";var res=val.toString();res=res.replace(/\\/g,"\\\\");res=res.replace(/\*/g,"\\*");res=res.replace(/\$/g,"\\$");res=res.replace(/\+/g,"\\+");res=res.replace(/\?/g,"\\?");res=res.replace(/\,/g,"\\,");res=res.replace(/\./g,"\\.");res=res.replace(/\:/g,"\\:");res=res.replace(/\=/g,"\\=");res=res.replace(/\-/g,"\\-");res=res.replace(/\!/g,"\\!");res=res.replace(/\|/g,"\\|");res=res.replace(/\(/g,"\\(");res=res.replace(/\)/g,"\\)");res=res.replace(/\[/g,"\\[");res=res.replace(/\]/g,"\\]");res=res.replace(/\{/g,"\\{");res=res.replace(/\}/g,"\\}");return res;}
igtbl_xml={createXmlElement:function(doc,tagName,ns)
{if(typeof(doc.createNode)!="undefined")
return doc.createNode(1,tagName,ns);else if(doc.createElement)
return doc.createElement(tagName);},createXmlTextNode:function(doc,ns)
{if(typeof(doc.createNode)!="undefined")
return doc.createNode(4,"",ns);else if(doc.createCDATASection)
return doc.createCDATASection("");},createDocumentFromString:function(xml)
{if(!ig_csom.IsIE)
{var objDOMParser=new DOMParser();return objDOMParser.parseFromString(xml,"text/xml");}
else
{var doc=new ActiveXObject("Microsoft.XMLDOM")
doc.async="false";doc.loadXML(xml);return doc;}},disposeDocument:function(node)
{if(node.parentNode)
node=igtbl_dom.find.rootNode(node);igtbl_xml.disposeNode(node);},disposeNode:function(node)
{while(node.childNodes.length>0)
igtbl_xml.disposeNode(node.childNodes[0]);if(node.parentNode)
{if(typeof(node.parentNode.removeChild)!="undefined")
node.parentNode.removeChild(node);}}};function igtbl_Object(type)
{if(arguments.length>0)
this.init(type);}
igtbl_Object.prototype.init=function(type)
{this.Type=type;}
igtbl_WebObject.prototype=new igtbl_Object();igtbl_WebObject.prototype.constructor=igtbl_WebObject;igtbl_WebObject.base=igtbl_Object.prototype;function igtbl_WebObject(type,element,node)
{if(arguments.length>0)
this.init(type,element,node);}
igtbl_WebObject.prototype.init=function(type,element,node,viewState)
{igtbl_WebObject.base.init.apply(this,[type]);if(element)
{this.Id=element.id;this.Element=element;}
if(node)
this.Node=node;if(viewState)
this.ViewState=viewState;}
igtbl_WebObject.prototype.get=function(name)
{if(this.Node)
return this.Node.getAttribute(name);if(this.Element)
return this.Element.getAttribute(name);return null;}
igtbl_WebObject.prototype.set=function(name,value)
{if(this.Node)
this.Node.setAttribute(name,value);else if(this.Element)
this.Element.setAttribute(name,value);if(this.ViewState)
ig_ClientState.setPropertyValue(this.ViewState,name,value);}
igtbl_Band.prototype=new igtbl_WebObject();igtbl_Band.prototype.constructor=igtbl_Band;igtbl_Band.base=igtbl_WebObject.prototype;function igtbl_Band(grid,node,index,bandsInitArray,colsInitArray)
{if(arguments.length>0)
this.init(grid,node,index,bandsInitArray,colsInitArray);}
var igtbl_ptsBand=["init",function(grid,node,index,bandsInitArray,colsInitArray)
{igtbl_Band.base.init.apply(this,["band",null,node]);this.Grid=grid;this.Index=index;var defaultProps=new Array("Key","AllowAddNew","AllowColSizing","AllowDelete","AllowSort","ItemClass","AltClass","AllowUpdate","CellClickAction","ColHeadersVisible","ColFootersVisible","CollapseImage","CurrentRowImage","CurrentEditRowImage","DefaultRowHeight","EditCellClass","Expandable","ExpandImage","FooterClass","GroupByRowClass","GroupCount","HeaderClass","HeaderClickAction","Visible","IsGrouped","ExpAreaClass","NonSelHeaderClass","RowLabelClass","SelGroupByRowClass","SelHeadClass","SelCellClass","RowSizing","SelectTypeCell","SelectTypeColumn","SelectTypeRow","RowSelectors","NullText","RowTemplate","ExpandEffects","AllowColumnMoving","ClientSortEnabled","Indentation","RowLabelWidth","DataKeyField","HeaderHTML","FooterHTML","FixedHeaderIndicator","AllowRowNumbering","IndentationType","HasHeaderLayout","HasFooterLayout","GroupByColumnsHidden","AddNewRowVisible","AddNewRowView","AddNewRowStyle","_optSelectRow","ShowAllCondition","ShowEmptyCondition","ShowNonEmptyCondition","Filter_AllString","Filter_EmptyString","Filter_NonEmptyString","ServerPassedFilters","ApplyOnAdd","FilterDropDownRowCount","RowFilterMode","FilterDropDownStyle","FilterHighlightRowStyle","CellTitleMode","HeaderTitleMode","FilterUIType","AllowRowFiltering","FilterRowView","FilterEvaluationTrigger","FilterRowStyle","FilterOperandDropDownStyle","FilterOperandItemStyle","FilterOperandItemHoverStyle","FilterOperandButtonStyle","FilterOperandStrings","SortingAlgorithm");this.VisibleColumnsCount=0;this.Columns=new Array();var bandArray;bandArray=bandsInitArray[index];var bandCount=0;if(bandArray)
{bandCount=bandsInitArray.length;for(var i=0;i<bandArray.length;i++)
this[defaultProps[i]]=bandArray[i];if(this.RowTemplate!="")
this.ExpandEffects=new igtbl_expandEffects(this.ExpandEffects);if(this.HeaderHTML!="")
this.HeaderHTML=unescape(this.HeaderHTML);if(this.FooterHTML!="")
this.FooterHTML=unescape(this.FooterHTML);}
else
bandCount=this.Node.parentNode.selectNodes("Band").length;var colsArray=colsInitArray[index];if(!node)
{for(var i=0;i<colsArray.length;i++)
{this.Columns[i]=new igtbl_Column(null,this,i,-1,colsArray[i]);if(!this.Columns[i].Hidden)
this.VisibleColumnsCount++;if(this.Columns[i].getSelClass()!=this.getSelClass())
this._selClassDiffer=true;}}
else
{this.Columns.Node=this.Node.selectSingleNode("Columns");var columNodes=this.Columns.Node.selectNodes("Column");var nodeIndex=0;for(var i=0;i<columNodes.length;i++)
{this.Columns[i]=new igtbl_Column(columNodes[i],this,i,nodeIndex,colsArray[i]);if(!this.Columns[i].Hidden&&this.Columns[i].hasCells())
this.VisibleColumnsCount++;if(!colsArray[i][33])
nodeIndex++;if(this.Columns[i].getSelClass()!=this.getSelClass())
this._selClassDiffer=true;}}
igtbl_dispose(defaultProps);if(node)
{this.ColumnsOrder="";for(var i=0;i<this.Columns.length;i++)
this.ColumnsOrder+=this.Columns[i].Key+(i<this.Columns.length-1?";":"");}
this._filterPanels=new Object();if(this.ServerPassedFilters[0])
{for(var itr=0;itr<this.ServerPassedFilters.length;itr+=2)
{var filterPanel=this._filterPanels[this.ServerPassedFilters[itr]];var filterSettingsOpCode=this.ServerPassedFilters[itr+1][0];var filterSettingsValue=this.ServerPassedFilters[itr+1][1];var colIndex=this.ServerPassedFilters[itr].split("_");colIndex=colIndex[colIndex.length-1];if(!filterPanel)
{var filteredColumn=this.Columns[colIndex];filterPanel=this._filterPanels[this.ServerPassedFilters[itr]]=new igtbl_FilterDropDown(filteredColumn);}
filterPanel.setFilter(filterSettingsOpCode,filterSettingsValue,true);}}
if(this.AllowAddNew==1)
{if(this.Index==0)
this.curTable=grid.Element;if(grid.AddNewBoxVisible)
{var addNew=igtbl_getElementById(grid.Id+"_addBox");if(grid.AddNewBoxView==0)
this.addNewElem=addNew.childNodes[0].rows[0].cells[1].childNodes[0].rows[this.Index].cells[this.Index];else
this.addNewElem=addNew.childNodes[0].rows[0].cells[1].childNodes[0].rows[0].cells[this.Index*2];}}
this.SortedColumns=new Array();var rs=this.getRowSelectors();if(bandCount==1)
{if(rs==2)
this.firstActiveCell=0;else
this.firstActiveCell=1;}
else
{if(rs==2)
this.firstActiveCell=1;else
this.firstActiveCell=2;}
this._sqlWhere="";this.SortImplementation=null;},"_alignColumns",function()
{if(this.HasHeaderLayout)
{var drsEls=igtbl_getDocumentElement(this.Grid.Id+"_drs");if(drsEls)
{var master=drsEls[0].firstChild.firstChild;if(master.tagName!="COLGROUP")
{return;}
for(var d=1;d<drsEls.length;d++)
{var colGroup=drsEls[d].firstChild.firstChild;if(colGroup.tagName!="COLGROUP")
{continue;}
for(var c=0;c<colGroup.childNodes.length;c++)
{colGroup.childNodes[c].width=master.childNodes[c].width;}}}}},"getSelectTypeRow",function()
{var res=this.Grid.SelectTypeRow;if(this.SelectTypeRow!=0)
res=this.SelectTypeRow;return res;},"getSelectTypeCell",function()
{var res=this.Grid.SelectTypeCell;if(this.SelectTypeCell!=0)
res=this.SelectTypeCell;return res;},"getSelectTypeColumn",function()
{var res=this.Grid.SelectTypeColumn;if(this.SelectTypeColumn!=0)
res=this.SelectTypeColumn;return res;},"getColumnFromKey",function(key)
{var column=null;for(var i=0;i<this.Columns.length;i++)
if(this.Columns[i].Key==key)
{column=this.Columns[i];break;}
if(!column)
{for(var i=0;i<this.Columns.length;i++)
{var colKey=this.Columns[i].Key;if(colKey!=null&&key!=null&&colKey.toLowerCase()==key.toLowerCase())
{column=this.Columns[i];break;}}}return column;},"getExpandImage",function()
{var ei=this.Grid.ExpandImage;if(this.ExpandImage!="")
ei=this.ExpandImage;return ei;},"getCollapseImage",function()
{var ci=this.Grid.CollapseImage;if(this.CollapseImage!="")
ci=this.CollapseImage;return ci;},"getRowStyleClassName",function()
{if(this.ItemClass!="")
return this.ItemClass;return this.Grid.ItemClass;},"getRowAltClassName",function()
{if(this.AltClass!="")
return this.AltClass;return this.Grid.AltClass;},"getExpandable",function()
{if(this.Expandable!=0)
return this.Expandable;else return this.Grid.Expandable;},"getCellClickAction",function()
{var res=this.Grid.CellClickAction;if(this.CellClickAction!=0)
res=this.CellClickAction;return res;},"getExpAreaClass",function()
{if(this.ExpAreaClass!="")
return this.ExpAreaClass;return this.Grid.ExpAreaClass;},"getRowLabelClass",function()
{if(this.RowLabelClass!="")
return this.RowLabelClass;return this.Grid.RowLabelClass;},"getItemClass",function()
{if(this.ItemClass!="")
return this.ItemClass;return this.Grid.ItemClass;},"getAltClass",function()
{if(this.AltClass!="")
return this.AltClass;else if(this.Grid.AltClass!="")
return this.Grid.AltClass;else if(this.ItemClass!="")
return this.ItemClass;return this.Grid.ItemClass;},"getSelClass",function()
{if(this.SelCellClass!="")
return this.SelCellClass;return this.Grid.SelCellClass;},"getFooterClass",function()
{if(this.FooterClass!="")
return this.FooterClass;return this.Grid.FooterClass;},"getGroupByRowClass",function()
{if(this.GroupByRowClass!="")
return this.GroupByRowClass;return this.Grid.GroupByRowClass;},"addNew",function()
{if(typeof(igtbl_addNew)=="undefined")
return null;if(this.AddNewRowVisible==1)
{igtbl_activateAddNewRow(this.Grid,this.Index,igtbl_getClickRow(this.Grid,this.Index));return;}
return igtbl_addNew(this.Grid.Id,this.Index);},"getHeadClass",function()
{if(this.HeaderClass!="")
return this.HeaderClass;return this.Grid.HeaderClass;},"getRowSelectors",function()
{var res=this.Grid.RowSelectors;if(this.RowSelectors!=0)
res=this.RowSelectors;return res;},"removeColumn",function(index)
{if(!this.Node)return;var column=this.Columns[index];if(!column)
return;var elem=column._getHeadTags(true);var fElem=column._getFootTags(true);var cols=column._getColTags(true);for(var i=0;elem&&i<elem.length;i++)
{if(elem[i])
{elem[i].parentNode.removeChild(elem[i]);elem[i].id="";}}
for(var i=0;fElem&&i<fElem.length;i++)
{if(fElem[i])
{fElem[i].parentNode.removeChild(fElem[i]);fElem[i].id="";}}
for(var i=0;cols&&i<cols.length;i++)
if(cols[i])
cols[i].parentNode.removeChild(cols[i]);column.colElem=elem;column.colFElem=fElem;if(column.Node)
column.Node.parentNode.removeChild(column.Node);if(this.Columns.splice)
this.Columns.splice(index,1);else
this.Columns=this.Columns.slice(0,index).concat(this.Columns.slice(index+1));column.Id="";column.fId="";this.reIdColumns();return column;},"insertColumn",function(column,index)
{if(!this.Node||!column||!column.Node||index<0||index>this.Columns.length)
return;var column1=this.Columns[index];var hAr;var hAr1;var fAr;var fAr1;var insIndex;if(column1)
{this.Columns.Node.insertBefore(column.Node,this.Columns[index].Node);if(this.Columns.splice)
this.Columns.splice(index,0,column);else
this.Columns=this.Columns.slice(0,index).concat(column,this.Columns.slice(index));insIndex=index;while(column1&&!column1.hasCells())
column1=this.Columns[++index];}
else
{this.Columns.Node.appendChild(column.Node);this.Columns[this.Columns.length]=column;insIndex=this.Columns.length-1;}
if(column1)
{hAr=column.colElem;fAr=column.colFElem;if(column.getFixed()===column1.getFixed())
{hAr1=column1._getHeadTags(true);for(var i=0;hAr&&i<hAr.length;i++)
{var tr=hAr1[i].parentNode;tr.insertBefore(hAr[i],hAr1[i])}
if(fAr)
{fAr1=column1._getFootTags(true);for(var i=0;i<fAr.length;i++)
{var tr=fAr1[i].parentNode;tr.insertBefore(fAr[i],fAr1[i])}}}
else
{column1=this.Columns[index-1];hAr1=this.Columns[index-1]._getHeadTags(true);for(var i=0;hAr&&i<hAr.length;i++)
{var tr=hAr1[i].parentNode;tr.insertBefore(hAr[i],hAr1[i].nextSibling)}
if(fAr)
{fAr1=this.Columns[index-1]._getFootTags(true);for(var i=0;i<fAr.length;i++)
{var tr=fAr1[i].parentNode;tr.insertBefore(fAr[i],fAr1[i].nextSibling)}}}
if(column.getVisible()&&column1.getVisible())
{if(column.getFixed()===column1.getFixed())
column1._insertCols(true,column.Width);else
this.Columns[index-1]._insertCols(false,column.Width);}
else if(column.getVisible())
{column2=column1;if(!column1.hasCells())
{while(column2&&!column2.hasCells())
column2=this.Columns[column2.Index+1];}
if(column2&&column2.getVisible())
column2._insertCols(true,column.Width);else
{column2=column1;while(column2&&!column2.getVisible())
column2=this.Columns[column2.Index-1];if(!column2)return;column2._insertCols(false,column.Width);}}}
else
{column1=this.Columns[insIndex-1];while(column1&&!column1.hasCells())
column1=this.Columns[--insIndex];if(!column1)return;hAr=column.colElem;fAr=column.colFElem;hAr1=column1._getHeadTags(true);for(var i=0;hAr&&i<hAr.length;i++)
{var tr=hAr1[i].parentNode;tr.appendChild(hAr[i])}
if(fAr)
{fAr1=column1._getFootTags(true);for(var i=0;i<fAr.length;i++)
{var tr=fAr1[i].parentNode;tr.appendChild(fAr[i])}}
if(column.getVisible()&&column1.getVisible())
column1._insertCols(false,column.Width);else if(column.getVisible())
{while(column1&&!column1.getVisible())
column1=this.Columns[column1.Index-1];if(!column1)return;column1._insertCols(false,column.Width);}}
this.reIdColumns();igtbl_dispose(hAr);igtbl_dispose(fAr);igtbl_dispose(hAr1);igtbl_dispose(fAr1);return column;},"reIdColumns",function()
{if(!this.Node)return;for(var i=0;i<this.Columns.length;i++)
if(!this.Columns[i]._reIded)
this.Columns[i]._reId(i);for(var i=this.Columns.length-1;i>=0;i--)
delete this.Columns[i]._reIded;},"getSelGroupByRowClass",function()
{if(this.SelGroupByRowClass!="")
return this.SelGroupByRowClass;return this.Grid.SelGroupByRowClass;},"getBorderCollapse",function()
{if(this.get("BorderCollapse")=="Separate")
return"";if(this.Grid.get("BorderCollapseDefault")=="Separate")
return"";if(this.curTable)
return this.curTable.style.borderCollapse;return this.Grid.Element.style.borderCollapse;},"getIndentation",function()
{var result=this.Indentation;if(result==22)
result=this.Grid.Indentation;return result;},"getSortingAlgorithm",function()
{if(this.SortingAlgorithm==0)
return this.Grid.SortingAlgorithm;return this.SortingAlgorithm;},"getSortImplementation",function()
{if(this.SortImplementation==null)
return this.Grid.SortImplementation;return this.SortImplementation;}];for(var i=0;i<igtbl_ptsBand.length;i+=2)
igtbl_Band.prototype[igtbl_ptsBand[i]]=igtbl_ptsBand[i+1];function igtbl_expandEffects(values)
{this.Delay=values[0];this.Duration=values[1];this.Opacity=values[2];this.ShadowColor=values[3];this.ShadowWidth=values[4];this.EffectType=values[5];}
igtbl_Cell.prototype=new igtbl_WebObject();igtbl_Cell.prototype.constructor=igtbl_Cell;igtbl_Cell.base=igtbl_WebObject.prototype;function igtbl_Cell(element,node,row,index)
{if(arguments.length>0)
this.init(element,node,row,index);}
var igtbl_ptsCell=["init",function(element,node,row,index)
{igtbl_Cell.base.init.apply(this,["cell",element,node]);var gs=row.OwnerCollection.Band.Grid;this.Row=row;this.Band=row.Band;if(typeof(index)!="number")
try{index=parseInt(index.toString(),10);}catch(e){}
this.Column=this.Band.Columns[index];this.Index=index;var cell=this.Element;if(cell)
{cell.Object=this;this.NextSibling=cell.nextSibling;if(cell.cellIndex==this.Band.firstaActiveCell)
this.PrevSibling=null;else
this.PrevSibling=cell.previousSibling;if(this.Column.MaskDisplay)
this.MaskedValue=igtbl_getInnerText(cell);}
if(this.Column.ColumnType==3&&element)
{var childNodes=null;if(element.childNodes.length>0)
childNodes=element.childNodes[0].childNodes;for(var chkBoxCount=0;childNodes&&chkBoxCount<childNodes.length;chkBoxCount++)
{var chkBox=childNodes[chkBoxCount];if(chkBox.tagName=="INPUT"&&chkBox.type=="checkbox")
{chkBox.unselectable="on";break;}}}
this._Changes=new Object();},"getElement",function()
{if(this._scrElem)
return this._scrElem;return this.Element;},"getNodeValue",function()
{if(!this.Node)
return;var v=igtbl_getNodeValue(this.Node);if(typeof(this.Value)=="undefined"&&this.Column.ColumnType==9)
{var subWithTrailingA=v.substring(v.indexOf('>')+1);v=subWithTrailingA.substring(0,subWithTrailingA.indexOf('<'));subWithTrailingA=null;}
else if(this.Column.ColumnType==9&&typeof(this.Value)!="undefined")
{return this.Value;}
return v;},"setNodeValue",function(value,displayValue)
{if(!this.Node)
return;igtbl_setNodeValue(this.Node,value,displayValue);},"getValue",function(textValue,force)
{if(typeof(this.Value)!="undefined"&&!textValue&&!force)
return this.Value;var value;if(this.Node)
value=this.getNodeValue();if(this.Element)
{if(!this.Node)
value=this.Element.getAttribute(igtbl_sigCellText);if(typeof(value)!="string"||textValue||(this.Column.ColumnType==9&&this.Node))
{value=this.Element.getAttribute(igtbl_sUnmaskedValue);if(value)value=unescape(value);if(typeof(value)=="undefined"||value==null)
{var elem=this.Element;if(elem.childNodes.length>0&&elem.childNodes[0].tagName=="NOBR")
elem=elem.childNodes[0];if(this.Column.ColumnType==9&&elem.childNodes.length>0&&elem.childNodes[0].tagName=="A")
elem=elem.childNodes[0];if(this.Row.IsFilterRow)
{var tempE=null;var chldNodes=elem.childNodes;for(var itr=0;itr<chldNodes.length;itr++)
{if(chldNodes[itr].tagName=="SPAN")
{tempE=chldNodes[itr];break;}}
elem=tempE;}
value=igtbl_getInnerText(elem);if(value==" ")value="";}
else if(textValue)
{if(this.MaskedValue)
value=this.MaskedValue;else
value=value.toString();}
var oCombo=null;this.Column.ensureWebCombo();if(this.Column.WebComboId)
oCombo=igcmbo_getComboById(this.Column.WebComboId);if(oCombo)
{if(!textValue&&!this.Element.getAttribute(igtbl_sUnmaskedValue))
{var oCombo=igcmbo_getComboById(this.Column.WebComboId);if(oCombo&&oCombo.DataTextField)
{var re=new RegExp("^"+igtbl_getRegExpSafe(value)+"$","gi");var column=oCombo.grid.Bands[0].getColumnFromKey(oCombo.DataTextField);if(column)
{var cell=column.find(re);if(cell&&oCombo.DataValueField)
value=cell.Row.getCellByColumn(oCombo.grid.Bands[0].getColumnFromKey(oCombo.DataValueField)).getValue(true);}
delete re;}}}
else if(this.Column.ColumnType==3&&this.Element.childNodes.length>0)
{if(!this.Element.getAttribute(igtbl_sUnmaskedValue))
{var chBox=this.Element.childNodes[0];while(chBox&&chBox.tagName!="INPUT")
chBox=chBox.childNodes[0];value=false;if(chBox)
value=chBox.checked;if(textValue)
value=value.toString();}}
else if(this.Column.ColumnType==5&&this.Column.ValueList.length>0)
{if(!textValue)
{var tempValue=this.Element.getAttribute(igtbl_sigDataValue);if(tempValue!=null)
{value=tempValue;}
else
for(var i=0;i<this.Column.ValueList.length;i++)
if(this.Column.ValueList[i]&&this.Column.ValueList[i][1]==value)
{value=this.Column.ValueList[i][0];break;}}}
else if(this.Column.ColumnType==7&&this.Element.childNodes.length>0)
{var button=this.Element.childNodes[0];while(button&&button.tagName!="INPUT")
button=button.childNodes[0];if(button)
value=button.value;}
if(typeof(value)=="string"&&this.Column.AllowNull&&value==this.Column.getNullText())
{if(textValue)
value=this.Column.getNullText();else
value=null;}}}
else
{if(this.Column.MergeCells)
{var upRow=this.Row.getPrevRow();if(upRow)
value=upRow.getCellByColumn(this.Column).getValue();}}
if(typeof(value)!="undefined")
{if(!textValue)
{value=this.Column.getValueFromString(value);}}
else if(textValue)
value="";if(!textValue)
this.Value=value;return value;},"setValue",function(value,fireEvents)
{if(typeof(fireEvents)=="undefined")
fireEvents=true;var gn=this.Row.gridId;var gs=igtbl_getGridById(gn);if(this.Column.DataType!=8&&typeof(value)=="string")
value=igtbl_string.trim(value);if(!gs.insideBeforeUpdate)
{gs.insideBeforeUpdate=true;var ev=value;if((ev==null||ev==this.Column.getNullText()||typeof(ev)=='undefined')&&typeof(ev)!='number'&&typeof(ev)!='boolean')
{ev=this.Column.getNullText();value=null;}
else
{ev=ev.toString().replace(/\r\n/g,"\\r\\n");ev=ev.toString().replace(/\\/g,"\\\\");ev=ev.replace(/\"/g,"\\\"");}
var res=fireEvents&&this.Element&&igtbl_fireEvent(gn,gs.Events.BeforeCellUpdate,"(\""+gn+"\",\""+this.Element.id+"\",\""+ev+"\")");gs.insideBeforeUpdate=false;return;}
var v=value;var oldValue=this.getValue();if(typeof(value)!="undefined"&&value!=null)
{if((oldValue&&oldValue.getMonth||this.Column.DataType==7)&&typeof(value)=="string")
{if(this.Column.MaskDisplay.substr(0,1).toLowerCase()=="h")
value="01/01/01 "+value;else
{var year="";for(var i=value.length-1;i>=0;i--)
{var y=parseInt(value.charAt(i),10);if(isNaN(y))
break;else
year=y.toString()+year;}
if(year&&year.length<3)
value=value.substr(0,i+1)+(parseInt(year,10)+gs.DefaultCentury).toString();}
value=new Date(value);}
if(value.getMonth)
{if(isNaN(value))value=oldValue;v=value;if(value)
value=igtbl_dateToString(value);}}
var displayValue=null;if(this.Element)
{if(this.Element.getAttribute(igtbl_sigCellText)!=null)
{this.Element.setAttribute(igtbl_sigCellText,typeof(value)=="undefined"||value==null?"":value.toString());if(this.Node)
this.Node.setAttribute(igtbl_sigCellText,value==null?"":value);}
else
{var rendVal=null;var colEditor=this.Column.getEditorControl();if(colEditor&&colEditor.getRenderedValue&&(rendVal=colEditor.getRenderedValue(v))!=null)
{v=rendVal;if(value!=null)
{if(igtbl_number.isNumberType(this.Column.DataType))
{colEditor.setValue(v);value=colEditor.getValue();}
if(value!=null)
{if(typeof(value.getMonth)!="undefined")
value=igtbl_dateToString(value);this.Element.setAttribute(igtbl_sUnmaskedValue,value.toString());}
else
this.Element.removeAttribute(igtbl_sUnmaskedValue);}
else
this.Element.removeAttribute(igtbl_sUnmaskedValue);this.MaskedValue=v;}
else
{if(this.Column.AllowNull&&(typeof(v)=="undefined"||v==null||typeof(v)=="string"&&(v==""||v==this.Column.getNullText())))
{v=this.Column.getNullText();value="";}
else
v=typeof(value)!="undefined"&&value!=null?value.toString():"";if(this.Column.MaskDisplay!="")
{if(this.Column.AllowNull&&v==this.Column.getNullText())
{this.Element.setAttribute(igtbl_sUnmaskedValue,null);this.MaskedValue=(v==""?" ":v);}
else
{v=igtbl_Mask(gn,v,this.Column.DataType,this.Column.MaskDisplay);if(v=="")
{var umv=this.Element.getAttribute(igtbl_sUnmaskedValue);if(ig_csom.notEmpty(umv))
v=igtbl_Mask(gn,umv,this.Column.DataType,this.Column.MaskDisplay);else
{v=this.Column.getNullText();value="";}}
else
{if(this.Column.MaskDisplay=="MM/dd/yyyy"||this.Column.MaskDisplay=="MM/dd/yy"||this.Column.MaskDisplay=="hh:mm"||this.Column.MaskDisplay=="HH:mm"||this.Column.MaskDisplay=="hh:mm tt")
value=v;this.Element.setAttribute(igtbl_sUnmaskedValue,value);this.MaskedValue=v;}}}
else if(ig_csom.notEmpty(this.Element.getAttribute(igtbl_sUnmaskedValue)))
this.Element.setAttribute(igtbl_sUnmaskedValue,igtbl_string.toString(value));if(!(this.Column.AllowNull&&v==this.Column.getNullText()))
{if(this.Column.MaskDisplay=="")
{if(typeof(value)!="undefined"&&value!=null&&this.Column.DataType!=7)
{v=this.Column.getValueFromString(value);if(v!=null)
{v=v.toString();value=v;}}
if(this.Column.FieldLength>0)
{v=v.substr(0,this.Column.FieldLength);value=v;}
if(this.Column.Case==1)
v=v.toLowerCase();else if(this.Column.Case==2)
v=v.toUpperCase();}}}
var setInner=true;this.Column.ensureWebCombo();if(this.Column.WebComboId&&typeof(igcmbo_getComboById)!="undefined")
{var oCombo=igcmbo_getComboById(this.Column.WebComboId);if(oCombo&&oCombo.DataValueField)
{var re=new RegExp("^"+igtbl_getRegExpSafe(value)+"$","gi");var column=oCombo.grid.Bands[0].getColumnFromKey(oCombo.DataValueField);if(column)
{var cell=column.find(re);if(cell&&oCombo.Prompt&&cell.Row.getIndex()==0)
cell=column.findNext();}
if(cell&&oCombo.DataTextField)
v=cell.Row.getCellByColumn(oCombo.grid.Bands[0].getColumnFromKey(oCombo.DataTextField)).getValue(true);this.Element.setAttribute(igtbl_sigDataValue,value);this.Element.setAttribute(igtbl_sUnmaskedValue,value.toString());delete re;}}
else if(this.Column.ColumnType==3&&this.Element.childNodes.length>0)
{igtbl_dontHandleChkBoxChange=true;var chBox=this.Element.childNodes[0];while(chBox&&chBox.tagName!="INPUT")
chBox=chBox.childNodes[0];if(chBox)
{if(!value||value.toString().toLowerCase()=="false"||v=="0")
chBox.checked=false;else
chBox.checked=true;this.Element.setAttribute("chkBoxState",v);this.Element.setAttribute(igtbl_sUnmaskedValue,v);}
igtbl_dontHandleChkBoxChange=false;setInner=false;}
else if(this.Column.ColumnType==5&&this.Column.ValueList.length>0)
{var v2=value;if(this.Column.DataType==11)
{v2=this._getBoolFromStringIfPossible(value);}
for(var i=0;i<this.Column.ValueList.length;i++)
{var valueListValue;if(this.Column.ValueList[i])
valueListValue=this.Column.ValueList[i][0];if(this.Column.DataType==11)
{valueListValue=this._getBoolFromStringIfPossible(valueListValue);}
if(valueListValue===v2)
{v=this.Column.ValueList[i][1];displayValue=v;this.Element.setAttribute(igtbl_sigDataValue,value);if(this.Node)
this.Node.setAttribute(igtbl_sigDataValue,value);break;}}
if(i==this.Column.ValueList.length)
this.Element.removeAttribute(igtbl_sigDataValue);}
else if(this.Column.ColumnType==7&&this.Element.childNodes.length>0)
{var button=this.Element.childNodes[0];while(button&&button.tagName!="INPUT")
button=button.childNodes[0];if(button)
{button.value=v;setInner=false;}
else
{button=igtbl_getElementById(gn+"_bt");if(button)
button.value=v;}}
if(setInner)
{var vs=(this.Column.ColumnType==8||this.Column.ColumnType==10)?v:igtbl_string.trim(v);var e=this.Element;if(vs=="")
{vs=" ";e.setAttribute(igtbl_sUnmaskedValue,v);}
else if(vs!=v)
e.setAttribute(igtbl_sUnmaskedValue,v);else if(e.getAttribute(igtbl_sUnmaskedValue,"")=="")
e.removeAttribute(igtbl_sUnmaskedValue);e=this.getElement();el=e;if(el.firstChild&&el.firstChild.tagName=="NOBR")
el=el.firstChild;if((this.Column.ColumnType==9||this.getTargetURL())&&el.firstChild&&el.firstChild.tagName=="A")
el=el.firstChild;if(this.Row.IsFilterRow)
{var tempE=null;var chldNodes=el.childNodes;for(var itr=0;itr<chldNodes.length;itr++)
{if(chldNodes[itr].tagName=="SPAN")
{tempE=chldNodes[itr];break;}}
el=tempE;}
if((this.Column.ColumnType==9||this.getTargetURL())&&el.tagName=="A")
{if((value!=" "&&vs==" ")||vs=="")
{igtbl_setInnerText(el,"");if(el.parentNode.innerHTML.indexOf(" ")>0&&el.parentNode.innerHTML.lastIndexOf(" ")<(el.parentNode.innerHTML.length-1)&&el.parentNode.innerHTML.indexOf("&nbsp;")>0&&el.parentNode.innerHTML.lastIndexOf("&nbsp;")<(el.parentNode.innerHTML.length-1-5))
el.parentNode.innerHTML+="&nbsp;";}
else
{igtbl_setInnerText(el,vs);}}
else
igtbl_setInnerText(el,vs,this.Column.Wrap);if(e.getAttribute(igtbl_sigDataValue)&&this.Column.ColumnType!=5&&(!this.Column.WebComboId||typeof(igcmbo_getComboById)=="undefined"))
e.setAttribute(igtbl_sigDataValue,vs);if(el.tagName=="A"&&this.Column.ColumnType==9&&!this.getTargetURL())
el.href=(v.indexOf('@')>=0?"mailto:":"")+v;if(this.Node)
{this.Node.firstChild.text=(e.getAttribute(igtbl_sUnmaskedValue,"")=="")?"&nbsp;":vs;}}}}
if(this.Node)
{var nodeValue=(value==null||value==='')?this.Column.getNullText():value;if(this.Column.ColumnType==7)
this.setNodeValue("<input type=\"button\" style=\"width:100%;height:100%;\" value=\""+nodeValue+"\" onclick=\"igtbl_colButtonClick(event,'"+this.Band.Grid.Id+"',null, igtbl_srcElement(event));\" class=\"\" tabindex=\"-1\" />");else
this.setNodeValue(nodeValue,displayValue);}
var newValue=this.getValue(false,true);if(!((typeof(newValue)=="undefined"||newValue==null)&&(typeof(oldValue)=="undefined"||oldValue==null)||newValue!=null&&oldValue!=null&&newValue.valueOf()==oldValue.valueOf()))
{this.Row._dataChanged|=2;if(typeof(this._oldValue)=="undefined")
{if(oldValue&&oldValue.getMonth)
oldValue=igtbl_dateToString(oldValue);this._oldValue=oldValue;}
if(!this.Row.IsAddNewRow)
igtbl_saveChangedCell(gs,this,value);if(this.Row.IsFilterRow)
{var columnFilter=this.Column._getFilterPanel(this.Row.Element);var filterOp=parseInt(this._getFilterTypeImage().getAttribute("operator"));if(this.Column.DataType==7&&value!=null&&value!="")
{var re=new RegExp("^\\s+");value=value.replace(re,"");}
if(value==null||value=="")filterOp=igtbl_filterComparisionOperator.All;columnFilter.setFilter(filterOp,value);if(this.Row.Band.FilterEvaluationTrigger==1)
{if(this.Row.Band.Index>0)
{columnFilter.RowIsland=this.Row.OwnerCollection;}
if(!gs.fireEvent(gs.Events.BeforeRowFilterApplied,[gs.Id,this.Column]))
{columnFilter.applyFilter();gs.fireEvent(gs.Events.AfterRowFilterApplied,[gs.Id,this.Column]);}}}
else
{gs.invokeXmlHttpRequest(gs.eReqType.UpdateCell,this,value?value.toString():value);if(fireEvents&&this.Element)
{igtbl_fireEvent(gn,gs.Events.AfterCellUpdate,"(\""+gn+"\",\""+this.Element.id+"\")");if(gs.LoadOnDemand==3||this.Row.IsAddNewRow)
gs.NeedPostBack=false;}}}},"_getBoolFromStringIfPossible",function(str)
{if(str==null||str.length==0||typeof(str)!="string")return str;if(str.toLowerCase()=="true")return true;if(str.toLowerCase()=="false")return false;return str;},"getRow",function()
{return this.Row;},"getNextTabCell",function(shift,addRow,filterRow)
{var g=this.Row.Band.Grid;var cell=null;switch(g.TabDirection)
{case 0:case 1:if(shift&&g.TabDirection==0||!shift&&g.TabDirection==1)
{cell=this;do
{cell=cell.getPrevCell();if(!cell)break;}while(!cell.Element);if(!cell)
{var row=this.Row.getNextTabRow(true,false,addRow,filterRow);if(row&&!row.GroupByRow)
{cell=row.getCell(row.cells.length-1);do
{if(!cell.Column.getVisible()||!cell.Element)
cell=cell.getPrevCell();if(!cell)break;}while(!cell.Element);}}}
else
{cell=this;do
{cell=cell.getNextCell();if(!cell)break;}while(!cell.Element);if(!cell)
{var row=this.Row.getNextTabRow(false,false,addRow);if(row&&!row.GroupByRow)
{cell=row.getCell(0);do
{if(!cell.Column.getVisible()||!cell.Element)
cell=cell.getNextCell();if(!cell)break;}while(!cell.Element);}}}
break;case 2:case 3:if(shift&&g.TabDirection==2||!shift&&g.TabDirection==3)
{var row=this.Row.getPrevRow();if(row&&row.getExpanded())
{row=this.Row.getNextTabRow(true,false,addRow);cell=row.getCell(row.cells.length-1);do
{if(!cell.Column.getVisible()||!cell.Element)
cell=cell.getPrevCell();if(!cell)break;}while(!cell.Element);}
else if(row)
cell=row.getCell(this.Index);else
{if(this.Index==0)
{row=this.Row.getNextTabRow(true,false,addRow);if(row&&!row.GroupByRow)
{cell=row.getCell(row.cells.length-1);do
{if(!cell.Column.getVisible()||!cell.Element)
cell=cell.getPrevCell();if(!cell)break;}while(!cell.Element);}}
else
{cell=this.Row.OwnerCollection.getRow(this.Row.OwnerCollection.length-1).getCell(this.Index-1);do
{if(!cell.Column.getVisible()||!cell.Element)
cell=cell.getPrevCell();if(!cell)break;}while(!cell.Element);}}}
else
{if(this.Row.getExpanded())
{cell=this.Row.Rows.getRow(0).getCell(0);do
{if(!cell.Column.getVisible()||!cell.Element)
cell=cell.getNextCell();if(!cell)break;}while(!cell.Element);}
else
{var row=this.Row.getNextRow();if(row)
cell=row.getCell(this.Index);else if(this.Index<this.Row.cells.length-1)
{cell=this.Row.OwnerCollection.getRow(0).getCell(this.Index+1);do
{if(!cell.Column.getVisible()||!cell.Element)
cell=cell.getNextCell();if(!cell)break;}while(!cell.Element);}
else
{row=this.Row.getNextTabRow(false,false,addRow,filterRow);if(row&&!row.GroupByRow)
{cell=row.getCell(0);do
{if(!cell.Column.getVisible()||!cell.Element)
cell=cell.getNextCell();if(!cell)break;}while(!cell.Element);}}}}
break;}
return cell;},"beginEdit",function(keyCode)
{if(this.isEditable())
{igtbl_editCell((typeof(event)!="undefined"?event:null),this.Row.gridId,this.Element,keyCode);var ec=this.Band.Grid._editorCurrent;if(ec)
{ec.setAttribute("noOnBlur",true);if(igtbl_isVisible(ec))
window.setTimeout("igtbl_cancelNoOnBlurTB('"+this.Band.Grid.Id+"','"+ec.id+"')",100);else
ec.removeAttribute("noOnBlur");}}},"endEdit",function(force)
{var ec=this.Column.getEditorControl();if(!ec)
ec=this.Column.Band.Grid._editorCurrent;if(ec&&ec.Element)
ec=ec.Element;if(force)
if(ec&&ec.removeAttribute)
ec.removeAttribute("noOnBlur");if(ec&&ec.getAttribute&&ec.getAttribute("noOnBlur"))
return;igtbl_hideEdit(this.Row.gridId);},"getSelected",function()
{if(this._Changes["SelectedCells"])
return true;return false;},"setSelected",function(select)
{var stc=this.Band.getSelectTypeCell();if(stc>1)
{if(stc==2)
this.Band.Grid.clearSelectionAll();igtbl_selectCell(this.Row.gridId,this,select);}},"getNextCell",function(includeHiddenColumns)
{var nc=this.Index+1;while(includeHiddenColumns!=true&&nc<this.Row.cells.length&&!this.Row.getCell(nc).Column.getVisible())
{nc++;}
if(nc<this.Row.cells.length)
return this.Row.getCell(nc);return null;},"getPrevCell",function(includeHiddenColumns)
{var pc=this.Index-1;while(includeHiddenColumns!=true&&pc>=0&&!this.Row.getCell(pc).Column.getVisible())
{pc--;}
if(pc>=0)
return this.Row.getCell(pc);return null;},"activate",function()
{this.Row.Band.Grid.setActiveCell(this);},"scrollToView",function()
{var g=this.Row.Band.Grid;var adj=0;if((this.Row.IsFilterRow||this.Row.IsAddNewRow)&&this.Row.isFixed())
{var rowTblElem=this.Row.Element.parentNode;while(rowTblElem&&rowTblElem.tagName!="TABLE")
rowTblElem=rowTblElem.parentNode;adj=-(rowTblElem.offsetLeft);}
if(g.UseFixedHeaders)
{var c=this.Column;var w=0,i=0,c1=null;while(i<c.Index)
{c1=c.Band.Columns[i++];if(c1.getVisible())
{if(!c1.getFixed())
w+=c1.getWidth();}}
if(!c.getFixed()&&w+c.getWidth()<g._scrElem.scrollLeft)
{igtbl_scrollLeft(g._scrElem,w)}
igtbl_scrollToView(g.Id,this.Element,c.getWidth(),w,(this.Row.IsFilterRow||this.Row.IsAddNewRow)&&this.Row.isFixed()?1:0,adj);return;}
igtbl_scrollToView(g.Id,this.Element,null,null,(this.Row.IsFilterRow||this.Row.IsAddNewRow)&&this.Row.isFixed()?1:0,adj);},"isEditable",function()
{var attr="";if(this.Node)
attr=this.Node.getAttribute(igtbl_litPrefix+"allowedit");else if(this.Element)
attr=this.Element.getAttribute("allowedit");if(attr=="yes")
return true;if(attr=="no")
return false;if(this.Row.IsFilterRow)
{return this.Column.AllowRowFiltering!=1;}
return igtbl_getAllowUpdate(this.Row.gridId,this.Column.Band.Index,this.Column.Index)==1;},"getEditable",function()
{if(this.Node)
return this.Node.getAttribute(igtbl_litPrefix+"allowedit");else if(this.Element)
return this.Element.getAttribute("allowedit");},"setEditable",function(bEdit)
{if(bEdit==null||typeof(bEdit)=="undefined")
bEdit=false;var attr=bEdit?"yes":"no";if(this.Node)
this.Node.setAttribute(igtbl_litPrefix+"allowedit",attr)
if(this.Element)
this.Element.setAttribute("allowedit",attr);if(this.Column.ColumnType==3)
{var checkboxes=this.Element.getElementsByTagName("input");if(checkboxes.length==1)
{checkboxes[0].disabled=!bEdit;}}},"hasButtonEditor",function(cellButtonDisplay)
{return this.Column.ColumnType==7&&!this.Row.GroupByRow&&!this.Row.IsFilterRow&&(typeof(cellButtonDisplay)=="undefined"||this.Column.CellButtonDisplay==cellButtonDisplay);},"renderActive",function(render)
{var g=this.Row.Band.Grid;if(!g.Activation.AllowActivation||!this.Element)
return;var e=this.getElement();if(typeof(render)=="undefined")render=true;var ao=g.Activation;if(render)
{igtbl_setClassName(e,ao._cssClass);igtbl_setClassName(e,ao._cssClassL);igtbl_setClassName(e,ao._cssClassR);}
else
{igtbl_removeClassName(e,ao._cssClassR);igtbl_removeClassName(e,ao._cssClassL);igtbl_removeClassName(e,ao._cssClass);}},"getLevel",function(s)
{var l=this.Row.getLevel();l[l.length]=this.Column.Index;if(s)
{s=l.join("_");igtbl_dispose(l);delete l;return s;}
return l;},"selectCell",function(selFlag)
{var e=this.getElement();if(!e)
return;var className=null;if(selFlag!=false)
className=this.Column.getSelClass();igtbl_changeStyle(this.Row.gridId,e,className);},"select",function(selFlag,fireEvent)
{var gs=this.Column.Band.Grid;var gn=gs.Id;var cellID=this.Element.id;if(gs._exitEditCancel||gs._noCellChange)
return;if(this.Band.getSelectTypeCell()<2)
return;if(igtbl_fireEvent(gn,gs.Events.BeforeSelectChange,"(\""+gn+"\",\""+cellID+"\")")==true)
return;if(selFlag!=false)
{this.selectCell();gs._recordChange("SelectedCells",this,gs.GridIsLoaded);if(!gs.SelectedCellsRows[this.Element.parentNode.id])
gs.SelectedCellsRows[this.Element.parentNode.id]=new Object();gs.SelectedCellsRows[this.Element.parentNode.id][cellID]=true;}
else
{if(gs.SelectedCells[cellID]||gs._containsChange("SelectedCells",this))
{gs._removeChange("SelectedCells",this);var scr=gs.SelectedCellsRows[this.Element.parentNode.id];if(scr&&scr[cellID])
delete scr[cellID];}
if(igtbl_getLength(gs.SelectedCellsRows[this.Element.parentNode.id])==0)
delete gs.SelectedCellsRows[this.Element.parentNode.id];if(!this.Column.Selected&&!this.Row.getSelected())
this.selectCell(false);}
if(fireEvent!=false)
{var gsNPB=gs.NeedPostBack;igtbl_fireEvent(gn,gs.Events.AfterSelectChange,"(\""+gn+"\",\""+cellID+"\");");if(!gsNPB&&!(gs.Events.AfterSelectChange[1]&1))
gs.NeedPostBack=false;if(gs.NeedPostBack)
igtbl_moveBackPostField(gn,"SelectedCells");}},"getOldValue",function()
{return this._oldValue;},"getTargetURL",function()
{var url=null;if(this.Node&&(url=this.Node.getAttribute("targetURL")))
return url;if(this.Element&&(url=this.Element.getAttribute("targetURL")))
return url;if(this.Column.ColumnType==9)
return this.getValue();return url;},"getTitleModeResolved",function()
{var result;if(this.Element)
{result=this.Element.getAttribute("iTM");}
if(!result&&this.Band)
{result=this.Band.CellTitleMode;}
if(!result&&this.Band&&this.Band.Grid)
{result=this.Band.Grid.CellTitleMode;}
return result;},"setTargetURL",function(url)
{if(this.Node&&this.Node.getAttribute("targetURL"))
this.Node.setAttribute("targetURL",url);if(this.Element&&this.Element.getAttribute("targetURL"))
this.Element.setAttribute("targetURL",url);var urls=igtbl_splitUrl(url);var el=this.Element;if(el)
{if(el.firstChild&&el.firstChild.tagName=="NOBR")
el=el.firstChild;if(el.firstChild&&el.firstChild.tagName=="A")
el=el.firstChild;}
if(this.Column.ColumnType==9)
this.setValue(urls[0]);if(el&&el.tagName=="A")
{if(this.Column.ColumnType!=9)
el.href=urls[0];if(urls[1])
el.target=urls[1];else
el.target="_self";}
igtbl_dispose(urls);},"_getFilterTypeImage",function()
{if(!this.Row.IsFilterRow)return null;return this._getFilterTypeImageRecus(this.Element.childNodes);},"_getFilterTypeImageRecus",function(elements)
{if(elements!=null)
{for(var itr=0;itr<elements.length;itr++)
{if(elements[itr].tagName=="IMG")
return elements[itr];else
return this._getFilterTypeImageRecus(elements[itr].childNodes);}}
return null;},"_setFilterTypeImage",function(filterType)
{var g=this.Row.Band.Grid;for(var itr=0;itr<g.FilterButtonImages.length;itr++)
{var fbi=g.FilterButtonImages[itr];if(filterType==fbi[0])
{var img=this._getFilterTypeImage();if(img)
{img.src=fbi[1];img.alt=img.title=fbi[2];img.setAttribute("operator",filterType);if(ig_csom.IsFireFox&&img.parentNode&&img.parentNode.tagName=="BUTTON")
img.parentNode.title=fbi[2];}}}}];for(var i=0;i<igtbl_ptsCell.length;i+=2)
igtbl_Cell.prototype[igtbl_ptsCell[i]]=igtbl_ptsCell[i+1];igtbl_Column.prototype=new igtbl_WebObject();igtbl_Column.prototype.constructor=igtbl_Column;igtbl_Column.base=igtbl_WebObject.prototype;function igtbl_Column(node,band,index,nodeIndex,colInitArray)
{if(arguments.length>0)
this.init(node,band,index,nodeIndex,colInitArray);}
var igtbl_ptsColumn=["init",function(node,band,index,nodeIndex,colInitArray)
{igtbl_Column.base.init.apply(this,["column",null,node]);this.Band=band;this.Index=index;this.Id=(band.Grid.Id
+"_"
+"c_"+band.Index.toString()+"_"+index.toString());if(band.ColFootersVisible==1)
this.fId=(band.Grid.Id
+"_"
+"f_"+band.Index.toString()+"_"+index.toString());var defaultProps=new Array("Key","HeaderText","DataType","CellMultiline","Hidden","AllowGroupBy","AllowColResizing","AllowUpdate","Case","FieldLength","CellButtonDisplay","HeaderClickAction","IsGroupBy","MaskDisplay","Selected","SortIndicator","NullText","ButtonClass","SelCellClass","SelHeadClass","ColumnType","ValueListPrompt","ValueList","ValueListClass","EditorControlID","DefaultValue","TemplatedColumn","Validators","CssClass","Style","Width","AllowNull","Wrap","ServerOnly","HeaderClass","ButtonStyle","Fixed","FooterClass","FixedHeaderIndicator","FooterText","HeaderStyle","FooterStyle","HeaderWrap","HeaderImageUrl","HeaderImageAltText","HeaderImageHeight","HeaderImageWidth","MergeCells","DefaultFilterList","RowFilterMode","AllowRowFiltering","GatherFilterData","FilterIcon","HeaderTitleMode","FilterOperatorDefaultValue","FilterComparisonType","SortingAlgorithm","SortCaseSensitive","WidthResolved");var columnArray;columnArray=colInitArray;if(columnArray)
{for(var i=0;i<columnArray.length;i++)
this[defaultProps[i]]=columnArray[i];if(this.Key&&this.Key.length>0)
this.Key=unescape(this.Key);if(this.HeaderText&&this.HeaderText.length>0)
this.HeaderText=unescape(this.HeaderText);if(this.HeaderImageUrl&&this.HeaderImageUrl.length>0)
this.HeaderImageUrl=unescape(this.HeaderImageUrl);if(this.HeaderImageAltText&&this.HeaderImageAltText.length>0)
this.HeaderImageAltText=unescape(this.HeaderImageAltText);this._AltCssClass=this.Band.getRowAltClassName()+(this.CssClass?" ":"")+this.CssClass;this.CssClass=this.Band.getRowStyleClassName()+(this.CssClass?" ":"")+this.CssClass;}
this.ensureWebCombo();if(node)
{this.Node.setAttribute("index",index+1);this.Node.setAttribute("cellIndex",nodeIndex+1);}
igtbl_dispose(defaultProps);if(this.EditorControlID)
this.editorControl=igtbl_getElementById(this.EditorControlID);if(this.Validators&&this.Validators.length>0&&typeof(Page_Validators)!="undefined")
{for(var i=0;i<this.Validators.length;i++)
{var val=igtbl_getElementById(this.Validators[i]);if(val)
val.enabled=false;}}
this._Changes=new Object();this.SortImplementation=null;},"getEditorControl",function()
{if(!this.editorControl)
return null;if(this.editorControl.Object)
this.editorControl=this.editorControl.Object;return this.editorControl;},"getAllowUpdate",function()
{var g=this.Band.Grid;var res=g.AllowUpdate;if(this.Band.AllowUpdate!=0)
res=this.Band.AllowUpdate;if(this.AllowUpdate!=0)
res=this.AllowUpdate;if(this.TemplatedColumn&2)
res=2;return res;},"setAllowUpdate",function(value)
{this.AllowUpdate=value;switch(this.DataType)
{case 11:igtbl_AdjustCheckboxDisabledState(this,this.Band.Index,this.Band.Grid.Rows,this.getAllowUpdate());break;}},"getHidden",function()
{return this.Hidden;},"setHidden",function(h)
{if(this.Node)
{if(h===false)
this.Node.removeAttribute("hidden");else
this.Node.setAttribute("hidden",true);}
igtbl_hideColumn(this.Band.Grid.Rows,this,h);this.Hidden=h;if(this.Band.Index==0)
this.Band.Grid.alignStatMargins();var ac=this.Band.Grid.getActiveCell();if(ac&&ac.Column==this&&h)
this.Band.Grid.setActiveCell(null);else
this.Band.Grid.alignGrid();},"getVisible",function()
{return!this.getHidden()&&this.hasCells();},"hasCells",function()
{return!this.ServerOnly&&(!this.IsGroupBy||this.Band.GroupByColumnsHidden==2);},"getNullText",function()
{return igtbl_getNullText(this.Band.Grid.Id,this.Band.Index,this.Index);},"find",function(re,back)
{var g=this.Band.Grid;if(re)
g.regExp=re;if(!g.regExp||!this.hasCells())
return null;g.lastSearchedCell=null;if(back==true||back==false)
g.backwardSearch=back;var row=null;if(!g.backwardSearch)
{row=g.Rows.getRow(0);if(row&&row.getHidden())
row=row.getNextRow();while(row&&(row.Band!=this.Band||row.getCellByColumn(this).getValue(true).search(g.regExp)==-1))
row=row.getNextTabRow(false,true);}
else
{var rows=g.Rows;while(rows)
{row=rows.getRow(rows.length-1);if(row&&row.getHidden())
row=row.getPrevRow();if(row&&row.Expandable)
rows=row.Rows;else
{if(!row)
row=rows.ParentRow;rows=null;}}
while(row&&(row.Band!=this.Band||row.getCellByColumn(this).getValue(true).search(g.regExp)==-1))
row=row.getNextTabRow(true,true);}
g.lastSearchedCell=(row?row.getCellByColumn(this):null);return g.lastSearchedCell;},"hideValidators",function()
{if(!this.Validators)
return;for(var v=0;v<this.Validators.length;v++)
{var validator=document.getElementById(this.Validators[v]);validator.isvalid=true;ValidatorUpdateDisplay(validator);}},"findNext",function(re,back)
{var g=this.Band.Grid;if(!g.lastSearchedCell||g.lastSearchedCell.Column!=this)
return this.find(re,back);if(re)
g.regExp=re;if(!g.regExp)
return null;if(back==true||back==false)
g.backwardSearch=back;var row=g.lastSearchedCell.Row.getNextTabRow(g.backwardSearch,true);while(row&&(row.Band!=this.Band||row.getCellByColumn(this).getValue(true).search(g.regExp)==-1))
row=row.getNextTabRow(g.backwardSearch,true);g.lastSearchedCell=(row?row.getCellByColumn(this):null);return g.lastSearchedCell;},"getFooterText",function()
{var fId=this.Band.Grid.Id
+"_"
+"f_"+this.Band.Index+"_"+this.Index;var foot=igtbl_getElementById(fId);if(foot)
return igtbl_getInnerText(foot);return"";},"setFooterText",function(value,useMask)
{var fId=this.Band.Grid.Id
+"_"
+"f_"+this.Band.Index+"_"+this.Index;var foot=igtbl_getDocumentElement(fId);if(foot)
{if(useMask&&this.MaskDisplay)
value=igtbl_Mask(this.Band.Grid.Id,value.toString(),this.DataType,this.MaskDisplay);else if(useMask&&this.getEditorControl()&&this.editorControl.getRenderedValue)
value=this.getEditorControl().getRenderedValue(value);if(igtbl_string.trim(value)=="")
value="&nbsp;";if(!foot.length)
foot=[foot];var fElem=foot[0];if(fElem.childNodes.length>0&&fElem.childNodes[0].tagName=="NOBR")
value="<nobr>"+value+"</nobr>";for(var i=0;i<foot.length;i++)
{fElem=foot[i];fElem.innerHTML=value;}}},"getSelClass",function()
{if(this.SelCellClass!="")
return this.SelCellClass;return this.Band.getSelClass();},"getHeadClass",function()
{if(this.HeaderClass!="")
return this.HeaderClass;return this.Band.getHeadClass();},"getFooterClass",function()
{if(this.FooterClass!="")
return this.FooterClass;return this.Band.getFooterClass();},"compareRows",function(row1,row2)
{if(igtbl_columnCompareRows)
return igtbl_columnCompareRows.apply(this,[row1,row2]);return 0;},"compareCells",function(cell1,cell2)
{if(igtbl_columnCompareCells)
return igtbl_columnCompareCells.apply(this,[cell1,cell2]);return 0;},"move",function(toIndex)
{if(!this.Node)return;var band=this.Band;var bandNo=band.Index;var gs=band.Grid;if(bandNo==0&&!band.IsGrouped)
{var arIndex=-1,acColumn=null,acrIndex=-1;if(gs.oActiveRow&&gs.oActiveRow.OwnerCollection==gs.Rows)
arIndex=gs.oActiveRow.getIndex();if(gs.oActiveCell&&gs.oActiveCell.Row.OwnerCollection==gs.Rows)
{acColumn=gs.oActiveCell.Column;acrIndex=gs.oActiveCell.Row.getIndex();}
gs.setActiveRow(null);gs.setActiveCell(null);if((gs.StatHeader||gs.StatFooter)&&gs.Rows.FilterRow)
{var cells=gs.Rows.FilterRow.getCellElements();var cell=cells[this.Index];var insertCell=cells[toIndex];cell.parentNode.removeChild(cell);var parent=insertCell.parentNode;if(toIndex>this.Index)
{var shouldAppend=(insertCell.nextSibling==null);if(shouldAppend)parent.appendChild(cell);else parent.insertBefore(cell,insertCell.nextSibling);}
else
parent.insertBefore(cell,insertCell);var cellObj=gs.Rows.FilterRow.cells[this.Index];gs.Rows.FilterRow.cells[this.Index]=gs.Rows.FilterRow.cells[toIndex];gs.Rows.FilterRow.cells[toIndex]=cellObj;cell=parent.cells[band.firstActiveCell];for(var i=0;i<band.Columns.length&&cell;i++)
if(band.Columns[i].hasCells())
{var splitId=cell.id.split("_");splitId[splitId.length-1]=i.toString();cell.id=splitId.join("_");cellObj=gs.Rows.FilterRow.cells[i];if(cellObj)
{cellObj.index=i;cellObj.Id=cell.id;}
cell=cell.nextSibling;}}
this._move(toIndex);gs.Rows.repaint();if(arIndex!=-1)
gs.Rows.getRow(arIndex).activate();if(acColumn&&acrIndex>=0)
gs.Rows.getRow(acrIndex).getCellByColumn(acColumn).activate();}
else
{var elem=igtbl_getDocumentElement(this.Id);var rAr=new Array();if(typeof(elem)!="undefined")
{if(!elem.length)
elem=[elem];for(var i=0;i<elem.length;i++)
{var pe=elem[i].parentNode.parentNode.parentNode.parentNode;if(pe.tagName=="DIV"&&pe.id.substr(pe.id.length-4)=="_drs")
pe=pe.parentNode.parentNode.parentNode.parentNode.parentNode;var ps=pe.parentNode.previousSibling;if(ps)
rAr[i]=igtbl_getRowById(ps.id);}}
var arIndex=-1,acColumn=null,acrIndex=-1,aRows=null;if(gs.oActiveRow)
{arIndex=gs.oActiveRow.getIndex();aRows=gs.oActiveRow.OwnerCollection;if(aRows.Band.Index>=bandNo)
gs.setActiveRow(null);}
if(gs.oActiveCell)
{acColumn=gs.oActiveCell.Column;acrIndex=gs.oActiveCell.Row.getIndex();aRows=gs.oActiveCell.Row.OwnerCollection;if(aRows.Band.Index>=bandNo)
gs.setActiveCell(null);}
this._move(toIndex);for(var i=0;i<rAr.length;i++)
{if(rAr[i])
{rAr[i].Rows.repaint();if(aRows==rAr[i].Rows)
{if(arIndex!=-1)
aRows.getRow(arIndex).activate();if(acColumn)
aRows.getRow(acrIndex).getCellByColumn(acColumn).activate();aRows=null;}
rAr[i]=null;}}
igtbl_dispose(rAr);delete rAr;}},"_move",function(toIndex)
{oldIndex=this.Index;this.Band.Grid._recordChange("ColumnMove",this,toIndex);var b=this.Band,oldSortedColumn=null;if(b.SortedColumns&&b.SortedColumns.length>0)
{oldSortedColumn=new Array();for(var i=0;i<b.SortedColumns.length;i++)
for(var j=0;j<b.Columns.length;j++)
if(b.Columns[j].Id==b.SortedColumns[i])
{oldSortedColumn[i]=b.Columns[j];break;}}
this.Band.insertColumn(this.Band.removeColumn(this.Index),toIndex);if(oldSortedColumn)
for(var i=0;i<oldSortedColumn.length;i++)
{b.SortedColumns[i]=oldSortedColumn[i].Id;oldSortedColumn[i]=null;}
igtbl_dispose(oldSortedColumn);igtbl_swapCells(this.Band.Grid.Rows,this.Band.Index,oldIndex,toIndex);},"_filterOnBand",function(bandIndex,recordSet)
{var band=this.Band;if(!recordSet||band.Index>bandIndex)return;if(bandIndex==recordSet.Band.Index)
{this._filterOnRowIsland(recordSet);}
else
{var recordsetLength=recordSet.length;for(var itr=0;itr<recordsetLength;itr++)
{this._filterOnBand(bandIndex,recordSet.getRow(itr).Rows);}}},"_filterOnRowIsland",function(rowCollection)
{var siblingRows=null;if(rowCollection)
{siblingRows=rowCollection;}
else
{if((this.Band.Index==0&&this.Band.GroupCount==0))
{siblingRows=this.Band.Grid.Rows;}
else
{var colE=this.Band.Grid.event.srcElement;if(!colE)
this.Band.Grid.event.target;var parentTable=colE;do
{parentTable=parentTable.parentNode;}while(parentTable&&!(parentTable.tagName=="DIV"&&parentTable.id.length>0))
if(!parentTable)return;var parentRow=igtbl_getRowById(parentTable.id.slice(0,parentTable.id.length-7));if(parentRow)
siblingRows=parentRow.Rows;else
siblingRows=this.Band.Grid.Rows;}}
var srCount=siblingRows.length;var cellIndex=this.Index;var oFilterConditions=null;if(this.Band.Index==0&&this.Band.GroupCount==0)
{oFilterConditions=this.Band._filterPanels;}
else if((this.Band.Columns[0].RowFilterMode==1&&this.Band.GroupCount==0))
{oFilterConditions=this.Band._filterPanels;}
else
{oFilterConditions=this.Band._filterPanels[siblingRows.Element.parentNode.id];}
var myFilterCondition=oFilterConditions[this.Id];if(myFilterCondition&&myFilterCondition.IsActive())
{var myDirectColumnHeader=igtbl_getChildElementById(siblingRows.Element.parentNode,myFilterCondition.Column.Id);if(this.Band.Index==0&&this.Band.Grid.StatHeader)
{myDirectColumnHeader=this.Band.Grid.StatHeader.getElementByColumn(this)}
else
{myDirectColumnHeader=igtbl_getChildElementById(siblingRows.Element.parentNode,myFilterCondition.Column.Id);}
if(myDirectColumnHeader)
{var filterImg=this._findFilterImage(myDirectColumnHeader);if(filterImg)
{var alt=filterImg.getAttribute("alt");if(myFilterCondition.getOperator()==igtbl_filterComparisionOperator.All)
{filterImg.src=this.Band.Grid.FilterDefaultImage;if(alt!=null)
{var clpsAlt=filterImg.getAttribute("igAltF0");if(clpsAlt!=null)
{filterImg.setAttribute("igAltF1",alt);filterImg.setAttribute("alt",clpsAlt);filterImg.removeAttribute("igAltF0");}}}
else
{filterImg.src=this.Band.Grid.FilterAppliedImage;if(alt!=null)
{var clpsAlt=filterImg.getAttribute("igAltF1");if(clpsAlt!=null)
{filterImg.setAttribute("igAltF0",alt);filterImg.setAttribute("alt",clpsAlt);filterImg.removeAttribute("igAltF1");}}}}}}
for(var srCounter=0;srCounter<srCount;srCounter++)
{this._evaluateFilters(siblingRows.getRow(srCounter),oFilterConditions,this.Band);}},"_findFilterImage",function(elem)
{if(elem.tagName=="IMG"&&elem.getAttribute("imgType")=="filter")
return elem;for(var itr=0;itr<elem.childNodes.length;itr++)
{var e=this._findFilterImage(elem.childNodes[itr]);if(e)return e;}
return null;},"_evaluateFilters",function(oRow,oFilterCollection,oBand)
{if(oRow.GroupByRow)
{var srCount=oRow.Rows.length;for(var srCounter=0;srCounter<srCount;srCounter++)
{this._evaluateFilters(oRow.Rows.getRow(srCounter),oFilterCollection,oBand);}
return;}
var showRow=true;for(var filter in oFilterCollection)
{filter=oFilterCollection[filter];if(filter.IsActive())
{var filterCol=filter.Column;var evalValue=filter.getEvaluationValue();var cellValue=oRow.getCell(filterCol.Index).getValue();switch(filterCol.DataType)
{case 7:{if(evalValue)
evalValue=new Date(evalValue).valueOf();if(cellValue)
cellValue=new Date(cellValue).valueOf();break;}
case 11:{if(evalValue)
evalValue=igtbl_string.stringToBool(evalValue);if(cellValue)
cellValue=igtbl_string.stringToBool(cellValue);}}
if(!this._evaluateExpression(filter.getOperator(),cellValue,evalValue,filterCol.FilterComparisonType,filterCol.DataType))
{showRow=false;break;}}}
if(oRow.getHidden()!=!showRow)
oRow.setHidden(!showRow);},"_evaluateExpression",function(operator,operand1,operand2,caseSensitive,columnDataType)
{operator=parseInt(operator);switch(operator)
{case(igtbl_filterComparisionOperator.NotEmpty):{return operand1&&(typeof(operand1)=="string"?operand1.length>0:true);break;}
case(igtbl_filterComparisionOperator.Empty):{if(operand1===null)return true;if(typeof(operand1)=="string")
return operand1.length==0;return false;break;}
case(igtbl_filterComparisionOperator.All):{return true;break;}
case(igtbl_filterComparisionOperator.Equals):{if(caseSensitive==igtbl_filterComparisonType.CaseInsensitive&&columnDataType==8&&operand1&&operand2)
{operand2=operand2.replace("?",".");operand2=operand2.replace("*",".*");operand2=igtbl_regExp.escape(operand2,[".","*"]);var re=new RegExp("^"+operand2+"$","i");return operand1.match(re)!=null;}
else
return operand1==operand2;break;}
case(igtbl_filterComparisionOperator.NotEquals):{if(caseSensitive==igtbl_filterComparisonType.CaseInsensitive&&columnDataType==8&&operand1&&operand2)
{operand2=operand2.replace("?",".");operand2=operand2.replace("*",".*");operand2=igtbl_regExp.escape(operand2,[".","*"]);var re=new RegExp("^"+operand2+"$","i");return operand1.match(re)==null;}
else
return operand1!=operand2;break;}
case(igtbl_filterComparisionOperator.Like):{if(columnDataType==8)
{operand2=operand2.replace("?",".");operand2=operand2.replace("*",".*");operand2=igtbl_regExp.escape(operand2,[".","*"]);var re=new RegExp("^"+operand2,caseSensitive==1?"i":"");return operand1&&operand1.match(re);}
return false;break;}
case(igtbl_filterComparisionOperator.NotLike):{if(columnDataType==8)
{var likeMatches=this._evaluateExpression(igtbl_filterComparisionOperator.Like,operand1,operand2,caseSensitive,columnDataType)||this._evaluateExpression(igtbl_filterComparisionOperator.Equals,operand1,operand2,caseSensitive,columnDataType);return!likeMatches;}
return false;break;}
case(igtbl_filterComparisionOperator.GreaterThan):{if(columnDataType==8)
{if(operand1==null)return false;if(operand1&&operand2==null)return true;if(caseSensitive==igtbl_filterComparisonType.CaseInsensitive)
{return operand1.toLowerCase()>operand2.toLowerCase();}
else
{return operand1>operand2;}}
else
{return operand1>operand2;}
break;}
case(igtbl_filterComparisionOperator.GreaterThanOrEqualTo):{return this._evaluateExpression(igtbl_filterComparisionOperator.GreaterThan,operand1,operand2,caseSensitive,columnDataType)||this._evaluateExpression(igtbl_filterComparisionOperator.Equals,operand1,operand2,caseSensitive,columnDataType);break;}
case(igtbl_filterComparisionOperator.LessThanOrEqualTo):{return(!this._evaluateExpression(igtbl_filterComparisionOperator.GreaterThan,operand1,operand2,caseSensitive,columnDataType))||this._evaluateExpression(igtbl_filterComparisionOperator.Equals,operand1,operand2,caseSensitive,columnDataType);break;}
case(igtbl_filterComparisionOperator.LessThan):{return(!this._evaluateExpression(igtbl_filterComparisionOperator.GreaterThanOrEqualTo,operand1,operand2,caseSensitive,columnDataType));break;}
case(igtbl_filterComparisionOperator.StartsWith):{if(columnDataType==8)
{if(operand1==null)return false;if(operand1&&operand2==null)return true;operand2=operand2.replace("?","\\?");operand2=operand2.replace("*","\\*");operand2=igtbl_regExp.escape(operand2,[".","*"]);var re=new RegExp("^"+operand2,caseSensitive==1?"i":"");return operand1&&operand1.match(re);}
break;}
case(igtbl_filterComparisionOperator.DoesNotStartWith):{return(!this._evaluateExpression(igtbl_filterComparisionOperator.StartsWith,operand1,operand2,caseSensitive,columnDataType));break;}
case(igtbl_filterComparisionOperator.EndsWith):{if(columnDataType==8)
{if(operand1==null)return false;if(operand1&&operand2==null)return true;operand2=operand2.replace("?","\\?");operand2=operand2.replace("*","\\*");operand2=igtbl_regExp.escape(operand2,[".","*"]);var re=new RegExp(operand2+"$",caseSensitive==1?"i":"");return operand1&&operand1.match(re);}
break;}
case(igtbl_filterComparisionOperator.DoesNotEndWith):{return(!this._evaluateExpression(igtbl_filterComparisionOperator.EndsWith,operand1,operand2,caseSensitive,columnDataType));break;}
case(igtbl_filterComparisionOperator.Contains):{if(columnDataType==8)
{if(operand1==null)return false;if(operand1&&operand2==null)return true;var re=new RegExp(igtbl_regExp.escape(operand2),caseSensitive==1?"i":"");return operand1&&operand1.match(re);}
break;}
case(igtbl_filterComparisionOperator.DoesNotContain):{return(!this._evaluateExpression(igtbl_filterComparisionOperator.Contains,operand1,operand2,caseSensitive,columnDataType));break;}}},"_fillFilterList",function(vc,sr)
{var srCount=sr.length;var cellIndex=this.Index;var oCell=null;var oRow=null;for(var srCounter=0;srCounter<srCount;srCounter++)
{oRow=sr.getRow(srCounter);var cellValue;var cellText;if(!oRow.GroupByRow)
{oCell=oRow.getCell(cellIndex);cellValue=oCell.getValue();cellText=oCell.getValue(true);if(cellValue!=null&&typeof(cellValue.getFullYear)=="function")
{cellValue=igtbl_dateToString(cellValue);}
if(cellText)
{if((typeof(cellValue)=="string"&&cellValue.length==0)||(typeof(cellText)=="string"&&cellText.length==0))
continue;if(ig_shared.IsFireFox)
{cellText=cellText.replace(/\r\n/g," ");cellText=cellText.replace(/\n/g," ");if(typeof(cellValue)=="string")
cellValue=cellText;}
vc.push([cellValue,cellText]);}}
else
{this._fillFilterList(vc,oRow.Rows);}}},"_getSiblingRowIsland",function()
{var siblingRows=null;if(this.Band.Index==0&&this.Band.GroupCount==0)
{siblingRows=this.Band.Grid.Rows;}
else
{var colE=null;try
{colE=this.Band.Grid.event.srcElement;if(!colE)colE=this.Band.Grid.event.target;}
catch(e)
{colE=this._filterSrcElement;delete this._filterSrcElement;}
if(!colE)return null;var parentTable=colE;do
{parentTable=parentTable.parentNode;}while(parentTable&&!(parentTable.tagName=="TABLE"&&parentTable.id.length>0))
if(!parentTable)return;var parentRow=igtbl_getRowById(parentTable.id);if(!parentRow)
siblingRows=this.Band.Grid.Rows;else
siblingRows=parentRow.Rows;}
return siblingRows;},"_getFilterValuesFromSiblings",function(rowCollection)
{var siblingRows=null;if(rowCollection)
{siblingRows=rowCollection;}
else
{siblingRows=this._getSiblingRowIsland();}
var workingList=new Array();if(!siblingRows||siblingRows.length==0)
{}
else
{this._fillFilterList(workingList,siblingRows);if(this.DataType==2||this.DataType==3||this.DataType==16||this.DataType==17||this.DataType==18||this.DataType==19||this.DataType==20||this.DataType==21)
workingList.sort(_igtbl_sortNumber);else
workingList.sort();}
return workingList;},"_getFilterPanel",function(sourceElement)
{var filterPanel=null;var band=this.Band;var g=band.Grid;if(this.RowFilterMode==2)
{if(band.Index==0&&band.GroupCount==0)
{if(!band._filterPanels[this.Id])
{band._filterPanels[this.Id]=new igtbl_FilterDropDown(this);}
filterPanel=band._filterPanels[this.Id];filterPanel.RowIsland=g.Rows;}
else
{var colE=sourceElement;if(!colE)
colE=g.event.srcElement;if(!colE)
colE=g.event.target;var parentTable=colE;if(band.Index==0&&band.IsGrouped&&(g.StationaryMargins==1||g.StationaryMargins==3)&&g.get("StationaryMarginsOutlookGroupBy")=="True")
{parentTable=g.Rows.Element;}
do
{parentTable=parentTable.parentNode;}while(parentTable&&!(parentTable.tagName=="TABLE"&&parentTable.id.length>0))
if(!parentTable)return;filterPanel=band._filterPanels[parentTable.id];if(filterPanel)
filterPanel=filterPanel[this.Id];else
{band._filterPanels[parentTable.id]=new Object();}
if(!filterPanel)
{filterPanel=band._filterPanels[parentTable.id][this.Id]=new igtbl_FilterDropDown(this);}
if(filterPanel.RowIsland==null||filterPanel.RowIsland.Type!="rows")
{var row=igtbl_getRowById(parentTable.id);if(row)
filterPanel.RowIsland=row.Rows;else
{if(band.Index==0&&band.IsGrouped&&(g.StationaryMargins==1||g.StationaryMargins==3)&&g.get("StationaryMarginsOutlookGroupBy")=="True")
{filterPanel.RowIsland=g.Rows;}}}}}
else if(this.RowFilterMode==1)
{filterPanel=band._filterPanels[this.Id];if(!filterPanel)
{filterPanel=band._filterPanels[this.Id]=new igtbl_FilterDropDown(this);}
if(band.Index==0&&band.IsGrouped)
{filterPanel.RowIsland=g.Rows;}}
else
{return null;}
return filterPanel;},"_getFilterValuesFromBand",function()
{var resultSet;if(this.Band.Index==0&&!this.Band.IsGrouped)
{resultSet=this._getFilterValuesFromSiblings();}
else
{resultSet=this._fillFilterListFromBand(this.Band.Index,this.Band.Grid.Rows);}
if(this.DataType==2||this.DataType==3||this.DataType==16||this.DataType==17||this.DataType==18||this.DataType==19||this.DataType==20||this.DataType==21)
resultSet.sort(_igtbl_sortNumber);else
resultSet.sort();return resultSet;},"_fillFilterListFromBand",function(bandIndex,recordSet)
{var resultSet=new Array();if(!recordSet||bandIndex<recordSet.Band.Index)
return resultSet;if(bandIndex==recordSet.Band.Index)
{resultSet=this._getFilterValuesFromSiblings(recordSet);}
else
{var recordsetLength=recordSet.length;for(var itr=0;itr<recordsetLength;itr++)
{var tempSet=this._fillFilterListFromBand(bandIndex,recordSet.getRow(itr).Rows);if(tempSet&&tempSet.length>0)
resultSet=resultSet.concat(tempSet);}}
return resultSet;},"showFilterDropDown",function(drop)
{var autoDropCheck=(typeof(drop)==='undefined')
if(autoDropCheck)
drop=true;if(this.AllowRowFiltering<2)return;var filterPanel=this._getFilterPanel();if(filterPanel==null)return;if(!drop)
{if(filterPanel.IsDropped)
{filterPanel.show(false);}
return;}
else
{if(autoDropCheck)
drop=!filterPanel.IsDropped;filterPanel.show(drop);}},"getFilterIcon",function()
{return this.FilterIcon;},"setFilterIcon",function(show)
{if(show!=this.getFilterIcon())
{this.FilterIcon=show;var headerTags=this._getHeadTags();if(headerTags)
{for(var x=0;x<headerTags.length;x++)
{var filterIcon=this._findFilterImage(headerTags[x]);if(filterIcon)
{if(show)
filterIcon.style.display="";else
filterIcon.style.display="none";}}
this.Band.Grid._removeChange("ColumnFilterIconChanged",this);this.Band.Grid._recordChange("ColumnFilterIconChanged",this,show);}}},"getLevel",function(s)
{var l=new Array();l[0]=this.Band.Index;l[1]=this.Index;if(s)
{s=l.join("_");igtbl_dispose(l);delete l;return s;}
return l;},"getFixed",function()
{if(this.Band.Grid.UseFixedHeaders)
return this.Fixed;},"setFixed",function(fixed)
{this.Fixed=fixed;},"getWidth",function()
{if(typeof(this.Width)!="string")
return this.Width;var e=igtbl_getElementById(this.Id);if(!e||!e.offsetWidth||typeof(this.Width)=="string"&&this.Width.substr(this.Width.length-2,2)=="px")
this.Width=igtbl_parseInt(this.Width);if(typeof(this.Width)=="string")
{this.Width=e.offsetWidth;}
return this.Width;},"setWidth",function(width)
{var gs=this.Band.Grid,gn=gs.Id;var colObj=igtbl_getElementById(this.Id);if(!colObj)return;var scrLeft=gs._scrElem?gs._scrElem.scrollLeft:0;var fac=this.Band.firstActiveCell;var c1w=width;var corr=0;if(c1w>0&&!igtbl_fireEvent(gn,gs.Events.BeforeColumnSizeChange,"(\""+gn+"\",\""+colObj.id+"\","+c1w+")"))
{var fixed=(gs.UseFixedHeaders&&!this.getFixed());var colEl=this.Element;if(colEl&&gs.UseFixedHeaders&&gs.IsXHTML&&ig_csom.IsIE)
{corr=colEl.offsetWidth-colEl.clientWidth;corr+=igtbl_parseInt(colEl.currentStyle.paddingLeft);corr+=igtbl_parseInt(colEl.currentStyle.paddingRight);}
if((gs.UseFixedHeaders||gs.XmlLoadOnDemandType==2)&&this.Band.Index==0)
{var origWidth=gs._scrElem.firstChild.offsetWidth;var scrw=origWidth+c1w-this.getWidth()-(fixed?corr:0);if(scrw>=0&&this.Band.Index==0&&scrw!=origWidth)
{gs._scrElem.firstChild.style.width=scrw+corr+"px";}}
var fixedColIndex=null;var columns=igtbl_getDocumentElement(this.Id);if(!columns.length)
columns=[columns];if(fixed)
{for(var i=0;i<columns.length;i++)
{var cells=igtbl_enumColumnCells(gn,columns[i]);for(var j=0;j<cells.length;j++)
{var cg=cells[j].parentNode.parentNode.previousSibling;if(cg)
{var colIndex=cells[j].cellIndex;if(ig_shared.IsNetscape6)
{var curIndex=this.Index;while(colIndex>0&&--curIndex>=0)
if(!this.Band.Columns[curIndex].getVisible()&&!this.Band.Columns[curIndex].getFixed()&&!this.Band.Columns[curIndex].IsGroupBy)
colIndex--;fixedColIndex=colIndex;}
var c=cg.childNodes[colIndex];if(c)
{c.style.width=c1w+"px";c.width=c1w+"px";}}
cells[j].style.width=c1w+"px";}}
var colFoots=igtbl_getDocumentElement(this.fId);if(colFoots)
{if(!colFoots.length)
colFoots=[colFoots];for(var i=0;i<colFoots.length;i++)
{var cg=colFoots[i].parentNode.parentNode.previousSibling;if(cg&&cg.tagName=="COLGROUP")
{var c=cg.childNodes[colFoots[i].cellIndex];if(ig_shared.IsNetscape6&&fixedColIndex!=null&&fixedColIndex>-1&&fixedColIndex<cg.childNodes.length)
c=cg.childNodes[fixedColIndex];if(c)
{c.style.width=c1w+"px";c.width=c1w+"px";}}
var nfth=colFoots[i].parentNode;while(nfth&&nfth.tagName!="TH")
nfth=nfth.parentNode;if(nfth&&this.Band.Index==0&&this.Band.Index==0&&gs.StatFooter)
{cg=nfth.parentNode.parentNode.previousSibling;if(this.Band.AddNewRowView==2&&gs.Rows.AddNewRow)
{cg=cg.previousSibling;var addRow=gs.Rows.AddNewRow;var c=addRow.getCell(this.Index).Element.parentNode.parentNode.previousSibling.childNodes[colFoots[i].cellIndex];if(c)
{c.style.width=c1w+"px";}}
if(cg)
{var nfthCellIndex=nfth.cellIndex;if(ig_csom.IsFireFox)
{var nfthElements=nfth.parentNode.childNodes;for(var cellIndexItr=0;cellIndexItr<nfth.cellIndex;cellIndexItr++)
if(nfthElements[0].style.display=="none")
nfthCellIndex--;}
var c=cg.childNodes[nfthCellIndex+colFoots[i].cellIndex];if(c)
{c.style.width=c1w+"px";c.width=c1w+"px";}}}
colFoots[i].style.width=c1w+"px";}}}
for(var i=0;i<columns.length;i++)
{var cg=columns[i].parentNode.parentNode.previousSibling;var colIndex=columns[i].cellIndex;if(columns[i].style.display=="none"||ig_csom.IsFireFox)
{var itr=0;var parentCollection=columns[i].parentNode.childNodes;for(;itr<parentCollection.length;itr++)
{if(parentCollection[itr]==columns[i])break;}
colIndex=itr;}
if(ig_csom.IsNetscape6)
{var pn=columns[i].parentNode;for(var j=0;j<=columns[i].cellIndex;j++)
{var colObject=igtbl_getColumnById(pn.childNodes[j].id);if(pn.childNodes[j].style.display=="none"&&colObject&&!colObject.getVisible())
colIndex--;}}
if(this.Band.HasHeaderLayout&&cg)
{var colOffs=parseInt(columns[i].getAttribute("coloffs"),10);if(this.getFixed()!==false)
colOffs+=this.Band.firstActiveCell;;var c=cg.childNodes[colOffs];var widthChange=c1w-c.width;c.style.width=c1w+"px";if(ig_csom.IsFireFox)
{c.width=c1w;c.offsetParent.style.width=(c.offsetParent.clientWidth+widthChange)+"px";}
if(fixed)
{var nfth=columns[i].parentNode;while(nfth&&nfth.tagName!="TH")
nfth=nfth.parentNode;if(nfth)
{cg=nfth.parentNode.parentNode.previousSibling;if(cg)
{var c=cg.childNodes[nfth.cellIndex+colIndex];var widthChange=c1w-c.width;c.style.width=c1w+"px";c.width=c1w+"px";if(ig_csom.IsFireFox)
{c.width=c1w;c.offsetParent.style.width=(c.offsetParent.clientWidth+widthChange)+"px";}}}}}
else
{var c;if(cg)
c=cg.childNodes[colIndex];else
c=columns[i];c.style.width=c1w+"px";c.width=c1w;columns[i].style.width=c1w+"px";if(fixed)
{var nfth=columns[i].parentNode;while(nfth&&nfth.tagName!="TH")
nfth=nfth.parentNode;if(nfth)
{cg=nfth.parentNode.parentNode.previousSibling;if(cg)
{var nfthCellIndex=nfth.cellIndex;if(ig_csom.IsFireFox)
{var nfthElements=nfth.parentNode.childNodes;for(var cellIndexItr=0;cellIndexItr<nfth.cellIndex;cellIndexItr++)
if(nfthElements[0].style.display=="none")
nfthCellIndex--;}
var c=cg.childNodes[nfthCellIndex+colIndex];c.style.width=c1w+corr+"px";c.width=c1w+corr+"px";}
if(this.Band.Index==0&&this.Band.AddNewRowView==1&&!this.Band.IsGrouped&&gs.StatHeader)
{cg=cg.previousSibling;var addRow=gs.Rows.AddNewRow;var c=addRow.getCell(this.Index).Element.parentNode.parentNode.previousSibling.childNodes[columns[i].cellIndex];c.style.width=c1w+"px";}}}
else
{var table=columns[i];while(table&&table.tagName!="TABLE")
table=table.parentNode;if(table&&table.style.width.length>0)
{var oldWidth=table.style.width;if(oldWidth.length>2&&oldWidth.substr(oldWidth.length-2,2)=="px")
{var tbw=igtbl_parseInt(oldWidth)+c1w-this.getWidth();if(tbw>0)
table.style.width=tbw.toString()+"px";}}}
if(gs.get("StationaryMarginsOutlookGroupBy")=="True"&&this.Band.Index==0&&this.Band.IsGrouped&&i==0)
{table=gs.getDivElement().firstChild;for(var gri=0;gri<gs.Rows.rows.length;gri++)
{var grId=gn+"_gr_"+gri.toString();var grTbl,grWidth;if(document.getElementById(grId)&&document.getElementById(grId).childNodes[0]&&document.getElementById(grId).childNodes[0].childNodes[0])
{grTbl=document.getElementById(grId).childNodes[0].childNodes[0];var currentgrTblWidth=(grTbl.width&&grTbl.width.indexOf("%")!=-1||grTbl.width=="")?null:igtbl_parseInt(grTbl.width);if(currentgrTblWidth==null)
break;grWidth=currentgrTblWidth+c1w-this.getWidth();grTbl.width=grWidth.toString()+"px";}
for(var j=1;j<gs.GroupCount;j++)
{grId=grId+"_0";if(document.getElementById(grId)&&document.getElementById(grId).childNodes[0]&&document.getElementById(grId).childNodes[0].childNodes[0])
{grTbl=document.getElementById(grId).childNodes[0].childNodes[0];currentgrTblWidth=(grTbl.width&&grTbl.width.indexOf("%")!=-1)||grTbl.width==""?null:igtbl_parseInt(grTbl.width);if(currentgrTblWidth==null)
break;grWidth=currentgrTblWidth+c1w-this.getWidth();grTbl.width=grWidth.toString()+"px";}}}}}}
this.Width=c1w;if(this.Node)this.Node.setAttribute(igtbl_litPrefix+"width",c1w);if(this.Band.Index==0)
{if(gs.StatHeader)
gs.StatHeader.ScrollTo(gs.Element.parentNode.scrollLeft);if(gs.StatFooter)
{if(!fixed)
gs.StatFooter.Resize(this.Index,c1w);gs.StatFooter.ScrollTo(gs.Element.parentNode.scrollLeft);}}
gs.alignStatMargins();gs.alignDivs(0,true);this.Band._alignColumns();gs._removeChange("ResizedColumns",this);gs._recordChange("ResizedColumns",this,c1w);igtbl_fireEvent(gn,gs.Events.AfterColumnSizeChange,"(\""+gn+"\",\""+colObj.id+"\","+c1w+")");if(gs.NeedPostBack)
igtbl_doPostBack(gn);var de=gs.getDivElement();if(!gs.MainGrid.style.height&&de.clientHeight!=de.scrollHeight&&!de.getAttribute("scdAdded"))
{var scDiv=document.createElement("DIV");scDiv.id=gs.Element.id+"_scd";scDiv.innerHTML="&nbsp;";var divsHeight=de.scrollHeight-de.clientHeight;if(divsHeight<0)divsHeight=-divsHeight;scDiv.style.height=divsHeight+1;de.appendChild(scDiv);de.style.overflowY="hidden";de.setAttribute("scdAdded","true");}
scrLeftNew=gs._scrElem?gs._scrElem.scrollLeft:0;if(scrLeft!=scrLeftNew)
gs._recordChange("ScrollLeft",gs,scrLeftNew);return true;}
return false;},"ensureWebCombo",function()
{if(typeof(igcmbo_getComboById)!="undefined"&&igcmbo_getComboById(this.EditorControlID)&&!this.WebComboId)
this.WebComboId=this.EditorControlID;},"getRealIndex",function(row)
{if(!this.hasCells())
return-1;var ri=-1;var colspan=1;var cell=null;if(row)
cell=row.Element.cells[row.Band.firstActiveCell];var i=0;while(i<this.Index+1&&!this.Band.Columns[i].hasCells())
i++;if(i>this.Index)
return ri;ri=0;for(;i<this.Index;i++)
{if(!this.Band.Columns[i].hasCells())
continue;if(row)
{if(colspan>1)
{colspan--;continue;}
var cellSplit;if(cell)
{cellSplit=cell.id.split("_");if(parseInt(cellSplit[cellSplit.length-1],10)>i)
ri--;else
{cell=cell.nextSibling;if(cell)
colspan=cell.colSpan;}}}
ri++;}
return ri;},"getFixedHeaderIndicator",function()
{if(this.FixedHeaderIndicator!=0)
return this.FixedHeaderIndicator;if(this.Band.FixedHeaderIndicator!=0)
return this.Band.FixedHeaderIndicator;return this.Band.Grid.FixedHeaderIndicator;},"getValueFromString",function(value)
{if(value==null||typeof(value)=="undefined")
return null;value=value.toString();if(this.AllowNull&&value==this.getNullText())
return null;return igtbl_valueFromString(value,this.DataType);},"_getHeadTags",function(withAddRow)
{var elem=null;if(this.Id)
elem=igtbl_getDocumentElement(this.Id);elem=igtbl_getArray(elem);if(withAddRow)
{var addRow=this.Band.Grid.Rows.AddNewRow;var addNewPresent=(addRow&&addRow.isFixedTop());if(!addNewPresent)
return elem;var ri=this.Band.firstActiveCell;var columns=this.Band.Columns;for(var i=0;i<this.Index;i++)
if(columns[i].hasCells())
ri++;if(this.getFixed()===false)
{var fnfRi=this.Band.firstActiveCell;for(var i=0;i<columns.length&&columns[i].getFixed();i++)
if(columns[i].hasCells())
fnfRi++;ri=ri-fnfRi;var tbl=addRow.Element.cells[fnfRi].firstChild.firstChild;elem[elem.length]=tbl.rows[0].cells[ri];}
else
elem[elem.length]=addRow.Element.cells[ri];}
return elem;},"_getFootTags",function(withAddRow)
{var elem=null;if(this.fId)
elem=igtbl_getDocumentElement(this.fId);elem=igtbl_getArray(elem);if(withAddRow)
{var addRow=this.Band.Grid.Rows.AddNewRow;var addNewPresent=(addRow&&addRow.isFixedBottom());if(!addNewPresent)
return elem;var ri=this.Band.firstActiveCell;var columns=this.Band.Columns;for(var i=0;i<this.Index;i++)
if(columns[i].hasCells())
ri++;if(this.getFixed()===false)
{var fnfRi=this.Band.firstActiveCell;for(var i=0;i<columns.length&&columns[i].getFixed();i++)
if(columns[i].hasCells())
fnfRi++;ri=ri-fnfRi;var tbl=addRow.Element.cells[fnfRi].firstChild.firstChild;elem[elem.length]=tbl.rows[0].cells[ri];}
else
elem[elem.length]=addRow.Element.cells[ri];}
return elem;},"_getColTags",function(withAddRow)
{if(!this.hasCells())
return null;var band=this.Band;var fac=band.firstActiveCell;var g=band.Grid;var columns=band.Columns;var res=new Array();var gColOffs=fac;if(!this.getHidden())
{for(var i=0;i<this.Index;i++)
if(columns[i].getVisible())
gColOffs++;}
else
{for(var i=0;i<columns.length;i++)
if(columns[i].hasCells())
gColOffs++;for(var i=columns.length-1;i>=this.Index;i--)
if(columns[i].getHidden())
gColOffs--;}
var fnfColumn=null;var lColOffs=0;var fnfRi=fac;if(this.getFixed()===false)
{fnfColumn=this;while(fnfColumn.Index>0&&!this.Band.Columns[fnfColumn.Index-1].getFixed())
fnfColumn=this.Band.Columns[fnfColumn.Index-1];for(var i=0;i<fnfColumn.Index;i++)
{if(columns[i].getVisible())
lColOffs++;if(columns[i].hasCells())
fnfRi++;}
lColOffs=gColOffs-lColOffs-fac;}
else
lColOffs=gColOffs;var addRow=g.Rows.AddNewRow;var addNewHead=(addRow&&!addRow.isFixedTop());var addNewFoot=(addRow&&!addRow.isFixedBottom());var hAr=this._getHeadTags();if(hAr)
{var cg;for(var i=0;i<hAr.length;i++)
{if(this.getFixed()===false)
{var nfth=hAr[i].parentNode;while(nfth&&nfth.tagName!="TH")
nfth=nfth.parentNode;if(nfth)
{cg=nfth.parentNode.parentNode.previousSibling;if(cg)
res[res.length]=cg.childNodes[gColOffs];}}
cg=hAr[i].parentNode.parentNode.previousSibling;if(cg)
res[res.length]=cg.childNodes[lColOffs];}}
var fAr=this._getFootTags();if(fAr)
{var cg;for(var i=0;i<fAr.length;i++)
{if(this.getFixed()===false)
{var nfth=fAr[i].parentNode;while(nfth&&nfth.tagName!="TH")
nfth=nfth.parentNode;if(nfth)
{cg=nfth.parentNode.parentNode.previousSibling;if(addNewFoot)
cg=cg.previousSibling;if(cg)
res[res.length]=cg.childNodes[gColOffs];}}
if(this.Band.Index==0&&this.Band.Grid.StatFooter)
{cg=fAr[i].parentNode.parentNode.previousSibling;if(this.getFixed()!==false&&addNewFoot)
cg=cg.previousSibling;if(cg)
res[res.length]=cg.childNodes[lColOffs];}}}
if(withAddRow&&(addNewHead||addNewFoot)&&this.getFixed()===false)
{cg=addRow.Element.cells[fnfRi].firstChild.firstChild.firstChild;res[res.length]=cg.childNodes[lColOffs];}
if(res.length>0)
return res;return null;},"_insertCols",function(front,width)
{var cols=this._getColTags(true);for(var i=0;cols&&i<cols.length;i++)
{if(cols[i])
{var col=document.createElement("COL");col.width=width;var cg=cols[i].parentNode;if(front)
cg.insertBefore(col,cols[i]);else
{if(cols[i].nextSibling)
cg.insertBefore(col,cols[i].nextSibling);else
cg.appendChild(col);}}}},"_reId",function(i)
{if(i==this.Index)return;this._rec=true;for(var j=0;j<this.Band.Columns.length;j++)
{var col=this.Band.Columns[j];if(!col._rec&&col.Index==i)
{col._rec=true;this.Band.Columns[j]._reId(j);delete col._rec;}}
delete this._rec;var elem=null;var fElem=null;column=this;if(column.hasCells())
{if(this.Id)
elem=this._getHeadTags(true);else
elem=this.colElem;if(this.fId)
fElem=this._getFootTags(true);else
fElem=this.colFElem;}
column.Id=this.Band.Grid.Id
+"_"
+"c_"+this.Band.Index.toString()+"_"+i.toString();column.Index=i;if(this.Band.ColFootersVisible==1)
column.fId=this.Band.Grid.Id
+"_"
+"f_"+this.Band.Index.toString()+"_"+i.toString();if(elem)
for(var j=0;j<elem.length;j++)
{c=elem[j];if(c&&c.tagName=="TH")
{c.id=column.Id;c.setAttribute("columnNo",i.toString());}
else if(c)
{var r=c.parentNode;while(r&&(r.tagName!="TR"||!r.getAttribute("level")))
r=r.parentNode;if(r)
{cid=r.id.split("_");cid[0]=cid[0].substr(0,cid[0].length-1)+"c";cid[cid.length]=i.toString();c.id=cid.join("_")}}}
if(fElem)
for(var j=0;j<fElem.length;j++)
{c=fElem[j];if(c&&c.tagName=="TH")
c.id=column.fId;else if(c)
{var r=c.parentNode;while(r&&(r.tagName!="TR"||!r.getAttribute("level")))
r=r.parentNode;if(r)
{cid=r.id.split("_");cid[0]=cid[0].substr(0,cid[0].length-1)+"c";cid[cid.length]=i.toString();c.id=cid.join("_")}}}
igtbl_dispose(elem);igtbl_dispose(fElem);this._reIded=true;},"setHeaderText",function(value)
{var headerElements;if(this.Element)
{headerElements=new Array(this.Element);}
else
{headerElements=igtbl_getDocumentElement(this.Id);if(!headerElements)
{return;}
if(!headerElements.length)
{headerElements=new Array(headerElements);}}
for(hE=0;hE<headerElements.length;hE++)
{var el=headerElements[hE];for(n=0;n<el.childNodes.length;n++)
{if(el.childNodes[n].nodeType==1)
{el=el.childNodes[n];}}
if(el.tagName=="IMG")
{if(el.imgType)
{el.parentElement.innerHTML=value+el.outerHTML;}
else
{var caption;if(el.nextSibling&&el.nextSibling.tagName=="NOBR")
{caption=el.nextSibling;}
else
{caption=document.createElement("NOBR");if(el.nextSibling)
el.parentElement.insertBefore(caption,el.nextSibling);else
el.parentElement.appendChild(caption);}
if(caption)
{caption.innerHTML=value;}}}
else
{el.innerHTML=value;}}},"getTitleModeResolved",function()
{var result=this.HeaderTitleMode;if(!result&&this.Band)
{result=this.Band.HeaderTitleMode;}
if(!result&&this.Band&&this.Band.Grid)
{result=this.Band.Grid.HeaderTitleMode;}
return result;},"getSortingAlgorithm",function()
{if(this.SortingAlgorithm==0)
return this.Band.getSortingAlgorithm();return this.SortingAlgorithm;},"getSortImplementation",function()
{if(this.SortImplementation==null)
return this.Band.getSortImplementation();return this.SortImplementation;}];for(var i=0;i<igtbl_ptsColumn.length;i+=2)
igtbl_Column.prototype[igtbl_ptsColumn[i]]=igtbl_ptsColumn[i+1];var igtbl_reqType=new Object();igtbl_reqType.None=0;igtbl_reqType.ChildRows=1;igtbl_reqType.MoreRows=2;igtbl_reqType.Sort=3;igtbl_reqType.UpdateCell=4;igtbl_reqType.AddNewRow=5;igtbl_reqType.DeleteRow=6;igtbl_reqType.UpdateRow=7;igtbl_reqType.Custom=8;igtbl_reqType.Page=9;igtbl_reqType.Scroll=10;igtbl_reqType.FilterDropDownFill=11;igtbl_reqType.Filter=12;igtbl_reqType.Refresh=13;var igtbl_readyState=new Object();igtbl_readyState.Ready=0;igtbl_readyState.Loading=1;var igtbl_error=new Object();igtbl_error.Ok=0;igtbl_error.LoadFailed=1;var igtbl_featureRowView={"Top":1,"Bottom":2};var igtbl_featureRowView={"Top":1,"Bottom":2};var igtbl_filterComparisonType={"CaseInsensitive":1,"CaseSensitive":2};var igtbl_RowFilterMode={"AllRowsInBand":1,"SiblingRowsOnly":2};var igtbl_filterComparisionOperator={"All":0,"Empty":1,"NotEmpty":2,"Equals":3,"NotEquals":4,"Like":5,"NotLike":6,"LessThan":7,"LessThanOrEqualTo":8,"GreaterThan":9,"GreaterThanOrEqualTo":10,"StartsWith":11,"DoesNotStartWith":12,"EndsWith":13,"DoesNotEndWith":14,"Contains":15,"DoesNotContain":16};var igtbl_dataType={"Int16":2,"Int32":3,"Single":4,"Double":5,"DateTime":7,"String":8,"Boolean":11,"Object":12,"Decimal":14,"Byte":16,"SByte":17,"UInt16":18,"UInt32":19,"Int64":20,"UInt64":21,"Char":22};var igtbl_CellTitleMode=new Object();igtbl_CellTitleMode.NotSet=0;igtbl_CellTitleMode.Always=1;igtbl_CellTitleMode.OnOverflow=2;igtbl_CellTitleMode.Never=3;var igtbl_ClipboardError={"Failure":-1,"Ok":0,"NotSupported":1,"NoActiveObject":2,"NothingToPaste":3,"NothingToCopy":4};var igtbl_ClipboardOperation={"Copy":0,"Cut":1,"Paste":2};var igtbl_cellButtonDisplay={"OnMouseEnter":0,"Always":1};igtbl_Events.prototype=new igtbl_WebObject();igtbl_Events.prototype.constructor=igtbl_Events;igtbl_Events.base=igtbl_WebObject.prototype;function igtbl_Events(grid,eventsInitArray)
{if(arguments.length>0)
this.init(grid,eventsInitArray);}
var igtbl_ptsEvents=["init",function(grid,eventsInitArray)
{igtbl_Events.base.init.apply(this,["events",null,null]);this._defaultProps=new Array("AfterCellUpdate","AfterColumnMove","AfterColumnSizeChange","AfterEnterEditMode","AfterExitEditMode","AfterRowActivate","AfterRowCollapsed","AfterRowDeleted","AfterRowTemplateClose","AfterRowTemplateOpen","AfterRowExpanded","AfterRowInsert","AfterRowSizeChange","AfterSelectChange","AfterSortColumn","BeforeCellChange","BeforeCellUpdate","BeforeColumnMove","BeforeColumnSizeChange","BeforeEnterEditMode","BeforeExitEditMode","BeforeRowActivate","BeforeRowCollapsed","BeforeRowDeleted","BeforeRowTemplateClose","BeforeRowTemplateOpen","BeforeRowExpanded","BeforeRowInsert","BeforeRowSizeChange","BeforeSelectChange","BeforeSortColumn","ClickCellButton","CellChange","CellClick","ColumnDrag","ColumnHeaderClick","DblClick","EditKeyDown","EditKeyUp","InitializeLayout","InitializeRow","KeyDown","KeyUp","MouseDown","MouseOver","MouseOut","MouseUp","RowSelectorClick","TemplateUpdateCells","TemplateUpdateControls","ValueListSelChange","BeforeRowUpdate","AfterRowUpdate","BeforeXmlHttpRequest","AfterXmlHttpResponseProcessed","XmlHTTPResponse","XmlVirtualScroll","BeforeFilterDroppedDown","BeforeFilterPopulated","BeforeFilterClosed","AfterFilterDroppedDown","AfterFilterPopulated","AfterFilterClosed","BeforeRowFilterApplied","AfterRowFilterApplied","BeforeRowDeactivate","BeforeClipboardOperation","AfterClipboardOperation","ClipboardError","GridCornerImageClick");var eventsArray;eventsArray=eventsInitArray;if(eventsArray)
for(var i=0;i<eventsArray.length;i++)
this[this._defaultProps[i]]=eventsArray[i];},"unload",function()
{for(var i=0;i<this._defaultProps.length;i++)
this[this._defaultProps[i]]=null;igtbl_dispose(this._defaultProps);}];for(var i=0;i<igtbl_ptsEvents.length;i+=2)
igtbl_Events.prototype[igtbl_ptsEvents[i]]=igtbl_ptsEvents[i+1];function igtbl_fireEvent(gn,eventObj,eventString)
{var gs=igtbl_getGridById(gn);if(!gs||!gs.isLoaded())return;var result=false;if(eventObj[0]!="")
{try
{if(typeof(eval(eventObj[0]))!="function")
throw"Event handler does not exist.";}
catch(ex)
{alert("There is a problem with the event handler method: '"+eventObj[0]+"'. Please check the method name's spelling.")
return false;}
result=eval(eventObj[0]+eventString);}
if(gs.GridIsLoaded&&result!=true&&eventObj[1]>=1&&!gs.CancelPostBack)
igtbl_needPostBack(gn);gs.CancelPostBack=false;return result;}
function igtbl_cancelEvent(evnt)
{ig_cancelEvent(evnt);return false;}
igtbl_FilterDropDown.prototype=new igtbl_WebObject();igtbl_FilterDropDown.prototype.constructor=igtbl_FilterDropDown;igtbl_FilterDropDown.base=igtbl_WebObject.prototype;function igtbl_FilterDropDown(column)
{if(column!=null)
{var divElem,grid=column.Band.Grid;divElem=document.createElement("DIV");divElem.style.zIndex=grid._getZ(10000,1);divElem.style.position="absolute";divElem.setAttribute("filter",1);divElem.setAttribute("bandNo",column.Band.Index);divElem.className=column.Band.FilterDropDownStyle;if(divElem.className.length==0)
divElem.className=grid.FilterDropDownStyle;divElem.id=column.Id+"_Filter";var mainGrid=grid.MainGrid;document.body.insertBefore(divElem,document.body.firstChild);this.init(divElem,column);divElem.style.display="none";}}
var igtbl_ptsFilterDropDown=["init",function(element,column)
{igtbl_FilterDropDown.base.init.apply(this,["filterDropDown",element,null]);this.Column=column;this.RowIsland=null;this.Element.object=this;this._evaluationValue=null;this._operator=igtbl_filterComparisionOperator.Equals;this._activeFilter=false;},"getHighlightStyle",function()
{var b=this.Column.Band;if(b.FilterHighlightRowStyle&&b.FilterHighlightRowStyle.length>0)
return b.FilterHighlightRowStyle;return b.Grid.FilterHighlightRowStyle;},"IsActive",function()
{return this._activeFilter;},"setFilter",function(operand,value,serverSet)
{this._operator=operand;this._evaluationValue=value;this._activeFilter=true;var rowIsland=this.RowIsland;var parentRowId;if(rowIsland&&rowIsland.ParentRow)
{parentRowId=rowIsland.ParentRow.getLevel(true)+"\x01"+rowIsland.ParentRow.DataKey;}
else
{parentRowId="\x01";}
var col=this.Column;var g=col.Band.Grid;g._removeFilterChange(col,parentRowId);g._recordChange("FilterColumn",col,operand+"\x01"+igtbl_escape(value)+"\x01"+parentRowId+"\x01"+(serverSet?"server":"client"));},"setOperator",function(op)
{this._operator=op;},"getOperator",function()
{return this._operator;},"setEvaluationValue",function(op)
{this._evaluationValue=op;},"getEvaluationValue",function()
{return this._evaluationValue;},"getWorkingFilterList",function(){return this._currentWorkingList;},"setWorkingFilterList",function(oList){this._currentWorkingList=oList;},"_setFilter",function(value)
{var band=this.Column.Band;switch(value)
{case(band.Filter_AllString):{this.setFilter(igtbl_filterComparisionOperator.All,value);break;}
case(band.Filter_EmptyString):{this.setFilter(igtbl_filterComparisionOperator.Empty,value);break;}
case(band.Filter_NonEmptyString):{this.setFilter(igtbl_filterComparisionOperator.NotEmpty,value);break;}
default:{this.setFilter(igtbl_filterComparisionOperator.Equals,value);break;}}
var grid=band.Grid;return grid.fireEvent(grid.Events.BeforeRowFilterApplied,[grid.Id,this.Column]);},"applyFilter",function()
{var col=this.Column;var g=col.Band.Grid;if(g.LoadOnDemand==3&&!col.Band.IsGrouped)
{g.invokeXmlHttpRequest(g.eReqType.Filter,col,(col.RowFilterMode==1&&col.Band.FilterUIType!=1?null:this.RowIsland));return;}
if(this.Column.AllowRowFiltering==3)
{igtbl_doPostBack(g.Id);return;}
if(col.RowFilterMode==1)
{col._filterOnBand(col.Band.Index,col.Band.Grid.Rows);}
else
{col._filterOnRowIsland(this.RowIsland);}},"_showFillingList",function(col,workingList)
{if(typeof(workingList)=="undefined")
workingList=this._currentWorkingList;var resultList;if(col.RowFilterMode==1)
{resultList=col._getFilterValuesFromBand();}
else
{resultList=col._getFilterValuesFromSiblings();}
resultList=this._cleanList(resultList);workingList=workingList.concat(resultList);col.Band.Grid._hidePI();this._afterFilterFilled(col.Band.Grid,col,workingList);return workingList;},"_afterFilterFilled",function(grid,col,workingList)
{this._currentWorkingList=workingList;grid.fireEvent(grid.Events.AfterFilterPopulated,[grid.Id,this,workingList]);var filterTableElem=this._buildFilterTable(workingList,this);this._lastWorkingList=workingList;this._filterTable=filterTableElem;this._showFilter(filterTableElem);col.Band.Grid._currentFilterShowing=filterTableElem;grid.fireEvent(grid.Events.AfterFilterDroppedDown,[grid.Id,this]);this.IsDropped=true;this.Element.style.display="";this.Column.Band.Grid._currentFilterDropped=this;},"show",function(show)
{var col=this.Column;var grid=col.Band.Grid;if(show)
{if(this.IsDropped)return;for(var gridId in igtbl_gridState)
{var g=igtbl_getGridById(gridId);if(g._currentFilterDropped)
g._currentFilterDropped.show(false);}
if(grid.fireEvent(grid.Events.BeforeFilterDroppedDown,[grid.Id,this])==true)
{return true;}
var workingList=new Array();for(var iList=0;iList<col.DefaultFilterList.length;iList++)
{workingList.push(col.DefaultFilterList[iList]);}
this._currentWorkingList=workingList;if(this.Column.GatherFilterData==2||grid.fireEvent(grid.Events.BeforeFilterPopulated,[grid.Id,this,this.Column,this._currentWorkingList,this._lastWorkingList])==true)
{workingList=this._currentWorkingList;this._afterFilterFilled(grid,col,workingList);}
else
{if(grid.LoadOnDemand==3)
{grid.invokeXmlHttpRequest(grid.eReqType.FilterDropDownFill,col,(col.RowFilterMode==1?null:col._getSiblingRowIsland()));return;}
if(grid.EnableProgressIndicator)
{grid._displayPI();if((col.Band.Index>0&&this.RowIsland)||(col.Band.IsGrouped&&col.Band.RowFilterMode==2))
{col._filterSrcElement=grid.event.srcElement;if(!col._filterSrcElement)
col._filterSrcElement=grid.event.target;setTimeout("igtbl_getGridById('"+grid.Id+"').Bands["+col.Band.Index+"]._filterPanels['"+this.RowIsland.Element.parentNode.id+"']['"+col.Id+"']._showFillingList(igtbl_getColumnById('"+col.Id+"'));");}
else
setTimeout("igtbl_getGridById('"+grid.Id+"').Bands["+col.Band.Index+"]._filterPanels['"+col.Id+"']._showFillingList(igtbl_getColumnById('"+col.Id+"'));");}
else
this._showFillingList(col);}}
else
{if(grid.fireEvent(grid.Events.BeforeFilterClosed,[grid.Id,this])==true)
{return true;}
this.IsDropped=false;if(col.Band.transPanel)col.Band.transPanel.hide();this.Element.style.display="none";grid._currentFilterDropped=null;grid.fireEvent(grid.Events.AfterFilterClosed,[grid.Id,this]);}},"_cleanList",function(workingList)
{if(workingList==null||workingList.length<2)return workingList;var currentValue=workingList[workingList.length-1];for(var itr=workingList.length-2;itr>-1;itr--)
{if(currentValue[1]===workingList[itr][1])
{workingList.splice(itr,1);}
else
{currentValue=workingList[itr];}}
return workingList;},"_buildFilterTable",function(workingList,filterObject)
{var divElem;if(workingList===this._lastWorkingList&&this._filterTable)
{divElem=this._filterTable;}
else
{divElem=this.Element;divElem.style.overflow="auto";if(divElem.childNodes.length>0)
{for(var itr=divElem.childNodes.length-1;itr>=0;itr--)
divElem.removeChild(divElem.childNodes[itr]);}
var elem=document.createElement("TABLE");elem.cellSpacing=0;elem.className=divElem.className;elem.style.borderStyle="none";elem.style.borderWidth="0px";ig_csom.addEventListener(elem,"mouseup",igtbl_filterMouseUp);ig_csom.addEventListener(elem,"mouseover",igtbl_filterMouseOver);ig_csom.addEventListener(elem,"mouseout",igtbl_filterMouseOut);ig_csom.addEventListener(elem,"selectstart",ig_cancelEvent);ig_csom.addEventListener(document,"mouseup",igtbl_filterMouseUpDocument);var gridDiv=this.Column.Band.Grid.getDivElement()
ig_csom.addEventListener(gridDiv,"scroll",igtbl_filterGridScroll);elem._filterObject=filterObject;var colGroup=document.createElement("COLGROUP");var tbody=document.createElement("TBODY");elem.appendChild(colGroup);elem.appendChild(tbody);divElem.appendChild(elem);var column=document.createElement("COL");column.style.width="100%";colGroup.appendChild(column);for(var itr=0;itr<workingList.length;itr++)
{var row=document.createElement("TR");ig_csom.addEventListener(row,"mouseup",igtbl_filterOptionMouseUp);row.setAttribute("fo",1);row.style.height=this.Column.Band.DefaultRowHeight;row.setAttribute("value",workingList[itr][0]);var cell=document.createElement("TD");var cellText=document.createTextNode(workingList[itr][1]);cell.appendChild(cellText);row.appendChild(cell);tbody.appendChild(row);}}
return divElem;},"_showFilter",function(filterDivElem)
{var band=this.Column.Band;var gridObj=band.Grid;var tPan=band.transPanel;if(tPan==null&&ig_csom.IsIEWin)
{band.transPanel=tPan=ig_csom.createTransparentPanel();if(tPan)
{filterDivElem.parentNode.insertBefore(tPan.Element,filterDivElem);tPan.Element.style.zIndex=igtbl_parseInt(filterDivElem.style.zIndex)-1;}}
var fc=this.Column.Element;if(!fc||!fc.offsetHeight)
{try
{fc=gridObj.event.srcElement;if(!fc)
{fc=gridObj.event.target;}}
catch(excep)
{}
if(!fc)
{var colHeaderIndex=0;var headerTags=this.Column._getHeadTags();if(this.RowIsland&&this.RowIsland.Element&&!(band.Index==0&&gridObj.StatHeader))
{var parentTable=this.RowIsland.Element;do
{parentTable=parentTable.parentNode;}while(parentTable&&!(parentTable.tagName=="TABLE"&&parentTable.id.length>0))
if(parentTable)
{for(var itr=0;itr<headerTags.length;itr++)
{var parTable=headerTags[itr];do
{parTable=parTable.parentNode;}
while(parTable&&!(parTable.tagName=="TABLE"&&parTable.id.length>0))
if(parTable.id==parentTable.id)
{colHeaderIndex=itr;}}}}
fc=headerTags[colHeaderIndex];}
while(fc.tagName!="TH")
{fc=fc.parentNode;}}
filterDivElem.style.display="";if(band.FilterDropDownRowCount>0)
{var rows=filterDivElem.childNodes[0].childNodes[1].childNodes;if(rows.length>0)
{var calcValue=(rows.length<band.FilterDropDownRowCount)?rows.length:band.FilterDropDownRowCount;var calcHeight=(rows[0].clientHeight*calcValue);var scrollBarHeight=filterDivElem.offsetHeight-filterDivElem.clientHeight;filterDivElem.style.height=(calcHeight+scrollBarHeight)+"px";}}
if(!filterDivElem.getAttribute("adjusted")&&filterDivElem.offsetWidth-filterDivElem.clientWidth>10&&filterDivElem.offsetWidth-filterDivElem.clientWidth<30)
{filterDivElem.style.width=(filterDivElem.offsetWidth+(filterDivElem.offsetWidth-filterDivElem.clientWidth)).toString()+"px";filterDivElem.setAttribute("adjusted","true");}
ig_csom.absPosition(fc,filterDivElem,ig_Location.BelowLeft,tPan);if(gridObj.StatHeader&&gridObj.Rows.length<1&&this.Column.Element!=null&&this.Column.Element.parentElement!=null)
{var offs=filterDivElem.style.posTop+this.Column.Element.parentElement.clientHeight;filterDivElem.style.top=offs+"px";band.transPanel.Element.style.top=(offs-1)+"px";}
this._filterPanel=filterDivElem;}]
for(var i=0;i<igtbl_ptsFilterDropDown.length;i+=2)
igtbl_FilterDropDown.prototype[igtbl_ptsFilterDropDown[i]]=igtbl_ptsFilterDropDown[i+1];igtbl_FilterIconsList.prototype=new igtbl_WebObject();igtbl_FilterIconsList.prototype.constructor=igtbl_FilterIconsList;igtbl_FilterIconsList.base=igtbl_WebObject.prototype;function igtbl_FilterIconsList(column)
{if(column!=null)
{var divElem,grid=column.Band.Grid;divElem=document.createElement("DIV");divElem.style.zIndex=grid._getZ(10000,1);divElem.style.position="absolute";divElem.setAttribute("filterIconList",1);divElem.setAttribute("bandNo",column.Band.Index);divElem.id=column.Id+"_FilterIconList";var mainGrid=grid.MainGrid;document.body.insertBefore(divElem,document.body.firstChild);this.init(divElem,column);divElem.style.display="none";}}
var igtbl_ptsFilterIconDropDown=["init",function(element,column)
{igtbl_FilterDropDown.base.init.apply(this,["filterIconDropDown",element,null]);this.Column=column;this.Element.object=this;var gridDiv=this.Column.Band.Grid.getDivElement()
ig_csom.addEventListener(gridDiv,"scroll",igtbl_filterGridScroll);var divElem=element;var g=column.Band.Grid;var elem=document.createElement("TABLE");elem.cellSpacing=0;elem.className=g.FilterOperandDropDownStyle+" "+column.Band.FilterOperandDropDownStyle;ig_csom.addEventListener(elem,"selectstart",ig_cancelEvent);ig_csom.addEventListener(elem,"mouseup",igtbl_filterIconsMouseUp);ig_csom.addEventListener(elem,"mouseover",igtbl_filterMouseOver);ig_csom.addEventListener(elem,"mouseout",igtbl_filterMouseOut);ig_csom.addEventListener(document,"mouseup",igtbl_filterMouseUpDocument);var colGroup=document.createElement("COLGROUP");var tbody=document.createElement("TBODY");elem.appendChild(colGroup);elem.appendChild(tbody);divElem.appendChild(elem);var column=document.createElement("COL");column.style.width="100%";colGroup.appendChild(column);for(var itr=0;itr<g.FilterButtonImages.length;itr++)
{var filterImageObj=g.FilterButtonImages[itr];var row=document.createElement("TR");var cell=document.createElement("TD");var div=document.createElement("DIV");cell.appendChild(div);var img=document.createElement("IMG");img.src=filterImageObj[1];img.title=filterImageObj[2];img.style.verticalAlign="middle";img.setAttribute("operator",filterImageObj[0]);div.appendChild(img);cell.className=g.FilterOperandItemStyle+" "+this.Column.Band.FilterOperandItemStyle;row.appendChild(cell);var cellText=document.createTextNode(this.Column.Band.FilterOperandStrings[itr]);div.appendChild(cellText);tbody.appendChild(row);row.setAttribute("filterListOption","true");row.setAttribute("operator",filterImageObj[0]);}},"show",function(cell,force)
{if(force&&this.IsDropped)return;var col=this.Column;var band=col.Band;var grid=band.Grid;if(cell)
{if(grid.fireEvent(grid.Events.BeforeFilterDroppedDown,[grid.Id,this])==true)
{return true;}
for(var gridId in igtbl_gridState)
{var g=igtbl_getGridById(gridId);if(g._currentFilterDropped)
g._currentFilterDropped.show(false);}
this._showListOption(igtbl_filterComparisionOperator.Like,col.DataType==8);this._showListOption(igtbl_filterComparisionOperator.NotLike,col.DataType==8);this._showListOption(igtbl_filterComparisionOperator.Contains,col.DataType==8);this._showListOption(igtbl_filterComparisionOperator.DoesNotContain,col.DataType==8);this._showListOption(igtbl_filterComparisionOperator.StartsWith,col.DataType==8);this._showListOption(igtbl_filterComparisionOperator.DoesNotStartWith,col.DataType==8);this._showListOption(igtbl_filterComparisionOperator.EndsWith,col.DataType==8);this._showListOption(igtbl_filterComparisionOperator.DoesNotEndWith,col.DataType==8);this._showDropDown(this.Element);this.IsDropped=true;grid.fireEvent(grid.Events.AfterFilterDroppedDown,[grid.Id,this]);this._currentCell=cell;grid._currentFilterDropped=this;}
else
{this.Element.style.display="none";this.IsDropped=false;if(band.transPanel)
band.transPanel.hide();grid._currentFilterDropped=null;this._currentCell=null;}},"_showDropDown",function(filterDivElem)
{var band=this.Column.Band;var gridObj=band.Grid;var tPan=band.transPanel;if(tPan==null&&ig_csom.IsIEWin)
{band.transPanel=tPan=ig_csom.createTransparentPanel();if(tPan)
{filterDivElem.parentNode.insertBefore(tPan.Element,filterDivElem);tPan.Element.style.zIndex=igtbl_parseInt(filterDivElem.style.zIndex)-1;}}
var fc;try
{fc=gridObj.event.srcElement;if(!fc)
{fc=gridObj.event.target;}}
catch(excep)
{}
filterDivElem.style.display="";if(!filterDivElem.getAttribute("adjusted")&&filterDivElem.offsetWidth-filterDivElem.clientWidth>10&&filterDivElem.offsetWidth-filterDivElem.clientWidth<30)
{filterDivElem.style.width=(filterDivElem.offsetWidth+(filterDivElem.offsetWidth-filterDivElem.clientWidth)).toString()+"px";filterDivElem.setAttribute("adjusted","true");}
ig_csom.absPosition(fc,filterDivElem,ig_Location.BelowLeft,tPan);filterDivElem.CurrentDropSource=this;},"getHighlightStyle",function()
{var b=this.Column.Band;return b.Grid.FilterOperandItemHoverStyle+" "+b.FilterOperandItemHoverStyle;},"_showListOption",function(option,show)
{var e=this.Element.childNodes[0].childNodes[1].childNodes;for(var itr=0;itr<e.length;itr++)
{var node=e[itr];if(node.getAttribute("operator")==option)
{node.style.display=(show?"":"none");}}}]
for(var i=0;i<igtbl_ptsFilterIconDropDown.length;i+=2)
igtbl_FilterIconsList.prototype[igtbl_ptsFilterIconDropDown[i]]=igtbl_ptsFilterIconDropDown[i+1];var igtbl_waitDiv=null;var igtbl_wndOldCursor=null;var igtbl_oldMouseDown=null;var igtbl_currentEditTempl=null;var igtbl_focusedElement=null;igtbl_Grid.prototype=new igtbl_WebObject();igtbl_Grid.prototype.constructor=igtbl_Grid;igtbl_Grid.base=igtbl_WebObject.prototype;function igtbl_Grid(element,node,gridInitArray,bandsInitArray,colsInitArray,eventsInitArray,xmlInitProps)
{if(arguments.length>0)
this.init(element,node,gridInitArray,bandsInitArray,colsInitArray,eventsInitArray,xmlInitProps);}
var igtbl_ptsGrid=["init",function(element,node,gridInitArray,bandsInitArray,colsInitArray,eventsInitArray,xmlInitProps)
{igtbl_Grid.base.init.apply(this,["grid",element,node]);this.IsXHTML=igtbl_isXHTML;if(node)
{this.XmlNS="";this.Xml=node;this.Node=this.Xml.selectSingleNode("UltraWebGrid/UltraGridLayout");}
this.ViewState=ig_ClientState.addNode(ig_ClientState.createRootNode(),"UltraWebGrid");this.ViewState=ig_ClientState.addNode(this.ViewState,"DisplayLayout");this.StateChanges=ig_ClientState.addNode(this.ViewState,"StateChanges");this.Id=this.Id.substr(2);this._Changes=new Array();this.SelectedRows=new Object();this.SelectedColumns=new Object();this.SelectedCells=new Object();this.SelectedCellsRows=new Object();this.ExpandedRows=new Object();this.CollapsedRows=new Object();this.ResizedColumns=new Object();this.ResizedRows=new Object();this.ChangedRows=new Object();this.ChangedCells=new Object();this.AddedRows=new Object();this.DeletedRows=new Object();this.ActiveCell="";this.ActiveRow="";this.grid=this;this.activeRect=null;this.SuspendUpdates=false;this._lastSelectedRow="";this.ScrollPos=0;this.currentTriImg=null;this.newImg=null;this.NeedPostBack=false;this.CancelPostBack=false;this.GridIsLoaded=false;this._exitEditCancel=false;this._noCellChange=false;this._insideSetActive=false;this.MainGrid=igtbl_getElementById(this.Id+"_main");this.DivElement=igtbl_getElementById(this.Id+"_div");var defaultProps=new Array("AddNewBoxVisible","AddNewBoxView","AllowAddNew","AllowColSizing","AllowDelete","AllowSort","ItemClass","AltClass","AllowUpdate","CellClickAction","EditCellClass","Expandable","FooterClass","GroupByRowClass","GroupCount","HeaderClass","HeaderClickAction","Indentation","NullText","ExpAreaClass","RowLabelClass","SelGroupByRowClass","SelHeadClass","SelCellClass","RowSizing","SelectTypeCell","SelectTypeColumn","SelectTypeRow","ShowBandLabels","ViewType","AllowPaging","PageCount","CurrentPageIndex","PageSize","CollapseImage","ExpandImage","CurrentRowImage","CurrentEditRowImage","NewRowImage","BlankImage","SortAscImg","SortDscImg","Activation","cultureInfo","RowSelectors","UniqueID","StationaryMargins","LoadOnDemand","RowLabelBlankImage","EIRM","TabDirection","ClientID","DefaultCentury","UseFixedHeaders","FixedHeaderIndicator","FixedHeaderOnImage","FixedHeaderOffImage","StopperStyle","FixedColumnScrollType","TableLayout","AllowRowNumbering","ClientSideRenumbering","XmlLoadOnDemandType","Section508Compliant","_rowToolTipFormatStr","_childRowToolTipFormatStr","FilterDropDownStyle","FilterHighlightRowStyle","FilterDefaultImage","FilterAppliedImage","ImageDirectory","_progressIndicatorImage","EnableProgressIndicator","CellTitleMode","HeaderTitleMode","SortAscAltText","SortDescAltText","ExpandAltText","CollapseAltText","_currentRowAltText","_currentEditRowAltText","_fixedHeaderOnAltText","_fixedHeaderOffAltText","_newRowAltText","GridCornerImage","UrlExecutionPath","ActivationObjectClassTB","ActivationObjectClassL","ActivationObjectClassR","FilterButtonImages","FilterRowStyle","FilterOperandDropDownStyle","FilterOperandItemStyle","FilterOperandItemHoverStyle","FilterOperandButtonStyle","SortingAlgorithm");this.Bands=new Array();var props;props=gridInitArray;if(props)
{for(var i=0;i<defaultProps.length;i++)
this[defaultProps[i]]=props[i];this.Activation=new igtbl_initActivation(this.Activation);this.Activation._cssClass=this.ActivationObjectClassTB;this.Activation._cssClassL=this.ActivationObjectClassL;this.Activation._cssClassR=this.ActivationObjectClassR;this.cultureInfo=this.cultureInfo.split("|");}
if(this.UseFixedHeaders||this.XmlLoadOnDemandType!=0&&this.XmlLoadOnDemandType!=4)
{this._scrElem=this.Element.parentNode.previousSibling;this._tdContainer=this._scrElem.parentNode.parentNode;}
else
this._tdContainer=this.Element.parentNode.parentNode;var xmlProps=xmlInitProps;this._AddnlProps=xmlProps;this.RowsServerLength=xmlProps[0];this.RowsRange=xmlProps[1];this.RowsRetrieved=xmlProps[2];if(this.XmlLoadOnDemandType==2&&this.RowsRetrieved>this.RowsRange)
this._recordChange("RowToStart",this,this.RowsRetrieved-this.RowsRange);if(!node)
{var bandsArray=bandsInitArray;var bandCount=bandsArray.length;for(var i=0;i<bandCount;i++)
this.Bands[i]=new igtbl_Band(this,null,i,bandsInitArray,colsInitArray);}
else
{this.Bands.Node=this.Xml.selectSingleNode("UltraWebGrid/Bands");var bandNodes=this.Bands.Node.selectNodes("Band");for(var i=0;i<bandNodes.length;i++)
this.Bands[i]=new igtbl_Band(this,bandNodes[i],i,bandsInitArray,colsInitArray);}
igtbl_dispose(defaultProps);igtbl_gridState[this.Id]=this;this.Events=new igtbl_Events(this,eventsInitArray);this.Rows=new igtbl_Rows((this.Node?this.Xml.selectSingleNode("UltraWebGrid/Rs"):null),this.Bands[0],null);this.Rows._getRowToStart=function()
{if(this.Grid.XmlLoadOnDemandType==2)
{var topRowNo=Math.floor(this.Grid.getDivElement().scrollTop/this.Grid.getDefaultRowHeight());return topRowNo;}
return 0;};if(this.Bands&&!this.Bands[0].IsGrouped&&this.StationaryMargins!=1&&this.StationaryMargins!=3)
{igtbl_assignColumnElements(this.Rows.Element.previousSibling,this.Bands[0]);}
this.regExp=null;this.backwardSearch=false;this.lastSearchedCell=null;this.lastSortedColumn="";if(this.AllowRowNumbering==2)this.CurrentRowNumber=0;this.GroupByBox=new igtbl_initGroupByBox(this);this.eReqType=igtbl_reqType;this.eReadyState=igtbl_readyState;this.eError=igtbl_error;this.eFilterComparisionOperator=igtbl_filterComparisionOperator;this.eRowFilterMode=igtbl_RowFilterMode;this.eFeatureRowView=igtbl_featureRowView;this.eFilterComparisonType=igtbl_filterComparisonType;this.eClipboardOperation=igtbl_ClipboardOperation;this.eClipboardError=igtbl_ClipboardError;if(this.Node||!ig_csom.IsIE&&this.LoadOnDemand==3)
{this.ReqType=this.eReqType.None;this.ReadyState=this.eReadyState.Ready;this.Error=this.eError.Ok;this._innerObj=document.createElement("div");this.QueryString="";this.XslProcessor=new igtbl_XSLTProcessor(this._AddnlProps[11]);if(ig_csom.IsIE)
this.XmlResp=ig_createActiveXFromProgIDs(["MSXML2.DOMDocument","Microsoft.XMLDOM"]);else
this.DOMParser=new DOMParser();if(node)
this.Rows.render();}
if(xmlProps[14])
{_igtbl_processServerPassedColumnFilters(xmlProps[14],this)}
this._progressIndicator=new ig_progressIndicator(this.ImageDirectory+this._progressIndicatorImage,this.MainGrid);this._progressIndicator.setLocation(ig_Location.MiddleCenter);if(this.Bands[0].ColHeadersVisible!=2&&(this.StationaryMargins==1||this.StationaryMargins==3)&&igtbl_getElementById(this.Id+"_hdiv"))
this.StatHeader=new igtbl_initStatHeader(this);if(this.Bands[0].ColFootersVisible==1&&(this.StationaryMargins==2||this.StationaryMargins==3)&&igtbl_getElementById(this.Id+"_fdiv"))
this.StatFooter=new igtbl_initStatFooter(this);this._calculateStationaryHeader();this.VirtualScrollDelay=500;if(this.XmlLoadOnDemandType==3)
window.setTimeout("_igtbl_getMoreRows('"+this.Id+"');",100);if(typeof(igtbl_oldGlobalMouseMove)=="undefined")
{igtbl_oldGlobalMouseMove=igtbl_addEventListener(document.body,"mousemove",igtbl_globalMouseMove);}
var thisForm=igtbl_getThisForm(this.Element);if(thisForm)
{this._thisForm=thisForm;if(thisForm.igtblGrid&&thisForm.igtblGrid.Id!=this.Id)
this.oldIgtblGrid=thisForm.igtblGrid;else
{if(thisForm.addEventListener&&!igtbl_isAtlas)
{thisForm.addEventListener('submit',igtbl_submit,false);}
else if(typeof(thisForm.igtbl_oldOnSubmit)=="undefined"||thisForm.igtbl_oldOnSubmit==null)
{thisForm.igtbl_oldOnSubmit=thisForm.onsubmit;thisForm.onsubmit=igtbl_submit;}
if(typeof(MSOLayout_RemoveWebPart)=="undefined"&&(typeof(thisForm.igtbl_oldSubmit)=="undefined"||thisForm.igtbl_oldSubmit==null))
{thisForm.igtbl_oldSubmit=thisForm.submit;thisForm.submit=igtbl_formSubmit;}
if(typeof(window._igtbl_doPostBackOld)=="undefined"||window._igtbl_doPostBackOld==null)
{window._igtbl_doPostBackOld=window.__doPostBack;window.__doPostBack=igtbl_submit;}
window._igtbl_thisForm=thisForm;}
thisForm.igtblGrid=this;}
this.SortImplementation=null;this._initFF();},"_initFF",function()
{if(!ig_csom.IsFireFox||this._initedFF)
return;var height=0;var mr=document.getElementById(this.Id+"_mr");var mc=document.getElementById(this.Id+"_mc");var gbr=document.getElementById(this.Id+"_gbr");var hdiv=document.getElementById(this.Id+"_hdiv");if(gbr)height+=gbr.offsetHeight;if(hdiv)height+=hdiv.offsetHeight;if(height==0)
return;var gridMain=document.getElementById(this.Id+"_mtb");if(gridMain)
{for(i=0;i<gridMain.childNodes.length;i++)
{var e=gridMain.childNodes[i];if(e.childNodes[0]&&e.childNodes[0].id==this.Id+"_pager")
height+=e.childNodes[0].offsetHeight;}}
var gridHeighInPX=this.MainGrid.style.height.indexOf("%")==-1;if(mr&&mc&&gridHeighInPX)
{mr.style.height=(mr.offsetHeight-height)+'px';mc.style.height=(mc.offsetHeight-height)+'px';}
this._initedFF=true;},"_getZ",function(z,more)
{var elem=this.MainGrid;while(elem)
{if(elem.nodeName=='BODY'||elem.nodeName=='FORM')
break;var zi=ig_shared.getStyleValue(null,'zIndex',elem);if(zi&&zi.substring)
zi=(zi.length>3&&zi.charCodeAt(0)<58)?parseInt(zi):0;if(zi&&zi>=z)
z=zi+1+(more?1:0);elem=elem.parentNode;}
return z;},"sortColumn",function(colId,shiftKey)
{var bandNo=igtbl_bandNoFromColId(colId);var band=this.Bands[bandNo];var colNo=igtbl_colNoFromColId(colId);if(band.Columns[colNo].SortIndicator==3)
return;var headClk=igtbl_getHeaderClickAction(this.Id,bandNo,colNo);if(headClk==2||headClk==3)
{var gs=igtbl_getGridById(this.Id);if(!band.ClientSortEnabled)
gs.NeedPostBack=true;var eventCanceled=igtbl_fireEvent(this.Id,this.Events.BeforeSortColumn,"(\""+this.Id+"\",\""+colId+"\")");if(eventCanceled&&band.ClientSortEnabled)
return;if(!eventCanceled)
this.addSortColumn(colId,(headClk==2||!shiftKey));else
gs.NeedPostBack=false;if(!eventCanceled&&band.ClientSortEnabled)
{var el=igtbl_getDocumentElement(colId);if(!el.length&&el.tagName=="TH"&&el.getAttribute("groupInfo"))
igtbl_sortGroupedRows(this.Rows,bandNo,colId);else
{if(!el.length)
{el=new Array();el[0]=igtbl_getElementById(colId);}
for(var i=0;i<el.length;i++)
{var rows=el[i].parentNode;while(rows&&(rows.tagName!="TABLE"||(rows.tagName=="TABLE"&&rows.id=="")))rows=rows.parentNode;if(rows&&rows.tBodies[0])rows=rows.tBodies[0];if(!rows||!rows.Object)continue;rows.Object.sort();}}
gs._recalcRowNumbers();igtbl_hideEdit(this.Id);igtbl_fireEvent(this.Id,this.Events.AfterSortColumn,"(\""+this.Id+"\",\""+colId+"\")");}}},"addSortColumn",function(colId,clear)
{var colAr=colId.split(";");if(colAr.length>1)
{for(var i=0;i<colAr.length;i++)
if(colAr[i]!="")
{var band=this.Bands[igtbl_bandNoFromColId(colAr[i])];band.SortedColumns[band.SortedColumns.length]=colAr[i];var colObj=igtbl_getColumnById(colAr[i]);var colNo=igtbl_colNoFromColId(colAr[i]);var bandNo=band.Index;if(colObj.IsGroupBy)
{var postString="group:"+bandNo+":"+colNo+":false:band:"+bandNo;this._recordChange("ColumnGroup",band.Columns[colNo],postString);colObj._Changes["ColumnGroup"].setFireEvent(false);}
else
this._recordChange("SortedColumns",band.Columns[colNo],"false"+":"+band.Columns[colNo].SortIndicator);}}
else
{var band=this.Bands[igtbl_bandNoFromColId(colId)];var colNo=igtbl_colNoFromColId(colId);if(band.Columns[colNo].SortIndicator==3)
return;if(clear)
{var scLen=band.SortedColumns.length;for(var i=scLen-1;i>=0;i--)
{var cn=igtbl_colNoFromColId(band.SortedColumns[i]);if(cn!=colNo&&band.Columns[cn].SortIndicator!=3&&!band.Columns[cn].IsGroupBy)
{band.Columns[cn].SortIndicator=0;if(band.ClientSortEnabled)
{var colEl=igtbl_getDocumentElement(band.SortedColumns[i]);if(colEl)
{if(!colEl.length)
colEl=[colEl];for(var j=0;j<colEl.length;j++)
{var img=null;var el=colEl[j];for(var x=0;x<el.childNodes.length;x++)
{if(el.childNodes[x].tagName=="NOBR")
{el=el.childNodes[x];break;}}
if(el.childNodes.length)
{img=_igtbl_findSortImage(el.childNodes);}
if(img)
el.removeChild(img);}}}}
if(band.Columns[cn].IsGroupBy)
break;band.SortedColumns=band.SortedColumns.slice(0,-1);if(this.LoadOnDemand==3&&this._containsChange("SortedColumns",band.Columns[cn]))
this._removeChange("SortedColumns",band.Columns[cn]);}}
if(band.Columns[colNo].SortIndicator==1)
band.Columns[colNo].SortIndicator=2;else
band.Columns[colNo].SortIndicator=1;if(this.LoadOnDemand==3&&this._containsChange("SortedColumns",band.Columns[colNo]))
{this._removeChange("SortedColumns",band.Columns[colNo]);}
this._recordChange("SortedColumns",band.Columns[colNo],clear.toString()+":"+band.Columns[colNo].SortIndicator);band.Grid.lastSortedColumn=colId;if(band.ClientSortEnabled)
{var colEl=igtbl_getDocumentElement(colId);if(colEl)
{if(!colEl.length)
colEl=[colEl];for(var i=0;i<colEl.length;i++)
{var img=null;var el=colEl[i];if(el.firstChild&&el.firstChild.tagName=="NOBR")
el=el.firstChild;if(el.childNodes.length)
{img=_igtbl_findSortImage(el.childNodes);}
if(img===null)
{img=document.createElement("img");img.border="0";img.height="12";img.width="12";img.setAttribute("imgType","sort");if(!el.getAttribute("charApnd"))
{el.innerHTML+="&nbsp;";el.setAttribute("charApnd",1);}
img.alt=this.SortDescAltText;img.setAttribute("igAltS",this.SortAscAltText);el.appendChild(img);}
var alt=img.getAttribute("alt");if(band.Columns[colNo].SortIndicator==1)
{img.src=this.SortAscImg;if(alt!=null)
{var clpsAlt=img.getAttribute("igAltS");var clpsAlt2=img.getAttribute("igAltUp")
if(clpsAlt!=null)
{img.setAttribute("igAltD",alt);img.setAttribute("alt",clpsAlt);img.removeAttribute("igAltS");}
else if(clpsAlt2!=null)
{img.setAttribute("igAltDn",alt);img.setAttribute("alt",clpsAlt2);img.removeAttribute("igAltUp");}}}
else
{img.src=this.SortDscImg;if(alt!=null)
{var clpsAlt=img.getAttribute("igAltD");var clpsAlt2=img.getAttribute("igAltDn")
if(clpsAlt!=null)
{img.setAttribute("igAltS",alt);img.setAttribute("alt",clpsAlt);img.removeAttribute("igAltD");}
else if(clpsAlt2!=null)
{img.setAttribute("igAltUp",alt);img.setAttribute("alt",clpsAlt2);img.removeAttribute("igAltDn");}}}}}}
if(!band.Columns[colNo].IsGroupBy)
{for(var i=0;i<band.SortedColumns.length;i++)
if(band.SortedColumns[i]==colId)
break;if(i==band.SortedColumns.length)
{band.Columns[colNo].ensureWebCombo();band.SortedColumns[band.SortedColumns.length]=colId;}}}},"getActiveCell",function()
{return this.oActiveCell;},"setActiveCell",function(cell,force)
{if(!this.Activation.AllowActivation||this._insideSetActive)
return;if(!cell||!cell.Element||cell.Element.tagName!="TD")
cell=null;if(!force&&(cell&&this.oActiveCell==cell||this._exitEditCancel))
{this._noCellChange=true;return;}
if(!cell)
{this.ActiveCell="";this.ActiveRow="";var row=this.oActiveRow;cell=this.oActiveCell;if(cell)
row=cell.Row;if(row)
row.setSelectedRowImg(true);if(cell)
cell.renderActive(false);if(this.oActiveRow)
this.oActiveRow.renderActive(false);this.oActiveCell=null;this.oActiveRow=null;if(cell)
this._removeChange("ActiveCell",cell);if(row)
this._removeChange("ActiveRow",row);if(this.AddNewBoxVisible)
this.updateAddNewBox();return;}
var change=true;var oldACell=this.oActiveCell;var oldARow=this.oActiveRow;if(!oldARow&&oldACell)
oldARow=oldACell.Row;this.endEdit();if(this._exitEditCancel||this.fireEvent(this.Events.BeforeCellChange,[this.Id,cell.Element.id])==true)
change=false;if(change&&cell.Row!=oldARow)
{if(oldARow)
{change&=this.fireEvent(this.Events.BeforeRowDeactivate,[this.Id,oldARow.Element.id])!=true;if(oldARow.IsAddNewRow)
oldARow.commit();else
oldARow.processUpdateRow();}
if(this._exitEditCancel||this.fireEvent(this.Events.BeforeRowActivate,[this.Id,cell.Row.Element.id])==true)
change=false;}
if(!change)
{this._noCellChange=true;return;}
this._noCellChange=false;if(this.oActiveCell)
this.oActiveCell.renderActive(false);if(this.oActiveRow)
this.oActiveRow.renderActive(false);this.oActiveCell=cell;this.ActiveCell=cell.Element.id;if(this.oActiveRow)
this._removeChange("ActiveRow",this.oActiveRow);this.oActiveRow=null;this.ActiveRow="";this.oActiveCell.renderActive();if(this.oActiveCell.Row!=oldARow)
this.setNewRowImg(null);this.oActiveCell.Row.setSelectedRowImg();this.colButtonMouseOut();if(this.AddNewBoxVisible)
this.updateAddNewBox();igtbl_activate(this.Id);this._removeChange("ActiveCell",oldACell);this._recordChange("ActiveCell",this.oActiveCell);this.fireEvent(this.Events.CellChange,[this.Id,this.oActiveCell.Element.id]);if(this.oActiveCell&&this.oActiveCell.Row!=oldARow)
this.fireEvent(this.Events.AfterRowActivate,[this.Id,this.oActiveCell.Row.Element.id]);if(cell.Row.IsFilterRow)
this.NeedPostBack=false;},"getActiveRow",function()
{if(this.oActiveRow!=null)
return this.oActiveRow;if(this.oActiveCell!=null)
return this.oActiveCell.Row;return null;},"setActiveRow",function(row,force,fireEvents)
{if(!this.Activation.AllowActivation||this._insideSetActive)
return;if(typeof(fireEvents)=="undefined")
fireEvents=true;if(!row||!row.Element||row.Element.tagName!="TR")
row=null;if(!force&&(row&&this.oActiveRow==row||this._exitEditCancel))
{this._noCellChange=true;return;}
if(!row)
{this.ActiveCell="";this.ActiveRow="";row=this.oActiveRow;var cell=this.oActiveCell;if(cell)
row=cell.Row;if(row)
row.setSelectedRowImg(true);if(cell)
cell.renderActive(false);if(this.oActiveRow)
this.oActiveRow.renderActive(false);this.oActiveCell=null;this.oActiveRow=null;if(cell)
this._removeChange("ActiveCell",cell);this._removeChange("ActiveRow",row);if(this._fromServerActiveRow)
this._recordChange("ActiveRow",this,-1);if(this.AddNewBoxVisible)
this.updateAddNewBox();return;}
var change=true;var oldACell=this.oActiveCell;var oldARow=this.oActiveRow;if(!oldARow&&oldACell)
oldARow=oldACell.Row;this.endEdit();if(fireEvents&&row!=oldARow&&oldARow)
{change&=this.fireEvent(this.Events.BeforeRowDeactivate,[this.Id,oldARow.Element.id])!=true;if(oldARow.IsAddNewRow)
oldARow.commit();else
oldARow.processUpdateRow();}
if(this._exitEditCancel||fireEvents&&this.fireEvent(this.Events.BeforeRowActivate,[this.Id,row.Element.id])==true)
change=false;var cellChanged=this.oActiveCell!=null;if(change&&cellChanged)
change=!this.fireEvent(this.Events.BeforeCellChange,[this.Id,this.oActiveCell.Element.id]);if(!change)
{this._noCellChange=true;return;}
this._noCellChange=false;if(this.oActiveCell)
this.oActiveCell.renderActive(false);if(this.oActiveRow)
this.oActiveRow.renderActive(false);this.oActiveRow=row;this.ActiveRow=row.Element.id;if(cellChanged)
this._removeChange("ActiveCell",this.oActiveCell);this.oActiveCell=null;this.ActiveCell="";this.oActiveRow.renderActive();this.oActiveRow.setSelectedRowImg();this.colButtonMouseOut();if(this.AddNewBoxVisible)
this.updateAddNewBox();var mouseDownStatus=this._mouseDown;igtbl_activate(this.Id);this._mouseDown=mouseDownStatus;igtbl_activate(this.Id);this._removeChange("ActiveRow",oldARow);this._recordChange("ActiveRow",this.oActiveRow);if(fireEvents)
{if(cellChanged)
this.fireEvent(this.Events.CellChange,[this.Id,""]);var oldNPB=this.NeedPostBack;this.fireEvent(this.Events.AfterRowActivate,[this.Id,row.Element.id]);if(!oldNPB&&this.NeedPostBack&&oldARow==row)
this.NeedPostBack=false;if(row.IsFilterRow)
this.NeedPostBack=false;}},"deleteSelectedRows",function()
{igtbl_deleteSelRows(this.Id);igtbl_activate(this.Id);this._recalcRowNumbers();},"unloadGrid",function()
{if(this.Id)
igtbl_unloadGrid(this.Id);},"dispose",function()
{igtbl_unloadGrid(this.Id,true);},"beginEditTemplate",function()
{var row=this.getActiveRow();if(row)
row.editRow();},"endEditTemplate",function(saveChanges)
{var row=this.getActiveRow();if(row)
row.endEditRow(saveChanges);},"find",function(re,back,searchHiddenColumns)
{var g=this;if(re)
g.regExp=re;if(!g.regExp)
return null;g.lastSearchedCell=null;if(back==true||back==false)
g.backwardSearch=back;var row=null;if(!g.backwardSearch)
{row=g.Rows.getRow(0);if(row&&row.getHidden())
row=row.getNextRow();while(row&&row.find(re,back,searchHiddenColumns)==null)
{row=row.getNextTabRow(false,true);}}
else
{var rows=g.Rows;while(rows)
{row=rows.getRow(rows.length-1);if(row&&row.getHidden())
row=row.getPrevRow();if(row&&row.Expandable)
rows=row.Rows;else
{if(!row)
row=rows.ParentRow;rows=null;}}
while(row&&row.find(re,back,searchHiddenColumns)==null)
row=row.getNextTabRow(true,true);}
return g.lastSearchedCell;},"findNext",function(re,back,searchHiddenColumns)
{var g=this;if(!g.lastSearchedCell)
{return this.find(re,back,searchHiddenColumns);}
if(re)
g.regExp=re;if(!g.regExp)
return null;if(back==true||back==false)
g.backwardSearch=back;var row=g.lastSearchedCell.Row;while(row&&row.findNext(re,back,searchHiddenColumns)==null)
row=row.getNextTabRow(g.backwardSearch,true);return g.lastSearchedCell;},"alignStatMarginsScrollBar",function(elem)
{var divElem=this.getDivElement();if(divElem.clientHeight<divElem.scrollHeight)
{var scrollbarWidth=divElem.offsetWidth-divElem.clientWidth;if(elem.Element.parentNode.style.width=="100%")
elem.Element.parentNode.parentNode.style.paddingRight=(scrollbarWidth>0)?scrollbarWidth+"px":"";}
else
{if(elem.Element.parentNode.style.width=="100%")
elem.Element.parentNode.parentNode.style.paddingRight="";}},"alignStatMargins",function()
{if(this.MainGrid.offsetHeight==0)return;if(this.UseFixedHeaders)
{var hDiv=igtbl_getElementById(this.Id+"_hdiv");if(this.Bands.length==1&&this.StatHeader&&hDiv&&hDiv.firstChild&&hDiv.firstChild.tHead&&hDiv.firstChild.tHead.rows&&hDiv.firstChild.tHead.rows.length>0)
{var lastHead=hDiv.firstChild.tHead.rows[hDiv.firstChild.tHead.rows.length-1];if(lastHead.lastChild&&lastHead.lastChild.firstChild&&lastHead.lastChild.firstChild.id==this.Id+"_drs")
{var hDivScr=lastHead.lastChild.firstChild;var divEl=this.getDivElement();hDivScr.firstChild.style.left=-divEl.scrollLeft+"px";}}
else
{if(this.StatHeader)
this.StatHeader.ScrollTo(this.getDivElement().scrollLeft);if(this.StatFooter)
this.StatFooter.ScrollTo(this.getDivElement().scrollLeft);}}
else
{var scrollLeft=this.getDivElement().scrollLeft;if(this.StatHeader)
{var divContent=this.DivElement.firstChild;var percentageWidth=this.Element.style.width.indexOf("%")!=-1;if(!percentageWidth)
{if(!this.DivElement.getAttribute("scrollDivWidth"))
{var comWidth=this.DivElement.firstChild.offsetWidth;this.DivElement.firstChild.style.width=comWidth+"px";}}
else
{if(!igtbl_dom.table.hasPercentageColumns(this.Element,this.Bands[0].firstActiveCell))
{var comWidth=this.Element.offsetWidth;if(this.get("StationaryMarginsOutlookGroupBy")=="True")
comWidth+=_igtbl_headerRowIndentation(this,this.Bands[0].SortedColumns);this.DivElement.firstChild.style.width=comWidth+"px";this.StatHeader.Element.parentNode.style.width=comWidth+"px";if(this.StatFooter)
this.StatFooter.Element.parentNode.style.width=comWidth+"px";}
else
this.alignStatMarginsScrollBar(this.StatHeader);}
this.StatHeader.ScrollTo(scrollLeft);}
if(this.StatFooter)
{this.StatFooter.ScrollTo(scrollLeft);if(!this.StatHeader||ig_csom.IsIE)
{var percentageWidth=this.Element.style.width.indexOf("%")!=-1;if(percentageWidth&&igtbl_dom.table.hasPercentageColumns(this.Element,this.Bands[0].firstActiveCell))
this.alignStatMarginsScrollBar(this.StatFooter);}}
var mtb=igtbl_getElementById(this.Id+"_mtb");if(mtb&&mtb.rows&&mtb.rows.length==2&&mtb.parentNode&&!ig_shared.IsNetscape6)
{var r1H=igtbl_getAbsBounds(mtb.rows[1]).h;var r2H=0;var re=new RegExp("_gbr$","");if(this.StatHeader||!mtb.rows[0].id.match(re))
{r2H=igtbl_getAbsBounds(mtb.rows[0]).h;}
igtbl_getElementById(this.Id+"_mc").style.height=(r1H-r2H)+"px";}}},"selectCellRegion",function(startCell,endCell)
{var sCol=startCell.Column,eCol=endCell.Column;if(sCol.Index>eCol.Index)
{var c=sCol;sCol=eCol;eCol=c;}
var sRow=startCell.Row,sRowIndex=sRow.getIndex(),eRow=endCell.Row,eRowIndex=eRow.getIndex();if(sRowIndex>eRowIndex)
{var c=sRow;sRow=eRow;eRow=c;var i=sRowIndex;sRowIndex=eRowIndex;eRowIndex=i;}
var pc=sRow.OwnerCollection;var band=sCol.Band;var selArray=new Array();if(sRowIndex>-1)
for(var i=sRowIndex;i<=eRowIndex;i++)
{var row=pc.getRow(i);if(!row.getHidden())
for(var j=sCol.Index;j<=eCol.Index;j++)
{var col=band.Columns[j];if(col.getVisible())
{var cell=row.getCellByColumn(col);if(cell&&cell.Element)
selArray[selArray.length]=cell.Element.id;}}}
if(selArray.length>0)
igtbl_gSelectArray(this.Id,0,selArray);delete selArray;},"selectRowRegion",function(startRow,endRow)
{var sRowIndex=startRow.getIndex(),eRowIndex=endRow.getIndex();if(sRowIndex>eRowIndex)
{var r=startRow;startRow=endRow;endRow=r;var i=sRowIndex;sRowIndex=eRowIndex;eRowIndex=i;}
if((startRow.isFixedTop&&startRow.isFixedTop())||(startRow.isFixedBottom&&startRow.isFixedBottom())||(endRow.isFixedTop&&endRow.isFixedTop())||(endRow.isFixedBottom&&endRow.isFixedBottom()))return;var pc=startRow.OwnerCollection;var selArray=new Array();for(var i=sRowIndex;i<=eRowIndex;i++)
{var row=pc.getRow(i);if(row&&!row.getHidden())
selArray[selArray.length]=row.Element.id;}
if(selArray.length>0)
igtbl_gSelectArray(this.Id,1,selArray);delete selArray;},"selectColRegion",function(startCol,endCol)
{if(startCol.Index>endCol.Index)
{var c=startCol;startCol=endCol;endCol=c;}
var band=startCol.Band;var selArray=new Array();for(var i=startCol.Index;i<=endCol.Index;i++)
{var col=band.Columns[i];if(col.getVisible())
selArray[selArray.length]=col.Id;}
if(selArray.length>0)
igtbl_gSelectArray(this.Id,2,selArray);delete selArray;},"startHourGlass",function()
{if(!igtbl_waitDiv)
{igtbl_waitDiv=document.createElement("div");document.body.insertBefore(igtbl_waitDiv,document.body.firstChild);igtbl_waitDiv.style.zIndex=this._getZ(10000,1);igtbl_waitDiv.style.position="absolute";igtbl_waitDiv.style.left=0;igtbl_waitDiv.style.top=0;igtbl_waitDiv.style.backgroundColor="transparent";}
igtbl_waitDiv.style.display="";igtbl_waitDiv.style.width=document.body.clientWidth;igtbl_waitDiv.style.height=document.body.clientHeight;igtbl_waitDiv.style.cursor="wait";if(igtbl_wndOldCursor===null)
igtbl_wndOldCursor=document.body.style.cursor;document.body.style.cursor="wait";},"stopHourGlass",function()
{if(igtbl_waitDiv)
{igtbl_waitDiv.style.cursor="";igtbl_waitDiv.style.display="none";document.body.style.cursor=igtbl_wndOldCursor;igtbl_wndOldCursor=null;}},"clearSelectionAll",function()
{igtbl_clearSelectionAll(this.Id);},"alignGrid",function(){},"suspendUpdates",function(suspend)
{if(suspend==false)
{this.SuspendUpdates=false;}
else
this.SuspendUpdates=true;},"beginEdit",function()
{if(this.oActiveCell)
this.oActiveCell.beginEdit();},"endEdit",function(force)
{var ec=this._editorCurrent;if(!ec&&this.oActiveCell)
{ec=this.oActiveCell.Column.getEditorControl();if(ec&&ec.Element)
ec=ec.Element;}
if(force)
if(ec&&ec.removeAttribute)
ec.removeAttribute("noOnBlur");if(ec&&ec.getAttribute&&ec.getAttribute("noOnBlur"))
return;igtbl_hideEdit(this.Id);},"fireEvent",function(eventObj,args)
{if(!this.isLoaded())return;var result=false;if(eventObj[0]!="")
{try
{if(typeof(eval(eventObj[0]))!="function")
throw"Event handler does not exist.";}
catch(ex)
{alert("There is a problem with the event handler method: '"+eventObj[0]+"'. Please check the method name's spelling.")
return false;}
result=eval(eventObj[0]).apply(this,args);}
if(this.GridIsLoaded&&result!=true&&eventObj[1]>0&&!this.CancelPostBack)
this.NeedPostBack=true;this.CancelPostBack=false;return result;},"setNewRowImg",function(row)
{var gs=this;if(row)
row.setSelectedRowImg(true);if(gs.newImg!=null)
{gs._lastSelectedRow=null;var imgObj;imgObj=document.createElement("img");imgObj.src=gs.BlankImage;imgObj.border="0";imgObj.setAttribute("imgType","blank");gs.newImg.parentNode.appendChild(imgObj);gs.newImg.parentNode.removeChild(gs.newImg);var oRow=igtbl_getRowById(imgObj.parentNode.parentNode.id);if(oRow)
gs._recalcRowNumbers(oRow);gs.newImg=null;}
if(!row||row.Band.getRowSelectors()==2||row.Band.AllowRowNumbering>1)
return;var imgObj;imgObj=document.createElement("img");imgObj.src=gs.NewRowImage;imgObj.border="0";imgObj.setAttribute("imgType","newRow");if(gs.Section508Compliant)
imgObj.setAttribute("alt",gs._newRowAltText);var cell=row.Element.cells[row.Band.firstActiveCell-1];cell.innerHTML="";cell.appendChild(imgObj);gs.newImg=imgObj;},"colButtonMouseOut",function()
{igtbl_colButtonMouseOut(null,this.Id);},"sort",function()
{if(igtbl_sortGrid)
{igtbl_sortGrid.apply(this);this._recordChange("Sort",this)
this._recalcRowNumbers();}},"updateAddNewBox",function()
{igtbl_updateAddNewBox(this.Id);},"update",function()
{if(typeof(igtbl_hideEdit)!="undefined")
{if(this._editorCurrent)
this._editorCurrent.removeAttribute("noOnBlur");igtbl_hideEdit(this.Id);}
var p=igtbl_getElementById(this.ClientID);if(!p)return;if(this.Element.parentNode)
{if(this.Element.parentNode.scrollLeft)
ig_ClientState.setPropertyValue(this.ViewState,"ScrollLeft",this.Element.parentNode.scrollLeft.toString());if(this.Element.parentNode.scrollTop)
ig_ClientState.setPropertyValue(this.ViewState,"ScrollTop",this.Element.parentNode.scrollTop.toString());}
p.value=ig_ClientState.getText(this.ViewState.parentNode);},"goToPage",function(page)
{page=igtbl_parseInt(page);if(!this.isLoaded()||!this.AllowPaging||this.CurrentPageIndex==page||page<1||page>this.PageCount)
return;if(!this.Node&&!ig_csom.IsNetscape6||this.LoadOnDemand!=3)
{this._recordChange("PageChanged",this,page);igtbl_doPostBack(this.Id);}
else
{this.invokeXmlHttpRequest(this.eReqType.Page,this,page);}},"getRowByLevel",function(level)
{if(typeof(level)=="string")
level=level.split("_");var rows=this.Rows;var adj=0;if(typeof(rows._getRowToStart)!="undefined")
adj=rows._getRowToStart();for(var i=0;i<level.length-1;i++)
{rows=rows.getRow(level[i]-adj).Rows;if(typeof(rows._getRowToStart)!="undefined")
adj=rows._getRowToStart();}
return rows.getRow(level[level.length-1]-adj);},"xmlHttpRequest",function(type,waitResponse)
{if(this.fireEvent(this.Events.BeforeXmlHttpRequest,[this.Id,type])==true)
return;var smartCallback=null;var serverContext={QueryString:igtbl_escape(this.QueryString),requestType:"xml"};var clientContext={requestType:"json"};smartCallback=new ig_SmartCallback(clientContext,serverContext,null,this.UniqueID,this,waitResponse)
smartCallback.callbackFinished=igtbl_onReadyStateChange;smartCallback.Type=type;smartCallback.QueryString=this.QueryString;smartCallback.RowToQuery=this.RowToQuery;smartCallback.ReqType=type;this._displayPI();smartCallback.execute();},"_containsChange",function(type,obj)
{return obj&&(obj._Changes[type]!=null);},"_recordChange",function(type,obj,value,inId)
{var stateChange=new igtbl_StateChange(type,this,obj,value);if(typeof(this[type])!="undefined")
{var id=obj?(obj.Element?obj.Element.id:obj.Id):inId;if(typeof(value)!="undefined"&&value!=null)
this[type][id]=value;else
this[type][id]=inId?stateChange:true;}
return stateChange;},"_removeChange",function(type,obj,lastOnly)
{var ch;if(obj&&(ch=obj._Changes[type]))
{if(ch.length)
{if(lastOnly)
ch[ch.length-1].remove(lastOnly);else
{for(var i=ch.length-1;i>=1;i--)
ch[i].remove();obj._Changes[type].remove();}}
else
ch.remove(lastOnly);if(typeof(this[type])!="undefined")
{var id=obj.Element?obj.Element.id:obj.Id;delete this[type][id];}}},"_removeFilterChange",function(obj,parentRowId)
{if(obj.RowFilterMode==2)
{var parentRowIds=parentRowId.split("\x01");var ch;if(obj&&(ch=obj._Changes["FilterColumn"]))
{if(ch.length)
{for(var i=ch.length-1;i>=1;i--)
{var values=(ch[i].Node.Value)?ch[i].Node.Value.split("%01"):ch[i].Node.props;if(values[2]==parentRowIds[0]&&values[3]==parentRowIds[1])
ch[i].remove();}}
else
{var values=(ch.Node.Value)?ch.Node.Value.split("%01"):ch.Node.props;if(values[2]==parentRowIds[0]&&values[3]==parentRowIds[1])
ch.remove();}}}
else
this._removeChange("FilterColumn",obj);},"alignDivs",function(scrollLeft,force)
{if(this.MainGrid.offsetHeight==0)return;if(!this.UseFixedHeaders&&(this.XmlLoadOnDemandType==0||this.XmlLoadOnDemandType==4))
{this._alignFilterRow(this.Rows);return;}
var mainGrid=this.MainGrid;if(!mainGrid)return;var divs=this._scrElem;var divf=this.Element.parentNode;var isInit=false;this.Element.setAttribute("noOnResize",true);if(ig_csom.IsFireFox)
{if(divs.style.height!=divs.parentNode.style.height)
divs.style.height=divs.parentNode.style.height;}
if(!divs.firstChild.style.width&&this.Element.offsetWidth)
{var expandAreaWidth=(this.Bands.length>1)?this.Element.firstChild.firstChild.offsetWidth:0;divs.firstChild.style.width=(this.Element.offsetWidth+expandAreaWidth+(this.GroupCount==1?this.Bands[0].getIndentation():0)).toString()+"px";if(!mainGrid.style.height)
divs.style.overflowY="hidden";isInit=true;}
var calculatedScrollWidth=divs.getAttribute("scrollDivWidth");if(calculatedScrollWidth)
{if(calculatedScrollWidth>divs.firstChild.offsetWidth)
{divs.firstChild.style.width=calculatedScrollWidth+"px";}}
if(!mainGrid.style.width)
divs.style.width=mainGrid.clientWidth.toString()+"px";if(this.XmlLoadOnDemandType==0||this.XmlLoadOnDemandType==4)
divs.firstChild.style.height=this.Element.offsetHeight.toString()+"px";else
this._setScrollDivHeight();if(!mainGrid.style.height)
{divs.style.height=this.Element.offsetHeight.toString()+"px";if(divs.scrollHeight!=divs.clientHeight)
{var divsHeight=this.Element.offsetHeight+divs.scrollHeight-divs.clientHeight;if(divsHeight<0)divsHeight=-divsHeight;divs.style.height=divsHeight.toString()+"px";}
divs.parentNode.style.height=divs.offsetHeight.toString()+"px";}
if(isInit)
{if(!divs.style.width||divs.style.width.charAt(divs.style.width.length-1)!="%")
divs.setAttribute("oldW",divs.offsetWidth);if(!divs.style.height||divs.style.height.charAt(divs.style.height.length-1)!="%")
divs.setAttribute("oldH",divs.offsetHeight);}
var relOffs=false;if(ig_csom.IsIE)
{while(mainGrid&&mainGrid.tagName!=(igtbl_isXHTML?"HTML":"BODY")&&!relOffs)
{relOffs=mainGrid.style.position!=""&&mainGrid.style.position!="static";if(!relOffs)mainGrid=mainGrid.parentNode;}}
divf.style.left=(parseInt(divf.style.left,10)+igtbl_getAbsolutePos2("Left",divs)-igtbl_getAbsolutePos2("Left",divf)).toString()+"px";divf.style.top=(parseInt(divf.style.top,10)+igtbl_getAbsolutePos2("Top",divs)-igtbl_getAbsolutePos2("Top",divf)).toString()+"px";{divf.style.width=igtbl_clientWidth(divs).toString()+"px";divf.style.height=igtbl_clientHeight(divs).toString()+"px";}
if(this.XmlLoadOnDemandType==0||this.XmlLoadOnDemandType==4)
divs.firstChild.style.height=this.Element.offsetHeight.toString()+"px";else
this._setScrollDivHeight();if(divf.firstChild.style.left=="")
divf.firstChild.style.left="0px";if(divf.firstChild.style.top=="")
divf.firstChild.style.top="0px";if(!scrollLeft)
scrollLeft=divs.scrollLeft;else
{igtbl_scrollLeft(divs,scrollLeft);}
var doHoriz=false;if(!this._oldScrollLeftAlign)
this._oldScrollLeftAlign=0;if(this._oldScrollLeftAlign!=scrollLeft)
{this._oldScrollLeftAlign=scrollLeft;doHoriz=true;}
if(parseInt(divf.firstChild.style.top,10)!=-divs.scrollTop)
{if(this.XmlLoadOnDemandType!=2)
divf.firstChild.style.top=(-divs.scrollTop).toString()+"px";if(this.StatHeader||this.StateFooter)
doHoriz=true;}
if(doHoriz||force)
{if(this.UseFixedHeaders)
{var rowDivs=igtbl_getDocumentElement(this.Id+"_drs");if(rowDivs)
{if(!rowDivs.length)
rowDivs=[rowDivs];for(var i=0;i<rowDivs.length;i++)
rowDivs[i].firstChild.style.left=(-scrollLeft).toString()+"px";}}
else
{if(this.XmlLoadOnDemandType!=2)
divf.firstChild.style.top=(-divs.scrollTop).toString()+"px";divf.firstChild.style.left=(-divs.scrollLeft).toString()+"px";}}
if(isInit)
{divf.style.left=(parseInt(divf.style.left,10)+igtbl_getAbsolutePos2("Left",divs)-igtbl_getAbsolutePos2("Left",divf)).toString()+"px";divf.style.top=(parseInt(divf.style.top,10)+igtbl_getAbsolutePos2("Top",divs)-igtbl_getAbsolutePos2("Top",divf)).toString()+"px";divf.style.width=igtbl_clientWidth(divs).toString()+"px";divf.style.height=igtbl_clientHeight(divs).toString()+"px";}
this._alignFilterRow(this.Rows);this.Element.removeAttribute("noOnResize");},"_alignFilterRow",function(rowsObj)
{var filterRow=rowsObj.FilterRow;if(filterRow)
{var elem=filterRow.getCellElements();for(var i=0;elem!=null&&elem.length&&i<elem.length;i++)
{var spanElm=null;for(var j=0;j<elem[i].childNodes.length;j++)
{if(elem[i].childNodes[j].tagName=="SPAN")
{spanElm=elem[i].childNodes[j];}}
if(spanElm!=null)
{spanElm.style.width="0px";var tdWidth=elem[i].clientWidth-(ig_csom.IsFireFox30?1:0);spanElm.style.width=(tdWidth-spanElm.offsetLeft)+"px";}}}
for(var i=0;i<rowsObj.length;i++)
{if(rowsObj.getRow(i).ChildRowsCount>0)
this._alignFilterRow(rowsObj.getRow(i).Rows);}},"_setScrollDivHeight",function()
{var divs=this._scrElem;var estRowsHeight=(this.RowsServerLength+1)*(this.getDefaultRowHeight());if(!this.StatHeader&&this.Bands[0].ColHeadersVisible==1)
estRowsHeight+=this.getDefaultRowHeight();if(!this.StatFooter&&this.Bands[0].ColFootersVisible==1)
estRowsHeight+=this.getDefaultRowHeight();var height=(this.Rows.Element.parentNode.offsetHeight>estRowsHeight)?this.Rows.Element.parentNode.offsetHeight:estRowsHeight;divs.firstChild.style.height=height+"px";},"_recalcRowNumbers",function(row)
{if(this.ClientSideRenumbering!=1)return;if(row&&row.Band.AllowRowNumbering<2||!row&&this.AllowRowNumbering<2)return;for(var i=0;i<this.Bands.length;i++)
this.Bands[i]._currentRowNumber=0;if(!row)
igtbl_RecalculateRowNumbers(this.Rows,1,this.Bands[0],this.Rows.Node);else
switch(row.Band.AllowRowNumbering)
{case(2):case(4):igtbl_RecalculateRowNumbers(this.Rows,1,this.Bands[0],this.Rows.Node);break;case(3):var rc=row.ParentRow?row.ParentRow.Rows:this.Rows;igtbl_RecalculateRowNumbers(rc,1,rc.Band,rc.Node);break;}},"_calculateStationaryHeader",function()
{var band=this.Bands[0];if(!band.IsGrouped&&this.StatHeader&&(this.StationaryMargins==1||this.StationaryMargins==3))
{var tr=this.StatHeader.Element.parentNode.parentNode.parentNode.parentNode;var oldTRDisplay=tr.style.display;var th=this.Element.childNodes[1];var i=0;var drs=null;var row=th.firstChild;while(i<row.cells.length&&(!row.cells[i].firstChild||row.cells[i].firstChild.id!=this.Id+"_drs"))i++;if(i<row.cells.length)
{var td=row.cells[i];drs=td.firstChild;}
if(this.Rows&&(this.Rows.length>0||(this.Rows.AddNewRow&&!this.Rows.AddNewRow.isFixedBottom()||this.Rows.FilterRow&&!this.Rows.FilterRow.isFixedBottom())))
{var gridHeighInPX=this.MainGrid.style.height.indexOf("%")==-1;if(this.Rows.length==1&&gridHeighInPX&&th.offsetHeight>1)
{var mrElem=document.getElementById(this.Id+"_mr");var mcElem=document.getElementById(this.Id+"_mc");if(mrElem&&mcElem&&igtbl_parseInt(mrElem.style.height)&&igtbl_parseInt(mcElem.style.height))
{if(mrElem.style.height.substr(mrElem.style.height.length-2)=="px"&&mcElem.style.height.substr(mcElem.style.height.length-2)=="px")
{mrElem.style.height=(igtbl_parseInt(mrElem.style.height)-igtbl_parseInt(th.clientHeight)).toString()+"px";mcElem.style.height=(igtbl_parseInt(mcElem.style.height)-igtbl_parseInt(th.clientHeight)).toString()+"px";}
else
{mrElem.style.height=(mrElem.offsetHeight-igtbl_parseInt(th.offsetHeight)).toString()+"px";mcElem.style.height=(mcElem.offsetHeight-igtbl_parseInt(th.offsetHeight)).toString()+"px";}}}
tr.style.display="";var hdiv=tr.childNodes[0].childNodes[0];if(hdiv.style.height=="0pt")hdiv.style.height="";if(!this._fixHeightOnce&&igtbl_isXHTML&&this.MainGrid&&this.MainGrid.style.height)
{this._fixHeightOnce=true;var height=this.MainGrid.style.height;if(height.substr(height.length-2)=="px")
this.MainGrid.style.height=(igtbl_parseInt(height)-th.offsetHeight).toString()+"px";}
th.style.display="none";if(drs)
drs.style.display="none";}
else
{tr.style.display="none";th.style.display="";if(!this._fixHeightOnce&&igtbl_isXHTML&&this.MainGrid&&this.MainGrid.style.height)
{this._fixHeightOnce=true;var height=this.MainGrid.style.height;if(height.substr(height.length-2)=="px")
this.MainGrid.style.height=(igtbl_parseInt(height)+th.offsetHeight).toString()+"px";}
if(drs)
drs.style.display="";}
if(oldTRDisplay!=tr.style.display)
{for(var i=0;i<band.Columns.length;i++)
{var cols=igtbl_getDocumentElement(band.Columns[i].Id);if(cols&&cols.length==2)
{if(oldTRDisplay=="")
{cols[1].innerHTML=cols[0].innerHTML;cols[0].innerHTML="&nbsp;";}
else
{cols[0].innerHTML=cols[1].innerHTML;cols[1].innerHTML="&nbsp;";}}}}}},"_getCurrentFiltersString",function(col,band,parentRowId)
{if(!band)
{if(col)band=col.Band;else band=this.Bands[0];}
var currentFilters;if(((col&&col.RowFilterMode==igtbl_RowFilterMode.AllRowsInBand)||band.Index==0)&&!band.IsGrouped)
{currentFilters=band._filterPanels;}
else
{if(parentRowId&&parentRowId.length>0)
{var tempId="";if(band.IsGrouped)
{tempId=parentRowId.replace("_gr","_t");}
else
{tempId=parentRowId.replace("_r","_t")}
currentFilters=band._filterPanels[tempId];}}
var currentFilterString="";if(currentFilters)
{var seperator="\x05";for(var cf in currentFilters)
{if(currentFilters[cf]&&currentFilters[cf].getOperator&&currentFilters[cf].IsActive())
{var newfilter=false;if(col)newfilter=(cf==col.Id);var foundColumn=igtbl_getColumnById(cf);currentFilterString+=foundColumn.getLevel(true)+seperator+currentFilters[cf].getOperator()+seperator+currentFilters[cf].getEvaluationValue()+seperator+newfilter+"\x03";}}}
return currentFilterString;},"invokeXmlHttpRequest",function(type,object,data,waitResponse)
{var g=this;if(!g.Node&&!ig_csom.IsNetscape6||g.LoadOnDemand!=3)
return;switch(type)
{case g.eReqType.FilterDropDownFill:{var rows=data;var col=object;var parentRowDataKey="";var parentRowId="";var oSqlWhere=null;if(rows)
{if(rows.ParentRow&&(rows.Band.Index>0||rows.Band.IsGrouped))
{parentRowId=rows.ParentRow.Id;parentRowDataKey=rows.ParentRow.DataKey;}
if(rows.ParentRow)
oSqlWhere=rows.ParentRow._generateBandsSqlWhere(rows.ParentRow.Band);}
var sqlWhere="";var newLevel="";if(oSqlWhere)
{sqlWhere=oSqlWhere.sqlWhere?oSqlWhere.sqlWhere:"";newLevel=oSqlWhere.newLevel?oSqlWhere.newLevel:"";}
g.QueryString="FilterDropFill\x01"+col.getLevel(true)+"\x01"+parentRowDataKey+"\x01"+parentRowId+"\x01"+sqlWhere+"\x01"+newLevel;g.xmlHttpRequest(type);break;}
case g.eReqType.Filter:{var rows=data;var col=object;var parentRowDataKey="";var parentRowId="";if(rows)
{if(rows.Band.Index>0||rows.Band.IsGrouped)
{parentRowId=rows.ParentRow.Id;parentRowDataKey=rows.ParentRow.DataKey;}}
var currentFilterString=this._getCurrentFiltersString(col,col.Band,parentRowId);g.QueryString="Filter\x01"+col.getLevel(true)+"\x01"+parentRowDataKey+"\x01"+parentRowId+"\x01"+currentFilterString+"\x01"+g._buildSortOrder(g);igtbl_scrollTop(g.getDivElement(),0);g.xmlHttpRequest(type);break;}
case g.eReqType.UpdateCell:{var cell=object;var row=cell.Row;if(g.LoadOnDemand==3&&(typeof(g.Events.AfterRowUpdate)=="undefined"||g.Events.AfterRowUpdate[1]==0&&(g.Events.XmlHTTPResponse[1]==1||g.Events.AfterCellUpdate[1]==1)))
{var cellInfo=row._generateUpdateRowSemaphore();g.QueryString="UpdateCell\x01"+cell.Band.Index+"\x02"+cell.Column.Index+"\x02"+cell.Row.getIndex(true)+"\x02"+cell.Row.DataKey+"\x02"+data+"\x02"+cell.getLevel(true)+"\x02"+cell.getOldValue()+"\x02"+(cellInfo.length>0?"CellValues\x06"+cellInfo:"");g.xmlHttpRequest(type);}
break;}
case g.eReqType.AddNewRow:{var rows=object;if((typeof(g.Events.AfterRowUpdate)=="undefined"||g.Events.AfterRowUpdate[1]==0&&g.Events.XmlHTTPResponse[1]==1))
{g.QueryString="AddNewRow\x01"+rows.Band.Index+"\x02"+(rows.ParentRow?rows.ParentRow.getIndex(true)+"\x02"+rows.ParentRow.DataKey:"\x02");g.xmlHttpRequest(type);}
break;}
case g.eReqType.Sort:{var rows=object;rows.sortXml();break;}
case g.eReqType.ChildRows:{var row=object;row.requestChildRows();break;}
case g.eReqType.DeleteRow:{if(g.LoadOnDemand==3&&(!g.Events.XmlHTTPResponse||g.Events.XmlHTTPResponse[1]||g.Events.AfterRowDeleted[1]))
{var row=object;var cellInfo=row._generateUpdateRowSemaphore(true);var dataKey=row._generateHierarchicalDataKey();g.QueryString="DeleteRow\x01"+row.Band.Index+"\x02"+row.getIndex(true)+"\x02"+dataKey+"\x02"+row.getLevel(true)+"\x02"+row.DataKey+"\x02"+g.RowsRetrieved+"\x04"+(cellInfo.length>0?"CellValues\x06"+cellInfo+"\x04":"")+"Page"+"\x03"+(g.AllowPaging===true?g.CurrentPageIndex:-1);g.QueryString+="\x04"+g._buildSortOrder();g.RowToQuery=row;g.xmlHttpRequest(type,waitResponse);}
break;}
case g.eReqType.UpdateRow:{var row=object;var cellInfo="";if(row._dataChanged&1)
{g.QueryString="AddNewRow\x06"+(row.ParentRow?row.ParentRow.getLevel(true)+"\x02"+row.ParentRow.DataKey:"\x02")+(g.QueryString.length>0?"\x04":"")+g.QueryString;this.setNewRowImg(null);}
else
cellInfo=row._generateUpdateRowSemaphore();var dataKey=row._generateHierarchicalDataKey();g.QueryString="UpdateRow\x01"+row._dataChanged+"\x02"+row.Band.Index+"\x02"+row.getLevel(true)+"\x02"+dataKey+"\x02"+g.RowsRetrieved+"\x02"+g.CurrentPageIndex+"\x04"+(cellInfo.length>0?"CellValues\x06"+cellInfo+"\x04":"")+g.QueryString;g.RowToQuery=row;g.xmlHttpRequest(type);break;}
case g.eReqType.MoreRows:{if(this.AllowPaging||this.GroupCount>0||g.requestingMoreRows)return;g.requestingMoreRows=true;var de=g.getDivElement();de.setAttribute("oldST",de.scrollTop.toString());if(g.RowsServerLength>g.Rows.length)
{g.QueryString="NeedMoreRows\x01"+g.RowsRetrieved+"\x02"+g.Rows.length.toString();var sortOrder="";sortOrder=g._buildSortOrder();g.QueryString+="\x02"+g.Bands[0].ColumnsOrder;g.QueryString+="\x02"+sortOrder;g.QueryString+="\x02"+this.Bands[0]._sqlWhere;var currentFilters="";var bandFilters=g.Bands[0]._filterPanels;if(bandFilters)
{for(var colId in bandFilters)
{var filter=bandFilters[colId];if(filter.IsActive())
{var col=igtbl_getColumnById(colId);currentFilters+=col.getLevel(true)+"\x05"+filter.getOperator()+"\x05"+filter.getEvaluationValue()+"\x03";}}}
g.QueryString+="\x02"+currentFilters;de.setAttribute("noOnScroll","true");g.xmlHttpRequest(g.eReqType.MoreRows);}
break;}
case g.eReqType.Custom:{g.QueryString="Custom\x01"+data;g.xmlHttpRequest(g.eReqType.Custom);break;}
case g.eReqType.Page:{g.QueryString="Page\x01"+data+"\x01"+g.CurrentPageIndex+"\x01"+g._buildSortOrder(g)
+"\x01"+g._getCurrentFiltersString();;g._pageToGo=data;g.xmlHttpRequest(g.eReqType.Page);break;}
case g.eReqType.Scroll:{if(this.AllowPaging)return;var de=g.getDivElement();de.setAttribute("oldST",de.scrollTop.toString());var topRowNo=Math.floor(de.scrollTop/g.getDefaultRowHeight());if(g.XmlLoadOnDemandType==2)
{g._recordChange("RowToStart",g,topRowNo);}
g.QueryString="NeedMoreRows\x01"+topRowNo.toString()+"\x02"+topRowNo.toString();var sortOrder="";sortOrder=g._buildSortOrder();g.QueryString+="\x02"+g.Bands[0].ColumnsOrder;g.QueryString+="\x02"+sortOrder;g.QueryString+="\x02"+this.Bands[0]._sqlWhere;g.QueryString+="\x02"+g._getCurrentFiltersString();g.xmlHttpRequest(g.eReqType.Scroll);break;}
case g.eReqType.Refresh:{var rows=g.Rows;if(object&&object.Type)
{if(object.Type=="rows")
rows=object;else if(object.Type=="row")
rows=object.Rows;}
if(rows)
rows.refresh();break;}}},"getDefaultRowHeight",function()
{var rh=igtbl_parseInt(this.Bands[0].DefaultRowHeight);if(!rh)
rh=22;if(igtbl_isXHTML)
rh+=2;return rh;},"_buildSortOrder",function()
{var sortOrder="";for(var i=0;i<this.Bands[0].SortedColumns.length;i++)
{var col=igtbl_getColumnById(this.Bands[0].SortedColumns[i]);sortOrder+=col.Key+(col.SortIndicator==2?" DESC":"")+(i<this.Bands[0].SortedColumns.length-1?",":"");}
return sortOrder;},"_ensureValidParent",function(obj)
{e=obj.Element;var pe=e?e.parentNode:null;if(pe&&pe.tagName!="FORM"&&pe.tagName!=(igtbl_isXHTML?"HTML":"BODY"))
try
{ig_csom._skipNew=true;npe=igtbl_getElementById(this.Id);if(npe)
npe=npe.form;if(obj._relocate)
obj._relocate(npe,window.document.body);else
{pe.removeChild(e);if(npe)
try
{npe.appendChild(e);}
catch(ex)
{npe=null;}
if(!npe)
document.body.insertBefore(e,document.body.firstChild);e.style.zIndex=9999;}
ig_csom._skipNew=false;}
catch(ex){}},"getDivElement",function()
{var de=this.DivElement;if(this._scrElem)
de=this._scrElem;return de;},"isDisabled",function()
{var result=false;if(this._thisForm&&igtbl_isDisabled(this._thisForm)||igtbl_isDisabled(this.MainGrid))
result=true;return result;},"isLoaded",function()
{if(!this.GridIsLoaded)
return false;return!this.isDisabled();},"resize",function(width,height)
{width=(width<0)?0:width
height=(height<0)?0:height
if(!ig_csom.IsIE||((ig_csom.IsIE6||ig_csom.IsIE7)&&igtbl_isXHTML))
{var marginWidth=igtbl_dom.dimensions.bordersWidth(this.MainGrid);this.MainGrid.style.width=width+"px";document.getElementById(this.Id+"_mc").style.width=(width-marginWidth)>=0?width-marginWidth:0+"px";this.MainGrid.style.height=0+"px";this._fixHeightOnce=false;var marginHeight=igtbl_dom.dimensions.bordersHeight(this.MainGrid);for(var x=0;x<this.MainGrid.rows.length;x++)
{if(this.MainGrid.rows[x].id!=this.Id+"_mr")
{marginHeight+=this.MainGrid.rows[x].offsetHeight;}}
if(height<marginHeight)
{height=marginHeight;}
this.MainGrid.style.height=height+"px";document.getElementById(this.Id+"_mr").style.height=(height-marginHeight)+"px";document.getElementById(this.Id+"_mc").style.height=(height-marginHeight)+"px";}else
{this.MainGrid.style.width=width+"px";this.MainGrid.style.height=height+"px";}
if(ig_csom.IsFireFox)
{this.alignStatMargins();this.alignDivs(0,true);}},"hide",function()
{this.getDivElement().style.display="none";this.MainGrid.style.display="none";},"show",function()
{this.MainGrid.style.display="";this.getDivElement().style.display="none";this.getDivElement().style.display="";this._initFF();if(this.alignDivs)
this.alignDivs();if(this.alignStatMargins)
this.alignStatMargins();if(this.StatHeader)
{_igtbl_headerOrFooterHeight(this.StatHeader.Element)}
igtbl_browserWorkarounds.ieTabScrollBarAdjustment(this.Bands[0]);},"onCBSubmit",function()
{this.update();},"getProgressIndicator",function()
{return this._progressIndicator;},"_displayPI",function()
{if(!this.EnableProgressIndicator)
return;if(!this._piIndex)
this._piIndex=0;this._piIndex++;document.body.setAttribute("noOnBodyResize","true");window.setTimeout("document.body.removeAttribute(\"noOnBodyResize\")",100);this._progressIndicator.display();},"_hidePI",function()
{if(!this.EnableProgressIndicator)
return;if(this._piIndex)
this._piIndex--;if(this._piIndex)
return;document.body.setAttribute("noOnBodyResize","true");window.setTimeout("document.body.removeAttribute(\"noOnBodyResize\")",100);this._progressIndicator.hide();},"_generateSelArray",function()
{var activeRow=null;var activeCell=this.getActiveCell();if(!activeCell)
{activeRow=this.getActiveRow();if(activeRow)
activeCell=activeRow.getCell(0);}
if(!activeCell)
return null;var rowColl=activeCell.Row.OwnerCollection;var clipArray=new Array();var assignCell=function(cell,rowColl,clipObject)
{if(!cell)return clipObject;var colIndex=cell.Column.Index;var rowIndex=cell.Row.getIndex();if(rowColl!=cell.Row.OwnerCollection)
return clipObject;var clipArray=clipObject.clipArray;var l=clipObject.leftIndex,t=clipObject.topIndex,r=clipObject.rightIndex,b=clipObject.bottomIndex;if(clipArray.length==0)
{clipArray[0]=new Array();clipArray[0][0]=cell;l=r=colIndex;t=b=rowIndex;}
else
{if(t>rowIndex)
{for(var i=0;i<t-rowIndex;i++)
{var insElem=new Array();for(var j=0;j<=r-l;j++)
insElem.push(null);clipArray.unshift(insElem);}
t=rowIndex;}
if(b<rowIndex)
{for(var i=0;i<rowIndex-b;i++)
{var insElem=new Array();for(var j=0;j<=r-l;j++)
insElem.push(null);clipArray.push(insElem);}
b=rowIndex;}
if(l>colIndex)
{for(var i=0;i<clipArray.length;i++)
for(var j=0;j<l-colIndex;j++)
clipArray[i].unshift(null);l=colIndex;}
if(r<colIndex)
{for(var i=0;i<clipArray.length;i++)
for(var j=0;j<colIndex-r;j++)
clipArray[i].push(null);r=colIndex;}
clipArray[rowIndex-t][colIndex-l]=cell;}
clipObject.leftIndex=l;clipObject.topIndex=t;clipObject.rightIndex=r;clipObject.bottomIndex=b;return clipObject;};var clipObject={"clipArray":clipArray,"leftIndex":-1,"rightIndex":-1,"topIndex":-1,"bottomIndex":-1};for(var cellID in this.SelectedCells)
{var cell=igtbl_getCellById(cellID);clipObject=assignCell(cell,rowColl,clipObject);}
for(var rowID in this.SelectedRows)
{var row=igtbl_getRowById(rowID);if(row.OwnerCollection!=rowColl)
continue;for(var i=0;i<row.cells.length;i++)
clipObject=assignCell(row.getCell(i),rowColl,clipObject);}
for(var colID in this.SelectedColumns)
{var col=igtbl_getColumnById(colID);if(col.Band.Index!=rowColl.Band.Index)
continue;var cols=igtbl_getDocumentElement(colID);if(!cols.length)cols=[cols];for(var i=0;i<cols.length;i++)
{var cells=igtbl_enumColumnCells(this.Id,cols[i]);if(cells&&cells.length)
{for(var j=0;j<cells.length;j++)
clipObject=assignCell(igtbl_getCellByElement(cells[j]),rowColl,clipObject);}}}
if(clipArray.length==0&&(activeRow||activeCell))
{clipArray[0]=new Array();if(activeCell)
clipArray[0][0]=activeCell;else
for(var i=0;i<activeRow.cells.length;i++)
clipArray[0][i]=activeRow.getCell(i);}
return clipArray;},"_getSelectedCells",function()
{var selArray=new Array();for(var cellID in this.SelectedCells)
{var cell=igtbl_getCellById(cellID);selArray.push(cell);}
for(var rowID in this.SelectedRows)
{var row=igtbl_getRowById(rowID);for(var i=0;i<row.cells.length;i++)
selArray.push(row.getCell(i));}
for(var colID in this.SelectedColumns)
{var col=igtbl_getColumnById(colID);var cols=igtbl_getDocumentElement(colID);if(!cols.length)cols=[cols];for(var i=0;i<cols.length;i++)
{var cells=igtbl_enumColumnCells(this.Id,cols[i]);if(cells&&cells.length)
{for(var j=0;j<cells.length;j++)
selArray.push(igtbl_getCellByElement(cells[j]));}}}
if(selArray.length==0)
{var activeCell=this.getActiveCell();var activeRow=this.getActiveRow();if(activeCell)
selArray.push(activeCell);else if(activeRow)
for(var i=0;i<activeRow.cells.length;i++)
selArray.push(activeRow.getCell(i));}
return selArray;},"copy",function(copytext,cutting)
{if(typeof(cutting)=="undefined")cutting=false;var clipArray=true;var options={"copyFormatted":false};if(!copytext)
{if(this.fireEvent(this.Events.BeforeClipboardOperation,[this.Id,(!cutting?this.eClipboardOperation.Copy:this.eClipboardOperation.Cut),options])===true)
return false;copytext="";clipArray=this._generateSelArray();for(var i=0;clipArray&&i<clipArray.length;i++)
{var cLine=clipArray[i];for(var j=0;j<cLine.length;j++)
{if(cLine[j]!=null)
{var v=cLine[j].getValue(options.copyFormatted);if(v!==null&&v!==undefined)
copytext+=v.toString();}
if(j<cLine.length-1)
copytext+='\t';}
copytext+="\r\n";}}
if(!copytext)
{var oError=new Object();oError.OperationType=cutting?this.eClipboardOperation.Cut:this.eClipboardOperation.Copy;oError.Data=null;oError.Options=options;this.fireEvent(this.Events.ClipboardError,[this.Id,this.eClipboardError.NothingToCopy,oError]);return false;}
var copyFailed=false;var excMessage;try
{if(!igtbl_setClipboardData(copytext))
{var oError=new Object();oError.OperationType=cutting?this.eClipboardOperation.Cut:this.eClipboardOperation.Copy;oError.Data=copytext;oError.Options=options;this.fireEvent(this.Events.ClipboardError,[this.Id,this.eClipboardError.NotSupported,"",oError]);return false;}}
catch(exc)
{copyFailed=true;excMessage=exc;}
if(copyFailed)
{var oError=new Object();oError.Options=options;oError.OperationType=cutting?this.eClipboardOperation.Cut:this.eClipboardOperation.Copy;oError.Data=copytext;this.fireEvent(this.Events.ClipboardError,[this.Id,this.eClipboardError.Failure,excMessage,oError]);return false;}
if(!cutting)
this.fireEvent(this.Events.AfterClipboardOperation,[this.Id,this.eClipboardOperation.Copy,clipArray]);return clipArray;},"cut",function()
{var clipArray=this.copy(null,true);if(clipArray&&clipArray!==true&&clipArray.length)
{for(var i=0;i<clipArray.length;i++)
{var cLine=clipArray[i];for(var j=0;j<cLine.length;j++)
{var cell=cLine[j];if(cell!=null&&cell.isEditable())
{if(cell.Column.AllowNull)
cell.setValue(null);else if(typeof(cell.Column.DefaultValue)!="undefined"&&cell.Column.DefaultValue!==null)
cell.setValue(cell.Column.DefaultValue);}}}
this.fireEvent(this.Events.AfterClipboardOperation,[this.Id,this.eClipboardOperation.Cut,clipArray]);return clipArray;}
return false;},"paste",function()
{var options={"strictPaste":false,"selectPastedCells":true,"ignoreHiddenColumns":false,"ignoreServerOnlyCells":true};if(this.fireEvent(this.Events.BeforeClipboardOperation,[this.Id,this.eClipboardOperation.Paste,options])===true)
return false;var pasteFailed=false;var pasteText=null;try
{pasteText=igtbl_getClipboardData();if(pasteText==undefined)
{var oError=new Object();oError.Options=options;oError.OperationType=this.eClipboardOperation.Paste;oError.Data=null;this.fireEvent(this.Events.ClipboardError,[this.Id,this.eClipboardError.NotSupported,"",oError]);return false;}}
catch(exc)
{var oError=new Object();oError.Options=options;oError.OperationType=this.eClipboardOperation.Paste;oError.Data=null;this.fireEvent(this.Events.ClipboardError,[this.Id,this.eClipboardError.Failure,exc,oError]);return false;}
if(!pasteText)
{var oError=new Object();oError.Options=options;oError.OperationType=this.eClipboardOperation.Paste;oError.Data=null;this.fireEvent(this.Events.ClipboardError,[this.Id,this.eClipboardError.NothingToPaste,"",oError]);return false;}
var clipArray=this.processPastedText(pasteText,options);this.fireEvent(this.Events.AfterClipboardOperation,[this.Id,this.eClipboardOperation.Paste,clipArray]);return clipArray;},"processPastedText",function(pasteText,options,newLineDelimiter)
{if(!options)options={"strictPaste":false,"selectPastedCells":true,"ignoreHiddenColumns":false,"ignoreServerOnlyCells":true};if(!newLineDelimiter)newLineDelimiter="\n";var clipArray=pasteText.split(newLineDelimiter);for(var i=clipArray.length-1;i>=0;i--)
if(clipArray[i])
{var lineStr=clipArray[i];if(lineStr.substr(lineStr.length-1,1)=="\r")
lineStr=lineStr.substr(0,lineStr.length-1);clipArray[i]=lineStr.split("\t");}
if(clipArray.length>1&&!clipArray[clipArray.length-1])
clipArray.pop();var cell;if(clipArray.length==1&&clipArray[0].length==1)
{var v=clipArray[0][0];var clipArray=this._getSelectedCells();for(var i=0;i<clipArray.length;i++)
{cell=clipArray[i];if(!cell)continue;if(v)
{if(cell.isEditable())
cell.setValue(v);}
else
{if(cell.isEditable())
{if(cell.Column.AllowNull)
cell.setValue(null);else if(typeof(cell.Column.DefaultValue)!="undefined"&&cell.Column.DefaultValue!==null)
cell.setValue(cell.Column.DefaultValue);}}}}
else
{var activeRow=null;var activeCell=this.getActiveCell();if(!activeCell)
{activeRow=this.getActiveRow();if(activeRow)
activeCell=activeRow.getCell(0);}
if(!activeCell)
{var oError=new Object();oError.OperationType=this.eClipboardOperation.Paste;oError.Data=pasteText;this.fireEvent(this.Events.ClipboardError,[this.Id,this.eClipboardError.NoActiveObject,"",oError]);return false;}
if(clipArray.length&&(clipArray.length>1||clipArray[0].length>1))
{var curSelArray=this._generateSelArray();if(curSelArray&&curSelArray.length&&(curSelArray.length>1||curSelArray[0].length>1)&&curSelArray.length==clipArray.length&&curSelArray[0].length==clipArray[0].length)
activeCell=curSelArray[0][0];}
var sct=activeCell.Band.getSelectTypeCell();if(options.selectPastedCells&&sct==3)
this.clearSelectionAll();var row=activeCell.Row;var cellIndex=activeCell.Index;for(var i=0;row&&i<clipArray.length;i++)
{var cell=row.getCell(cellIndex);if(clipArray[i])
for(var j=0;cell&&j<clipArray[i].length;j++)
{if(!cell.Element)
{cell=cell.getNextCell();if(options.ignoreServerOnlyCells)
j--;continue;}
if(clipArray[i][j])
{if(cell.isEditable()&&(!options.ignoreHiddenColumns||!cell.Column.getHidden()))
{cell.setValue(clipArray[i][j]);if(options.selectPastedCells&&sct==3)
igtbl_selectCell(this.Id,cell,true);}
clipArray[i][j]=cell;}
else
{clipArray[i][j]=null;if(options.strictPaste&&cell.isEditable()&&(!options.ignoreHiddenColumns||!cell.Column.getHidden()))
{if(cell.Column.AllowNull)
cell.setValue(null);else if(typeof(cell.Column.DefaultValue)!="undefined"&&cell.Column.DefaultValue!==null)
cell.setValue(cell.Column.DefaultValue);if(options.selectPastedCells&&sct==3)
igtbl_selectCell(this.Id,cell,true);}}
cell=cell.getNextCell();}
row=row.getNextRow();}}
return clipArray;}];for(var i=0;i<igtbl_ptsGrid.length;i+=2)
igtbl_Grid.prototype[igtbl_ptsGrid[i]]=igtbl_ptsGrid[i+1];function _igtbl_findSortImage(nodes)
{if(nodes==null)return null;for(var imgNodeIndex=0;imgNodeIndex<nodes.length;imgNodeIndex++)
{var currentNode=nodes[imgNodeIndex];if(currentNode.tagName=="IMG"&&currentNode.getAttribute("imgType")=="sort")
return currentNode;else
{var lowerNode=_igtbl_findSortImage(currentNode.childNodes);if(lowerNode)return lowerNode;}}
return null;}
igtbl_Row.prototype=new igtbl_WebObject();igtbl_Row.prototype.constructor=igtbl_Row;igtbl_Row.base=igtbl_WebObject.prototype;function igtbl_Row(element,node,rows,index)
{if(arguments.length>0)
this.init(element,node,rows,index);}
var igtbl_ptsRow=["init",function(element,node,rows,index)
{igtbl_Row.base.init.apply(this,["row",element,node]);var gs=rows.Band.Grid;var gn=gs.Id;this.gridId=gs.Id;var row=this.Element;row.Object=this;this.OwnerCollection=rows;if(this.OwnerCollection)
this.ParentRow=this.OwnerCollection.ParentRow;else
this.ParentRow=null;this.Band=this.OwnerCollection.Band;this.GroupByRow=false;this.GroupColId=null;if(row.getAttribute("groupRow"))
{this.GroupByRow=true;this.GroupColId=row.getAttribute("groupRow");var sTd=row.childNodes[0].childNodes[0].tBodies[0].childNodes[0].childNodes[0];this.MaskedValue=sTd.getAttribute("cellValue");this.Value=this.MaskedValue;if(sTd.getAttribute(igtbl_sUnmaskedValue))
this.Value=sTd.getAttribute(igtbl_sUnmaskedValue);this.Value=igtbl_getColumnById(this.GroupColId).getValueFromString(this.Value);}
var fr=igtbl_getFirstRow(row);this.Expandable=((fr.nextSibling&&fr.nextSibling.getAttribute("hiddenRow")||this.Element.getAttribute("showExpand")));this.ChildRowsCount=0;this.VisChildRowsCount=0;if(this.Expandable)
{if(fr.nextSibling&&fr.nextSibling.getAttribute("hiddenRow"))
{this.HiddenElement=fr.nextSibling;if(this.getExpanded()&&!gs.ExpandedRows[this.Element.id])
gs.ExpandedRows[this.Element.id]=this;this.ChildRowsCount=igtbl_rowsCount(igtbl_getChildRows(gn,row));this.VisChildRowsCount=igtbl_visRowsCount(igtbl_getChildRows(gn,row));var rowsNode=(this.Node?this.Node.selectSingleNode("Rs"):null);this.Rows=new igtbl_Rows(rowsNode,gs.Bands[rows.Band.Index+(this.GroupByRow?0:1)],this);var rowIslandFilters=null;if(rowsNode)
rowIslandFilters=eval(rowsNode.getAttribute("columnFilters"));if(rowIslandFilters)
_igtbl_processServerPassedColumnFilters(rowIslandFilters,gs);this.FirstChildRow=this.Rows.getRow(0);}}
this.FirstRow=fr;if(!this.GroupByRow)
{this.cells=new Array(this.Band.Columns.length);if(gs.UseFixedHeaders)
{for(var i=0;i<this.Element.cells.length;i++)
{if(this.Element.cells[i].childNodes.length>0&&this.Element.cells[i].firstChild.tagName=="DIV"&&this.Element.cells[i].firstChild.id.substr(this.Element.cells[i].firstChild.id.length-4)=="_drs")
{this.nfElement=this.Element.cells[i].firstChild.firstChild.childNodes[1].rows[0];this.nfElement.Object=this;break;}}}
if(!this.IsAddNewRow&&!this.IsFilterRow)
{var tr=this.Element;var cellId=this.Id.split("_");var lastIndex=cellId.length;cellId[1]="rc";var j=0;if(this.Band.Grid.Bands.length>1)j++;if(this.Band.getRowSelectors()<2)j++;var cols=this.Band.Columns;var nonFixed=false,colSpan=1;for(var i=0;i<cols.length;i++)
{if(colSpan>1)
{colSpan--;continue;}
if(cols[i].getFixed()===false&&!nonFixed)
{tr=this.nfElement;j=0;nonFixed=true;}
if(cols[i].hasCells())
{if(tr&&tr.cells[j]&&!tr.cells[j].id)
{cellId[lastIndex]=cols[i].Index.toString();tr.cells[j].id=cellId.join("_");colSpan=tr.cells[j].colSpan;}
j++;}}}}
if(this.Node)
{if(!this.Expandable)
this.Expandable=this.Node.selectSingleNode("Rs")!=null||this.Node.getAttribute("showExpand")=="true";}
if(this.Node)
{this.DataKey="";if(this.get(igtbl_litPrefix+"DataKey"))
this.DataKey=unescape(this.get(igtbl_litPrefix+"DataKey"));}
else
{if(this.Element.getAttribute(igtbl_litPrefix+"DataKey"))
this.DataKey=unescape(this.Element.getAttribute(igtbl_litPrefix+"DataKey"));}
this.Expanded=this.getExpanded();this._Changes=new Object();this._dataChanged=0;if(gs.ExpandedRows[this.Id])
{var stateChange=gs.ExpandedRows[this.Id];stateChange.Object=this;gs.ExpandedRows[this.Id]=this;if(this.DataKey)
{var value=this.DataKey;if(value==""&&typeof(value)=="string")value="\x01";ig_ClientState.setPropertyValue(stateChange.Node,"Value",value);}
this._Changes[stateChange.Type]=stateChange;}},"getDataKey",function()
{if(typeof(this.DataKey)=="undefined"||this.DataKey===null)return null;var dKey=this.DataKey.split('\x07');return dKey;},"getIndex",function(virtual)
{if(this.Node)
{var index=igtbl_parseInt(this.Node.getAttribute("i"));var g=this.Band.Grid;if(this.Band.Index==0&&!virtual&&g.XmlLoadOnDemandType==2)
{var firstRow=g.Rows.getRow(0);var topRow;if(firstRow)
topRow=igtbl_parseInt(firstRow.Node.getAttribute("i"));else
{var de=g.getDivElement();topRow=Math.floor(de.scrollTop/g.getDefaultRowHeight());}
index-=topRow;}
return index;}
else if(this.OwnerCollection)
return this.OwnerCollection.indexOf(this);return-1;},"toggleRow",function()
{this.setExpanded(!this.getExpanded());},"getExpanded",function(expand)
{return(this.Expandable&&this.HiddenElement&&this.HiddenElement.style.display=="");},"setExpanded",function(expand)
{if(this.Band.getExpandable()!=1||!this.Expandable)
return;if(expand!=false)
expand=true;var gn=this.gridId;if(expand==this.getExpanded())
{if(expand&&!this._Changes["ExpandedRows"])
igtbl_stateExpandRow(gn,this,expand);return;}
var gs=igtbl_getGridById(gn);if(gs.isDisabled())return;if(igtbl_inEditMode(gn))
{var elem=gs._editorCurrent;if(elem&&elem.getAttribute("noOnBlur"))
elem.removeAttribute("noOnBlur");igtbl_hideEdit(gn);}
if(gs._scrElem&&gs.IsXHTML&&this.GroupByRow&&expand&&!this.HiddenElement)
gs._scrElem.scrollLeft=0;var rcrRes=true;if(gs.LoadOnDemand==3&&!this.HiddenElement)
rcrRes=this.requestChildRows();if(rcrRes)
this._setExpandedComplete(expand);if(this.Node&&ig_csom.IsIE)
{var row=this;var rowElement=this.Element;while(rowElement=rowElement.nextSibling)
{if(!igtbl_string.isNullOrEmpty(rowElement,"hiddenRow")||!igtbl_string.isNullOrEmpty(rowElement,"groupRow"))
{rowElement.style.position="";rowElement.style.position="relative";}}}
if(gs._editorButton&&gs.oActiveCell&&gs.oActiveCell.Row.ParentRow&&gs.oActiveCell.hasButtonEditor(igtbl_cellButtonDisplay.OnMouseEnter))
{if(expand==false&&gs._editorButton.style.display!="none"&&gs.oActiveCell.Row.ParentRow==this)
igtbl_showColButton(gn,"hide");else if(gs.oActiveCell.Row.ParentRow!=this&&gs._editorButton.style.display!="none"||gs.oActiveCell.Row.ParentRow==this&&gs._editorButton.style.display=="none")
igtbl_showColButton(gn,gs.oActiveCell.Element);}},"_setExpandedComplete",function(expand)
{var gn=this.gridId;var gs=igtbl_getGridById(gn);if(this.Node)
{var rsn=this.Node.selectSingleNode("Rs");if(!this.Rows)
{if(this.GroupByRow||gs.Bands.length>this.Band.Index+1)
this.Rows=new igtbl_Rows(rsn,gs.Bands[this.Band.Index+(this.GroupByRow?0:1)],this);var rowIslandFilters=null;if(rsn)
rowIslandFilters=eval(rsn.getAttribute("columnFilters"));if(rowIslandFilters)
_igtbl_processServerPassedColumnFilters(rowIslandFilters,gs);}
if(!this.HiddenElement&&this.Rows)
{this.prerenderChildRows();if(rsn)
this.Rows.render();}
if(gs.LoadOnDemand!=3&&this.Rows&&typeof(this.Rows.Band.SortedColumns)!="undefined"&&igtbl_getLength(this.Rows.Band.SortedColumns)>0)
{if(!(this.GroupByRow&&igtbl_getColumnById(this.GroupColId).Id==this.Band.SortedColumns[this.Band.SortedColumns.length-1]))
this.Rows.sort();}}
else if(!this.Rows)
{if(this.GroupByRow||gs.Bands.length>this.Band.Index+1)
this.Rows=new igtbl_Rows(null,gs.Bands[this.Band.Index+(this.GroupByRow?0:1)],this);if((gs.LoadOnDemand==0||gs.LoadOnDemand==3)&&this.Rows)
this.prerenderChildRows();}
var srcRow=this.getFirstRow().id;var sr=igtbl_getElementById(srcRow);var hr=this.HiddenElement;var cancel=false;if(expand!=false)
{if(igtbl_fireEvent(gn,gs.Events.BeforeRowExpanded,"(\""+gn+"\",\""+srcRow+"\");")==true)
cancel=true;if(!cancel)
{if(ig_csom.IsFireFox&&this.GroupByRow)
{var cr=this;while(cr&&cr.GroupByRow)
{if(!cr._origHeight)
cr._origHeight=cr.FirstRow.offsetHeight;cr=cr.ParentRow;}}
if(!gs.NeedPostBack||gs.LoadOnDemand!=0&&this.Rows&&(this.Rows.length>0||this.Rows.AddNewRow||this.Rows.hasRowFilters()))
{gs.NeedPostBack=false;if(hr)
{hr.style.display="";hr.style.visibility="";if(ig_csom.IsIE6)
{var selectElements=hr.getElementsByTagName("select");for(var x=0;x<selectElements.length;x++)
{if(selectElements[x].getAttribute("beforeExpandDisplay")!=null)
{selectElements[x].style.display=selectElements[x].getAttribute("beforeExpandDisplay");selectElements[x].setAttribute("beforeExpandDisplay",null);}}}}
var eImg=sr.childNodes[0].childNodes[0];eImg.src=this.Band.getCollapseImage();var alt=eImg.getAttribute("alt");if(alt!=null)
{var clpsAlt=eImg.getAttribute("igAltC");if(clpsAlt!=null)
{eImg.setAttribute("igAltX",alt);eImg.setAttribute("alt",clpsAlt);eImg.removeAttribute("igAltC");}}}
igtbl_stateExpandRow(gn,this,true);if(!gs.NeedPostBack)
igtbl_fireEvent(gn,gs.Events.AfterRowExpanded,"(\""+gn+"\",\""+srcRow+"\");");if(gs.AddNewBoxVisible)
gs.updateAddNewBox();}}
else
{if(igtbl_fireEvent(gn,gs.Events.BeforeRowCollapsed,"(\""+gn+"\",\""+srcRow+"\")")==true)
cancel=true;if(!cancel)
{if(!gs.NeedPostBack)
{if(hr)
{hr.style.display="none";hr.style.visibility="hidden";if(ig_csom.IsIE6)
{var selectElements=hr.getElementsByTagName("select");for(var x=0;x<selectElements.length;x++)
{if(selectElements[x].style.display!="none")
{selectElements[x].setAttribute("beforeExpandDisplay",selectElements[x].style.display);selectElements[x].style.display="none";}
else
selectElements[x].setAttribute("beforeExpandDisplay",null);}}}
var eImg=sr.childNodes[0].childNodes[0];eImg.src=this.Band.getExpandImage();var alt=eImg.getAttribute("alt");if(alt!=null)
{var xpAlt=eImg.getAttribute("igAltX");if(xpAlt!=null)
{eImg.setAttribute("igAltC",alt);eImg.setAttribute("alt",xpAlt);eImg.removeAttribute("igAltX");}}}
igtbl_stateExpandRow(gn,this,false);var cr=this;while(cr)
{if(cr.GroupByRow&&cr._origHeight)
{cr.Element.firstChild.firstChild.style.height=cr._origHeight+"px";}
cr=cr.ParentRow;}
if(!gs.NeedPostBack)
igtbl_fireEvent(gn,gs.Events.AfterRowCollapsed,"(\""+gn+"\",\""+srcRow+"\");");}}
if(!cancel)
{if(gs.NeedPostBack)
{if(expand!=false)
igtbl_moveBackPostField(gn,"ExpandedRows");else
igtbl_moveBackPostField(gn,"CollapsedRows");}}
if(gs.XmlLoadOnDemandType!=2)
{if(gs.UseFixedHeaders||gs.XmlLoadOnDemandType!=0)
{if(this.GroupByRow)
hr=this.Rows.getRow(0).Element;var tableWidth=(hr&&hr.lastChild.firstChild&&hr.lastChild.firstChild.id==gs.Id+"_drs")?hr.lastChild.firstChild.firstChild.offsetWidth:this.Element.offsetWidth;for(var iCells=0;hr&&iCells<hr.childNodes.length-1;iCells++)
{var hc=hr.childNodes[iCells];tableWidth+=hc.offsetWidth;}
if(this.Band.Index>0)
{for(var iBandsIndex=this.Band.Index;iBandsIndex>=0;iBandsIndex--)
{tableWidth+=gs.Bands[iBandsIndex].getIndentation();}}
if(gs.GroupCount>0)
tableWidth+=gs.Bands[0].getIndentation()*gs.GroupCount;var divs=gs._scrElem;if(this.OwnerCollection.Grid.Element.offsetHeight>0&&divs&&divs.firstChild.offsetWidth<tableWidth)
{divs.setAttribute("scrollDivWidth",tableWidth);}}
gs.alignDivs();}
if(!gs.UseFixedHeaders&&(gs.StatHeader||gs.StatFooter))
gs.alignStatMargins();if(gs.NeedPostBack)
igtbl_doPostBack(gn);},"getFirstRow",function()
{return igtbl_getFirstRow(this.Element);},"requestChildRows",function()
{if(this.Rows)
if(this.Node)
{if(this.Rows.Node)
return true;}
else
return true;var g=this.Band.Grid;if(this.Node&&this.Node.selectSingleNode("Rs"))
return true;g.QueryString="LODXml\x01"+this._buildChildRowsQuery();g.RowToQuery=this;g.xmlHttpRequest(g.eReqType.ChildRows,!g.GridIsLoaded);return false;},"_buildChildRowsQuery",function()
{var g=this.Band.Grid;var sqlWhere="";var sortOrder="";var newLevel="";for(var i=0;i<=this.Band.Index;i++)
{var cr=this;while(cr&&cr.Band!=g.Bands[i])
cr=cr.ParentRow;if(g.Bands[i].DataKeyField&&cr.get(igtbl_litPrefix+"DataKey"))
{sqlWhere+=cr._generateSqlWhere(g.Bands[i].DataKeyField,unescape(cr.get(igtbl_litPrefix+"DataKey")));if(newLevel!=null)
newLevel+=(i>0?"_":"")+cr.getIndex().toString();}
else
newLevel=null;sqlWhere+=(i==this.Band.Index?"":";");}
var queryString=(newLevel==null?this.getLevel(true):newLevel);for(var i=0;i<g.Bands.length;i++)
{var so="";for(var j=0;j<g.Bands[i].SortedColumns.length;j++)
{var col=igtbl_getColumnById(g.Bands[i].SortedColumns[j]);so+=col.Key+(col.SortIndicator==2?" DESC":"")+(j<g.Bands[i].SortedColumns.length-1?",":"");}
sortOrder+=so+(i==g.Bands.length-1?"":";");}
var band=g.Bands[this.Band.Index+1],sCols;if(band)
{sCols=band.Index;for(var i=0;i<band.SortedColumns.length;i++)
{var col=igtbl_getColumnById(band.SortedColumns[i]);sCols+="|"+col.Index;sCols+=":"+col.IsGroupBy.toString();sCols+=":"+col.SortIndicator;}}
var bandFilter="";if(this.Band.RowFilterMode==1)
{bandFilter=g._getCurrentFiltersString(band.Columns[0],band);}
queryString+="\x02"+sqlWhere;queryString+="\x02"+sortOrder;queryString+="\x02";if(band&&band.ColumnsOrder)
queryString+=band.ColumnsOrder;queryString+="\x02"+sCols;queryString+="\x02"+bandFilter;var filterString="";for(var x=0;x<=this.Band.Index;x++)
{var b=this.Band.Grid.Bands[x];filterString+=g._getCurrentFiltersString(b.Columns[0],b,this.Id);}
queryString+="\x02"+filterString;this.Band.Grid.NeedPostBack=false;return queryString;},"_generateBandsSqlWhere",function(band)
{var oSqlWhere=new Object();var g=band.Grid;oSqlWhere.sqlWhere="";oSqlWhere.newLevel="";for(var i=0;i<=band.Index;i++)
{var cr=this;while(cr&&cr.Band!=g.Bands[i])
cr=cr.ParentRow;if(g.Bands[i].DataKeyField&&cr.get(igtbl_litPrefix+"DataKey"))
{oSqlWhere.sqlWhere+=cr._generateSqlWhere(g.Bands[i].DataKeyField,unescape(cr.get(igtbl_litPrefix+"DataKey")));if(oSqlWhere.newLevel!=null)
oSqlWhere.newLevel+=(i>0?"_":"")+cr.getIndex().toString();}
else
{if(this.Band.IsGrouped)
{if(oSqlWhere.newLevel!=null)
oSqlWhere.newLevel+=(i>0?"_":"")+cr.getIndex().toString();}
else
{oSqlWhere.newLevel=null;}}
oSqlWhere.sqlWhere+=(i==this.Band.Index?"":";");}
return oSqlWhere;},"prerenderChildRows",function()
{if(!this.HiddenElement)
{var g=this.Band.Grid;var band=this.Rows.Band;if(!band.Visible)return;var hidRow=document.createElement("tr");this.HiddenElement=hidRow;if(!this.GroupByRow)
{if(this.Element.nextSibling)
this.Element.parentNode.insertBefore(this.HiddenElement,this.Element.nextSibling);else
this.Element.parentNode.appendChild(this.HiddenElement);}
else
this.getFirstRow().parentNode.appendChild(this.HiddenElement);var rn=this.Element.id.split("_");rn[0]=this.gridId+"rh";hidRow.id=rn.join("_");hidRow.setAttribute("hiddenRow",true);hidRow.setAttribute("groupRow",this.GroupColId);hidRow.style.position="relative";var majCell;var img;var tBody;var childGroupRows=(this.Rows.Node&&this.Rows.SelectedNodes[0]&&this.Rows.SelectedNodes[0].nodeName=="Group");if(this.GroupByRow)
{var majCell=document.createElement("td");hidRow.appendChild(majCell);majCell.style.paddingLeft=this.Band.getIndentation()+"px";}
else
{if(band.IndentationType!=2)
{var ec=document.createElement("th");hidRow.appendChild(ec);if(!band._optSelectRow)
ec.className=this.Band.getExpAreaClass();ec.style.borderWidth=0;ec.style.textAlign="center";ec.style.padding=0;ec.style.cursor="default";ec.style.height="auto";ec.innerHTML="&nbsp;";if(this.Band.getRowSelectors()==1)
{var rsc=document.createElement("th");hidRow.appendChild(rsc);rsc.className=this.Band.getRowLabelClass();rsc.style.height="auto";img=document.createElement("img");img.src=g.BlankImage;img.border=0;img.style.visibility="hidden";rsc.appendChild(img);}}
majCell=document.createElement("td");hidRow.appendChild(majCell);majCell.style.overflow="auto";majCell.style.width="100%";majCell.style.border=0;majCell.colSpan=this.Band.VisibleColumnsCount+1+(this.Band.getRowSelectors()==1?1:0);if(g.UseFixedHeaders&&band._optSelectRow)
majCell.className=g.StopperStyle;}
if(!childGroupRows&&(band.HeaderHTML||band.FooterHTML))
{var str="<table>";if(band.HeaderHTML)
str+=band.HeaderHTML;str+="<tbody></tbody>";if(band.FooterHTML)
str+=band.FooterHTML;str+="</table>";majCell.innerHTML=str;table=majCell.firstChild;tBody=table.tBodies[0];}
else
table=document.createElement("table");rn[0]=this.gridId;rn[1]="t";table.id=rn.join("_");if(!ig_csom.IsIE)
table.width="1";table.border=0;table.cellPadding=g.Element.cellPadding;table.cellSpacing=g.Element.cellSpacing;table.setAttribute("bandNo",band.Index);table.style.position="relative";table.style.borderCollapse=this.Band.getBorderCollapse();if(band._wdth=="100%")
{table.width="100%";}
table.style.tableLayout="fixed";if(this.Rows&&this.Rows.Node&&this.Rows.Node.selectSingleNode("Group"))
table.style.tableLayout="auto";if(g.TableLayout!=1)
table.style.tableLayout="auto";if(childGroupRows)
{majCell.appendChild(table);table.width="100%";var tHead=document.createElement("thead");var tr=document.createElement("tr");var th=document.createElement("th");th.innerHTML="&nbsp;";tr.appendChild(th);tHead.appendChild(tr);tHead.style.display="none";table.appendChild(tHead);tBody=document.createElement("tbody");table.appendChild(tBody);}
else
{if(!band.HeaderHTML)
{majCell.appendChild(table);var colGr=document.createElement("colgroup");var col;var tableWidth=0;if(g.Bands.length>1)
{col=document.createElement("col");if(band.getIndentation()>0)
col.width=band.getIndentation();else
col.style.display="none";colGr.appendChild(col);if(col.width)
tableWidth+=parseInt(col.width,10);}
if(band.getRowSelectors()==1)
{col=document.createElement("col");col.width=(band.RowLabelWidth?band.RowLabelWidth:"22px");colGr.appendChild(col);if(col.width)
tableWidth+=parseInt(col.width,10);}
var tablePercWidth="";for(var i=0;i<band.Columns.length;i++)
{var co=band.Columns[i];if(co.getVisible())
{col=document.createElement("col");if(g.UseFixedHeaders&&!co.getFixed()&&co.Node&&co.Node.getAttribute(igtbl_litPrefix+"widthResolved"))
try
{col.width=co.Node.getAttribute(igtbl_litPrefix+"widthResolved");if(col.width.length>1&&col.width.substr(col.width.length-1)=="%")
tablePercWidth="100%";}catch(e){;}
else
try{col.width=co.getWidth();}catch(e){;}
colGr.appendChild(col);}}
for(var i=0;i<band.Columns.length;i++)
if(band.Columns[i].getHidden())
{col=document.createElement("col");col.width="1px";col.style.display="none";colGr.appendChild(col);}
if(table.childNodes.length>0)
table.insertBefore(colGr,table.childNodes[0]);else
table.appendChild(colGr);var tHead=document.createElement("thead");if(this.Band.Index==0&&this.Band.Grid.StatHeader&&this.GroupByRow&&g.get("StationaryMarginsOutlookGroupBy")=="True")
tHead.style.display="none";if(table.childNodes.length>1)
table.insertBefore(tHead,table.childNodes[1]);else
table.appendChild(tHead);igtbl_addEventListener(tHead,"mousedown",igtbl_headerClickDown);igtbl_addEventListener(tHead,"mouseup",igtbl_headerClickUp);igtbl_addEventListener(tHead,"mouseout",igtbl_headerMouseOut);igtbl_addEventListener(tHead,"mousemove",igtbl_headerMouseMove);igtbl_addEventListener(tHead,"mouseover",igtbl_headerMouseOver);igtbl_addEventListener(tHead,"contextmenu",igtbl_headerContextMenu);if(band._optSelectRow)
{tHead.className=band.getItemClass();tHead.className+=" ";tHead.className+=band.getHeadClass();}
var tr=document.createElement("tr");tHead.appendChild(tr);var th;if(g.Bands.length>1)
{th=document.createElement("th");if(!band._optSelectRow)
th.className=band.NonSelHeaderClass;th.height=band.DefaultRowHeight;img=document.createElement("img");img.src=g.BlankImage;img.border=0;th.appendChild(img);tr.appendChild(th);}
if(band.getRowSelectors()==1)
{th=document.createElement("th");if(!band._optSelectRow)
th.className=band.NonSelHeaderClass;th.height=band.DefaultRowHeight;img=document.createElement("img");img.src=g.GridCornerImage?g.GridCornerImage:g.BlankImage;igtbl_addEventListener(img,"click",igtbl_GridCornerClick);img.setAttribute("gridName",g.UniqueID);img.border=0;th.appendChild(img);tr.appendChild(th);}
var nfrow=null;var setHeight=false;for(var i=0;i<band.Columns.length;i++)
{var column=band.Columns[i];if(column.hasCells())
{th=document.createElement("th");th.id=this.gridId+"_c"+"_"+band.Index+"_"+i.toString();th.setAttribute("columnNo",i);if(column.getHidden())
th.style.display="none";var headerNode=null;if(column.Node)
{headerNode=column.Node.selectSingleNode("Header");var titleAttrib;if(headerNode&&(titleAttrib=headerNode.getAttribute(igtbl_litPrefix+"title")))
th.setAttribute("title",unescape(titleAttrib))}
var colHeadImg="";var colHeadImgUrl;var colHeadImgAltText;var colHeadImgHeight;var colHeadImgWidth;if(headerNode)
{colHeadImgUrl=headerNode.getAttribute("ImageUrl");colHeadImgAltText=headerNode.getAttribute("ImageAltText");colHeadImgHeight=headerNode.getAttribute("ImageHeight");colHeadImgWidth=headerNode.getAttribute("ImageWidth");}
else
{colHeadImgUrl=column.HeaderImageUrl;colHeadImgAltText=column.HeaderImageAltText;colHeadImgHeight=column.HeaderImageHeight;colHeadImgWidth=column.HeaderImageWidth;}
if(colHeadImgUrl||colHeadImgAltText)
{colHeadImg="<img";if(colHeadImgUrl)
colHeadImg+=" src="+unescape(colHeadImgUrl);if(colHeadImgAltText)
colHeadImg+=" alt="+unescape(colHeadImgAltText);if(colHeadImgHeight)
colHeadImg+=" Height="+colHeadImgHeight;if(colHeadImgWidth)
colHeadImg+=" Width="+colHeadImgWidth;colHeadImg+=">";}
var filterImage="";if(column.AllowRowFiltering>=2&&column.Band.FilterUIType==2)
{var useAppliedImage=false;if(column.RowFilterMode==1||column.Band.Index==0)
{var filterPanel=g.Bands[column.Band.Index]._filterPanels[column.Id];useAppliedImage=(filterPanel&&filterPanel.getOperator()>0);}
else
{var innerTableId=this.Id.replace("_r_","_t_");var filterPanel=band._filterPanels[innerTableId];useAppliedImage=filterPanel&&filterPanel[column.Id]&&filterPanel[column.Id].getOperator()>0;}
filterImage="<img src='";filterImage+=(useAppliedImage?g.FilterAppliedImage:g.FilterDefaultImage);filterImage+="' border='0px' imgType='filter'";filterImage+=" onmousedown='javascript:ig_cancelEvent(event);'";filterImage+=" onmouseup='javascript:igtbl_showFilterOptions(\""+column.Id+"\",event);'";if(!column.getFilterIcon())
{filterImage+="style=\"display:none\"";}
filterImage+=" imgType=\"filter\">";}
var ht="";if(colHeadImg.length>0)
ht+=colHeadImg;var headerText=column.HeaderText;if(!column.HeaderWrap)
ht+="<nobr>"+(headerText?headerText:"&nbsp;");else
ht+=column.HeaderText;ht+=filterImage;var sortIndImg="";switch(column.SortIndicator)
{case 1:sortIndImg="&nbsp;<img src='"+g.SortAscImg+"' alt='"+g.SortAscAltText+"' border='0' height='12' width='12' imgType='sort'>";break;case 2:sortIndImg="&nbsp;<img src='"+g.SortDscImg+"' alt='"+g.SortDescAltText+"' border='0' height='12' width='12' imgType='sort'>";break;}
ht+=sortIndImg;if(g.UseFixedHeaders&&column.getFixedHeaderIndicator()==2)
{if(column.Fixed)
ht+="&nbsp;<img src='"+g.FixedHeaderOnImage+"' alt='"+g._fixedHeaderOnAltText+"' border='0' width='12' height='12' imgType='fixed' onclick='igtbl_fixedClick(event)'>";else
ht+="&nbsp;<img src='"+g.FixedHeaderOffImage+"' alt='"+g._fixedHeaderOffAltText+"' border='0' width='12' height='12' imgType='fixed' onclick='igtbl_fixedClick(event)'>";}
if(!column.HeaderWrap)
ht+="</nobr>";if(g.UseFixedHeaders&&!column.Fixed&&!nfrow)
{var nftd=document.createElement("th");nftd.colSpan=band.Columns.length-column.Index;if(!g.IsXHTML)
nftd.width="100%";else
{nftd.style.verticalAlign="top";setHeight=true;}
nftd.style.textAlign="left";if(band._optSelectRow)
nftd.className=g.StopperStyle;tr.appendChild(nftd);var nfd=document.createElement("div");nftd.appendChild(nfd);nfd.id=g.Id+"_drs";nfd.style.overflow="hidden";if(!g.IsXHTML)
nfd.style.width="100%";nfd.style.height="100%";if(g.IsXHTML)
nfd.style.position="relative";var nftable=document.createElement("table");nfd.appendChild(nftable);if(!ig_csom.IsIE)
nftable.width="1";nftable.border="0";nftable.cellPadding=g.Element.cellPadding;nftable.cellSpacing=g.Element.cellSpacing;nftable.style.position="relative";nftable.style.tableLayout="fixed";nftable.style.height="100%";var nfcgs=document.createElement("colgroup");nftable.appendChild(nfcgs);for(var j=column.Index;j<band.Columns.length;j++)
{if(band.Columns[j].getVisible())
{var nfcg=document.createElement("col");nfcg.width=band.Columns[j].Width;nfcgs.appendChild(nfcg);}}
for(var j=column.Index;j<band.Columns.length;j++)
{if(band.Columns[j].getHidden())
{var nfcg=document.createElement("col");nfcg.width="1px";nfcg.style.display="none";nfcgs.appendChild(nfcg);}}
if(g._scrElem.scrollLeft)
nftable.style.left=(-g._scrElem.scrollLeft).toString()+"px";var nftb=document.createElement("tbody");nftable.appendChild(nftb);nfrow=document.createElement("tr");nftb.appendChild(nfrow);}
{if(!band._optSelectRow)
th.className=column.getHeadClass();else if(this.HeaderClass)
th.className=this.HeaderClass;if(column.HeaderStyle)
th.style.cssText=column.HeaderStyle;th.innerHTML=ht;}
if(nfrow)
{nfrow.appendChild(th);if(setHeight)
{var nftd=nfrow.parentNode.parentNode.parentNode.parentNode;nftd.style.height=nftd.parentNode.offsetHeight+"px";setHeight=false;}}
else
tr.appendChild(th);tableWidth+=column.getWidth();}}
if(nftable&&nfd&&nftable.offsetWidth>nfd.offsetWidth)
nfd.style.width=nftable.offsetWidth+'px';if(band.ColHeadersVisible!=1)
tHead.style.display="none";if(tablePercWidth)
table.style.width=tablePercWidth;if(table.tBodies.length==0)
{tBody=document.createElement("tbody");table.appendChild(tBody);if(band._optSelectRow)
{tBody.className=band.getItemClass();tBody.className+=" ";tBody.className+=band.getHeadClass();}}}
if(this.Rows.Band.FilterUIType==1&&this.Rows.Band.FilterRowView==1)
{var tr=document.createElement("tr");tBody.appendChild(tr);tr.id=this.gridId+"_flr_"+this.getLevel(true);tr.setAttribute("filterRow","true");if(band._optSelectRow)tr.className=band.getItemClass();var td;if(g.Bands.length>1)
{td=document.createElement("th");tr.appendChild(td);td.className=igtbl_getExpAreaClass(this.gridId,band.Index);td.style.height=band.DefaultRowHeight;img=document.createElement("img");td.appendChild(img);img.src=g.BlankImage;img.border=0;}
if(band.getRowSelectors()==1)
{td=document.createElement("th");tr.appendChild(td);td.className=igtbl_getRowLabelClass(this.gridId,band.Index);td.id=this.gridId+"_fll_"+this.getLevel(true);td.style.height=band.DefaultRowHeight;img=document.createElement("img");td.appendChild(img);img.src=g.BlankImage;img.border=0;}
var nfrow=null;setHeight=false;var filterTypeImage=null;var filterButtonImgString="";for(var i=0;i<band.Columns.length;i++)
{var column=band.Columns[i];if(column.hasCells())
{if(filterTypeImage==null||filterTypeImage[0]!=column.FilterOperatorDefaultValue)
{filterTypeImage=null;var filImgs=g.FilterButtonImages;for(var itr=0;itr<filImgs.length;itr++)
{if(column.FilterOperatorDefaultValue==filImgs[itr][0])
{filterTypeImage=filImgs[itr];break;}}
filterButtonImgString="<button onclick=\"igtbl_filterTypeSelect(event);\" class=\""+band.FilterOperandButtonStyle+" "+band.Grid.FilterOperandButtonStyle+"\" style=\"height:100%;padding:0px;\"><img src=\""+filterTypeImage[1]+"\" alt=\""+filterTypeImage[2]+"\" operator="+filterTypeImage[0]+" /></button><span></span>";}
var filterValue="";if(band.RowFilterMode==2&&band._filterPanels&&band._filterPanels[table.id]&&band._filterPanels[table.id][column.Id])
filterValue=band._filterPanels[table.id][column.Id].getEvaluationValue();else if(band._filterPanels&&band._filterPanels[column.Id])
filterValue=band._filterPanels[column.Id].getEvaluationValue();if(filterValue!="")
{filterButtonImgString=filterButtonImgString.replace("</span>",filterValue+"</span>");}
td=document.createElement("td");td.id=this.gridId+"_flc_"+this.getLevel(true)+"_"+i.toString();var ct=filterButtonImgString;td.className=band.FilterRowStyle+" "+band.Grid.FilterRowStyle;if(column.getHidden())
td.style.display="none";if(g.UseFixedHeaders&&!column.Fixed&&!nfrow)
{var nftd=document.createElement("td");nftd.colSpan=band.Columns.length-column.Index;if(band._optSelectRow)
{nftd.className=g.StopperStyle;if(g.IsXHTML)
setHeight=true;}
else
{if(!g.IsXHTML)
nftd.width="100%";else
{nftd.style.verticalAlign="top";setHeight=true;}}
tr.appendChild(nftd);var nfd=document.createElement("div");nftd.appendChild(nfd);nfd.id=g.Id+"_drs";nfd.style.overflow="hidden";if(!g.IsXHTML)
nfd.style.width="100%";nfd.style.height="100%";if(g.IsXHTML)
nfd.style.position="relative";var nftable=document.createElement("table");nfd.appendChild(nftable);nftable.border="0";nftable.cellPadding=g.Element.cellPadding;nftable.cellSpacing=g.Element.cellSpacing;nftable.style.position="relative";nftable.style.tableLayout="fixed";nftable.style.height="100%";var nfcgs=document.createElement("colgroup");nftable.appendChild(nfcgs);for(var j=column.Index;j<band.Columns.length;j++)
{if(band.Columns[j].getVisible())
{var nfcg=document.createElement("col");nfcg.width=band.Columns[j].Width;nfcgs.appendChild(nfcg);}}
for(var j=column.Index;j<band.Columns.length;j++)
{if(band.Columns[j].getHidden())
{var nfcg=document.createElement("col");nfcg.width="1px";nfcg.style.display="none";nfcgs.appendChild(nfcg);}}
if(g._scrElem.scrollLeft)
nftable.style.left=(-g._scrElem.scrollLeft).toString()+"px";var nftb=document.createElement("tbody");nftable.appendChild(nftb);nfrow=document.createElement("tr");nfrow.id=this.gridId+"_flfr_"+this.getLevel(true);nftb.appendChild(nfrow);}
if(column.CssClass&&!band._optSelectRow)
td.className+=(td.className.length>0?" ":"")+column.CssClass;td.innerHTML=ct;if(nfrow)
{nfrow.appendChild(td);if(setHeight)
{var nftd=nfrow.parentNode.parentNode.parentNode.parentNode;nftd.style.height=nftd.parentNode.offsetHeight+"px";setHeight=false;}}
else
tr.appendChild(td);}}
this.Rows.FilterRow=new igtbl_FilterRow(tr,this.Rows);}
if(!this.GroupByRow&&this.Rows.Band.AddNewRowVisible==1&&this.Rows.Band.AllowAddNew==1)
{var tr=document.createElement("tr");tBody.appendChild(tr);tr.id=this.gridId+"_anr_"
+this.getLevel(true);tr.setAttribute("addNewRow","true");if(band._optSelectRow)
tr.className=band.getItemClass();var td;if(g.Bands.length>1)
{td=document.createElement("th");tr.appendChild(td);td.className=igtbl_getExpAreaClass(this.gridId,band.Index);td.style.height=band.DefaultRowHeight;img=document.createElement("img");td.appendChild(img);img.src=g.BlankImage;img.border=0;}
if(band.getRowSelectors()==1)
{td=document.createElement("th");tr.appendChild(td);td.className=igtbl_getRowLabelClass(this.gridId,band.Index);td.id=this.gridId+"_anl_"
+this.getLevel(true);td.style.height=band.DefaultRowHeight;img=document.createElement("img");td.appendChild(img);img.src=g.BlankImage;img.border=0;}
var nfrow=null;setHeight=false;for(var i=0;i<band.Columns.length;i++)
{var column=band.Columns[i];if(column.hasCells())
{td=document.createElement("td");td.id=this.gridId+"_anc_"
+this.getLevel(true)+"_"+i.toString();var ct=column.DefaultValue;if(band.AddNewRowStyle)
td.style.cssText=band.AddNewRowStyle;if(column.getHidden())
td.style.display="none";if(!column.Wrap)
{switch(column.ColumnType)
{case 3:if(!ct||ct.toString().toLowerCase()=="false"||ct=="0")
ct=false;else
ct=true;ct="<nobr><input type='checkbox' tabIndex='-1' "+(ct?'CHECKED':'')+" on"+(ig_csom.IsIE?"property":"")+"change='igtbl_chkBoxChange(event,\""+g.Id+"\");'>";break;default:ct="<nobr>"+(ct?ct:"&nbsp;")+"</nobr>";break;}}
if(g.UseFixedHeaders&&!column.Fixed&&!nfrow)
{var nftd=document.createElement("td");nftd.colSpan=band.Columns.length-column.Index;if(band._optSelectRow)
{nftd.className=g.StopperStyle;if(g.IsXHTML)
setHeight=true;}
else
{if(!g.IsXHTML)
nftd.width="100%";else
{nftd.style.verticalAlign="top";setHeight=true;}}
tr.appendChild(nftd);var nfd=document.createElement("div");nftd.appendChild(nfd);nfd.id=g.Id+"_drs";nfd.style.overflow="hidden";if(!g.IsXHTML)
nfd.style.width="100%";nfd.style.height="100%";if(g.IsXHTML)
nfd.style.position="relative";var nftable=document.createElement("table");nfd.appendChild(nftable);nftable.border="0";nftable.cellPadding=g.Element.cellPadding;nftable.cellSpacing=g.Element.cellSpacing;nftable.style.position="relative";nftable.style.tableLayout="fixed";nftable.style.height="100%";var nfcgs=document.createElement("colgroup");nftable.appendChild(nfcgs);for(var j=column.Index;j<band.Columns.length;j++)
{if(band.Columns[j].getVisible())
{var nfcg=document.createElement("col");nfcg.width=band.Columns[j].Width;nfcgs.appendChild(nfcg);}}
for(var j=column.Index;j<band.Columns.length;j++)
{if(band.Columns[j].getHidden())
{var nfcg=document.createElement("col");nfcg.width="1px";nfcg.style.display="none";nfcgs.appendChild(nfcg);}}
if(g._scrElem.scrollLeft)
nftable.style.left=(-g._scrElem.scrollLeft).toString()+"px";var nftb=document.createElement("tbody");nftable.appendChild(nftb);nfrow=document.createElement("tr");nfrow.id=this.gridId+"_anfr_"+this.getLevel(true);nftb.appendChild(nfrow);}
if(column.CssClass&&!band._optSelectRow)
td.className=(td.className.length>0?" ":"")+column.CssClass;td.innerHTML=ct;if(nfrow)
{nfrow.appendChild(td);if(setHeight)
{var nftd=nfrow.parentNode.parentNode.parentNode.parentNode;nftd.style.height=nftd.parentNode.offsetHeight+"px";setHeight=false;nfrow.style.height=nftd.style.height;}}
else
tr.appendChild(td);}}
this.Rows.AddNewRow=new igtbl_AddNewRow(tr,this.Rows);igtbl_setNewRowImg(this.gridId,tr);g.newImg=null;}
var footersNode=null;if(this.Rows.Node)
footersNode=this.Rows.Node.selectSingleNode("Footers");if(band.ColFootersVisible==1&&!band.FooterHTML)
{var tFoot=document.createElement("tfoot");table.appendChild(tFoot);if(this.Band.Index==0&&this.Band.Grid.StatFooter&&this.GroupByRow&&g.get("StationaryMarginsOutlookGroupBy")=="True")
tFoot.style.display="none";if(band._optSelectRow)
{tFoot.className=band.getItemClass();tFoot.className+=" ";tFoot.className+=band.getHeadClass();tFoot.className+=" ";tFoot.className+=band.getFooterClass();}
var tr=document.createElement("tr");tFoot.appendChild(tr);var th;if(g.Bands.length>1)
{th=document.createElement("th");tr.appendChild(th);if(!band._optSelectRow)
th.className=band.getExpAreaClass();th.height=band.DefaultRowHeight;img=document.createElement("img");th.appendChild(img);img.src=band.Grid.BlankImage;img.border=0;img.style.visibility="hidden";}
if(band.getRowSelectors()==1)
{th=document.createElement("th");tr.appendChild(th);if(!band._optSelectRow)
th.className=band.getRowLabelClass();th.height=band.DefaultRowHeight;img=document.createElement("img");th.appendChild(img);img.src=band.Grid.BlankImage;img.border=0;img.style.visibility="hidden";}
var footers=null;if(footersNode)
footers=footersNode.selectNodes("Footer");var nfrow=null;setHeight=false;for(var i=0;i<band.Columns.length;i++)
{var column=band.Columns[i];if(column.hasCells())
{th=document.createElement("th");th.id=this.gridId+"_"+"f"+"_"+band.Index+"_"+i.toString();if(column.getHidden())
th.style.display="none";var ht="&nbsp;";if(footers&&i<footers.length&&footers[i].getAttribute("caption"))
ht=unescape(footers[i].getAttribute("caption"));else if(column.Node)
{var fn=column.Node.selectSingleNode("Footer");if(fn&&fn.getAttribute("caption"))
ht=unescape(fn.getAttribute("caption"));}
if(g.UseFixedHeaders&&!column.Fixed&&!nfrow)
{var nftd=document.createElement("th");nftd.colSpan=band.Columns.length-column.Index;nftd.style.textAlign="left";if(band._optSelectRow)
nftd.className=g.StopperStyle;if(!g.IsXHTML)
nftd.width="100%";else
{nftd.style.verticalAlign="top";setHeight=true;}
tr.appendChild(nftd);var nfd=document.createElement("div");nftd.appendChild(nfd);nfd.id=g.Id+"_drs";nfd.style.overflow="hidden";if(!g.IsXHTML)
nfd.style.width="100%";nfd.style.height="100%";if(g.IsXHTML)
nfd.style.position="relative";var nftable=document.createElement("table");nfd.appendChild(nftable);nftable.border="0";nftable.cellPadding=g.Element.cellPadding;nftable.cellSpacing=g.Element.cellSpacing;nftable.style.position="relative";nftable.style.tableLayout="fixed";nftable.style.height="100%";var nfcgs=document.createElement("colgroup");nftable.appendChild(nfcgs);for(var j=column.Index;j<band.Columns.length;j++)
{if(band.Columns[j].getVisible())
{var nfcg=document.createElement("col");nfcg.width=band.Columns[j].Width;nfcgs.appendChild(nfcg);}}
for(var j=column.Index;j<band.Columns.length;j++)
{if(band.Columns[j].getHidden())
{var nfcg=document.createElement("col");nfcg.width="1px";nfcg.style.display="none";nfcgs.appendChild(nfcg);}}
if(g._scrElem.scrollLeft)
nftable.style.left=(-g._scrElem.scrollLeft).toString()+"px";var nftb=document.createElement("tbody");nftable.appendChild(nftb);nfrow=document.createElement("tr");nftb.appendChild(nfrow);}
{if(!band._optSelectRow)
th.className=column.getFooterClass();else if(column.FooterClass)
th.className=column.FooterClass;if(column.FooterStyle)
th.style.cssText=column.FooterStyle;th.innerHTML=ht;}
if(nfrow)
{nfrow.appendChild(th);if(setHeight)
{var nftd=nfrow.parentNode.parentNode.parentNode.parentNode;nftd.style.height=nftd.parentNode.offsetHeight+"px";setHeight=false;}}
else
tr.appendChild(th);}}}}
this.Rows.Element=tBody;tBody.Object=this.Rows;}},"getLevel",function(s,paged)
{var l=new Array();l[0]=this.getIndex(true);var thisRow=this;var pr=this.ParentRow;while(pr)
{l[l.length]=pr.getIndex(true);thisRow=pr;pr=pr.ParentRow;}
if(paged&&thisRow.Band.Grid.AllowPaging)
l[l.length-1]+=(thisRow.Band.Grid.CurrentPageIndex-1)*thisRow.Band.Grid.PageSize;if(l.length>1)
l=l.reverse();if(s)
{s=l.join("_");igtbl_dispose(l);delete l;return s;}
return l;},"getCell",function(index)
{if(index<0||!this.cells||index>=this.cells.length)
return null;if(!this.cells[index])
{var cell=null;var col=this.Band.Columns[index];if(col.hasCells())
{if(this.Band.Grid.UseFixedHeaders&&!col.getFixed())
{var i=0,ci=this.Band.firstActiveCell,colspan=1;var cells=this.Element.cells;while(i<=index)
{if(!this.Band.Columns[i].getFixed()&&(i==0||this.Band.Columns[i-1].getFixed()))
{var tempCells=cells[cells.length-1].firstChild.firstChild.rows
if(tempCells&&tempCells.length>0)
{cells=tempCells[0].cells;ci=0;colspan=1;}}
if(this.Band.Columns[i].hasCells())
{if(i==index&&colspan==1)
cell=cells[ci];if(colspan==1)
{if(cells[ci])
colspan=cells[ci].colSpan;ci++;}
else
colspan--;}
i++;}}
else
{var ri=col.getRealIndex(this);if(ri>=0)
{cell=this.Element.cells[this.Band.firstActiveCell+ri];if(cell)
{var column=igtbl_getColumnById(cell.id);if(!column||!igtbl_isColEqual(column,col))
cell=null;}}}}
var node=null;if(this.Node)
{var cni=-1,colNo=0;while(colNo<col.Node.parentNode.childNodes.length)
{if(!col.Node.parentNode.childNodes[colNo].getAttribute("serverOnly"))
cni++;if(colNo==col.Node.getAttribute("columnNo"))
break;colNo++;}
if(cni>=0&&cni<col.Node.parentNode.childNodes.length)
node=this.Node.selectSingleNode("Cs").childNodes[cni];}
this.cells[index]=new igtbl_Cell(cell,node,this,index);}
return this.cells[index];},"getCellByColumn",function(col)
{return this.getCell(col.Index);},"getCellFromKey",function(key)
{var cell=null;var col=this.Band.getColumnFromKey(key);if(col)
cell=this.getCellByColumn(col);return cell;},"getChildRow",function(index)
{if(!this.Expandable)
return null;if(!this.FirstChildRow&&this.Rows)
this.FirstChildRow=this.Rows.getRow(0);if(index<0||index>=this.ChildRowsCount||!this.FirstChildRow)
return null;var i=0;var r=this.FirstChildRow.Element;while(i<index&&r)
{r=igtbl_getNextSibRow(this.gridId,r);i++;}
if(!r)
return null;return igtbl_getRowById(r.id);},"compare",function(row)
{if(this.OwnerCollection!=row.OwnerCollection)
return 0;if(this.GroupByRow)
return igtbl_getColumnById(this.GroupColId).compareRows(this,row);else
{var sc=this.OwnerCollection.Band.SortedColumns;for(var i=0;i<sc.length;i++)
{var col=igtbl_getColumnById(sc[i]);if(col.hasCells())
{var cell1=this.getCellByColumn(col);var cell2=row.getCellByColumn(col);var res=col.compareCells(cell1,cell2);if(res!=0)
{return res;}}}}
return 0;},"remove",function(fireEvents)
{return this.OwnerCollection.remove(this.OwnerCollection.indexOf(this),fireEvents);},"getNextTabRow",function(shift,ignoreCollapse,addRow,filterRow)
{var row=null;if(shift)
{row=this.getPrevRow(addRow,filterRow);if(row)
{while(row.Rows&&(row.getExpanded()||ignoreCollapse&&row.Expandable))
{if(addRow&&row.Rows.AddNewRow&&(row.Band.AddNewRowView==2||this.Rows.length==0&&this.Band.AddNewRowView==1))
row=row.Rows.AddNewRow;else
row=row.Rows.getRow(row.Rows.length-1);}}
else if(this.ParentRow)
row=this.ParentRow;}
else
{if(this.Rows&&(this.getExpanded()||ignoreCollapse&&this.Expandable))
{if(addRow&&this.Rows.AddNewRow&&(this.Band.AddNewRowView==1||this.Rows.length==0&&this.Band.AddNewRowView==2))
row=this.Rows.AddNewRow;else
row=this.Rows.getRow(0);}
else
{row=this.getNextRow(addRow,filterRow);if(!row&&this.ParentRow)
{var pr=this.ParentRow;while(!row&&pr)
{row=pr.getNextRow(addRow);pr=pr.ParentRow;}}}}
return row;},"getSelected",function()
{if(this._Changes["SelectedRows"])
return true;return false;},"setSelected",function(select)
{var str=this.Band.getSelectTypeRow();if(str>1)
{if(str==2)
this.Band.Grid.clearSelectionAll();igtbl_selectRow(this.gridId,this,select);}},"getNextRow",function(addRow,filterRow)
{var nr;if(this.IsAddNewRow)
{if(this.Band.AddNewRowView==1)
{if(this.Band.Index==0&&this.Band.Grid.StatHeader||this._dataChanged)
return null;nr=0;}
else
if(this.Band.Index==0&&this.Band.Grid.StatFooter)
return null;}
else
if(this.IsFilterRow)
{if(this.Band.FilterRowView==igtbl_featureRowView.Top)
{if(this.Band.Index==0&&this.Band.Grid.StatHeader)
return null;nr=0;}
else
if(this.Band.Index==0&&this.Band.Grid.StatFooter)
return null;}
else
nr=this.getIndex()+1;var nextRow=this.OwnerCollection.getRow(nr);while(nr<this.OwnerCollection.length&&nextRow&&nextRow.getHidden())
{nr++;nextRow=this.OwnerCollection.getRow(nr);}
if(nr<this.OwnerCollection.length&&nextRow)
return nextRow;if(addRow&&this.Band.AddNewRowVisible==1&&this.Band.AddNewRowView==2&&nr==this.OwnerCollection.length)
return this.OwnerCollection.AddNewRow;if(filterRow&&this.Band.FilterUIType==1&&this.Band.FilterRowView==igtbl_featureRowView.Bottom&&nr==this.OwnerCollection.length)
return this.OwnerCollection.FilterRow;return null;},"getPrevRow",function(addRow,filterRow)
{var pr;if(this.IsAddNewRow)
{if(this.Band.AddNewRowView==2)
{if(this.Band.Index==0&&this.Band.Grid.StatFooter||this._dataChanged)
return null;pr=this.OwnerCollection.length-1;}
else
if(this.Band.Index==0&&this.Band.Grid.StatHeader)
return null;}
else
if(this.IsFilterRow)
{if(this.Band.FilterRowView==igtbl_featureRowView.Bottom)
{if(this.Band.Index==0&&this.Band.Grid.StatFooter)
return null;pr=this.OwnerCollection.length-1;}
else
if(this.Band.Index==0&&this.Band.Grid.StatHeader)
return null;}
else
pr=this.getIndex()-1;var foundRow=null;while(pr>=0)
{foundRow=this.OwnerCollection.getRow(pr);if(!foundRow||!foundRow.getHidden())
break;pr--;}
if(pr>=0)
return foundRow;if(addRow&&this.Band.AddNewRowVisible==1&&this.Band.AddNewRowView==1&&pr==-1)
return this.OwnerCollection.AddNewRow;if(filterRow&&this.Band.FilterUIType==1&&this.Band.FilterRowView==igtbl_featureRowView.Top&&pr==-1)
return this.OwnerCollection.FilterRow;return null;},"activate",function(fireEvents)
{this.Band.Grid.setActiveRow(this,false,fireEvents);},"isActive",function()
{return this.Band.Grid.getActiveRow()==this;},"scrollToView",function()
{igtbl_scrollToView(this.gridId,this.Element);},"deleteRow",function(skipRowRecalc)
{var gs=igtbl_getGridById(this.gridId);var del=false;var rowId=this.Element.id;if(this.Band.AllowDelete==1||this.Band.AllowDelete==0&&gs.AllowDelete==1)
{var rows=this.OwnerCollection;if(igtbl_inEditMode(this.gridId))
{igtbl_hideEdit(this.gridId);if(igtbl_inEditMode(this.gridId))
return false;}
if(igtbl_fireEvent(this.gridId,gs.Events.BeforeRowDeleted,"(\""+this.gridId+"\",\""+rowId+"\")")==true)
return false;var btn=igtbl_getElementById(this.gridId+"_bt");del=true;var prevAdded=typeof(gs.AddedRows[rowId])!="undefined";if(!prevAdded)
gs.invokeXmlHttpRequest(gs.eReqType.DeleteRow,this,null,true);if(gs.XmlResponseObject&&gs.XmlResponseObject.Cancel)return;if(btn&&btn.style.display=="")
btn.style.display="none";igtbl_scrollLeft(gs.Element.parentNode,0);this.OwnerCollection.setLastRowId();if(this.getExpanded())
this.toggleRow();if(this.Band.SortedColumns.length==0)
{igtbl_clearRowChanges(gs,this);for(var rid in gs.AddedRows)
if(rid==rowId||rid.substr(0,rowId.length+1)==rowId+"_")
igtbl_clearRowChanges(gs,igtbl_getRowById(rid));}
if(!rows.deletedRows)
rows.deletedRows=new Array();var ar=this.Band.Grid.getActiveRow();var needPB=false;this.Element.setAttribute("deleted",true);if(typeof(this.Node)=="undefined")
{var overlappingColSpan=-1;for(var i=0;i<this.Band.Columns.length;i++)
{var cell=this.getCellByColumn(this.Band.Columns[i]);if(!cell&&this.Band.Columns[i].hasCells())
{var row=this;while(row.getPrevRow()&&!cell)
{row=row.getPrevRow();cell=row.getCellByColumn(this.Band.Columns[i]);}
if(row==this||!cell||cell.Column.hasCells()&&cell.Element!=null&&cell.Element.rowSpan==1)
{needPB=true;break;}}
else if(cell&&cell.Column.hasCells()&&(!cell.Element||cell.Element.rowSpan>1))
{if(overlappingColSpan>1)
overlappingColSpan--;if(cell.Element&&cell.Element.rowSpan>1)
{needPB=true;break;}}
if(cell&&cell.Element)
{if(cell.Element.rowSpan>1)
cell.Element.rowSpan--;if(cell.Element.colSpan>1)
overlappingColSpan=cell.Element.colSpan;}}}
if(!needPB)
{rows.deletedRows[rows.deletedRows.length]=this.remove(false);if(gs.LoadOnDemand==3&&(!gs.Events.XmlHTTPResponse||gs.Events.XmlHTTPResponse[1]||gs.Events.AfterRowDeleted[1]))
gs._removeChange("DeletedRows",this);var pr=this.ParentRow;if(pr)
{pr.VisChildRowsCount--;pr.ChildRowsCount--;}
while(pr)
{if(pr.Expandable&&pr.Rows.length==0)
{if(pr.Rows.Band.AddNewRowVisible!=1)
pr.setExpanded(false);if(pr.GroupByRow)
{gs._removeChange("CollapsedRows",pr);gs.DeletedRows[pr.Element.id]=true;pr.Element.setAttribute("deleted",true);rows.deletedRows[rows.deletedRows.length]=pr.remove(false);gs._removeChange("DeletedRows",pr);delete gs.SelectedRows[pr.Element.id];}
else
{if(pr.Rows.Band.AddNewRowVisible!=1)
pr.Element.childNodes[0].childNodes[0].style.display="none";}
pr.Expandable=false;}
pr=pr.ParentRow;}
if(this.Node&&!gs.isDeletingSelected)
rows.reIndex(this.getIndex(true));if(ar==this)
this.Band.Grid.setActiveRow(null);else
{var ac=this.Band.Grid.getActiveCell();if(ac&&ac.Row==this)
this.Band.Grid.setActiveCell(null);}}
else
{gs._recordChange("DeletedRows",this);igtbl_needPostBack(this.gridId);}
if(prevAdded&&this.Band.SortedColumns.length<=0)
this._Changes["DeletedRows"].setFireEvent(false);gs._calculateStationaryHeader();if(!skipRowRecalc)gs._recalcRowNumbers();igtbl_fireEvent(this.gridId,gs.Events.AfterRowDeleted,"(\""+this.gridId+"\",\""+rowId+"\");");if(gs.LoadOnDemand==3)
gs.NeedPostBack=false;}
return del;},"getLeft",function(offsetElement)
{return igtbl_getLeftPos(igtbl_getElemVis(this.Element.cells,igtbl_getBandFAC(this.gridId,this.Element)),true,offsetElement);},"getTop",function(offsetElement)
{var t=igtbl_getTopPos(this.Element,true,offsetElement);return t;},"editRow",function(force)
{var au=igtbl_getAllowUpdate(this.gridId,this.Band.Index);if(igtbl_currentEditTempl!=null||!force&&au!=1&&au!=3||this.IsAddNewRow||this.IsFilterRow)
return;var editTempl=igtbl_getElementById(this.Band.RowTemplate);if(!editTempl)
return;var tPan=this.Band.transPanel;if(tPan==null&&ig_csom.IsIEWin)
{this.Band.transPanel=tPan=ig_csom.createTransparentPanel();if(tPan)
{editTempl.parentNode.insertBefore(tPan.Element,editTempl);tPan.Element.style.zIndex=igtbl_parseInt(editTempl.style.zIndex)-1;}}
var gridObj=igtbl_getGridById(this.gridId);gridObj.Element.setAttribute("noOnResize",true);window.setTimeout("igtbl_clearNoOnResize('"+this.gridId+"')",100);if(igtbl_fireEvent(this.gridId,gridObj.Events.BeforeRowTemplateOpen,"(\""+this.gridId+"\",\""+this.Element.id+"\",\""+this.Band.RowTemplate+"\")"))
return;try
{if(editTempl.style.filter!=null&&this.Band.ExpandEffects)
{var ee=this.Band.ExpandEffects;if(ee.EffectType!='NotSet')
{editTempl.style.filter="progid:DXImageTransform.Microsoft."+ee.EffectType+"(duration="+ee.Duration/1000+");"
if(ee.ShadowWidth>0)
editTempl.style.filter+=" progid:DXImageTransform.Microsoft.Shadow(Direction=135, Strength="+ee.ShadowWidth+",color="+ee.ShadowColor+");"
if(ee.Opacity<100)
editTempl.style.filter+=" progid:DXImageTransform.Microsoft.Alpha(Opacity="+ee.Opacity+");"
if(editTempl.filters[0]!=null)
editTempl.filters[0].apply();if(editTempl.filters[0]!=null)
editTempl.filters[0].play();}
else
{if(ee.ShadowWidth>0)
editTempl.runtimeStyle.filter="progid:DXImageTransform.Microsoft.Shadow(Direction=135, Strength="+ee.ShadowWidth+",ee.Color="+ee.ShadowColor+");"
if(ee.Opacity<100)
editTempl.runtimeStyle.filter+=" progid:DXImageTransform.Microsoft.Alpha(Opacity="+ee.Opacity+");"}}}
catch(ex){}
editTempl.style.display="";editTempl.style.visibility="hidden";if(!editTempl.style.width)
editTempl.style.width=editTempl.offsetWidth;if(!editTempl.style.height)
editTempl.style.height=editTempl.offsetHeight;editTempl.setAttribute("noHide",true);var fc=igtbl_getElemVis(this.Element.cells,igtbl_getBandFAC(this.gridId,this.Element));editTempl.style.left=igtbl_getRelativePos(this.gridId,fc,"Left");var tw=igtbl_clientWidth(editTempl);var bw=gridObj.IsXHTML?document.documentElement.clientWidth:document.body.clientWidth;var gdw=gridObj.Element.parentNode.scrollLeft;if(gridObj.IsXHTML)
{var leftVal=gridObj.MainGrid.offsetLeft+fc.offsetLeft-gridObj.DivElement.scrollLeft;if(leftVal<0)leftVal=gridObj.MainGrid.offsetLeft;editTempl.style.left=leftVal+(ig_csom.IsIE?"":"px");}
else
editTempl.style.left=editTempl.offsetLeft+gdw;if(editTempl.offsetLeft+tw-igtbl_getBodyScrollLeft()>bw)
if(bw-tw+igtbl_getBodyScrollLeft()-gdw>0)
editTempl.style.left=bw-tw+igtbl_getBodyScrollLeft()-gdw;else
editTempl.style.left=0;var th=igtbl_clientHeight(editTempl);var bh=gridObj.IsXHTML?document.documentElement.clientHeight:document.body.clientHeight;if(gridObj.IsXHTML)
{var elemAbsBounds=igtbl_getAbsBounds(this.Element,gridObj,true);var topVal=elemAbsBounds.y;topVal+=elemAbsBounds.h;editTempl.style.top=topVal+(ig_csom.IsIE?"":"px");}
else
editTempl.style.top=igtbl_getRelativePos(this.gridId,fc,"Top")+this.Element.offsetHeight+"px";if(editTempl.offsetTop+th-igtbl_getBodyScrollTop()>bh)
if(bh-th+igtbl_getBodyScrollTop()>0)
editTempl.style.top=bh-th+igtbl_getBodyScrollTop()+"px";else
editTempl.style.top="0px";if(tPan)
{tPan.setPosition(editTempl.style.top,editTempl.style.left,editTempl.style.width,editTempl.style.height);tPan.show();var z=gridObj._getZ(10000,1);editTempl.style.zIndex=z;tPan.Element.style.zIndex=z;}
editTempl.setAttribute("editRow",this.Element.id);igtbl_fillEditTemplate(this,editTempl.childNodes);editTempl.style.visibility="visible";if(igtbl_focusedElement&&igtbl_isVisible(igtbl_focusedElement))
{igtbl_focusedElement.focus();if(igtbl_focusedElement.select)
igtbl_focusedElement.select();igtbl_focusedElement=null;}
igtbl_currentEditTempl=this.Band.RowTemplate;igtbl_oldMouseDown=igtbl_addEventListener(document,"mousedown",igtbl_gRowEditMouseDown,false);igtbl_justAssigned=true;window.setTimeout(igtbl_resetJustAssigned,100);editTempl.removeAttribute("noHide");igtbl_fireEvent(this.gridId,gridObj.Events.AfterRowTemplateOpen,"(\""+this.gridId+"\",\""+this.Element.id+"\")");},"endEditRow",function(saveChanges)
{if(arguments.length==0||typeof(saveChanges)=="undefined")
saveChanges=false;var gs=igtbl_getGridById(this.gridId);var editTempl=igtbl_getElementById(this.Band.RowTemplate);if(!editTempl||editTempl.style.display!="")
return;if(editTempl.getAttribute("noHide"))
return;if(igtbl_fireEvent(this.gridId,gs.Events.BeforeRowTemplateClose,"(\""+this.gridId+"\",\""+this.Element.id+"\","+saveChanges.toString()+")"))
return;editTempl.style.display="none";if(this.Band.transPanel)
this.Band.transPanel.hide();igtbl_currentEditTempl=null;igtbl_removeEventListener(document,"mousedown",igtbl_gRowEditMouseDown,igtbl_oldMouseDown,false);igtbl_oldMouseDown=null;if(saveChanges)
igtbl_unloadEditTemplate(this,editTempl.childNodes);igtbl_fireEvent(this.gridId,gs.Events.AfterRowTemplateClose,"(\""+this.gridId+"\",\""+this.Element.id+"\","+saveChanges.toString()+")");if(gs.NeedPostBack)
igtbl_doPostBack(gs.Id);var rowTemplate=igtbl_srcElement(igtbl_getGridById(this.gridId).event)
while(rowTemplate!=null&&rowTemplate.id!=this.Band.RowTemplate)
rowTemplate=rowTemplate.parentNode;if(rowTemplate)
igtbl_activate(this.gridId);},"getHidden",function()
{return(this.Element.style.display=="none");},"setHidden",function(h)
{this.Element.style.display=(h?"none":"");igtbl_browserWorkarounds.ieBorderCollapseArtifacts(this,h);if(this.getExpanded())
this.setExpanded(false);var g=this.Band.Grid;if(g.UseFixedHeaders)
{var drs=null;var row=this.Element;var i=0;while(i<row.cells.length&&(!row.cells[i].firstChild||row.cells[i].firstChild.id!=g.Id+"_drs"))i++;if(i<row.cells.length)
{var td=row.cells[i];drs=td.firstChild;}
if(drs)
drs.style.display=(h?"none":"");}
if(this.ParentRow)
this.ParentRow.VisChildRowsCount+=(h?-1:1);var ac=this.Band.Grid.getActiveCell();if(ac&&ac.Row==this&&h)
this.Band.Grid.setActiveCell(null);else
{var ar=this.Band.Grid.getActiveRow();if(ar&&ar==this&&h)
this.Band.Grid.setActiveRow(null);else
this.Band.Grid.alignGrid();}
for(var i=0;i<this.Band.Columns.length;i++)
{if(this.Band.Columns[i].ColumnType==7)
{var cellElement=this.getCell(i).getElement();cellElement.style.display=(h?"none":"");}}},"find",function(re,back,searchHiddenColumns)
{var g=this.Band.Grid;if(re)
g.regExp=re;if(!g.regExp)
return null;g.lastSearchedCell=null;if(back==true||back==false)
g.backwardSearch=back;
var cell=null;
var targetCell=null;
if(!g.backwardSearch)
{
    cell=this.getCell(0);
    if(cell && !cell.Column.getVisible() && searchHiddenColumns!=true)
    {
        cell = cell.getNextCell();
    }   
    var success = false;
    while(cell && !targetCell && cell.Index < 4)
    {
        if (cell.getValue(true).search(g.regExp) != -1)
        {
            targetCell = cell;
        }
        cell=cell.getNextCell(searchHiddenColumns);
    }
}
else
{cell=this.getCell(this.cells.length-1);if(cell&&!cell.Column.getVisible()&&searchHiddenColumns!=true)
{cell=cell.getPrevCell();}
while(cell&&cell.getValue(true).search(g.regExp)==-1)
{cell=cell.getPrevCell(searchHiddenColumns);}}
if(targetCell)
g.lastSearchedCell=targetCell;return g.lastSearchedCell;},"findNext",function(re,back,searchHiddenColumns)
{var g=this.Band.Grid;if(!g.lastSearchedCell||g.lastSearchedCell.Row!=this)
{return this.find(re,back,searchHiddenColumns);}
if(re)
g.regExp=re;if(!g.regExp)
return null;if(back==true||back==false)
g.backwardSearch=back;var cell=null;if(!g.backwardSearch)
{cell=g.lastSearchedCell.getNextCell(searchHiddenColumns);while(cell&&cell.getValue(true).search(g.regExp)==-1)
{cell=cell.getNextCell(searchHiddenColumns);}}
else
{cell=g.lastSearchedCell.getPrevCell(searchHiddenColumns);while(cell&&cell.getValue(true).search(g.regExp)==-1)
{cell=cell.getPrevCell(searchHiddenColumns);}}
if(cell)
g.lastSearchedCell=cell;else
g.lastSearchedCell=null;return g.lastSearchedCell;},"setSelectedRowImg",function(hide)
{var gs=this.Band.Grid;if(this.Band.AllowRowNumbering>=2||this.IsAddNewRow)
return;var row=this.Element;if(gs.currentTriImg!=null)
{gs._lastSelectedRow=null;var imgObj;imgObj=document.createElement("img");imgObj.setAttribute("imgType","blank");imgObj.border="0";if(gs.RowLabelBlankImage)
imgObj.src=gs.RowLabelBlankImage;else
{imgObj.src=gs.BlankImage;imgObj.style.visibility="hidden";}
gs.currentTriImg.parentNode.appendChild(imgObj);gs.currentTriImg.parentNode.removeChild(gs.currentTriImg);gs.currentTriImg=null;}
if(!hide&&row&&!row.getAttribute("deleted")&&!row.getAttribute("groupRow")&&this.Band.getRowSelectors()!=2)
{var rl=row.cells[this.Band.firstActiveCell-1];if(rl.childNodes.length==0||!(rl.childNodes[0].tagName=="IMG"&&rl.childNodes[0].getAttribute("imgType")=="newRow"))
{var imgObj;var bIndex=this.Band.Index;imgObj=document.createElement("img");imgObj.src=igtbl_getCurrentRowImage(this.gridId,bIndex);imgObj.border="0";imgObj.setAttribute("imgType","tri");if(gs.Section508Compliant)
{var altT=igtbl_getCurrentRowAltText(this.gridId,bIndex);if(altT)imgObj.setAttribute("alt",altT);}
var cell=row.cells[this.Band.firstActiveCell-1];cell.innerHTML="";cell.appendChild(imgObj);gs.currentTriImg=imgObj;}
gs._lastSelectedRow=row.id;}},"renderActive",function(render)
{var g=this.Band.Grid;var ao=g.Activation;if(!ao.AllowActivation)
return;if(typeof(render)=="undefined")render=true;if(this.GroupByRow)
{var fr=this.getFirstRow();fr=fr.firstChild;if(render)
{igtbl_setClassName(fr,ao._cssClass);igtbl_setClassName(fr,ao._cssClassL);igtbl_setClassName(fr,ao._cssClassR);}
else
{igtbl_removeClassName(fr,ao._cssClassR);igtbl_removeClassName(fr,ao._cssClassL);igtbl_removeClassName(fr,ao._cssClass);}}
else
{{var i=0;var els=this.getCellElements();if(!els||els.length==0)return;var cell=els[i];while(cell&&this.Band.Columns[i].getHidden()&&i<this.cells.length)
cell=els[++i];if(i<els.length)
{if(render)
igtbl_setClassName(cell,ao._cssClassL);else
igtbl_removeClassName(cell,ao._cssClassL);}
for(i=0;i<els.length;i++)
{cell=els[i];if(render)
igtbl_setClassName(cell,ao._cssClass);else
igtbl_removeClassName(cell,ao._cssClass);}
i=els.length-1;cell=els[i];while(cell&&this.Band.Columns[i].getHidden()&&i>=0)
cell=els[--i];if(i>=0)
{if(render)
igtbl_setClassName(cell,ao._cssClassR);else
igtbl_removeClassName(cell,ao._cssClassR);}
igtbl_dispose(els);}}},"select",function(selFlag,fireEvent)
{var gs=this.Band.Grid;if(this.Band.getSelectTypeRow()<2||this.getSelected()==selFlag)
return false;if(gs._exitEditCancel||gs._noCellChange)
return false;if(fireEvent!=false)
if(igtbl_fireEvent(gs.Id,gs.Events.BeforeSelectChange,"(\""+gs.Id+"\",\""+this.Element.id+"\")")==true)
return false;if(!this.GroupByRow)
{var style=null;if(selFlag!=false)
style=this.Band.getSelClass();if(this.Band._optSelectRow)
{if(style)
{var aoStyle="";if(gs.oActiveRow==this)
{var styles=this.Element.className.split(" ");aoStyle=" "+styles[styles.length-1];styles=styles.slice(0,styles.length-1);this.Element.className=styles.join(" ");if(this.nfElement)
this.nfElement.className=this.Element.className;}
this.Element.className+=" "+style+aoStyle;if(this.nfElement)
this.nfElement.className+=" "+style+aoStyle;}
else
{var styles=this.Element.className;style=this.Band.getSelClass();if(style&&styles.indexOf(style)>-1)
this.Element.className=styles.replace(style,"");if(this.nfElement)
this.nfElement.className=this.Element.className;}}
else if(!this.Band._selClassDiffer)
{var els=this.getCellElements();for(var i=0;i<els.length;i++)
igtbl_changeStyle(gs.Id,els[i],style);}
if(this.Band._selClassDiffer)
for(var i=0;i<this.cells.length;i++)
this.getCell(i).selectCell(selFlag);}
else if(selFlag!=false)
igtbl_changeStyle(gs.Id,this.FirstRow.cells[0],this.Band.getSelGroupByRowClass());else
igtbl_changeStyle(gs.Id,this.FirstRow.cells[0],null);if(selFlag!=false)
gs._recordChange("SelectedRows",this,gs.GridIsLoaded.toString());else if(gs.SelectedRows[this.Element.id]||gs._containsChange("SelectedRows",this))
gs._removeChange("SelectedRows",this);if(fireEvent!=false)
{var gsNPB=gs.NeedPostBack;igtbl_fireEvent(gs.Id,gs.Events.AfterSelectChange,"(\""+gs.Id+"\",\""+this.Element.id+"\");");if(!gsNPB&&!(gs.Events.AfterSelectChange[1]&2))
gs.NeedPostBack=false;if(gs.NeedPostBack)
igtbl_moveBackPostField(gs.Id,"SelectedRows");}
return true;},"processUpdateRow",function()
{return this._processUpdateRow();},"_processUpdateRow",function()
{var result=false;var g=this.Band.Grid;if(!this._dataChanged||typeof(g.Events.BeforeRowUpdate)=="undefined")
return result;for(var i=0;(this._dataChanged&2)&&i<this.cells.length;i++)
if(typeof(this.getCell(i)._oldValue)!="undefined")
break;if(i<this.cells.length)
{g.QueryString="";result=g.fireEvent(g.Events.BeforeRowUpdate,[g.Id,this.Element.id]);if((this._dataChanged&2))
for(;i<this.cells.length;i++)
{var cell=this.getCell(i);if(typeof(cell._oldValue)!="undefined")
{if(result)
cell.setValue(cell._oldValue,false);else if(g.LoadOnDemand==3)
g.QueryString+=(g.QueryString&&g.QueryString.length>0?"\x04":"")+"UpdateCell\x06"+cell.Column.Key+"\x02"+igtbl_escape(cell.getValue()==null?cell.Column.getNullText():igtbl_dateToString(cell.getValue()));}}
if(!result)
{if(g.LoadOnDemand==3&&(g.Events.AfterRowUpdate[1]||g.Events.XmlHTTPResponse[1]))
g.invokeXmlHttpRequest(g.eReqType.UpdateRow,this);else
{g.fireEvent(g.Events.AfterRowUpdate,[g.Id,this.Element.id]);if(g.NeedPostBack)
igtbl_doPostBack(g.Id);}}
this._dataChanged=0;}
return result;},"_getRowNumber",function()
{var index=null;var band=this.Band;var oLbl;if(this.Id&&this.Id.indexOf(this.gridId+"_r_")==0)
{var oLbl=igtbl_getElementById(this.Id.replace("_r_","_l_"));}
if(!oLbl)
{var oLbl=igtbl_getElementById(this.gridId+"_l_"+this.getLevel(true));}
if(band.getRowSelectors()<2&&band.AllowRowNumbering>1&&oLbl)
{index=igtbl_getInnerText(oLbl);}
return index;},"_setRowNumber",function(value)
{var band=this.Band;var oRS=band.firstActiveCell-1;var oLbl=-1;if(this.Element)
oLbl=this.Element.childNodes[oRS];if(band.getRowSelectors()<2&&band.AllowRowNumbering>1)
{if(this.Node)this.Node.setAttribute(igtbl_litPrefix+"rowNumber",value);if(oLbl)oLbl.innerHTML=value;return value;}
else
return-1;},"_generateHierarchicalDataKey",function()
{var currentRow=this;var dataKey="";while(currentRow)
{if(currentRow.DataKey)
dataKey=currentRow.DataKey+dataKey;if(currentRow.ParentRow)
dataKey='\x08'+dataKey;currentRow=currentRow.ParentRow;}
return dataKey;},"_generateUpdateRowSemaphore",function(clear)
{var cellInfo="";for(var j=0;j<this.cells.length;j++)
{var cell=this.getCell(j);if(cell)
{if(typeof(cell.getOldValue())!="undefined")
{var oldValue=cell.getOldValue();oldValue=igtbl_dateToString(oldValue);cellInfo+=(cellInfo.length>0?"\x03":"")+igtbl_escape(cell.Column.Key+"\x05"+cell.Column.Index+"\x05"+oldValue);if(clear)
delete cell._oldValue;}
else
cellInfo+=(cellInfo.length>0?"\x03":"")+igtbl_escape(cell.Column.Key+"\x05"+cell.Column.Index+"\x05"+(cell.getValue()==null?cell.Column.getNullText():igtbl_dateToString(cell.getValue())));}}
return cellInfo;},"_generateSqlWhere",function(dataKeyField,value)
{if(!dataKeyField)return;var sqlWhere="";var dkfArray=dataKeyField.split(",");var valArray=value.split('\x07');for(var i=0;i<dkfArray.length;i++)
{var dk=igtbl_string.trim(dkfArray[i]);if(i>0)
sqlWhere+=" AND ";if(this.Band.getColumnFromKey(dk).DataType==8)
sqlWhere+=dk+"='"+valArray[i]+"'";else
sqlWhere+=dk+"="+valArray[i];}
if(this.Band._sqlWhere)
{if(sqlWhere)
sqlWhere+=" AND ";sqlWhere+=this.Band._sqlWhere;}
return sqlWhere;},"getChildRows",function()
{var rows=null;row=this.Element;if(row.getAttribute("groupRow"))
rows=row.childNodes[0].childNodes[0].childNodes[0].rows[1].childNodes[0].childNodes[0].tBodies[0].rows;else
{if(row.nextSibling&&row.nextSibling.getAttribute("hiddenRow"))
{if(this.Band.IndentationType==2)
rows=row.nextSibling.firstChild.firstChild.tBodies[0].rows;else
rows=row.nextSibling.childNodes[this.Band.firstActiveCell].firstChild.tBodies[0].rows;}
else
rows=null;}
return rows;},"getCellElements",function(flCells)
{var re=this.Element,nfr=false;if(!re||!re.cells.length||this.GroupByRow)return;var result=new Array();var start=0;if(this.Band.Grid.Bands.length>1)start++;if(this.Band.getRowSelectors()<2)start++;for(var i=start;i<re.cells.length;i++)
{if(this.Band.Grid.UseFixedHeaders&&!nfr)
{if(re.cells[i].childNodes.length>0&&re.cells[i].firstChild.tagName=="DIV"&&re.cells[i].firstChild.id.substr(re.cells[i].firstChild.id.length-4)=="_drs")
{re=re.cells[i].firstChild.firstChild.childNodes[1].rows[0];i=0;nfr=true;}}
if(flCells)
{if(re.cells[i].offsetHeight>0)
{result[result.length]=re.cells[i];break;}}
else
result[result.length]=re.cells[i];}
if(flCells)
{if(this.Band.Grid.UseFixedHeaders&&!nfr)
{re=re.cells[re.cells.length-1].firstChild.firstChild.childNodes[1].rows[0];i=0;}
for(var j=re.cells.length-1;j>=i;j--)
if(re.cells[j].offsetHeight>0)
{result[result.length]=re.cells[j];break;}}
return result;},"getRowSelectorElement",function()
{if(!this.GroupByRow&&this.Band.getRowSelectors()!=2)
return this.Element.cells[this.Band.firstActiveCell-1];return null;},"getExpansionElement",function()
{if(this.GroupByRow)return null;if(this.Band.getRowSelectors()!=2)
{if(this.Band.firstActiveCell>1)
return this.Element.cells[0];}
else if(this.Band.firstActiveCell>0)
return this.Element.cells[0];return null;},"_evaluateFilters",function()
{if(this.Band.ApplyOnAdd==0)return;var oFilterConditions;if(this.Band.Index==0&&this.Band.GroupCount==0)
{oFilterConditions=this.Band._filterPanels;}
else if((this.Band.Columns[0].RowFilterMode==1&&this.Band.GroupCount==0)||(this.Band.Index==0&&this.Band.GroupCount>0&&this.Band.Grid.StatHeader))
{oFilterConditions=this.Band._filterPanels;}
else
{var siblingRows=this.OwnerCollection;oFilterConditions=this.Band._filterPanels[siblingRows.Element.parentNode.id];}
if(oFilterConditions)
{this.getCell(0).Column._evaluateFilters(this,oFilterConditions,this.Band);}},"dispose",function()
{igtbl_cleanRow(this);igtbl_dispose(this);}];for(var i=0;i<igtbl_ptsRow.length;i+=2)
igtbl_Row.prototype[igtbl_ptsRow[i]]=igtbl_ptsRow[i+1];igtbl_Row.prototype["getRowNumber"]=igtbl_Row.prototype["_getRowNumber"];igtbl_AddNewRow.prototype=new igtbl_Row();igtbl_AddNewRow.prototype.constructor=igtbl_AddNewRow;igtbl_AddNewRow.base=igtbl_Row.prototype;function igtbl_AddNewRow(element,rows)
{if(arguments.length>0)
this.init(element,rows);}
var igtbl_ptsAddNewRow=["init",function(element,rows)
{this.IsAddNewRow=true;igtbl_AddNewRow.base.init.apply(this,[element,null,rows,-1]);this.Type="addNewRow";},"commit",function()
{if(this._dataChanged)
{this._dataChanged=0;var ac=this.Band.Grid.oActiveCell,ar=this.Band.Grid.oActiveRow;var newRow=igtbl_rowsAddNew(this.gridId,this.ParentRow,this);if(newRow)
{for(var i=0;i<this.Band.Columns.length;i++)
{var cellObj=this.getCell(i);cellObj.setValue(cellObj.Column.getValueFromString(cellObj.Column.DefaultValue));}
this._dataChanged=0;if(ac&&ac.Row.IsAddNewRow)
{var acSel=ac.getSelected();if(acSel)
ac.setSelected(false);var nac=newRow.getCell(ac.Column.Index);nac.activate();if(acSel)
nac.setSelected();}
else if(ar.IsAddNewRow)
{var arSel=ar.getSelected();if(arSel)
ar.setSelected(false);newRow.activate();if(arSel)
newRow.setSelected();}
newRow.processUpdateRow();}
return newRow;}
return null;},"isFixed",function()
{return this.isFixedTop()||this.isFixedBottom();},"isFixedTop",function()
{return this.Band.Index==0&&this.Band.Grid.StatHeader!=null&&this.Band.AddNewRowView==1;},"isFixedBottom",function()
{return this.Band.Index==0&&this.Band.Grid.StatFooter!=null&&this.Band.AddNewRowView==2;}];for(var i=0;i<igtbl_ptsAddNewRow.length;i+=2)
igtbl_AddNewRow.prototype[igtbl_ptsAddNewRow[i]]=igtbl_ptsAddNewRow[i+1];igtbl_FilterRow.prototype=new igtbl_Row();igtbl_FilterRow.prototype.constructor=igtbl_FilterRow;igtbl_FilterRow.base=igtbl_Row.prototype;function igtbl_FilterRow(element,rows)
{if(arguments.length>0)
this.init(element,rows);}
var igtbl_ptsFilterRow=["init",function(element,rows)
{this.IsFilterRow=true;igtbl_FilterRow.base.init.apply(this,[element,null,rows,-1]);this.Type="filterRow";},"isFixed",function()
{return this.isFixedTop()||this.isFixedBottom();},"isFixedTop",function()
{return this.Band.Index==0&&this.Band.Grid.StatHeader!=null&&this.Band.FilterRowView==1;},"isFixedBottom",function()
{return this.Band.Index==0&&this.Band.Grid.StatFooter!=null&&this.Band.FilterRowView==2;}];for(var i=0;i<igtbl_ptsFilterRow.length;i+=2)
igtbl_FilterRow.prototype[igtbl_ptsFilterRow[i]]=igtbl_ptsFilterRow[i+1];igtbl_Rows.prototype=new igtbl_WebObject();igtbl_Rows.prototype.constructor=igtbl_Rows;igtbl_Rows.base=igtbl_WebObject.prototype;function igtbl_Rows(node,band,parentRow)
{if(arguments.length>0)
{var element=null;if(band.Index==0&&!parentRow)
element=band.Grid.Element.tBodies[0];else if(parentRow&&parentRow.Element)
{if(parentRow.GroupByRow)
{var tb=parentRow.Element.childNodes[0].childNodes[0].tBodies[0];if(tb.childNodes.length>1)
this.Element=tb.childNodes[1].childNodes[0].childNodes[0].tBodies[0];}
else if(parentRow.Element.nextSibling&&parentRow.Element.nextSibling.getAttribute("hiddenRow"))
this.Element=parentRow.Element.nextSibling.childNodes[parentRow.Band.IndentationType==2?0:parentRow.Band.firstActiveCell].childNodes[0].tBodies[0];}
this.init(element,node,band,parentRow);}}
var igtbl_ptsRows=["init",function(element,node,band,parentRow)
{igtbl_Rows.base.init.apply(this,["rows",element,node]);this.Grid=band.Grid;this.Band=band;this.ParentRow=parentRow;this.rows=new Array();this.length=0;if(node)
{this.SelectedNodes=node.selectNodes("R");if(!this.SelectedNodes.length)
{this.SelectedNodes=node.selectNodes("Group");if(this.SelectedNodes.length)
this.GroupColId=this.SelectedNodes[0].getAttribute(igtbl_litPrefix+"groupRow");}
this.length=this.SelectedNodes.length;}
else
{if(parentRow)
this.length=parentRow.ChildRowsCount;else
{this.length=this.Element.childNodes.length;for(var i=0;i<this.Element.childNodes.length;i++)
{var r=this.Element.childNodes[i];if(r.getAttribute("hiddenRow")||r.getAttribute("addNewRow")||r.getAttribute("filterRow"))
this.length--;}}}
if(this.Element)
this.Element.Object=this;this.lastRowId="";if(!this.ParentRow||!this.ParentRow.GroupByRow)
{var anr=igtbl_getElementById(this.Grid.Id+"_anr"+(this.ParentRow?"_"+this.ParentRow.getLevel(true,true):""));if(anr)
this.AddNewRow=new igtbl_AddNewRow(anr,this);}
var filterRow=igtbl_getElementById(this.Grid.Id+"_flr"+(this.ParentRow?"_"+this.ParentRow.getLevel(true):""));if(filterRow)
{this.FilterRow=new igtbl_FilterRow(filterRow,this);}},"reapplyRowStyles",function()
{var alternateStyle=this.Band.getRowAltClassName();var rowStyle=this.Band.getRowStyleClassName();var useAlternateRowStyle=(alternateStyle!="")&&(alternateStyle!=rowStyle);if(!useAlternateRowStyle)return;var altRow=false;for(var i=0;i<this.length;i++)
{var curRow=this.getRow(i);if(curRow.getHidden())
continue;var className="";if(useAlternateRowStyle)
className=altRow?alternateStyle:rowStyle;if(className&&!curRow.GroupByRow)
{var rowE=curRow.Element;if(curRow.Band._optSelectRow)
{if(altRow)
igtbl_dom.css.replaceClass(rowE,rowStyle,alternateStyle);else
igtbl_dom.css.removeClass(rowE,alternateStyle);}
else
{var j=curRow.Band.firstActiveCell;var colNo=0;var rowElem=curRow.Element;var nonFixed=false;while(j<rowElem.cells.length)
{var col=curRow.Band.Columns[colNo];if(col.getFixed()===false&&!nonFixed)
{j=0;rowElem=curRow.nfElement;nonFixed=true;}
var e=rowElem.cells[j];if(e)
{if(useAlternateRowStyle)
{var colCssClass=(!altRow)?col.CssClass:col._AltCssClass;colCssClass=colCssClass&&className!=colCssClass?" "+colCssClass:"";if(e.className!=className+colCssClass)
e.className=className+colCssClass;}
else
e.className=className+(col.CssClass?" "+col.CssClass:"");}
j++;colNo++;}}}
if(useAlternateRowStyle)
altRow=!altRow;}},"getRow",function(rowNo,rowElement)
{if(typeof(rowNo)!="number")
{rowNo=parseInt(rowNo);if(isNaN(rowNo))
return null;}
if(rowNo<0||!this.Element||!this.Element.childNodes)
return null;if(rowNo>=this.length)
{if(this.length>this.rows.length)
this.rows[this.length-1]=null;return null;}
if(rowNo>=this.rows.length)
this.rows[this.length-1]=null;if(!this.rows[rowNo])
{var row=rowElement;if(!row)
{var cr=0;if(this.Grid.Bands.length==1&&!this.Grid.Bands[0].IsGrouped)
{var adj=0;if(!igtbl_getElementById(this.Grid.Id+"_hdiv")&&this.Grid.Bands[0].AddNewRowVisible==1&&this.Grid.Bands[0].AddNewRowView==1)
adj++;if(this.Grid.Bands[0].AllowRowFiltering>=2&&this.Grid.Bands[0].FilterUIType==1)
{if(!igtbl_getElementById(this.Grid.Id+"_hdiv"))
adj++;else
{var filterRow=this.FilterRow;if(filterRow&&filterRow.Id)
{var filterRowElm=document.getElementById(filterRow.Id);if(filterRowElm&&filterRowElm.parentNode&&filterRowElm.parentNode.parentNode&&filterRowElm.parentNode.parentNode.parentNode&&filterRowElm.parentNode.parentNode.parentNode.id!=(this.Grid.Id+"_hdiv"))
adj++;}}}
row=this.Element.childNodes[rowNo+adj];}
else
for(var i=0;i<this.Element.childNodes.length;i++)
{var r=this.Element.childNodes[i];if(!r.getAttribute("hiddenRow")&&!r.getAttribute("addNewRow")&&!r.getAttribute("filterRow"))
{if(rowNo==cr)
{row=this.Element.childNodes[i];break;}
cr++;}}}
if(!row)
return null;this.rows[rowNo]=new igtbl_Row(row,(this.Node?this.SelectedNodes[rowNo]:null),this,rowNo);}
return this.rows[rowNo];},"getRowById",function(rowId)
{for(var i=0;i<this.length;i++)
{var row=this.getRow(i);if(row.Element.id==rowId)
return row;}
return null;},"getColumn",function(colNo)
{var thead=this.Element.previousSibling;if(!thead||thead.tagName!="THEAD")
return;var j=-1;var metFixed=false;for(var i=0;i<this.Band.Columns.length;i++)
{var column=this.Band.Columns[i];if(column.hasCells())
j++;if(column.getFixed()===false&&!metFixed)
{metFixed=true;j=0;}
if(i==colNo)
break;}
if(j<0||j>=this.Band.Columns.length)
return null;if(this.Band.Columns[i].getFixed()===false)
{thead=thead.firstChild.cells[thead.firstChild.cells.length-1];return thead.firstChild.firstChild.rows[0].cells[j];}
return thead.firstChild.cells[j+this.Band.firstActiveCell];},"indexOf",function(row)
{if(row.IsAddNewRow)
return-1;if(row.IsFilterRow)
return-1;if(row.Node)
{var index=parseInt(row.Node.getAttribute("i"),10);if(typeof(this._getRowToStart)!="undefined")
index-=this._getRowToStart();return index;}
if(this.Grid.Bands.length==1&&!this.Grid.Bands[0].IsGrouped)
{var index=row.Element.sectionRowIndex;if(this.Band.AddNewRowVisible==1&&this.Band.AddNewRowView==1&&!this.Grid.StatHeader)
index--;if(this.Band.FilterUIType==1&&this.Band.FilterRowView==igtbl_featureRowView.Top&&!this.Grid.StatHeader)
index--;return index;}
var level=-1;var rId=row.Element.id,rows=this.Element.rows;for(var i=0;i<rows.length;i++)
{var r=rows[i];if(!r.getAttribute("hiddenRow")&&!r.getAttribute("addNewRow")&&!r.getAttribute("filterRow"))
level++;else
continue;if(r.id==rId)
return level;}
return-1;},"insert",function(row,rowNo)
{var g=this.Grid;if(!row||row.OwnerCollection&&row.OwnerCollection!=this)
{if(g.getActiveRow()==row)
g.setActiveRow(null);return false;}
if(!g._isSorting)
{if(g.fireEvent(g.Events.BeforeRowInsert,[g.Id,(this.ParentRow?this.ParentRow.Element.id:""),row.Element.id,rowNo])==true)
{if(g.getActiveRow()==row)
g.setActiveRow(null);return false;}}
var row1=this.getRow(rowNo);if(row1)
{if(this.rows.splice)
this.rows.splice(rowNo,0,row);else
this.rows=this.rows.slice(0,rowNo).concat(row,this.rows.slice(rowNo));this.Element.insertBefore(row.Element,row1.Element);if(row.Expandable&&row.HiddenElement&&!row.GroupByRow)
this.Element.insertBefore(row.HiddenElement,row1.Element);if(this.Node)
{var curNode=row.Node;var curIndex=igtbl_parseInt(row1.Node.getAttribute("i"));this.Node.insertBefore(row.Node,row1.Node);while(curNode&&curNode.nodeName=="R")
{curNode.setAttribute("i",curIndex++);curNode=curNode.nextSibling;}
this.SelectedNodes=this.Node.selectNodes("R");if(!this.SelectedNodes.length)
this.SelectedNodes=this.Node.selectNodes("Group");}}
else
{this.rows[this.rows.length]=row;this.Element.appendChild(row.Element);if(row.Expandable&&row.HiddenElement&&!row.GroupByRow)
this.Element.appendChild(row.HiddenElement);if(this.Node)
this.Node.appendChild(row.Node);row.Node.setAttribute("i",this.rows.length-1);}
this.length++;if(typeof(row._removedFrom)!="undefined")
{g._removeChange("DeletedRows",row);g._recordChange("MoveRow",row,row._removedFrom+":"+row.getLevel(true));if(row._Changes.MoveRow.length)
row._Changes.MoveRow[row._Changes.MoveRow.length-1].Node.setAttribute("Level",row._removedFrom);else
row._Changes.MoveRow.Node.setAttribute("Level",row._removedFrom);delete row._removedFrom;}
if(!g._isSorting)
{var oldNPB=g.NeedPostBack;g.fireEvent(g.Events.AfterRowInsert,[g.Id,row.Element.id,rowNo]);if(!oldNPB&&g.NeedPostBack&&!g.Events.AfterRowInsert[1]&2)
g.NeedPostBack=false;if(g.NeedPostBack)
igtbl_doPostBack(g.Id,"");}
return true;},"remove",function(rowNo,fireEvents)
{var row=this.getRow(rowNo);if(!row)
return;if(typeof(fireEvents)=="undefined")fireEvents=true;if(!this.Grid._isSorting)
{this.setLastRowId();if(fireEvents&&this.Grid.fireEvent(this.Grid.Events.BeforeRowDeleted,[this.Grid.Id,row.Element.id])==true)
return null;this.Grid._recordChange("DeletedRows",row);row._removedFrom=row.getLevel(true);}
this.Element.removeChild(row.Element);if(row.Expandable&&row.HiddenElement&&!row.GroupByRow)
this.Element.removeChild(row.HiddenElement);if(row.Node)
{var curNode=row.Node.nextSibling;row.Node.parentNode.removeChild(row.Node);while(curNode&&curNode.nodeName=="R")
{curNode.setAttribute("i",igtbl_parseInt(curNode.getAttribute("i"))-1);curNode=curNode.nextSibling;}
var rows=row.OwnerCollection;rows.SelectedNodes=rows.Node.selectNodes("R");if(!rows.SelectedNodes.length)
rows.SelectedNodes=rows.Node.selectNodes("Group");}
if(this.rows.splice)
this.rows.splice(rowNo,1);else
this.rows=this.rows.slice(0,rowNo).concat(this.rows.slice(rowNo+1));this.length--;if(fireEvents&&!this.Grid._isSorting)
this.Grid.fireEvent(this.Grid.Events.AfterRowDeleted,[this.Grid.Id,row.Element.id]);if(ig_csom.IsFireFox)
{if(this.Grid.getActiveRow()===row||this.Grid.getActiveRow()==null)
{this.Grid.setActiveRow(null);var tmp=this.Grid.Rows.getRow(0);if(tmp)
{this.Grid.setActiveRow(tmp);this.Grid.setActiveRow(null);}}
else
{var tmp=this.Grid.getActiveRow();this.Grid.setActiveRow(null);this.Grid.setActiveRow(tmp);}}
return row;},"sort",function(sortedCols)
{var issortch=false;if(!this.Grid._isSorting)
this.Grid._isSorting=issortch=true;if(typeof(igtbl_clctnSort)!="undefined")
igtbl_clctnSort.apply(this,[sortedCols]);if(issortch)
delete this.Grid._isSorting;},"getFooterText",function(columnKey)
{var tFoot;if(this.Band.Index==0&&this.Grid.StatFooter)
tFoot=this.Grid.StatFooter.Element;else
tFoot=this.Element.nextSibling;var col=this.Band.getColumnFromKey(columnKey);if(tFoot&&tFoot.tagName=="TFOOT"&&col)
{var fId=this.Grid.Id
+"_"
+"f_"+this.Band.Index+"_"+col.Index;for(var i=0;i<tFoot.rows[0].childNodes.length;i++)
if(tFoot.rows[0].childNodes[i].id==fId)
return igtbl_getInnerText(tFoot.rows[0].childNodes[i]);}
return"";},"setFooterText",function(columnKey,value,useMask)
{var tFoot;if(this.Band.Index==0&&this.Grid.StatFooter)
tFoot=this.Grid.StatFooter.Element;else
tFoot=this.Element.nextSibling;var col=this.Band.getColumnFromKey(columnKey);if(tFoot&&tFoot.tagName=="TFOOT"&&col)
{var fId=this.Grid.Id
+"_"
+"f_"+this.Band.Index+"_"+col.Index;if(useMask&&col.MaskDisplay)
value=igtbl_Mask(this.Grid.Id,value.toString(),col.DataType,col.MaskDisplay);var foot=igtbl_getChildElementById(tFoot,fId);if(foot)
{if(igtbl_string.trim(value)=="")
value="&nbsp;";if(foot.childNodes.length>0&&foot.childNodes[0].tagName=="NOBR")
value="<nobr>"+value+"</nobr>";foot.innerHTML=value;}}},"render",function()
{var strTransform=this.applyXslToNode(this.Node);if(strTransform)
{var anId=(this.AddNewRow?this.AddNewRow.Id:null);this.Grid._innerObj.innerHTML="<table style=\"table-layout:fixed;\">"+strTransform+"</table>";var tbl=this.Element.parentNode;igtbl_replaceChild(tbl,this.Grid._innerObj.firstChild.firstChild,this.Element);igtbl_fixDOEXml();var _b=this.Band;var headerDiv=igtbl_getElementById(this.Grid.Id+"_hdiv");var footerDiv=igtbl_getElementById(this.Grid.Id+"_fdiv");if(this.AddNewRow)
{if(_b.Index>0||_b.AddNewRowView==1&&!headerDiv||_b.AddNewRowView==2&&!footerDiv)
{var anr=this.AddNewRow.Element;anr.parentNode.removeChild(anr);if(_b.AddNewRowView==1&&tbl.tBodies[0].rows.length>0)
tbl.tBodies[0].insertBefore(anr,tbl.tBodies[0].rows[0]);else
tbl.tBodies[0].appendChild(anr);}
this.AddNewRow.Element=igtbl_getElementById(anId);this.AddNewRow.Element.Object=this.AddNewRow;}
this.Element=tbl.tBodies[0];this.Element.Object=this;this._setupFilterRow();for(var i=0;i<this.Band.Columns.length;i++)
{var column=this.Band.Columns[i];if(column.Selected&&column.hasCells())
{var col=this.getColumn(i);if(col)
igtbl_selColRI(this.Grid.Id,col,this.Band.Index,i);}}
if(this.ParentRow)
{this.ParentRow.ChildRowsCount=this.length;this.ParentRow.VisChildRowsCount=this.length;}}},"applyXslToNode",function(node)
{if(!node)return"";if(typeof(rowToStart)=="undefined")
rowToStart=0;var xslProc=this.Grid.XslProcessor;xslProc.input=node;var hasGrouped=false;if(this.SelectedNodes&&this.SelectedNodes.length&&this.SelectedNodes[0].nodeName=="Group")
hasGrouped=true;var prL="";if(this.ParentRow)
{prL=this.ParentRow.Element.id.split("_");prL=prL.slice(1);prL=prL.slice(1);prL=prL.join("_")+"_";}
if(hasGrouped)
{if(!this.Band._wdth)
{var pdng=0;if(this.Grid.get("StationaryMarginsOutlookGroupBy")!="True")
pdng=5;var wdth=0;if(this.Grid.Bands.length>0)
wdth+=22;if(this.Band.getRowSelectors()==1)
wdth+=22;for(var i=0;i<this.Band.Columns.length;i++)
if(this.Band.Columns[i].getVisible())
{var colWidth=this.Band.Columns[i].Width;if((colWidth||colWidth==="")&&typeof(colWidth)=="string"&&(colWidth.length<=2||colWidth.substr(colWidth.length-2)!="px"))
{wdth=0;break;}
wdth+=this.Band.Columns[i].getWidth()+pdng;}
if(wdth>0)
{var j=this.Band.getIndentation();for(var i=this.Band.SortedColumns.length-1;i>=0;i--)
{var col=igtbl_getColumnById(this.Band.SortedColumns[i]);if(!col.IsGroupBy)
continue;if(this.GroupColId==this.Band.SortedColumns[i])
break;j+=this.Band.getIndentation();}
wdth+=j;this.Band._wdth=wdth;}
else
this.Band._wdth="100%";}
node.setAttribute("grpWidth",this.Band._wdth);}
node.setAttribute("parentRowLevel",prL)
if(this.Grid.UseFixedHeaders&&this.Grid._scrElem.scrollLeft)
this.Grid.Node.setAttribute("fixedScrollLeft","left:"+(-this.Grid._scrElem.scrollLeft).toString()+"px;");else
this.Grid.Node.removeAttribute("fixedScrollLeft");xslProc.transform();return xslProc.output;},"_setupFilterRow",function()
{if(!this.FilterRow)return;var _b=this.Band;var headerDiv=igtbl_getElementById(this.Grid.Id+"_hdiv");var footerDiv=igtbl_getElementById(this.Grid.Id+"_fdiv");var tbl=this.Element.parentNode;var flr=this.FilterRow.Element;if(_b.Index>0||_b.FilterRowView==igtbl_featureRowView.Top&&(!headerDiv||this.length==0)||_b.FilterRowView==2&&!footerDiv)
{flr.parentNode.removeChild(flr);if(_b.FilterRowView==igtbl_featureRowView.Top&&tbl.tBodies[0].rows.length>0)
tbl.tBodies[0].insertBefore(flr,tbl.tBodies[0].rows[0]);else
{if(headerDiv)
{var oldHeaderHeight=headerDiv.offsetHeight;headerDiv.style.height="";if(ig_csom.IsFireFox)
{var mr=document.getElementById(this.Grid.Id+"_mr");if(mr&&mr.style.height.indexOf("px")!=-1)
{var headerHeight=headerDiv.offsetHeight;mr.style.height=(mr.offsetHeight+(oldHeaderHeight-headerHeight))+"px";}}}
tbl.tBodies[0].appendChild(flr);}}
else if(!this.Band.IsGrouped&&this.length>0&&headerDiv&&_b.FilterRowView==igtbl_featureRowView.Top&&!igtbl_dom.isParent(this.FilterRow.Element,headerDiv))
{flr.parentNode.removeChild(flr);if(headerDiv)
headerDiv.style.height="";headerDiv.firstChild.tBodies[0].appendChild(flr);}
var filterRowElement=igtbl_getElementById(this.FilterRow.Id);if(filterRowElement)
{this.FilterRow.Element=filterRowElement;this.FilterRow.Element.Object=this.FilterRow;}},"getHeaderRow",function()
{if(this.Band.Index==0&&this.Grid.StatHeader&&this.length>0)
{return this.Grid.StatHeader.Element;}
return null;},"addNew",function()
{var g=this.Grid;if(this.AddNewRow)
return igtbl_activateAddNewRow(this.Grid,this.Band.Index,this.ParentRow);return igtbl_rowsAddNew(g.Id,this.ParentRow);},"dispose",function(self)
{for(var i=0;i<this.rows.length;i++)
{if(this.rows[i])
{if(this.rows[i].Rows)
this.rows[i].Rows.dispose(true);igtbl_cleanRow(this.rows[i]);}}
igtbl_dispose(this.rows);delete this.rows;if(self)
{this.Grid=null;this.Band=null;this.ParentRow=null;this.deletedRows=null;this.Element.Object=null;if(this.AddNewRow)
igtbl_cleanRow(this.AddNewRow);if(this.FilterRow)
igtbl_cleanRow(this.FilterRow);igtbl_dispose(this);}
else
this.rows=new Array();},"reIndex",function(sRow)
{for(var i=sRow;i<this.length;i++)
this.getRow(i).Node.setAttribute("i",i.toString());},"repaint",function()
{var strTransform=this.applyXslToNode(this.Node);if(strTransform)
{var anId=(this.AddNewRow?this.AddNewRow.Id:null);this.Grid._innerObj.innerHTML="<table>"+strTransform+"</table>";var tbl=this.Element.parentNode;var newEl=this.Grid._innerObj.firstChild.firstChild;for(var i=this.rows.length-1;i>=0;i--)
if(this.rows[i])
{if(this.rows[i].HiddenElement)
{if(i==newEl.rows.length-1)
newEl.appendChild(this.rows[i].HiddenElement);else
newEl.insertBefore(this.rows[i].HiddenElement,newEl.rows[i+1]);var img=newEl.rows[i].firstChild;if(this.rows[i].getExpanded()&&img)
{img=newEl.rows[i].firstChild.firstChild;if(img&&img.tagName=="IMG")
{img.src=this.Band.getCollapseImage();var alt=img.getAttribute("alt");if(alt!=null)
{var clpsAlt=img.getAttribute("igAltC");if(clpsAlt!=null)
{img.setAttribute("igAltX",alt);img.setAttribute("alt",clpsAlt);img.removeAttribute("igAltC");}}}}}
var row=this.rows[i];var reSelectRow=false;if(row.getSelected())
{reSelectRow=true;row.select(false,false);}
row.Element=newEl.rows[i];row.Element.Object=row;var metFixed=false;var ri=0;for(var j=0;row.cells&&j<row.cells.length;j++)
{var cell=row.cells[j];var column=this.Band.Columns[j];if(column.getFixed()===false&&!metFixed)
{metFixed=true;ri=0;}
if(cell)
{cell.Column=column;cell.Index=j;if(cell.Column.hasCells())
{var reSelectCell=false;if(cell.getSelected())
{reSelectCell=true;cell.select(false,false);}
if(cell.Column.getFixed()===false)
{rowEl=row.Element.cells[row.Element.cells.length-1];cell.Element=rowEl.firstChild.firstChild.rows[0].cells[ri];}
else
cell.Element=row.Element.cells[cell.Column.getRealIndex()+this.Band.firstActiveCell];var nodePosition=parseInt(cell.Column.Node.getAttribute("cellIndex"))-1;if(nodePosition<row.Node.selectSingleNode("Cs").childNodes.length)
cell.Node=row.Node.selectSingleNode("Cs").childNodes[nodePosition];cell.Element.Object=cell;cell.Id=cell.Element.id;if(reSelectCell)
cell.select(true,false);ri++;}
else
cell.Element=null;}
else if(column.hasCells())
ri++;}
if(reSelectRow)
row.select(true,false);}
var anr;if(this.AddNewRow)
{if(this.Band.AddNewRowView==1&&(this.Band.Index>0||!igtbl_getElementById(this.Grid.Id+"_hdiv")))
anr=this.AddNewRow.Element;}
if(anr)
{while(anr.nextSibling)
anr.parentNode.removeChild(anr.nextSibling)
while(newEl.rows.length)
anr.parentNode.appendChild(newEl.rows[0]);}
else
igtbl_replaceChild(tbl,newEl,this.Element);igtbl_fixDOEXml();this.Element=tbl.tBodies[0];this.Element.Object=this;if(this.AddNewRow)
{if(this.Band.AddNewRowView==2&&(this.Band.Index>0||!igtbl_getElementById(this.Grid.Id+"_fdiv")))
{anr=this.AddNewRow.Element;tbl.tBodies[0].appendChild(anr);}
this.AddNewRow.Element=igtbl_getElementById(anId);this.AddNewRow.Element.Object=this.AddNewRow;}}},"_buildSortXmlQueryString",function(op)
{var g=this.Grid;var row=this.ParentRow;g.QueryString=op+"\x01";if(row)
g.QueryString+=row.getLevel(true);var sqlWhere="";var sortOrder="";for(var i=0;i<=this.Band.Index;i++)
{var cr=row;var sqlW="";while(cr&&cr.Band!=g.Bands[i])
cr=cr.ParentRow;if(g.Bands[i].DataKeyField&&cr&&cr.get(igtbl_litPrefix+"DataKey"))
sqlW+=cr._generateSqlWhere(g.Bands[i].DataKeyField,unescape(cr.get(igtbl_litPrefix+"DataKey")));else if(g.Bands[i]._sqlWhere)
{if(sqlW)
sqlW+=" AND ";sqlW+=g.Bands[i]._sqlWhere;}
sqlWhere=sqlW+(i==this.Band.Index?"":";");}
for(var i=0;i<g.Bands.length;i++)
{var so="";for(var j=0;j<g.Bands[i].SortedColumns.length;j++)
{var col=igtbl_getColumnById(g.Bands[i].SortedColumns[j]);so+=col.Key+(col.SortIndicator==2?" DESC":"")+(j<g.Bands[i].SortedColumns.length-1?",":"");}
sortOrder+=so+(i==g.Bands.length-1?"":";");}
var band=this.Band,sCols;if(band)
{sCols=band.Index;for(var i=0;i<band.SortedColumns.length;i++)
{var col=igtbl_getColumnById(band.SortedColumns[i]);sCols+="|"+col.Index;sCols+=":"+col.IsGroupBy.toString();sCols+=":"+col.SortIndicator;}}
g.QueryString+="\x02"+sqlWhere;g.QueryString+="\x02"+sortOrder;if(this.Band.ColumnsOrder)
g.QueryString+="\x02"+this.Band.ColumnsOrder;g.QueryString+="\x02"+sCols;var currentFilters="";if(this.hasRowFilters())
{var bandFilters=this.CurrentFilterScope();if(bandFilters)
{for(var colId in bandFilters)
{var filter=bandFilters[colId];if(filter.IsActive())
{var col=igtbl_getColumnById(colId);currentFilters+=col.getLevel(true)+"\x05"+filter.getOperator()+"\x05"+filter.getEvaluationValue()+"\x03";}}}}
g.QueryString+="\x02"+currentFilters;},"sortXml",function(sortedCols)
{if(this.Band.SortedColumns.length==0)
return;var g=this.Grid;this._buildSortXmlQueryString("Sort");g.RowToQuery=this.ParentRow;g.xmlHttpRequest(g.eReqType.Sort);},"getLastRowId",function()
{if(!this.lastRowId)
this.setLastRowId();return this.lastRowId;},"setLastRowId",function(lrId)
{if(arguments.length==0&&!this.lastRowId)
{if(this.length>0)
this.lastRowId=this.getRow(this.length-1).Element.id;}
else if(lrId)
this.lastRowId=lrId;},"CurrentFilterScope",function()
{if(this.Band.RowFilterMode==1||(this.Band.Index==0&&!this.Band.IsGrouped))
{return this.Band._filterPanels;}
if(this.Band.RowFilterMode==2)
{var filterPanels=this.Band._filterPanels;var myTable=this.Element;while(myTable!=null&&myTable.tagName!="TABLE")
{myTable=myTable.parentNode;}
if(!filterPanels||!myTable)return null;for(var fp in filterPanels)
{if(myTable.id==fp)
return filterPanels[fp];}}
return null;},"hasRowFilters",function()
{switch(this.Band.RowFilterMode)
{case"1":case 1:{var filterPanels=this.Band._filterPanels;if(!filterPanels||igtbl_getLength(filterPanels)==0)return false;return true;break;}
case"2":case 2:{var filterPanels=this.Band._filterPanels;if(filterPanels&&this.Band.Index==0&&!this.Band.IsGrouped&&igtbl_getLength(filterPanels)>0)return true;var myTable=this.Element;while(myTable!=null&&myTable.tagName!="TABLE")
{myTable=myTable.parentNode;}
if(!filterPanels||!myTable)return false;for(var fp in filterPanels)
{if(myTable.id==fp)
return true;}
break;}
default:return false;}
return false;},"refresh",function(data)
{var g=this.Grid;g.setActiveCell(null);g.setActiveRow(null);g.clearSelectionAll();this._buildSortXmlQueryString("Refresh");if(data)
g.QueryString+="\x02"+data;g.RowToQuery=this.ParentRow;g.xmlHttpRequest(g.eReqType.Refresh);},"getFilterRow",function()
{if(this.Band.RowFilterMode==1)return null;if(this.Band.FilterUIType!=1)return null;return this.FilterRow;}];for(var i=0;i<igtbl_ptsRows.length;i+=2)
igtbl_Rows.prototype[igtbl_ptsRows[i]]=igtbl_ptsRows[i+1];igtbl_StateChange.prototype=new igtbl_WebObject();igtbl_StateChange.prototype.constructor=igtbl_StateChange;igtbl_StateChange.base=igtbl_WebObject.prototype;function igtbl_StateChange(type,grid,obj,value)
{if(arguments.length>0)
this.init(type,grid,obj,value);}
igtbl_StateChange.prototype.init=function(type,grid,obj,value)
{igtbl_StateChange.base.init.apply(this,[type]);this.Node=ig_ClientState.addNode(grid.StateChanges,"StateChange");this.Grid=grid;this.Object=obj;ig_ClientState.setPropertyValue(this.Node,"Type",this.Type);if(typeof(value)!="undefined"&&value!=null)
{if(value==""&&typeof(value)=="string")value="\x01";ig_ClientState.setPropertyValue(this.Node,"Value",value);}
if(obj)
{if(obj.getLevel)
ig_ClientState.setPropertyValue(this.Node,"Level",obj.getLevel(true));var dataKey=null;if(obj.Type=="row"||obj.Type=="cell"||obj.Type=="rows")
{var row=obj;if(obj.Type=="cell")
row=obj.Row;else if(obj.Type=="rows")
row=obj.ParentRow;if(row)
{dataKey=(row.DataKey?row.DataKey:"");while(row.ParentRow)
{row=row.ParentRow;dataKey=(row.DataKey?row.DataKey:"")+"\x04"+dataKey;}}}
if(dataKey)
ig_ClientState.setPropertyValue(this.Node,"DataKey",dataKey);if(this.Object._Changes[this.Type])
{var ch=this.Object._Changes[this.Type];if(!ch.length)
ch=new Array(ch);this.Object._Changes[this.Type]=ch.concat(this);}
else
this.Object._Changes[this.Type]=this;}}
igtbl_StateChange.prototype.remove=function(lastOnly)
{if(lastOnly&&this.Grid.StateChanges.lastChild!=this.Node)
return;ig_ClientState.removeNode(this.Grid.StateChanges,this.Node);var ch=this.Object._Changes[this.Type];if(ch.length)
{for(var i=0;i<ch.length;i++)
if(ch[i]==this)
{ch=this.Object._Changes[this.Type]=ch.slice(0,i).concat(ch.slice(i+1));break;}
if(ch.length==1)
{this.Object._Changes[this.Type]=ch[0];ch[0]=null;igtbl_dispose(ch);}}
else
delete this.Object._Changes[this.Type];this.Grid=null;this.Object=null;this.Node=null;igtbl_dispose(this);}
igtbl_StateChange.prototype.setFireEvent=function(value)
{ig_ClientState.setPropertyValue(this.Node,"FireEvent",value.toString());}
function igtbl_XSLTProcessor(xsltURL)
{if(!xsltURL)
return null;if(ig_csom.IsIE)
{var xslt=ig_createActiveXFromProgIDs(["MSXML2.FreeThreadedDOMDocument","Microsoft.FreeThreadedXMLDOM"]);xslt.async=false;xslt.load(xsltURL);var xslTemplate=new ActiveXObject("MSXML2.XSLTemplate");xslTemplate.stylesheet=xslt;this.Processor=xslTemplate.createProcessor();}
else
{var xmlResp=new DOMParser();var xmlHttp=new XMLHttpRequest();xmlHttp.open("GET",xsltURL,false);xmlHttp.send(null);this.Processor=new XSLTProcessor();this.Processor.importStylesheet(xmlResp.parseFromString(xmlHttp.responseText,"text/xml"));}}
igtbl_XSLTProcessor.prototype.addParameter=function(name,value)
{if(!this.Processor)return null;if(ig_csom.IsIE)
return this.Processor.addParameter(name,value);else
return this.Processor.setParameter(null,name,value);};igtbl_XSLTProcessor.prototype.transform=function()
{if(!this.input)
return false;if(ig_csom.IsIE)
{this.Processor.input=this.input;this.Processor.transform();this.output=this.Processor.output;}
else
return this.outputDocument=this.Processor.transformToDocument(this.input);return true;};if(document.implementation&&document.implementation.createDocument)
{igtbl_XSLTProcessor.prototype.__defineGetter__("output",function _igtbl_XSLTProcOutput()
{if(ig_csom.IsIE)
return this.Processor.output;else
{if(!this.outputDocument||!this.outputDocument.firstChild)
return null;var output=this.outputDocument.firstChild.innerHTML;if(!output)
output="<tbody></tbody>";return output;}});XMLDocument.prototype.selectNodes=Element.prototype.selectNodes=function(sExpr)
{try
{var xpe=new XPathEvaluator();var nsResolver=xpe.createNSResolver(this.ownerDocument==null?this.documentElement:this.ownerDocument.documentElement);var result=xpe.evaluate(sExpr,this,nsResolver,0,null);var found=[];var res;while(res=result.iterateNext())
found.push(res);return found;}
catch(exc){;}
return null;};XMLDocument.prototype.selectSingleNode=Element.prototype.selectSingleNode=function(sExpr)
{try
{var xpe=new XPathEvaluator();var nsResolver=xpe.createNSResolver(this.ownerDocument==null?this.documentElement:this.ownerDocument.documentElement);var result=xpe.evaluate(sExpr,this,nsResolver,0,null);var res=result.iterateNext();return res;}
catch(exc){;}
return null;};Element.prototype.__defineGetter__("text",function(){return this.textContent;});Element.prototype.__defineSetter__("text",function(sText){this.textContent=sText;});CDATASection.prototype.__defineGetter__("text",function(){return this.textContent;});CDATASection.prototype.__defineSetter__("text",function(sText){this.textContent=sText;});}
function igtbl_getNodeValue(node)
{var value=node.getAttribute("uV");if(value!==null)
{if(value.indexOf&&value.indexOf('&#')>=0)
{var elem=ig_shared._gridValueFilter;if(!elem)
elem=ig_shared._gridValueFilter=document.createElement('SPAN');elem.innerHTML=value;value=elem.innerHTML;}
return unescape(value);}
value=node.getAttribute("iDV");if(value!==null)
return unescape(value);value=node.getAttribute("iCT");if(value!==null)
return unescape(value);value=node.firstChild.text;if(value=="&nbsp;")
value="";value=value.replace(/<br\/>/g,"\r\n");return value;}
function igtbl_setNodeValue(node,value,displayValue)
{var valueSet=false;if(node.getAttribute("uV")!=null)
{node.setAttribute("uV",igtbl_escape(value));valueSet=true;}
if(node.getAttribute("iDV")!=null)
{node.setAttribute("iDV",igtbl_escape(value));valueSet=true;}
if(node.getAttribute("iCT")!=null)
{node.setAttribute("iCT",igtbl_escape(value));valueSet=true;}
if(displayValue)
node.firstChild.text=displayValue;else if(!valueSet)
node.firstChild.text=(value===""?"&nbsp;":value.toString());}
var igtbl_documentMouseMove=null;var igtbl_documentMouseUp=null;function igtbl_dragDropMouseMove(evnt)
{if(!evnt)
evnt=event;var gs=igtbl_getGridById(igtbl_lastActiveGrid);if(!gs&&igtbl_documentMouseMove||(igtbl_button(igtbl_lastActiveGrid,evnt)!=0&&!gs.Element.getAttribute("mouseDown")))
{igtbl_headerDragDrop();return;}
if(!gs)
return;gs.event=evnt;if(gs._colMovingTimerID)
{window.clearTimeout(gs._colMovingTimerID);gs._colMovingTimerID=null;}
if(gs.dragDropDiv&&gs.dragDropDiv.style.display=="")
{var col=gs.dragDropDiv.srcElement;var bandNo=parseInt(igtbl_bandNoFromColId(col.id),10);var band=gs.Bands[bandNo];var colNo=parseInt(igtbl_colNoFromColId(col.id),10);var x=evnt.clientX+igtbl_getBodyScrollLeft();var y=evnt.clientY+igtbl_getBodyScrollTop();gs.dragDropDiv.style.left=(x-gs.dragDropDiv.offsetWidth/2)+"px";gs.dragDropDiv.style.top=(y-gs.dragDropDiv.offsetHeight/2)+"px";var gb=gs.GroupByBox;var gbx;var gby;var pNode=gs.Element.parentNode;var totalScrollTop=0;var totalScrollLeft=0
while(pNode&&pNode.tagName!="BODY")
{totalScrollTop+=pNode.scrollTop;totalScrollLeft+=pNode.scrollLeft;pNode=pNode.parentNode;}
totalScrollLeft-=gs.DivElement.scrollLeft;totalScrollTop-=gs.DivElement.scrollTop;if(gb.Element)
{gbx=igtbl_getLeftPos(gb.Element,false);gbx-=totalScrollLeft;gby=igtbl_getTopPos(gb.Element,false);gby-=totalScrollTop;}
if(gb.Element&&x>=gbx&&x<gbx+gb.Element.offsetWidth&&y>=gby&&y<gby+gb.Element.offsetHeight&&band.Columns[colNo].AllowGroupBy==1)
{if(gb.groups.length==0)
{gb.pimgUp.style.display="";gb.pimgUp.style.left=(gbx-gb.pimgUp.offsetWidth/2)+"px";gb.pimgUp.style.top=(gby+gb.Element.offsetHeight)+"px";gb.pimgDn.style.display="";gb.pimgDn.style.left=(gbx-gb.pimgDn.offsetWidth/2)+"px";gb.pimgDn.style.top=(gby-gb.pimgDn.offsetHeight)+"px";gb.postString="group:"+bandNo+":"+colNo+":true:band:"+bandNo;}
else
{var el=null;var frontPark=false;var grNo=0;for(var i=0;i<gb.groups.length;i++)
{var ge=gb.groups[i].Element;var gex=igtbl_getLeftPos(ge,false);gex-=totalScrollLeft;var gey=igtbl_getTopPos(ge,false);gey-=totalScrollTop;var eBandNo=gb.groups[i].groupInfo[1];if(eBandNo<bandNo)
{el=gb.groups[i];grNo=i;frontPark=false;}
else if(eBandNo==bandNo)
{if(!(el&&x<gex))
{el=gb.groups[i];grNo=i;if(el.groupInfo[0]=='band'||x<gex+ge.offsetWidth/2)
frontPark=true;else
frontPark=false;if(x>=gex&&x<gex+ge.offsetWidth)
break;}}
else if(!el)
{el=gb.groups[i];grNo=i;frontPark=true;}}
if(el&&(((el.groupInfo[0]=="col"&&!(el.groupInfo[1]==bandNo&&el.groupInfo[2]==colNo)||el.groupInfo[0]=="band")&&(frontPark&&(grNo==0||gb.groups[grNo-1].groupInfo[0]=="band"||!(gb.groups[grNo-1].groupInfo[1]==bandNo&&gb.groups[grNo-1].groupInfo[2]==colNo))||!frontPark&&(grNo>=gb.groups.length-1||gb.groups[grNo+1].groupInfo[0]=="band"||!(gb.groups[grNo+1].groupInfo[1]==bandNo&&gb.groups[grNo+1].groupInfo[2]==colNo))))))
{var gex=igtbl_getLeftPos(el.Element,false);gex-=totalScrollLeft;var gey=igtbl_getTopPos(el.Element,false);gey-=totalScrollTop;gb.pimgUp.style.display="";gb.pimgUp.style.left=(gex-gb.pimgUp.offsetWidth/2+(frontPark?0:el.Element.offsetWidth))+"px";gb.pimgUp.style.top=(gey+el.Element.offsetHeight)+"px";gb.pimgDn.style.display="";gb.pimgDn.style.left=(gex-gb.pimgDn.offsetWidth/2+(frontPark?0:el.Element.offsetWidth))+"px";gb.pimgDn.style.top=(gey-gb.pimgDn.offsetHeight)+"px";gb.postString="group:"+bandNo+":"+colNo+":"+frontPark+":"+el.groupInfo[0]+":"+el.groupInfo[1]+(el.groupInfo[0]=="col"?":"+el.groupInfo[2]:"");}
else
{gb.postString="";gb.moveString="";gb.pimgUp.style.display="none";gb.pimgDn.style.display="none";}}}
else
{var defaultInit=true;if(band.AllowColumnMoving>1&&!band.HasHeaderLayout&&!band.HasFooterLayout)
{var gdiv;if(bandNo==0)
{if((gs.StationaryMargins==1||gs.StationaryMargins==3)&&gs.StatHeader)
gdiv=gs.StatHeader.Element.parentNode.parentNode;else
gdiv=gs.Element.parentNode;}
else
gdiv=col.parentNode;var gx=igtbl_getLeftPos(gdiv);var gy=igtbl_getTopPos(gdiv);if((ig_csom.IsIE6||ig_csom.IsIE7)&&igtbl_isXHTML&&gs.Bands.length>1)
{gx=gx-gdiv.scrollLeft;gy=gy-gs.DivElement.scrollTop;}
var colEl=igtbl_overHeader(gs.Rows,x,y,gx,gy,gdiv.offsetWidth,gdiv.offsetHeight,totalScrollTop,totalScrollLeft);if(colEl)
{var tBandNo=parseInt(igtbl_bandNoFromColId(colEl.id),10);var tColNo=parseInt(igtbl_colNoFromColId(colEl.id),10);if(tBandNo==bandNo&&tColNo!=colNo)
{var cx=igtbl_getLeftPos(colEl,false);var cy=igtbl_getTopPos(colEl,false);if((ig_csom.IsIE6||ig_csom.IsIE7)&&igtbl_isXHTML&&gs.Bands.length>1)
{cx=cx-gdiv.scrollLeft;cy=cy-gs.DivElement.scrollTop;}
var ow=colEl.offsetWidth;if(cx+ow>gx+gdiv.offsetWidth)
{ow=gx+gdiv.offsetWidth-cx;}
else if(cx<gx)
{ow=ow-(gx-cx);cx=gx;}
var frontPark=false;if(x<(cx-totalScrollLeft)+ow/2)
frontPark=true;var beforeColId=colEl.id;var col=gs.Bands[tBandNo].Columns[tColNo];var mCol=band.Columns[colNo];var beforeCol=gs.Bands[tBandNo].Columns[tColNo+1];if(beforeCol==mCol&&mCol.IsGroupBy)
beforeCol=gs.Bands[tBandNo].Columns[tColNo+2];if(!frontPark&&beforeCol)
beforeColId=beforeCol.Id;else if(!frontPark)
beforeColId=null;if(gs.UseFixedHeaders&&frontPark&&!col.getFixed())
{var overlapColEl=igtbl_overHeader(gs.Rows,cx,y,gx,gy,gdiv.offsetWidth,gdiv.offsetHeight,totalScrollTop,totalScrollLeft);if(overlapColEl&&colEl!=overlapColEl&&igtbl_getColumnById(overlapColEl.id).getFixed())
{newX=igtbl_getLeftPos(overlapColEl,false)+overlapColEl.offsetWidth;ow=cx-newX;cx=newX;col.colMovingFixedEdge=true;}}
var allowMove=false;if(!gs.UseFixedHeaders||(frontPark&&(mCol.Fixed&&(col.Fixed||tColNo>0&&tColNo-1!=colNo&&gs.Bands[tBandNo].Columns[tColNo-1].Fixed)||!mCol.Fixed&&!col.Fixed)||!frontPark&&(mCol.Fixed&&col.Fixed||!mCol.Fixed&&(!beforeCol||mCol!=beforeCol&&!beforeCol.Fixed))))
allowMove=true;if(allowMove&&(frontPark&&(!colEl.previousSibling||!colEl.previousSibling.id||parseInt(igtbl_colNoFromColId(colEl.previousSibling.id),10)!=colNo)||!frontPark&&(!colEl.nextSibling||!colEl.nextSibling.id||parseInt(igtbl_colNoFromColId(colEl.nextSibling.id),10)!=colNo)))
{if(igtbl_fireEvent(gs.Id,gs.Events.ColumnDrag,"(\""+gs.Id+"\",\""+mCol.Id+"\","+(beforeColId?"\""+beforeColId+"\"":null)+")")!=true)
{gb.pimgUp.style.display="";gb.pimgUp.style.left=(cx-totalScrollLeft-gb.pimgUp.offsetWidth/2+(frontPark?0:ow))+"px";gb.pimgUp.style.top=(cy-totalScrollTop+colEl.offsetHeight)+"px";gb.pimgDn.style.display="";gb.pimgDn.style.left=(cx-totalScrollLeft-gb.pimgDn.offsetWidth/2+(frontPark?0:ow))+"px";gb.pimgDn.style.top=(cy-totalScrollTop-gb.pimgDn.offsetHeight)+"px";if(gs.dragDropDiv.srcElement&&gs.dragDropDiv.srcElement.getAttribute("groupInfo"))
gb.postString="ungroup:"+bandNo+":"+colNo;else
gb.postString="";gb.moveString="move:"+bandNo+":"+colNo+":"+frontPark+":"+tBandNo+":"+tColNo;defaultInit=false;if(cx-gx<=5||(col.colMovingFixedEdge&&frontPark))
{gs._colMovingTimerID=window.setTimeout("_igtbl_columnMovingScroll('"+gs.Id+"',"+tBandNo+","+col.Index+", 'left')",500);col.colMovingFixedEdge=undefined;}
else if((gx+gdiv.offsetWidth)-(cx+ow)<=5)
gs._colMovingTimerID=window.setTimeout("_igtbl_columnMovingScroll('"+gs.Id+"',"+tBandNo+","+col.Index+", 'right')",500);else if(gs.UseFixedHeaders&&col.getFixed()&&!frontPark&&!mCol.getFixed())
{var firstColEl=igtbl_overHeader(gs.Rows,cx+ow+4,y,gx,gy,gdiv.offsetWidth,gdiv.offsetHeight,totalScrollTop,totalScrollLeft);if(firstColEl)
{var firstCol=igtbl_getColumnById(firstColEl.id);if(firstCol&&!firstCol.getFixed())
gs._colMovingTimerID=window.setTimeout("_igtbl_columnMovingScroll('"+gs.Id+"',"+tBandNo+","+firstCol.Index+", 'left')",700);}}}}}}}
if(defaultInit)
{if(col&&col.getAttribute&&col.getAttribute("groupInfo"))
gb.postString="ungroup:"+bandNo+":"+colNo;else
gb.postString="";gb.moveString="";gb.pimgUp.style.display="none";gb.pimgDn.style.display="none";}}}
igtbl_cancelEvent(evnt);return true;}
function _igtbl_columnMovingScroll(gn,bandIndex,columnIndex,direction)
{var g=igtbl_getGridById(gn);var col=g.Bands[bandIndex].Columns[columnIndex];window.clearTimeout(g._colMovingTimerID);g._colMovingTimerID=null;if(!col.getFixed())
{var scrollToCol=null;if(direction=='left')
scrollToCol=_igtbl_previousVisibleColumn(col)
else
scrollToCol=_igtbl_nextVisibleColumn(col);if(scrollToCol!=null)
{igtbl_scrollToView(gn,scrollToCol.Element,scrollToCol.offsetWidth);g._colMovingTimerID=setTimeout("_igtbl_columnMovingScroll('"+g.Id+"',"+bandIndex+","+scrollToCol.Index+",'"+direction+"')",500);}}}
function _igtbl_previousVisibleColumn(col)
{if(col.Index>0)
{var prevCol=col.Band.Columns[col.Index-1];while(prevCol!=null&&prevCol.getHidden())
{if(prevCol.Index>0)
prevCol=col.Band.Columns[prevCol.Index-1];else
prevCol=null;}
return prevCol;}
return null;}
function _igtbl_nextVisibleColumn(col)
{if(col.Index<col.Band.Columns.length-1)
{var nextCol=col.Band.Columns[col.Index+1];while(nextCol!=null&&nextCol.getHidden())
{if(nextCol.Index<col.Band.Columns.length-1)
nextCol=col.Band.Columns[nextCol.Index+1];else
nextCol=null;}
return nextCol;}
return null;}
function igtbl_overHeader(rows,x,y,gx,gy,gw,gh,totalScrollTop,totalScrollLeft)
{if(!totalScrollTop)
totalScrollTop=0;if(!totalScrollLeft)
totalScrollLeft=0;var g=rows.Grid;var useExp=0;y+=totalScrollTop;while(rows)
{var firstRow=rows.length>0?rows.getRow(0):null;if(!firstRow||(firstRow&&!firstRow.GroupByRow)||(rows.Band.Index==0&&rows.Grid.StatHeader))
{var colsCount;if(firstRow&&!firstRow.GroupByRow)
colsCount=firstRow.cells.length;else
colsCount=rows.Band.Columns.length;for(var i=0;i<colsCount;i++)
{if(!rows.Band.Columns[i].getVisible())
continue;var colEl;if(firstRow&&!firstRow.GroupByRow)
{var cell=firstRow.getCell(i);colEl=igtbl_getColumnByCellId(cell.Element.id);}
else
colEl=rows.Band.Columns[i].Element;if(colEl)
{var cy=igtbl_getTopPos(colEl);if(y>gy+gh)
return false;var cx=igtbl_getLeftPos(colEl);if((ig_csom.IsIE6||ig_csom.IsIE7)&&igtbl_isXHTML&&!g.StatHeader)
{cx=cx-g.DivElement.scrollLeft;cy=cy-g.DivElement.scrollTop;}
var cx1=cx+colEl.offsetWidth;var cy1=cy+colEl.offsetHeight;if(cx<gx)cx=gx;if(cy<gy)cy=gy;if(cx1>gx+gw)cx1=gx+gw;if(cy1>gy+gh)cy1=gy+gh;if(!(y>cy&&y<cy1))
break;if(x>=(cx-totalScrollLeft)&&x<(cx1-totalScrollLeft))
{return colEl;}}}}
rows=null;var i=0;for(var rowId in g.ExpandedRows)
{if(i==useExp)
{var row=igtbl_getRowById(rowId);rows=row.Rows;useExp++;break;}
i++;}}}
function igtbl_headerDragStart(gn,se,evnt)
{var gs=igtbl_getGridById(gn);if(!gs)return;var column=igtbl_getColumnById(se.id);if(!column)return;if(!column.IsGroupBy)
{var j=0;for(var i=0;i<column.Band.Columns.length;i++)
{var col=column.Band.Columns[i];if(col.hasCells()&&col.getVisible())
j++;}
if(j<=1)
return;}
if(igtbl_fireEvent(gs.Id,gs.Events.BeforeColumnMove,"(\""+gs.Id+"\",\""+se.id+"\")")==true)
return;if(!gs.dragDropDiv)
{gs.dragDropDiv=document.createElement("DIV");gs.dragDropDiv.style.display="none";document.body.insertBefore(gs.dragDropDiv,document.body.firstChild);var gb=gs.GroupByBox;if(gb&&gb.pimgUp.parentNode!=document.body)
{gb.pimgUp.parentNode.removeChild(gb.pimgUp);document.body.insertBefore(gb.pimgUp,document.body.firstChild);gb.pimgDn.parentNode.removeChild(gb.pimgDn);document.body.insertBefore(gb.pimgDn,document.body.firstChild);}
gs.dragDropDiv.setAttribute("GroupByHeaderFloatingDiv",1);}
gs.dragDropDiv.style.position="absolute";gs.dragDropDiv.style.display="";if(ig_csom.IsNetscape6)
{gs.dragDropDiv.style.MozOpacity="0.6";gs.dragDropDiv.style.cursor="-moz-grabbing";}
else if(ig_csom.IsIE)
gs.dragDropDiv.style.filter+="progid:DXImageTransform.Microsoft.Alpha(Opacity=60);";gs.dragDropDiv.style.left=(evnt.clientX+igtbl_getBodyScrollLeft()-se.offsetWidth/2)+"px";gs.dragDropDiv.style.top=(evnt.clientY+igtbl_getBodyScrollTop()-se.offsetHeight/2)+"px";gs.dragDropDiv.style.width=se.offsetWidth+"px";gs.dragDropDiv.style.height=se.offsetHeight+"px";gs.dragDropDiv.style.zIndex=gs._getZ(100000,1);gs.dragDropDiv.innerHTML="<table style=\"width:100%;height:100%\"><thead><tr><th></th></tr></thead></table>";var th=gs.dragDropDiv.firstChild.firstChild.firstChild.firstChild;th.innerHTML=se.innerHTML;srcTh=se;while(th.tagName!="TABLE")
{th.className=srcTh.className;th.style.cssText=srcTh.style.cssText;th=th.parentNode;srcTh=srcTh.parentNode;}
gs.dragDropDiv.srcElement=se;igtbl_documentMouseMove=igtbl_addEventListener(document,"mousemove",igtbl_dragDropMouseMove);igtbl_documentMouseUp=igtbl_addEventListener(document,"mouseup",igtbl_headerDragDrop);}
function igtbl_headerDragDrop()
{var gs=igtbl_getGridById(igtbl_lastActiveGrid);if(!gs||!gs.dragDropDiv)
return;if(gs._colMovingTimerID)
{window.clearTimeout(gs._colMovingTimerID);gs._colMovingTimerID=null;}
gs.dragDropDiv.style.display="none";igtbl_removeEventListener(document,"mousemove",igtbl_dragDropMouseMove,igtbl_documentMouseMove);igtbl_removeEventListener(document,"mouseup",igtbl_headerDragDrop,igtbl_documentMouseUp);igtbl_documentMouseUp=null;igtbl_documentMouseMove=null;gs.GroupByBox.pimgUp.style.display="none";gs.GroupByBox.pimgDn.style.display="none";var col=gs.dragDropDiv.srcElement;_igtbl_processUpdates(gs,null);var bandNo=parseInt(igtbl_bandNoFromColId(col.id),10);var band=gs.Bands[bandNo];var xmlClientSideMoving=(gs.Node&&band.AllowColumnMoving==3);if(gs.GroupByBox.moveString!=""&&!gs.GroupByBox.postString&&!xmlClientSideMoving)
igtbl_fireEvent(gs.Id,gs.Events.AfterColumnMove,"(\""+gs.Id+"\",\""+col.id+"\")");if(gs.Node&&band.AllowColumnMoving==3&&gs.GroupByBox.moveString!=""&&gs.GroupByBox.postString=="")
{var moveAr=gs.GroupByBox.moveString.split(":");var fromIndex=parseInt(moveAr[2],10),toIndex=parseInt(moveAr[5],10)+(moveAr[3]=="true"?0:1);if(fromIndex<toIndex)
toIndex--;band.Columns[fromIndex].move(toIndex);if(gs.GroupByBox.moveString!=""&&xmlClientSideMoving)
igtbl_fireEvent(gs.Id,gs.Events.AfterColumnMove,"(\""+gs.Id+"\",\""+col.id+"\")");}
else
{if(gs.GroupByBox.postString!=""||gs.GroupByBox.moveString!="")
{var c=igtbl_getColumnById(col.id);if(gs.GroupByBox.postString)
gs._recordChange("ColumnGroup",c,gs.GroupByBox.postString);if(gs.GroupByBox.moveString)
gs._recordChange("ColumnMove",c,gs.GroupByBox.moveString);igtbl_doPostBack(igtbl_lastActiveGrid,"");}}
gs.GroupByBox.postString="";gs.GroupByBox.moveString="";gs.Element.removeAttribute("mouseDown");}