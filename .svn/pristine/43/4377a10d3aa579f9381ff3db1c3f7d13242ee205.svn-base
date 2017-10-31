var ID2OBJECT = new Array();
function IGBrowser() 
{
	this.ScriptVersion		="<SUCCESSFUL_COMPONENT_VERSION>"; 
	this.AgentName			=navigator.userAgent.toLowerCase();
	this.IsFirefox          =this.AgentName.indexOf("firefox") != -1
	this.MajorVersionNumber =parseInt(navigator.appVersion);
	this.IsDom				=(document.getElementById)?true:false;
	this.IsSafari			=this.AgentName.indexOf("safari")!=-1;
	this.IsNetscape			=(document.layers?true:false);
	this.IsNetscape4Plus	=(this.IsNetscape && this.MajorVersionNumber >=4)?true:false;
	this.IsNetscape6		=!this.IsSafari&&(this.IsDom&&navigator.appName=="Netscape");
	this.IsOpera			=this.AgentName.indexOf('opera')!=-1;
	this.IsMac				=(this.AgentName.indexOf("mac")!=-1);
	this.IsIE				=(document.all?true:false);
	this.IsIE4				=(document.all&&!this.IsDom)?true:false;
	this.IsIE4Plus			=(this.IsIE && this.MajorVersionNumber >= 4)?true:false;
	this.IsIE5				=(document.all&&this.IsDom)?true:false;
	this.IsWin				=((this.AgentName.indexOf("win")!=-1) || (this.AgentName.indexOf("16bit")!=-1));
	this.ID					="IGB";	
	this.Dispose = function(obj)
	{
		if (this.IsIE && this.IsWin)	
			for(var item in obj)
			{
				if(typeof(obj[item])!="undefined" && obj[item]!=null && !obj[item].tagName && !obj[item].disposing && typeof(obj[item])!="string")
				{
					try 
					{
						obj[item].disposing=true;
						ig_dispose(obj[item]);
					} 
					catch(e1) {;}
			}
			try{delete obj[item];}catch(e2){;}
		}
	}
	this.GetObject = function(id, doc) 
	{
		var i,x;  
		if (!doc) doc=document; 
		if (!(x=doc[id])&&doc.all) x=doc.all[id]; 
		if (!x && document.getElementById) x=document.getElementById(id); 
		return x;
	}	
	this.ShowObject = function(obj,disp) 
	{ 
		(this.IsNetscape)? '':(!disp)? obj.style.display="inline":obj.style.display=disp;
		(this.IsNetscape)? obj.visibility='show':obj.style.visibility='visible';  
	}
	this.HideObject = function(obj,disp) 
	{ 
		(this.IsNetscape)? '':(arguments.length!=2)? obj.style.display="none":obj.style.display=disp;
		(this.IsNetscape)? obj.visibility='hide':obj.style.visibility='hidden';  
	}
	this.AddEventListener = function (o,e,f,c)
	{}	
	this.AddEventListener = function(obj,eventName,callbackFunction,flag)
	{}	
	this.WriteHTML = function(obj,html) 
	{		
		if (this.IsNetscape)
		{
			var doc=obj.document;
			doc.write(html);
			doc.close();
			return false;
		}
		if (obj.innerHTML) obj.innerHTML=html; 
	}
	this.SetXClientOverflowSafe=function(obj,x)
	{
		var objW=IGB.GetWidth(obj);
		var objR=objW+x;
		var clientW=IGB.GetClientWidth();
		if((clientW-objR)>(x-objW-5))
			this.SetX(obj,x);
		else
			this.SetXScrollContainerSafe(obj,x-objW-5);
	}

	this.GetWidth=function(obj)
		{
			var w=0;
			if(this.IsNetscape)
			{
				w=(obj.width)?obj.width:obj.clip.width;
				return w;
			}
			w=(this.IsOpera)?obj.style.pixelWidth:obj.offsetWidth;
			return w;
	}

	this.GetClientWidth=function()
		{
			var w=(this.IsIE)?window.document.body.clientWidth:window.innerWidth;
			return w;
		}
		
	this.SetXScrollContainerSafe=function(obj,x)
		{
			var hSC=this.GetHScrolledContainer(obj);
			if(hSC!=null)
				this.SetXScrollContainerAdjusted(obj,x,hSC);
			else
				this.SetX(obj,x);
	}

	this.GetHScrolledContainer=function(obj)
	{
		if(obj.scrollLeft>0&&obj.tagName!='BODY')
			return obj;
		else 
			if(obj.offsetParent!=null)
				return this.GetHScrolledContainer(obj.offsetParent);
			else
				return null;
	}
	
	this.SetXScrollContainerAdjusted=function(obj,x,container)
	{
		this.SetX(obj,x+container.scrollLeft);
	}
	
	this.SetX = function(obj, x)
	{ 
		obj.style.pixelLeft=x;
	}	
	this.SetY = function(obj, y) 
	{ 
		obj.style.pixelTop=y;
	}
	ID2OBJECT["IGB"] = this;
	this.Listener	  = new Array();
	this.AddListener  = function(type, function_ref) 
	{
		this.Listener[type] = function_ref;
	}
	this.CurrentX = 0;
	this.CurrentY = 0;
	this.GlobalHandleMouseMove = function(evt)
	{
		if (this.IsNetscape4Plus)
		{
			this.CurrentX=evt.pageX;
			this.CurrentY=evt.pageY;
		}
		else if (this.IsNetscape6)
		{
			this.CurrentX=evt.clientX;
			this.CurrentY=evt.clientY;
		}
		else if (this.IsIE5)
		{
			this.CurrentX=event.clientX;
			this.CurrentY=event.clientY;
		}
		IGProcessEventsObjects("onmousemove", this);
		return false;
	}
	this.FunctionName = function(f)
	{
		if (f==null)
		{
			return "anonymous";
		}
		var s=f.toString().match(/function\s*(\w*)/)[1];
		if((s==null)|| (s.length==0)) return "anonymous";
		return s;
	}
	this.DecodeArguments=function(inputString) 
	{
		var splitArray = inputString.split('&');
		for (i = 0; i < splitArray.length; i++)
		{
		    if (decodeURI)
		    {
		        splitArray[i] = decodeURI(splitArray[i]);
		    }
		    else
		    {
                splitArray[i] = unescape(splitArray[i]);
		    }
		}
		return splitArray;
	}
}
function IGRectangle(x, y, width, height)
{
	this.X=x;
	this.Y=y;
	this.Width=width;
	this.Height=height;
	this.Inside = function(x,y)
	{
		return (x >=this.X && y >= this.Y && x <=(this.X+this.Width) && y <=(this.Y+this.Height));
	}
}
function IGPoint(x, y) 
{
	this.X = x;
	this.Y = y;
}
var IGB = new IGBrowser();
function IGProcessEventsObjects(type, sender_object)
{}
function IGProcessEvents(type, sender_element)
{}
function IGBubbleEvent(type, sender_element, sender_object)
{}
var RepeatingDelegate=null;
var DelegateParameter=null;
var DelegateeObject=null;
var TimerId= null;
function GetDelay(nextTimeOut)
{}
function RepeatingHandler(nextTimeOut) 
{}
function Repeating(trueToStartfalseToEnd, delegate, parameters, ThisObject) 
{}
function Fader()
{}
function Bounce(evt, id, func_name, paramArray) 
{
	var this_ref = ID2OBJECT[id];  
	var fn = func_name;
	if (this_ref)
	{
		if (fn)
		{
			fn = func_name;
		}
		else
		{
			fn = "on"+evt.type;
		}

		eval("this_ref."+fn+"(evt, id, paramArray)");
	}
}
function IGWindowViewer(id, imageId, vuid, srcBounds, destBounds) 
{
	this.SourceBounds = srcBounds;
	this.DestBounds	= destBounds;
	this.ImageId	= imageId;
	this.VUId		= vuid;
	ID2OBJECT[this.ID]= this;
	this.Parent		  = null;
	this.HTML		  = "";
	this.Listener	  = new Array();
	this.AddListener  = function(type, function_ref) 
	{}
	this.MoveBy=function(x,y)
	{
		this.SourceBounds.X += x;
		this.SourceBounds.Y += y;
		this.Render();
	}
	this.Render=function()
	{}
}
function IGCrossHair(id, toggleOnClick)
{
	this.ID					= id;
	this.ToggleOnClick		= toggleOnClick;
	this.Visible			= false;	
	this.SpanImageObject	= null;
	this.HairHorizontal		= null;
	this.HairVertical		= null;
	ID2OBJECT[this.ID]= this;
	this.Parent		  = null;
	this.HTML		  = "";
	this.Listener	  = new Array();
	this.AddListener  = function(type, function_ref) 
	{}
	this.Render=function(b)
	{}
}
function IGScrollBar(id, width, height, scrollerLength, url, orientation, uniqueId) 
{
	this.Orientation = orientation==null?'horizontal':orientation;
	this.ID			 = id;
	this.UniqueID    = uniqueId;
	this.ImageURL	 = url;
	this.Width		 = width;
	this.Height		 = height;
	this.ScrollerLen = scrollerLength;	
	this.UseImageFromId = this.ID
	this.Location	 = new IGPoint(0,0);
	this.Minimum	 = 0;
	this.Maximum	 = 100;
	this.Value		 = 0;
	this.SmallChange  = 5;
	this.LargeChange  = 15;
	ID2OBJECT[this.UniqueID] = this;
	this.Parent		  = null;
	this.HTML		  = "";
	this.Listener	  = new Array();
	this.Where        = null;	
	this.AddListener  = function(type, function_ref) 
	{
		this.Listener[type] = function_ref; // save listener into array.
	}
    this.SetLocation = function()
    {}    
	this.Render = function(where) 
	{}
	this.GetIGShared = function()
	{	    
	    if (typeof(ig_shared) != "undefined" && ig_shared)
	    {
	        return ig_shared;
	    }
	    else if (top.ig_shared)
	    {
	        return top.ig_shared;
	    }
	    return null;
	}
    this.InitSize = function(id)
    {        
    }
	this.SetValue = function(val)
	{
	}
}
function ScrollItH(id, scroll) 
{}
function ScrollItV(id, scroll) 
{}
var EngagedObject=null;
var OldMouseDown;
var OldMouseMove;
var OldMouseUp;
var MouseDownX, MouseDownY;
function EngageObject(which) 
{
	EngagedObject = ID2OBJECT[which]; 
}
function ReleaseObject() 
{
	EngagedObject = null;
}
function NewMouseDown(evt) 
{}
function NewMouseMove(evt) 
{}
function NewMouseUp(evt) 
{}
function ScrollbarMouseWheel(id)
{}
function InitilizeScrollbar() 
{} 
function IGUltraChart(id, imageUrl, uniqueId)
{
	this.ID			= id;
	this.ImageUrl	= imageUrl;
	this.UniqueID   = uniqueId;		
	this.EnableTooltipFading = false;
	this.EventData	= null;
	this.TooltipData= null;
	this.RowCount = 0;
	this.ColumnCount = 0;
	this.TooltipDisplay = 0;
	this.EnableCrossHair = false;
	this.EnableServerEvent = false;
	this.Section508Compliant = false;	
	ID2OBJECT[this.ID]= this;
	this.Parent		  = null;
	this.HTML		  = "";
	this.Listener	  = new Array();
	this.DEBUG		  = false;
	this.AddListener  = function(type, function_ref) 
	{
		this.Listener[type] = function_ref;
	}
	this.payloadHandler = null;
	this.SB1 = null;
	this.SB2 = null;
	this.igWindowVuer = null;
	this.iGCrossHair = null;
	this.XhairBounds = null;
	this.TooltipFader = null;
	this.TooltipVisible = false;
	this.CreateComponents=function(vals)
	{
		var indexOfColon = this.UniqueID.lastIndexOf(":");
		var idPrefix;
		if (indexOfColon != -1)
		{
			idPrefix = this.UniqueID.substring(indexOfColon + 1, this.UniqueID.length);
		}
		else
		{
			idPrefix = this.UniqueID;
		}
        if (idPrefix.indexOf("$") != -1)
        {
            var tokens = idPrefix.split("$");
            if (tokens.length > 0)
            {
                idPrefix = tokens[tokens.length - 1];
            }
        }
		this.SB1				= new IGScrollBar(idPrefix + "_SB1", vals[2], vals[3],  30, this.ImageUrl, "", this.UniqueID + "_SB1");
		this.SB2				= new IGScrollBar(idPrefix + "_SB2", vals[9], vals[10], 30, this.ImageUrl, "vertical", this.UniqueID + "_SB2");
		this.igWindowVuer		= new IGWindowViewer(this.ID + "_igWindowVuer", this.ID + "_ScrollImage", this.ID + "_igWindowVuer");
		this.iGCrossHair		= new IGCrossHair(this.ID + "_iGCrossHair");
		this.TooltipFader		= new Fader();
		this.SB1.Parent = this;
		this.SB2.Parent = this;
		this.igWindowVuer.Parent = this;
		this.iGCrossHair.Parent = this;
		var baseImage = IGB.GetObject(this.ID + "_BaseImage");
		var scrollImage = IGB.GetObject(this.ID + "_ScrollImage");
		var table = IGB.GetObject(this.ID + "_table");
		this.iGCrossHair.SpanImageObject	= baseImage;
		this.iGCrossHair.HairHorizontal		= IGB.GetObject(this.ID+"_HairHorizontal");
		this.iGCrossHair.HairVertical		= IGB.GetObject(this.ID+"_HairVertical");
		this.iGCrossHair.Visible			= this.EnableCrossHair;		
	}
	this.Render=function(vals) 
	{}
	this.SB2_Scroll=function(evt, sender_element, sender_object) 
	{}
	this.onmousewheel=function(evt, id)
	{}
	this.onmousemove=function(evt, id)
	{}	
	this.cloneTooltip=function()
	{
	    var tooltip_ref = IGB.GetObject(this.ID+"_IGTooltip");
        var tooltip_ref_body = tooltip_ref.cloneNode(true);
        tooltip_ref_body.id = tooltip_ref_body.id.replace("_IGTooltip", "_IGTooltipBody");
        tooltip_ref_body.style.zIndex = 60000;
        document.body.appendChild(tooltip_ref_body);
        return tooltip_ref_body;
	}
	this.ShowTooltip=function(evt, id, args)
	{
		var tooltip_ref_body = IGB.GetObject(this.ID+"_IGTooltipBody");
		if (!tooltip_ref_body)
	    {   
            tooltip_ref_body = this.cloneTooltip();
	    }
		var x = 0, y = 0;
		if (IGB.IsNetscape6) 
		{	
		    x = evt.pageX;
		    y = evt.pageY;
		} 
		else 
		{
			x = window.event.clientX + document.body.scrollLeft;
			y = window.event.clientY + document.body.scrollTop;

			if (IGB.IsMac && IGB.IsIE)
			{
				x -= 10;
				y -= 15;
			}
		}		
		x += 10;
		y -= 40;		
		var text = "";
		var data_id = args[4]+"_"+args[1]+"_"+args[2];	
		if (this.TooltipData!=null)
		{
			text = this.TooltipData[data_id];
		}		
		IGB.WriteHTML(tooltip_ref_body, "<nobr>"+text+"</nobr>");
		this.TooltipVisible = true;
		IGB.SetXClientOverflowSafe(tooltip_ref_body, x);
		IGB.SetY(tooltip_ref_body, y);	
		IGB.ShowObject(tooltip_ref_body);		
		var function_ref = this.Listener["showtooltip"];
		if (function_ref != null) 
		{
			function_ref.apply(this, [text, tooltip_ref_body]);
		}
	}
	this.HideTooltip=function(evt, id, args)
	{	
		var tooltip_ref_body = IGB.GetObject(this.ID+"_IGTooltipBody");		
		var text = "";
		var data_id = args[4]+"_"+args[1]+"_"+args[2];
		if (this.TooltipData!=null)
		{
			text = this.TooltipData[data_id];
		}		
		tooltip_ref_body.style.visibility = 'hidden';		
		this.TooltipVisible = false;
		var function_ref = this.Listener["hidetooltip"];
		if (function_ref != null) 
		{
			function_ref.apply(this, [text, tooltip_ref_body]);
		}
	}
this.onallevent=function(evt, id, args)
	{
		var function_ref = this.Listener[evt.type];
		if (function_ref != null) 
		{
			var v = IGB.DecodeArguments( this.EventData[args[4]+"_"+args[1]+"_"+args[2]] );
			function_ref.apply(this, [this, v[0], v[1], v[2], v[3], v[4], evt.type, args[4] ] );
		}
		if (evt.type == "mouseover" && this.TooltipDisplay == 1)
		{			
			this.ShowTooltip(evt, id, args);			
		}
		else if (evt.type == "click" && this.TooltipDisplay == 2)
		{
			this.ShowTooltip(evt, id, args);
		}
		else if (evt.type == "mouseout" )
		{
			this.HideTooltip(evt, id, args);
		}
	}
}
