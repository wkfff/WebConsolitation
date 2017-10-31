using System;
using System.Windows.Forms;
using Krista.FM.Client.MDXExpert.Common;
using System.Reflection;
using Infragistics.Win.UltraWinTree;
using ADODB;
using System.Collections.Generic;

namespace Krista.FM.Client.MDXExpert.Controls
{    

    public partial class CubeChooser : Form
    {
        #region поля

        private string supplier;
        private string infoType;
        private string namePart;
        private string cubeName;
        private string description;
        private string calcTime;
        private bool isVirtual;
        private string selectedCube = "";
        private List<CubeInfo> cubesInfo;

        #endregion

        public CubeChooser()
        {
            InitializeComponent();
            panel5.Visible = false;
            splitter1.Visible = false;
            this.cubesInfo = new List<CubeInfo>();
        }

        public enum TreeNodeType
        {
            ntSupplier,
            ntInfoType,
            ntName
        }

        /// <summary>
        ///данные узла дерева
        /// </summary>
        public class PNodeRec
        {
            private TreeNodeType nodeType;
            private string cubeName;
            private string description;
            private string calcTime;
            private bool isVirtual;


            public TreeNodeType NodeType
            {
                get { return nodeType; }
                set { nodeType = value; }
            }

            public string CubeName
            {
                get { return cubeName; }
                set { cubeName = value; }
            }

            public string Description
            {
                get { return description; }
                set { description = value; }
            }

            public string CalcTime
            {
                get { return calcTime; }
                set { calcTime = value; }
            }

            public bool IsVirtual
            {
                get { return isVirtual; }
                set { isVirtual = value; }
            }

            public PNodeRec() 
            { 
            }
            /// <summary>
            /// Конструктор
            /// </summary>
            /// <param name="NodeType">Тип узла</param>
            /// <param name="CubeName">Полное имя куба</param>
            /// <param name="Description">Описание</param>
            /// <param name="CalcTime">Время последнего расчета</param>
            /// <param name="IsVirtual">признак виртуальности куба</param>
            public PNodeRec(TreeNodeType nodeType, string cubeName, string description, string calcTime, bool isVirtual)
            {
                this.nodeType = nodeType;
                this.cubeName = cubeName;
                this.description = description;
                this.calcTime = calcTime;
                this.isVirtual = isVirtual;
            }

        }

        /// <summary>
        /// Разделение полного имени куба на поставщика, тип информации и сокр.имя куба
        /// </summary>
        private void SplitCubeName()
        {   
            string[] tmpStrL = cubeName.Split('_');

            supplier = "";
            infoType = "";
            namePart = "";

            switch (tmpStrL.Length)
            {
                case 1:
                    namePart = tmpStrL[0];
                    break;
                case 2:
                    supplier = tmpStrL[0];
                    namePart = tmpStrL[1];
                    break;
                default:
                    supplier = tmpStrL[0];
                    infoType = tmpStrL[1];
                    namePart = tmpStrL[2];
                    for (int i = 3; i < tmpStrL.Length; i++)
                    {
                        namePart = namePart + "_" + tmpStrL[i];
                    }
                    break;
            }

        }
        
        /// <summary>
        /// Поиск узла в дереве
        /// </summary>
        /// <param name="Node">Первый узел, с кот. начинается поиск по сестрам</param>
        /// <param name="NodeName">имя узла, кот. ищем</param>
        /// <param name="NodeType">тип узла, кот ищем</param>
        /// <returns>Если нашли узел, то возвращаем его, иначе null</returns>
        private UltraTreeNode FindNode (UltraTreeNode node, string nodeName, TreeNodeType nodeType)
       {
           while(node != null)
           {
               if (node.Text == nodeName && ((PNodeRec)node.Tag).NodeType == nodeType) 
               {
                   return node; 
               }
               node = node.GetSibling(NodePosition.Next);
           }
           return node;
       }

        /// <summary>
        /// Добавление узла к дереву
        /// </summary>
        /// <param name="ParentNode">Узел-родитель, к которому будем добавлять</param>
        /// <param name="NodeType">Тип узла</param>
        /// <param name="ObjName">Имя узла</param>
        /// <param name="imIndex1">индекс картинки для узла</param>
        /// <param name="imIndex2">индекс альтернативной картинки</param>
        /// <returns>добавленый узел</returns>
        private UltraTreeNode AddNode(UltraTreeNode parentNode, TreeNodeType nodeType, string objName,
             int imIndex1, int imIndex2)
        {
            UltraTreeNode node = new UltraTreeNode();

            node = null;
    
            if (objName != "")
            {
                if (parentNode != null)
                {
                    if (parentNode.Nodes.Count > 0)
                    {
                        node = FindNode(parentNode.Nodes[0], objName, nodeType);
                    }
                }
                else
                {   
                    if (tvCubes.Nodes.Count > 0)
                    {
                        node = FindNode(tvCubes.Nodes[0], objName, nodeType);
                    }
                }

                if (node == null)
                {
                    switch (nodeType)
                    {
                        case TreeNodeType.ntSupplier:
                        case TreeNodeType.ntInfoType:
                            if (parentNode == null)
                            {
                                node = tvCubes.Nodes.Add();
                            }
                            else
                            {
                                node = parentNode.Nodes.Add();
                            }

                            node.Tag = new PNodeRec(nodeType, "", "", "", false); 
                            break;

                        case TreeNodeType.ntName:
                            if (parentNode == null)
                            {
                                node = tvCubes.Nodes.Add();
                            }
                            else
                            {
                                node = parentNode.Nodes.Add();
                            }

                            node.Tag = new PNodeRec(nodeType, cubeName, description, calcTime, isVirtual);
                            break;
                        
                        default:
                            break;

                    }

                    node.Text = objName;

                    if (isVirtual)
                    { 
                        node.Override.NodeAppearance.Image = imIndex1;
                    }
                    else
                    {
                        node.Override.NodeAppearance.Image = imIndex2;
                    }

                }
            }
            return node;

        }

        /// <summary>
        /// Добавление узла поставщика
        /// </summary>
        /// <returns>Узел поставщик</returns>
        private UltraTreeNode AddSupplierNode()
        {
            return AddNode(null, TreeNodeType.ntSupplier, supplier, 2, 2);
        }

        /// <summary>
        /// Добавление узла тип информации 
        /// </summary>
        /// <param name="supplierNode">Поставщик(узел-родитель для узла тип информации)</param>
        /// <returns>Узел тип информации</returns>
        private UltraTreeNode AddInfoTypeNode(UltraTreeNode supplierNode)
        {
            return AddNode(supplierNode, TreeNodeType.ntInfoType, infoType, 3, 3);    
        }

        /// <summary>
        /// Добавление узла сокр. имя куба
        /// </summary>
        /// <param name="supplierNode">Узел поставщик</param>
        /// <param name="infoTypeNode">Узлел тип информации</param>
        /// <returns>Узел - сокр. имя куба</returns>
        private UltraTreeNode AddNameNode(UltraTreeNode supplierNode, UltraTreeNode infoTypeNode)
        {
            if (infoTypeNode != null)
            {
                return AddNode(infoTypeNode, TreeNodeType.ntName, namePart, 1, 0);
            }
            else
            {
                return AddNode(supplierNode, TreeNodeType.ntName, namePart, 1, 0);
            }
        }

        /// <summary>
        /// Добавление куба в дерево
        /// </summary>
        private void AddCubeToTree()
        {
            AddNameNode(AddSupplierNode(), AddInfoTypeNode(AddSupplierNode()));
        }

        public void ClearCubeList()
        {
            this.lvCubeList.Items.Clear();
            this.tvCubes.Nodes.Clear();
        }
        
        /*
        public void LoadMetadata(AdomdConnection connection)
        {
            try
            {
                if ((connection != null) && (connection.State == ConnectionState.Open))
                {
                    this.lvCubeList.Items.Clear();

                    for (int i = 0; i < connection.Cubes.Count; i++)
                    {
                        cubeName = connection.Cubes[i].Name;
                        description = connection.Cubes[i].Description;
                        calcTime = connection.Cubes[i].LastProcessed.ToString();

                        

                        this.lvCubeList.Items.Add(cubeName);
                        this.lvCubeList.Items[i].Value = cubeName;
                        this.lvCubeList.Items[i].SubItems[0].Value = description;
                        this.lvCubeList.Items[i].SubItems[1].Value = calcTime;

                        isVirtual = (connection.Cubes[i].Properties["CUBE_TYPE"].Value.ToString() == "VIRTUAL CUBE"); // (cubeRecordset.Fields["CUBE_TYPE"].Value.ToString() == "VIRTUAL CUBE");

                        if (isVirtual)
                        {
                            this.lvCubeList.Items[i].Appearance.Image = 1;
                        }
                        else
                        {
                            this.lvCubeList.Items[i].Appearance.Image = 0;
                        }

                        SplitCubeName();
                        AddCubeToTree();

                    }
                }
            }
           catch
            {
            }

        }

        /// <summary>
        /// Загрузка данных из схемы
        /// </summary>
        public void LoadMetadata()
        {
            ADODB.Connection con = new ADODB.Connection();
            ADODB.Recordset cubeRecordset = null;

            try
            {
                con.Open(Consts.TmpConnStr, "", "", -1);

                if (con.State == (int)ADODB.ObjectStateEnum.adStateOpen)
                {
                    cubeRecordset = con.OpenSchema(SchemaEnum.adSchemaCubes, Missing.Value, Missing.Value);

                    if (!cubeRecordset.BOF)
                    {
                        cubeRecordset.MoveFirst();
                    }
                    int i = 0;

                    this.lvCubeList.Items.Clear();

                    while (!cubeRecordset.EOF)
                    {
                        cubeName = cubeRecordset.Fields["CUBE_NAME"].Value.ToString();
                        description = cubeRecordset.Fields["DESCRIPTION"].Value.ToString();
                        calcTime = cubeRecordset.Fields["LAST_DATA_UPDATE"].Value.ToString();

                        this.lvCubeList.Items.Add(cubeName);
                        this.lvCubeList.Items[i].Value = cubeName;
                        this.lvCubeList.Items[i].SubItems[0].Value = description;
                        this.lvCubeList.Items[i].SubItems[1].Value = calcTime;

                        isVirtual = (cubeRecordset.Fields["CUBE_TYPE"].Value.ToString() == "VIRTUAL CUBE");

                        if (isVirtual)
                        {
                            this.lvCubeList.Items[i].Appearance.Image = 1;
                        }
                        else
                        {
                            this.lvCubeList.Items[i].Appearance.Image = 0;
                        }

                        SplitCubeName();
                        AddCubeToTree();


                        i++;
                        cubeRecordset.MoveNext();
                    }
                }
            }
            finally
            {
                if (cubeRecordset != null)
                {
                    if (cubeRecordset.State == (int)ADODB.ObjectStateEnum.adStateOpen)
                    {
                        cubeRecordset.Close();
                    }
                    cubeRecordset = null;
                }

                if (con != null)
                {
                    if (con.State == (int)ADODB.ObjectStateEnum.adStateOpen)
                    {
                        con.Close();
                    }
                    con = null;
                }
            }
        }
        */

        /// <summary>
        /// Загрузка данных из схемы
        /// </summary>
        public void LoadMetadata()
        {
            this.InitCubesInfo();
            ClearCubeList();

            for (int i = 0; i < this.cubesInfo.Count; i++)
            {
                CubeInfo cubeInfo = this.cubesInfo[i];

                this.cubeName = cubeInfo.Name;
                this.description = cubeInfo.Description;
                this.calcTime = cubeInfo.ProcessingDate.ToString();

                this.lvCubeList.Items.Add(this.cubeName);
                this.lvCubeList.Items[i].Value = this.cubeName;
                this.lvCubeList.Items[i].SubItems[0].Value = this.description;
                this.lvCubeList.Items[i].SubItems[1].Value = this.calcTime;

                this.lvCubeList.Items[i].Appearance.Image = cubeInfo.IsVirtual ? 1 : 0;

                SplitCubeName();
                AddCubeToTree();
            }
        }

        /// <summary>
        /// Проверяет дату процессинга куба в базе данных, если она свежее переданной в параметре вернем 
        /// false иначе true
        /// </summary>
        /// <param name="cubeName">имя куба, дату процессинга которого будем смотреть</param>
        /// <param name="lastAdomdConnetionDate">последняя дата переподключения c многомеркой</param>
        /// <returns>если состояние актуально true иначе false</returns>
        public bool IsActualProcessingDate(string cubeName, DateTime lastAdomdConnetionDate)
        {
            //если нет даты последнего обновления значит отчет новый
            if (cubeName == string.Empty || lastAdomdConnetionDate == null)
                return true;

            //пока информацию о кубак обновляем каждый раз при вызове метода
            this.InitCubesInfo();

            foreach (CubeInfo cubeInfo in this.cubesInfo)
            {
                if (cubeInfo.Name == cubeName)
                {
                    return cubeInfo.ProcessingDate < lastAdomdConnetionDate;
                }
            }
            return true;
        }

        private void InitCubesInfo()
        {
            //с начала очистим инфу о кубах
            this.cubesInfo.Clear();

            ADODB.Connection con = new ADODB.Connection();
            ADODB.Recordset cubeRecordset = null;

            try
            {
                con.Open(Consts.TmpConnStr, "", "", -1);

                if (con.State == (int)ADODB.ObjectStateEnum.adStateOpen)
                {
                    cubeRecordset = con.OpenSchema(SchemaEnum.adSchemaCubes, Missing.Value, Missing.Value);

                    if (!cubeRecordset.BOF)
                    {
                        cubeRecordset.MoveFirst();
                    }

                    while (!cubeRecordset.EOF)
                    {
                        string name = cubeRecordset.Fields["CUBE_NAME"].Value.ToString();
                        string description = cubeRecordset.Fields["DESCRIPTION"].Value.ToString();
                        DateTime processingDate = (DateTime)cubeRecordset.Fields["LAST_DATA_UPDATE"].Value;
                        bool isVirtual = (cubeRecordset.Fields["CUBE_TYPE"].Value.ToString() == "VIRTUAL CUBE");

                        this.cubesInfo.Add(CubeInfo.GetCubeInfo(name, processingDate, description, isVirtual));

                        cubeRecordset.MoveNext();
                    }
                }
            }
            catch(Exception exc)
            {
                CommonUtils.ProcessException(exc);
            }
            finally
            {
                if (cubeRecordset != null)
                {
                    if (cubeRecordset.State == (int)ADODB.ObjectStateEnum.adStateOpen)
                    {
                        cubeRecordset.Close();
                    }
                    cubeRecordset = null;
                }

                if (con != null)
                {
                    if (con.State == (int)ADODB.ObjectStateEnum.adStateOpen)
                    {
                        con.Close();
                    }
                    con = null;
                }
            }
        }
        /*
        /// <summary>
        /// Поиск куба в дереве
        /// </summary>
        /// <param name="node">Начальный узел</param>
        /// <param name="cubeName">Имя куба</param>
        /// <returns>узел, если его нашли, иначе - null</returns>
        private UltraTreeNode FindCubeNode(UltraTreeNode node, string cubeName)
        {
            UltraTreeNode tmpNode = new UltraTreeNode();
            tmpNode = null;

            while (node != null)
            {
                if (((node.Tag as PNodeRec).NodeType == TreeNodeType.ntName) &&
                    ((node.Tag as PNodeRec).CubeName.ToUpper() == cubeName.ToUpper()))
                {
                    tmpNode = node;
                    break;
                }
                else if (node.Nodes.Count > 0)
                {
                    tmpNode = FindCubeNode(node.Nodes[0], cubeName);
                }

                if (tmpNode == null)
                {
                    node = node.GetSibling(NodePosition.Next);
                }
                else
                {
                    break;
                }
            }
            return tmpNode;
        }
        */

        private UltraTreeNode FindCubeNode(UltraTreeNode node, string cubeName)
        {

            while (node != null)
            {
                if (((node.Tag as PNodeRec).NodeType == TreeNodeType.ntName) &&
                    ((node.Tag as PNodeRec).CubeName.ToUpper() == cubeName.ToUpper()))
                {
                    return node;
                }
                else
                {
                    if (node.Nodes.Count > 0)
                    {
                        UltraTreeNode result = FindCubeNode(node.Nodes[0], cubeName);
                        if (result != null)
                            return result;
                    }
                }
                node = node.GetSibling(NodePosition.Next);
            }
            return null;
        }





        /// <summary>
        /// Свойство - выбранное имя куба
        /// </summary>
        public string SelectedCubeName
        { 
            get
            {
                if (tabControl.SelectedTab == tabControl.Tabs[0])
                {
                    if (tvCubes.SelectedNodes.Count > 0)
                    {
                        if ((tvCubes.SelectedNodes[0].Tag as PNodeRec).NodeType == TreeNodeType.ntName)
                        {
                            selectedCube = (tvCubes.SelectedNodes[0].Tag as PNodeRec).CubeName;
                        }
                    }

                }
                else
                {
                    if (lvCubeList.SelectedItems.Count != 0)
                    {
                        selectedCube = lvCubeList.SelectedItems[0].Text;
                    }

                }
                return selectedCube;

            }
            set
            {
                string curStr;
                string inStr;

                inStr = value.ToUpper();
                
                for (int i = 0; i < lvCubeList.Items.Count; i++)
                {
                    curStr = lvCubeList.Items[i].Text.ToUpper();
                    if (curStr == inStr)
                    {
                       // lvCubeList.Items[i].IsSelected = true;
                       // lvCubeList.Items[i].IsActive = true;
                        selectedCube = value;
                        break;
                    }
                }



                UltraTreeNode tmpNode = new UltraTreeNode();

                if (tvCubes.Nodes.Count > 0)
                {
                    tmpNode = tvCubes.Nodes[0];
                    tmpNode = FindCubeNode(tmpNode, inStr);
                    if (tmpNode != null)
                    {
                        tmpNode.Selected = true;
                        //tvCubes.SelectedNodes[0] = tmpNode;
                        selectedCube = value;
                    }
                }

            }
        }
        
        /// <summary>
        /// Функция выбора куба. Запускает форму выбора куба
        /// </summary>
        /// <param name="defaultCubeName"> Значение по умолчанию. Если заданный куб есть в списке кубов, то выделяется при открытии формы</param>
        /// <returns>имя выбранного куба</returns>
        public string SelectCube(string defaultCubeName)
        {
            //LoadMetadata();
            if (defaultCubeName != "")
            {
                SelectedCubeName = defaultCubeName;
            }
            if (ShowDialog() == DialogResult.OK)
            {
                return SelectedCubeName;
            }
            else
            {
                return "";
            }
        }

        #region Обработчики

        private void tvCubes_DoubleClick(object sender, EventArgs e)
        {
            
            if (tvCubes.SelectedNodes.Count > 0)
            {
                if ((tvCubes.SelectedNodes[0].Tag as PNodeRec).NodeType == TreeNodeType.ntName)
                {
                    this.DialogResult = DialogResult.OK;
                }
            }
        }

        private void lvCubeList_DoubleClick(object sender, EventArgs e)
        {
            if (lvCubeList.SelectedItems.Count != 0)
            {
                this.DialogResult = DialogResult.OK;
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            tabControl_SelectedTabChanged(sender, null);
            label5.Text = "База данных: " + Consts.tmpCatalogName + " (" + Consts.tmpServerName + ")";
        }

        private void tvCubes_AfterSelect(object sender, Infragistics.Win.UltraWinTree.SelectEventArgs e)
        {
            if (e.NewSelections.Count == 0)
            {
                return;
            }

            PNodeRec nodeData;

            nodeData = (PNodeRec)e.NewSelections[0].Tag;

            if (nodeData.NodeType == TreeNodeType.ntName)
            {
                lCubeName.Text = nodeData.CubeName;
                lCalcTime.Text = nodeData.CalcTime;
                if (nodeData.Description != "")
                {
                    lDescription.Text = nodeData.Description;
                }
                else
                {
                    lDescription.Text = "Нет описания";
                }
                panel5.Visible = true;
                splitter1.Visible = true;
            }
            else
            {
                panel5.Visible = false;
                splitter1.Visible = false;
            }

        }

        private void tabControl_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
        {
            switch (tabControl.SelectedTab.Index)
            {
                case 0:
                    {
                        tvCubes.Select();
                        break;
                    }
                case 1:
                    {
                        lvCubeList.Select();
                        break;
                    }
            }
        }

        #endregion

        struct CubeInfo
        {
            public string Name;
            public DateTime ProcessingDate;
            public string Description;
            public bool IsVirtual;

            static public CubeInfo GetCubeInfo(string name, DateTime processingDate, string description,
                bool isVirtual)
            {
                CubeInfo cubeInfo = new CubeInfo();
                cubeInfo.Name = name;
                cubeInfo.ProcessingDate = processingDate;
                cubeInfo.Description = description;
                cubeInfo.IsVirtual = isVirtual;

                return cubeInfo;
            }
        }
    }
}