using System.Collections;
using System.Collections.Generic;
using Infragistics.Documents.Reports.Graphics;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Band;
using Infragistics.Documents.Reports.Report.Flow;
using Infragistics.Documents.Reports.Report.Grid;
using Infragistics.Documents.Reports.Report.Index;
using Infragistics.Documents.Reports.Report.QuickList;
using Infragistics.Documents.Reports.Report.QuickTable;
using Infragistics.Documents.Reports.Report.QuickText;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Segment;
using Infragistics.Documents.Reports.Report.TOC;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Reports.Report.Tree;
using IList = Infragistics.Documents.Reports.Report.List.IList;

namespace Krista.FM.Server.Dashboards.Common.Export
{
	/// <summary>
	/// Класс-декоратор, сливающий функционал ISection и IBand для правильного экспорта таблиц
	/// </summary>
	public class ReportHolderPdf : ISection, IBand
	{
		/// <summary>
		/// Текущий раздел в PDF-документе
		/// </summary>
		public ISection CurrentSection { set; get; }

		/// <summary>
		/// Текущий Band в PDF-документе
		/// </summary>
		public IBand CurrentBand { set; get; }

		#region поля для внутреннего использования

		private Stack<IBand> bandsStack;
		
		#endregion

		/// <summary>
		/// Инстанцирует класс, сливающий функционал ISection и IBand
		/// </summary>
		/// <param name="report"></param>
		public ReportHolderPdf(Report report)
		{
			CurrentSection = report.AddSection();
			CurrentBand = CurrentSection.AddBand();
			bandsStack = new Stack<IBand>();
		}

		/// <summary>
		/// Родительский Band, если он существует
		/// </summary>
		public IBand ParentBand
		{
			get
			{
				if (CurrentBand.Parent is IBand)
				{
					return CurrentBand.Parent as IBand;
				}
				return null;
			}
		}

		/// <summary>
		/// Сохраняет текущий Band в стек
		/// </summary>
		public void SetBand(IBand band)
		{
			CurrentBand = band;
		}
		
		/// <summary>
		/// Сохраняет текущий Band в стек
		/// </summary>
		public void PushBand()
		{
			bandsStack.Push(CurrentBand);
		}

		/// <summary>
		/// Извлекает Band из стека и делает его текущим
		/// </summary>
		public void PopBand()
		{
			if (bandsStack.Count > 0)
			{
				CurrentBand = bandsStack.Pop();
			}
		}

		#region переопределенные методы и свойства

		/// <summary>
		/// Добавляет Band
		/// </summary>
		public IBand AddBand()
		{
			CurrentBand = CurrentBand.AddBand();
			return CurrentBand;
		}

		/// <summary>
		/// Добавляет Table
		/// </summary>
		public ITable AddTable()
		{
			return CurrentBand.AddTable();
		}
		
		#endregion

		#region методы переадресованные на IBand

		public IFlow AddFlow()
		{
			return CurrentBand.AddFlow();
		}

		public IText AddText()
		{
			return CurrentBand.AddText();
		}

		public IImage AddImage(Image image)
		{
			return CurrentBand.AddImage(image);
		}

		public IMetafile AddMetafile(Metafile metafile)
		{
			return CurrentBand.AddMetafile(metafile);
		}

		public IRule AddRule()
		{
			return CurrentBand.AddRule();
		}

		public IGap AddGap()
		{
			return CurrentBand.AddGap();
		}

		public IGroup AddGroup()
		{
			return CurrentBand.AddGroup();
		}

		public IChain AddChain()
		{
			return CurrentBand.AddChain();
		}

		public IGrid AddGrid()
		{
			return CurrentBand.AddGrid();
		}

		public IList AddList()
		{
			return CurrentBand.AddList();
		}

		public ITree AddTree()
		{
			return CurrentBand.AddTree();
		}

		public ISite AddSite()
		{
			return CurrentBand.AddSite();
		}

		public ICanvas AddCanvas()
		{
			return CurrentBand.AddCanvas();
		}

		public IRotator AddRotator()
		{
			return CurrentBand.AddRotator();
		}

		public IContainer AddContainer(string name)
		{
			return CurrentBand.AddContainer(name);
		}

		public ICondition AddCondition(IContainer container, bool fit)
		{
			return CurrentBand.AddCondition(container, fit);
		}

		public IStretcher AddStretcher()
		{
			return CurrentBand.AddStretcher();
		}

		public void AddPageBreak()
		{
			CurrentBand.AddPageBreak();
		}

		public ITOC AddTOC()
		{
			return CurrentBand.AddTOC();
		}

		public IIndex AddIndex()
		{
			return CurrentBand.AddIndex();
		}

		public IQuickText AddQuickText(string text)
		{
			return CurrentBand.AddQuickText(text);
		}

		public IQuickImage AddQuickImage(Image image)
		{
			return CurrentBand.AddQuickImage(image);
		}

		public IQuickList AddQuickList()
		{
			return CurrentBand.AddQuickList();
		}

		public IQuickTable AddQuickTable()
		{
			return CurrentBand.AddQuickTable();
		}

		#endregion

		#region родные методы IBand

		public void AddDummy()
		{
			CurrentBand.AddDummy();
		}

		public Size Measure()
		{
			return CurrentBand.Measure();
		}

		#endregion

		#region свойства IBand

		public ContentAlignment Alignment
		{
			get { return CurrentBand.Alignment; }
			set { CurrentBand.Alignment = value; }
		}

		public IBandHeader Header
		{
			get { return CurrentBand.Header; }
		}

		public IBandFooter Footer
		{
			get { return CurrentBand.Footer; }
		}

		public IBandDivider Divider
		{
			get { return CurrentBand.Divider; }
		}

		public Width Width
		{
			get { return CurrentBand.Width; }
			set { CurrentBand.Width = value; }
		}

		public Height Height
		{
			get { return CurrentBand.Height; }
			set { CurrentBand.Height = value; }
		}

		public Borders Borders
		{
			get { return CurrentBand.Borders; }
			set { CurrentBand.Borders = value; }
		}

		public Margins Margins
		{
			get { return CurrentBand.Margins; }
			set { CurrentBand.Margins = value; }
		}

		public Paddings Paddings
		{
			get { return CurrentBand.Paddings; }
			set { CurrentBand.Paddings = value; }
		}

		public Background Background
		{
			get { return CurrentBand.Background; }
			set { CurrentBand.Background = value; }
		}

		public bool KeepSolid
		{
			get { return CurrentBand.KeepSolid; }
			set { CurrentBand.KeepSolid = value; }
		}

		public bool Stretch
		{
			get { return CurrentBand.Stretch; }
			set { CurrentBand.Stretch = value; }
		}

		object IBand.Parent
		{
			get { return CurrentBand.Parent; }
		}

		#endregion

		#region методы ISection

		public ISectionHeader AddHeader()
		{
			return CurrentSection.AddHeader();
		}

		public ISectionFooter AddFooter()
		{
			return CurrentSection.AddFooter();
		}

		public IStationery AddStationery()
		{
			return CurrentSection.AddStationery();
		}

		public IDecoration AddDecoration()
		{
			return CurrentSection.AddDecoration();
		}

		public ISectionPage AddPage()
		{
			return CurrentSection.AddPage();
		}

		public ISectionPage AddPage(PageSize size)
		{
			return CurrentSection.AddPage(size);
		}

		public ISectionPage AddPage(float width, float height)
		{
			return CurrentSection.AddPage(width, height);
		}

		public ISegment AddSegment()
		{
			return CurrentSection.AddSegment();
		}

		#endregion

		#region свойства ISection

		public bool Flip
		{
			get { return CurrentSection.Flip; }
			set { CurrentSection.Flip = value; }
		}

		public PageSize PageSize
		{
			get { return CurrentSection.PageSize; }
			set { CurrentSection.PageSize = value; }
		}

		public PageOrientation PageOrientation
		{
			get { return CurrentSection.PageOrientation; }
			set { CurrentSection.PageOrientation = value; }
		}

		public ContentAlignment PageAlignment
		{
			get { return CurrentSection.PageAlignment; }
			set { CurrentSection.PageAlignment = value; }
		}

		public Borders PageBorders
		{
			get { return CurrentSection.PageBorders; }
			set { CurrentSection.PageBorders = value; }
		}

		public Margins PageMargins
		{
			get { return CurrentSection.PageMargins; }
			set { CurrentSection.PageMargins = value; }
		}

		public Paddings PagePaddings
		{
			get { return CurrentSection.PagePaddings; }
			set { CurrentSection.PagePaddings = value; }
		}

		public Background PageBackground
		{
			get { return CurrentSection.PageBackground; }
			set { CurrentSection.PageBackground = value; }
		}

		public PageNumbering PageNumbering
		{
			get { return CurrentSection.PageNumbering; }
			set { CurrentSection.PageNumbering = value; }
		}

		public SectionLineNumbering LineNumbering
		{
			get { return CurrentSection.LineNumbering; }
			set { CurrentSection.LineNumbering = value; }
		}

		public Report Parent
		{
			get { return CurrentSection.Parent; }
		}

		public IEnumerable Content
		{
			get { return CurrentSection.Content; }
		}

		#endregion
	}
}
