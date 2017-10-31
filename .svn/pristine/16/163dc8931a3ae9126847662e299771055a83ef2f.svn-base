using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Xml;
using Infragistics.Win.UltraWinTree;
using Krista.FM.Common.Xml;

namespace Krista.FM.Client.MDXExpert.Data
{
    public partial class PivotDataElementEditorCtrl : UserControl
    {
        public PivotDataElementEditorCtrl()
        {
            InitializeComponent();
        }

        private int cnt = 0;

        private IWindowsFormsEditorService edSvc = null;


        private void AddPivotDataNodes(UltraTreeNode root, XmlNode xmlNode)
        {
            UltraTreeNode node = root;
            string key;
            string caption;

            

            while (xmlNode != null)
            {
                cnt++;
                key = "";
                caption = "";

                switch (xmlNode.Name)
                {
                    case "axis":
                        key = "axis " + XmlHelper.GetStringAttrValue(xmlNode, "type", "");
                        break;

                    case "fieldset":
                        key = "fieldset " + XmlHelper.GetStringAttrValue(xmlNode, "uname", "");
                        break;

                    case "member":
                        key = XmlHelper.GetStringAttrValue(xmlNode, "uname", "");
                        break;

                    case "uname":
                        key = xmlNode.InnerText;
                        break;

                    default:
                        key = xmlNode.Name + cnt.ToString();
                        caption = xmlNode.Name;
                        break;

                }

                if (caption == "")
                {
                    caption = key;
                }

                if (xmlNode.Name != "dummy")
                {
                    if (root == null)
                    {
                        node = tvPivotData.Nodes.Add(key, caption);
                    }
                    else
                    {
                        node = root.Nodes.Add(key, caption);
                    }
                }

                if (xmlNode.HasChildNodes)
                {
                    AddPivotDataNodes(node, xmlNode.FirstChild);
                }

                xmlNode = xmlNode.NextSibling;
            }
        }

        /// <summary>
        /// приклепляем итоги
        /// </summary>
        /// <param name="parentNode">родительский узел</param>
        /// <param name="totals">итоги</param>
        private void LoadTotals(UltraTreeNode parentNode, List<PivotTotal> totals)
        {
            UltraTreeNode totalNode;
            foreach (PivotTotal total in totals)
            {
                totalNode = parentNode.Nodes.Add(total.UniqueName, total.Caption);
                totalNode.Tag = total;
            }
        }

        /// <summary>
        /// прикрепляем иерархии
        /// </summary>
        /// <param name="parentNode">родительский узел</param>
        /// <param name="fieldSets">набор иерархий</param>
        private void LoadFieldsets(UltraTreeNode parentNode, FieldSetCollection fieldSets)
        {
            UltraTreeNode fsNode;

            foreach (FieldSet fs in fieldSets)
            {
                fsNode = parentNode.Nodes.Add(fs.UniqueName, fs.Caption);
                fsNode.Tag = fs;
                LoadFields(fsNode, fs.Fields);
            }
        }

        /// <summary>
        /// прикрепляем поля
        /// </summary>
        /// <param name="parentNode">родительский узел</param>
        /// <param name="fields">поля</param>
        private void LoadFields(UltraTreeNode parentNode, List<PivotField> fields)
        {
            UltraTreeNode fieldNode;

            foreach (PivotField f in fields)
            {
                fieldNode = parentNode.Nodes.Add(f.UniqueName, f.Caption);
                fieldNode.Tag = f;
            }
        }

        /// <summary>
        /// прикрепляем ось
        /// </summary>
        /// <param name="axis">ось</param>
        private void LoadAxis(Axis axis)
        {
            UltraTreeNode axisNode; 
 
            if (axis.AxisType == AxisType.atTotals)
            {
                if (((TotalAxis)axis).Totals.Count > 0)
                {
                    axisNode = tvPivotData.Nodes.Add("axis "+ axis.AxisType.ToString(), axis.Caption);
                    axisNode.Tag = axis;
                    LoadTotals(axisNode, ((TotalAxis)axis).Totals);
                }
            }
            else
            {
                if (((PivotAxis)axis).FieldSets.Count > 0)
                {
                    axisNode = tvPivotData.Nodes.Add("axis " + axis.AxisType.ToString(), axis.Caption);
                    axisNode.Tag = axis;
                    LoadFieldsets(axisNode, ((PivotAxis)axis).FieldSets);
                }
            }

        }

        /// <summary>
        /// Загружаем PivotData в редактор
        /// </summary>
        /// <param name="pivotData"></param>
        public void LoadPivotData(Client.MDXExpert.Data.PivotData pivotData)
        {
            tvPivotData.Nodes.Clear();

            foreach (Axis axis in pivotData.Axes)
            {
                LoadAxis(axis);
            }
        }

        public void SetEditorService(IWindowsFormsEditorService value)
        {
            edSvc = value;
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            if (edSvc != null)
            {
                edSvc.CloseDropDown();
            }
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            if (edSvc != null)
            {
                edSvc.CloseDropDown();
            }
        }

    }

    
    public class PivotObjectConverter : System.ComponentModel.TypeConverter // ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            PivotObject pObj = (PivotObject)value;
            string result = String.Empty;

            switch (pObj.ObjectType)
            { 
                case PivotObjectType.poAxis:
                    result = ((Axis)pObj).Caption;
                    break;
                case PivotObjectType.poField:
                    result = ((PivotField)pObj).Caption;
                    break;
                case PivotObjectType.poFieldSet:
                    result = ((FieldSet)pObj).Caption;
                    break;
                case PivotObjectType.poTotal:
                    result = ((PivotTotal)pObj).Caption;
                    break;
                default:
                    result = "(нет выбранных элементов)";
                    break;
            }

            return result;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            return ((PivotObject)value);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false; 
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false; 
        }

        public override PropertyDescriptorCollection  GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            PivotObject pObj = (PivotObject)value;

            switch (pObj.ObjectType)
            {
                case PivotObjectType.poAxis:
                    return TypeDescriptor.GetProperties(typeof(Axis), attributes).Sort(
                      new string[] { "Caption" });
                case PivotObjectType.poField:
                    return TypeDescriptor.GetProperties(typeof(PivotField), attributes).Sort(
                      new string[] { "Caption", "UniqueName" });
                case PivotObjectType.poFieldSet:
                    return TypeDescriptor.GetProperties(typeof(FieldSet), attributes).Sort(
                      new string[] { "Caption", "UniqueName" });
                case PivotObjectType.poTotal:
                    return TypeDescriptor.GetProperties(typeof(PivotTotal), attributes).Sort(
                      new string[] { "Caption", "UniqueName" });
                default:
                    return null;
            }
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }


    }

    public class PivotDataElementEditor : System.Drawing.Design.UITypeEditor
    {
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return System.Drawing.Design.UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            PivotDataElementEditorCtrl myValues = new PivotDataElementEditorCtrl();

            myValues.LoadPivotData(((PivotObject)value).ParentPivotData);

            myValues.SetEditorService(edSvc);

            edSvc.DropDownControl(myValues);

            if (myValues.tvPivotData.SelectedNodes.Count > 0)
            {
                return (new PivotObjectConverter()).ConvertFrom(myValues.tvPivotData.SelectedNodes[0].Tag);
            }
            else
            {
                return value;
            }
        }
    }

}
