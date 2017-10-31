using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.Client.Common.Gui;

namespace Krista.FM.Client.Workplace.Gui
{
    public class PaneDescriptor : IDisposable
    {
        private string @class;
        private string title;
        private string icon;
        private string category;
        private string shortcut;

        private Type padType;

        private IPaneContent padContent;
        private bool padContentCreated;

        public PaneDescriptor(Type padType, string title, string icon)
		{
			this.padType = padType;
			this.@class = padType.FullName;
			this.title = title;
			this.icon = icon;
			this.category = "none";
			this.shortcut = "";
		}

        public string Title
        {
            get { return title; }
        }

        public string Icon
        {
            get { return icon; }
        }

        public string Category
        {
            get { return category; }
            set 
            { 
                if (value == null)
                    throw new ArgumentNullException("value");
                category = value;
            }
        }

        public string Shortcut
        {
            get { return shortcut; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                shortcut = value;
            }
        }

        public string Class
        {
            get { return @class; }
        }

        public bool HasFocus
        {
            get
            {
                return (padContent != null) ? padContent.Control.ContainsFocus : false;
            }
        }

        public IPaneContent PadContent
        {
            get
            {
                CreatePad();
                return padContent;
            }
        }

        public void Dispose()
        {
            if (padContent != null)
            {
                padContent.Dispose();
                padContent = null;
            }
        }

        public void RedrawContent()
        {
            if (padContent != null)
            {
                padContent.RedrawContent();
            }
        }

        public void CreatePad()
        {
            if (!padContentCreated)
            {
                padContentCreated = true;
                padContent = (IPaneContent)Activator.CreateInstance(padType);
            }
        }

        public void BringPadToFront()
        {
            CreatePad();
            if (padContent == null) 
                return;

            if (!WorkplaceSingleton.Workplace.WorkplaceLayout.IsVisible(this))
            {
                WorkplaceSingleton.Workplace.WorkplaceLayout.ShowPad(this);
            }
            WorkplaceSingleton.Workplace.WorkplaceLayout.ActivatePad(this);
        }
    }
}
