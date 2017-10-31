using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using MASSpace = Microsoft.AnalysisServices.AdomdClient;
using TreeSpace = Infragistics.WebUI.UltraWebNavigator;
using System.ComponentModel;

namespace Krista.FM.Server.Dashboards.Components
{
    [Serializable]
    struct NodeKey
    {        
        public string Name;        
        public string UName;
    }

    [Serializable]
    public enum DataProviderKind
    {
        Primary = 0, 
        Secondary = 1
    }

    public partial class DimensionTree : UserControl
    {
        private string cubeName;
        public string CubeName
        {
            get { return cubeName; }
            set { cubeName = value; }
        }
        private string hierarchyName;
        public string HierarchyName
        {
            get { return hierarchyName; }
            set { hierarchyName = value; }                        
        }

        private string defaultMember;
        public string DefaultMember
        {
            get { return defaultMember; }
            set { defaultMember = value; }
        }

        private DataProviderKind providerKind = DataProviderKind.Primary;
        public DataProviderKind ProviderKind
        {
            get { return providerKind; }
            set { providerKind = value; }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            treeControl.CheckBoxes = true;
        }
        
        protected void Page_Init(object sender, EventArgs e)
        {
            CustomReportPage ownerPage = (CustomReportPage)Page;
            if (ProviderKind == DataProviderKind.Primary)
            {
               // adoMDconnection = DataProvidersFactory.PrimaryMASDataProvider.Connection;
            }
            else
            {
               // adoMDconnection = DataProvidersFactory.SecondaryMASDataProvider.Connection;
            }
            

            if (!ownerPage.PageParams.Contains(this))
            {
                ownerPage.PageParams.Add(this);
            }

            if (!Page.IsPostBack)
            {
                InitTree();
            }                    
        }
        
        //подключение
        private MASSpace.AdomdConnection adoMDconnection;
        public MASSpace.AdomdConnection AdoMDconnection
        {
            get { return adoMDconnection; }
            set { adoMDconnection = value; }
        }

        private bool multipleChoice = true;
        /// <summary>
        /// ? Множественный выбор элементов
        /// </summary>        
        public bool MultipleChoice
        {
            get { return multipleChoice;}
            set 
            {
                multipleChoice = value;                
            }
        }


        [NotifyParentProperty(true)]        
        public Unit Height
        {
            get {return treeControl.Height;}
            set 
            {
                treeControl.Height = value;
         
            }
        }

        [NotifyParentProperty(true)]
        public Unit Width
        {
            get {return treeControl.Width;}
            set 
            {
                //this.Width = value;
                
                treeControl.Width = value;                                                
         
            }
        }
         

        
        /// <summary>
        /// Получение объекта иерархии
        /// </summary>
        /// <param name="cubeUN">юник-нэйм куба</param>
        /// <param name="hierarchyUN">юник нэйм иерархии</param>
        /// <returns></returns>
        public MASSpace.Hierarchy GetHierarchyByUniqueName(string cubeUN, string hierarchyUN)
        {
            if (adoMDconnection.State == ConnectionState.Open)
            {
                MASSpace.CubeDef cube = adoMDconnection.Cubes[cubeUN];
                if (cube != null)
                {
                    foreach (MASSpace.Dimension dim in cube.Dimensions)
                    {
                        foreach (MASSpace.Hierarchy h in dim.Hierarchies)
                        {
                            if (h.UniqueName == hierarchyUN)
                            {
                                return h;
                            }
                        }
                    }                            
                }
            }            
            return null;
        }
        
        /// <summary>
        /// Загрузка мемберов в дерево
        /// </summary>
        /// <param name="mems">Элементы измерения</param>
        /// <param name="branch">Ветка, в которую нужно добавлять</param>
        private void LoadMembers(MASSpace.MemberCollection mems, TreeSpace.Nodes branch, bool Recursive)
        {
            foreach (MASSpace.Member mem in mems)
            {
                //добавляем мембер
                string nodeName = string.IsNullOrEmpty(mem.Caption) ? "(Пусто)" : mem.Caption;
                TreeSpace.Node node = branch.Add(nodeName);                                
                
                NodeKey nk = new NodeKey();
                nk.Name = mem.Name;
                nk.UName = mem.UniqueName;                
                node.Tag = nk;

                bool IsActiveNode = ((mem.UniqueName == defaultMember) && !Page.IsPostBack);

                node.Checked = IsActiveNode;
                if (IsActiveNode) 
                {

                    node.Selected = true;
                }

                node.LoadOnDemand = (mem.ChildCount > 0);
                
                //обрабатываем детей, если рекурсивно
                if (Recursive)
                {                
                    LoadMembers(mem.GetChildren(), node.Nodes, true);                    
                }
            }
        } 
        
       /// <summary>
       /// Выставляет флажок элемента в группе узлов (у остальных снимает);
       /// </summary>
       private void SetCheckedNode(TreeSpace.Nodes branch, string MemUname)
       {
            if (branch != null)
            {
                foreach (TreeSpace.Node treeNode in branch)
                {
                    treeNode.Checked = (((NodeKey)treeNode.Tag).UName == MemUname);
                    SetCheckedNode(treeNode.Nodes, MemUname);
                    
                }
            }            
       }
       
       /// <summary>
       /// Выставляем чекнутый элемент
       /// </summary>
       public void SetChecked(string MemUname)
       {
            SetCheckedNode(treeControl.Nodes, MemUname);
       }
        
        /// <summary>
        /// Инициализация дерева
        /// </summary>
        public void InitTree()
        {
            treeControl.ClearAll();            
            if (string.IsNullOrEmpty(cubeName) || string.IsNullOrEmpty(hierarchyName)) return;

            MASSpace.Hierarchy h = GetHierarchyByUniqueName(cubeName, hierarchyName);
            if (h != null)
            {
                LoadMembers(h.Levels[0].GetMembers(), treeControl.Nodes, true);
                /*
                //если корень один, тогда откроем его и качнем еще один уровень
                if (h.Levels[0].GetMembers().Count == 1)
                {
                    TreeSpace.Node root = treeControl.Nodes[0];
                    LoadMembers(h.Levels[0].GetMembers()[0].GetChildren(), root.Nodes, false);
                    root.Expand(false);
                } */
            }                
        }

        /// <summary>
        /// Выбранные элементы в терминах MDX.
        /// В зависимости от MultipleChoice это либо сет либо один мембер
        /// </summary>
        public string ChoiceSet
        {
            get
            {
                string elementEnum = string.Empty;
                
                if (MultipleChoice)
                {
                    foreach (object elem in treeControl.CheckedNodes)
	                {
	                    if (!string.IsNullOrEmpty(elementEnum)) elementEnum += ", ";
		                elementEnum += ((NodeKey)((TreeSpace.Node)elem).Tag).UName;
	                }
                    elementEnum = "{" + elementEnum + "}";
                }
                else
                {
                    try
                    {
                        elementEnum = ((NodeKey)((TreeSpace.Node)treeControl.CheckedNodes[0]).Tag).UName;
                    }
                    catch
                    {
                    }
                }

                return elementEnum;
            }
        }
        
        public string Selected
        {
            get
            {
                return ((NodeKey)treeControl.SelectedNode.Tag).UName;
            }
        }

        protected void treeControl_DemandLoad(object sender, Infragistics.WebUI.UltraWebNavigator.WebTreeNodeEventArgs e)
        {
            
        
            TreeSpace.Node node = e.Node;
            string memName = ((NodeKey)node.Tag).Name;

            MASSpace.Hierarchy h = GetHierarchyByUniqueName(cubeName, hierarchyName);
            if (h != null)
            {
                MASSpace.MemberCollection mc = h.Levels[node.Level].GetMembers();
                LoadMembers(mc[memName].GetChildren(), node.Nodes, true);
            }                
            e.Node.LoadOnDemand = true;                                                                         
        }
    }
}