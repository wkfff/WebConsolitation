namespace Krista.FM.Client.SchemeEditor.Gui.Wizard
{
    partial class CreatePresentationFormWizard
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.wizardForm = new Krista.FM.Client.Common.Wizards.WizardForm();
            this.wizardWelcomePage1 = new Krista.FM.Client.Common.Wizards.WizardWelcomePage();
            this.wizardPageName = new Krista.FM.Client.Common.Wizards.WizardPageBase();
            this.u_tb_Name = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.wizardPageAttributes = new Krista.FM.Client.Common.Wizards.WizardPageBase();
            this.ultraTree = new Infragistics.Win.UltraWinTree.UltraTree();
            this.wizardPageLevelNamingTemplate = new Krista.FM.Client.Common.Wizards.WizardPageBase();
            this.u_tb_level = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.wizardFinalPage1 = new Krista.FM.Client.Common.Wizards.WizardFinalPage();
            this.wizardPageName.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.u_tb_Name)).BeginInit();
            this.wizardPageAttributes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTree)).BeginInit();
            this.wizardPageLevelNamingTemplate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.u_tb_level)).BeginInit();
            this.SuspendLayout();
            // 
            // wizardForm
            // 
            this.wizardForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardForm.Location = new System.Drawing.Point(0, 0);
            this.wizardForm.Name = "wizardForm";
            this.wizardForm.PageIndex = 0;
            this.wizardForm.Pages.AddRange(new Krista.FM.Client.Common.Wizards.WizardPageBase[] {
            this.wizardWelcomePage1,
            this.wizardPageName,
            this.wizardPageAttributes,
            this.wizardPageLevelNamingTemplate,
            this.wizardFinalPage1});
            this.wizardForm.Size = new System.Drawing.Size(792, 467);
            this.wizardForm.TabIndex = 0;
            // 
            // wizardWelcomePage1
            // 
            this.wizardWelcomePage1.BackColor = System.Drawing.Color.White;
            this.wizardWelcomePage1.Description = "";
            this.wizardWelcomePage1.Description2 = "";
            this.wizardWelcomePage1.Index = 0;
            this.wizardWelcomePage1.Name = "wizardWelcomePage1";
            this.wizardWelcomePage1.Size = new System.Drawing.Size(792, 420);
            this.wizardWelcomePage1.TabIndex = 0;
            this.wizardWelcomePage1.Title = "Вас приветствует мастер создания представления";
            this.wizardWelcomePage1.WizardPageParent = this.wizardForm;
            // 
            // wizardPageName
            // 
            this.wizardPageName.Controls.Add(this.u_tb_Name);
            this.wizardPageName.Description = "Шаг 1. Имя представления";
            this.wizardPageName.Index = 1;
            this.wizardPageName.Name = "wizardPageName";
            this.wizardPageName.Size = new System.Drawing.Size(776, 356);
            this.wizardPageName.TabIndex = 0;
            this.wizardPageName.Title = "Мастер создания представления";
            this.wizardPageName.WizardPageParent = this.wizardForm;
            this.wizardPageName.Load += new System.EventHandler(this.wizardPageName_Load);
            // 
            // u_tb_Name
            // 
            this.u_tb_Name.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.u_tb_Name.Location = new System.Drawing.Point(25, 28);
            this.u_tb_Name.Name = "u_tb_Name";
            this.u_tb_Name.Size = new System.Drawing.Size(728, 21);
            this.u_tb_Name.TabIndex = 0;
            // 
            // wizardPageAttributes
            // 
            this.wizardPageAttributes.Controls.Add(this.ultraTree);
            this.wizardPageAttributes.Description = "Шаг 4. Набор атрибутов представления";
            this.wizardPageAttributes.Index = 4;
            this.wizardPageAttributes.Name = "wizardPageAttributes";
            this.wizardPageAttributes.Size = new System.Drawing.Size(776, 356);
            this.wizardPageAttributes.TabIndex = 0;
            this.wizardPageAttributes.Title = "Мастер создания представления";
            this.wizardPageAttributes.WizardPageParent = this.wizardForm;
            this.wizardPageAttributes.Load += new System.EventHandler(this.wizardPageAttributes_Load);
            // 
            // ultraTree
            // 
            this.ultraTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraTree.Location = new System.Drawing.Point(0, 0);
            this.ultraTree.Name = "ultraTree";
            this.ultraTree.Size = new System.Drawing.Size(776, 356);
            this.ultraTree.TabIndex = 0;
            // 
            // wizardPageLevelNamingTemplate
            // 
            this.wizardPageLevelNamingTemplate.Controls.Add(this.u_tb_level);
            this.wizardPageLevelNamingTemplate.Description = "Шаг 5. Имена уровней иерархии";
            this.wizardPageLevelNamingTemplate.Index = 5;
            this.wizardPageLevelNamingTemplate.Name = "wizardPageLevelNamingTemplate";
            this.wizardPageLevelNamingTemplate.Size = new System.Drawing.Size(776, 356);
            this.wizardPageLevelNamingTemplate.TabIndex = 0;
            this.wizardPageLevelNamingTemplate.Title = "Мастер создания представления";
            this.wizardPageLevelNamingTemplate.WizardPageParent = this.wizardForm;
            this.wizardPageLevelNamingTemplate.Load += new System.EventHandler(this.wizardPageLevelNamingTemplate_Load);
            // 
            // u_tb_level
            // 
            this.u_tb_level.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.u_tb_level.Location = new System.Drawing.Point(25, 28);
            this.u_tb_level.Name = "u_tb_level";
            this.u_tb_level.Size = new System.Drawing.Size(727, 21);
            this.u_tb_level.TabIndex = 0;
            // 
            // wizardFinalPage1
            // 
            this.wizardFinalPage1.BackColor = System.Drawing.Color.White;
            this.wizardFinalPage1.Description = "Создание представления завершено";
            this.wizardFinalPage1.Description2 = "";
            this.wizardFinalPage1.FinishPage = true;
            this.wizardFinalPage1.Index = 8;
            this.wizardFinalPage1.Name = "wizardFinalPage1";
            this.wizardFinalPage1.Size = new System.Drawing.Size(498, 328);
            this.wizardFinalPage1.TabIndex = 0;
            this.wizardFinalPage1.Title = "";
            this.wizardFinalPage1.WelcomePage = true;
            this.wizardFinalPage1.WizardPageParent = this.wizardForm;
            // 
            // CreatePresentationFormWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 467);
            this.Controls.Add(this.wizardForm);
            this.MinimumSize = new System.Drawing.Size(750, 450);
            this.Name = "CreatePresentationFormWizard";
            this.Text = "CreatePresentationFormWizard";
            this.wizardPageName.ResumeLayout(false);
            this.wizardPageName.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.u_tb_Name)).EndInit();
            this.wizardPageAttributes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraTree)).EndInit();
            this.wizardPageLevelNamingTemplate.ResumeLayout(false);
            this.wizardPageLevelNamingTemplate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.u_tb_level)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Krista.FM.Client.Common.Wizards.WizardForm wizardForm;
        private Krista.FM.Client.Common.Wizards.WizardPageBase wizardPageName;
        private Krista.FM.Client.Common.Wizards.WizardPageBase wizardPageAttributes;
        private Krista.FM.Client.Common.Wizards.WizardPageBase wizardPageLevelNamingTemplate;
        private Krista.FM.Client.Common.Wizards.WizardWelcomePage wizardWelcomePage1;
        private Krista.FM.Client.Common.Wizards.WizardFinalPage wizardFinalPage1;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor u_tb_Name;
        private Infragistics.Win.UltraWinTree.UltraTree ultraTree;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor u_tb_level;

    }
}