﻿namespace MDXParser
{
    using System;
    using System.Text;

    public class MDXScriptNode : MDXListNode<MDXStatementNode>
    {
        internal void Add(MDXStatementNode stmt)
        {
            if (stmt != null)
            {
                base.Add(stmt);
            }
        }

        internal override void AppendMDX(StringBuilder mdx, Formatter f, int indent, bool comma)
        {
            base.AppendComment(mdx, f, indent);
            foreach (MDXStatementNode node in base.m_List)
            {
                node.AppendMDX(mdx, f, indent);
                mdx.Append("");
            }
        }

        public override void BuildDependencyGraph(DependencyGraph g, Vertex root)
        {
            foreach (MDXStatementNode node in this)
            {
                node.BuildDependencyGraph(g, root);
            }
        }

        internal override void FillParseTree(TreeNodeCollection Nodes)
        {
            foreach (MDXStatementNode node in base.m_List)
            {
                node.FillParseTree(Nodes);
            }
        }

        internal override MDXConstruct GetConstruct()
        {
            return MDXConstruct.Script;
        }
    }
}

