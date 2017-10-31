using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Xml;
using System.Xml.XPath;
using Krista.FM.Client.TreeLogger.Properties;

namespace Krista.FM.Client.TreeLogger
{
	public class Log
	{
		private LogLongOperation globalOp;
		private frmLog loggerForm;

		private Log()
		{ }

		public Log(string logCaption)
		{
			loggerForm = new frmLog();
			loggerForm.log = this;
		}

		public LogLongOperation GlobalOperation
		{
			get { return globalOp; }
		}

		private TreeNode CreateTreeNode(string opName, string imageKey)
		{
			int imageIndex = loggerForm.imageListSmall.Images.IndexOfKey(imageKey);
			return new TreeNode(opName, imageIndex, imageIndex);
		}

		private TreeNode CreateOperationNode(string opName, OperationStatus opStatus)
		{
			return CreateTreeNode(opName, LoggerUtils.OpStatusToImageKey(opStatus));
		}

		private void CreateTimeNodes(LogLongOperation logLongOp)
		{
			logLongOp.durationTimeNode = CreateTreeNode(Resources.durationNodeCaption,
				"Time_16");
			logLongOp.startTimeNode = CreateTreeNode(Resources.startNodeCaption, "Time_16");
			logLongOp.endTimeNode = CreateTreeNode(Resources.endNodeCaption, "Time_16");
			AddNode(logLongOp.durationTimeNode.Nodes, logLongOp.startTimeNode, 0);
			AddNode(logLongOp.durationTimeNode.Nodes, logLongOp.endTimeNode, 1);
			AddNode(logLongOp.TreeNode.Nodes, logLongOp.durationTimeNode, 0);
		}

		private void CreateSummaryNodes(LogLongOperation logLongOp)
		{
			logLongOp.warningsNode = CreateTreeNode(Resources.warningNodeCaption,
				"Warning_16");
			logLongOp.errorsNode = CreateTreeNode(Resources.errorNodeCaption, "Error_16");

			loggerForm.InvokeSetFontStyle(ref logLongOp.warningsNode, FontStyle.Bold);
			loggerForm.InvokeSetFontStyle(ref logLongOp.errorsNode, FontStyle.Bold);

			AddNode(logLongOp.TreeNode.Nodes, logLongOp.warningsNode, 1);
			AddNode(logLongOp.TreeNode.Nodes, logLongOp.errorsNode, 2);
		}

		public void CreateWarningNode(LogLongOperation logLongOp)
		{			
			logLongOp.warningsNode = CreateTreeNode(Resources.warningNodeCaption,
				"Warning_16");
			AddNode(logLongOp.TreeNode.Nodes, logLongOp.warningsNode, 1);
		}

		public void CreateErrorNode(LogLongOperation logLongOp)
		{
			logLongOp.errorsNode = CreateTreeNode(Resources.errorNodeCaption, "Error_16");
			int nodeIndex = 1;
			if (logLongOp.warningsNode != null)
			{
				nodeIndex = 2;
			}

			AddNode(logLongOp.TreeNode.Nodes, logLongOp.errorsNode, nodeIndex);
		}

		private void AddNode(TreeNodeCollection nodes, TreeNode node)
		{
			AddNode(nodes, node, nodes.Count);
		}

		private void AddNode(TreeNodeCollection nodes, TreeNode node, int index)
		{
			loggerForm.InvokeAddNode(nodes, node, index);
		}

		private void AddChildOperation(LogOperation parentOp, LogOperation childOp)
		{
			if (childOp != null)
			{
				if (parentOp != null)
				{
					parentOp.Children.Add(childOp);
					if (parentOp.ShowBuffer == 0 ||
						((parentOp.Children.Count % parentOp.ShowBuffer) == 0))
						loggerForm.InvokeFlushBuffer(parentOp);
				}
				else
					AddNode(loggerForm.treeViewLog.Nodes, childOp.TreeNode);
			}
		}

		internal LogOperation AddOperation(LogOperation parentOp, string opName,
			OperationStatus opStatus, int showBuffer)
		{
			if (parentOp == null && globalOp != null) { parentOp = globalOp; }
			LogOperation logOp = new LogOperation(parentOp, opName, opStatus,
				this, CreateOperationNode(opName, opStatus), showBuffer);
			AddChildOperation(parentOp, logOp);
			return logOp;
		}

		internal LogLongOperation AddLongOperation(LogOperation parentOp, string opName,
			OperationStatus opStatus, int showBuffer, int progressMaximum)
		{
			if (parentOp == null && globalOp != null) { parentOp = globalOp; }
			DataGridViewRow opRow = null;
			if (progressMaximum >= 0)
				opRow = loggerForm.InvokeCreateOperationRow(opName, opStatus);
			LogLongOperation logLongOp = new LogLongOperation(parentOp, opName, opStatus,
				this, CreateOperationNode(opName, opStatus), showBuffer,
				progressMaximum, opRow);
			if (opRow != null) { opRow.Tag = logLongOp; }
			CreateTimeNodes(logLongOp);
			//CreateSummaryNodes(logLongOp);
			if (opStatus == OperationStatus.Running) { StartOperation(logLongOp); }
			AddChildOperation(parentOp, logLongOp);
			return logLongOp;
		}

		internal LogOperation AppendLogMessage(LogOperation parentOp, string logMessage,
			OperationStatus opStatus)
		{
			return AddOperation(parentOp, logMessage, opStatus, 0);
		}

		internal void EndOperation(LogOperation logOp, OperationStatus opStatus)
		{
			logOp.Status = opStatus;
			loggerForm.InvokeEndOperation(logOp, opStatus);
		}

		internal void EndOperation(LogLongOperation logLongOp)
		{
			EndOperation(logLongOp, DateTime.Now);
		}

		internal void EndOperation(LogLongOperation logLongOp, DateTime endTime)
		{
			loggerForm.InvokeEndLongOperation(logLongOp, OperationStatus.Undefined, endTime);
		}

		internal void EndOperation(LogLongOperation logLongOp, OperationStatus opStatus)
		{
			EndOperation(logLongOp, opStatus, DateTime.Now);
		}

		internal void EndOperation(LogLongOperation logLongOp, OperationStatus opStatus,
			DateTime endTime)
		{
			loggerForm.InvokeEndLongOperation(logLongOp, opStatus, endTime);
		}

		private void InitLog(string logCaption)
		{
			globalOp = AddLongOperation(null, logCaption, OperationStatus.Waiting, 0, -1);
			loggerForm.InvokeInitLog(logCaption);
		}

		public void ProgressUpdate()
		{
			loggerForm.InvokeProgressUpdate();
		}

		public void ResetLog(string logCaption)
		{
			globalOp = null;
			loggerForm.InvokeResetLog();
			InitLog(logCaption);
		}

		internal LogOperation ShowLog(IWin32Window owner, string logCaption)
		{
			loggerForm.InvokeShowLog(owner, logCaption, globalOp);
			return globalOp;
		}

		public LogOperation ShowLog(IWin32Window owner)
		{
			loggerForm.InvokeShowLog(owner, "", globalOp);
			return globalOp;
		}

		internal void StartOperation(LogLongOperation logLongOp)
		{
			StartOperation(logLongOp, DateTime.Now);
		}

		internal void StartOperation(LogLongOperation logLongOp, DateTime startTime)
		{
			if (logLongOp != null)
			{
				logLongOp.Status = OperationStatus.Running;
				loggerForm.InvokeStartOperation(logLongOp, startTime);
			}
		}

		public void ToggleButtons(bool buttonsEnabled)
		{
			loggerForm.InvokeToggleButtons(buttonsEnabled);
		}

		private void WriteNodeToXML(XmlWriter xmlWriter, LogOperation logOp)
		{
			xmlWriter.WriteStartElement("node");
			if (logOp is LogLongOperation)
				xmlWriter.WriteElementString("lognodetype",
					LogNodeType.logLongOperation.ToString());
			else
				xmlWriter.WriteElementString("lognodetype",
					LogNodeType.logOperation.ToString());

			xmlWriter.WriteElementString("name", logOp.Name);
			xmlWriter.WriteElementString("showbuffer", logOp.ShowBuffer.ToString());
			xmlWriter.WriteElementString("status", logOp.Status.ToString());
			if (logOp is LogLongOperation)
			{
				LogLongOperation logLongOp = logOp as LogLongOperation;
				xmlWriter.WriteElementString("progressmaximum",
					logLongOp.ProgressMaximum.ToString());
				if (logLongOp.startTimeNode.Tag == null)
					logLongOp.startTimeNode.Tag = DateTime.Now;
				xmlWriter.WriteElementString("starttime",
					((DateTime)logLongOp.startTimeNode.Tag).ToString());
				if (logLongOp.endTimeNode.Tag == null)
					logLongOp.endTimeNode.Tag = DateTime.Now;
				xmlWriter.WriteElementString("endtime",
					((DateTime)logLongOp.endTimeNode.Tag).ToString());
			}
			xmlWriter.WriteStartElement("children");
			for (int i = 0; i < logOp.Children.Count; i++)
				WriteNodeToXML(xmlWriter, logOp.Children[i]);
			xmlWriter.WriteEndElement();

			xmlWriter.WriteEndElement();
		}

		public void ToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("xmllog");
			WriteNodeToXML(xmlWriter, globalOp);
			xmlWriter.WriteEndElement();
		}

		public void SaveToXML(string fileName)
		{
			XmlWriter xmlWriter = XmlWriter.Create(fileName);
			xmlWriter.WriteStartDocument();
			try
			{
				ToXML(xmlWriter);
			}
			finally
			{
				xmlWriter.Flush();
				xmlWriter.WriteEndDocument();
			}
		}

		private void ReadNodeChildren(LogOperation parentNode, XPathNavigator nodeNavigator)
		{
			XPathNodeIterator children = nodeNavigator.Select("./children/node");
			while (children.MoveNext())
				ReadNodeFromXML(parentNode, children.Current);
		}

		private DateTime StrToDateTime(string value)
		{
			return DateTime.Parse(value);
		}

		private void ReadNodeFromXML(LogOperation parentNode, XPathNavigator nodeNavigator)
		{
			LogNodeType nodeType = (LogNodeType)Enum.Parse(typeof(LogNodeType),
				nodeNavigator.SelectSingleNode("lognodetype").Value);

			if (nodeType == LogNodeType.logLongOperation)
			{
				OperationStatus opStatus = (OperationStatus)Enum.Parse(typeof(OperationStatus),
					nodeNavigator.SelectSingleNode("status").Value);
				LogLongOperation logLongOp = parentNode.AddLongOperation(
					nodeNavigator.SelectSingleNode("name").Value, opStatus,
					nodeNavigator.SelectSingleNode("showbuffer").ValueAsInt,
					nodeNavigator.SelectSingleNode("progressmaximum").ValueAsInt);
				logLongOp.StartOperation(
					StrToDateTime(nodeNavigator.SelectSingleNode("starttime").Value));
				ReadNodeChildren(logLongOp, nodeNavigator);
				logLongOp.EndOperation(opStatus,
					StrToDateTime(nodeNavigator.SelectSingleNode("endtime").Value));
			}
			else
			{
				LogOperation logOp = parentNode.AddOperation(
					nodeNavigator.SelectSingleNode("name").Value,
					(OperationStatus)Enum.Parse(typeof(OperationStatus),
					nodeNavigator.SelectSingleNode("status").Value));
				ReadNodeChildren(logOp, nodeNavigator);
			}
		}

		public void FromXML(XPathNavigator logNavigator)
		{
			XPathNavigator nodeNavigator = logNavigator.SelectSingleNode("xmllog/node");
			OperationStatus opStatus = (OperationStatus)Enum.Parse(typeof(OperationStatus),
					nodeNavigator.SelectSingleNode("status").Value);
			loggerForm.BeginUpdate();
			try
			{
				ResetLog(nodeNavigator.SelectSingleNode("name").Value);
				globalOp.StartOperation(
						StrToDateTime(nodeNavigator.SelectSingleNode("starttime").Value));
				ReadNodeChildren(globalOp, nodeNavigator);
				globalOp.EndOperation(opStatus,
					StrToDateTime(nodeNavigator.SelectSingleNode("endtime").Value));
			}
			finally
			{
				loggerForm.EndUpdate();
			}
			
		}

		public void LoadFromXML(string fileName)
		{
			XPathDocument logFile = new XPathDocument(fileName);
			XPathNavigator logNavigator = logFile.CreateNavigator();
			FromXML(logNavigator);
			ToggleButtons(true);
		}
	}

	public enum OperationStatus
	{
		Waiting = 0,
		Running = 1,
		FinishedOK = 2,
		FinishedWithErrors = 3,
		FinishedWithWarnings = 4,
		Info = 5,
		Undefined = 6,
	}

	public enum NodeType
	{
		Operation = 0,
		Time = 100,
		Start = 101,
		End = 102,
		Duration = 103,
		Warning = 200,
		Error = 300,
	}
}