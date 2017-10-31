using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

using Krista.FM.Client.MobileReports.Common;

namespace Krista.FM.Client.MobileReports.Bootloader
{
    public class ResourceInfo
    {
        #region Поля
        private int _startIndex;
        private int _lenght;
        private Uri _url;
        private string _originalValue;
        private string _name;
        private string _resourcesFolder;
        private bool _successLoaded;
        private ResourceType _resourceType;
        #endregion

        #region Свойства
        public int StartIndex
        {
            get { return _startIndex; }
            set { _startIndex = value; }
        }

        public int Lenght
        {
            get { return _lenght; }
            set { _lenght = value; }
        }

        public Uri Url
        {
            get { return _url; }
            set { _url = value; }
        }

        public string OriginalValue
        {
            get { return _originalValue; }
            set { _originalValue = value; }
        }

        public string NewValue
        {
            get { return this.ResourcesFolder + "/" + this.Name; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string ResourcesFolder
        {
            get { return _resourcesFolder; }
            set { _resourcesFolder = value; }
        }

        public bool SuccessLoaded
        {
            get { return _successLoaded; }
            set { _successLoaded = value; }
        }

        public ResourceType ResourceType
        {
            get { return _resourceType; }
            set { _resourceType = value; }
        }
        #endregion

        public ResourceInfo(Group group, Uri absoluteUrl, string resourcesFolder)
        {
            this.StartIndex = group.Index;
            this.Lenght = group.Length;
            this.OriginalValue = group.Value;
            this.Url = this.GetResourcesUrl(absoluteUrl, this.OriginalValue);
            this.Name = this.GetResourcesName(this.Url);
            this.ResourcesFolder = resourcesFolder;
            this.ResourceType = this.GetResourceType(this.OriginalValue);
        }

        public ResourceInfo(Uri absoluteUrl)
        {
            this.Url = absoluteUrl;
            this.Name = this.GetResourcesName(this.Url);
        }

        private string GetResourcesName(Uri resourcesUrl)
        {
            if (resourcesUrl != null)
                return resourcesUrl.Segments[resourcesUrl.Segments.Length - 1];
            else
                return string.Empty;
        }

        private Uri GetResourcesUrl(Uri absoluteUrl, string resourcesUrl)
        {
            Uri result = null;
            Uri.TryCreate(absoluteUrl, resourcesUrl, out result);
            return result;
        }

        private ResourceType GetResourceType(string value)
        {
            if (string.IsNullOrEmpty(value))
                return ResourceType.Other;

            string extension = Path.GetExtension(value);
            if (extension.IndexOf('?') > 0)
                extension = extension.Remove(extension.IndexOf('?'));

            if (Utils.IsExistText(extension, Consts.imageExtesions))
                return ResourceType.Image;
            else
                if (Utils.IsExistText(extension, Consts.scriptExtesions))
                    return ResourceType.Script;
                else
                    if (Utils.IsExistText(extension, Consts.cssExtesions))
                        return ResourceType.CSS;

            return ResourceType.Other;
        }
    }

    public enum ResourceType
    {
        Script,
        CSS,
        Image,
        Other
    }
}
