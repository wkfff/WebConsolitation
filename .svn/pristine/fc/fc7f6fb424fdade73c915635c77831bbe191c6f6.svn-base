using Infragistics.Win.UltraWinTree;
using Krista.FM.ServerLibrary;
using Krista.FM.Utils.DTSGenerator.SMOObjects;
using Krista.FM.Utils.DTSGenerator.TreeObjects;
using Resources = Krista.FM.Utils.DTSGenerator.Properties.Resources;

namespace Krista.FM.Utils.DTSGenerator.TreeObjects
{
    public class SSISAttributeNode : SSISMajorTreeBase<SSISAttributeObject>
    {
        public SSISAttributeNode(SSISAttributeObject controlObject, SSISSMO smoObject) 
            : base(controlObject, smoObject)
        {
            this.Override.NodeStyle = NodeStyle.CheckBox;

            this.Text = ((IDataAttribute)controlObject.ControlObject).Name;

            this.LeftImages.Add(Resources.Attribute);
        }
    }
}