﻿using System;

using Infragistics.Win.UltraWinTree;

namespace Krista.FM.Client.SchemeEditor
{
	/// <summary>
	/// Enumerates the possiblt Drop States.
	/// </summary>
	[Flags]
	public enum DropLinePositionEnum
	{
		None = 0,
		OnNode = 1,
		AboveNode = 2,
		BelowNode = 4,
		All = OnNode | AboveNode | BelowNode
	}

	/// <summary>
	/// This class has to implement the DrawFilter Interface
	/// so it can be used as a DrawFilter by the tree
	/// </summary>
	public class ObjectsTreeViewDropHightLightDrawFilter : Infragistics.Win.IUIElementDrawFilter
	{
		public event EventHandler Invalidate;
		//public delegate void InvalidateEventHandler( object sender, EventArgs e );

		public event QueryStateAllowedForNodeEventHandler QueryStateAllowedForNode;
		public delegate void QueryStateAllowedForNodeEventHandler( object sender, QueryStateAllowedForNodeEventArgs e );

		/// <summary>
		/// Used by the QueryStateAllowedForNode event.
		/// </summary>
		public class QueryStateAllowedForNodeEventArgs : EventArgs
		{
			public UltraTreeNode Node;
			public DropLinePositionEnum DropLinePosition;
			public DropLinePositionEnum StatesAllowed;
		}

		public ObjectsTreeViewDropHightLightDrawFilter()
		{
			//Initialize the properties to the defaults
			InitProperties();
		} 

		/// <summary>
		/// Initialize the properties to the defaults
		/// </summary>
		private void InitProperties()
		{
			mvarDropHighLightNode = null;
			mvarDropLinePosition = DropLinePositionEnum.None;
			mvarDropHighLightBackColor = System.Drawing.SystemColors.Highlight;
			mvarDropHighLightForeColor = System.Drawing.SystemColors.HighlightText;
			mvarDropLineColor = System.Drawing.SystemColors.ControlText;
			mvarEdgeSensitivity = 0;
			mvarDropLineWidth = 2;
		}


		/// <summary>
		/// Clean up
		/// </summary>
		public void Dispose()
		{
			mvarDropHighLightNode = null;
		}

		private UltraTreeNode mvarDropHighLightNode;
		/// <summary>
		/// The DropHighLightNode is a reference to the node the
		/// Mouse is currently over
		/// </summary>
		public UltraTreeNode DropHightLightNode
		{
			get
			{
				return mvarDropHighLightNode;
			}
			set
			{
				//If the Node is being set to the same value,
				// just exit
				if (mvarDropHighLightNode.Equals(value))
				{	
					return;
				}
				mvarDropHighLightNode = value;
				//The DropNode has changed.
				PositionChanged();
			}
		}

		private DropLinePositionEnum mvarDropLinePosition;
		public DropLinePositionEnum DropLinePosition
		{
			get
			{
				return mvarDropLinePosition;
			}
			set
			{
				//If the position is the same as it was, 
				//just exit
				if (mvarDropLinePosition == value)
				{
					return;
				}
				mvarDropLinePosition = value;
				//The Drop Position has changed
				PositionChanged();
			}
		}

		
		private int mvarDropLineWidth;
		/// <summary>
		/// The width of the DropLine
		/// </summary>
		public int DropLineWidth
		{
			get
			{
				return mvarDropLineWidth;
			}
			set
			{
				mvarDropLineWidth = value;
			}
		}

		private System.Drawing.Color mvarDropHighLightBackColor; 
		/// <summary>
		/// The BackColor of the DropHighLight node
		/// This only affect the node when it is being dropped On. 
		/// Not Above or Below. 
		/// </summary>
		public System.Drawing.Color DropHighLightBackColor
		{
			get
			{
				return mvarDropHighLightBackColor;
			}
			set
			{
				mvarDropHighLightBackColor = value;
			}
		}

		//The ForeColor of the DropHighLight node
		//This only affect the node when it is being dropped On. 
		//Not Above or Below. 
		private System.Drawing.Color  mvarDropHighLightForeColor;
		public System.Drawing.Color  DropHighLightForeColor
		{
			get
			{
				return mvarDropHighLightForeColor;
			}
			set
			{
				mvarDropHighLightForeColor = value;
			}
		}

		//The color of the DropLine
		private System.Drawing.Color  mvarDropLineColor ;
		public System.Drawing.Color DropLineColor
		{
			get
			{
				return mvarDropLineColor;
			}
			set
			{
				mvarDropLineColor = value;
			}
		}

		//Determines how close to the top or bottom edge of a node
		//the mouse must be to be consider dropping Above or Below
		//respectively. 
		//By default the top 1/3 of the node is Above, the bottom 1/3
		//is Below, and the middle is On. 
		private int mvarEdgeSensitivity ;
		public int EdgeSensitivity
		{
			get
			{
				return mvarEdgeSensitivity;
			}
			set
			{
				mvarEdgeSensitivity = value;
			}
		}

		//When the DropNode or DropPosition change, we fire the
		//Invalidate event to let the program know to invalidate
		//the Tree control. 
		//This is neccessary since the DrawFilter does not have a 
		//reference to the Tree Control (although it probably could)
		private void PositionChanged()
		{
			// if nobody is listening then just return
			//
			if ( null == Invalidate)
				return;

			EventArgs e = EventArgs.Empty;
		
			Invalidate(this, e);
		}

		//Set the DropNode to Nothing and the position to None. This
		//Will clear whatever Drophighlight is in the tree
		public void ClearDropHighlight()
		{
			SetDropHighlightNode(null, DropLinePositionEnum.None);
		}

		//Call this proc every time the DragOver event of the 
		//Tree fires. 
		//Note that the point pass in MUST be in Tree coords, not
		//form coords
		public void SetDropHighlightNode(UltraTreeNode Node, System.Drawing.Point PointInTreeCoords )
		{
			//The distance from the edge of the node used to 
			//determine whether to drop Above, Below, or On a node
			int DistanceFromEdge; 
        
			//The new DropLinePosition
			DropLinePositionEnum NewDropLinePosition;
		
			DistanceFromEdge = mvarEdgeSensitivity;
			//Check to see if DistanceFromEdge is 0
			if (DistanceFromEdge == 0)
			{
				//It is, so we use the default value - one third. 
				DistanceFromEdge = Node.Bounds.Height / 3;
			}

			//Determine which part of the node the point is in
			if (PointInTreeCoords.Y < (Node.Bounds.Top + DistanceFromEdge))
			{
				//Point is in the top of the node
				NewDropLinePosition = DropLinePositionEnum.AboveNode;
			}
			else
			{
				if (PointInTreeCoords.Y > (Node.Bounds.Bottom - DistanceFromEdge))
				{
					//Point is in the bottom of the node
					NewDropLinePosition = DropLinePositionEnum.BelowNode;
				}
				else
				{
					//Point is in the middle of the node
					NewDropLinePosition = DropLinePositionEnum.OnNode;
				}
			}

			//Now that we have the new DropLinePosition, call the
			//real proc to get things rolling
			SetDropHighlightNode(Node, NewDropLinePosition);
		}

		private void SetDropHighlightNode(UltraTreeNode node, DropLinePositionEnum dropLinePosition)
		{
			//Use to store whether there have been any changes in 
			//DropNode or DropLinePosition
			bool IsPositionChanged = false;

			try	
			{
				//Check to see if the nodes are equal and if 
				//the dropline position are equal
				if (mvarDropHighLightNode != null && mvarDropHighLightNode.Equals(node) && (mvarDropLinePosition == dropLinePosition))
				{
					//They are both equal. Nothing has changed. 
					IsPositionChanged = false;
				}
				else	
				{
					//One or both have changed
					IsPositionChanged = true;
				}
			}
			catch 
			{
				//If we reach this code, it means mvarDropHighLightNode 
				//is null, so it could not be compared
				if (mvarDropHighLightNode == null)
				{
					//Check to see if Node is nothing, so we//ll know
					//if Node = mvarDropHighLightNode
					IsPositionChanged = !(node == null);
				}
			}

			//Set both properties without calling PositionChanged
			mvarDropHighLightNode = node;
			mvarDropLinePosition = dropLinePosition;

			//Check to see if the PositionChanged
			if (IsPositionChanged)
			{
				//Position did change.
				PositionChanged();
			}
		}

		//Only need to trap for 2 phases:
		//AfterDrawElement: for drawing the DropLine
		//BeforeDrawElement: for drawing the DropHighlight
		Infragistics.Win.DrawPhase Infragistics.Win.IUIElementDrawFilter.GetPhasesToFilter(ref Infragistics.Win.UIElementDrawParams drawParams) 
		{
			return Infragistics.Win.DrawPhase.AfterDrawElement | Infragistics.Win.DrawPhase.BeforeDrawElement;
		}

		//The actual drawing code
		bool Infragistics.Win.IUIElementDrawFilter.DrawElement(Infragistics.Win.DrawPhase drawPhase, ref Infragistics.Win.UIElementDrawParams drawParams)
		{
			Infragistics.Win.UIElement aUIElement;
			System.Drawing.Graphics g;
			UltraTreeNode aNode;

			//If there//s no DropHighlight node or no position
			//specified, then we don//t need to draw anything special. 
			//Just exit Function
			if ((mvarDropHighLightNode == null) || (mvarDropLinePosition == DropLinePositionEnum.None))
			{
				return false;
			}

			//Create a new QueryStateAllowedForNodeEventArgs object
			//to pass to the event
			QueryStateAllowedForNodeEventArgs eArgs = new QueryStateAllowedForNodeEventArgs();

			//Initialize the object with the correct info
			eArgs.Node = mvarDropHighLightNode;
			eArgs.DropLinePosition = mvarDropLinePosition;

			//Default to all states allowed. 
			eArgs.StatesAllowed = DropLinePositionEnum.All;

			//Raise the event
			QueryStateAllowedForNode(this, eArgs);

			//Check to see if the user allowed the current state
			//for this node. If not, exit function
			if ((eArgs.StatesAllowed & mvarDropLinePosition) != mvarDropLinePosition)
			{
				return false;
			}

			//Get the element being drawn
			aUIElement = drawParams.Element;

			//Determine which drawing phase we are in. 
			switch (drawPhase)
			{
				case Infragistics.Win.DrawPhase.BeforeDrawElement:
				{
					//We are in BeforeDrawElement, so we are only concerned with 
					//drawing the OnNode state. 
					if ((mvarDropLinePosition & DropLinePositionEnum.OnNode) == DropLinePositionEnum.OnNode)
					{
						//Check to see if we are drawing a NodeTextUIElement
						if (aUIElement.GetType() == typeof(NodeTextUIElement))
						{
							//Get a reference to the node that this
							//NodeTextUIElement is associated with
							aNode = (UltraTreeNode)aUIElement.GetContext(typeof(UltraTreeNode));

							//See if this is the DropHighlightNode
							if (aNode.Equals(mvarDropHighLightNode))
							{
								//Set the ForeColor and Backcolor of the node 
								//to the DropHighlight colors 
								//Note that AppearanceData only affects the
								//node for this one paint. It will not
								//change any properties of the node
								drawParams.AppearanceData.BackColor = mvarDropHighLightBackColor;
								drawParams.AppearanceData.ForeColor = mvarDropHighLightForeColor;
							}
						}
					}
					break;
				}
				case Infragistics.Win.DrawPhase.AfterDrawElement:
				{
					//We're in AfterDrawElement
					//So the only states we are conderned with are
					//Below and Above
					//Check to see if we are drawing the Tree Element
					if (aUIElement.GetType() == typeof(UltraTreeUIElement))
					{
						//Declare a pen to us for drawing Droplines
						System.Drawing.Pen p = new System.Drawing.Pen(mvarDropLineColor, mvarDropLineWidth);

						//Get a reference to the Graphics object
						//we are drawing to. 
						g = drawParams.Graphics;

						//Get the NodeSelectableAreaUIElement for the 
						//current DropNode. We will use this for
						//positioning and sizing the DropLine
						NodeSelectableAreaUIElement tElement ;
						tElement = (NodeSelectableAreaUIElement)drawParams.Element.GetDescendant(typeof(NodeSelectableAreaUIElement), mvarDropHighLightNode);

                        // тут иногда вылетает дерево красным крестом
                        if (tElement == null)
                            return false;

						//The left edge of the DropLine

                        // Тут вылетает с красным крестом иногда
						int LeftEdge = tElement.Rect.Left - 4;

						//We need a reference to the control to 
						//determine the right edge of the line
						UltraTree aTree; 
						aTree = (UltraTree)tElement.GetContext(typeof(UltraTree));
						int RightEdge = aTree.DisplayRectangle.Right -4;

						//Used to store the Vertical position of the 
						//DropLine
						int LineVPosition;

						if ((mvarDropLinePosition & DropLinePositionEnum.AboveNode) == DropLinePositionEnum.AboveNode)
						{
							//Draw line above node
							LineVPosition = mvarDropHighLightNode.Bounds.Top;
							g.DrawLine(p, LeftEdge, LineVPosition, RightEdge, LineVPosition);
							p.Width = 1;
							g.DrawLine(p, LeftEdge, LineVPosition - 3, LeftEdge, LineVPosition + 2);
							g.DrawLine(p, LeftEdge + 1, LineVPosition - 2, LeftEdge + 1, LineVPosition + 1);
							g.DrawLine(p, RightEdge, LineVPosition - 3, RightEdge, LineVPosition + 2);
							g.DrawLine(p, RightEdge - 1, LineVPosition - 2, RightEdge - 1, LineVPosition + 1);
						}
						if ((mvarDropLinePosition & DropLinePositionEnum.BelowNode) == DropLinePositionEnum.BelowNode)
						{
							//Draw Line below node
							LineVPosition = mvarDropHighLightNode.Bounds.Bottom;
							g.DrawLine(p, LeftEdge, LineVPosition, RightEdge, LineVPosition);
							p.Width = 1;
							g.DrawLine(p, LeftEdge, LineVPosition - 3, LeftEdge, LineVPosition + 2);
							g.DrawLine(p, LeftEdge + 1, LineVPosition - 2, LeftEdge + 1, LineVPosition + 1);
							g.DrawLine(p, RightEdge, LineVPosition - 3, RightEdge, LineVPosition + 2);
							g.DrawLine(p, RightEdge - 1, LineVPosition - 2, RightEdge - 1, LineVPosition + 1);
						}
					}
					break;
				}				
			}
			return false;
		}
	}
}
