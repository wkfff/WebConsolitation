namespace MDXParser
{
    using System;
    using System.Text;

    internal class MDXFunctionNode : MDXBaseFunctionNode
    {
        internal MDXFunctionNode(string FunctionName, MDXExpListNode ExpList, MDXObject obj, bool builtin) : base(FunctionName, ExpList, obj, builtin)
        {
            if ((((base.m_Function.Equals("sum", StringComparison.CurrentCultureIgnoreCase) || base.m_Function.Equals("aggregate", StringComparison.CurrentCultureIgnoreCase)) || (base.m_Function.Equals("min", StringComparison.CurrentCultureIgnoreCase) || base.m_Function.Equals("max", StringComparison.CurrentCultureIgnoreCase))) || ((base.m_Function.Equals("avg", StringComparison.CurrentCultureIgnoreCase) || base.m_Function.Equals("filter", StringComparison.CurrentCultureIgnoreCase)) || base.m_Function.Equals("order", StringComparison.CurrentCultureIgnoreCase))) && (base.m_Arguments.Count >= 2))
            {
                base.m_Arguments[1].SetOuterIterator(base.m_Arguments[0]);
            }
        }

        internal override void Analyze(Analyzer analyzer)
        {
            base.Analyze(analyzer);
            if (!base.m_IsCommonSubExpr)
            {
                if (base.m_Function.Contains("."))
                {
                    Message m = new Message(this);
                    m.Id = 30;
                    m.Text = string.Format("The usage of user-defined stored procedure '{0}' will disable block computation mode", base.m_Function);
                    m.Severity = 3;
                    m.URL = "http://sqlblog.com/blogs/mosha/archive/2007/04/19/best-practices-for-server-adomd-net-stored-procedures.aspx";
                    analyzer.Add(m);
                }
                if (base.m_Function.StartsWith("StrTo", StringComparison.InvariantCultureIgnoreCase))
                {
                    bool flag = false;
                    if (((base.m_Arguments.Count >= 2) && (base.m_Arguments[1].GetType() == typeof(MDXFlagNode))) && (base.m_Arguments[1] as MDXFlagNode).GetLabel().Equals("CONSTRAINED"))
                    {
                        flag = true;
                    }
                    if (!flag)
                    {
                        Message message2 = new Message(this);
                        message2.Id = 0x1f;
                        message2.Text = string.Format("'{0}' function without CONSTRAINED flag will disable block computation mode", base.m_Function);
                        message2.Severity = 3;
                        message2.URL = "http://crawlmsdn.microsoft.com/en-us/library/bb934106(SQL.100).aspx";
                        analyzer.Add(message2);
                    }
                }
                if (base.XofY("IIF", "IIF"))
                {
                    Message message3 = new Message(this);
                    message3.Id = 0x20;
                    message3.Text = string.Format("AS2008 only: Nested IIFs can be rewritten more efficiently as CASE operator", new object[0]);
                    message3.Severity = 2;
                    analyzer.Add(message3);
                }
                if ((((base.m_Function.Equals("Order", StringComparison.InvariantCultureIgnoreCase) || base.m_Function.Equals("Filter", StringComparison.InvariantCultureIgnoreCase)) || (base.m_Function.StartsWith("Top", StringComparison.InvariantCultureIgnoreCase) || base.m_Function.StartsWith("Bottom", StringComparison.InvariantCultureIgnoreCase))) || (base.m_Function.Equals("NonEmpty", StringComparison.InvariantCultureIgnoreCase) || base.m_Function.Equals("Exists", StringComparison.InvariantCultureIgnoreCase))) && base.XofY(base.m_Function, base.m_Function))
                {
                    Message message4 = new Message(this);
                    message4.Id = 0x21;
                    message4.Text = string.Format("Calling {0}({0}) is not good for performance, consider rewriting as single {0} call", base.m_Function);
                    message4.Severity = 2;
                    analyzer.Add(message4);
                }
                if (analyzer.InRHS && base.m_Function.Equals("Order", StringComparison.InvariantCultureIgnoreCase))
                {
                    Message message5 = new Message(this);
                    message5.Id = 0x22;
                    message5.Text = string.Format("Due to bug in Order function, consider rewriting Order(set,exp,BDESC) as TopCount(set,set.Count,exp)", new object[0]);
                    message5.Severity = 1;
                    message5.URL = "http://cwebbbi.spaces.live.com/blog/cns!7B84B0F2C239489A!1262.entry";
                    analyzer.Add(message5);
                }
                if (base.XofY("Filter", "CrossJoin"))
                {
                    Message message6 = new Message(this);
                    message6.Id = 0x23;
                    message6.Text = string.Format("Consider rewriting Filter(CrossJoin(set)) as CrossJoin(Filter()) for improved performance", new object[0]);
                    message6.Severity = 1;
                    analyzer.Add(message6);
                }
                if (base.XofY("Filter", "Order"))
                {
                    Message message7 = new Message(this);
                    message7.Id = 0x24;
                    message7.Text = string.Format("Consider changing Filter(Order(set)) to Order(Filter()) for better performance", new object[0]);
                    message7.Severity = 3;
                    analyzer.Add(message7);
                }
                if (base.XofY("NonEmpty", "Order"))
                {
                    Message message8 = new Message(this);
                    message8.Id = 0x25;
                    message8.Text = string.Format("Consider changing NonEmpty(Order(set)) to Order(NonEmpty()) for better performance", new object[0]);
                    message8.Severity = 3;
                    analyzer.Add(message8);
                }
                if (base.XofY("Count", "Filter"))
                {
                    Message message9 = new Message(this);
                    message9.Id = 0x26;
                    message9.Text = string.Format("Consider rewriting Count(Filter(set, cond)) construct as Sum(set, summator) with summator IIF(cond, 1, NULL)", new object[0]);
                    message9.URL = "http://sqlblog.com/blogs/mosha/archive/2007/11/22/optimizing-count-filter-expressions-in-mdx.aspx";
                    message9.Severity = 2;
                    analyzer.Add(message9);
                }
                if ((base.m_Function.Equals("Filter", StringComparison.InvariantCultureIgnoreCase) && (base.m_Arguments.Count > 1)) && (base.m_Arguments[1].GetType() == typeof(MDXBinOpNode)))
                {
                    bool flag2 = false;
                    MDXBinOpNode node = base.m_Arguments[1] as MDXBinOpNode;
                    if (node.m_Exp1.GetType() == typeof(MDXPropertyNode))
                    {
                        MDXPropertyNode node2 = node.m_Exp1 as MDXPropertyNode;
                        if (node2.m_Function.Equals("Properties", StringComparison.InvariantCultureIgnoreCase))
                        {
                            flag2 = true;
                        }
                    }
                    if (node.m_Exp2.GetType() == typeof(MDXPropertyNode))
                    {
                        MDXPropertyNode node3 = node.m_Exp2 as MDXPropertyNode;
                        if (node3.m_Function.Equals("Properties", StringComparison.InvariantCultureIgnoreCase))
                        {
                            flag2 = true;
                        }
                    }
                    if (flag2)
                    {
                        Message message10 = new Message(this);
                        message10.Id = 0x27;
                        message10.Text = string.Format("Consider rewriting Filter(set, h.Properties() {0} ...) using Exists function", node.m_Op);
                        message10.Severity = 2;
                        analyzer.Add(message10);
                    }
                }
                if (base.XofY("Rank", "Order"))
                {
                    Message message11 = new Message(this);
                    message11.Id = 40;
                    message11.Text = string.Format("Consider rewriting Rank(item, Order(set, sort_criteria)) as Rank(item, set, sort_criteria) or using DYNAMIC sets", new object[0]);
                    message11.URL = "http://sqlblog.com/blogs/mosha/archive/2006/03/14/ranking-in-mdx.aspx";
                    message11.Severity = 5;
                    analyzer.Add(message11);
                }
                if (base.XofY("Aggregate", ".ALLMEMBERS"))
                {
                    Message message12 = new Message(this);
                    message12.Id = 0x29;
                    message12.Text = string.Format("Aggregate(<o>.ALLMEMBERS) will break when there will be calculated members. Use Aggregate(<o>.MEMBERS) instead.", new object[0]);
                    message12.Severity = 5;
                    analyzer.Add(message12);
                }
                if (base.XofY("Tail", "Head") || base.XofY("Head", "Tail"))
                {
                    Message message13 = new Message(this);
                    message13.Id = 0x2a;
                    message13.Text = string.Format("Consider using MDX function SubSet instead of Tail(Head(...)) construct", new object[0]);
                    message13.Severity = 0;
                    analyzer.Add(message13);
                }
                if (base.XofY("Count", "Descendants"))
                {
                    Message message14 = new Message(this);
                    message14.Id = 0x2b;
                    message14.Text = string.Format("If you are trying to count number of members in current selection - consider introducing special measure group for this dimension", new object[0]);
                    message14.URL = "http://sqlblog.com/blogs/mosha/archive/2007/05/27/counting-days-in-mdx.aspx";
                    message14.Severity = 2;
                    analyzer.Add(message14);
                }
            }
        }

        internal override void AppendMDX(StringBuilder mdx, Formatter f, int indent, bool comma)
        {
            base.AppendCommentAndIndent(mdx, f, indent, comma);
            string str = (base.IsBuiltin && f.Options.ColorFunctionNames) ? f.Keyword(base.m_Function) : base.m_Function;
            mdx.Append(str);
            bool flag = false;
            string mDX = null;
            if (base.m_Arguments.Count != 0)
            {
                if (base.m_Arguments.Count == 1)
                {
                    mDX = base.m_Arguments[0].GetMDX(f, -1);
                    if ((indent >= 0) && ((mDX.Length + indent) > 70))
                    {
                        f.AppendIndent(mdx, indent);
                        flag = true;
                    }
                }
                else
                {
                    f.AppendIndent(mdx, indent);
                    flag = true;
                }
            }
            mdx.Append("(");
            bool flag2 = true;
            foreach (MDXExpNode node in base.m_Arguments)
            {
                f.AppendCommaAtTheEndOfLine(mdx, !flag2);
                if (base.m_Arguments.Count > 1)
                {
                    node.AppendMDX(mdx, f, f.Indent(indent), !flag2);
                }
                else if (flag)
                {
                    node.AppendMDX(mdx, f, f.Indent(indent));
                }
                else
                {
                    mdx.Append(mDX);
                }
                flag2 = false;
            }
            if (flag)
            {
                f.AppendIndent(mdx, indent);
            }
            mdx.Append(")");
        }

        public override MDXDataType GetMDXType()
        {
            if ((base.m_Object != null) && (base.m_Object.ReturnType != MDXDataType.Unknown))
            {
                return base.m_Object.ReturnType;
            }
            if (base.m_Function.Equals("Iif", StringComparison.InvariantCultureIgnoreCase))
            {
                if (base.m_Arguments.Count > 1)
                {
                    return base.m_Arguments[1].GetMDXType();
                }
                return MDXDataType.Number;
            }
            if (base.m_Function.Equals("Generate", StringComparison.InvariantCultureIgnoreCase))
            {
                if (base.m_Arguments.Count > 1)
                {
                    return base.m_Arguments[1].GetMDXType();
                }
                return MDXDataType.Set;
            }
            if (base.m_Function.Contains("!"))
            {
                return MDXDataType.Number;
            }
            if (base.m_Function.Contains("."))
            {
                return MDXDataType.Unknown;
            }
            return MDXDataType.Set;
        }

        internal override bool IsCacheTrivial()
        {
            return base.m_Function.StartsWith("KPI");
        }
    }
}

