// -- FILE ------------------------------------------------------------------
// name       : RtfHtmlElementPath.cs
// project    : RTF Framelet
// created    : Jani Giannoudis - 2008.06.09
// language   : c#
// environment: .NET 2.0
// copyright  : (c) 2004-2009 by Itenso GmbH, Switzerland
// --------------------------------------------------------------------------
using System.Text;
using System.Collections;
using System.Web.UI;

namespace Itenso.Rtf.Converter.Html
{

	// ------------------------------------------------------------------------
	public class RtfHtmlElementPath
	{

		// ----------------------------------------------------------------------
		public RtfHtmlElementPath()
		{
		} // RtfHtmlElementPath

		// ----------------------------------------------------------------------
		public int Count
		{
			get { return this.elements.Count; }
		} // Count

		// ----------------------------------------------------------------------
		public HtmlTextWriterTag Current
		{
			get { return (HtmlTextWriterTag)this.elements.Peek(); }
		} // Current

		// ----------------------------------------------------------------------
		public bool IsCurrent( HtmlTextWriterTag tag )
		{
			return Current == tag;
		} // IsCurrent

		// ----------------------------------------------------------------------
		public bool Contains( HtmlTextWriterTag tag )
		{
			return this.elements.Contains( tag );
		} // Contains

		// ----------------------------------------------------------------------
		public void Push( HtmlTextWriterTag tag )
		{
			this.elements.Push( tag );
		} // Push

		// ----------------------------------------------------------------------
		public void Pop()
		{
			this.elements.Pop();
		} // Pop

		// ----------------------------------------------------------------------
		public override string ToString()
		{
			if ( elements.Count == 0 )
			{
				return base.ToString();
			}

			StringBuilder sb = new StringBuilder();
			bool first = true;
			foreach ( object element in this.elements )
			{
				if ( !first )
				{
					sb.Insert( 0, " > " );
				}
				sb.Insert( 0, element.ToString() );
				first = false;
			}

			return sb.ToString();
		} // ToString

		// ----------------------------------------------------------------------
		// members
		private readonly Stack elements = new Stack();

	} // class RtfHtmlElementPath

} // namespace Itenso.Rtf.Converter.Html
// -- EOF -------------------------------------------------------------------
