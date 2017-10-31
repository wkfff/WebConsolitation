using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace Plan
{
public partial class Fmmd_All : Form
{
    XmlDocument fmmdall;
    class DimensionInfo
    {
        public string DimensionName;
        public string TableName;
        public XmlNode Node;           
    }

    public Fmmd_All()
    {
        InitializeComponent();
        btnDelete.Enabled = false;
        btnSave.Enabled = false;
        btnSearch.Enabled = false;
    }

    private void btnOpen_Click(object sender, EventArgs e)
    {
        if (openFileDialog1.ShowDialog() == DialogResult.OK)
        {
            fmmdall = new XmlDocument();
            openFileDialog1.Filter = "XML files (*.xml)|*.XML|All files (*.*)|*.*";
            fmmdall.Load(openFileDialog1.FileName);
            //if (fmmdall.IsReadOnly)
            //{
            //    fmmdall.IsReadOnly = false;
            //}
             //FileInfo file = new FileInfo("D:\max.xml");
            //if (file.IsReadOnly){file.IsReadOnly = false;//сохраняем}
            btnSearch.Enabled = true;
            btnOpenFile.Enabled = false;
        }
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
        XmlNodeList nodesProperty;
        //nodesProperty = fmmdall.SelectNodes("//XMLDSOConverter//Databases//Database//DatabaseDimensions//DatabaseDimension//property");
        XmlNodeList nodes;
        nodes = fmmdall.SelectNodes("//XMLDSOConverter//Databases//Database//DatabaseDimensions//DatabaseDimension");
        string[] tableD = { "," };
        string[] splitedT;
        if (nodes != null)
        {
            string dimName;
            DimensionInfo dimInfo;
            for (int i = 0; i < nodes.Count; i++)
            {
                dimName = nodes[i].Attributes.GetNamedItem("name").Value.ToUpper();
                if (dimName.IndexOf("ПЛАН") > 0)
                {
                    leftTree.Nodes.Add(dimName);
                    dimInfo = new DimensionInfo();
                    dimInfo.DimensionName = dimName;
                    dimInfo.Node = nodes[i];
                    leftTree.Nodes[leftTree.Nodes.Count - 1].Tag = dimInfo;
                    XmlNode nodeFromClause = nodes[i].SelectSingleNode("./property[@name='FromClause']");
                    if (nodeFromClause != null)
                    {
                        string Tables = nodeFromClause.InnerText;
                        splitedT = Tables.Split(tableD, StringSplitOptions.RemoveEmptyEntries);

                        if (splitedT.Length > 1)
                        {
                            dimInfo.TableName = splitedT[0];
                        }
                        else
                        {
                            dimInfo.TableName = nodeFromClause.InnerText;
                        }
                        string DimensionName = nodes[i].SelectSingleNode("@name").Value;
                    }

                }
            }
        }
        btnSearch.Enabled = false;
        btnDelete.Enabled = true;
   }
    private void xml(object sender, CancelEventArgs e)
    {
        
    }
    private void button3_Click(object sender, EventArgs e)
    {
        TreeNode node = leftTree.SelectedNode;
        if (node != null )
        {
            leftTree.SelectedNode.Remove();
            rightTree.Nodes.Add(node);

        }
    }

    private void button4_Click(object sender, EventArgs e)
    {
        TreeNode node = rightTree.SelectedNode;
        if (node != null && node.IsSelected != false)
        {
            rightTree.SelectedNode.Remove();
            leftTree.Nodes.Add(node);
        }
    }

    private void button5_Click(object sender, EventArgs e)
    {
        while (leftTree.Nodes.Count>0)
        {
            TreeNode node = leftTree.Nodes[0];
            leftTree.Nodes[0].Remove();
            rightTree.Nodes.Add(node);
        }
    }

    private void button6_Click(object sender, EventArgs e)
    {
        while (rightTree.Nodes.Count > 0)
        {
            TreeNode node = rightTree.Nodes[0];
            rightTree.Nodes[0].Remove();
            leftTree.Nodes.Add(node); 
        }
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
        XmlNodeList nodesCubeProperty = fmmdall.SelectNodes("//XMLDSOConverter//Databases//Database//Cubes//Cube//property");
        XmlNodeList nodesCube = fmmdall.SelectNodes("//XMLDSOConverter//Databases//Database//Cubes//Cube");
        XmlNodeList nodesPartition = fmmdall.SelectNodes("//XMLDSOConverter//Databases//Database//Cubes//Cube//Partitions//Partition");
        XmlNodeList nodesProperty = fmmdall.SelectNodes("//XMLDSOConverter//Databases//Database//Cubes//Cube//CustomProperties");
        XmlNode dimensionsNode = fmmdall.SelectSingleNode("//XMLDSOConverter//Databases//Database//DatabaseDimensions");
        XmlNode node = fmmdall.SelectSingleNode("//XMLDSOConverter");
        XmlComment commentElement = fmmdall.CreateComment("Это комментарий :   ");
        XmlNode commentNode = node.InsertBefore(commentElement, node.FirstChild);
        commentElement.Value = commentNode.Value + "\r\n";
        XmlNodeList nodes = fmmdall.SelectNodes("//XMLDSOConverter//Databases//Database//DatabaseDimensions//DatabaseDimension");
        DimensionInfo dimensionInfo;
        String nodeDim;
        string Table;
        XmlNode cubeDimensions;
        for (int i = 0; i < rightTree.Nodes.Count; i++)
        {     
            dimensionInfo = (DimensionInfo)rightTree.Nodes[i].Tag;                
            node = dimensionInfo.Node;
            commentNode.Value = commentNode.Value + "      Измерение : " + node.Attributes.GetNamedItem("name").Value.ToUpper() + "\r\n";
            dimensionsNode.RemoveChild(node);
            nodeDim = dimensionInfo.DimensionName;
            string[] expressionDelimiters = { ") AND (", "(", ")", };
            string[] splitedTables;
            string[] tableDelimiter = { "=" };
            string[] splitedTable;
            string[] expressionDelimitersS = { "(", ")",")(" };
            string[] splitedTablesS;
            string[] tableDelimiterS = { "{","}","}{" };
            string[] splitedTableS;
            for (int j = 0; j < nodesCube.Count; j++)
            {
                XmlNode nodeFromClause = nodesCube[j].SelectSingleNode("./property[@name='FromClause']");
                XmlNode nodeJoinClause = nodesCube[j].SelectSingleNode("./property[@name='JoinClause']");                    
                if (nodeFromClause != null)                    
                {
                    //string cubeName = nodesCube[j].SelectSingleNode("@name").Value;
                    Table = nodeFromClause.InnerText;
                    Table = Table.Replace(dimensionInfo.TableName + ",", "");
                    Table = Table.Replace(dimensionInfo.TableName, "");
                    nodeFromClause.InnerText = Table;
                 }
                if (nodeJoinClause != null)
                {
                    Table = nodeJoinClause.InnerText;
                    splitedTables = Table.Split(expressionDelimiters, StringSplitOptions.RemoveEmptyEntries);
                    for (int k = 0; k < splitedTables.Length; k++)
                    {
                        Table = splitedTables[k];
                        splitedTable = Table.Split(tableDelimiter, StringSplitOptions.RemoveEmptyEntries);
                        if (splitedTable[0].IndexOf(dimensionInfo.TableName) >= 0 || splitedTable[1].IndexOf(dimensionInfo.TableName) >= 0)
                        {
                            nodeJoinClause.InnerText = nodeJoinClause.InnerText.Replace("(" + splitedTable[0] + "=" + splitedTable[1] + ")" + " AND ", "");
                            nodeJoinClause.InnerText = nodeJoinClause.InnerText.Replace("(" + splitedTable[0] + splitedTable[1] + ")", "");
                            nodeJoinClause.InnerText = nodeJoinClause.InnerText.Replace(" AND " + "(" + splitedTable[0] + "=" + splitedTable[1] + ")", "");
                            //string expressionToReplace = String.Format("({0}={1})", splitedTable[0], splitedTable[1]);
                            //nodeJoinClause.InnerText = nodeJoinClause.InnerText.Replace(expressionToReplace + " AND ", "");
                            //nodeJoinClause.InnerText = nodeJoinClause.InnerText.Replace(expressionToReplace, "");
                        }
                    }
                }
                cubeDimensions = nodesCube[j].SelectSingleNode("CubeDimensions");
                XmlNodeList cubeDimensionsList = cubeDimensions.SelectNodes("CubeDimension");
                for (int l = 0; l < cubeDimensionsList.Count; l++)
                {
                    if (cubeDimensionsList[l].Attributes.GetNamedItem("name").Value.ToUpper() == nodeDim)
                    {
                        //commentNode.Value = commentNode.Value + "           Измерение в кубе : " + cubeDimensionsList[l].Attributes.GetNamedItem("name").Value.ToUpper() + "\r\n";
                        cubeDimensions.RemoveChild(cubeDimensionsList[l]);
                    }
                }
            }
            for (int j = 0; j < nodesPartition.Count; j++)
            {
                XmlNode nodeFromClause = nodesPartition[j].SelectSingleNode("./property[@name='FromClause']");
                XmlNode nodeJoinClause = nodesPartition[j].SelectSingleNode("./property[@name='JoinClause']");
                if (nodeFromClause != null)
                {
                    //string cubeName = nodesCube[j].SelectSingleNode("@name").Value;
                    Table = nodeFromClause.InnerText;
                    Table = Table.Replace(dimensionInfo.TableName + ",", "");
                    Table = Table.Replace(dimensionInfo.TableName, "");
                    nodeFromClause.InnerText = Table;
                }
                if (nodeJoinClause != null)
                {
                    Table = nodeJoinClause.InnerText;
                    splitedTables = Table.Split(expressionDelimiters, StringSplitOptions.RemoveEmptyEntries);
                    for (int k = 0; k < splitedTables.Length; k++)
                    {
                        Table = splitedTables[k];
                        splitedTable = Table.Split(tableDelimiter, StringSplitOptions.RemoveEmptyEntries);
                        if (splitedTable[0].IndexOf(dimensionInfo.TableName) >= 0 || splitedTable[1].IndexOf(dimensionInfo.TableName) >= 0)
                        {
                            nodeJoinClause.InnerText = nodeJoinClause.InnerText.Replace("(" + splitedTable[0] + "=" + splitedTable[1] + ")" + " AND ", "");
                            nodeJoinClause.InnerText = nodeJoinClause.InnerText.Replace("(" + splitedTable[0] + splitedTable[1] + ")", "");
                            nodeJoinClause.InnerText = nodeJoinClause.InnerText.Replace(" AND " + "(" + splitedTable[0] + "=" + splitedTable[1] + ")", "");
                        }
                   }
                }
            }
            for (int j = 0; j < nodesProperty.Count; j++)
            {
                XmlNode nodeSchemaJoins = nodesProperty[j].SelectSingleNode("./Property[@name='SchemaJoins']");
                if (nodeSchemaJoins != null)
                {
                    Table = nodeSchemaJoins.InnerText;
                    splitedTables = Table.Split(expressionDelimiters, StringSplitOptions.RemoveEmptyEntries);
                    for (int k = 0; k < splitedTables.Length; k++)
                    {
                        Table = splitedTables[k];
                        splitedTable = Table.Split(tableDelimiter, StringSplitOptions.RemoveEmptyEntries);
                        if (splitedTable[0].IndexOf(dimensionInfo.TableName) >= 0 || splitedTable[1].IndexOf(dimensionInfo.TableName) >= 0)
                        {
                            nodeSchemaJoins.InnerText = nodeSchemaJoins.InnerText.Replace("(" + splitedTable[0] + "=" + splitedTable[1] + ")" + " AND ", "");
                            nodeSchemaJoins.InnerText = nodeSchemaJoins.InnerText.Replace("(" + splitedTable[0] + splitedTable[1] + ")", "");
                            nodeSchemaJoins.InnerText = nodeSchemaJoins.InnerText.Replace(" AND " + "(" + splitedTable[0] + "=" + splitedTable[1] + ")", "");
                        }
                    }

                }
            }
            for (int j = 0; j < nodesProperty.Count; j++)
            {
                XmlNode nodeSchemaLayout = nodesProperty[j].SelectSingleNode("./Property[@name='SchemaLayout']");
                             
                if (nodeSchemaLayout != null)
                {
                    Table = nodeSchemaLayout.InnerText;
                    splitedTablesS = Table.Split(expressionDelimitersS, StringSplitOptions.RemoveEmptyEntries);
                    for (int k = 0; k < splitedTablesS.Length; k++)
                    {
                        Table = splitedTablesS[k];
                        splitedTableS = Table.Split(tableDelimiterS, StringSplitOptions.RemoveEmptyEntries);
                        //dimensionInfo.TableName = dimensionInfo.TableName.Replace("DV\".\"" ,"");
                        dimensionInfo.TableName = dimensionInfo.TableName.Replace("\"", "");

                            if (splitedTableS[1].IndexOf(dimensionInfo.TableName) >= 0) //|| splitedTableS[2].IndexOf(dimensionInfo.TableName) >= 0)
                            {
                                nodeSchemaLayout.InnerText = nodeSchemaLayout.InnerText.Replace("(" + "{" + splitedTableS[0] + "}" + "{" + splitedTableS[1] + "}" + "{" + splitedTableS[2] + "}" + "{" + splitedTableS[3] + "}" + "{" + splitedTableS[4] + "}" + "{" + splitedTableS[5] + "}" + "{" + splitedTableS[6] + "}" + "{" + splitedTableS[7] + "}" + ")", "");
                                //nodeSchemaLayout.InnerText = nodeSchemaLayout.InnerText.Replace("(" + splitedTableS[0] + splitedTableS[1] + ")" , "");
                            }
                    } 
                }
                 
            }
       }
        
        while (0 < rightTree.Nodes.Count)
        {
            rightTree.Nodes[0].Remove();
        }
        btnSave.Enabled = true;
        btnDelete.Enabled = false;
  }
private void btnExit_Click(object sender, EventArgs e)
{
    Fmmd_All.ActiveForm.Close();
}

    private void btnSave_Click(object sender, EventArgs e)
    {
        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
        {
            saveFileDialog1.Filter = "XML files (*.xml)|*.XML|All files (*.*)|*.*";
            fmmdall.Save(saveFileDialog1.FileName);
        }
        MessageBox.Show("Файл успешно сохранен в каталоге \"" + saveFileDialog1.FileName + "\"",
                "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
        btnSave.Enabled = false;
    }

           


}

   
}