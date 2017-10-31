using Infragistics.Win.UltraWinTree;
using Krista.FM.ServerLibrary;
using Krista.FM.Utils.DTSGenerator.SMOObjects;
using Krista.FM.Utils.DTSGenerator.TreeObjects;

namespace Krista.FM.Utils.DTSGenerator.TreeObjects
{
    public class SSISPackageNode : SSISMajorTreeBase<SSISPackageObject>
    {
        public SSISPackageNode(SSISPackageObject controlObject, SSISSMOPackage smoObject) 
            : base(controlObject, smoObject)
        {
            this.Override.NodeStyle = NodeStyle.CheckBox;

            this.Text = ((IPackage) controlObject.ControlObject).Name;

            this.LeftImages.Add(Properties.Resources.Folder);
        }
    }
}