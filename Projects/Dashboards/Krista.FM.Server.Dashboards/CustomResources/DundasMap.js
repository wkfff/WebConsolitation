var DundasMap=
{ 
MapManager:function()
	{ 
		this.initialize=function()
		{ 
			//debugger;
			this.isIE =window.navigator.userAgent.indexOf("MSIE") >-1; 
			this.mapDiv=document.getElementById(this.mapControlId); 
			this.mapDiv.gridSectionManager=this; 
			this.viewport=document.getElementById(this.mapControlId+"Viewport"); 
			this.contentFiller=document.getElementById(this.mapControlId+"ContentFiller"); 
			this.zoomPanel=document.getElementById(this.mapControlId+"ZoomPanelDiv"); 
			this.zoomPanelThumb=document.getElementById(this.mapControlId+"ZoomPanelThumb"); 
			this.navigationPanel=document.getElementById(this.mapControlId+"NavigationPanelDiv"); 
			this.distanceScalePanel=document.getElementById(this.mapControlId+"DistanceScalePanel"); 
			this.mapImage=document.getElementById(this.mapControlId+"Image"); 
			if (this.viewport !=null)
			{ 
				this.contentFiller.style.width=this.gridSectionsXCount*this.gridSectionSizeWidth+"px"; 
				this.contentFiller.style.height=this.gridSectionsYCount*this.gridSectionSizeHeight+"px"; 
				this.contentFiller.style.left=this.contentOffsetX+"px"; 
				this.contentFiller.style.top=this.contentOffsetY+"px"; 
				if (this.contentCachingEnabled)
				{ 
					this.gridSectionsOffsetX=0; 
					this.gridSectionsOffsetY=0; 
				} 
				else
				{ 
					this.gridSectionsOffsetX=Math.round(parseInt(this.contentFiller.style.left) % this.gridSectionSizeWidth); 
					this.gridSectionsOffsetY=Math.round(parseInt(this.contentFiller.style.top) % this.gridSectionSizeHeight); 
				} 
				this.viewportSizeWidth=parseInt(this.viewport.style.width); 
				this.viewportSizeHeight=parseInt(this.viewport.style.height); 
				this.requests=new DundasMap.LinkedList(this); 
				this.downloads=new DundasMap.LinkedList(this); 
				this.recreateGridSections(); 
				this.updateVisibleSections(); 
			} 
			this.mapDiv.doCallback=function(commandName, commandArgument, contentOnly, async)
			{ 
				return this.gridSectionManager.doCallback(commandName, commandArgument, contentOnly, async); 
			} 
			this.mapDiv.getViewCenter=function()
			{ 
				return this.gridSectionManager.getViewCenter(); 
			} 
			this.mapDiv.setViewCenter=function(x, y)
			{ 
				this.gridSectionManager.setViewCenter(x, y); 
			} 
			this.mapDiv.getZoom=function()
			{ 
				return this.gridSectionManager.getZoom(); 
			} 
			this.mapDiv.setZoom=function(zoom)
			{ 
				this.gridSectionManager.setZoom(zoom); 
			} 
		} 
		this.getViewCenter=function()
		{ 
			var zoom=this.getZoom(); 
			var contentWidth=this.contentSizeWidth*zoom/100.0; 
			var contentHeight=this.contentSizeHeight*zoom/100.0; 
			var viewCenterX=parseInt(this.viewport.style.width)/2.0-parseInt(this.contentFiller.style.left); 
			var viewCenterY=parseInt(this.viewport.style.height)/2.0-parseInt(this.contentFiller.style.top); 
			viewCenterX/=contentWidth/100.0; 
			viewCenterY/=contentHeight/100.0; 
			var result=new DundasMap.Point(viewCenterX, viewCenterY); 
			return result; 
		} 
		this.setViewCenter=function(x, y)
		{ 
			var zoom=this.getZoom(); 
			var contentWidth=this.contentSizeWidth*zoom/100.0; 
			var contentHeight=this.contentSizeHeight*zoom/100.0; 
			x/=100.0/contentWidth; 
			y/=100.0/contentHeight; 
			this.contentFiller.style.left=parseInt(this.viewport.style.width)/2.0-x+"px"; 
			this.contentFiller.style.top=parseInt(this.viewport.style.height)/2.0-y+"px"; 
			this.updateVisibleSections(); 
		} 
		this.onClick=function(e)
		{ 
			if (window.event)
			{ 
				e=window.event; 
				e.layerX=e.offsetX; 
				e.layerY=e.offsetY; 
				e.target=e.srcElement; 
			} 
			var layerX=e.layerX; 
			var layerY=e.layerY; 
			var target=e.target; 
			if (this.viewport !=null)
			{ 
				while (target !=this.mapDiv)
				{ 
					if (target.tagName=="AREA")
					{ 
						var imageId=target.parentNode.id.split("Map")[0]; 
						target=document.getElementById(imageId); 
					} 
					layerX+=target.offsetLeft; 
					layerY+=target.offsetTop; 
					target=target.parentNode; 
				} 
			} 
			var url=this.getClickCallbackUrl(layerX, layerY); 
			var request=this.createRequest(); 
			if (request !=null)
			{ 
				this.performCallback(request, url, true); 
			} 
		} 
		this.doCallback=function(commandName, commandArgument, contentOnly, async)
		{ 
			if (contentOnly==null)
			{ 
				contentOnly=false; 
			} 
			if (contentOnly)
			{ 
				this.recreateGridSections(); 
				this.resetCachedSections(); 
				this.updateVisibleSections(commandName, commandArgument, async); 
			} 
			else
			{ 
				this.updateEntireControl(commandName, commandArgument, async); 
			} 
		} 
		this.updateEntireControl=function(commandName, commandArgument, async)
		{ 
			if (async==null)
			{ 
				async=true; 
			} 
			var url=this.getFullUpdateCallbackUrl(commandName, commandArgument); 
			var request=this.createRequest(); 
			if (request !=null)
			{ 
				this.performCallback(request, url, async); 
			} 
		} 
		this.updateVisibleSections=function(commandName, commandArgument, async)
		{ 
			if (async==null)
			{ 
				async=true; 
			} 
			var gridXParam=""; 
			var gridYParam=""; 
			var contentOffsetX=parseInt(this.contentFiller.style.left)-this.gridSectionsOffsetX; 
			var contentOffsetY=parseInt(this.contentFiller.style.top)-this.gridSectionsOffsetY; 
			var minVisibleX=Math.floor(-contentOffsetX/this.gridSectionSizeWidth)+1; 
			minVisibleX=Math.max(0, minVisibleX); 
			minVisibleX=Math.min(this.gridSectionsXCount-1, minVisibleX); 
			var minVisibleY=Math.floor(-contentOffsetY/this.gridSectionSizeHeight)+1; 
			minVisibleY=Math.max(0, minVisibleY); 
			minVisibleY=Math.min(this.gridSectionsYCount-1, minVisibleY); 
			var maxVisibleX=Math.floor((-contentOffsetX+this.viewportSizeWidth-1)/this.gridSectionSizeWidth)+1; 
			maxVisibleX=Math.max(0, maxVisibleX); 
			maxVisibleX=Math.min(this.gridSectionsXCount-1, maxVisibleX); 
			var maxVisibleY=Math.floor((-contentOffsetY+this.viewportSizeHeight-1)/this.gridSectionSizeHeight)+1; 
			maxVisibleY=Math.max(0, maxVisibleY); 
			maxVisibleY=Math.min(this.gridSectionsYCount-1, maxVisibleY); 
			this.currentVisibleBounds=new DundasMap.Rectangle(minVisibleX, minVisibleY, maxVisibleX-minVisibleX+1, maxVisibleX-minVisibleX+1); 
			for (var i=minVisibleX; i <= maxVisibleX; i++ )
			{ 
				for (var j=minVisibleY; j <= maxVisibleY;	j++ )
				{ 
					if (this.gridSections[i][j]==null)
					{ 
						this.gridSections[i][j]=new Image(); 
						this.gridSections[i][j].id="ImageX"+i+"Y"+j; 
						this.gridSections[i][j].name=this.gridSections[i][j].id; 
						this.gridSections[i][j].border=0; 
						this.gridSections[i][j].width=this.gridSectionSizeWidth; 
						this.gridSections[i][j].height=this.gridSectionSizeHeight; 
						this.gridSections[i][j].style.position="absolute"; 
						this.gridSections[i][j].style.left=(i-1)*this.gridSectionSizeWidth-this.gridSectionsOffsetX+"px"; 
						this.gridSections[i][j].style.top=(j-1)*this.gridSectionSizeHeight-this.gridSectionsOffsetY+"px"; 
						this.gridSections[i][j].style.width=this.gridSectionSizeWidth+"px"; 
						this.gridSections[i][j].style.height=this.gridSectionSizeHeight+"px"; 
						this.gridSections[i][j].style.backgroundPosition="center center"; 
						this.gridSections[i][j].style.backgroundRepeat="no-repeat"; 
						this.gridSections[i][j].style.backgroundImage="url("+this.renderingImageUrl+")"; 
						this.gridSections[i][j].src=this.emptyImageUrl; 
						if (this.isIE)
						{ 
							this.gridSections[i][j].galleryimg="no"; 
						} 
						this.gridSections[i][j].i=i; 
						this.gridSections[i][j].j=j; 
						this.contentFiller.appendChild(this.gridSections[i][j]); 
						gridXParam+=i+";"; 
						gridYParam+=j+";"; 
					} 
				} 
			} 
			if (gridXParam !="")
			{ 
				var url=this.getContentOnlyCallbackUrl(gridXParam, gridYParam, commandName, commandArgument, this.enableContentImageMaps); 
				var request=this.createRequest(); 
				if (request !=null)
				{ 
					this.requests.abortOutOfViewRequests(true); 
					this.requests.add(request, gridXParam, gridYParam, this.getZoom()); 
					this.performCallback(request, url, async); 
				} 
			} 
		} 
		this.performCallback=function(request, callbackUrl, async)
		{ 
			var thisManager=this; 
			request.onreadystatechange=function()
			{ 
				if (request.readyState==4 && parseInt(request.status) > 300)
				{ 
					window.location=callbackUrl; 
					return; 
				} 
				if (request.readyState==4 && request.status==200 && request.responseText !="")
				{ 
					if (thisManager.requests !=null)
					{ 
						var requestNode=thisManager.requests.find(request); 
						if (requestNode !=null && !requestNode.containsVisibleSections())
						{ 
							requestNode.removeGridSections(); 
							thisManager.requests.remove(request); 
							return; 
						} 
					} 
					var zoom=thisManager.getZoom(); 
					var nodes=request.responseText.split("[!]"); 
					for (var i=0; 
					i < nodes.length; 
					i++)
					{ 
						var subNodes=nodes[i].split("[!!]"); 
						if (subNodes[0]=="GridSection")
						{ 
							var x=subNodes[1]; 
							var y=subNodes[2]; 
							if (thisManager.gridSections[x] !=null && thisManager.gridSections[x][y] !=null)
							{ 
								var image=thisManager.gridSections[x][y]; 
								image.imageUrl=subNodes[3]; 
								if (subNodes.length==5)
								{ 
									image.imageMapUrl=subNodes[4]; 
								} 
								thisManager.downloads.add(image, x, y, zoom); 
								setTimeout(function()
								{ 
									thisManager.downloads.processDownloads(); 
								} 
								, 0); 
							} 
						} 
						else if (subNodes[0]=="DistanceScalePanel")
						{ 
							var url=subNodes[1]; 
							if (thisManager.isIE)
							{ 
								thisManager.distanceScalePanel.style.filter="progid:DXImageTransform.Microsoft.AlphaImageLoader(src='"+url+"')"; 
							} 
							else
							{ 
								thisManager.distanceScalePanel.src=url; 
							} 
						} 
						else if (subNodes[0]=="CallbackReturnArguments")
						{ 
							if (thisManager.mapDiv.oncallbackcomplete !=null)
							{ 
								thisManager.mapDiv.oncallbackcomplete(subNodes[1], subNodes[2]); 
							} 
						} 
						else if (subNodes[0]=="ClickReturnArguments")
						{ 
							if (thisManager.mapDiv.onclickcomplete !=null)
							{ 
								thisManager.mapDiv.onclickcomplete(subNodes[1], subNodes[2]); 
							} 
						} 
						else if (subNodes[0]=="FullUpdateHtml")
						{ 
							var tempDiv=document.createElement("DIV"); 
							tempDiv.innerHTML=subNodes[1]; 
							var newMap=tempDiv.firstChild; 
							while (newMap && newMap.nodeType !=1)
							{ 
								newMap=newMap.nextSibling; 
							} 
							var newInnerDiv=newMap.firstChild; 
							while (newInnerDiv && newInnerDiv.nodeType !=1)
							{ 
								newInnerDiv=newInnerDiv.nextSibling; 
							} 
							var newMapImage=newInnerDiv.firstChild; 
							while (newMapImage && newMapImage.nodeType !=1)
							{ 
								newMapImage=newMapImage.nextSibling; 
							} 
							var newImageUrl=newMapImage.src; 
							var oldMapImage=thisManager.mapImage; 
							oldMapImage.setAttribute("width", newMapImage.getAttribute("width", 2)); 
							oldMapImage.setAttribute("height", newMapImage.getAttribute("height", 2)); 
							newInnerDiv.replaceChild(oldMapImage, newMapImage); 
							var callbackCompleteRef=thisManager.mapDiv.oncallbackcomplete; 
							var clickCompleteRef=thisManager.mapDiv.onclickcomplete; 
							thisManager.mapDiv.parentNode.replaceChild(newMap, thisManager.mapDiv); 
							eval(subNodes[2]); 
							thisManager.mapImage.src=newImageUrl; 
							thisManager.mapDiv.oncallbackcomplete=callbackCompleteRef; 
							thisManager.mapDiv.onclickcomplete=clickCompleteRef; 
						} 
						else if (subNodes[0]=="ExecuteJavaScript")
						{ 
							eval(subNodes[1]); 
						} 
						else if (subNodes[0]=="UpdateClientControl")
						{ 
							var clientControl=document.getElementById(subNodes[1]); 
							if (clientControl !=null)
							{ 
								var tempDiv=document.createElement("DIV"); 
								tempDiv.innerHTML=subNodes[2]; 
								var newControl=tempDiv.firstChild
								while (newControl && newControl.nodeType !=1)
								{ 
									newControl=newControl.nextSibling; 
								} 
								clientControl.parentNode.replaceChild(newControl, clientControl); 
								clientControl=document.getElementById(subNodes[1]); 
							} 
						} 
					} 
					if (thisManager.requests !=null)
					{ 
						thisManager.requests.remove(request); 
					} 
				} 
			} 
			request.open("GET", callbackUrl, async); 
			request.send(" "); 
		} 
		this.downloadImageMap=function(request, imageMapUrl, x, y)
		{ 
			var thisManager=this; 
			request.onreadystatechange=function()
			{ 
				if (request.readyState==4 && parseInt(request.status) > 300)
				{ 
					window.location=imageMapUrl; 
					return; 
				} 
				if (request.readyState==4 && request.status==200 && request.responseText !="")
				{ 
					if (thisManager.gridSections[x] !=null && thisManager.gridSections[x][y] !=null)
					{ 
						var image=thisManager.gridSections[x][y]; 
						image.imageMapText=request.responseText; 
						if (!thisManager.isIE)
						{ 
							thisManager.addImageMap(image); 
						} 
						setTimeout(function()
						{ 
							thisManager.downloads.processDownloads(); 
						} 
						, 0); 
					} 
				} 
			} 
			request.open("GET", imageMapUrl, true); 
			request.send(" "); 
		} 
		this.findImageMap=function(imageMapName)
		{ 
			for (var i=0; 
			i < this.viewport.childNodes.length; 
			i++)
			{ 
				if (this.viewport.childNodes[i].id==imageMapName)
				{ 
					return i; 
				} 
			} 
			return-1; 
		} 
		this.getViewParams=function()
		{ 
			var zoom=this.getZoom(); 
			var contentWidth=this.contentSizeWidth*zoom/100.0; 
			var contentHeight=this.contentSizeHeight*zoom/100.0; 
			var viewCenterX=parseInt(this.viewport.style.width)/2.0-parseInt(this.contentFiller.style.left); 
			var viewCenterY=parseInt(this.viewport.style.height)/2.0-parseInt(this.contentFiller.style.top); 
			viewCenterX/=contentWidth/100.0; 
			viewCenterY/=contentHeight/100.0; 
			return "&_viewCenterX="+viewCenterX+"&_viewCenterY="+viewCenterY+"&_zoom="+zoom+
			"&_gridOffsetX="+this.gridSectionsOffsetX+"&_gridOffsetY="+this.gridSectionsOffsetY; 
		} 
		this.getClickCallbackUrl=function(x, y)
		{ 
			var requestParams="_mapControlID="+this.mapControlId+
			"&_controlPersistence="+this.controlPersistence+
			"&_clickX="+x+"&_clickY="+y; 
			if (this.viewport !=null)
			{ 
				requestParams+=this.getViewParams(); 
			} 
			var now=new Date(); 
			requestParams+="&_time="+now.getTime()+now.getMilliseconds(); 
			if (location.href.indexOf("?")==-1)
			{ 
				return location.href+"?"+requestParams; 
			} 
			else
			{ 
				return location.href+"&"+requestParams; 
			} 
		} 
		this.getFullUpdateCallbackUrl=function(commandName, commandArgument)
		{ 
			var requestParams="_mapControlID="+this.mapControlId+"&_fullUpdate=1"+
			"&_controlPersistence="+this.controlPersistence; 
			if (this.viewport !=null)
			{ 
				requestParams+=this.getViewParams(); 
			} 
			if (commandName !=null)
			{ 
				requestParams+="&_command="+encodeURIComponent(commandName); 
			} 
			if (commandArgument !=null)
			{ 
				requestParams+="&_arguments="+encodeURIComponent(commandArgument); 
			} 
			var now=new Date(); 
			requestParams+="&_time="+now.getTime()+now.getMilliseconds(); 
			if (location.href.indexOf("?")==-1)
			{ 
				return location.href+"?"+requestParams; 
			} 
			else
			{ 
				return location.href+"&"+requestParams; 
			} 
		} 
		this.getContentOnlyCallbackUrl=function(gridX, gridY, commandName, commandArgument, imageMap)
		{ 
			var requestParams="_mapControlID="+this.mapControlId+
			"&_controlPersistence="+this.controlPersistence+
			"&_imageUrl="+encodeURIComponent(this.imageUrl)+
			"&_gridX="+gridX+"&_gridY="+gridY; 
			if (this.viewport !=null)
			{ 
				requestParams+=this.getViewParams(); 
			} 
			if (commandName !=null)
			{ 
				requestParams+="&_command="+encodeURIComponent(commandName); 
			} 
			if (commandArgument !=null)
			{ 
				requestParams+="&_arguments="+encodeURIComponent(commandArgument); 
			} 
			if (imageMap==true)
			{ 
				requestParams+="&_imageMap=1"; 
			} 
			var now=new Date(); 
			requestParams+="&_time="+now.getTime()+now.getMilliseconds(); 
			if (location.href.indexOf("?")==-1)
			{ 
				return location.href+"?"+requestParams; 
			} 
			else
			{ 
				return location.href+"&"+requestParams; 
			} 
		} 
		this.createRequest=function()
		{ 
			var request=null; 
			if (window.XMLHttpRequest)
			{ 
				try
				{ 
					request=new XMLHttpRequest(); 
				} 
				catch (e)
				{ 
					request=null; 
				} 
			} 
			else if (window.ActiveXObject)
			{ 
				try
				{ 
					request=new ActiveXObject("Msxml2.XMLHTTP")} 
				catch (e)
				{ 
					try
					{ 
						request=new ActiveX("Microsoft.XMLHTTP"); 
					} 
					catch (e)
					{ 
						request=null; 
					} 
				} 
			} 
			return request; 
		} 
		this.addImageMap=function(image)
		{ 
			if (image.imageMapText !=null)
			{ 
				var imageMap=document.createElement("MAP"); 
				imageMap.id=image.name+"Map"; 
				imageMap.name=imageMap.id; 
				imageMap.i=image.i; 
				imageMap.j=image.j; 
				imageMap.innerHTML=image.imageMapText; 
				this.viewport.appendChild(imageMap); 
				image.useMap="#"+imageMap.id; 
			} 
		} 
		this.determineGridSectionCounts=function()
		{ 
			var zoomFactor=this.getZoom()/100.0; 
			var contentWidth=this.contentSizeWidth*zoomFactor; 
			var contentHeight=this.contentSizeHeight*zoomFactor; 
			this.gridSectionsXCount=Math.ceil(contentWidth/this.gridSectionSizeWidth)+2; 
			this.gridSectionsYCount=Math.ceil(contentHeight/this.gridSectionSizeHeight)+2; 
			if (this.contentCachingEnabled)
			{ 
				this.gridSectionsOffsetX=0; 
				this.gridSectionsOffsetY=0; 
			} 
			else
			{ 
				this.gridSectionsOffsetX=Math.round(parseInt(this.contentFiller.style.left) % this.gridSectionSizeWidth); 
				this.gridSectionsOffsetY=Math.round(parseInt(this.contentFiller.style.top) % this.gridSectionSizeHeight); 
			} 
		} 
		this.recreateGridSections=function()
		{ 
			if (this.viewport !=null)
			{ 
				this.requests.abortOutOfViewRequests(false); 
				this.gridSections=new Array(this.gridSectionsXCount); 
				for (var i=0; 
				i < this.gridSectionsXCount; 
				i++)
				{ 
					this.gridSections[i]=new Array(this.gridSectionsYCount); 
				} 
			} 
		} 
		this.resetCachedSections=function()
		{ 
			if (this.downloads !=null)
			{ 
				this.downloads.clearDownloads(); 
			} 
			for (var i=0; 
			i < this.contentFiller.childNodes.length; 
			i++)
			{ 
				var child=this.contentFiller.childNodes[i]; 
				if (child.tagName=="IMG" && child.i !=null && child.j !=null)
				{ 
					this.gridSections[child.i][child.j]=null; 
					this.contentFiller.removeChild(child); 
					i--; 
				} 
			} 
			if (this.enableContentImageMaps)
			{ 
				for (var i=0; 
				i < this.viewport.childNodes.length; 
				i++)
				{ 
					if (this.viewport.childNodes[i].tagName=="MAP")
					{ 
						this.viewport.removeChild(this.viewport.childNodes[i--]); 
					} 
				} 
			} 
		} 
		this.scrollTo=function(x, y)
		{ 
			var zoomFactor=this.getZoom()/100.0; 
			var contentWidth=this.contentSizeWidth*zoomFactor; 
			var contentHeight=this.contentSizeHeight*zoomFactor; 
			var margin=20; 
			x=Math.max(x,-parseInt(contentWidth)+margin); 
			x=Math.min(x, parseInt(this.viewport.style.width)-margin); 
			y=Math.max(y,-parseInt(contentHeight)+margin); 
			y=Math.min(y, parseInt(this.viewport.style.height)-margin); 
			this.contentFiller.style.left=x+"px"; 
			this.contentFiller.style.top=y+"px"; 
			if (this.mapDiv.onpan !=null)
			{ 
				this.mapDiv.onpan(); 
			} 
		} 
		this.scrollContent=function(direction, step)
		{ 
            //debugger;
			var zoomFactor=this.getZoom()/100.0; 
			var contentWidth=this.contentSizeWidth*zoomFactor; 
			var contentHeight=this.contentSizeHeight*zoomFactor; 
			var stepInPixelsX=step/100.0*contentWidth; 
			var stepInPixelsY=step/100.0*contentHeight; 
			var margin=20; 
			if (direction=="East")
			{ 
				var x=parseInt(this.contentFiller.style.left)-stepInPixelsX; 
				x=Math.max(x,-parseInt(contentWidth)+margin); 
				this.contentFiller.style.left=x+"px"; 
			} 
			else if (direction=="West")
			{ 
				var x=parseInt(this.contentFiller.style.left)+stepInPixelsX; 
				x=Math.min(x, parseInt(this.viewport.style.width)-margin); 
				this.contentFiller.style.left=x+"px"; 
			} 
			else if (direction=="North")
			{ 
				var y=parseInt(this.contentFiller.style.top)+stepInPixelsY; 
				y=Math.min(y, parseInt(this.viewport.style.height)-margin); 
				this.contentFiller.style.top=y+"px"; 
			} 
			else if (direction=="South")
			{ 
				var y=parseInt(this.contentFiller.style.top)-stepInPixelsY; 
				y=Math.max(y,-parseInt(contentHeight)+margin); 
				this.contentFiller.style.top=y+"px"; 
			} 
			if (this.mapDiv.onpan !=null)
			{ 
				this.mapDiv.onpan(); 
			} 
			this.updateVisibleSections(); 
		} 
		this.onDragStart=function(e)
		{ 
			e=e || window.event; 
			e.returnValue=false; 
		} 
		this.onMouseDown=function(e)
		{ 
			//debugger;
			e=e || window.event; 
			this.mouseDownX=e.clientX; 
			this.mouseDownY=e.clientY; 
			this.updateTimeoutID=null; 
			if (e.cancelable && e.preventDefault)
			{ 
				e.preventDefault(); 
			} 
			if (!this.enablePanning)
			{ 
				return; 
			} 
			this.initialScrollX=parseInt(this.contentFiller.style.left); 
			this.initialScrollY=parseInt(this.contentFiller.style.top); 
			if (window.addEventListener)
			{ 
				window.__thisManager=this; 
				window.addEventListener("mousemove", this.onMouseMove, false); 
				window.addEventListener("mouseup", this.onMouseUp, false); 
			} 
			else if (this.viewport.attachEvent)
			{ 
				//debugger;
				this.viewport.__thisManager=this; 
				this.viewport.onmousemove=this.onMouseMove; 
				this.viewport.onmouseup=this.onMouseUp; 
				this.viewport.setCapture(); 
			} 
		} 
		this.onMouseMove=function(e)
		{ 
			//debugger;
			e=e || window.event; 
			var thisManager=null; 
			if (window.__thisManager)
			{ 
				thisManager=window.__thisManager; 
			} 
			else if (this.__thisManager)
			{ 
				thisManager=this.__thisManager; 
			} 
			if (thisManager.mouseDownX !=null)
			{ 
				var deltaX=e.clientX-thisManager.mouseDownX; 
				var deltaY=e.clientY-thisManager.mouseDownY; 
				thisManager.scrollTo(thisManager.initialScrollX+deltaX, thisManager.initialScrollY+deltaY); 
				thisManager.updateVisibleSections(); 
			} 
			return true; 
		} 
		this.onMouseUp=function(e)
		{ 
			e=e || window.event; 
			var thisManager=null; 
			if (window.__thisManager)
			{ 
				thisManager=window.__thisManager; 
				window.removeEventListener("mousemove", thisManager.onMouseMove, false); 
				window.removeEventListener("mouseup", thisManager.onMouseUp, false); 
				window.__thisManager=null; 
			} 
			else if (this.__thisManager)
			{ 
				thisManager=this.__thisManager; 
				thisManager.viewport.releaseCapture(); 
				thisManager.viewport.onmousemove=null; 
				thisManager.viewport.onmouseup=null; 
				this.__thisManager=null; 
			} 
			thisManager.mouseDownX=null; 
			thisManager.mouseDownY=null; 
		} 
		this.getZoomPanelScaleRect=function()
		{ 
			var result; 
			if (this.zoomPanelOrientation=="Horizontal")
			{ 
				result=new DundasMap.Rectangle(this.zoomPanelScaleMargin, 0, 100-(this.zoomPanelScaleMargin*2), 100); 
			} 
			else 
			{ 
				result=new DundasMap.Rectangle(0, this.zoomPanelScaleMargin, 100, 100-(this.zoomPanelScaleMargin*2)); 
			} 
			var conversionFactorX=100/(parseInt(this.zoomPanel.style.width)-this.zoomPanelBorderWidth*2); 
			var conversionFactorY=100/(parseInt(this.zoomPanel.style.height)-this.zoomPanelBorderWidth*2); 
			result.x/=conversionFactorX; 
			result.y/=conversionFactorY; 
			result.width/=conversionFactorX; 
			result.height/=conversionFactorY; 
			result.x+=this.zoomPanelBorderWidth; 
			result.y+=this.zoomPanelBorderWidth; 
			return result; 
		} 
		this.getZoomPanelRect=function(plus)
		{ 
			var result=null; 
			if (this.zoomPanelReversed)
			{ 
				plus=!plus; 
			} 
			if (this.zoomPanelOrientation=="Horizontal")
			{ 
				if (plus)
				{ 
					result=new DundasMap.Rectangle(100-this.zoomPanelScaleMargin, 0, this.zoomPanelScaleMargin, 100); 
				} 
				else 
				{ 
					result=new DundasMap.Rectangle(0, 0, this.zoomPanelScaleMargin, 100); 
				} 
			} 
			else 
			{ 
				if (plus)
				{ 
					result=new DundasMap.Rectangle(0, 0, 100, this.zoomPanelScaleMargin); 
				} 
				else 
				{ 
					result=new DundasMap.Rectangle(0, 100-this.zoomPanelScaleMargin, 100, this.zoomPanelScaleMargin); 
				} 
			} 
			var conversionFactorX=100/(parseInt(this.zoomPanel.style.width)-this.zoomPanelBorderWidth*2); 
			var conversionFactorY=100/(parseInt(this.zoomPanel.style.height)-this.zoomPanelBorderWidth*2); 
			result.x/=conversionFactorX; 
			result.y/=conversionFactorY; 
			result.width/=conversionFactorX; 
			result.height/=conversionFactorY; 
			result.x+=this.zoomPanelBorderWidth; 
			result.y+=this.zoomPanelBorderWidth; 
			return result; 
		} 
		this.getZoomPanelThumbRect=function()
		{ 
			var result; 
			var scaleRect=this.getZoomPanelScaleRect(); 
			if (this.zoomPanelOrientation=="Horizontal")
			{ 
				var thumbPosition=scaleRect.x; 
				if (this.zoomPanelReversed)
				{ 
					thumbPosition+=scaleRect.width*(1-this.zoomPanelPointerValue/100); 
				} 
				else
				{ 
					thumbPosition+=scaleRect.width*this.zoomPanelPointerValue/100; 
				} 
				result=new DundasMap.Rectangle(thumbPosition, scaleRect.y+scaleRect.height/2,
				this.zoomPanelPointerWidth/100*scaleRect.height,
				this.zoomPanelPointerLength/100*scaleRect.height); 
			} 
			else 
			{ 
				var thumbPosition=scaleRect.y; 
				if (this.zoomPanelReversed)
				{ 
					thumbPosition+=scaleRect.height*this.zoomPanelPointerValue/100; 
				} 
				else
				{ 
					thumbPosition+=scaleRect.height*(1-this.zoomPanelPointerValue/100); 
				} 
				result=new DundasMap.Rectangle(scaleRect.x+scaleRect.width/2, thumbPosition,
				this.zoomPanelPointerLength/100*scaleRect.width,
				this.zoomPanelPointerWidth/100*scaleRect.width); 
			} 
			result.x-=result.width/2; 
			result.y-=result.height/2; 
			return result; 
		} 
		this.getZoomPanelValueInPixels=function(valueInPercents)
		{ 
			var result=0; 
			var scaleRect=this.getZoomPanelScaleRect(); 
			if (this.zoomPanelOrientation=="Horizontal")
			{ 
				result=scaleRect.width*(valueInPercents-this.zoomPanelPointerValue)/100; 
			} 
			else 
			{ 
				result=scaleRect.height*(this.zoomPanelPointerValue-valueInPercents)/100; 
			} 
			if (this.zoomPanelReversed)
			{ 
				result *=-1; 
			} 
			return result; 
		} 
		this.getZoomPanelValueInPercents=function(valueInPixels)
		{ 
			var result=0; 
			var scaleRect=this.getZoomPanelScaleRect(); 
			if (this.zoomPanelOrientation=="Horizontal")
			{ 
				var initialValueInPixels=scaleRect.width*this.zoomPanelPointerValue/100; 
				if (this.zoomPanelReversed)
				{ 
					result=(initialValueInPixels-valueInPixels)/ scaleRect.width*100; 
				} 
				else
				{ 
					result=(initialValueInPixels+valueInPixels)/ scaleRect.width*100; 
				} 
			} 
			else 
			{ 
				var initialValueInPixels=scaleRect.height*this.zoomPanelPointerValue/100; 
				if (this.zoomPanelReversed)
				{ 
					result=(initialValueInPixels+valueInPixels)/ scaleRect.height*100; 
				} 
				else
				{ 
					result=(initialValueInPixels-valueInPixels)/ scaleRect.height*100; 
				} 
			} 
			return result; 
		} 
		this.snapToPossibleZoomValue=function(value)
		{ 
            //debugger;
			if (!this.zoomPanelSnapToTickMarks)
			{ 
				return value; 
			} 
			var possibleValues=this.getPossibleZoomValues(); 
			var minDelta=99999999; 
			var minDeltaIndex=0; 
			for (var i=0; 
			i < possibleValues.length; 
			i++)
			{ 
				var currentDelta=Math.abs(possibleValues[i]-value); 
				if (currentDelta < minDelta)
				{ 
					minDelta=currentDelta; 
					minDeltaIndex=i; 
				} 
			} 
			return possibleValues[minDeltaIndex]; 
		} 
		this.getPossibleZoomValues=function()
		{ 
			var result=new Array(this.zoomPanelTickCount); 
			var currentValue=0; 
			var increment=100/(this.zoomPanelTickCount-1); 
			for (var i=0; 
			i < this.zoomPanelTickCount; 
			i++)
			{ 
				result[i]=this.getZoomFromThumbValue(currentValue); 
				currentValue+=increment; 
				result[i]*=100; 
				result[i]=Math.round(result[i]); 
				result[i]/=100; 
			} 
			return result; 
		} 
		this.getCurrentThumbValue=function()
		{ 
			if (this.zoomPanelOrientation=="Horizontal")
			{ 
				return this.getZoomPanelValueInPercents(parseInt(this.zoomPanelThumb.style.left)); 
			} 
			else 
			{ 
				return this.getZoomPanelValueInPercents(parseInt(this.zoomPanelThumb.style.top)); 
			} 
		} 
		this.setCurrentThumbValue=function(value)
		{ 
			if (this.zoomPanelOrientation=="Horizontal")
			{ 
				this.zoomPanelThumb.style.left=this.getZoomPanelValueInPixels(value)+"px"; 
			} 
			else 
			{ 
				this.zoomPanelThumb.style.top=this.getZoomPanelValueInPixels(value)+"px"; 
			} 
		} 
		this.getZoomFromThumbValue=function(pos)
		{ 
			var c=this.minimumZoom; 
			var maxScaleValue=100.0; 
			var res=0; 
			if (this.zoomType=="Quadratic")
			{ 
				var a=(this.maximumZoom-c)/(maxScaleValue*maxScaleValue); 
				res=a*pos*pos+c; 
			} 
			else if (this.zoomType=="Exponential")
			{ 
				var a=(this.maximumZoom-c)/(maxScaleValue*maxScaleValue); 
				res=c*Math.pow(this.maximumZoom/c, pos/maxScaleValue); 
			} 
			else
			{ 
				var a=(this.maximumZoom-c)/maxScaleValue; 
				res=a*pos+c; 
			} 
			return res; 
		} 
		this.getThumbValueFromZoom= function(zoom)
		{ 
			var c=this.minimumZoom; 
			var maxScaleValue=100.0; 
			var res=0; 
			if (this.zoomType=="Quadratic")
			{ 
				var a=(this.maximumZoom-c)/(maxScaleValue*maxScaleValue); 
				res=Math.sqrt((zoom-c)/a); 
			} 
			else if (this.zoomType=="Exponential")
			{ 
				var a=Math.log(this.maximumZoom/c)/maxScaleValue; 
				res=Math.log(zoom/c)/a; 
			} 
			else
			{ 
				var a=(this.maximumZoom-c)/maxScaleValue; 
				res=(zoom-c)/a; 
			} 
			return res; 
		} 
		this.snapZoomPanelThumb=function()
		{ 
			if (!this.zoomPanelSnapToTickMarks)
			{ 
				return; 
			} 
			var value=this.getCurrentThumbValue(); 
			var interval=100/(this.zoomPanelTickCount-1); 
			value/=interval; 
			value=Math.round(value); 
			value*=interval; 
			this.setCurrentThumbValue(value); 
		} 
		this.incrementZoom=function(positive)
		{ 
			var oldThumbValue=this.getCurrentThumbValue(); 
			if ((positive && oldThumbValue > 99) || (!positive && oldThumbValue < 1))
			{ 
				return; 
			} 
			var interval=100/(this.zoomPanelTickCount-1); 
			var newThumbValue=null; 
			if (positive)
			{ 
				newThumbValue=oldThumbValue+interval; 
				if (newThumbValue > 100)
				{ 
					newThumbValue=100; 
				} 
			} 
			else 
			{ 
				newThumbValue=oldThumbValue-interval; 
				if (newThumbValue < 0)
				{ 
					newThumbValue=0; 
				} 
			} 
			this.setCurrentThumbValue(newThumbValue); 
			this.snapZoomPanelThumb(); 
			var newZoom=this.getZoomFromThumbValue(this.getCurrentThumbValue()); 
			newZoom=this.snapToPossibleZoomValue(newZoom); 
			this.changeZoom(newZoom); 
		} 
		this.getZoom=function()
		{ 
			return this.currentZoom; 
		} 
		this.setZoom=function(newZoom)
		{ 
			if (this.zoomPanel !=null)
			{ 
				var newThumbValue=this.getThumbValueFromZoom(newZoom); 
				this.setCurrentThumbValue(newThumbValue); 
			} 
			this.changeZoom(newZoom); 
		} 
		this.changeZoom=function(newZoom)
		{ 
			var oldZoom=this.currentZoom; 
			this.resetCachedSections(); 
			var contentOffsetX=parseInt(this.contentFiller.style.left); 
			var contentOffsetY=parseInt(this.contentFiller.style.top); 
			var centerX=parseInt(this.viewport.style.width)/2.0; 
			var centerY=parseInt(this.viewport.style.height)/2.0; 
			var xScroll=(contentOffsetX-centerX)/oldZoom*newZoom+centerX; 
			var yScroll=(contentOffsetY-centerY)/oldZoom*newZoom+centerY; 
			this.contentFiller.style.left=xScroll+"px"; 
			this.contentFiller.style.top=yScroll+"px"; 
			this.currentZoom=newZoom; 
			this.determineGridSectionCounts(); 
			this.contentFiller.style.width=this.gridSectionsXCount*this.gridSectionSizeWidth+"px"; 
			this.contentFiller.style.height=this.gridSectionsYCount*this.gridSectionSizeHeight+"px"; 
			this.recreateGridSections(); 
			this.updateVisibleSections(); 
			if (this.mapDiv.onzoom !=null)
			{ 
				this.mapDiv.onzoom(); 
			} 
		} 
		this.onZoomPanelMouseDown=function(e)
		{ 
			if (window.event)
			{ 
				e=window.event; 
				e.layerX=e.offsetX; 
				e.layerY=e.offsetY; 
			} 
			if (e.cancelable && e.preventDefault)
			{ 
				e.preventDefault(); 
			} 
			if (this.getZoomPanelThumbRect().contains(e.layerX, e.layerY))
			{ 
				this.mouseDownX=e.clientX; 
				this.mouseDownY=e.clientY; 
				this.initialThumbOffsetX=parseInt(this.zoomPanelThumb.style.left); 
				this.initialThumbOffsetY=parseInt(this.zoomPanelThumb.style.top); 
				if (window.addEventListener)
				{ 
					window.__thisManager=this; 
					window.addEventListener("mousemove", this.onZoomPanelMouseMove, false); 
					window.addEventListener("mouseup", this.onZoomPanelMouseUp, false); 
				} 
				else if (this.zoomPanel.attachEvent)
				{ 
					this.zoomPanel.__thisManager=this; 
					this.zoomPanel.onmousemove=this.onZoomPanelMouseMove; 
					this.zoomPanel.onmouseup=this.onZoomPanelMouseUp; 
					this.zoomPanel.setCapture(); 
				} 
			} 
		} 
		this.onZoomPanelMouseMove=function(e)
		{ 
			if (window.event)
			{ 
				e=window.event; 
				e.layerX=e.offsetX; 
				e.layerY=e.offsetY; 
			} 
			var thisManager=null; 
			if (window.__thisManager)
			{ 
				thisManager=window.__thisManager; 
			} 
			else if (this.__thisManager)
			{ 
				thisManager=this.__thisManager; 
			} 
			if (thisManager.mouseDownX !=null)
			{ 
				var minValue=thisManager.getZoomPanelValueInPixels(0); 
				var maxValue=thisManager.getZoomPanelValueInPixels(100); 
				if (thisManager.zoomPanelOrientation=="Horizontal")
				{ 
					var deltaX=e.clientX-thisManager.mouseDownX; 
					var newValue=thisManager.initialThumbOffsetX+deltaX; 
					if (thisManager.zoomPanelReversed)
					{ 
						if (newValue > minValue)
						{ 
							newValue=minValue; 
						} 
						if (newValue < maxValue)
						{ 
							newValue=maxValue; 
						} 
					} 
					else
					{ 
						if (newValue < minValue)
						{ 
							newValue=minValue; 
						} 
						if (newValue > maxValue)
						{ 
							newValue=maxValue; 
						} 
					} 
					thisManager.zoomPanelThumb.style.left=newValue+"px"; 
				} 
				else 
				{ 
					var deltaY=e.clientY-thisManager.mouseDownY; 
					var newValue=thisManager.initialThumbOffsetY+deltaY; 
					if (thisManager.zoomPanelReversed)
					{ 
						if (newValue < minValue)
						{ 
							newValue=minValue; 
						} 
						if (newValue > maxValue)
						{ 
							newValue=maxValue; 
						} 
					} 
					else
					{ 
						if (newValue > minValue)
						{ 
							newValue=minValue; 
						} 
						if (newValue < maxValue)
						{ 
							newValue=maxValue; 
						} 
					} 
					thisManager.zoomPanelThumb.style.top =newValue+"px"; 
				} 
			} 
			return true; 
		} 
		this.onZoomPanelMouseUp=function(e)
		{ 
			e=e || window.event; 
			var thisManager=null; 
			if (window.__thisManager)
			{ 
				thisManager=window.__thisManager; 
				window.removeEventListener("mousemove", thisManager.onZoomPanelMouseMove, false); 
				window.removeEventListener("mouseup", thisManager.onZoomPanelMouseUp, false); 
				window.__thisManager=null; 
			} 
			else if (this.__thisManager)
			{ 
				thisManager=this.__thisManager; 
				thisManager.zoomPanel.releaseCapture(); 
				thisManager.zoomPanel.onmousemove=null; 
				thisManager.zoomPanel.onmouseup=null; 
				this.__thisManager=null; 
			} 
			thisManager.mouseDownX=null; 
			thisManager.mouseDownY=null; 
			thisManager.snapZoomPanelThumb(); 
			var newZoom=thisManager.getZoomFromThumbValue(thisManager.getCurrentThumbValue()); 
			newZoom=thisManager.snapToPossibleZoomValue(newZoom); 
			thisManager.changeZoom(newZoom); 
		} 
		this.onZoomPanelClick=function(e)
		{ 
			if (window.event)
			{ 
				e=window.event; 
				e.layerX=e.offsetX; 
				e.layerY=e.offsetY; 
				e.target=e.srcElement; 
			} 
			var layerX=e.layerX; 
			var layerY=e.layerY; 
			var target=e.target; 
            //debugger;
            // в IE проверка target != this.zoomPanel не срабатывает
			while (target.id !=this.zoomPanel.id && target != null)
			{ 
				layerX+=e.target.offsetLeft; 
				layerY+=e.target.offsetTop; 
				target=target.parentNode; 
			} 
			if (this.getZoomPanelRect(true).contains(layerX, layerY))
			{ 
				this.incrementZoom(true); 
				return true; 
			} 
			if (this.getZoomPanelRect(false).contains(layerX, layerY))
			{ 
				this.incrementZoom(false); 
				return true; 
			} 
		} 
		this.hitTestNavigationButton=function(x, y)
		{ 
			x/=parseInt(this.navigationPanel.style.width)/100.0; 
			y/=parseInt(this.navigationPanel.style.height)/100.0; 
			if (this.navigationPanelEastButton.contains(x, y))
			{ 
				return "East"; 
			} 
			else if (this.navigationPanelWestButton.contains(x, y))
			{ 
				return "West"; 
			} 
			else if (this.navigationPanelNorthButton.contains(x, y))
			{ 
				return "North"; 
			} 
			else if (this.navigationPanelSouthButton.contains(x, y))
			{ 
				return "South"; 
			} 
			else
			{ 
				return null; 
			} 
		} 
		this.onNavigationPanelMouseDown=function(e)
		{ 
			if (window.event)
			{ 
				e=window.event; 
				e.layerX=e.offsetX; 
				e.layerY=e.offsetY; 
			} 
			if (e.cancelable && e.preventDefault)
			{ 
				e.preventDefault(); 
			} 
		} 
		this.onNavigationPanelClick=function(e)
		{ 
			if (window.event)
			{ 
				e=window.event; 
				e.layerX=e.offsetX; 
				e.layerY=e.offsetY; 
			} 
			var clickedButton=this.hitTestNavigationButton(e.layerX, e.layerY); 
			if (clickedButton !=null)
			{ 
				this.scrollContent(clickedButton, this.navigationPanelScrollStep*100/this.getZoom()); 
			} 
		} 
	} 
	,
Point:function(x, y)
	{ 
		this.x=x; 
		this.y=y; 
	} 
	,
Rectangle:function(x, y, width, height)
	{ 
		this.x=x; 
		this.y=y; 
		this.width=width; 
		this.height=height; 
		this.intersectsWith=function(rect)
		{ 
			if (((rect.x < (this.x+this.width)) && (this.x < (rect.x+rect.width))) &&
					(rect.y < (this.y+this.height)))
			{ 
				return (this.y < (rect.y+rect.height)); 
			} 
			return false; 
		} 
		this.contains=function(x, y)
		{ 
			if (((this.x <=x) && (x < (this.x+this.width))) && (this.y <=y))
			{ 
				return (y < (this.y+this.height)); 
			} 
			return false; 
		} 
		this.unionWith=function(rect)
		{ 
			var num1=min(this.x, rect.x); 
			var num2=max(this.x+this.width, rect.x+rect.width); 
			var num3=min(this.y, rect.y); 
			var num4=max(this.y+this.height, rect.y+rect.height); 
			this.x=num1; 
			this.y=num3; 
			this.width=num2-num1; 
			this.height=num4-num3; 
		} 
	} 
	,
LinkedList:function(gridSectionManager)
	{ 
		this.gridSectionManager=gridSectionManager; 
		this.head=null; 
		this.downloading=false; 
		this.add=function(obj, gridXParam, gridYParam, zoom)
		{ 
			if (this.head==null)
			{ 
				this.head=new DundasMap.ListNode(obj, gridXParam, gridYParam, zoom, this.gridSectionManager); 
			} 
			else
			{ 
				var currentNode=this.head; 
				while (currentNode.next !=null)
				{ 
					currentNode=currentNode.next; 
				} 
				currentNode.next=new DundasMap.ListNode(obj, gridXParam, gridYParam, zoom, this.gridSectionManager); 
			} 
		} 
		this.remove=function(obj)
		{ 
			if (this.head==null)
			{ 
				return; 
			} 
			else if (this.head.value==obj)
			{ 
				this.head=this.head.next; 
			} 
			else
			{ 
				var currentNode=this.head; 
				while (currentNode.next !=null)
				{ 
					if (currentNode.next.value==obj)
					{ 
						currentNode.next=currentNode.next.next; 
						return; 
					} 
					currentNode=currentNode.next; 
				} 
			} 
		} 
		this.clearDownloads=function()
		{ 
			var currentNode=this.head; 
			var prevNode=null; 
			while (currentNode !=null)
			{ 
				if (currentNode.value.src !=null)
				{ 
					currentNode.value.src=this.gridSectionManager.emptyImageUrl; 
				} 
				currentNode.value=null; 
				prevNode=currentNode; 
				currentNode=currentNode.next; 
			} 
			this.head=null; 
			this.downloading=false; 
		} 
		this.getCount=function()
		{ 
			var count=0; 
			var currentNode=this.head; 
			while (currentNode !=null)
			{ 
				count++; 
				currentNode=currentNode.next; 
			} 
			return count; 
		} 
		this.find=function(value)
		{ 
			var currentNode=this.head; 
			while (currentNode !=null)
			{ 
				if (currentNode.value==value)
				{ 
					return currentNode; 
				} 
				currentNode=currentNode.next; 
			} 
			return null; 
		} 
		this.abortOutOfViewRequests=function(checkVisibility)
		{ 
			var currentNode=this.head; 
			var prevNode=null; 
			while (currentNode !=null)
			{ 
				if (!checkVisibility || !currentNode.containsVisibleSections())
				{ 
					currentNode.abortRequest(); 
					if (prevNode==null)
					{ 
						this.head=currentNode.next; 
					} 
					else
					{ 
						prevNode.next=currentNode.next; 
					} 
				} 
				else
				{ 
					prevNode=currentNode; 
				} 
				currentNode=currentNode.next; 
			} 
		} 
		this.processDownloads=function()
		{ 
			if (this.downloading || this.head==null)
			{ 
				return; 
			} 
			var image=this.head.value; 
			if (this.head.isVisible())
			{ 
				var thisList=this; 
				var onloadHandler=function(e)
				{ 
					if (thisList.head !=null && image.src !=thisList.gridSectionManager.emptyImageUrl)
					{ 
						var x=thisList.head.gridXParam; 
						var y=thisList.head.gridYParam; 
						if (image.buffer !=null)
						{ 
							thisList.swapBuffer(image, x, y); 
							image=image.buffer; 
						} 
						thisList.downloading=false; 
						thisList.head=thisList.head.next; 
						if (image.imageMapUrl !=null)
						{ 
							var request=thisList.gridSectionManager.createRequest(); 
							if (request !=null)
							{ 
								thisList.gridSectionManager.downloadImageMap(request, image.imageMapUrl, x, y); 
							} 
						} 
						else
						{ 
							setTimeout(function()
							{ 
								thisList.processDownloads(); 
							} 
							, 0); 
						} 
					} 
				} 
				; 
				this.downloading=true; 
				if (this.gridSectionManager.isIE)
				{ 
					image.buffer=new Image(); 
					image.buffer.onload=onloadHandler; 
					image.buffer.src=image.imageUrl; 
				} 
				else if (image.addEventListener !=null)
				{ 
					image.addEventListener("load", onloadHandler, false); 
					image.src=image.imageUrl; 
				} 
			} 
			else
			{ 
				if (this.head==null)
				{ 
					this.downloading=false; 
					return; 
				} 
				else if (image !=null && image.parentNode !=null)
				{ 
					image.parentNode.removeChild(image); 
					image.src=this.gridSectionManager.emptyImageUrl; 
					this.gridSectionManager.gridSections[this.head.gridXParam][this.head.gridYParam]=null; 
				} 
				this.head=this.head.next; 
				this.processDownloads(); 
			} 
		} 
		this.swapBuffer=function(image, x, y)
		{ 
			image.buffer.id=image.id; 
			image.buffer.name=image.name; 
			image.buffer.border=image.border; 
			image.buffer.width=image.width; 
			image.buffer.height=image.height; 
			image.buffer.style.position=image.style.position; 
			image.buffer.style.left=image.style.left; 
			image.buffer.style.top=image.style.top; 
			image.buffer.style.width=image.style.width;
			image.buffer.style.height=image.style.height;
			image.buffer.galleryimg=image.galleryimg;
			image.buffer.i=image.i; 
			image.buffer.j=image.j; 
			image.buffer.imageMapUrl=image.imageMapUrl;
 	  if (image.imageMapUrl !=null)
			{ 
				var thisManager=this.gridSectionManager;
				image.buffer.onmouseenter=function()
				{ 
					if (thisManager.findImageMap(this.name+"Map")==-1) 
		
					{ 
						thisManager.addImageMap(this); 
					} 
				} 
				image.buffer.onmouseleave=function()
				{ 
					var index=thisManager.findImageMap(this.name+"Map"); 
					if (index !=-1)
					{ 
						this.useMap=null; 
						thisManager.viewport.removeChild(thisManager.viewport.childNodes[index]); 
					} 
				} 
				image.buffer.onmousemove=function()
				{ 
					if (this.imageMapText !=null)
					{ 
						if (thisManager.findImageMap(this.name+"Map")==-1)
						{ 
							thisManager.addImageMap(this); 
						} 
					} 
				} 
			} 
			if (this.gridSectionManager.gridSections[x] !=null && 
					this.gridSectionManager.gridSections[x][y]==image)
			{ 
				this.gridSectionManager.gridSections[x][y]=image.buffer; 
				image.buffer.swapNode(image); 
			} 
		} 
	} 
	,
ListNode:function(obj, gridXParam, gridYParam, zoom, gridSectionManager)
	{ 
		this.value=obj; 
		this.gridXParam=gridXParam; 
		this.gridYParam=gridYParam; 
		this.zoom=zoom; 
		this.gridSectionManager=gridSectionManager; 
		this.next=null; 
		this.containsVisibleSections=function()
		{ 
			var xParams=this.gridXParam.substr(0, this.gridXParam.length-1).split(";"); 
			var yParams=this.gridYParam.substr(0, this.gridYParam.length-1).split(";"); 
			for (var i=0; 
			i < xParams.length; 
			i++)
			{ 
				var x=parseInt(xParams[i]); 
				var y=parseInt(yParams[i]); 
				if (this.gridSectionManager.currentVisibleBounds.contains(x, y))
				{ 
					return true; 
				} 
			} 
			return false; 
		} 
		this.isVisible=function()
		{ 
			return (this.zoom==this.gridSectionManager.getZoom() &&
			this.gridSectionManager.currentVisibleBounds.contains(gridXParam, gridYParam)); 
		} 
		this.abortRequest=function()
		{ 
			if (this.value.readyState < 2)
			{ 
				this.value.abort(); 
				this.value=null; 
				this.removeGridSections(); 
			} 
		} 
		this.removeGridSections=function()
		{ 
			var xParams=this.gridXParam.substr(0, this.gridXParam.length-1).split(";"); 
			var yParams=this.gridYParam.substr(0, this.gridYParam.length-1).split(";"); 
			for (var i=0; 
			i < xParams.length; 
			i++)
			{ 
				var x=parseInt(xParams[i]); 
				var y=parseInt(yParams[i]); 
				if (this.gridSectionManager.gridSections[x] !=null && this.gridSectionManager.gridSections[x][y] !=null)
				{ 
					this.gridSectionManager.gridSections[x][y].parentNode.removeChild(
					this.gridSectionManager.gridSections[x][y]); 
					this.gridSectionManager.gridSections[x][y]=null; 
				} 
			} 
		} 
	} 
};