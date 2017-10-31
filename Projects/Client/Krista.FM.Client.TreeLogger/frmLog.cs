using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using Krista.FM.Client.TreeLogger.Properties;

namespace Krista.FM.Client.TreeLogger
{
	internal partial class frmLog : Form
	{
		internal delegate void AddNodeDelegate(TreeNodeCollection nodes,
			TreeNode node, int index);
		internal delegate DataGridViewRow CreateOperationRowDelegate(
			string opName, OperationStatus opStatus);
		internal delegate void EndLongOperationDelegate(
			LogLongOperation logLongOp, OperationStatus opStatus, DateTime endTime);
		internal delegate void EndOperationDelegate(
			LogOperation logLongOp, OperationStatus opStatus);
		internal delegate void FlushBufferDelegate(LogOperation logOp, bool callEnsureVisible);
		internal delegate void InitLogDelegate(string logCaption);
		internal delegate void ProgressUpdateDelegate();
		internal delegate void ResetLogDelegate();
		internal delegate void SetFontStyleDelegate(ref TreeNode treeNode, FontStyle fontStyle);
		internal delegate void ShowLogDelegate(
			IWin32Window owner, string logCaption, LogOperation globalOp);
		internal delegate void StartOperationDelegate(LogLongOperation logLongOp,
			DateTime startTime);
		internal delegate void ToggleButtonsDelegate(bool buttonsEnabled);

		internal Log log;

		internal bool needUpdate = true;

		internal frmLog()
		{
			InitializeComponent();
		}

		public void BeginUpdate()
		{
			treeViewLog.BeginUpdate();
			needUpdate = false;			
		}

		public void EndUpdate()
		{
			CollapseNodes();
			treeViewLog.EndUpdate();
			needUpdate = true;			
		}

		private void CollapseNodes()
		{
			for (int i = 0; i < treeViewLog.Nodes.Count; i++)
			{
				treeViewLog.Nodes[i].Collapse(false);
			}
		}

		private void SetDurationTime(LogLongOperation logLongOp, DateTime endTime)
		{
			logLongOp.endTimeNode.Tag = endTime;
			logLongOp.endTimeNode.Text = endTime.ToString(
				Resources.endNodeCaption + "HH:mm:ss");
			if (logLongOp.startTimeNode.Tag == null)
				logLongOp.startTimeNode.Tag = endTime;
			TimeSpan duration = ((DateTime)logLongOp.endTimeNode.Tag).Subtract(
				(DateTime)logLongOp.startTimeNode.Tag);
			logLongOp.durationTimeNode.Tag = duration;

			string durationString = String.Format("{0:D2}:{1:D2}:{2:D2}",
				duration.Hours.ToString(),
				duration.Minutes.ToString(),
				duration.Seconds.ToString());
			logLongOp.durationTimeNode.Text = Resources.durationNodeCaption +
				durationString;
		}

		internal void SetFontStyle(ref TreeNode treeNode, FontStyle fontStyle)
		{			
			if (treeNode != null && treeNode.TreeView != null &&
				treeNode.TreeView.Font != null)
			{
				treeNode.NodeFont = new Font(treeNode.TreeView.Font, fontStyle);
			}			
		}

		internal void InvokeSetFontStyle(ref TreeNode treeNode, FontStyle fontStyle)
		{
			if (InvokeRequired)
				Invoke(new SetFontStyleDelegate(SetFontStyle), treeNode, fontStyle);
			else
				SetFontStyle(ref treeNode, fontStyle);
		}

		private void AddNode(TreeNodeCollection nodes, TreeNode node, int index)
		{
			if (nodes != null && node != null) { nodes.Insert(index, node); }
		}

		public void InvokeAddNode(TreeNodeCollection nodes, TreeNode node, int index)
		{
			if (InvokeRequired)
				Invoke(new AddNodeDelegate(AddNode), nodes, node, index);
			else
				AddNode(nodes, node, index);
		}

		internal TreeNode AppendErrorNode(LogOperation opInfo)
		{
			int nodeIndex = -1;
			TreeNode timeNode = GetNodeByType(opInfo.TreeNode, NodeType.Warning);
			if (timeNode == null)
				timeNode = GetNodeByType(opInfo.TreeNode, NodeType.Time);
			if (timeNode != null) { nodeIndex = timeNode.Index + 1; }
			TreeNode errorNode = GetNodeByType(opInfo.TreeNode, NodeType.Error,
				nodeIndex, Resources.errorNodeCaption, "Error_16", FontStyle.Bold, true);
			return errorNode;
		}

		internal TreeNode AppendWarningNode(LogOperation opInfo)
		{
			int nodeIndex = -1;
			TreeNode timeNode = GetNodeByType(opInfo.TreeNode, NodeType.Time);
			if (timeNode != null) { nodeIndex = timeNode.Index + 1; }
			TreeNode warningNode = GetNodeByType(opInfo.TreeNode, NodeType.Warning,
				nodeIndex, Resources.warningNodeCaption, "Warning_16", FontStyle.Bold, true);
			return warningNode;
		}

		internal DataGridViewRow CreateOperationRow(string opName, OperationStatus opStatus)
		{
			string[] opRow = new string[] { opName, LoggerUtils.OpStatusToString(opStatus) };
			gridOperations.Rows.Add(opRow);
			DataGridViewRow row = gridOperations.Rows[gridOperations.Rows.Count - 1];
			row.DefaultCellStyle.BackColor = Color.LightGray;
			return row;
		}

		internal DataGridViewRow InvokeCreateOperationRow(string opName, OperationStatus opStatus)
		{
			if (InvokeRequired || gridOperations.InvokeRequired)
			{
				IAsyncResult AsyncResult = BeginInvoke(
					new CreateOperationRowDelegate(CreateOperationRow), opName, opStatus);
				return (DataGridViewRow)EndInvoke(AsyncResult);
			}
			else
				return CreateOperationRow(opName, opStatus);
		}

		internal void InitGrid(LogLongOperation logLongOp)
		{
			if (logLongOp != null && logLongOp.GridRow != null)
			{
				logLongOp.GridRow.Cells["Status"].Value = LoggerUtils.
					OpStatusToString(OperationStatus.Running);
				logLongOp.GridRow.Selected = true;
			}
		}

		internal void EndOperation(LogOperation logOp, OperationStatus opStatus)
		{
			//FlushBuffer(logOp);
			if (needUpdate && logOp.TreeNode.Parent != null) { treeViewLog.BeginUpdate(); }
			try
			{
				FlushBuffer(logOp, false);
				logOp.TreeNode.ImageKey = LoggerUtils.OpStatusToImageKey(opStatus);
				logOp.TreeNode.SelectedImageKey = logOp.TreeNode.ImageKey;
			}
			finally
			{
				if (needUpdate)
				{
					if (logOp.TreeNode.Parent != null) { treeViewLog.EndUpdate(); }
					logOp.TreeNode.Collapse();
					logOp.TreeNode.EnsureVisible();
				}				
			}
		}

		internal void InvokeEndOperation(LogOperation logOp, OperationStatus opStatus)
		{
			if (InvokeRequired)
				Invoke(new EndOperationDelegate(EndOperation), logOp, opStatus);
			else
				EndOperation(logOp, opStatus);
		}

		private OperationStatus GetOperationStatus(LogLongOperation logLongOp,
			OperationStatus oldOpStatus)
		{
			OperationStatus newOpStatus = oldOpStatus;
			if (newOpStatus == OperationStatus.Undefined)
			{
				if (logLongOp.errorsNode != null && logLongOp.errorsNode.Nodes.Count > 0)
					newOpStatus = OperationStatus.FinishedWithErrors;
				else
					if (logLongOp.warningsNode != null && logLongOp.warningsNode.Nodes.Count > 0)
						newOpStatus = OperationStatus.FinishedWithWarnings;
					else
						newOpStatus = OperationStatus.FinishedOK;
			}
			return newOpStatus;
		}

		private void AddWarning(LogLongOperation logLongOp, TreeNode node)
		{
			if (logLongOp.warningsNode == null)
			{
				log.CreateWarningNode(logLongOp);
			}
			node.Remove();
			logLongOp.warningsNode.Nodes.Add(node);
		}

		private void AddError(LogLongOperation logLongOp, TreeNode node)
		{
			if (logLongOp.errorsNode == null)
			{
				log.CreateErrorNode(logLongOp);
			}
			node.Remove();
			logLongOp.errorsNode.Nodes.Add(node);
		}

		internal void EndLongOperation(LogLongOperation logLongOp,
			OperationStatus opStatus, DateTime endTime)
		{
			OperationStatus newOpStatus = OperationStatus.Undefined;			
			if (needUpdate && logLongOp.TreeNode.Parent != null) { treeViewLog.BeginUpdate(); }
			try
			{
				FlushBuffer(logLongOp, false);
				SetDurationTime(logLongOp, endTime);
				for (int i = 0; i < logLongOp.Children.Count; i++)
				{
					if (logLongOp.Children[i].Status == OperationStatus.FinishedWithWarnings)
					{
						//logLongOp.Children[i].TreeNode.Remove();
						//logLongOp.warningsNode.Nodes.Add(logLongOp.Children[i].TreeNode);
						AddWarning(logLongOp, logLongOp.Children[i].TreeNode);
					}
					else
						if (logLongOp.Children[i].Status == OperationStatus.FinishedWithErrors)
						{
							//logLongOp.Children[i].TreeNode.Remove();
							//logLongOp.errorsNode.Nodes.Add(logLongOp.Children[i].TreeNode);
							AddError(logLongOp, logLongOp.Children[i].TreeNode);
						}
				}
				if (logLongOp.warningsNode != null)
				{
					logLongOp.warningsNode.Text = Resources.warningNodeCaption +
						logLongOp.warningsNode.Nodes.Count;
				}
				if (logLongOp.errorsNode != null)
				{
					logLongOp.errorsNode.Text = Resources.errorNodeCaption +
						logLongOp.errorsNode.Nodes.Count;
				}
				
				logLongOp.TreeNode.Text = logLongOp.Name;				

				newOpStatus = GetOperationStatus(logLongOp, opStatus);
				logLongOp.TreeNode.ImageKey = LoggerUtils.OpStatusToImageKey(newOpStatus);
				logLongOp.TreeNode.SelectedImageKey = logLongOp.TreeNode.ImageKey;
				//logLongOp.TreeNode.Collapse();
			}
			finally
			{
				if (needUpdate)
				{
					if (logLongOp.TreeNode.Parent != null) { treeViewLog.EndUpdate(); }
					logLongOp.TreeNode.Collapse();
					logLongOp.TreeNode.EnsureVisible();
				}				
			}
			logLongOp.Status = newOpStatus;
			if (logLongOp.GridRow != null)
			{
				logLongOp.GridRow.Cells["Status"].Value = LoggerUtils.OpStatusToString(newOpStatus);
				logLongOp.GridRow.DefaultCellStyle.BackColor = Color.White;
				logLongOp.GridRow.Selected = false;
			}
		}

		internal void InvokeEndLongOperation(LogLongOperation logLongOp,
			OperationStatus opStatus, DateTime endTime)
		{
			if (InvokeRequired)
				Invoke(new EndLongOperationDelegate(EndLongOperation), logLongOp,
					opStatus, endTime);
			else
				EndLongOperation(logLongOp, opStatus, endTime);
		}
		
		internal void FlushBuffer(LogOperation logOp, bool callEnsureVisible)
		{
			int buffer = 1;
			if (logOp.ShowBuffer > 1) { buffer = logOp.ShowBuffer; }
			List<TreeNode> opNodes = new List<TreeNode>();
			int startIndex = logOp.Children.Count - buffer;
			if (startIndex < 0) { startIndex = 0; }
			for (int i = startIndex; i < logOp.Children.Count; i++)
				if (logOp.Children[i].TreeNode.Parent == null)
					opNodes.Add(logOp.Children[i].TreeNode);

			logOp.TreeNode.Nodes.AddRange(opNodes.ToArray());
			if (callEnsureVisible && logOp.TreeNode.LastNode != null)
			{
				logOp.TreeNode.LastNode.EnsureVisible();
			}
		}

		internal void InvokeFlushBuffer(LogOperation logOp)
		{
			if (InvokeRequired)
				Invoke(new FlushBufferDelegate(FlushBuffer), logOp, true);
			else
				FlushBuffer(logOp, true);
		}

		internal void InitProgressBar(LogLongOperation logLongOp)
		{
			if (logLongOp != null && logLongOp.ProgressMaximum >= 0)
			{
				lbCaption.Text = logLongOp.Name;
				progressBarMain.Maximum = logLongOp.ProgressMaximum;
				progressBarMain.Value = 0;
				progressBarMain.Step = 1;
			}
		}

		private void ProgressUpdate()
		{
			progressBarMain.PerformStep();
		}

		internal void InvokeProgressUpdate()
		{
			if (InvokeRequired)
				Invoke(new ProgressUpdateDelegate(ProgressUpdate));
			else
				ProgressUpdate();
		}

		internal void ResetLog()
		{
			gridOperations.Rows.Clear();
			progressBarMain.Minimum = 0;
			progressBarMain.Maximum = 100;
			progressBarMain.Value = 0;
			treeViewLog.Nodes.Clear();
		}

		internal void InvokeResetLog()
		{
			if (InvokeRequired)
				Invoke(new ResetLogDelegate(ResetLog));
			else
				ResetLog();
		}

		internal void InitLog(string logCaption)
		{
			Text = logCaption;
			ToggleButtons(false);
		}

		internal void InvokeInitLog(string logCaption)
		{
			if (InvokeRequired)
				Invoke(new InitLogDelegate(InitLog), logCaption);
			else
				InitLog(logCaption);
		}

		internal void ShowLog(IWin32Window owner, string logCaption, LogOperation globalOp)
		{
			if (!string.IsNullOrEmpty(logCaption))
				globalOp.TreeNode.Text = logCaption;
			Show(owner);
		}

		internal void InvokeShowLog(IWin32Window owner, string logCaption, LogOperation globalOp)
		{
			if (InvokeRequired)
				Invoke(new ShowLogDelegate(ShowLog), owner, logCaption, globalOp);
			else
				ShowLog(owner, logCaption, globalOp);
		}

		private void StartOperation(LogLongOperation logLongOp, DateTime startTime)
		{
			InitGrid(logLongOp);
			InitProgressBar(logLongOp);
			if (needUpdate && logLongOp.TreeNode.Parent != null) { treeViewLog.BeginUpdate(); }
			try
			{
				logLongOp.startTimeNode.Tag = startTime;
				logLongOp.startTimeNode.Text = startTime.ToString(
					Resources.startNodeCaption + "HH:mm:ss");
				logLongOp.TreeNode.ImageKey = LoggerUtils.OpStatusToImageKey(
					OperationStatus.Running);
				logLongOp.TreeNode.Text += "  (" + logLongOp.startTimeNode.Text + ")";
			}
			finally
			{
				if (needUpdate)
				{
					if (logLongOp.TreeNode.Parent != null) { treeViewLog.EndUpdate(); }
					logLongOp.TreeNode.EnsureVisible();
				}				
			}
		}

		internal void InvokeStartOperation(LogLongOperation logLongOp, DateTime startTime)
		{
			if (InvokeRequired)
				Invoke(new StartOperationDelegate(StartOperation), logLongOp, startTime);
			else
				StartOperation(logLongOp, startTime);
		}

		private void ToggleButtons(bool buttonsEnabled)
		{
			btnOpenLog.Enabled = buttonsEnabled;
			btnSaveLog.Enabled = buttonsEnabled;
			btnCloseLogger.Enabled = buttonsEnabled;
		}

		internal void InvokeToggleButtons(bool buttonsEnabled)
		{
			if (InvokeRequired)
				Invoke(new ToggleButtonsDelegate(ToggleButtons), buttonsEnabled);
			else
				ToggleButtons(buttonsEnabled);
		}

		private void frmLog_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing)
				e.Cancel = true;
		}

		private void btnCloseLogger_Click(object sender, EventArgs e)
		{
			if (Owner == null)
			{
				this.Close();
				Application.Exit();
			}
			else
				this.Hide();
		}

		private void btnOpenLog_Click(object sender, EventArgs e)
		{
			if (openFileDialog.ShowDialog(this) == DialogResult.OK)
				log.LoadFromXML(openFileDialog.FileName);
		}

		private void btnSaveLog_Click(object sender, EventArgs e)
		{
			saveFileDialog.FileName = Resources.defaultLogFileName +
				DateTime.Now.ToString("_yyyy_MM_dd_HHmm");
			if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
				log.SaveToXML(saveFileDialog.FileName);
		}

		private TreeNode GetNodeByType(TreeNode parentNode, NodeType nodeType)
		{
			return GetNodeByType(parentNode, nodeType, -1, "", "",
				parentNode.TreeView.Font.Style, false);
		}

		private TreeNode GetNodeByType(TreeNode parentNode, NodeType nodeType,
			string nodeText, string imageKey, bool createIfNotFound)
		{
			return GetNodeByType(parentNode, nodeType, -1, nodeText, imageKey,
				parentNode.TreeView.Font.Style, createIfNotFound);
		}

		private TreeNode GetNodeByType(TreeNode parentNode, NodeType nodeType,
			string nodeText, string imageKey, FontStyle fontStyle, bool createIfNotFound)
		{
			return GetNodeByType(parentNode, nodeType, -1, nodeText, imageKey,
				fontStyle, createIfNotFound);
		}

		private TreeNode GetNodeByType(TreeNode parentNode, NodeType nodeType, int nodeIndex,
			string nodeText, string imageKey, FontStyle fontStyle, bool createIfNotFound)
		{
			TreeNode childNode = null;
			int childIndex = parentNode.Nodes.IndexOfKey(nodeType.ToString());
			if (childIndex >= 0)
				childNode = parentNode.Nodes[childIndex];
			else
				if (createIfNotFound)
				{
					if (nodeIndex < 0) { nodeIndex = parentNode.Nodes.Count; }
					childNode = parentNode.Nodes.Insert(nodeIndex, nodeType.ToString(),
						nodeText, imageKey, imageKey);
					if (fontStyle != childNode.TreeView.Font.Style)
						SetFontStyle(ref childNode, fontStyle);
				}
			return childNode;
		}

		private void gridOperations_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex >= 0)
			{
				LogOperation opInfo = (LogOperation)gridOperations.Rows[e.RowIndex].Tag;
				treeViewLog.Select();
				treeViewLog.SelectedNode = opInfo.TreeNode;
				treeViewLog.SelectedNode.Expand();
			}
		}
	}
}