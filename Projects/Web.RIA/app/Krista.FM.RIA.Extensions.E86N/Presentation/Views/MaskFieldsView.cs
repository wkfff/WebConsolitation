using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.ViewModel;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Views
{
    public struct BtnMenuItems
    {
        public string Action;
        
        public string Caption;
        
        public string Ico;

        public bool Import;
    }
    
    public class MaskFieldsView : View
    {
        private readonly IAuthService auth;

        public MaskFieldsView()
        {
            MaskRe = new Dictionary<string, string>();
            FieldsWidth = new Dictionary<string, int>();
            StoreListeners = new Dictionary<string, string>();
            HideFields = new List<string>();
            ImpExps = new List<BtnMenuItems>();
            auth = Resolver.Get<IAuthService>();
            UseHierarchy = true;
        }

        public IEntity Entity { get; set; }

        public bool Readonly { get; set; }

        /// <summary>
        /// Определяет будет ли справочник отображаться как иерархический
        /// </summary>
        public bool UseHierarchy { get; set; }
        
        public List<BtnMenuItems> ImpExps { get; set; }

        public ViewPage Page { get; set; }

        public IViewService ViewService { get; set; }

        public Dictionary<string, string> StoreListeners { get; set; }

        /// <summary>
        /// Ширина полей
        /// </summary>
        public Dictionary<string, int> FieldsWidth { get; set; }

        /// <summary>
        /// Скрываемые поля todo в дизайнере есть признак Visible может его использовать вместо атрибута надо пробовать?
        /// </summary>
        public List<string> HideFields { get; set; }
        
        /// <summary>
        /// Маска ввода
        /// </summary>
        public Dictionary<string, string> MaskRe { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            ResourceManager.GetInstance(page).RegisterClientScriptBlock("MaskBuilder", Resource.CodeMaskBuilder);

            Page = page;

            var gridView = new GridModelControl
                               {
                                   Entity = Entity,
                                   Readonly = auth.IsSpectator() || Readonly,
                                   Id = "gridView{0}".FormatWith(Entity.FullDBName),
                                   Title = Entity.FullCaption,
                                   ViewService = ViewService
                               };

            // если есть сервайс то регистрируем у него скрипт
            if (ViewService != null)
            {
                ResourceManager.GetInstance(page).RegisterOnReadyScript(
                    ViewService.GetClientScript());
            }

            if (UseHierarchy)
            {
                // приводим к типу "классификатор"
                var classifier = Entity as IClassifier;

                // если приведение успешно
                if (classifier != null)
                {
                    // если классификатор иерархичный
                    if (classifier.Levels.HierarchyType == HierarchyType.ParentChild)
                    {
                        gridView.ParentId = "PARENTID";
                    }
                }
            }

            List<Component> gridCompList = gridView.Build(page);

            var gridStore = page.FindControl(gridView.Id + "Store") as Store;

            if (gridStore != null)
            {
                // Устанавливаем обработчики событий
                foreach (var storeListener in StoreListeners)
                {
                    gridStore.Listeners.AddListerer(storeListener.Key, storeListener.Value);
                }
            }

            // маска отображения
            Entity.Attributes.Values.Where(x => x.Mask.IsNotNullOrEmpty()).Each(
                x =>
                    {
                        ColumnBase column = gridView.Columns.Find(g => g.ColumnID == x.Name);
                        if (column != null)
                        {
                            column.Renderer.Handler = "return buildMask(value, '{0}', {1});".FormatWith(x.Mask, true.ToString().ToLower());
                        }
                    });

            // настраиваем ширину полей
            foreach (var item in FieldsWidth)
            {
                var column = gridView.Columns.Find(x => x.ColumnID == item.Key);
                if (column != null)
                {
                    column.Width = item.Value;
                }
            }

            // скрываем поля
            foreach (string item in HideFields)
            {
                var column = gridView.Columns.Find(x => x.ColumnID == item);
                if (column != null)
                {
                    column.Hidden = true;
                }
            }

            if (!gridView.Readonly)
            {
                // настраиваем максимальную длинну вводимых данных по дизайнеру
                Entity.Attributes.Values.Each(
                    a =>
                        {
                            if (a.Size != 0)
                            {
                                var column = gridView.Columns.Find(x => x.ColumnID == a.Name);
                                if (column != null)
                                {
                                    column.SetMaxLengthEdior(a.Size);
                                }
                            }
                        });

                // настраиваем маску ввода(regular expression)
                foreach (var item in MaskRe)
                {
                    var column = gridView.Columns.Find(x => x.ColumnID == item.Key);
                    if (column != null)
                    {
                        column.SetMaskReEdior(item.Value);
                    }
                }
            }
            
            if (ImpExps.Count != 0)
            {
                var viewport = new Viewport();
                var borderLayout = new BorderLayout();
                borderLayout.Center.Items.Add(gridCompList);
                borderLayout.North.Items.Add(CreateTopPanel());
                viewport.Items.Add(borderLayout);
                return new List<Component> { viewport };
            }

            return gridCompList;
        }

        private Panel CreateTopPanel()
        {
            var toolbar = new Toolbar();
            foreach (BtnMenuItems btn in ImpExps)
            {
                if (!(auth.IsSpectator() && btn.Import))
                {
                    var import = new UpLoadFileBtnControl
                    {
                        Id = "btnImpExp" + ImpExps.IndexOf(btn)
                                               .ToString(CultureInfo.InvariantCulture),
                        UploadController = btn.Action,
                        Name = btn.Caption,
                        Upload = btn.Import,
                        Icon = (Icon)Enum.Parse(typeof(Icon), btn.Ico)
                    };
                    toolbar.Add(import.Build(Page));
                }
            }

            return new Panel
            {
                Height = toolbar.Items.Any() ? 27 : 0,
                Border = false,
                TopBar = { toolbar },
            };
        }
    }
}
