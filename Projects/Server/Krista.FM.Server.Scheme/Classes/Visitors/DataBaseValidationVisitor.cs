using System;
using Krista.FM.Server.Scheme.ScriptingEngine;
using Krista.FM.Server.Scheme.ScriptingEngine.Classes;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.Classes.Visitors
{
    /// <summary>
    /// ������������� ���������� � �������� � ����������� ���� ������
    /// </summary>
    internal class DataBaseValidationVisitor : ValidationVisitor, IValidationVisitor
    {
        #region IValidationVisitor Members

        public void Visit(IPackage node)
        {
            foreach (IPackage item in node.Packages.Values)
            {
                Visit(item);
            }

            foreach (IEntity item in node.Classes.Values)
            {
                Visit(item);
            }

            foreach (IEntityAssociation item in node.Associations.Values)
            {
                if (item as IBridgeAssociation != null)
                {
                    Visit(item as IBridgeAssociation);
                }
                else
                {
                    Visit(item);
                }
            }
        }

        public void Visit(IEntity node)
        {
            
            EntityScriptingEngine entityScriptingEngine = SchemeClass.ScriptingEngineFactory.EntityScriptingEngine;

            // �������� �� ������� ����� �������
            if (entityScriptingEngine.ExistsObject
                (entityScriptingEngine.GeneratorName(node.FullDBName), ObjectTypes.Table))
                LogError(ErrorType.DataBaseError, node.ParentPackage, node,
                         String.Format("� ���� ������ ����������� ������� {0}", node.FullDBName));

            // �������� �� ������� ������������� (���� ��� �����)

            string shortNameHeader;
            string shortNameSelectPart;

            bool generateShortName =
                SchemeClass.ScriptingEngineFactory.ClassifierEntityScriptingEngine.GenerateShortNameParts(
                    (Classifier) node, out shortNameHeader, out shortNameSelectPart);
            if (!((((Classifier)node).Levels.HierarchyType == HierarchyType.Regular && ((Classifier)node).IsDivided) || generateShortName))
            {
                if (entityScriptingEngine.ExistsObject(ClassifierEntityScriptingEngine.GetViewName(node), ObjectTypes.View))
                {
                    LogError(ErrorType.DataBaseError, node.ParentPackage, node,
                             String.Format("� ���� ������ ����������� ������������� ������ {0} ��� �������������� {1}",
                                           ClassifierEntityScriptingEngine.GetViewName(node), node.FullCaption));
                }
            }
        }

        public void Visit(IEntityAssociation node)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Visit(IDocument node)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Visit(IDataAttribute node)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
