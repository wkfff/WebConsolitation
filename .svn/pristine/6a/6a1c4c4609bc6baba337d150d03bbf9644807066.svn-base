using System;
using System.Windows.Forms;
using Krista.FM.ServerLibrary;
using Infragistics.Win;
using System.Data;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.Wizard
{
    /// <summary>
    /// Состояния мастера создания версии классификатора
    /// </summary>
    public enum StateVersionWizard
    {
        /// <summary>
        /// Стартовая страница
        /// </summary>
        WelcomState = 0,
        /// <summary>
        /// Страница настроики имени версии
        /// </summary>
        NamePageState = 1,
        /// <summary>
        /// Страница настройки источника версии
        /// </summary>
        DataSourcePageState = 2, 
        /// <summary>
        /// Страница настройки представления, по умолчанию берется последнее представление
        /// </summary>
        PresentationPageState = 3,
        /// <summary>
        /// Финальная страница
        /// </summary>
        FinalState = 4
    }
    
    /// <summary>
    /// Мастер по созданию версии классификатора (в будущем будем позволять еще редактировать существующие версии)
    /// </summary>
    public partial class CreateVersionForm : Form
    {
        /// <summary>
        /// Схема, с которой работаем
        /// </summary>
        private IScheme scheme;
        /// <summary>
        /// Классификатор, для которого создаем версию
        /// </summary>
        private IEntity entity;
        /// <summary>
        /// Состояние мастера создания представления
        /// </summary>
        private StateVersionWizard state;
        /// <summary>
        /// Имя версии классификатора
        /// </summary>
        private static string versionName;
        /// <summary>
        /// ID источника
        /// </summary>
        private static int sourceID;
        /// <summary>
        /// Представление структуры классификатора
        /// </summary>
        private static string presentationKey;
        /// <summary>
        /// Имя представления структуры классификатора
        /// </summary>
        private static string presentationName;
        /// <summary>
        /// Признак деления классификатора по версиям структуры
        /// </summary>
        private bool isPresentationDivide;

        private DataSourcesHelper dataSourcesHelper;

        private CreateVersionForm()
        {
            Clean();

            InitializeComponent();

            this.wizardForm.Finish += wizardForm_Finish;
            this.wizardForm.WizardClosed += new EventHandler(wizardForm_WizardClosed);
        }

        /// <summary>
        /// Очистка
        /// </summary>
        private void Clean()
        {
            versionName = string.Empty;
            sourceID = 0;
            presentationKey = string.Empty;
            presentationName = string.Empty;
        }

        void wizardForm_WizardClosed(object sender, EventArgs e)
        {
            Close();
        }
        
        void wizardForm_Finish(object sender, Common.Wizards.WizardForm.EventNextArgs e)
        {
            Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scheme"></param>
        /// <param name="entity"></param>
        public CreateVersionForm(IScheme scheme, IEntity entity)
            : this()
        {
            this.scheme = scheme;
            this.entity = entity;

            isPresentationDivide = (entity.Presentations.Count == 0) ? false : true;

            state = StateVersionWizard.WelcomState;

            Initialize();
        }

        private void Initialize()
        {
            this.Text = String.Format("Создание версии для {0}", entity.FullCaption);
        }

        /// <summary>
        /// Движение вперед по мастеру
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wizardForm_Next(object sender, Common.Wizards.WizardForm.EventNextArgs e)
        {
            switch (state)
            {
                case StateVersionWizard.WelcomState:
                    state = StateVersionWizard.DataSourcePageState;
                    break;
                case StateVersionWizard.DataSourcePageState:
                    if (SetSourceID())
                    {
                        if (!isPresentationDivide)
                        {
                            state = StateVersionWizard.FinalState;
                            e.Step = 2;
                        }
                        else
                        {
                            state = StateVersionWizard.PresentationPageState;
                        }
                    }
                    else
                        e.Step = 0;
                    break;
                case StateVersionWizard.PresentationPageState:
                    if (SetPresentation())
                        state = StateVersionWizard.FinalState;
                    else
                        e.Step = 0;
                    break;
            }
        }

        /// <summary>
        /// Движение назад по мастеру
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wizardForm_Back(object sender, Common.Wizards.WizardForm.EventNextArgs e)
        {
            switch (state)
            {
                case StateVersionWizard.DataSourcePageState:
                    state = StateVersionWizard.WelcomState;
                    break;
                case StateVersionWizard.PresentationPageState:
                    state = StateVersionWizard.DataSourcePageState;
                    break;
                case StateVersionWizard.FinalState:
                    if (!isPresentationDivide)
                    {
                        state = StateVersionWizard.DataSourcePageState;
                        e.Step = 2;
                    }
                    else
                        state = StateVersionWizard.PresentationPageState;
                    break;
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool SetPresentation()
        {
            if (ultraComboEditor.SelectedItem != null)
            {
                presentationKey = ultraComboEditor.SelectedItem.DataValue.ToString();
                presentationName = ultraComboEditor.SelectedItem.DisplayText;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool SetSourceID()
        {
            dataSourceSelectControl.SelectDataSources(ref sourceID);
            if (sourceID == 0)
            {
                if (MessageBox.Show("Поле источник данных обязательно для заполнения",
                    "Незаполнено обязательное поле", MessageBoxButtons.OK, MessageBoxIcon.Warning) == DialogResult.OK)
                    return false;
            }

            DataTable dtSource = DataSourcesHelper.GetDataSourcesInfo(sourceID, scheme);
            if (dtSource.Rows.Count > 0)
            {
                if (Convert.ToBoolean(dtSource.Rows[0]["Locked"]))
                {
                    if (MessageBox.Show("Источник заблокирован от изменений",
                    "Укажите незаблокированный источник", MessageBoxButtons.OK, MessageBoxIcon.Warning) == DialogResult.OK)
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool SetName()
        {
            if (String.IsNullOrEmpty(ultraTextEditor.Text))
            {
                if (MessageBox.Show("Поле имя версии обязательно для заполнения",
                    "Незаполнено обязательное поле", MessageBoxButtons.OK, MessageBoxIcon.Warning) == DialogResult.OK)
                return false;
            }
                
            versionName = ultraTextEditor.Text;
            return true;
        }

        private void dataSourceSelectControl_Load(object sender, EventArgs e)
        {
            wizardPageDataSource.Description += String.Format("\nСписок видов поступающей информации: {0}",
                                                               ((IDataSourceDividedClass) entity).DataSourceKinds);
            dataSourceSelectControl.InitializeScheme(scheme, ((IDataSourceDividedClass)entity).DataSourceKinds);
        }

        public static bool ShowCreateVersionWizard(IScheme scheme, IEntity entity,
           ref string _versionName, ref int _sourceID, ref string _presentationKey)
        {
            CreateVersionForm form = new CreateVersionForm(scheme, entity);

            if (form.ShowDialog() == DialogResult.OK)
            {
                _versionName = versionName;
                _sourceID = sourceID;
                _presentationKey = presentationKey;

                return true;
            }

            return false;
        }

        private void wizardPagePresentation_Load(object sender, EventArgs e)
        {
            ultraComboEditor.Items.Clear();

            foreach (IPresentation presentation in entity.Presentations.Values)
            {
                ValueListItem item = new ValueListItem();
                item.DisplayText = presentation.Name;
                item.DataValue = presentation.ObjectKey;

                ultraComboEditor.Items.Add(item);
            }

            ultraComboEditor.Text = entity.Presentations.DefaultPresentation;
        }

        void wizardFinalPage1_Enter(object sender, System.EventArgs e)
        {
            versionName = String.Format("{0}.{1}", entity.FullCaption,
                                        scheme.DataSourceManager.GetDataSourceName(sourceID));
            wizardFinalPage1.Description =
                String.Format("Идентификатор источника: {0}\nВерсия структуры: {1}\nИмя версии классификатора: {2}",
                              sourceID,
                              (String.IsNullOrEmpty(presentationKey)) ? "не делится по версиям" : presentationName,
                              versionName);
        }
    }
}