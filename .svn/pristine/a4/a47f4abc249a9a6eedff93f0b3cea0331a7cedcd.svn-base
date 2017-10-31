using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace Krista.FM.Client.MDXExpert.Controls
{
    public partial class ImageSelectControl : UserControl
    {
        public Image Image
        {
            get 
            {
                return ((Image)pbImage.Image); 
            }
            set 
            { 
                pbImage.Image = value;
            }
        }

        public ImageSelectControl(Image image)
        {
            InitializeComponent();
            this.Image = image;
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            if (Tag != null)
            {
                ((IWindowsFormsEditorService)Tag).CloseDropDown();
            }
            else
            {
                this.Hide();
            }
        }

        private void btClear_Click(object sender, EventArgs e)
        {
            this.Image = null;
        }

        private void btSelect_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.Image = Image.FromFile(openFileDialog.FileName);
            }
        }

    }

    public class ImageSelectEditor : UITypeEditor
    {

        /// <summary>
        /// Реализация метода редактирования
        /// </summary>
        public override Object EditValue(ITypeDescriptorContext context, IServiceProvider provider, Object value)
        {
            if ((context != null) && (provider != null))
            {
                IWindowsFormsEditorService svc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (svc != null)
                {
                    ImageSelectControl flctrl = new ImageSelectControl((Image)value);
                    flctrl.Tag = svc;

                    svc.DropDownControl(flctrl);
                    value = flctrl.Image;


                }
            }

            return base.EditValue(context, provider, value); //result 
        }

        /// <summary>
        /// Возвращаем стиль редактора - выпадающее окно
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null)
            {
                return UITypeEditorEditStyle.DropDown;
            }
            else
            {
                return base.GetEditStyle(context);
            }
        }

    }

}
