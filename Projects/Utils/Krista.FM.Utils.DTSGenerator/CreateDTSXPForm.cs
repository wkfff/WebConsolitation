using System;
using System.ComponentModel;
using System.Windows.Forms;
using Infragistics.Win.UltraWinTree;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.Utils.DTSGenerator.TreeObjects;

namespace Krista.FM.Utils.DTSGenerator
{
    public partial class CreateDTSXPForm : Form
    {
        private readonly SSISPackageNode rootPackageNode;

        private readonly SSISPackageObject packageObject;

        private readonly IScheme sourceScheme;

        private readonly IDatabase destinationDatabase;

        private readonly IScheme destinationScheme;

        private LogicalCallContextData _context;

        #region - Constructors -

        public CreateDTSXPForm(LogicalCallContextData context, SSISPackageNode rootPackageNode, IScheme sourceScheme, IScheme destinationScheme, IDatabase database)
            : this (context, sourceScheme, destinationScheme, database)
        {
            this.rootPackageNode = rootPackageNode;
        }

        public CreateDTSXPForm(LogicalCallContextData context, SSISPackageObject packageObject, IScheme sourceScheme, IScheme destinationScheme, IDatabase database)
            : this(context, sourceScheme, destinationScheme, database)
        {
            this.packageObject = packageObject;
        }

        public CreateDTSXPForm(LogicalCallContextData context, IScheme sourceScheme, IScheme destinationScheme, IDatabase database)
        {
            this.sourceScheme = sourceScheme;
            this.destinationScheme = destinationScheme;
            destinationDatabase = database;

            _context = context;

            InitializeComponent();

            InitializeBackGroundWorker();
        }

        #endregion - Constructors -

        private void InitializeBackGroundWorker()
        {
            backgroundWorker.DoWork += backgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
        }

        void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, e.Error.Source);
            }
            else
            {
                UltraTree.Nodes.Add("Обработка завершена");
            }
        }

        void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            SSISPackageNode ssisPackageNode = e.Argument as SSISPackageNode;
            IntegrationScheme ssis = new IntegrationScheme(_context, sourceScheme, destinationScheme,
                                                           (ssisPackageNode == null)
                                                               ? packageObject.Name
                                                               : ssisPackageNode.Text, this, destinationDatabase);

            if (ssisPackageNode != null)
                ssis.AnalisysAllScheme(ssisPackageNode);
            if (packageObject != null)
                ssis.AnalisysScheme(packageObject);
        }
       
        public UltraTree UltraTree
        {
            get { return ultraTree; }
        }

        private void CreateDTSXPForm_Load(object sender, EventArgs e)
        {
            UltraTree.Nodes.Add("Начало обработки...");

            backgroundWorker.RunWorkerAsync(rootPackageNode);
        }
    }

    public enum StatusValidate
    {
        /// <summary>
        /// Успешная валидация
        /// </summary>
        Sucsess = 0,
        /// <summary>
        /// Неуспешная валидация
        /// </summary>
        Failure = 1
    }

    public class CreateUltraTreeNode : UltraTreeNode
    {
        private readonly StatusValidate validate;


        public CreateUltraTreeNode(StatusValidate validate)
        {
            this.validate = validate;
            InitializeLeftImages();
        }

        private void InitializeLeftImages()
        {
            switch(validate)
            {
                case StatusValidate.Sucsess:
                    //this.LeftImages.Add();
                    break;
                case StatusValidate.Failure:
                    // this.LeftImages.Add();
                    break;
            }
        }
    }

    /// <summary>
    /// Просто информационный узел
    /// </summary>
    public class InformationNode : UltraTreeNode
    {
        public InformationNode(string key) 
            : base(key)
        {
        }

        public InformationNode(string key, NodeClass clas, NodeType type)
            :this(key) 
        {
            Imagebyclass(clas);

            switch(type)
            {
                case NodeType.information:
                    LeftImages.Add(Properties.Resources.information);
                    break;
                case NodeType.savepackage:
                    LeftImages.Add(Properties.Resources.Сохранить_16х16_Вариант1);
                    break;
                case NodeType.start:
                    LeftImages.Add(Properties.Resources.start);
                    break;
            }
        }

        private void Imagebyclass(NodeClass clas)
        {
            switch(clas)
            {
                case NodeClass.packageTask:
                    LeftImages.Add(Properties.Resources.executepackagetask);
                    break;
                case NodeClass.dataflowTask:
                    LeftImages.Add(Properties.Resources.dataflowtask);
                    break;
                case NodeClass.scriptTask:
                    LeftImages.Add(Properties.Resources.executesqltask);
                    break;
            }
        }
    }

    /// <summary>
    /// Задача на перенос пакета
    /// </summary>
    public class PackageTaskNode : CreateUltraTreeNode
    {
        public PackageTaskNode(StatusValidate validate, NodeClass cls) 
            : base(validate)
        {
            Imagebyclass(cls);

            switch(validate)
            {
                case StatusValidate.Sucsess:
                    LeftImages.Add(Properties.Resources.validationSucess);
                    break;
                case StatusValidate.Failure:
                    LeftImages.Add(Properties.Resources.validationFailure);
                    break;
            }
        }

        private void Imagebyclass(NodeClass clas)
        {
            switch (clas)
            {
                case NodeClass.packageTask:
                    LeftImages.Add(Properties.Resources.executepackagetask);
                    break;
                case NodeClass.dataflowTask:
                    LeftImages.Add(Properties.Resources.dataflowtask);
                    break;
                case NodeClass.scriptTask:
                    LeftImages.Add(Properties.Resources.executesqltask);
                    break;
            }
        }
    }
}