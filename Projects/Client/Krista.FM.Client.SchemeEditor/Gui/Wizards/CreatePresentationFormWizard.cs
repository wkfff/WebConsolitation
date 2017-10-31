using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Krista.FM.ServerLibrary;
using Infragistics.Win.UltraWinTree;

namespace Krista.FM.Client.SchemeEditor.Gui.Wizard
{
    /// <summary>
    /// Состояние мастера создания представления
    /// </summary>
    public enum MasterState
    {
        /// <summary>
        /// 
        /// </summary>
        StepWelcom,
        /// <summary>
        /// 
        /// </summary>
        StepSetName,
        /// <summary>
        /// 
        /// </summary>
        StepSetDataAttributes,
        /// <summary>
        /// 
        /// </summary>
        StepSetLevelNamingTemplate,
        /// <summary>
        /// 
        /// </summary>
        StepFinish
    }

    /// <summary>
    /// Мастер создания представления
    /// </summary>
    public partial class CreatePresentationFormWizard : Form
    {
        /// <summary>
        /// Объект, для которого создаем представление
        /// </summary>
        private readonly IEntity entity;

        /// <summary>
        /// Состояние окна мастера
        /// </summary>
        private MasterState state; 

        private static string _name;
        private static string _levelNamingTemplate;
        private static List<IDataAttribute> _attributes;

        public CreatePresentationFormWizard()
        {
            InitializeComponent();

            state = MasterState.StepWelcom;

            wizardForm.Next += wizardForm_Next;
            wizardForm.Back += wizardForm_Back;
            wizardForm.Cancel += wizardForm_Cancel;
            wizardForm.Finish += wizardForm_Finish;
        }      

        public CreatePresentationFormWizard(IEntity entity)
            : this()
        {
            this.entity = entity;
            InitializeWizardForm();
        }

        private void InitializeWizardForm()
        {
            this.Text = String.Format("{0}", entity.FullName);
        }

        void wizardForm_Finish(object sender, Common.Wizards.WizardForm.EventNextArgs e)
        {
            Close();
        } 

        void wizardForm_Cancel(object sender, Common.Wizards.WizardForm.EventWizardCancelArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        void wizardForm_Back(object sender, Common.Wizards.WizardForm.EventNextArgs e)
        {
            switch (state)
            {
                case MasterState.StepSetName:
                    state = MasterState.StepWelcom;
                    break;
                case MasterState.StepSetDataAttributes:
                    state = MasterState.StepSetName;
                    break;
                case MasterState.StepSetLevelNamingTemplate:
                    state = MasterState.StepSetDataAttributes;
                    break;
                case MasterState.StepFinish:
                    state = MasterState.StepSetLevelNamingTemplate;
                    break;
            }
        }

        void wizardForm_Next(object sender, Common.Wizards.WizardForm.EventNextArgs e)
        {
            switch (state)
            {
                case MasterState.StepWelcom:
                    state = MasterState.StepSetName;
                    break;
                case MasterState.StepSetName:
                    SetPresentationName(u_tb_Name.Text);
                    state = MasterState.StepSetDataAttributes;
                    break;
                case MasterState.StepSetDataAttributes:
                    SetPresentationAttributes(ultraTree.Nodes);
                    state = MasterState.StepSetLevelNamingTemplate;
                    break;
                case MasterState.StepSetLevelNamingTemplate:
                    state = MasterState.StepFinish;
                    SetPresentationLevelNamingTemplate(u_tb_level.Text);
                    break;

            }
        }

        
        public static bool ShowCreatePresentationDialog(IEntity entity, out string name,
           out List<IDataAttribute> attributes, out string levelNameTemplate)
        {
            CreatePresentationFormWizard form = new CreatePresentationFormWizard(entity);

            name = _name;
            levelNameTemplate = _levelNamingTemplate;
            attributes = _attributes;

            if (form.ShowDialog() == DialogResult.OK)
            {
                name = _name;
                levelNameTemplate = _levelNamingTemplate;
                attributes = _attributes;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Уснановка имен уровней иерархии для представления
        /// </summary>
        /// <param name="levelNamingTemplate"></param>
        private static void SetPresentationLevelNamingTemplate(string levelNamingTemplate)
        {
            _levelNamingTemplate = levelNamingTemplate;
        }

        /// <summary>
        /// Установка атрибутов для представления
        /// </summary>
        /// <param name="treeNodesCollection"></param>
        private static void SetPresentationAttributes(TreeNodesCollection treeNodesCollection)
        {
            if (_attributes == null)
                _attributes = new List<IDataAttribute>();

            _attributes.Clear();

            foreach (UltraTreeNode node in treeNodesCollection)
            {
                IDataAttribute attr;
                if (((IDataAttribute)node.Tag).Kind == DataAttributeKindTypes.Regular)
                    attr = (IDataAttribute)((IDataAttribute)node.Tag).Clone();
                else
                    attr = (IDataAttribute)node.Tag;
                
                if (node.CheckedState == CheckState.Unchecked)
                {
                    attr.Visible = false;
                }

                _attributes.Add(attr);
            }
        }

        /// <summary>
        /// Установка имени представления
        /// </summary>
        /// <param name="name"></param>
        private static void SetPresentationName(string name)
        {
            _name = name;
        }

        #region Инициализация форм визарда

        /// <summary>
        /// Инициализация формы выбора атрибутов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wizardPageAttributes_Load(object sender, EventArgs e)
        {
            ultraTree.Nodes.Clear();

            foreach (IDataAttribute attribute in entity.Attributes.Values)
            {
                UltraTreeNode node = new UltraTreeNode(String.Format("{0}", attribute.Caption));
                node.Tag = attribute;
                node.Override.NodeStyle = NodeStyle.CheckBox;

                switch (attribute.Class)
                {
                    case DataAttributeClassTypes.Fixed:
                    case DataAttributeClassTypes.Reference:
                    case DataAttributeClassTypes.System:
                        {
                            node.CheckedState = CheckState.Checked;
                            node.Enabled = false;
                            break;
                        }
                    case DataAttributeClassTypes.Typed:
                        {
                            if (!attribute.IsNullable)
                                node.CheckedState = CheckState.Checked;
                            break;
                        }
                }

                ultraTree.Nodes.Add(node);
            }
        }

        private void wizardPageLevelNamingTemplate_Load(object sender, EventArgs e)
        {
            if (entity is IClassifier)
            {
                if (((IClassifier)entity).Levels.HierarchyType == HierarchyType.Regular)
                    u_tb_level.Enabled = false;
                else
                {
                    u_tb_level.Enabled = true;

                    string levelWithTemplateName = string.Empty;
                    foreach (IDimensionLevel item in ((IClassifier)entity).Levels.Values)
                    {
                        if (item.LevelType != LevelTypes.All)
                        {
                            levelWithTemplateName = item.ObjectKey;
                            break;
                        }
                    }

                    u_tb_level.Text = (((IClassifier)entity).Levels.ContainsKey(levelWithTemplateName))
                        ? ((IClassifier)entity).Levels[levelWithTemplateName].LevelNamingTemplate
                        : String.Empty;
                }
            }
        }

        private void wizardPageName_Load(object sender, EventArgs e)
        {
            u_tb_Name.Text = String.Format("{0}.Presentation{1}",
                entity.FullCaption,
                entity.Presentations.Count + 1);
        }

        #endregion
    }
}