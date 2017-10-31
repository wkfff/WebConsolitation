using System;
using System.Drawing;
using System.Resources;
using System.Reflection;
using System.Collections;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Windows.Forms;

namespace Krista.FM.Client.SchemeEditor
{
	/// <summary>
	/// Summary description for HighlightEdit.
	/// </summary>
	public partial class HighlightEdit : RichTextBox
	{
		private ResourceManager resManager;
		private Regex regexSyntax;
		private Regex regexMultilineComment;
		private Regex regexFormatOpen;
		private Regex regexFormatClose;
		private ArrayList listKeyword;
		const short  WM_PAINT = 0x00f;
		private static bool _Paint = true;
		private int IndentTab;
		private bool syntaxHighlightingAll;

		public HighlightEdit()
		{
			Initialize();
		}
		private void Initialize()
		{
			this.AcceptsTab = true;
			this.TextChanged += new System.EventHandler(this.HighlightEdit_TextChanged);
			this.KeyPress+=new KeyPressEventHandler(HighlightEdit_KeyPress);
			this.MouseDown+=new MouseEventHandler(HighlightEdit_MouseDown);
            resManager = new ResourceManager("Krista.FM.Client.SchemeEditor.KeywordCSharp", Assembly.GetExecutingAssembly());
			regexSyntax=new Regex(@"(\w+|\*/)|[^A-Za-z0-9_ \f\t\v]", RegexOptions.IgnoreCase|RegexOptions.Compiled);
			regexMultilineComment=new Regex(@"/\*",RegexOptions.RightToLeft|RegexOptions.IgnoreCase|RegexOptions.Compiled);
			regexFormatOpen = new Regex(@"\{", RegexOptions.IgnoreCase|RegexOptions.Compiled);
			regexFormatClose = new Regex(@"\}", RegexOptions.IgnoreCase|RegexOptions.Compiled);
			IndentTab=0;
			listKeyword=new ArrayList();
			IDictionaryEnumerator resEnumerator=resManager.GetResourceSet(CultureInfo.CurrentUICulture,true,true).GetEnumerator();
			while (resEnumerator.MoveNext())
			{
				listKeyword.Add(resEnumerator.Key);
			}
			
		}
		
		#region Paint

		protected override void WndProc(ref Message m) 
		{
			// sometimes we want to eat the paint message so we don't have to see all the 
			//  flicker from when we select the text to change the color.
			if (m.Msg == WM_PAINT) 
			{

				if (_Paint)

					base.WndProc(ref m);

				else

					m.Result = IntPtr.Zero;

			}

			else

				base.WndProc (ref m);
		}
		#endregion

		#region Keyword and Position

		struct WordAndPosition
		{
			public string Word;
			public int Position;
			public int Length;	
		}


		private bool IsKeyword(string word)
		{
			return listKeyword.IndexOf(word)>=0;
		}

		private int CountFormatSymbol(Regex r,string line)
		{
			return r.Matches(line).Count;
		}
		#endregion

		#region Format

		public string FormatText(string formatText)
		{
			int IndentFormatText=0;
		    string[] formatTextSplit=formatText.Split(new Char[] {'\n'});
			formatText="";

			foreach(string line in formatTextSplit)
			{
			    int countFormatOpen;
			    countFormatOpen=CountFormatSymbol(regexFormatOpen,line);
			    int countFormatClose;
			    countFormatClose=CountFormatSymbol(regexFormatClose,line);
				if (countFormatClose>0)
				{
					IndentFormatText=IndentFormatText-1;
				}
				for (int i=0; i<IndentFormatText; i++)
				{
					formatText=formatText+'\t';
				}
	
				if (countFormatOpen>0)
				{
					IndentFormatText=IndentFormatText+countFormatOpen;
				}

				formatText=formatText+line.Trim()+"\n";
			}
			return formatText;
		}

		private void FormatLine(int CountIndentTab)
		{
			for (int i=0; i<CountIndentTab; i++)
			{
				this.SelectedText=this.SelectedText+'\t';
			}
		}
		#endregion

		#region SyntaxHighlighting

		private void SyntaxHighlightingLine(string currentLine,int startLine,int currentPosition)
		{
			_Paint=false;

			if (currentLine.Trim().StartsWith("//"))
			{
				this.Select(startLine,currentLine.Length);
				this.SelectionColor=Color.Green;
				goto defaultValues;
			}

			ArrayList buffer=new ArrayList();
			Match m;

			for (m = regexSyntax.Match(currentLine); m.Success ; m = m.NextMatch()) 
			{

				WordAndPosition wordAndPosition=new WordAndPosition();
				wordAndPosition.Word = m.Value;
				wordAndPosition.Position = m.Index;
				wordAndPosition.Length = m.Length;
				buffer.Add(wordAndPosition);
			}

			int i=0;
			string currentWord="";
			foreach (WordAndPosition wp in buffer)
			{
				currentWord=wp.Word;

				if (currentWord=="*/")
				{
					Match matchComment=regexMultilineComment.Match(this.Text.Substring(0,startLine+wp.Position));
					if (matchComment.Index>=0)
					{
						this.Select(matchComment.Index,startLine+wp.Position+wp.Length);
						this.SelectionColor=Color.Green;
						continue;
					}
				}


				this.Select(startLine+wp.Position,wp.Length);

				if (IsKeyword(currentWord))
				{
					this.SelectionColor=Color.Blue;
				}
				else
				{
					this.SelectionColor=Color.Black;
				}
				i++;
			}
			defaultValues:
				this.Select(currentPosition,0);
			this.SelectionColor=Color.Black;
			_Paint=true;
		}

		public void SyntaxHighlightingAll()
		{
			int startLine=0;
			int currentPosition=0;
			syntaxHighlightingAll=true;
			foreach (string line in this.Lines)
			{
				SyntaxHighlightingLine(line,startLine,currentPosition);
				currentPosition=currentPosition+line.Length+1;
				startLine=startLine+line.Length+1;	
			}
			syntaxHighlightingAll=false;
		}

		private void SyntaxHighlighting()
		{
			int currentPosition=this.SelectionStart;
			if (currentPosition>0)
			{
				// Calculate the starting position of the current line.
				int startLine = 0;
				for (startLine = currentPosition - 1; startLine > 0; startLine--) 
				{
					if (this.Text[startLine] == '\n')  
					{ startLine++; break; }
				}

				// Calculate the end position of the current line.
				int endLine = 0;
				for (endLine = currentPosition; endLine < this.Text.Length; endLine++) 
				{
					if (this.Text[endLine] == '\n') break;
				}

				string currentLine=this.Text.Substring(startLine, endLine - startLine);
				SyntaxHighlightingLine(currentLine,startLine,currentPosition);
			}
			else SyntaxHighlightingAll();
		}
		#endregion

		
		private void HighlightEdit_TextChanged(object sender, EventArgs e)
		{
			if (syntaxHighlightingAll)
			{
				return;
			}
			SyntaxHighlighting();
		}
		#region Mouse and Keyboard Handling

		private void HighlightEdit_KeyPress(object sender, KeyPressEventArgs e)
		{

			if (e.KeyChar=='{')
			{
				IndentTab=CountFormatSymbol(regexFormatOpen,this.Text.Substring(0,this.SelectionStart))+1;
			}

			if (e.KeyChar=='}')
			{
				int currentPosition=this.SelectionStart;
				// Calculate the starting position of the current line.
				int startLine = 0;
				for (startLine = currentPosition - 1; startLine > 0; startLine--) 
				{
					if (this.Text[startLine] == '\n')  
					{ startLine++; break; }
				}
				this.Select(startLine,currentPosition -startLine);
				if (this.SelectedText.Trim()=="")
				{
					this.SelectedText="";
				}
				this.Select(currentPosition,0);

				FormatLine(IndentTab-1-1*CountFormatSymbol(regexFormatClose,this.Text.Substring(0,this.SelectionStart)));
			}

			if (e.KeyChar=='\r')
			{
				FormatLine(IndentTab);	
			}

		}

		private void HighlightEdit_MouseDown(object sender, MouseEventArgs e)
		{
			IndentTab=CountFormatSymbol(regexFormatOpen,this.Text.Substring(0,this.SelectionStart));
		}
		#endregion
	}
}
