using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.Extensions;

namespace Krista.FM.RIA.Extensions.FO41.Helpers
{
    public class Node
    {
        public Node()
        {
            DependsOn = new List<Node>();
            Depended = new List<Node>();
        }

        public int ID { get; set; }

        public string Symbol { get; set; }

        public string PeriodName { get; set; }

        public string Formula { get; set; }

        public List<Node> DependsOn { get; set; }

        public List<Node> Depended { get; set; }

        public int DependentCount()
        {
            return Depended.Sum(node => node.DependentCount() + 1);
        }

        public void Print(string indent)
        {
            Console.Write(indent + Symbol);
            foreach (var node in Depended)
            {
                node.Print(indent + " ---> ");
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder(Symbol);
            if (Formula.IsNotNullOrEmpty())
            {
                sb.Append(" DependsOn=").Append(DependsOn.Count);
                sb.Append(" = ").Append(Formula);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Определяет, равен ли заданный объект <see cref="T:System.Object"/> текущему объекту <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true, если указанный объект <see cref="T:System.Object"/> равен текущему объекту <see cref="T:System.Object"/>; в противном случае — false.
        /// </returns>
        /// <param name="obj">Объект <see cref="T:System.Object"/>, который требуется сравнить с текущим объектом <see cref="T:System.Object"/>. 
        /// </param><exception cref="T:System.NullReferenceException">Параметр <paramref name="obj"/> имеет значение null.
        /// </exception><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Node))
            {
                return false;
            }

            var node = obj as Node;

            if (Symbol.Equals(node.Symbol) && PeriodName != null && PeriodName.Equals(node.PeriodName))
            {
                if (Formula == null)
                {
                    return true;
                }

                if (Formula.Equals(node.Formula))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
