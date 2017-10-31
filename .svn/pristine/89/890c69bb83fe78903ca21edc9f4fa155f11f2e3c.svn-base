using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.Client.OLAPStructures;
using Krista.FM.Client.OLAPResources;
using Microsoft.AnalysisServices;
using CommandType=Krista.FM.Client.OLAPStructures.CommandType;

namespace Krista.FM.Client.OLAPAdmin
{
	public partial class ctrlObjectExplorer : UserControl
	{
		protected EventHandler valueChangedUserHandler;
		protected List<object> selectedObjects = new List<object>();
		protected List<object> selectedBlocks = new List<object>();

		public delegate void SetButtonsStateDelegate(bool enabled, object[] names);
        	   
		public ctrlObjectExplorer()
		{
			InitializeComponent();

            InfragisticComponentsCustomize.CustomizeUltraTabControl(ultraTabControl);
		}

       

	    public void RefreshItems(List<object> items)
		{
			objectList.RefreshItems(items);	
		}

		public void Init(List<object> items, string caption,
			EventHandler _valueChangedUserHandler,
			ctrlFiltratedList.ItemsCollectionChangedEventHandler _itemsCollectionChangedHandler,
			DrawItemEventHandler drawItemHandler)
		{
			valueChangedUserHandler = _valueChangedUserHandler;
			objectList.Init(items, caption, null, ValueChangedHandler, drawItemHandler);
			objectList.textBoxFilter.Focus();			
			objectList.ItemsCollectionChanged += _itemsCollectionChangedHandler;
		}

		private object[] ListToArray(List<object> list)
		{
			object[] array = new object[list.Count];
			list.CopyTo(array, 0);
			return array;
		}
	
		private Versions ReadVersions(AnnotationCollection annotations)
		{
			string versionsName = "versions";
			if (annotations.Contains(versionsName))
			{
				return Versions.ReadFromXML(annotations[versionsName].Value.CreateNavigator());
			}
			return null;
		}

		private void UpdateVersionInfo()
		{
			if (objectList.listBoxItems.SelectedItems.Count == 1)
			{
				OLAPObjectHeader header =
					(OLAPObjectHeader)objectList.listBoxItems.SelectedItem;
				Versions versions = null;
				if (header.NamedComponent != null)
				{
					versions = ReadVersions(header.NamedComponent.Annotations);
				}
				else
				{
					if (header.VersionedObjectInfo != null)
					{
						versions = header.VersionedObjectInfo.Versions;
					}
				}
				VersionGrid.SelectedObject = versions;
			}
			else
			{
				VersionGrid.SelectedObject = null;
			}
		}

		private void ValueChangedHandler(object sender, EventArgs e)
		{
			if (valueChangedUserHandler != null)
			{
				valueChangedUserHandler(sender, e);
			}
			if (objectList.listBoxItems.Items != null)
		    {
				selectedObjects.Clear();
				selectedBlocks.Clear();
				for (int i = 0; i < objectList.listBoxItems.SelectedItems.Count; i++)
				{
					if (objectList.listBoxItems.SelectedItems[i] is OLAPObjectHeader)
					{
						OLAPObjectHeader header =
							(OLAPObjectHeader)objectList.listBoxItems.SelectedItems[i];
						if (header.NamedComponent != null)
						{
							selectedObjects.Add(header.NamedComponent);
						}
						else
						{	
							selectedObjects.Add(header.ObjectInfo);
						}

						selectedBlocks.Add(header.ControlBlock);
					}
				}               
 
                ObjectGrid.SelectedObjects = ListToArray(selectedObjects);
				ControlGrid.SelectedObjects = ListToArray(selectedBlocks);
				UpdateVersionInfo();
		    }			
		}

        public void AddButton(string text, string name, Image image, ToolClickEventHandler onClick)
		{
            ButtonTool item = new ButtonTool(name);
			item.SharedProps.Caption = text;
			item.SharedProps.AppearancesSmall.Appearance.Image = image;
			//item.ImageScaling = ToolStripItemImageScaling.None;
			item.ToolClick +=onClick;
            item.SharedProps.DisplayStyle = ToolDisplayStyle.ImageAndText;
            ultraToolbarsManager.Tools.Add(item);

            ultraToolbarsManager.Toolbars[0].Tools.Add(item);

            ultraToolbarsManager.RefreshMerge();
		}

		public void AddSeparator()
		{			
			ToolStripItem item = new ToolStripSeparator();			
			//toolStripObjects.Items.Add(item);
		}

		public void InvokeSetButtonState(bool enabled, object[] names)
		{
			if (InvokeRequired)
			{
				Invoke(new SetButtonsStateDelegate(SetButtonsState),
					enabled, names);
			}
			else
				SetButtonsState(enabled, names);

		}

		public void SetButtonsState(bool enabled, object[] names)
		{
			for (int i = 0; i < ultraToolbarsManager.Tools.Count; i++)
			{	
				for (int j = 0; j < names.Length; j++)
				{
                    if (ultraToolbarsManager.Tools[i].Key.
						Equals(names[j].ToString(), StringComparison.OrdinalIgnoreCase))
					{
                        ultraToolbarsManager.Tools[i].SharedProps.Enabled = enabled;
					}
				}
			}
		}


	    public UltraToolbarsManager UltraToolbarsManager
	    {
	        get { return ultraToolbarsManager; }
	    }

	    public string Caption
		{
			get { return objectList.Caption; }
			set { objectList.Caption = value; }
		}


	    public ctrlFiltratedList ObjectList
	    {
	        get { return objectList; }
	    }
	}
}