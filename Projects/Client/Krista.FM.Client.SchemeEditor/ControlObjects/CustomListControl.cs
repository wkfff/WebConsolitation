using System;
using System.Windows.Forms;
using Infragistics.Win.UltraWinTree;
using Krista.FM.ServerLibrary;
using System.Collections;
using System.Collections.Generic;


namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    public abstract class CustomListControl<TIntf, TItem> : CustomTreeNodeControl<TIntf>
    {
        public CustomListControl(string name, string text, TIntf controlObject, CustomTreeNodeControl parent, int imageIndex)
            : base(name, text, controlObject, parent, imageIndex)
        {
            AddLoadNode();
        }

        /// <summary>
        /// Очищает коллекцию узлов вызывая у каждого Dispose
        /// </summary>
        protected void DisposeAndClearNodes()
        {
            foreach (CustomTreeNodeControl item in Nodes)
                item.Dispose();
            if (!Disposed)
                Nodes.Clear();
        }

        protected override void OnDispose()
        {
            DisposeAndClearNodes();

            base.OnDispose();
        }

        protected void StartExpand()
        {
            if (SchemeEditor != null)
            {
                SchemeEditor.Operation.Text = "Загрузка...";
                SchemeEditor.Operation.StartOperation();
            }
        }

        protected void EndExpand()
        {
            if (SchemeEditor != null)
                SchemeEditor.Operation.StopOperation();
        }

        public virtual CustomTreeNodeControl Create(TItem item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected override void ExpandNode()
        {
            try
            {
                StartExpand();

                base.ExpandNode();

                List<CustomTreeNodeControl> list = new List<CustomTreeNodeControl>();
                IEnumerable levels = (IEnumerable)ControlObject;

                foreach (object item in levels)
                {
                    if (item is TItem)
                        list.Add(Create((TItem)item));
                    else
                    {
                        KeyValuePair<string, TItem> kvp = (KeyValuePair<string, TItem>)item;
                        list.Add(Create(kvp.Value));
                    }
                }

                Nodes.AddRange(list.ToArray());
            }
            finally
            {
                EndExpand();
            }
        }
    }
}
