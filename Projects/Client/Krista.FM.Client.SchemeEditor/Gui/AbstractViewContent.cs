using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Krista.FM.Client.SchemeEditor.Gui
{
    public abstract class AbstractViewContent : AbstractBaseViewContent, IViewContent
    {
        private string untitledName = String.Empty;
        private string titleName = null;
        private string fileName = null;

        bool isViewOnly = false;


        public AbstractViewContent()
        {
        }

        public AbstractViewContent(string titleName)
        {
            this.titleName = titleName;
        }

        public AbstractViewContent(string titleName, string fileName)
        {
            this.titleName = titleName;
            this.fileName = fileName;
        }

        protected void SetTitleAndFileName(string fileName)
        {
            TitleName = Path.GetFileName(fileName);
            FileName = fileName;
            IsDirty = false;
        }

        public event EventHandler FileNameChanged;

        protected virtual void OnFileNameChanged(EventArgs e)
        {
            if (FileNameChanged != null)
            {
                FileNameChanged(this, e);
            }
        }

        #region IViewContent implementation
        public virtual string UntitledName
        {
            get { return untitledName; }
            set { untitledName = value; }
        }

        public virtual string TitleName
        {
            get { return titleName; }
            set 
            { 
                titleName = value;
                OnTitleNameChanged(EventArgs.Empty);
            }
        }

        public virtual string FileName
        {
            get { return fileName; }
            set
            {
                fileName = value;
                OnFileNameChanged(EventArgs.Empty);
            }
        }

        public virtual bool IsUntitled
        {
            get { return titleName == null; }
        }

        public virtual bool IsReadOnly
        {
            get { return false; }
        }

        public virtual bool IsViewOnly
        {
            get { return isViewOnly; }
            set { isViewOnly = value; }
        }

        public virtual void Save()
        {
            if (IsDirty)
            {
                Save(fileName);
            }
        }

        public virtual void Save(string fileName)
        {
            throw new System.NotImplementedException();
        }

        public virtual void Load(string fileName)
        {
            throw new System.NotImplementedException();
        }

        public event EventHandler TitleNameChanged;
        public event EventHandler Saving;
        public event SaveEventHandler Saved;

        protected virtual void OnTitleNameChanged(EventArgs e)
        {
            if (TitleNameChanged != null)
            {
                TitleNameChanged(this, e);
            }
        }

        protected virtual void OnSaving(EventArgs e)
        {
            if (Saving != null)
            {
                Saving(this, e);
            }
        }

        protected virtual void OnSaved(SaveEventArgs e)
        {
            if (Saved != null)
            {
                Saved(this, e);
            }
        }

        #region ICanBeDirty implementation
        public virtual bool IsDirty
        {
            get { return isDirty; }
            set
            {
                if (isDirty != value)
                {
                    isDirty = value;
                    OnDirtyChanged(EventArgs.Empty);
                }
            }
        }
        bool isDirty = false;

        public event EventHandler DirtyChanged;

        protected virtual void OnDirtyChanged(EventArgs e)
        {
            if (DirtyChanged != null)
            {
                DirtyChanged(this, e);
            }
        }
        #endregion
        #endregion
    }
}
