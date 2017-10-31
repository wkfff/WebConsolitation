using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Infragistics.Win.UltraWinTabbedMdi;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinTree;
using Krista.FM.Client.DiagramEditor.Commands;
using Krista.FM.Common;
using Krista.FM.Common.Handling;
using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.Design;
using Krista.FM.Client.DiagramEditor;
using Krista.FM.Client.SchemeEditor.Commands;
using Krista.FM.Client.SchemeEditor.ControlObjects;
using Krista.FM.Client.SchemeEditor.DiargamEditor;
using Krista.FM.Client.SchemeEditor.Gui;
using Krista.FM.Client.SchemeEditor.Properties;
using Krista.FM.Client.SchemeEditor.Services;
using Krista.FM.Client.SchemeEditor.Services.NavigationService;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SchemeEditor
{
    /// <summary>
    /// Редактор схем 
    /// </summary>
    public partial class SchemeEditor : ISchemeEditor, IWorkbench, INavigation
    {

        #region Поля

        private static SchemeEditor singleInstance;

        /// <summary>
        /// Форма приложения 
        /// </summary>
        private Form mainForm;

        /// <summary>
        /// Форма приложения 
        /// </summary>
        public Form Form
        {
            get { return mainForm; }
        }

        /// <summary>
        /// Форма отображения колес
        /// </summary>
        private Operation operation;

        /// <summary>
        /// Редактируемая схема
        /// </summary>
        private IScheme scheme;

        private static Dictionary<Images, System.Drawing.Bitmap> imagesList;

        private ObjectsTreeView objectsTreeView;
        private PropertyGrid propertyGrid;
        private DeveloperDescriptionControl developerDescriptionControl;
        private Krista.FM.Client.Design.Editors.SemanticsGridControl semanticsGridControl;
        private DataSuppliersGridControl dataSuppliersGridControl;
        private DataKindsGridControl dataKindsGridControl;
        private ModificationsTreeControl modificationsTreeControl;
        private Services.SearchService.SearchTabControl searchTabControl;
        private SessionGrid sessionGridControl;
        private MacroSetControl macroSetControl;

        private Infragistics.Win.UltraWinDock.UltraDockManager dockManager;
        private Infragistics.Win.UltraWinTabbedMdi.UltraTabbedMdiManager tabbedMdiManager;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsManager toolbarManager;
        //private Infragistics.Win.UltraWinStatusBar.UltraStatusBar statusBar;
        //private static ImageList imagesList;

        #endregion Поля

        #region Конструктор

        static SchemeEditor()
        {
            imagesList = new Dictionary<Images, System.Drawing.Bitmap>(20);
            imagesList.Add(Images.Empty, Resource.Empty);
            imagesList.Add(Images.SuffixEmpty, Resource.SuffixEmpty);
            imagesList.Add(Images.SuffixLock, Resource.SuffixLock);
            imagesList.Add(Images.SuffixCheck1, Resource.SuffixCheck1);
            imagesList.Add(Images.SuffixCheck2, Resource.SuffixCheck2);
            imagesList.Add(Images.SuffixCheck3, Resource.SuffixCheck3);
            imagesList.Add(Images.SuffixUser, Resource.SuffixUser);
            imagesList.Add(Images.SuffixPlus, Resource.SuffixPlus);
            imagesList.Add(Images.SuffixMinus, Resource.SuffixMinus);
            imagesList.Add(Images.SuffixDelete, Resource.SuffixDelete);
            imagesList.Add(Images.Folders, Resource.Folders);
            imagesList.Add(Images.Folder, Resource.Folder);
            imagesList.Add(Images.Classes, Resource.Classes);
            imagesList.Add(Images.Class, Resource.Class);
            imagesList.Add(Images.ClassBlue, Resource.ClassBlue);
            imagesList.Add(Images.ClassYellow, Resource.ClassYellow);
            imagesList.Add(Images.ClassGreen, Resource.ClassGreen);
            imagesList.Add(Images.ClassViolet, Resource.ClassViolet);
            imagesList.Add(Images.File, Resource.File);
            imagesList.Add(Images.Documents, Resource.Documents);
            imagesList.Add(Images.Associasions, Resource.Associations);
            imagesList.Add(Images.Associasions2, Resource.Associations2);
            imagesList.Add(Images.Associasion, Resource.Association);
            imagesList.Add(Images.AssociasionBridge, Resource.AssociationBridge);
            imagesList.Add(Images.Hierarchy, Resource.Hierarchy);
            imagesList.Add(Images.Attributes, Resource.Attributes);
            imagesList.Add(Images.Attribute, Resource.Attribute);
            imagesList.Add(Images.AttributeKey, Resource.AttributeKey);
            imagesList.Add(Images.AttributeLock, Resource.AttributeLock);
            imagesList.Add(Images.AttributeLink, Resource.AttributeLink);
            imagesList.Add(Images.AttributeServ, Resource.AttributeServ);
            imagesList.Add(Images.Refresh, Resource.Обновить_16х16_Вариант2);
            imagesList.Add(Images.XMLConfigaration, Resource.XMLКонфигурация_16х16_Вариант1);
            imagesList.Add(Images.ViewChange, Resource.Показать_изменения_16х16_Вариант2);
            imagesList.Add(Images.ApplayChange, Resource.Применить_изменения_16х16_Вариант5);
            imagesList.Add(Images.CancelChange, Resource.Отменить_изменения_16х16_Вариант4);
            imagesList.Add(Images.Remove, Resource.Удалить__16х16_Вариант1);
            imagesList.Add(Images.Edit, Resource.Редактировать_16х16_Вариант4);
            imagesList.Add(Images.CreatePackage, Resource.Добавить_пакет_16х16_Вариант2);
            imagesList.Add(Images.CreateClass, Resource.Добавить_класс_16х16_Вариант1);
            imagesList.Add(Images.CreateAssociation, Resource.Добавить_ассоциацию_16х16_Вариант1);
            imagesList.Add(Images.CreateAttribute, Resource.Добавить_атрибут_16х16_Вариант1);
            imagesList.Add(Images.CreateDocument, Resource.Новый_документ_16х16_Вариант1);
            imagesList.Add(Images.FixedString, Resource.Фиксирование_строки_16х16_Вариант1);
            imagesList.Add(Images.AssociateMapping, Resource.Правило_формирования_16х16_Вариант1);
            imagesList.Add(Images.CreateAssociateMapping, Resource.Создать_Правило_формирования_16х16_Вариант1);
            imagesList.Add(Images.AssociateRuleMapping, Resource.Правило_сопоставления_16х16_Вариант1);
            imagesList.Add(Images.CreateAssociateRuleMapping, Resource.Создать_Правило_сопоставления_16х16_Вариант1);
            imagesList.Add(Images.Constant, Resource.Constant);
            imagesList.Add(Images.Database, Resource.Database);
            imagesList.Add(Images.Scheme, Resource.Scheme);
            imagesList.Add(Images.Properties, Resource.Properties);
            imagesList.Add(Images.PumpRegistry, Resource.PUMP_DATAPUMP_16);
            imagesList.Add(Images.AssociationMD, Resource.Добавить_ассоциациюMD_16х16_Вариант2);
            imagesList.Add(Images.DataRow, Resource.SaveChanges);
            imagesList.Add(Images.GoToObject, Resource.GoToObject);
            imagesList.Add(Images.DataSuppliersLocked, Resource.DataSuppliersLocked);
            imagesList.Add(Images.ErrorReport, Resource.ErrorReport);
            imagesList.Add(Images.View, Resource.View);
            imagesList.Add(Images.Views, Resource.Views);
            imagesList.Add(Images.UniqueKey, Resource.UniqueKey);
            imagesList.Add(Images.UniqueKeys, Resource.UniqueKeys);
            imagesList.Add(Images.SuffixCheck21, Resource.SuffixCheck21);
        }

        /// <summary>
        /// Редактор схем
        /// </summary>
        /// <param name="mainForm">Форма приложения</param>
        public SchemeEditor()
        {
            string configDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Krista.FM.Client.SchemeDesigner");
            PropertyService.InitializeService(
                configDirectory,
                Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "data"),
                "Krista.FM.Client.SchemeDesigner.Properties");
            PropertyService.Load();
            ResourceService.InitializeService(Path.Combine(PropertyService.DataDirectory, "resources"));
            ResourceService.RegisterStrings("Krista.FM.Client.SchemeEditor.Properties.Resource", GetType().Assembly);
        }


        #endregion Конструктор

        public static SchemeEditor Instance
        {
            get
            {
                if (singleInstance == null)
                {
                    singleInstance = new SchemeEditor();
                }
                return singleInstance;
            }
        }

        /// <summary>
        /// Инициалихация редактора.
        /// Вызывается в конце загрузки главной формы
        /// </summary>
        public void Initialize()
        {
            WorkbenchSingleton.InitializeWorkbench();
            StatusBarService.SetSchemeInfo(scheme.Server.Machine, scheme.Server.GetConfigurationParameter("ServerPort"), scheme.Name, ClientAuthentication.UserName);
            mainForm.Text = String.Format("{0} - {1}", scheme.Name, mainForm.Text);

            if (LogicalCallContextData.GetContext()["Supervisor"] is bool &&
                (bool)LogicalCallContextData.GetContext()["Supervisor"] == true)
                mainForm.Text += " [Режим супервизора]";
            if (Convert.ToBoolean(LogicalCallContextData.GetContext()["IgnoreVersions"]))
                mainForm.Text += " [Без контроля версий]";

            try
            {
                this.objectsTreeView.Nodes[0].Expanded = true;
                this.objectsTreeView.Nodes[0].Nodes[0].Expanded = true;
            }
            catch
            {
            }

            this.ModifiableObject = scheme;

            #region Удалить и перенести в Krista.FM.Client.SchemeDesigner.usersettings
            this.TabbedMdiManager.TabGroupSettings.ShowTabListButton = Infragistics.Win.DefaultableBoolean.True;
            this.TabbedMdiManager.TabSettings.DisplayFormIcon = Infragistics.Win.DefaultableBoolean.True;

            #endregion

            NaviButtonProcessing();
        }

        public void SetMainForm(Form mainForm)
        {
            this.mainForm = mainForm;
            MessageService.MainForm = mainForm;
        }

        public void Close()
        {
            // если был создан объект для индикации операций - уничтожаем
            if (operation != null)
            {
                operation.ReleaseThread();
                operation = null;
            }
        }

        /// <summary>
        /// Устанавливает выделенный объект по его полному имени.
        /// Должен вызываться при выделении объекта в дереве объектов или в диаграмме
        /// </summary>
        /// <param name="name">Полное наименование объекта</param>
        public void SetSlectedObject(string name)
        {
            //WorkbenchSingleton.Workbench.ShowView(new BrowserPane(new Uri("http://gw.krista.ru/")));

            CustomTreeNodeControl node = (CustomTreeNodeControl)objectsTreeView.GetNodeByKey(name);
            if (node != null)
            {
                string caption = node.Caption;
                int captionLength = 36;
                caption = caption.Length > captionLength ? caption.Substring(0, captionLength - 3) + "..." : caption;

                dockManager.ControlPanes["propertyGrid"].Text = caption == String.Empty ? "Свойства" : caption + " - Свойства";
                dockManager.ControlPanes["developerDescriptionControl"].Text = caption == String.Empty ? "Описание разработчика" : caption + " - Описание разработчика";
                dockManager.ControlPanes["macroSetControl"].Text = caption == String.Empty ? "Обработчики событий" : caption + " - Обработчики событий";

                propertyGrid.SelectedObject = node.ControlObject;
                developerDescriptionControl.SelectedObject = (IServerSideObject)node.ControlObject;
                macroSetControl.SelectedObject = (IServerSideObject)node.ControlObject;

                // Начало работы навигатора.
                NavigationService.Instance.OnStateChange(name);

                // Установка доступности кнопок.
                NaviButtonProcessing();
            }
        }

        internal void ShowObjectProperties(object obj)
        {
            propertyGrid.SelectedObject = obj;

            dockManager.ControlPanes["propertyGrid"].Closed = false;
            dockManager.ControlPanes["propertyGrid"].Activate();
        }

        /// <summary>
        /// Возвращает активную форму редактора диаграм
        /// </summary>
        public DiargamEditorForm ActiveDiargamEditorForm
        {
            get
            {
                return mainForm.ActiveMdiChild as DiargamEditorForm;
            }
        }

        /// <summary>
        /// Создает новую форму для отображения диаграммы.
        /// </summary>
        private Form CreateDiagramForm(IDocument document)
        {
            DiargamEditorForm form = new DiargamEditorForm();
            form.Text = document.Name;
			form.DiargamEditor.SchemeEditor = this;
			
			switch (document.DocumentType)
			{
				case DocumentTypes.Diagram:
					form.DiargamEditor.Diagram = new DiagramEditor.Diagrams.ClassDiagram(form.DiargamEditor, document);
					break;
				case DocumentTypes.DocumentEntityDiagram:
					form.DiargamEditor.Diagram = new DiagramEditor.Diagrams.DocumentEntityDiagram(form.DiargamEditor, document);
					break;
			}

            form.DiargamEditor.NewAssociationCommand = new NewAssociation(this);
            form.DiargamEditor.NewEntityCommand = new NewEntityCommand(this);
            form.DiargamEditor.NewPackageCommand = new NewPackageCommand(this);
            return form;
        }

        /// <summary>
        /// Создает новую форму для отображения данных таблицы.
        /// </summary>
        private Form CreateEntityGridForm(IEntity entity)
        {
            EntityGridForm form = new EntityGridForm();
            form.Text = entity.OlapName;
            form.EntityGrid.Entity = entity;
            return form;
        }

        public void ShowMdiForm(Form form)
        {
            form.MdiParent = mainForm;
            form.Show();
        }

        /// <summary>
        /// Обкрывает объект в новом окне.
        /// </summary>
        /// <param name="obj">объект.</param>
        private Form OpenObjectForm(ICommonDBObject obj)
        {
            try
            {
                Operation.StartOperation();
                Operation.Text = "Открытие объекта...";

                Form form;

                if (obj is IDocument && ((IDocument)obj).DocumentType != DocumentTypes.URL)
                {
                    form = CreateDiagramForm((IDocument)obj);
                }
                else if (obj is IEntity)
                    form = CreateEntityGridForm((IEntity)obj);
                else
                    form = new EmptyForm();

                ShowMdiForm(form);

                return form;
            }
            finally
            {
                Operation.StopOperation();
            }
        }

        /// <summary>
        /// Открывает серверный объект.
        /// </summary>
        /// <param name="serverObject">Серверный объект.</param>
        internal void OpenObject(ICommonDBObject serverObject)
        {
            MdiTab tab = tabbedMdiManager.TabFromKey(serverObject.Key);

            if (tab != null)
                tab.Activate();
            else
                OpenObjectForm(serverObject);
        }

        #region Применение изменений

        private IMajorModifiable modifiableObject;

        /// <summary>
        /// Интерфейс для получения изменений
        /// </summary>
        internal IMajorModifiable ModifiableObject
        {
            get { return modifiableObject; }
            set { modifiableObject = value; }
        }

        /// <summary>
        /// Обработчик проверяющий корректность описания
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void modificationsForm_TextChanged(object sender, EnableEventArgs e)
        {
            try
            {
                ICommentsCheckService service = Scheme.GetService(typeof(ICommentsCheckService)) as ICommentsCheckService;
                if (service != null)
                    e.Enable = service.CheckComments(e.Text);
                else
                    e.Enable = true;
            }
            catch
            {
                e.Enable = true;
            }
        }

        /// <summary>
        /// Применение всех изменений в дереве изменений
        /// </summary>
        /// <param name="commonDBObject">Пакет для которого применять изменения</param>
        /// <returns>Резутьтат выполнения операции. false - операция отменена пользователем</returns>
        internal bool ApplayModifications(ICommonDBObject commonDBObject)
        {
            if (commonDBObject is IPackage)
            {
                if (!SavePackageOpenDocuments((IPackage)commonDBObject))
                    return false;
            }

            this.ModifiableObject = commonDBObject;

            ModificationsForm modificationsForm = new ModificationsForm();
            modificationsForm.Text = "Применение изменений";
            modificationsForm.ModificationsTreeControl.Refresh();
            modificationsForm.ModificationsTreeControl.ExpandTree();
            modificationsForm.CommentsChanged += new EnableEventHandler(modificationsForm_TextChanged);
            DialogResult dr = ShowDialog(modificationsForm);
            if (dr == DialogResult.OK)
            {
                try
                {
                    modificationsForm.ModificationsTreeControl.Applay(modificationsForm.Comments);
                }
                catch (Exception e)
                {
                    FormException.ShowErrorForm(e);
                    return false;
                }
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Сохраняет все открытые диаграммы принадлежащие пакету package.
        /// При сохранении диаграммы выводит стандартное диалоговое окно сохранения,
        /// если пользователь отменяет сохранение, то функция возвращает false.
        /// </summary>
        /// <param name="package">Пакет документы которого необходимо сохранить.</param>
        /// <returns>false - сохранение отменено пользователем.</returns>
        private bool SavePackageOpenDocuments(IPackage package)
        {
            // TODO Сохранить все открытые диаграммы принадлежащие пакету package.
            // Перед сохранением каждой диаграммы необходимо задавать вопрос пользователю.

            // Просматриваем открытые диаграммы
            foreach (KeyValuePair<string, IDocument> document in package.Documents)
            {
                MdiTab tab = tabbedMdiManager.TabFromKey(((IDocument)document.Value).Key);
                if (tab != null)
                {
                    DiagramEditor.DiargamEditor diargamEditor = ((DiargamEditorForm)tab.Form).DiargamEditor;
                    if (!SaveDocument(diargamEditor))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Диалог сохранения диаграммы
        /// </summary>
        /// <param name="diargamEditor">Компонент диаграмма</param>
        /// <returns>false - в случае отмены сохранения</returns>
        private bool SaveDocument(Krista.FM.Client.DiagramEditor.DiargamEditor diargamEditor)
        {
            if (diargamEditor.Diagram.IsChanged)
            {
                DialogResult dr = MessageBox.Show(String.Format("Сохранить диаграмму \"{0}\"?", diargamEditor.Diagram.Document.Name), "Сохранение", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (dr)
                {
                    case DialogResult.No:
                        break;
                    case DialogResult.Yes:
                        diargamEditor.SaveDiagram();
                        break;
                    case DialogResult.Cancel:
                        return false;
                }
            }

            return true;
        }

        private struct ModificationsThreadParameters
        {
            internal IModificationItem ModificationItem;
            internal string Comments;

            public ModificationsThreadParameters(IModificationItem modificationItem, string comments)
            {
                this.ModificationItem = modificationItem;
                this.Comments = comments;
            }
        }

        /// <summary>
        /// Запускает процесс применения изменений.
        /// </summary>
        /// <param name="modificationItem">Операция модификации структуры.</param>
        internal void StartModificationsApplayProcess(IModificationItem modificationItem, string comments)
        {
            Thread thread = new Thread(new ParameterizedThreadStart(ApplayModificationsEntryPoint));
            thread.Start(new ModificationsThreadParameters(modificationItem, comments));
        }

        private void EnableUI()
        {
            ModificationsTreeControl.ReadOnly = false;
            ObjectsTreeView.Enabled = true;
            PropertyGrid.Enabled = true;
        }

        private void DisableUI()
        {
            ModificationsTreeControl.ReadOnly = true;
            ObjectsTreeView.Enabled = false;
            PropertyGrid.Enabled = false;
        }

        private void ShowModificationsTreeControl()
        {
            dockManager.ControlPanes["modificationsTreeControl"].Closed = false;
            dockManager.ControlPanes["modificationsTreeControl"].Activate();
        }

        private void ApplayModificationsEntryPoint(object parameter)
        {
            Operation.Text = "Применение изменений...";
            Operation.StartOperation();

            mainForm.Invoke(new VoidDelegate(ShowModificationsTreeControl));
            mainForm.Invoke(new VoidDelegate(DisableUI));

            try
            {
                //TODO: тут периодически вылетает на "Недопустимая операция в нескольких потоках"
                modificationsTreeControl.ClearTree();

                ModificationsThreadParameters mtp = (ModificationsThreadParameters)parameter;

                bool result = ApplayModifications(mtp.ModificationItem);

                if (ModifiableObject != null && ModifiableObject is ICommonDBObject)
                {
                    if (result)
                    {
                        ((ICommonDBObject)ModifiableObject).EndEdit(mtp.Comments);
                    }
					mainForm.Invoke(new SetStringDelegate(RefreshPackage), new object[] { ((ICommonDBObject)ModifiableObject).Key });
                }
            }
            catch (Exception e)
            {
                FormException.ShowErrorForm(e);
            }
            finally
            {
                mainForm.Invoke(new VoidDelegate(EnableUI));
                Operation.StopOperation();
            }
        }

        private bool ApplayModifications(IModificationItem modificationItem)
        {
            IModificationContext modificationContext = Scheme.CreateModificationContext();
            ModificationMessageHandling mmh = null;
            try
            {
                mmh = new ModificationMessageHandling();
                mmh.OnClientModificationMessage += new ModificationMessageEventHandler(mmh_OnClientModificationMessage);
                modificationContext.OnModificationMessage += new ModificationMessageEventHandler(mmh.OnServerModificationMessage);

                modificationContext.BeginUpdate();
                bool isAppliedPartially = false;
                modificationItem.Applay(modificationContext, out isAppliedPartially);
                modificationContext.EndUpdate();
                return true;
            }
            catch (Exception e)
            {
                modificationContext.EndUpdate();
                FormException.ShowErrorForm(e);
                return false;
            }
            finally
            {
                mmh.OnClientModificationMessage -= new ModificationMessageEventHandler(mmh_OnClientModificationMessage);
                if (mmh != null)
                    mmh.Dispose();
                modificationContext.OnModificationMessage -= new ModificationMessageEventHandler(mmh.OnServerModificationMessage);
                modificationContext.Dispose();
            }
        }

        private void mmh_OnClientModificationMessage(object sender, ModificationMessageEventArgs e)
        {
            try
            {
                Operation.StartOperation();
                Operation.Text = e.Message;
                if (e.Item != null)
                {
                    ModificationsTreeControl.SetCurrentNode(e.Item, e.IndentLevel);
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion Применение изменений

        #region Обработчики событий

        private bool sFlag = false;
        /// <summary>
        /// Обработка нажатий клавишь
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 19 && Control.ModifierKeys == Keys.Control)
                sFlag = true;
            else if (sFlag && e.KeyChar == 22 && Control.ModifierKeys == Keys.Control)
            {
                sFlag = false;
            }
            else
                sFlag = false;
        }

        void tabbedMdiManager_InitializeTab(object sender, Infragistics.Win.UltraWinTabbedMdi.MdiTabEventArgs e)
        {
            if (e.Tab.Form is DiargamEditorForm)
            {
                Krista.FM.Client.DiagramEditor.DiargamEditor diargamEditor = ((DiargamEditorForm)e.Tab.Form).DiargamEditor;
                e.Tab.Key = diargamEditor.Diagram.Document.Key;
            }
            if (e.Tab.Form is EntityGridForm)
            {
                EntityGridControl egc = ((EntityGridForm)e.Tab.Form).EntityGrid;
                e.Tab.Key = egc.Entity.Key;
            }
            switch (e.Tab.Form.Text)
            {
                case "Список семантик":
                    {
                        e.Tab.Key = "semanticsGridControl";
                        break;
                    }
                case "Виды поступающей информации":
                    {
                        e.Tab.Key = "dataKindsGridControl";
                        break;
                    }
                case "Поставщики данных":
                    {
                        e.Tab.Key = "dataSuppliersGridControl";
                        break;
                    }
                case "Сессии":
                    {
                        e.Tab.Key = "sessionGridControl";
                        break;
                    }
                case "Разделы кубов":
                    {
                        e.Tab.Key = "partitionsView";
                        break;
                    }
                case "Измерения":
                    {
                        e.Tab.Key = "dimentionsView";
                        break;
                    }
            }

        }

        /// <summary>
        /// Смена закладки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tabbedMdiManager_TabActivated(object sender, MdiTabEventArgs e)
        {
            switch (e.Tab.Key)
            {
                case "semanticsGridControl":
                    {
                        semanticsGridControl.Value = SchemeEditor.Instance.Scheme.Semantics;
                        break;
                    }
                case "dataKindsGridControl":
                    {
                        dataKindsGridControl.DataSupplierCollection = SchemeEditor.Instance.Scheme.DataSourceManager.DataSuppliers;
                        break;
                    }
                case "dataSuppliersGridControl":
                    {
                        dataSuppliersGridControl.DataSupplierCollection = SchemeEditor.Instance.Scheme.DataSourceManager.DataSuppliers;
                        break;
                    }
                case "sessionGridControl":
                    {
                        sessionGridControl.Sessions = SchemeEditor.Instance.Scheme.SessionManager.Sessions;
                        break;
                    }
            }

            //TODO обновление элементов панили управления (включая масштаб) 
        }

        void tabbedMdiManager_TabClosing(object sender, Infragistics.Win.UltraWinTabbedMdi.CancelableMdiTabEventArgs e)
        {
            if (e.Tab.Form is DiargamEditorForm)
            {
                Krista.FM.Client.DiagramEditor.DiargamEditor diargamEditor = ((DiargamEditorForm)e.Tab.Form).DiargamEditor;

                if (!SaveDocument(diargamEditor))
                    e.Cancel = true;
            }
        }

        void tabbedMdiManager_StoreTab(object sender, StoreTabEventArgs e)
        {
        }

        void tabbedMdiManager_RestoreTab(object sender, RestoreTabEventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(e.Tab.Key)
                    && e.Tab.Key != "semanticsGridControl"
                    && e.Tab.Key != "dataSuppliersGridControl"
                    && e.Tab.Key != "dataKindsGridControl"
                    && e.Tab.Key != "sessionGridControl")
                {
                    ICommonDBObject serverObject = SchemeEditor.Instance.Scheme.GetObjectByKey(e.Tab.Key);
                    e.Form = OpenObjectForm(serverObject);
                }
            }
            catch
            {
            }
        }

        #endregion Обработчики событий

        #region Свойства

        /// <summary>
        /// Редактируемая схема
        /// </summary>
        public IScheme Scheme
        {
            get { return scheme; }
            set
            {
                scheme = value;
                if (scheme != null)
                {
                    objectsTreeView.SetRootNode(new PackageControl(scheme.RootPackage, null));
                    //dataSuppliersGridControl.DataSupplierCollection = scheme.DataSourceManager.DataSuppliers;
                    //dataKindsGridControl.DataSupplierCollection = scheme.DataSourceManager.DataSuppliers;
                    //semanticsGridControl.Value = scheme.Semantics;
                }
            }
        }

        /// <summary>
        /// Объект для индикации длительных операций (создается при первом обращении)
        /// </summary>
        public Operation Operation
        {
            get
            {
                if (operation == null)
                    operation = new Operation();
                return operation;
            }
        }

        public static Dictionary<Images, System.Drawing.Bitmap> ImagesList
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return imagesList; }
        }

        public ObjectsTreeView ObjectsTreeView
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return objectsTreeView; }
            set { objectsTreeView = value; }
        }

        public PropertyGrid PropertyGrid
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return propertyGrid; }
            set { propertyGrid = value; }
        }

        public DeveloperDescriptionControl DeveloperDescriptionControl
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return developerDescriptionControl; }
            set { developerDescriptionControl = value; }
        }

        public Krista.FM.Client.Design.Editors.SemanticsGridControl SemanticsGridControl
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return semanticsGridControl; }
            set { semanticsGridControl = value; }
        }

        public DataSuppliersGridControl DataSuppliersGridControl
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return dataSuppliersGridControl; }
            set { dataSuppliersGridControl = value; }
        }

        public DataKindsGridControl DataKindsGridControl
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return dataKindsGridControl; }
            set { dataKindsGridControl = value; }
        }

        public ModificationsTreeControl ModificationsTreeControl
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return modificationsTreeControl; }
            set
            {
                modificationsTreeControl = value;
            }
        }

        public Services.SearchService.SearchTabControl SearchTabControl
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return searchTabControl; }
            set
            {
                searchTabControl = value;
            }
        }

        public Infragistics.Win.UltraWinDock.UltraDockManager DockManager
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return dockManager; }
            set
            {
                dockManager = value;
            }
        }

        public Infragistics.Win.UltraWinTabbedMdi.UltraTabbedMdiManager TabbedMdiManager
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return tabbedMdiManager; }
            set
            {
                tabbedMdiManager = value;
                tabbedMdiManager.InitializeTab += new Infragistics.Win.UltraWinTabbedMdi.MdiTabEventHandler(tabbedMdiManager_InitializeTab);
                tabbedMdiManager.TabClosing += new Infragistics.Win.UltraWinTabbedMdi.CancelableMdiTabEventHandler(tabbedMdiManager_TabClosing);
                tabbedMdiManager.StoreTab += new StoreTabEventHandler(tabbedMdiManager_StoreTab);
                tabbedMdiManager.RestoreTab += new RestoreTabEventHandler(tabbedMdiManager_RestoreTab);
                tabbedMdiManager.TabActivated += new MdiTabEventHandler(tabbedMdiManager_TabActivated);
            }
        }

        public UltraToolbarsManager ToolbarManager
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return toolbarManager; }
            set
            {
                toolbarManager = value;
                toolbarManager.BeforeToolDropdown += new BeforeToolDropdownEventHandler(toolbarManager_BeforeToolDropdown);
            }
        }

        public SessionGrid SessionGridControl
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return sessionGridControl; }
            set { sessionGridControl = value; }
        }

        public MacroSetControl MacroSetControl
        {
            get { return macroSetControl; }
            set { macroSetControl = value; }
        }

        #endregion Свойства

        #region ISchemeEditor Members

        /// <summary>
        /// Shows the specified System.Windows.Forms.Form.
        /// </summary>
        /// <param name="dialog">The System.Windows.Forms.Form to display.</param>
        /// <returns>A System.Windows.Forms.DialogResult indicating the result code returned by the System.Windows.Forms.Form.</returns>
        public DialogResult ShowDialog(Form dialog)
        {
            return dialog.ShowDialog(this.mainForm);
        }

        private ICommonObject GetObjectByPathName(IPackage container, string[] parts, int index)
        {
            switch (parts[index])
            {
                case "IPackageCollection":
                    return GetObjectByPathName(container.Packages[parts[index + 1]], parts, index + 2);
                case "IEntityCollection":
                    return container.Classes[parts[index + 1]];
                case "IEntityAssociationCollection":
                    return container.Associations[parts[index + 1]];
                default:
                    throw new Exception(String.Format("В пути встретился неизвестный модификатор {0}", parts[index]));
            }
        }

        /// <summary>
        /// Возвращает серверный объект по квалифицированному пути
        /// </summary>
        /// <param name="pathName">Квалифицированный путь объекта</param>
        /// <returns>Серверный объект</returns>
        public ICommonObject GetObjectByPathName(string pathName)
        {
            /*CustomTreeNodeControl node = (CustomTreeNodeControl)objectsTreeView.GetNodeByKey(pathName);
            if (node == null)
                return null;
            return node.ControlObject as ICommonObject;*/

            string[] parts = pathName.Split(new string[] { "::" }, StringSplitOptions.None);

            if (parts[0] != SystemSchemeObjects.ROOT_PACKAGE_KEY)
                throw new Exception(String.Format("Некоректный путь: \"{0}\"", pathName));

            return GetObjectByPathName(Scheme.RootPackage, parts, 1);
        }

        /// <summary>
        /// Удаляет объект из схемы
        /// </summary>
        /// <param name="pathName">Квалифицированный путь объекта</param>
        public void DeleteObject(string serverKey)
        {
            ICommonDBObject obj = this.Scheme.GetObjectByKey(serverKey);

            IPackage package = obj == null ? null : obj.ParentPackage;

            CustomTreeNodeControl node = this.objectsTreeView.GetNodeByKey(GetNameByServerName(serverKey)) as CustomTreeNodeControl;

            if (package != null)
            {
                if (MessageBox.Show(String.Format("Вы действительно хотите удалить объект \r {0} ?", serverKey), "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                {
                    if (obj is IEntity)
                    {
                        if (package.Classes.ContainsKey(obj.ObjectKey))
                            package.Classes.Remove(obj.ObjectKey);
                    }
                    //TODO: дублирование кода!
                    if (obj is IAssociation)
                    {
                        if (package.Associations.ContainsKey(obj.ObjectKey))
                        {
                            package.Associations.Remove(obj.ObjectKey);
                            ((IAssociation)obj).RoleData.Associations.Remove(obj.ObjectKey);
                            ((IAssociation)obj).RoleData.Attributes.Remove(((IAssociation)obj).RoleDataAttribute.ObjectKey);
                            ((IAssociation)obj).RoleBridge.Associated.Remove(obj.ObjectKey);
                        }
                    }
                }
            }

            if (node != null)
            {
                node = node.Parent as CustomTreeNodeControl;

                node.Refresh();
            }
        }

        public void SelectObject(string name, bool isCorrectName)
        {
            try
            {
                if (!isCorrectName)
                    name = GetNameByServerName(name);

                // Пытаемся развернуть, если переход не с навигатора.
                if (NavigationService.Instance.WhileNavigate)
                {
                    OpenTree(name);
                }

                UltraTreeNode node = objectsTreeView.GetNodeByKey(name);
                if (node == null)
                    throw new Exception(String.Format("Объект \"{0}\" не найден.", name));

                if (!objectsTreeView.Focused)
                    objectsTreeView.Focus();

                objectsTreeView.SelectedNodes.Clear();

                node.BringIntoView();
                node.Selected = true;
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("При открытии объекта в дереве возникла ошибка :{0}", e));
            }
        }

        public string GetNameByServerName(string serverName)
        {
            string separator = "::";

            string treeName = String.Empty;

            string[] parts = serverName.Split(new string[] { "::" }, StringSplitOptions.None);
            foreach (string s in parts)
            {
                if (s == "Package:" + SystemSchemeObjects.ROOT_PACKAGE_KEY)
                {
                    treeName += SystemSchemeObjects.ROOT_PACKAGE_KEY;
                }
                else
                {
                    string[] inner = s.Split(new string[] { ":" }, StringSplitOptions.None);
                    switch (inner[0])
                    {
                        case "Package":
                            treeName += String.Format("::IPackageCollection{0}{1}", separator, inner[1]);
                            break;

                        case "FixedClassifier":
                        case "BridgeClassifier":
                        case "DataClassifier":
                        case "FactTable":
                        case "TableEntity":
                            treeName += String.Format("::IEntityCollection{0}{1}", separator, inner[1]);
                            break;

                        case "Fact2BridgeAssociation":
                        case "BridgeAssociation":
                        case "BridgeAssociationItSelf":
                        case "FactAssociation":
                        case "MasterDetailAssociation":
                            treeName += String.Format("::IEntityAssociationCollection{0}{1}", separator, inner[1]);
                            break;

                        case "Document":
                            treeName += String.Format("::IDocumentsCollection{0}{1}", separator, inner[1]);
                            break;
                        case "EntityDataAttribute":
                        case "DocumentAttribute":
                            treeName += String.Format("::IDataAttributeCollection{0}{1}", separator, inner[1]);
                            break;
                        case "EntityAssociationAttribute":
                            //treeName += String.Format("::IRefDataAttributeCollection{0}{1}", separator, inner[1]);
                            break;
                        case "RegularLevel":
                        case "ParentChildLevel":
                            treeName += String.Format("::IDimensionLevelCollection{0}{1}", separator, inner[1]);
                            break;
                        case "AssociateRule":
                            treeName += String.Format("::IAssociateRuleCollection{0}{1}", separator, inner[1]);
                            break;
                        case "AssociateMapping":
                            treeName += String.Format("::AssociateMappingList{0}{1}", separator, inner[1]);
                            break;
                    }
                }
            }
            return treeName;
        }

        public void OpenTree(string fullName)
        {
            string[] parts = fullName.Split(new string[] { "::" }, StringSplitOptions.None);

            if (parts[0] != SystemSchemeObjects.ROOT_PACKAGE_KEY)
                throw new Exception(String.Format("Некоректный путь: \"{0}\"", fullName));

            UltraTreeNode node = this.objectsTreeView.Nodes[SystemSchemeObjects.ROOT_PACKAGE_KEY];
            this.objectsTreeView.Nodes[SystemSchemeObjects.ROOT_PACKAGE_KEY].Expanded = true;
            OpenTree(node, parts, 1, SystemSchemeObjects.ROOT_PACKAGE_KEY);
        }

        private void OpenTree(UltraTreeNode node, string[] parts, int index, string s)
        {
            switch (parts[index])
            {
                case "IPackageCollection":
                    s += String.Format("::{0}", parts[index]);
                    node.Nodes[s].Expanded = true;
                    node = node.Nodes[s];

                    s += String.Format("::{0}", parts[index + 1]);
                    node.Nodes[s].Expanded = true;
                    node = node.Nodes[s];

                    if (index + 2 < parts.Length)
                    {
                        OpenTree(node, parts, index + 2, s);
                    }
                    break;

                case "IEntityCollection":
                    s += "::IEntityCollection";
                    node.Nodes[s].Expanded = true;
                    node = node.Nodes[s];

                    // если открываем атрибут
                    if (index + 2 < parts.Length)
                    {
                        s += String.Format("::{0}", parts[index + 1]);
                        node.Nodes[s].Expanded = true;
                        node = node.Nodes[s];

                        OpenTree(node, parts, index + 2, s);
                    }
                    break;

                case "IEntityAssociationCollection":
                    s += "::IEntityAssociationCollection";
                    node.Nodes[s].Expanded = true;
                    node = node.Nodes[s];

                    // если открываем сопоставление или правила формирования, или соответствие 
                    if (index + 2 < parts.Length)
                    {
                        s += String.Format("::{0}", parts[index + 1]);
                        node.Nodes[s].Expanded = true;
                        node = node.Nodes[s];

                        OpenTree(node, parts, index + 2, s);
                    }
                    break;

                case "IDocumentsCollection":
                    s += "::IDocumentsCollection";
                    node.Nodes[s].Expanded = true;
                    break;

                case "IDataAttributeCollection":
                    s += "::IDataAttributeCollection";
                    node.Nodes[s].Expanded = true;
                    break;

                case "IRefDataAttributeCollection":
                    s += "::" + parts[index + 1];
                    node.Nodes[s].Expanded = true;
                    break;

                case "IAssociateRuleCollection":
                    s += "::IAssociateRuleCollection";
                    node.Nodes[s].Expanded = true;
                    node = node.Nodes[s];

                    // правило соответствия
                    if (index + 2 < parts.Length)
                    {
                        s += String.Format("::{0}", parts[index + 1]);
                        node.Nodes[s].Expanded = true;
                        node = node.Nodes[s];
                    }
                    break;

                case "AssociateMappingList":
                    s += "::AssociateMappingList";
                    node.Nodes[s].Expanded = true;
                    break;

                case "IDimensionLevelCollection":
                    s += "::IDimensionLevelCollection";
                    node.Nodes[s].Expanded = true;
                    break;

                case "IUniqueKeyCollection":
                    s += "::IUniqueKeyCollection";
                    node.Nodes[s].Expanded = true;
                    break;

                default:
                    throw new Exception(String.Format("Некорректное имя объекта :{0}", s));
            }
        }

        /// <summary>
        /// Обновляем пакет в дереве по серверному имени
        /// </summary>
        /// <param name="objectName">Имя пакета на сервере</param>
        public void RefreshPackage(string objectName)
        {
            CustomTreeNodeControl node = this.objectsTreeView.GetNodeByKey(this.GetNameByServerName(objectName)) as CustomTreeNodeControl;
            node.Refresh();
        }
        #endregion

        #region Навигатор

        /// <summary>
        /// Заполнение пунктов меню.
        /// </summary>
        /// <param name="menu">Меню для заполнения</param>
        /// <param name="tempList">Список элементов, которыми будем заполнять</param>        
        /// <param name="startCycle">Определяет, с какого места в списке начнется заполнение</param>
        private void FillNaviMenu(ToolBase menu, string[] tempList, int startCycle)
        {
            // Очищаем меню.
            ((PopupMenuTool)menu).Tools.Clear();
            // Цикл заполнения, с конца списка истории, пока не дойдем до нуля.            
            for (int i = startCycle; i >= 0; i--)
            {
                if (tempList[i] != null)
                {
                    // Берем имя, картинку, и пишем в меню.
                    Infragistics.Win.UltraWinTree.UltraTreeNode node = objectsTreeView.GetNodeByKey(tempList[i].ToString());
                    if (node != null)
                    {
                        ButtonTool newMenuRecord = new ButtonTool(tempList[i].ToString());
                        int ImageIndex = ((Client.SchemeEditor.ControlObjects.CustomTreeNodeControl)node).ImageIndex;
                        newMenuRecord.CustomizedImage = Client.SchemeEditor.SchemeEditor.ImagesList[(Images)ImageIndex];
                        newMenuRecord.CustomizedCaption = node.Text.ToString();
                        try
                        {
                            ((PopupMenuTool)menu).Tools.AddTool(newMenuRecord.Key);
                        }
                        catch
                        {
                            toolbarManager.Tools.Add(newMenuRecord);
                            ((PopupMenuTool)menu).Tools.AddTool(newMenuRecord.Key);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Обработка доступности кнопок
        /// </summary>
        public void NaviButtonProcessing()
        {
            this.ToolbarManager.Tools["Back"].SharedProps.Enabled = NavigationService.Instance.CanBackward();
            this.ToolbarManager.Tools["Forward"].SharedProps.Enabled = NavigationService.Instance.CanForward();
        }

        /// <summary>
        /// Нажатие на меню кнопки в навигаторе
        /// </summary>
        /// <param name="item"></param>
        public void NaviMenuItemClicked(ToolBase item)
        {
            if (item.Owner.ToString() == "[Back] - PopupMenuTool")
            {
                NavigationService.Instance.Backward(this, item.Index + 1);
            }
            if (item.Owner.ToString() == "[Forward] - PopupMenuTool")
            {
                NavigationService.Instance.Forward(this, item.Index + 1);
            }
            this.NaviButtonProcessing();
        }

        /// <summary>
        /// Обработка нажатия на меню кнопок навигатора
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolbarManager_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "Back":
                    {
                        // Формируем временную копию списка назад
                        string[] tempList = NavigationService.Instance.initHistBackArray();
                        // Заполняем меню.
                        this.FillNaviMenu(toolbarManager.Tools["Back"], tempList, NavigationService.Instance.CountBack);
                        break;
                    }
                case "Forward":
                    {
                        // Формируем временную копию списка вперед
                        string[] tempList = NavigationService.Instance.initHistForwardArray();
                        // Заполняем меню.
                        this.FillNaviMenu(toolbarManager.Tools["Forward"], tempList, NavigationService.Instance.CountForward);
                        break;
                    }
            }
        }

        #endregion

        #region Сохранение всех диграмм

        public void SaveAllDigrams(string resultPath, bool addingMode)
        {
            try
            {
                DiargamEditorForm form = new DiargamEditorForm();
                form.DiargamEditor.SchemeEditor = this;
				form.DiargamEditor.Diagram = new DiagramEditor.Diagrams.ClassDiagram(form.DiargamEditor);

                IPackage root = scheme.RootPackage;
                SaveDiagrams(root, form, resultPath, addingMode);
            }
            catch
            {
            }
        }

        private void SaveDiagrams(IPackage root, DiargamEditorForm form, string resultPath, bool addingMode)
        {
            try
            {
                foreach (IPackage package in root.Packages.Values)
                {
                    SaveDiagrams(package, form, resultPath, addingMode);
                }

                foreach (IDocument document in root.Documents.Values)
                {
                    SaveDocumentToFile(document, form, resultPath, addingMode);
                }
            }
            catch
            {
            }
        }

        private void SaveDocumentToFile(IDocument document, DiargamEditorForm form, string resultpath, bool addingMode)
        {
            try
            {
                string s = CheckName(document.Name);

                if (!addingMode)
                    Save(document, form, resultpath);

                else
                    if (!File.Exists(String.Format(@"{0}\diagrams\{1}.emf", resultpath, s)))
                        Save(document, form, resultpath);


            }
            catch (Exception e)
            {
                FormException.ShowErrorForm(e);
            }
            finally
            {
                form.DiargamEditor.Diagram.Dispose();
            }
        }

        private static void Save(IDocument document, DiargamEditorForm form, string resultpath)
        {
            if (document.DocumentType == DocumentTypes.Diagram || document.DocumentType == DocumentTypes.DocumentEntityDiagram)
            {
                form.DiargamEditor.Diagram.Document = document;

                form.DiargamEditor.Diagram.Load();

                form.Text = document.Name;

                ((CommandSaveMetafile)form.DiargamEditor.Diagram.DiagramСommands[AbstractDiagram.SaveMetafileContextMenuItemKey]).BaseDirectory =
                    resultpath;
                ((CommandSaveMetafile)form.DiargamEditor.Diagram.DiagramСommands[AbstractDiagram.SaveMetafileContextMenuItemKey]).Execute();
            }
        }

        private static string CheckName(string p)
        {
            char[] illegalCharacters = new char[] { ':', '/', '\\', '|', '*', '<', '>', '?', '"' };
            for (int i = 0; i < illegalCharacters.Length; i++)
            {
                if (p.IndexOf(illegalCharacters[i]) > -1)
                {
                    p = p.Replace(illegalCharacters[i], '_');
                }
            }
            return p;
        }

        #endregion Сохранение всех диграмм

    }
}
