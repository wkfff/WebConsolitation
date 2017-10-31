using Infragistics.Win.UltraWinTree;
using Krista.FM.ServerLibrary;
using Krista.FM.Utils.DTSGenerator.SMOObjects;
using Krista.FM.Utils.DTSGenerator.TreeObjects;

namespace Krista.FM.Utils.DTSGenerator.TreeObjects
{
    public class SSISEntitiesNode : SSISMajorTreeBase<SSISEntitiesObject>
    {
        public SSISEntitiesNode(SSISEntitiesObject controlObject, SSISSMO smoObject) 
            : base(controlObject, smoObject)
        {
            this.Override.NodeStyle = NodeStyle.CheckBox;

            this.Text = ((IEntity) controlObject.ControlObject).FullCaption;

            DefineLeftIcon(controlObject);
        }

        private void DefineLeftIcon(SSISEntitiesObject controlObject)
        {
            switch(((IEntity)controlObject.ControlObject).ClassType)
            {
                case ClassTypes.clsFixedClassifier:
                    this.LeftImages.Add(Properties.Resources.ClassViolet);
                    break;

                case ClassTypes.clsFactData:
                    this.LeftImages.Add(Properties.Resources.ClassBlue);
                    break;

                case ClassTypes.clsDataClassifier:
                    this.LeftImages.Add(Properties.Resources.ClassYellow);
                    break;

                case ClassTypes.clsBridgeClassifier:
                    this.LeftImages.Add(Properties.Resources.ClassGreen);
                    break;

                case ClassTypes.Table:
                    this.LeftImages.Add(Properties.Resources.Class);
                    break;
            }
        }
    }
}