var igtbl_sUnmaskedValue="uV";
var ig_csom=new ig_initShared();

var igtbl_dataType={
	"Int16":2,
	"Int32":3,
	"Single":4,
	"Double":5,
	"DateTime": 7,
	"String": 8,
	"Boolean": 11,
	"Object": 12,
	"Decimal": 14,
	"Byte": 16,
	"SByte": 17,
	"UInt16": 18,
	"UInt32": 19,
	"Int64": 20,
	"UInt64": 21,	
	"Char": 22
};

function igtbl_getGridById(gridId) 
{	
	var grid=igtbl_gridState[gridId];	
	return grid;
}

function ig_initShared()
{
	this.getElementById = function (tagName)
	{		
		return document.all[tagName];		
	}
}

function igtbl_getElementById(tagId) 
{	
	var obj=ig_csom.getElementById(tagId);
	if(obj && obj.length && typeof(obj.tagName)=="undefined")
	{
		var i=0;
		while(i<obj.length && (obj[i].id!=tagId)) i++;
		if(i<obj.length) obj=obj[i];
		else obj=obj[0];
	}
	return obj;
}

function igtbl_WebObject(type,element,node)
{
	if(arguments.length>0)
		this.init(type,element,node);
}
igtbl_WebObject.prototype.init=function(type,element,node,viewState)
{	
	if(element)
	{
		this.Id=element.id;
		this.Element=element;
	}	
}

// Band object
function igtbl_Band(grid,node,index
	,bandsInitArray,colsInitArray
)
{
	if(arguments.length>0)
		this.init(grid,node,index
			,bandsInitArray,colsInitArray
		);
}
var igtbl_ptsBand=[	
"init",
function(grid,node,index
	,bandsInitArray,colsInitArray
)
{
	this.Grid=grid;
	this.Index=index;
	this.Columns = new Array();
	var colsArray=colsInitArray[index];	
	for(var i=0;i<colsArray.length;i++)
	{
		this.Columns[i]=new igtbl_Column(null,this,i
			,-1,colsArray[i]
		);		
	}		
	this.SortedColumns=new Array();
	this.firstActiveCell=0;
}
];
for(var i=0;i<igtbl_ptsBand.length;i+=2)
	igtbl_Band.prototype[igtbl_ptsBand[i]]=igtbl_ptsBand[i+1];

// Cell object
igtbl_Cell.base=igtbl_WebObject.prototype;
function igtbl_Cell(element,node,row,index)
{
	if(arguments.length>0)
		this.init(element,node,row,index);
}
var igtbl_ptsCell=[
"init",
function(element,node,row,index)
{
	igtbl_Cell.base.init.apply(this,["cell",element,node]);	
	this.Row=row;
	this.Band=row.Band;
	if(typeof(index)!="number")
		try{index=parseInt(index.toString(),10);}catch(e){}
	this.Column=this.Band.Columns[index];
	this.Index=index;	
},
"getValue",
function(textValue)
{	
	var value;	
	if(typeof(value)!="string" || textValue)
	{
		value=this.Element.getAttribute(igtbl_sUnmaskedValue);
		if (value) value = unescape(value);			
		if(typeof(value)=="undefined" || value==null)
		{
			var elem=this.Element;
			value=igtbl_getInnerText(elem);
			if(value==" ") value="";
		}
		else if(textValue)
		{
			if(this.MaskedValue)
				value=this.MaskedValue;
			else
				value=value.toString();
		}		
		if(typeof(value)=="string" && this.Column.AllowNull && value=="")
		{
			if(textValue)
				value="";
			else
				value=null;
		}
	}
	if(typeof(value)!="undefined")
	{
		if(!textValue)
		{
			value = this.Column.getValueFromString(value);
		}
	}
	else if(textValue)
		value="";
	if(!textValue)
		this.Value=value;
	return value;
}
];
for(var i=0;i<igtbl_ptsCell.length;i+=2)
	igtbl_Cell.prototype[igtbl_ptsCell[i]]=igtbl_ptsCell[i+1];

// Column object
function igtbl_Column(node,band,index,nodeIndex,colInitArray)
{
	if(arguments.length>0)
		this.init(node,band,index,nodeIndex
			,colInitArray
		);
}
var igtbl_ptsColumn=[
"init",
function(node,band,index,nodeIndex
	,colInitArray
)
{
	this.Band=band;
	this.Index=index;
	this.Id=(band.Grid.Id
		+"_"
		+"c_"+band.Index.toString()+"_"+index.toString());
	var defaultProps=new Array("Key","HeaderText","DataType");
	if(colInitArray)
	{
		for(var i=0;i<colInitArray.length;i++)
			this[defaultProps[i]]=colInitArray[i];
	}	
	igtbl_dispose(defaultProps);	
},
"hasCells",
function()
{
	return !this.ServerOnly && (!this.IsGroupBy || this.Band.GroupByColumnsHidden==2);
},
"compareRows",
function(row1,row2)
{
	if(igtbl_columnCompareRows)
		return igtbl_columnCompareRows.apply(this,[row1,row2]);
	return 0;
},
"compareCells",
function(cell1,cell2)
{
	if(igtbl_columnCompareCells)
		return igtbl_columnCompareCells.apply(this,[cell1,cell2]);
	return 0;
},
"getRealIndex",
function(row)
{
	if(!this.hasCells())
		return -1;
	var ri=-1;
	var colspan=1;
	var cell=null;
	if(row)
		cell=row.Element.cells[row.Band.firstActiveCell];
	var i=0;
	while(i<this.Index+1 && !this.Band.Columns[i].hasCells())
		i++;
	if(i>this.Index)
		return ri;
	ri=0;
	for(;i<this.Index;i++)
	{
		if(!this.Band.Columns[i].hasCells())
			continue;
		if(row)
		{
			if(colspan>1)
			{
				colspan--;
				continue;
			}
			var cellSplit;
			if(cell)
			{
				cellSplit=cell.id.split("_");
				if(parseInt(cellSplit[cellSplit.length-1],10)>i)
					ri--;
				else
				{
					cell=cell.nextSibling;
					if(cell)
						colspan=cell.colSpan;
				}
			}
		}
		ri++;
	}
	return ri;
},
"getValueFromString",
function(value)
{
	if(value==null || typeof(value)=="undefined")
		return null;
	value=value.toString();
	if(this.AllowNull && value=="")
		return null;
	return igtbl_valueFromString(value,this.DataType);
}
];
for(var i=0;i<igtbl_ptsColumn.length;i+=2)
	igtbl_Column.prototype[igtbl_ptsColumn[i]]=igtbl_ptsColumn[i+1];

// Client events object
function igtbl_Events(grid,eventsInitArray)
{
	if(arguments.length>0)
		this.init(grid,eventsInitArray);
}
var igtbl_ptsEvents=[
"init",
function(grid,eventsInitArray)
{
	this._defaultProps=new Array();	
	if(eventsInitArray)
		for(var i=0;i<eventsInitArray.length;i++)
			this[this._defaultProps[i]]=eventsInitArray[i];
}
];
for(var i=0;i<igtbl_ptsEvents.length;i+=2)
	igtbl_Events.prototype[igtbl_ptsEvents[i]]=igtbl_ptsEvents[i+1];


// Grid object
igtbl_Grid.base=igtbl_WebObject.prototype;
function igtbl_Grid(element,node
	,gridInitArray,bandsInitArray,colsInitArray,eventsInitArray
)
{
	if(arguments.length>0)
		this.init(element,node
		,gridInitArray,bandsInitArray,colsInitArray,eventsInitArray
		);
}

var igtbl_ptsGrid=[
"init",
function(element,node
	,gridInitArray,bandsInitArray,colsInitArray,eventsInitArray
)
{
	igtbl_Grid.base.init.apply(this,["grid",element,node]);	
	
	this.Id=this.Id.substr(2);

// Initialize properties
	var defaultProps=new Array();
	this.Bands=new Array();
	
	var bandsArray=bandsInitArray;
	var bandCount=bandsArray.length;
	for(var i=0;i<bandCount;i++) 
		this.Bands[i]=new igtbl_Band(this,null,i
			,bandsInitArray,colsInitArray
		);
	
	igtbl_dispose(defaultProps);
	igtbl_gridState[this.Id]=this;	
	this.Events=new igtbl_Events(this
		,eventsInitArray
	);
	this.Rows=new igtbl_Rows(null,this.Bands[0],null);
	
	this.SortImplementation=null;
},
"sortColumn",
function(colId)
{	
	this.addSortColumn(colId, true);
	var el=igtbl_getDocumentElement(colId);
	
	if(!el.length)
	{
		el=new Array();
		el[0]=igtbl_getElementById(colId);
	}
	for(var i=0;i<el.length;i++)
	{
		var rows=el[i].parentNode;
		
		while(rows && (rows.tagName!="TABLE" || (rows.tagName=="TABLE" && rows.id=="") ) ) rows=rows.parentNode;
		
		if(rows && rows.tBodies[0]) rows=rows.tBodies[0];
		if(!rows || !rows.Object) continue;
		rows.Object.sort();
	}	
},
"addSortColumn",
function(colId,clear)
{
	
	var band=this.Bands[igtbl_bandNoFromColId(colId)];
	var colNo=igtbl_colNoFromColId(colId);
	
	if(clear)
	{
		var scLen=band.SortedColumns.length;
		for(var i=scLen-1;i>=0;i--)
		{				
			band.SortedColumns=band.SortedColumns.slice(0,-1);
		}
	}
	if(band.Columns[colNo].SortIndicator==1)
		band.Columns[colNo].SortIndicator=2;
	else
		band.Columns[colNo].SortIndicator=1;
	
	var colEl=igtbl_getDocumentElement(colId);
	
	if (colEl)
	{
		if(!colEl.length)
			colEl=[colEl];
		for(var i=0;i<colEl.length;i++)
		{
			
			var el=colEl[i];
			if(el.firstChild && el.firstChild.tagName=="NOBR")
				el=el.firstChild;	
		}
	}
	for(var i=0;i<band.SortedColumns.length;i++)
		if(band.SortedColumns[i]==colId)
			break;
	if(i==band.SortedColumns.length)
	{
		band.SortedColumns[band.SortedColumns.length]=colId;
	}	
}
];
for(var i=0;i<igtbl_ptsGrid.length;i+=2)
	igtbl_Grid.prototype[igtbl_ptsGrid[i]]=igtbl_ptsGrid[i+1];


// Row object
igtbl_Row.base=igtbl_WebObject.prototype;
function igtbl_Row(element,node,rows,index)
{
	if(arguments.length>0)
		this.init(element,node,rows,index);
}
var igtbl_ptsRow=[
"init",
function(element,node,rows,index)
{
	igtbl_Row.base.init.apply(this,["row",element,node]);
	
	this.OwnerCollection=rows;	
	this.Band=this.OwnerCollection.Band;
	if(!this.GroupByRow)
	{
		this.cells=new Array(this.Band.Columns.length);		
		if(!this.IsAddNewRow && !this.IsFilterRow)
		{
			var tr=this.Element;
			var cellId=this.Id.split("_");
			var lastIndex=cellId.length;
			cellId[1]="rc";
			var j=0;
			if(this.Band.Grid.Bands.length>1) j++;			
			var cols=this.Band.Columns;
			var nonFixed=false,colSpan=1;
			for(var i=0;i<cols.length;i++)
			{
				if(colSpan>1)
				{
					colSpan--;
					continue;
				}				
				if(cols[i].hasCells())
				{				
					if(tr && tr.cells[j] && !tr.cells[j].id)
					{	
						cellId[lastIndex]=cols[i].Index.toString();
						tr.cells[j].id=cellId.join("_");
						colSpan=tr.cells[j].colSpan;
					}	
					j++;					
				}
			}
		}
	}	
},
"getCell",
function(index)
{
	if(index<0 || !this.cells || index>=this.cells.length)
		return null;
	if(!this.cells[index])
	{
		var cell=null;
		var col=this.Band.Columns[index];
		if(col.hasCells())		
		{
			var ri=col.getRealIndex(this);
			if(ri>=0)
			{
				cell=this.Element.cells[this.Band.firstActiveCell+ri];
				if(cell)
				{
					var column=igtbl_getColumnById(cell.id);
					if(!column || !igtbl_isColEqual(column,col))
						cell=null;
				}
			}			
		}
		var node=null;
		if(this.Node)
		{
			var cni=-1,colNo=0;
			while(colNo<col.Node.parentNode.childNodes.length)
			{
				if(!col.Node.parentNode.childNodes[colNo].getAttribute("serverOnly"))
					cni++;
				if(colNo==col.Node.getAttribute("columnNo"))
					break;
				colNo++;
			}
			if(cni>=0 && cni<col.Node.parentNode.childNodes.length)
				node=this.Node.selectSingleNode("Cs").childNodes[cni];
		}
		this.cells[index]=new igtbl_Cell(cell,node,this,index);
	}
	return this.cells[index];
},
"getCellByColumn",
function(col)
{
	return this.getCell(col.Index);
},
"dispose",
function()
{
	igtbl_cleanRow(this);
	igtbl_dispose(this);
}
];
for(var i=0;i<igtbl_ptsRow.length;i+=2)
	igtbl_Row.prototype[igtbl_ptsRow[i]]=igtbl_ptsRow[i+1];
igtbl_Row.prototype["getRowNumber"] = igtbl_Row.prototype["_getRowNumber"];

// Rows collection object
igtbl_Rows.base=igtbl_WebObject.prototype;
function igtbl_Rows(node,band,parentRow)
{
	if(arguments.length>0)
	{
		var element=null;
		if(band.Index==0 && !parentRow)
			element=band.Grid.Element.tBodies[0];
		else if(parentRow && parentRow.Element)
		{
			if(parentRow.GroupByRow)
			{
				var tb=parentRow.Element.childNodes[0].childNodes[0].tBodies[0];
				if(tb.childNodes.length>1)
					this.Element=tb.childNodes[1].childNodes[0].childNodes[0].tBodies[0];
			}
			else if(parentRow.Element.nextSibling && parentRow.Element.nextSibling.getAttribute("hiddenRow"))
				this.Element=parentRow.Element.nextSibling.childNodes[parentRow.Band.IndentationType==2?0:parentRow.Band.firstActiveCell].childNodes[0].tBodies[0];
		}
		this.init(element,node,band,parentRow);
	}
}
var igtbl_ptsRows=[
"init",
function(element,node,band,parentRow)
{
	igtbl_Rows.base.init.apply(this,["rows",element,node]);
	
	this.Grid=band.Grid;
	this.Band=band;
	this.ParentRow=parentRow;
	this.rows=new Array();
	this.length=0;	
	this.length=this.Element.childNodes.length;	
	if(this.Element)
		this.Element.Object=this;
	this.lastRowId="";
	
},
"getRow",
function(rowNo,rowElement)
{
	if(typeof(rowNo)!="number")
	{
		rowNo=parseInt(rowNo);
		if(isNaN(rowNo))
			return null;
	}
	if(rowNo<0 || !this.Element || !this.Element.childNodes)
		return null;
	if(rowNo>=this.length)
	{
		if(this.length>this.rows.length)
			this.rows[this.length-1]=null;
		return null;
	}
	if(rowNo>=this.rows.length)
		this.rows[this.length-1]=null;
	if(!this.rows[rowNo])
	{
		var row=rowElement;
		if(!row)
		{
			var cr=0;
			if(this.Grid.Bands.length==1 && !this.Grid.Bands[0].IsGrouped)
			{
				var adj=0;
				if(!igtbl_getElementById(this.Grid.Id+"_hdiv") && this.Grid.Bands[0].AddNewRowVisible==1 && this.Grid.Bands[0].AddNewRowView==1)
					adj++;
                
				if(!igtbl_getElementById(this.Grid.Id+"_hdiv") && this.Grid.Bands[0].AllowRowFiltering >= 2 && this.Grid.Bands[0].FilterUIType==1)
					adj++;
				row=this.Element.childNodes[rowNo+adj];
			}
			else
				for(var i=0;i<this.Element.childNodes.length;i++)
				{
					var r=this.Element.childNodes[i];
					if(!r.getAttribute("hiddenRow")
						&& !r.getAttribute("addNewRow")
                        && !r.getAttribute("filterRow")
					)
					{
						if(rowNo==cr)
						{
							row=this.Element.childNodes[i];
							break;
						}
						cr++;
					}
				}
		}
		if(!row)
			return null;
		this.rows[rowNo]=new igtbl_Row(row,(this.Node?this.SelectedNodes[rowNo]:null),this,rowNo);
	}
	return this.rows[rowNo];
},
"insert",
function(row,rowNo)
{	
	var row1=this.getRow(rowNo);
	if(row1)
	{
		if(this.rows.splice)
			this.rows.splice(rowNo,0,row);
		else
			this.rows=this.rows.slice(0,rowNo).concat(row,this.rows.slice(rowNo));
		this.Element.insertBefore(row.Element,row1.Element);
	}
	else
	{
		this.rows[this.rows.length]=row;
		this.Element.appendChild(row.Element);
	}
	this.length++;	
	return true;
},
"remove",
function(rowNo)
{
	var row=this.getRow(rowNo);
	if(this.rows.splice)
		this.rows.splice(rowNo,1);
	else
		this.rows=this.rows.slice(0,rowNo).concat(this.rows.slice(rowNo+1));
	this.length--;	
	return row;
},
"sort",
function(sortedCols)
{
	var issortch=false;
	if(!this.Grid._isSorting)
		this.Grid._isSorting=issortch=true;
	if(typeof(igtbl_clctnSort)!="undefined")
		igtbl_clctnSort.apply(this,[sortedCols]);
	if(issortch)
		delete this.Grid._isSorting;
}
];
for(var i=0;i<igtbl_ptsRows.length;i+=2)
	igtbl_Rows.prototype[igtbl_ptsRows[i]]=igtbl_ptsRows[i+1];


function igtbl_sortGrid()
{	
		this.Rows.sort();
}

function igtbl_columnCompareRows(row1,row2)
{
	if(!row1.GroupByRow || !row2.GroupByRow)
		return;
	var res=0;
	var v1=row1.Value;
	var v2=row2.Value;
	if(v1!=null || v2!=null)
	{
		switch(this.DataType)
		{
			case 8:
			{				
					if(v1==null && v2!=null)
						res=-1;
					else if(v1!=null && v2==null)
						res=1;
					else if(v1<v2)
						res=-1;
					else if(v1>v2)
						res=1;	
				break;
			}
			default:
				if(v1==null && v2!=null)
					res=-1;
				else if(v1!=null && v2==null)
					res=1;
				else if(v1<v2)
					res=-1;
				else if(v1>v2)
					res=1;
		}
		if(this.SortIndicator==2)
			res=-res;
	}
	return res;
}

function igtbl_columnCompareCells(cell1,cell2)
{
	var res=0;
	var v1=cell1.getValue(this.ColumnType==5 || this.WebComboId);
	var v2=cell2.getValue(this.ColumnType==5 || this.WebComboId);
	if(v1!=null || v2!=null)
	{
		if(!cell1.Column.SortCaseSensitive)
		{
			if(typeof(v1)=="string")
				v1=v1.toLowerCase();
			if(typeof(v2)=="string")
				v2=v2.toLowerCase();
		}
		switch(this.DataType)
		{
			case 8:
			{
				
					if(v1==null && v2!=null)
						res=-1;
					else if(v1!=null && v2==null)
						res=1;
					else if(v1<v2)
						res=-1;
					else if(v1>v2)
						res=1;	
				break;
			}
			default:
				if(v1==null && v2!=null)
					res=-1;
				else if(v1!=null && v2==null)
					res=1;
				else if(v1<v2)
					res=-1;
				else if(v1>v2)
					res=1;
		}
		if(this.SortIndicator==2)
			res=-res;
	}
	return res;
}

function igtbl_compare(av1, av2, caseSens, sort)
{
	return igtbl_compareRows(av1,av2, sort);
}

function igtbl_compareRows(av1,av2, columns)
{
	var res=0;
	for(var i=0;i<columns.length && res==0;i++)
	{
		var v1=av1[i+1];
		var v2=av2[i+1];
		if(v1!=null || v2!=null)
		{
			var t1=typeof(v1);
			var t2=typeof(v2);
			if(!columns[i].SortCaseSensitive)
			{
				if(t1=="string")
					v1=v1.toLowerCase();
				if(t2=="string")
					v2=v2.toLowerCase();
			}
			if(t1=="string" && t2=="string")
			{
				
				if(v1==null && v2!=null)
					res=-1;
				else if(v1!=null && v2==null)
					res=1;
				else
				{
					res=v1.localeCompare(v2);		
				}
			}
			else
			{
				if(v1==null && v2!=null)
					res=-1;
				else if(v1!=null && v2==null)
					res=1;
				else if(v1<v2)
					res=-1;
				else if(v1>v2)
					res=1;
			}
			if(columns[i].SortIndicator && columns[i].SortIndicator==2 ||
				typeof(columns[i]) == "number" && columns[i] == 2)
			{
				res=-res;
			}
		}
	}
	return res;
}

function igtbl_quickSort1(cln,array,colInfo,left,right)
{
	var i,j,comp,temp;
	i=left;
	j=right;
	comp=array[Math.floor((left+right)/2)];
	do
	{
		while(igtbl_compareRows(array[i],comp,colInfo)<0 && i<right)
			i++;

		while(igtbl_compareRows(array[j],comp,colInfo)>0 && j>left)
			j--;
		if(i<=j)
		{
			if(i<j)
			{
				temp=array[i];
				array[i]=array[j];
				array[j]=temp;
			}
			i++;
			j--;
		}
	}
	while(i<=j);
	if(left<j)
		igtbl_quickSort1(cln,array,colInfo,left,j);
	if(i<right)
		igtbl_quickSort1(cln,array,colInfo,i,right);
}

function igtbl_clctnSort(sortedCols)
{	
	if(!sortedCols)
		sortedCols=this.Band.SortedColumns;
	var sortArray=new Array(this.length);
	var colInfo=new Array();
	var chkBoxArray=new Array();
	for(var i=0;i<this.length;i++)
		sortArray[i]=[i];
	for(var j=0;j<this.Band.SortedColumns.length;j++)
	{
		var column=igtbl_getColumnById(this.Band.SortedColumns[j]);
		if(column.IsGroupBy)
		{
			if(this.length>0)
			{
				var grCol=igtbl_getColumnById(this.getRow(0).GroupColId);
				if(grCol==column)
				{
					for(var i=0;i<this.length;i++)
						sortArray[i][j+1]=this.getRow(i).Value;
					colInfo[j]=column;
				}
			}
		}
		else
		{		    
		    if(column.ColumnType==5)
		    {
		        for(var i=0;i<this.length;i++)
			    {
				    var srtCol = this.getRow(i).getCellByColumn(column);
				    if (srtCol)	sortArray[i][j+1]=srtCol.getValue(true);
			    }
		    }
		    else
		    {
   			    for(var i=0;i<this.length;i++)
			    {
				    var srtCol = this.getRow(i).getCellByColumn(column);
				    if (srtCol)	sortArray[i][j+1]=srtCol.getValue();
			    }
		    }
			
		    colInfo[j] = column;
		}
	}
	for(i=0;i<this.Band.Columns.length;i++)
	{
		var col=this.Band.Columns[i];
		if(col.hasCells() && col.ColumnType==3)
			chkBoxArray[chkBoxArray.length]=i;
	}
	if(sortedCols.length>0 && this.length>0)
	{	
		igtbl_quickSort1(this,sortArray,colInfo,0,this.length-1);		
	}
	var cntnSort=false;
	for(var i=this.Band.Index+1;i<this.Grid.Bands.length && !cntnSort;i++)
		if(this.Grid.Bands[i].SortedColumns.length>0)
			cntnSort=true;
		
	for(var i=0;i<this.length;i++)
	{
		if(sortArray[i][0]!=i)
		{
			var san=sortArray[i][0];
			this.insert(this.remove(san),i);
			igtbl_dontHandleChkBoxChange=true;
			for(var j=0;j<chkBoxArray.length;j++)
			{
				var cell=this.getRow(i).getCell(chkBoxArray[j]);
				if(cell && cell.Element.getAttribute("chkBoxState"))
				{
					var chkBoxEl=cell.getElement().firstChild;
					if(chkBoxEl.tagName=="NOBR")
						chkBoxEl=chkBoxEl.firstChild;
					chkBoxEl.checked=(cell.Element.getAttribute("chkBoxState")=="true");
				}
			}
			igtbl_dontHandleChkBoxChange=false;
			sortArray[i][0]=i;
			for(j=i+1;j<sortArray.length;j++)
				if(sortArray[j][0]<san)
					sortArray[j][0]++;
		}
		var curRow=this.getRow(i);
		var className="";
		if(curRow.Expandable)
		{
			var col=sortedCols.length>0?igtbl_getColumnById(sortedCols[0]):null;
			if(col && col.IsGroupBy)
			{
				if(curRow.Rows)
					curRow.Rows.sort(sortedCols.slice(1));
			}
			else if(cntnSort && curRow.Rows)
				curRow.Rows.sort(this.Grid.Bands[this.Band.Index+1].SortedColumns);
		}
	}	
	if(this.Node)
		this.reIndex(0);
	igtbl_dispose(sortArray);
	delete sortArray;
	igtbl_dispose(chkBoxArray);
	delete chkBoxArray;
}

var igtbl_gridState=new Object();

function igtbl_getBandById(tagId) 
{
	if(!tagId)
		return null;
	var parts = tagId.split("_");
	var gridId = parts[0];
	var el=igtbl_getElementById(tagId);
	var bandIndex=igtbl_getBandNo(el);
	var objTypeId = parts[1];

	if(objTypeId=="c" && el && el.tagName=="TH")
	{
		bandIndex=parts[2];
	}
	if(!igtbl_getGridById(gridId))
		return null;
	var grid = igtbl_getGridById(gridId);
	return grid.Bands[bandIndex];
}

function igtbl_getColumnById(tagId) 
{
	if(!tagId)
		return null;
	var parts = tagId.split("_");
	var bandIndex = parts.length - 2;
	var gridId = parts[0];
	var objTypeId = parts[1];
	var el=igtbl_getElementById(tagId);
	
	if(objTypeId=="anc" && el && el.tagName=="TD")
	{
		bandIndex=igtbl_getBandById(tagId).Index;
	}
	else
    if(objTypeId=="flc" && el.tagName=="TD") 
    {
        bandIndex=igtbl_getBandById(tagId).Index;
    }
    else
	if(objTypeId=="rc" && el && el.tagName=="TD")
	{
		bandIndex=igtbl_getBandById(tagId).Index;
	}
	else if(objTypeId=="cf")
	{
		if(el && el.tagName!="TH")
			return null;
		bandIndex=parts[2];
	}
	else if(objTypeId=="cg")
	{
		if(el && el.tagName!=
			"TH"
		)
			return null;
		bandIndex=parts[2];
	}
	else if (objTypeId=="c")
	{
		if (el && el.tagName!="TH")
			return;
		bandIndex=parts[2];			
	}
	else
		return null;

	if(!igtbl_getGridById(gridId))
		return null;
	var grid = igtbl_getGridById(gridId);
	var band = grid.Bands[bandIndex];
	var colIndex = parts[parts.length - 1];
	return band.Columns[colIndex];
}

function igtbl_getBandNo(cell)
{
	if(!cell)
		return -1;
	var tbl=cell;
	while(tbl && !tbl.getAttribute("bandNo"))
		tbl=tbl.parentNode;
	if(tbl)
		return parseInt(tbl.getAttribute("bandNo"));
	return -1;
}

function igtbl_bandNoFromColId(colId)
{
	var s=colId.split("_");
	if(s.length<3)
		return null;
	return parseInt(s[s.length-2]);
}

function igtbl_colNoFromColId(colId)
{
	var s=colId.split("_");
	if(s.length<3)
		return null;
	return parseInt(s[s.length-1]);
}

function igtbl_colNoFromId(id)
{	
	var s=id.split("_");
	if(s.length==0)
		return null;
	return parseInt(s[s.length-1]);
}

function igtbl_initGrid(gridId
	,gridInitArray,bandsInitArray,colsInitArray,eventsInitArray) 
{	
	var gridElement=igtbl_getElementById("G_"+gridId);
	var xml;
	return new igtbl_Grid(gridElement,xml
		,gridInitArray,bandsInitArray,colsInitArray,eventsInitArray
		);
}

function igtbl_dispose(obj)
{	
	for(var item in obj)
	{
		if(typeof(obj[item])!="undefined" && obj[item]!=null && !obj[item].tagName && !obj[item].disposing && typeof(obj[item])!="string")
		{
			try {				
				obj[item].disposing=true;
				igtbl_dispose(obj[item]);				
			} catch(exc1) {;}
		}
		try {
			delete obj[item];
		} catch(exc2) {
			return;
		}
	}
}

function igtbl_headerClickUp(evnt,gn) 
{	
	var se=evnt.srcElement;
	var gs=igtbl_getGridById(gn);	
	gs.sortColumn(se.id);	
	return true;
}

function igtbl_isColEqual(col1,col2)
{
	if(col1==null && col2==null)
		return true;
	if(col1==null || col2==null)
		return false;
	if(col1.Band.Index==col2.Band.Index && col1.Key==col1.Key && col1.Index==col2.Index)
		return true;
	return false;
}

function igtbl_getInnerText(elem)
{
	if(!elem)return "";
	
	if (elem.nodeName=="#text"){return elem.nodeValue;}		
	var txt="",nn=elem.childNodes;
	
	if(elem.nodeName=="#text")txt=elem.nodeValue;
	else if(elem.nodeName=="BR")txt="\r\n";
	else if(nn)for(var i=0;i<nn.length;i++)txt+=igtbl_getInnerText(nn[i]);
	var sp=String.fromCharCode(160);
	while(txt.indexOf(sp)>=0)txt=txt.replace(sp," ");
	return txt;
}

function igtbl_getDocumentElement(elemID)
{	
	{
		var elem=document.getElementById(elemID);
		if(elem)
		{
			var elems=document.getElementsByTagName(elem.tagName);
			var els=[];
			for(var i=0;i<elems.length;i++)
			{
				if(elems[i].id==elemID)
					els[els.length]=elems[i];
			}
			return (els && els.length == 1) ? els[0] : els;
		}
		return null;
	}
}

function igtbl_valueFromString(value,dataType)
{
	if(typeof(value)=="undefined" || value==null)
		return value;
	switch(dataType)
	{
		case igtbl_dataType.Int16:
		case igtbl_dataType.Int32:
		case igtbl_dataType.Byte:
		case igtbl_dataType.SByte: 
		case igtbl_dataType.UInt16:
		case igtbl_dataType.UInt32:
		case igtbl_dataType.Int64:
		case igtbl_dataType.UInt64:
			if(typeof(value)=="number")
				return value;
			if(typeof(value)=="boolean")
				return (value?1:0);
			if(value.toString().toLowerCase()=="true")
				return 1;
			value=parseInt(value.toString(),10);
			if(value.toString()=="NaN")
				value=0;
			break;
		case igtbl_dataType.Single:
		case igtbl_dataType.Double:
		case igtbl_dataType.Decimal:
			if(typeof(value)=="float")
				return value;
			value=parseFloat(value.toString());
			if(value.toString()=="NaN")
				value=0.0;
			break;
		case igtbl_dataType.Boolean:
			if(!value || value.toString()=="0" || value.toString().toLowerCase()=="false")
				value=false;
			else
				value=true;
			break;
		case igtbl_dataType.DateTime:
			
			var d;
			if(typeof(value)=="string")
			{
				var dtV=value.split(".");
				var ms=0,lastPart=dtV.length>1?dtV[1].substr(dtV[1].length-3).toUpperCase():"";
				if(dtV.length>1 && (lastPart==" AM" || lastPart==" PM"))
				{
					ms=igtbl_parseInt(dtV[1]);
					dtV[0]+=lastPart;
				}
				else
					dtV[0]=value;
				d=new Date(dtV[0]);
				if(!isNaN(d))
					d.setMilliseconds(ms);
			}
			else
				d=new Date(value);
			if(d.toString()!="NaN" && d.toString()!="Invalid Date")
				value=d;
			else
				value=igtbl_string.trim(value.toString());
			delete d;
			break;
		case igtbl_dataType.String: 
			break;
		default:
			value=igtbl_string.trim(value.toString());
	}
	return value;
}

// Set a namespace for our code
window.iPhone = window.iPhone || {};

(function() {

	// Local shorthand variable
	var $i = this;

	// Shared variables
	$i.vars = {};

	// Shared utilities
	$i.utils = {

		// Adds class name to element
		addClass : function(element, elClass) {
			var curr = element.className;
			if (!new RegExp(("(^|\\s)" + elClass + "(\\s|$)"), "i").test(curr)) {
				element.className = curr + ((curr.length > 0) ? " " : "") + elClass;
			}
			return element;
		},

		// Removes class name from element
		removeClass : function(element, elClass) {
			if (elClass) {
				element.className = element.className.replace(elClass, "");
			} else {
				element.className = "";
				element.removeAttribute("class");
			}
			return element;
		},
		
		// updateOrientation checks the current orientation, sets the body's class attribute to portrait,
		updateOrientation : function() {
			var orientation = window.orientation;
			
			switch (orientation) {
				
				// If we're horizontal
				case 90:
				case -90:
				
				// Set orient to landscape
				document.body.setAttribute("orient", "landscape");
				break;	
				
				// If we're vertical
				default:
				
				// Set orient to portrait
				document.body.setAttribute("orient", "portrait");
				break;
			}
			
		},
		
		gettranslateY : function(element) {
			var transform = element.style.webkitTransform;
			
			if (transform && transform !== "") {
				var translateY = parseFloat((/translateY\((\-?.*)px\)/).exec(transform)[1]);
			}
			
			return translateY;
		},
		
		settranslateY : function(element, value) {
			element.style.webkitTransform = "translateY(" + value + "px)";
		},
			
		settranslateX : function(element, value) {
			element.style.webkitTransform = "translateX(" + value + "px)";
		},
		
		scrollToY : function(y) {			
			var header = document.querySelector("#header");	
			//var caption = document.querySelector("#caption");	
			
			// Prep for animation
			header.style.webkitTransition = "-webkit-transform 500ms cubic-bezier(0.1, 0.25, 0.1, 1.0)";
			//caption.style.webkitTransition = "-webkit-transform 500ms cubic-bezier(0.1, 0.25, 0.1, 1.0)";
			
			if (y > 20)
			{	
				$i.utils.settranslateY(header, y - 20);
				//$i.utils.settranslateY(caption, y - 20);
			}
			else
			{				
				$i.utils.settranslateY(header, y);
				//$i.utils.settranslateY(caption, y);
			}
			
			// Clean up after ourselves
			 // setTimeout(function() {
				 // header.style.webkitTransition = "none";
				 // caption.style.webkitTransition = "none";
			 // }, totalTime);
		},
		
		scrollToX : function(x) {			
			var footer = document.querySelector("#footer");	
			//var caption = document.querySelector("#caption");	
			// Prep for animation
			footer.style.webkitTransition = "-webkit-transform 500ms cubic-bezier(0.1, 0.25, 0.1, 1.0)";
			//caption.style.webkitTransition = "-webkit-transform 500ms cubic-bezier(0.1, 0.25, 0.1, 1.0)";
			
			// Animate to specified  point
			$i.utils.settranslateX(footer, x);
			
			
			 //Clean up after ourselves
			//  setTimeout(function() {
			//	 $i.utils.settranslateX(caption, x);
			//}, 501);
		}
	};
	
	// Initialize
	$i.init = function() {
		
		// Sniff for orientation property
		if (typeof window.orientation !== "undefined") {
			
			// Fire events in onload namespace
			for (var key in $i.onload) {
				$i.onload[key]();
			}
			
			// Remove scroll class on orientation change
			window.addEventListener("orientationchange", function() {
				$i.utils.removeClass(document.body, "scrolled");
			}, false);
			
			// Point to the updateOrientation function when iPhone switches between portrait and landscape modes.
			$i.utils.updateOrientation();
			window.addEventListener("orientationchange", $i.utils.updateOrientation, false);
		}
	};
	
	$i.onload = {
		// Enable area for scrolling
		enableScrollOnContent : function() {
			// Grab elements
			
			var content = document.querySelector("#content");
			var container = document.querySelector("#container");
			var header = document.querySelector("#header");
			var footer = document.querySelector("#footer");
			
			
			// Can't prevent user from tapping status bar
			// So instead, readjust fixed positions
			window.addEventListener("scroll", function() {				
				$i.utils.scrollToY(window.pageYOffset);
				$i.utils.scrollToX(window.pageXOffset);
				$i.utils.addClass(document.body, "scrolled");
				
			}, false);
			
			// Scroll on finger drag
			content.addEventListener("touchmove", function(e) {		
				var start = window.pageXOffset;
				var end = $i.utils.gettranslateX(footer);
				//$i.utils.scrollToY(window.pageYOffset);
				//$i.utils.scrollToX(window.pageXOffset);								
			}, false);
			
			// Ease movement when finger is removed
			window.addEventListener("touchend", function(e) {	
					$i.utils.scrollToY(window.pageYOffset);
					$i.utils.scrollToX(window.pageXOffset);
			}, false);	
			
		}
	};	
	// Fire on load
	window.addEventListener("load", $i.init, false);	
}).call(window.iPhone); // Initialize