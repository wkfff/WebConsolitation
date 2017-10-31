using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Collections.ObjectModel;

namespace Krista.FM.Client.TreeLogger
{
	public class LogOperation
	{
		protected Log logger;
		private string name;
		private List<LogOperation> children;
		//private Collection<LogOperation> children;
		private LogOperation parentOp;
		private int showBuffer;
		private OperationStatus status;
		private TreeNode treeNode;

		#region Конструкторы
		protected LogOperation()
		{ }

		internal protected LogOperation(LogOperation _parentOp, string _name,
			OperationStatus _status, Log _logger, TreeNode _treeNode, int _showBuffer)
		{
			if (_logger == null) { throw new Exception("Логгер не задан"); }
			if (_treeNode == null) { throw new Exception("Узел дерева не задан"); }

			logger = _logger;
			name = _name;
			children = new List<LogOperation>();
			showBuffer = _showBuffer;
			status = _status;
			treeNode = _treeNode;
			parentOp = _parentOp;
		}
		#endregion

		internal int ShowBuffer
		{
			get { return showBuffer; }
		}

		internal OperationStatus Status
		{
			get { return status; }
			set { status = value; }
		}

		internal TreeNode TreeNode
		{
			get { return treeNode; }
		}

		#region Публичные свойства
		public string Name
		{
			get { return name; }
		}

		public List<LogOperation> Children
		{
			get { return children; }
		}

		public LogOperation ParentOperation
		{
			get { return parentOp; }
		}
		#endregion

		#region Публичные методы
		public LogLongOperation AddLongOperation(string opName, OperationStatus opStatus)
		{
			if (logger != null)
				return logger.AddLongOperation(this, opName, opStatus, 0, -1);
			else
				return null;
		}

		public LogLongOperation AddLongOperation(string opName, OperationStatus opStatus,
			int showBuffer)
		{
			if (logger != null)
				return logger.AddLongOperation(this, opName, opStatus, showBuffer, -1);
			else
				return null;
		}

		public LogLongOperation AddLongOperation(string opName, OperationStatus opStatus,
			int showBuffer, int progressMaximum)
		{
			if (logger != null)
				return logger.AddLongOperation(this, opName, opStatus, showBuffer,
					progressMaximum);
			else
				return null;
		}

		public LogOperation AddOperation(string opName, OperationStatus opStatus)
		{
			if (logger != null)
				return logger.AddOperation(this, opName, opStatus, -1);
			else
				return null;
		}

		public LogOperation AppendLogMessage(string logMessage)
		{
			if (logger != null)
				return logger.AppendLogMessage(this, logMessage, OperationStatus.Info);
			else
				return null;
		}

		public virtual void EndOperation(OperationStatus opStatus)
		{
			if (logger != null)
				logger.EndOperation(this, opStatus);
		}
		#endregion
	}

	public class LogLongOperation : LogOperation
	{
		private int progressMaximum;
		private DataGridViewRow gridRow;

		internal TreeNode startTimeNode;
		internal TreeNode endTimeNode;
		internal TreeNode durationTimeNode;
		internal TreeNode warningsNode;
		internal TreeNode errorsNode;

		#region Конструкторы
		internal LogLongOperation(LogOperation _parentOp, string _name,
			OperationStatus _status, Log _logger, TreeNode _treeNode, int _showBuffer,
			int _progressMaximum, DataGridViewRow _gridRow)
			:
			base(_parentOp, _name, _status, _logger, _treeNode, _showBuffer)
		{
			progressMaximum = _progressMaximum;
			gridRow = _gridRow;
		}
		#endregion

		internal DataGridViewRow GridRow
		{
			get { return gridRow; }
		}

		#region Публичные свойства
		public int ProgressMaximum
		{
			get { return progressMaximum; }
			set { progressMaximum = value; }
		}
		#endregion

		#region Публичные методы
		public LogOperation AddOperation(string opName, OperationStatus opStatus, int endValue)
		{
			if (logger != null)
				return logger.AddOperation(this, opName, opStatus, endValue);
			else
				return null;
		}

		public void EndOperation()
		{
			if (logger != null)
				logger.EndOperation(this);
		}

		public void EndOperation(DateTime endTime)
		{
			if (logger != null)
				logger.EndOperation(this, endTime);
		}

		public override void EndOperation(OperationStatus opStatus)
		{
			if (logger != null)
				logger.EndOperation(this, opStatus);
		}

		public void EndOperation(OperationStatus opStatus, DateTime endTime)
		{
			if (logger != null)
				logger.EndOperation(this, opStatus, endTime);
		}

		public void StartOperation()
		{
			if (logger != null)
				logger.StartOperation(this);
		}

		public void StartOperation(DateTime startTime)
		{
			if (logger != null)
				logger.StartOperation(this, startTime);
		}

		public void ProgressUpdate()
		{
			if (logger != null)
				logger.ProgressUpdate();
		}
		#endregion
	}

	internal enum LogNodeType
	{
		logOperation = 1,
		logLongOperation = 2,
	}
}